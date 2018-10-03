using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mastermind
{
	partial class KnuthSettings : Form
	{
		public KnuthSettings()
		{
			InitializeComponent();
		}

		public KnuthSolver.KnuthSolverSettings Settings;

		/// <summary>
		/// Initializes the form controls.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Form.OnLoad(EventArgs)"/></param>
		private void KnuthSettings_Load(object sender, EventArgs e)
		{
			PoolSizeTextBox.Text = Settings.MaximumPoolSize.ToString();
		}

		/// <summary>
		/// Saves the settings if ok was selected.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Form.OnFormClosing(FormClosingEventArgs)"/></param>
		private void KnuthSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(DialogResult == DialogResult.OK)
			{
				Settings.MaximumPoolSize = int.Parse(PoolSizeTextBox.Text);
			}
		}

		/// <summary>
		/// Validates the form controls.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnValidating(CancelEventArgs)"/></param>
		private void PoolSizeTextBox_Validating(object sender, CancelEventArgs e)
		{
			int Temp;

			if(!int.TryParse(PoolSizeTextBox.Text, out Temp))
			{
				e.Cancel = true;
			}
			else
			{
				if(Temp <= 0)
					e.Cancel = true;
			}
		}
	}
}
