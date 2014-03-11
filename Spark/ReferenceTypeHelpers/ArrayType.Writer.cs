using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class ArrayType : IReferenceTypeHelper<Array>
		{
			private static readonly Dictionary<Type, IDataWriter<Array>> s_dataWritersByArrayType = new Dictionary<Type, IDataWriter<Array>>(20)
			{
				{ typeof(bool[]),		new DataWriter<bool>	(TypeHelper.Bool.Write) },
				{ typeof(byte[]),		new DataWriter<byte>	(TypeHelper.Byte.Write) },
				{ typeof(sbyte[]),		new DataWriter<sbyte>	(TypeHelper.SByte.Write) },
				{ typeof(char[]),		new DataWriter<char>	(TypeHelper.Char.Write) },
				{ typeof(short[]),		new DataWriter<short>	(TypeHelper.Short.Write) },
				{ typeof(ushort[]),		new DataWriter<ushort>	(TypeHelper.UShort.Write) },
				{ typeof(int[]),		new DataWriter<int>		(TypeHelper.Int.Write) },
				{ typeof(uint[]),		new DataWriter<uint>	(TypeHelper.UInt.Write) },
				{ typeof(float[]),		new DataWriter<float>	(TypeHelper.Float.Write) },
				{ typeof(double[]),		new DataWriter<double>	(TypeHelper.Double.Write) },
				{ typeof(long[]),		new DataWriter<long>	(TypeHelper.Long.Write) },
				{ typeof(ulong[]),		new DataWriter<ulong>	(TypeHelper.ULong.Write) },
				{ typeof(decimal[]),	new DataWriter<decimal>	(TypeHelper.Decimal.Write) },
				{ typeof(DateTime[]),	new DataWriter<DateTime>(TypeHelper.DateTime.Write) },

				{ typeof(string[]),		new DataWriter<string>	(TypeHelper.String.GetDataWriter(null).Write) },
			};

			//
			private abstract class DataWriterBase : IDataWriter<Array>
			{
				private bool m_isValueType = false;

				public DataWriterBase(bool isValueType)
				{
					m_isValueType = isValueType;
				}

				public void WriteObject(object instance, byte[] data, ref int startIndex, LinkedList<int> sizes)
				{
					Write(instance as Array, data, ref startIndex, sizes);
				}

				public void Write(Array array, byte[] data, ref int startIndex, LinkedList<int> sizes)
				{
					if (array == null)
					{
						data[startIndex++] = NullReference;
						return;
					}

					int dataSize = sizes.First.Value;
					sizes.RemoveFirst();

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

					if (m_isValueType)
						WriteValue(array, data, ref startIndex);
					else
						WriteReference(array, data, ref startIndex, sizes);
				}

				protected abstract void WriteValue(Array array, byte[] data, ref int startIndex);
				protected abstract void WriteReference(Array array, byte[] data, ref int startIndex, LinkedList<int> sizes);
			}

			//
			private class DataWriter : DataWriterBase
			{
				private WriteValueDelegate m_writeValue = null;
				private WriteReferenceDelegate m_writeReference = null;

				public DataWriter(WriteValueDelegate writeValue)
					: base(true)
				{
					m_writeValue = writeValue;
				}

				public DataWriter(WriteReferenceDelegate writeReference)
					: base(false)
				{
					m_writeReference = writeReference;
				}

				protected override void WriteValue(Array array, byte[] data, ref int startIndex)
				{
					foreach (var item in array)
						m_writeValue(item, data, ref startIndex);
				}

				protected override void WriteReference(Array array, byte[] data, ref int startIndex, LinkedList<int> sizes)
				{
					foreach (var item in array)
						m_writeReference(item, data, ref startIndex, sizes);
				}
			}

			//
			private class DataWriter<T> : DataWriterBase
			{
				private WriteValueDelegate<T> m_writeValue = null;
				private WriteReferenceDelegate<T> m_writeReference = null;

				public DataWriter(WriteValueDelegate<T> writeValue)
					: base(true)
				{
					m_writeValue = writeValue;
				}

				public DataWriter(WriteReferenceDelegate<T> writeReference)
					: base(false)
				{
					m_writeReference = writeReference;
				}

				protected override void WriteValue(Array array, byte[] data, ref int startIndex)
				{
					switch (array.Rank)
					{
						case 1:
							foreach (var item in (T[])array)
								m_writeValue(item, data, ref startIndex);
							break;

						case 2:
							foreach (var item in (T[,])array)
								m_writeValue(item, data, ref startIndex);
							break;

						case 3:
							foreach (var item in (T[, ,])array)
								m_writeValue(item, data, ref startIndex);
							break;

						case 4:
							foreach (var item in (T[, , ,])array)
								m_writeValue(item, data, ref startIndex);
							break;

						default:
							throw new NotImplementedException(string.Format("Write array with rank {0} not implemented", array.Rank));
					}
				}

				protected override void WriteReference(Array array, byte[] data, ref int startIndex, LinkedList<int> sizes)
				{
					switch (array.Rank)
					{
						case 1:
							foreach (var item in (T[])array)
								m_writeReference(item, data, ref startIndex, sizes);
							break;

						case 2:
							foreach (var item in (T[,])array)
								m_writeReference(item, data, ref startIndex, sizes);
							break;

						case 3:
							foreach (var item in (T[, ,])array)
								m_writeReference(item, data, ref startIndex, sizes);
							break;

						case 4:
							foreach (var item in (T[, , ,])array)
								m_writeReference(item, data, ref startIndex, sizes);
							break;

						default:
							throw new NotImplementedException(string.Format("Write array with rank {0} not implemented", array.Rank));
					}
				}
			}
		}
	}
}