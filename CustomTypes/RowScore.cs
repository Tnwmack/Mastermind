using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// Holds row score information
	/// </summary>
	public struct RowScore
	{
		/// <summary>
		/// The number of pegs in the right spot and color
		/// </summary>
		public readonly int NumCorrectSpot;

		/// <summary>
		/// The number of pegs the right color but wrong spot
		/// </summary>
		public readonly int NumCorrectColor;

		/// <summary>
		/// Creates a new row score
		/// </summary>
		/// <param name="NumCorrectSpot">Number in the correct spot</param>
		/// <param name="NumCorrectColor">Number of correct colors</param>
		public RowScore(int NumCorrectSpot, int NumCorrectColor)
		{
			this.NumCorrectSpot = NumCorrectSpot;
			this.NumCorrectColor = NumCorrectColor;
		}

		/// <summary>
		/// Gets a text version of the string, num correct over num colors
		/// </summary>
		/// <returns>The text version</returns>
		public override string ToString()
		{
			return String.Format("{0}\r\n{1}", NumCorrectSpot, NumCorrectColor);
		}

		/// <see cref="System.Object.Equals(object)"/> 
		public static bool operator == (RowScore A, RowScore B)
		{
			return A.Equals(B);
		}

		/// <see cref="System.Object.Equals(object)"/> 
		public static bool operator != (RowScore A, RowScore B)
		{
			return !A.Equals(B);
		}

		/// <see cref="System.Object.Equals(object)"/> 
		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(RowScore))
				return false;

		RowScore B = (RowScore)obj;

			return (NumCorrectSpot == B.NumCorrectSpot) && (NumCorrectColor == B.NumCorrectColor);
		}

		/// <summary>
		/// Gets the hash of the scores
		/// </summary>
		/// <returns>The hash code</returns>
		public override int GetHashCode()
		{
			return ((NumCorrectSpot + NumCorrectColor) * (NumCorrectSpot + NumCorrectColor + 1)) / 2 + NumCorrectColor;
		}
	}
}
