using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private delegate void WriteDataDelegate(object value, byte[] data, ref int startIndex);
	private delegate void WriteDataDelegate<T>(T value, byte[] data, ref int startIndex);

	private static class Writer
	{
		private static readonly Dictionary<Type, WriteDataDelegate> s_writersByType = new Dictionary<Type, WriteDataDelegate>(16);

		public static WriteDataDelegate Get(Type type)
		{
			WriteDataDelegate writer = null;

			if (!s_writersByType.TryGetValue(type, out writer))
			{
				lock (s_writersByType)
				{
					writer = GetDelegate(type);
					s_writersByType[type] = writer;
				}
			}

			return writer;
		}

		private static WriteDataDelegate GetDelegate(Type type)
		{
			Type baseType = type.BaseType;

			if (baseType != typeof(object))
			{
				if (baseType == typeof(ValueType))
				{
					if (type == typeof(int))
						return TypeHelper.Int.WriteObject;

					if (type == typeof(float))
						return TypeHelper.Float.WriteObject;

					if (type == typeof(bool))
						return TypeHelper.Bool.WriteObject;

					if (type == typeof(char))
						return TypeHelper.Char.WriteObject;

					if (type == typeof(long))
						return TypeHelper.Long.WriteObject;

					if (type == typeof(short))
						return TypeHelper.Short.WriteObject;

					if (type == typeof(byte))
						return TypeHelper.Byte.WriteObject;

					if (type == typeof(DateTime))
						return TypeHelper.DateTime.WriteObject;

					if (type == typeof(double))
						return TypeHelper.Double.WriteObject;

					if (type == typeof(uint))
						return TypeHelper.UInt.WriteObject;

					if (type == typeof(ushort))
						return TypeHelper.UShort.WriteObject;

					if (type == typeof(ulong))
						return TypeHelper.ULong.WriteObject;

					if (type == typeof(sbyte))
						return TypeHelper.SByte.WriteObject;

					if (type == typeof(decimal))
						return TypeHelper.Decimal.WriteObject;

					throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
				}
				else if (baseType == typeof(Enum))
				{
					return EnumTypeHelper.Instance.GetWriter(type);
				}
				else if (baseType == typeof(Array))
				{
					return TypeHelper.Array.WriteObject;
				}
				else
				{
					throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
				}
			}
			else
			{
				if (type == typeof(string))
					return TypeHelper.String.WriteObject;

				if (IsGenericList(type))
					return TypeHelper.List.WriteObject;

				if (type.IsClass)
					return TypeHelper.Object.WriteObject;

				throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
			}
		}

		public static void Write<T>(T value, byte[] data, ref int startIndex)
		{
			//if (typeof(T) == typeof(string))
			//{
			//	Write(value, data, ref startIndex);
			//}
			//if (typeof(T).BaseType == typeof(Enum))
			//{
			//	EnumTypeHelper.Instance.GetWriter(typeof(T))(value, data, ref startIndex);
			//}
			//else if (typeof(T).BaseType == typeof(Array))
			//{
			//	WriteArray(value, data, ref startIndex);
			//}
			//else if (IsGenericList(typeof(T)))
			//{
			//	WriteList(value, data, ref startIndex);
			//}
			//else
			//{
				LowLevelType<T>.Write(value, data, ref startIndex);
			//}
		}
	}
}