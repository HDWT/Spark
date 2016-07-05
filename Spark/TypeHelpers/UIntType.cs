using System;
using System.Collections.Generic;
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
			public int intValue;

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
			public int GetSize(object value)
			{
				return GetSize((uint)value);
			}

			public int GetSize(uint value)
			{
				UIntTypeMapper mapper = new UIntTypeMapper();
				mapper.value = value;

				return TypeHelper.IntType.GetSize(mapper.intValue);
			}

			public uint FromBytes(byte[] data, int startIndex)
			{
				UIntTypeMapper mapper = new UIntTypeMapper();

				mapper.byte1 = data[startIndex++];
				mapper.byte2 = data[startIndex++];
				mapper.byte3 = data[startIndex++];
				mapper.byte4 = data[startIndex++];

				return mapper.value;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public uint Read(byte[] data, ref int startIndex)
			{
				UIntTypeMapper mapper = new UIntTypeMapper();
				mapper.intValue = TypeHelper.IntType.Read(data, ref startIndex);

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

				TypeHelper.IntType.Write(mapper.intValue, data, ref startIndex);
			}
		}
	}
}
