using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct ULongTypeMapper
		{
			[FieldOffset(0)]
			public ulong value;

			[FieldOffset(0)]
			public long longValue;

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

		private class ULongType : ITypeHelper<ulong>
		{
			public int GetSize(object value)
			{
				return GetSize((ulong)value);
			}

			public int GetSize(ulong value)
			{
				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.value = value;

				return TypeHelper.Long.GetSize(mapper.longValue);
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public ulong Read(byte[] data, ref int startIndex)
			{
				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.longValue = TypeHelper.Long.Read(data, ref startIndex);

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((ulong)value, data, ref startIndex);
			}

			public void Write(ulong value, byte[] data, ref int startIndex)
			{
				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.value = value;

				TypeHelper.Long.Write(mapper.longValue, data, ref startIndex);
			}
		}
	}
}
