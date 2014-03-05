using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private interface ITypeHelper<T>
	{
		int GetSize(object value, LinkedList<int> sizes);
		int GetSize(T value, LinkedList<int> sizes);

		object ReadObject(Type type, byte[] data, ref int startIndex);
		T Read(byte[] data, ref int startIndex);
		
		void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes);
		void Write(T value, byte[] data, ref int startIndex, LinkedList<int> sizes);
	}

	private abstract class TypeHelper<T>
	{
		private static ITypeHelper<T> m_typeHelper = null;

		public static void Create(ITypeHelper<T> typeHelper)
		{
			m_typeHelper = typeHelper;
		}

		public static int GetSize(T value, LinkedList<int> sizes)
		{
			return m_typeHelper.GetSize(value, sizes);
		}

		public static void Write(T value, byte[] data, ref int startIndex, LinkedList<int> sizes)
		{
			m_typeHelper.Write(value, data, ref startIndex, sizes);
		}
	}

	private static partial class TypeHelper
	{
		public static readonly ITypeHelper<bool>		Bool		= new BoolType();
		public static readonly ITypeHelper<byte>		Byte		= new ByteType();
		public static readonly ITypeHelper<sbyte>		SByte		= new SByteType();
		public static readonly ITypeHelper<char>		Char		= new CharType();
		public static readonly ITypeHelper<short>		Short		= new ShortType();
		public static readonly ITypeHelper<ushort>		UShort		= new UShortType();
		public static readonly ITypeHelper<int>			Int			= new IntType();
		public static readonly ITypeHelper<uint>		UInt		= new UIntType();
		public static readonly ITypeHelper<long>		Long		= new LongType();
		public static readonly ITypeHelper<ulong>		ULong		= new ULongType();
		public static readonly ITypeHelper<float>		Float		= new FloatType();
		public static readonly ITypeHelper<double>		Double		= new DoubleType();
		public static readonly ITypeHelper<decimal>		Decimal		= new DecimalType();
		public static readonly ITypeHelper<string>		String		= new StringType();
		public static readonly ITypeHelper<DateTime>	DateTime	= new DateTimeType();
		public static readonly ITypeHelper<Array>		Array		= new ArrayType();
		public static readonly ITypeHelper<IList>		List		= new ListType();
		public static readonly ITypeHelper<object>		Object		= new ObjectType();
	}
}
