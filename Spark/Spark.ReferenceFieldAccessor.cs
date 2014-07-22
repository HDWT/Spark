using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	[StructLayout(LayoutKind.Explicit)]
	private struct ReferenceFieldAccessor
	{
		[FieldOffset(0)]
		public object instance;

		[FieldOffset(0)]
		private object[] m_fields;

		[FieldOffset(0)]
		private ArrayFields m_arrayFields;

		[FieldOffset(0)]
		private ArrayElements m_arrayElements;

		[StructLayout(LayoutKind.Explicit)]
		private class ArrayFields
		{
			[FieldOffset(0)]
			public int length;
		}

		[StructLayout(LayoutKind.Explicit)]
		private class ArrayElements
		{
			[FieldOffset(0)]
			public object first;

			[FieldOffset(IntPtrSize)]
			public object second;
		}

		public object Get(int fieldIndex)
		{
			if (instance == null)
				return null;

			object toRet = null;

			if (fieldIndex == 0)
			{
				toRet = m_arrayElements.first;
			}
			else if (fieldIndex == 1)
			{
				toRet = m_arrayElements.second;
			}
			else
			{
				int realLength = m_arrayFields.length;
				m_arrayFields.length = fieldIndex;

				toRet = m_fields[fieldIndex - 2];
				m_arrayFields.length = realLength;
			}

			return toRet;
		}
	}
}