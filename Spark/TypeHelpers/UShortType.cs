using System;
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

		public static class UShortType
		{
			private const ushort UShortZero = (ushort)0x00;

			public static int GetSize(object value)
			{
				return GetSize((ushort)value);
			}

			public static int GetSize(ushort value)
			{
				if (value == UShortZero)
					return 1;

				UShortTypeMapper mapper = new UShortTypeMapper();
				mapper.value = value;

				if (mapper.byte2 != zero)
					return 3;

				if (mapper.byte1 != zero)
					return 2;

				throw new System.ArgumentException();
			}

			public static ushort Read(byte[] data, ref int startIndex)
			{
				int dataSize = data[startIndex++];

				if (dataSize == zero)
					return UShortZero;

				UShortTypeMapper mapper = new UShortTypeMapper();

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

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((ushort)value, data, ref startIndex);
			}

			public static void Write(ushort value, byte[] data, ref int startIndex)
			{
				if (value == UShortZero)
				{
					data[startIndex++] = zero;
					return;
				}

				UShortTypeMapper mapper = new UShortTypeMapper();
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