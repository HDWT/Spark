﻿using System;
using System.Reflection;
using System.Reflection.Emit;

public static partial class Spark
{
	private interface IDataMember
	{
		ushort Id { get; }
		System.Type Type { get; }

		int GetSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);
		void WriteValue(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);
		void ReadValue(object instance, byte[] data, ref int startIndex);
		void SetValue(object instance, object value);
	}

	const byte IgnoreDataSizeBlockMark = 128;

	private class SizeGetter
	{
		private GetValueSizeDelegate m_getValueTypeSize = null;
		private GetReferenceSizeDelegate m_getReferenceTypeSize = null;

		public SizeGetter(GetValueSizeDelegate getValueTypeSize)
		{
			m_getValueTypeSize = getValueTypeSize;
		}

		public SizeGetter(GetReferenceSizeDelegate getReferenceTypeSize)
		{
			m_getReferenceTypeSize = getReferenceTypeSize;
		}

		public int GetSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			return (m_getValueTypeSize != null) ? m_getValueTypeSize(instance) : m_getReferenceTypeSize(instance, sizes, values);
		}
	}

	private class DataWriter
	{
		private WriteValueDelegate m_writeValueType = null;
		private WriteReferenceDelegate m_writeReferenceType = null;

		public DataWriter(WriteValueDelegate writeValueType)
		{
			m_writeValueType = writeValueType;
		}

		public DataWriter(WriteReferenceDelegate writeReferenceType)
		{
			m_writeReferenceType = writeReferenceType;
		}

		public void Write(object value, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			if (m_writeValueType != null)
				m_writeValueType(value, data, ref startIndex);
			else
				m_writeReferenceType(value, data, ref startIndex, sizes, values);
		}
	}

	// Aot-compile
	private class DataMember : IDataMember
	{
		protected const int HeaderSize = sizeof(ushort);

		private ushort m_id = 0;
		private bool m_ignoreDataSizeBlock = false;

		private Type m_type = null;
		private TypeFlags m_typeFlags;
		private bool m_isClass = false;

		protected FieldInfo m_fieldInfo = null;
		protected PropertyInfo m_propertyInfo = null;

		public ushort Id { get { return m_id; } }
		public Type Type { get { return m_type; } }
		public bool IsField { get { return m_fieldInfo != null; } }
		public bool IsProperty { get { return m_propertyInfo != null; } }

		protected readonly ReadDataDelegate readData = null;

		protected readonly DataWriter m_dataWriter = null;
		protected readonly SizeGetter m_sizeGetter = null;

		private int m_fieldOffset = -1;

		public DataMember(ushort id, FieldInfo fieldInfo, int fieldOffset)
			: this(id, fieldInfo.FieldType)
		{
			m_fieldInfo = fieldInfo;
			m_fieldOffset = fieldOffset;
			m_isClass = fieldInfo.FieldType.IsClass || fieldInfo.FieldType.IsInterface;
		}

		public DataMember(ushort id, PropertyInfo propertyInfo)
			: this(id, propertyInfo.PropertyType)
		{
			m_propertyInfo = propertyInfo;
			m_isClass = m_propertyInfo.PropertyType.IsClass || m_propertyInfo.PropertyType.IsInterface;
		}

		private DataMember(ushort id, Type type)
		{
			m_id = id;
			m_type = type;
			m_typeFlags = GetTypeFlags(type);

			bool isValueType = ((m_typeFlags & TypeFlags.Value) == TypeFlags.Value);

			if ((m_typeFlags & TypeFlags.Enum) == TypeFlags.Enum)
				EnumTypeHelper.Instance.Register(type);

			m_ignoreDataSizeBlock = isValueType;

			m_sizeGetter = isValueType
				? new SizeGetter(SizeCalculator.GetForValueType(type))
				: new SizeGetter(SizeCalculator.GetForReferenceType(type));

			m_dataWriter = isValueType
				? new DataWriter(Writer.GetDelegateForValueType(type))
				: new DataWriter(Writer.GetDelegateForReferenceType(type));

			readData = Reader.Get(m_type);
		}

		private object GetValue(object instance)
		{
			if (m_fieldOffset == -1)
				return IsField ? m_fieldInfo.GetValue(instance) : m_propertyInfo.GetValue(instance, null); // Indexer ??

			if (IsField)
			{
				FieldAccessor valueTypeAccessor = new FieldAccessor();

				valueTypeAccessor.wObject = FieldAccessor.ObjectWrapperPool.Get();
				valueTypeAccessor.wObject.instance = instance;

				if (Is64Bit)
					valueTypeAccessor.wAddress.address64 += m_fieldOffset;
				else
					valueTypeAccessor.wAddress.address32 += m_fieldOffset;

				object result = valueTypeAccessor.Get(m_fieldInfo.FieldType, m_isClass);
				
				/*
				object reflectionResult = m_fieldInfo.GetValue(instance);

				if ((result == null && reflectionResult != null) || (result != null && reflectionResult == null))
					throw new System.ArgumentException("Something wrong: " + result + " | " + reflectionResult + ".");

				if (result != null && !result.Equals(m_fieldInfo.GetValue(instance)))
					throw new System.ArgumentException("Something wrong: " + result + " | " + reflectionResult + ".");
				*/

				FieldAccessor.ObjectWrapperPool.Return(valueTypeAccessor.wObject);

				return result;	
			}
			else
			{
				return m_propertyInfo.GetValue(instance, null); // Indexer ??
			}
		}

		public virtual void SetValue(object instance, object value)
		{
			if (IsField)
				m_fieldInfo.SetValue(instance, value);

			if (IsProperty)
				m_propertyInfo.SetValue(instance, value, null); // Indexer ??
		}

		public virtual int GetSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			object value = GetValue(instance);

			values.Enqueue(value);

			if (!m_ignoreDataSizeBlock && (value == instance))
				return HeaderSize + 1;

			return HeaderSize + m_sizeGetter.GetSize(value, sizes, values);
		}

		public virtual void WriteValue(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			object value = values.Dequeue();

			WriteHeader(data, ref startIndex);

			if (!m_ignoreDataSizeBlock && (value == instance))
				data[startIndex++] = byte.MaxValue;
			else
				m_dataWriter.Write(value, data, ref startIndex, sizes, values);
		}

		public virtual void ReadValue(object instance, byte[] data, ref int startIndex)
		{
			if (data[startIndex] == byte.MaxValue)
			{
				startIndex++;
				SetValue(instance, instance);
			}
			else
			{
				object value = readData(m_type, data, ref startIndex);

				SetValue(instance, value);
			}
		}

		protected void WriteHeader(byte[] data, ref int startIndex)
		{
			data[startIndex++] = (byte)(m_id);
			data[startIndex++] = (byte)(m_id >> 8);

			if (m_ignoreDataSizeBlock)
				data[startIndex - 1] += IgnoreDataSizeBlockMark; 
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
		private static readonly Type[] DynamicSetMethodParameters = { typeof(object), typeof(T) };

		private delegate T DynamicGetValueDelegate(object instance);
		private delegate void DynamicSetValueDelegate(object instance, T value);

		private DynamicGetValueDelegate m_getValue = null;
		private DynamicSetValueDelegate m_setValue = null;

		public DataMember(ushort id, FieldInfo fieldInfo)
			: base(id, fieldInfo, -1)
		{
			m_getValue = CreateGetMethod(fieldInfo);
			m_setValue = CreateSetMethod(fieldInfo);
		}

		public DataMember(ushort id, PropertyInfo propertyInfo)
			: base(id, propertyInfo)
		{
			m_getValue = CreateGetMethod(m_propertyInfo);
			m_setValue = CreateSetMethod(m_propertyInfo);
		}

		private static DynamicGetValueDelegate CreateGetMethod(FieldInfo fieldInfo)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("DynamicGet_" + fieldInfo.Name, typeof(T), DynamicGetMethodParameters, typeof(DataMember<T>), true);

			ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldfld, fieldInfo);
			ilGenerator.Emit(OpCodes.Ret);

			return (DynamicGetValueDelegate)dynamicMethod.CreateDelegate(typeof(DynamicGetValueDelegate));
		}

		private static DynamicGetValueDelegate CreateGetMethod(PropertyInfo propertyInfo)
		{
			DynamicMethod getter = new DynamicMethod("DynamicGet_" + propertyInfo.Name, typeof(T), DynamicGetMethodParameters, typeof(DataMember<T>), true);

			ILGenerator ilGenerator = getter.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);

			ilGenerator.EmitCall(OpCodes.Callvirt, propertyInfo.GetGetMethod(true), null);
			ilGenerator.Emit(OpCodes.Ret);

			return (DynamicGetValueDelegate)getter.CreateDelegate(typeof(DynamicGetValueDelegate));
		}

		private static DynamicSetValueDelegate CreateSetMethod(FieldInfo fieldInfo)
		{
			DynamicMethod setter = new DynamicMethod("DynamicSet_" + fieldInfo.Name, null, DynamicSetMethodParameters, typeof(DataMember<T>), true);

			ILGenerator ilGenerator = setter.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stfld, fieldInfo);

			ilGenerator.Emit(OpCodes.Ret);

			return (DynamicSetValueDelegate)setter.CreateDelegate(typeof(DynamicSetValueDelegate));
		}

		private static DynamicSetValueDelegate CreateSetMethod(PropertyInfo propertyInfo)
		{
			DynamicMethod setter = new DynamicMethod("DynamicSet_" + propertyInfo.Name, null, DynamicSetMethodParameters, typeof(DataMember<T>), true);

			ILGenerator ilGenerator = setter.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
			ilGenerator.Emit(OpCodes.Ldarg_1);

			ilGenerator.EmitCall(OpCodes.Callvirt, propertyInfo.GetSetMethod(true), null);
			ilGenerator.Emit(OpCodes.Ret);

			return (DynamicSetValueDelegate)setter.CreateDelegate(typeof(DynamicSetValueDelegate));
		}

		public override int GetSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			T value = m_getValue(instance);

			return HeaderSize + m_sizeGetter.GetSize(value, sizes, values);
		}

		public override void SetValue(object instance, object value)
		{
			if (m_setValue == null)
				base.SetValue(instance, value);
			else
				m_setValue(instance, (T)value);
		}

		public override void WriteValue(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values)
		{
			T value = m_getValue(instance);

			WriteHeader(data, ref startIndex);
			m_dataWriter.Write(value, data, ref startIndex, sizes, values);
		}
	}
}