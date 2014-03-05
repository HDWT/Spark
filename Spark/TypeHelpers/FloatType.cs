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
			public uint uintValue;

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
			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((float)value, sizes);
			}

			public int GetSize(float value, LinkedList<int> sizes)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.value = value;

				return TypeHelper.UInt.GetSize(mapper.uintValue, sizes);
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public float Read(byte[] data, ref int startIndex)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.uintValue = TypeHelper.UInt.Read(data, ref startIndex);

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((float)value, data, ref startIndex, sizes);
			}

			public void Write(float value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				FloatTypeMapper mapper = new FloatTypeMapper();
				mapper.value = value;

				TypeHelper.UInt.Write(mapper.uintValue, data, ref startIndex, sizes);
			}
		}
	}
}
