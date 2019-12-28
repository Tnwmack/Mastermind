using System;
using System.Threading.Tasks;

namespace Mastermind
{
	public class GeneticAlgorithm<T, F> where T : IGeneticItem, ICloneable where F : IGeneticItemFactory<T>
	{
		public struct PoolMember : IComparable<PoolMember>
		{
			public readonly T Item;
			public readonly int Score;

			public PoolMember(T Item, int Score)
			{
				this.Item = (T)Item.Clone();
				this.Score = Score;
			}

			public int CompareTo(PoolMember other)
			{
				return other.Score - Score;
			}
		}

		public PoolMember[] Pool;
		private PoolMember[] NewPool;

		protected readonly Random Generator = new Random();

		protected readonly F PoolItemFactory;

		public GeneticAlgorithm(F PoolItemFactory)
		{
			this.PoolItemFactory = PoolItemFactory;
		}

		public void GeneratePool(int Size)
		{
			Pool = new PoolMember[Size];
			NewPool = new PoolMember[Size];

			for (int i = 0; i < Pool.Length; i ++)
			{
				Pool[i] = new PoolMember(PoolItemFactory.GetRandom(), 0);
			}
		}

		public void ScoreAndSortPool()
		{
			Parallel.For(0, Pool.Length,
				(i) =>
				{
					Pool[i] = new PoolMember(Pool[i].Item, Pool[i].Item.GetScore());
				});

			SortPool();
		}

		public void SortPool()
		{
			Array.Sort(Pool);
		}

		public int SelectIndexWeighted(int MaxIndexInclusive)
		{
			//Linear falloff probability where:
			//P(x) = -1*x + 1
			//CDF(X) = -(1/2)x^2 + x = y
			//X = 1 - (1 - 2*y)^(1/2)

			float y = (float)Generator.NextDouble() * 0.5f;
			float x = 1.0f - (float)Math.Sqrt(1.0f - 2.0f * y);
			x *= MaxIndexInclusive;
			int Result = (int)Math.Floor(x);

			return Result;
		}

		public int SelectIndexWeighted(int MinimumIndexInclusive, int MaxIndexInclusive)
		{
			return SelectIndexWeighted(MaxIndexInclusive - MinimumIndexInclusive) + MinimumIndexInclusive;
		}

		public void Evolve(int ElitismCutoff, double CrossoverChance, double MutationChance)
		{
			//TODO: Add multiple parent option and crossover fitness heuristic

			int NewPoolIndex = 0;

			//Direct copy top members
			for(int i = 0; i < ElitismCutoff && NewPoolIndex < Pool.Length; i ++)
			{
				NewPool[NewPoolIndex ++] = new PoolMember(Pool[i].Item, 0);
			}

			int NumCrossovers = (int)((Pool.Length - ElitismCutoff) * CrossoverChance) / 2;

			//Perform crossovers
			for (int i = 0; i < NumCrossovers && NewPoolIndex < Pool.Length; i++)
			{
				T ParentA = Pool[SelectIndexWeighted(Pool.Length - 1)].Item;
				T ParentB = Pool[SelectIndexWeighted(Pool.Length - 1)].Item;

				if (Generator.NextDouble() <= MutationChance)
				{
					ParentA = PoolItemFactory.Mutate(ParentA);
					ParentB = PoolItemFactory.Mutate(ParentB);
				}

				T NewItemA, NewItemB;
				PoolItemFactory.Cross(ParentA, ParentB, out NewItemA, out NewItemB);

				NewPool[NewPoolIndex++] = new PoolMember(NewItemA, 0);
				NewPool[NewPoolIndex++] = new PoolMember(NewItemB, 0);
			}

			//Perform copies
			while (NewPoolIndex < Pool.Length)
			{
				T Item;

				if (Generator.NextDouble() <= MutationChance)
				{
					Item = Pool[SelectIndexWeighted(Pool.Length - 1)].Item;
					Item = PoolItemFactory.Mutate(Item);
				}
				else
				{
					Item = Pool[SelectIndexWeighted(ElitismCutoff, Pool.Length - 1)].Item;
				}
				
				NewPool[NewPoolIndex++] = new PoolMember(Item, 0);
			}

			var OldPool = Pool;
			Pool = NewPool;
			NewPool = OldPool;
		}
	}
}
