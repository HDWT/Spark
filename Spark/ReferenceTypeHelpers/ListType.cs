using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class ListType : IReferenceTypeHelper<IList>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			private static readonly Dictionary<Type, Type> s_elementTypesByListType = new Dictionary<Type, Type>(16);

			public ISizeGetter<IList> GetSizeGetter(Type listType)
			{
				ISizeGetter<IList> sizeGetter = null;

				if (!s_sizeGettersByListType.TryGetValue(listType, out sizeGetter))
				{
					Type elementType = GetElementType(listType);
					TypeFlags typeFlags = GetTypeFlags(elementType);

					sizeGetter = (HasFlag(typeFlags, TypeFlags.Value))
						? new SizeGetter(SizeCalculator.GetForValueType(elementType))
						: new SizeGetter(SizeCalculator.GetForReferenceType(elementType));

					s_sizeGettersByListType[listType] = sizeGetter;
				}

				return sizeGetter;
			}

			public IDataWriter<IList> GetDataWriter(Type listType)
			{
				IDataWriter<IList> dataWriter = null;

				if (!s_dataWritersByListType.TryGetValue(listType, out dataWriter))
				{
					Type elementType = GetElementType(listType);
					TypeFlags typeFlags = GetTypeFlags(elementType);

					dataWriter = (HasFlag(typeFlags, TypeFlags.Value))
						? new DataWriter(Writer.GetDelegateForValueType(elementType))
						: new DataWriter(Writer.GetDelegateForReferenceType(elementType));

					s_dataWritersByListType[listType] = dataWriter;
				}

				return dataWriter;
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
				Type elementType = GetElementType(type);

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
		}
	}
}