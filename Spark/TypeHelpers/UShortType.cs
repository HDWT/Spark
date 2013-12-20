using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct UShortTypeMapper
		{
			[FieldOffset(0)]
			public ushort value;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;
		}

		private class UShortType : ITypeHelper<ushort>
		{
			public int GetSize(ushort value)
			{
				UShortTypeMapper mapper = new UShortTypeMapper();
				mapper.value = value;

				if (mapper.byte2 != zero)
					return 3;

				if (mapper.byte1 != zero)
					return 2;

				return 1;
			}

			public ushort Read(byte[] data, ref int startIndex)
			{
				UShortTypeMapper mapper = new UShortTypeMapper();

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

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((ushort)value, data, ref startIndex);
			}

			public void Write(ushort value, byte[] data, ref int startIndex)
			{
				UShortTypeMapper mapper = new UShortTypeMapper();
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