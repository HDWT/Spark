using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		public class ObjectType : ITypeHelper<object>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			private static readonly Dictionary<Type, SizeGetter> s_sizeGettersByType = new Dictionary<Type, SizeGetter>(16);

			private class SizeGetter
			{
				private DataType m_dataType = null;

				public SizeGetter(Type type)
				{
					m_dataType = DataType.Get(type);
				}

				public int GetSize(object value, LinkedList<int> sizes)
				{
					if (value == null)
						return MinDataSize;

					var node = sizes != null ? sizes.Last : null;

					int dataSize = MinDataSize + m_dataType.GetDataSize(value, sizes);
					int size = dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));

					if (sizes != null)
					{
						if (node == null)
							sizes.AddFirst(size);
						else
							sizes.AddAfter(node, size);
						//Console.WriteLine("add object " + size);
					}

					return size;
				}
			}

			public static GetSizeDelegate GetGetSizeDelegate(Type type)
			{
				SizeGetter sizeGetter = null;

				if (!s_sizeGettersByType.TryGetValue(type, out sizeGetter))
				{
					lock (s_sizeGettersByType)
					{
						sizeGetter = new SizeGetter(type);
						s_sizeGettersByType.Add(type, sizeGetter);
					}
				}

				return sizeGetter.GetSize;
			}

			public int GetSize(object value, LinkedList<int> sizes)
			{
				if (value == null)
					return MinDataSize;

				var node = sizes != null ? sizes.Last : null;

				DataType dataType = DataType.Get(value.GetType());

				int dataSize = MinDataSize + dataType.GetDataSize(value, sizes);

				int size = dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));


				if (sizes != null)
				{
					if (node == null)
						sizes.AddFirst(size);
					else
						sizes.AddAfter(node, size);
					//Console.WriteLine("add object " + size);
				}

				return size;
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

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				if (value == null)
				{
					data[startIndex++] = NullReference;
					return;
				}

				DataType dataType = DataType.Get(value.GetType());

				int myDataSize = sizes.First.Value;
				sizes.RemoveFirst();

				int dataSize = myDataSize;// GetSize(value, null); // CHECK HERE !!!

				//if (myDataSize != dataSize)
					//Console.WriteLine("Object size " + myDataSize + " != " + dataSize);

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
				dataType.WriteValues(value, data, ref startIndex, sizes);
			}

			public void Write(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				WriteObject(value, data, ref startIndex, sizes);
			}
		}
	}
}