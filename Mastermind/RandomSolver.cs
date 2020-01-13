using System;
using System.Windows.Forms;

namespace Mastermind
{
	/// <summary>
	/// Generates totally random guesses, mainly used for testing
	/// </summary>
	class RandomSolver : ISolver
	{
		/// <see cref="ISolver.OnStatusChange"/>
		public event Action<string> OnStatusChange;

		private Random Generator = new Random();

		/// <see cref="ISolver.GetGuess"/>
		public RowState GetGuess(GameBoard Board)
		{
			OnStatusChange?.Invoke("");
			return RowState.GetRandomColors(Generator, Board.NumColors, Board.NumColumns);
		}

		/// <see cref="ISolver.ShowSettingsDialog"/>
		public void ShowSettingsDialog()
		{
			MessageBox.Show(Properties.Resources.RandomSolver_NoSettings, Properties.Resources.Notice, 
				MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <see cref="ISolver.Reset"/>
		public void Reset()
		{

		}

		/// <see cref="ISolver.Abort"/>
		public void Abort()
		{

		}
	}
}
