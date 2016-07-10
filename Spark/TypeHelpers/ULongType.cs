using System;
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

		public static class ULongType
		{
			public static int GetSize(object value)
			{
				return GetSize((ulong)value);
			}

			public static int GetSize(ulong value)
			{
				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.value = value;

				return LongType.GetSize(mapper.longValue);
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static ulong Read(byte[] data, ref int startIndex)
			{
				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.longValue = LongType.Read(data, ref startIndex);

				return mapper.value;
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((ulong)value, data, ref startIndex);
			}

			public static void Write(ulong value, byte[] data, ref int startIndex)
			{
				ULongTypeMapper mapper = new ULongTypeMapper();
				mapper.value = value;

				LongType.Write(mapper.longValue, data, ref startIndex);
			}
		}
	}
}
