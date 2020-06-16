using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// A generic implementation of a genetic algorithm.
	/// </summary>
	/// <typeparam name="TItem">The type of the item to optimize.</typeparam>
	/// <typeparam name="TItemFactory">Factory that generates items of type T.</typeparam>
	public class GeneticAlgorithm<TItem, TItemFactory> where TItem : IGeneticItem, ICloneable where TItemFactory : IGeneticItemFactory<TItem>
	{
		/// <summary>
		/// Genetic pool of items T with scores
		/// </summary>
		private GeneticAlgorithmPoolMember<TItem>[] Pool { get; set; }
		private GeneticAlgorithmPoolMember<TItem>[] NewPool;

		private Random Generator { get; } = new Random();

		private TItemFactory PoolItemFactory { get; }

		/// <summary>
		/// Gets the size of the pool.
		/// </summary>
		public int PoolSize => Pool.Length;

		/// <summary>
		/// Gets an enumerator for the genetic pool.
		/// </summary>
		/// <returns>The pool enumerator.</returns>
		public IEnumerator<GeneticAlgorithmPoolMember<TItem>> GetEnumerator() => ((IEnumerable<GeneticAlgorithmPoolMember<TItem>>)Pool).GetEnumerator();

		/// <summary>
		/// Gets a pool item by index (ordered by score)
		/// </summary>
		/// <param name="Index">The index to retreive.</param>
		/// <returns>The pool item at the specified index.</returns>
		public GeneticAlgorithmPoolMember<TItem> GetPoolItem(int Index) => Pool[Index];

		/// <summary>
		/// Generic GeneticAlgorithm constructor
		/// </summary>
		/// <param name="PoolItemFactory">Factory implementation that generates items of type T.</param>
		public GeneticAlgorithm(TItemFactory PoolItemFactory)
		{
			this.PoolItemFactory = PoolItemFactory;
		}

		/// <summary>
		/// Initializes the genetic pool with random data.
		/// </summary>
		/// <param name="Size">The desired size of the new pool.</param>
		public void GeneratePool(int Size)
		{
			Pool = new GeneticAlgorithmPoolMember<TItem>[Size];
			NewPool = new GeneticAlgorithmPoolMember<TItem>[Size];

			for (int i = 0; i < Pool.Length; i ++)
			{
				Pool[i] = new GeneticAlgorithmPoolMember<TItem>(PoolItemFactory.GetRandom(), 0);
			}
		}

		/// <summary>
		/// Scores the pool and sorts it.
		/// </summary>
		public void ScoreAndSortPool()
		{
			Parallel.For(0, Pool.Length,
				(i) =>
				{
					Pool[i] = new GeneticAlgorithmPoolMember<TItem>(Pool[i].Item, Pool[i].Item.GetScore());
				});

			SortPool();
		}

		/// <summary>
		/// Sorts the pool
		/// </summary>
		public void SortPool()
		{
			Array.Sort(Pool);
		}

		/// <summary>
		/// Randomly shuffles the pool.
		/// </summary>
		public void ShufflePool()
		{
			for (int i = 0; i < Pool.Length * 10; i++)
			{
				int IndexA = Generator.Next(Pool.Length);
				int IndexB = Generator.Next(Pool.Length);

				GeneticAlgorithmPoolMember<TItem> Temp = Pool[IndexA];
				Pool[IndexA] = Pool[IndexB];
				Pool[IndexB] = Temp;
			}
		}

		/// <summary>
		/// Chooses a random integer index using a negative linear falloff function of P(x) = -1*x + 1.
		/// </summary>
		/// <param name="MaxIndexInclusive">Maximum number than can be returned.</param>
		/// <returns>Random weighted number.</returns>
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

		/// <summary>
		/// <see cref="SelectIndexWeighted(int)"/>
		/// </summary>
		/// <param name="MinimumIndexInclusive">Minimum number that can be returned</param>
		/// <param name="MaxIndexInclusive"><see cref="SelectIndexWeighted(int)"/></param>
		/// <returns><see cref="SelectIndexWeighted(int)"/></returns>
		public int SelectIndexWeighted(int MinimumIndexInclusive, int MaxIndexInclusive)
		{
			return SelectIndexWeighted(MaxIndexInclusive - MinimumIndexInclusive) + MinimumIndexInclusive;
		}

		/// <summary>
		/// Performs the standard genetic evolution operations of the pool.
		/// </summary>
		/// <param name="ElitismCutoff">Number of high scoring members to copy unmodified.</param>
		/// <param name="CrossoverChance">Total pool ratio that undergoes crossover operations.</param>
		/// <param name="MutationChance">Total pool ratio that undergoes mutation operations.</param>
		public void Evolve(int ElitismCutoff, double CrossoverChance, double MutationChance)
		{
			//TODO: Add multiple parent option, crossover fitness heuristic and pool reset heuristic

			int NewPoolIndex = 0;

			//Direct copy top members
			for(int i = 0; i < ElitismCutoff && NewPoolIndex < Pool.Length; i ++)
			{
				NewPool[NewPoolIndex ++] = new GeneticAlgorithmPoolMember<TItem>(Pool[i].Item, 0);
			}

			int NumCrossovers = (int)((Pool.Length - ElitismCutoff) * CrossoverChance) / 2;

			//Perform crossovers
			for (int i = 0; i < NumCrossovers && NewPoolIndex < Pool.Length; i++)
			{
				TItem ParentA = Pool[SelectIndexWeighted(Pool.Length - 1)].Item;
				TItem ParentB = Pool[SelectIndexWeighted(Pool.Length - 1)].Item;

				if (Generator.NextDouble() <= MutationChance)
				{
					ParentA = PoolItemFactory.Mutate(ParentA);
					ParentB = PoolItemFactory.Mutate(ParentB);
				}

				TItem NewItemA, NewItemB;
				PoolItemFactory.Cross(ParentA, ParentB, out NewItemA, out NewItemB);

				NewPool[NewPoolIndex++] = new GeneticAlgorithmPoolMember<TItem>(NewItemA, 0);
				NewPool[NewPoolIndex++] = new GeneticAlgorithmPoolMember<TItem>(NewItemB, 0);
			}

			//Perform copies
			while (NewPoolIndex < Pool.Length)
			{
				TItem Item;

				if (Generator.NextDouble() <= MutationChance)
				{
					Item = Pool[SelectIndexWeighted(Pool.Length - 1)].Item;
					Item = PoolItemFactory.Mutate(Item);
				}
				else
				{
					Item = Pool[SelectIndexWeighted(ElitismCutoff, Pool.Length - 1)].Item;
				}
				
				NewPool[NewPoolIndex++] = new GeneticAlgorithmPoolMember<TItem>(Item, 0);
			}

			var OldPool = Pool;
			Pool = NewPool;
			NewPool = OldPool;
		}
	}
}
