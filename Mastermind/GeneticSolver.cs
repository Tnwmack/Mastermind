using System;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace Mastermind
{
	/// <summary>
	/// A Mastermind solver that uses genetic algorithms.
	/// </summary>
	public class GeneticSolver : ISolver
	{
		/// <see cref="ISolver.OnStatusChange"/>
		public event Action<string> OnStatusChange;

		private volatile bool AbortProcessing = false;

		//Tuning parameters
		private GeneticSolverSettings Settings = new GeneticSolverSettings();

		/// <summary>
		/// Genetic solver constructor
		/// </summary>
		public GeneticSolver()
		{

		}

		private GeneticAlgorithm<GeneticSolverGuess, GeneticSolverGuessFactory> Solver;

		private void UpdateMessage(int Generation)
		{
			//Calculate some diagnostic information
			long PoolScore = 0;

			foreach(GeneticAlgorithmPoolMember<GeneticSolverGuess> G in Solver)
			{
				PoolScore += G.Score;
			}

			int AveragePoolScore = (int)(PoolScore / Solver.PoolSize);

			PoolScore = 0;

			for (int i = 0; i < Settings.ElitismCutoff; i++)
			{
				PoolScore += Solver.GetPoolItem(i).Score;
			}

			int ElitePoolScore = (int)(PoolScore / Settings.ElitismCutoff);

			OnStatusChange?.Invoke(string.Format(CultureInfo.CurrentCulture, "Best: {0}, Elite: {1}, Pool: {2}, Generations: {3}",
				Solver.GetPoolItem(0).Score, ElitePoolScore, AveragePoolScore, Generation));
		}

		/// <summary>
		/// Performs the selection, crossover and mutation operations on the pool
		/// </summary>
		private void Evolve()
		{
			//Score and sort using new info from the last guess
			Solver.ScoreAndSortPool();

			//Keep evolving until a possible solution is found (with at least one generation), or a max number of generations have occurred

			int Generation = 0;
			do
			{
				Solver.Evolve(Settings.ElitismCutoff, Settings.CrossoverAmount, Settings.MutationRate);
				Solver.ScoreAndSortPool();
				UpdateMessage(Generation + 1);
				Generation++;
			} while (Generation < Settings.MaxGenerations && Solver.GetPoolItem(0).Score != 0 && !AbortProcessing);
		}

		/// <see cref="ISolver.GetGuess(GameBoard)"/>
		public RowState GetGuess(GameBoard Board)
		{
			Contract.Requires(Board != null);

			//Do a seed guess
			if (Board.Guesses.Count < 2 || (Board.Guesses.Count == 2 && Board.NumColumns > 6 && Board.NumColors > 5))
				return SeedGuess.GetGuess(Board);

			if(Solver == null)
			{
				Solver = new GeneticAlgorithm<GeneticSolverGuess, GeneticSolverGuessFactory>(new GeneticSolverGuessFactory(Board, Settings));
				Solver.GeneratePool(Settings.PoolSize);
			}

			//Perform pool evolution
			Evolve();

			//Skip guesses that have already been made
			foreach(GeneticAlgorithmPoolMember<GeneticSolverGuess> G in Solver)
			{
				if (!Board.Guesses.Exists(m => m.Row == G.Item.GuessState))
				{
					return G.Item.GuessState;
				}
			}

			return new RowState(new byte[Board.NumColumns]);
		}

		/// <see cref="ISolver.ShowSettingsDialog"/>
		public void ShowSettingsDialog()
		{
			using (GeneticSettings SettDlg = new GeneticSettings((GeneticSolverSettings)Settings.Clone()))
			{
				System.Windows.Forms.DialogResult Res = SettDlg.ShowDialog();

				if (Res == System.Windows.Forms.DialogResult.OK)
				{
					Settings = SettDlg.Settings;
					Reset();
				}
			}
		}

		/// <see cref="ISolver.Reset"/>
		public void Reset()
		{
			Solver = null;
			AbortProcessing = false;
		}

		/// <see cref="ISolver.Abort"/>
		public void Abort()
		{
			AbortProcessing = true;
		}
	}
}
