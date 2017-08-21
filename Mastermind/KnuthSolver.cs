using System;
using System.Collections.Generic;
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
		/// The pool of all possible solutions
		/// </summary>
		private LinkedList<RowState> Pool;

		/// <summary>
		/// Random generator for lookups
		/// </summary>
		private Random Generator = new Random();

		/// <summary>
		/// Initializes the pool with a seed guess
		/// </summary>
		/// <param name="Board">The board being used</param>
		/// <param name="SeedGuess">The initial guess</param>
		private void FillPool(GameBoard Board, BoardRow SeedGuess)
		{
			FillPool(Board, null, 0, SeedGuess);
		}

		/// <summary>
		/// Initializes the pool with a seed guess
		/// </summary>
		/// <param name="Board">The board being used</param>
		/// <param name="Colors">The current color row being processed, used for recursion</param>
		/// <param name="Column">The current column in the color row, used for recursion</param>
		/// <param name="SeedGuess">The initial guess</param>
		private void FillPool(GameBoard Board, byte[] Colors, int Column, BoardRow SeedGuess)
		{
			if(Column == 0) //Initial case
			{
			//The pool seeding can be threaded.
			//A new thread is created with each color possibilty in column 1,
			//the function then recursivly fills the pool with all possibilities for each color in each column.

			Thread[] FillThreads = new Thread[Board.NumColors];

				for (int i = 0; i < Board.NumColors; i ++)
				{
				byte[] ColorThread = new byte[Board.NumColumns];
					ColorThread[0] = (byte)i;

					FillThreads[i] = new Thread(new ThreadStart(delegate { FillPool(Board, ColorThread, Column + 1, SeedGuess); }));
					FillThreads[i].Priority = ThreadPriority.BelowNormal;
					FillThreads[i].Start();
				}

				//Wait for each thead to finish
				for (int i = 0; i < FillThreads.Length; i++)
				{
					FillThreads[i].Join();
				}
			}
			else if (Column == Board.NumColumns - 1) //terminal case
			{
				for (int c = 0; c < Board.NumColors; c++)
				{
					Colors[Column] = (byte)c;

				RowState NewMember = new RowState(Colors);

					//If the current row is a possible solution, add it to the pool
					if (IsConsistent(NewMember, SeedGuess))
					{
						//This lock is kind of slow, but using multiple 
						//pools with a final merge doesn't seem to be faster.
						lock (Pool)
						{
							Pool.AddLast(NewMember);
						}
					}
				}
			}
			else //intermediate case
			{
				//try each color in this column then recurse the next columns
				for (int c = 0; c < Board.NumColors; c++)
				{
					Colors[Column] = (byte)c;
					FillPool(Board, Colors, Column + 1, SeedGuess);
				}
			}
		}

		/// <summary>
		/// Create and fill the guess pool
		/// </summary>
		/// <param name="Board">The board being used</param>
		/// <param name="SeedGuess">The initial guess</param>
		/// <returns></returns>
		private bool GeneratePool(GameBoard Board, BoardRow SeedGuess)
		{
		//Get a rough estimate of the final pool size
		//Since the pool is only filled with possible solutions, it's hard to guess the final pool size.
		//Colors^(Columns-1) seems close enough in testing.
		byte[] TempColors = new byte[Board.NumColumns];
		long StartBytes = GC.GetTotalMemory(true);
		LinkedListNode<RowState> Temp = new LinkedListNode<RowState>(new RowState(TempColors));
		long EndBytes = GC.GetTotalMemory(true);
		long PoolSize = (long)Math.Pow(Board.NumColors, Board.NumColumns - 1) * (EndBytes - StartBytes);

			PoolSize /= 1024;
			PoolSize /= 1024;

			if (PoolSize > 250)
			{
				if (MessageBox.Show("Pool Size may be " + PoolSize.ToString() + " MB\r\nContinue?",
					"Pool Size Warning",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Information) == DialogResult.No)
				{
					return false;
				}
			}

			//Fill the pool with recursion
			Pool = new LinkedList<RowState>();
			FillPool(Board, SeedGuess);

			return true;
		}

		/// <summary>
		/// Checks if the row is a possible solution given the scored row
		/// </summary>
		/// <param name="Colors">The row to check</param>
		/// <param name="Row">A played row with score</param>
		/// <returns>True if the row can be a solution</returns>
		private bool IsConsistent(RowState Colors, BoardRow Row)
		{
		int SamePosAndColor = 0, SameColor = 0;

			/*if (Colors == Board.Answer)
				System.Diagnostics.Debugger.Break();*/

			for (int i = 0; i < Colors.Length; i++)
			{
				if (Colors[i] == Row.Row[i])
				{
					SamePosAndColor++;
				}
			}

			if (SamePosAndColor != Row.Score.NumCorrectSpot)
				return false;

		bool[] Matched = new bool[Colors.Length];

			for (int i = 0; i < Colors.Length; i++)
			{
				for (int j = 0; j < Colors.Length; j++)
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
		LinkedListNode<RowState> Node = Pool.First;
		BoardRow LastGuess = Board.Guesses.Last();

			//manually enumerate the pool for easier removal
			while (Node != null)
			{
			LinkedListNode<RowState> Next = Node.Next;

				if (!IsConsistent(Node.Value, LastGuess))
				{
					System.Diagnostics.Debug.Assert(Node.Value != Board.Answer);
					Pool.Remove(Node); //O(1) operation
				}

				Node = Next;
			}

			GC.Collect(); //The pool may be drastically smaller, so request a garbage collection
		}

		/// <summary>
		/// Picks a possible answer from the pool
		/// </summary>
		/// <returns>The possible answer to use</returns>
		private RowState ChooseGuess()
		{
		//TODO: choose a better heuristic method
		int Index = Generator.Next(Pool.Count);

			return Pool.ElementAt(Index);
		}

		/// <see cref="Solver.GetGuess(GameBoard)"/>
		public RowState GetGuess(GameBoard Board)
		{
			if(Board.Guesses.Count == 0)
			{
			//This guess pattern is recommended by Knuth
			byte[] Guess = new byte[Board.NumColumns];
			int Split = Board.NumColumns / 2;

				for (int i = Split; i < Board.NumColumns; i++)
					Guess[i] = 1;

				return new RowState(Guess);
			}

			if (Pool == null)
			{
				if (!GeneratePool(Board, Board.Guesses.Last()))
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
			Pool?.Clear();
			Pool = null;
			GC.Collect();
		}

		/// <see cref="Solver.ShowSettingsDialog"/>
		public void ShowSettingsDialog()
		{
			MessageBox.Show("No settings for this solver.", "Knuth Solver",
				MessageBoxButtons.OK, 
				MessageBoxIcon.Information);
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
