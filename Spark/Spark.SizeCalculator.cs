using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private delegate int GetSizeDelegate(object value);
	private delegate int GetSizeDelegate<T>(T value);

	private delegate int GetValueSizeDelegate(object instance);
	private delegate int GetValueSizeDelegate<T>(T instance);

	private delegate int GetReferenceSizeDelegate(object instance, LinkedList<int> sizes);
	private delegate int GetReferenceSizeDelegate<T>(T instance, LinkedList<int> sizes);

	private static class SizeCalculator
	{
		private static readonly Dictionary<Type, GetValueSizeDelegate> s_getValueSizeDelegates = new Dictionary<Type,GetValueSizeDelegate>(16)
		{
			{ typeof(bool),		TypeHelper.Bool.GetSize },
			{ typeof(byte),		TypeHelper.Byte.GetSize },
			{ typeof(sbyte),	TypeHelper.SByte.GetSize },
			{ typeof(char),		TypeHelper.Char.GetSize },
			{ typeof(short),	TypeHelper.Short.GetSize },
			{ typeof(ushort),	TypeHelper.UShort.GetSize },
			{ typeof(int),		TypeHelper.Int.GetSize },
			{ typeof(uint),		TypeHelper.UInt.GetSize },
			{ typeof(float),	TypeHelper.Float.GetSize },
			{ typeof(double),	TypeHelper.Double.GetSize },
			{ typeof(long),		TypeHelper.Long.GetSize },
			{ typeof(ulong),	TypeHelper.ULong.GetSize },
			{ typeof(decimal),	TypeHelper.Decimal.GetSize },
			{ typeof(DateTime), TypeHelper.DateTime.GetSize },
		};

		private static readonly Dictionary<Type, GetReferenceSizeDelegate> s_getReferenceSizeDelegates = new Dictionary<Type, GetReferenceSizeDelegate>(16)
		{
			{ typeof(string), TypeHelper.String.GetSizeGetter(null).GetObjectSize }
		};

		public static GetValueSizeDelegate GetForValueType(Type type)
		{
			GetValueSizeDelegate getSizeDelegate = null;

			if (!s_getValueSizeDelegates.TryGetValue(type, out getSizeDelegate))
			{
				if (type.IsEnum) // BaseType == Enum ?
				{
					getSizeDelegate = EnumTypeHelper.Instance.GetSize;
					s_getValueSizeDelegates[type] = getSizeDelegate;
				}
				else
				{
					throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
				}
			}

			return getSizeDelegate;
		}

		public static GetReferenceSizeDelegate GetForReferenceType(Type type)
		{
			GetReferenceSizeDelegate getSizeDelegate = null;

			if (!s_getReferenceSizeDelegates.TryGetValue(type, out getSizeDelegate))
			{
				if (type.IsArray)
					getSizeDelegate = TypeHelper.Array.GetSizeGetter(type).GetObjectSize;

				else if (IsGenericList(type))
					getSizeDelegate = TypeHelper.List.GetSizeGetter(type).GetObjectSize;

				else if (type.IsClass)
					getSizeDelegate = TypeHelper.Object.GetSizeGetter(type).GetObjectSize;

				else throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));

				s_getReferenceSizeDelegates[type] = getSizeDelegate;
			}

			return getSizeDelegate;
		}

		// Возвращает минимальное количество байт, которое необходимо для записи [dataSize]
		public static byte GetMinSize(int dataSize)
		{
			if (dataSize <= 0xFF)
				return 1;

			if (dataSize <= 0xFFFF)
				return 2;

			if (dataSize <= 0xFFFFFF)
				return 3;

			if (dataSize <= 0x7FFFFFFF)
				return 4;

			throw new ArgumentException("Invalid data size: " + dataSize);
		}
	}
}