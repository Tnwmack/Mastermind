namespace Mastermind
{
	partial class KnuthSettings
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
			this.PoolSizeTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.OKButton = new System.Windows.Forms.Button();
			this.CanButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Maximum pool size:";
			// 
			// PoolSizeTextBox
			// 
			this.PoolSizeTextBox.Location = new System.Drawing.Point(115, 12);
			this.PoolSizeTextBox.Name = "PoolSizeTextBox";
			this.PoolSizeTextBox.Size = new System.Drawing.Size(80, 20);
			this.PoolSizeTextBox.TabIndex = 1;
			this.PoolSizeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PoolSizeTextBox_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(201, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "megabytes";
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Location = new System.Drawing.Point(12, 49);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 3;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			// 
			// CanButton
			// 
			this.CanButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CanButton.Location = new System.Drawing.Point(93, 49);
			this.CanButton.Name = "CanButton";
			this.CanButton.Size = new System.Drawing.Size(75, 23);
			this.CanButton.TabIndex = 4;
			this.CanButton.Text = "Cancel";
			this.CanButton.UseVisualStyleBackColor = true;
			// 
			// KnuthSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(272, 88);
			this.Controls.Add(this.CanButton);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.PoolSizeTextBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "KnuthSettings";
			this.Text = "KnuthSettings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KnuthSettings_FormClosing);
			this.Load += new System.EventHandler(this.KnuthSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox PoolSizeTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CanButton;
	}
}