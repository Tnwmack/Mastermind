using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	class GeneticSolver : Solver
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
			public float MutationRate = 0.1f;

			/// <summary>
			/// The number top scoring pool members that do not 
			/// undergo crossovers or mutation
			/// </summary>
			public int ElitismCutoff = 20;

			/// <summary>
			/// If true, the members are shufffled together randomly
			/// If false, they are split and crossed
			/// </summary>
			public bool PerformUnion = false;

			/// <summary>
			/// The penalty score for correct color and spot discrepancies
			/// </summary>
			public int MatchScore = 50;

			/// <summary>
			/// The penalty score for correct color discrepancies
			/// </summary>
			public int PartialMatchScore = 20;

			/// <see cref="ICloneable.Clone"/>
			public object Clone()
			{
			GeneticSolverSettings Result = new GeneticSolverSettings();

				Result.PoolSize = PoolSize;
				Result.CrossoverAmount = CrossoverAmount;
				Result.MutationRate = MutationRate;
				Result.ElitismCutoff = ElitismCutoff;
				Result.PerformUnion = PerformUnion;
				Result.MatchScore = MatchScore;
				Result.PartialMatchScore = PartialMatchScore;

				return Result;
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
		bool[] Matched = new bool[Row.Length];
		int SamePosAndColor = 0, SameColor = 0;
		int Score = 0;

			for (int i = 0; i < Row.Length; i++)
			{
				if (Row[i] == PlayedRow.Row[i])
				{
					SamePosAndColor++;
				}
			}

		int MatchDifference = Math.Abs(PlayedRow.Score.NumCorrectSpot - SamePosAndColor);
			Score -= MatchDifference * Settings.MatchScore;

			for (int i = 0; i < Row.Length; i++)
			{
				for (int j = 0; j < Row.Length; j++)
				{
					if ((Row[i] == PlayedRow.Row[j]) && !Matched[j])
					{
						SameColor++;
						Matched[j] = true;
					}
				}
			}

		int ColorDifference = Math.Abs(PlayedRow.Score.NumCorrectColor - (SameColor - SamePosAndColor));
			Score -= Settings.PartialMatchScore * ColorDifference;

			return Score;



			/*bool[] Matched = new bool[Row.Length];
			int SamePosAndColor = 0, SameColor = 0;
			int Score = 0;

				for (int i = 0; i < Row.Length; i++)
				{
					if (Row[i] == PlayedRow.Row[i])
					{
						SamePosAndColor++;
						Matched[i] = true;
					}
				}

			int MatchDifference = Math.Abs(PlayedRow.Score.NumCorrectSpot - SamePosAndColor);
				Score -= MatchDifference * Settings.MatchScore;

				for (int i = 0; i < Row.Length; i++)
				{
					for (int j = 0; j < Row.Length; j++)
					{
						if (i != j && Row[i] == PlayedRow.Row[j] && !Matched[j])
						{
							SameColor++;
							Matched[j] = true;
						}
					}
				}

			int ColorDifference = Math.Abs(PlayedRow.Score.NumCorrectColor - SameColor);
				Score -= Settings.PartialMatchScore * ColorDifference;

				return Score;*/
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
		int Crossovers = (int)(Pool.Length * Settings.CrossoverAmount);
		int Columns = Pool[0].Row.Length;
		byte[] ColorA = new byte[Columns];
		byte[] ColorB = new byte[Columns];

			for (int Cross = 0; Cross < Crossovers; Cross++)
			{
				int IndexA = SelectParent(0.5f);
				int IndexB = SelectParent(0.5f);

				//int IndexA = Parents[Cross * 2];
				//int IndexB = Parents[Cross * 2 + 1];

				Pool[IndexA].Row.CopyTo(ColorA);
				Pool[IndexB].Row.CopyTo(ColorB);

				//Perform a full random union operation
				if (Settings.PerformUnion)
				{
					for (int i = 0; i < 8; i++)
					{
						for (int j = 0; j < Columns; j++)
						{
							int Swap = Generator.Next(Columns);
							byte Temp = ColorA[j];

							ColorA[j] = ColorB[Swap];
							ColorB[Swap] = Temp;
						}
					}
				}
				else
				{
					//Perform a random linear crossover
					int CrossPoint = Generator.Next(Columns);

					for (int i = CrossPoint; i < Columns; i++)
					{
						byte Temp = ColorA[i];

						ColorA[i] = ColorB[i];
						ColorB[i] = Temp;
					}
				}

				//Do mutations
				for (int i = 0; i < Columns; i++)
				{
					if (Generator.NextDouble() < Settings.MutationRate)
						ColorA[i] = (byte)Generator.Next(Board.NumColors);

					if (Generator.NextDouble() < Settings.MutationRate)
						ColorB[i] = (byte)Generator.Next(Board.NumColors);
				}

				Pool[IndexA].Row = new RowState(ColorA);
				Pool[IndexB].Row = new RowState(ColorB);
			}
		}

		/// <summary>
		/// Gets a random guess that maximises the colors used
		/// </summary>
		/// <param name="Board">The board in use</param>
		/// <returns>A random guess</returns>
		private RowState GetNoisyGuess(GameBoard Board)
		{
		byte[] ResultColors = new byte[Board.NumColumns];

			if (Board.NumColors <= Board.NumColumns)
			{
			byte Color = 0;

				for (int i = 0; i < ResultColors.Length; i++)
				{
					ResultColors[i] = Color;

					Color ++;
					Color = (byte)(Color % Board.NumColors);
				}

				//Randomize order
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < ResultColors.Length; j++)
					{
					int Index = Generator.Next(ResultColors.Length);
					byte Temp = ResultColors[j];
						ResultColors[j] = ResultColors[Index];
						ResultColors[Index] = Temp;
					}
				}
			}
			else
			{
			byte[] Colors = new byte[Board.NumColors];

				//Generate array of colors
				for (int i = 0; i < Colors.Length; i++)
					Colors[i] = (byte)i;

				for (int i = 0; i < ResultColors.Length; i++)
				{
				int Index = Generator.Next(Colors.Length);
					ResultColors[i] = Colors[Index];
				}
			}

			return new RowState(ResultColors);
		}

		/// <see cref="Solver.GetGuess(GameBoard)"/>
		public RowState GetGuess(GameBoard Board)
		{
			//Use high-entropy first guesses
			if( (Board.Guesses.Count == 0) || (Board.Guesses.Count == 1))
			{
				return GetNoisyGuess(Board);
			}

			if (Pool == null)
				GeneratePool(Board);

			//The pool must be scored and sorted prior to evolution
			//The parent selection and elitism are based on the score
			ScorePool(Board);
			SortPool();

			Evolve(Board);

			ScorePool(Board);
			SortPool();

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

			Status = "Top Guess Score: " + Guess.Score.ToString();

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
