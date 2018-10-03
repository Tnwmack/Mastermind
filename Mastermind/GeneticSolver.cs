using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// A Mastermind solver that uses genetic algorithms.
	/// </summary>
	public class GeneticSolver : Solver
	{
		private Random Generator = new Random();

		/// <summary>
		/// The current status to send to the GUI.
		/// </summary>
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
			/// The number of crossovers to perform (pool*CrossoverAmount), 
			/// remainder are elites and mutations
			/// </summary>
			public float CrossoverAmount = 0.7f;

			/// <summary>
			/// The rate that colors are mutated (columns*MutationRate) mutations
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
			/// The penalty score for correct color but incorrect spot discrepancies
			/// </summary>
			public int PartialMatchScore = 20;

			/// <summary>
			/// The maximum number of generations before forcing a guess
			/// </summary>
			public int MaxGenerations = 200;

			/// <summary>
			/// Linear crossover function is used if true, random if false
			/// </summary>
			public bool LinearCrossover = true;

			/// <summary>
			/// Varies the crossover amount based on certain criteria. CrossoverAmount is ignored.
			/// </summary>
			public bool DynamicCrossoverAmount = false;

			/// <summary>
			/// Varies the mutation rate based on certain criteria. MutationRate is ignored.
			/// </summary>
			public bool DynamicMutationRate = true;

			/// <summary>
			/// Pool members with less than this score are eliminated, 0 to disable.
			/// </summary>
			public int ScoreCutoff = -500;

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
					MaxGenerations = MaxGenerations,
					LinearCrossover = LinearCrossover,
					DynamicCrossoverAmount = DynamicCrossoverAmount,
					DynamicMutationRate = DynamicMutationRate,
					ScoreCutoff = ScoreCutoff
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

		/// <summary>
		/// The genetic pool.
		/// </summary>
		private PoolMember[] Pool;

		/// <summary>
		/// The number of generations in the last update
		/// </summary>
		private int Generations = 0;

		/// <summary>
		/// Genetic solver constructor
		/// </summary>
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

			for(int i = 0; i < Pool.Length; i ++)
			{
			PoolMember NewMember = new PoolMember();
				NewMember.Row = RowState.GetRandomColors(Generator, Board.NumColors, Board.NumColumns);
				Pool[i] = NewMember;
			}
		}

		/// <summary>
		/// Score the row possibility compared to the given guess
		/// </summary>
		/// <param name="Row">The row to score</param>
		/// <param name="PlayedRow">The previously scored row</param>
		/// <returns>0 if the row is a possible fit, decreasing values the worse the fit is</returns>
		private int CompareRows(RowState Row, BoardRow PlayedRow)
		{
		bool[] RowMatched = new bool[Row.Length];
		bool[] GuessMatched = new bool[Row.Length];
		int SamePosAndColor = 0, SameColor = 0;
		int Score = 0;

			//Score reds
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

			//Score whites
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
					Pool[i].Score = EvalRow(Pool[i].Row, Board);
				});
		}

		/// <summary>
		/// Sort the pool in descending order
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
		private int EvalRow(RowState Row, GameBoard Board)
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
		/// <param name="MaxIndex">Ignore pool members higher than this index</param>
		/// <returns>A parent index to use</returns>
		private int SelectParent(int MaxIndex)
		{
			int UsablePoolSize = Math.Min(MaxIndex, Pool.Length);
			//P(x) = -1*x + 1
			//CDF(X) = -(1/2)x^2 + x = y
			//X = 1 - (1 - 2*y)^(1/2)
			float y = (float)Generator.NextDouble() * 0.5f;
			float x = 1.0f - (float)Math.Sqrt(1.0f - 2.0f * y);
			x *= UsablePoolSize;
			int Result = (int)Math.Floor(x);

			return Result < UsablePoolSize ? Result : UsablePoolSize - 1; //This actually can happen
		}

		/// <summary>
		/// Gets the index for the minimum score cutoff 
		/// </summary>
		/// <returns>The index of the pool member that exceeds the cutoff value</returns>
		private int GetEugenicsIndex()
		{
			if (Settings.ScoreCutoff == 0)
				return Pool.Length;

		int MinIndex = 0;

			for (int i = 0; i < Pool.Length; i ++)
			{
				if (Pool[i].Score < Settings.ScoreCutoff)
				{
					MinIndex = i;
					break;
				}
			}

			return Math.Max(Pool.Length / 4, MinIndex);
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		/// <param name="Board">The board to use</param>
		private void Evolve(GameBoard Board)
		{
			//Score and sort from the last guess
			ScorePool(Board);
			SortPool();

			Generations = 0;

			//Keep evolving until a possible solution is found (with at least one generation), or a max number of generations have occurred
			do
			{
				PerformEvolveOperations(Board);
				ScorePool(Board);
				SortPool();
			} while (++Generations < Settings.MaxGenerations && Pool[0].Score != 0);
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		/// <param name="Board">The board to use</param>
		private void PerformEvolveOperations(GameBoard Board)
		{
		int Columns = Pool[0].Row.Length;
		int PoolCutoff = GetEugenicsIndex();
		byte[] ColorA = new byte[Columns];
		byte[] ColorB = new byte[Columns];
		byte[] NewColorA = new byte[Columns];
		byte[] NewColorB = new byte[Columns];
		byte[] ShuffleColor = new byte[Columns * 2];
		bool[] Selected = new bool[Columns];

		PoolMember[] NewPool = new PoolMember[Settings.PoolSize]; //TODO: Look into using a backbuffer pool
		int NewPoolIndex = 0;

			//Copy elite members
			for (int i = 0; i < Settings.ElitismCutoff; i ++)
				NewPool[i] = Pool[i];

			NewPoolIndex = Settings.ElitismCutoff;

		float CrossoverAmount = Settings.CrossoverAmount;

			//Tweak crossover rate to improve final convergence
			if (Settings.DynamicCrossoverAmount)
			{
				CrossoverAmount = 0.6f;

				//With high white, use a high crossover value
				//With high red, use a low crossover value

				int TippingPointRed = (int)Math.Floor(Columns * 0.8);
				int TippingPointWhite = (int)Math.Floor(Columns * 0.7);

				if (Board.Guesses[Board.Guesses.Count - 1].Score.NumCorrectSpot >= TippingPointRed)
				{
					int Diff = Columns - Board.Guesses[Board.Guesses.Count - 1].Score.NumCorrectSpot;
					CrossoverAmount = (float)Diff / (float)Columns;
				}
				else if(Board.Guesses[Board.Guesses.Count - 1].Score.NumCorrectColor >= TippingPointWhite)
				{
					int Diff = Columns - Board.Guesses[Board.Guesses.Count - 1].Score.NumCorrectColor;
					CrossoverAmount = 1.0f - (float)Diff / (float)Columns;
				}
			}

			//Do Crossover members
			for (int Cross = 0; Cross + 1 < (int)Math.Floor((Pool.Length - Settings.ElitismCutoff) * CrossoverAmount); 
				Cross += 2, NewPoolIndex += 2)
			{
				//Linear crossover
				if (Settings.LinearCrossover)
				{
					Pool[SelectParent(PoolCutoff)].Row.CopyTo(ColorA);
					Pool[SelectParent(PoolCutoff)].Row.CopyTo(ColorB);
					int Index = Generator.Next(Columns);

					for (int i = 0; i < Index; i++)
					{
						NewColorA[i] = ColorA[i];
						NewColorB[i] = ColorB[i];
					}

					for (int i = Index; i < Columns; i++)
					{
						NewColorA[i] = ColorB[i];
						NewColorB[i] = ColorA[i];
					}

					NewPool[NewPoolIndex].Row = new RowState(NewColorA);
					NewPool[NewPoolIndex + 1].Row = new RowState(NewColorB);
				}
				else //Random crossover
				{
					Pool[SelectParent(PoolCutoff)].Row.CopyTo(ShuffleColor, 0);
					Pool[SelectParent(PoolCutoff)].Row.CopyTo(ShuffleColor, Columns);

					for (int i = 0; i < ShuffleColor.Length; i++)
					{
						int SwapIndex = Generator.Next(ShuffleColor.Length);
						byte Temp = ShuffleColor[SwapIndex];
						ShuffleColor[SwapIndex] = ShuffleColor[i];
						ShuffleColor[i] = Temp;
					}

					Array.Copy(ShuffleColor, 0, NewColorA, 0, Columns);
					Array.Copy(ShuffleColor, Columns, NewColorB, 0, Columns);

					NewPool[NewPoolIndex].Row = new RowState(NewColorA);
					NewPool[NewPoolIndex + 1].Row = new RowState(NewColorB);
				}
			}

			float MutationRate = Settings.MutationRate;

			//Tweak the mutation rate to improve final convergence
			if(Settings.DynamicMutationRate)
			{
				MutationRate = 0.25f;

				int TippingPoint = (int)Math.Floor(Columns * 0.8); 

				if(Board.Guesses[Board.Guesses.Count - 1].Score.NumCorrectSpot >= TippingPoint)
				{
					int Diff = Columns - Board.Guesses[Board.Guesses.Count - 1].Score.NumCorrectSpot;
					MutationRate = (float)Diff / (float)Columns;
				}
			}

			//Mutations
			for (; NewPoolIndex < NewPool.Length; NewPoolIndex ++)
			{
				int Parent = SelectParent(PoolCutoff);
				Pool[Parent].Row.CopyTo(NewColorA);

				for(int i = 0; i < Selected.Length; i ++)
					Selected[i] = false;

				for (int i = 0; i < Math.Max((int)Math.Round(Columns * MutationRate), 1); i ++)
				{
					int Column = 0;

					do
					{
						Column = Generator.Next(Columns);
					}
					while (Selected[Column]);

					Selected[Column] = true;
					NewColorA[Column] = (byte)Generator.Next(Board.NumColors);
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
				return SeedGuess.GetGuess(Board);

			if (Pool == null)
				GeneratePool(Board);

			//Perform pool evolution
			Evolve(Board);

		System.Collections.IEnumerator PoolEnum = Pool.GetEnumerator();
		PoolMember Guess = new PoolMember();

			//Skip guesses that have already been made
			do
			{
				if (!PoolEnum.MoveNext()) break;
				Guess = (PoolMember)PoolEnum.Current;
			}
			while (Board.Guesses.Exists(m => m.Row == Guess.Row));

			//Calculate some diagnostic information
		long PoolScore = 0;

			for (int i = 0; i < Pool.Length; i++)
			{
				PoolScore += Pool[i].Score;
			}

		int AveragePoolScore = (int)(PoolScore / Pool.Length);

			PoolScore = 0;

			for (int i = 0; i < Settings.ElitismCutoff; i++)
			{
				PoolScore += Pool[i].Score;
			}

		int ElitePoolScore = (int)(PoolScore / Settings.ElitismCutoff);

			Status = string.Format("Best: {0}, Elite: {1}, Pool: {2}, Generations: {3}", 
				Guess.Score, ElitePoolScore, AveragePoolScore, Generations);

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
