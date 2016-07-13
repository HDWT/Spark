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

			lock (m_mutex)
			{
				if (!s_dataTypes.TryGetValue(type, out dataType))
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

			m_assignableTypes = TryGetAsAttributeParams(type.GetCustomAttributes(true));

			for (int i = 0; i < m_assignableTypes.Count; ++i)
			{
				if (!type.IsAssignableFrom(m_assignableTypes[i].Type))
					m_assignableTypes.RemoveAt(i--);
					//throw new ArgumentException(string.Format("Type '{0}' is not assignable from {1}", type, m_assignableTypes[i].Type));
			}

			TypeFlags typeFlags = GetTypeFlags(type);

			if ((IsFlag(typeFlags, TypeFlags.List) || IsFlag(typeFlags, TypeFlags.Dictionary)))
			{
				ConstructorParameterType[0] = typeof(int);
				m_constructor = type.GetConstructor(ConstructorParameterType);
			}
			else
			{
				m_constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
			}

			if ((m_constructor == null) && (m_assignableTypes.Count == 0))
				throw new ArgumentException(string.Format("Required default constructor for type '{0}'", type));
		}

		public void InitMembers()
		{
			if (m_members != null)
				return;

			Type type = m_type;

			//
			List<IDataMember> members = null;
			int inheritanceDepth = 0;

			while ((type != null) && (type != typeof(System.Object)))
			{
				if (inheritanceDepth > MaxInheritanceDepth)
					throw new System.ArgumentException(string.Format("Max inheritance depth = {0}", MaxInheritanceDepth));

				FieldInfo[] fields = FieldOrder.GetSortedFields(type);// type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				if (members == null)
					members = new List<IDataMember>(fields.Length + properties.Length);

				AutoAttribute autoAttribute = null;
				object[] classAttributes = type.GetCustomAttributes(false);

				for (int i = 0; i < classAttributes.Length; ++i)
				{
					if (classAttributes[i] is AutoAttribute)
					{
						autoAttribute = classAttributes[i] as AutoAttribute;
						break;
					}
				}

				ushort memberId = 0;
				ushort autoMemberId = 1;
				int firstMemberOfThisType = members.Count;

				System.Type previousFieldType = null;
				bool previousTypeIsClassOrInterface = false;

				int fieldOffset = 0;

				foreach (var field in fields)
				{
					bool currentTypeIsClassOrInterface = field.FieldType.IsClass || field.FieldType.IsInterface;

					if (previousFieldType != null)
					{
						if (previousFieldType.IsEnum)
							previousFieldType = EnumTypeHelper.GetUnderlyingType(previousFieldType);

						if (previousTypeIsClassOrInterface)
							fieldOffset += IntPtr.Size;
						else if (previousFieldType == typeof(int) || previousFieldType == typeof(uint) || previousFieldType == typeof(float))
							fieldOffset += 4;
						else if (previousFieldType == typeof(short) || previousFieldType == typeof(ushort) || previousFieldType == typeof(char))
							fieldOffset += 2;
						else if (previousFieldType == typeof(byte) || previousFieldType == typeof(sbyte) || previousFieldType == typeof(bool))
							fieldOffset += 1;
						else if (previousFieldType == typeof(long) || previousFieldType == typeof(ulong) || previousFieldType == typeof(double) || previousFieldType == typeof(DateTime))
							fieldOffset += 8;
						else if (previousFieldType == typeof(decimal))
							fieldOffset += 16;
						else
							throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", previousFieldType));
					}

					if (IsMono)
					{
						if (previousFieldType == typeof(DateTime))
							fieldOffset += 8;

						if (field.FieldType == typeof(short) || field.FieldType == typeof(ushort) || field.FieldType == typeof(char))
						{
							int overhead = fieldOffset % 2;

							if (overhead != 0)
								fieldOffset += 2 - overhead;
						}
						else if (field.FieldType == typeof(int) || field.FieldType == typeof(uint) || field.FieldType == typeof(float))
						{
							int overhead = fieldOffset % 4;

							if (overhead != 0)
								fieldOffset += 4 - overhead;
						}
						else if (field.FieldType == typeof(long) || field.FieldType == typeof(ulong) || field.FieldType == typeof(double))
						{
							int overhead = fieldOffset % 8;

							if (overhead != 0)
								fieldOffset += 8 - overhead;
						}
						else if (field.FieldType == typeof(decimal))
						{
							int overhead = fieldOffset % 4;

							if (overhead != 0)
								fieldOffset += 4 - overhead;
						}
						else if (field.FieldType == typeof(DateTime))
						{
							int overhead = fieldOffset % 8;

							if (overhead != 0)
								fieldOffset += 8 - overhead;
						}
					}
					else
					{
						// align decimal
						if (field.FieldType == typeof(decimal) || field.FieldType == typeof(DateTime))
						{
							int overhead = fieldOffset % IntPtr.Size;

							if (overhead != 0)
								fieldOffset += IntPtr.Size - overhead;
						}
					}

					// aligin reference in 64 bit
					if (Is64Bit && (previousFieldType != null) && !currentTypeIsClassOrInterface && previousTypeIsClassOrInterface)
					{
						int d = (field.FieldType == typeof(long) || field.FieldType == typeof(ulong) || field.FieldType == typeof(double)) ? 8 : 4;
						int overhead = fieldOffset % d;

						if (overhead != 0)
							fieldOffset += d - overhead;
					}

					previousFieldType = field.FieldType;
					previousTypeIsClassOrInterface = currentTypeIsClassOrInterface;

					if ((autoAttribute != null) && autoAttribute.Has(AutoMode.Fields))
					{
						memberId = autoMemberId++;
					}
					else
					{
						if (!TryGetMemberAttributeId(field.GetCustomAttributes(false), out memberId))
							continue;
					}

					if (field.FieldType.ContainsGenericParameters)
						continue; //throw new System.ArgumentException(string.Format("Member of '{0}' with identifier {1} has generic params", type, memberId % MaxMemberId));

					memberId += (ushort)(inheritanceDepth * MaxMemberId);

					for (int i = firstMemberOfThisType; i < members.Count; ++i)
					{
						if (members[i].Id == memberId)
							throw new System.ArgumentException(string.Format("Member of '{0}' with identifier {1} is already declared", type, memberId % MaxMemberId));
					}

					if (FullAot)
					{
						if (ExperimentalMagic)
						{
							if ((type.BaseType != null) && (type.BaseType != typeof(System.Object)))
								members.Add(new DataMember(memberId, field, -1));
							else
								members.Add(new DataMember(memberId, field, fieldOffset));
						}
						else
						{
							members.Add(new DataMember(memberId, field, -1));
						}
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
					if ((autoAttribute != null) && autoAttribute.Has(AutoMode.Properties))
					{
						if ((property.GetGetMethod(true) == null) || (property.GetSetMethod(true) == null))
							continue;

						memberId = autoMemberId++;
					}
					else
					{
						if (!TryGetMemberAttributeId(property.GetCustomAttributes(false), out memberId))
							continue;
					}

					if (property.PropertyType.ContainsGenericParameters)
						continue; //throw new System.ArgumentException(string.Format("Member of '{0}' with identifier {1} has generic params", type, memberId % MaxMemberId));

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

			m_members = (members != null) ? members.ToArray() : new IDataMember[0];
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

				if (memberIndex == InvalidMemberIndex)
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
		public void WriteValues(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
		{
			if (m_assignableTypes.Count != 0)
			{
				Type instanceType = instance.GetType();

				if (instanceType != m_type)
				{
					for (int i = 0; i < m_assignableTypes.Count; ++i)
					{
						if (m_assignableTypes[i].Type != instanceType)
							continue;

						m_assignableTypes[i].GetDataType().WriteValues(instance, data, ref startIndex, sizes, values, context);
						return;
					}
				}
			}

			data[startIndex++] = m_typeId;

			for (int i = 0; i < m_members.Length; ++i)
				m_members[i].WriteValue(instance, data, ref startIndex, sizes, values, context);
		}

		/// <summary> Возвращает количество байт, которое потребуется для записи {instance} </summary>
		public int GetDataSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values, Context context)
		{
			if (m_assignableTypes.Count != 0)
			{
				Type instanceType = instance.GetType();

				if (instanceType != m_type)
				{
					for (int i = 0; i < m_assignableTypes.Count; ++i)
					{
						if (m_assignableTypes[i].Type != instanceType)
							continue;

						return m_assignableTypes[i].GetDataType().GetDataSize(instance, sizes, values, context);
					}
				}
			}

			int size = sizeof(byte); // Один байт для опредения полиморфизма

			for (int i = 0; i < m_members.Length; ++i)
				size += m_members[i].GetSize(instance, sizes, values, context);

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