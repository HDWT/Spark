using System;
using System.Collections.Generic;
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
			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((ushort)value, sizes);
			}

			public int GetSize(ushort value, LinkedList<int> sizes)
			{
				return 2;
			}

			public ushort Read(byte[] data, ref int startIndex)
			{
				UShortTypeMapper mapper = new UShortTypeMapper();

				mapper.byte1 = data[startIndex++];
				mapper.byte2 = data[startIndex++];

				return mapper.value;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((ushort)value, data, ref startIndex, sizes);
			}

			public void Write(ushort value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				UShortTypeMapper mapper = new UShortTypeMapper();
				mapper.value = value;

				data[startIndex++] = mapper.byte1;
				data[startIndex++] = mapper.byte2;
			}
		}
	}
}