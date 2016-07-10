using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		public struct ArrayMapper
		{
			[FieldOffset(0)]
			public byte[] byteArray;

			[FieldOffset(0)]
			public char[] charArray;
		}

		public static class ByteType
		{
			public static int GetSize(object value)
			{
				return GetSize((byte)value);
			}

			public static int GetSize(byte value)
			{
				return (value == 0) ? 1 : 2;
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static byte Read(byte[] data, ref int startIndex)
			{
				byte dataSize = data[startIndex++];

				if (dataSize == zero)
					return zero;

				if (dataSize == one)
					return data[startIndex++];

				throw new System.ArgumentException(string.Format("Spark.Read - Invalid data size = {0}", dataSize));
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((byte)value, data, ref startIndex);
			}

			public static void Write(byte value, byte[] data, ref int startIndex)
			{
				if (value == zero)
				{
					data[startIndex++] = zero;
				}
				else
				{
					data[startIndex++] = one;
					data[startIndex++] = value;
				}
			}
		}
	}
}