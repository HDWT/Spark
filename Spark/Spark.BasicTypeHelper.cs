using System;
using System.Runtime.InteropServices;


public static partial class Spark
{
	[StructLayout(LayoutKind.Explicit)]
	private class BasicTypeHelper
	{
		public static readonly BasicTypeHelper Instance = new BasicTypeHelper();

		#region --- Basic Types ---

		[FieldOffset(0)]
		private bool m_bool;

		[FieldOffset(0)]
		private byte m_byte;

		[FieldOffset(0)]
		private sbyte m_sbyte;

		[FieldOffset(0)]
		private char m_char;

		[FieldOffset(0)]
		private short m_short;

		[FieldOffset(0)]
		private ushort m_ushort;

		[FieldOffset(0)]
		private int m_int;

		[FieldOffset(0)]
		private uint m_uint;

		[FieldOffset(0)]
		private long m_long;

		[FieldOffset(0)]
		private ulong m_ulong;

		[FieldOffset(0)]
		private float m_float;

		[FieldOffset(0)]
		private double m_double;

		[FieldOffset(0)]
		private decimal m_decimal;

		#endregion

		#region --- Bytes ---

		[FieldOffset(0)]
		private byte m_byte0;

		[FieldOffset(1)]
		private byte m_byte1;

		[FieldOffset(2)]
		private byte m_byte2;

		[FieldOffset(3)]
		private byte m_byte3;

		[FieldOffset(4)]
		private byte m_byte4;

		[FieldOffset(5)]
		private byte m_byte5;

		[FieldOffset(6)]
		private byte m_byte6;

		[FieldOffset(7)]
		private byte m_byte7;

		[FieldOffset(8)]
		private byte m_byte8;

		[FieldOffset(9)]
		private byte m_byte9;

		[FieldOffset(10)]
		private byte m_byte10;

		[FieldOffset(11)]
		private byte m_byte11;

		[FieldOffset(12)]
		private byte m_byte12;

		[FieldOffset(13)]
		private byte m_byte13;

		[FieldOffset(14)]
		private byte m_byte14;

		[FieldOffset(15)]
		private byte m_byte15;

		#endregion

		#region --- Private Variables ---

		[FieldOffset(16)]
		private int m_size;

		#endregion

		private bool Bool { get { return m_bool; } set { ResetValue(); m_bool = value; m_size = sizeof(bool); } }
		private byte Byte { get { return m_byte; } set { ResetValue(); m_byte = value; m_size = sizeof(byte); } }
		private sbyte SByte { get { return m_sbyte; } set { ResetValue(); m_sbyte = value; m_size = sizeof(sbyte); } }
		private char Char { get { return m_char; } set { ResetValue(); m_char = value; m_size = sizeof(char); } }
		private short Short { get { return m_short; } set { ResetValue(); m_short = value; m_size = sizeof(short); } }
		private ushort UShort { get { return m_ushort; } set { ResetValue(); m_ushort = value; m_size = sizeof(ushort); } }
		private int Int { get { return m_int; } set { ResetValue(); m_int = value; m_size = sizeof(int); } }
		private uint UInt { get { return m_uint; } set { ResetValue(); m_uint = value; m_size = sizeof(uint); } }
		private long Long { get { return m_long; } set { ResetValue(); m_long = value; m_size = sizeof(long); } }
		private ulong ULong { get { return m_ulong; } set { ResetValue(); m_ulong = value; m_size = sizeof(ulong); } }
		private float Float { get { return m_float; } set { ResetValue(); m_float = value; m_size = sizeof(float); } }
		private double Double { get { return m_double; } set { ResetValue(); m_double = value; m_size = sizeof(double); } }
		private decimal Decimal { get { return m_decimal; } set { ResetValue(); m_decimal = value; m_size = sizeof(decimal); } }

		private byte this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_byte0;
					case 1: return m_byte1;
					case 2: return m_byte2;
					case 3: return m_byte3;
					case 4: return m_byte4;
					case 5: return m_byte5;
					case 6: return m_byte6;
					case 7: return m_byte7;
					case 8: return m_byte8;
					case 9: return m_byte9;
					case 10: return m_byte10;
					case 11: return m_byte11;
					case 12: return m_byte12;
					case 13: return m_byte13;
					case 14: return m_byte14;
					case 15: return m_byte15;

