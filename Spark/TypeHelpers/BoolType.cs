using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		public static class BoolType
		{
			public static int GetSize(object value)
			{
				return GetSize((bool)value);
			}

			public static int GetSize(bool value)
			{
				return (value == false) ? 1 : 2;
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static bool Read(byte[] data, ref int startIndex)
			{
				byte dataSize = data[startIndex++];

				if (dataSize == zero)
					return false;

				if (dataSize == one)
					return data[startIndex++] != zero;

				throw new System.ArgumentException(string.Format("Spark.Read - Invalid data size = {0}", dataSize));
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((bool)value, data, ref startIndex);
			}

			public static void Write(bool value, byte[] data, ref int startIndex)
			{
				if (value)
				{
					data[startIndex++] = one;
					data[startIndex++] = one;
				}
				else
				{
					data[startIndex++] = zero;
				}
			}
		}
	}
}