
namespace IFME
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
            this.tabSetting = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.grpFileName = new System.Windows.Forms.GroupBox();
            this.lblFileNameEx = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPostfix = new System.Windows.Forms.TextBox();
            this.rdoPostfixCustom = new System.Windows.Forms.RadioButton();
            this.rdoPostfixDateTime = new System.Windows.Forms.RadioButton();
            this.rdoPostfixNone = new System.Windows.Forms.RadioButton();
            this.grpPrefix = new System.Windows.Forms.GroupBox();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.rdoPrefixCustom = new System.Windows.Forms.RadioButton();
            this.rdoPrefixDateTime = new System.Windows.Forms.RadioButton();
            this.rdoPrefixNone = new System.Windows.Forms.RadioButton();
            this.grpTempFolder = new System.Windows.Forms.GroupBox();
            this.btnTempBrowse = new System.Windows.Forms.Button();
            this.txtTempPath = new System.Windows.Forms.TextBox();
            this.grpLanguage = new System.Windows.Forms.GroupBox();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.tabProcessing = new System.Windows.Forms.TabPage();
            this.grpMuxMp4 = new System.Windows.Forms.GroupBox();
            this.chkMuxMp4EM = new System.Windows.Forms.CheckBox();
            this.chkMuxMp4SM = new System.Windows.Forms.CheckBox();
            this.chkMuxMp4FK = new System.Windows.Forms.CheckBox();
            this.tabPlugins = new System.Windows.Forms.TabPage();
            this.chkSkipTest = new System.Windows.Forms.CheckBox();
            this.btnNone = new System.Windows.Forms.Button();
            this.btnOnly265 = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.lstPlugins = new System.Windows.Forms.ListView();
            this.colPluginsName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPluginsArh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPluginsVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPluginsAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabProfiles = new System.Windows.Forms.TabPage();
            this.lstProfiles = new System.Windows.Forms.ListView();
            this.colProfileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProfileFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProfileAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProfileAuthorWeb = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnFactoryReset = new System.Windows.Forms.Button();
            this.tabSetting.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.grpFileName.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpPrefix.SuspendLayout();
            this.grpTempFolder.SuspendLayout();
            this.grpLanguage.SuspendLayout();
            this.tabProcessing.SuspendLayout();
            this.grpMuxMp4.SuspendLayout();
            this.tabPlugins.SuspendLayout();
            this.tabProfiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSetting
            // 
            this.tabSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSetting.Controls.Add(this.tabGeneral);
            this.tabSetting.Controls.Add(this.tabProcessing);
            this.tabSetting.Controls.Add(this.tabPlugins);
            this.tabSetting.Controls.Add(this.tabProfiles);
            this.tabSetting.Location = new System.Drawing.Point(12, 12);
            this.tabSetting.Name = "tabSetting";
            this.tabSetting.Padding = new System.Drawing.Point(16, 4);
            this.tabSetting.SelectedIndex = 0;
            this.tabSetting.Size = new System.Drawing.Size(776, 546);
            this.tabSetting.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.grpFileName);
            this.tabGeneral.Controls.Add(this.grpTempFolder);
            this.tabGeneral.Controls.Add(this.grpLanguage);
            this.tabGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(768, 518);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // grpFileName
            // 
            this.grpFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFileName.Controls.Add(this.lblFileNameEx);
            this.grpFileName.Controls.Add(this.groupBox1);
            this.grpFileName.Controls.Add(this.grpPrefix);
            this.grpFileName.Location = new System.Drawing.Point(6, 333);
            this.grpFileName.Name = "grpFileName";
            this.grpFileName.Size = new System.Drawing.Size(756, 179);
            this.grpFileName.TabIndex = 2;
            this.grpFileName.TabStop = false;
            this.grpFileName.Text = "&New Filename";
            // 
            // lblFileNameEx
            // 
            this.lblFileNameEx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileNameEx.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblFileNameEx.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblFileNameEx.Location = new System.Drawing.Point(6, 16);
            this.lblFileNameEx.Name = "lblFileNameEx";
            this.lblFileNameEx.Size = new System.Drawing.Size(744, 24);
            this.lblFileNameEx.TabIndex = 1;
            this.lblFileNameEx.Text = "Mysteries Of Legendary Worlds.mkv";
            this.lblFileNameEx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFileNameEx.Click += new System.EventHandler(this.lblFileNameEx_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtPostfix);
            this.groupBox1.Controls.Add(this.rdoPostfixCustom);
            this.groupBox1.Controls.Add(this.rdoPostfixDateTime);
            this.groupBox1.Controls.Add(this.rdoPostfixNone);
            this.groupBox1.Location = new System.Drawing.Point(381, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(369, 130);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Postfi&x";
            // 
            // txtPostfix
            // 
            this.txtPostfix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPostfix.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtPostfix.Location = new System.Drawing.Point(120, 83);
            this.txtPostfix.Name = "txtPostfix";
            this.txtPostfix.Size = new System.Drawing.Size(234, 24);
            this.txtPostfix.TabIndex = 2;
            this.txtPostfix.TextChanged += new System.EventHandler(this.txtPostfix_TextChanged);
            // 
            // rdoPostfixCustom
            // 
            this.rdoPostfixCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoPostfixCustom.Location = new System.Drawing.Point(14, 83);
            this.rdoPostfixCustom.Name = "rdoPostfixCustom";
            this.rdoPostfixCustom.Size = new System.Drawing.Size(100, 24);
            this.rdoPostfixCustom.TabIndex = 4;
            this.rdoPostfixCustom.Text = "&Custom";
            this.rdoPostfixCustom.UseVisualStyleBackColor = true;
            this.rdoPostfixCustom.CheckedChanged += new System.EventHandler(this.rdoPrePostFixFilename_CheckedChanged);
            // 
            // rdoPostfixDateTime
            // 
            this.rdoPostfixDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoPostfixDateTime.Location = new System.Drawing.Point(14, 53);
            this.rdoPostfixDateTime.Name = "rdoPostfixDateTime";
            this.rdoPostfixDateTime.Size = new System.Drawing.Size(340, 24);
            this.rdoPostfixDateTime.TabIndex = 1;
            this.rdoPostfixDateTime.Text = "&Date Time";
            this.rdoPostfixDateTime.UseVisualStyleBackColor = true;
            this.rdoPostfixDateTime.CheckedChanged += new System.EventHandler(this.rdoPrePostFixFilename_CheckedChanged);
            // 
            // rdoPostfixNone
            // 
            this.rdoPostfixNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoPostfixNone.Checked = true;
            this.rdoPostfixNone.Location = new System.Drawing.Point(14, 23);
            this.rdoPostfixNone.Name = "rdoPostfixNone";
            this.rdoPostfixNone.Size = new System.Drawing.Size(340, 24);
            this.rdoPostfixNone.TabIndex = 0;
            this.rdoPostfixNone.TabStop = true;
            this.rdoPostfixNone.Text = "&None";
            this.rdoPostfixNone.UseVisualStyleBackColor = true;
            this.rdoPostfixNone.CheckedChanged += new System.EventHandler(this.rdoPrePostFixFilename_CheckedChanged);
            // 
            // grpPrefix
            // 
            this.grpPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpPrefix.Controls.Add(this.txtPrefix);
            this.grpPrefix.Controls.Add(this.rdoPrefixCustom);
            this.grpPrefix.Controls.Add(this.rdoPrefixDateTime);
            this.grpPrefix.Controls.Add(this.rdoPrefixNone);
            this.grpPrefix.Location = new System.Drawing.Point(6, 43);
            this.grpPrefix.Name = "grpPrefix";
            this.grpPrefix.Size = new System.Drawing.Size(369, 130);
            this.grpPrefix.TabIndex = 0;
            this.grpPrefix.TabStop = false;
            this.grpPrefix.Text = "&Prefix";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrefix.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtPrefix.Location = new System.Drawing.Point(120, 83);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(234, 24);
            this.txtPrefix.TabIndex = 3;
            this.txtPrefix.TextChanged += new System.EventHandler(this.txtPrefix_TextChanged);
            // 
            // rdoPrefixCustom
            // 
            this.rdoPrefixCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoPrefixCustom.Checked = true;
            this.rdoPrefixCustom.Location = new System.Drawing.Point(14, 83);
            this.rdoPrefixCustom.Name = "rdoPrefixCustom";
            this.rdoPrefixCustom.Size = new System.Drawing.Size(100, 24);
            this.rdoPrefixCustom.TabIndex = 2;
            this.rdoPrefixCustom.TabStop = true;
            this.rdoPrefixCustom.Text = "&Custom";
            this.rdoPrefixCustom.UseVisualStyleBackColor = true;
            this.rdoPrefixCustom.CheckedChanged += new System.EventHandler(this.rdoPrePostFixFilename_CheckedChanged);
            // 
            // rdoPrefixDateTime
            // 
            this.rdoPrefixDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoPrefixDateTime.Location = new System.Drawing.Point(14, 53);
            this.rdoPrefixDateTime.Name = "rdoPrefixDateTime";
            this.rdoPrefixDateTime.Size = new System.Drawing.Size(340, 24);
            this.rdoPrefixDateTime.TabIndex = 1;
            this.rdoPrefixDateTime.Text = "&Date Time";
            this.rdoPrefixDateTime.UseVisualStyleBackColor = true;
            this.rdoPrefixDateTime.CheckedChanged += new System.EventHandler(this.rdoPrePostFixFilename_CheckedChanged);
            // 
            // rdoPrefixNone
            // 
            this.rdoPrefixNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoPrefixNone.Location = new System.Drawing.Point(14, 23);
            this.rdoPrefixNone.Name = "rdoPrefixNone";
            this.rdoPrefixNone.Size = new System.Drawing.Size(340, 24);
            this.rdoPrefixNone.TabIndex = 0;
            this.rdoPrefixNone.Text = "&None";
            this.rdoPrefixNone.UseVisualStyleBackColor = true;
            this.rdoPrefixNone.CheckedChanged += new System.EventHandler(this.rdoPrePostFixFilename_CheckedChanged);
            // 
            // grpTempFolder
            // 
            this.grpTempFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTempFolder.Controls.Add(this.btnTempBrowse);
            this.grpTempFolder.Controls.Add(this.txtTempPath);
            this.grpTempFolder.Location = new System.Drawing.Point(6, 177);
            this.grpTempFolder.Name = "grpTempFolder";
            this.grpTempFolder.Size = new System.Drawing.Size(756, 150);
            this.grpTempFolder.TabIndex = 1;
            this.grpTempFolder.TabStop = false;
            this.grpTempFolder.Text = "Temporary &Folder";
            // 
            // btnTempBrowse
            // 
            this.btnTempBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnTempBrowse.Location = new System.Drawing.Point(587, 63);
            this.btnTempBrowse.Name = "btnTempBrowse";
            this.btnTempBrowse.Size = new System.Drawing.Size(100, 24);
            this.btnTempBrowse.TabIndex = 1;
            this.btnTempBrowse.Text = "&Browse";
            this.btnTempBrowse.UseVisualStyleBackColor = true;
            this.btnTempBrowse.Click += new System.EventHandler(this.btnTempBrowse_Click);
            // 
            // txtTempPath
            // 
            this.txtTempPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTempPath.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtTempPath.Location = new System.Drawing.Point(69, 63);
            this.txtTempPath.Name = "txtTempPath";
            this.txtTempPath.ReadOnly = true;
            this.txtTempPath.Size = new System.Drawing.Size(512, 24);
            this.txtTempPath.TabIndex = 0;
            this.txtTempPath.Text = "/home/anime4000/.config/ifme/temp";
            // 
            // grpLanguage
            // 
            this.grpLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLanguage.Controls.Add(this.cboLanguage);
            this.grpLanguage.Location = new System.Drawing.Point(6, 6);
            this.grpLanguage.Name = "grpLanguage";
            this.grpLanguage.Size = new System.Drawing.Size(756, 165);
            this.grpLanguage.TabIndex = 0;
            this.grpLanguage.TabStop = false;
            this.grpLanguage.Text = "Interface &Language";
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.Enabled = false;
            this.cboLanguage.Font = new System.Drawing.Font("Tahoma", 10F);
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Location = new System.Drawing.Point(69, 65);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(618, 24);
            this.cboLanguage.TabIndex = 0;
            // 
            // tabProcessing
            // 
            this.tabProcessing.Controls.Add(this.grpMuxMp4);
            this.tabProcessing.Location = new System.Drawing.Point(4, 24);
            this.tabProcessing.Name = "tabProcessing";
            this.tabProcessing.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcessing.Size = new System.Drawing.Size(768, 518);
            this.tabProcessing.TabIndex = 1;
            this.tabProcessing.Text = "Processing";
            this.tabProcessing.UseVisualStyleBackColor = true;
            // 
            // grpMuxMp4
            // 
            this.grpMuxMp4.Controls.Add(this.chkMuxMp4EM);
            this.grpMuxMp4.Controls.Add(this.chkMuxMp4SM);
            this.grpMuxMp4.Controls.Add(this.chkMuxMp4FK);
            this.grpMuxMp4.Location = new System.Drawing.Point(6, 6);
            this.grpMuxMp4.Name = "grpMuxMp4";
            this.grpMuxMp4.Size = new System.Drawing.Size(756, 160);
            this.grpMuxMp4.TabIndex = 0;
            this.grpMuxMp4.TabStop = false;
            this.grpMuxMp4.Text = "MP&4 Muxing Options";
            // 
            // chkMuxMp4EM
            // 
            this.chkMuxMp4EM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMuxMp4EM.Location = new System.Drawing.Point(6, 102);
            this.chkMuxMp4EM.Name = "chkMuxMp4EM";
            this.chkMuxMp4EM.Size = new System.Drawing.Size(744, 32);
            this.chkMuxMp4EM.TabIndex = 2;
            this.chkMuxMp4EM.Text = "&Empty Moov Atom\r\n(Add an empty movie header at the start of the file so streamin" +
    "g can begin before the whole video is downloaded.)";
            this.chkMuxMp4EM.UseVisualStyleBackColor = true;
            // 
            // chkMuxMp4SM
            // 
            this.chkMuxMp4SM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMuxMp4SM.Location = new System.Drawing.Point(6, 64);
            this.chkMuxMp4SM.Name = "chkMuxMp4SM";
            this.chkMuxMp4SM.Size = new System.Drawing.Size(744, 32);
            this.chkMuxMp4SM.TabIndex = 1;
            this.chkMuxMp4SM.Text = "&Separate Moof Atom\r\n(Place important streaming info (moof) before the actual vid" +
    "eo data (mdat). Required for some streaming services.)";
            this.chkMuxMp4SM.UseVisualStyleBackColor = true;
            // 
            // chkMuxMp4FK
            // 
            this.chkMuxMp4FK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMuxMp4FK.Location = new System.Drawing.Point(6, 26);
            this.chkMuxMp4FK.Name = "chkMuxMp4FK";
            this.chkMuxMp4FK.Size = new System.Drawing.Size(744, 32);
            this.chkMuxMp4FK.TabIndex = 0;
            this.chkMuxMp4FK.Text = "&Fragmented Keyframes\r\n(Break the video into smaller parts at each keyframe. This" +
    " helps with faster seeking and smoother streaming.)";
            this.chkMuxMp4FK.UseVisualStyleBackColor = true;
            // 
            // tabPlugins
            // 
            this.tabPlugins.Controls.Add(this.chkSkipTest);
            this.tabPlugins.Controls.Add(this.btnNone);
            this.tabPlugins.Controls.Add(this.btnOnly265);
            this.tabPlugins.Controls.Add(this.btnCheckAll);
            this.tabPlugins.Controls.Add(this.lstPlugins);
            this.tabPlugins.Location = new System.Drawing.Point(4, 24);
            this.tabPlugins.Name = "tabPlugins";
            this.tabPlugins.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlugins.Size = new System.Drawing.Size(768, 518);
            this.tabPlugins.TabIndex = 2;
            this.tabPlugins.Text = "Encoders";
            this.tabPlugins.UseVisualStyleBackColor = true;
            // 
            // chkSkipTest
            // 
            this.chkSkipTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSkipTest.Location = new System.Drawing.Point(264, 488);
            this.chkSkipTest.Name = "chkSkipTest";
            this.chkSkipTest.Size = new System.Drawing.Size(498, 24);
            this.chkSkipTest.TabIndex = 4;
            this.chkSkipTest.Text = "Uncheck this if you wish to disable encoder test to make program load faster! War" +
    "ning!";
            this.chkSkipTest.UseVisualStyleBackColor = true;
            this.chkSkipTest.CheckedChanged += new System.EventHandler(this.chkSkipTest_CheckedChanged);
            // 
            // btnNone
            // 
            this.btnNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNone.Location = new System.Drawing.Point(178, 488);
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(80, 24);
            this.btnNone.TabIndex = 3;
            this.btnNone.Text = "&None";
            this.btnNone.UseVisualStyleBackColor = true;
            this.btnNone.Click += new System.EventHandler(this.btnNone_Click);
            // 
            // btnOnly265
            // 
            this.btnOnly265.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOnly265.Location = new System.Drawing.Point(92, 488);
            this.btnOnly265.Name = "btnOnly265";
            this.btnOnly265.Size = new System.Drawing.Size(80, 24);
            this.btnOnly265.TabIndex = 2;
            this.btnOnly265.Text = "x26&5";
            this.btnOnly265.UseVisualStyleBackColor = true;
            this.btnOnly265.Click += new System.EventHandler(this.btnOnly265_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCheckAll.Location = new System.Drawing.Point(6, 488);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(80, 24);
            this.btnCheckAll.TabIndex = 1;
            this.btnCheckAll.Text = "&All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // lstPlugins
            // 
            this.lstPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPlugins.CheckBoxes = true;
            this.lstPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPluginsName,
            this.colPluginsArh,
            this.colPluginsVersion,
            this.colPluginsAuthor});
            this.lstPlugins.FullRowSelect = true;
            this.lstPlugins.HideSelection = false;
            this.lstPlugins.Location = new System.Drawing.Point(6, 6);
            this.lstPlugins.Name = "lstPlugins";
            this.lstPlugins.Size = new System.Drawing.Size(756, 476);
            this.lstPlugins.TabIndex = 0;
            this.lstPlugins.UseCompatibleStateImageBehavior = false;
            this.lstPlugins.View = System.Windows.Forms.View.Details;
            // 
            // colPluginsName
            // 
            this.colPluginsName.Text = "Name";
            this.colPluginsName.Width = 256;
            // 
            // colPluginsArh
            // 
            this.colPluginsArh.Text = "Arch";
            this.colPluginsArh.Width = 64;
            // 
            // colPluginsVersion
            // 
            this.colPluginsVersion.Text = "Version";
            this.colPluginsVersion.Width = 128;
            // 
            // colPluginsAuthor
            // 
            this.colPluginsAuthor.Text = "Developer";
            this.colPluginsAuthor.Width = 256;
            // 
            // tabProfiles
            // 
            this.tabProfiles.Controls.Add(this.lstProfiles);
            this.tabProfiles.Location = new System.Drawing.Point(4, 24);
            this.tabProfiles.Name = "tabProfiles";
            this.tabProfiles.Size = new System.Drawing.Size(768, 518);
            this.tabProfiles.TabIndex = 3;
            this.tabProfiles.Text = "Profiles";
            this.tabProfiles.UseVisualStyleBackColor = true;
            // 
            // lstProfiles
            // 
            this.lstProfiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProfileName,
            this.colProfileFileName,
            this.colProfileAuthor,
            this.colProfileAuthorWeb});
            this.lstProfiles.FullRowSelect = true;
            this.lstProfiles.HideSelection = false;
            this.lstProfiles.Location = new System.Drawing.Point(6, 6);
            this.lstProfiles.MultiSelect = false;
            this.lstProfiles.Name = "lstProfiles";
            this.lstProfiles.Size = new System.Drawing.Size(756, 506);
            this.lstProfiles.TabIndex = 0;
            this.lstProfiles.UseCompatibleStateImageBehavior = false;
            this.lstProfiles.View = System.Windows.Forms.View.Details;
            // 
            // colProfileName
            // 
            this.colProfileName.Text = "Name";
            this.colProfileName.Width = 256;
            // 
            // colProfileFileName
            // 
            this.colProfileFileName.Text = "File Name";
            this.colProfileFileName.Width = 200;
            // 
            // colProfileAuthor
            // 
            this.colProfileAuthor.Text = "Author";
            this.colProfileAuthor.Width = 96;
            // 
            // colProfileAuthorWeb
            // 
            this.colProfileAuthorWeb.Text = "Website";
            this.colProfileAuthorWeb.Width = 180;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(708, 564);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(622, 564);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnFactoryReset
            // 
            this.btnFactoryReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFactoryReset.Location = new System.Drawing.Point(12, 565);
            this.btnFactoryReset.Name = "btnFactoryReset";
            this.btnFactoryReset.Size = new System.Drawing.Size(150, 23);
            this.btnFactoryReset.TabIndex = 3;
            this.btnFactoryReset.Text = "Factory &Reset";
            this.btnFactoryReset.UseVisualStyleBackColor = true;
            this.btnFactoryReset.Click += new System.EventHandler(this.btnFactoryReset_Click);
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.btnFactoryReset);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabSetting);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.Shown += new System.EventHandler(this.frmOptions_Shown);
            this.tabSetting.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.grpFileName.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpPrefix.ResumeLayout(false);
            this.grpPrefix.PerformLayout();
            this.grpTempFolder.ResumeLayout(false);
            this.grpTempFolder.PerformLayout();
            this.grpLanguage.ResumeLayout(false);
            this.tabProcessing.ResumeLayout(false);
            this.grpMuxMp4.ResumeLayout(false);
            this.tabPlugins.ResumeLayout(false);
            this.tabProfiles.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSetting;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabProcessing;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabPage tabPlugins;
        private System.Windows.Forms.ListView lstPlugins;
        private System.Windows.Forms.ColumnHeader colPluginsName;
        private System.Windows.Forms.ColumnHeader colPluginsArh;
        private System.Windows.Forms.ColumnHeader colPluginsVersion;
        private System.Windows.Forms.ColumnHeader colPluginsAuthor;
        private System.Windows.Forms.GroupBox grpLanguage;
        private System.Windows.Forms.GroupBox grpTempFolder;
        private System.Windows.Forms.GroupBox grpFileName;
        private System.Windows.Forms.TextBox txtTempPath;
        private System.Windows.Forms.Button btnTempBrowse;
        private System.Windows.Forms.GroupBox grpPrefix;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.RadioButton rdoPrefixCustom;
        private System.Windows.Forms.RadioButton rdoPrefixDateTime;
        private System.Windows.Forms.RadioButton rdoPrefixNone;
        private System.Windows.Forms.TextBox txtPostfix;
        private System.Windows.Forms.RadioButton rdoPostfixCustom;
        private System.Windows.Forms.RadioButton rdoPostfixDateTime;
        private System.Windows.Forms.RadioButton rdoPostfixNone;
        private System.Windows.Forms.Button btnNone;
        private System.Windows.Forms.Button btnOnly265;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.CheckBox chkSkipTest;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Label lblFileNameEx;
        private System.Windows.Forms.Button btnFactoryReset;
        private System.Windows.Forms.TabPage tabProfiles;
        private System.Windows.Forms.ListView lstProfiles;
        private System.Windows.Forms.ColumnHeader colProfileName;
        private System.Windows.Forms.ColumnHeader colProfileFileName;
        private System.Windows.Forms.ColumnHeader colProfileAuthor;
        private System.Windows.Forms.ColumnHeader colProfileAuthorWeb;
        private System.Windows.Forms.GroupBox grpMuxMp4;
        private System.Windows.Forms.CheckBox chkMuxMp4FK;
        private System.Windows.Forms.CheckBox chkMuxMp4EM;
        private System.Windows.Forms.CheckBox chkMuxMp4SM;
    }
}