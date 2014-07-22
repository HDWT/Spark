﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

public static partial class Spark
{
	private interface IDataMember
	{
		ushort Id { get; }

		int GetSize(object instance, QueueWithIndexer sizes);
		void WriteValue(object instance, byte[] data, ref int startIndex, QueueWithIndexer sizes);
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

		public int GetSize(object instance, QueueWithIndexer sizes)
		{
			return (m_getValueTypeSize != null) ? m_getValueTypeSize(instance) : m_getReferenceTypeSize(instance, sizes);
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

		public void Write(object value, byte[] data, ref int startIndex, QueueWithIndexer sizes)
		{
			if (m_writeValueType != null)
				m_writeValueType(value, data, ref startIndex);
			else
				m_writeReferenceType(value, data, ref startIndex, sizes);
		}
	}

	// Aot-compile
	private class DataMember : IDataMember
	{
		protected const int HeaderSize = sizeof(ushort);

		private ushort m_id = 0;
		private bool m_ignoreDataSizeBlock = false;

		private Type m_type = null;
		private ConstructorInfo m_constructor = null;

		protected FieldInfo m_fieldInfo = null;
		protected PropertyInfo m_propertyInfo = null;

		public ushort Id { get { return m_id; } }
		public bool IsField { get { return m_fieldInfo != null; } }
		public bool IsProperty { get { return m_propertyInfo != null; } }

		//protected readonly GetSizeDelegate getSize = null;
		//protected readonly WriteValueDelegate writeData = null;
		protected readonly ReadDataDelegate readData = null;

		protected readonly DataWriter m_dataWriter = null;
		protected readonly SizeGetter m_sizeGetter = null;

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

			TypeFlags typeFlags = GetTypeFlags(type);
			m_ignoreDataSizeBlock = ((typeFlags & TypeFlags.Value) == TypeFlags.Value) && (typeFlags != TypeFlags.Value);

			m_type = type;
			m_constructor = m_type.GetConstructor(Type.EmptyTypes);

			if ((typeFlags & TypeFlags.Enum) == TypeFlags.Enum)
				EnumTypeHelper.Instance.Register(type);

			bool isValueType = type.IsValueType;

			m_sizeGetter = isValueType
				? new SizeGetter(SizeCalculator.GetForValueType(type))
				: new SizeGetter(SizeCalculator.GetForReferenceType(type));

			m_dataWriter = isValueType
				? new DataWriter(Writer.GetDelegateForValueType(type))
				: new DataWriter(Writer.GetDelegateForReferenceType(type));

			//getSize = SizeCalculator.Get(m_type);
			//writeData = Writer.Get(m_type);

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

		//public virtual int GetSize(object instance)
		//{
		//	object value = GetValue(instance);

		//	return HeaderSize + getSize(value);
		//}

		public virtual int GetSize(object instance, QueueWithIndexer sizes)
		{
			object value = GetValue(instance);

			return HeaderSize + m_sizeGetter.GetSize(value, sizes);
		}

		//public virtual void WriteValue(object instance, byte[] data, ref int startIndex)
		//{
		//	object value = GetValue(instance);

		//	WriteHeader(data, ref startIndex);
		//	writeData(value, data, ref startIndex, null);
		//}

		public virtual void WriteValue(object instance, byte[] data, ref int startIndex, QueueWithIndexer sizes)
		{
			object value = GetValue(instance);

			WriteHeader(data, ref startIndex);
			m_dataWriter.Write(value, data, ref startIndex, sizes);
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
			: base(id, fieldInfo)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("DynamicGet_" + fieldInfo.Name, typeof(T), DynamicGetMethodParameters, typeof(DataMember<T>), true);

			ILGenerator il = dynamicMethod.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldInfo);
			il.Emit(OpCodes.Ret);

			m_getValue = (DynamicGetValueDelegate)dynamicMethod.CreateDelegate(typeof(DynamicGetValueDelegate));
		}

		public DataMember(ushort id, PropertyInfo propertyInfo)
			: base(id, propertyInfo)
		{
			m_getValue = CreateGetMethod(m_propertyInfo);
			//m_setPropertyValue = CreateSetMethod(m_propertyInfo);
		}

