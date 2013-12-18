using System;
using System.Collections.Generic;

public static partial class Spark
{
	private class EnumTypeHelper
	{
		public static readonly EnumTypeHelper Instance = new EnumTypeHelper();

		private readonly Values<byte>	m_byteValues	= new Values<byte>	(Writer.Get(typeof(byte)),		Reader.Get(typeof(byte)),	SizeCalculator.Evaluate);
		private readonly Values<sbyte>	m_sbyteValues	= new Values<sbyte>	(Writer.Get(typeof(sbyte)),		Reader.Get(typeof(sbyte)),	SizeCalculator.Evaluate);
		private readonly Values<short>	m_shortValues	= new Values<short>	(Writer.Get(typeof(short)),		Reader.Get(typeof(short)),	SizeCalculator.Evaluate);
		private readonly Values<ushort> m_ushortValues	= new Values<ushort>(Writer.Get(typeof(ushort)),	Reader.Get(typeof(ushort)), SizeCalculator.Evaluate);
		private readonly Values<int>	m_intValues		= new Values<int>	(Writer.Get(typeof(int)),		Reader.Get(typeof(int)),	SizeCalculator.Evaluate);
		private readonly Values<uint>	m_uintValues	= new Values<uint>	(Writer.Get(typeof(uint)),		Reader.Get(typeof(uint)),	SizeCalculator.Evaluate);
		private readonly Values<long>	m_longValues	= new Values<long>	(Writer.Get(typeof(long)),		Reader.Get(typeof(long)),	SizeCalculator.Evaluate);
		private readonly Values<ulong>	m_ulongValues	= new Values<ulong>	(Writer.Get(typeof(ulong)),		Reader.Get(typeof(ulong)),	SizeCalculator.Evaluate);

		public void Register(Type enumType)
		{
			if (enumType.BaseType != typeof(Enum))
				throw new System.ArgumentException(string.Format("'{0}' is not an Enum Type"));

			Type underlyingType = Enum.GetUnderlyingType(enumType);

			if (underlyingType == typeof(int))
				m_intValues.Register(enumType);

			else if (underlyingType == typeof(short))
				m_shortValues.Register(enumType);

			else if (underlyingType == typeof(byte))
				m_byteValues.Register(enumType);

			else if (underlyingType == typeof(long))
				m_longValues.Register(enumType);

			else if (underlyingType == typeof(uint))
				m_uintValues.Register(enumType);

			else if (underlyingType == typeof(ushort))
				m_ushortValues.Register(enumType);

			else if (underlyingType == typeof(sbyte))
				m_sbyteValues.Register(enumType);

			else if (underlyingType == typeof(ulong))
				m_ulongValues.Register(enumType);

			else throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' not supported", underlyingType));
		}

		public WriteDataDelegate GetWriter(Type enumType)
		{
			if (enumType.BaseType != typeof(Enum))
				throw new System.ArgumentException(string.Format("'{0}' is not an Enum Type"));

			Type underlyingType = Enum.GetUnderlyingType(enumType);

			if (underlyingType == typeof(int))
				return m_intValues.Write;

			if (underlyingType == typeof(short))
				return m_shortValues.Write;

			if (underlyingType == typeof(byte))
				return m_byteValues.Write;

			if (underlyingType == typeof(long))
				return m_longValues.Write;

			if (underlyingType == typeof(uint))
				return m_uintValues.Write;

			if (underlyingType == typeof(ushort))
				return m_ushortValues.Write;

			if (underlyingType == typeof(sbyte))
				return m_sbyteValues.Write;

			if (underlyingType == typeof(ulong))
				return m_ulongValues.Write;

			throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' not supported", underlyingType));
		}

