using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private static class ListTypeHelper
	{
		public static int GetSize(IList list)
		{
			Type elementType = list.GetType().GetGenericArguments()[0];

			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					return GetListSize<int>(list);

				if (elementType == typeof(float))
					return GetListSize<float>(list);

				if (elementType == typeof(bool))
					return GetListSize<bool>(list);

				if (elementType.IsEnum)
					return GetListSize(list, elementType);

				if (elementType == typeof(DateTime))
					return GetListSize<DateTime>(list);

				if (elementType == typeof(short))
					return GetListSize<short>(list);

				if (elementType == typeof(long))
					return GetListSize<long>(list);

				if (elementType == typeof(double))
					return GetListSize<double>(list);

				if (elementType == typeof(byte))
					return GetListSize<byte>(list);

				if (elementType == typeof(char))
					return GetListSize<char>(list);

				if (elementType == typeof(uint))
					return GetListSize<uint>(list);

				if (elementType == typeof(ushort))
					return GetListSize<ushort>(list);

				if (elementType == typeof(ulong))
					return GetListSize<ulong>(list);

				if (elementType == typeof(decimal))
					return GetListSize<decimal>(list);

				if (elementType == typeof(sbyte))
					return GetListSize<sbyte>(list);

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					return GetListSize<string>(list);

				if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
					return GetListSize(list, elementType);

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private static int GetListSize<T>(IList list)
		{
			int size = 0;

			foreach (T item in (List<T>)list)
				size += SizeCalculator.Evaluate<T>(item);

			return size;
		}

		private static int GetListSize(IList list, Type elementType)
		{
			int size = 0;

			var GetSize = SizeCalculator.Get(elementType);

			foreach (var item in list)
				size += GetSize(item);

			return size;
		}

		/// <summary> </summary>
		public static void Write(IList list, byte[] data, ref int startIndex)
		{
			Type elementType = list.GetType().GetGenericArguments()[0];

			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					WriteList<int>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(float))
					WriteList<float>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(bool))
					WriteList<bool>(list, elementType, data, ref startIndex);

				else if (elementType.IsEnum)
					WriteList(list, elementType, data, ref startIndex);

				else if (elementType == typeof(DateTime))
					WriteList<DateTime>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(short))
					WriteList<short>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(long))
					WriteList<long>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(double))
					WriteList<double>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(byte))
					WriteList<byte>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(char))
					WriteList<char>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(uint))
					WriteList<uint>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(ushort))
					WriteList<ushort>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(ulong))
					WriteList<ulong>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(decimal))
					WriteList<decimal>(list, elementType, data, ref startIndex);

				else if (elementType == typeof(sbyte))
					WriteList<sbyte>(list, elementType, data, ref startIndex);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					WriteList<string>(list, elementType, data, ref startIndex);

				else if (elementType.IsArray || IsGenericList(elementType) || elementType.IsClass)
					WriteList(list, elementType, data, ref startIndex);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private static void WriteList<T>(IList list, Type elementType, byte[] data, ref int startIndex)
		{
			foreach (T item in (List<T>)list)
				Writer.Write<T>(item, data, ref startIndex);
		}

		private static void WriteList(IList list, Type elementType, byte[] data, ref int startIndex)
		{
			var WriteData = Writer.Get(elementType);

			foreach (var item in list)
				WriteData(item, data, ref startIndex);
		}
	}
}