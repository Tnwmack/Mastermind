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
	partial class GeneticSettings : Form
	{
		public GeneticSolver.GeneticSolverSettings Settings;

		public GeneticSettings()
		{
			InitializeComponent();
		}

		private bool ValidateBox<T>(TextBox Box, T Min, T Max) where T : IComparable<T>
		{
			if (String.IsNullOrEmpty(Box.Text))
				return false;

			//Try parsing the box to the requested type
		Type[] ArgTypes = { typeof(string), typeof(T).MakeByRefType() };
		System.Reflection.MethodInfo TryParseMethod = typeof(T).GetMethod("TryParse", ArgTypes);

			if (TryParseMethod == null)
				throw new ArgumentException("No TryParse method defined for type");

		object[] args = { Box.Text, null };
		bool Parsed = (bool)TryParseMethod.Invoke(null, args);

			//Compare the result to the bounds
			if (Parsed)
			{
			int MinResult = ((T)args[1]).CompareTo(Min);
			int MaxResult = ((T)args[1]).CompareTo(Max);

				if (MaxResult > 0)
					Parsed = false;

				if (MinResult < 0)
					Parsed = false;
			}
			
			if (Parsed)
			{
				Box.BackColor = Color.White;
				return true;
			}
			else
			{
				Box.BackColor = Color.IndianRed;
				return false;
			}
		}

		private void PoolSizeTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void CrossoversTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void MutationTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void ElitismTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void MatchTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void PartialTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void CutoffTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void GenerationsTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void DynCrossoversCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CrossoversTextBox.Enabled = !DynCrossoversCheckBox.Checked;
		}

		private void DynMutationsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			MutationTextBox.Enabled = !DynMutationsCheckBox.Checked;
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			//ValidateAll();
		}

		private void GeneticSettings_Load(object sender, EventArgs e)
		{
			if (Settings == null)
				return;

			PoolSizeTextBox.Text = Settings.PoolSize.ToString();
			CrossoversTextBox.Text = Settings.CrossoverAmount.ToString();
			DynCrossoversCheckBox.Checked = Settings.DynamicCrossoverAmount;
			MutationTextBox.Text = Settings.MutationRate.ToString();
			DynMutationsCheckBox.Checked = Settings.DynamicMutationRate;
			ElitismTextBox.Text = Settings.ElitismCutoff.ToString();
			MatchTextBox.Text = Settings.MatchScore.ToString();
			PartialTextBox.Text = Settings.PartialMatchScore.ToString();
			CutoffTextBox.Text = Settings.ScoreCutoff.ToString();
			GenerationsTextBox.Text = Settings.MaxGenerations.ToString();
			LinearCheckBox.Checked = Settings.LinearCrossover;
		}

		private bool ValidateAll()
		{
			if (!ValidateBox<Int32>(PoolSizeTextBox, 0, Int32.MaxValue))
				return false;

			if (!ValidateBox<Single>(CrossoversTextBox, 0.0f, 1.0f))
				return false;

			if (!ValidateBox<Single>(MutationTextBox, 0.0f, 1.0f))
				return false;

			if (!ValidateBox<Int32>(ElitismTextBox, 0, Int32.MaxValue))
				return false;

			if (!ValidateBox<Int32>(MatchTextBox, 0, Int32.MaxValue))
				return false;

			if (!ValidateBox<Int32>(PartialTextBox, 0, Int32.MaxValue))
				return false;

			if (!ValidateBox<Int32>(CutoffTextBox, Int32.MinValue, 0))
				return false;

			if (!ValidateBox<Int32>(GenerationsTextBox, 0, Int32.MaxValue))
				return false;

			return true;
		}

		private void GeneticSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(DialogResult == DialogResult.OK)
			{
				if (!ValidateAll())
				{
					e.Cancel = true;
					return;
				}

				Settings.PoolSize = int.Parse(PoolSizeTextBox.Text);
				Settings.CrossoverAmount = float.Parse(CrossoversTextBox.Text);
				Settings.DynamicCrossoverAmount = DynCrossoversCheckBox.Checked;
				Settings.MutationRate = float.Parse(MutationTextBox.Text);
				Settings.DynamicMutationRate = DynMutationsCheckBox.Checked;
				Settings.ElitismCutoff = int.Parse(ElitismTextBox.Text);
				Settings.MatchScore = int.Parse(MatchTextBox.Text);
				Settings.PartialMatchScore = int.Parse(PartialTextBox.Text);
				Settings.ScoreCutoff = int.Parse(CutoffTextBox.Text);
				Settings.MaxGenerations = int.Parse(GenerationsTextBox.Text);
				Settings.LinearCrossover = LinearCheckBox.Checked;
			}
		}

		
	}
}
