using System;
using System.Collections.Generic;
#pragma warning disable 0649,168
public static partial class Spark
{
	public static class TestLowLevel
	{
		private static System.Random random = new Random(DateTime.Now.Millisecond);
		//private static byte[] data = null;
		private static Color[] colorValues = (Color[])Enum.GetValues(typeof(Color));

		private enum Color : int
		{
			Black = -1000000,
			White = -100000,
			Red = -10000,
			Green = -1000,
			Blue = -100,
			Yellow = -10,
			Orange = -1,
			Grey = 0,
			Pink = 1,
			Purple = 10,
			Brown = 100,
			Magenta = 1000,
			Violet = 10000,
			Silver = 100000,
			AlphaChanel = 1000000,
		}

		[System.Flags]
		private enum EFlag
		{
			Flag1 = 1 << 0,
			Flag2 = 1 << 1,
			Flag3 = 1 << 2,
			Flag4 = 1 << 3,
		}

		public static void Run()
		{
			TestSingle();
			TestArray();
			TestList();
			TestDictionary();
			TestPolymorphism();
			TestAbstractClass();

			TestGenericClass();
			TestAutoAttribute();
		}

		public static void TestSingle()
		{
			TestValues<bool>((lhs, rhs) => { return lhs == rhs; }, true, false);

			for (int i = 0; i < sizeof(byte) * 8; ++i)
				TestValues<byte>((lhs, rhs) => { return lhs == rhs; }, (byte)(byte.MaxValue >> i));

			for (int i = 0; i < sizeof(sbyte) * 8; ++i)
				TestValues<sbyte>((lhs, rhs) => { return lhs == rhs; }, (sbyte)(sbyte.MinValue >> i), (sbyte)(sbyte.MaxValue >> i));

			for (int i = 0; i < sizeof(char) * 8; ++i)
				TestValues<char>((lhs, rhs) => { return lhs == rhs; }, (char)(sbyte.MinValue >> i), (char)(sbyte.MaxValue >> i));

			for (int i = 0; i < sizeof(short) * 8; ++i)
				TestValues<short>((lhs, rhs) => { return lhs == rhs; }, (short)(short.MinValue >> i), (short)(short.MaxValue >> i));

			for (int i = 0; i < sizeof(ushort) * 8; ++i)
				TestValues<ushort>((lhs, rhs) => { return lhs == rhs; }, (ushort)(ushort.MaxValue >> i));

			for (int i = 0; i < sizeof(int) * 8; ++i)
				TestValues<int>((lhs, rhs) => { return lhs == rhs; }, (int.MinValue >> i), (int.MaxValue >> i));

			for (int i = 0; i < sizeof(uint) * 8; ++i)
				TestValues<uint>((lhs, rhs) => { return lhs == rhs; }, (uint)(uint.MaxValue >> i));

			for (int i = 0; i < sizeof(long) * 8; ++i)
				TestValues<long>((lhs, rhs) => { return lhs == rhs; }, (long)(long.MinValue >> i), (long)(long.MaxValue >> i));

			for (int i = 0; i < sizeof(ulong) * 8; ++i)
				TestValues<ulong>((lhs, rhs) => { return lhs == rhs; }, (ulong)(ulong.MaxValue >> i));

			TestValues<float>((lhs, rhs) => { return float.Equals(lhs, rhs); }, 0, float.MinValue, float.MaxValue, float.Epsilon, float.NaN, float.NegativeInfinity, float.PositiveInfinity);
			TestValues<double>((lhs, rhs) => { return double.Equals(lhs, rhs); }, 0, double.MinValue, double.MaxValue, double.Epsilon, double.NaN, double.NegativeInfinity, double.PositiveInfinity);
			TestValues<decimal>((lhs, rhs) => { return decimal.Equals(lhs, rhs); }, 0, decimal.MinValue, decimal.MaxValue, decimal.Zero, decimal.One, decimal.MinusOne);

			TestValues<string>((lhs, rhs) => { return string.Equals(lhs, rhs); }, null, string.Empty);

			for (int i = 0; i < sizeof(ushort) * 8; ++i)
				TestValues<string>((lhs, rhs) => { return string.Equals(lhs, rhs); }, new string((char)random.Next(char.MinValue, char.MaxValue), ushort.MaxValue >> i));

			foreach (Color color in Enum.GetValues(typeof(Color)))
				TestValues<Color>((lhs, rhs) => { return lhs == rhs; }, color);

			TestEnumFlags();

			for (int i = 0; i < sizeof(long) * 8; ++i)
				TestValues<DateTime>((lhs, rhs) => { return lhs == rhs; }, new DateTime((long)(DateTime.MaxValue.Ticks >> i)));
		}

