using System.Drawing;

namespace Mastermind
{
	/// <summary>
	/// Represents a placeholder color mapping
	/// </summary>
	public class TempColors : ColorMapping
	{
		/// <see cref="ColorMapping.GetBrush(int)"/>
		public Brush GetBrush(int ColorID)
		{
			return Brushes.Black;
		}
	}
}
