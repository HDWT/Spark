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
					return TypeHelper.Float.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Bool))
					return TypeHelper.Bool.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Enum))
					return EnumTypeHelper.Instance.GetReader(type);

				if (IsFlag(typeFlags, TypeFlags.Char))
					return TypeHelper.Char.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Long))
					return TypeHelper.Long.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Short))
					return TypeHelper.Short.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Byte))
					return TypeHelper.Byte.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.DateTime))
					return TypeHelper.DateTime.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Double))
					return TypeHelper.Double.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.UInt))
					return TypeHelper.UInt.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.UShort))
					return TypeHelper.UShort.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.ULong))
					return TypeHelper.ULong.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.SByte))
					return TypeHelper.SByte.ReadObject;

				if (IsFlag(typeFlags, TypeFlags.Decimal))
					return TypeHelper.Decimal.ReadObject;

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