using System;
using System.Collections.Generic;

public static partial class Spark
{
	[System.Flags]
	private enum FormatFlags : byte
	{
		None = 0,
		LZ4Compression = 1 << 0,
	}

	private static readonly byte s_version = 1;
	private static readonly int s_headerSize = 2;
	private static FormatFlags s_formatFlags = FormatFlags.None;// FormatFlags.LZ4Compression;

	public static bool UTF8Encoding = false;

	// Restriction: (MaxMemberId * InheritanceDepth - 1) < 32768
	private const ushort MaxMemberId = 1024; // Zero-Based
	private const ushort MaxInheritanceDepth = 32;

	public static bool FullAot = true;

	public static byte[] Serialize(object instance)
	{
		if (instance == null)
			return new byte[0];

		Type type = instance.GetType();
		int index = 0;

		TypeFlags typeFlags = GetTypeFlags(type);

		if ((typeFlags == TypeFlags.Reference) || (typeFlags == TypeFlags.Value))
		{
			DataType dataType = DataType.Get(type);

			QueueWithIndexer sizes = new QueueWithIndexer();

			int dataSize = s_headerSize + dataType.GetDataSize(instance, sizes);
			byte[] data = new byte[dataSize];

			WriteHeader(data, ref index);
			dataType.WriteValues(instance, data, ref index, sizes);

			if (LZ4Compression)
				data = LZ4Encode(data);

			return data;
		}
		else
		{
			QueueWithIndexer sizes = new QueueWithIndexer();

			bool isValueType = type.IsValueType;

			int dataSize = s_headerSize;
			dataSize += (isValueType)
				? SizeCalculator.GetForValueType(type)(instance)
				: SizeCalculator.GetForReferenceType(type)(instance, sizes);

			byte[] data = new byte[dataSize];

			WriteHeader(data, ref index);

			if (isValueType)
				Writer.GetDelegateForValueType(type)(instance, data, ref index);
			else
				Writer.GetDelegateForReferenceType(type)(instance, data, ref index, sizes);

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

		if ((typeFlags == TypeFlags.Reference) || (typeFlags == TypeFlags.Value))
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
		data[index++] = s_version;
		data[index++] = (byte)s_formatFlags;
	}

	[Flags]
	private enum TypeFlags
	{
		Reference	= 1 << 0,
		Value		= 1 << 1,
		Basic		= 1 << 2,

		Enum		= 1 << 10 | Value,
		Array		= 1 << 11 | Reference,
		List		= 1 << 12 | Reference,
		Dictionary	= 1 << 13 | Reference,

		String		= 1 << 15 | Basic | Reference,
		DateTime	= 1 << 16 | Basic | Value,

		Bool		= 1 << 19 | Basic | Value,
		Byte		= 1 << 20 | Basic | Value,
		SByte		= 1 << 21 | Basic | Value,
		Char		= 1 << 22 | Basic | Value,
		Short		= 1 << 23 | Basic | Value,
		UShort		= 1 << 24 | Basic | Value,
		Int			= 1 << 25 | Basic | Value,
		UInt		= 1 << 26 | Basic | Value,
		Long		= 1 << 27 | Basic | Value,
		ULong		= 1 << 28 | Basic | Value,
		Float		= 1 << 29 | Basic | Value,
		Double		= 1 << 30 | Basic | Value,
		Decimal		= 1 << 31 | Basic | Value,
	}

	private static TypeFlags GetTypeFlags(Type type)
	{
		if (type == typeof(int))
			return TypeFlags.Int;

		if (type == typeof(float))
			return TypeFlags.Float;

		if (type == typeof(bool))
			return TypeFlags.Bool;

		if (type == typeof(char))
			return TypeFlags.Char;

		if (type == typeof(long))
			return TypeFlags.Long;

		if (type == typeof(short))
			return TypeFlags.Short;

		if (type == typeof(byte))
			return TypeFlags.Byte;

		if (type == typeof(double))
			return TypeFlags.Double;

		if (type == typeof(DateTime))
			return TypeFlags.DateTime;

		if (type == typeof(uint))
			return TypeFlags.UInt;

		if (type == typeof(ushort))
			return TypeFlags.UShort;

		if (type == typeof(ulong))
			return TypeFlags.ULong;

		if (type == typeof(sbyte))
			return TypeFlags.SByte;

		if (type == typeof(decimal))
			return TypeFlags.Decimal;

		if (type == typeof(string))
			return TypeFlags.String;

		if (IsGenericList(type))
			return TypeFlags.List;

		if (IsGenericDictionary(type))
			return TypeFlags.Dictionary;

		Type baseType = type.BaseType;

		if (baseType != null)
		{
			if (baseType == typeof(Enum))
				return TypeFlags.Enum;

			if (baseType == typeof(Array))
				return TypeFlags.Array;
		}

		return TypeFlags.Reference;
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
		void BeforeDeserialize(object instance);
		void AfterDeserialize(object instance);

		void BeforeSetValue(object instance, ushort memberId, bool validMemberId);
		void AfterSetValue(object instance, ushort memberId, bool validMemberId);
	}
}
