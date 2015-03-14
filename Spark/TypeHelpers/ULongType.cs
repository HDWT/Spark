using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct ULongTypeMapper
		{
			[FieldOffset(0)]
			public ulong value;

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

		private class ULongType : ITypeHelper<ulong>
		{
			const int  UnionSize	= 0x07;		// 0 0000 111

			const int  InvertMask	= 0x80;		// 1 0000 000
			const byte Invert		= 0x80;		// 1 0000 000

			const int  SizeMask		= 0x78;		// 0 1111 000
			const byte Size9		= 0x70;		// 0 1110 000
			const byte Size8		= 0x68;		// 0 1101 000
			const byte Size7		= 0x60;		// 0 1100 000
			const byte Size6		= 0x58;		// 0 1011 000
			const byte Size5		= 0x50;		// 0 1010 000
			const byte Size4		= 0x48;		// 0 1001 000
			const byte Size3		= 0x40;		// 0 1000 000
			const byte Size2		= 0x38;		// 0 0111 000
			const byte Size1		= 0x30;		// 0 0110 000

			public int GetSize(object value)
			{
				return GetSize((ulong)value);
			}

			public int GetSize(ulong value)
			{
				if (value > 0xFFFFFFFF00000000UL)
					value = ~value;

				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.value = value;

				if (value <= 0x00000000FFFFFFFFUL)
				{
					if (value <= 0x000000000000FFFFUL)
					{
						if (mapper.byte2 > UnionSize)
							return 3;

						return (mapper.byte2 != 0 || mapper.byte1 > UnionSize) ? (2) : (1);
					}
					else
					{
						if (mapper.byte4 > UnionSize)
							return 5;

						return (mapper.byte4 != 0 || mapper.byte3 > UnionSize) ? (4) : (3);
					}
				}
				else
				{
					if (value <= 0x0000FFFFFFFFFFFFUL)
					{
						if (mapper.byte6 > UnionSize)
							return 7;

						return (mapper.byte6 != 0 || mapper.byte5 > UnionSize) ? (6) : (5);
					}
					else
					{
						if (mapper.byte8 > UnionSize)
							return 9;

						return (mapper.byte8 != 0 || mapper.byte7 > UnionSize) ? (8) : (7);
					}
				}
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public ulong Read(byte[] data, ref int startIndex)
			{
				ULongTypeMapper mapper = new ULongTypeMapper();

				int dataSize = (data[startIndex] & SizeMask);
				int startIndexBefore = startIndex;
				byte startIndexValue = data[startIndex];

				data[startIndex] -= (byte)dataSize;

				bool invert = false;

				if ((data[startIndex] & InvertMask) == InvertMask)
				{
					invert = true;
					data[startIndex] -= (byte)InvertMask;
				}

				if (dataSize == Size9)
				{
					startIndex++;
					mapper.byte8 = data[startIndex++];
					mapper.byte7 = data[startIndex++];
					mapper.byte6 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size8)
				{
					mapper.byte8 = data[startIndex++];
					mapper.byte7 = data[startIndex++];
					mapper.byte6 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size7)
				{
					mapper.byte7 = data[startIndex++];
					mapper.byte6 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size6)
				{
					mapper.byte6 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size5)
				{
					mapper.byte5 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size4)
				{
					mapper.byte4 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size3)
				{
					mapper.byte3 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size2)
				{
					mapper.byte2 = data[startIndex++];
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == Size1)
				{
					mapper.byte1 = data[startIndex++];
				}
				else
				{
					throw new System.ArgumentException(string.Format("Invalid data size = {0}", dataSize));
				}

				data[startIndexBefore] = startIndexValue;

				return (invert) ? (~mapper.value) : (mapper.value);
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((ulong)value, data, ref startIndex);
			}

			public void Write(ulong value, byte[] data, ref int startIndex)
			{
				if (value > 0xFFFFFFFF00000000UL)
				{
					value = ~value;
					data[startIndex] = Invert;
				}

				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.value = value;

				if (value <= 0x00000000FFFFFFFFUL)
				{
					if (value <= 0x000000000000FFFFUL)
					{
						if (mapper.byte2 > UnionSize)
						{
							data[startIndex]   += Size3;
							data[startIndex++] += mapper.byte3;
							data[startIndex++]  = mapper.byte2;
							data[startIndex++]  = mapper.byte1;
						}
						else if (mapper.byte2 != 0 || mapper.byte1 > UnionSize)
						{
							data[startIndex]   += Size2;
							data[startIndex++] += mapper.byte2;
							data[startIndex++]  = mapper.byte1;
						}
						else
						{
							data[startIndex]   += Size1;
							data[startIndex++] += mapper.byte1;
						}
					}
					else
					{
						if (mapper.byte4 > UnionSize)
						{
							data[startIndex]   += Size5;
							data[startIndex++] += mapper.byte5;
							data[startIndex++]  = mapper.byte4;
							data[startIndex++]  = mapper.byte3;
						}
						else if (mapper.byte4 != 0 || mapper.byte3 > UnionSize)
						{
							data[startIndex]   += Size4;
							data[startIndex++] += mapper.byte4;
							data[startIndex++]  = mapper.byte3;
						}
						else
						{
							data[startIndex]   += Size3;
							data[startIndex++] += mapper.byte3;
						}

						data[startIndex++] = mapper.byte2;
						data[startIndex++] = mapper.byte1;
					}
				}
				else
				{
					if (value <= 0x0000FFFFFFFFFFFFUL)
					{
						if (mapper.byte6 > UnionSize)
						{
							data[startIndex]   += Size7;
							data[startIndex++] += mapper.byte7;
							data[startIndex++]  = mapper.byte6;
							data[startIndex++]  = mapper.byte5;
						}
						else if (mapper.byte6 != 0 || mapper.byte5 > UnionSize)
						{
							data[startIndex]   += Size6;
							data[startIndex++] += mapper.byte6;
							data[startIndex++]  = mapper.byte5;
						}
						else
						{
							data[startIndex]   += Size5;
							data[startIndex++] += mapper.byte5;
						}

						data[startIndex++] = mapper.byte4;
						data[startIndex++] = mapper.byte3;
						data[startIndex++] = mapper.byte2;
						data[startIndex++] = mapper.byte1;
					}
					else
					{
						if (mapper.byte8 > UnionSize)
						{
							data[startIndex++] += Size9;
							data[startIndex++]  = mapper.byte8;
							data[startIndex++]  = mapper.byte7;
						}
						else if (mapper.byte8 != 0 || mapper.byte7 > UnionSize)
						{
							data[startIndex]   += Size8;
							data[startIndex++] += mapper.byte8;
							data[startIndex++]  = mapper.byte7;
						}
						else
						{
							data[startIndex]   += Size7;
							data[startIndex++] += mapper.byte7;
						}

						data[startIndex++] = mapper.byte6;
						data[startIndex++] = mapper.byte5;
						data[startIndex++] = mapper.byte4;
						data[startIndex++] = mapper.byte3;
						data[startIndex++] = mapper.byte2;
						data[startIndex++] = mapper.byte1;
					}
				}
			}
		}
	}
}
