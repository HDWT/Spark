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

			if (typeFlags.Has(TypeFlags.Value))
			{
				if (typeFlags.Is(TypeFlags.Int))
					return TypeHelper.IntType.ReadObject;

				if (typeFlags.Is(TypeFlags.Float))
					return TypeHelper.Float.ReadObject;

				if (typeFlags.Is(TypeFlags.Bool))
					return TypeHelper.Bool.ReadObject;

				if (typeFlags.Is(TypeFlags.Enum))
					return EnumTypeHelper.Instance.GetReader(type);

				if (typeFlags.Is(TypeFlags.Char))
					return TypeHelper.Char.ReadObject;

				if (typeFlags.Is(TypeFlags.Long))
					return TypeHelper.Long.ReadObject;

				if (typeFlags.Is(TypeFlags.Short))
					return TypeHelper.Short.ReadObject;

				if (typeFlags.Is(TypeFlags.Byte))
					return TypeHelper.Byte.ReadObject;

				if (typeFlags.Is(TypeFlags.DateTime))
					return TypeHelper.DateTime.ReadObject;

				if (typeFlags.Is(TypeFlags.Double))
					return TypeHelper.Double.ReadObject;

				if (typeFlags.Is(TypeFlags.UInt))
					return TypeHelper.UInt.ReadObject;

				if (typeFlags.Is(TypeFlags.UShort))
					return TypeHelper.UShort.ReadObject;

				if (typeFlags.Is(TypeFlags.ULong))
					return TypeHelper.ULong.ReadObject;

				if (typeFlags.Is(TypeFlags.SByte))
					return TypeHelper.SByte.ReadObject;

				if (typeFlags.Is(TypeFlags.Decimal))
					return TypeHelper.Decimal.ReadObject;

				throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
			}

			if (typeFlags.Is(TypeFlags.String))
				return TypeHelper.String.ReadObject;

			if (typeFlags.Is(TypeFlags.Array))
				return TypeHelper.Array.ReadObject;

			if (typeFlags.Is(TypeFlags.List))
				return TypeHelper.List.ReadObject;

			if (typeFlags.Is(TypeFlags.Dictionary))
				return TypeHelper.Dictionary.ReadObject;

			if (typeFlags.Is(TypeFlags.Class) || typeFlags.Is(TypeFlags.Abstract) || typeFlags.Is(TypeFlags.Interface))
				return TypeHelper.Object.ReadObject;

			throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
		}
	}
}