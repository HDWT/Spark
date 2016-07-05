using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private class BoolType : ITypeHelper<bool>
		{
			public int GetSize(object value)
			{
				return GetSize((bool)value);
			}

			public int GetSize(bool value)
			{
				return (value == false) ? 1 : 2;
			}

			public bool FromBytes(byte[] data, int startIndex)
			{
				return data[startIndex++] != 0;
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public bool Read(byte[] data, ref int startIndex)
			{
				byte dataSize = data[startIndex++];

				if (dataSize == zero)
					return false;

				if (dataSize == one)
					return data[startIndex++] != zero;

				throw new System.ArgumentException(string.Format("Spark.Read - Invalid data size = {0}", dataSize));
			}

			public void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((bool)value, data, ref startIndex);
			}

			public void Write(bool value, byte[] data, ref int startIndex)
			{
				if (value)
				{
					data[startIndex++] = one;
					data[startIndex++] = one;
				}
				else
				{
					data[startIndex++] = zero;
				}
			}
		}
	}
}