using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct FloatTypeMapper
		{
			[FieldOffset(0)]
			public float value;

			[FieldOffset(0)]
			public int intValue;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;

			[FieldOffset(2)]
			public byte byte3;

			[FieldOffset(3)]
			public byte byte4;
		}

		private class FloatType : ITypeHelper<float>
		{
			public int GetSize(object value)
			{
				return GetSize((float)value);
			}

			public int GetSize(float value)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.value = value;

				return TypeHelper.IntType.GetSize(mapper.intValue);
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public float Read(byte[] data, ref int startIndex)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.intValue = TypeHelper.IntType.Read(data, ref startIndex);

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((float)value, data, ref startIndex);
			}

			public void Write(float value, byte[] data, ref int startIndex)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.value = value;

				TypeHelper.IntType.Write(mapper.intValue, data, ref startIndex);
			}
		}
	}
}
