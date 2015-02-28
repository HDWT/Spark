using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class DictionaryType : IReferenceTypeHelper<IDictionary>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			public ISizeGetter<IDictionary> GetSizeGetter(Type type)
			{
				return new SizeGetter();
			}

			public IDataWriter<IDictionary> GetDataWriter(Type type)
			{
				return new DataWriter();
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

				// Сколько байт занимает поле dictionaryCount
				int dictionaryCountBlock = data[startIndex++];
				int dictionaryCount = 0;

				for (int i = 0; i < dictionaryCountBlock; ++i)
					dictionaryCount += (data[startIndex++] << 8 * i);

				// Создаем новый экземпляр
				DataType dataType = DataType.Get(type);

				Type keyType = type.GetGenericArguments()[0];
				Type valueType = type.GetGenericArguments()[1];

				IDictionary dictionary = (IDictionary)dataType.CreateInstance(dictionaryCount);

				var ReadKeyData = Reader.Get(keyType);
				var ReadValueData = Reader.Get(valueType);

				for (int i = 0; i < dictionaryCount; ++i)
				{
					object key = ReadKeyData(keyType, data, ref startIndex);
					object value = ReadValueData(valueType, data, ref startIndex);

					dictionary.Add(key, value);
				}

				return dictionary;
			}

			public IDictionary Read(byte[] data, ref int startIndex)
			{
				throw new NotImplementedException();
			}
		}
	}
}