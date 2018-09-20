﻿namespace Mastermind
{
	partial class GeneticSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OKButton = new System.Windows.Forms.Button();
			this.CanButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.PoolSizeTextBox = new System.Windows.Forms.TextBox();
			this.CrossoversTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.MutationTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.ElitismTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.MatchTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.PartialTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.GenerationsTextBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.LinearCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Location = new System.Drawing.Point(12, 217);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 8;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CanButton
			// 
			this.CanButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CanButton.Location = new System.Drawing.Point(93, 217);
			this.CanButton.Name = "CanButton";
			this.CanButton.Size = new System.Drawing.Size(75, 23);
			this.CanButton.TabIndex = 9;
			this.CanButton.Text = "Cancel";
			this.CanButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(40, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Pool Size:";
			// 
			// PoolSizeTextBox
			// 
			this.helpProvider1.SetHelpString(this.PoolSizeTextBox, "The size of the pool. Default: 500");
			this.PoolSizeTextBox.Location = new System.Drawing.Point(100, 12);
			this.PoolSizeTextBox.Name = "PoolSizeTextBox";
			this.helpProvider1.SetShowHelp(this.PoolSizeTextBox, true);
			this.PoolSizeTextBox.Size = new System.Drawing.Size(100, 20);
			this.PoolSizeTextBox.TabIndex = 0;
			this.PoolSizeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PoolSizeTextBox_Validating);
			// 
			// CrossoversTextBox
			// 
			this.helpProvider1.SetHelpString(this.CrossoversTextBox, "The ratio of crossovers that take place, the remainder will be mutations. Eg: 0.5" +
        " = 0.5*poolsize crossovers and 0.5*poolsize mutations will be performed. Default" +
        ": 0.7");
			this.CrossoversTextBox.Location = new System.Drawing.Point(100, 38);
			this.CrossoversTextBox.Name = "CrossoversTextBox";
			this.helpProvider1.SetShowHelp(this.CrossoversTextBox, true);
			this.CrossoversTextBox.Size = new System.Drawing.Size(100, 20);
			this.CrossoversTextBox.TabIndex = 1;
			this.CrossoversTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.CrossoversTextBox_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(32, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Crossovers:";
			// 
			// MutationTextBox
			// 
			this.helpProvider1.SetHelpString(this.MutationTextBox, "The number of columns mutated after a parent is selected. Default: 0.25");
			this.MutationTextBox.Location = new System.Drawing.Point(100, 64);
			this.MutationTextBox.Name = "MutationTextBox";
			this.helpProvider1.SetShowHelp(this.MutationTextBox, true);
			this.MutationTextBox.Size = new System.Drawing.Size(100, 20);
			this.MutationTextBox.TabIndex = 2;
			this.MutationTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.MutationTextBox_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Mutation Rate:";
			// 
			// ElitismTextBox
			// 
			this.helpProvider1.SetHelpString(this.ElitismTextBox, "This many of the top pool members will not be crossed or mutated. Default: 20");
			this.ElitismTextBox.Location = new System.Drawing.Point(100, 90);
			this.ElitismTextBox.Name = "ElitismTextBox";
			this.helpProvider1.SetShowHelp(this.ElitismTextBox, true);
			this.ElitismTextBox.Size = new System.Drawing.Size(100, 20);
			this.ElitismTextBox.TabIndex = 3;
			this.ElitismTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.ElitismTextBox_Validating);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(24, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Elitism Cutoff:";
			// 
			// MatchTextBox
			// 
			this.helpProvider1.SetHelpString(this.MatchTextBox, "The score weight of matching color and column in evaluation. Default: 50");
			this.MatchTextBox.Location = new System.Drawing.Point(100, 116);
			this.MatchTextBox.Name = "MatchTextBox";
			this.helpProvider1.SetShowHelp(this.MatchTextBox, true);
			this.MatchTextBox.Size = new System.Drawing.Size(100, 20);
			this.MatchTextBox.TabIndex = 4;
			this.MatchTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.MatchTextBox_Validating);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(23, 119);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(71, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Match Score:";
			// 
			// PartialTextBox
			// 
			this.helpProvider1.SetHelpString(this.PartialTextBox, "The score weight of matching color but not column in evaluation. Default: 20");
			this.PartialTextBox.Location = new System.Drawing.Point(100, 142);
			this.PartialTextBox.Name = "PartialTextBox";
			this.helpProvider1.SetShowHelp(this.PartialTextBox, true);
			this.PartialTextBox.Size = new System.Drawing.Size(100, 20);
			this.PartialTextBox.TabIndex = 5;
			this.PartialTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PartialTextBox_Validating);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(24, 145);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(70, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "Partial Score:";
			// 
			// GenerationsTextBox
			// 
			this.helpProvider1.SetHelpString(this.GenerationsTextBox, "The maximum number of times to evolve the pool before giving up and guessing. Def" +
        "ault: 200");
			this.GenerationsTextBox.Location = new System.Drawing.Point(100, 168);
			this.GenerationsTextBox.Name = "GenerationsTextBox";
			this.helpProvider1.SetShowHelp(this.GenerationsTextBox, true);
			this.GenerationsTextBox.Size = new System.Drawing.Size(100, 20);
			this.GenerationsTextBox.TabIndex = 6;
			this.GenerationsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.GenerationsTextBox_Validating);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(4, 171);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(90, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "Max Generations:";
			// 
			// LinearCheckBox
			// 
			this.LinearCheckBox.AutoSize = true;
			this.helpProvider1.SetHelpString(this.LinearCheckBox, "Checked to use linear crossovers, random crossovers otherwise. Default: Linear");
			this.LinearCheckBox.Location = new System.Drawing.Point(7, 194);
			this.LinearCheckBox.Name = "LinearCheckBox";
			this.helpProvider1.SetShowHelp(this.LinearCheckBox, true);
			this.LinearCheckBox.Size = new System.Drawing.Size(132, 17);
			this.LinearCheckBox.TabIndex = 7;
			this.LinearCheckBox.Text = "Use Linear Crossovers";
			this.LinearCheckBox.UseVisualStyleBackColor = true;
			// 
			// GeneticSettings
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CanButton;
			this.ClientSize = new System.Drawing.Size(212, 251);
			this.Controls.Add(this.LinearCheckBox);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.GenerationsTextBox);
			this.Controls.Add(this.PartialTextBox);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.MatchTextBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.ElitismTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.MutationTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.CrossoversTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.PoolSizeTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CanButton);
			this.Controls.Add(this.OKButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GeneticSettings";
			this.Text = "Genetic Settings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GeneticSettings_FormClosing);
			this.Load += new System.EventHandler(this.GeneticSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CanButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox PoolSizeTextBox;
		private System.Windows.Forms.TextBox CrossoversTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox MutationTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox ElitismTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox MatchTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox PartialTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.HelpProvider helpProvider1;
		private System.Windows.Forms.TextBox GenerationsTextBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox LinearCheckBox;
	}
}