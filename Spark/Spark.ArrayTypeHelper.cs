using System;
using System.Collections;

public static partial class Spark
{
	private static class ArrayTypeHelper
	{
		public static int GetSize(Array array)
		{
			Type elementType = array.GetType().GetElementType();

			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					return GetArraySize<int>(array);

				if (elementType == typeof(float))
					return GetArraySize<float>(array);

				if (elementType == typeof(bool))
					return GetArraySize<bool>(array);

				if (elementType.IsEnum)
					return GetArraySize(array, elementType);

				if (elementType == typeof(DateTime))
					return GetArraySize<DateTime>(array);

				if (elementType == typeof(short))
					return GetArraySize<short>(array);

				if (elementType == typeof(long))
					return GetArraySize<long>(array);

				if (elementType == typeof(double))
					return GetArraySize<double>(array);

				if (elementType == typeof(byte))
					return GetArraySize<byte>(array);

				if (elementType == typeof(char))
					return GetArraySize<char>(array);

				if (elementType == typeof(uint))
					return GetArraySize<uint>(array);

				if (elementType == typeof(ushort))
					return GetArraySize<ushort>(array);

				if (elementType == typeof(ulong))
					return GetArraySize<ulong>(array);

				if (elementType == typeof(decimal))
					return GetArraySize<decimal>(array);

				if (elementType == typeof(sbyte))
					return GetArraySize<sbyte>(array);

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					return GetArraySize<string>(array);

				if (elementType.IsArray || elementType.IsGenericList() || elementType.IsClass)
					return GetArraySize(array, elementType);

				throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private static int GetArraySize<T>(Array array)
		{
			int size = 0;

			switch (array.Rank)
			{
				case 1:
					foreach (T item in (T[])array)
						size += SizeCalculator.Evaluate<T>(item);
					return size;

				case 2:
					foreach (T item in (T[,])array)
						size += SizeCalculator.Evaluate<T>(item);
					return size;

				case 3:
					foreach (T item in (T[, ,])array)
						size += SizeCalculator.Evaluate<T>(item);
					return size;

				case 4:
					foreach (T item in (T[, , ,])array)
						size += SizeCalculator.Evaluate<T>(item);
					return size;

				default:
					throw new NotImplementedException(string.Format("Get size for array with rank {0} not implemented", array.Rank));
			}
		}

		private static int GetArraySize(Array array, Type elementType)
		{
			int size = 0;

			var GetSize = SizeCalculator.Get(elementType);

			foreach (var item in array)
				size += GetSize(item);

			return size;
		}

		/// <summary> </summary>
		public static void Write(Array array, byte[] data, ref int startIndex)
		{
			Type elementType = array.GetType().GetElementType();

			if (elementType.IsValueType)
			{
				if (elementType == typeof(int))
					WriteArray<int>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(float))
					WriteArray<float>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(bool))
					WriteArray<bool>(array, elementType, data, ref startIndex);

				else if (elementType.IsEnum)
					WriteArray(array, elementType, data, ref startIndex);

				else if (elementType == typeof(DateTime))
					WriteArray<DateTime>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(short))
					WriteArray<short>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(long))
					WriteArray<long>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(double))
					WriteArray<double>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(byte))
					WriteArray<byte>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(char))
					WriteArray<char>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(uint))
					WriteArray<uint>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(ushort))
					WriteArray<ushort>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(ulong))
					WriteArray<ulong>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(decimal))
					WriteArray<decimal>(array, elementType, data, ref startIndex);

				else if (elementType == typeof(sbyte))
					WriteArray<sbyte>(array, elementType, data, ref startIndex);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
			else
			{
				if (elementType == typeof(string))
					WriteArray<string>(array, elementType, data, ref startIndex);

				else if (elementType.IsArray || elementType.IsGenericList() || elementType.IsClass)
					WriteArray(array, elementType, data, ref startIndex);

				else throw new NotImplementedException(string.Format("Type '{0}' is not suppoerted", elementType));
			}
		}

		private static void WriteArray<T>(Array array, Type elementType, byte[] data, ref int startIndex)
		{
			switch (array.Rank)
			{
				case 1:
					foreach (var item in (T[])array)
						Writer.Write<T>(item, data, ref startIndex);
					break;

				case 2:
					foreach (var item in (T[,])array)
						Writer.Write<T>(item, data, ref startIndex);
					break;

				case 3:
					foreach (var item in (T[, ,])array)
						Writer.Write<T>(item, data, ref startIndex);
					break;

				case 4:
					foreach (var item in (T[, , ,])array)
						Writer.Write<T>(item, data, ref startIndex);
					break;

				default:
					throw new NotImplementedException(string.Format( "Write array with rank {0} not implemented", array.Rank));
			}
		}

		private static void WriteArray(Array array, Type elementType, byte[] data, ref int startIndex)
		{
			var WriteData = Writer.Get(elementType);

			foreach (var item in array)
				WriteData(item, data, ref startIndex);
		}
	}
}
