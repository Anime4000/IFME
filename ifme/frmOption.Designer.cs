namespace ifme
{
	partial class frmOption
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOption));
			this.tabOption = new System.Windows.Forms.TabControl();
			this.tabOptionGeneral = new System.Windows.Forms.TabPage();
			this.grpNewFilename = new System.Windows.Forms.GroupBox();
			this.grpNewFilenamePostfix = new System.Windows.Forms.GroupBox();
			this.txtNamePostfix = new System.Windows.Forms.TextBox();
			this.rdoNamePostfixCustom = new System.Windows.Forms.RadioButton();
			this.rdoNamePostfixNone = new System.Windows.Forms.RadioButton();
			this.grpNewFilenamePrefix = new System.Windows.Forms.GroupBox();
			this.rdoNamePrefixNone = new System.Windows.Forms.RadioButton();
			this.txtNamePrefix = new System.Windows.Forms.TextBox();
			this.rdoNamePrefixDateTime = new System.Windows.Forms.RadioButton();
			this.rdoNamePrefixCustom = new System.Windows.Forms.RadioButton();
			this.grpTempFolder = new System.Windows.Forms.GroupBox();
			this.btnTempPath = new System.Windows.Forms.Button();
			this.txtTempPath = new System.Windows.Forms.TextBox();
			this.grpLanguage = new System.Windows.Forms.GroupBox();
			this.lblLanguageAuthor = new System.Windows.Forms.Label();
			this.cboLanguage = new System.Windows.Forms.ComboBox();
			this.tabOptionEncoding = new System.Windows.Forms.TabPage();
			this.grpAviSynth = new System.Windows.Forms.GroupBox();
			this.lblAviSynthVersion = new System.Windows.Forms.Label();
			this.lblAviSynthInstall = new System.Windows.Forms.Label();
			this.grpFFmpeg = new System.Windows.Forms.GroupBox();
			this.rdoFFmpeg64 = new System.Windows.Forms.RadioButton();
			this.rdoFFmpeg32 = new System.Windows.Forms.RadioButton();
			this.tabOptionModule = new System.Windows.Forms.TabPage();
			this.lstModule = new System.Windows.Forms.ListView();
			this.colModName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colModArch = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colModAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.grpFrameCountOffset = new System.Windows.Forms.GroupBox();
			this.lblFrameCountOffset = new System.Windows.Forms.Label();
			this.nudFrameCountOffset = new System.Windows.Forms.NumericUpDown();
			this.tabOption.SuspendLayout();
			this.tabOptionGeneral.SuspendLayout();
			this.grpNewFilename.SuspendLayout();
			this.grpNewFilenamePostfix.SuspendLayout();
			this.grpNewFilenamePrefix.SuspendLayout();
			this.grpTempFolder.SuspendLayout();
			this.grpLanguage.SuspendLayout();
			this.tabOptionEncoding.SuspendLayout();
			this.grpAviSynth.SuspendLayout();
			this.grpFFmpeg.SuspendLayout();
			this.tabOptionModule.SuspendLayout();
			this.grpFrameCountOffset.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFrameCountOffset)).BeginInit();
			this.SuspendLayout();
			// 
			// tabOption
			// 
			this.tabOption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabOption.Controls.Add(this.tabOptionGeneral);
			this.tabOption.Controls.Add(this.tabOptionEncoding);
			this.tabOption.Controls.Add(this.tabOptionModule);
			this.tabOption.Location = new System.Drawing.Point(12, 12);
			this.tabOption.Name = "tabOption";
			this.tabOption.SelectedIndex = 0;
			this.tabOption.Size = new System.Drawing.Size(616, 427);
			this.tabOption.TabIndex = 0;
			// 
			// tabOptionGeneral
			// 
			this.tabOptionGeneral.Controls.Add(this.grpNewFilename);
			this.tabOptionGeneral.Controls.Add(this.grpTempFolder);
			this.tabOptionGeneral.Controls.Add(this.grpLanguage);
			this.tabOptionGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabOptionGeneral.Name = "tabOptionGeneral";
			this.tabOptionGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabOptionGeneral.Size = new System.Drawing.Size(608, 401);
			this.tabOptionGeneral.TabIndex = 0;
			this.tabOptionGeneral.Text = "General";
			this.tabOptionGeneral.UseVisualStyleBackColor = true;
			// 
			// grpNewFilename
			// 
			this.grpNewFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpNewFilename.Controls.Add(this.grpNewFilenamePostfix);
			this.grpNewFilename.Controls.Add(this.grpNewFilenamePrefix);
			this.grpNewFilename.Location = new System.Drawing.Point(6, 260);
			this.grpNewFilename.Name = "grpNewFilename";
			this.grpNewFilename.Size = new System.Drawing.Size(596, 135);
			this.grpNewFilename.TabIndex = 2;
			this.grpNewFilename.TabStop = false;
			this.grpNewFilename.Text = "&New filename";
			// 
			// grpNewFilenamePostfix
			// 
			this.grpNewFilenamePostfix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpNewFilenamePostfix.Controls.Add(this.txtNamePostfix);
			this.grpNewFilenamePostfix.Controls.Add(this.rdoNamePostfixCustom);
			this.grpNewFilenamePostfix.Controls.Add(this.rdoNamePostfixNone);
			this.grpNewFilenamePostfix.Location = new System.Drawing.Point(301, 19);
			this.grpNewFilenamePostfix.Name = "grpNewFilenamePostfix";
			this.grpNewFilenamePostfix.Size = new System.Drawing.Size(289, 110);
			this.grpNewFilenamePostfix.TabIndex = 5;
			this.grpNewFilenamePostfix.TabStop = false;
			this.grpNewFilenamePostfix.Text = "P&ostfix";
			// 
			// txtNamePostfix
			// 
			this.txtNamePostfix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtNamePostfix.Font = new System.Drawing.Font("Tahoma", 10F);
			this.txtNamePostfix.Location = new System.Drawing.Point(102, 58);
			this.txtNamePostfix.Name = "txtNamePostfix";
			this.txtNamePostfix.Size = new System.Drawing.Size(181, 24);
			this.txtNamePostfix.TabIndex = 2;
			// 
			// rdoNamePostfixCustom
			// 
			this.rdoNamePostfixCustom.Location = new System.Drawing.Point(6, 58);
			this.rdoNamePostfixCustom.Name = "rdoNamePostfixCustom";
			this.rdoNamePostfixCustom.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePostfixCustom.TabIndex = 1;
			this.rdoNamePostfixCustom.TabStop = true;
			this.rdoNamePostfixCustom.Text = "C&ustom:";
			this.rdoNamePostfixCustom.UseVisualStyleBackColor = true;
			// 
			// rdoNamePostfixNone
			// 
			this.rdoNamePostfixNone.Location = new System.Drawing.Point(6, 28);
			this.rdoNamePostfixNone.Name = "rdoNamePostfixNone";
			this.rdoNamePostfixNone.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePostfixNone.TabIndex = 0;
			this.rdoNamePostfixNone.TabStop = true;
			this.rdoNamePostfixNone.Text = "Non&e";
			this.rdoNamePostfixNone.UseVisualStyleBackColor = true;
			// 
			// grpNewFilenamePrefix
			// 
			this.grpNewFilenamePrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpNewFilenamePrefix.Controls.Add(this.rdoNamePrefixNone);
			this.grpNewFilenamePrefix.Controls.Add(this.txtNamePrefix);
			this.grpNewFilenamePrefix.Controls.Add(this.rdoNamePrefixDateTime);
			this.grpNewFilenamePrefix.Controls.Add(this.rdoNamePrefixCustom);
			this.grpNewFilenamePrefix.Location = new System.Drawing.Point(6, 19);
			this.grpNewFilenamePrefix.Name = "grpNewFilenamePrefix";
			this.grpNewFilenamePrefix.Size = new System.Drawing.Size(289, 110);
			this.grpNewFilenamePrefix.TabIndex = 4;
			this.grpNewFilenamePrefix.TabStop = false;
			this.grpNewFilenamePrefix.Text = "P&refix";
			// 
			// rdoNamePrefixNone
			// 
			this.rdoNamePrefixNone.Location = new System.Drawing.Point(6, 19);
			this.rdoNamePrefixNone.Name = "rdoNamePrefixNone";
			this.rdoNamePrefixNone.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePrefixNone.TabIndex = 0;
			this.rdoNamePrefixNone.TabStop = true;
			this.rdoNamePrefixNone.Text = "&None";
			this.rdoNamePrefixNone.UseVisualStyleBackColor = true;
			// 
			// txtNamePrefix
			// 
			this.txtNamePrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtNamePrefix.Font = new System.Drawing.Font("Tahoma", 10F);
			this.txtNamePrefix.Location = new System.Drawing.Point(102, 79);
			this.txtNamePrefix.Name = "txtNamePrefix";
			this.txtNamePrefix.Size = new System.Drawing.Size(181, 24);
			this.txtNamePrefix.TabIndex = 3;
			// 
			// rdoNamePrefixDateTime
			// 
			this.rdoNamePrefixDateTime.Location = new System.Drawing.Point(6, 49);
			this.rdoNamePrefixDateTime.Name = "rdoNamePrefixDateTime";
			this.rdoNamePrefixDateTime.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePrefixDateTime.TabIndex = 1;
			this.rdoNamePrefixDateTime.TabStop = true;
			this.rdoNamePrefixDateTime.Text = "&Date Time";
			this.rdoNamePrefixDateTime.UseVisualStyleBackColor = true;
			// 
			// rdoNamePrefixCustom
			// 
			this.rdoNamePrefixCustom.Location = new System.Drawing.Point(6, 79);
			this.rdoNamePrefixCustom.Name = "rdoNamePrefixCustom";
			this.rdoNamePrefixCustom.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePrefixCustom.TabIndex = 2;
			this.rdoNamePrefixCustom.TabStop = true;
			this.rdoNamePrefixCustom.Text = "&Custom:";
			this.rdoNamePrefixCustom.UseVisualStyleBackColor = true;
			// 
			// grpTempFolder
			// 
			this.grpTempFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpTempFolder.Controls.Add(this.btnTempPath);
			this.grpTempFolder.Controls.Add(this.txtTempPath);
			this.grpTempFolder.Location = new System.Drawing.Point(6, 138);
			this.grpTempFolder.Name = "grpTempFolder";
			this.grpTempFolder.Size = new System.Drawing.Size(596, 116);
			this.grpTempFolder.TabIndex = 1;
			this.grpTempFolder.TabStop = false;
			this.grpTempFolder.Text = "&Temporary folder";
			// 
			// btnTempPath
			// 
			this.btnTempPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTempPath.Image = global::ifme.Properties.Resources.icon16_document_save;
			this.btnTempPath.Location = new System.Drawing.Point(483, 46);
			this.btnTempPath.Name = "btnTempPath";
			this.btnTempPath.Size = new System.Drawing.Size(24, 24);
			this.btnTempPath.TabIndex = 1;
			this.btnTempPath.UseVisualStyleBackColor = true;
			this.btnTempPath.Click += new System.EventHandler(this.btnTempPath_Click);
			// 
			// txtTempPath
			// 
			this.txtTempPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtTempPath.Font = new System.Drawing.Font("Tahoma", 10F);
			this.txtTempPath.Location = new System.Drawing.Point(89, 46);
			this.txtTempPath.Name = "txtTempPath";
			this.txtTempPath.ReadOnly = true;
			this.txtTempPath.Size = new System.Drawing.Size(388, 24);
			this.txtTempPath.TabIndex = 0;
			// 
			// grpLanguage
			// 
			this.grpLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpLanguage.Controls.Add(this.lblLanguageAuthor);
			this.grpLanguage.Controls.Add(this.cboLanguage);
			this.grpLanguage.Location = new System.Drawing.Point(6, 6);
			this.grpLanguage.Name = "grpLanguage";
			this.grpLanguage.Size = new System.Drawing.Size(596, 126);
			this.grpLanguage.TabIndex = 0;
			this.grpLanguage.TabStop = false;
			this.grpLanguage.Text = "&Language";
			// 
			// lblLanguageAuthor
			// 
			this.lblLanguageAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblLanguageAuthor.Location = new System.Drawing.Point(89, 61);
			this.lblLanguageAuthor.Name = "lblLanguageAuthor";
			this.lblLanguageAuthor.Size = new System.Drawing.Size(418, 32);
			this.lblLanguageAuthor.TabIndex = 1;
			this.lblLanguageAuthor.Text = "Line 1\r\nLine 2";
			// 
			// cboLanguage
			// 
			this.cboLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLanguage.Font = new System.Drawing.Font("Tahoma", 10F);
			this.cboLanguage.FormattingEnabled = true;
			this.cboLanguage.Location = new System.Drawing.Point(89, 34);
			this.cboLanguage.Name = "cboLanguage";
			this.cboLanguage.Size = new System.Drawing.Size(418, 24);
			this.cboLanguage.TabIndex = 0;
			// 
			// tabOptionEncoding
			// 
			this.tabOptionEncoding.Controls.Add(this.grpFrameCountOffset);
			this.tabOptionEncoding.Controls.Add(this.grpAviSynth);
			this.tabOptionEncoding.Controls.Add(this.grpFFmpeg);
			this.tabOptionEncoding.Location = new System.Drawing.Point(4, 22);
			this.tabOptionEncoding.Name = "tabOptionEncoding";
			this.tabOptionEncoding.Padding = new System.Windows.Forms.Padding(3);
			this.tabOptionEncoding.Size = new System.Drawing.Size(608, 401);
			this.tabOptionEncoding.TabIndex = 1;
			this.tabOptionEncoding.Text = "Encoding";
			this.tabOptionEncoding.UseVisualStyleBackColor = true;
			// 
			// grpAviSynth
			// 
			this.grpAviSynth.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpAviSynth.Controls.Add(this.lblAviSynthVersion);
			this.grpAviSynth.Controls.Add(this.lblAviSynthInstall);
			this.grpAviSynth.Location = new System.Drawing.Point(6, 6);
			this.grpAviSynth.Name = "grpAviSynth";
			this.grpAviSynth.Size = new System.Drawing.Size(596, 150);
			this.grpAviSynth.TabIndex = 0;
			this.grpAviSynth.TabStop = false;
			this.grpAviSynth.Text = "&AviSynth";
			// 
			// lblAviSynthVersion
			// 
			this.lblAviSynthVersion.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAviSynthVersion.Font = new System.Drawing.Font("Tahoma", 10F);
			this.lblAviSynthVersion.Location = new System.Drawing.Point(148, 74);
			this.lblAviSynthVersion.Name = "lblAviSynthVersion";
			this.lblAviSynthVersion.Size = new System.Drawing.Size(300, 48);
			this.lblAviSynthVersion.TabIndex = 1;
			this.lblAviSynthVersion.Text = "Unknown Version";
			this.lblAviSynthVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblAviSynthInstall
			// 
			this.lblAviSynthInstall.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAviSynthInstall.Font = new System.Drawing.Font("Tahoma", 14F);
			this.lblAviSynthInstall.Location = new System.Drawing.Point(148, 26);
			this.lblAviSynthInstall.Name = "lblAviSynthInstall";
			this.lblAviSynthInstall.Size = new System.Drawing.Size(300, 48);
			this.lblAviSynthInstall.TabIndex = 0;
			this.lblAviSynthInstall.Text = "Not Found";
			this.lblAviSynthInstall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// grpFFmpeg
			// 
			this.grpFFmpeg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpFFmpeg.Controls.Add(this.rdoFFmpeg64);
			this.grpFFmpeg.Controls.Add(this.rdoFFmpeg32);
			this.grpFFmpeg.Location = new System.Drawing.Point(6, 162);
			this.grpFFmpeg.Name = "grpFFmpeg";
			this.grpFFmpeg.Size = new System.Drawing.Size(596, 100);
			this.grpFFmpeg.TabIndex = 1;
			this.grpFFmpeg.TabStop = false;
			this.grpFFmpeg.Text = "&Default Decoder";
			// 
			// rdoFFmpeg64
			// 
			this.rdoFFmpeg64.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rdoFFmpeg64.Location = new System.Drawing.Point(168, 53);
			this.rdoFFmpeg64.Name = "rdoFFmpeg64";
			this.rdoFFmpeg64.Size = new System.Drawing.Size(300, 24);
			this.rdoFFmpeg64.TabIndex = 1;
			this.rdoFFmpeg64.TabStop = true;
			this.rdoFFmpeg64.Text = "FFmpeg &64bit (Support large resolution && high bitdepth)";
			this.rdoFFmpeg64.UseVisualStyleBackColor = true;
			// 
			// rdoFFmpeg32
			// 
			this.rdoFFmpeg32.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.rdoFFmpeg32.Location = new System.Drawing.Point(168, 23);
			this.rdoFFmpeg32.Name = "rdoFFmpeg32";
			this.rdoFFmpeg32.Size = new System.Drawing.Size(300, 24);
			this.rdoFFmpeg32.TabIndex = 0;
			this.rdoFFmpeg32.TabStop = true;
			this.rdoFFmpeg32.Text = "FFmpeg &32bit (Support AviSynth)";
			this.rdoFFmpeg32.UseVisualStyleBackColor = true;
			// 
			// tabOptionModule
			// 
			this.tabOptionModule.Controls.Add(this.lstModule);
			this.tabOptionModule.Location = new System.Drawing.Point(4, 22);
			this.tabOptionModule.Name = "tabOptionModule";
			this.tabOptionModule.Padding = new System.Windows.Forms.Padding(3);
			this.tabOptionModule.Size = new System.Drawing.Size(608, 401);
			this.tabOptionModule.TabIndex = 2;
			this.tabOptionModule.Text = "Module";
			this.tabOptionModule.UseVisualStyleBackColor = true;
			// 
			// lstModule
			// 
			this.lstModule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colModName,
            this.colModArch,
            this.colModAuthor});
			this.lstModule.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstModule.FullRowSelect = true;
			this.lstModule.Location = new System.Drawing.Point(3, 3);
			this.lstModule.Name = "lstModule";
			this.lstModule.Size = new System.Drawing.Size(602, 395);
			this.lstModule.TabIndex = 0;
			this.lstModule.UseCompatibleStateImageBehavior = false;
			this.lstModule.View = System.Windows.Forms.View.Details;
			// 
			// colModName
			// 
			this.colModName.Text = "Name";
			this.colModName.Width = 310;
			// 
			// colModArch
			// 
			this.colModArch.Text = "Arch";
			// 
			// colModAuthor
			// 
			this.colModAuthor.Text = "Developer";
			this.colModAuthor.Width = 200;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(553, 445);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(472, 445);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "&OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// grpFrameCountOffset
			// 
			this.grpFrameCountOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpFrameCountOffset.Controls.Add(this.nudFrameCountOffset);
			this.grpFrameCountOffset.Controls.Add(this.lblFrameCountOffset);
			this.grpFrameCountOffset.Location = new System.Drawing.Point(6, 268);
			this.grpFrameCountOffset.Name = "grpFrameCountOffset";
			this.grpFrameCountOffset.Size = new System.Drawing.Size(596, 127);
			this.grpFrameCountOffset.TabIndex = 2;
			this.grpFrameCountOffset.TabStop = false;
			this.grpFrameCountOffset.Text = "&Frame Count Offset";
			// 
			// lblFrameCountOffset
			// 
			this.lblFrameCountOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblFrameCountOffset.Location = new System.Drawing.Point(6, 18);
			this.lblFrameCountOffset.Name = "lblFrameCountOffset";
			this.lblFrameCountOffset.Size = new System.Drawing.Size(584, 64);
			this.lblFrameCountOffset.TabIndex = 0;
			this.lblFrameCountOffset.Text = resources.GetString("lblFrameCountOffset.Text");
			this.lblFrameCountOffset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// nudFrameCountOffset
			// 
			this.nudFrameCountOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.nudFrameCountOffset.Font = new System.Drawing.Font("Tahoma", 10F);
			this.nudFrameCountOffset.Location = new System.Drawing.Point(238, 85);
			this.nudFrameCountOffset.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
			this.nudFrameCountOffset.Name = "nudFrameCountOffset";
			this.nudFrameCountOffset.Size = new System.Drawing.Size(120, 24);
			this.nudFrameCountOffset.TabIndex = 1;
			// 
			// frmOption
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabOption);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmOption";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.frmOption_Load);
			this.tabOption.ResumeLayout(false);
			this.tabOptionGeneral.ResumeLayout(false);
			this.grpNewFilename.ResumeLayout(false);
			this.grpNewFilenamePostfix.ResumeLayout(false);
			this.grpNewFilenamePostfix.PerformLayout();
			this.grpNewFilenamePrefix.ResumeLayout(false);
			this.grpNewFilenamePrefix.PerformLayout();
			this.grpTempFolder.ResumeLayout(false);
			this.grpTempFolder.PerformLayout();
			this.grpLanguage.ResumeLayout(false);
			this.tabOptionEncoding.ResumeLayout(false);
			this.grpAviSynth.ResumeLayout(false);
			this.grpFFmpeg.ResumeLayout(false);
			this.tabOptionModule.ResumeLayout(false);
			this.grpFrameCountOffset.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudFrameCountOffset)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabOption;
		private System.Windows.Forms.TabPage tabOptionGeneral;
		private System.Windows.Forms.TabPage tabOptionEncoding;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.TabPage tabOptionModule;
		private System.Windows.Forms.GroupBox grpLanguage;
		private System.Windows.Forms.GroupBox grpTempFolder;
		private System.Windows.Forms.GroupBox grpFFmpeg;
		private System.Windows.Forms.ComboBox cboLanguage;
		private System.Windows.Forms.Button btnTempPath;
		private System.Windows.Forms.TextBox txtTempPath;
		private System.Windows.Forms.Label lblLanguageAuthor;
		private System.Windows.Forms.GroupBox grpNewFilename;
		private System.Windows.Forms.TextBox txtNamePrefix;
		private System.Windows.Forms.RadioButton rdoNamePrefixCustom;
		private System.Windows.Forms.RadioButton rdoNamePrefixDateTime;
		private System.Windows.Forms.RadioButton rdoNamePrefixNone;
		private System.Windows.Forms.RadioButton rdoFFmpeg64;
		private System.Windows.Forms.RadioButton rdoFFmpeg32;
		private System.Windows.Forms.ListView lstModule;
		private System.Windows.Forms.ColumnHeader colModName;
		private System.Windows.Forms.ColumnHeader colModArch;
		private System.Windows.Forms.ColumnHeader colModAuthor;
		private System.Windows.Forms.GroupBox grpAviSynth;
		private System.Windows.Forms.Label lblAviSynthInstall;
		private System.Windows.Forms.Label lblAviSynthVersion;
		private System.Windows.Forms.GroupBox grpNewFilenamePrefix;
		private System.Windows.Forms.GroupBox grpNewFilenamePostfix;
		private System.Windows.Forms.TextBox txtNamePostfix;
		private System.Windows.Forms.RadioButton rdoNamePostfixCustom;
		private System.Windows.Forms.RadioButton rdoNamePostfixNone;
		private System.Windows.Forms.GroupBox grpFrameCountOffset;
		private System.Windows.Forms.Label lblFrameCountOffset;
		private System.Windows.Forms.NumericUpDown nudFrameCountOffset;
	}
}