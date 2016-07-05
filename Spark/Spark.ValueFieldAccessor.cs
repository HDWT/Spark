using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	[StructLayout(LayoutKind.Explicit)]
	private struct ValueFieldAccessor
	{
		private const int MaxFieldSize = sizeof(decimal);
		private static readonly int FieldOffset = Is64Bit ? 8 : 4;

		[FieldOffset(0)]
		public object instance;

		[FieldOffset(0)]
		private byte[] m_bytes;

		[FieldOffset(0)]
		private ArrayFields m_arrayFields;

		[StructLayout(LayoutKind.Explicit)]
		private class ArrayFields
		{
			[FieldOffset(0)]
			public int length0;

			[FieldOffset(4)]
			public int length4;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;

			[FieldOffset(2)]
			public byte byte3;

			[FieldOffset(3)]
			public byte byte4;

			[FieldOffset(4)]
			public byte byte5;

			[FieldOffset(5)]
			public byte byte6;

			[FieldOffset(6)]
			public byte byte7;

			[FieldOffset(7)]
			public byte byte8;
		}

		public object Get(Type fieldType, int offset)
		{
			byte[] bytes = null;
			int realLength = 0;

			if (offset < FieldOffset)
			{
				bytes = new byte[MaxFieldSize];

				bytes[0] = m_arrayFields.byte1;
				bytes[1] = m_arrayFields.byte2;
				bytes[2] = m_arrayFields.byte3;
				bytes[3] = m_arrayFields.byte4;
				bytes[4] = m_arrayFields.byte5;
				bytes[5] = m_arrayFields.byte6;
				bytes[6] = m_arrayFields.byte7;
				bytes[7] = m_arrayFields.byte8;
			}
			else
			{
				offset -= FieldOffset;
			}

			if (Is64Bit)
			{
				realLength = m_arrayFields.length4;
				m_arrayFields.length4 = offset + MaxFieldSize;
			}
			else
			{
				realLength = m_arrayFields.length0;
				m_arrayFields.length0 = offset + MaxFieldSize;
			}

			if (bytes != null)
			{
				for (int i = 0; (i < m_arrayFields.length0) && (i + FieldOffset < bytes.Length); ++i)
					bytes[i + FieldOffset] = m_bytes[i];
			}
			else
			{
				bytes = m_bytes;
			}

			//foreach (var v in bytes)
			//	Console.WriteLine(v);

			System.Type enumUnderlyingType = null;
			object toRet = null;

			if (EnumTypeHelper.Instance.TryGetUnderlyingType(fieldType, out enumUnderlyingType))
				fieldType = enumUnderlyingType;

			if (fieldType == typeof(int))
				toRet = TypeHelper.IntType.FromBytes(bytes, offset);

			else if (fieldType == typeof(float))
				toRet = TypeHelper.Float.FromBytes(bytes, offset);

			else if (fieldType == typeof(bool))
				toRet = TypeHelper.Bool.FromBytes(bytes, offset);

			else if (fieldType == typeof(char))
				toRet = TypeHelper.Char.FromBytes(bytes, offset);

			else if (fieldType == typeof(long))
				toRet = TypeHelper.Long.FromBytes(bytes, offset);

			else if (fieldType == typeof(short))
				toRet = TypeHelper.Short.FromBytes(bytes, offset);

			else if (fieldType == typeof(byte))
				toRet = TypeHelper.Byte.FromBytes(bytes, offset);

			else if (fieldType == typeof(DateTime))
				toRet = TypeHelper.DateTime.FromBytes(bytes, offset);

			else if (fieldType == typeof(double))
				toRet = TypeHelper.Double.FromBytes(bytes, offset);

			else if (fieldType == typeof(uint))
				toRet = TypeHelper.UInt.FromBytes(bytes, offset);

			else if (fieldType == typeof(ushort))
				toRet = TypeHelper.UShort.FromBytes(bytes, offset);

			else if (fieldType == typeof(ulong))
				toRet = TypeHelper.ULong.FromBytes(bytes, offset);

			else if (fieldType == typeof(sbyte))
				toRet = TypeHelper.SByte.FromBytes(bytes, offset);

			else if (fieldType == typeof(decimal))
				toRet = TypeHelper.Decimal.FromBytes(bytes, offset);

			else throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", fieldType));

			//
			if (Is64Bit)
				m_arrayFields.length4 = realLength;
			else
				m_arrayFields.length0 = realLength;

			return toRet;
		}
	}
}