using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private delegate void WriteValueDelegate(object value, byte[] data, ref int startIndex);
	private delegate void WriteValueDelegate<T>(T value, byte[] data, ref int startIndex);

	private delegate void WriteReferenceDelegate(object value, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context);
	private delegate void WriteReferenceDelegate<T>(T value, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context);

	private static class Writer
	{
		private static readonly Dictionary<Type, WriteValueDelegate> s_writeValueDelegates = new Dictionary<Type, WriteValueDelegate>(16)
		{
			{ typeof(bool),		TypeHelper.BoolType.WriteObject },
			{ typeof(byte),		TypeHelper.ByteType.WriteObject },
			{ typeof(sbyte),	TypeHelper.SByteType.WriteObject },
			{ typeof(char),		TypeHelper.CharType.WriteObject },
			{ typeof(short),	TypeHelper.ShortType.WriteObject },
			{ typeof(ushort),	TypeHelper.UShortType.WriteObject },
			{ typeof(int),		TypeHelper.IntType.WriteObject },
			{ typeof(uint),		TypeHelper.UIntType.WriteObject },
			{ typeof(float),	TypeHelper.FloatType.WriteObject },
			{ typeof(double),	TypeHelper.DoubleType.WriteObject },
			{ typeof(long),		TypeHelper.LongType.WriteObject },
			{ typeof(ulong),	TypeHelper.ULongType.WriteObject },
			{ typeof(decimal),	TypeHelper.DecimalType.WriteObject },
			
			{ typeof(DateTime), TypeHelper.DateTimeType.WriteObject },
			
		};

		private static readonly Dictionary<Type, WriteReferenceDelegate> s_writeReferenceDelegates = new Dictionary<Type, WriteReferenceDelegate>(16)
		{
			{ typeof(string),	TypeHelper.String.GetDataWriter(null).WriteObject },
		};

		public static WriteValueDelegate GetDelegateForValueType(Type type)
		{
			WriteValueDelegate writeValueDelegate = null;

			if (!s_writeValueDelegates.TryGetValue(type, out writeValueDelegate))
			{
				if (type.BaseType == typeof(Enum))// IsEnum) // BaseType == Enum ?
				{
					writeValueDelegate = EnumTypeHelper.Instance.GetWriter(type);
					s_writeValueDelegates[type] = writeValueDelegate;
				}
				else
				{
					throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));
				}
			}

			return writeValueDelegate;
		}

		public static WriteReferenceDelegate GetDelegateForReferenceType(Type type)
		{
			WriteReferenceDelegate writeDelegate = null;

			if (!s_writeReferenceDelegates.TryGetValue(type, out writeDelegate))
			{
				TypeFlags typeFlags = GetTypeFlags(type);

				if (IsFlag(typeFlags, TypeFlags.Array))
					writeDelegate = TypeHelper.Array.GetDataWriter(type).WriteObject;

				else if (IsFlag(typeFlags, TypeFlags.List))
					writeDelegate = TypeHelper.List.GetDataWriter(type).WriteObject;

				else if (IsFlag(typeFlags, TypeFlags.Dictionary))
					writeDelegate = TypeHelper.Dictionary.GetDataWriter(type).WriteObject;

				else if (IsFlag(typeFlags, TypeFlags.Class) || IsFlag(typeFlags, TypeFlags.Abstract) || IsFlag(typeFlags, TypeFlags.Interface))
					writeDelegate = TypeHelper.Object.GetDataWriter(type).WriteObject;

				else throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));

				s_writeReferenceDelegates[type] = writeDelegate;
			}

			return writeDelegate;
		}
	}
}