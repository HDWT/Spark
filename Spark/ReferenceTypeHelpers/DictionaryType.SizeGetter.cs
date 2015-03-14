﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private partial class DictionaryType : IReferenceTypeHelper<IDictionary>
		{
			private class SizeGetter : ISizeGetter<IDictionary>
			{
				public int GetObjectSize(object dictionary, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
				{
					return GetSize(dictionary as IDictionary, sizes, values);
				}

				public int GetSize(IDictionary dictionary, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
				{
					if (dictionary == null)
						return MinDataSize;

					int dataSize = MinDataSize + GetElementsSize(dictionary, sizes, values);

					dataSize += 1 + SizeCalculator.GetMinSize(dictionary.Count);

					return dataSize + SizeCalculator.GetMinSize(dataSize + SizeCalculator.GetMinSize(dataSize));
				}

				private static int GetElementsSize(IDictionary dictionary, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
				{
					Type keyType = dictionary.GetType().GetGenericArguments()[0];
					Type valueType = dictionary.GetType().GetGenericArguments()[1];

					var getKeySizeAsValueType = keyType.IsValueType ? SizeCalculator.GetForValueType(keyType) : null;
					var getKeySizeAsReferenceType = (getKeySizeAsValueType == null) ? SizeCalculator.GetForReferenceType(keyType) : null;

					var getValueSizeAsValueType = valueType.IsValueType ? SizeCalculator.GetForValueType(valueType) : null;
					var getValueSizeAsReferenceType = (getValueSizeAsValueType == null) ? SizeCalculator.GetForReferenceType(valueType) : null;

					int size = 0;

					foreach (DictionaryEntry pair in dictionary)
					{
						size += (getKeySizeAsValueType != null) ? getKeySizeAsValueType(pair.Key) : getKeySizeAsReferenceType(pair.Key, sizes, values);
						size += (getValueSizeAsValueType != null) ? getValueSizeAsValueType(pair.Value) : getValueSizeAsReferenceType(pair.Value, sizes, values);
					}

					return size;
				}
			}
		}
	}
}