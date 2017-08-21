namespace Mastermind
{
	partial class MainMenu
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
			this.SolverButton = new System.Windows.Forms.Button();
			this.AITypeComboBox = new System.Windows.Forms.ComboBox();
			this.GuessButton = new System.Windows.Forms.Button();
			this.BoardSettingsButton = new System.Windows.Forms.Button();
			this.GameStateLabel = new System.Windows.Forms.Label();
			this.ResetButton = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.SolverStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.GameBoardControl = new CustomControlsLibrary.BoardControl();
			this.AnswerKeyControl = new CustomControlsLibrary.AnswerControl();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// SolverButton
			// 
			this.SolverButton.Location = new System.Drawing.Point(12, 39);
			this.SolverButton.Name = "SolverButton";
			this.SolverButton.Size = new System.Drawing.Size(100, 23);
			this.SolverButton.TabIndex = 2;
			this.SolverButton.Text = "Solver Settings";
			this.SolverButton.UseVisualStyleBackColor = true;
			this.SolverButton.Click += new System.EventHandler(this.SolverButton_Click);
			// 
			// AITypeComboBox
			// 
			this.AITypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.AITypeComboBox.FormattingEnabled = true;
			this.AITypeComboBox.Items.AddRange(new object[] {
            "Knuth Solver",
            "Genetic Solver",
            "Random Solver"});
			this.AITypeComboBox.Location = new System.Drawing.Point(12, 12);
			this.AITypeComboBox.Name = "AITypeComboBox";
			this.AITypeComboBox.Size = new System.Drawing.Size(100, 21);
			this.AITypeComboBox.TabIndex = 4;
			this.AITypeComboBox.SelectedIndexChanged += new System.EventHandler(this.AITypeComboBox_SelectedIndexChanged);
			// 
			// GuessButton
			// 
			this.GuessButton.Location = new System.Drawing.Point(12, 97);
			this.GuessButton.Name = "GuessButton";
			this.GuessButton.Size = new System.Drawing.Size(100, 23);
			this.GuessButton.TabIndex = 5;
			this.GuessButton.Text = "Guess";
			this.GuessButton.UseVisualStyleBackColor = true;
			this.GuessButton.Click += new System.EventHandler(this.GuessButton_Click);
			// 
			// BoardSettingsButton
			// 
			this.BoardSettingsButton.Location = new System.Drawing.Point(12, 68);
			this.BoardSettingsButton.Name = "BoardSettingsButton";
			this.BoardSettingsButton.Size = new System.Drawing.Size(100, 23);
			this.BoardSettingsButton.TabIndex = 6;
			this.BoardSettingsButton.Text = "Board Settings";
			this.BoardSettingsButton.UseVisualStyleBackColor = true;
			this.BoardSettingsButton.Click += new System.EventHandler(this.BoardSettingsButton_Click);
			// 
			// GameStateLabel
			// 
			this.GameStateLabel.AutoSize = true;
			this.GameStateLabel.Location = new System.Drawing.Point(12, 152);
			this.GameStateLabel.Name = "GameStateLabel";
			this.GameStateLabel.Size = new System.Drawing.Size(35, 13);
			this.GameStateLabel.TabIndex = 7;
			this.GameStateLabel.Text = "label1";
			// 
			// ResetButton
			// 
			this.ResetButton.Location = new System.Drawing.Point(12, 126);
			this.ResetButton.Name = "ResetButton";
			this.ResetButton.Size = new System.Drawing.Size(100, 23);
			this.ResetButton.TabIndex = 8;
			this.ResetButton.Text = "Reset";
			this.ResetButton.UseVisualStyleBackColor = true;
			this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SolverStatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 571);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(339, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 10;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// SolverStatusLabel
			// 
			this.SolverStatusLabel.Name = "SolverStatusLabel";
			this.SolverStatusLabel.Size = new System.Drawing.Size(74, 17);
			this.SolverStatusLabel.Text = "Solver Status";
			// 
			// GameBoardControl
			// 
			this.GameBoardControl.AutoScroll = true;
			this.GameBoardControl.AutoScrollMinSize = new System.Drawing.Size(0, 32);
			this.GameBoardControl.Columns = 4;
			this.GameBoardControl.Location = new System.Drawing.Point(118, 56);
			this.GameBoardControl.Name = "GameBoardControl";
			this.GameBoardControl.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
			this.GameBoardControl.PegBorderSize = 2F;
			this.GameBoardControl.ScoreColumnWidth = 30F;
			this.GameBoardControl.Size = new System.Drawing.Size(209, 505);
			this.GameBoardControl.TabIndex = 1;
			this.GameBoardControl.Text = "boardControl1";
			// 
			// AnswerKeyControl
			// 
			this.AnswerKeyControl.Location = new System.Drawing.Point(118, 12);
			this.AnswerKeyControl.Name = "AnswerKeyControl";
			this.AnswerKeyControl.NumColors = 7;
			this.AnswerKeyControl.PegBorderSize = 2F;
			this.AnswerKeyControl.Size = new System.Drawing.Size(209, 38);
			this.AnswerKeyControl.TabIndex = 0;
			this.AnswerKeyControl.Text = "AnswerKeyControl";
			this.AnswerKeyControl.OnAnswerChanged += new CustomControlsLibrary.AnswerControl.OnAnswerChangedDelegate(this.AnswerKeyControl_OnAnswerChanged);
			// 
			// MainMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(339, 593);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.ResetButton);
			this.Controls.Add(this.GameStateLabel);
			this.Controls.Add(this.BoardSettingsButton);
			this.Controls.Add(this.GuessButton);
			this.Controls.Add(this.AITypeComboBox);
			this.Controls.Add(this.SolverButton);
			this.Controls.Add(this.GameBoardControl);
			this.Controls.Add(this.AnswerKeyControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainMenu";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Mastermind";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CustomControlsLibrary.AnswerControl AnswerKeyControl;
		private CustomControlsLibrary.BoardControl GameBoardControl;
		private System.Windows.Forms.Button SolverButton;
		private System.Windows.Forms.ComboBox AITypeComboBox;
		private System.Windows.Forms.Button GuessButton;
		private System.Windows.Forms.Button BoardSettingsButton;
		private System.Windows.Forms.Label GameStateLabel;
		private System.Windows.Forms.Button ResetButton;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel SolverStatusLabel;
	}
}

