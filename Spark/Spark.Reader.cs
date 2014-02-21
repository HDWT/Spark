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
			if (type.IsValueType)
			{
				if (type == typeof(int))
					return TypeHelper.Int.ReadObject;

				if (type == typeof(float))
					return TypeHelper.Float.ReadObject;

				if (type == typeof(bool))
					return TypeHelper.Bool.ReadObject;

				if (type.IsEnum)
					return EnumTypeHelper.Instance.GetReader(type);

				if (type == typeof(char))
					return TypeHelper.Char.ReadObject;

				if (type == typeof(long))
					return TypeHelper.Long.ReadObject;

				if (type == typeof(short))
					return TypeHelper.Short.ReadObject;

				if (type == typeof(byte))
					return TypeHelper.Byte.ReadObject;

				if (type == typeof(DateTime))
					return TypeHelper.DateTime.ReadObject;

				if (type == typeof(double))
					return TypeHelper.Double.ReadObject;

				if (type == typeof(uint))
					return TypeHelper.UInt.ReadObject;

				if (type == typeof(ushort))
					return TypeHelper.UShort.ReadObject;

				if (type == typeof(ulong))
					return TypeHelper.ULong.ReadObject;

				if (type == typeof(sbyte))
					return TypeHelper.SByte.ReadObject; 

				if (type == typeof(decimal))
					return TypeHelper.Decimal.ReadObject;

				throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
			}

			if (type == typeof(string))
				return TypeHelper.String.ReadObject;

			if (type.IsArray)
				return TypeHelper.Array.ReadObject;

			if (IsGenericList(type))
				return TypeHelper.List.ReadObject;

			if (type.IsClass)
				return TypeHelper.Object.ReadObject;

			throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
		}
	}
}