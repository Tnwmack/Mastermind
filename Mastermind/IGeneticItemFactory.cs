namespace Mastermind
{
	/// <summary>
	/// Interface for a factory that can generate IGeneticItems.
	/// </summary>
	/// <typeparam name="T">Type of IGeneticItems this factory generates.</typeparam>
	public interface IGeneticItemFactory<T> where T : IGeneticItem
	{
		/// <summary>
		/// Generates a random item.
		/// </summary>
		/// <returns>A randomly generated item.</returns>
		T GetRandom();

		/// <summary>
		/// Performs a cross operation using parents A and B.
		/// </summary>
		/// <param name="A">Parent A.</param>
		/// <param name="B">Parent B.</param>
		/// <param name="ResultA">Result A.</param>
		/// <param name="ResultB">Result B.</param>
		void Cross(T A, T B, out T ResultA, out T ResultB);

		/// <summary>
		/// Performs a mutation operation using the given parent.
		/// </summary>
		/// <param name="Item">Parent to mutate.</param>
		/// <returns>Mutated output.</returns>
		T Mutate(T Item);
	}
}
