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

		private void KnuthSettings_Load(object sender, EventArgs e)
		{
			PoolSizeTextBox.Text = Settings.MaximumPoolSize.ToString();
		}

		private void KnuthSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(DialogResult == DialogResult.OK)
			{
				Settings.MaximumPoolSize = int.Parse(PoolSizeTextBox.Text);
			}
		}

		private void PoolSizeTextBox_Validating(object sender, CancelEventArgs e)
		{
			int Temp;

			if(!int.TryParse(PoolSizeTextBox.Text, out Temp))
			{
				e.Cancel = true;
			}
		}
	}
}
