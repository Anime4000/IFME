namespace ifme.framework
{
	partial class frmImgSequence
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
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lstImages = new System.Windows.Forms.ListView();
			this.chFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.chLocation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cboFps = new System.Windows.Forms.ComboBox();
			this.lblFps = new System.Windows.Forms.Label();
			this.lblCount = new System.Windows.Forms.Label();
			this.cboPixFmt = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(537, 407);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(456, 407);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnClear
			// 
			this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClear.Enabled = false;
			this.btnClear.Image = global::ifme.framework.Properties.Resources.list_clear;
			this.btnClear.Location = new System.Drawing.Point(588, 12);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(24, 24);
			this.btnClear.TabIndex = 2;
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.Image = global::ifme.framework.Properties.Resources.list_add;
			this.btnAdd.Location = new System.Drawing.Point(558, 12);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(24, 24);
			this.btnAdd.TabIndex = 4;
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// lstImages
			// 
			this.lstImages.AllowDrop = true;
			this.lstImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstImages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFiles,
            this.chLocation});
			this.lstImages.FullRowSelect = true;
			this.lstImages.Location = new System.Drawing.Point(12, 42);
			this.lstImages.Name = "lstImages";
			this.lstImages.Size = new System.Drawing.Size(600, 359);
			this.lstImages.TabIndex = 5;
			this.lstImages.UseCompatibleStateImageBehavior = false;
			this.lstImages.View = System.Windows.Forms.View.Details;
			// 
			// chFiles
			// 
			this.chFiles.Text = "Files";
			this.chFiles.Width = 250;
			// 
			// chLocation
			// 
			this.chLocation.Text = "Location";
			this.chLocation.Width = 300;
			// 
			// cboFps
			// 
			this.cboFps.FormattingEnabled = true;
			this.cboFps.Items.AddRange(new object[] {
            "12",
            "15",
            "23",
            "23.976",
            "24",
            "25",
            "29.97",
            "30",
            "48",
            "50",
            "60"});
			this.cboFps.Location = new System.Drawing.Point(44, 12);
			this.cboFps.Name = "cboFps";
			this.cboFps.Size = new System.Drawing.Size(60, 21);
			this.cboFps.TabIndex = 6;
			this.cboFps.Text = "30";
			// 
			// lblFps
			// 
			this.lblFps.AutoSize = true;
			this.lblFps.Location = new System.Drawing.Point(12, 15);
			this.lblFps.Name = "lblFps";
			this.lblFps.Size = new System.Drawing.Size(26, 13);
			this.lblFps.TabIndex = 7;
			this.lblFps.Text = "fps:";
			// 
			// lblCount
			// 
			this.lblCount.AutoSize = true;
			this.lblCount.Location = new System.Drawing.Point(12, 412);
			this.lblCount.Name = "lblCount";
			this.lblCount.Size = new System.Drawing.Size(35, 13);
			this.lblCount.TabIndex = 12;
			this.lblCount.Text = "Total:";
			// 
			// cboPixFmt
			// 
			this.cboPixFmt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPixFmt.FormattingEnabled = true;
			this.cboPixFmt.Items.AddRange(new object[] {
            "yuv420p",
            "yuv422p",
            "yuv444p",
            "yuv420p10le",
            "yuv422p10le",
            "yuv444p10le"});
			this.cboPixFmt.Location = new System.Drawing.Point(110, 12);
			this.cboPixFmt.Name = "cboPixFmt";
			this.cboPixFmt.Size = new System.Drawing.Size(121, 21);
			this.cboPixFmt.TabIndex = 13;
			// 
			// frmImgSequence
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 442);
			this.Controls.Add(this.cboPixFmt);
			this.Controls.Add(this.lblCount);
			this.Controls.Add(this.lblFps);
			this.Controls.Add(this.cboFps);
			this.Controls.Add(this.lstImages);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "frmImgSequence";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Generate Image Sequence (English)";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmImgSequence_FormClosing);
			this.Load += new System.EventHandler(this.frmImgSequence_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.ListView lstImages;
		private System.Windows.Forms.ColumnHeader chFiles;
		private System.Windows.Forms.ComboBox cboFps;
		private System.Windows.Forms.Label lblFps;
		private System.Windows.Forms.ColumnHeader chLocation;
		private System.Windows.Forms.Label lblCount;
		private System.Windows.Forms.ComboBox cboPixFmt;
	}
}