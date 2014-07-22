using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	[StructLayout(LayoutKind.Explicit)]
	private struct ValueFieldAccessor
	{
		private const int MaxFieldSize = sizeof(decimal);

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
			public int length;

			[FieldOffset(0)]
			public byte byte1;

			[FieldOffset(1)]
			public byte byte2;

			[FieldOffset(2)]
			public byte byte3;

			[FieldOffset(3)]
			public byte byte4;
		}

		public object Get(Type fieldType, int offset)
		{
			byte[] bytes = null;

			if (offset < 4)
			{
				bytes = new byte[MaxFieldSize + 60];

				bytes[0] = m_arrayFields.byte1;
				bytes[1] = m_arrayFields.byte2;
				bytes[2] = m_arrayFields.byte3;
				bytes[3] = m_arrayFields.byte4;
			}
			else
			{
				offset -= 4;
			}

			var realLength = m_arrayFields.length;
			m_arrayFields.length = offset + MaxFieldSize + 100;

			if (bytes != null)
			{
				for (int i = 0; (i < m_arrayFields.length) && (i + 4 < bytes.Length); ++i)
					bytes[i + 4] = m_bytes[i];
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

			m_arrayFields.length = realLength;
			return toRet;
		}
	}
}