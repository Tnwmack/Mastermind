using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// Contains the settings for the game board
	/// </summary>
	public class BoardSettings : ICloneable
	{
		/// <summary>
		/// The number of rows in the game
		/// </summary>
		public int Rows = 10;

		/// <summary>
		/// The number of columns in the game
		/// </summary>
		public int Columns = 4;

		/// <summary>
		/// The number of colors in the game
		/// </summary>
		public int Colors = 7;

		/// <see cref="ICloneable.Clone"/>
		public object Clone()
		{
		BoardSettings BS = new BoardSettings();
			BS.Rows = Rows;
			BS.Columns = Columns;
			BS.Colors = Colors;

			return BS;
		}
	}
}
