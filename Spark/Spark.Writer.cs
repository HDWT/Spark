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
		const byte NullReference = 0;

		public static WriteDataDelegate Get(Type type)
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
						return WriteDateTime;

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

					throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
				}
				else if (baseType == typeof(Enum))
				{
					return EnumTypeHelper.Instance.GetWriter(type);
				}
				else if (baseType == typeof(Array))
				{
					return WriteArray;
				}
				else
				{
					throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
				}
			}
			else
			{
				if (type == typeof(string))
					return WriteString;

				if (IsGenericList(type))
					return WriteList;

				if (type.IsClass)
					return WriteClass;

				throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
			}



			//if (type.IsValueType)
			//{
			//	WriteDataDelegate writer = null;

			//	if (type.IsEnum)
			//		return EnumTypeHelper.Instance.GetWriter(type);

			//	if (type == typeof(DateTime))
			//		return WriteDateTime;

			//	if (BasicTypeHelper.TryGetWriter(type, out writer))
			//		return writer;

			//	//throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
			//}

			//if (type == typeof(string))
			//	return WriteString;

			//if (type.IsArray)
			//	return WriteArray;

			//if (type.IsGenericList())
			//	return WriteList;



			//throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
		}

		public static void Write(object value, byte[] data, ref int startIndex)
		{
			Type type = value.GetType();
			Type baseType = type.BaseType;

			if (baseType != typeof(object))
			{
				if (baseType == typeof(ValueType))
				{
					if (type == typeof(bool))
						LowLevelType<bool>.Write((bool)value, data, ref startIndex);

					else if (type == typeof(byte))
						LowLevelType<byte>.Write((byte)value, data, ref startIndex);

					else if (type == typeof(sbyte))
						LowLevelType<sbyte>.Write((sbyte)value, data, ref startIndex);

					else if (type == typeof(char))
						LowLevelType<char>.Write((char)value, data, ref startIndex);

					else if (type == typeof(short))
						LowLevelType<short>.Write((short)value, data, ref startIndex);

					else if (type == typeof(ushort))
						LowLevelType<ushort>.Write((ushort)value, data, ref startIndex);

					else if (type == typeof(int))
						LowLevelType<int>.Write((int)value, data, ref startIndex);

					else if (type == typeof(uint))
						LowLevelType<uint>.Write((uint)value, data, ref startIndex);

					else if (type == typeof(long))
						LowLevelType<long>.Write((long)value, data, ref startIndex);

					else if (type == typeof(ulong))
						LowLevelType<ulong>.Write((ulong)value, data, ref startIndex);

					else if (type == typeof(float))
						LowLevelType<float>.Write((float)value, data, ref startIndex);

					else if (type == typeof(double))
						LowLevelType<double>.Write((double)value, data, ref startIndex);

					else if (type == typeof(decimal))
						LowLevelType<decimal>.Write((decimal)value, data, ref startIndex);

					else if (type == typeof(DateTime))
						LowLevelType<DateTime>.Write((DateTime)value, data, ref startIndex);

					else throw new System.ArgumentException();
				}
				else if (baseType == typeof(Enum))
				{
					EnumTypeHelper.Instance.GetWriter(type)(value, data, ref startIndex);
				}
				else if (baseType == typeof(Array))
				{
					WriteArray(value, data, ref startIndex);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
			else
			{
				if (type == typeof(string))
					Write((string)value, data, ref startIndex);

				else if (IsGenericList(type))
					WriteList(value, data, ref startIndex);

				else throw new System.ArgumentException();
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

		public static void Write(DateTime value, byte[] data, ref int startIndex)
		{
			TypeHelper.Long.Write(value.Ticks, data, ref startIndex);
			//BasicTypeHelper.Instance.WriteLong(value.Ticks, data, ref startIndex);
		}

		private static void WriteDateTime(object value, byte[] data, ref int startIndex)
		{
			Write((DateTime)value, data, ref startIndex);
		}

		public static void Write(string value, byte[] data, ref int startIndex)
		{
			if (value == null)
			{
				data[startIndex++] = NullReference;
				return;
			}

			int dataSize = SizeCalculator.Evaluate(value);
			byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

			// Сколько байт занимает поле dataSize
			int dataSizeBlockIndex = startIndex;
			data[startIndex++] = dataSizeBlock;

			// Записываем длину строки
			for (int i = 0; i < dataSizeBlock; ++i)
			{
				data[startIndex++] = (byte)dataSize;
				dataSize >>= 8;
			}

			bool forwardPadding = (startIndex % 2 != 0);

			if (forwardPadding)
				data[dataSizeBlockIndex] += ForwardPaddingMark;

			if (forwardPadding)
				data[startIndex++] = PaddingValue;

			TypeHelper.CharMapper mapper = new TypeHelper.CharMapper();

			// Записываем все символы в строке
			foreach (char ch in value)
			{
				mapper.value = ch;

				data[startIndex++] = mapper.byte1;
				data[startIndex++] = mapper.byte2;
			}

			if (!forwardPadding)
				data[startIndex++] = PaddingValue;
		}

		private static void WriteString(object value, byte[] data, ref int startIndex)
		{
			Write((string)value, data, ref startIndex);
		}

		private static void WriteArray(object value, byte[] data, ref int startIndex)
		{
			if (value == null)
			{
				data[startIndex++] = NullReference;
				return;
			}

			Array array = (Array)value;

			int dataSize = SizeCalculator.Evaluate(value);
			byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

			// Сколько байт занимает поле dataSize
			data[startIndex++] = dataSizeBlock;

			// 
			for (int i = 0; i < dataSizeBlock; ++i)
			{
				data[startIndex++] = (byte)dataSize;
				dataSize >>= 8;
			}

			// An array can have a maximum of 32 dimensions(MSDN)
			data[startIndex++] = (byte)array.Rank;

			for (int i = 0; i < array.Rank; ++i)
			{
				int arrayLength = array.GetLength(i);
				byte arrayLengthBlock = SizeCalculator.GetMinSize(arrayLength);

				data[startIndex++] = arrayLengthBlock;

				// 
				for (int j = 0; j < arrayLengthBlock; ++j)
				{
					data[startIndex++] = (byte)arrayLength;
					arrayLength >>= 8;
				}
			}

			ArrayTypeHelper.Write(array, data, ref startIndex);
		}

		private static void WriteList(IList list, byte[] data, ref int startIndex)
		{
			int dataSize = SizeCalculator.Evaluate(list);
			byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

			// Сколько байт занимает поле dataSize
			data[startIndex++] = dataSizeBlock;

			// 
			for (int i = 0; i < dataSizeBlock; ++i)
			{
				data[startIndex++] = (byte)dataSize;
				dataSize >>= 8;
			}

			// Размер листа
			int listCount = list.Count;
			byte listCountBlock = SizeCalculator.GetMinSize(listCount);

			data[startIndex++] = listCountBlock;

			// 
			for (int j = 0; j < listCountBlock; ++j)
			{
				data[startIndex++] = (byte)listCount;
				listCount >>= 8;
			}

			ListTypeHelper.Write(list, data, ref startIndex);
		}

		private static void WriteList(object value, byte[] data, ref int startIndex)
		{
			if (value == null)
			{
				data[startIndex++] = NullReference;
				return;
			}

			WriteList((IList)value, data, ref startIndex);

			//IList list = ;

			/*
			int dataSize = SizeCalculator.Evaluate(value);
			byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

			// Сколько байт занимает поле dataSize
			data[startIndex++] = dataSizeBlock;

			// 
			for (int i = 0; i < dataSizeBlock; ++i)
			{
				data[startIndex++] = (byte)dataSize;
				dataSize >>= 8;
			}

			// Размер листа
			int listCount = list.Count;
			byte listCountBlock = SizeCalculator.GetMinSize(listCount);

			data[startIndex++] = listCountBlock;

			// 
			for (int j = 0; j < listCountBlock; ++j)
			{
				data[startIndex++] = (byte)listCount;
				listCount >>= 8;
			}

			ListTypeHelper.Write(list, data, ref startIndex);
			*/ 
		}

		private static void WriteClass(object value, byte[] data, ref int startIndex)
		{
			if (value == null)
			{
				data[startIndex++] = NullReference;
				return;
			}

			DataType dataType = DataType.Get(value.GetType());

			int dataSize = SizeCalculator.Evaluate(value);
			byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

			// Сколько байт занимает поле dataSize
			data[startIndex++] = dataSizeBlock;

			// Записываем размер класса
			for (int i = 0; i < dataSizeBlock; ++i)
			{
				data[startIndex++] = (byte)dataSize;
				dataSize >>= 8;
			}

			// Записываем все поля класса
			dataType.WriteValues(value, data, ref startIndex);
		}
	}
}