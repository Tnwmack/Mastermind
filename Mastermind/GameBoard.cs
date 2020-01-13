using System;
using System.Collections.Generic;

namespace Mastermind
{
	/// <summary>
	/// Manages the game board
	/// </summary>
	public class GameBoard
	{
		/// <summary>
		/// Called when the game board has changed (eg, a guess was added).
		/// </summary>
		public event Action<GameBoard> OnBoardChanged;

		/// <summary>
		/// Call when the game state has changed (eg, the game was won).
		/// </summary>
		public event Action<GameBoard> OnGameStateChanged;

		/// <summary>
		/// The currently selected answer key
		/// </summary>
		public RowState Answer { get; set; }

		/// <summary>
		/// The entered guesses
		/// </summary>
		public List<BoardRow> Guesses { get; }

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

		private GameState currentGameState = GameState.InProgress;

		/// <summary>
		/// The current state of the game
		/// </summary>
		public GameState CurrentGameState
		{
			get { return currentGameState; }

			private set
			{
				currentGameState = value;
				OnGameStateChanged?.Invoke(this);
			}
		}

		/// <summary>
		/// The number of colors in play
		/// </summary>
		public int NumColors { get; }

		/// <summary>
		/// The number of columns in play
		/// </summary>
		public int NumColumns { get; }

		/// <summary>
		/// The number of rows in play
		/// </summary>
		public int NumRows { get; }

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
			NumColors = numColors;
			NumColumns = numColumns;
			NumRows = numRows;
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

			//Match correct color
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
			OnBoardChanged?.Invoke(this);

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

		/// <summary>
		/// Clears the board and resets the game state to in progress
		/// </summary>
		public void Reset()
		{
			Guesses.Clear();
			CurrentGameState = GameState.InProgress;
			OnBoardChanged?.Invoke(this);
		}

	}
}
