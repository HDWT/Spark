using System;
using System.Collections.Generic;

public static partial class Spark
{
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

		Class		= 1 << 14 | Reference,
		Abstract	= 1 << 15 | Reference,
		Interface	= 1 << 16 | Reference,

		String		= 1 << 17 | Basic | Reference,
		DateTime	= 1 << 18 | Basic | Value,

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

	private static Dictionary<Type, TypeFlags> s_flagsByType = new Dictionary<Type, TypeFlags>()
	{
		{ typeof(int),		TypeFlags.Int },
		{ typeof(float),	TypeFlags.Float },
		{ typeof(bool),		TypeFlags.Bool },
		{ typeof(char),		TypeFlags.Char },
		{ typeof(long),		TypeFlags.Long },
		{ typeof(short),	TypeFlags.Short },
		{ typeof(byte),		TypeFlags.Byte },
		{ typeof(double),	TypeFlags.Double },
		{ typeof(DateTime), TypeFlags.DateTime },
		{ typeof(uint),		TypeFlags.UInt },
		{ typeof(ushort),	TypeFlags.UShort },
		{ typeof(ulong),	TypeFlags.ULong },
		{ typeof(sbyte),	TypeFlags.SByte },
		{ typeof(decimal),	TypeFlags.Decimal },
		{ typeof(string),	TypeFlags.String },
	};

	private static TypeFlags GetTypeFlags(Type type)
	{
		TypeFlags typeFlags = TypeFlags.Reference;

		if (s_flagsByType.TryGetValue(type, out typeFlags))
			return typeFlags;

		Type baseType = type.BaseType;

		if (baseType != null && baseType == typeof(Enum))
			typeFlags = TypeFlags.Enum;

		else if (baseType != null && baseType == typeof(Array))
			typeFlags = TypeFlags.Array;

		else if (IsGenericList(type))
			typeFlags = TypeFlags.List;

		else if (IsGenericDictionary(type))
			typeFlags = TypeFlags.Dictionary;

		else if (type.IsInterface)
			typeFlags = TypeFlags.Interface;

		else if (type.IsClass)
			typeFlags = (type.IsAbstract) ? TypeFlags.Abstract : TypeFlags.Class;

		s_flagsByType[type] = typeFlags;
		return typeFlags;
	}
}