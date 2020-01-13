using System;

namespace Mastermind
{
	/// <summary>
	/// Holds settings for the solver
	/// </summary>
	public class GeneticSolverSettings : ICloneable
	{
		/// <summary>
		/// The size of the gene pool
		/// </summary>
		public int PoolSize { get; set; } = 500;

		/// <summary>
		/// The number of crossovers to perform (pool*CrossoverAmount), 
		/// remainder are elites and mutations
		/// </summary>
		public float CrossoverAmount { get; set; } = 0.7f;

		/// <summary>
		/// The rate that colors are mutated (columns*MutationRate) mutations
		/// </summary>
		public float MutationRate { get; set; } = 0.05f;

		/// <summary>
		/// The number top scoring pool members that do not 
		/// undergo crossovers or mutation
		/// </summary>
		public int ElitismCutoff { get; set; } = 20;

		/// <summary>
		/// The penalty score for correct color and spot discrepancies
		/// </summary>
		public int MatchScore { get; set; } = 50;

		/// <summary>
		/// The penalty score for correct color but incorrect spot discrepancies
		/// </summary>
		public int PartialMatchScore { get; set; } = 20;

		/// <summary>
		/// The maximum number of generations before forcing a guess
		/// </summary>
		public int MaxGenerations { get; set; } = 40;

		/// <see cref="ICloneable.Clone"/>
		public object Clone()
		{
			return new GeneticSolverSettings
			{
				PoolSize = PoolSize,
				CrossoverAmount = CrossoverAmount,
				MutationRate = MutationRate,
				ElitismCutoff = ElitismCutoff,
				MatchScore = MatchScore,
				PartialMatchScore = PartialMatchScore,
				MaxGenerations = MaxGenerations
			};
		}
	}
}
