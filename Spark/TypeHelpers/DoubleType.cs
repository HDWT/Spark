using System;
using System.Collections.Generic;
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

		private class DoubleType : ITypeHelper<double>
		{
			public int GetSize(object value)
			{
				return GetSize((double)value);
			}

			public int GetSize(double value)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.value = value;

				return TypeHelper.ULong.GetSize(mapper.ulongValue);
			}

			public double FromBytes(byte[] data, int startIndex)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.ulongValue = TypeHelper.ULong.FromBytes(data, startIndex);

				return mapper.value;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public double Read(byte[] data, ref int startIndex)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.ulongValue = TypeHelper.ULong.Read(data, ref startIndex);

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((double)value, data, ref startIndex);
			}

			public void Write(double value, byte[] data, ref int startIndex)
			{
				DoubleTypeMapper mapper = new DoubleTypeMapper();
				mapper.value = value;

				TypeHelper.ULong.Write(mapper.ulongValue, data, ref startIndex);
			}
		}
	}
}
