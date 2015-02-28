using System;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private class DataType
	{
		private const int InvalidMemberIndex = -1;

		private static readonly object[] ConstructorParameter = new object[1];
		private static readonly Type[] ConstructorParameterType = new Type[1];

		private static readonly Type[] DataMemberGenericType = new Type[1];
		private static readonly Type[] DataMemberConstructorParamTypes_IdField = { typeof(ushort), typeof(FieldInfo) };
		private static readonly Type[] DataMemberConstructorParamTypes_IdProperty = { typeof(ushort), typeof(PropertyInfo) };
		private static readonly object[] DataMemberConstructorParameters_IdType = new object[2];

		private static readonly Dictionary<Type, DataType> s_dataTypes = new Dictionary<Type, DataType>(16);
		private static readonly object m_mutex = new object();
		
		private readonly IDataMember[] m_members = null;
		private readonly ConstructorInfo m_constructor = null;
		
		public static DataType Get(Type type)
		{
			DataType dataType = null;

			if (!s_dataTypes.TryGetValue(type, out dataType))
			{
				lock (m_mutex)
				{
					dataType = new DataType(type);
					s_dataTypes[type] = dataType;
				}
			}

			return dataType;
		}

		public static void Register(Type type)
		{
			lock (m_mutex)
			{
				if (!s_dataTypes.ContainsKey(type))
					s_dataTypes[type] = new DataType(type);
			}
		}

		public DataType(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			List<IDataMember> members = new List<IDataMember>(fields.Length);

			ushort memberId = 0;

			foreach (var field in fields)
			{
				if (!TryGetMemberAttributeId(field.GetCustomAttributes(false), out memberId))
					continue;

				foreach (var member in members)
				{
					if (member.Id == memberId)
						throw new System.ArgumentException(string.Format("Member of '{0}' with identifier {1} is already declared", type, memberId));
				}

				if (FullAot)
				{
					members.Add(new DataMember(memberId, field));
				}
				else
				{
					DataMemberGenericType[0] = field.FieldType;

					Type specificType = typeof(DataMember<>).MakeGenericType(DataMemberGenericType);
					ConstructorInfo constructor = specificType.GetConstructor(DataMemberConstructorParamTypes_IdField);

					DataMemberConstructorParameters_IdType[0] = memberId;
					DataMemberConstructorParameters_IdType[1] = field;

					IDataMember dataMember = constructor.Invoke(DataMemberConstructorParameters_IdType) as IDataMember;

					members.Add(dataMember);
				}
			}

			foreach (var property in properties)
			{
				if (!TryGetMemberAttributeId(property.GetCustomAttributes(false), out memberId))
					continue;

				foreach (var member in members)
				{
					if (member.Id == memberId)
						throw new System.ArgumentException(string.Format("Member of '{0}' with identifier {1} is already declared", type, memberId));
				}

				if (FullAot)
				{
					members.Add(new DataMember(memberId, property));
				}
				else
				{
					DataMemberGenericType[0] = property.PropertyType;

					Type specificType = typeof(DataMember<>).MakeGenericType(DataMemberGenericType);
					ConstructorInfo constructor = specificType.GetConstructor(DataMemberConstructorParamTypes_IdProperty);

					DataMemberConstructorParameters_IdType[0] = memberId;
					DataMemberConstructorParameters_IdType[1] = property;

					IDataMember dataMember = constructor.Invoke(DataMemberConstructorParameters_IdType) as IDataMember;

					members.Add(dataMember);
				}
			}

			m_members = members.ToArray();

			if (IsGenericList(type) || IsGenericDictionary(type))
			{
				ConstructorParameterType[0] = typeof(int);
				m_constructor = type.GetConstructor(ConstructorParameterType);
			}
			else
			{
				m_constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
			}

			if (m_constructor == null)
				throw new ArgumentException("Required default constructor for type '" + type + "'");
		}

		public object CreateInstance()
		{
			return m_constructor.Invoke(null);
		}

		public object CreateInstance(object parameter)
		{
			lock (ConstructorParameter)
			{
				ConstructorParameter[0] = parameter;
				return m_constructor.Invoke(ConstructorParameter);
			}
		}

		public void ReadValues(object instance, byte[] data, ref int startIndex, int endIndex)
		{
			if (endIndex < startIndex)
				throw new ArgumentException(string.Format("Read values failed. StartIndex = {0}, EndIndex = {1}", startIndex, endIndex));

			while (startIndex != endIndex)
			{
				bool ignoreDataSizeBlock = false;

				byte memberIdL = data[startIndex++];
				byte memberIdH = data[startIndex++];

				if (memberIdH >= IgnoreDataSizeBlockMark)
				{
					ignoreDataSizeBlock = true;
					memberIdH -= IgnoreDataSizeBlockMark;
				}

				ushort memberId = (ushort)(memberIdL + (memberIdH << 8));
				int memberIndex = GetMemberIndex(memberId);

				if (memberIndex == InvalidMemberIndex)
				{
					byte dataSizeBlock = data[startIndex++];

					// Erase reserved bytes
					dataSizeBlock = (byte)(dataSizeBlock & ~0x0F);

					//if (dataSizeBlock >= ForwardPaddingMark)
					//	dataSizeBlock -= ForwardPaddingMark;

					if (dataSizeBlock != 0)
					{
						int dataSize = 0;

						if (ignoreDataSizeBlock)
						{
							dataSize = dataSizeBlock;
							startIndex += dataSize;
						}
						else
						{
							for (int i = 0; i < dataSizeBlock; ++i)
								dataSize += data[startIndex + i] << (8 * i);

							startIndex += dataSize - 1; // 1 прибавили при чтении dataSizeBlock
						}
					}
				}
				else
				{
					m_members[memberIndex].ReadValue(instance, data, ref startIndex);
				}
			}
		}

		/// <summary> Записывает все поля {instance} в массив байт {data} начиная с индекса {startInder} </summary>
		//public void WriteValues(object instance, byte[] data, ref int startIndex)
		//{
		//	for (int i = 0; i < m_members.Length; ++i)
		//		m_members[i].WriteValue(instance, data, ref startIndex);
		//}

		public void WriteValues(object instance, byte[] data, ref int startIndex, QueueWithIndexer sizes)
		{
			for (int i = 0; i < m_members.Length; ++i)
				m_members[i].WriteValue(instance, data, ref startIndex, sizes);
		}

		/// <summary> Возвращает количество байт, которое потребуется для записи {instance} </summary>
		//public int GetDataSize(object instance)
		//{
		//	int size = 0;

		//	for (int i = 0; i < m_members.Length; ++i)
		//		size += m_members[i].GetSize(instance);

		//	return size;
		//}

		public int GetDataSize(object instance, QueueWithIndexer sizes)
		{
			int size = 0;

			for (int i = 0; i < m_members.Length; ++i)
				size += m_members[i].GetSize(instance, sizes);

			return size;
		}

		/// <summary> Возвращает индекс мембера по Id или -1, если поля с таким индексом нет </summary>
		private int GetMemberIndex(ushort id)
		{
			for (int index = 0; index < m_members.Length; ++index)
			{
				if (m_members[index].Id == id)
					return index;
			}

			return InvalidMemberIndex;
		}
	}
}