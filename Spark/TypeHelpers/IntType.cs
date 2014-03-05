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

		private class IntType : ITypeHelper<int>
		{
			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((int)value, sizes);
			}

			public int GetSize(int value, LinkedList<int> sizes)
			{
				IntTypeMapper mapper = new IntTypeMapper();
				mapper.value = value;

				return TypeHelper.UInt.GetSize(mapper.uintValue, sizes);
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public int Read(byte[] data, ref int startIndex)
			{
				IntTypeMapper mapper = new IntTypeMapper();
				mapper.uintValue = TypeHelper.UInt.Read(data, ref startIndex);

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((int)value, data, ref startIndex, sizes);
			}

			public void Write(int value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				IntTypeMapper mapper = new IntTypeMapper();
				mapper.value = value;

				TypeHelper.UInt.Write(mapper.uintValue, data, ref startIndex, sizes);
			}
		}
	}
}
