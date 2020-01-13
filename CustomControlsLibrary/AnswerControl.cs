using Mastermind;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
	/// <summary>
	/// A mastermind answer display control
	/// </summary>
	public partial class AnswerControl : Control
	{
		/// <summary>
		/// <see cref="OnAnswerChanged"/> 
		/// </summary>
		/// <param name="NewAnswer">The new answer key selected</param>
		public delegate void OnAnswerChangedDelegate(RowState NewAnswer);
		
		/// <summary>
		/// Called when the answer is changed
		/// </summary>
		public event OnAnswerChangedDelegate OnAnswerChanged;

		/// <summary>
		/// The color mapping to use
		/// </summary>
		public IColorMapping Colors = new TempColors();

		/// <summary>
		/// The answer currently displayed
		/// </summary>
		public RowState CurrentAnswer
		{
			get { return new RowState(ColumnColors); }
		}

		private byte[] ColumnColors = new byte[4];

		/// <summary>
		/// The number of columns displayed
		/// </summary>
		public int Columns
		{
			get { return ColumnColors.Length; }
		}

		/// <summary>
		/// The number of colors to choose from
		/// </summary>
		public int NumColors { get; set; } = 1;

		/// <summary>
		/// The size of the black peg border in pixels
		/// </summary>
		public float PegBorderSize { get; set; } = 2.0f;

		private Brush PegBackColor = new SolidBrush(Color.Black);

		private Random Generator = new Random();

		/// <summary>
		/// Creates a new answer control
		/// </summary>
		public AnswerControl()
		{
			InitializeComponent();

			DoubleBuffered = true;
		}

		/// <summary>
		/// Change the displayed answer. 
		/// </summary>
		/// <remarks>Does not trigger the OnAnswerChanged event.</remarks>
		/// <param name="NewAnswer">The answer to display</param>
		public void SetAnswer(RowState NewAnswer)
		{
			ColumnColors = new byte[NewAnswer.Length];
			NewAnswer.CopyTo(ColumnColors);

			Invalidate();
		}

		/// <summary>
		/// Get the rect the given peg occupies
		/// </summary>
		/// <param name="Column">The peg column</param>
		/// <returns>The rectangle the peg occupies</returns>
		protected RectangleF GetPegRect(int Column)
		{
		float ColumnWidth = Math.Min(ClientSize.Width / Columns, 
			ClientSize.Height - 1); //-1 to prevent border clipping

			return new RectangleF(ColumnWidth * Column,
				0, ColumnWidth, ColumnWidth);
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

			for (int i = 0; i < Columns; i++)
			{
			RectangleF Pos = GetPegRect(i);
			RectangleF Forground = Pos;

				Forground.X += PegBorderSize;
				Forground.Y += PegBorderSize;
				Forground.Width -= PegBorderSize * 2.0f;
				Forground.Height -= PegBorderSize * 2.0f;

				g.FillEllipse(PegBackColor, Pos);
				g.FillEllipse(Colors.GetBrush(ColumnColors[i]), Forground);
			}
		}

		/// <summary>
		/// Changes the displayed peg on the answer key
		/// </summary>
		/// <param name="e">Mouse event data</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			//Cycle through the colors in the clicked column
			if (e.Button == MouseButtons.Left)
			{
				for (int i = 0; i < Columns; i++)
				{
					if (GetPegRect(i).Contains(e.Location))
					{
						ColumnColors[i] = (byte)(++ColumnColors[i] % NumColors);
						break;
					}
				}
			}
			else if (e.Button == MouseButtons.Right) //Generate a random color row
			{
			RowState NewRS = RowState.GetRandomColors(Generator, NumColors, Columns);

				NewRS.CopyTo(ColumnColors);
			}

			Invalidate();

			OnAnswerChanged?.Invoke(CurrentAnswer);
		}
	}
}
