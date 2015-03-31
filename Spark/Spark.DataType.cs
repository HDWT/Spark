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
		
		private IDataMember[] m_members = null;
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

					dataType.InitMembers();
				}
			}

			return dataType;
		}

		public static void Register(Type type)
		{
			lock (m_mutex)
			{
				if (!s_dataTypes.ContainsKey(type))
				{
					DataType dataType = new DataType(type);
					s_dataTypes[type] = dataType;

					dataType.InitMembers();
				}
			}
		}

		private Type m_type = null;
		private byte m_typeId = 0;
		private List<TypeId> m_assignableTypes = null;

		public DataType(Type type)
		{
			m_type = type;
			m_typeId = GetTypeId(type);

			m_assignableTypes = TryGetAsAttributeParams(type.GetCustomAttributes(false));

			if (m_assignableTypes != null)
			{
				for (int i = 0; i < m_assignableTypes.Count; ++i)
				{
					if (!type.IsAssignableFrom(m_assignableTypes[i].Type))
						throw new ArgumentException(string.Format("Type '{0}' is not assignable from {1}", type, m_assignableTypes[i].Type));
				}
			}

			TypeFlags typeFlags = GetTypeFlags(type);

			if ((m_assignableTypes == null) && (typeFlags.Is(TypeFlags.List) || typeFlags.Is(TypeFlags.Dictionary)))
			{
				ConstructorParameterType[0] = typeof(int);
				m_constructor = type.GetConstructor(ConstructorParameterType);
			}
			else
			{
				m_constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
			}

			if ((m_constructor == null) && (m_assignableTypes == null))
				throw new ArgumentException(string.Format("Required default constructor for type '{0}'", type));
		}

		public void InitMembers()
		{
			if (m_members != null)
				throw new System.InvalidOperationException();

			Type type = m_type;

			//
			List<IDataMember> members = null;
			int inheritanceDepth = 0;

			while (type != null && type != typeof(System.Object))
			{
				if (inheritanceDepth > MaxInheritanceDepth)
					throw new System.ArgumentException(string.Format("Max inheritance depth = {0}", MaxInheritanceDepth));

				FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				if (members == null)
					members = new List<IDataMember>(fields.Length + properties.Length);

				ushort memberId = 0;
				int firstMemberOfThisType = members.Count;

				foreach (var field in fields)
				{
					if (!TryGetMemberAttributeId(field.GetCustomAttributes(false), out memberId))
						continue;

					memberId += (ushort)(inheritanceDepth * MaxMemberId);

					for (int i = firstMemberOfThisType; i < members.Count; ++i)
					{
						if (members[i].Id == memberId)
							throw new System.ArgumentException(string.Format("Member of '{0}' with identifier {1} is already declared", type, memberId % MaxMemberId));
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

					memberId += (ushort)(inheritanceDepth * 1024);

					for (int i = firstMemberOfThisType; i < members.Count; ++i)
					{
						if (members[i].Id == memberId)
							throw new System.ArgumentException(string.Format("Member of '{0}' with identifier {1} is already declared", type, memberId % MaxMemberId));
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

				type = type.BaseType;
				inheritanceDepth++;
			}

			m_members = (members != null) ?  members.ToArray() :  new IDataMember[0];
		}

		public bool TryCreateInstance(byte typeId, out object instance)
		{
			if (typeId == m_typeId)
				typeId = 0;

			if (typeId == 0)
			{
				if (m_constructor == null)
					throw new ArgumentException(string.Format("Required default constructor for type '{0}'", m_type));

				instance = m_constructor.Invoke(null);
				return true;
			}

			for (int i = 0; i < m_assignableTypes.Count; ++i)
			{
				if (m_assignableTypes[i].Id == typeId)
					return m_assignableTypes[i].GetDataType().TryCreateInstance(0, out instance);
			}

			instance = null;
			return false;
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

			ICallbacks callbacks = instance as ICallbacks;

			if (callbacks != null)
				callbacks.BeforeDeserialize();

			byte typeId = data[startIndex++];

			if (typeId == m_typeId)
				typeId = 0;

			if (typeId != 0)
			{
				for (int i = 0; i < m_assignableTypes.Count; ++i)
				{
					if (m_assignableTypes[i].Id != typeId)
						continue;

					int typeIdIndex = --startIndex;
					data[typeIdIndex] = 0; // Полиморфизм. Читаем поля в другой класс

					m_assignableTypes[i].GetDataType().ReadValues(instance, data, ref startIndex, endIndex);

					data[typeIdIndex] = typeId; // Восстанавливаем значение. byte[] data - не должен меняться
					return;
				}

				startIndex = endIndex;
				return;
			}

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

				if (callbacks != null)
					callbacks.BeforeSetValue(memberId, memberIndex != InvalidMemberIndex);

				if ((memberIndex == InvalidMemberIndex) || (SimulateMissingFields && DateTime.UtcNow.Ticks % 2 == 0))
				{
					byte dataSizeBlock = data[startIndex++];

					// Erase reserved bytes
					dataSizeBlock = (byte)(dataSizeBlock & 0x0F);

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

				if (callbacks != null)
					callbacks.AfterSetValue(memberId, memberIndex != InvalidMemberIndex);
			}

			if (callbacks != null)
				callbacks.AfterDeserialize();
		}

		/// <summary> Записывает все поля {instance} в массив байт {data} начиная с индекса {startInder} </summary>
		public void WriteValues(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			if (m_assignableTypes != null)
			{
				Type instanceType = instance.GetType();

				if (instanceType != m_type)
				{
					for (int i = 0; i < m_assignableTypes.Count; ++i)
					{
						if (m_assignableTypes[i].Type != instanceType)
							continue;

						m_assignableTypes[i].GetDataType().WriteValues(instance, data, ref startIndex, sizes, values);
						return;
					}
				}
			}

			data[startIndex++] = m_typeId;

			for (int i = 0; i < m_members.Length; ++i)
				m_members[i].WriteValue(instance, data, ref startIndex, sizes, values);
		}

		/// <summary> Возвращает количество байт, которое потребуется для записи {instance} </summary>
		public int GetDataSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			if (m_assignableTypes != null)
			{
				Type instanceType = instance.GetType();

				if (instanceType != m_type)
				{
					for (int i = 0; i < m_assignableTypes.Count; ++i)
					{
						if (m_assignableTypes[i].Type != instanceType)
							continue;

						return m_assignableTypes[i].GetDataType().GetDataSize(instance, sizes, values);
					}
				}
			}

			int size = sizeof(byte); // Один байт для опредения полиморфизма

			for (int i = 0; i < m_members.Length; ++i)
				size += m_members[i].GetSize(instance, sizes, values);

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