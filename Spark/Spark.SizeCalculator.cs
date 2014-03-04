using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private delegate int GetSizeDelegate(object value);
	private delegate int GetSizeDelegate<T>(T value);

	private static class SizeCalculator
	{
		private static readonly Dictionary<Type, GetSizeDelegate> s_getSizeDelegatesByType = new Dictionary<Type, GetSizeDelegate>(16);

		public static GetSizeDelegate Get(Type type)
		{
			GetSizeDelegate sizeGetter = null;

			if (!s_getSizeDelegatesByType.TryGetValue(type, out sizeGetter))
			{
				lock (s_getSizeDelegatesByType)
				{
					sizeGetter = GetDelegate(type);
					s_getSizeDelegatesByType.Add(type, sizeGetter);
				}
			}

			return sizeGetter;
		}

		private static GetSizeDelegate GetDelegate(Type type)
		{
			if (type.IsValueType)
			{
				if (type == typeof(int))
					return TypeHelper.Int.GetSize;

				if (type == typeof(float))
					return TypeHelper.Float.GetSize;;

				if (type == typeof(bool))
					return TypeHelper.Bool.GetSize;;

				if (type.IsEnum)
					return EnumTypeHelper.Instance.GetSize;

				if (type == typeof(DateTime))
					return TypeHelper.DateTime.GetSize;

				if (type == typeof(short))
					return TypeHelper.Short.GetSize;

				if (type == typeof(long))
					return TypeHelper.Long.GetSize;

				if (type == typeof(double))
					return TypeHelper.Double.GetSize;

				if (type == typeof(byte))
					return TypeHelper.Byte.GetSize;

				if (type == typeof(char))
					return TypeHelper.Char.GetSize;

				if (type == typeof(uint))
					return TypeHelper.UInt.GetSize;

				if (type == typeof(ushort))
					return TypeHelper.UShort.GetSize;

				if (type == typeof(ulong))
					return TypeHelper.ULong.GetSize;

				if (type == typeof(decimal))
					return TypeHelper.Decimal.GetSize;

				if (type == typeof(sbyte))
					return TypeHelper.SByte.GetSize;

				throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
			}
			else
			{
				if (type == typeof(string))
					return TypeHelper.String.GetSize;

				if (type.IsArray)
					return TypeHelper.Array.GetSize;

				if (IsGenericList(type))
					return TypeHelper.List.GetSize;

				if (type.IsClass)
					return TypeHelper.ObjectType.GetGetSizeDelegate(type);// Object.GetSize;

				throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
			}
		}

		public static GetSizeDelegate<T> Get<T>()
		{
			//if (typeof(T).BaseType == typeof(Array))
			//	return Evaluate<T>;

			return LowLevelType<T>.GetSize;
		}

		public static int Evaluate<T>(T value)
		{
			//if (!IsLowLevelType(typeof(T)))
			//	return EvaluateClass(value);

			//if (typeof(T).BaseType == typeof(Enum))
			//	return EvaluateEnum(value);

			//if (typeof(T).BaseType == typeof(Array))
			//	return EvaluateArray(value);

			//if (IsGenericList(typeof(T)))
			//	return EvaluateList(value);

			return LowLevelType<T>.GetSize(value);
		}

		// Возвращает минимальное количество байт, которое необходимо для записи [dataSize]
		public static byte GetMinSize(int dataSize)
		{
			if (dataSize <= 0xFF)
				return 1;

			if (dataSize <= 0xFFFF)
				return 2;

			if (dataSize <= 0xFFFFFF)
				return 3;

			if (dataSize <= 0x7FFFFFFF)
				return 4;

			throw new ArgumentException("Invalid data size: " + dataSize);
		}
	}
}