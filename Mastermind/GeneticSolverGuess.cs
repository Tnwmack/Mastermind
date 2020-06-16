using System;
using System.Diagnostics.Contracts;

namespace Mastermind
{
	/// <summary>
	/// Implementation of IGeneticItem that evaluates a Mastermind row state.
	/// </summary>
	public class GeneticSolverGuess : IGeneticItem, ICloneable
	{
		private readonly GameBoard Board;
		private readonly int MatchScore;
		private readonly int PartialMatchScore;

		/// <summary>
		/// The guess being evaluated.
		/// </summary>
		public RowState GuessState { get; }

		/// <summary>
		/// GeneticSolverGuess constructor.
		/// </summary>
		/// <param name="Board">The board being used.</param>
		/// <param name="MatchScore">Score used when a hard match (same color and place) is found.</param>
		/// <param name="PartialMatchScore">Score used when a soft match (same color, different place) is found.</param>
		/// <param name="State"><see cref="GuessState"/></param>
		public GeneticSolverGuess(GameBoard Board, int MatchScore, int PartialMatchScore, RowState State)
		{
			Contract.Requires(Board != null);

			this.Board = Board;
			this.MatchScore = MatchScore;
			this.PartialMatchScore = PartialMatchScore;
			this.GuessState = State;
		}

		/// <summary>
		/// Score the row possibility compared to the given guess
		/// </summary>
		/// <param name="PlayedRow">The previously scored row</param>
		/// <returns>0 if the row is a possible fit, decreasing values the worse the fit is</returns>
		private int CompareRows(BoardRow PlayedRow)
		{
			int NumColumns = Board.NumColumns;

		//Copying the rows to the stack before using them is a fair amount faster (cache efficiency?)
		Span<bool> RowColumnMatched = stackalloc bool[NumColumns];
		Span<bool> GuessColumnMatched = stackalloc bool[NumColumns];

		Span<byte> GuessStateColumns = stackalloc byte[NumColumns];
		Span<byte> PlayedRowColumns = stackalloc byte[NumColumns];

			for(int i = 0; i < NumColumns; i ++)
			{
				GuessStateColumns[i] = GuessState[i];
				PlayedRowColumns[i] = PlayedRow.Row[i];
			}

		int SamePosAndColor = 0, SameColor = 0;
		int Score = 0;

			//Score reds
			for (int i = 0; i<NumColumns; i++)
			{
				if (GuessStateColumns[i] == PlayedRowColumns[i])
				{
					SamePosAndColor++;
					RowColumnMatched[i] = GuessColumnMatched[i] = true;
				}
}

		int MatchDifference = Math.Abs(PlayedRow.Score.NumCorrectSpot - SamePosAndColor);
			Score -= MatchScore* MatchDifference;

			//Score whites
			for (int i = 0; i<NumColumns; i++)
			{
				for (int j = 0; !RowColumnMatched[i] && j<NumColumns; j++)
				{
					if (!GuessColumnMatched[j] && (GuessStateColumns[i] == PlayedRowColumns[j]))
					{
						SameColor++;
						GuessColumnMatched[j] = true;
						break;
					}
				}
			}

		int ColorDifference = Math.Abs(PlayedRow.Score.NumCorrectColor - SameColor);
			Score -= PartialMatchScore* ColorDifference;

			return Score;
		}

		/// <summary>
		/// Score a row based on all previous guesses
		/// </summary>
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

		/// <see cref="IGeneticItem.GetScore"/>
		public int GetScore()
		{
			return EvalRow();
		}

		/// <see cref="ICloneable.Clone"/>
		public object Clone()
		{
			return new GeneticSolverGuess(Board, MatchScore, PartialMatchScore, GuessState);
		}
	}
}
