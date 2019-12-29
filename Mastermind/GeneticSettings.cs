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

		public GeneticSettings(GeneticSolver.GeneticSolverSettings Settings)
		{
			this.Settings = Settings;
			InitializeComponent();
		}

		/// <summary>
		/// Validates a text input by parsing and bounds checking. Changes box background on failure.
		/// </summary>
		/// <typeparam name="T">The desired parsed type, must support "TryParse" and "CompareTo".</typeparam>
		/// <param name="Box">The TextBox to parse.</param>
		/// <param name="Min">The minimum allowed value.</param>
		/// <param name="Max">The maximum allowed value.</param>
		/// <returns>True if the text was parsed correctly and is within the bounds.</returns>
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

		/// <see cref="Control.OnValidating(CancelEventArgs)"/>
		private void TextBox_Validating(object sender, CancelEventArgs e)
		{
			if (!ValidateAll())
				e.Cancel = true;
		}

		/// <summary>
		/// Closes the settings form with an ok result.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Button.OnClick(EventArgs)"/></param>
		private void OKButton_Click(object sender, EventArgs e)
		{
			//ValidateAll();
		}

		/// <summary>
		/// Sets the controls to the initial settings.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Form.OnLoad(EventArgs)"/></param>
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
		}

		/// <summary>
		/// Validates all of the form controls.
		/// </summary>
		/// <returns>True if all controls validated correctly.</returns>
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

		/// <summary>
		/// Saves the settings if ok was clicked and the controls validate.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Form.OnFormClosing(FormClosingEventArgs)"/></param>
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
			}
		}
	}
}
