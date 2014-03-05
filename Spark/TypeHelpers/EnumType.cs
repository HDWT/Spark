using System;
using System.Collections.Generic;

public static partial class Spark
{
	private class EnumTypeHelper
	{
		public static readonly EnumTypeHelper Instance = new EnumTypeHelper();

		private readonly Values<byte>	m_byteValues	= new Values<byte>(TypeHelper.Byte);
		private readonly Values<sbyte>	m_sbyteValues	= new Values<sbyte>(TypeHelper.SByte);
		private readonly Values<short>	m_shortValues	= new Values<short>(TypeHelper.Short);
		private readonly Values<ushort> m_ushortValues	= new Values<ushort>(TypeHelper.UShort);
		private readonly Values<int>	m_intValues		= new Values<int>(TypeHelper.Int);
		private readonly Values<uint>	m_uintValues	= new Values<uint>(TypeHelper.UInt);
		private readonly Values<long>	m_longValues	= new Values<long>(TypeHelper.Long);
		private readonly Values<ulong>	m_ulongValues	= new Values<ulong>(TypeHelper.ULong);

		private static readonly Dictionary<Type, Type> s_underlyingTypesByEnumType = new Dictionary<Type, Type>(16);

		public void Register(Type enumType)
		{
			Type underlyingType = GetUnderlyingType(enumType);

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

			else throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' is not supported", underlyingType));
		}

		public WriteDataDelegate GetWriter(Type enumType)
		{
			Type underlyingType = GetUnderlyingType(enumType);

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

			throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' is not supported", underlyingType));
		}

		public ReadDataDelegate GetReader(Type enumType)
		{
			Type underlyingType = GetUnderlyingType(enumType);

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

			throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' is not supported", underlyingType));
		}

		public int GetSize(object value, LinkedList<int> sizes)
		{
			Type enumType = value.GetType();
			Type underlyingType = GetUnderlyingType(enumType);

			if (underlyingType == typeof(int))
				return m_intValues.GetSize((int)value, sizes);

			if (underlyingType == typeof(short))
				return m_shortValues.GetSize((short)value, sizes);

			if (underlyingType == typeof(byte))
				return m_byteValues.GetSize((byte)value, sizes);

			if (underlyingType == typeof(long))
				return m_longValues.GetSize((long)value, sizes);

			if (underlyingType == typeof(uint))
				return m_uintValues.GetSize((uint)value, sizes);

			if (underlyingType == typeof(ushort))
				return m_ushortValues.GetSize((ushort)value, sizes);

			if (underlyingType == typeof(sbyte))
				return m_sbyteValues.GetSize((sbyte)value, sizes);

			if (underlyingType == typeof(ulong))
				return m_ulongValues.GetSize((ulong)value, sizes);

			throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' is not supported", underlyingType));
		}

		private static Type GetUnderlyingType(Type enumType)
		{
			Type underlyingType = null;

			if (!s_underlyingTypesByEnumType.TryGetValue(enumType, out underlyingType))
			{
				lock (s_underlyingTypesByEnumType)
				{
					underlyingType = Enum.GetUnderlyingType(enumType);
					s_underlyingTypesByEnumType[enumType] = underlyingType;
				}
			}

			return underlyingType;
		}

		private class Values<T> where T : IComparable<T>, IEquatable<T>
		{
			private readonly Dictionary<Type, EnumInfo> m_enumsInfo = new Dictionary<Type, EnumInfo>();

			private ITypeHelper<T> m_typeHelper = null;

			private class EnumInfo
			{
				private Array	m_values			= null;
				private T[]		m_underlyingValues	= null;
				private int[]	m_sizes				= null;

				public EnumInfo(Type enumType, GetSizeDelegate getSize)
				{
					m_values = Enum.GetValues(enumType);

					m_underlyingValues = new T[m_values.Length];
					m_sizes = new int[m_values.Length];

					int index = 0;
					foreach (T value in m_values)
					{
						m_underlyingValues[index] = value;
						m_sizes[index] = getSize(value, null);

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

			public Values(ITypeHelper<T> typeHelper)
			{
				m_typeHelper = typeHelper;
			}

			public void Register(Type enumType)
			{
				lock (m_enumsInfo)
				{
					if (m_enumsInfo.ContainsKey(enumType))
						return;

					m_enumsInfo[enumType] = new EnumInfo(enumType, m_typeHelper.GetSize);
				}
			}

			public int GetSize(T value, LinkedList<int> sizes)
			{
				return m_typeHelper.GetSize(value, sizes);
			}

			public object Read(Type enumType, byte[] data, ref int startIndex)
			{
				T underlyingValue = (T)m_typeHelper.ReadObject(enumType, data, ref startIndex);

				EnumInfo enumInfo = null;

				if (!m_enumsInfo.TryGetValue(enumType, out enumInfo))
				{
					lock (m_enumsInfo)
					{
						enumInfo = new EnumInfo(enumType, m_typeHelper.GetSize);
						m_enumsInfo[enumType] = enumInfo;
					}
				}

				return enumInfo.GetValue(underlyingValue);
			}

			public WriteDataDelegate Write
			{
				get { return m_typeHelper.WriteObject; }
			}
		}
	}
}
