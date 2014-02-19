using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private delegate object ReadDataDelegate(Type type, byte[] data, ref int startIndex);

	private static class Reader
	{
		const byte NullReference = 0;

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

				throw new NotImplementedException();
			}

			if (type == typeof(string))
				return TypeHelper.String.ReadObject;

			if (type.IsArray)
				return TypeHelper.Array.ReadObject;

			if (IsGenericList(type))
				return ReadList;

			if (type.IsClass)
				return ReadClass;

			throw new NotImplementedException("Reader for " + type.Name + " type not implemented");
		}

		/*
		private static object ReadArray(Type type, byte[] data, ref int startIndex)
		{
			// Сколько байт занимает поле dataSize
			byte dataSizeBlock = data[startIndex++];

			if (dataSizeBlock == NullReference)
				return null;

			// Читаем размер данных
			int dataSize = 0;

			for (int i = 0; i < dataSizeBlock; ++i)
				dataSize += (data[startIndex++] << 8 * i);

			int rank = data[startIndex++];

			int[] lengths = new int[rank];

			for (int i = 0; i < rank; ++i)
			{
				// Сколько байт занимает поле arrayLength
				byte arrayLengthBlock = data[startIndex++];

				// Читаем длину массива
				int arrayLength = 0;

				for (int j = 0; j < arrayLengthBlock; ++j)
					arrayLength += (data[startIndex++] << 8 * j);

				lengths[i] = arrayLength;
			}

			//
			Type elementType = type.GetElementType();

			int[] indices = new int[rank];

			var array = Array.CreateInstance(elementType, lengths);

			var ReadElementData = Reader.Get(elementType);

			if (indices.Length == 1)
			{
				for (int i = 0; i < array.Length; ++i)
				{
					object value = ReadElementData(elementType, data, ref startIndex);
					array.SetValue(value, i);
				}
			}
			else
			{
				int lastIndex = indices.Length - 1;

				for (int i = 0; i < array.Length; ++i)
				{
					object value = ReadElementData(elementType, data, ref startIndex);
					array.SetValue(value, indices);

					indices[lastIndex]++;

					for (int j = lastIndex; j >= 0; --j)
					{
						if ((j == 0) || (indices[j] != lengths[j]))
							continue;

						indices[j] = 0;
						indices[j - 1]++;
					}
				}
			}

			return array;
		}*/

		private static object ReadList(Type type, byte[] data, ref int startIndex)
		{
			// Сколько байт занимает поле dataSize
			byte dataSizeBlock = data[startIndex++];

			if (dataSizeBlock == NullReference)
				return null;

			// Читаем размер данных
			int dataSize = 0;

			for (int i = 0; i < dataSizeBlock; ++i)
				dataSize += (data[startIndex++] << 8 * i);

			// Сколько байт занимает поле listCount
			int listCountBlock = data[startIndex++];
			int listCount = 0;

			for (int i = 0; i < listCountBlock; ++i)
				listCount += (data[startIndex++] << 8 * i);

			// Создаем новый экземпляр
			DataType dataType = DataType.Get(type);
			Type elementType = type.GetGenericArguments()[0];

			IList list = (IList)dataType.CreateInstance(listCount);

			var ReadElementData = Reader.Get(elementType);

			for (int i = 0; i < listCount; ++i)
				list.Add(ReadElementData(elementType, data, ref startIndex));

			return list;
		}

		private static object ReadClass(Type type, byte[] data, ref int startIndex)
		{
			int index = startIndex;

			// Сколько байт занимает поле dataSize
			byte dataSizeBlock = data[startIndex++];

			if (dataSizeBlock == NullReference)
				return null;

			// Читаем размер класса
			int dataSize = 0;

			for (int i = 0; i < dataSizeBlock; ++i)
				dataSize += (data[startIndex++] << 8 * i);

			// Создаем новый экземпляр класса
			DataType dataType = DataType.Get(type);

			var value = dataType.CreateInstance();

			// Читаем все поля класса
			dataType.ReadValues(value, data, ref startIndex, index + dataSize);
			return value;
		}
	}
}