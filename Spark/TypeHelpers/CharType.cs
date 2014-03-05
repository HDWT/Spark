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
			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((char)value, sizes);
			}

			public int GetSize(char value, LinkedList<int> sizes)
			{
				return 2;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public char Read(byte[] data, ref int startIndex)
			{
				CharMapper mapper = new CharMapper();

				mapper.byte1 = data[startIndex++];
				mapper.byte2 = data[startIndex++];

				return mapper.value;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((char)value, data, ref startIndex, sizes);
			}

			public void Write(char value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				CharMapper mapper = new CharMapper();
				mapper.value = value;

				data[startIndex++] = mapper.byte1;
				data[startIndex++] = mapper.byte2;
			}
		}
	}
}