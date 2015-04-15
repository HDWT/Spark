using System;
using System.Collections;
using System.Collections.Generic;

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

	private static FormatFlags s_formatFlags = FormatFlags.None;// FormatFlags.LZ4Compression;

	public static bool FullAot = true;
	public static bool SimulateMissingFields = false;

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
		if (data.Length == 0)
			return default(T);

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

			if (dataType.TryCreateInstance(data[index], out newInstance))
				dataType.ReadValues(newInstance, data, ref index, data.Length);

			if (typeFlags == TypeFlags.Reference)
				return (newInstance != null) ? (T)newInstance : default(T);

			return (T)newInstance;
		}
		else
		{
			var ReadData = Reader.Get(type);
			return (T)ReadData(type, data, ref index);
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

	/// <summary> Custom constructor. Default constructor required </summary>
	public interface ICreator
	{
		object CreateInstance();
	}
}
