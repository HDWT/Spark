using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SparkTest
{
	public static class RandomClassGenerator
	{
		private static readonly Random s_random = new Random(DateTime.UtcNow.Millisecond);

		private static List<System.Type> s_types = new List<Type>()
		{
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(char),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(string),
		};

		public static void Make(int classCount, int fieldCount)
		{
			List<string> sourceCode = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();

			int currentIndent = 0;

			Action<string> addLine = (line) =>
			{
				int indent = currentIndent;

				stringBuilder.Length = 0;

				while (indent-- > 0)
					stringBuilder.Append('\t');

				stringBuilder.Append(line);
				//stringBuilder.Append("\r\n");

				sourceCode.Add(stringBuilder.ToString());
			};

			addLine("#pragma warning disable 414");
			addLine(string.Empty);

			addLine("using System;");
			addLine("using System.Collections;");
			addLine("using System.Collections.Generic;");
			addLine(string.Empty);

			addLine("public class ClassesForTest");
			addLine("{");
			{
				currentIndent++;

				for (int classNumber = 1; classNumber <= classCount; ++classNumber)
				{
					addLine(string.Format("private class Class_N{0}", classNumber));
					addLine("{");
					{
						currentIndent++;

						for (int fieldNumber = 1; fieldNumber <= fieldCount; ++fieldNumber)
						{
							addLine(string.Format("[Spark.Member({0})]", fieldNumber));

							int randomFieldIndex = s_random.Next(0, s_types.Count);
							System.Type randomType = s_types[randomFieldIndex];

							string randomValue = GetRandomValueAsString(randomType);

							addLine(string.Format("private {0} m_field{1} = {2};", randomType.Name, fieldNumber, randomValue));

							if (fieldNumber != fieldCount)
								addLine(string.Empty);
						}

						currentIndent--;
					}
					addLine("}");
					addLine(string.Empty);
				}

				addLine("public static void RunTest()");
				addLine("{");
				{
					currentIndent++;

					for (int classNumber = 1; classNumber <= classCount; ++classNumber)
						addLine(string.Format("Spark.Serialize(new Class_N{0}());", classNumber));

					currentIndent--;
				}
				addLine("}");

				currentIndent--;
			}
			addLine("}");

			addLine(string.Empty);
			addLine("#pragma warning restore 414");

			File.WriteAllLines("../../../GeneratedClasses.auto.cs", sourceCode.ToArray());
			File.Copy("../../../GeneratedClasses.auto.cs", "../../../GeneratedClasses.auto.cs.tmp", true);
		}

		private static string GetRandomValueAsString(System.Type type)
		{
			if (type == typeof(bool))
				return s_random.Next(2) == 0 ? "false" : "true";

			if (type == typeof(byte))
				return s_random.Next(byte.MinValue, byte.MaxValue + 1).ToString();

			if (type == typeof(sbyte))
				return s_random.Next(sbyte.MinValue, sbyte.MaxValue + 1).ToString();

			if (type == typeof(char))
				return "(Char)" + s_random.Next(char.MinValue, char.MaxValue + 1).ToString();

			if (type == typeof(short))
				return s_random.Next(short.MinValue, short.MaxValue + 1).ToString();

			if (type == typeof(ushort))
				return s_random.Next(ushort.MinValue, ushort.MaxValue + 1).ToString();

			if (type == typeof(int))
				return s_random.Next(int.MinValue, int.MaxValue).ToString();

			if (type == typeof(uint))
			{
				uint v = (uint)s_random.Next(0, int.MaxValue) + (uint)s_random.Next(0, int.MaxValue);

				return v.ToString();
			}

			if (type == typeof(long))
			{
				long v = (long)s_random.Next(int.MinValue, int.MaxValue);

				v = v << 32;
				v += (long)s_random.Next(int.MinValue, int.MaxValue);

				return v.ToString();
			}

			if (type == typeof(ulong))
			{
				ulong v = (ulong)s_random.Next(0, int.MaxValue) + (ulong)s_random.Next(0, int.MaxValue);

				v = v << 32;
				v += (ulong)s_random.Next(0, int.MaxValue) + (ulong)s_random.Next(0, int.MaxValue);

				return v.ToString();
			}

			if (type == typeof(float))
			{
				short v1 = (short)s_random.Next(short.MinValue, short.MaxValue);
				short v2 = (byte)s_random.Next(byte.MinValue, byte.MaxValue);

				return string.Format("{0}.{1}f", v1, v2);
			}

			if (type == typeof(double))
			{
				short v1 = (short)s_random.Next(short.MinValue, short.MaxValue);
				short v2 = (byte)s_random.Next(byte.MinValue, byte.MaxValue);

				return string.Format("{0}.{1}", v1, v2);
			}

			if (type == typeof(decimal))
			{
				decimal v = (s_random.Next(2) == 0)
					?  long.MinValue - s_random.Next(0, int.MaxValue)
					: long.MaxValue + s_random.Next(0, int.MaxValue);

				return v.ToString();
			}

			if (type == typeof(DateTime))
				return string.Format("new DateTime({0}, {1}, {2})", s_random.Next(1987, 2016), s_random.Next(1, 13), s_random.Next(1, 29));

			if (type == typeof(string))
			{
				int stringLength = s_random.Next(40);

				if (stringLength < 5)
					return "null";

				if (stringLength < 10)
					return "string.Empty";

				string v = string.Empty;

				while (stringLength-- > 0)
				{
					char ch = (char)(s_random.Next(32, 126));

					if ((ch == '\'') || (ch == '\"') || (ch == '\\'))
						v += '\\';

					v += ch;
				}

				return string.Format("\"{0}\"", v);
			}

			throw new System.ArgumentException("Not supported type " + type);
		}
	}
}