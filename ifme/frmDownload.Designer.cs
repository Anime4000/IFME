namespace ifme
{
	partial class frmDownload
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
			this.lblFile = new System.Windows.Forms.Label();
			this.lblSave = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.pbDownload = new System.Windows.Forms.ProgressBar();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// lblFile
			// 
			this.lblFile.Location = new System.Drawing.Point(34, 12);
			this.lblFile.Name = "lblFile";
			this.lblFile.Size = new System.Drawing.Size(588, 16);
			this.lblFile.TabIndex = 0;
			this.lblFile.Text = "File:";
			this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblSave
			// 
			this.lblSave.Location = new System.Drawing.Point(34, 34);
			this.lblSave.Name = "lblSave";
			this.lblSave.Size = new System.Drawing.Size(588, 16);
			this.lblSave.TabIndex = 1;
			this.lblSave.Text = "Save:";
			this.lblSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(9, 71);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(38, 13);
			this.lblStatus.TabIndex = 2;
			this.lblStatus.Text = "Status";
			// 
			// pbDownload
			// 
			this.pbDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pbDownload.Location = new System.Drawing.Point(12, 87);
			this.pbDownload.Name = "pbDownload";
			this.pbDownload.Size = new System.Drawing.Size(610, 23);
			this.pbDownload.TabIndex = 3;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::ifme.Properties.Resources.applications_internet;
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::ifme.Properties.Resources.disk;
			this.pictureBox2.Location = new System.Drawing.Point(12, 34);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(16, 16);
			this.pictureBox2.TabIndex = 5;
			this.pictureBox2.TabStop = false;
			// 
			// frmDownload
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(634, 122);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.pbDownload);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.lblSave);
			this.Controls.Add(this.lblFile);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "frmDownload";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Download";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDownload_FormClosing);
			this.Load += new System.EventHandler(this.frmDownload_Load);
			this.Shown += new System.EventHandler(this.frmDownload_Shown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblFile;
		private System.Windows.Forms.Label lblSave;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.ProgressBar pbDownload;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
	}
}