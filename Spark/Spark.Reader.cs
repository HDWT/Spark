using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private delegate object ReadDataDelegate(Type type, byte[] data, ref int startIndex);

	private static class Reader
	{
		public static ReadDataDelegate Get(Type type)
		{
			TypeFlags typeFlags = GetTypeFlags(type);

			if (HasFlag(typeFlags, TypeFlags.Value))
			{
				if (IsFlag(typeFlags, TypeFlags.Int))
					return TypeHelper.IntType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Float))
					return TypeHelper.FloatType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Bool))
					return TypeHelper.BoolType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Enum))
					return EnumTypeHelper.Instance.GetReader(type);

				if (IsFlag(typeFlags, TypeFlags.Char))
					return TypeHelper.CharType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Long))
					return TypeHelper.LongType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Short))
					return TypeHelper.ShortType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Byte))
					return TypeHelper.ByteType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.DateTime))
					return TypeHelper.DateTimeType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Double))
					return TypeHelper.DoubleType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.UInt))
					return TypeHelper.UIntType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.UShort))
					return TypeHelper.UShortType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.ULong))
					return TypeHelper.ULongType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.SByte))
					return TypeHelper.SByteType.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Decimal))
					return TypeHelper.DecimalType.ReadObject;

				throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
			}

			if (IsFlag(typeFlags, TypeFlags.String))
				return TypeHelper.String.ReadObject;

			if (IsFlag(typeFlags, TypeFlags.Array))
				return TypeHelper.Array.ReadObject;

			if (IsFlag(typeFlags, TypeFlags.List))
				return TypeHelper.List.ReadObject;

			if (IsFlag(typeFlags, TypeFlags.Dictionary))
				return TypeHelper.Dictionary.ReadObject;

			if (IsFlag(typeFlags, TypeFlags.Class) || IsFlag(typeFlags, TypeFlags.Abstract) || IsFlag(typeFlags, TypeFlags.Interface))
				return TypeHelper.Object.ReadObject;

			throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
		}
	}
}