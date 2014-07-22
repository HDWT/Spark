﻿using System;
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
			public int GetSize(object value)
			{
				return GetSize((long)value);
			}

			public int GetSize(long value)
			{
				LongTypeMapper mapper = new LongTypeMapper();
				mapper.value = value;

				return TypeHelper.ULong.GetSize(mapper.ulongValue);
			}

			public long FromBytes(byte[] data, int startIndex)
			{
				LongTypeMapper mapper = new LongTypeMapper();

				mapper.byte1 = data[startIndex++];
				mapper.byte2 = data[startIndex++];
				mapper.byte3 = data[startIndex++];
				mapper.byte4 = data[startIndex++];
				mapper.byte5 = data[startIndex++];
				mapper.byte6 = data[startIndex++];
				mapper.byte7 = data[startIndex++];
				mapper.byte8 = data[startIndex++];

				//mapper.ulongValue = TypeHelper.ULong.FromBytes(data, startIndex);

				return mapper.value;
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

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((long)value, data, ref startIndex);
			}

			public void Write(long value, byte[] data, ref int startIndex)
			{
				LongTypeMapper mapper = new LongTypeMapper();
				mapper.value = value;

				TypeHelper.ULong.Write(mapper.ulongValue, data, ref startIndex);
			}
		}
	}
}
