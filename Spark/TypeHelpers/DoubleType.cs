using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleTypeMapper
		{
			[FieldOffset(0)]
			public double value;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;

			[FieldOffset(2)]
			public byte byte3;

			[FieldOffset(3)]
			public byte byte4;

			[FieldOffset(4)]
			public byte byte5;

			[FieldOffset(5)]
			public byte byte6;

			[FieldOffset(6)]
			public byte byte7;

			[FieldOffset(7)]
			public byte byte8;
		}

		private class DoubleType : ITypeHelper<double>
		{
			public int GetSize(double value)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.value = value;

				if (mapper.byte8 != zero)
					return 9;

				if (mapper.byte7 != zero)
					return 8;

				if (mapper.byte6 != zero)
					return 7;

				if (mapper.byte5 != zero)
					return 6;

				if (mapper.byte4 != zero)
					return 5;

				if (mapper.byte3 != zero)
					return 4;

				if (mapper.byte2 != zero)
					return 3;

				if (mapper.byte1 != zero)
					return 2;

				return 1;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public double Read(byte[] data, ref int startIndex)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();

				int dataSize = data[startIndex++];

				if ((dataSize < 0) || (dataSize > 8))
					throw new System.ArgumentException("Invalid data size");

				if (dataSize < 5)
				{
					if (dataSize < 3)
					{
						if (dataSize >= 1)
							mapper.byte1 = data[startIndex++];

						if (dataSize == 2)
							mapper.byte2 = data[startIndex++];
					}
					else
					{
						mapper.byte1 = data[startIndex++];
						mapper.byte2 = data[startIndex++];
						mapper.byte3 = data[startIndex++];

						if (dataSize == 4)
							mapper.byte4 = data[startIndex++];
					}
				}
				else
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte5 = data[startIndex++];

					if (dataSize >= 6)
						mapper.byte6 = data[startIndex++];

					if (dataSize >= 7)
						mapper.byte7 = data[startIndex++];

					if (dataSize == 8)
						mapper.byte8 = data[startIndex++];
				}

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((double)value, data, ref startIndex);
			}

			public void Write(double value, byte[] data, ref int startIndex)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.value = value;

				if (mapper.byte8 != zero)
				{
					data[startIndex++] = eight;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
				}
				else if (mapper.byte7 != zero)
				{
					data[startIndex++] = seven;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
				}
				else if (mapper.byte6 != zero)
				{
					data[startIndex++] = six;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
				}
				else if (mapper.byte5 != zero)
				{
					data[startIndex++] = five;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
				}
				else if (mapper.byte4 != zero)
				{
					data[startIndex++] = four;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
				}
				else if (mapper.byte3 != zero)
				{
					data[startIndex++] = three;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
				}
				else if (mapper.byte2 != zero)
				{
					data[startIndex++] = two;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
				}
				else if (mapper.byte1 != zero)
				{
					data[startIndex++] = one;
					data[startIndex++] = mapper.byte1;
				}
				else
				{
					data[startIndex++] = zero;
				}
			}
		}
	}
}
