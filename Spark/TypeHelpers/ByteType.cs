
using System;
public static partial class Spark
{
	private static partial class TypeHelper
	{
		private class ByteType : ITypeHelper<byte>
		{
			public int GetSize(byte value)
			{
				return (value != zero) ? 2 : 1;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public byte Read(byte[] data, ref int startIndex)
			{
				int dataSize = data[startIndex++];

				if ((dataSize < 0) || (dataSize > 1))
					throw new System.ArgumentException("Invalid data size");

				return (dataSize == 1) ? data[startIndex++] : zero;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((byte)value, data, ref startIndex);
			}

			public void Write(byte value, byte[] data, ref int startIndex)
			{
				if (value != zero)
				{
					data[startIndex++] = one;
					data[startIndex++] = value;
				}
				else
				{
					data[startIndex++] = zero;
				}
			}
		}
	}
}