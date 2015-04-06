namespace ifme.hitoha
{
	partial class frmAviSynthHFR
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
			this.lblPreset = new System.Windows.Forms.Label();
			this.cboPreset = new System.Windows.Forms.ComboBox();
			this.lblTuning = new System.Windows.Forms.Label();
			this.cboTuning = new System.Windows.Forms.ComboBox();
			this.chkUseGPU = new System.Windows.Forms.CheckBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.chkDoubleFps = new System.Windows.Forms.CheckBox();
			this.cboInputType = new System.Windows.Forms.ComboBox();
			this.lblInputType = new System.Windows.Forms.Label();
			this.lblInputTypeInfo = new System.Windows.Forms.Label();
			this.lblPresetInfo = new System.Windows.Forms.Label();
			this.lblTuningInfo = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblPreset
			// 
			this.lblPreset.AutoSize = true;
			this.lblPreset.Location = new System.Drawing.Point(12, 122);
			this.lblPreset.Name = "lblPreset";
			this.lblPreset.Size = new System.Drawing.Size(42, 13);
			this.lblPreset.TabIndex = 1;
			this.lblPreset.Text = "Preset:";
			// 
			// cboPreset
			// 
			this.cboPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPreset.FormattingEnabled = true;
			this.cboPreset.Items.AddRange(new object[] {
            "Medium",
            "Fast",
            "Faster"});
			this.cboPreset.Location = new System.Drawing.Point(82, 119);
			this.cboPreset.Name = "cboPreset";
			this.cboPreset.Size = new System.Drawing.Size(121, 21);
			this.cboPreset.TabIndex = 1;
			this.cboPreset.SelectedIndexChanged += new System.EventHandler(this.cboPreset_SelectedIndexChanged);
			// 
			// lblTuning
			// 
			this.lblTuning.AutoSize = true;
			this.lblTuning.Location = new System.Drawing.Point(12, 15);
			this.lblTuning.Name = "lblTuning";
			this.lblTuning.Size = new System.Drawing.Size(43, 13);
			this.lblTuning.TabIndex = 2;
			this.lblTuning.Text = "Tuning:";
			// 
			// cboTuning
			// 
			this.cboTuning.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTuning.FormattingEnabled = true;
			this.cboTuning.Items.AddRange(new object[] {
            "Film",
            "Animation",
            "Smooth",
            "Weak"});
			this.cboTuning.Location = new System.Drawing.Point(82, 12);
			this.cboTuning.Name = "cboTuning";
			this.cboTuning.Size = new System.Drawing.Size(121, 21);
			this.cboTuning.TabIndex = 2;
			this.cboTuning.SelectedIndexChanged += new System.EventHandler(this.cboTuning_SelectedIndexChanged);
			// 
			// chkUseGPU
			// 
			this.chkUseGPU.AutoSize = true;
			this.chkUseGPU.Location = new System.Drawing.Point(27, 30);
			this.chkUseGPU.Name = "chkUseGPU";
			this.chkUseGPU.Size = new System.Drawing.Size(128, 17);
			this.chkUseGPU.TabIndex = 3;
			this.chkUseGPU.Text = "Use &GPU acceleration";
			this.chkUseGPU.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(472, 445);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(553, 445);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// chkDoubleFps
			// 
			this.chkDoubleFps.AutoSize = true;
			this.chkDoubleFps.Location = new System.Drawing.Point(27, 53);
			this.chkDoubleFps.Name = "chkDoubleFps";
			this.chkDoubleFps.Size = new System.Drawing.Size(158, 17);
			this.chkDoubleFps.TabIndex = 4;
			this.chkDoubleFps.Text = "&Double fps instead of 60fps";
			this.chkDoubleFps.UseVisualStyleBackColor = true;
			// 
			// cboInputType
			// 
			this.cboInputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboInputType.FormattingEnabled = true;
			this.cboInputType.Items.AddRange(new object[] {
            "2D",
            "SBS",
            "OU",
            "HSBS",
            "HOU"});
			this.cboInputType.Location = new System.Drawing.Point(82, 226);
			this.cboInputType.Name = "cboInputType";
			this.cboInputType.Size = new System.Drawing.Size(121, 21);
			this.cboInputType.TabIndex = 0;
			this.cboInputType.SelectedIndexChanged += new System.EventHandler(this.cboInputType_SelectedIndexChanged);
			// 
			// lblInputType
			// 
			this.lblInputType.AutoSize = true;
			this.lblInputType.Location = new System.Drawing.Point(12, 229);
			this.lblInputType.Name = "lblInputType";
			this.lblInputType.Size = new System.Drawing.Size(64, 13);
			this.lblInputType.TabIndex = 0;
			this.lblInputType.Text = "Input Type:";
			// 
			// lblInputTypeInfo
			// 
			this.lblInputTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInputTypeInfo.Location = new System.Drawing.Point(12, 250);
			this.lblInputTypeInfo.Name = "lblInputTypeInfo";
			this.lblInputTypeInfo.Size = new System.Drawing.Size(616, 80);
			this.lblInputTypeInfo.TabIndex = 0;
			this.lblInputTypeInfo.Text = "Info";
			// 
			// lblPresetInfo
			// 
			this.lblPresetInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPresetInfo.Location = new System.Drawing.Point(12, 143);
			this.lblPresetInfo.Name = "lblPresetInfo";
			this.lblPresetInfo.Size = new System.Drawing.Size(616, 80);
			this.lblPresetInfo.TabIndex = 1;
			this.lblPresetInfo.Text = "Info";
			// 
			// lblTuningInfo
			// 
			this.lblTuningInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTuningInfo.Location = new System.Drawing.Point(12, 36);
			this.lblTuningInfo.Name = "lblTuningInfo";
			this.lblTuningInfo.Size = new System.Drawing.Size(616, 80);
			this.lblTuningInfo.TabIndex = 2;
			this.lblTuningInfo.Text = "Info";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.chkUseGPU);
			this.groupBox1.Controls.Add(this.chkDoubleFps);
			this.groupBox1.Location = new System.Drawing.Point(12, 349);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(616, 90);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Feature";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Enabled = false;
			this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Italic);
			this.label1.Location = new System.Drawing.Point(12, 450);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(219, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Plugins by SVP Team, SubJunk, spirton.com.";
			// 
			// frmAviSynthHFR
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lblTuningInfo);
			this.Controls.Add(this.lblPresetInfo);
			this.Controls.Add(this.lblInputTypeInfo);
			this.Controls.Add(this.lblInputType);
			this.Controls.Add(this.cboInputType);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.cboTuning);
			this.Controls.Add(this.lblTuning);
			this.Controls.Add(this.cboPreset);
			this.Controls.Add(this.lblPreset);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.MinimumSize = new System.Drawing.Size(656, 518);
			this.Name = "frmAviSynthHFR";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AviSynth: High Frame Rate by (EXPERIMENTAL)";
			this.Load += new System.EventHandler(this.frmAviSynthHFR_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblPreset;
		private System.Windows.Forms.ComboBox cboPreset;
		private System.Windows.Forms.Label lblTuning;
		private System.Windows.Forms.ComboBox cboTuning;
		private System.Windows.Forms.CheckBox chkUseGPU;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox chkDoubleFps;
		private System.Windows.Forms.ComboBox cboInputType;
		private System.Windows.Forms.Label lblInputType;
		private System.Windows.Forms.Label lblInputTypeInfo;
		private System.Windows.Forms.Label lblPresetInfo;
		private System.Windows.Forms.Label lblTuningInfo;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;

	}
}