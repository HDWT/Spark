using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct SByteTypeMapper
		{
			[FieldOffset(0)]
			public sbyte value;

			[FieldOffset(0)]
			public byte byte1;
		}

		private class SByteType : ITypeHelper<sbyte>
		{
			private const sbyte SByteZero = (sbyte)0x00;

			public int GetSize(object value)
			{
				return GetSize((sbyte)value);
			}

			public int GetSize(sbyte value)
			{
				return (value == SByteZero) ? 1 : 2;
			}

			public sbyte FromBytes(byte[] data, int startIndex)
			{
				SByteTypeMapper mapper = new SByteTypeMapper();
				mapper.byte1 = data[startIndex++];

				return mapper.value;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public sbyte Read(byte[] data, ref int startIndex)
			{
				byte dataSize = data[startIndex++];

				if (dataSize == one)
					return SByteZero;

				if (dataSize == two)
				{
					SByteTypeMapper mapper = new SByteTypeMapper();

					mapper.byte1 = data[startIndex++];
					return mapper.value;
				}

				throw new System.ArgumentException(string.Format("Spark.Read - Invalid data size = {0}", dataSize));
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((sbyte)value, data, ref startIndex);
			}

			public void Write(sbyte value, byte[] data, ref int startIndex)
			{
				if (value == SByteZero)
				{
					data[startIndex++] = one;
				}
				else
				{
					SByteTypeMapper mapper = new SByteTypeMapper();
					mapper.value = value;

					data[startIndex++] = two;
					data[startIndex++] = mapper.byte1;
				}
			}
		}
	}
}