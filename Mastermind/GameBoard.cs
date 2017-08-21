using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// Manages the game board
	/// </summary>
	public class GameBoard
	{
		private int numColors;
		private int numColumns;
		private int numRows;
		//private int currentRow = 0;

		/// <summary>
		/// The currently selected answer key
		/// </summary>
		public RowState Answer;

		/// <summary>
		/// The entered guesses
		/// </summary>
		public List<BoardRow> Guesses;

		/// <summary>
		/// States the game can be in
		/// </summary>
		public enum GameState
		{
			/// <summary>
			/// Guesses can be entered
			/// </summary>
			InProgress,

			/// <summary>
			/// The game was won
			/// </summary>
			Won,

			/// <summary>
			/// The game was lost
			/// </summary>
			Lost,
		}

		/// <summary>
		/// The current state of the game
		/// </summary>
		public GameState CurrentGameState = GameState.InProgress;

		/// <summary>
		/// The number of colors in play
		/// </summary>
		public int NumColors { get { return numColors; } }

		/// <summary>
		/// The number of columns in play
		/// </summary>
		public int NumColumns { get { return numColumns; } }

		/// <summary>
		/// The number of rows in play
		/// </summary>
		public int NumRows { get { return numRows; } }

		//public int CurrentRow { get { return currentRow; } }

		/// <summary>
		/// Gameboard constructor
		/// </summary>
		/// <param name="numColors">Number of colors used</param>
		/// <param name="numColumns">Number of columns to use</param>
		/// <param name="numRows">Number of rows in the game</param>
		/// <param name="Answer">The answer to use</param>
		public GameBoard(int numColors, int numColumns, int numRows, RowState Answer)
		{
			this.numColors = numColors;
			this.numColumns = numColumns;
			this.numRows = numRows;
			this.Answer = Answer;

			Guesses = new List<BoardRow>();
		}

		/// <summary>
		/// Calculate and generate the row score
		/// </summary>
		/// <param name="State">The row to score</param>
		/// <param name="Answer">The answer key</param>
		public static RowScore ScoreRow(RowState State, RowState Answer)
		{
		int Correct = 0, CorrectColor = 0;
		bool[] AnswerColumnMatched = new bool[State.Length];
		bool[] GuessColumnMatched = new bool[State.Length];

			//Match correct place and color
			for (int i = 0; i < State.Length; i++)
			{
				if (State[i] == Answer[i])
				{
					Correct++;
					AnswerColumnMatched[i] = true;
					GuessColumnMatched[i] = true;
				}
			}

			//Math correct color
			for (int i = 0; i < State.Length; i++)
			{
				for (int j = 0; j < State.Length; j++)
				{
					if ((State[i] == Answer[j]) &&
						!AnswerColumnMatched[j] &&
						!GuessColumnMatched[i])
					{
						AnswerColumnMatched[j] = true;
						GuessColumnMatched[i] = true;
						CorrectColor++;
						break; //Stop matching this color
					}
				}

			}

			return new RowScore(Correct, CorrectColor);
		}

		/// <summary>
		/// Add a guess to the game board
		/// </summary>
		/// <param name="Guess">The guess to add</param>
		/// <returns>True if the guess was added</returns>
		public bool AddGuess(RowState Guess)
		{
			if (CurrentGameState != GameState.InProgress)
				return false;

		BoardRow NewRow = new BoardRow();

			NewRow.Row = Guess;
			NewRow.Score = ScoreRow(Guess, Answer);

			Guesses.Add(NewRow);

			if (NewRow.Score.NumCorrectSpot == NumColumns)
			{
				CurrentGameState = GameState.Won;
				return true;
			}

			if (Guesses.Count == NumRows)
			{
				CurrentGameState = GameState.Lost;
			}

			return true;
		}

	}
}
