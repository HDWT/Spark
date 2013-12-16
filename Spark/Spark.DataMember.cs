using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

public static partial class Spark
{
	private interface IDataMember
	{
		ushort Id { get; }

		int GetSize(object instance);

		void SetValue(object instance, object value);
		void WriteValue(object instance, byte[] data, ref int startIndex);
		void ReadValue(object instance, byte[] data, ref int startIndex);
	}

	// Aot-compile
	private class DataMember : IDataMember
	{
		protected const int HeaderSize = sizeof(ushort);

		private ushort m_id = 0;
		private Type m_type = null;
		private ConstructorInfo m_constructor = null;

		protected FieldInfo m_fieldInfo = null;
		protected PropertyInfo m_propertyInfo = null;

		public ushort Id { get { return m_id; } }
		public bool IsField { get { return m_fieldInfo != null; } }
		public bool IsProperty { get { return m_propertyInfo != null; } }

		private readonly GetSizeDelegate getSize = null;
		private readonly WriteDataDelegate writeData = null;
		private readonly ReadDataDelegate readData = null;

		public DataMember(ushort id, FieldInfo fieldInfo)
			: this(id, fieldInfo.FieldType)
		{
			m_fieldInfo = fieldInfo;
		}

		public DataMember(ushort id, PropertyInfo propertyInfo)
			: this(id, propertyInfo.PropertyType)
		{
			m_propertyInfo = propertyInfo;
		}

		private DataMember(ushort id, Type type)
		{
			m_id = id;
			m_type = type;
			m_constructor = m_type.GetConstructor(Type.EmptyTypes);

			if (type.IsEnum)
				EnumTypeHelper.Instance.Register(type);

			getSize = SizeCalculator.Get(m_type);
			writeData = Writer.Get(m_type);
			readData = Reader.Get(m_type);
		}

		private object GetValue(object instance)
		{
			return IsField ? m_fieldInfo.GetValue(instance) : m_propertyInfo.GetValue(instance, null); // Indexer ??
		}

		public virtual void SetValue(object instance, object value)
		{
			if (IsField)
				m_fieldInfo.SetValue(instance, value);

			if (IsProperty)
				m_propertyInfo.SetValue(instance, value, null); // Indexer ??
		}

		public virtual int GetSize(object instance)
		{
			object value = GetValue(instance);

			return HeaderSize + getSize(value);
		}

		public virtual void WriteValue(object instance, byte[] data, ref int startIndex)
		{
			object value = GetValue(instance);

			WriteHeader(data, ref startIndex);
			writeData(value, data, ref startIndex);
		}

		public virtual void ReadValue(object instance, byte[] data, ref int startIndex)
		{
			object value = readData(m_type, data, ref startIndex);

			SetValue(instance, value);
		}

		protected void WriteHeader(byte[] data, ref int startIndex)
		{
			data[startIndex++] = (byte)(m_id);
			data[startIndex++] = (byte)(m_id >> 8);
		}

		protected ushort ReadHeader(byte[] data, ref int startIndex)
		{
			return (ushort)(data[startIndex++] << 8 + data[startIndex++]);
		}
	}

	// Jit-compile
	private class DataMember<T> : DataMember
	{
		private static readonly Type[] DynamicGetMethodParameters = { typeof(object) };

		private delegate T DynamicGetValueDelegate(object instance);
		private DynamicGetValueDelegate dynamicGetValue = null;

		private bool m_isLowLevelType = false;

		public DataMember(ushort id, FieldInfo fieldInfo)
			: base(id, fieldInfo)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("DynamicGet_" + fieldInfo.Name, typeof(T), DynamicGetMethodParameters, typeof(DataMember<T>), true);

			ILGenerator il = dynamicMethod.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldInfo);
			il.Emit(OpCodes.Ret);

			dynamicGetValue = (DynamicGetValueDelegate)dynamicMethod.CreateDelegate(typeof(DynamicGetValueDelegate));

			m_isLowLevelType = IsLowLevelType(typeof(T));
		}

		public DataMember(ushort id, PropertyInfo propertyInfo)
			: base(id, propertyInfo)
		{
			m_isLowLevelType = IsLowLevelType(typeof(T));
		}

		private T GetValue(object instance)
		{
			return IsField ? dynamicGetValue(instance) : (T)m_propertyInfo.GetValue(instance, null); // Indexer ??
		}

		public override int GetSize(object instance)
		{
			if (m_isLowLevelType)
			{
				T value = GetValue(instance);

				return HeaderSize + SizeCalculator.Evaluate<T>(value);
			}
			else
			{
				return base.GetSize(instance);
			}
		}

		public override void WriteValue(object instance, byte[] data, ref int startIndex)
		{
			if (m_isLowLevelType)
			{
				T value = GetValue(instance);

				WriteHeader(data, ref startIndex);
				Writer.Write<T>(value, data, ref startIndex);
			}
			else
			{
				base.WriteValue(instance, data, ref startIndex);
			}
		}
	}
}