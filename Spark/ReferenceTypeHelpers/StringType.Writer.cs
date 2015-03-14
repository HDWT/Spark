using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class StringType : IReferenceTypeHelper<string>
		{
			private class DataWriter : IDataWriter<string>
			{
				public void WriteObject(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
				{
					Write(instance as string, data, ref startIndex, sizes, values);
				}

				public void Write(string aString, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
				{
					if (aString == null)
					{
						data[startIndex++] = NullReference;
						return;
					}

					int dataSize = s_sizeGetter.GetSize(aString, sizes, values);
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
					for (int i = 0; i < aString.Length; ++i)
					{
						mapper.value = aString[i];

						data[startIndex++] = mapper.byte1;
						data[startIndex++] = mapper.byte2;
					}

					if (!forwardPadding)
						data[startIndex++] = PaddingValue;
				}
			}
		}
	}
}