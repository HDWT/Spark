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

				public int GetObjectSize(object instance, LinkedList<int> sizes)
				{
					if (instance == null)
						return MinDataSize;

					var lastNode = sizes.Last;

					int dataSize = MinDataSize + m_dataType.GetDataSize(instance, sizes);
					int size = dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));

					if (lastNode == null)
						sizes.AddFirst(size);
					else
						sizes.AddAfter(lastNode, size);

					return size;
				}

				public int GetSize(object value, LinkedList<int> sizes)
				{
					return GetObjectSize(value, sizes);
				}
			}
		}
	}
}