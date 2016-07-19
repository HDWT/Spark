using System;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private static readonly bool	FullAot				= true;
	private static readonly bool	ExperimentalMagic	= true; // Be prepared!
	private static EFormatFlags		FormatFlags			= EFormatFlags.None;

	private static readonly bool	DoubleCheckingMode	= false; // For testing only

	[System.Flags]
	private enum EFormatFlags : byte
	{
		None = 0,
		LZ4Compression = 1 << 0,
	}

	private static readonly bool Is64Bit = false;
	private static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

	private static readonly byte Version = 1;
	private static readonly int HeaderSize = 2;

	static Spark()
	{
		Is64Bit = (IntPtr.Size == 8);
	}

	private class Context
	{
		public FieldAccessor.ObjectWrapper objectWrapper = new FieldAccessor.ObjectWrapper();
	}

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

			Context context = new Context();
			QueueWithIndexer<int> sizes = new QueueWithIndexer<int>();
			QueueWithIndexer<object> values = FullAot ? new QueueWithIndexer<object>() : null;

			int dataSize = HeaderSize + dataType.GetDataSize(instance, sizes, values, context);
			byte[] data = new byte[dataSize];

			WriteHeader(data, ref index);
			dataType.WriteValues(instance, data, ref index, sizes, values, context);

			if (LZ4Compression)
				data = LZ4Encode(data);

			return data;
		}
		else
		{
			Context context = new Context();
			QueueWithIndexer<int> sizes = new QueueWithIndexer<int>();
			QueueWithIndexer<object> values = FullAot ? new QueueWithIndexer<object>() : null;

			bool isValueType = ((typeFlags & TypeFlags.Value) == TypeFlags.Value);

			int dataSize = HeaderSize;
			dataSize += (isValueType)
				? SizeCalculator.GetForValueType(type)(instance)
				: SizeCalculator.GetForReferenceType(type)(instance, sizes, values, context);

			byte[] data = new byte[dataSize];

			WriteHeader(data, ref index);

			if (isValueType)
				Writer.GetDelegateForValueType(type)(instance, data, ref index);
			else
				Writer.GetDelegateForReferenceType(type)(instance, data, ref index, sizes, values, context);

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
		EFormatFlags formatFlags = (EFormatFlags)data[index++];

		if (version != 1)
			throw new System.ArgumentException("Invalid version " + version);

		if ((formatFlags & EFormatFlags.LZ4Compression) == EFormatFlags.LZ4Compression)
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
		data[index++] = (byte)FormatFlags;
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
