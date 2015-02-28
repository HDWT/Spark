using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class DictionaryType : IReferenceTypeHelper<IDictionary>
		{
			private class DataWriter : IDataWriter<IDictionary>
			{
				public void WriteObject(object instance, byte[] data, ref int startIndex, QueueWithIndexer sizes)
				{
					Write(instance as IDictionary, data, ref startIndex, sizes);
				}

				public void Write(IDictionary dictionary, byte[] data, ref int startIndex, QueueWithIndexer sizes)
				{
					if (dictionary == null)
					{
						data[startIndex++] = NullReference;
						return;
					}

					int dataSize = sizes.Dequeue();
					byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

					// Сколько байт занимает поле dataSize
					data[startIndex++] = dataSizeBlock;

					// 
					for (int i = 0; i < dataSizeBlock; ++i)
					{
						data[startIndex++] = (byte)dataSize;
						dataSize >>= 8;
					}

					// Размер словоря
					int dictionaryCount = dictionary.Count;
					byte dictionaryCountBlock = SizeCalculator.GetMinSize(dictionaryCount);

					data[startIndex++] = dictionaryCountBlock;

					// 
					for (int j = 0; j < dictionaryCountBlock; ++j)
					{
						data[startIndex++] = (byte)dictionaryCount;
						dictionaryCount >>= 8;
					}

					Type keyType = dictionary.GetType().GetGenericArguments()[0];
					Type valueType = dictionary.GetType().GetGenericArguments()[1];

					var writeKeySizeAsValueType = keyType.IsValueType ? Writer.GetDelegateForValueType(keyType) : null;
					var writeKeySizeAsReferenceType = (writeKeySizeAsValueType == null) ? Writer.GetDelegateForReferenceType(keyType) : null;

					var writeValueSizeAsValueType = valueType.IsValueType ? Writer.GetDelegateForValueType(valueType) : null;
					var writeValueSizeAsReferenceType = (writeValueSizeAsValueType == null) ? Writer.GetDelegateForReferenceType(valueType) : null;

					foreach (DictionaryEntry pair in dictionary)
					{
						if (writeKeySizeAsValueType != null)
							writeKeySizeAsValueType(pair.Key, data, ref startIndex);
						else
							writeKeySizeAsReferenceType(pair.Key, data, ref startIndex, sizes);

						if (writeValueSizeAsValueType != null)
							writeValueSizeAsValueType(pair.Value, data, ref startIndex);
						else
							writeValueSizeAsReferenceType(pair.Value, data, ref startIndex, sizes);
					}
				}
			}
		}
	}
}