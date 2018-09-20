﻿using System;
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

		private bool ValidateBox<T>(TextBox Box, T Min, T Max)
		{
			if (String.IsNullOrEmpty(Box.Text))
				return false;

		Type[] ArgTypes = { typeof(string), typeof(T).MakeByRefType() };
		var TryParseMethod = typeof(T).GetMethod("TryParse", ArgTypes);

			if (TryParseMethod == null)
				return false;

		object[] args = { Box.Text, null };
		bool Parsed = (bool)TryParseMethod.Invoke(null, args);

		Type[] ArgTypes2 = { typeof(T) };
		var CompareToMethod = typeof(T).GetMethod("CompareTo", ArgTypes2);

			if (CompareToMethod != null)
			{
				int MinResult = (int)CompareToMethod.Invoke(args[1], new object[] { Min });
				int MaxResult = (int)CompareToMethod.Invoke(args[1], new object[] { Max });

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

		private void GenerationsTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			ValidateAll();
		}

		private void GeneticSettings_Load(object sender, EventArgs e)
		{
			if (Settings == null)
				return;

			PoolSizeTextBox.Text = Settings.PoolSize.ToString();
			CrossoversTextBox.Text = Settings.CrossoverAmount.ToString();
			MutationTextBox.Text = Settings.MutationRate.ToString();
			ElitismTextBox.Text = Settings.ElitismCutoff.ToString();
			MatchTextBox.Text = Settings.MatchScore.ToString();
			PartialTextBox.Text = Settings.PartialMatchScore.ToString();
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
				Settings.MutationRate = float.Parse(MutationTextBox.Text);
				Settings.ElitismCutoff = int.Parse(ElitismTextBox.Text);
				Settings.MatchScore = int.Parse(MatchTextBox.Text);
				Settings.PartialMatchScore = int.Parse(PartialTextBox.Text);
				Settings.MaxGenerations = int.Parse(GenerationsTextBox.Text);
				Settings.LinearCrossover = LinearCheckBox.Checked;
			}
		}
	}
}
