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
		public event Action<string> SetMessage;

		private volatile bool AbortProcessing = false;

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


		/// <summary>
		/// Genetic solver constructor
		/// </summary>
		public GeneticSolver()
		{

		}

		public class Guess : IGeneticItem, ICloneable
		{
			private readonly GameBoard Board;
			private readonly int MatchScore;
			private readonly int PartialMatchScore;

			public readonly RowState GuessState;

			public Guess(GameBoard Board, int MatchScore, int PartialMatchScore, RowState State)
			{
				this.Board = Board;
				this.MatchScore = MatchScore;
				this.PartialMatchScore = PartialMatchScore;
				this.GuessState = State;

				RowColumnMatched = new bool[Board.NumColumns];
				GuessColumnMatched = new bool[Board.NumColumns];
			}

			private bool[] RowColumnMatched;
			private bool[] GuessColumnMatched;

			/// <summary>
			/// Score the row possibility compared to the given guess
			/// </summary>
			/// <param name="Row">The row to score</param>
			/// <param name="PlayedRow">The previously scored row</param>
			/// <returns>0 if the row is a possible fit, decreasing values the worse the fit is</returns>
			private int CompareRows(BoardRow PlayedRow)
			{
				for (int i = 0; i < GuessState.Length; i++)
				{
					RowColumnMatched[i] = GuessColumnMatched[i] = false;
				}

				int SamePosAndColor = 0, SameColor = 0;
				int Score = 0;

				//Score reds
				for (int i = 0; i < GuessState.Length; i++)
				{
					if (GuessState[i] == PlayedRow.Row[i])
					{
						SamePosAndColor++;
						RowColumnMatched[i] = GuessColumnMatched[i] = true;
					}
				}

				int MatchDifference = Math.Abs(PlayedRow.Score.NumCorrectSpot - SamePosAndColor);
				Score -= MatchScore * MatchDifference;

				//Score whites
				for (int i = 0; i < GuessState.Length; i++)
				{
					for (int j = 0; j < GuessState.Length && !RowColumnMatched[i]; j++)
					{
						if (!GuessColumnMatched[j] && (GuessState[i] == PlayedRow.Row[j]))
						{
							SameColor++;
							GuessColumnMatched[j] = true;
							break;
						}
					}
				}

				int ColorDifference = Math.Abs(PlayedRow.Score.NumCorrectColor - SameColor);
				Score -= PartialMatchScore * ColorDifference;

				return Score;
			}

			/// <summary>
			/// Score a row based on all previous guesses
			/// </summary>
			/// <param name="Row">The row to score</param>
			/// <param name="Board">The board info</param>
			/// <returns>0 if the row is a possible fit, decreasing values the worse the fit is</returns>
			private int EvalRow()
			{
				int Score = 0;

				foreach (BoardRow BR in Board.Guesses)
				{
					Score += CompareRows(BR);
				}

				return Score;
			}

			public int GetScore()
			{
				return EvalRow();
			}

			public object Clone()
			{
				return new Guess(Board, MatchScore, PartialMatchScore, GuessState);
			}
		}


		public class GuessFactory : IGeneticItemFactory<Guess>
		{
			private readonly GameBoard Board;
			private readonly GeneticSolverSettings Settings;
			private Random RandGenerator = new Random();

			public GuessFactory(GameBoard Board, GeneticSolverSettings Settings)
			{
				this.Board = Board;
				this.Settings = Settings;
				NewColorA = new byte[Board.NumColumns];
				NewColorB = new byte[Board.NumColumns];
				Selected = new bool[Board.NumColumns];
			}

			public Guess GetRandom()
			{
				return new Guess(Board, Settings.MatchScore, Settings.PartialMatchScore, 
					RowState.GetRandomColors(RandGenerator, Board.NumColors, Board.NumColumns));
			}

			private byte[] NewColorA;
			private byte[] NewColorB;
			private bool[] Selected;

			public void Cross(Guess A, Guess B, out Guess ResultA, out Guess ResultB)
			{
				int SplitIndex = RandGenerator.Next(Board.NumColumns);

				for (int i = 0; i < SplitIndex; i++)
				{
					NewColorA[i] = A.GuessState[i];
					NewColorB[i] = B.GuessState[i];
				}

				for (int i = SplitIndex; i < Board.NumColumns; i++)
				{
					NewColorA[i] = B.GuessState[i];
					NewColorB[i] = A.GuessState[i];
				}

				ResultA = new Guess(Board, Settings.MatchScore, Settings.PartialMatchScore, new RowState(NewColorA));
				ResultB = new Guess(Board, Settings.MatchScore, Settings.PartialMatchScore, new RowState(NewColorB));
			}

			public Guess Mutate(Guess Item)
			{
				Item.GuessState.CopyTo(NewColorA);

				for (int i = 0; i < Selected.Length; i++)
					Selected[i] = false;

				int ColumnsToMutate = RandGenerator.Next(1, Board.NumColumns / 2 + 1);

				do
				{
					int i = RandGenerator.Next(Board.NumColumns);

					if (!Selected[i])
					{
						Selected[i] = true;
						NewColorA[i] = (byte)RandGenerator.Next(Board.NumColors);
						ColumnsToMutate--;
					}
				} while (ColumnsToMutate > 0);

				return new Guess(Board, Settings.MatchScore, Settings.PartialMatchScore, new RowState(NewColorA));
			}
		}

		private GeneticAlgorithm<Guess, GuessFactory> Solver;

		private void UpdateMessage(int Generation)
		{
			//Calculate some diagnostic information
			long PoolScore = 0;

			for (int i = 0; i < Solver.Pool.Length; i++)
			{
				PoolScore += Solver.Pool[i].Score;
			}

			int AveragePoolScore = (int)(PoolScore / Solver.Pool.Length);

			PoolScore = 0;

			for (int i = 0; i < Settings.ElitismCutoff; i++)
			{
				PoolScore += Solver.Pool[i].Score;
			}

			int ElitePoolScore = (int)(PoolScore / Settings.ElitismCutoff);

			SetMessage?.Invoke(string.Format("Best: {0}, Elite: {1}, Pool: {2}, Generations: {3}",
				Solver.Pool[0].Score, ElitePoolScore, AveragePoolScore, Generation));
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		private void Evolve()
		{
			//Score and sort using new info from the last guess
			Solver.ScoreAndSortPool();

			//Keep evolving until a possible solution is found (with at least one generation), or a max number of generations have occurred

			int Generation = 0;
			do
			{
				Solver.Evolve(Settings.ElitismCutoff, Settings.CrossoverAmount, Settings.MutationRate);
				Solver.ScoreAndSortPool();
				UpdateMessage(Generation + 1);
				Generation++;
			} while (Generation < Settings.MaxGenerations && Solver.Pool[0].Score != 0 && !AbortProcessing);
		}

		/// <see cref="Solver.GetGuess(GameBoard)"/>
		public RowState GetGuess(GameBoard Board)
		{
			//Do a seed guess
			if (Board.Guesses.Count < 2 || (Board.Guesses.Count == 2 && Board.NumColumns > 6 && Board.NumColors > 5))
				return SeedGuess.GetGuess(Board);

			if(Solver == null)
			{
				Solver = new GeneticAlgorithm<Guess, GuessFactory>(new GuessFactory(Board, Settings));
				Solver.GeneratePool(Settings.PoolSize);
			}

			//Perform pool evolution
			Evolve();

			//Skip guesses that have already been made
			for(int i = 0; i < Solver.Pool.Length; i ++)
			{
				Guess G = Solver.Pool[i].Item;

				if (!Board.Guesses.Exists(m => m.Row == G.GuessState))
				{
					return G.GuessState;
				}
			}

			return new RowState(new byte[Board.NumColumns]);
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
			Solver = null;
			AbortProcessing = false;
		}

		/// <see cref="Solver.Abort"/>
		public void Abort()
		{
			AbortProcessing = true;
		}
	}
}
