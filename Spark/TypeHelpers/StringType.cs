using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private class StringType : ITypeHelper<string>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			const byte ForwardPaddingMark = 128;
			const byte PaddingValue = 0;

			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((string)value, sizes);
			}

			public int GetSize(string value, LinkedList<int> sizes)
			{
				if (value == null)
					return MinDataSize;

				int dataSize = MinDataSize + value.Length * sizeof(char) + 1; // +1 for padding

				int size = dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));


				if (sizes != null)
				{
					//Console.WriteLine("add string " + size);
					sizes.AddLast(size);
				}

				return size;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public string Read(byte[] data, ref int startIndex)
			{
				int index = startIndex;

				// Сколько байт занимает поле dataSize
				byte dataSizeBlock = data[startIndex++];

				if (dataSizeBlock == NullReference)
					return null;

				bool forwardPadding = false;

				if (dataSizeBlock >= ForwardPaddingMark)
				{
					dataSizeBlock -= ForwardPaddingMark;
					forwardPadding = true;
				}

				// Читаем длину строки
				int dataSize = 0;

				for (int i = 0; i < dataSizeBlock; ++i)
					dataSize += (data[startIndex++] << 8 * i);

				if (forwardPadding)
					startIndex++;

				// Длина строки
				int stringLength = (dataSize - 1 - dataSizeBlock - MinDataSize) / sizeof(char);

				TypeHelper.ArrayMapper mapper = new TypeHelper.ArrayMapper();
				mapper.byteArray = data;

				string str = new string(mapper.charArray, startIndex / 2, stringLength);
				startIndex += stringLength * sizeof(char);

				if (!forwardPadding)
					startIndex++;

				if (startIndex != index + dataSize)
				{
					//Console.WriteLine("Index error: " + startIndex + " / " + (index + dataSize));
					startIndex = index + dataSize;
				}

				return str;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((string)value, data, ref startIndex, sizes);
			}

			public void Write(string value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				if (value == null)
				{
					data[startIndex++] = NullReference;
					return;
				}

				int myDataSize = sizes.First.Value;
				sizes.RemoveFirst();

				int dataSize = myDataSize;// SizeCalculator.Evaluate(value, null);

				//if (myDataSize != dataSize)
					//Console.WriteLine("String size " + myDataSize + " != " + dataSize);

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
		}
	}
}