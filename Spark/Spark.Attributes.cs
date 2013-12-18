using System;

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
	}

	private static bool TryGetMemberAttributeId(object[] attributes, out ushort memberId)
	{
		foreach (var attribute in attributes)
		{
			Type attributeType = attribute.GetType();

			if (attributeType != typeof(MemberAttribute))
				continue;

			memberId = (ushort)attributeType.GetField("Id").GetValue(attribute);

			if (memberId >= 32768)
				throw new System.ArgumentException("Member identifier must be less then 32768");

			return true;
		}

		memberId = 0;
		return false;
	}
}
