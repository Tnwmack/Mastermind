using System;
using System.Windows.Forms;

namespace Mastermind
{
	class RandomSolver : Solver
	{
		public Random Generator = new Random();

		public RowState GetGuess(GameBoard Board)
		{
			return RowState.GetRandomColors(Generator, Board.NumColors, Board.NumColumns);
		}

		public void ShowSettingsDialog()
		{
			MessageBox.Show("No settings for this solver.", "Random Guess", 
				MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void Reset()
		{

		}

		public string GetMessage()
		{
			return "";
		}
	}
}
