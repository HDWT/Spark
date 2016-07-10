using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class ArrayType : IReferenceTypeHelper<Array>
		{
			private static readonly Dictionary<Type, ISizeGetter<Array>> s_sizeGettersByArrayType = new Dictionary<Type, ISizeGetter<Array>>(20)
			{
				{ typeof(bool[]),		new SizeGetter<bool>	(BoolType.GetSize) },
				{ typeof(byte[]),		new SizeGetter<byte>	(ByteType.GetSize) },
				{ typeof(sbyte[]),		new SizeGetter<sbyte>	(SByteType.GetSize) },
				{ typeof(char[]),		new SizeGetter<char>	(CharType.GetSize) },
				{ typeof(short[]),		new SizeGetter<short>	(ShortType.GetSize) },
				{ typeof(ushort[]),		new SizeGetter<ushort>	(UShortType.GetSize) },
				{ typeof(int[]),		new SizeGetter<int>		(IntType.GetSize) },
				{ typeof(uint[]),		new SizeGetter<uint>	(UIntType.GetSize) },
				{ typeof(float[]),		new SizeGetter<float>	(FloatType.GetSize) },
				{ typeof(double[]),		new SizeGetter<double>	(DoubleType.GetSize) },
				{ typeof(long[]),		new SizeGetter<long>	(LongType.GetSize) },
				{ typeof(ulong[]),		new SizeGetter<ulong>	(ULongType.GetSize) },
				{ typeof(decimal[]),	new SizeGetter<decimal>	(DecimalType.GetSize) },
				{ typeof(DateTime[]),	new SizeGetter<DateTime>(DateTimeType.GetSize) },

				//{ typeof(string[]),		new SizeGetter<string>	(TypeHelper.String.GetSizeGetter(null).GetSize) },
			};

			//
			private abstract class SizeGetterBase : ISizeGetter<Array>
			{
				private bool m_isValueType = false;

				public SizeGetterBase(bool isValueType)
				{
					m_isValueType = isValueType;
				}

				public int GetObjectSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					return GetSize(instance as Array, sizes, values, context);
				}

				public int GetSize(Array array, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					if (array == null)
						return MinDataSize;

					// Get last node before get elements size
					int sizeIndex = sizes.Count;
					sizes.Enqueue(0);

					int dataSize = MinDataSize + 1; // +1 byte for Rank - An array can have a maximum of 32 dimensions(MSDN)

					dataSize += (m_isValueType)
						? GetElementsSize(array)
						: GetElementsSize(array, sizes, values, context);

					for (int i = 0; i < array.Rank; ++i)
						dataSize += 1 + SizeCalculator.GetMinSize(array.GetLength(i));

					int size = dataSize + SizeCalculator.GetMinSize2(dataSize);

					sizes[sizeIndex] = size;

					return size;
				}

				protected abstract int GetElementsSize(Array array);
				protected abstract int GetElementsSize(Array array, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context);
			}

			//
			private class SizeGetter : SizeGetterBase
			{
				private GetValueSizeDelegate m_getValueSize = null;
				private GetReferenceSizeDelegate m_getReferenceSize = null;

				public SizeGetter(GetValueSizeDelegate getValueSize)
					: base(true)
				{
					m_getValueSize = getValueSize;
				}

				public SizeGetter(GetReferenceSizeDelegate getReferenceValueSize)
					: base(false)
				{
					m_getReferenceSize = getReferenceValueSize;
				}

				protected override int GetElementsSize(Array array)
				{
					int size = 0;

					foreach (var item in array)
						size += m_getValueSize(item);

					return size;
				}

				protected override int GetElementsSize(Array array, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					int size = 0;

					foreach (var item in array)
						size += m_getReferenceSize(item, sizes, values, context);

					return size;
				}
			}

			//
			private class SizeGetter<T> : SizeGetterBase
			{
				private GetValueSizeDelegate<T> m_getValueSize = null;
				private GetReferenceSizeDelegate<T> m_getReferenceSize = null;

				public SizeGetter(GetValueSizeDelegate<T> getValueSize)
					: base(true)
				{
					m_getValueSize = getValueSize;
				}

				public SizeGetter(GetReferenceSizeDelegate<T> getReferenceValueSize)
					: base(false)
				{
					m_getReferenceSize = getReferenceValueSize;
				}

				protected override int GetElementsSize(Array array)
				{
					int size = 0;

					switch (array.Rank)
					{
						case 1:
							foreach (T item in (T[])array)
								size += m_getValueSize(item);
							break;

						case 2:
							foreach (T item in (T[,])array)
								size += m_getValueSize(item);
							break;

						case 3:
							foreach (T item in (T[, ,])array)
								size += m_getValueSize(item);
							break;

						case 4:
							foreach (T item in (T[, , ,])array)
								size += m_getValueSize(item);
							break;

						default:
							throw new NotImplementedException(string.Format("Get size for array with rank {0} not implemented", array.Rank));
					}

					return size;
				}

				protected override int GetElementsSize(Array array, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					int size = 0;

					switch (array.Rank)
					{
						case 1:
							foreach (T item in (T[])array)
								size += m_getReferenceSize(item, sizes, values, context);
							break;

						case 2:
							foreach (T item in (T[,])array)
								size += m_getReferenceSize(item, sizes, values, context);
							break;

						case 3:
							foreach (T item in (T[, ,])array)
								size += m_getReferenceSize(item, sizes, values, context);
							break;

						case 4:
							foreach (T item in (T[, , ,])array)
								size += m_getReferenceSize(item, sizes, values, context);
							break;

						default:
							throw new NotImplementedException(string.Format("Get size for array with rank {0} not implemented", array.Rank));
					}

					return size;
				}
			}
		}
	}
}