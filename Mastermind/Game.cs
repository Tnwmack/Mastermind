using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	class Game
	{
		public enum SolverType
		{
			Knuth,
			Genetic,
			Hybrid,
			Random,
		}

		public GameBoard Board { get; private set; }
		public ISolver AI { get; private set; }
		public BoardSettings Settings { get; private set; } = new BoardSettings();

		public event Action<string> OnAIStatusChange;

		public Game()
		{
			
		}

		/// <summary>
		/// Make the AI generate a guess and add it to the board
		/// </summary>
		/// <returns>The guess added, or null if no guess could be made</returns>
		public BoardRow ? AddGuessFromAI()
		{
			if (Board == null || AI == null)
				throw new InvalidOperationException();

			if(Board.CurrentGameState == GameBoard.GameState.InProgress)
			{
			RowState RS = AI.GetGuess(Board);

				if (Board.AddGuess(RS))
					return Board.Guesses.Last();
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
			AI?.Reset();
			Board?.Reset();
		}

		public void SetAI(SolverType Solver)
		{
			if(AI != null)
				AI.OnStatusChange -= AI_OnStatusChange;

			switch(Solver)
			{
				case SolverType.Knuth:
					AI = new KnuthSolver();
					break;

				case SolverType.Genetic:
					AI = new GeneticSolver();
					break;

				case SolverType.Hybrid:
					AI = new HybridSolver();
					break;

				case SolverType.Random:
					AI = new RandomSolver();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			AI.OnStatusChange += AI_OnStatusChange;
			AI.Reset();
		}

		public void SetBoardSettings(BoardSettings Settings)
		{
			this.Settings = (BoardSettings)Settings.Clone();
			Reset();
		}

		public void GenerateBoard(RowState Answer)
		{
			Board = new GameBoard(Settings.Colors, Settings.Columns, Settings.Rows, Answer);
			Reset();
		}

		public void ShowAISettingsDialog()
		{
			AI?.ShowSettingsDialog();
		}

		/// <summary>
		/// Rebroadcasts status messages from the AI solvers.
		/// </summary>
		/// <param name="Status">Status message</param>
		private void AI_OnStatusChange(string Status)
		{
			OnAIStatusChange?.Invoke(Status);
		}
	}
}
