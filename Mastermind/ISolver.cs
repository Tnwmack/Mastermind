using System;

namespace Mastermind
{
	/// <summary>
	/// This interface is implemented by game solvers
	/// </summary>
	interface ISolver
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
		/// Called when all long operations must stop immediatly
		/// </summary>
		void Abort();

		/// <summary>
		/// Gets a status message to display to the user
		/// </summary>
		event Action<string> OnStatusChange;
	}
}
