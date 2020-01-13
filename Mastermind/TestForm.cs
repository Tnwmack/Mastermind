using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Mastermind
{
	partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		public delegate void OnRunClickedDelegate(TestForm Sender, int Iterations);
		public event OnRunClickedDelegate OnRunClicked;

		public delegate void OnStopClickedDelegate(TestForm Sender);
		public event OnStopClickedDelegate OnStopClicked;

		private bool TestInProgress = false;
		private readonly Dictionary<Control, String> LabelText = new Dictionary<Control, string>();

		/// <see cref="Form.OnLoad(EventArgs)"/>
		private void TestForm_Load(object sender, EventArgs e)
		{
			LabelText.Add(IterationLabel, IterationLabel.Text);
			LabelText.Add(TimeLabel, TimeLabel.Text);
			LabelText.Add(GuessesLabel, GuessesLabel.Text);
			LabelText.Add(FailuresLabel, FailuresLabel.Text);
		}

		/// <summary>
		/// Enables and disables the run and stop buttons based on the current state.
		/// </summary>
		private void SetButtonStates()
		{
			if (this.InvokeRequired)
			{
				this.Invoke((MethodInvoker)delegate
				{
					SetButtonStates();
				});
			}
			else
			{
				if (TestInProgress)
				{
					RunButton.Enabled = false;
					StopButton.Enabled = true;
					this.Cursor = Cursors.WaitCursor;
				}
				else
				{
					RunButton.Enabled = true;
					StopButton.Enabled = false;
					this.Cursor = Cursors.Default;
				}
			}	
		}

		private void RunButton_Click(object sender, EventArgs e)
		{
			TestInProgress = true;
			SetButtonStates();

			OnRunClicked?.BeginInvoke(this, (int)IterationNumeric.Value, null, null);
		}

		private void StopButton_Click(object sender, EventArgs e)
		{
			OnStopClicked?.BeginInvoke(this, null, null);
		}

		private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(TestInProgress)
			{
				MessageBox.Show(Properties.Resources.TestForm_StopTests, Properties.Resources.Error, 
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.Cancel = true;
			}
		}

		public void OnTestsCompleted()
		{
			TestInProgress = false;
			SetButtonStates();
		}

		public void UpdateResults(int IterationCount, double AverageTime, double AverageGuesses, int NumFailures)
		{
			if (this.InvokeRequired)
			{
				this.Invoke((MethodInvoker)delegate
				{
					UpdateResults(IterationCount, AverageTime, AverageGuesses, NumFailures);
				});
			}
			else
			{
				IterationLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", LabelText[IterationLabel], IterationCount);
				TimeLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} {1:G3}", LabelText[TimeLabel], AverageTime);
				GuessesLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} {1:G3}", LabelText[GuessesLabel], AverageGuesses);
				FailuresLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", LabelText[FailuresLabel], NumFailures);
			}
		}
	}
}
