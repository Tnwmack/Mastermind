using System;
using System.Linq;

namespace Mastermind
{
	/// <summary>
	/// A solver implementation that attempts to merge the genetic algorithm and Knuth solvers.
	/// </summary>
	class HybridSolver : ISolver
	{
		/// <see cref="ISolver.OnStatusChange"/>
		public event Action<string> OnStatusChange;

		private readonly Action<string> OnSetMessageDelegate; 

		private readonly GeneticSolver GSolver = new GeneticSolver();
		private readonly KnuthSolver KSolver = new KnuthSolver();

		private bool InGeneticMode = true;

		/// <summary>
		/// HybridSolver constructor.
		/// </summary>
		public HybridSolver()
		{
			OnSetMessageDelegate = new Action<string>(OnSetMessage);
		}

		/// <see cref="ISolver.GetGuess(GameBoard)"/>
		public RowState GetGuess(GameBoard Board)
		{
			if(Board.Guesses.Count == 0)
			{
				return GSolver.GetGuess(Board);
			}
			
			//TODO: Make the crossover point configurable
			if(Board.Guesses.Last().Score.NumCorrectSpot >= (Board.NumColumns - 4))
			{
				InGeneticMode = false;
				GSolver.Reset();
				GSolver.OnStatusChange -= OnSetMessageDelegate;
				KSolver.OnStatusChange += OnSetMessageDelegate;
			}

			if(InGeneticMode)
			{
				return GSolver.GetGuess(Board);
			}
			else
			{
				return KSolver.GetGuess(Board);
			}
		}

		public void OnSetMessage(string Message)
		{
			if(InGeneticMode)
			{
				OnStatusChange?.Invoke("Genetic Mode: " + Message);
			}
			else
			{
				OnStatusChange?.Invoke("Knuth Mode: " + Message);
			}
		}

		/// <see cref="ISolver.Reset"/>
		public void Reset()
		{
			GSolver.Reset();
			KSolver.Reset();

			InGeneticMode = true;
			KSolver.OnStatusChange -= OnSetMessageDelegate;
			GSolver.OnStatusChange -= OnSetMessageDelegate;
			GSolver.OnStatusChange += OnSetMessageDelegate;
		}

		/// <see cref="ISolver.Abort"/>
		public void Abort()
		{
			if (InGeneticMode)
			{
				GSolver.Abort();
			}
			else
			{
				KSolver.Abort();
			}
		}

		/// <see cref="ISolver.ShowSettingsDialog"/>
		public void ShowSettingsDialog()
		{
			//TODO: Does this make sense? Maybe make a new dlg
			if (InGeneticMode)
			{
				GSolver.ShowSettingsDialog();
			}
			else
			{
				KSolver.ShowSettingsDialog();
			}
		}
	}
}
