using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private class SByteType : ITypeHelper<sbyte>
		{
			[FieldOffset(0)]
			private sbyte m_value;

			[FieldOffset(0)]
			private byte m_byte1;

			public int GetSize(sbyte value)
			{
				m_value = value;

				return (m_byte1 != zero) ? 2 : 1;
			}

			public void Write(sbyte value, byte[] data, ref int startIndex)
			{
				m_value = value;

				if (m_byte1 != zero)
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