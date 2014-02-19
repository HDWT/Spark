using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private delegate int GetSizeDelegate(object value);
	private delegate int GetSizeDelegate<T>(T value);

	private static class SizeCalculator
	{
		public const int MinDataSize = 1;

		public static GetSizeDelegate Get(Type type)
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
					return EvaluateEnum;

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

				throw new NotImplementedException("Type '" + type + "' is not suppoerted");
			}
			else
			{
				if (type == typeof(string))
					return TypeHelper.String.GetSize;

				if (type.IsArray)
					return TypeHelper.Array.GetSize;

				if (IsGenericList(type))
					return EvaluateList;

				if (type.IsClass)
					return EvaluateClass;

				throw new NotImplementedException("Type '" + type + "' is not suppoerted");
			}
		}

		public static GetSizeDelegate<T> Get<T>()
		{
			//if (typeof(T).BaseType == typeof(Array))
			//	return Evaluate<T>;

			return LowLevelType<T>.GetSize;
		}

		public static int Evaluate(object value)
		{
			Type type = value.GetType();
			Type baseType = type.BaseType;

			if (baseType != typeof(object))
			{
				if (baseType == typeof(ValueType))
				{
					if (type == typeof(bool))
						return LowLevelType<bool>.GetSize((bool)value);

					if (type == typeof(byte))
						return LowLevelType<byte>.GetSize((byte)value);

					if (type == typeof(sbyte))
						return LowLevelType<sbyte>.GetSize((sbyte)value);

					if (type == typeof(char))
						return LowLevelType<char>.GetSize((char)value);

					if (type == typeof(short))
						return LowLevelType<short>.GetSize((short)value);

					if (type == typeof(ushort))
						return LowLevelType<ushort>.GetSize((ushort)value);

					if (type == typeof(int))
						return LowLevelType<int>.GetSize((int)value);

					if (type == typeof(uint))
						return LowLevelType<uint>.GetSize((uint)value);

					if (type == typeof(long))
						return LowLevelType<long>.GetSize((long)value);

					if (type == typeof(ulong))
						return LowLevelType<ulong>.GetSize((ulong)value);

					if (type == typeof(float))
						return LowLevelType<float>.GetSize((float)value);

					if (type == typeof(double))
						return LowLevelType<double>.GetSize((double)value);

					if (type == typeof(decimal))
						return LowLevelType<decimal>.GetSize((decimal)value);

					if (type == typeof(DateTime))
						return LowLevelType<DateTime>.GetSize((DateTime)value);

					throw new System.ArgumentException();
				}

				if (baseType == typeof(Enum))
					return EnumTypeHelper.Instance.GetSize(value);

				if (baseType == typeof(Array))
					TypeHelper.Array.GetSize(value);

				if (type.IsClass)
					return EvaluateClass(value);

				throw new System.ArgumentException();
			}
			else
			{
				if (type == typeof(string))
					return TypeHelper.String.GetSize(value);

				if (IsGenericList(type))
					return Evaluate((IList)value);

				if (type.IsClass)
					return EvaluateClass(value);

				throw new System.ArgumentException();
			}
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


		public static int Evaluate(Enum value)
		{
			return EnumTypeHelper.Instance.GetSize(value);
		}

		public static int Evaluate(IList list)
		{
			if (list == null)
				return MinDataSize;

			int dataSize = MinDataSize + ListTypeHelper.GetSize(list);

			dataSize += 1 + GetMinSize(list.Count);

			return dataSize + GetMinSize(dataSize + GetMinSize(dataSize));
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

		// Private

		private static int EvaluateEnum(object value)
		{
			return Evaluate((Enum)value);
		}

		private static int EvaluateList(object value)
		{
			return Evaluate((IList)value);
		}

		private static int EvaluateClass(object value)
		{
			if (value == null)
				return MinDataSize;

			DataType dataType = DataType.Get(value.GetType());

			int dataSize = MinDataSize + dataType.GetDataSize(value);

			return dataSize + GetMinSize(dataSize + GetMinSize(dataSize));
		}
	}
}