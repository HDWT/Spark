using System;
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

		public static class FloatType
		{
			public static int GetSize(object value)
			{
				return GetSize((float)value);
			}

			public static int GetSize(float value)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.value = value;

				return IntType.GetSize(mapper.intValue);
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static float Read(byte[] data, ref int startIndex)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.intValue = IntType.Read(data, ref startIndex);

				return mapper.value;
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((float)value, data, ref startIndex);
			}

			public static void Write(float value, byte[] data, ref int startIndex)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.value = value;

				IntType.Write(mapper.intValue, data, ref startIndex);
			}
		}
	}
}
