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
		private LinkedList<RowState> Pool;

		private Random Generator = new Random();

		private void FillPool(GameBoard Board, BoardRow SeedGuess)
		{
			FillPool(Board, null, 0, SeedGuess);
		}

		private void FillPool(GameBoard Board, byte[] Colors, int Column, BoardRow SeedGuess)
		{
			if(Column == 0)
			{
			Thread[] FillThreads = new Thread[Board.NumColors];

				for (int i = 0; i < Board.NumColors; i ++)
				{
				byte[] ColorThread = new byte[Board.NumColumns];
					ColorThread[0] = (byte)i;

					FillThreads[i] = new Thread(new ThreadStart(delegate { FillPool(Board, ColorThread, Column + 1, SeedGuess); }));
					FillThreads[i].Priority = ThreadPriority.BelowNormal;
					FillThreads[i].Start();
				}

				for (int i = 0; i < FillThreads.Length; i++)
				{
					FillThreads[i].Join();
				}
			}
			else if (Column == Board.NumColumns - 1)
			{
				for (int c = 0; c < Board.NumColors; c++)
				{
					Colors[Column] = (byte)c;

				RowState NewMember = new RowState(Colors);

					if (IsConsistent(NewMember, SeedGuess))
					{
						//This lock is kind of slow, but multiple pools isn't faster
						lock (Pool)
						{
							Pool.AddLast(NewMember);
						}
					}
				}
			}
			else
			{
				for (int c = 0; c < Board.NumColors; c++)
				{
					Colors[Column] = (byte)c;
					FillPool(Board, Colors, Column + 1, SeedGuess);
				}
			}
		}

		private bool GeneratePool(GameBoard Board, BoardRow SeedGuess)
		{
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

			Pool = new LinkedList<RowState>();
			FillPool(Board, SeedGuess);

			return true;
		}


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

		private void Evolve(GameBoard Board)
		{
		LinkedListNode<RowState> Node = Pool.First;
		BoardRow LastGuess = Board.Guesses.Last();

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

			GC.Collect(); //The pool may be much smaller, so request a garbage collection
		}

		private RowState ChooseGuess()
		{
		//TODO: choose a better heuristic method
		int Index = Generator.Next(Pool.Count);

			return Pool.ElementAt(Index);
		}

		public RowState GetGuess(GameBoard Board)
		{
			if(Board.Guesses.Count == 0)
			{
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

		public void Reset()
		{
			Pool?.Clear();
			Pool = null;
			GC.Collect();
		}

		public void ShowSettingsDialog()
		{
			MessageBox.Show("No settings for this solver.", "Knuth Solver",
				MessageBoxButtons.OK, 
				MessageBoxIcon.Information);
		}

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
