using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private class CharType : ITypeHelper<char>
		{
			[FieldOffset(0)]
			private char m_value;

			[FieldOffset(0)]
			private byte m_byte1;

			[FieldOffset(1)]
			private byte m_byte2;

			public int GetSize(char value)
			{
				m_value = value;

				if (m_byte2 != zero)
					return 3;

				if (m_byte1 != zero)
					return 2;

				return 1;
			}

			public void Write(char value, byte[] data, ref int startIndex)
			{
				m_value = value;

				if (m_byte2 != zero)
				{
					data[startIndex++] = two;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
				}
				else if (m_byte1 != zero)
				{
					data[startIndex++] = one;
					data[startIndex++] = m_byte1;
				}
				else
				{
					data[startIndex++] = zero;
				}
			}
		}
	}
}