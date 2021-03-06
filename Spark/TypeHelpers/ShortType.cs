﻿using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct ShortTypeMapper
		{
			[FieldOffset(0)]
			public short value;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;
		}

		public static class ShortType
		{
			private const short ShortZero = (short)0x00;

			public static int GetSize(object value)
			{
				return GetSize((short)value);
			}

			public static int GetSize(short value)
			{
				if (value == ShortZero)
					return 1;

				ShortTypeMapper mapper = new ShortTypeMapper();
				mapper.value = value;

				if (mapper.byte2 != zero)
					return 3;

				if (mapper.byte1 != zero)
					return 2;

				throw new System.ArgumentException();
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static short Read(byte[] data, ref int startIndex)
			{
				byte dataSize = data[startIndex++];

				if (dataSize == zero)
					return ShortZero;

				ShortTypeMapper mapper = new ShortTypeMapper();

				if (dataSize == two)
				{
					mapper.byte1 = data[startIndex++];
					mapper.byte2 = data[startIndex++];
				}
				else if (dataSize == one)
				{
					mapper.byte1 = data[startIndex++];
				}
				else
				{
					throw new System.ArgumentException(string.Format("Spark.Read - Invalid data size = {0}", dataSize));
				}

				return mapper.value;
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((short)value, data, ref startIndex);
			}

			public static void Write(short value, byte[] data, ref int startIndex)
			{
				if (value == ShortZero)
				{
					data[startIndex++] = zero;
					return;
				}

				ShortTypeMapper mapper = new ShortTypeMapper();
				mapper.value = value;

				if (mapper.byte2 != zero)
				{
					data[startIndex++] = two;
					data[startIndex++] = mapper.byte1;
					data[startIndex++] = mapper.byte2;
				}
				else if (mapper.byte1 != zero)
				{
					data[startIndex++] = one;
					data[startIndex++] = mapper.byte1;
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}
	}
}