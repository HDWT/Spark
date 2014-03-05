using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static partial class Spark
{
	private static partial class TypeHelper
	{
		private class DateTimeType : ITypeHelper<DateTime>
		{
			public int GetSize(object value, LinkedList<int> sizes)
			{
				return GetSize((DateTime)value, sizes);
			}

			public int GetSize(DateTime value, LinkedList<int> sizes)
			{
				return TypeHelper.Long.GetSize(value.Ticks, sizes);
			}

			public object ReadObject(Type type, byte[] data, ref int startIndex)
			{
				return Read(data, ref startIndex);
			}

			public DateTime Read(byte[] data, ref int startIndex)
			{
				long ticks = TypeHelper.Long.Read(data, ref startIndex);

				return new DateTime(ticks);
			}

			public void WriteObject(object value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				Write((DateTime)value, data, ref startIndex, sizes);
			}

			public void Write(DateTime value, byte[] data, ref int startIndex, LinkedList<int> sizes)
			{
				TypeHelper.Long.Write(value.Ticks, data, ref startIndex, sizes);
			}
		}
	}
}