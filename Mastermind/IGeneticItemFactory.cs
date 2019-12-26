using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	public interface IGeneticItemFactory<T> where T : IGeneticItem
	{
		T GetRandom();

		void Cross(T A, T B, out T ResultA, out T ResultB);

		T Mutate(T Item);
	}
}
