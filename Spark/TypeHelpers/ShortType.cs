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
			public int GetSize(object value)
			{
				return GetSize((short)value);
			}

			public int GetSize(short value)
			{
				return 2;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public short Read(byte[] data, ref int startIndex)
			{
				ShortTypeMapper mapper = new ShortTypeMapper();

				mapper.byte1 = data[startIndex++];
				mapper.byte2 = data[startIndex++];

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

				data[startIndex++] = mapper.byte1;
				data[startIndex++] = mapper.byte2;
			}
		}
	}
}