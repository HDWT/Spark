using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private delegate void WriteDataDelegate(object value, byte[] data, ref int startIndex);
	private delegate void WriteDataDelegate<T>(T value, byte[] data, ref int startIndex);

	private static class Writer
	{
		const byte NullReference = 0;

		public static WriteDataDelegate Get(Type type)
		{
			Type baseType = type.BaseType;

			if (baseType != typeof(object))
			{
				if (baseType == typeof(ValueType))
				{
					if (type == typeof(int))
						return TypeHelper.Int.WriteObject;

					if (type == typeof(float))
						return TypeHelper.Float.WriteObject;

					if (type == typeof(bool))
						return TypeHelper.Bool.WriteObject;

					if (type == typeof(char))
						return TypeHelper.Char.WriteObject;

					if (type == typeof(long))
						return TypeHelper.Long.WriteObject;

					if (type == typeof(short))
						return TypeHelper.Short.WriteObject;

					if (type == typeof(byte))
						return TypeHelper.Byte.WriteObject;

					if (type == typeof(DateTime))
						return TypeHelper.DateTime.WriteObject;

					if (type == typeof(double))
						return TypeHelper.Double.WriteObject;

					if (type == typeof(uint))
						return TypeHelper.UInt.WriteObject;

					if (type == typeof(ushort))
						return TypeHelper.UShort.WriteObject;

					if (type == typeof(ulong))
						return TypeHelper.ULong.WriteObject;

					if (type == typeof(sbyte))
						return TypeHelper.SByte.WriteObject;

					if (type == typeof(decimal))
						return TypeHelper.Decimal.WriteObject;

					throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
				}
				else if (baseType == typeof(Enum))
				{
					return EnumTypeHelper.Instance.GetWriter(type);
				}
				else if (baseType == typeof(Array))
				{
					return TypeHelper.Array.WriteObject;
				}
				else
				{
					throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
				}
			}
			else
			{
				if (type == typeof(string))
					return TypeHelper.String.WriteObject;

				if (IsGenericList(type))
					return WriteList;

				if (type.IsClass)
					return WriteClass;

				throw new NotImplementedException("Writer for " + type.Name + " type not implemented");
			}
		}

		public static void Write<T>(T value, byte[] data, ref int startIndex)
		{
			//if (typeof(T) == typeof(string))
			//{
			//	Write(value, data, ref startIndex);
			//}
			//if (typeof(T).BaseType == typeof(Enum))
			//{
			//	EnumTypeHelper.Instance.GetWriter(typeof(T))(value, data, ref startIndex);
			//}
			//else if (typeof(T).BaseType == typeof(Array))
			//{
			//	WriteArray(value, data, ref startIndex);
			//}
			//else if (IsGenericList(typeof(T)))
			//{
			//	WriteList(value, data, ref startIndex);
			//}
			//else
			//{
				LowLevelType<T>.Write(value, data, ref startIndex);
			//}
		}

		private static void WriteList(IList list, byte[] data, ref int startIndex)
		{
			int dataSize = SizeCalculator.Evaluate(list);
			byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

			// Сколько байт занимает поле dataSize
			data[startIndex++] = dataSizeBlock;

			// 
			for (int i = 0; i < dataSizeBlock; ++i)
			{
				data[startIndex++] = (byte)dataSize;
				dataSize >>= 8;
			}

			// Размер листа
			int listCount = list.Count;
			byte listCountBlock = SizeCalculator.GetMinSize(listCount);

			data[startIndex++] = listCountBlock;

			// 
			for (int j = 0; j < listCountBlock; ++j)
			{
				data[startIndex++] = (byte)listCount;
				listCount >>= 8;
			}

			ListTypeHelper.Write(list, data, ref startIndex);
		}

		private static void WriteList(object value, byte[] data, ref int startIndex)
		{
			if (value == null)
			{
				data[startIndex++] = NullReference;
				return;
			}

			WriteList((IList)value, data, ref startIndex);
		}

		private static void WriteClass(object value, byte[] data, ref int startIndex)
		{
			if (value == null)
			{
				data[startIndex++] = NullReference;
				return;
			}

			DataType dataType = DataType.Get(value.GetType());

			int dataSize = SizeCalculator.Evaluate(value);
			byte dataSizeBlock = SizeCalculator.GetMinSize(dataSize);

			// Сколько байт занимает поле dataSize
			data[startIndex++] = dataSizeBlock;

			// Записываем размер класса
			for (int i = 0; i < dataSizeBlock; ++i)
			{
				data[startIndex++] = (byte)dataSize;
				dataSize >>= 8;
			}

			// Записываем все поля класса
			dataType.WriteValues(value, data, ref startIndex);
		}
	}
}