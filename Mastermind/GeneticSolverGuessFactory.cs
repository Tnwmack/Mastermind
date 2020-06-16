using System;
using System.Diagnostics.Contracts;

namespace Mastermind
{
	/// <summary>
	/// Implementation of IGeneticItemFactory that creates and modifies Mastermind row guesses.
	/// </summary>
	public class GeneticSolverGuessFactory : IGeneticItemFactory<GeneticSolverGuess>
	{
		private readonly GameBoard Board;
		private readonly GeneticSolverSettings Settings;
		private readonly Random RandGenerator = new Random();

		/// <summary>
		/// GeneticSolverGuessFactory constructor.
		/// </summary>
		/// <param name="Board">The board being used.</param>
		/// <param name="Settings">Genetic solver tuning parameters.</param>
		public GeneticSolverGuessFactory(GameBoard Board, GeneticSolverSettings Settings)
		{
			Contract.Requires(Board != null);

			this.Board = Board;
			this.Settings = Settings;
		}

		/// <see cref="IGeneticItemFactory{GeneticSolverGuess}.GetRandom"/>
		public GeneticSolverGuess GetRandom()
		{
			return new GeneticSolverGuess(Board, Settings.MatchScore, Settings.PartialMatchScore,
				RowState.GetRandomColors(RandGenerator, Board.NumColors, Board.NumColumns));
		}

		/// <see cref="IGeneticItemFactory{GeneticSolverGuess}.Cross"/>
		public void Cross(GeneticSolverGuess A, GeneticSolverGuess B, out GeneticSolverGuess ResultA, out GeneticSolverGuess ResultB)
		{
			Contract.Requires(A != null);
			Contract.Requires(B != null);

			Span<byte> NewColorA = stackalloc byte[Board.NumColumns];
			Span<byte> NewColorB = stackalloc byte[Board.NumColumns];

			int SplitIndex = RandGenerator.Next(Board.NumColumns);

			for (int i = 0; i < SplitIndex; i++)
			{
				NewColorA[i] = A.GuessState[i];
				NewColorB[i] = B.GuessState[i];
			}

			for (int i = SplitIndex; i < Board.NumColumns; i++)
			{
				NewColorA[i] = B.GuessState[i];
				NewColorB[i] = A.GuessState[i];
			}

			ResultA = new GeneticSolverGuess(Board, Settings.MatchScore, Settings.PartialMatchScore, new RowState(NewColorA));
			ResultB = new GeneticSolverGuess(Board, Settings.MatchScore, Settings.PartialMatchScore, new RowState(NewColorB));
		}

		/// <see cref="IGeneticItemFactory{GeneticSolverGuess}.Mutate"/>
		public GeneticSolverGuess Mutate(GeneticSolverGuess Item)
		{
			Contract.Requires(Item != null);

			Span<byte> Colors = stackalloc byte[Board.NumColumns];
			Span<bool> Selected = stackalloc bool[Board.NumColumns];

			for (int i = 0; i < Board.NumColumns; i++)
				Colors[i] = Item.GuessState[i];

			int ColumnsToMutate = RandGenerator.Next(1, Board.NumColumns / 2 + 1);

			do
			{
				int i = RandGenerator.Next(Board.NumColumns);

				if (!Selected[i])
				{
					Selected[i] = true;
					Colors[i] = (byte)RandGenerator.Next(Board.NumColors);
				}
				else
					continue;
			} while (--ColumnsToMutate > 0);

			return new GeneticSolverGuess(Board, Settings.MatchScore, Settings.PartialMatchScore, new RowState(Colors));
		}
	}
}
