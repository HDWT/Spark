using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleTypeMapper
		{
			[FieldOffset(0)]
			public double value;

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

		public static class DoubleType
		{
			public static int GetSize(object value)
			{
				return GetSize((double)value);
			}

			public static int GetSize(double value)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.value = value;

				return LongType.GetSize(mapper.longValue);
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static double Read(byte[] data, ref int startIndex)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.longValue = LongType.Read(data, ref startIndex);

				return mapper.value;
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((double)value, data, ref startIndex);
			}

			public static void Write(double value, byte[] data, ref int startIndex)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.value = value;

				LongType.Write(mapper.longValue, data, ref startIndex);
			}
		}
	}
}
