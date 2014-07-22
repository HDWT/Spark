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

	private delegate int GetReferenceSizeDelegate(object instance, QueueWithIndexer sizes);
	private delegate int GetReferenceSizeDelegate<T>(T instance, QueueWithIndexer sizes);

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
			{ typeof(int),		TypeHelper.IntType.GetSize },
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

		[StructLayout(LayoutKind.Explicit)]
		struct ListToArray
		{
			[StructLayout(LayoutKind.Explicit)]
			private class ArrayWraper
			{
				[FieldOffset(0)]
				public ValueSizeGetter[] array;
			}

			[FieldOffset(0)]
			public List<ValueSizeGetter> list;

			[FieldOffset(0)]
			private ArrayWraper m_arrayWraper;

			//[FieldOffset(4)]
			//public int arrayLength;

			public ValueSizeGetter[] array
			{
				get { return m_arrayWraper.array; }
			}
		}

		private static readonly List<ValueSizeGetter> s_testList = new List<ValueSizeGetter>()
		{
			new ValueSizeGetter(typeof(bool), TypeHelper.Bool.GetSize),
			new ValueSizeGetter(typeof(byte), TypeHelper.Byte.GetSize),
			new ValueSizeGetter(typeof(sbyte), TypeHelper.SByte.GetSize),
			new ValueSizeGetter(typeof(char), TypeHelper.Char.GetSize),
			new ValueSizeGetter(typeof(short), TypeHelper.Short.GetSize),
			new ValueSizeGetter(typeof(ushort), TypeHelper.UShort.GetSize),
			new ValueSizeGetter(typeof(int), TypeHelper.IntType.GetSize),
			new ValueSizeGetter(typeof(uint), TypeHelper.UInt.GetSize),
			new ValueSizeGetter(typeof(float), TypeHelper.Float.GetSize),
			new ValueSizeGetter(typeof(double), TypeHelper.Double.GetSize),
			new ValueSizeGetter(typeof(long), TypeHelper.Long.GetSize),
			new ValueSizeGetter(typeof(ulong), TypeHelper.ULong.GetSize),
			new ValueSizeGetter(typeof(decimal), TypeHelper.Decimal.GetSize),
			new ValueSizeGetter(typeof(DateTime), TypeHelper.DateTime.GetSize),
		};

		static SizeCalculator()
		{
			s_testList.Sort();
		}

		private static bool TryGetValue(Type type, out GetValueSizeDelegate getSizeDelegate)
		{
			ListToArray listToArray = new ListToArray();
			listToArray.list = s_testList;

			int index = System.Array.BinarySearch<ValueSizeGetter>(listToArray.array, 0, listToArray.list.Count, new ValueSizeGetter(type));

			if (index >= 0)
			{
				getSizeDelegate = listToArray.list[index].getValueSize;
				return true;
			}

			getSizeDelegate = null;
			return false;
		}

		struct ValueSizeGetter : IComparable<ValueSizeGetter>
		{
			public Type type;
			public GetValueSizeDelegate getValueSize;

			public ValueSizeGetter(Type type)
			{
				this.type = type;
				this.getValueSize = null;
			}
			public ValueSizeGetter(Type type, GetValueSizeDelegate getValueSize)
			{
				this.type = type;
				this.getValueSize = getValueSize;
			}

			public int CompareTo(ValueSizeGetter other)
			{
				int moduleCompareResult = type.Module.MetadataToken.CompareTo(other.type.Module.MetadataToken);

				if (moduleCompareResult != 0)
					return moduleCompareResult;

				return type.MetadataToken.CompareTo(other.type.MetadataToken);
			}
		}

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