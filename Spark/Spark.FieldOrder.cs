using System;
using System.Collections.Generic;
using System.Reflection;

public static partial class Spark
{
	private static class FieldOrder
	{
		private static readonly Dictionary<System.Type, int> s_ordersByType = new Dictionary<Type, int>();
		private static readonly List<FieldInfo>[] s_fieldsByOrder = new List<FieldInfo>[6]; // 6 orders now

		static FieldOrder()
		{
			if (IsMono)
			{
				// Reference if 64-bit

				s_ordersByType.Add(typeof(long),		1);
				s_ordersByType.Add(typeof(double),		1);
				s_ordersByType.Add(typeof(ulong),		1);

				// Reference if 32-bit

				s_ordersByType.Add(typeof(int),			1);
				s_ordersByType.Add(typeof(uint),		1);
				s_ordersByType.Add(typeof(float),		1);

				s_ordersByType.Add(typeof(short),		1);
				s_ordersByType.Add(typeof(ushort),		1);
				s_ordersByType.Add(typeof(char),		1);

				s_ordersByType.Add(typeof(bool),		1);
				s_ordersByType.Add(typeof(sbyte),		1);
				s_ordersByType.Add(typeof(byte),		1);

				s_ordersByType.Add(typeof(decimal),		1);
				s_ordersByType.Add(typeof(DateTime),	1);
			}
			else
			{
				// Reference if 64-bit

				s_ordersByType.Add(typeof(long),	Is64Bit ? 1 : 0);
				s_ordersByType.Add(typeof(double),	Is64Bit ? 1 : 0);
				s_ordersByType.Add(typeof(ulong),	Is64Bit ? 1 : 0);

				// Reference if 32-bit

				s_ordersByType.Add(typeof(int),			2);
				s_ordersByType.Add(typeof(uint),		2);
				s_ordersByType.Add(typeof(float),		2);

				s_ordersByType.Add(typeof(short),		3);
				s_ordersByType.Add(typeof(ushort),		3);
				s_ordersByType.Add(typeof(char),		3);

				s_ordersByType.Add(typeof(bool),		4);
				s_ordersByType.Add(typeof(sbyte),		4);
				s_ordersByType.Add(typeof(byte),		4);

				s_ordersByType.Add(typeof(decimal),		5);
				s_ordersByType.Add(typeof(DateTime),	5);
			}

			//
			for (int i = 0; i < s_fieldsByOrder.Length; ++i)
				s_fieldsByOrder[i] = new List<FieldInfo>();
		}

		/// <summary> </summary>
		public static int Get(FieldInfo field)
		{
			System.Type fieldType = field.FieldType;

			if (fieldType.IsClass || fieldType.IsInterface)
				return IsMono /* !! check mono x86 */ || Is64Bit ? 0 : 1;

			if (fieldType.IsEnum)
				fieldType = EnumTypeHelper.GetUnderlyingType(fieldType);

			int fieldOrder = int.MaxValue;

			if (!s_ordersByType.TryGetValue(fieldType, out fieldOrder))
				throw new ArgumentException(string.Format("Type '{0}' is not suppoerted", fieldType));

			return fieldOrder;
		}

		/// <summary> </summary>
		public static FieldInfo[] GetSortedFields(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

			lock (s_fieldsByOrder)
			{
				foreach (var list in s_fieldsByOrder)
					list.Clear();

				foreach (var field in fields)
				{
					int fieldOrder = Get(field);

					s_fieldsByOrder[fieldOrder].Add(field);
				}

				for (int i = 0, row = 0, col = 0; i < fields.Length; ++i, ++col)
				{
					while (col >= s_fieldsByOrder[row].Count)
					{
						row++;
						col = 0;
					}

					fields[i] = s_fieldsByOrder[row][col];
				}
			}

			return fields;
		}
	}
}