					default: throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (index)
				{
					case 0: m_byte0 = value; break;
					case 1: m_byte1 = value; break;
					case 2: m_byte2 = value; break;
					case 3: m_byte3 = value; break;
					case 4: m_byte4 = value; break;
					case 5: m_byte5 = value; break;
					case 6: m_byte6 = value; break;
					case 7: m_byte7 = value; break;
					case 8: m_byte8 = value; break;
					case 9: m_byte9 = value; break;
					case 10: m_byte10 = value; break;
					case 11: m_byte11 = value; break;
					case 12: m_byte12 = value; break;
					case 13: m_byte13 = value; break;
					case 14: m_byte14 = value; break;
					case 15: m_byte15 = value; break;

					default: throw new IndexOutOfRangeException();
				}
			}
		}

		public byte GetMinSize(bool value)
		{
			Bool = value;
			return GetMinSize();
		}

		public byte GetMinSize(byte value)
		{
			Byte = value;
			return GetMinSize();
		}

		public byte GetMinSize(sbyte value)
		{
			SByte = value;
			return GetMinSize();
		}

		public byte GetMinSize(char value)
		{
			Char = value;
			return GetMinSize();
		}

		public short GetMinSize(short value)
		{
			Short = value;
			return GetMinSize();
		}

		public ushort GetMinSize(ushort value)
		{
			UShort = value;
			return GetMinSize();
		}

		public int GetMinSize(int value)
		{
			Int = value;
			return GetIntMinSize();
			//return GetMinSize();
		}

		public byte GetMinSize(uint value)
		{
			UInt = value;
			return GetMinSize();
		}

		public byte GetMinSize(long value)
		{
			Long = value;
			return GetMinSize();
		}

		public byte GetMinSize(ulong value)
		{
			ULong = value;
			return GetMinSize();
		}

		public byte GetMinSize(float value)
		{
			Float = value;
			return GetMinSize();
		}

		public byte GetMinSize(double value)
		{
			Double = value;
			return GetMinSize();
		}

		public byte GetMinSize(decimal value)
		{
			Decimal = value;
			return GetMinSize();
		}

		//[FieldOffset(15)]
		static byte[] sizes = { 0, 1, 2, 3, 4 };
		static byte zero = 0;

		private int GetIntMinSize() // As fast as a hare
		{
			if (m_byte3 != zero)
				return 4;// sizes[4];

			if (m_byte2 != zero)
				return 3;// sizes[3];

			if (m_byte1 != zero)
				return 2; // sizes[2];

			if (m_byte0 != zero)
				return 1; // sizes[1];

			return 0;// sizes[0];
		}

		private byte GetMinSize()
		{
			for (int i = m_size - 1; i >= 0; --i)
			{
				if (this[i] != 0)
					return (byte)(i + 1);
			}

			return 0;
		}

		private void ResetValue()
		{
			m_decimal = 0; // Reset all 16 bytes
		}

		#region --- Readers ---

		public object ReadBool(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_bool;
		}

		public object ReadByte(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_byte;
		}

		public object ReadSByte(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_sbyte;
		}

		public object ReadChar(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_char;
		}

		public object ReadShort(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_short;
		}

		public object ReadUShort(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_ushort;
		}

		public object ReadInt(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_int;
		}

		public object ReadUInt(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_uint;
		}

		public object ReadLong(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_long;
		}

		public object ReadULong(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_ulong;
		}

		public object ReadFloat(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_float;
		}

		public object ReadDouble(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_double;
		}

		public object ReadDecimal(Type type, byte[] data, ref int startIndex)
		{
			ReadValue(data, ref startIndex);
			return m_decimal;
		}

		private void ReadValue(byte[] data, ref int startIndex)
		{
			ResetValue();

			int dataSize = data[startIndex++];

			for (int i = 0; i < dataSize; ++i)
				this[i] = data[startIndex++];
		}

		#endregion

		#region --- Writers ---

		public void Write(bool value, byte[] array, ref int startIndex)
		{
			Bool = (bool)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(byte value, byte[] array, ref int startIndex)
		{
			Byte = (byte)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(sbyte value, byte[] array, ref int startIndex)
		{
			SByte = (sbyte)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(char value, byte[] array, ref int startIndex)
		{
			Char = (char)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(short value, byte[] array, ref int startIndex)
		{
			Short = (short)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(ushort value, byte[] array, ref int startIndex)
		{
			UShort = (ushort)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(int value, byte[] array, ref int startIndex)
		{
			Int = (int)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(uint value, byte[] array, ref int startIndex)
		{
			UInt = (uint)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(long value, byte[] array, ref int startIndex)
		{
			Long = (long)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(ulong value, byte[] array, ref int startIndex)
		{
			ULong = (ulong)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(float value, byte[] array, ref int startIndex)
		{
			Float = (float)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(double value, byte[] array, ref int startIndex)
		{
			Double = (double)value;
			WriteValue(array, ref startIndex);
		}

		public void Write(decimal value, byte[] array, ref int startIndex)
		{
			Decimal = (decimal)value;
			WriteValue(array, ref startIndex);
		}

		// Object
		public void WriteBool(object value, byte[] array, ref int startIndex)
		{
			Bool = (bool)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteByte(object value, byte[] array, ref int startIndex)
		{
			Byte = (byte)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteSByte(object value, byte[] array, ref int startIndex)
		{
			SByte = (sbyte)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteChar(object value, byte[] array, ref int startIndex)
		{
			Char = (char)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteShort(object value, byte[] array, ref int startIndex)
		{
			Short = (short)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteUShort(object value, byte[] array, ref int startIndex)
		{
			UShort = (ushort)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteInt(object value, byte[] array, ref int startIndex)
		{
			Int = (int)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteUInt(object value, byte[] array, ref int startIndex)
		{
			UInt = (uint)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteLong(object value, byte[] array, ref int startIndex)
		{
			Long = (long)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteULong(object value, byte[] array, ref int startIndex)
		{
			ULong = (ulong)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteFloat(object value, byte[] array, ref int startIndex)
		{
			Float = (float)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteDouble(object value, byte[] array, ref int startIndex)
		{
			Double = (double)value;
			WriteValue(array, ref startIndex);
		}

		public void WriteDecimal(object value, byte[] array, ref int startIndex)
		{
			Decimal = (decimal)value;
			WriteValue(array, ref startIndex);
		}

		private void WriteValue(byte[] array, ref int startIndex)
		{
			byte minSize = GetMinSize();

			array[startIndex++] = minSize;

			for (int i = 0; i < minSize; ++i)
				array[startIndex++] = this[i];
		}

		public static bool TryGetWriter(Type type, out WriteDataDelegate writer)
		{
			writer = null;

			if (type.IsValueType)
			{
				if (type == typeof(int))
					writer = Instance.WriteInt;

				else if (type == typeof(float))
					writer = Instance.WriteFloat;

				else if (type == typeof(bool))
					writer = Instance.WriteBool;

				else if (type == typeof(char))
					writer = Instance.WriteChar;

				else if (type == typeof(long))
					writer = Instance.WriteLong;

				else if (type == typeof(short))
					writer = Instance.WriteShort;

				else if (type == typeof(byte))
					writer = Instance.WriteByte;

				else if (type == typeof(double))
					writer = Instance.WriteDouble;

				else if (type == typeof(uint))
					writer = Instance.WriteUInt;

				else if (type == typeof(ushort))
					writer = Instance.WriteUShort;

				else if (type == typeof(ulong))
					writer = Instance.WriteULong;

				else if (type == typeof(sbyte))
					writer = Instance.WriteSByte;

				else if (type == typeof(decimal))
					writer = Instance.WriteDecimal;
			}

			return (writer != null);
		}

		public void WriteArray(int[] array, byte[] data, ref int startIndex)
		{
			decimal tempValue = Decimal;

			//WriteDataDelegate writer = null;

			//if (!TryGetWriter(typeof(T), out writer))
			//	throw new NotImplementedException();

			for (int i = 0; i < array.Length; ++i)
			{
				Int = array[i];

				byte minSize = GetMinSize();

				data[startIndex++] = minSize;

				for (int j = 0; j < minSize; ++j)
					data[startIndex++] = this[j];
			}

			Decimal = tempValue; // Restore value
		}

		#endregion
	}
}