using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class ArrayType : IReferenceTypeHelper<Array>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			private static readonly Dictionary<Type, Type> s_elementTypesByArrayType = new Dictionary<Type, Type>(16);

			public ISizeGetter<Array> GetSizeGetter(Type arrayType)
			{
				ISizeGetter<Array> sizeGetter = null;

				if (!s_sizeGettersByArrayType.TryGetValue(arrayType, out sizeGetter))
				{
					Type elementType = GetElementType(arrayType);
					TypeFlags typeFlags = GetTypeFlags(elementType);

					sizeGetter = (HasFlag(typeFlags, TypeFlags.Value))
						? new SizeGetter(SizeCalculator.GetForValueType(elementType))
						: new SizeGetter(SizeCalculator.GetForReferenceType(elementType));

					s_sizeGettersByArrayType[arrayType] = sizeGetter;
				}

				return sizeGetter;
			}

			public IDataWriter<Array> GetDataWriter(Type arrayType)
			{
				IDataWriter<Array> dataWriter = null;

				if (!s_dataWritersByArrayType.TryGetValue(arrayType, out dataWriter))
				{
					Type elementType = GetElementType(arrayType);
					TypeFlags typeFlags = GetTypeFlags(elementType);

					dataWriter = (HasFlag(typeFlags, TypeFlags.Value))
						? new DataWriter(Writer.GetDelegateForValueType(elementType))
						: new DataWriter(Writer.GetDelegateForReferenceType(elementType));

					s_dataWritersByArrayType[arrayType] = dataWriter;
				}

				return dataWriter;
			}

			private static Type GetElementType(Type arrayType)
			{
				Type elementType = null;

				if (!s_elementTypesByArrayType.TryGetValue(arrayType, out elementType))
				{
					elementType = arrayType.GetElementType();
					s_elementTypesByArrayType[arrayType] = elementType;
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

				int rank = data[startIndex++];

				int[] lengths = new int[rank];

				for (int i = 0; i < rank; ++i)
				{
					// Сколько байт занимает поле arrayLength
					byte arrayLengthBlock = data[startIndex++];

					// Читаем длину массива
					int arrayLength = 0;

					for (int j = 0; j < arrayLengthBlock; ++j)
						arrayLength += (data[startIndex++] << 8 * j);

					lengths[i] = arrayLength;
				}

				//
				Type elementType = GetElementType(type);

				int[] indices = new int[rank];

				var array = System.Array.CreateInstance(elementType, lengths);

				var ReadElementData = Reader.Get(elementType);

				if (indices.Length == 1)
				{
					for (int i = 0; i < array.Length; ++i)
					{
						object value = ReadElementData(elementType, data, ref startIndex);
						array.SetValue(value, i);
					}
				}
				else
				{
					int lastIndex = indices.Length - 1;

					for (int i = 0; i < array.Length; ++i)
					{
						object value = ReadElementData(elementType, data, ref startIndex);
						array.SetValue(value, indices);

						indices[lastIndex]++;

						for (int j = lastIndex; j >= 0; --j)
						{
							if ((j == 0) || (indices[j] != lengths[j]))
								continue;

							indices[j] = 0;
							indices[j - 1]++;
						}
					}
				}

				return array;
			}

			public Array Read(byte[] data, ref int startIndex)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}