
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