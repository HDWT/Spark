using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct BoolTypeMapper
		{
			[FieldOffset(0)]
			public bool value;

			[FieldOffset(0)]
			public byte byte1;
		}

		private class BoolType : ITypeHelper<bool>
		{
			public int GetSize(bool value)
			{
				BoolTypeMapper mapper = new BoolTypeMapper();
				mapper.value = value;

				return (mapper.byte1 != zero) ? 2 : 1;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public bool Read(byte[] data, ref int startIndex)
			{
				BoolTypeMapper mapper = new BoolTypeMapper();

				int dataSize = data[startIndex++];

				if ((dataSize < 0) || (dataSize > 1))
					throw new System.ArgumentException("Invalid data size");

				if (dataSize == 1)
					mapper.byte1 = data[startIndex++];

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((bool)value, data, ref startIndex);
			}

			public void Write(bool value, byte[] data, ref int startIndex)
			{
				BoolTypeMapper mapper = new BoolTypeMapper();
				mapper.value = value;

				if (mapper.byte1 != zero)
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