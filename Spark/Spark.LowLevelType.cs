using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private const byte IgnoreDataSizeBlockMark = 128;

	private class LowLevelType<T>
	{
		private static GetSizeDelegate<T> m_getSize = null;
		private static WriteDataDelegate<T> m_writeData = null;

		private LowLevelType()
		{ }

		public static void Create(GetSizeDelegate<T> getSize, WriteDataDelegate<T> writeData)
		{
			m_getSize = getSize;
			m_writeData = writeData;
		}

		public static int GetSize(T value)
		{
			if (m_getSize == null)
				throw new NotImplementedException("Get Size for " + typeof(T) + " type not implemented");

			return m_getSize(value);
		}

		public static void Write(T value, byte[] data, ref int startIndex)
		{
			if (m_writeData == null)
				throw new NotImplementedException("Writer for " + typeof(T).Name + " type not implemented");

			m_writeData(value, data, ref startIndex);
		}
	}

	private static void InitLowLevelTypes()
	{
		LowLevelType<bool>.Create(TypeHelper.Bool.GetSize, TypeHelper.Bool.Write);
		LowLevelType<byte>.Create(TypeHelper.Byte.GetSize, TypeHelper.Byte.Write);
		LowLevelType<sbyte>.Create(TypeHelper.SByte.GetSize, TypeHelper.SByte.Write);
		LowLevelType<char>.Create(TypeHelper.Char.GetSize, TypeHelper.Char.Write);
		LowLevelType<short>.Create(TypeHelper.Short.GetSize, TypeHelper.Short.Write);
		LowLevelType<ushort>.Create(TypeHelper.UShort.GetSize, TypeHelper.UShort.Write);
		LowLevelType<int>.Create(TypeHelper.Int.GetSize, TypeHelper.Int.Write);// BasicTypeHelper.Instance.Write);
		LowLevelType<uint>.Create(TypeHelper.UInt.GetSize, TypeHelper.UInt.Write);
		LowLevelType<long>.Create(TypeHelper.Long.GetSize, TypeHelper.Long.Write);
		LowLevelType<ulong>.Create(TypeHelper.ULong.GetSize, TypeHelper.ULong.Write);
		LowLevelType<float>.Create(TypeHelper.Float.GetSize, TypeHelper.Float.Write);
		LowLevelType<double>.Create(TypeHelper.Double.GetSize, TypeHelper.Double.Write);
		LowLevelType<decimal>.Create(TypeHelper.Decimal.GetSize, TypeHelper.Decimal.Write);
		LowLevelType<string>.Create(TypeHelper.String.GetSize, TypeHelper.String.Write);
		LowLevelType<DateTime>.Create(SizeCalculator.Evaluate, Writer.Write);
		LowLevelType<Array>.Create(SizeCalculator.Evaluate, Writer.Write);
	}
}