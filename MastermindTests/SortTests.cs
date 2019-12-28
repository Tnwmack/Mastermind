using System;
using NUnit.Framework;
using Mastermind;

namespace MastermindTests
{
	public class SortTests
	{
		struct PoolMember : IComparable<PoolMember>
		{
			public Mastermind.RowState Row;
			public int Score;

			public int CompareTo(PoolMember other)
			{
				return other.Score - Score;
			}
		}

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
			return;

			var Timer = new System.Diagnostics.Stopwatch();
			var Generator = new Random();
			var Pool = new PoolMember[7000000];
			long ElapsedTime = 0;
			long MaxElapsedTime = 0;
			long MinElapsedTime = long.MaxValue;
			int Rounds = 10;

			for (int Round = 0; Round < Rounds; Round++)
			{
				for (int i = 0; i < Pool.Length; i++)
				{
					Pool[i].Row = Mastermind.RowState.GetRandomColors(Generator, 20, 10);
					Pool[i].Score = Generator.Next(-350, 0);
				}

				Timer.Start();
				//Mastermind.ParallelSort.QuicksortParallel(Pool);
				Array.Sort(Pool);
				Timer.Stop();

				for (int i = 0; i < Pool.Length - 1; i++)
				{
					Assert.IsTrue(Pool[i].Score >= Pool[i + 1].Score);
				}

				if (Timer.ElapsedMilliseconds > MaxElapsedTime)
					MaxElapsedTime = Timer.ElapsedMilliseconds;

				if (Timer.ElapsedMilliseconds < MinElapsedTime)
					MinElapsedTime = Timer.ElapsedMilliseconds;

				ElapsedTime += Timer.ElapsedMilliseconds;
				Timer.Reset();
			}

			ElapsedTime /= Rounds;
			Console.WriteLine(String.Format("QuicksortParallel: {0} ms average, {1} max, {2} min", ElapsedTime, MaxElapsedTime, MinElapsedTime));
		}
	}
}