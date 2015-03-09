using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		public partial class ObjectType : IReferenceTypeHelper<object>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			public ISizeGetter<object> GetSizeGetter(Type objectType)
			{
				ISizeGetter<object> sizeGetter = null;

				if (!s_sizeGettersByObjectType.TryGetValue(objectType, out sizeGetter))
				{
					sizeGetter = new SizeGetter(objectType);
					s_sizeGettersByObjectType[objectType] = sizeGetter;
				}

				return sizeGetter;
			}

			public IDataWriter<object> GetDataWriter(Type objectType)
			{
				IDataWriter<object> dataWriter = null;

				if (!s_dataWritersByObjectType.TryGetValue(objectType, out dataWriter))
				{
					dataWriter = new DataWriter(objectType);
					s_dataWritersByObjectType[objectType] = dataWriter;
				}

				return dataWriter;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
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
				object newInstance = null;

				if (dataType.TryCreateInstance(data[startIndex], out newInstance))
					dataType.ReadValues(newInstance, data, ref startIndex, index + dataSize); // Читаем все поля класса
				else
					startIndex = index + dataSize; // Класс с таким ID не найден

				return newInstance;
			}

			//
			public object Read(byte[] data, ref int startIndex)
			{
				throw new NotImplementedException();
			}
		}
	}
}