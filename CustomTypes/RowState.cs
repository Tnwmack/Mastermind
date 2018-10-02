using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// Holds a row state.
	/// This is an immutable class.
	/// </summary>
	public struct RowState
	{
		private byte[] Colors;

		/// <summary>
		/// Creates a new row state
		/// </summary>
		/// <param name="InitColors">The colors to use for the row</param>
		public RowState(params byte[] InitColors)
		{
			//Colors = (byte[])InitColors.Clone();
			Colors = new byte[InitColors.Length];

			for (int i = InitColors.Length - 1; i >= 0; i--)
				Colors[i] = InitColors[i];
		}

		/// <summary>
		/// Generates a random row
		/// </summary>
		/// <param name="RandGenerator">The generator to use</param>
		/// <param name="NumColors">The number of colors to choose between</param>
		/// <param name="NumColumns">The number of columns to generate</param>
		/// <returns>The random row</returns>
		public static RowState GetRandomColors(Random RandGenerator, int NumColors, int NumColumns)
		{
		RowState NewState = new RowState();
			NewState.Colors = new byte[NumColumns];

			for (int i = 0; i < NumColumns; i ++)
				NewState.Colors[i] = (byte)RandGenerator.Next(0, NumColors);

			return NewState;
		}

		/// <summary>
		/// Copies the row colors to a byte array
		/// </summary>
		/// <param name="Dest">The destination array</param>
		public void CopyTo(byte[] Dest)
		{
			Colors.CopyTo(Dest, 0);
		}

		/// <summary>
		/// Copies the row colors to a byte array
		/// </summary>
		/// <param name="Dest">The destination array</param>
		/// <param name="StartIndex">The index in Dest to start copying to</param>
		public void CopyTo(byte[] Dest, int StartIndex)
		{
			Colors.CopyTo(Dest, StartIndex);
		}

		/// <summary>
		/// Converts this row to a string of numbers
		/// </summary>
		/// <returns>The text version of the row</returns>
		public override string ToString()
		{
		StringBuilder SB = new StringBuilder();

			for(int i = 0; i < Colors.Length; i ++)
			{
				SB.Append(Colors[i].ToString());

				if (i + 1 != Colors.Length)
					SB.Append(" ");
			}

			return SB.ToString();
		}

		/// <summary>
		/// Gets the color at the given column number
		/// </summary>
		/// <param name="index">The column to get the color</param>
		/// <returns>The color number</returns>
		public byte this[int index]
		{
			get { return Colors[index]; }
		}

		/// <summary>
		/// Gets the number of columns in this row
		/// </summary>
		public int Length
		{
			get { return Colors.Length; }
		}

		/// <see cref="System.Object.Equals(object)"/>
		public static bool operator == (RowState A, RowState B)
		{
			return A.Equals(B);
		}

		/// <see cref="System.Object.Equals(object)"/>
		public static bool operator != (RowState A, RowState B)
		{
			return !A.Equals(B);
		}

		/// <see cref="System.Object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(RowState))
				return false;

		RowState B = (RowState)obj;

			for (int i = 0; i < Colors.Length; i ++)
			{
				if (Colors[i] != B.Colors[i])
					return false;
			}

			return true;
		}

		/// <summary>
		/// Gets the hash of the colors
		/// </summary>
		/// <returns>The hash code</returns>
		public override int GetHashCode()
		{
			return ((System.Collections.IStructuralEquatable)Colors).GetHashCode(EqualityComparer<object>.Default);
		}
	}
}
