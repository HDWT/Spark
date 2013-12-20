using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct UIntTypeMapper
		{
			[FieldOffset(0)]
			public uint value;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;

			[FieldOffset(2)]
			public byte byte3;

			[FieldOffset(3)]
			public byte byte4;
		}

		private class UIntType : ITypeHelper<uint>
		{
			public int GetSize(uint value)
			{
				UIntTypeMapper mapper = new UIntTypeMapper();
				mapper.value = value;

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

			public uint Read(byte[] data, ref int startIndex)
			{
				UIntTypeMapper mapper = new UIntTypeMapper();

				int dataSize = data[startIndex++];

				if ((dataSize < 0) || (dataSize > 4))
					throw new System.ArgumentException();

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

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((uint)value, data, ref startIndex);
			}

			public void Write(uint value, byte[] data, ref int startIndex)
			{
				UIntTypeMapper mapper = new UIntTypeMapper();
				mapper.value = value;

				if (mapper.byte4 != zero)
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
