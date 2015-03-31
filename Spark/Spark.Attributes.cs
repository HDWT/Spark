using System;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	// Restriction: (MaxMemberId * InheritanceDepth - 1) < 32768
	private const ushort MaxMemberId = 1024;
	private const ushort MaxInheritanceDepth = 32;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class MemberAttribute : Attribute
	{
		public readonly ushort Id = 0;

		public MemberAttribute(ushort identifier)
		{
			this.Id = identifier;
		}

		private static FieldInfo s_idField = typeof(MemberAttribute).GetField("Id");
		public static ushort GetId(object obj)
		{
			return (ushort)s_idField.GetValue(obj);
		}
	}

	private static bool TryGetMemberAttributeId(object[] attributes, out ushort memberId)
	{
		for (int i = 0; i < attributes.Length; ++i)
		{
			Type attributeType = attributes[i].GetType();

			if (attributeType != typeof(MemberAttribute))
				continue;

			memberId = MemberAttribute.GetId(attributes[i]);

			if (memberId >= MaxMemberId)
				 throw new System.ArgumentException(string.Format("Member identifier must be less than {0}", MaxMemberId));

			return true;
		}

		memberId = 0;
		return false;
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
	public class AsAttribute : Attribute
	{
		public readonly byte Id = 0;
		public readonly System.Type Type = null;

		public AsAttribute(byte identifier, System.Type type)
		{
			this.Id = identifier;
			this.Type = type;
		}

		private static FieldInfo s_idField = typeof(AsAttribute).GetField("Id");
		public static byte GetId(object obj)
		{
			return (byte)s_idField.GetValue(obj);
		}

		private static FieldInfo s_typeField = typeof(AsAttribute).GetField("Type");
		public static System.Type GetType(object obj)
		{
			return (System.Type)s_typeField.GetValue(obj);
		}
	}

	private class TypeId
	{
		public byte Id { get; private set; }
		public System.Type Type { get; private set; }

		private DataType m_dataType = null;

		public TypeId(byte id, System.Type type)
		{
			this.Id = id;
			this.Type = type;
		}

		public DataType GetDataType()
		{
			if (m_dataType == null)
				m_dataType = DataType.Get(this.Type);

			return m_dataType;
		}
	}

	private static byte GetTypeId(Type type)
	{
		byte id = 0;
		
		Type currentType = type;
		Type[] interfaces = type.GetInterfaces();

		HashSet<int> knownIds = null;

		for (int index = -1; index < interfaces.Length; ++index)
		{
			if (index >= 0)
				currentType = interfaces[index];

			object[] attributes = currentType.GetCustomAttributes(index < 0);

			for (int i = 0; i < attributes.Length; ++i)
			{
				Type attributeType = attributes[i].GetType();

				if (attributeType != typeof(AsAttribute))
					continue;

				byte typeId = AsAttribute.GetId(attributes[i]);
				System.Type classType = AsAttribute.GetType(attributes[i]);

				if (typeId == 0)
					throw new System.ArgumentException("Type identifier must be greater than 0");

				if (knownIds == null)
					knownIds = new HashSet<int>();

				if (knownIds.Contains(typeId))
					throw new System.ArgumentException(string.Format("Type identifier '{0}' declared more than once for type {1}", typeId, currentType.Name));

				knownIds.Add(typeId);

				if (type == classType)
				{
					if (id != 0)
						throw new System.ArgumentException(string.Format("Type identifier '{0}' declared more than once for type {1}", classType.Name, currentType.Name));

					id = typeId;
				}
			}
		}

		return id;
	}

	private static List<TypeId> TryGetAsAttributeParams(object[] attributes)
	{
		List<TypeId> attributeParams = new List<TypeId>();

		for (int i = 0; i < attributes.Length; ++i)
		{
			Type attributeType = attributes[i].GetType();

			if (attributeType != typeof(AsAttribute))
				continue;

			byte typeId = AsAttribute.GetId(attributes[i]);
			System.Type classType = AsAttribute.GetType(attributes[i]);

			if (typeId == 0)
				throw new System.ArgumentException("Type identifier must be greater than 0");

			attributeParams.Add(new TypeId(typeId, classType));
		}

		return attributeParams;
	}
}
