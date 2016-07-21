using System;
using System.Reflection;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

	[StructLayout(LayoutKind.Explicit)]
	private struct FieldAccessor
	{
		[FieldOffset(0)]
		public ObjectWrapper wObject;

		[FieldOffset(0)]
		public AddressWrapper wAddress;

		[StructLayout(LayoutKind.Explicit)]
		public class ObjectWrapper
		{
			[FieldOffset(0)]
			public object instance;

			[FieldOffset(0)]
			public ValueFields fields;

			[FieldOffset(0)]
			public ReferenceFields referenceFields;
		}

		[StructLayout(LayoutKind.Explicit)]
		public class AddressWrapper
		{
			[FieldOffset(0)]
			public ulong address64;

			[FieldOffset(0)]
			public uint address32;
		}

		[StructLayout(LayoutKind.Explicit)]
		public class ReferenceFields
		{
			[FieldOffset(0)]
			public object Object;

			[FieldOffset(0)]
			public string String;
		}

		[StructLayout(LayoutKind.Explicit)]
		public class ValueFields
		{
			[FieldOffset(0)]
			public decimal Decimal;

			[FieldOffset(0)]
			public double Double;

			[FieldOffset(0)]
			public long Long;

			[FieldOffset(0)]
			public ulong ULong;

			[FieldOffset(0)]
			public float Float;

			[FieldOffset(0)]
			public int Int;

			[FieldOffset(0)]
			public uint UInt;

			[FieldOffset(0)]
			public short Short;

			[FieldOffset(0)]
			public ushort UShort;

			[FieldOffset(0)]
			public char Char;

			[FieldOffset(0)]
			public byte Byte;

			[FieldOffset(0)]
			public sbyte SByte;

			[FieldOffset(0)]
			public bool Bool;


			[FieldOffset(0)]
			public byte Byte1;

			[FieldOffset(1)]
			public byte Byte2;

			[FieldOffset(2)]
			public byte Byte3;

			[FieldOffset(3)]
			public byte Byte4;

			[FieldOffset(4)]
			public byte Byte5;

			[FieldOffset(5)]
			public byte Byte6;

			[FieldOffset(6)]
			public byte Byte7;

			[FieldOffset(7)]
			public byte Byte8;

			[FieldOffset(8)]
			public byte Byte9;

			[FieldOffset(9)]
			public byte Byte10;

			[FieldOffset(10)]
			public byte Byte11;

			[FieldOffset(11)]
			public byte Byte12;

			[FieldOffset(12)]
			public byte Byte13;

			[FieldOffset(13)]
			public byte Byte14;

			[FieldOffset(14)]
			public byte Byte15;

			[FieldOffset(15)]
			public byte Byte16;
		}

		public uint GetOffset(FieldInfo fieldInfo)
		{
			wObject.instance = fieldInfo.FieldHandle;

			if (IntPtr.Size == 8)
			{
				wAddress.address64 = wObject.fields.ULong;

				if (IsMono)
				{
					wAddress.address64 += 8;
					return (uint)(wObject.fields.UShort - IntPtr.Size * 2);
				}
				else
				{
					wAddress.address64 += 4;
					return wObject.fields.UShort;
				}
			}
			else
			{
				wAddress.address32 = wObject.fields.UInt;

				if (IsMono)
				{
					wAddress.address32 += 8;
					return (uint)(wObject.fields.UShort - IntPtr.Size * 2);
				}
				else
				{
					wAddress.address32 += 4;
					return wObject.fields.UShort;
				}
			}
		}

		public object Get(Type fieldType, bool isReferenceType)
		{
			if (isReferenceType)
				return wObject.referenceFields.Object;

			System.Type enumUnderlyingType = null;
			object toRet = null;

			if (EnumTypeHelper.Instance.TryGetUnderlyingType(fieldType, out enumUnderlyingType))
				fieldType = enumUnderlyingType;

			if (fieldType == typeof(int))
				toRet = wObject.fields.Int;

			else if (fieldType == typeof(float))
				toRet = wObject.fields.Float;

			else if (fieldType == typeof(bool))
				toRet = wObject.fields.Bool;

			else if (fieldType == typeof(char))
				toRet = wObject.fields.Char;

			else if (fieldType == typeof(long))
				toRet = wObject.fields.Long;

			else if (fieldType == typeof(short))
				toRet = wObject.fields.Short;

			else if (fieldType == typeof(byte))
				toRet = wObject.fields.Byte;

			else if (fieldType == typeof(DateTime))
				toRet = new DateTime(wObject.fields.Long);

			else if (fieldType == typeof(double))
				toRet = wObject.fields.Double;

			else if (fieldType == typeof(uint))
				toRet = wObject.fields.UInt;

			else if (fieldType == typeof(ushort))
				toRet = wObject.fields.UShort;

			else if (fieldType == typeof(ulong))
				toRet = wObject.fields.ULong;

			else if (fieldType == typeof(sbyte))
				toRet = wObject.fields.SByte;

			else if (fieldType == typeof(decimal))
				toRet = wObject.fields.Decimal;

			else throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", fieldType));
			
			return toRet;
		}
	}
}