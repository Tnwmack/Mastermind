using System;

namespace Mastermind
{
	/// <summary>
	/// Contains the settings for the game board
	/// </summary>
	public class BoardSettings : ICloneable
	{
		//TODO: Make this immutable

		/// <summary>
		/// The number of rows in the game
		/// </summary>
		public int Rows { get; set; } = 10;

		/// <summary>
		/// The number of columns in the game
		/// </summary>
		public int Columns { get; set; } = 4;

		/// <summary>
		/// The number of colors in the game
		/// </summary>
		public int Colors { get; set; } = 7;

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
