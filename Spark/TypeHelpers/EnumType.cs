using System;
using System.Collections.Generic;

public static partial class Spark
{
	private class EnumTypeHelper
	{
		public static readonly EnumTypeHelper Instance = new EnumTypeHelper();

		private readonly Values<byte>	m_byteValues	= new Values<byte>(TypeHelper.Byte.GetSize, TypeHelper.Byte.ReadObject);
		private readonly Values<sbyte>	m_sbyteValues	= new Values<sbyte>(TypeHelper.SByte.GetSize, TypeHelper.SByte.ReadObject);
		private readonly Values<short>	m_shortValues	= new Values<short>(TypeHelper.Short.GetSize, TypeHelper.Short.ReadObject);
		private readonly Values<ushort> m_ushortValues	= new Values<ushort>(TypeHelper.UShort.GetSize, TypeHelper.UShort.ReadObject);
		private readonly Values<int>	m_intValues		= new Values<int>(TypeHelper.IntType.GetSize, TypeHelper.IntType.ReadObject);
		private readonly Values<uint>	m_uintValues	= new Values<uint>(TypeHelper.UInt.GetSize, TypeHelper.UInt.ReadObject);
		private readonly Values<long>	m_longValues	= new Values<long>(TypeHelper.Long.GetSize, TypeHelper.Long.ReadObject);
		private readonly Values<ulong>	m_ulongValues	= new Values<ulong>(TypeHelper.ULong.GetSize, TypeHelper.ULong.ReadObject);

		private static readonly Dictionary<Type, Type> s_underlyingTypesByEnumType = new Dictionary<Type, Type>(16);

		private static readonly Dictionary<Type, GetValueSizeDelegate> s_getSizeDelegatesByEnumType = new Dictionary<Type, GetValueSizeDelegate>(16);
		private static readonly Dictionary<Type, WriteValueDelegate> s_writeDataDelegatesByEnumType = new Dictionary<Type, WriteValueDelegate>(16);
		private static readonly Dictionary<Type, ReadDataDelegate> s_readDataDelegatesByEnumType = new Dictionary<Type, ReadDataDelegate>(16);

		public bool TryGetUnderlyingType(Type enumType, out Type underlyingType)
		{
			return s_underlyingTypesByEnumType.TryGetValue(enumType, out underlyingType);
		}

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

		public GetValueSizeDelegate GetGetSizeDelegate(Type enumType)
		{
			GetValueSizeDelegate getSizeDelegate = null;

			if (!s_getSizeDelegatesByEnumType.TryGetValue(enumType, out getSizeDelegate))
			{
				Type underlyingType = GetUnderlyingType(enumType);

				getSizeDelegate = SizeCalculator.GetForValueType(underlyingType);
				s_getSizeDelegatesByEnumType[enumType] = getSizeDelegate;
			}

			return getSizeDelegate;
		}

		public WriteValueDelegate GetWriter(Type enumType)
		{
			WriteValueDelegate writeData = null;

			if (!s_writeDataDelegatesByEnumType.TryGetValue(enumType, out writeData))
			{
				Type underlyingType = GetUnderlyingType(enumType);

				writeData = Writer.GetDelegateForValueType(underlyingType);
				s_writeDataDelegatesByEnumType[enumType] = writeData;
			}

			return writeData;
		}

		public ReadDataDelegate GetReader(Type enumType)
		{
			ReadDataDelegate reader = null;

			if (!s_readDataDelegatesByEnumType.TryGetValue(enumType, out reader))
			{
				Type underlyingType = GetUnderlyingType(enumType);

				if (underlyingType == typeof(int))
					reader = m_intValues.Read;

				else if (underlyingType == typeof(short))
					reader = m_shortValues.Read;

				else if (underlyingType == typeof(byte))
					reader = m_byteValues.Read;

				else if (underlyingType == typeof(long))
					reader = m_longValues.Read;

				else if (underlyingType == typeof(uint))
					reader = m_uintValues.Read;

				else if (underlyingType == typeof(ushort))
					reader = m_ushortValues.Read;

				else if (underlyingType == typeof(sbyte))
					reader = m_sbyteValues.Read;

				else if (underlyingType == typeof(ulong))
					reader = m_ulongValues.Read;

				else throw new System.ArgumentException(string.Format("Enum with underlying type '{0}' is not supported", underlyingType));

				s_readDataDelegatesByEnumType[enumType] = reader;
			}

			return reader;
		}

		public static Type GetUnderlyingType(Type enumType)
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
			private static readonly object m_mutex = new object();

			private class EnumInfo
			{
				private object[]	m_values			= null;
				private T[]			m_underlyingValues	= null;
				private int[]		m_sizes				= null;
				private bool		m_flags				= false;

				public EnumInfo(Type enumType, GetSizeDelegate getSize)
				{
					Array enumValues = Enum.GetValues(enumType);

					m_values = new object[enumValues.Length];

					for (int i = 0; i < m_values.Length; ++i)
						m_values[i] = enumValues.GetValue(i);

					m_underlyingValues = new T[m_values.Length];
					m_sizes = new int[m_values.Length];

					foreach (var attribute in enumType.GetCustomAttributes(false))
					{
						if (attribute.GetType() == typeof(System.FlagsAttribute))
						{
							m_flags = true;
							break;
						}
					}

					for (int i = 0; i < m_values.Length; ++i)
					{
						m_underlyingValues[i] = (T)m_values[i];
						m_sizes[i] = getSize(m_values[i]);
					}
				}

				public object GetValue(T underlyingValue)
				{
					if (m_flags)
					{
						return underlyingValue;
					}
					else
					{
						for (int index = 0; index < m_underlyingValues.Length; ++index)
						{
							if (m_underlyingValues[index].CompareTo(underlyingValue) == 0)
								return m_values[index];
						}
					}

					return m_values[0];
				}
			}

			public Values(GetSizeDelegate getSize, ReadDataDelegate readObject)
			{
				m_getSize = getSize;
				m_readObject = readObject;
			}

			GetSizeDelegate m_getSize = null;
			ReadDataDelegate m_readObject = null;

			public void Register(Type enumType)
			{
				lock (m_mutex)
				{
					if (m_enumsInfo.ContainsKey(enumType))
						return;

					m_enumsInfo[enumType] = new EnumInfo(enumType, m_getSize);
				}
			}

			public object Read(Type enumType, byte[] data, ref int startIndex)
			{
				T underlyingValue = (T)m_readObject(enumType, data, ref startIndex);

				EnumInfo enumInfo = null;

				if (!m_enumsInfo.TryGetValue(enumType, out enumInfo))
				{
					lock (m_mutex)
					{
						enumInfo = new EnumInfo(enumType, m_getSize);
						m_enumsInfo[enumType] = enumInfo;
					}
				}

				return enumInfo.GetValue(underlyingValue);
			}
		}
	}
}
