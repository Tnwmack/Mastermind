using Mastermind;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
	/// <summary>
	/// A mastermind board display control
	/// </summary>
	public partial class BoardControl : ScrollableControl
	{
		//The list of rows to display
		private List<BoardRow> Rows = new List<BoardRow>();

		/// <summary>
		/// The color mapping to use
		/// </summary>
		public IColorMapping Colors = new TempColors();

		/// <summary>
		/// The number of columns
		/// </summary>
		public int Columns { get; set; } = 4;

		/// <summary>
		/// The width of the score column in pixels
		/// </summary>
		public float ScoreColumnWidth { get; set; } = 15.0f;

		/// <summary>
		/// The size of the black peg border in pixels
		/// </summary>
		public float PegBorderSize { get; set; } = 2.0f;

		//When true, the auto scroll min size needs to be updated
		private bool RecalculateScrollSize = true;

		private Brush PegBackColor = new SolidBrush(Color.Black);
		private Font ScoreFont = new Font(FontFamily.GenericMonospace, 20.0f, FontStyle.Bold, GraphicsUnit.World);

		/// <summary>
		/// Creates a new board control
		/// </summary>
		public BoardControl()
		{
			InitializeComponent();

			DoubleBuffered = true; //To prevent flickering
			ResizeRedraw = true;
			AutoScrollMinSize = new Size(0, 32);
			AutoScrollPosition = new Point();
		}

		/// <summary>
		/// Add a new row to the control
		/// </summary>
		/// <param name="NewRow">The new row to add</param>
		public void AddRow(BoardRow NewRow)
		{
			Rows.Add(NewRow);
			RecalculateScrollSize = true;
			Invalidate();
		}

		/// <summary>
		/// Clears the board
		/// </summary>
		public void ResetBoard()
		{
			Rows.Clear();
			RecalculateScrollSize = true;
			Invalidate();
		}

		/// <summary>
		/// Gets the location to draw the row score
		/// </summary>
		/// <param name="Row">The row number</param>
		/// <param name="MinHeight">The minimum row height</param>
		/// <returns>The location to draw the score</returns>
		protected RectangleF GetScoreRect(int Row, float MinHeight)
		{
			float ColumnWidth = (ClientSize.Width - ScoreColumnWidth) / Columns;
			float RowHeight = Math.Max(MinHeight, ColumnWidth);

			return new RectangleF(ClientSize.Width - ScoreColumnWidth,
				RowHeight * Row, ScoreColumnWidth, RowHeight);
		}

		/// <summary>
		/// Gets the location to draw the peg
		/// </summary>
		/// <param name="Column">The peg column</param>
		/// <param name="Row">The peg row</param>
		/// <param name="MinHeight">The minimum row height</param>
		/// <returns>The location to draw the peg</returns>
		protected RectangleF GetPegRect(int Column, int Row, float MinHeight)
		{
			float ColumnWidth = (ClientSize.Width - ScoreColumnWidth) / Columns;
			float RowHeight = Math.Max(MinHeight, ColumnWidth);

			return new RectangleF(ColumnWidth * Column,
				RowHeight * Row, ColumnWidth, ColumnWidth);
		}

		/// <summary>
		/// Called when the scrollbar is moved
		/// </summary>
		/// <param name="se">The scroll parameters</param>
		protected override void OnScroll(ScrollEventArgs se)
		{
			base.OnScroll(se);

			//The control is not automatically invalidated on scrolling
			Invalidate();
		}

		/// <summary>
		/// Paints the control
		/// </summary>
		/// <param name="pe">The paint parameters</param>
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);

		Graphics g = pe.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			//Recalculate the board width and scroll area height
			if (RecalculateScrollSize && Rows.Count > 0)
			{
			SizeF MaxScoreWidth = new SizeF();

				foreach (BoardRow b in Rows)
				{
					SizeF Temp = g.MeasureString(b.Score.ToString(), ScoreFont);

					if (Temp.Width > MaxScoreWidth.Width)
						MaxScoreWidth = Temp;
				}

				ScoreColumnWidth = MaxScoreWidth.Width;

			RectangleF EndPegPos = GetPegRect(0, Rows.Count - 1, MaxScoreWidth.Height);
			RectangleF EndScorePos = GetScoreRect(Rows.Count - 1, MaxScoreWidth.Height);
			float EndPos = Math.Max(EndPegPos.Bottom, EndScorePos.Bottom);

				AutoScrollMinSize = new Size(0, (int)EndPos + 2); //+2 for border clipping
				AutoScrollPosition = new Point(0, (int)EndPos + 2);
				RecalculateScrollSize = false;
			}

			//Scroll translate
			g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

			//Draw by row
			for (int i = 0; i < Rows.Count; i ++)
			{
			SizeF ScoreSize = g.MeasureString(Rows[i].Score.ToString(), ScoreFont);

				//Draw each column in the row
				for (int j = 0; j < Rows[i].Row.Length; j ++)
				{
				RectangleF BackgroundPos = GetPegRect(j, i, ScoreSize.Height);
				RectangleF ForgroundPos = BackgroundPos;

					ForgroundPos.X += PegBorderSize;
					ForgroundPos.Y += PegBorderSize;
					ForgroundPos.Width -= PegBorderSize * 2.0f;
					ForgroundPos.Height -= PegBorderSize * 2.0f;

					g.FillEllipse(PegBackColor, BackgroundPos);
					g.FillEllipse(Colors.GetBrush(Rows[i].Row[j]), ForgroundPos);
				}
				
				g.DrawString(Rows[i].Score.ToString(), ScoreFont, Brushes.Black, GetScoreRect(i, ScoreSize.Height));
			}
		}
	}
}
