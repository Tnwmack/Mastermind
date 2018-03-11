namespace Mastermind
{
	partial class TestForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.IterationLabel = new System.Windows.Forms.Label();
			this.IterationNumeric = new System.Windows.Forms.NumericUpDown();
			this.RunButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.FailuresLabel = new System.Windows.Forms.Label();
			this.GuessesLabel = new System.Windows.Forms.Label();
			this.TimeLabel = new System.Windows.Forms.Label();
			this.StopButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.IterationNumeric)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of iterations:";
			// 
			// IterationLabel
			// 
			this.IterationLabel.AutoSize = true;
			this.IterationLabel.Location = new System.Drawing.Point(6, 16);
			this.IterationLabel.Name = "IterationLabel";
			this.IterationLabel.Size = new System.Drawing.Size(106, 13);
			this.IterationLabel.TabIndex = 1;
			this.IterationLabel.Text = "Completed Iterations:";
			// 
			// IterationNumeric
			// 
			this.IterationNumeric.Location = new System.Drawing.Point(121, 12);
			this.IterationNumeric.Name = "IterationNumeric";
			this.IterationNumeric.Size = new System.Drawing.Size(65, 20);
			this.IterationNumeric.TabIndex = 2;
			this.IterationNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// RunButton
			// 
			this.RunButton.Location = new System.Drawing.Point(12, 120);
			this.RunButton.Name = "RunButton";
			this.RunButton.Size = new System.Drawing.Size(75, 23);
			this.RunButton.TabIndex = 3;
			this.RunButton.Text = "Run";
			this.RunButton.UseVisualStyleBackColor = true;
			this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.FailuresLabel);
			this.groupBox1.Controls.Add(this.GuessesLabel);
			this.groupBox1.Controls.Add(this.TimeLabel);
			this.groupBox1.Controls.Add(this.IterationLabel);
			this.groupBox1.Location = new System.Drawing.Point(14, 38);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(213, 76);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Results";
			// 
			// FailuresLabel
			// 
			this.FailuresLabel.AutoSize = true;
			this.FailuresLabel.Location = new System.Drawing.Point(6, 55);
			this.FailuresLabel.Name = "FailuresLabel";
			this.FailuresLabel.Size = new System.Drawing.Size(98, 13);
			this.FailuresLabel.TabIndex = 4;
			this.FailuresLabel.Text = "Number of Failures:";
			// 
			// GuessesLabel
			// 
			this.GuessesLabel.AutoSize = true;
			this.GuessesLabel.Location = new System.Drawing.Point(6, 42);
			this.GuessesLabel.Name = "GuessesLabel";
			this.GuessesLabel.Size = new System.Drawing.Size(146, 13);
			this.GuessesLabel.TabIndex = 3;
			this.GuessesLabel.Text = "Average Number of Guesses:";
			// 
			// TimeLabel
			// 
			this.TimeLabel.AutoSize = true;
			this.TimeLabel.Location = new System.Drawing.Point(6, 29);
			this.TimeLabel.Name = "TimeLabel";
			this.TimeLabel.Size = new System.Drawing.Size(148, 13);
			this.TimeLabel.TabIndex = 2;
			this.TimeLabel.Text = "Average Time To Solve (sec):";
			// 
			// StopButton
			// 
			this.StopButton.Enabled = false;
			this.StopButton.Location = new System.Drawing.Point(93, 120);
			this.StopButton.Name = "StopButton";
			this.StopButton.Size = new System.Drawing.Size(75, 23);
			this.StopButton.TabIndex = 5;
			this.StopButton.Text = "Stop";
			this.StopButton.UseVisualStyleBackColor = true;
			this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
			// 
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(239, 152);
			this.Controls.Add(this.StopButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.RunButton);
			this.Controls.Add(this.IterationNumeric);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "TestForm";
			this.Text = "Testing";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestForm_FormClosing);
			this.Load += new System.EventHandler(this.TestForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.IterationNumeric)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label IterationLabel;
		private System.Windows.Forms.NumericUpDown IterationNumeric;
		private System.Windows.Forms.Button RunButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label FailuresLabel;
		private System.Windows.Forms.Label GuessesLabel;
		private System.Windows.Forms.Label TimeLabel;
		private System.Windows.Forms.Button StopButton;
	}
}