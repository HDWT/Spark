using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class ListType : IReferenceTypeHelper<IList>
		{
			private static readonly Dictionary<Type, IDataWriter<IList>> s_dataWritersByListType = new Dictionary<Type, IDataWriter<IList>>(20)
			{
				{ typeof(List<bool>),		new DataWriter<bool>	(TypeHelper.Bool.Write) },
				{ typeof(List<byte>),		new DataWriter<byte>	(TypeHelper.Byte.Write) },
				{ typeof(List<sbyte>),		new DataWriter<sbyte>	(TypeHelper.SByte.Write) },
				{ typeof(List<char>),		new DataWriter<char>	(TypeHelper.Char.Write) },
				{ typeof(List<short>),		new DataWriter<short>	(TypeHelper.Short.Write) },
				{ typeof(List<ushort>),		new DataWriter<ushort>	(TypeHelper.UShort.Write) },
				{ typeof(List<int>),		new DataWriter<int>		(IntType.Write) },
				{ typeof(List<uint>),		new DataWriter<uint>	(TypeHelper.UInt.Write) },
				{ typeof(List<float>),		new DataWriter<float>	(TypeHelper.Float.Write) },
				{ typeof(List<double>),		new DataWriter<double>	(TypeHelper.Double.Write) },
				{ typeof(List<long>),		new DataWriter<long>	(TypeHelper.Long.Write) },
				{ typeof(List<ulong>),		new DataWriter<ulong>	(TypeHelper.ULong.Write) },
				{ typeof(List<decimal>),	new DataWriter<decimal>	(TypeHelper.Decimal.Write) },
				{ typeof(List<DateTime>),	new DataWriter<DateTime>(TypeHelper.DateTime.Write) },

				{ typeof(List<string>),		new DataWriter<string>	(TypeHelper.String.GetDataWriter(null).Write) },
			};

			//
			private abstract class DataWriterBase : IDataWriter<IList>
			{
				private bool m_isValueType = false;

				public DataWriterBase(bool isValueType)
				{
					m_isValueType = isValueType;
				}

				public void WriteObject(object instance, byte[] data, ref int startIndex, QueueWithIndexer sizes)
				{
					Write(instance as IList, data, ref startIndex, sizes);
				}

				public void Write(IList list, byte[] data, ref int startIndex, QueueWithIndexer sizes)
				{
					if (list == null)
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

					// Размер листа
					int listCount = list.Count;
					byte listCountBlock = SizeCalculator.GetMinSize(listCount);

					data[startIndex++] = listCountBlock;

					// 
					for (int j = 0; j < listCountBlock; ++j)
					{
						data[startIndex++] = (byte)listCount;
						listCount >>= 8;
					}

					if (m_isValueType)
						WriteValue(list, data, ref startIndex);
					else
						WriteReference(list, data, ref startIndex, sizes);
				}

				protected abstract void WriteValue(IList list, byte[] data, ref int startIndex);
				protected abstract void WriteReference(IList list, byte[] data, ref int startIndex, QueueWithIndexer sizes);
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

				protected override void WriteValue(IList list, byte[] data, ref int startIndex)
				{
					for (int i = 0; i < list.Count; ++i)
						m_writeValue(list[i], data, ref startIndex);
				}

				protected override void WriteReference(IList list, byte[] data, ref int startIndex, QueueWithIndexer sizes)
				{
					for (int i = 0; i < list.Count; ++i)
						m_writeReference(list[i], data, ref startIndex, sizes);
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

				protected override void WriteValue(IList list, byte[] data, ref int startIndex)
				{
					List<T> theList = (List<T>)list;

					for (int i = 0; i < theList.Count; ++i)
						m_writeValue(theList[i], data, ref startIndex);
				}

				protected override void WriteReference(IList list, byte[] data, ref int startIndex, QueueWithIndexer sizes)
				{
					List<T> theList = (List<T>)list;

					for (int i = 0; i < theList.Count; ++i)
						m_writeReference(theList[i], data, ref startIndex, sizes);
				}
			}
		}
	}
}