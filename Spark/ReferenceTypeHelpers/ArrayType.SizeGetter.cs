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
				{ typeof(bool[]),		new SizeGetter<bool>	(TypeHelper.Bool.GetSize) },
				{ typeof(byte[]),		new SizeGetter<byte>	(TypeHelper.Byte.GetSize) },
				{ typeof(sbyte[]),		new SizeGetter<sbyte>	(TypeHelper.SByte.GetSize) },
				{ typeof(char[]),		new SizeGetter<char>	(TypeHelper.Char.GetSize) },
				{ typeof(short[]),		new SizeGetter<short>	(TypeHelper.Short.GetSize) },
				{ typeof(ushort[]),		new SizeGetter<ushort>	(TypeHelper.UShort.GetSize) },
				{ typeof(int[]),		new SizeGetter<int>		(TypeHelper.Int.GetSize) },
				{ typeof(uint[]),		new SizeGetter<uint>	(TypeHelper.UInt.GetSize) },
				{ typeof(float[]),		new SizeGetter<float>	(TypeHelper.Float.GetSize) },
				{ typeof(double[]),		new SizeGetter<double>	(TypeHelper.Double.GetSize) },
				{ typeof(long[]),		new SizeGetter<long>	(TypeHelper.Long.GetSize) },
				{ typeof(ulong[]),		new SizeGetter<ulong>	(TypeHelper.ULong.GetSize) },
				{ typeof(decimal[]),	new SizeGetter<decimal>	(TypeHelper.Decimal.GetSize) },
				{ typeof(DateTime[]),	new SizeGetter<DateTime>(TypeHelper.DateTime.GetSize) },

				{ typeof(string[]),		new SizeGetter<string>	(TypeHelper.String.GetSizeGetter(null).GetSize) },
			};

			//
			private abstract class SizeGetterBase : ISizeGetter<Array>
			{
				private bool m_isValueType = false;

				public SizeGetterBase(bool isValueType)
				{
					m_isValueType = isValueType;
				}

				public int GetObjectSize(object instance, LinkedList<int> sizes)
				{
					return GetSize(instance as Array, sizes);
				}

				public int GetSize(Array array, LinkedList<int> sizes)
				{
					if (array == null)
						return MinDataSize;

					// Get last node before get elements size
					var lastNode = sizes.Last;

					int dataSize = MinDataSize + 1; // +1 byte for Rank - An array can have a maximum of 32 dimensions(MSDN)

					dataSize += (m_isValueType)
						? GetElementsSize(array)
						: GetElementsSize(array, sizes);

					for (int i = 0; i < array.Rank; ++i)
						dataSize += 1 + SizeCalculator.GetMinSize(array.GetLength(i));

					int size = dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));

					if (lastNode == null)
						sizes.AddFirst(size);
					else
						sizes.AddAfter(lastNode, size);

					return size;
				}

				protected abstract int GetElementsSize(Array array);
				protected abstract int GetElementsSize(Array array, LinkedList<int> sizes);
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

				protected override int GetElementsSize(Array array, LinkedList<int> sizes)
				{
					int size = 0;

					foreach (var item in array)
						size += m_getReferenceSize(item, sizes);

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

				protected override int GetElementsSize(Array array, LinkedList<int> sizes)
				{
					int size = 0;

					switch (array.Rank)
					{
						case 1:
							foreach (T item in (T[])array)
								size += m_getReferenceSize(item, sizes);
							break;

						case 2:
							foreach (T item in (T[,])array)
								size += m_getReferenceSize(item, sizes);
							break;

						case 3:
							foreach (T item in (T[, ,])array)
								size += m_getReferenceSize(item, sizes);
							break;

						case 4:
							foreach (T item in (T[, , ,])array)
								size += m_getReferenceSize(item, sizes);
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