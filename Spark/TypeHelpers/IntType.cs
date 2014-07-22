using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct IntTypeMapper
		{
			[FieldOffset(0)]
			public int value;

			[FieldOffset(0)]
			public uint uintValue;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;

			[FieldOffset(2)]
			public byte byte3;

			[FieldOffset(3)]
			public byte byte4;
		}

		public class IntType //: ITypeHelper<int>
		{
			const int  UnionSize	= 0x0F; // 0 000 1111

			const int  InvertMask	= 0x80;	// 1 000 0000
			const byte Invert		= 0x80;	// 1 000 0000

			const int  SizeMask		= 0x70;	// 0 111 0000
			const byte Size5		= 0x50;	// 0 101 0000
			const byte Size4		= 0x40;	// 0 100 0000
			const byte Size3		= 0x30;	// 0 011 0000
			const byte Size2		= 0x20;	// 0 010 0000
			const byte Size1		= 0x10;	// 0 001 0000

			const int negative = -65536; //- 1 - 0x0000ffff;

			public static int GetSize(object value)
			{
				return GetSize((int)value);
			}

			public static int GetSize(int value)
			{
				if (value < 0 && value > negative)
					value = ~value;

				IntTypeMapper mapper = new IntTypeMapper();
				mapper.value = value;

				if (mapper.byte4 != 0)
					return (mapper.byte4 > UnionSize) ? 5 : 4;

				if (mapper.byte3 != 0)
					return (mapper.byte3 > UnionSize) ? 4 : 3;

				if (mapper.byte2 != 0)
					return (mapper.byte2 > UnionSize) ? 3 : 2;

				if (mapper.byte1 != 0)
					return (mapper.byte1 > UnionSize) ? 2 : 1;

				return 1;
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static int Read(byte[] data, ref int startIndex)
			{
				IntTypeMapper mapper = new IntTypeMapper();

				int dataSize = (data[startIndex] & SizeMask);
				data[startIndex] -= (byte)dataSize;

				bool invert = false;

				//if ((data[startIndex] & InvertMask) == InvertMask)
				if (data[startIndex] >= InvertMask)
				{
					invert = true;
					data[startIndex] -= (byte)InvertMask;
				}

				if (dataSize == Size5)
				{
					startIndex++;
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
					throw new System.ArgumentException(string.Format("Spark.Read - Invalid data size = {0}", dataSize));
				}

				return (invert) ? (~mapper.value) : (mapper.value);
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((int)value, data, ref startIndex);
			}

			public static void Write(int value, byte[] data, ref int startIndex)
			{
				if (value < 0 && value > negative)
				{
					value = ~value;
					data[startIndex] = Invert;
				}
	
				IntTypeMapper mapper = new IntTypeMapper();
				mapper.value = value;

				if (mapper.byte4 != 0)
				{
					if (mapper.byte4 > UnionSize)
					{
						data[startIndex++] += Size5;
						data[startIndex++] = mapper.byte4;
					}
					else
					{
						data[startIndex] += Size4;
						data[startIndex++] += mapper.byte4;
					}

					data[startIndex++] = mapper.byte3;
					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte1;
				}
				else if (mapper.byte3 != 0)
				{
					if (mapper.byte3 > UnionSize)
					{
						data[startIndex++] += Size4;
						data[startIndex++] = mapper.byte3;
					}
					else
					{
						data[startIndex] += Size3;
						data[startIndex++] += mapper.byte3;
					}

					data[startIndex++] = mapper.byte2;
					data[startIndex++] = mapper.byte1;
				}
				else if (mapper.byte2 != 0)
				{
					if (mapper.byte2 > UnionSize)
					{
						data[startIndex++] += Size3;
						data[startIndex++] = mapper.byte2;
					}
					else
					{
						data[startIndex] += Size2;
						data[startIndex++] += mapper.byte2;
					}

					data[startIndex++] = mapper.byte1;
				}
				else if (mapper.byte1 > UnionSize)
				{
					data[startIndex++] += Size2;
					data[startIndex++] = mapper.byte1;
				}
				else
				{
					data[startIndex] += Size1;
					data[startIndex++] += mapper.byte1;
				}

				//Stopwatch.Stop();
			}
		}
	}
}
