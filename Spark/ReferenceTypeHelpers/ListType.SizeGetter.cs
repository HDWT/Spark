using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class ListType : IReferenceTypeHelper<IList>
		{
			private static readonly Dictionary<Type, ISizeGetter<IList>> s_sizeGettersByListType = new Dictionary<Type, ISizeGetter<IList>>(20)
			{
				{ typeof(List<bool>),		new SizeGetter<bool>	(BoolType.GetSize) },
				{ typeof(List<byte>),		new SizeGetter<byte>	(ByteType.GetSize) },
				{ typeof(List<sbyte>),		new SizeGetter<sbyte>	(SByteType.GetSize) },
				{ typeof(List<char>),		new SizeGetter<char>	(CharType.GetSize) },
				{ typeof(List<short>),		new SizeGetter<short>	(ShortType.GetSize) },
				{ typeof(List<ushort>),		new SizeGetter<ushort>	(UShortType.GetSize) },
				{ typeof(List<int>),		new SizeGetter<int>		(IntType.GetSize) },
				{ typeof(List<uint>),		new SizeGetter<uint>	(UIntType.GetSize) },
				{ typeof(List<float>),		new SizeGetter<float>	(FloatType.GetSize) },
				{ typeof(List<double>),		new SizeGetter<double>	(DoubleType.GetSize) },
				{ typeof(List<long>),		new SizeGetter<long>	(LongType.GetSize) },
				{ typeof(List<ulong>),		new SizeGetter<ulong>	(ULongType.GetSize) },
				{ typeof(List<decimal>),	new SizeGetter<decimal>	(DecimalType.GetSize) },
				{ typeof(List<DateTime>),	new SizeGetter<DateTime>(DateTimeType.GetSize) },

				//{ typeof(List<string>),		new SizeGetter<string>	(TypeHelper.String.GetSizeGetter(null).GetSize) },
			};

			//
			private abstract class SizeGetterBase : ISizeGetter<IList>
			{
				private bool m_isValueType = false;

				public SizeGetterBase(bool isValueType)
				{
					m_isValueType = isValueType;
				}

				public int GetObjectSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					return GetSize(instance as IList, sizes, values, context);
				}

				public int GetSize(IList list, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					if (list == null)
						return MinDataSize;

					// Get last node before get elements size
					int sizeIndex = sizes.Count;
					sizes.Enqueue(0);

					int dataSize = MinDataSize + 1 + SizeCalculator.GetMinSize(list.Count);

					dataSize += (m_isValueType)
						? GetElementsSize(list)
						: GetElementsSize(list, sizes, values, context);

					int size = dataSize + SizeCalculator.GetMinSize2(dataSize);

					sizes[sizeIndex] = size;

					return size;
				}

				protected abstract int GetElementsSize(IList list);
				protected abstract int GetElementsSize(IList list, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context);
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

				protected override int GetElementsSize(IList list)
				{
					int size = 0;

					for (int i = 0; i < list.Count; ++i)
						size += m_getValueSize(list[i]);

					return size;
				}

				protected override int GetElementsSize(IList list, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					int size = 0;

					for (int i = 0; i < list.Count; ++i)
						size += m_getReferenceSize(list[i], sizes, values, context);

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

				protected override int GetElementsSize(IList list)
				{
					List<T> aList = (List<T>)list;
					int size = 0;

					for (int i = 0; i < aList.Count; ++i)
						size += m_getValueSize(aList[i]);

					return size;
				}

				protected override int GetElementsSize(IList list, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					List<T> aList = (List<T>)list;
					int size = 0;

					for (int i = 0; i < aList.Count; ++i)
						size += m_getReferenceSize(aList[i], sizes, values, context);

					return size;
				}
			}
		}
	}
}