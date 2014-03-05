using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		public struct ArrayMapper
		{
			[FieldOffset(0)]
			public byte[] byteArray;

			[FieldOffset(0)]
			public char[] charArray;
		}

		private class ByteType : ITypeHelper<byte>
		{
			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((byte)value, sizes);
			}

			public int GetSize(byte value, LinkedList<int> sizes)
			{
				return 1;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public byte Read(byte[] data, ref int startIndex)
			{
				return data[startIndex++];
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((byte)value, data, ref startIndex, sizes);
			}

			public void Write(byte value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				data[startIndex++] = value;
			}
		}
	}
}