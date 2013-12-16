using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private class ULongType : ITypeHelper<ulong>
		{
			[FieldOffset(0)]
			private ulong m_value;

			[FieldOffset(0)]
			private byte m_byte1;

			[FieldOffset(1)]
			private byte m_byte2;

			[FieldOffset(2)]
			private byte m_byte3;

			[FieldOffset(3)]
			private byte m_byte4;

			[FieldOffset(4)]
			private byte m_byte5;

			[FieldOffset(5)]
			private byte m_byte6;

			[FieldOffset(6)]
			private byte m_byte7;

			[FieldOffset(7)]
			private byte m_byte8;

			public int GetSize(ulong value)
			{
				m_value = value;

				if (m_byte8 != zero)
					return 9;

				if (m_byte7 != zero)
					return 8;

				if (m_byte6 != zero)
					return 7;

				if (m_byte5 != zero)
					return 6;

				if (m_byte4 != zero)
					return 5;

				if (m_byte3 != zero)
					return 4;

				if (m_byte2 != zero)
					return 3;

				if (m_byte1 != zero)
					return 2;

				return 1;
			}

			public void Write(ulong value, byte[] data, ref int startIndex)
			{
				m_value = value;

				if (m_byte8 != zero)
				{
					data[startIndex++] = eight;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
				}
				else if (m_byte7 != zero)
				{
					data[startIndex++] = seven;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
				}
				else if (m_byte6 != zero)
				{
					data[startIndex++] = six;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
				}
				else if (m_byte5 != zero)
				{
					data[startIndex++] = five;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
				}
				else if (m_byte4 != zero)
				{
					data[startIndex++] = four;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
				}
				else if (m_byte3 != zero)
				{
					data[startIndex++] = three;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
				}
				else if (m_byte2 != zero)
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
