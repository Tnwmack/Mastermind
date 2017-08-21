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
		public BoardSettings Settings;

		/// <summary>
		/// Creates a new settings dialog
		/// </summary>
		/// <param name="InitialSettings">The existing settings to fill the form with</param>
		public BoardSettingsForm(BoardSettings InitialSettings)
		{
			InitializeComponent();

			Settings = (BoardSettings)InitialSettings.Clone();

			if (Settings.Rows > 20)
				RowsComboBox.SelectedIndex = 20;
			else
				RowsComboBox.SelectedIndex = Settings.Rows - 1;

			ColumnsComboBox.SelectedIndex = Settings.Columns - 4;
			ColorsComboBox.SelectedIndex = Settings.Colors - 2;
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			if (RowsComboBox.SelectedIndex == 20)
				Settings.Rows = int.MaxValue;
			else
				Settings.Rows = RowsComboBox.SelectedIndex + 1;

			Settings.Colors = ColorsComboBox.SelectedIndex + 2;
			Settings.Columns = ColumnsComboBox.SelectedIndex + 4;
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{

		}
	}
}
