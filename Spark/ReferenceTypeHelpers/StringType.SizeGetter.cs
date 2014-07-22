using System;
using System.Collections.Generic;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class StringType : IReferenceTypeHelper<string>
		{
			private class SizeGetter : ISizeGetter<string>
			{
				public int GetObjectSize(object instance, QueueWithIndexer sizes)
				{
					return GetSize(instance as string, sizes);
				}

				public int GetSize(string aString, QueueWithIndexer sizes)
				{
					if (aString == null)
						return MinDataSize;

					int dataSize = MinDataSize + aString.Length * sizeof(char) + 1; // +1 for padding
					int size = dataSize + SizeCalculator.GetMinSize2(dataSize);

					return size; // dataSize + SizeCalculator.GetMinSize2(dataSize);// SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));
				}
			}
		}
	}
}