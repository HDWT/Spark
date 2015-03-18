using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		public partial class ObjectType : IReferenceTypeHelper<object>
		{
			private static readonly Dictionary<Type, ISizeGetter<object>> s_sizeGettersByObjectType = new Dictionary<Type, ISizeGetter<object>>(16);

			private class SizeGetter : ISizeGetter<object>
			{
				private DataType m_dataType = null;

				public SizeGetter(Type objectType)
				{
					m_dataType = DataType.Get(objectType);
				}

				public int GetObjectSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
				{
					if (instance == null)
						return MinDataSize;

					int sizeIndex = sizes.Count;
					sizes.Enqueue(0);

					int dataSize = MinDataSize + m_dataType.GetDataSize(instance, sizes, values);
					int size = dataSize + SizeCalculator.GetMinSize2(dataSize);

					sizes[sizeIndex] = size;

					return size;
				}

				public int GetSize(object value, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
				{
					return GetObjectSize(value, sizes, values);
				}
			}
		}
	}
}