		public ReadDataDelegate GetReader(Type enumType)
		{
			if (enumType.BaseType != typeof(Enum))
				throw new System.ArgumentException(string.Format("'{0}' is not an Enum Type"));

			Type underlyingType = Enum.GetUnderlyingType(enumType);

			if (underlyingType == typeof(int))
				return m_intValues.Read;

			if (underlyingType == typeof(short))
				return m_shortValues.Read;

			if (underlyingType == typeof(byte))
				return m_byteValues.Read;

			if (underlyingType == typeof(long))
				return m_longValues.Read;

			if (underlyingType == typeof(uint))
				return m_uintValues.Read;

			if (underlyingType == typeof(ushort))
				return m_ushortValues.Read;

			if (underlyingType == typeof(sbyte))
				return m_sbyteValues.Read;

			if (underlyingType == typeof(ulong))
				return m_ulongValues.Read;

			throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' not supported", underlyingType));
		}

		public int GetSize(object value)
		{
			Type enumType = value.GetType();

			if (enumType.BaseType != typeof(Enum))
				throw new System.ArgumentException(string.Format("'{0}' is not an Enum Type"));

			Type underlyingType = Enum.GetUnderlyingType(enumType);

			if (underlyingType == typeof(int))
				return m_intValues.GetSize((int)value);

			if (underlyingType == typeof(short))
				return m_shortValues.GetSize((short)value);

			if (underlyingType == typeof(byte))
				return m_byteValues.GetSize((byte)value);

			if (underlyingType == typeof(long))
				return m_longValues.GetSize((long)value);

			if (underlyingType == typeof(uint))
				return m_uintValues.GetSize((uint)value);

			if (underlyingType == typeof(ushort))
				return m_ushortValues.GetSize((ushort)value);

			if (underlyingType == typeof(sbyte))
				return m_sbyteValues.GetSize((sbyte)value);

			if (underlyingType == typeof(ulong))
				return m_ulongValues.GetSize((ulong)value);

			throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' not supported", underlyingType));
		}

		private class Values<T> where T : IComparable<T>, IEquatable<T>
		{
			private readonly Dictionary<Type, EnumInfo> m_enumsInfo = new Dictionary<Type, EnumInfo>();

			private readonly WriteDataDelegate	m_writeData = null;
			private readonly ReadDataDelegate	m_readData	= null;
			private readonly Func<T, int>		m_getSize	= null;

			public WriteDataDelegate Write { get { return m_writeData; } }

			private class EnumInfo
			{
				private Array	m_values			= null;
				private T[]		m_underlyingValues	= null;
				private int[]	m_sizes				= null;

				public EnumInfo(Type enumType, Func<T, int> getSize)
				{
					m_values = Enum.GetValues(enumType);

					m_underlyingValues = new T[m_values.Length];
					m_sizes = new int[m_values.Length];

					int index = 0;
					foreach (T value in m_values)
					{
						m_underlyingValues[index] = value;
						m_sizes[index] = getSize(value);

						index++;
					}
				}

				public object GetValue(T underlyingValue)
				{
					for (int index = 0; index < m_underlyingValues.Length; ++index)
					{
						if (m_underlyingValues[index].CompareTo(underlyingValue) == 0)
							return m_values.GetValue(index);
					}

					return m_values.GetValue(0);
				}
			}

			public Values(WriteDataDelegate writeData, ReadDataDelegate readData, Func<T, int> getSize)
			{
				m_writeData = writeData;
				m_readData	= readData;
				m_getSize	= getSize;
			}

			public void Register(Type enumType)
			{
				if (m_enumsInfo.ContainsKey(enumType))
					return;

				m_enumsInfo[enumType] = new EnumInfo(enumType, m_getSize);
			}

			public object Read(Type enumType, byte[] data, ref int startIndex)
			{
				T underlyingValue = (T)m_readData(enumType, data, ref startIndex);

				EnumInfo enumInfo = null;

				if (!m_enumsInfo.TryGetValue(enumType, out enumInfo))
				{
					enumInfo = new EnumInfo(enumType, m_getSize);
					m_enumsInfo[enumType] = enumInfo;
				}

				return enumInfo.GetValue(underlyingValue);
			}

			public int GetSize(T value)
			{
				return m_getSize(value);
			}
		}
	}
}
