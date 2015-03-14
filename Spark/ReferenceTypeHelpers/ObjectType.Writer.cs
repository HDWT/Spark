using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		public partial class ObjectType : IReferenceTypeHelper<object>
		{
			private static readonly Dictionary<Type, IDataWriter<object>> s_dataWritersByObjectType = new Dictionary<Type, IDataWriter<object>>(16);

			private class DataWriter : IDataWriter<object>
			{
				private DataType m_dataType = null;

				public DataWriter(Type type)
				{
					m_dataType = DataType.Get(type);
				}

				public void WriteObject(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes)
				{
					if (instance == null)
					{
						data[startIndex++] = NullReference;
						return;
					}

					int dataSize = sizes.Dequeue();
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
					m_dataType.WriteValues(instance, data, ref startIndex, sizes);
				}

				public void Write(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes)
				{
					WriteObject(instance, data, ref startIndex, sizes);
				}
			}
		}
	}
}