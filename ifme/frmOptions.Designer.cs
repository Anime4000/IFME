namespace ifme
{
	partial class frmOptions
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tabOptions = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.grpPreview = new System.Windows.Forms.GroupBox();
			this.numDuration = new System.Windows.Forms.NumericUpDown();
			this.grpLog = new System.Windows.Forms.GroupBox();
			this.chkLogSave = new System.Windows.Forms.CheckBox();
			this.grpUpdate = new System.Windows.Forms.GroupBox();
			this.chkUpdate = new System.Windows.Forms.CheckBox();
			this.grpFormat = new System.Windows.Forms.GroupBox();
			this.rdoUseMkv = new System.Windows.Forms.RadioButton();
			this.rdoUseMp4 = new System.Windows.Forms.RadioButton();
			this.grpTemp = new System.Windows.Forms.GroupBox();
			this.btnTempFindFolder = new System.Windows.Forms.Button();
			this.txtTempDir = new System.Windows.Forms.TextBox();
			this.grpLang = new System.Windows.Forms.GroupBox();
			this.btnLangAdd = new System.Windows.Forms.Button();
			this.lblLangWho = new System.Windows.Forms.Label();
			this.lblLangDisplay = new System.Windows.Forms.Label();
			this.cboLang = new System.Windows.Forms.ComboBox();
			this.tabPerformance = new System.Windows.Forms.TabPage();
			this.btnCPUBoost = new System.Windows.Forms.Button();
			this.lblCPUInfo = new System.Windows.Forms.Label();
			this.lblCPUAffinity = new System.Windows.Forms.Label();
			this.clbCPU = new System.Windows.Forms.CheckedListBox();
			this.lblCPUPriority = new System.Windows.Forms.Label();
			this.cboPerf = new System.Windows.Forms.ComboBox();
			this.tabAddons = new System.Windows.Forms.TabPage();
			this.btnAddonRemove = new System.Windows.Forms.Button();
			this.btnAddonInstall = new System.Windows.Forms.Button();
			this.lstAddons = new System.Windows.Forms.ListView();
			this.colAddID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAddName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAddVer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAddDev = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAddMaintainBy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnResetSettings = new System.Windows.Forms.Button();
			this.grpTag = new System.Windows.Forms.GroupBox();
			this.txtTag = new System.Windows.Forms.TextBox();
			this.tabOptions.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.grpPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
			this.grpLog.SuspendLayout();
			this.grpUpdate.SuspendLayout();
			this.grpFormat.SuspendLayout();
			this.grpTemp.SuspendLayout();
			this.grpLang.SuspendLayout();
			this.tabPerformance.SuspendLayout();
			this.tabAddons.SuspendLayout();
			this.grpTag.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(512, 407);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "{0}";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(406, 407);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "{0}";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// tabOptions
			// 
			this.tabOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabOptions.Controls.Add(this.tabGeneral);
			this.tabOptions.Controls.Add(this.tabPerformance);
			this.tabOptions.Controls.Add(this.tabAddons);
			this.tabOptions.Location = new System.Drawing.Point(12, 12);
			this.tabOptions.Name = "tabOptions";
			this.tabOptions.SelectedIndex = 0;
			this.tabOptions.Size = new System.Drawing.Size(600, 389);
			this.tabOptions.TabIndex = 3;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.grpTag);
			this.tabGeneral.Controls.Add(this.grpPreview);
			this.tabGeneral.Controls.Add(this.grpLog);
			this.tabGeneral.Controls.Add(this.grpUpdate);
			this.tabGeneral.Controls.Add(this.grpFormat);
			this.tabGeneral.Controls.Add(this.grpTemp);
			this.tabGeneral.Controls.Add(this.grpLang);
			this.tabGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabGeneral.Size = new System.Drawing.Size(592, 363);
			this.tabGeneral.TabIndex = 0;
			this.tabGeneral.Text = "{0}";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// grpPreview
			// 
			this.grpPreview.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpPreview.Controls.Add(this.numDuration);
			this.grpPreview.Location = new System.Drawing.Point(173, 216);
			this.grpPreview.Name = "grpPreview";
			this.grpPreview.Size = new System.Drawing.Size(120, 72);
			this.grpPreview.TabIndex = 5;
			this.grpPreview.TabStop = false;
			this.grpPreview.Text = "{0}";
			// 
			// numDuration
			// 
			this.numDuration.Location = new System.Drawing.Point(6, 29);
			this.numDuration.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
			this.numDuration.Name = "numDuration";
			this.numDuration.Size = new System.Drawing.Size(108, 20);
			this.numDuration.TabIndex = 0;
			// 
			// grpLog
			// 
			this.grpLog.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpLog.Controls.Add(this.chkLogSave);
			this.grpLog.Location = new System.Drawing.Point(6, 294);
			this.grpLog.Name = "grpLog";
			this.grpLog.Size = new System.Drawing.Size(287, 63);
			this.grpLog.TabIndex = 4;
			this.grpLog.TabStop = false;
			this.grpLog.Text = "{0}";
			// 
			// chkLogSave
			// 
			this.chkLogSave.AutoSize = true;
			this.chkLogSave.Location = new System.Drawing.Point(6, 27);
			this.chkLogSave.Name = "chkLogSave";
			this.chkLogSave.Size = new System.Drawing.Size(42, 17);
			this.chkLogSave.TabIndex = 0;
			this.chkLogSave.Text = "{0}";
			this.chkLogSave.UseVisualStyleBackColor = true;
			// 
			// grpUpdate
			// 
			this.grpUpdate.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpUpdate.Controls.Add(this.chkUpdate);
			this.grpUpdate.Location = new System.Drawing.Point(299, 216);
			this.grpUpdate.Name = "grpUpdate";
			this.grpUpdate.Size = new System.Drawing.Size(287, 72);
			this.grpUpdate.TabIndex = 3;
			this.grpUpdate.TabStop = false;
			this.grpUpdate.Text = "{0}";
			// 
			// chkUpdate
			// 
			this.chkUpdate.Checked = true;
			this.chkUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUpdate.Location = new System.Drawing.Point(6, 19);
			this.chkUpdate.Name = "chkUpdate";
			this.chkUpdate.Size = new System.Drawing.Size(275, 47);
			this.chkUpdate.TabIndex = 0;
			this.chkUpdate.Text = "{0}";
			this.chkUpdate.UseVisualStyleBackColor = true;
			// 
			// grpFormat
			// 
			this.grpFormat.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpFormat.Controls.Add(this.rdoUseMkv);
			this.grpFormat.Controls.Add(this.rdoUseMp4);
			this.grpFormat.Location = new System.Drawing.Point(6, 216);
			this.grpFormat.Name = "grpFormat";
			this.grpFormat.Size = new System.Drawing.Size(161, 72);
			this.grpFormat.TabIndex = 2;
			this.grpFormat.TabStop = false;
			this.grpFormat.Text = "{0}";
			// 
			// rdoUseMkv
			// 
			this.rdoUseMkv.AutoSize = true;
			this.rdoUseMkv.Checked = true;
			this.rdoUseMkv.Location = new System.Drawing.Point(6, 20);
			this.rdoUseMkv.Name = "rdoUseMkv";
			this.rdoUseMkv.Size = new System.Drawing.Size(72, 17);
			this.rdoUseMkv.TabIndex = 1;
			this.rdoUseMkv.TabStop = true;
			this.rdoUseMkv.Text = "MKV ({0})";
			this.rdoUseMkv.UseVisualStyleBackColor = true;
			// 
			// rdoUseMp4
			// 
			this.rdoUseMp4.AutoSize = true;
			this.rdoUseMp4.Location = new System.Drawing.Point(6, 43);
			this.rdoUseMp4.Name = "rdoUseMp4";
			this.rdoUseMp4.Size = new System.Drawing.Size(45, 17);
			this.rdoUseMp4.TabIndex = 0;
			this.rdoUseMp4.Text = "MP4";
			this.rdoUseMp4.UseVisualStyleBackColor = true;
			// 
			// grpTemp
			// 
			this.grpTemp.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpTemp.Controls.Add(this.btnTempFindFolder);
			this.grpTemp.Controls.Add(this.txtTempDir);
			this.grpTemp.Location = new System.Drawing.Point(6, 110);
			this.grpTemp.Name = "grpTemp";
			this.grpTemp.Size = new System.Drawing.Size(580, 100);
			this.grpTemp.TabIndex = 1;
			this.grpTemp.TabStop = false;
			this.grpTemp.Text = "{0}";
			// 
			// btnTempFindFolder
			// 
			this.btnTempFindFolder.Location = new System.Drawing.Point(458, 39);
			this.btnTempFindFolder.Name = "btnTempFindFolder";
			this.btnTempFindFolder.Size = new System.Drawing.Size(32, 23);
			this.btnTempFindFolder.TabIndex = 1;
			this.btnTempFindFolder.Text = "...";
			this.btnTempFindFolder.UseVisualStyleBackColor = true;
			this.btnTempFindFolder.Click += new System.EventHandler(this.btnTempFindFolder_Click);
			// 
			// txtTempDir
			// 
			this.txtTempDir.Location = new System.Drawing.Point(90, 40);
			this.txtTempDir.Name = "txtTempDir";
			this.txtTempDir.ReadOnly = true;
			this.txtTempDir.Size = new System.Drawing.Size(362, 20);
			this.txtTempDir.TabIndex = 0;
			// 
			// grpLang
			// 
			this.grpLang.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpLang.Controls.Add(this.btnLangAdd);
			this.grpLang.Controls.Add(this.lblLangWho);
			this.grpLang.Controls.Add(this.lblLangDisplay);
			this.grpLang.Controls.Add(this.cboLang);
			this.grpLang.Location = new System.Drawing.Point(6, 4);
			this.grpLang.Name = "grpLang";
			this.grpLang.Size = new System.Drawing.Size(580, 100);
			this.grpLang.TabIndex = 0;
			this.grpLang.TabStop = false;
			this.grpLang.Text = "{0}";
			// 
			// btnLangAdd
			// 
			this.btnLangAdd.Location = new System.Drawing.Point(467, 19);
			this.btnLangAdd.Name = "btnLangAdd";
			this.btnLangAdd.Size = new System.Drawing.Size(23, 23);
			this.btnLangAdd.TabIndex = 3;
			this.btnLangAdd.Text = "+";
			this.btnLangAdd.UseVisualStyleBackColor = true;
			this.btnLangAdd.Click += new System.EventHandler(this.btnLangAdd_Click);
			// 
			// lblLangWho
			// 
			this.lblLangWho.Location = new System.Drawing.Point(196, 44);
			this.lblLangWho.Name = "lblLangWho";
			this.lblLangWho.Size = new System.Drawing.Size(294, 40);
			this.lblLangWho.TabIndex = 2;
			this.lblLangWho.Text = "{0}\r\n{1}\r\n{2}";
			// 
			// lblLangDisplay
			// 
			this.lblLangDisplay.Location = new System.Drawing.Point(90, 44);
			this.lblLangDisplay.Name = "lblLangDisplay";
			this.lblLangDisplay.Size = new System.Drawing.Size(100, 40);
			this.lblLangDisplay.TabIndex = 1;
			this.lblLangDisplay.Text = "{0}:\r\n{1}:\r\n{2}:";
			this.lblLangDisplay.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cboLang
			// 
			this.cboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLang.FormattingEnabled = true;
			this.cboLang.Location = new System.Drawing.Point(90, 20);
			this.cboLang.Name = "cboLang";
			this.cboLang.Size = new System.Drawing.Size(371, 21);
			this.cboLang.TabIndex = 0;
			this.cboLang.SelectedIndexChanged += new System.EventHandler(this.cboLang_SelectedIndexChanged);
			// 
			// tabPerformance
			// 
			this.tabPerformance.Controls.Add(this.btnCPUBoost);
			this.tabPerformance.Controls.Add(this.lblCPUInfo);
			this.tabPerformance.Controls.Add(this.lblCPUAffinity);
			this.tabPerformance.Controls.Add(this.clbCPU);
			this.tabPerformance.Controls.Add(this.lblCPUPriority);
			this.tabPerformance.Controls.Add(this.cboPerf);
			this.tabPerformance.Location = new System.Drawing.Point(4, 22);
			this.tabPerformance.Name = "tabPerformance";
			this.tabPerformance.Padding = new System.Windows.Forms.Padding(3);
			this.tabPerformance.Size = new System.Drawing.Size(592, 363);
			this.tabPerformance.TabIndex = 1;
			this.tabPerformance.Text = "{0}";
			this.tabPerformance.UseVisualStyleBackColor = true;
			// 
			// btnCPUBoost
			// 
			this.btnCPUBoost.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnCPUBoost.Location = new System.Drawing.Point(221, 260);
			this.btnCPUBoost.Name = "btnCPUBoost";
			this.btnCPUBoost.Size = new System.Drawing.Size(150, 23);
			this.btnCPUBoost.TabIndex = 5;
			this.btnCPUBoost.Text = "{0}";
			this.btnCPUBoost.UseVisualStyleBackColor = true;
			this.btnCPUBoost.Click += new System.EventHandler(this.btnCPUBoost_Click);
			// 
			// lblCPUInfo
			// 
			this.lblCPUInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblCPUInfo.Location = new System.Drawing.Point(6, 336);
			this.lblCPUInfo.Name = "lblCPUInfo";
			this.lblCPUInfo.Size = new System.Drawing.Size(580, 26);
			this.lblCPUInfo.TabIndex = 4;
			this.lblCPUInfo.Text = "Tips: {0}";
			// 
			// lblCPUAffinity
			// 
			this.lblCPUAffinity.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblCPUAffinity.AutoSize = true;
			this.lblCPUAffinity.Location = new System.Drawing.Point(218, 112);
			this.lblCPUAffinity.Name = "lblCPUAffinity";
			this.lblCPUAffinity.Size = new System.Drawing.Size(23, 13);
			this.lblCPUAffinity.TabIndex = 3;
			this.lblCPUAffinity.Text = "{0}";
			// 
			// clbCPU
			// 
			this.clbCPU.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.clbCPU.CheckOnClick = true;
			this.clbCPU.FormattingEnabled = true;
			this.clbCPU.Location = new System.Drawing.Point(221, 128);
			this.clbCPU.Name = "clbCPU";
			this.clbCPU.Size = new System.Drawing.Size(150, 109);
			this.clbCPU.TabIndex = 2;
			// 
			// lblCPUPriority
			// 
			this.lblCPUPriority.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblCPUPriority.AutoSize = true;
			this.lblCPUPriority.Location = new System.Drawing.Point(218, 72);
			this.lblCPUPriority.Name = "lblCPUPriority";
			this.lblCPUPriority.Size = new System.Drawing.Size(23, 13);
			this.lblCPUPriority.TabIndex = 1;
			this.lblCPUPriority.Text = "{0}";
			this.lblCPUPriority.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cboPerf
			// 
			this.cboPerf.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPerf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPerf.FormattingEnabled = true;
			this.cboPerf.Items.AddRange(new object[] {
            "REALTIME",
            "HIGH",
            "ABOVE NORMAL",
            "NORMAL",
            "BELOW NORMAL",
            "LOW"});
			this.cboPerf.Location = new System.Drawing.Point(221, 88);
			this.cboPerf.Name = "cboPerf";
			this.cboPerf.Size = new System.Drawing.Size(150, 21);
			this.cboPerf.TabIndex = 0;
			// 
			// tabAddons
			// 
			this.tabAddons.Controls.Add(this.btnAddonRemove);
			this.tabAddons.Controls.Add(this.btnAddonInstall);
			this.tabAddons.Controls.Add(this.lstAddons);
			this.tabAddons.Location = new System.Drawing.Point(4, 22);
			this.tabAddons.Name = "tabAddons";
			this.tabAddons.Padding = new System.Windows.Forms.Padding(3);
			this.tabAddons.Size = new System.Drawing.Size(592, 363);
			this.tabAddons.TabIndex = 2;
			this.tabAddons.Text = "{0}";
			this.tabAddons.UseVisualStyleBackColor = true;
			// 
			// btnAddonRemove
			// 
			this.btnAddonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAddonRemove.Location = new System.Drawing.Point(152, 334);
			this.btnAddonRemove.Name = "btnAddonRemove";
			this.btnAddonRemove.Size = new System.Drawing.Size(100, 23);
			this.btnAddonRemove.TabIndex = 2;
			this.btnAddonRemove.Text = "{0}";
			this.btnAddonRemove.UseVisualStyleBackColor = true;
			this.btnAddonRemove.Click += new System.EventHandler(this.btnAddonRemove_Click);
			// 
			// btnAddonInstall
			// 
			this.btnAddonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAddonInstall.Location = new System.Drawing.Point(6, 334);
			this.btnAddonInstall.Name = "btnAddonInstall";
			this.btnAddonInstall.Size = new System.Drawing.Size(140, 23);
			this.btnAddonInstall.TabIndex = 1;
			this.btnAddonInstall.Text = "{0}";
			this.btnAddonInstall.UseVisualStyleBackColor = true;
			this.btnAddonInstall.Click += new System.EventHandler(this.btnAddonInstall_Click);
			// 
			// lstAddons
			// 
			this.lstAddons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstAddons.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAddID,
            this.colAddName,
            this.colAddVer,
            this.colAddDev,
            this.colAddMaintainBy});
			this.lstAddons.FullRowSelect = true;
			this.lstAddons.Location = new System.Drawing.Point(6, 6);
			this.lstAddons.Name = "lstAddons";
			this.lstAddons.Size = new System.Drawing.Size(580, 322);
			this.lstAddons.TabIndex = 0;
			this.lstAddons.UseCompatibleStateImageBehavior = false;
			this.lstAddons.View = System.Windows.Forms.View.Details;
			// 
			// colAddID
			// 
			this.colAddID.Text = "{0}";
			this.colAddID.Width = 30;
			// 
			// colAddName
			// 
			this.colAddName.Text = "{0}";
			this.colAddName.Width = 170;
			// 
			// colAddVer
			// 
			this.colAddVer.Text = "{0}";
			this.colAddVer.Width = 131;
			// 
			// colAddDev
			// 
			this.colAddDev.Text = "{0}";
			this.colAddDev.Width = 145;
			// 
			// colAddMaintainBy
			// 
			this.colAddMaintainBy.Text = "{0}";
			this.colAddMaintainBy.Width = 100;
			// 
			// btnResetSettings
			// 
			this.btnResetSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnResetSettings.AutoSize = true;
			this.btnResetSettings.Location = new System.Drawing.Point(12, 407);
			this.btnResetSettings.Name = "btnResetSettings";
			this.btnResetSettings.Size = new System.Drawing.Size(200, 23);
			this.btnResetSettings.TabIndex = 6;
			this.btnResetSettings.Text = "{0}";
			this.btnResetSettings.UseVisualStyleBackColor = true;
			this.btnResetSettings.Click += new System.EventHandler(this.btnResetSettings_Click);
			// 
			// grpTag
			// 
			this.grpTag.Controls.Add(this.txtTag);
			this.grpTag.Location = new System.Drawing.Point(299, 294);
			this.grpTag.Name = "grpTag";
			this.grpTag.Size = new System.Drawing.Size(287, 63);
			this.grpTag.TabIndex = 6;
			this.grpTag.TabStop = false;
			this.grpTag.Text = "Tag";
			// 
			// txtTag
			// 
			this.txtTag.Location = new System.Drawing.Point(6, 25);
			this.txtTag.Name = "txtTag";
			this.txtTag.Size = new System.Drawing.Size(275, 20);
			this.txtTag.TabIndex = 0;
			// 
			// frmOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(624, 442);
			this.Controls.Add(this.btnResetSettings);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tabOptions);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "frmOptions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "{0}";
			this.Load += new System.EventHandler(this.frmOptions_Load);
			this.tabOptions.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.grpPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
			this.grpLog.ResumeLayout(false);
			this.grpLog.PerformLayout();
			this.grpUpdate.ResumeLayout(false);
			this.grpFormat.ResumeLayout(false);
			this.grpFormat.PerformLayout();
			this.grpTemp.ResumeLayout(false);
			this.grpTemp.PerformLayout();
			this.grpLang.ResumeLayout(false);
			this.tabPerformance.ResumeLayout(false);
			this.tabPerformance.PerformLayout();
			this.tabAddons.ResumeLayout(false);
			this.grpTag.ResumeLayout(false);
			this.grpTag.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TabControl tabOptions;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.GroupBox grpUpdate;
		private System.Windows.Forms.CheckBox chkUpdate;
		private System.Windows.Forms.GroupBox grpFormat;
		private System.Windows.Forms.RadioButton rdoUseMkv;
		private System.Windows.Forms.RadioButton rdoUseMp4;
		private System.Windows.Forms.GroupBox grpTemp;
		private System.Windows.Forms.Button btnTempFindFolder;
		private System.Windows.Forms.TextBox txtTempDir;
		private System.Windows.Forms.GroupBox grpLang;
		private System.Windows.Forms.Label lblLangWho;
		private System.Windows.Forms.Label lblLangDisplay;
		private System.Windows.Forms.ComboBox cboLang;
		private System.Windows.Forms.TabPage tabPerformance;
		private System.Windows.Forms.Button btnCPUBoost;
		private System.Windows.Forms.Label lblCPUInfo;
		private System.Windows.Forms.Label lblCPUAffinity;
		private System.Windows.Forms.CheckedListBox clbCPU;
		private System.Windows.Forms.Label lblCPUPriority;
		private System.Windows.Forms.ComboBox cboPerf;
		private System.Windows.Forms.TabPage tabAddons;
		private System.Windows.Forms.Button btnAddonRemove;
		private System.Windows.Forms.Button btnAddonInstall;
		public System.Windows.Forms.ListView lstAddons;
		private System.Windows.Forms.ColumnHeader colAddID;
		private System.Windows.Forms.ColumnHeader colAddName;
		private System.Windows.Forms.ColumnHeader colAddVer;
		private System.Windows.Forms.ColumnHeader colAddDev;
		private System.Windows.Forms.ColumnHeader colAddMaintainBy;
		private System.Windows.Forms.Button btnLangAdd;
		private System.Windows.Forms.Button btnResetSettings;
		private System.Windows.Forms.GroupBox grpLog;
		private System.Windows.Forms.CheckBox chkLogSave;
		private System.Windows.Forms.GroupBox grpPreview;
		private System.Windows.Forms.NumericUpDown numDuration;
		private System.Windows.Forms.GroupBox grpTag;
		private System.Windows.Forms.TextBox txtTag;
	}
}