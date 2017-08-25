using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	class HybridSolver : Solver
	{
		private GeneticSolver GSolver = new GeneticSolver();
		private KnuthSolver KSolver = new KnuthSolver();

		private bool InGeneticMode = true;

		/// <see cref="Solver.GetGuess(GameBoard)"/>
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

		/// <see cref="Solver.GetMessage"/>
		public string GetMessage()
		{
			if(InGeneticMode)
			{
				return "Genetic Mode: " +  GSolver.GetMessage();
			}
			else
			{
				return "Knuth Mode: " + KSolver.GetMessage();
			}
		}

		/// <see cref="Solver.Reset"/>
		public void Reset()
		{
			GSolver.Reset();
			KSolver.Reset();

			InGeneticMode = true;
		}

		/// <see cref="Solver.ShowSettingsDialog"/>
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
