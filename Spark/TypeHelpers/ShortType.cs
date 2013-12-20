using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct ShortTypeMapper
		{
			[FieldOffset(0)]
			public short value;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;
		}

		private class ShortType : ITypeHelper<short>
		{
			public int GetSize(short value)
			{
				ShortTypeMapper mapper = new ShortTypeMapper();
				mapper.value = value;

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

			public short Read(byte[] data, ref int startIndex)
			{
				ShortTypeMapper mapper = new ShortTypeMapper();

				int dataSize = data[startIndex++];

				if ((dataSize < 0) || (dataSize > 2))
					throw new System.ArgumentException("Invalid data size");

				if (dataSize == 1)
				{
					mapper.byte1 = data[startIndex++];
				}
				else if (dataSize == 2)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
				}

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((short)value, data, ref startIndex);
			}

			public void Write(short value, byte[] data, ref int startIndex)
			{
				ShortTypeMapper mapper = new ShortTypeMapper();
				mapper.value = value;

				if (mapper.byte2 != zero)
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