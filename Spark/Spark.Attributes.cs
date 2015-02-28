using System;
using System.Reflection;

public static partial class Spark
{
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
				 throw new System.ArgumentException(string.Format("Member identifier must be less then {0}", MaxMemberId));

			return true;
		}

		memberId = 0;
		return false;
	}
}