		private static DynamicGetValueDelegate CreateGetMethod(PropertyInfo propertyInfo)
		{
			DynamicMethod getter = new DynamicMethod("DynamicGet_" + propertyInfo.Name, 
				MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, 
				typeof(T), DynamicGetMethodParameters, propertyInfo.DeclaringType, false);

			ILGenerator getIL = getter.GetILGenerator();

			//getIL.DeclareLocal(typeof(T));
			getIL.Emit(OpCodes.Ldarg_0); //Load the first argument
			//(target object)
			//Cast to the source type
			//getIL.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);


			//Get the property value
			getIL.EmitCall(OpCodes.Call, propertyInfo.GetGetMethod(), null);
			//if (typeof(T).IsValueType)
			//{
			//	getIL.Emit(OpCodes.Box, typeof(T));
			//	//Box if necessary
			//}
			//getIL.Emit(OpCodes.Stloc_0); //Store it

			//getIL.Emit(OpCodes.Ldloc_0);
			getIL.Emit(OpCodes.Ret);

			/*
			ILGenerator ilGenerator = getter.GetILGenerator();

			ilGenerator.DeclareLocal(typeof(T));
			ilGenerator.Emit(OpCodes.Ldarg_0);

			if (propertyInfo.ReflectedType.IsClass || propertyInfo.ReflectedType.IsInterface)
				ilGenerator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
			else
				ilGenerator.Emit(OpCodes.Unbox, propertyInfo.ReflectedType);

			//ilGenerator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
			ilGenerator.EmitCall(OpCodes.Callvirt, propertyInfo.GetGetMethod(), null);

			//if (!propertyInfo.PropertyType.IsClass)
			//	ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);

			ilGenerator.Emit(OpCodes.Ret);*/

			return (DynamicGetValueDelegate)getter.CreateDelegate(typeof(DynamicGetValueDelegate));
		}

		private static DynamicSetValueDelegate CreateSetMethod(PropertyInfo propertyInfo)
		{
			DynamicMethod setter = new DynamicMethod("DynamicSet_" + propertyInfo.Name, typeof(void), DynamicSetMethodParameters, propertyInfo.DeclaringType);

			ILGenerator ilGenerator = setter.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
			ilGenerator.Emit(OpCodes.Ldarg_1);

			if (propertyInfo.PropertyType.IsClass)
				ilGenerator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
			else
				ilGenerator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

			ilGenerator.EmitCall(OpCodes.Callvirt, propertyInfo.GetSetMethod(), null);
			ilGenerator.Emit(OpCodes.Ret);

			return (DynamicSetValueDelegate)setter.CreateDelegate(typeof(DynamicSetValueDelegate));
		}

		//private T GetValue(object instance)
		//{
		//	return IsField ? m_getFieldValue(instance) : m_getPropertyValue(instance);

		//	//return IsField ? m_getFieldValue(instance) : (T)m_propertyInfo.GetValue(instance, null); // Indexer ??
		//}

		//public override int GetSize(object instance)
		//{
		//	T value = m_getValue(instance);

		//	return HeaderSize + getSize(value);
		//}

		public override int GetSize(object instance, QueueWithIndexer sizes)
		{
			//Stopwatch.Start();
			T value = m_getValue(instance);
			//object value = m_getValue(instance);

			int size = HeaderSize + m_sizeGetter.GetSize(value, sizes);
			//Stopwatch.Stop();

			return size;
		}

		//public override void WriteValue(object instance, byte[] data, ref int startIndex)
		//{
		//	T value = m_getValue(instance);

		//	WriteHeader(data, ref startIndex);
		//	writeData(value, data, ref startIndex, null);
		//}

		public override void WriteValue(object instance, byte[] data, ref int startIndex, QueueWithIndexer sizes)
		{
			T value = m_getValue(instance);
			//object value = m_getValue(instance);

			WriteHeader(data, ref startIndex);
			m_dataWriter.Write(value, data, ref startIndex, sizes);
		}
	}
}