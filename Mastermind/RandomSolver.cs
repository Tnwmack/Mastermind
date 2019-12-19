using System;
using System.Windows.Forms;

namespace Mastermind
{
	/// <summary>
	/// Generates totally random guesses, mainly used for testing
	/// </summary>
	class RandomSolver : Solver
	{
		public event Action<string> SetMessage;
		private Random Generator = new Random();

		/// <see cref="Solver.GetGuess"/>
		public RowState GetGuess(GameBoard Board)
		{
			return RowState.GetRandomColors(Generator, Board.NumColors, Board.NumColumns);
		}

		/// <see cref="Solver.ShowSettingsDialog"/>
		public void ShowSettingsDialog()
		{
			MessageBox.Show("No settings for this solver.", "Random Guess", 
				MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <see cref="Solver.Reset"/>
		public void Reset()
		{

		}

		/// <see cref="Solver.Abort"/>
		public void Abort()
		{

		}
	}
}
