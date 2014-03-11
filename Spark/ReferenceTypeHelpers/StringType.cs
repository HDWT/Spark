using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class StringType : IReferenceTypeHelper<string>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			const byte ForwardPaddingMark = 128;
			const byte PaddingValue = 0;

			private static readonly SizeGetter s_sizeGetter = new SizeGetter();
			private static readonly DataWriter s_dataWriter = new DataWriter();

			public ISizeGetter<string> GetSizeGetter(Type nullRef)
			{
				return s_sizeGetter;
			}

			public IDataWriter<string> GetDataWriter(Type nullRef)
			{
				return s_dataWriter;
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
		}
	}
}