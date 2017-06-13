namespace ifme
{
	partial class frmShutdown
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
			this.lblInfo = new System.Windows.Forms.Label();
			this.cboShutdown = new System.Windows.Forms.ComboBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblInfo
			// 
			this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInfo.Location = new System.Drawing.Point(12, 9);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(276, 48);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.Text = "What happen if all media done encoding?";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cboShutdown
			// 
			this.cboShutdown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboShutdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboShutdown.FormattingEnabled = true;
			this.cboShutdown.Items.AddRange(new object[] {
            "Do Noting",
            "Reboot",
            "Shutdown"});
			this.cboShutdown.Location = new System.Drawing.Point(15, 60);
			this.cboShutdown.Name = "cboShutdown";
			this.cboShutdown.Size = new System.Drawing.Size(276, 21);
			this.cboShutdown.TabIndex = 1;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(216, 87);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmShutdown
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(300, 122);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.cboShutdown);
			this.Controls.Add(this.lblInfo);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmShutdown";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Shutdown Option";
			this.Load += new System.EventHandler(this.frmShutdown_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.ComboBox cboShutdown;
		private System.Windows.Forms.Button btnOK;
	}
}