using System;
using System.Collections.Generic;

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

		public static void Run()
		{
			//TestSingle();
			//TestArray();
			//TestList();
			TestDictionary();

			TestGenericClass();
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
			TestDictionary<bool, int>(GetDictionaryWithRandomValues(random.Next(10, 100), GetRandomByte, () => { return (int)GetRandomDouble(); }));
			TestDictionary<string, int>(GetDictionaryWithRandomValues(random.Next(10, 100), GetRandomString, () => { return (int)GetRandomDouble(); }));
			TestDictionary<decimal, string>(GetDictionaryWithRandomValues(random.Next(10, 100), GetRandomDecimal, GetRandomString));
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
				T newValue = Deserialize<T>(data);

				//Console.WriteLine(value + " / " + newValue);

				if (!comparer(value, newValue))
					throw new InvalidProgramException("Test " + typeof(T) + " fail: " + newValue + " / " + value + " expected");
			}
		}

		private static void TestArray<T>(System.Func<T, T, bool> elementComparer, T[] array)
		{
			byte[] data = Serialize(array);
			T[] newArray = Deserialize<T[]>(data);

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
			List<T> newList = Deserialize<List<T>>(data);

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
			Dictionary<TKey, TValue> newDictionary = Deserialize<Dictionary<TKey, TValue>>(data);

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
	}
}