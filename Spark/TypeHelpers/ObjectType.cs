using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private class ObjectType : ITypeHelper<object>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			public int GetSize(object value)
			{
				if (value == null)
					return MinDataSize;

				DataType dataType = DataType.Get(value.GetType());

				int dataSize = MinDataSize + dataType.GetDataSize(value);

				return dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));
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

				var value = dataType.CreateInstance();

				// Читаем все поля класса
				dataType.ReadValues(value, data, ref startIndex, index + dataSize);
				return value;
			}

			//
			public object Read(byte[] data, ref int startIndex)
			{
				throw new NotImplementedException();
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				if (value == null)
				{
					data[startIndex++] = NullReference;
					return;
				}

				DataType dataType = DataType.Get(value.GetType());

				int dataSize = GetSize(value);
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
				dataType.WriteValues(value, data, ref startIndex);
			}

			public void Write(object value, byte[] data, ref int startIndex)
			{
				WriteObject(value, data, ref startIndex);
			}
		}
	}
}