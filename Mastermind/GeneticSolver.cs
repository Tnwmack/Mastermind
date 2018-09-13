using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	public class GeneticSolver : Solver
	{
		private Random Generator = new Random();

		private string Status = "";

		/// <summary>
		/// Holds settings for the solver
		/// </summary>
		public class GeneticSolverSettings : ICloneable
		{
			/// <summary>
			/// The size of the gene pool
			/// </summary>
			public int PoolSize = 500;

			/// <summary>
			/// The number of crossovers to perform (pool*CrossoverAmount)
			/// </summary>
			public float CrossoverAmount = 0.7f;

			/// <summary>
			/// The rate that colors are mutated during a crossover
			/// </summary>
			public float MutationRate = 0.25f;

			/// <summary>
			/// The number top scoring pool members that do not 
			/// undergo crossovers or mutation
			/// </summary>
			public int ElitismCutoff = 20;

			/// <summary>
			/// The penalty score for correct color and spot discrepancies
			/// </summary>
			public int MatchScore = 50;

			/// <summary>
			/// The penalty score for correct color discrepancies
			/// </summary>
			public int PartialMatchScore = 20;

			/// <summary>
			/// The maximum number of generations before forcing a guess
			/// </summary>
			public int MaxGenerations = 200;

			/// <see cref="ICloneable.Clone"/>
			public object Clone()
			{
                return new GeneticSolverSettings
                {
                    PoolSize = PoolSize,
                    CrossoverAmount = CrossoverAmount,
                    MutationRate = MutationRate,
                    ElitismCutoff = ElitismCutoff,
                    MatchScore = MatchScore,
                    PartialMatchScore = PartialMatchScore,
					MaxGenerations = MaxGenerations
				};
			}
		}

		//Tuning parameters
		private GeneticSolverSettings Settings = new GeneticSolverSettings();

		struct PoolMember : IComparable<PoolMember>
		{
			public RowState Row;
			public int Score;

			public int CompareTo(PoolMember other)
			{
				return other.Score - Score;
			}
		}

		//SortedSet<PoolMember> sorted = new SortedSet<PoolMember>();

		private PoolMember[] Pool;
		private float[] CDF;
		private List<int>[] IndexBuckets;
		private int Generations = 0;

		public GeneticSolver()
		{

		}

		/// <summary>
		/// Generate the initial pool with random guesses
		/// </summary>
		/// <param name="Board">The board to use</param>
		public void GeneratePool(GameBoard Board)
		{
			Pool = new PoolMember[Settings.PoolSize];

			for(int i = 0; i < Settings.PoolSize; i ++)
			{
			PoolMember NewMember = new PoolMember();
				NewMember.Row = RowState.GetRandomColors(Generator, Board.NumColors, Board.NumColumns);
				Pool[i] = NewMember;
			}


			//P(x) = -1*x + 1
			//CDF(X) = (1/2)x^2

			//Generate the Cumulative Distribution Function from the probability function
		//float PSum = 1.0f;
			
			CDF = new float[Settings.PoolSize - Settings.ElitismCutoff];

			for (int i = 0; i < CDF.Length; i ++)
			{
				//CDF[i] = 0.4999f - ( 0.5f * (float)Math.Pow((float)i / (float)CDF.Length, 2));
				CDF[i] = 0.5f * (float)Math.Pow( 1.0f - ((float)i / (float)CDF.Length), 2);
			}

			//Bucket the CDF for faster lookup
			IndexBuckets = new List<int>[CDF.Length / 25];

			for(int i = 0; i < IndexBuckets.Length; i ++)
			{
				IndexBuckets[i] = new List<int>();
			}

			for(int i = 0; i < CDF.Length; i ++)
			{
			int Bucket = (int)(CDF[i] / (0.5f / (float)IndexBuckets.Length));
				
				if (Bucket == IndexBuckets.Length)
					Bucket--;

				IndexBuckets[Bucket].Add(i);
			}
		}

		/// <summary>
		/// Score the row probabity compared to the previous guess
		/// </summary>
		/// <param name="Row">The row to score</param>
		/// <param name="PlayedRow">The previous scored row</param>
		/// <returns>0 if the row is a possible fit, decreasing values the worse the fit is</returns>
		private int CompareRows(RowState Row, BoardRow PlayedRow)
		{
		bool[] RowMatched = new bool[Row.Length];
        bool[] GuessMatched = new bool[Row.Length];
        int SamePosAndColor = 0, SameColor = 0;
		int Score = 0;

			for (int i = 0; i < Row.Length; i++)
			{
				if (Row[i] == PlayedRow.Row[i])
				{
					SamePosAndColor++;
                    RowMatched[i] = GuessMatched[i] = true;
                }
			}

		int MatchDifference = Math.Abs(PlayedRow.Score.NumCorrectSpot - SamePosAndColor);
			Score -= Settings.MatchScore * MatchDifference;

			for (int i = 0; i < Row.Length; i++)
			{
				for (int j = 0; j < Row.Length && !RowMatched[i]; j++)
				{
					if ((Row[i] == PlayedRow.Row[j]) && !GuessMatched[j])
					{
						SameColor++;
						GuessMatched[j] = true;
						break;
					}
				}
			}

		int ColorDifference = Math.Abs(PlayedRow.Score.NumCorrectColor - SameColor);
			Score -= Settings.PartialMatchScore * ColorDifference;

			return Score;
		}

		/// <summary>
		/// Score the entire pool
		/// </summary>
		/// <param name="Board">The gameboard in use</param>
		private void ScorePool(GameBoard Board)
		{
			Parallel.For(0, Pool.Length,
				(i) =>
				{
					Pool[i].Score = Eval(Pool[i].Row, Board);
				});
		}

		/// <summary>
		/// Sort the pool in decending order
		/// </summary>
		private void SortPool()
		{
			ParallelSort.QuicksortParallel(Pool);
			//ParallelSort.QuicksortSequential(Pool);
		}

		/// <summary>
		/// Score a row based on all previous guesses
		/// </summary>
		/// <param name="Row">The row to score</param>
		/// <param name="Board">The board info</param>
		/// <returns>0 if the row is a possible fit, decreasing values the worse the fit is</returns>
		private int Eval(RowState Row, GameBoard Board)
		{
		int Score = 0;

			foreach(BoardRow BR in Board.Guesses)
			{
				Score += CompareRows(Row, BR);
			}

			return Score;
		}

		/// <summary>
		/// Selects a suitable parent for crossover operations
		/// </summary>
		/// <returns>A parent index to use</returns>
		private int SelectParent(float WeightSum)
		{
		float Rand = (float)Generator.NextDouble() * WeightSum;
		int Bucket = (int)(Rand / (WeightSum / (float)IndexBuckets.Length));

			if (Bucket == IndexBuckets.Length)
				Bucket--;

		List<int> PoolBucket = IndexBuckets[Bucket];

			for(int i = 0; i < PoolBucket.Count - 1; i ++)
			{
				if((Rand > CDF[PoolBucket[i + 1]]) && (Rand <= CDF[PoolBucket[i]]))
				{
					return PoolBucket[i] + Settings.ElitismCutoff;
				}
			}

			return PoolBucket[PoolBucket.Count - 1];
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		private void Evolve(GameBoard Board)
		{
			ScorePool(Board);
			SortPool();

			Generations = 0;

			//Keep evolving until a possible solution is found, or a max number of generations have occured
			for (int i = 0; i < Settings.MaxGenerations && Pool[0].Score != 0; i++)
			{
				PerformEvolveOperation(Board);
				ScorePool(Board);
				SortPool();

				Generations++;
			}
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		private void PerformEvolveOperation(GameBoard Board)
		{
		int Crossovers = (int)(Pool.Length * Settings.CrossoverAmount);
		int Columns = Pool[0].Row.Length;
		byte[] ColorA = new byte[Columns];
		byte[] ColorB = new byte[Columns];
		byte[] NewColorA = new byte[Columns];
		byte[] NewColorB = new byte[Columns];
		int[] ShuffleIndex = new int[Columns * 2];

			for(int i = 0; i < ShuffleIndex.Length; i ++)
				ShuffleIndex[i] = i;

		PoolMember[] NewPool = new PoolMember[Settings.PoolSize];
		int NewPoolIndex = 0;

			//Copy elite members
			for (int e = 0; e < Settings.ElitismCutoff; e ++)
			{
				NewPool[e] = Pool[e];
			}

			NewPoolIndex = Settings.ElitismCutoff;

			//Do Crossover members
			for (int Cross = 0; Cross < Crossovers / 2; Cross += 2, NewPoolIndex += 2)
			{
			int IndexA = SelectParent(0.5f);
			int IndexB = SelectParent(0.5f);

				Pool[IndexA].Row.CopyTo(ColorA);
				Pool[IndexB].Row.CopyTo(ColorB);

				for(int i = 0; i < ShuffleIndex.Length; i ++)
				{
					int Swap = Generator.Next(ShuffleIndex.Length);
					int Temp = ShuffleIndex[Swap];
					ShuffleIndex[Swap] = ShuffleIndex[i];
					ShuffleIndex[i] = Temp;
				}

				for (int i = 0; i < Columns; i ++)
				{
					byte Color = 0;

					if (ShuffleIndex[i] < Columns)
						Color = ColorA[ShuffleIndex[i]];
					else
						Color = ColorB[ShuffleIndex[i] - Columns];

					NewColorA[i] = Color;
				}

				for (int i = Columns; i < Columns * 2; i++)
				{
					byte Color = 0;

					if (ShuffleIndex[i] < Columns)
						Color = ColorA[ShuffleIndex[i]];
					else
						Color = ColorB[ShuffleIndex[i] - Columns];

					NewColorB[i - Columns] = Color;
				}

				NewPool[NewPoolIndex].Row = new RowState(NewColorA);
				NewPool[NewPoolIndex + 1].Row = new RowState(NewColorB);
			}

			//Mutations
			for(; NewPoolIndex < NewPool.Length; NewPoolIndex ++)
			{
				int Parent = SelectParent(0.5f);
				Pool[Parent].Row.CopyTo(NewColorA);

				bool[] Selected = new bool[Columns];

				int MutationIndex = 0;
				int NumMutations = (int)(Columns * Settings.MutationRate);

				while(MutationIndex < NumMutations)
				{
					int Column = (int)(Generator.NextDouble() * Columns);

					if (Selected[Column])
						continue;

					Selected[Column] = true;
					NewColorA[Column] = (byte)(Generator.NextDouble() * Board.NumColors);
					MutationIndex++;
				}

				NewPool[NewPoolIndex].Row = new RowState(NewColorA);
			}

			Pool = NewPool;
		}

		/// <see cref="Solver.GetGuess(GameBoard)"/>
		public RowState GetGuess(GameBoard Board)
		{
			//Do a seed guess
			if (Board.Guesses.Count < 2 || (Board.Guesses.Count == 2 && Board.NumColumns > 6 && Board.NumColors > 5))
				return SeedGuess.GetGuess(Board, (byte)Board.Guesses.Count);

			if (Pool == null)
				GeneratePool(Board);

			//Perform pool evolution
			Evolve(Board);

		PoolMember Guess = new PoolMember();

			//Skip guesses that have already been made
			for (int s = 0; s < Pool.Length; s ++)
			{
			int i = 0;

				for (i = 0; i < Board.Guesses.Count; i++)
				{
					if (Board.Guesses[i].Row == Pool[s].Row)
						break;
				}

				if(i == Board.Guesses.Count)
				{
					Guess = Pool[s];
					break;
				}
			}

			Status = string.Format("Best Score: {0}, Generations: {1}", 
				Guess.Score, Generations);

			return Guess.Row;
		}

		/// <see cref="Solver.ShowSettingsDialog"/>
		public void ShowSettingsDialog()
		{
			using (GeneticSettings SettDlg = new GeneticSettings())
			{
				SettDlg.Settings = (GeneticSolverSettings)Settings.Clone();
				System.Windows.Forms.DialogResult Res = SettDlg.ShowDialog();

				if (Res == System.Windows.Forms.DialogResult.OK)
				{
					Settings = SettDlg.Settings;
					Reset();
				}
			}
		}

		/// <see cref="Solver.Reset"/>
		public void Reset()
		{
			Pool = null;
			GC.Collect();
		}

		/// <see cref="Solver.GetMessage"/>
		public string GetMessage()
		{
			return Status;
		}
	}
}
