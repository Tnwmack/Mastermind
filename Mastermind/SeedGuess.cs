using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	class SeedGuess
	{
		public static RowState GetGuess(GameBoard Board, byte Index)
		{
		//This guess pattern is recommended by Knuth
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
