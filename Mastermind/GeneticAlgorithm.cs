using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	class GeneticAlgorithm<T, F> where T : IGeneticItem where F : IGeneticItemFactory<T>
	{
		public struct PoolMember : IComparable<PoolMember>
		{
			public readonly T Item;
			public readonly int Score;

			public PoolMember(T Item, int Score)
			{
				this.Item = Item;
				this.Score = Score;
			}

			public int CompareTo(PoolMember other)
			{
				return other.Score - Score;
			}
		}

		public PoolMember[] Pool;
		private PoolMember[] NewPool;

		private readonly Random Generator = new Random();

		protected readonly F PoolItemFactory;

		public GeneticAlgorithm(F PoolItemFactory)
		{
			this.PoolItemFactory = PoolItemFactory;
		}

		public void GeneratePool(int Size)
		{
			Pool = new PoolMember[Size];
			NewPool = new PoolMember[Size];

			for(int i = 0; i < Pool.Length; i ++)
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

		public int SelectParentWeighted(int MaxIndexInclusive)
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

		public int SelectParentRandom(int MaxIndexInclusive)
		{
			//Linear falloff probability where:
			//P(x) = -1*x + 1
			//CDF(X) = -(1/2)x^2 + x = y
			//X = 1 - (1 - 2*y)^(1/2)

			return (int)Math.Floor(Generator.NextDouble() * MaxIndexInclusive);
		}

		public void Evolve(int ElitismCutoff, double CrossoverChance, double MutationChance)
		{
			int NewPoolIndex = 0;

			//Direct copy top members
			for(int i = 0; i < ElitismCutoff; i ++)
			{
				NewPool[NewPoolIndex ++] = new PoolMember(Pool[i].Item, 0);
			}

			//Perform crossovers and mutations
			for(int i = 0; i < Pool.Length && NewPoolIndex < Pool.Length; i ++)
			{
				if(Generator.NextDouble() < CrossoverChance && 
					i + 1 < Pool.Length &&
					NewPoolIndex + 1 < Pool.Length)
				{
					T ParentA = Pool[i].Item;
					T ParentB = Pool[i + 1].Item;

					if (Generator.NextDouble() < MutationChance)
					{
						ParentA = PoolItemFactory.Mutate(Pool[i].Item);
						ParentB = PoolItemFactory.Mutate(Pool[i + 1].Item);
					}

					T NewItemA, NewItemB;
					PoolItemFactory.Cross(ParentA, ParentB, out NewItemA, out NewItemB);

					NewPool[NewPoolIndex++] = new PoolMember(NewItemA, 0);
					NewPool[NewPoolIndex++] = new PoolMember(NewItemB, 0);
				}
				else
				{
					T Item = Pool[i].Item;

					if (Generator.NextDouble() < MutationChance)
					{
						Item = PoolItemFactory.Mutate(Item);
					}
					
					NewPool[NewPoolIndex++] = new PoolMember(Item, 0);
				}
			}

			var OldPool = Pool;
			Pool = NewPool;
			NewPool = OldPool;
		}
	}
}
