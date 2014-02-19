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
					return EvaluateInt;

				if (type == typeof(float))
					return EvaluateFloat;

				if (type == typeof(bool))
					return EvaluateBool;

				if (type.IsEnum)
					return EvaluateEnum;

				if (type == typeof(DateTime))
					return EvaluateDateTime;

				if (type == typeof(short))
					return EvaluateShort;

				if (type == typeof(long))
					return EvaluateLong;

				if (type == typeof(double))
					return EvaluateDouble;

				if (type == typeof(byte))
					return EvaluateByte;

				if (type == typeof(char))
					return EvaluateChar;

				if (type == typeof(uint))
					return EvaluateUInt;

				if (type == typeof(ushort))
					return EvaluateUShort;

				if (type == typeof(ulong))
					return EvaluateULong;

				if (type == typeof(decimal))
					return EvaluateDecimal;

				if (type == typeof(sbyte))
					return EvaluateSByte;

				throw new NotImplementedException("Type '" + type + "' is not suppoerted");
			}
			else
			{
				if (type == typeof(string))
					return TypeHelper.String.GetSize;

				if (type.IsArray)
					return EvaluateArray;

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
					return Evaluate((Array)value);

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

		public static int Evaluate(bool value)
		{
			return TypeHelper.Bool.GetSize(value);
		}

		public static int Evaluate(byte value)
		{
			return TypeHelper.Byte.GetSize(value);
		}

		public static int Evaluate(sbyte value)
		{
			return TypeHelper.Int.GetSize(value);
		}

		public static int Evaluate(char value)
		{
			return TypeHelper.Char.GetSize(value);
		}

		public static int Evaluate(short value)
		{
			return TypeHelper.Short.GetSize(value);
		}

		public static int Evaluate(ushort value)
		{
			return TypeHelper.UShort.GetSize(value);
		}

		public static int Evaluate(int value)
		{
			return TypeHelper.Int.GetSize(value);
		}

		public static int Evaluate(uint value)
		{
			return TypeHelper.UInt.GetSize(value);
		}

		public static int Evaluate(long value)
		{
			return TypeHelper.Long.GetSize(value);
		}

		public static int Evaluate(ulong value)
		{
			return TypeHelper.ULong.GetSize(value);
		}

		public static int Evaluate(float value)
		{
			return TypeHelper.Float.GetSize(value);
		}

		public static int Evaluate(double value)
		{
			return TypeHelper.Double.GetSize(value);
		}

		public static int Evaluate(decimal value)
		{
			return TypeHelper.Decimal.GetSize(value);
		}

		public static int Evaluate(Enum value)
		{
			return EnumTypeHelper.Instance.GetSize(value);
		}

		public static int Evaluate(DateTime value)
		{
			return Evaluate(value.Ticks);
		}

		public static int Evaluate(Array array)
		{
			if (array == null)
				return MinDataSize;

			int dataSize = MinDataSize + ArrayTypeHelper.GetSize(array);

			// 1 байт чтобы записать Rank - An array can have a maximum of 32 dimensions(MSDN)
			dataSize += 1;

			for (int i = 0; i < array.Rank; ++i)
				dataSize += 1 + GetMinSize(array.GetLength(i));

			return dataSize + GetMinSize(dataSize + GetMinSize(dataSize));
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

		private static int EvaluateBool(object value)
		{
			return Evaluate((bool)value);
		}

		private static int EvaluateByte(object value)
		{
			return Evaluate((byte)value);
		}

		private static int EvaluateSByte(object value)
		{
			return Evaluate((sbyte)value);
		}

		private static int EvaluateChar(object value)
		{
			return Evaluate((char)value);
		}

		private static int EvaluateShort(object value)
		{
			return Evaluate((short)value);
		}

		private static int EvaluateUShort(object value)
		{
			return Evaluate((ushort)value);
		}

		private static int EvaluateInt(object value)
		{
			return Evaluate((int)value);
		}

		private static int EvaluateUInt(object value)
		{
			return Evaluate((uint)value);
		}

		private static int EvaluateLong(object value)
		{
			return Evaluate((long)value);
		}

		private static int EvaluateULong(object value)
		{
			return Evaluate((ulong)value);
		}

		private static int EvaluateFloat(object value)
		{
			return Evaluate((float)value);
		}

		private static int EvaluateDouble(object value)
		{
			return Evaluate((double)value);
		}

		private static int EvaluateDecimal(object value)
		{
			return Evaluate((decimal)value);
		}

		private static int EvaluateEnum(object value)
		{
			return Evaluate((Enum)value);
		}

		private static int EvaluateDateTime(object value)
		{
			return Evaluate((DateTime)value);
		}

		private static int EvaluateArray(object value)
		{
			return Evaluate((Array)value);
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