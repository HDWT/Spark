using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalTypeMapper
		{
			[FieldOffset(0)]
			public decimal value;

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

			[FieldOffset(8)]
			public byte byte9;

			[FieldOffset(9)]
			public byte byte10;

			[FieldOffset(10)]
			public byte byte11;

			[FieldOffset(11)]
			public byte byte12;

			[FieldOffset(12)]
			public byte byte13;

			[FieldOffset(13)]
			public byte byte14;

			[FieldOffset(14)]
			public byte byte15;

			[FieldOffset(15)]
			public byte byte16;
		}

		private class DecimalType : ITypeHelper<decimal>
		{
			public int GetSize(object value)
			{
				return GetSize((decimal)value);
			}

			public int GetSize(decimal value)
			{
				DecimalTypeMapper mapper = new DecimalTypeMapper();
				mapper.value = value;

				if (mapper.byte16 != zero)
					return 17;

				if (mapper.byte15 != zero)
					return 16;

				if (mapper.byte14 != zero)
					return 15;

				if (mapper.byte13 != zero)
					return 14;

				if (mapper.byte12 != zero)
					return 13;

				if (mapper.byte11 != zero)
					return 12;

				if (mapper.byte10 != zero)
					return 11;

				if (mapper.byte9 != zero)
					return 10;

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

			public decimal Read(byte[] data, ref int startIndex)
			{
				DecimalTypeMapper mapper = new DecimalTypeMapper();

				int dataSize = data[startIndex++];

				if ((dataSize < 0) || (dataSize > 16))
					throw new System.ArgumentException("Invalid data size");

				if (dataSize >= 8)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte6 = data[startIndex++];
					mapper.byte7 = data[startIndex++];
					mapper.byte8 = data[startIndex++];

					if (dataSize >= 12)
					{
						mapper.byte9 = data[startIndex++];
						mapper.byte10 = data[startIndex++];
						mapper.byte11 = data[startIndex++];
						mapper.byte12 = data[startIndex++];

						if (dataSize >= 13)
							mapper.byte13 = data[startIndex++];

						if (dataSize >= 14)
							mapper.byte14 = data[startIndex++];

						if (dataSize >= 15)
							mapper.byte15 = data[startIndex++];

						if (dataSize == 16)
							mapper.byte16 = data[startIndex++];
					}
					else
					{
						if (dataSize >= 9)
							mapper.byte9 = data[startIndex++];

						if (dataSize >= 10)
							mapper.byte10 = data[startIndex++];

						if (dataSize >= 11)
							mapper.byte11 = data[startIndex++];
					}
				}
				else if (dataSize >= 4)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];

					if (dataSize >= 5)
						mapper.byte5 = data[startIndex++];

					if (dataSize >= 6)
						mapper.byte6 = data[startIndex++];

					if (dataSize >= 7)
						mapper.byte7 = data[startIndex++];
				}
				else
				{
					if (dataSize == 0)
						return mapper.value;

					if (dataSize >= 1)
						mapper.byte1 = data[startIndex++];

					if (dataSize >= 2)
						mapper.byte2 = data[startIndex++];

					if (dataSize >= 3)
						mapper.byte3 = data[startIndex++];
				}

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((decimal)value, data, ref startIndex);
			}

			public void Write(decimal value, byte[] data, ref int startIndex)
			{
				DecimalTypeMapper mapper = new DecimalTypeMapper();
				mapper.value = value;

				if (mapper.byte16 != zero)
				{
					data[startIndex++] = sixteen;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
					data[startIndex++] = mapper.byte10;
					data[startIndex++] = mapper.byte11;
					data[startIndex++] = mapper.byte12;
					data[startIndex++] = mapper.byte13;
					data[startIndex++] = mapper.byte14;
					data[startIndex++] = mapper.byte15;
					data[startIndex++] = mapper.byte16;
				}
				else if (mapper.byte15 != zero)
				{
					data[startIndex++] = fifteen;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
					data[startIndex++] = mapper.byte10;
					data[startIndex++] = mapper.byte11;
					data[startIndex++] = mapper.byte12;
					data[startIndex++] = mapper.byte13;
					data[startIndex++] = mapper.byte14;
					data[startIndex++] = mapper.byte15;
				}
				else if (mapper.byte14 != zero)
				{
					data[startIndex++] = fourteen;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
					data[startIndex++] = mapper.byte10;
					data[startIndex++] = mapper.byte11;
					data[startIndex++] = mapper.byte12;
					data[startIndex++] = mapper.byte13;
					data[startIndex++] = mapper.byte14;
				}
				else if (mapper.byte13 != zero)
				{
					data[startIndex++] = thirteen;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
					data[startIndex++] = mapper.byte10;
					data[startIndex++] = mapper.byte11;
					data[startIndex++] = mapper.byte12;
					data[startIndex++] = mapper.byte13;
				}
				else if (mapper.byte12 != zero)
				{
					data[startIndex++] = twelve;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
					data[startIndex++] = mapper.byte10;
					data[startIndex++] = mapper.byte11;
					data[startIndex++] = mapper.byte12;
				}
				else if (mapper.byte11 != zero)
				{
					data[startIndex++] = eleven;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
					data[startIndex++] = mapper.byte10;
					data[startIndex++] = mapper.byte11;
				}
				else if (mapper.byte10 != zero)
				{
					data[startIndex++] = ten;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
					data[startIndex++] = mapper.byte10;
				}
				else if (mapper.byte9 != zero)
				{
					data[startIndex++] = nine;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte4;
					data[startIndex++] = mapper.byte5;
					data[startIndex++] = mapper.byte6;
					data[startIndex++] = mapper.byte7;
					data[startIndex++] = mapper.byte8;
					data[startIndex++] = mapper.byte9;
				}
				else if (mapper.byte8 != zero)
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
