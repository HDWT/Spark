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

		private static readonly Dictionary<Type, Type> s_elementTypesByListType = new Dictionary<Type, Type>(16);
		private static readonly Dictionary<Type, GetSizeDelegate<IList>> s_getSizeDelegatesByListType = new Dictionary<Type, GetSizeDelegate<IList>>(16);

		public int GetSize(object list, LinkedList<int> sizes)
		{
			return GetSize((IList)list, sizes);
		}

		public int GetSize(IList list, LinkedList<int> sizes)
		{
			if (list == null)
				return MinDataSize;

			var node = sizes != null ? sizes.Last : null;

			int dataSize = MinDataSize + GetDataSize(list, sizes);

			dataSize += 1 + SizeCalculator.GetMinSize(list.Count);

			int size = dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));


			if (sizes != null)
			{
				if (node == null)
					sizes.AddFirst(size);
				else
					sizes.AddAfter(node, size);

				//Console.WriteLine("add list " + size);
			}

			return size;
		}

		private static Type GetElementType(Type listType)
		{
			Type elementType = null;

			if (!s_elementTypesByListType.TryGetValue(listType, out elementType))
			{
				elementType = listType.GetGenericArguments()[0];
				s_elementTypesByListType[listType] = elementType;
			}

			return elementType;
		}

		private GetSizeDelegate<IList> GetDataSizeDelegate(IList list, Type elementType)
		{
			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					return GetDataSize<int>;

				if (elementType == typeof(float))
					return GetDataSize<float>;

				if (elementType == typeof(bool))
					return GetDataSize<bool>;

				if (elementType.IsEnum)
					return GetDataSize2;

				if (elementType == typeof(DateTime))
					return GetDataSize<DateTime>;

				if (elementType == typeof(short))
					return GetDataSize<short>;

				if (elementType == typeof(long))
					return GetDataSize<long>;

				if (elementType == typeof(double))
					return GetDataSize<double>;

				if (elementType == typeof(byte))
					return GetDataSize<byte>;

				if (elementType == typeof(char))
					return GetDataSize<char>;

				if (elementType == typeof(uint))
					return GetDataSize<uint>;

				if (elementType == typeof(ushort))
					return GetDataSize<ushort>;

				if (elementType == typeof(ulong))
					return GetDataSize<ulong>;

				if (elementType == typeof(decimal))
					return GetDataSize<decimal>;

				if (elementType == typeof(sbyte))
					return GetDataSize<sbyte>;

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					return GetDataSize<string>;

				if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
					return GetDataSize2;

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private int GetDataSize(IList list, LinkedList<int> sizes)
		{
			Type listType = list.GetType();
			Type elementType = GetElementType(listType);

			GetSizeDelegate<IList> getSize = null;

			if (!s_getSizeDelegatesByListType.TryGetValue(elementType, out getSize))
			{
				getSize = GetDataSizeDelegate(list, elementType);
				s_getSizeDelegatesByListType[elementType] = getSize;
			}

			return getSize(list, sizes);
		}

		private int GetDataSize<T>(IList list, LinkedList<int> sizes)
		{
			int size = 0;

			List<T> theList = (List<T>)list;

			for (int i = 0; i < theList.Count; ++i)
				size += SizeCalculator.Evaluate<T>(theList[i], sizes);

			return size;
		}

		private int GetDataSize2(IList list, LinkedList<int> sizes)
		{
			int size = 0;

			Type elementType = GetElementType(list.GetType());

			var GetSize = SizeCalculator.Get(elementType);

			for (int i = 0; i < list.Count; ++i)
				size += GetSize(list[i], sizes);

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

		public void WriteObject(object list, byte[] data, ref int startIndex, LinkedList<int> sizes)
		{
			Write((IList)list, data, ref startIndex, sizes);
		}

		public void Write(IList list, byte[] data, ref int startIndex, LinkedList<int> sizes)
		{
			int myDataSize = sizes.First.Value;
			sizes.RemoveFirst();

			int dataSize = myDataSize; //GetSize(list, null); // CHECK HERE !!!

			//if (myDataSize != dataSize)
				//Console.WriteLine("List size " + myDataSize + " != " + dataSize);

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

			WriteListData(list, data, ref startIndex, sizes);
		}

		/// <summary> </summary>
		public static void WriteListData(IList list, byte[] data, ref int startIndex, LinkedList<int> sizes)
		{
			Type elementType = GetElementType(list.GetType());

			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					WriteListData<int>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(float))
					WriteListData<float>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(bool))
					WriteListData<bool>(list, elementType, data, ref startIndex, sizes);

				else if (elementType.IsEnum)
					WriteListData(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(DateTime))
					WriteListData<DateTime>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(short))
					WriteListData<short>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(long))
					WriteListData<long>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(double))
					WriteListData<double>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(byte))
					WriteListData<byte>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(char))
					WriteListData<char>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(uint))
					WriteListData<uint>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(ushort))
					WriteListData<ushort>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(ulong))
					WriteListData<ulong>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(decimal))
					WriteListData<decimal>(list, elementType, data, ref startIndex, sizes);

				else if (elementType == typeof(sbyte))
					WriteListData<sbyte>(list, elementType, data, ref startIndex, sizes);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					WriteListData<string>(list, elementType, data, ref startIndex, sizes);

				else if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
					WriteListData(list, elementType, data, ref startIndex, sizes);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private static void WriteListData<T>(IList list, Type elementType, byte[] data, ref int startIndex, LinkedList<int> sizes)
		{
			List<T> theList = (List<T>)list;

			for (int i = 0; i < theList.Count; ++i)
				Writer.Write<T>(theList[i], data, ref startIndex, sizes);
		}

		private static void WriteListData(IList list, Type elementType, byte[] data, ref int startIndex, LinkedList<int> sizes)
		{
			var WriteData = Writer.Get(elementType);

			for (int i = 0; i < list.Count; ++i)
				WriteData(list[i], data, ref startIndex, sizes);
		}
	}
}