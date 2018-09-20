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

			/// <summary>
			/// Linear crossover function is used if true, random if false
			/// </summary>
			public bool LinearCrossover = true;

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

			for(int i = 0; i < Pool.Length; i ++)
			{
			PoolMember NewMember = new PoolMember();
				NewMember.Row = RowState.GetRandomColors(Generator, Board.NumColors, Board.NumColumns);
				Pool[i] = NewMember;
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
					Pool[i].Score = EvalRow(Pool[i].Row, Board);
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
		/// <returns>A parent index to use</returns>
		private int SelectParent()
		{
			//P(x) = -1*x + 1
			//CDF(X) = -(1/2)x^2 + x = y
			//X = 1 - (1 - 2*y)^(1/2)
			float y = (float)Generator.NextDouble() * 0.5f;
			float x = 1.0f - (float)Math.Sqrt(1.0f - 2.0f * y);
			x *= Pool.Length;
			int Result = (int)Math.Floor(x);

			return Result < Pool.Length ? Result : Pool.Length - 1; //This actually can happen
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		private void Evolve(GameBoard Board)
		{
			//Score and sort from the last guess
			ScorePool(Board);
			SortPool();

			Generations = 0;

			//Keep evolving until a possible solution is found (at least one generation though), or a max number of generations have occured
			do
			{
				PerformEvolveOperations(Board);
				ScorePool(Board);
				SortPool();

				Generations++;
			} while (Generations < Settings.MaxGenerations && Pool[0].Score != 0);
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		private void PerformEvolveOperations(GameBoard Board)
		{
		int Crossovers = (int)(Pool.Length * Settings.CrossoverAmount);
		int Columns = Pool[0].Row.Length;
		byte[] ColorA = new byte[Columns];
		byte[] ColorB = new byte[Columns];
		byte[] NewColorA = new byte[Columns];
		byte[] NewColorB = new byte[Columns];
		byte[] ShuffleColor = new byte[Columns * 2];
		bool[] Selected = new bool[Columns];

		PoolMember[] NewPool = new PoolMember[Settings.PoolSize];
		int NewPoolIndex = 0;

			//Copy elite members
			for (int i = 0; i < Settings.ElitismCutoff; i ++)
				NewPool[i] = Pool[i];

			NewPoolIndex = Settings.ElitismCutoff;

			//Do Crossover members
			for (int Cross = 0; Cross < Crossovers; Cross += 2, NewPoolIndex += 2)
			{
				//Linear crossover
				if (Settings.LinearCrossover)
				{
					Pool[SelectParent()].Row.CopyTo(ColorA);
					Pool[SelectParent()].Row.CopyTo(ColorB);
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
					Pool[SelectParent()].Row.CopyTo(ShuffleColor, 0);
					Pool[SelectParent()].Row.CopyTo(ShuffleColor, Columns);

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

			//Mutations
			for(; NewPoolIndex < NewPool.Length; NewPoolIndex ++)
			{
				int Parent = SelectParent();
				Pool[Parent].Row.CopyTo(NewColorA);

				for(int i = 0; i < Selected.Length; i ++)
					Selected[i] = false;

				for(int i = 0; i < (int)Math.Round(Columns * Settings.MutationRate); i ++)
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
				return SeedGuess.GetGuess(Board, (byte)Board.Guesses.Count);

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

		long PoolScore = 0;

			for (int i = 0; i < Pool.Length; i++)
			{
				PoolScore += Pool[i].Score;
			}

		int AveragePoolScore = (int)(PoolScore / Pool.Length);

			Status = string.Format("Best Score: {0}, Average Score: {2}, Generations: {1}", 
				Guess.Score, Generations, AveragePoolScore);

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
