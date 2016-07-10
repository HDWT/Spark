using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public static partial class Spark
{
	private delegate int GetSizeDelegate(object value);
	private delegate int GetSizeDelegate<T>(T value);

	private delegate int GetValueSizeDelegate(object instance);
	private delegate int GetValueSizeDelegate<T>(T instance);

	private delegate int GetReferenceSizeDelegate(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context);
	private delegate int GetReferenceSizeDelegate<T>(T instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context);

	private static class SizeCalculator
	{
		private static readonly Dictionary<Type, GetValueSizeDelegate> s_getValueSizeDelegates = new Dictionary<Type,GetValueSizeDelegate>(16)
		{
			{ typeof(bool),		TypeHelper.BoolType.GetSize },
			{ typeof(byte),		TypeHelper.ByteType.GetSize },
			{ typeof(sbyte),	TypeHelper.SByteType.GetSize },
			{ typeof(char),		TypeHelper.CharType.GetSize },
			{ typeof(short),	TypeHelper.ShortType.GetSize },
			{ typeof(ushort),	TypeHelper.UShortType.GetSize },
			{ typeof(int),		TypeHelper.IntType.GetSize },
			{ typeof(uint),		TypeHelper.UIntType.GetSize },
			{ typeof(float),	TypeHelper.FloatType.GetSize },
			{ typeof(double),	TypeHelper.DoubleType.GetSize },
			{ typeof(long),		TypeHelper.LongType.GetSize },
			{ typeof(ulong),	TypeHelper.ULongType.GetSize },
			{ typeof(decimal),	TypeHelper.DecimalType.GetSize },
			{ typeof(DateTime), TypeHelper.DateTimeType.GetSize },
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
				if (type.BaseType == typeof(Enum))// .IsEnum) // BaseType == Enum ?
				{
					getSizeDelegate = EnumTypeHelper.Instance.GetGetSizeDelegate(type);
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
				TypeFlags typeFlags = GetTypeFlags(type);

				if (IsFlag(typeFlags, TypeFlags.Array))
					getSizeDelegate = TypeHelper.Array.GetSizeGetter(type).GetObjectSize;

				else if (IsFlag(typeFlags, TypeFlags.List))
					getSizeDelegate = TypeHelper.List.GetSizeGetter(type).GetObjectSize;

				else if (IsFlag(typeFlags, TypeFlags.Dictionary))
					getSizeDelegate = TypeHelper.Dictionary.GetSizeGetter(type).GetObjectSize;

				else if (IsFlag(typeFlags, TypeFlags.Class) || IsFlag(typeFlags, TypeFlags.Abstract) || IsFlag(typeFlags, TypeFlags.Interface))
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

		public static byte GetMinSize2(int dataSize)
		{
			if (dataSize <= 0xFF - 1)
				return 1;

			if (dataSize <= 0xFFFF - 2)
				return 2;

			if (dataSize <= 0xFFFFFF - 3)
				return 3;

			if (dataSize <= 0x7FFFFFFF - 4)
				return 4;

			throw new ArgumentException("Invalid data size: " + dataSize);
		}
	}
}