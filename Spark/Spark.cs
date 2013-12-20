using System;
using System.Collections.Generic;
using System.Reflection.Emit;

public static partial class Spark
{
	public static bool FullAot = false;

	static Spark()
	{
		InitLowLevelTypes();
	}

	public static byte[] Serialize(object instance)
	{
		if (instance == null)
			return new byte[0];

		Type type = instance.GetType();
		int index = 0;

		if (IsLowLevelType(type))
		{
			int dataSize = SizeCalculator.Evaluate(instance);
			byte[] data = new byte[dataSize];

			Writer.Write(instance, data, ref index);

			//WriteData(instance, data, ref index);
			return data;
		}
		else
		{
			DataType dataType = DataType.Get(type);

			int dataSize = dataType.GetDataSize(instance);
			byte[] data = new byte[dataSize];

			dataType.WriteValues(instance, data, ref index);

			return data;
		}
	}

	public static T Deserialize<T>(byte[] data)
	{
		if (data.Length == 0)
			return default(T);

		Type type = typeof(T);
		int index = 0;

		if (IsLowLevelType(type))
		{
			var ReadData = Reader.Get(type);
			return (T)ReadData(type, data, ref index);
		}
		else
		{
			DataType dataType = DataType.Get(type);

			T instance = (T)dataType.CreateInstance();

			dataType.ReadValues(instance, data, ref index, data.Length);
			return instance;
		}
	}

	public static int GetSize(object instance)
	{
		DataType dataType = DataType.Get(instance.GetType());

		return dataType.GetDataSize(instance);
	}

	//[Flags]
	//private enum TypeTypes
	//{
	//	Value = 1 << 0,
	//	Reference = 1 << 1,
	//	Enum = 1 << 2,
	//	Array = 1 << 3,
	//	List = 1 << 4,

	//	Bool = 1 << 19,
	//	Byte = 1 << 20,
	//	SByte = 1 << 21,
	//	Char = 1 << 22,
	//	Short = 1 << 23,
	//	UShort = 1 << 24,
	//	Int = 1 << 25,
	//	UInt = 1 << 26,
	//	Long = 1 << 27,
	//	ULong = 1 << 28,
	//	Float = 1 << 29,
	//	Double = 1 << 30,
	//	Decimal = 1 << 31,
	//}

	//static readonly Dictionary<Type, TypeTypes> LowLevel = new Dictionary<Type, TypeTypes>()
	//{
	//	{ typeof(int), TypeTypes.Value | TypeTypes.Int }
	//};

	private static bool IsLowLevelType(Type type)
	{
		Type baseType = type.BaseType;

		if (baseType == null)
			return false;

		if (baseType != typeof(object))
		{
			if (baseType == typeof(ValueType))
			{
				return
					type == typeof(int)			||
					type == typeof(float)		||
					type == typeof(bool)		||
					type == typeof(char)		||
					type == typeof(long)		||
					type == typeof(short)		||
					type == typeof(byte)		||
					type == typeof(double)		||
					type == typeof(DateTime)	||
					type == typeof(uint)		||
					type == typeof(ushort)		||
					type == typeof(ulong)		||
					type == typeof(sbyte)		||
					type == typeof(decimal);
			}

			if (baseType == typeof(Enum))
				return true;

			if (baseType == typeof(Array))
				return true;
		}
		else
		{
			if (type == typeof(string))
				return true;

			if (type.IsGenericList())
				return true;
		}

		return false;
	}

	private static bool IsEnum(this Type type)
	{
		return (type.BaseType == typeof(Enum));
	}

	private static bool IsArray(this Type type)
	{
		return (type.BaseType == typeof(Array));
	}

	private static bool IsGenericList(this Type type)
	{
		return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
	}
}
