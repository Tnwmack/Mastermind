namespace Mastermind
{
	partial class BoardSettingsForm
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
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.RowsComboBox = new System.Windows.Forms.ComboBox();
			this.ColumnsComboBox = new System.Windows.Forms.ComboBox();
			this.ColorsComboBox = new System.Windows.Forms.ComboBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.CanButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Game Rows:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Game Columns:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Game Colors:";
			// 
			// RowsComboBox
			// 
			this.RowsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RowsComboBox.FormattingEnabled = true;
			this.RowsComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "Unlimited"});
			this.RowsComboBox.Location = new System.Drawing.Point(99, 12);
			this.RowsComboBox.Name = "RowsComboBox";
			this.RowsComboBox.Size = new System.Drawing.Size(108, 21);
			this.RowsComboBox.TabIndex = 3;
			// 
			// ColumnsComboBox
			// 
			this.ColumnsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ColumnsComboBox.FormattingEnabled = true;
			this.ColumnsComboBox.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
			this.ColumnsComboBox.Location = new System.Drawing.Point(99, 39);
			this.ColumnsComboBox.Name = "ColumnsComboBox";
			this.ColumnsComboBox.Size = new System.Drawing.Size(108, 21);
			this.ColumnsComboBox.TabIndex = 4;
			// 
			// ColorsComboBox
			// 
			this.ColorsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ColorsComboBox.FormattingEnabled = true;
			this.ColorsComboBox.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
			this.ColorsComboBox.Location = new System.Drawing.Point(99, 66);
			this.ColorsComboBox.Name = "ColorsComboBox";
			this.ColorsComboBox.Size = new System.Drawing.Size(108, 21);
			this.ColorsComboBox.TabIndex = 5;
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Location = new System.Drawing.Point(12, 93);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 6;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CanButton
			// 
			this.CanButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CanButton.Location = new System.Drawing.Point(93, 93);
			this.CanButton.Name = "CanButton";
			this.CanButton.Size = new System.Drawing.Size(75, 23);
			this.CanButton.TabIndex = 7;
			this.CanButton.Text = "Cancel";
			this.CanButton.UseVisualStyleBackColor = true;
			this.CanButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// BoardSettingsForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CanButton;
			this.ClientSize = new System.Drawing.Size(219, 128);
			this.Controls.Add(this.CanButton);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.ColorsComboBox);
			this.Controls.Add(this.ColumnsComboBox);
			this.Controls.Add(this.RowsComboBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BoardSettingsForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Board Settings";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox RowsComboBox;
		private System.Windows.Forms.ComboBox ColumnsComboBox;
		private System.Windows.Forms.ComboBox ColorsComboBox;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CanButton;
	}
}