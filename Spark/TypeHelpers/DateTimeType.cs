using System;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		public static class DateTimeType
		{
			public static int GetSize(object value)
			{
				return GetSize((DateTime)value);
			}

			public static int GetSize(DateTime value)
			{
				return LongType.GetSize(value.Ticks);
			}

			public static object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public static DateTime Read(byte[] data, ref int startIndex)
			{
				long ticks = LongType.Read(data, ref startIndex);

				return new DateTime(ticks);
			}

			public static void WriteObject(object value, byte[] data, ref int startIndex)
			{
				Write((DateTime)value, data, ref startIndex);
			}

			public static void Write(DateTime value, byte[] data, ref int startIndex)
			{
				LongType.Write(value.Ticks, data, ref startIndex);
			}
		}
	}
}