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
			this.tabOption = new System.Windows.Forms.TabControl();
			this.tabOptionGeneral = new System.Windows.Forms.TabPage();
			this.tabOptionEncoding = new System.Windows.Forms.TabPage();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.tabOptionModule = new System.Windows.Forms.TabPage();
			this.grpLanguage = new System.Windows.Forms.GroupBox();
			this.grpTempFolder = new System.Windows.Forms.GroupBox();
			this.grpFFmpeg = new System.Windows.Forms.GroupBox();
			this.cboLanguage = new System.Windows.Forms.ComboBox();
			this.lblLanguageAuthor = new System.Windows.Forms.Label();
			this.txtTempPath = new System.Windows.Forms.TextBox();
			this.btnTempPath = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdoNamePrefixNone = new System.Windows.Forms.RadioButton();
			this.rdoNamePrefixDateTime = new System.Windows.Forms.RadioButton();
			this.rdoNamePrefixCustom = new System.Windows.Forms.RadioButton();
			this.txtNamePrefix = new System.Windows.Forms.TextBox();
			this.rdoFFmpeg32 = new System.Windows.Forms.RadioButton();
			this.rdoFFmpeg64 = new System.Windows.Forms.RadioButton();
			this.lstModule = new System.Windows.Forms.ListView();
			this.colModName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colModArch = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colModAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabOption.SuspendLayout();
			this.tabOptionGeneral.SuspendLayout();
			this.tabOptionEncoding.SuspendLayout();
			this.tabOptionModule.SuspendLayout();
			this.grpLanguage.SuspendLayout();
			this.grpTempFolder.SuspendLayout();
			this.grpFFmpeg.SuspendLayout();
			this.groupBox1.SuspendLayout();
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
			this.tabOption.Size = new System.Drawing.Size(616, 307);
			this.tabOption.TabIndex = 0;
			// 
			// tabOptionGeneral
			// 
			this.tabOptionGeneral.Controls.Add(this.groupBox1);
			this.tabOptionGeneral.Controls.Add(this.grpTempFolder);
			this.tabOptionGeneral.Controls.Add(this.grpLanguage);
			this.tabOptionGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabOptionGeneral.Name = "tabOptionGeneral";
			this.tabOptionGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabOptionGeneral.Size = new System.Drawing.Size(608, 281);
			this.tabOptionGeneral.TabIndex = 0;
			this.tabOptionGeneral.Text = "General";
			this.tabOptionGeneral.UseVisualStyleBackColor = true;
			// 
			// tabOptionEncoding
			// 
			this.tabOptionEncoding.Controls.Add(this.grpFFmpeg);
			this.tabOptionEncoding.Location = new System.Drawing.Point(4, 22);
			this.tabOptionEncoding.Name = "tabOptionEncoding";
			this.tabOptionEncoding.Padding = new System.Windows.Forms.Padding(3);
			this.tabOptionEncoding.Size = new System.Drawing.Size(608, 281);
			this.tabOptionEncoding.TabIndex = 1;
			this.tabOptionEncoding.Text = "Encoding";
			this.tabOptionEncoding.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(553, 325);
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
			this.btnOk.Location = new System.Drawing.Point(472, 325);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "&OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// tabOptionModule
			// 
			this.tabOptionModule.Controls.Add(this.lstModule);
			this.tabOptionModule.Location = new System.Drawing.Point(4, 22);
			this.tabOptionModule.Name = "tabOptionModule";
			this.tabOptionModule.Padding = new System.Windows.Forms.Padding(3);
			this.tabOptionModule.Size = new System.Drawing.Size(608, 281);
			this.tabOptionModule.TabIndex = 2;
			this.tabOptionModule.Text = "Module";
			this.tabOptionModule.UseVisualStyleBackColor = true;
			// 
			// grpLanguage
			// 
			this.grpLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpLanguage.Controls.Add(this.lblLanguageAuthor);
			this.grpLanguage.Controls.Add(this.cboLanguage);
			this.grpLanguage.Location = new System.Drawing.Point(6, 6);
			this.grpLanguage.Name = "grpLanguage";
			this.grpLanguage.Size = new System.Drawing.Size(596, 97);
			this.grpLanguage.TabIndex = 0;
			this.grpLanguage.TabStop = false;
			this.grpLanguage.Text = "&Language";
			// 
			// grpTempFolder
			// 
			this.grpTempFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpTempFolder.Controls.Add(this.btnTempPath);
			this.grpTempFolder.Controls.Add(this.txtTempPath);
			this.grpTempFolder.Location = new System.Drawing.Point(6, 109);
			this.grpTempFolder.Name = "grpTempFolder";
			this.grpTempFolder.Size = new System.Drawing.Size(596, 80);
			this.grpTempFolder.TabIndex = 1;
			this.grpTempFolder.TabStop = false;
			this.grpTempFolder.Text = "&Temporary folder";
			// 
			// grpFFmpeg
			// 
			this.grpFFmpeg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpFFmpeg.Controls.Add(this.rdoFFmpeg64);
			this.grpFFmpeg.Controls.Add(this.rdoFFmpeg32);
			this.grpFFmpeg.Location = new System.Drawing.Point(6, 6);
			this.grpFFmpeg.Name = "grpFFmpeg";
			this.grpFFmpeg.Size = new System.Drawing.Size(596, 100);
			this.grpFFmpeg.TabIndex = 0;
			this.grpFFmpeg.TabStop = false;
			this.grpFFmpeg.Text = "&Default Decoder";
			// 
			// cboLanguage
			// 
			this.cboLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLanguage.Font = new System.Drawing.Font("Tahoma", 10F);
			this.cboLanguage.FormattingEnabled = true;
			this.cboLanguage.Location = new System.Drawing.Point(89, 22);
			this.cboLanguage.Name = "cboLanguage";
			this.cboLanguage.Size = new System.Drawing.Size(418, 24);
			this.cboLanguage.TabIndex = 0;
			// 
			// lblLanguageAuthor
			// 
			this.lblLanguageAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblLanguageAuthor.Location = new System.Drawing.Point(89, 49);
			this.lblLanguageAuthor.Name = "lblLanguageAuthor";
			this.lblLanguageAuthor.Size = new System.Drawing.Size(418, 32);
			this.lblLanguageAuthor.TabIndex = 1;
			this.lblLanguageAuthor.Text = "Line 1\r\nLine 2";
			// 
			// txtTempPath
			// 
			this.txtTempPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtTempPath.Font = new System.Drawing.Font("Tahoma", 10F);
			this.txtTempPath.Location = new System.Drawing.Point(89, 28);
			this.txtTempPath.Name = "txtTempPath";
			this.txtTempPath.ReadOnly = true;
			this.txtTempPath.Size = new System.Drawing.Size(388, 24);
			this.txtTempPath.TabIndex = 0;
			// 
			// btnTempPath
			// 
			this.btnTempPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTempPath.Image = global::ifme.Properties.Resources.icon16_document_save;
			this.btnTempPath.Location = new System.Drawing.Point(483, 28);
			this.btnTempPath.Name = "btnTempPath";
			this.btnTempPath.Size = new System.Drawing.Size(24, 24);
			this.btnTempPath.TabIndex = 1;
			this.btnTempPath.UseVisualStyleBackColor = true;
			this.btnTempPath.Click += new System.EventHandler(this.btnTempPath_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.txtNamePrefix);
			this.groupBox1.Controls.Add(this.rdoNamePrefixCustom);
			this.groupBox1.Controls.Add(this.rdoNamePrefixDateTime);
			this.groupBox1.Controls.Add(this.rdoNamePrefixNone);
			this.groupBox1.Location = new System.Drawing.Point(6, 195);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(596, 80);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "&New file name prefix";
			// 
			// rdoNamePrefixNone
			// 
			this.rdoNamePrefixNone.Location = new System.Drawing.Point(89, 32);
			this.rdoNamePrefixNone.Name = "rdoNamePrefixNone";
			this.rdoNamePrefixNone.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePrefixNone.TabIndex = 0;
			this.rdoNamePrefixNone.TabStop = true;
			this.rdoNamePrefixNone.Text = "&None";
			this.rdoNamePrefixNone.UseVisualStyleBackColor = true;
			// 
			// rdoNamePrefixDateTime
			// 
			this.rdoNamePrefixDateTime.Location = new System.Drawing.Point(185, 32);
			this.rdoNamePrefixDateTime.Name = "rdoNamePrefixDateTime";
			this.rdoNamePrefixDateTime.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePrefixDateTime.TabIndex = 1;
			this.rdoNamePrefixDateTime.TabStop = true;
			this.rdoNamePrefixDateTime.Text = "&Date Time";
			this.rdoNamePrefixDateTime.UseVisualStyleBackColor = true;
			// 
			// rdoNamePrefixCustom
			// 
			this.rdoNamePrefixCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rdoNamePrefixCustom.Location = new System.Drawing.Point(281, 32);
			this.rdoNamePrefixCustom.Name = "rdoNamePrefixCustom";
			this.rdoNamePrefixCustom.Size = new System.Drawing.Size(90, 24);
			this.rdoNamePrefixCustom.TabIndex = 2;
			this.rdoNamePrefixCustom.TabStop = true;
			this.rdoNamePrefixCustom.Text = "&Custom:";
			this.rdoNamePrefixCustom.UseVisualStyleBackColor = true;
			// 
			// txtNamePrefix
			// 
			this.txtNamePrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtNamePrefix.Font = new System.Drawing.Font("Tahoma", 10F);
			this.txtNamePrefix.Location = new System.Drawing.Point(377, 32);
			this.txtNamePrefix.Name = "txtNamePrefix";
			this.txtNamePrefix.Size = new System.Drawing.Size(130, 24);
			this.txtNamePrefix.TabIndex = 3;
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
			this.lstModule.Size = new System.Drawing.Size(602, 275);
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
			// frmOption
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(640, 360);
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
			this.tabOptionEncoding.ResumeLayout(false);
			this.tabOptionModule.ResumeLayout(false);
			this.grpLanguage.ResumeLayout(false);
			this.grpTempFolder.ResumeLayout(false);
			this.grpTempFolder.PerformLayout();
			this.grpFFmpeg.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBox1;
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
	}
}