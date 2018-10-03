using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mastermind
{
	class KnuthSolver : Solver
	{
		/// <summary>
		/// Holds settings for the Knuth solver
		/// </summary>
		public class KnuthSolverSettings : ICloneable
		{
			/// <summary>
			/// The maximum allowable pool size
			/// </summary>
			public int MaximumPoolSize = 4096;

			/// <see cref="ICloneable.Clone"/>
			public object Clone()
			{
				KnuthSolverSettings Result = new KnuthSolverSettings();
				Result.MaximumPoolSize = MaximumPoolSize;
				return Result;
			}
		}

		private KnuthSolverSettings Settings = new KnuthSolverSettings();

		/// <summary>
		/// The pool of all possible solutions
		/// </summary>
		private ConcurrentBag<RowState> Pool;

		/// <summary>
		/// Random generator for lookups
		/// </summary>
		private Random Generator = new Random();

		/// <summary>
		/// Initializes the pool with a seed guess
		/// </summary>
		/// <param name="Board">The board being used</param>
		/// <param name="SeedRows">The initial guess row index</param>
		private void FillPool(GameBoard Board, int SeedRows)
		{
			FillPool(Board, null, 0, SeedRows, GC.GetTotalMemory(true));
		}

		/// <summary>
		/// Set to true if the pool size was reached in a fill thread
		/// </summary>
		private volatile bool OOMTriggered = false;

		/// <summary>
		/// Initializes the pool with a seed guess
		/// </summary>
		/// <param name="Board">The board being used</param>
		/// <param name="Colors">The current color row being processed, used for recursion</param>
		/// <param name="Column">The current column in the color row, used for recursion</param>
		/// <param name="SeedRows">The initial guess row index</param>
		/// <param name="StartMemory">The inital GC memory load</param>
		private void FillPool(GameBoard Board, byte[] Colors, int Column, int SeedRows, long StartMemory)
		{
			if (Column == 0) //Initial case
			{
				//The pool seeding can be threaded.
				//A new thread is created with each color possibilty in column 1,
				//the function then recursivly fills the pool with all possibilities for each color in each column.

				Thread[] FillThreads = new Thread[Board.NumColors];
				OOMTriggered = false;

				for (int i = 0; i < Board.NumColors; i++)
				{
					byte[] ColorThread = new byte[Board.NumColumns];
					ColorThread[0] = (byte)i;

					FillThreads[i] = new Thread(new ThreadStart(delegate { FillPool(Board, ColorThread, Column + 1, SeedRows, StartMemory); }));
					FillThreads[i].Priority = ThreadPriority.BelowNormal;
					FillThreads[i].Start();
				}

				//Wait for each thread to finish
				for (int i = 0; i < FillThreads.Length; i++)
				{
					FillThreads[i].Join();
				}

				if (OOMTriggered)
					throw new OutOfMemoryException();
			}
			else if (Column == Board.NumColumns - 1) //terminal case
			{
				for (int c = 0; c < Board.NumColors; c++)
				{
					Colors[Column] = (byte)c;

					RowState NewMember = new RowState(Colors);

					bool Consistent = true;

					for (int r = 0; r < SeedRows; r++)
					{
						if (!IsConsistent(NewMember, Board.Guesses[r], Board))
						{
							Consistent = false;
							break;
						}
					}

					//If the current row is a possible solution, add it to the pool
					if (Consistent)
					{
						//TODO: Pool.Count seems very slow, find a faster solution
						if (Pool.Count % 20000 == 0) //Recheck memory usage every 20000 allocations
						{
							if ((GC.GetTotalMemory(false) - StartMemory) / 1048576 > Settings.MaximumPoolSize)
							{
								OOMTriggered = true;
								return;
							}
						}

						Pool.Add(NewMember);
					}
				}
			}
			else //intermediate case
			{
				//try each color in this column then recurse the next columns
				for (int c = 0; c < Board.NumColors && !OOMTriggered; c++)
				{
					Colors[Column] = (byte)c;
					
					//Early esc test
					if(Column == Colors.Length / 2)
					{
						bool Consistent = true;

						for (int r = 0; r < SeedRows; r++)
						{
							if (!EarlyIsConsistent(new RowState(Colors), Board.Guesses[r]))
							{
								Consistent = false;
								break;
							}
						}

						if (!Consistent)
							continue; //On a dead-end
					}

					FillPool(Board, Colors, Column + 1, SeedRows, StartMemory);
				}
			}
		}


		/// <summary>
		/// Create and fill the guess pool
		/// </summary>
		/// <param name="Board">The board being used</param>
		/// <param name="SeedRows">The initial guess row index</param>
		/// <returns></returns>
		private bool GeneratePool(GameBoard Board, int SeedRows)
		{
			//Fill the pool with recursion
			Pool = new ConcurrentBag<RowState>();

			try
			{
				FillPool(Board, SeedRows);
			}

			catch (System.OutOfMemoryException)
			{
				MessageBox.Show("Maximum pool size exceeded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Reset();
				return false;
			}

			return true;
		}

		private bool EarlyIsConsistent(RowState Colors, BoardRow Row)
		{
		int SamePosAndColor = 0, SameColor = 0;
		int UnknownFactor = (int)Math.Ceiling((double)Colors.Length / 2.0);

			for (int i = 0; i < Colors.Length / 2; i ++)
			{
				if (Colors[i] == Row.Row[i])
				{
					SamePosAndColor++;
				}
			}

			if(SamePosAndColor > Row.Score.NumCorrectSpot)
				return false;

			bool[] Matched = new bool[Colors.Length];

			for (int i = 0; i < Colors.Length / 2; i ++)
			{
				for (int j = 0; j < Colors.Length; j ++)
				{
					if ((Colors[i] == Row.Row[j]) && !Matched[j])
					{
						SameColor++;
						Matched[j] = true;
						break;
					}
				}
			}

			if (SameColor - SamePosAndColor > Row.Score.NumCorrectColor)
				return false;

			return true;
		}

		/// <summary>
		/// Checks if the row is a possible solution given the scored row
		/// </summary>
		/// <param name="Colors">The row to check</param>
		/// <param name="Row">A played row with score</param>
		/// <param name="Board">The game board in use.</param>
		/// <returns>True if the row can be a solution</returns>
		private bool IsConsistent(RowState Colors, BoardRow Row, GameBoard Board)
		{
		int SamePosAndColor = 0, SameColor = 0;

			//if (Colors == Board.Answer)
			//	System.Diagnostics.Debugger.Break();

			//Optimization, counting backwards is supposedly faster since a NZ test is faster than a less than
			for (int i = Colors.Length - 1; i >= 0; i--)
			{
				if (Colors[i] == Row.Row[i])
				{
					SamePosAndColor++;
				}
			}

			if (SamePosAndColor != Row.Score.NumCorrectSpot)
				return false;

		bool[] Matched = new bool[Colors.Length];

			for (int i = Colors.Length - 1; i >= 0; i--)
			{
				for (int j = Colors.Length - 1; j >= 0; j--)
				{
					if ((Colors[i] == Row.Row[j]) && !Matched[j])
					{
						SameColor++;
						Matched[j] = true;
						break;
					}
				}
			}

			if(SameColor - SamePosAndColor != Row.Score.NumCorrectColor)
				return false;

			return true;
		}

		/// <summary>
		/// Removes impossible guesses from the pool given new information
		/// </summary>
		/// <param name="Board">The board being used</param>
		private void Evolve(GameBoard Board)
		{
			BoardRow LastGuess = Board.Guesses.Last();
			ConcurrentBag<RowState> NewPool = new ConcurrentBag<RowState>();
			Thread[] EvalThreads = new Thread[Pool.Count > 50000 ? Environment.ProcessorCount : 1];

			for(int i = 0; i < EvalThreads.Length; i ++)
			{
				EvalThreads[i] = new Thread(new ThreadStart(delegate
				{
					RowState Row;

					while(Pool.TryTake(out Row))
					{
						if (IsConsistent(Row, LastGuess, Board))
						{
							NewPool.Add(Row);
						}
					}
				}));

				EvalThreads[i].Priority = ThreadPriority.BelowNormal;
				EvalThreads[i].Start();
			}

			//Wait for each thread to finish
			for (int i = 0; i < EvalThreads.Length; i++)
			{
				EvalThreads[i].Join();
			}

			Pool = NewPool;

			GC.Collect(); //The pool may be drastically smaller, so request a garbage collection
		}

		/// <summary>
		/// Picks a possible answer from the pool
		/// </summary>
		/// <returns>The possible answer to use</returns>
		private RowState ChooseGuess()
		{
			System.Diagnostics.Debug.Assert(Pool.Count > 0);

		//TODO: choose a better heuristic method
		int Index = Generator.Next(Pool.Count);

			return Pool.ElementAt(Index);
		}

		/// <see cref="Solver.GetGuess(GameBoard)"/>
		public RowState GetGuess(GameBoard Board)
		{
			//Use a seed guess
			if (Board.Guesses.Count < 2 || (Board.Guesses.Count == 2 && Board.NumColumns > 6 && Board.NumColors > 5))
				return SeedGuess.GetGuess(Board);

			if (Pool == null)
			{
				if (!GeneratePool(Board, Board.Guesses.Count))
					return new RowState(new byte[Board.NumColumns]);
			}
			else
			{
				Evolve(Board);
			}

			return ChooseGuess();
		}

		/// <see cref="Solver.Reset"/>
		public void Reset()
		{
			//Pool?.Clear();
			Pool = null;
			GC.Collect();
		}

		/// <see cref="Solver.ShowSettingsDialog"/>
		public void ShowSettingsDialog()
		{
			using (KnuthSettings SettDlg = new KnuthSettings())
			{
				SettDlg.Settings = (KnuthSolverSettings)Settings.Clone();
				System.Windows.Forms.DialogResult Res = SettDlg.ShowDialog();

				if (Res == System.Windows.Forms.DialogResult.OK)
				{
					Settings = SettDlg.Settings;
					Reset();
				}
			}
		}

		/// <see cref="Solver.GetMessage"/>
		public string GetMessage()
		{
			if (Pool != null)
			{
				return "Pool Size: " + Pool.Count.ToString("##,#");
			}
			else
			{
				return "";
			}
		}
	}
}