		public static void TestArray()
		{
			for (int i = 0; i < sizeof(ushort) * 8; ++i)
			{
				TestArray<bool>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return random.Next() % 2 > 0; }));
				TestArray<byte>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (byte)GetRandomDouble(); }));
				TestArray<sbyte>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (sbyte)GetRandomDouble(); }));
				TestArray<char>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (char)GetRandomDouble(); }));
				TestArray<short>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (short)GetRandomDouble(); }));
				TestArray<ushort>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (ushort)GetRandomDouble(); }));
				TestArray<int>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (int)GetRandomDouble(); }));
				TestArray<uint>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (uint)GetRandomDouble(); }));
				TestArray<long>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (long)GetRandomDouble(); }));
				TestArray<ulong>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (ulong)GetRandomDouble(); }));
				TestArray<float>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return (float)GetRandomDouble(); }));
				TestArray<double>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomDouble(); }));
				TestArray<decimal>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomDecimal(); }));
				TestArray<string>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomString(); }));
				TestArray<Color>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomColor(); }));
				TestArray<DateTime>((lhs, rhs) => { return lhs == rhs; }, GetArrayWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomDateTime(); }));

				TestArray<string>((lhs, rhs) => { return lhs == rhs; }, null);
			}
		}

		public static void TestList()
		{
			for (int i = 0; i < sizeof(ushort) * 8; ++i)
			{
				TestList<bool>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return random.Next() % 2 > 0; }));
				TestList<byte>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (byte)GetRandomDouble(); }));
				TestList<sbyte>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (sbyte)GetRandomDouble(); }));
				TestList<char>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (char)GetRandomDouble(); }));
				TestList<short>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (short)GetRandomDouble(); }));
				TestList<ushort>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (ushort)GetRandomDouble(); }));
				TestList<int>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (int)GetRandomDouble(); }));
				TestList<uint>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (uint)GetRandomDouble(); }));
				TestList<long>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (long)GetRandomDouble(); }));
				TestList<ulong>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (ulong)GetRandomDouble(); }));
				TestList<float>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return (float)GetRandomDouble(); }));
				TestList<double>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomDouble(); }));
				TestList<decimal>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomDecimal(); }));
				TestList<string>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomString(); }));
				TestList<Color>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomColor(); }));
				TestList<DateTime>((lhs, rhs) => { return lhs == rhs; }, GetListWithRandomValues(ushort.MaxValue >> i, () => { return GetRandomDateTime(); }));

				TestList<string>((lhs, rhs) => { return lhs == rhs; }, null);
			}
		}

		public static void TestDictionary()
		{
			TestDictionary<bool, int>(GetDictionaryWithRandomValues<bool, int>(random.Next(10, 100), GetRandomByte, () => { return (int)GetRandomDouble(); }));
			TestDictionary<string, int>(GetDictionaryWithRandomValues<string, int>(random.Next(10, 100), GetRandomString, () => { return (int)GetRandomDouble(); }));
			TestDictionary<decimal, string>(GetDictionaryWithRandomValues<decimal, string>(random.Next(10, 100), GetRandomDecimal, GetRandomString));
		}

		private static void TestGenericClass()
		{
			byte[] data = Serialize(new GenericValue<int>(10));
			var v = Deserialize<GenericValue<int>>(data);

			GenericValue<int>[] array = new GenericValue<int>[ushort.MaxValue];

			for (int i = 0; i < array.Length; ++i)
				array[i] = new GenericValue<int>((int)GetRandomDouble());

			data = Serialize(array);
			var v3 = Deserialize<GenericValue<int>[]>(data);

			for (int i = ushort.MaxValue - 20; i < ushort.MaxValue + 10; ++i)
			{
				var zz = new GenericValue<GenericValue<byte[]>>(new GenericValue<byte[]>(new byte[i]));

				data = Serialize(zz);
				var v4 = Deserialize<GenericValue<GenericValue<byte[]>>>(data);
			}

			Console.WriteLine(v.Value);
		}

		public static void TestAutoAttribute()
		{
			//
			AutoTest1 auto1 = new AutoTest1(25, 54.4f, "00120");

			byte[] bytes1 = Spark.Serialize(auto1);
			AutoTest1 result1 = Spark.Deserialize<AutoTest1>(bytes1);

			if (!auto1.IsEqual(result1))
				throw new System.ArgumentException("AutoAttribute test failed");

			//
			AutoTest2 auto2 = new AutoTest2(25, 54.4f, "00120");

			byte[] bytes2 = Spark.Serialize(auto2);
			AutoTest2 result2 = Spark.Deserialize<AutoTest2>(bytes2);

			if (!auto2.IsEqual(result2))
				throw new System.ArgumentException("AutoAttribute test failed");

			//
			AutoTest3 auto3 = new AutoTest3(25, 54.4f, "00120");

			byte[] bytes3 = Spark.Serialize(auto3);
			AutoTest3 result3 = Spark.Deserialize<AutoTest3>(bytes3);

			if (!auto3.IsEqual(result3))
				throw new System.ArgumentException("AutoAttribute test failed");
		}

		private class GenericValue<T>
		{
			[Member(1)]
			public T Value = default(T);

			public GenericValue()
			{
			}

			public GenericValue(T value)
			{
				Value = value;
			}
		}

		private static void TestValues<T>(System.Func<T, T, bool> comparer, params T[] values)
		{
			foreach (var value in values)
			{
				byte[] data = Serialize(value);
				byte[] dataBeforeDeserialize = new byte[data.Length];
				System.Array.Copy(data, dataBeforeDeserialize, data.Length);

				T newValue = Deserialize<T>(data);

				for (int i = 0; i < data.Length; ++i)
				{
					if (data[i] != dataBeforeDeserialize[i])
						Console.WriteLine("Test " + typeof(T) + " fail: data changed on deserialization");
				}

				//Console.WriteLine(value + " / " + newValue);

				if (!comparer(value, newValue))
					throw new InvalidProgramException("Test " + typeof(T) + " fail: " + newValue + " / " + value + " expected");
			}
		}

		private static void TestEnumFlags()
		{
			EFlag flags = EFlag.Flag1 | EFlag.Flag2 | EFlag.Flag3;

			byte[] data = Spark.Serialize(flags);
			byte[] dataBeforeDeserialize = new byte[data.Length];
			System.Array.Copy(data, dataBeforeDeserialize, data.Length);

			EFlag result = Spark.Deserialize<EFlag>(data);

			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] != dataBeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(EFlag) + " fail: data changed on deserialization");
			}

			if (result != flags)
				throw new InvalidProgramException("Test " + typeof(EFlag) + " fail: " + result + " / " + flags + " expected");
		}

		private static void TestArray<T>(System.Func<T, T, bool> elementComparer, T[] array)
		{
			byte[] data = Serialize(array);
			byte[] dataBeforeDeserialize = new byte[data.Length];
			System.Array.Copy(data, dataBeforeDeserialize, data.Length);

			T[] newArray = Deserialize<T[]>(data);

			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] != dataBeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(T) + " fail: data changed on deserialization");
			}

			if (array == null && newArray == null)
				return;

			Console.WriteLine(array.Length + " / " + newArray.Length + " / " + data.Length);

			for (int i = 0; i < array.Length; ++i)
			{
				if (!elementComparer((T)array.GetValue(i), (T)newArray.GetValue(i)))
					throw new InvalidProgramException("Test " + typeof(T) + " fail: " + newArray.GetValue(i) + " / " + array.GetValue(i) + " expected");
			}
		}

		private static void TestList<T>(System.Func<T, T, bool> elementComparer, List<T> list)
		{
			byte[] data = Serialize(list);
			byte[] dataBeforeDeserialize = new byte[data.Length];
			System.Array.Copy(data, dataBeforeDeserialize, data.Length);

			List<T> newList = Deserialize<List<T>>(data);

			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] != dataBeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(T) + " fail: data changed on deserialization");
			}

			if (list == null && newList == null)
				return;

			Console.WriteLine(list.Count + " / " + newList.Count + " / " + data.Length);

			for (int i = 0; i < list.Count; ++i)
			{
				if (!elementComparer(list[i], newList[i]))
					throw new InvalidProgramException("Test " + typeof(T) + " fail: " + newList[i] + " / " + list[i] + " expected");
			}
		}

		private static void TestDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
			where TKey : IComparable<TKey>
			where TValue : IComparable<TValue>
		{
			byte[] data = Serialize(dictionary);
			byte[] dataBeforeDeserialize = new byte[data.Length];
			System.Array.Copy(data, dataBeforeDeserialize, data.Length);

			Dictionary<TKey, TValue> newDictionary = Deserialize<Dictionary<TKey, TValue>>(data);

			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] != dataBeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(Dictionary<TKey, TValue>) + " fail: data changed on deserialization");
			}

			if (dictionary == null && newDictionary == null)
				return;

			Console.WriteLine(string.Format("{0} / {1} / {2}", dictionary.Count, newDictionary.Count, data.Length));

			if (dictionary.Count != newDictionary.Count)
				throw new InvalidProgramException(string.Format("Test {0} fail: size {1} / {2} expected", typeof(Dictionary<TKey, TValue>), newDictionary.Count, dictionary.Count));

			//
			TKey[] keys = new TKey[dictionary.Count];
			TValue[] values = new TValue[dictionary.Count];

			dictionary.Keys.CopyTo(keys, 0);
			dictionary.Values.CopyTo(values, 0);

			//
			TKey[] newKeys = new TKey[newDictionary.Count];
			TValue[] newValues = new TValue[newDictionary.Count];

			newDictionary.Keys.CopyTo(newKeys, 0);
			newDictionary.Values.CopyTo(newValues, 0);

			//
			for (int i = 0; i < dictionary.Count; ++i)
			{
				if (keys[i].CompareTo(newKeys[i]) != 0)
					throw new InvalidProgramException(string.Format("Test {0} fail: key {1} / {2} expected", typeof(Dictionary<TKey, TValue>), newKeys[i], keys[i]));

				if (values[i].CompareTo(newValues[i])!= 0)
					throw new InvalidProgramException(string.Format("Test {0} fail: value {1} / {2} expected", typeof(Dictionary<TKey, TValue>), newValues[i], values[i]));
			}
		}

		private static T[] GetArrayWithRandomValues<T>(int size, Func<T> getRandomValue)
		{
			T[] array = new T[size];

			for (int i = 0; i < array.Length; ++i)
				array[i] = getRandomValue();

			return array;
		}

		private static List<T> GetListWithRandomValues<T>(int size, Func<T> getRandomValue)
		{
			List<T> list = new List<T>(size);

			for (int i = 0; i < size; ++i)
				list.Add(getRandomValue());

			return list;
		}

		private static Dictionary<TKey, TValue> GetDictionaryWithRandomValues<TKey, TValue>(int size, Func<TKey> getRandomKey, Func<TValue> getRandomValue)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

			for (int i = 0; i < size; ++i)
				dictionary[getRandomKey()] = getRandomValue();

			return dictionary;
		}

		private static bool GetRandomByte()
		{
			return random.Next() % 2 > 0;
		}

		private static double GetRandomDouble()
		{
			return (double)GetRandomDecimal();
		}

		private static decimal GetRandomDecimal()
		{
			int lo = random.Next(int.MinValue, int.MaxValue);
			int mid = random.Next(int.MinValue, int.MaxValue);
			int hi = random.Next(int.MinValue, int.MaxValue);
			bool isNegative = random.Next(int.MinValue, int.MaxValue) % 2 != 0;
			byte scale = (byte)random.Next(0, 29);

			return new decimal(lo, mid, hi, isNegative, scale);
		}

		private static string GetRandomString()
		{
			int size = random.Next(300);

			return new string((char)random.Next(char.MinValue, char.MaxValue), size);
		}

		private static Color GetRandomColor()
		{
			int index = random.Next(0, colorValues.Length);

			return colorValues[index];
		}

		private static DateTime GetRandomDateTime()
		{
			long ticks = (long)GetRandomDouble();

			if (ticks < DateTime.MinValue.Ticks)
				ticks = DateTime.MinValue.Ticks + random.Next(0, int.MaxValue);

			if (ticks > DateTime.MaxValue.Ticks)
				ticks = DateTime.MinValue.Ticks + random.Next(0, int.MaxValue);

			return new DateTime(ticks);
		}

		[Spark.As(10, typeof(OneThing))]
		[Spark.As(11, typeof(OtherThing))]
		private interface ISomething
		{
			string secondThing { get; set; }
			bool IsEqual(ISomething s);
		}

		[Spark.As(10, typeof(OneThing2))]
		[Spark.As(13, typeof(OtherThing2))]
		private interface ISomething2
		{
			string secondThing { get; set; }
		}

		private class OneThing : ISomething
		{
			[Spark.Member(1)]
			int firstThing;

			[Spark.Member(2)]
			public string secondThing { get; set; }

			public bool IsEqual(ISomething s)
			{
				if (s == null)
					return false;

				if (s.GetType() != this.GetType())
					return false;

				OneThing other = s as OneThing;

				return (other.firstThing == firstThing) && (other.secondThing == secondThing);
			}
		}

		private class OneThing2 : ISomething2
		{
			[Spark.Member(1)]
			int firstThing;

			[Spark.Member(2)]
			public string secondThing { get; set; }

			public bool IsEqual(OneThing2 other)
			{
				return (other.firstThing == firstThing) && (other.secondThing == secondThing);
			}
		}

		private class OtherThing : ISomething
		{
			[Spark.Member(1)]
			float firstThing;

			[Spark.Member(2)]
			public List<sbyte> someData = new List<sbyte>() { 1, 127, -1 };

			[Spark.Member(3)]
			public string secondThing { get; set; }

			public bool IsEqual(ISomething s)
			{
				if (s == null)
					return false;

				if (s.GetType() != this.GetType())
					return false;

				OtherThing other = s as OtherThing;

				if (other.firstThing != firstThing)
					return false;

				if (other.secondThing != secondThing)
					return false;

				if (other.someData.Count != someData.Count)
					return false;

				for (int i = 0; i < other.someData.Count; ++i)
				{
					if (other.someData[i] != someData[i])
						return false;
				}

				return true;
			}
		}

		private class OtherThing2 : ISomething2
		{
			[Spark.Member(1)]
			float firstThing;

			[Spark.Member(2)]
			public List<sbyte> someData = new List<sbyte>() { 1, 127, -1 };

			[Spark.Member(3)]
			public string secondThing { get; set; }

			public bool IsEquial(OtherThing2 other)
			{
				if (other.firstThing != firstThing)
					return false;

				if (other.secondThing != secondThing)
					return false;

				if (other.someData.Count != someData.Count)
					return false;

				for (int i = 0; i < other.someData.Count; ++i)
				{
					if (other.someData[i] != someData[i])
						return false;
				}

				return true;
			}
		}

		private static void TestPolymorphism()
		{
			ISomething s1 = new OneThing();
			s1.secondThing = "1:one";

			byte[] b1 = Spark.Serialize(s1);
			byte[] b1BeforeDeserialize = new byte[b1.Length];
			System.Array.Copy(b1, b1BeforeDeserialize, b1.Length);
			ISomething outS1 = Spark.Deserialize<ISomething>(b1);

			for (int i = 0; i < b1.Length; ++i)
			{
				if (b1[i] != b1BeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(ISomething) + " fail: data changed on deserialization");
			}

			if (outS1.GetType() != typeof(OneThing))
				throw new System.ArgumentException("Polymorphism not working!");

			if ((s1).IsEqual(outS1) == false)
				throw new System.ArgumentException("Polymorphism not working!");

			OneThing outS11 = Spark.Deserialize<OneThing>(b1);

			if ((s1).IsEqual(outS11) == false)
				throw new System.ArgumentException("Polymorphism not working!");

			//
			List<ISomething> list = new List<ISomething>();

			list.Add(new OneThing());
			list.Add(new OtherThing());
			list.Add(null);
			list.Add(new OneThing());

			byte[] b2 = Spark.Serialize(list);
			byte[] b2BeforeDeserialize = new byte[b2.Length];
			System.Array.Copy(b2, b2BeforeDeserialize, b2.Length);

			List<ISomething> outList = Spark.Deserialize<List<ISomething>>(b2);

			for (int i = 0; i < b2.Length; ++i)
			{
				if (b2[i] != b2BeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(List<ISomething>) + " fail: data changed on deserialization");
			}

			if (outList.Count != 4)
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList[0].GetType() != typeof(OneThing))
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList[1].GetType() != typeof(OtherThing))
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList[2] != null)
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList[3].GetType() != typeof(OneThing))
				throw new System.ArgumentException("Polymorphism not working!");

			// Simulation: class id changed
			List<ISomething2> outList2 = Spark.Deserialize<List<ISomething2>>(b2);

			for (int i = 0; i < b2.Length; ++i)
			{
				if (b2[i] != b2BeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(List<ISomething2>) + " fail: data changed on deserialization");
			}

			if (outList2.Count != 4)
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList2[0].GetType() != typeof(OneThing2))
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList2[1] != null)
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList2[2] != null)
				throw new System.ArgumentException("Polymorphism not working!");

			if (outList2[3].GetType() != typeof(OneThing2))
				throw new System.ArgumentException("Polymorphism not working!");
		}

		private static void TestAbstractClass()
		{
			OneThing oneThing = new OneThing();
			oneThing.secondThing = "2:second";

			OtherThing otherThing = new OtherThing();
			otherThing.secondThing = "6:six";

			AbstractIsBase ab = new AbstractIsBase();
			AnAbstractClass a = ab;

			a.int1 = 17;
			a.SetSomething(oneThing);

			ab.str = "str";
			ab.SetMySomething(otherThing);

			byte[] data = Spark.Serialize(a);
			byte[] dataBeforeDeserialize = new byte[data.Length];
			System.Array.Copy(data, dataBeforeDeserialize, data.Length);

			AnAbstractClass outA = Spark.Deserialize<AnAbstractClass>(data);

			for (int i = 0; i < data.Length; ++i)
			{
				if (data[i] != dataBeforeDeserialize[i])
					Console.WriteLine("Test " + typeof(AnAbstractClass) + " fail: data changed on deserialization");
			}

			if (outA.GetType() != typeof(AbstractIsBase))
				throw new System.ArgumentException("Polymorphism not working!");

			if (!outA.IsEqual(a))
				throw new System.ArgumentException("Polymorphism not working!");
		}

		[Spark.As(64, typeof(AbstractIsBase))]
		private abstract class AnAbstractClass
		{
			[Spark.Member(1)]
			public int int1 = 0;

			[Spark.Member(2)]
			protected float float1 = 0.5f;

			[Spark.Member(3)]
			private ISomething something = null;

			public void SetSomething(ISomething s)
			{
				something = s;
			}

			public virtual bool IsEqual(AnAbstractClass a)
			{
				if (a.int1 != int1)
					return false;

				if (a.float1 != float1)
					return false;

				if (something == null && a.something != null)
					return false;

				if (something.GetType() != a.something.GetType())
					return false;

				return something.IsEqual(a.something);
			}
		}

		private class AbstractIsBase : AnAbstractClass
		{
			[Spark.Member(1)]
			public string str = string.Empty;

			[Spark.Member(2)]
			protected float float2 = 0.5f;

			[Spark.Member(3)]
			private ISomething something = null;

			public void SetMySomething(ISomething s)
			{
				something = s;
			}

			public override bool IsEqual(AnAbstractClass a)
			{
				if (a == null)
					return false;

				if (a.GetType() != this.GetType())
					return false;

				if (!base.IsEqual(a))
					return false;

				AbstractIsBase thisClass = a as AbstractIsBase;

				if (str != thisClass.str)
					return false;

				if (float2 != thisClass.float2)
					return false;

				if (something == thisClass.something)
					return true;

				return something.IsEqual(thisClass.something);
			}
		}

		[Spark.Auto(AutoMode.All)]
		private class AutoTest1
		{
			public AutoTest1()
			{
			}

			public AutoTest1(int i, float f, string s)
			{
				i1 = i;
				f1 = f;
				s1 = s;
			}

			public int i1;
			private float f1;

			private string s1
			{
				get;
				set;
			}

			protected int i2
			{
				get { throw new System.FieldAccessException(); }
			}

			public float f2
			{
				set { throw new System.FieldAccessException(); }
			}

			public bool IsEqual(AutoTest1 other)
			{
				return (i1 == other.i1) && (f1 == other.f1) && (string.Compare(s1, other.s1) == 0);
			}
		}

		[Spark.Auto(AutoMode.Fields)]
		private class AutoTest2
		{
			public AutoTest2()
			{
			}

			public AutoTest2(int i, float f, string s)
			{
				i1 = i;
				f1 = f;
			}

			public int i1;
			private float f1;

			private string s1
			{
				get { throw new System.FieldAccessException(); }
				set { throw new System.FieldAccessException(); }
			}

			protected int i2
			{
				get { throw new System.FieldAccessException(); }
			}

			public float f2
			{
				set { throw new System.FieldAccessException(); }
			}

			public bool IsEqual(AutoTest2 other)
			{
				return (i1 == other.i1) && (f1 == other.f1);
			}
		}

		[Spark.Auto(AutoMode.Properties)]
		private class AutoTest3
		{
			public AutoTest3()
			{
			}

			public AutoTest3(int i, float f, string s)
			{
				i1 = i;
				f1 = f;
				s1 = s;
			}

			public int i1;
			private float f1;

			private string s1
			{
				get;
				set;
			}

			protected int i2
			{
				get { throw new System.FieldAccessException(); }
			}

			public float f2
			{
				set { throw new System.FieldAccessException(); }
			}

			public bool IsEqual(AutoTest3 other)
			{
				return string.Compare(s1, other.s1) == 0;
			}
		}
	}
}
#pragma warning restore 0649,168