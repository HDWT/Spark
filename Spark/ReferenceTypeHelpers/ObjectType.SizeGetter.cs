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

				public int GetObjectSize(object instance, QueueWithIndexer sizes)
				{
					if (instance == null)
						return MinDataSize;

					int sizeIndex = sizes.Count;
					sizes.Enqueue(0);

					int dataSize = MinDataSize + m_dataType.GetDataSize(instance, sizes);
					int size = dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));

					sizes[sizeIndex] = size;

					return size;
				}

				public int GetSize(object value, QueueWithIndexer sizes)
				{
					return GetObjectSize(value, sizes);
				}
			}
		}
	}
}