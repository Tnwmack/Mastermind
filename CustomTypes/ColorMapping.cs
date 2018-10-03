using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Mastermind
{
	/// <summary>
	/// An interface for mapping generic color IDs to brushes
	/// </summary>
	public interface IColorMapping
	{
		/// <summary>
		/// Returns a brush for the given generic ID
		/// </summary>
		/// <param name="ColorID">The generic color ID</param>
		/// <returns>The brush to use for the given color ID</returns>
		Brush GetBrush(int ColorID);
	}
}
