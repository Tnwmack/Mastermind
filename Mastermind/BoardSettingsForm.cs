using System;
using System.Windows.Forms;

namespace Mastermind
{
	/// <summary>
	/// Displays a modal settings dialog box for board settings
	/// </summary>
	public partial class BoardSettingsForm : Form
	{
		/// <summary>
		/// The settings selected by the user
		/// </summary>
		public BoardSettings Settings { get; set; }

		/// <summary>
		/// Creates a new settings dialog
		/// </summary>
		/// <param name="InitialSettings">The existing settings to fill the form with</param>
		public BoardSettingsForm(BoardSettings InitialSettings)
		{
			InitializeComponent();

			InitialSettings ??= new BoardSettings();
			Settings = (BoardSettings)InitialSettings.Clone();

			if (Settings.Rows > 20)
				RowsComboBox.SelectedIndex = 20;
			else
				RowsComboBox.SelectedIndex = Settings.Rows - 1;

			ColorsComboBox.SelectedIndex = Settings.Colors - 2;
			ColumnsComboBox.SelectedIndex = Settings.Columns - 4;
		}

		/// <summary>
		/// Confirms the new board settings.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnClick(EventArgs)"/></param>
		private void OKButton_Click(object sender, EventArgs e)
		{
			if (RowsComboBox.SelectedIndex == 20)
				Settings.Rows = int.MaxValue;
			else
				Settings.Rows = RowsComboBox.SelectedIndex + 1;

			Settings.Colors = ColorsComboBox.SelectedIndex + 2;
			Settings.Columns = ColumnsComboBox.SelectedIndex + 4;
		}

		/// <summary>
		/// Dismisses the settings form.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnClick(EventArgs)"/></param>
		private void CancelButton_Click(object sender, EventArgs e)
		{

		}
	}
}
