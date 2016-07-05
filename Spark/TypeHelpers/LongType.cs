using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct LongTypeMapper
		{
			[FieldOffset(0)]
			public long value;

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

		private class LongType : ITypeHelper<long>
		{
			private const long LongZero = (long)0x00;

			public int GetSize(object value)
			{
				return GetSize((long)value);
			}

			public int GetSize(long value)
			{
				if (value == LongZero)
					return 1;

				LongTypeMapper mapper = new LongTypeMapper();
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

				throw new System.ArgumentException();
			}

			public long FromBytes(byte[] data, int startIndex)
			{
				LongTypeMapper mapper = new LongTypeMapper();

				mapper.byte1 = data[startIndex++];
				mapper.byte2 = data[startIndex++];
				mapper.byte3 = data[startIndex++];
				mapper.byte4 = data[startIndex++];
				mapper.byte5 = data[startIndex++];
				mapper.byte6 = data[startIndex++];
				mapper.byte7 = data[startIndex++];
				mapper.byte8 = data[startIndex++];

				//mapper.ulongValue = TypeHelper.ULong.FromBytes(data, startIndex);

				return mapper.value;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public long Read(byte[] data, ref int startIndex)
			{
				byte dataSize = data[startIndex++];

				if (dataSize == zero)
					return LongZero;

				LongTypeMapper mapper = new LongTypeMapper();

				if (dataSize == eight)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte6 = data[startIndex++];
					mapper.byte7 = data[startIndex++];
					mapper.byte8 = data[startIndex++];
				}
				else if (dataSize == seven)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte6 = data[startIndex++];
					mapper.byte7 = data[startIndex++];
				}
				else if (dataSize == six)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
					mapper.byte6 = data[startIndex++];
				}
				else if (dataSize == five)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
					mapper.byte5 = data[startIndex++];
				}
				else if (dataSize == four)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
					mapper.byte4 = data[startIndex++];
				}
				else if (dataSize == three)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
					mapper.byte3 = data[startIndex++];
				}
				else if (dataSize == two)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
				}
				else if (dataSize == one)
				{
					mapper.byte1 = data[startIndex++];
				}
				else
				{
					throw new System.ArgumentException(string.Format("Spark.Read - Invalid data size = {0}", dataSize));
				}

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((long)value, data, ref startIndex);
			}

			public void Write(long value, byte[] data, ref int startIndex)
			{
				if (value == LongZero)
				{
					data[startIndex++] = zero;
					return;
				}

				LongTypeMapper mapper = new LongTypeMapper();
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
					throw new System.ArgumentException();
				}
			}
		}
	}
}
