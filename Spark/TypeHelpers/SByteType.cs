using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct SByteTypeMapper
		{
			[FieldOffset(0)]
			public sbyte value;

			[FieldOffset(0)]
			public byte byte1;
		}

		private class SByteType : ITypeHelper<sbyte>
		{
			public int GetSize(sbyte value)
			{
				SByteTypeMapper mapper = new SByteTypeMapper();
				mapper.value = value;

				return 1;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public sbyte Read(byte[] data, ref int startIndex)
			{
				SByteTypeMapper mapper = new SByteTypeMapper();
				mapper.byte1 = data[startIndex++];

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((sbyte)value, data, ref startIndex);
			}

			public void Write(sbyte value, byte[] data, ref int startIndex)
			{
				SByteTypeMapper mapper = new SByteTypeMapper();
				mapper.value = value;

				data[startIndex++] = mapper.byte1;
			}
		}
	}
}