using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private class ArrayType : ITypeHelper<Array>
		{
			const int MinDataSize = 1;
			const int NullReference = 0;

			private static readonly Dictionary<Type, Type> s_elementTypeByArrayType = new Dictionary<Type, Type>(16);
			private static readonly Dictionary<Type, GetSizeDelegate<Array>> s_getSizeDelegatesByArrayType = new Dictionary<Type, GetSizeDelegate<Array>>(16);

			private static Type GetElementType(Type arrayType)
			{
				Type elementType = null;

				if (!s_elementTypeByArrayType.TryGetValue(arrayType, out elementType))
				{
					lock (s_elementTypeByArrayType)
					{
						elementType = arrayType.GetElementType();
						s_elementTypeByArrayType[arrayType] = elementType;
					}
				}

				return elementType;
			}

			private GetSizeDelegate<Array> GetSizeGetter(Type arrayType)
			{
				GetSizeDelegate<Array> sizeGetter = null;

				if (!s_getSizeDelegatesByArrayType.TryGetValue(arrayType, out sizeGetter))
				{
					lock (s_getSizeDelegatesByArrayType)
					{
						sizeGetter = GetDataSizeDelegate(arrayType);
						s_getSizeDelegatesByArrayType[arrayType] = sizeGetter;
					}
				}

				return sizeGetter;
			}

			public int GetSize(object array)
			{
				return GetSize((Array)array);
			}

			public int GetSize(Array array)
			{
				if (array == null)
					return MinDataSize;

				int dataSize = MinDataSize + GetDataSize(array) + 1; // +1 byte for Rank - An array can have a maximum of 32 dimensions(MSDN)

				for (int i = 0; i < array.Rank; ++i)
					dataSize += 1 + SizeCalculator.GetMinSize(array.GetLength(i));

				return dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));
			}

			private GetSizeDelegate<Array> GetDataSizeDelegate(Type arrayType)
			{
				Type elementType = GetElementType(arrayType);

				if (elementType.IsValueType)
				{
					if (elementType == typeof(int))
						return GetDataSize<int>;

					if (elementType == typeof(float))
						return GetDataSize<float>;

					if (elementType == typeof(bool))
						return GetDataSize<bool>;

					if (elementType.IsEnum)
						return GetDataSize2;

					if (elementType == typeof(DateTime))
						return GetDataSize<DateTime>;

					if (elementType == typeof(short))
						return GetDataSize<short>;

					if (elementType == typeof(long))
						return GetDataSize<long>;

					if (elementType == typeof(double))
						return GetDataSize<double>;

					if (elementType == typeof(byte))
						return GetDataSize<byte>;

					if (elementType == typeof(char))
						return GetDataSize<char>;

					if (elementType == typeof(uint))
						return GetDataSize<uint>;

					if (elementType == typeof(ushort))
						return GetDataSize<ushort>;

					if (elementType == typeof(ulong))
						return GetDataSize<ulong>;

					if (elementType == typeof(decimal))
						return GetDataSize<decimal>;

					if (elementType == typeof(sbyte))
						return GetDataSize<sbyte>;

					throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
				}
				else
				{
					if (elementType == typeof(string))
						return GetDataSize<string>;

					if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
						return GetDataSize2;

					throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
				}
			}

			private int GetDataSize(Array array)
			{
				Type elementType = GetElementType(array.GetType());

				if (elementType.IsValueType)
				{
					if (elementType == typeof(int))
						return GetDataSize<int>(array);

					if (elementType == typeof(float))
						return GetDataSize<float>(array);

					if (elementType == typeof(bool))
						return GetDataSize<bool>(array);

					if (elementType.IsEnum)
						return GetDataSize2(array);

					if (elementType == typeof(DateTime))
						return GetDataSize<DateTime>(array);

					if (elementType == typeof(short))
						return GetDataSize<short>(array);

					if (elementType == typeof(long))
						return GetDataSize<long>(array);

					if (elementType == typeof(double))
						return GetDataSize<double>(array);

					if (elementType == typeof(byte))
						return GetDataSize<byte>(array);

					if (elementType == typeof(char))
						return GetDataSize<char>(array);

					if (elementType == typeof(uint))
						return GetDataSize<uint>(array);

					if (elementType == typeof(ushort))
						return GetDataSize<ushort>(array);

					if (elementType == typeof(ulong))
						return GetDataSize<ulong>(array);

					if (elementType == typeof(decimal))
						return GetDataSize<decimal>(array);

					if (elementType == typeof(sbyte))
						return GetDataSize<sbyte>(array);

					throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
				}
				else
				{
					if (elementType == typeof(string))
						return GetDataSize<string>(array);

					if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
						return GetDataSize2(array);

					throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
				}
			}

			private static int GetDataSize<T>(Array array)
			{
				int size = 0;

				switch (array.Rank)
				{
					case 1:
						foreach (T item in (T[])array)
							size += SizeCalculator.Evaluate<T>(item);
						return size;

					case 2:
						foreach (T item in (T[,])array)
							size += SizeCalculator.Evaluate<T>(item);
						return size;

					case 3:
						foreach (T item in (T[, ,])array)
							size += SizeCalculator.Evaluate<T>(item);
						return size;

					case 4:
						foreach (T item in (T[, , ,])array)
							size += SizeCalculator.Evaluate<T>(item);
						return size;

					default:
						throw new NotImplementedException(string.Format("Get size for array with rank {0} not implemented", array.Rank));
				}
			}

			private int GetDataSize2(Array array)
			{
				int size = 0;

				Type elementType = GetElementType(array.GetType());

				var GetSize = SizeCalculator.Get(elementType);

				foreach (var item in array)
					size += GetSize(item);

				return size;
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
				Type elementType = GetElementType(type);//.GetElementType();

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

			public void WriteObject(object array, byte[] data, ref int startIndex)
			{
				Write((Array)array, data, ref startIndex);
			}

			public void Write(Array array, byte[] data, ref int startIndex)
			{
				if (array == null)
				{
					data[startIndex++] = NullReference;
					return;
				}

				int dataSize = GetSize(array);
				byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

				// Сколько байт занимает поле dataSize
				data[startIndex++] = dataSizeBlock;

				// 
				for (int i = 0; i < dataSizeBlock; ++i)
				{
					data[startIndex++] = (byte)dataSize;
					dataSize >>= 8;
				}

				// An array can have a maximum of 32 dimensions(MSDN)
				data[startIndex++] = (byte)array.Rank;

				for (int i = 0; i < array.Rank; ++i)
				{
					int arrayLength = array.GetLength(i);
					byte arrayLengthBlock = SizeCalculator.GetMinSize(arrayLength);

					data[startIndex++] = arrayLengthBlock;

					// 
					for (int j = 0; j < arrayLengthBlock; ++j)
					{
						data[startIndex++] = (byte)arrayLength;
						arrayLength >>= 8;
					}
				}

				WriteArrayData(array, data, ref startIndex);
			}

			/// <summary> </summary>
			private void WriteArrayData(Array array, byte[] data, ref int startIndex)
			{
				Type elementType = GetElementType(array.GetType());

				if (elementType.IsValueType)
				{
					if (elementType == typeof(int))
						WriteArrayData<int>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(float))
						WriteArrayData<float>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(bool))
						WriteArrayData<bool>(array, elementType, data, ref startIndex);

					else if (elementType.IsEnum)
						WriteArrayData(array, elementType, data, ref startIndex);

					else if (elementType == typeof(DateTime))
						WriteArrayData<DateTime>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(short))
						WriteArrayData<short>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(long))
						WriteArrayData<long>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(double))
						WriteArrayData<double>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(byte))
						WriteArrayData<byte>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(char))
						WriteArrayData<char>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(uint))
						WriteArrayData<uint>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(ushort))
						WriteArrayData<ushort>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(ulong))
						WriteArrayData<ulong>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(decimal))
						WriteArrayData<decimal>(array, elementType, data, ref startIndex);

					else if (elementType == typeof(sbyte))
						WriteArrayData<sbyte>(array, elementType, data, ref startIndex);

					else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
				}
				else
				{
					if (elementType == typeof(string))
						WriteArrayData<string>(array, elementType, data, ref startIndex);

					else if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
						WriteArrayData(array, elementType, data, ref startIndex);

					else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
				}
			}

			private void WriteArrayData<T>(Array array, Type elementType, byte[] data, ref int startIndex)
			{
				switch (array.Rank)
				{
					case 1:
						foreach (var item in (T[])array)
							Writer.Write<T>(item, data, ref startIndex);
						break;

					case 2:
						foreach (var item in (T[,])array)
							Writer.Write<T>(item, data, ref startIndex);
						break;

					case 3:
						foreach (var item in (T[, ,])array)
							Writer.Write<T>(item, data, ref startIndex);
						break;

					case 4:
						foreach (var item in (T[, , ,])array)
							Writer.Write<T>(item, data, ref startIndex);
						break;

					default:
						throw new NotImplementedException(string.Format("Write array with rank {0} not implemented", array.Rank));
				}
			}

			private void WriteArrayData(Array array, Type elementType, byte[] data, ref int startIndex)
			{
				var WriteData = Writer.Get(elementType);

				foreach (var item in array)
					WriteData(item, data, ref startIndex);
			}
		}
	}
}