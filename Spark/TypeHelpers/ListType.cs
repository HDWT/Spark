using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private class ListType : ITypeHelper<IList>
	{
		const int MinDataSize = 1;
		const int NullReference = 0;

		public int GetSize(object list)
		{
			return GetSize((IList)list);
		}

		public int GetSize(IList list)
		{
			if (list == null)
				return MinDataSize;

			int dataSize = MinDataSize + GetDataSize(list);

			dataSize += 1 + SizeCalculator.GetMinSize(list.Count);

			return dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));
		}

		private int GetDataSize(IList list)
		{
			Type elementType = list.GetType().GetGenericArguments()[0];

			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					return GetDataSize<int>(list);

				if (elementType == typeof(float))
					return GetDataSize<float>(list);

				if (elementType == typeof(bool))
					return GetDataSize<bool>(list);

				if (elementType.IsEnum)
					return GetDataSize(list, elementType);

				if (elementType == typeof(DateTime))
					return GetDataSize<DateTime>(list);

				if (elementType == typeof(short))
					return GetDataSize<short>(list);

				if (elementType == typeof(long))
					return GetDataSize<long>(list);

				if (elementType == typeof(double))
					return GetDataSize<double>(list);

				if (elementType == typeof(byte))
					return GetDataSize<byte>(list);

				if (elementType == typeof(char))
					return GetDataSize<char>(list);

				if (elementType == typeof(uint))
					return GetDataSize<uint>(list);

				if (elementType == typeof(ushort))
					return GetDataSize<ushort>(list);

				if (elementType == typeof(ulong))
					return GetDataSize<ulong>(list);

				if (elementType == typeof(decimal))
					return GetDataSize<decimal>(list);

				if (elementType == typeof(sbyte))
					return GetDataSize<sbyte>(list);

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					return GetDataSize<string>(list);

				if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
					return GetDataSize(list, elementType);

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private int GetDataSize<T>(IList list)
		{
			int size = 0;

			foreach (T item in (List<T>)list)
				size += SizeCalculator.Evaluate<T>(item);

			return size;
		}

		private int GetDataSize(IList list, Type elementType)
		{
			int size = 0;

			var GetSize = SizeCalculator.Get(elementType);

			foreach (var item in list)
				size += GetSize(item);

			return size;
		}

		public object ReadObject(Type type, byte[] data, ref int startIndex)
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

		public IList Read(byte[] data, ref int startIndex)
		{
			throw new NotImplementedException();
		}

		public void WriteObject(object list, byte[] data, ref int startIndex)
		{
			Write((IList)list, data, ref startIndex);
		}

		public void Write(IList list, byte[] data, ref int startIndex)
		{
			int dataSize = GetSize(list);// SizeCalculator.Evaluate(value);
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

			WriteListData(list, data, ref startIndex);
		}

		/// <summary> </summary>
		public static void WriteListData(IList list, byte[] data, ref int startIndex)
		{
			Type elementType = list.GetType().GetGenericArguments()[0];

			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					WriteListData<int>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(float))
					WriteListData<float>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(bool))
					WriteListData<bool>(list, elementType, data, ref startIndex);

				else if (elementType.IsEnum)
					WriteListData(list, elementType, data, ref startIndex);

				else if (elementType == typeof(DateTime))
					WriteListData<DateTime>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(short))
					WriteListData<short>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(long))
					WriteListData<long>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(double))
					WriteListData<double>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(byte))
					WriteListData<byte>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(char))
					WriteListData<char>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(uint))
					WriteListData<uint>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(ushort))
					WriteListData<ushort>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(ulong))
					WriteListData<ulong>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(decimal))
					WriteListData<decimal>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(sbyte))
					WriteListData<sbyte>(list, elementType, data, ref startIndex);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					WriteListData<string>(list, elementType, data, ref startIndex);

				else if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
					WriteListData(list, elementType, data, ref startIndex);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private static void WriteListData<T>(IList list, Type elementType, byte[] data, ref int startIndex)
		{
			foreach (T item in (List<T>)list)
				Writer.Write<T>(item, data, ref startIndex);
		}

		private static void WriteListData(IList list, Type elementType, byte[] data, ref int startIndex)
		{
			var WriteData = Writer.Get(elementType);

			foreach (var item in list)
				WriteData(item, data, ref startIndex);
		}
	}
}