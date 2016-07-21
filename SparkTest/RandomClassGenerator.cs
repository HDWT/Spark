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
			if (classCount < 4)
				classCount = 4;

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

				List<string> randomFieldValues = new List<string>();
				List<bool> isArrayField = new List<bool>();

				for (int classNumber = 1; classNumber <= classCount; ++classNumber)
				{
					randomFieldValues.Clear();
					isArrayField.Clear();

					string parent = string.Empty;

					if (classNumber % 2 == 0)
						parent = string.Format(" : Class_N{0}", classNumber - 1);

					addLine(string.Format("private class Class_N{0}{1}", classNumber, parent));
					addLine("{");
					{
						currentIndent++;

						for (int fieldNumber = 1; fieldNumber <= fieldCount; ++fieldNumber)
						{
							bool isArray = (s_random.Next(0, 10) > 7);

							isArrayField.Add(isArray);

							addLine(string.Format("[Spark.Member({0})]", fieldNumber));

							string randomTypeName = string.Empty;
							string randomValue = string.Empty;

							int randomFieldIndex = s_random.Next(0, s_types.Count + 1);

							if (randomFieldIndex == s_types.Count)
							{
								int randomClassNumber = classNumber + 2;

								// Any base random type
								if (randomClassNumber > classCount)
								{
									randomFieldIndex = s_random.Next(0, s_types.Count);

									System.Type randomType = s_types[randomFieldIndex];
									randomTypeName = randomType.Name;

									if (isArray)
									{
										int arrayLength = s_random.Next(-1, 10);

										if (arrayLength == -1)
										{
											randomValue = "null";
										}
										else
										{
											randomValue = string.Format("new {0}[{1}]", randomType, arrayLength);

											if (arrayLength > 0)
											{
												randomValue += " { ";

												for (int arrayElement = 0; arrayElement < arrayLength; ++arrayElement)
												{
													if (arrayElement != 0)
														randomValue += ", ";

													randomValue += GetRandomValueAsString(randomType);
												}

												randomValue += " }";
											}
										}

										randomTypeName += "[]";
									}
									else
									{
										randomValue = GetRandomValueAsString(randomType);
									}
								}
								else // Random class
								{
									randomTypeName = string.Format("Class_N{0}", randomClassNumber);

									if (isArray)
									{
										int arrayLength = s_random.Next(-1, 10);

										if (arrayLength == -1)
										{
											randomValue = "null";
										}
										else
										{
											randomValue = string.Format("new Class_N{0}[{1}]", randomClassNumber, arrayLength);

											if (arrayLength > 0)
											{
												randomValue += " { ";

												for (int arrayElement = 0; arrayElement < arrayLength; ++arrayElement)
												{
													if (arrayElement != 0)
														randomValue += ", ";

													randomValue += (s_random.Next(10) > 3)
														? randomValue = string.Format("new Class_N{0}(true)", randomClassNumber)
														: "null";
												}

												randomValue += " }";
											}
										}

										randomTypeName += "[]";
									}
									else
									{
										randomValue = (s_random.Next(10) > 3)
											? randomValue = string.Format("new Class_N{0}(true)", randomClassNumber)
											: "null";
									}
								}
							}
							else
							{
								System.Type randomType = s_types[randomFieldIndex];
								randomTypeName = randomType.Name;

								if (isArray)
								{
									int arrayLength = s_random.Next(-1, 10);

									if (arrayLength == -1)
									{
										randomValue = "null";
									}
									else
									{
										randomValue = string.Format("new {0}[{1}]", randomType, arrayLength);

										if (arrayLength > 0)
										{
											randomValue += " { ";

											for (int arrayElement = 0; arrayElement < arrayLength; ++arrayElement)
											{
												if (arrayElement != 0)
													randomValue += ", ";

												randomValue += GetRandomValueAsString(randomType);
											}

											randomValue += " }";
										}
									}

									randomTypeName += "[]";
								}
								else
								{
									randomValue = GetRandomValueAsString(randomType);
								}
							}

							randomFieldValues.Add(randomValue);

							string randomAccessType = GetRandomAccessType();

							addLine(string.Format("{0} {1} m_field{2}_{3};", randomAccessType, randomTypeName, classNumber, fieldNumber));
							addLine(string.Empty);
						}

						addLine(string.Format("public Class_N{0}()", classNumber));
						addLine("{ }");
						addLine(string.Empty);

						//
						addLine(string.Format("public Class_N{0}(bool anything)", classNumber));

						if (parent != string.Empty)
							addLine("\t: base(anything)");

						addLine("{");
						{
							currentIndent++;

							for (int fieldNumber = 1; fieldNumber <= fieldCount; ++fieldNumber)
								addLine(string.Format("m_field{0}_{1} = {2};", classNumber, fieldNumber, randomFieldValues[fieldNumber - 1]));

							currentIndent--;
						}
						addLine("}");
						addLine(string.Empty);

						//
						addLine("public override bool Equals(object obj)");
						addLine("{");
						{
							currentIndent++;

							addLine(string.Format("Class_N{0} other = obj as Class_N{0};", classNumber));
							addLine(string.Empty);

							if (parent != string.Empty)
							{
								addLine("return base.Equals(obj)");

								if (isArrayField[0])
									addLine(string.Format("\t&& ArraysEqual(m_field{0}_{1}, other.m_field{0}_{1})", classNumber, 1));
								else
									addLine(string.Format("\t&& object.Equals(m_field{0}_{1}, other.m_field{0}_{1})", classNumber, 1));
							}
							else if (fieldCount == 0)
							{
								addLine("return true");
							}
							else
							{
								if (isArrayField[0])
									addLine(string.Format("return ArraysEqual(m_field{0}_{1}, other.m_field{0}_{1})", classNumber, 1));
								else
									addLine(string.Format("return object.Equals(m_field{0}_{1}, other.m_field{0}_{1})", classNumber, 1));
							}

							for (int fieldNumber = 2; fieldNumber <= fieldCount; ++fieldNumber)
							{
								if (isArrayField[fieldNumber - 1])
									addLine(string.Format("\t&& ArraysEqual(m_field{0}_{1}, other.m_field{0}_{1})", classNumber, fieldNumber));
								else
									addLine(string.Format("\t&& object.Equals(m_field{0}_{1}, other.m_field{0}_{1})", classNumber, fieldNumber));
							}

							addLine(";");

							currentIndent--;
						}
						addLine("}");
						addLine(string.Empty);

						//
						addLine("public override int GetHashCode()");
						addLine("{");
						{
							currentIndent++;

							addLine("return base.GetHashCode();");

							currentIndent--;
						}
						addLine("}");

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
					{
						if (classNumber > 1)
							addLine("//");

						addLine(string.Format("Class_N{0} sN{0} = new Class_N{0}(true);", classNumber));
						addLine(string.Format("byte[] bN{0} = Spark.Serialize(sN{0});", classNumber));
						addLine(string.Format("Class_N{0} dN{0} = Spark.Deserialize<Class_N{0}>(bN{0});", classNumber));

						addLine(string.Empty);
						addLine(string.Format("if (!sN{0}.Equals(dN{0}))", classNumber));
						addLine(string.Format("\tthrow new System.ArgumentException(\"Class_N{0} not equal after deserialization\");", classNumber));

						if (classNumber < classCount)
							addLine(string.Empty);
					}

					currentIndent--;
				}
				addLine("}");
				addLine(string.Empty);

				addLine("public static bool ArraysEqual<T>(T[] a1, T[] a2)");
				addLine("{");
				{
					currentIndent++;

					addLine("if (ReferenceEquals(a1, a2))");
					addLine("\treturn true;");
					addLine(string.Empty);

					addLine("if (a1 == null || a2 == null)");
					addLine("\treturn false;");
					addLine(string.Empty);

					addLine("if (a1.Length != a2.Length)");
					addLine("\treturn false;");
					addLine(string.Empty);

					addLine("EqualityComparer<T> comparer = EqualityComparer<T>.Default;");
					addLine(string.Empty);

					addLine("for (int i = 0; i < a1.Length; i++)");
					addLine("{");
					{
						currentIndent++;

						addLine("if (!comparer.Equals(a1[i], a2[i]))");
						addLine("\treturn false;");

						currentIndent--;
					}
					addLine("}");

					addLine(string.Empty);
					addLine("return true;");

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

		private static string GetRandomAccessType()
		{
			int index = s_random.Next(3);

			if (index == 0)
				return "protected";

			return (index == 1) ? "private" : "public";
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