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

			RowColumnMatched = new bool[Board.NumColumns]; //TODO: Board.NumColumns looks off here
			GuessColumnMatched = new bool[Board.NumColumns];
		}

		private readonly bool[] RowColumnMatched;
		private readonly bool[] GuessColumnMatched;

		/// <summary>
		/// Score the row possibility compared to the given guess
		/// </summary>
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
