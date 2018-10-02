using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// Generates generic seed guesses.
	/// </summary>
	class SeedGuess
	{
		/// <summary>
		/// Get a seed guess using a modified Knuth system.
		/// </summary>
		/// <param name="Board">The board in use.</param>
		/// <returns>A random guess.</returns>
		public static RowState GetGuess(GameBoard Board)
		{
		byte Index = (byte)Board.Guesses.Count;
		byte[] Guess = new byte[Board.NumColumns];
		int Split = Board.NumColumns / 2;

			Index *= 2;

			if (Index < Board.NumColors)
				for (int i = 0; i < Guess.Length; i++)
					Guess[i] = Index;

			if(Index + 1 < Board.NumColors )
				for (int i = Split; i < Board.NumColumns; i++)
					Guess[i] = (byte)(Index + 1);

			return new RowState(Guess);
		}
	}
}
