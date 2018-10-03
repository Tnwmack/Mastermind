using System.Drawing;

namespace Mastermind
{
	/// <summary>
	/// Represents a placeholder color mapping
	/// </summary>
	public class TempColors : IColorMapping
	{
		/// <see cref="IColorMapping.GetBrush(int)"/>
		public Brush GetBrush(int ColorID)
		{
			return Brushes.Black;
		}
	}
}
