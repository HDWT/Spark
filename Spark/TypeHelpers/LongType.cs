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
			public ulong ulongValue;

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
			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((long)value, sizes);
			}

			public int GetSize(long value, LinkedList<int> sizes)
			{
				LongTypeMapper mapper = new LongTypeMapper();
				mapper.value = value;

				return TypeHelper.ULong.GetSize(mapper.ulongValue, sizes);
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public long Read(byte[] data, ref int startIndex)
			{
				LongTypeMapper mapper = new LongTypeMapper();
				mapper.ulongValue = TypeHelper.ULong.Read(data, ref startIndex);

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((long)value, data, ref startIndex, sizes);
			}

			public void Write(long value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				LongTypeMapper mapper = new LongTypeMapper();
				mapper.value = value;

				TypeHelper.ULong.Write(mapper.ulongValue, data, ref startIndex, sizes);
			}
		}
	}
}
