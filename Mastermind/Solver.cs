using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// This interface is implemented by game solvers
	/// </summary>
	interface Solver
	{
		/// <summary>
		/// Gets a new guess for the game
		/// </summary>
		/// <param name="Board">The board in use</param>
		/// <returns>A new guess that should be played</returns>
		RowState GetGuess(GameBoard Board);

		/// <summary>
		/// Opens a settings dialog for parameter tuning
		/// </summary>
		void ShowSettingsDialog();

		/// <summary>
		/// Called when the game has been reset
		/// </summary>
		void Reset();

		/// <summary>
		/// Gets a status message to display to the user
		/// </summary>
		/// <returns>The status to display</returns>
		string GetMessage();
	}
}
