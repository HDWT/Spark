using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private delegate void WriteValueDelegate(object value, byte[] data, ref int startIndex);
	private delegate void WriteValueDelegate<T>(T value, byte[] data, ref int startIndex);

	private delegate void WriteReferenceDelegate(object value, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);
	private delegate void WriteReferenceDelegate<T>(T value, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);

	private static class Writer
	{
		private static readonly Dictionary<Type, WriteValueDelegate> s_writeValueDelegates = new Dictionary<Type, WriteValueDelegate>(16)
		{
			{ typeof(bool),		TypeHelper.Bool.WriteObject },
			{ typeof(byte),		TypeHelper.Byte.WriteObject },
			{ typeof(sbyte),	TypeHelper.SByte.WriteObject },
			{ typeof(char),		TypeHelper.Char.WriteObject },
			{ typeof(short),	TypeHelper.Short.WriteObject },
			{ typeof(ushort),	TypeHelper.UShort.WriteObject },
			{ typeof(int),		TypeHelper.IntType.WriteObject },
			{ typeof(uint),		TypeHelper.UInt.WriteObject },
			{ typeof(float),	TypeHelper.Float.WriteObject },
			{ typeof(double),	TypeHelper.Double.WriteObject },
			{ typeof(long),		TypeHelper.Long.WriteObject },
			{ typeof(ulong),	TypeHelper.ULong.WriteObject },
			{ typeof(decimal),	TypeHelper.Decimal.WriteObject },
			
			{ typeof(DateTime), TypeHelper.DateTime.WriteObject },
			
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
				if (type.IsArray)
					writeDelegate = TypeHelper.Array.GetDataWriter(type).WriteObject;

				else if (IsGenericList(type))
					writeDelegate = TypeHelper.List.GetDataWriter(type).WriteObject;

				else if (IsGenericDictionary(type))
					writeDelegate = TypeHelper.Dictionary.GetDataWriter(type).WriteObject;

				else if (type.IsClass || type.IsInterface)
					writeDelegate = TypeHelper.Object.GetDataWriter(type).WriteObject;

				else throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", type));

				s_writeReferenceDelegates[type] = writeDelegate;
			}

			return writeDelegate;
		}
	}
}