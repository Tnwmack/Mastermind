namespace Mastermind
{
	/// <summary>
	/// Interface for items that can be used for a genetic alogrithm.
	/// </summary>
	public interface IGeneticItem 
	{
		/// <summary>
		/// Calculates the fitness evaluation score.
		/// </summary>
		/// <returns>Score of this item (more negative is a worse fit).</returns>
		int GetScore();
	}
}
