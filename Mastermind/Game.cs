using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	class Game
	{
		public GameBoard Board;
		public Solver AI;

		public Game(GameBoard Board, Solver AI)
		{
			this.Board = Board;
			this.AI = AI;
		}

		/// <summary>
		/// Make the AI generate a guess and add it to the board
		/// </summary>
		/// <returns>The guess added, or null if no guess could be made</returns>
		public RowState ? GenerateGuess()
		{
			if(Board.CurrentGameState == GameBoard.GameState.InProgress)
			{
			RowState RS = AI.GetGuess(Board);

				if (Board.AddGuess(RS))
					return RS;
				else
					return null;
			}
			else
			{
				return null;
			}
		}

		public void Reset()
		{
			AI.Reset();
		}
	}
}
