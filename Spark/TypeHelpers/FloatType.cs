using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private class FloatType : ITypeHelper<float>
		{
			[FieldOffset(0)]
			private float m_value;

			[FieldOffset(0)]
			private byte m_byte1;

			[FieldOffset(1)]
			private byte m_byte2;

			[FieldOffset(2)]
			private byte m_byte3;

			[FieldOffset(3)]
			private byte m_byte4;

			public int GetSize(float value)
			{
				m_value = value;

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

			public void Write(float value, byte[] data, ref int startIndex)
			{
				m_value = value;

				if (m_byte4 != zero)
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
