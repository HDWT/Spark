using System;
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
			public int GetSize(byte value)
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

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((byte)value, data, ref startIndex);
			}

			public void Write(byte value, byte[] data, ref int startIndex)
			{
				data[startIndex++] = value;
			}
		}
	}
}