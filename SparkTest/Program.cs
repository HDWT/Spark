using System;
using System.Diagnostics;

namespace SparkTest
{
	enum Color
	{
		Red = 1,
		Green = 2,
		Blue = 0,
		Yellow = 14,
		Black = -5,
		White = 100,
	}

	class Program
	{
		delegate int TestDelegate(object obj);

		int i0 = 1;
		int i1 = 3;

		int Test(ref int ai0)
		{
			return ai0;
		}

		static void Do()
		{
			Program prog = new Program();

			int aaa = 19;

			prog.Test(ref aaa);
		}

		private static string GetIntList()
		{
			throw new NotImplementedException();
		}

		static void Main(string[] args)
		{
			//string myString = GetIntList();
			//Console.WriteLine(myString);

			//Console.ReadKey();
			//return;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			Spark.TestLowLevel.Run();

			sw.Stop();
			Console.WriteLine("MS: " + sw.ElapsedMilliseconds);

			//Spark.TestLowLevel.TestPolymorphism();
			//Spark.TestLowLevel.TestAbstractClass();

			//Spark.TestBasicValues.Run(10000);
			//return;

			//Color color = Color.Black;

			Spark.Serialize(new A());

			Spark.Serialize(new SizeTest());
			Spark.Serialize(new SizeTest());

			//Color[] colors = { Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.Black, Color.White };

			//var data = Spark.Serialize(colors);
			//Color color2 = Color.Green;

			//var colors2 = Spark.Deserialize<Color[]>(data);
			//Console.WriteLine(color2);

			//foreach (var c in colors2)
			//	Console.WriteLine(c);

			//List<int> list = new List<int> { 555, 1000, 99663311 };
			//var listData = Spark.Serialize(list);

			//List<int> list2 = Spark.Deserialize<List<int>>(listData);

			//foreach (var item in list2)
			//	Console.WriteLine(item);

			Console.WriteLine("Test done");
			Console.ReadKey();
			return;
		}
	}

	class A
	{
		[Spark.Member(3)]
		SizeTest sizeTest = new SizeTest();

		[Spark.Member(1)]
		public bool Bool;
		[Spark.Member(2)]
		public byte Byte;
	}

	class B : A
	{
		[Spark.Member(1)]
		public int a3 = 1;

		[Spark.Member(2)]
		public int a4 = 1;
	}

	class SizeTest
	{
		[Spark.Member(1)]
		private int i0 = 0;

		[Spark.Member(2)]
		public float f1 = 1.1f;


		public object GetInt(SizeTest sizeTest)
		{
			int i = sizeTest.i0;

			return i;
		}
	}

	class Test1
	{
		[Spark.Member(1)]
		private int int1 = 3;

		[Spark.Member(2)]
		private int int2 = 4;

		[Spark.Member(3)]
		private char char1 = 'b';

		[Spark.Member(4)]
		public Test2 test2 = null;

		[Spark.Member(5)]
		private char char2 = 'g';
	}

	class Test2
	{
		[Spark.Member(1)]
		private char char1 = 'a';

		[Spark.Member(2)]
		private int int1 = 6;

		[Spark.Member(3)]
		public Test1 test1 = new Test1();

		[Spark.Member(4)]
		private int int2 = 6;

		[Spark.Member(26)]
		private int int3 = 4;

		[Spark.Member(21)]
		private string s = "11111111";
	}
}
