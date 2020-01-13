namespace Mastermind
{
	/// <summary>
	/// A simple container for row and score pairs
	/// </summary>
	public struct BoardRow
	{
		/// <summary>
		/// The row colors
		/// </summary>
		public RowState Row;
		
		/// <summary>
		/// The row score
		/// </summary>
		public RowScore Score;

		/// <summary>
		/// Creates a new board row
		/// </summary>
		/// <param name="Row">The row colors to use</param>
		/// <param name="Score">The row score to use</param>
		public BoardRow(RowState Row, RowScore Score)
		{
			this.Row = Row;
			this.Score = Score;
		}

		/// <see cref="System.Object.Equals(object)"/>
		public static bool operator == (BoardRow A, BoardRow B)
		{
			return A.Equals(B);
		}

		/// <see cref="System.Object.Equals(object)"/>
		public static bool operator != (BoardRow A, BoardRow B)
		{
			return !A.Equals(B);
		}

		/// <see cref="System.Object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(BoardRow))
				return false;

		BoardRow B = (BoardRow)obj;

			return (Row == B.Row) && (Score == B.Score);
		}

		/// <summary>
		/// Gets the hash of the row and score
		/// </summary>
		/// <returns>The hash code</returns>
		public override int GetHashCode()
		{
			return Row.GetHashCode() ^ Score.GetHashCode();
		}
	}
}
