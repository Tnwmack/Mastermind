using System;
using NUnit.Framework;
using Mastermind;

namespace MastermindTests
{
	public class GeneticSolverTests
	{
		class TestItem : IGeneticItem, ICloneable
		{
			public readonly int Score;

			public TestItem(int Score)
			{
				this.Score = Score;
			}

			public object Clone()
			{
				return new TestItem(Score);
			}

			public int GetScore()
			{
				return Score;
			}
		}

		class TestItemFactory : IGeneticItemFactory<TestItem>
		{
			public int Score = 0;
			public int Crosses = 0;
			public int Mutations = 0;

			public void Cross(TestItem A, TestItem B, out TestItem ResultA, out TestItem ResultB)
			{
				ResultA = new TestItem(0);
				ResultB = new TestItem(0);
				Crosses++;
			}

			public TestItem GetRandom()
			{
				return new TestItem(Score++);
			}

			public TestItem Mutate(TestItem Item)
			{
				Mutations++;
				return new TestItem(0);
			}
		}

		private class GeneticAlgorithmTest : GeneticAlgorithm<TestItem, TestItemFactory>
		{
			protected new Random Generator = new Random(10);

			public GeneticAlgorithmTest(TestItemFactory PoolItemFactory) : base(PoolItemFactory)
			{
			}
		}

		void Shuffle<T>(T[] Arr)
		{
			Random Gen = new Random();

			for (int i = 0; i < Arr.Length * 10; i++)
			{
				int IndexA = Gen.Next(Arr.Length);
				int IndexB = Gen.Next(Arr.Length);

				T Temp = Arr[IndexA];
				Arr[IndexA] = Arr[IndexB];
				Arr[IndexB] = Temp;
			}
		}

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void TestGeneticAlgorithm()
		{
			int PoolSize = 100;

			//Test pool generation and sorting
			TestItemFactory Factory = new TestItemFactory();
			GeneticAlgorithmTest Test = new GeneticAlgorithmTest(Factory);

			Test.GeneratePool(PoolSize);
			Shuffle(Test.Pool);
			Test.ScoreAndSortPool();

			for (int i = 0; i < PoolSize; i++)
			{
				Assert.IsTrue(Test.Pool[i].Score == PoolSize - 1 - i);
			}

			//Test index selection
			//Check Indices distribution in debugger
			int[] Indices = new int[100];

			for (int i = 0; i < 20000000; i++)
			{
				Indices[Test.SelectIndexWeighted(Indices.Length - 1)]++;
			}

			Console.WriteLine("Indices[0]: {0}", Indices[0]);
			Console.WriteLine("Indices[25]: {0}", Indices[25]);
			Console.WriteLine("Indices[50]: {0}", Indices[50]);
			Console.WriteLine("Indices[75]: {0}", Indices[75]);
			Console.WriteLine("Indices[99]: {0}", Indices[99]);

			//Test evolutions

			PoolSize = 500;
			int Elites = 15;
			double Crosses = 0.5;
			double Mutations = 0.1;

			Factory.Score = 0;
			Test.GeneratePool(PoolSize);
			Shuffle(Test.Pool);
			Test.ScoreAndSortPool();

			for (int i = 0; i < Elites; i++)
			{
				Assert.IsTrue(Test.Pool[i].Score == PoolSize - 1 - i);
			}

			Test.Evolve(Elites, Crosses, Mutations);
			Test.ScoreAndSortPool();

			for (int i = 0; i < Elites; i++)
			{
				Assert.IsTrue(Test.Pool[i].Score == PoolSize - 1 - i);
			}

			for (int i = 0; i < PoolSize - 1; i++)
			{
				Assert.IsTrue(Test.Pool[i].Score >= Test.Pool[i + 1].Score);
			}

			Console.WriteLine("Crosses {3:P0} n={0}: {1} ({2} expected)", PoolSize, Factory.Crosses, (int)((PoolSize - Elites) * Crosses / 2.0), Crosses);
			Console.WriteLine("Mutations {3:P0} n={0}: {1} ({2} expected)", PoolSize, Factory.Mutations, (int)((PoolSize - Elites) * Mutations), Mutations);
		}
	}
}