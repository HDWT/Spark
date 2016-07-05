using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		public struct CharMapper
		{
			[FieldOffset(0)]
			public char value;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;
		}

		private class CharType : ITypeHelper<char>
		{
			private const char CharZero = (char)0x00;

			public int GetSize(object value)
			{
				return GetSize((char)value);
			}

			public int GetSize(char value)
			{
				if (value == CharZero)
					return 1;

				CharMapper mapper = new CharMapper();
				mapper.value = value;

				if (mapper.byte2 != zero)
					return 3;

				if (mapper.byte1 != zero)
					return 2;

				throw new System.ArgumentException();
			}

			public char FromBytes(byte[] data, int startIndex)
			{
				CharMapper mapper = new CharMapper();

				mapper.byte1 = data[startIndex++];
				mapper.byte2 = data[startIndex++];

				return mapper.value;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public char Read(byte[] data, ref int startIndex)
			{
				byte dataSize = data[startIndex++];

				if (dataSize == zero)
					return CharZero;

				CharMapper mapper = new CharMapper();

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

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((char)value, data, ref startIndex);
			}

			public void Write(char value, byte[] data, ref int startIndex)
			{
				if (value == CharZero)
				{
					data[startIndex++] = zero;
					return;
				}

				CharMapper mapper = new CharMapper();
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