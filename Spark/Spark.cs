﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private const byte Version = 1;
	private const int HeaderSize = 2;

	[System.Flags]
	private enum FormatFlags : byte
	{
		None = 0,
		LZ4Compression = 1 << 0,
	}

	private static FormatFlags s_formatFlags = FormatFlags.None;

	public static bool FullAot = false;

	public static byte[] Serialize(object instance)
	{
		if (instance == null)
			return new byte[0];

		Type type = instance.GetType();
		int index = 0;

		TypeFlags typeFlags = GetTypeFlags(type);

		if ((typeFlags == TypeFlags.Class) || (typeFlags == TypeFlags.Abstract) || (typeFlags == TypeFlags.Interface))
		{
			DataType dataType = DataType.Get(type);

			QueueWithIndexer<int> sizes = new QueueWithIndexer<int>();
			QueueWithIndexer<object> values = FullAot ? new QueueWithIndexer<object>() : null;

			int dataSize = HeaderSize + dataType.GetDataSize(instance, sizes, values);
			byte[] data = new byte[dataSize];

			WriteHeader(data, ref index);
			dataType.WriteValues(instance, data, ref index, sizes, values);

			if (LZ4Compression)
				data = LZ4Encode(data);

			return data;
		}
		else
		{
			QueueWithIndexer<int> sizes = new QueueWithIndexer<int>();
			QueueWithIndexer<object> values = FullAot ? new QueueWithIndexer<object>() : null;

			bool isValueType = ((typeFlags & TypeFlags.Value) == TypeFlags.Value);

			int dataSize = HeaderSize;
			dataSize += (isValueType)
				? SizeCalculator.GetForValueType(type)(instance)
				: SizeCalculator.GetForReferenceType(type)(instance, sizes, values);

			byte[] data = new byte[dataSize];

			WriteHeader(data, ref index);

			if (isValueType)
				Writer.GetDelegateForValueType(type)(instance, data, ref index);
			else
				Writer.GetDelegateForReferenceType(type)(instance, data, ref index, sizes, values);

			if (LZ4Compression)
				data = LZ4Encode(data);

			return data;
		}
	}

	public static T Deserialize<T>(byte[] data)
	{
		T instance = default(T);

		Deserialize<T>(data, ref instance);
		return instance;
	}

	public static void Deserialize<T>(byte[] data, ref T instance)
	{
		if (data.Length == 0)
		{
			instance = default(T);
			return;
		}

		Type type = typeof(T);
		int index = 0;

		byte version = data[index++];
		FormatFlags formatFlags = (FormatFlags)data[index++];

		if (version != 1)
			throw new System.ArgumentException("Invalid version " + version);

		if ((formatFlags & FormatFlags.LZ4Compression) == FormatFlags.LZ4Compression)
			data = LZ4Decode(data);

		TypeFlags typeFlags = GetTypeFlags(type);

		if ((typeFlags == TypeFlags.Class) || (typeFlags == TypeFlags.Abstract) || (typeFlags == TypeFlags.Interface))
		{
			DataType dataType = DataType.Get(type);
			object newInstance = null;

			if ((instance == null) && dataType.TryCreateInstance(data[index], out newInstance))
				instance = (newInstance != null) ? (T)newInstance : default(T);

			if (instance != null)
				dataType.ReadValues(instance, data, ref index, data.Length);
		}
		else
		{
			var ReadData = Reader.Get(type);
			instance = (T)ReadData(type, data, ref index);
		}
	}

	public static void ValidateTypes(Assembly assembly)
	{
		if (assembly == null)
			return;

		foreach (var type in assembly.GetTypes())
		{
			System.Threading.ThreadPool.QueueUserWorkItem((state) =>
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				bool validate = false;

				for (int i = 0; (i < fields.Length) && !validate; ++i)
				{
					var sparkMemberAtributes = fields[i].GetCustomAttributes(typeof(Spark.MemberAttribute), true);

					if ((sparkMemberAtributes != null) && (sparkMemberAtributes.Length != 0))
						validate = true;
				}

				for (int i = 0; (i < properties.Length) && !validate; ++i)
				{
					var sparkMemberAtributes = properties[i].GetCustomAttributes(typeof(Spark.MemberAttribute), true);

					if ((sparkMemberAtributes != null) && (sparkMemberAtributes.Length != 0))
						validate = true;
				}

				if (validate)
				{
					Spark.DataType dataType = Spark.DataType.Get(type);
					object instance = null;

					if (type.IsAbstract || type.IsInterface || type.ContainsGenericParameters)
					{
					}
					else if (IsGenericList(type) || IsGenericDictionary(type))
					{
						dataType.CreateInstance(1); // collection count
					}
					else
					{
						dataType.TryCreateInstance(0, out instance);
					}

					if (instance != null)
						Serialize(instance);
				}
			});
		}
	}

	private static void WriteHeader(byte[] data, ref int index)
	{
		data[index++] = Version;
		data[index++] = (byte)s_formatFlags;
	}

	private static bool IsGenericList(Type type)
	{
		return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
	}

	private static bool IsGenericDictionary(Type type)
	{
		return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
	}

	public interface ICallbacks
	{
		void BeforeDeserialize();
		void AfterDeserialize();

		void BeforeSetValue(ushort memberId, bool validMemberId);
		void AfterSetValue(ushort memberId, bool validMemberId);
	}
}
