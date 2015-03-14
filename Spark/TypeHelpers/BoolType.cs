using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private class BoolType : ITypeHelper<bool>
		{
			private static byte zero = 0;
			private static byte one = 1;

			public int GetSize(object value)
			{
				return 1;
			}

			public int GetSize(bool value)
			{
				return 1;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public bool Read(byte[] data, ref int startIndex)
			{
				return data[startIndex++] != zero;
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((bool)value, data, ref startIndex);
			}

			public void Write(bool value, byte[] data, ref int startIndex)
			{
				data[startIndex++] = value ? one : zero;
			}
		}
	}
}