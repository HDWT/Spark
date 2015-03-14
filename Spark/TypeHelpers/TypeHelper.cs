﻿using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private interface ITypeHelper<T>
	{
		int GetSize(object value);
		int GetSize(T value);

		object ReadObject(Type type, byte[] data, ref int startIndex);
		T Read(byte[] data, ref int startIndex);
		
		void WriteObject(object value, byte[] data, ref int startIndex);
		void Write(T value, byte[] data, ref int startIndex);
	}

	private interface IReferenceTypeHelper<T> where T : class
	{
		ISizeGetter<T> GetSizeGetter(Type type);
		IDataWriter<T> GetDataWriter(Type type);

		object ReadObject(Type type, byte[] data, ref int startIndex);
		T Read(byte[] data, ref int startIndex);
	}

	private interface ISizeGetter<T>
	{
		int GetObjectSize(object instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);
		int GetSize(T instance, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);
	}

	private interface IDataWriter<T>
	{
		void WriteObject(object instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);
		void Write(T instance, byte[] data, ref int startIndex, QueueWithIndexer<int> sizes, QueueWithIndexer<object> values);
	}

	private static partial class TypeHelper
	{
		public static readonly ITypeHelper<bool>					Bool		= new BoolType();
		public static readonly ITypeHelper<byte>					Byte		= new ByteType();
		public static readonly ITypeHelper<sbyte>					SByte		= new SByteType();
		public static readonly ITypeHelper<char>					Char		= new CharType();
		public static readonly ITypeHelper<short>					Short		= new ShortType();
		public static readonly ITypeHelper<ushort>					UShort		= new UShortType();
		//public static readonly ITypeHelper<int>					Int			= new IntType1();
		public static readonly ITypeHelper<uint>					UInt		= new UIntType();
		public static readonly ITypeHelper<long>					Long		= new LongType();
		public static readonly ITypeHelper<ulong>					ULong		= new ULongType();
		public static readonly ITypeHelper<float>					Float		= new FloatType();
		public static readonly ITypeHelper<double>					Double		= new DoubleType();
		public static readonly ITypeHelper<decimal>					Decimal		= new DecimalType();
		public static readonly ITypeHelper<DateTime>				DateTime	= new DateTimeType();

		public static readonly IReferenceTypeHelper<string>			String		= new StringType();
		public static readonly IReferenceTypeHelper<Array>			Array		= new ArrayType();
		public static readonly IReferenceTypeHelper<IList>			List		= new ListType();
		public static readonly IReferenceTypeHelper<IDictionary>	Dictionary	= new DictionaryType();
		public static readonly IReferenceTypeHelper<object>			Object		= new ObjectType();
	}
}
