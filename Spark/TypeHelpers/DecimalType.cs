using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private class DecimalType : ITypeHelper<decimal>
		{
			#region --- Private Fields ---

			[FieldOffset(0)]
			private decimal m_value;

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

			[FieldOffset(8)]
			private byte m_byte9;

			[FieldOffset(9)]
			private byte m_byte10;

			[FieldOffset(10)]
			private byte m_byte11;

			[FieldOffset(11)]
			private byte m_byte12;

			[FieldOffset(12)]
			private byte m_byte13;

			[FieldOffset(13)]
			private byte m_byte14;

			[FieldOffset(14)]
			private byte m_byte15;

			[FieldOffset(15)]
			private byte m_byte16;

			#endregion

			public int GetSize(decimal value)
			{
				m_value = value;

				if (m_byte16 != zero)
					return 17;

				if (m_byte15 != zero)
					return 16;

				if (m_byte14 != zero)
					return 15;

				if (m_byte13 != zero)
					return 14;

				if (m_byte12 != zero)
					return 13;

				if (m_byte11 != zero)
					return 12;

				if (m_byte10 != zero)
					return 11;

				if (m_byte9 != zero)
					return 10;

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

			public void Write(decimal value, byte[] data, ref int startIndex)
			{
				m_value = value;

				if (m_byte16 != zero)
				{
					data[startIndex++] = sixteen;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
					data[startIndex++] = m_byte10;
					data[startIndex++] = m_byte11;
					data[startIndex++] = m_byte12;
					data[startIndex++] = m_byte13;
					data[startIndex++] = m_byte14;
					data[startIndex++] = m_byte15;
					data[startIndex++] = m_byte16;
				}
				else if (m_byte15 != zero)
				{
					data[startIndex++] = fifteen;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
					data[startIndex++] = m_byte10;
					data[startIndex++] = m_byte11;
					data[startIndex++] = m_byte12;
					data[startIndex++] = m_byte13;
					data[startIndex++] = m_byte14;
					data[startIndex++] = m_byte15;
				}
				else if (m_byte14 != zero)
				{
					data[startIndex++] = fourteen;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
					data[startIndex++] = m_byte10;
					data[startIndex++] = m_byte11;
					data[startIndex++] = m_byte12;
					data[startIndex++] = m_byte13;
					data[startIndex++] = m_byte14;
				}
				else if (m_byte13 != zero)
				{
					data[startIndex++] = thirteen;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
					data[startIndex++] = m_byte10;
					data[startIndex++] = m_byte11;
					data[startIndex++] = m_byte12;
					data[startIndex++] = m_byte13;
				}
				else if (m_byte12 != zero)
				{
					data[startIndex++] = twelve;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
					data[startIndex++] = m_byte10;
					data[startIndex++] = m_byte11;
					data[startIndex++] = m_byte12;
				}
				else if (m_byte11 != zero)
				{
					data[startIndex++] = eleven;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
					data[startIndex++] = m_byte10;
					data[startIndex++] = m_byte11;
				}
				else if (m_byte10 != zero)
				{
					data[startIndex++] = ten;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
					data[startIndex++] = m_byte10;
				}
				else if (m_byte9 != zero)
				{
					data[startIndex++] = nine;
					data[startIndex++] = m_byte1;
					data[startIndex++] = m_byte2;
					data[startIndex++] = m_byte3;
					data[startIndex++] = m_byte4;
					data[startIndex++] = m_byte5;
					data[startIndex++] = m_byte6;
					data[startIndex++] = m_byte7;
					data[startIndex++] = m_byte8;
					data[startIndex++] = m_byte9;
				}
				else if (m_byte8 != zero)
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
