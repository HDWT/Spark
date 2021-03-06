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
				public int GetObjectSize(object dictionary, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					return GetSize(dictionary as IDictionary, sizes, values, context);
				}

				public int GetSize(IDictionary dictionary, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					if (dictionary == null)
						return MinDataSize;

					// Get last node before get elements size
					int sizeIndex = sizes.Count;
					sizes.Enqueue(0);

					int dataSize = MinDataSize + 1 + SizeCalculator.GetMinSize(dictionary.Count);

					dataSize += GetElementsSize(dictionary, sizes, values, context);

					int size = dataSize + SizeCalculator.GetMinSize2(dataSize);

					sizes[sizeIndex] = size;

					return size;
				}

				private static int GetElementsSize(IDictionary dictionary, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
				{
					Type keyType = dictionary.GetType().GetGenericArguments()[0];
					TypeFlags keyTypeFlags = GetTypeFlags(keyType);

					Type valueType = dictionary.GetType().GetGenericArguments()[1];
					TypeFlags valueTypeFlags = GetTypeFlags(valueType);

					var getKeySizeAsValueType = HasFlag(keyTypeFlags, TypeFlags.Value) ? SizeCalculator.GetForValueType(keyType) : null;
					var getKeySizeAsReferenceType = (getKeySizeAsValueType == null) ? SizeCalculator.GetForReferenceType(keyType) : null;

					var getValueSizeAsValueType = HasFlag(valueTypeFlags, TypeFlags.Value) ? SizeCalculator.GetForValueType(valueType) : null;
					var getValueSizeAsReferenceType = (getValueSizeAsValueType == null) ? SizeCalculator.GetForReferenceType(valueType) : null;

					int size = 0;

					foreach (DictionaryEntry pair in dictionary)
					{
						size += (getKeySizeAsValueType != null) ? getKeySizeAsValueType(pair.Key) : getKeySizeAsReferenceType(pair.Key, sizes, values, context);
						size += (getValueSizeAsValueType != null) ? getValueSizeAsValueType(pair.Value) : getValueSizeAsReferenceType(pair.Value, sizes, values, context);
					}

					return size;
				}
			}
		}
	}
}