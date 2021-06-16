namespace IFME
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.cboSubLang = new System.Windows.Forms.ComboBox();
            this.btnSubAdd = new System.Windows.Forms.Button();
            this.lblSubLang = new System.Windows.Forms.Label();
            this.btnSubDel = new System.Windows.Forms.Button();
            this.btnSubMoveDown = new System.Windows.Forms.Button();
            this.colSubLang = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSubFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSubId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Seperator6 = new System.Windows.Forms.Label();
            this.chkSubHard = new System.Windows.Forms.CheckBox();
            this.lstSub = new System.Windows.Forms.ListView();
            this.tabConfigSubtitle = new System.Windows.Forms.TabPage();
            this.btnSubMoveUp = new System.Windows.Forms.Button();
            this.colAudioId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cboAudioLang = new System.Windows.Forms.ComboBox();
            this.btnAudioMoveDown = new System.Windows.Forms.Button();
            this.btnAudioMoveUp = new System.Windows.Forms.Button();
            this.Seperator4 = new System.Windows.Forms.Label();
            this.btnAudioDel = new System.Windows.Forms.Button();
            this.btnAudioAdd = new System.Windows.Forms.Button();
            this.btnOutputBrowse = new System.Windows.Forms.Button();
            this.lblAudioLang = new System.Windows.Forms.Label();
            this.grpAudioCodec = new System.Windows.Forms.GroupBox();
            this.btnAudioEnc = new System.Windows.Forms.Button();
            this.btnAudioDec = new System.Windows.Forms.Button();
            this.lblAudioAdv = new System.Windows.Forms.Label();
            this.cboAudioMode = new System.Windows.Forms.ComboBox();
            this.lblAudioMode = new System.Windows.Forms.Label();
            this.cboAudioChannel = new System.Windows.Forms.ComboBox();
            this.lblAudioChannel = new System.Windows.Forms.Label();
            this.cboAudioSampleRate = new System.Windows.Forms.ComboBox();
            this.lblAudioSampleRate = new System.Windows.Forms.Label();
            this.cboAudioQuality = new System.Windows.Forms.ComboBox();
            this.lblAudioQuality = new System.Windows.Forms.Label();
            this.cboAudioEncoder = new System.Windows.Forms.ComboBox();
            this.lblAudioEncoder = new System.Windows.Forms.Label();
            this.tabConfigAudio = new System.Windows.Forms.TabPage();
            this.lstAudio = new System.Windows.Forms.ListView();
            this.colAudioLang = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAudioBitRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAudioSampleRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAudioChannel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstVideo = new System.Windows.Forms.ListView();
            this.colVideoId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVideoLang = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVideoRes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVideoFps = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVideoBitDepth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVideoPixFmt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabConfigAttachment = new System.Windows.Forms.TabPage();
            this.cboAttachMime = new System.Windows.Forms.ComboBox();
            this.lblAttachMime = new System.Windows.Forms.Label();
            this.lstAttach = new System.Windows.Forms.ListView();
            this.colAttachId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAttachFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAttachMime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAttachDel = new System.Windows.Forms.Button();
            this.btnAttachAdd = new System.Windows.Forms.Button();
            this.btnProfileSaveLoad = new System.Windows.Forms.Button();
            this.cboProfile = new System.Windows.Forms.ComboBox();
            this.cboFormat = new System.Windows.Forms.ComboBox();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.lblFormatProfile = new System.Windows.Forms.Label();
            this.PbxBanner = new System.Windows.Forms.PictureBox();
            this.tabConfigLog = new System.Windows.Forms.TabPage();
            this.rtfConsole = new System.Windows.Forms.RichTextBox();
            this.chkAdvTrim = new System.Windows.Forms.CheckBox();
            this.tabConfigAdvance = new System.Windows.Forms.TabPage();
            this.grpAdvHdr = new System.Windows.Forms.GroupBox();
            this.grpAdvTrim = new System.Windows.Forms.GroupBox();
            this.txtTrimDuration = new System.Windows.Forms.TextBox();
            this.txtTrimEnd = new System.Windows.Forms.TextBox();
            this.txtTrimStart = new System.Windows.Forms.TextBox();
            this.lblAdvTimeEqual = new System.Windows.Forms.Label();
            this.lblAdvTimeUntil = new System.Windows.Forms.Label();
            this.lblAdvTimeEnd = new System.Windows.Forms.Label();
            this.lblAdvTimeDuration = new System.Windows.Forms.Label();
            this.lblAdvTimeStart = new System.Windows.Forms.Label();
            this.btnVideoAdd = new System.Windows.Forms.Button();
            this.cboVideoPixFmt = new System.Windows.Forms.ComboBox();
            this.cboVideoBitDepth = new System.Windows.Forms.ComboBox();
            this.lblVideoPixFmt = new System.Windows.Forms.Label();
            this.lblVideoBitDepth = new System.Windows.Forms.Label();
            this.lblVideoFps = new System.Windows.Forms.Label();
            this.cboVideoFps = new System.Windows.Forms.ComboBox();
            this.grpVideoPicture = new System.Windows.Forms.GroupBox();
            this.cboVideoRes = new System.Windows.Forms.ComboBox();
            this.lblVideoRes = new System.Windows.Forms.Label();
            this.cboVideoDeInterField = new System.Windows.Forms.ComboBox();
            this.lblVideoDeInterField = new System.Windows.Forms.Label();
            this.cboVideoDeInterMode = new System.Windows.Forms.ComboBox();
            this.lblVideoDeInterMode = new System.Windows.Forms.Label();
            this.lblVideoLang = new System.Windows.Forms.Label();
            this.chkVideoDeInterlace = new System.Windows.Forms.CheckBox();
            this.grpVideoInterlace = new System.Windows.Forms.GroupBox();
            this.tabConfigVideo = new System.Windows.Forms.TabPage();
            this.grpVideoCodec = new System.Windows.Forms.GroupBox();
            this.btnVideoEnc = new System.Windows.Forms.Button();
            this.lblVideoAdv = new System.Windows.Forms.Label();
            this.btnVideoDec = new System.Windows.Forms.Button();
            this.nudVideoMultiPass = new System.Windows.Forms.NumericUpDown();
            this.nudVideoRateFactor = new System.Windows.Forms.NumericUpDown();
            this.lblVideoMultiPass = new System.Windows.Forms.Label();
            this.lblVideoRateFactor = new System.Windows.Forms.Label();
            this.cboVideoRateControl = new System.Windows.Forms.ComboBox();
            this.lblVideoRateControl = new System.Windows.Forms.Label();
            this.cboVideoTune = new System.Windows.Forms.ComboBox();
            this.cboVideoPreset = new System.Windows.Forms.ComboBox();
            this.lblVideoTune = new System.Windows.Forms.Label();
            this.lblVideoPreset = new System.Windows.Forms.Label();
            this.cboVideoEncoder = new System.Windows.Forms.ComboBox();
            this.lblVideoEncoder = new System.Windows.Forms.Label();
            this.cboVideoLang = new System.Windows.Forms.ComboBox();
            this.btnVideoMoveDown = new System.Windows.Forms.Button();
            this.btnVideoMoveUp = new System.Windows.Forms.Button();
            this.Seperator3 = new System.Windows.Forms.Label();
            this.btnVideoDel = new System.Windows.Forms.Button();
            this.tabConfig = new System.Windows.Forms.TabControl();
            this.tabConfigMediaInfo = new System.Windows.Forms.TabPage();
            this.txtMediaInfo = new System.Windows.Forms.TextBox();
            this.lstFile = new System.Windows.Forms.ListView();
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDonate = new System.Windows.Forms.Button();
            this.Seperator2 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFileDown = new System.Windows.Forms.Button();
            this.btnFileUp = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.Seperator1 = new System.Windows.Forms.Label();
            this.btnFileDelete = new System.Windows.Forms.Button();
            this.btnFileAdd = new System.Windows.Forms.Button();
            this.cmsFileAdd = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiImportFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportImgSeq = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiImportYouTube = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsProfiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiProfilesSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProfilesRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProfilesDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPower = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPowerOff = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsFileAddSubs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiFileAddSubs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFileAddSubsEmbed = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsFileAddAttach = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiFileAddAttach = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFileAddAttachEmbed = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbout = new System.Windows.Forms.Button();
            this.tabConfigSubtitle.SuspendLayout();
            this.grpAudioCodec.SuspendLayout();
            this.tabConfigAudio.SuspendLayout();
            this.tabConfigAttachment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbxBanner)).BeginInit();
            this.tabConfigLog.SuspendLayout();
            this.tabConfigAdvance.SuspendLayout();
            this.grpAdvTrim.SuspendLayout();
            this.grpVideoPicture.SuspendLayout();
            this.grpVideoInterlace.SuspendLayout();
            this.tabConfigVideo.SuspendLayout();
            this.grpVideoCodec.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoMultiPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoRateFactor)).BeginInit();
            this.tabConfig.SuspendLayout();
            this.tabConfigMediaInfo.SuspendLayout();
            this.cmsFileAdd.SuspendLayout();
            this.cmsProfiles.SuspendLayout();
            this.cmsPower.SuspendLayout();
            this.cmsFileAddSubs.SuspendLayout();
            this.cmsFileAddAttach.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboSubLang
            // 
            this.cboSubLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSubLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubLang.FormattingEnabled = true;
            this.cboSubLang.Location = new System.Drawing.Point(6, 237);
            this.cboSubLang.Name = "cboSubLang";
            this.cboSubLang.Size = new System.Drawing.Size(980, 21);
            this.cboSubLang.TabIndex = 8;
            this.cboSubLang.SelectedIndexChanged += new System.EventHandler(this.cboSubLang_SelectedIndexChanged);
            // 
            // btnSubAdd
            // 
            this.btnSubAdd.Location = new System.Drawing.Point(6, 6);
            this.btnSubAdd.Name = "btnSubAdd";
            this.btnSubAdd.Size = new System.Drawing.Size(24, 24);
            this.btnSubAdd.TabIndex = 0;
            this.btnSubAdd.UseVisualStyleBackColor = true;
            this.btnSubAdd.Click += new System.EventHandler(this.btnSubAdd_Click);
            // 
            // lblSubLang
            // 
            this.lblSubLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubLang.Location = new System.Drawing.Point(6, 216);
            this.lblSubLang.Name = "lblSubLang";
            this.lblSubLang.Size = new System.Drawing.Size(756, 18);
            this.lblSubLang.TabIndex = 7;
            this.lblSubLang.Text = "&Language:";
            this.lblSubLang.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnSubDel
            // 
            this.btnSubDel.Location = new System.Drawing.Point(36, 6);
            this.btnSubDel.Name = "btnSubDel";
            this.btnSubDel.Size = new System.Drawing.Size(24, 24);
            this.btnSubDel.TabIndex = 1;
            this.btnSubDel.UseVisualStyleBackColor = true;
            this.btnSubDel.Click += new System.EventHandler(this.btnSubDel_Click);
            // 
            // btnSubMoveDown
            // 
            this.btnSubMoveDown.Location = new System.Drawing.Point(104, 6);
            this.btnSubMoveDown.Name = "btnSubMoveDown";
            this.btnSubMoveDown.Size = new System.Drawing.Size(24, 24);
            this.btnSubMoveDown.TabIndex = 4;
            this.btnSubMoveDown.UseVisualStyleBackColor = true;
            this.btnSubMoveDown.Click += new System.EventHandler(this.btnSubMoveDown_Click);
            // 
            // colSubLang
            // 
            this.colSubLang.Text = "Language";
            this.colSubLang.Width = 220;
            // 
            // colSubFileName
            // 
            this.colSubFileName.Text = "File name";
            this.colSubFileName.Width = 481;
            // 
            // colSubId
            // 
            this.colSubId.Text = "Id";
            this.colSubId.Width = 50;
            // 
            // Seperator6
            // 
            this.Seperator6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Seperator6.Location = new System.Drawing.Point(66, 6);
            this.Seperator6.Name = "Seperator6";
            this.Seperator6.Size = new System.Drawing.Size(2, 24);
            this.Seperator6.TabIndex = 2;
            // 
            // chkSubHard
            // 
            this.chkSubHard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSubHard.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSubHard.Location = new System.Drawing.Point(736, 6);
            this.chkSubHard.Name = "chkSubHard";
            this.chkSubHard.Size = new System.Drawing.Size(250, 24);
            this.chkSubHard.TabIndex = 5;
            this.chkSubHard.Text = "&Burn Subtitle (Hard Sub)";
            this.chkSubHard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSubHard.UseVisualStyleBackColor = true;
            this.chkSubHard.CheckedChanged += new System.EventHandler(this.chkSubHard_CheckedChanged);
            // 
            // lstSub
            // 
            this.lstSub.AllowDrop = true;
            this.lstSub.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSubId,
            this.colSubFileName,
            this.colSubLang});
            this.lstSub.FullRowSelect = true;
            this.lstSub.HideSelection = false;
            this.lstSub.Location = new System.Drawing.Point(6, 36);
            this.lstSub.Name = "lstSub";
            this.lstSub.Size = new System.Drawing.Size(980, 177);
            this.lstSub.TabIndex = 6;
            this.lstSub.UseCompatibleStateImageBehavior = false;
            this.lstSub.View = System.Windows.Forms.View.Details;
            this.lstSub.SelectedIndexChanged += new System.EventHandler(this.lstSub_SelectedIndexChanged);
            this.lstSub.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstSub_DragDrop);
            this.lstSub.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstSub_DragEnter);
            // 
            // tabConfigSubtitle
            // 
            this.tabConfigSubtitle.Controls.Add(this.Seperator6);
            this.tabConfigSubtitle.Controls.Add(this.chkSubHard);
            this.tabConfigSubtitle.Controls.Add(this.lstSub);
            this.tabConfigSubtitle.Controls.Add(this.cboSubLang);
            this.tabConfigSubtitle.Controls.Add(this.btnSubAdd);
            this.tabConfigSubtitle.Controls.Add(this.lblSubLang);
            this.tabConfigSubtitle.Controls.Add(this.btnSubDel);
            this.tabConfigSubtitle.Controls.Add(this.btnSubMoveUp);
            this.tabConfigSubtitle.Controls.Add(this.btnSubMoveDown);
            this.tabConfigSubtitle.Location = new System.Drawing.Point(4, 24);
            this.tabConfigSubtitle.Name = "tabConfigSubtitle";
            this.tabConfigSubtitle.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigSubtitle.Size = new System.Drawing.Size(992, 264);
            this.tabConfigSubtitle.TabIndex = 2;
            this.tabConfigSubtitle.Text = "Subtitle";
            this.tabConfigSubtitle.UseVisualStyleBackColor = true;
            // 
            // btnSubMoveUp
            // 
            this.btnSubMoveUp.Location = new System.Drawing.Point(74, 6);
            this.btnSubMoveUp.Name = "btnSubMoveUp";
            this.btnSubMoveUp.Size = new System.Drawing.Size(24, 24);
            this.btnSubMoveUp.TabIndex = 3;
            this.btnSubMoveUp.UseVisualStyleBackColor = true;
            this.btnSubMoveUp.Click += new System.EventHandler(this.btnSubMoveUp_Click);
            // 
            // colAudioId
            // 
            this.colAudioId.Text = "Id";
            this.colAudioId.Width = 32;
            // 
            // cboAudioLang
            // 
            this.cboAudioLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAudioLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioLang.FormattingEnabled = true;
            this.cboAudioLang.Location = new System.Drawing.Point(6, 237);
            this.cboAudioLang.Name = "cboAudioLang";
            this.cboAudioLang.Size = new System.Drawing.Size(474, 21);
            this.cboAudioLang.TabIndex = 7;
            this.cboAudioLang.SelectedIndexChanged += new System.EventHandler(this.cboAudioLang_SelectedIndexChanged);
            // 
            // btnAudioMoveDown
            // 
            this.btnAudioMoveDown.Location = new System.Drawing.Point(104, 6);
            this.btnAudioMoveDown.Name = "btnAudioMoveDown";
            this.btnAudioMoveDown.Size = new System.Drawing.Size(24, 24);
            this.btnAudioMoveDown.TabIndex = 4;
            this.btnAudioMoveDown.UseVisualStyleBackColor = true;
            this.btnAudioMoveDown.Click += new System.EventHandler(this.btnAudioMoveDown_Click);
            // 
            // btnAudioMoveUp
            // 
            this.btnAudioMoveUp.Location = new System.Drawing.Point(74, 6);
            this.btnAudioMoveUp.Name = "btnAudioMoveUp";
            this.btnAudioMoveUp.Size = new System.Drawing.Size(24, 24);
            this.btnAudioMoveUp.TabIndex = 3;
            this.btnAudioMoveUp.UseVisualStyleBackColor = true;
            this.btnAudioMoveUp.Click += new System.EventHandler(this.btnAudioMoveUp_Click);
            // 
            // Seperator4
            // 
            this.Seperator4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Seperator4.Location = new System.Drawing.Point(66, 6);
            this.Seperator4.Name = "Seperator4";
            this.Seperator4.Size = new System.Drawing.Size(2, 24);
            this.Seperator4.TabIndex = 2;
            // 
            // btnAudioDel
            // 
            this.btnAudioDel.Location = new System.Drawing.Point(36, 6);
            this.btnAudioDel.Name = "btnAudioDel";
            this.btnAudioDel.Size = new System.Drawing.Size(24, 24);
            this.btnAudioDel.TabIndex = 1;
            this.btnAudioDel.UseVisualStyleBackColor = true;
            this.btnAudioDel.Click += new System.EventHandler(this.btnAudioDel_Click);
            // 
            // btnAudioAdd
            // 
            this.btnAudioAdd.Location = new System.Drawing.Point(6, 6);
            this.btnAudioAdd.Name = "btnAudioAdd";
            this.btnAudioAdd.Size = new System.Drawing.Size(24, 24);
            this.btnAudioAdd.TabIndex = 0;
            this.btnAudioAdd.UseVisualStyleBackColor = true;
            this.btnAudioAdd.Click += new System.EventHandler(this.btnAudioAdd_Click);
            // 
            // btnOutputBrowse
            // 
            this.btnOutputBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutputBrowse.Location = new System.Drawing.Point(988, 564);
            this.btnOutputBrowse.Name = "btnOutputBrowse";
            this.btnOutputBrowse.Size = new System.Drawing.Size(24, 24);
            this.btnOutputBrowse.TabIndex = 19;
            this.btnOutputBrowse.UseVisualStyleBackColor = true;
            this.btnOutputBrowse.Click += new System.EventHandler(this.btnOutputBrowse_Click);
            // 
            // lblAudioLang
            // 
            this.lblAudioLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAudioLang.Location = new System.Drawing.Point(6, 216);
            this.lblAudioLang.Name = "lblAudioLang";
            this.lblAudioLang.Size = new System.Drawing.Size(250, 18);
            this.lblAudioLang.TabIndex = 6;
            this.lblAudioLang.Text = "&Language:";
            this.lblAudioLang.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // grpAudioCodec
            // 
            this.grpAudioCodec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAudioCodec.Controls.Add(this.btnAudioEnc);
            this.grpAudioCodec.Controls.Add(this.btnAudioDec);
            this.grpAudioCodec.Controls.Add(this.lblAudioAdv);
            this.grpAudioCodec.Controls.Add(this.cboAudioMode);
            this.grpAudioCodec.Controls.Add(this.lblAudioMode);
            this.grpAudioCodec.Controls.Add(this.cboAudioChannel);
            this.grpAudioCodec.Controls.Add(this.lblAudioChannel);
            this.grpAudioCodec.Controls.Add(this.cboAudioSampleRate);
            this.grpAudioCodec.Controls.Add(this.lblAudioSampleRate);
            this.grpAudioCodec.Controls.Add(this.cboAudioQuality);
            this.grpAudioCodec.Controls.Add(this.lblAudioQuality);
            this.grpAudioCodec.Controls.Add(this.cboAudioEncoder);
            this.grpAudioCodec.Controls.Add(this.lblAudioEncoder);
            this.grpAudioCodec.Location = new System.Drawing.Point(486, 6);
            this.grpAudioCodec.Name = "grpAudioCodec";
            this.grpAudioCodec.Size = new System.Drawing.Size(500, 252);
            this.grpAudioCodec.TabIndex = 8;
            this.grpAudioCodec.TabStop = false;
            this.grpAudioCodec.Text = "&Codec";
            // 
            // btnAudioEnc
            // 
            this.btnAudioEnc.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAudioEnc.Location = new System.Drawing.Point(253, 170);
            this.btnAudioEnc.Name = "btnAudioEnc";
            this.btnAudioEnc.Size = new System.Drawing.Size(185, 23);
            this.btnAudioEnc.TabIndex = 14;
            this.btnAudioEnc.Text = "E&ncoder CLI";
            this.btnAudioEnc.UseVisualStyleBackColor = true;
            this.btnAudioEnc.Click += new System.EventHandler(this.btnAudioEnc_Click);
            // 
            // btnAudioDec
            // 
            this.btnAudioDec.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAudioDec.Location = new System.Drawing.Point(63, 170);
            this.btnAudioDec.Name = "btnAudioDec";
            this.btnAudioDec.Size = new System.Drawing.Size(184, 23);
            this.btnAudioDec.TabIndex = 13;
            this.btnAudioDec.Text = "&Decoder CLI";
            this.btnAudioDec.UseVisualStyleBackColor = true;
            this.btnAudioDec.Click += new System.EventHandler(this.btnAudioDec_Click);
            // 
            // lblAudioAdv
            // 
            this.lblAudioAdv.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAudioAdv.Location = new System.Drawing.Point(63, 149);
            this.lblAudioAdv.Name = "lblAudioAdv";
            this.lblAudioAdv.Size = new System.Drawing.Size(375, 18);
            this.lblAudioAdv.TabIndex = 12;
            this.lblAudioAdv.Text = "Advance &Option for Decoder and Encoder:";
            this.lblAudioAdv.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboAudioMode
            // 
            this.cboAudioMode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboAudioMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioMode.FormattingEnabled = true;
            this.cboAudioMode.Location = new System.Drawing.Point(317, 80);
            this.cboAudioMode.Name = "cboAudioMode";
            this.cboAudioMode.Size = new System.Drawing.Size(121, 21);
            this.cboAudioMode.TabIndex = 5;
            this.cboAudioMode.SelectedIndexChanged += new System.EventHandler(this.cboAudioMode_SelectedIndexChanged);
            // 
            // lblAudioMode
            // 
            this.lblAudioMode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAudioMode.Location = new System.Drawing.Point(317, 59);
            this.lblAudioMode.Name = "lblAudioMode";
            this.lblAudioMode.Size = new System.Drawing.Size(121, 18);
            this.lblAudioMode.TabIndex = 4;
            this.lblAudioMode.Text = "&Mode:";
            this.lblAudioMode.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboAudioChannel
            // 
            this.cboAudioChannel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboAudioChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioChannel.FormattingEnabled = true;
            this.cboAudioChannel.Location = new System.Drawing.Point(317, 125);
            this.cboAudioChannel.Name = "cboAudioChannel";
            this.cboAudioChannel.Size = new System.Drawing.Size(121, 21);
            this.cboAudioChannel.TabIndex = 11;
            this.cboAudioChannel.SelectedIndexChanged += new System.EventHandler(this.cboAudioChannel_SelectedIndexChanged);
            // 
            // lblAudioChannel
            // 
            this.lblAudioChannel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAudioChannel.Location = new System.Drawing.Point(317, 104);
            this.lblAudioChannel.Name = "lblAudioChannel";
            this.lblAudioChannel.Size = new System.Drawing.Size(121, 18);
            this.lblAudioChannel.TabIndex = 10;
            this.lblAudioChannel.Text = "C&hannel:";
            this.lblAudioChannel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboAudioSampleRate
            // 
            this.cboAudioSampleRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboAudioSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioSampleRate.FormattingEnabled = true;
            this.cboAudioSampleRate.Location = new System.Drawing.Point(190, 125);
            this.cboAudioSampleRate.Name = "cboAudioSampleRate";
            this.cboAudioSampleRate.Size = new System.Drawing.Size(121, 21);
            this.cboAudioSampleRate.TabIndex = 9;
            this.cboAudioSampleRate.SelectedIndexChanged += new System.EventHandler(this.cboAudioSampleRate_SelectedIndexChanged);
            // 
            // lblAudioSampleRate
            // 
            this.lblAudioSampleRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAudioSampleRate.Location = new System.Drawing.Point(190, 104);
            this.lblAudioSampleRate.Name = "lblAudioSampleRate";
            this.lblAudioSampleRate.Size = new System.Drawing.Size(121, 18);
            this.lblAudioSampleRate.TabIndex = 8;
            this.lblAudioSampleRate.Text = "Sample &Rate:";
            this.lblAudioSampleRate.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboAudioQuality
            // 
            this.cboAudioQuality.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboAudioQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioQuality.FormattingEnabled = true;
            this.cboAudioQuality.Location = new System.Drawing.Point(63, 125);
            this.cboAudioQuality.Name = "cboAudioQuality";
            this.cboAudioQuality.Size = new System.Drawing.Size(121, 21);
            this.cboAudioQuality.TabIndex = 7;
            this.cboAudioQuality.SelectedIndexChanged += new System.EventHandler(this.cboAudioQuality_SelectedIndexChanged);
            // 
            // lblAudioQuality
            // 
            this.lblAudioQuality.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAudioQuality.Location = new System.Drawing.Point(63, 104);
            this.lblAudioQuality.Name = "lblAudioQuality";
            this.lblAudioQuality.Size = new System.Drawing.Size(121, 18);
            this.lblAudioQuality.TabIndex = 6;
            this.lblAudioQuality.Text = "&Quality:";
            this.lblAudioQuality.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboAudioEncoder
            // 
            this.cboAudioEncoder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboAudioEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioEncoder.FormattingEnabled = true;
            this.cboAudioEncoder.Location = new System.Drawing.Point(63, 80);
            this.cboAudioEncoder.Name = "cboAudioEncoder";
            this.cboAudioEncoder.Size = new System.Drawing.Size(248, 21);
            this.cboAudioEncoder.TabIndex = 3;
            this.cboAudioEncoder.SelectedIndexChanged += new System.EventHandler(this.cboAudioEncoder_SelectedIndexChanged);
            // 
            // lblAudioEncoder
            // 
            this.lblAudioEncoder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAudioEncoder.Location = new System.Drawing.Point(63, 59);
            this.lblAudioEncoder.Name = "lblAudioEncoder";
            this.lblAudioEncoder.Size = new System.Drawing.Size(248, 18);
            this.lblAudioEncoder.TabIndex = 2;
            this.lblAudioEncoder.Text = "&Encoder:";
            this.lblAudioEncoder.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tabConfigAudio
            // 
            this.tabConfigAudio.Controls.Add(this.lblAudioLang);
            this.tabConfigAudio.Controls.Add(this.grpAudioCodec);
            this.tabConfigAudio.Controls.Add(this.cboAudioLang);
            this.tabConfigAudio.Controls.Add(this.btnAudioMoveDown);
            this.tabConfigAudio.Controls.Add(this.btnAudioMoveUp);
            this.tabConfigAudio.Controls.Add(this.Seperator4);
            this.tabConfigAudio.Controls.Add(this.btnAudioDel);
            this.tabConfigAudio.Controls.Add(this.btnAudioAdd);
            this.tabConfigAudio.Controls.Add(this.lstAudio);
            this.tabConfigAudio.Location = new System.Drawing.Point(4, 24);
            this.tabConfigAudio.Name = "tabConfigAudio";
            this.tabConfigAudio.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigAudio.Size = new System.Drawing.Size(992, 264);
            this.tabConfigAudio.TabIndex = 1;
            this.tabConfigAudio.Text = "Audio";
            this.tabConfigAudio.UseVisualStyleBackColor = true;
            // 
            // lstAudio
            // 
            this.lstAudio.AllowDrop = true;
            this.lstAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAudio.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAudioId,
            this.colAudioLang,
            this.colAudioBitRate,
            this.colAudioSampleRate,
            this.colAudioChannel});
            this.lstAudio.FullRowSelect = true;
            this.lstAudio.HideSelection = false;
            this.lstAudio.Location = new System.Drawing.Point(6, 36);
            this.lstAudio.Name = "lstAudio";
            this.lstAudio.Size = new System.Drawing.Size(474, 177);
            this.lstAudio.TabIndex = 5;
            this.lstAudio.UseCompatibleStateImageBehavior = false;
            this.lstAudio.View = System.Windows.Forms.View.Details;
            this.lstAudio.SelectedIndexChanged += new System.EventHandler(this.lstAudio_SelectedIndexChanged);
            this.lstAudio.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstAudio_DragDrop);
            this.lstAudio.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstAudio_DragEnter);
            // 
            // colAudioLang
            // 
            this.colAudioLang.Text = "Language";
            this.colAudioLang.Width = 100;
            // 
            // colAudioBitRate
            // 
            this.colAudioBitRate.Text = "Bit Rate";
            this.colAudioBitRate.Width = 100;
            // 
            // colAudioSampleRate
            // 
            this.colAudioSampleRate.Text = "Sample Rate";
            this.colAudioSampleRate.Width = 100;
            // 
            // colAudioChannel
            // 
            this.colAudioChannel.Text = "Channel";
            this.colAudioChannel.Width = 100;
            // 
            // lstVideo
            // 
            this.lstVideo.AllowDrop = true;
            this.lstVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstVideo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colVideoId,
            this.colVideoLang,
            this.colVideoRes,
            this.colVideoFps,
            this.colVideoBitDepth,
            this.colVideoPixFmt});
            this.lstVideo.FullRowSelect = true;
            this.lstVideo.HideSelection = false;
            this.lstVideo.Location = new System.Drawing.Point(6, 36);
            this.lstVideo.Name = "lstVideo";
            this.lstVideo.Size = new System.Drawing.Size(474, 177);
            this.lstVideo.TabIndex = 5;
            this.lstVideo.UseCompatibleStateImageBehavior = false;
            this.lstVideo.View = System.Windows.Forms.View.Details;
            this.lstVideo.SelectedIndexChanged += new System.EventHandler(this.lstVideo_SelectedIndexChanged);
            this.lstVideo.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstVideo_DragDrop);
            this.lstVideo.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstVideo_DragEnter);
            // 
            // colVideoId
            // 
            this.colVideoId.Text = "Id";
            this.colVideoId.Width = 32;
            // 
            // colVideoLang
            // 
            this.colVideoLang.Text = "Language";
            this.colVideoLang.Width = 100;
            // 
            // colVideoRes
            // 
            this.colVideoRes.Text = "Resolution";
            this.colVideoRes.Width = 100;
            // 
            // colVideoFps
            // 
            this.colVideoFps.Text = "fps";
            // 
            // colVideoBitDepth
            // 
            this.colVideoBitDepth.Text = "Bit Depth";
            // 
            // colVideoPixFmt
            // 
            this.colVideoPixFmt.Text = "Pixel Format";
            this.colVideoPixFmt.Width = 100;
            // 
            // tabConfigAttachment
            // 
            this.tabConfigAttachment.Controls.Add(this.cboAttachMime);
            this.tabConfigAttachment.Controls.Add(this.lblAttachMime);
            this.tabConfigAttachment.Controls.Add(this.lstAttach);
            this.tabConfigAttachment.Controls.Add(this.btnAttachDel);
            this.tabConfigAttachment.Controls.Add(this.btnAttachAdd);
            this.tabConfigAttachment.Location = new System.Drawing.Point(4, 24);
            this.tabConfigAttachment.Name = "tabConfigAttachment";
            this.tabConfigAttachment.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigAttachment.Size = new System.Drawing.Size(992, 264);
            this.tabConfigAttachment.TabIndex = 3;
            this.tabConfigAttachment.Text = "Attachment";
            this.tabConfigAttachment.UseVisualStyleBackColor = true;
            // 
            // cboAttachMime
            // 
            this.cboAttachMime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAttachMime.FormattingEnabled = true;
            this.cboAttachMime.Location = new System.Drawing.Point(6, 237);
            this.cboAttachMime.Name = "cboAttachMime";
            this.cboAttachMime.Size = new System.Drawing.Size(980, 21);
            this.cboAttachMime.TabIndex = 4;
            this.cboAttachMime.TextChanged += new System.EventHandler(this.cboAttachMime_TextChanged);
            // 
            // lblAttachMime
            // 
            this.lblAttachMime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttachMime.Location = new System.Drawing.Point(6, 216);
            this.lblAttachMime.Name = "lblAttachMime";
            this.lblAttachMime.Size = new System.Drawing.Size(756, 18);
            this.lblAttachMime.TabIndex = 3;
            this.lblAttachMime.Text = "&MIME Type:";
            this.lblAttachMime.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lstAttach
            // 
            this.lstAttach.AllowDrop = true;
            this.lstAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAttach.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAttachId,
            this.colAttachFileName,
            this.colAttachMime});
            this.lstAttach.FullRowSelect = true;
            this.lstAttach.HideSelection = false;
            this.lstAttach.Location = new System.Drawing.Point(6, 36);
            this.lstAttach.Name = "lstAttach";
            this.lstAttach.Size = new System.Drawing.Size(980, 177);
            this.lstAttach.TabIndex = 2;
            this.lstAttach.UseCompatibleStateImageBehavior = false;
            this.lstAttach.View = System.Windows.Forms.View.Details;
            this.lstAttach.SelectedIndexChanged += new System.EventHandler(this.lstAttach_SelectedIndexChanged);
            this.lstAttach.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstAttach_DragDrop);
            this.lstAttach.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstAttach_DragEnter);
            // 
            // colAttachId
            // 
            this.colAttachId.Text = "Id";
            this.colAttachId.Width = 50;
            // 
            // colAttachFileName
            // 
            this.colAttachFileName.Text = "File name";
            this.colAttachFileName.Width = 480;
            // 
            // colAttachMime
            // 
            this.colAttachMime.Text = "MIME";
            this.colAttachMime.Width = 220;
            // 
            // btnAttachDel
            // 
            this.btnAttachDel.Location = new System.Drawing.Point(36, 6);
            this.btnAttachDel.Name = "btnAttachDel";
            this.btnAttachDel.Size = new System.Drawing.Size(24, 24);
            this.btnAttachDel.TabIndex = 1;
            this.btnAttachDel.UseVisualStyleBackColor = true;
            this.btnAttachDel.Click += new System.EventHandler(this.btnAttachDel_Click);
            // 
            // btnAttachAdd
            // 
            this.btnAttachAdd.Location = new System.Drawing.Point(6, 6);
            this.btnAttachAdd.Name = "btnAttachAdd";
            this.btnAttachAdd.Size = new System.Drawing.Size(24, 24);
            this.btnAttachAdd.TabIndex = 0;
            this.btnAttachAdd.UseVisualStyleBackColor = true;
            this.btnAttachAdd.Click += new System.EventHandler(this.btnAttachAdd_Click);
            // 
            // btnProfileSaveLoad
            // 
            this.btnProfileSaveLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfileSaveLoad.Location = new System.Drawing.Point(988, 534);
            this.btnProfileSaveLoad.Name = "btnProfileSaveLoad";
            this.btnProfileSaveLoad.Size = new System.Drawing.Size(24, 24);
            this.btnProfileSaveLoad.TabIndex = 16;
            this.btnProfileSaveLoad.UseVisualStyleBackColor = true;
            this.btnProfileSaveLoad.Click += new System.EventHandler(this.btnProfileSaveLoad_Click);
            this.btnProfileSaveLoad.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnProfileSaveLoad_MouseUp);
            // 
            // cboProfile
            // 
            this.cboProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfile.Font = new System.Drawing.Font("Tahoma", 10F);
            this.cboProfile.FormattingEnabled = true;
            this.cboProfile.Location = new System.Drawing.Point(274, 534);
            this.cboProfile.Name = "cboProfile";
            this.cboProfile.Size = new System.Drawing.Size(708, 24);
            this.cboProfile.TabIndex = 15;
            this.cboProfile.SelectedIndexChanged += new System.EventHandler(this.cboProfile_SelectedIndexChanged);
            // 
            // cboFormat
            // 
            this.cboFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFormat.Font = new System.Drawing.Font("Tahoma", 10F);
            this.cboFormat.FormattingEnabled = true;
            this.cboFormat.Location = new System.Drawing.Point(148, 534);
            this.cboFormat.Name = "cboFormat";
            this.cboFormat.Size = new System.Drawing.Size(120, 24);
            this.cboFormat.TabIndex = 14;
            this.cboFormat.SelectedIndexChanged += new System.EventHandler(this.cboFormat_SelectedIndexChanged);
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputPath.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtOutputPath.Location = new System.Drawing.Point(148, 564);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(834, 24);
            this.txtOutputPath.TabIndex = 18;
            this.txtOutputPath.TextChanged += new System.EventHandler(this.txtOutputPath_TextChanged);
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOutputPath.Location = new System.Drawing.Point(12, 564);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(130, 24);
            this.lblOutputPath.TabIndex = 17;
            this.lblOutputPath.Text = "&Output Folder:";
            this.lblOutputPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFormatProfile
            // 
            this.lblFormatProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFormatProfile.Location = new System.Drawing.Point(12, 534);
            this.lblFormatProfile.Name = "lblFormatProfile";
            this.lblFormatProfile.Size = new System.Drawing.Size(130, 24);
            this.lblFormatProfile.TabIndex = 13;
            this.lblFormatProfile.Text = "Format && &Profile:";
            this.lblFormatProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PbxBanner
            // 
            this.PbxBanner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PbxBanner.BackColor = System.Drawing.Color.Black;
            this.PbxBanner.Location = new System.Drawing.Point(0, 0);
            this.PbxBanner.Name = "PbxBanner";
            this.PbxBanner.Size = new System.Drawing.Size(1024, 64);
            this.PbxBanner.TabIndex = 27;
            this.PbxBanner.TabStop = false;
            // 
            // tabConfigLog
            // 
            this.tabConfigLog.Controls.Add(this.rtfConsole);
            this.tabConfigLog.Location = new System.Drawing.Point(4, 24);
            this.tabConfigLog.Name = "tabConfigLog";
            this.tabConfigLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigLog.Size = new System.Drawing.Size(992, 264);
            this.tabConfigLog.TabIndex = 5;
            this.tabConfigLog.Text = "Logging";
            this.tabConfigLog.UseVisualStyleBackColor = true;
            // 
            // rtfConsole
            // 
            this.rtfConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.rtfConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfConsole.ForeColor = System.Drawing.Color.DarkGray;
            this.rtfConsole.Location = new System.Drawing.Point(3, 3);
            this.rtfConsole.Name = "rtfConsole";
            this.rtfConsole.ReadOnly = true;
            this.rtfConsole.Size = new System.Drawing.Size(986, 258);
            this.rtfConsole.TabIndex = 0;
            this.rtfConsole.Text = "";
            // 
            // chkAdvTrim
            // 
            this.chkAdvTrim.AutoSize = true;
            this.chkAdvTrim.Location = new System.Drawing.Point(15, 6);
            this.chkAdvTrim.Name = "chkAdvTrim";
            this.chkAdvTrim.Size = new System.Drawing.Size(266, 17);
            this.chkAdvTrim.TabIndex = 0;
            this.chkAdvTrim.Text = "&Trim Video && Audio (Copy stream wont effect this)";
            this.chkAdvTrim.UseVisualStyleBackColor = true;
            this.chkAdvTrim.CheckedChanged += new System.EventHandler(this.chkAdvTrim_CheckedChanged);
            // 
            // tabConfigAdvance
            // 
            this.tabConfigAdvance.Controls.Add(this.grpAdvHdr);
            this.tabConfigAdvance.Controls.Add(this.chkAdvTrim);
            this.tabConfigAdvance.Controls.Add(this.grpAdvTrim);
            this.tabConfigAdvance.Location = new System.Drawing.Point(4, 24);
            this.tabConfigAdvance.Name = "tabConfigAdvance";
            this.tabConfigAdvance.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigAdvance.Size = new System.Drawing.Size(992, 264);
            this.tabConfigAdvance.TabIndex = 4;
            this.tabConfigAdvance.Text = "Advanced";
            this.tabConfigAdvance.UseVisualStyleBackColor = true;
            // 
            // grpAdvHdr
            // 
            this.grpAdvHdr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAdvHdr.Location = new System.Drawing.Point(6, 112);
            this.grpAdvHdr.Name = "grpAdvHdr";
            this.grpAdvHdr.Size = new System.Drawing.Size(980, 146);
            this.grpAdvHdr.TabIndex = 2;
            this.grpAdvHdr.TabStop = false;
            this.grpAdvHdr.Text = "HDR";
            // 
            // grpAdvTrim
            // 
            this.grpAdvTrim.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAdvTrim.Controls.Add(this.txtTrimDuration);
            this.grpAdvTrim.Controls.Add(this.txtTrimEnd);
            this.grpAdvTrim.Controls.Add(this.txtTrimStart);
            this.grpAdvTrim.Controls.Add(this.lblAdvTimeEqual);
            this.grpAdvTrim.Controls.Add(this.lblAdvTimeUntil);
            this.grpAdvTrim.Controls.Add(this.lblAdvTimeEnd);
            this.grpAdvTrim.Controls.Add(this.lblAdvTimeDuration);
            this.grpAdvTrim.Controls.Add(this.lblAdvTimeStart);
            this.grpAdvTrim.Enabled = false;
            this.grpAdvTrim.Location = new System.Drawing.Point(6, 6);
            this.grpAdvTrim.Name = "grpAdvTrim";
            this.grpAdvTrim.Size = new System.Drawing.Size(980, 100);
            this.grpAdvTrim.TabIndex = 1;
            this.grpAdvTrim.TabStop = false;
            // 
            // txtTrimDuration
            // 
            this.txtTrimDuration.Font = new System.Drawing.Font("Tahoma", 9F);
            this.txtTrimDuration.Location = new System.Drawing.Point(574, 51);
            this.txtTrimDuration.Name = "txtTrimDuration";
            this.txtTrimDuration.Size = new System.Drawing.Size(100, 22);
            this.txtTrimDuration.TabIndex = 10;
            this.txtTrimDuration.Text = "00:01:00";
            this.txtTrimDuration.TextChanged += new System.EventHandler(this.txtTrim_TextChanged);
            this.txtTrimDuration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrim_KeyPress);
            this.txtTrimDuration.Validating += new System.ComponentModel.CancelEventHandler(this.txtTrim_Validating);
            // 
            // txtTrimEnd
            // 
            this.txtTrimEnd.Font = new System.Drawing.Font("Tahoma", 9F);
            this.txtTrimEnd.Location = new System.Drawing.Point(440, 52);
            this.txtTrimEnd.Name = "txtTrimEnd";
            this.txtTrimEnd.Size = new System.Drawing.Size(100, 22);
            this.txtTrimEnd.TabIndex = 9;
            this.txtTrimEnd.Text = "00:10:05";
            this.txtTrimEnd.TextChanged += new System.EventHandler(this.txtTrim_TextChanged);
            this.txtTrimEnd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrim_KeyPress);
            this.txtTrimEnd.Validating += new System.ComponentModel.CancelEventHandler(this.txtTrim_Validating);
            // 
            // txtTrimStart
            // 
            this.txtTrimStart.Font = new System.Drawing.Font("Tahoma", 9F);
            this.txtTrimStart.Location = new System.Drawing.Point(306, 52);
            this.txtTrimStart.Name = "txtTrimStart";
            this.txtTrimStart.Size = new System.Drawing.Size(100, 22);
            this.txtTrimStart.TabIndex = 8;
            this.txtTrimStart.Text = "00:09:46";
            this.txtTrimStart.TextChanged += new System.EventHandler(this.txtTrim_TextChanged);
            this.txtTrimStart.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrim_KeyPress);
            this.txtTrimStart.Validating += new System.ComponentModel.CancelEventHandler(this.txtTrim_Validating);
            // 
            // lblAdvTimeEqual
            // 
            this.lblAdvTimeEqual.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAdvTimeEqual.Location = new System.Drawing.Point(546, 51);
            this.lblAdvTimeEqual.Name = "lblAdvTimeEqual";
            this.lblAdvTimeEqual.Size = new System.Drawing.Size(22, 22);
            this.lblAdvTimeEqual.TabIndex = 7;
            this.lblAdvTimeEqual.Text = "=";
            this.lblAdvTimeEqual.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAdvTimeUntil
            // 
            this.lblAdvTimeUntil.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAdvTimeUntil.Location = new System.Drawing.Point(412, 51);
            this.lblAdvTimeUntil.Name = "lblAdvTimeUntil";
            this.lblAdvTimeUntil.Size = new System.Drawing.Size(22, 22);
            this.lblAdvTimeUntil.TabIndex = 6;
            this.lblAdvTimeUntil.Text = ">";
            this.lblAdvTimeUntil.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAdvTimeEnd
            // 
            this.lblAdvTimeEnd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAdvTimeEnd.Location = new System.Drawing.Point(440, 26);
            this.lblAdvTimeEnd.Name = "lblAdvTimeEnd";
            this.lblAdvTimeEnd.Size = new System.Drawing.Size(100, 22);
            this.lblAdvTimeEnd.TabIndex = 2;
            this.lblAdvTimeEnd.Text = "&End:";
            this.lblAdvTimeEnd.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblAdvTimeDuration
            // 
            this.lblAdvTimeDuration.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAdvTimeDuration.Location = new System.Drawing.Point(574, 26);
            this.lblAdvTimeDuration.Name = "lblAdvTimeDuration";
            this.lblAdvTimeDuration.Size = new System.Drawing.Size(100, 22);
            this.lblAdvTimeDuration.TabIndex = 4;
            this.lblAdvTimeDuration.Text = "&Duration:";
            this.lblAdvTimeDuration.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblAdvTimeStart
            // 
            this.lblAdvTimeStart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAdvTimeStart.Location = new System.Drawing.Point(306, 26);
            this.lblAdvTimeStart.Name = "lblAdvTimeStart";
            this.lblAdvTimeStart.Size = new System.Drawing.Size(100, 22);
            this.lblAdvTimeStart.TabIndex = 0;
            this.lblAdvTimeStart.Text = "&Start:";
            this.lblAdvTimeStart.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnVideoAdd
            // 
            this.btnVideoAdd.Location = new System.Drawing.Point(6, 6);
            this.btnVideoAdd.Name = "btnVideoAdd";
            this.btnVideoAdd.Size = new System.Drawing.Size(24, 24);
            this.btnVideoAdd.TabIndex = 0;
            this.btnVideoAdd.UseVisualStyleBackColor = true;
            this.btnVideoAdd.Click += new System.EventHandler(this.btnVideoAdd_Click);
            // 
            // cboVideoPixFmt
            // 
            this.cboVideoPixFmt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoPixFmt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoPixFmt.FormattingEnabled = true;
            this.cboVideoPixFmt.Items.AddRange(new object[] {
            "420",
            "422",
            "444"});
            this.cboVideoPixFmt.Location = new System.Drawing.Point(127, 82);
            this.cboVideoPixFmt.Name = "cboVideoPixFmt";
            this.cboVideoPixFmt.Size = new System.Drawing.Size(114, 21);
            this.cboVideoPixFmt.TabIndex = 7;
            this.cboVideoPixFmt.SelectedIndexChanged += new System.EventHandler(this.cboVideoPixFmt_SelectedIndexChanged);
            // 
            // cboVideoBitDepth
            // 
            this.cboVideoBitDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoBitDepth.FormattingEnabled = true;
            this.cboVideoBitDepth.Location = new System.Drawing.Point(6, 82);
            this.cboVideoBitDepth.Name = "cboVideoBitDepth";
            this.cboVideoBitDepth.Size = new System.Drawing.Size(115, 21);
            this.cboVideoBitDepth.TabIndex = 5;
            this.cboVideoBitDepth.SelectedIndexChanged += new System.EventHandler(this.cboVideoBitDepth_SelectedIndexChanged);
            // 
            // lblVideoPixFmt
            // 
            this.lblVideoPixFmt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoPixFmt.Location = new System.Drawing.Point(127, 61);
            this.lblVideoPixFmt.Name = "lblVideoPixFmt";
            this.lblVideoPixFmt.Size = new System.Drawing.Size(114, 18);
            this.lblVideoPixFmt.TabIndex = 6;
            this.lblVideoPixFmt.Text = "Pi&xel Format:";
            this.lblVideoPixFmt.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblVideoBitDepth
            // 
            this.lblVideoBitDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoBitDepth.Location = new System.Drawing.Point(6, 61);
            this.lblVideoBitDepth.Name = "lblVideoBitDepth";
            this.lblVideoBitDepth.Size = new System.Drawing.Size(115, 18);
            this.lblVideoBitDepth.TabIndex = 4;
            this.lblVideoBitDepth.Text = "&Bit-depth:";
            this.lblVideoBitDepth.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblVideoFps
            // 
            this.lblVideoFps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoFps.Location = new System.Drawing.Point(124, 16);
            this.lblVideoFps.Name = "lblVideoFps";
            this.lblVideoFps.Size = new System.Drawing.Size(117, 18);
            this.lblVideoFps.TabIndex = 2;
            this.lblVideoFps.Text = "Fr&ame Rate:";
            this.lblVideoFps.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboVideoFps
            // 
            this.cboVideoFps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoFps.FormattingEnabled = true;
            this.cboVideoFps.Items.AddRange(new object[] {
            "auto",
            "5",
            "10",
            "12",
            "15",
            "23.976",
            "24",
            "25",
            "29.97",
            "30",
            "48",
            "50",
            "59.94",
            "60",
            "75",
            "85",
            "100",
            "120",
            "144",
            "200",
            "240"});
            this.cboVideoFps.Location = new System.Drawing.Point(127, 37);
            this.cboVideoFps.Name = "cboVideoFps";
            this.cboVideoFps.Size = new System.Drawing.Size(114, 21);
            this.cboVideoFps.TabIndex = 3;
            this.cboVideoFps.TextChanged += new System.EventHandler(this.cboVideoFps_TextChanged);
            // 
            // grpVideoPicture
            // 
            this.grpVideoPicture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVideoPicture.Controls.Add(this.cboVideoPixFmt);
            this.grpVideoPicture.Controls.Add(this.cboVideoBitDepth);
            this.grpVideoPicture.Controls.Add(this.lblVideoPixFmt);
            this.grpVideoPicture.Controls.Add(this.lblVideoBitDepth);
            this.grpVideoPicture.Controls.Add(this.lblVideoFps);
            this.grpVideoPicture.Controls.Add(this.cboVideoFps);
            this.grpVideoPicture.Controls.Add(this.cboVideoRes);
            this.grpVideoPicture.Controls.Add(this.lblVideoRes);
            this.grpVideoPicture.Location = new System.Drawing.Point(739, 6);
            this.grpVideoPicture.Name = "grpVideoPicture";
            this.grpVideoPicture.Size = new System.Drawing.Size(247, 124);
            this.grpVideoPicture.TabIndex = 9;
            this.grpVideoPicture.TabStop = false;
            this.grpVideoPicture.Text = "&Picture";
            // 
            // cboVideoRes
            // 
            this.cboVideoRes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoRes.FormattingEnabled = true;
            this.cboVideoRes.Items.AddRange(new object[] {
            "auto",
            "640x480",
            "640x360",
            "720x404",
            "800x600",
            "854x480",
            "1024x768",
            "1024x576",
            "1280x720",
            "1920x1080",
            "2560x1440",
            "3840x2160",
            "7680x4320"});
            this.cboVideoRes.Location = new System.Drawing.Point(6, 37);
            this.cboVideoRes.Name = "cboVideoRes";
            this.cboVideoRes.Size = new System.Drawing.Size(115, 21);
            this.cboVideoRes.TabIndex = 1;
            this.cboVideoRes.TextChanged += new System.EventHandler(this.cboVideoRes_TextChanged);
            // 
            // lblVideoRes
            // 
            this.lblVideoRes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoRes.Location = new System.Drawing.Point(6, 16);
            this.lblVideoRes.Name = "lblVideoRes";
            this.lblVideoRes.Size = new System.Drawing.Size(115, 18);
            this.lblVideoRes.TabIndex = 0;
            this.lblVideoRes.Text = "Re&solution:";
            this.lblVideoRes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboVideoDeInterField
            // 
            this.cboVideoDeInterField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoDeInterField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoDeInterField.FormattingEnabled = true;
            this.cboVideoDeInterField.Items.AddRange(new object[] {
            "Top Field First",
            "Bottom Field First"});
            this.cboVideoDeInterField.Location = new System.Drawing.Point(6, 87);
            this.cboVideoDeInterField.Name = "cboVideoDeInterField";
            this.cboVideoDeInterField.Size = new System.Drawing.Size(235, 21);
            this.cboVideoDeInterField.TabIndex = 3;
            this.cboVideoDeInterField.SelectedIndexChanged += new System.EventHandler(this.cboVideoDeInterField_SelectedIndexChanged);
            // 
            // lblVideoDeInterField
            // 
            this.lblVideoDeInterField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoDeInterField.Location = new System.Drawing.Point(6, 66);
            this.lblVideoDeInterField.Name = "lblVideoDeInterField";
            this.lblVideoDeInterField.Size = new System.Drawing.Size(235, 18);
            this.lblVideoDeInterField.TabIndex = 2;
            this.lblVideoDeInterField.Text = "Fiel&ds:";
            this.lblVideoDeInterField.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboVideoDeInterMode
            // 
            this.cboVideoDeInterMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoDeInterMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoDeInterMode.FormattingEnabled = true;
            this.cboVideoDeInterMode.Items.AddRange(new object[] {
            "Deinterlace only frame",
            "Deinterlace each field",
            "Skips spatial interlacing frame check",
            "Skips spatial interlacing field check"});
            this.cboVideoDeInterMode.Location = new System.Drawing.Point(6, 42);
            this.cboVideoDeInterMode.Name = "cboVideoDeInterMode";
            this.cboVideoDeInterMode.Size = new System.Drawing.Size(235, 21);
            this.cboVideoDeInterMode.TabIndex = 1;
            this.cboVideoDeInterMode.SelectedIndexChanged += new System.EventHandler(this.cboVideoDeInterMode_SelectedIndexChanged);
            // 
            // lblVideoDeInterMode
            // 
            this.lblVideoDeInterMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoDeInterMode.Location = new System.Drawing.Point(6, 21);
            this.lblVideoDeInterMode.Name = "lblVideoDeInterMode";
            this.lblVideoDeInterMode.Size = new System.Drawing.Size(235, 18);
            this.lblVideoDeInterMode.TabIndex = 0;
            this.lblVideoDeInterMode.Text = "M&ode:";
            this.lblVideoDeInterMode.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblVideoLang
            // 
            this.lblVideoLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoLang.Location = new System.Drawing.Point(6, 216);
            this.lblVideoLang.Name = "lblVideoLang";
            this.lblVideoLang.Size = new System.Drawing.Size(250, 18);
            this.lblVideoLang.TabIndex = 6;
            this.lblVideoLang.Text = "&Language:";
            this.lblVideoLang.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // chkVideoDeInterlace
            // 
            this.chkVideoDeInterlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkVideoDeInterlace.Location = new System.Drawing.Point(748, 136);
            this.chkVideoDeInterlace.Name = "chkVideoDeInterlace";
            this.chkVideoDeInterlace.Size = new System.Drawing.Size(90, 16);
            this.chkVideoDeInterlace.TabIndex = 10;
            this.chkVideoDeInterlace.Text = "&Deinterlaced";
            this.chkVideoDeInterlace.UseVisualStyleBackColor = true;
            this.chkVideoDeInterlace.CheckedChanged += new System.EventHandler(this.chkVideoDeInterlace_CheckedChanged);
            // 
            // grpVideoInterlace
            // 
            this.grpVideoInterlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVideoInterlace.Controls.Add(this.cboVideoDeInterField);
            this.grpVideoInterlace.Controls.Add(this.lblVideoDeInterField);
            this.grpVideoInterlace.Controls.Add(this.cboVideoDeInterMode);
            this.grpVideoInterlace.Controls.Add(this.lblVideoDeInterMode);
            this.grpVideoInterlace.Location = new System.Drawing.Point(739, 136);
            this.grpVideoInterlace.Name = "grpVideoInterlace";
            this.grpVideoInterlace.Size = new System.Drawing.Size(247, 122);
            this.grpVideoInterlace.TabIndex = 11;
            this.grpVideoInterlace.TabStop = false;
            // 
            // tabConfigVideo
            // 
            this.tabConfigVideo.Controls.Add(this.lblVideoLang);
            this.tabConfigVideo.Controls.Add(this.chkVideoDeInterlace);
            this.tabConfigVideo.Controls.Add(this.grpVideoInterlace);
            this.tabConfigVideo.Controls.Add(this.grpVideoPicture);
            this.tabConfigVideo.Controls.Add(this.grpVideoCodec);
            this.tabConfigVideo.Controls.Add(this.cboVideoLang);
            this.tabConfigVideo.Controls.Add(this.btnVideoMoveDown);
            this.tabConfigVideo.Controls.Add(this.btnVideoMoveUp);
            this.tabConfigVideo.Controls.Add(this.Seperator3);
            this.tabConfigVideo.Controls.Add(this.btnVideoDel);
            this.tabConfigVideo.Controls.Add(this.btnVideoAdd);
            this.tabConfigVideo.Controls.Add(this.lstVideo);
            this.tabConfigVideo.Location = new System.Drawing.Point(4, 24);
            this.tabConfigVideo.Name = "tabConfigVideo";
            this.tabConfigVideo.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigVideo.Size = new System.Drawing.Size(992, 264);
            this.tabConfigVideo.TabIndex = 0;
            this.tabConfigVideo.Text = "Video";
            this.tabConfigVideo.UseVisualStyleBackColor = true;
            // 
            // grpVideoCodec
            // 
            this.grpVideoCodec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVideoCodec.Controls.Add(this.btnVideoEnc);
            this.grpVideoCodec.Controls.Add(this.lblVideoAdv);
            this.grpVideoCodec.Controls.Add(this.btnVideoDec);
            this.grpVideoCodec.Controls.Add(this.nudVideoMultiPass);
            this.grpVideoCodec.Controls.Add(this.nudVideoRateFactor);
            this.grpVideoCodec.Controls.Add(this.lblVideoMultiPass);
            this.grpVideoCodec.Controls.Add(this.lblVideoRateFactor);
            this.grpVideoCodec.Controls.Add(this.cboVideoRateControl);
            this.grpVideoCodec.Controls.Add(this.lblVideoRateControl);
            this.grpVideoCodec.Controls.Add(this.cboVideoTune);
            this.grpVideoCodec.Controls.Add(this.cboVideoPreset);
            this.grpVideoCodec.Controls.Add(this.lblVideoTune);
            this.grpVideoCodec.Controls.Add(this.lblVideoPreset);
            this.grpVideoCodec.Controls.Add(this.cboVideoEncoder);
            this.grpVideoCodec.Controls.Add(this.lblVideoEncoder);
            this.grpVideoCodec.Location = new System.Drawing.Point(486, 6);
            this.grpVideoCodec.Name = "grpVideoCodec";
            this.grpVideoCodec.Size = new System.Drawing.Size(247, 252);
            this.grpVideoCodec.TabIndex = 8;
            this.grpVideoCodec.TabStop = false;
            this.grpVideoCodec.Text = "&Codec";
            // 
            // btnVideoEnc
            // 
            this.btnVideoEnc.Location = new System.Drawing.Point(127, 216);
            this.btnVideoEnc.Name = "btnVideoEnc";
            this.btnVideoEnc.Size = new System.Drawing.Size(114, 23);
            this.btnVideoEnc.TabIndex = 14;
            this.btnVideoEnc.Text = "E&ncoder CLI";
            this.btnVideoEnc.UseVisualStyleBackColor = true;
            this.btnVideoEnc.Click += new System.EventHandler(this.btnVideoEnc_Click);
            // 
            // lblVideoAdv
            // 
            this.lblVideoAdv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoAdv.Location = new System.Drawing.Point(6, 195);
            this.lblVideoAdv.Name = "lblVideoAdv";
            this.lblVideoAdv.Size = new System.Drawing.Size(235, 18);
            this.lblVideoAdv.TabIndex = 12;
            this.lblVideoAdv.Text = "Advance &Option for Decoder and Encoder:";
            this.lblVideoAdv.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnVideoDec
            // 
            this.btnVideoDec.Location = new System.Drawing.Point(6, 216);
            this.btnVideoDec.Name = "btnVideoDec";
            this.btnVideoDec.Size = new System.Drawing.Size(115, 23);
            this.btnVideoDec.TabIndex = 13;
            this.btnVideoDec.Text = "&Decoder CLI";
            this.btnVideoDec.UseVisualStyleBackColor = true;
            this.btnVideoDec.Click += new System.EventHandler(this.btnVideoDec_Click);
            // 
            // nudVideoMultiPass
            // 
            this.nudVideoMultiPass.Location = new System.Drawing.Point(127, 172);
            this.nudVideoMultiPass.Name = "nudVideoMultiPass";
            this.nudVideoMultiPass.Size = new System.Drawing.Size(114, 20);
            this.nudVideoMultiPass.TabIndex = 11;
            this.nudVideoMultiPass.Leave += new System.EventHandler(this.nudVideoMultiPass_Leave);
            // 
            // nudVideoRateFactor
            // 
            this.nudVideoRateFactor.Location = new System.Drawing.Point(6, 172);
            this.nudVideoRateFactor.Name = "nudVideoRateFactor";
            this.nudVideoRateFactor.Size = new System.Drawing.Size(115, 20);
            this.nudVideoRateFactor.TabIndex = 9;
            this.nudVideoRateFactor.Leave += new System.EventHandler(this.nudVideoRateFactor_Leave);
            // 
            // lblVideoMultiPass
            // 
            this.lblVideoMultiPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoMultiPass.Location = new System.Drawing.Point(127, 151);
            this.lblVideoMultiPass.Name = "lblVideoMultiPass";
            this.lblVideoMultiPass.Size = new System.Drawing.Size(114, 18);
            this.lblVideoMultiPass.TabIndex = 10;
            this.lblVideoMultiPass.Text = "&Multi Pass:";
            this.lblVideoMultiPass.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblVideoRateFactor
            // 
            this.lblVideoRateFactor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoRateFactor.Location = new System.Drawing.Point(6, 151);
            this.lblVideoRateFactor.Name = "lblVideoRateFactor";
            this.lblVideoRateFactor.Size = new System.Drawing.Size(115, 18);
            this.lblVideoRateFactor.TabIndex = 8;
            this.lblVideoRateFactor.Text = "Rate &Factor:";
            this.lblVideoRateFactor.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboVideoRateControl
            // 
            this.cboVideoRateControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoRateControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoRateControl.FormattingEnabled = true;
            this.cboVideoRateControl.Location = new System.Drawing.Point(6, 127);
            this.cboVideoRateControl.Name = "cboVideoRateControl";
            this.cboVideoRateControl.Size = new System.Drawing.Size(235, 21);
            this.cboVideoRateControl.TabIndex = 7;
            this.cboVideoRateControl.SelectedIndexChanged += new System.EventHandler(this.cboVideoRateControl_SelectedIndexChanged);
            // 
            // lblVideoRateControl
            // 
            this.lblVideoRateControl.Location = new System.Drawing.Point(6, 106);
            this.lblVideoRateControl.Name = "lblVideoRateControl";
            this.lblVideoRateControl.Size = new System.Drawing.Size(235, 18);
            this.lblVideoRateControl.TabIndex = 6;
            this.lblVideoRateControl.Text = "&Rate control:";
            this.lblVideoRateControl.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboVideoTune
            // 
            this.cboVideoTune.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoTune.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoTune.FormattingEnabled = true;
            this.cboVideoTune.Location = new System.Drawing.Point(127, 82);
            this.cboVideoTune.Name = "cboVideoTune";
            this.cboVideoTune.Size = new System.Drawing.Size(114, 21);
            this.cboVideoTune.TabIndex = 5;
            this.cboVideoTune.SelectedIndexChanged += new System.EventHandler(this.cboVideoTune_SelectedIndexChanged);
            // 
            // cboVideoPreset
            // 
            this.cboVideoPreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoPreset.FormattingEnabled = true;
            this.cboVideoPreset.Location = new System.Drawing.Point(6, 82);
            this.cboVideoPreset.Name = "cboVideoPreset";
            this.cboVideoPreset.Size = new System.Drawing.Size(115, 21);
            this.cboVideoPreset.TabIndex = 3;
            this.cboVideoPreset.SelectedIndexChanged += new System.EventHandler(this.cboVideoPreset_SelectedIndexChanged);
            // 
            // lblVideoTune
            // 
            this.lblVideoTune.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoTune.Location = new System.Drawing.Point(127, 61);
            this.lblVideoTune.Name = "lblVideoTune";
            this.lblVideoTune.Size = new System.Drawing.Size(114, 18);
            this.lblVideoTune.TabIndex = 4;
            this.lblVideoTune.Text = "&Tune:";
            this.lblVideoTune.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblVideoPreset
            // 
            this.lblVideoPreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoPreset.Location = new System.Drawing.Point(6, 61);
            this.lblVideoPreset.Name = "lblVideoPreset";
            this.lblVideoPreset.Size = new System.Drawing.Size(115, 18);
            this.lblVideoPreset.TabIndex = 2;
            this.lblVideoPreset.Text = "Pre&set:";
            this.lblVideoPreset.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboVideoEncoder
            // 
            this.cboVideoEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoEncoder.FormattingEnabled = true;
            this.cboVideoEncoder.Location = new System.Drawing.Point(6, 37);
            this.cboVideoEncoder.Name = "cboVideoEncoder";
            this.cboVideoEncoder.Size = new System.Drawing.Size(235, 21);
            this.cboVideoEncoder.TabIndex = 1;
            this.cboVideoEncoder.SelectedIndexChanged += new System.EventHandler(this.cboVideoEncoder_SelectedIndexChanged);
            // 
            // lblVideoEncoder
            // 
            this.lblVideoEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoEncoder.Location = new System.Drawing.Point(6, 16);
            this.lblVideoEncoder.Name = "lblVideoEncoder";
            this.lblVideoEncoder.Size = new System.Drawing.Size(235, 18);
            this.lblVideoEncoder.TabIndex = 0;
            this.lblVideoEncoder.Text = "&Encoder:";
            this.lblVideoEncoder.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cboVideoLang
            // 
            this.cboVideoLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoLang.FormattingEnabled = true;
            this.cboVideoLang.Location = new System.Drawing.Point(6, 237);
            this.cboVideoLang.Name = "cboVideoLang";
            this.cboVideoLang.Size = new System.Drawing.Size(474, 21);
            this.cboVideoLang.TabIndex = 7;
            this.cboVideoLang.SelectedIndexChanged += new System.EventHandler(this.cboVideoLang_SelectedIndexChanged);
            // 
            // btnVideoMoveDown
            // 
            this.btnVideoMoveDown.Location = new System.Drawing.Point(104, 6);
            this.btnVideoMoveDown.Name = "btnVideoMoveDown";
            this.btnVideoMoveDown.Size = new System.Drawing.Size(24, 24);
            this.btnVideoMoveDown.TabIndex = 4;
            this.btnVideoMoveDown.UseVisualStyleBackColor = true;
            this.btnVideoMoveDown.Click += new System.EventHandler(this.btnVideoMoveDown_Click);
            // 
            // btnVideoMoveUp
            // 
            this.btnVideoMoveUp.Location = new System.Drawing.Point(74, 6);
            this.btnVideoMoveUp.Name = "btnVideoMoveUp";
            this.btnVideoMoveUp.Size = new System.Drawing.Size(24, 24);
            this.btnVideoMoveUp.TabIndex = 3;
            this.btnVideoMoveUp.UseVisualStyleBackColor = true;
            this.btnVideoMoveUp.Click += new System.EventHandler(this.btnVideoMoveUp_Click);
            // 
            // Seperator3
            // 
            this.Seperator3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Seperator3.Location = new System.Drawing.Point(66, 6);
            this.Seperator3.Name = "Seperator3";
            this.Seperator3.Size = new System.Drawing.Size(2, 24);
            this.Seperator3.TabIndex = 2;
            // 
            // btnVideoDel
            // 
            this.btnVideoDel.Location = new System.Drawing.Point(36, 6);
            this.btnVideoDel.Name = "btnVideoDel";
            this.btnVideoDel.Size = new System.Drawing.Size(24, 24);
            this.btnVideoDel.TabIndex = 1;
            this.btnVideoDel.UseVisualStyleBackColor = true;
            this.btnVideoDel.Click += new System.EventHandler(this.btnVideoDel_Click);
            // 
            // tabConfig
            // 
            this.tabConfig.AllowDrop = true;
            this.tabConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabConfig.Controls.Add(this.tabConfigMediaInfo);
            this.tabConfig.Controls.Add(this.tabConfigVideo);
            this.tabConfig.Controls.Add(this.tabConfigAudio);
            this.tabConfig.Controls.Add(this.tabConfigSubtitle);
            this.tabConfig.Controls.Add(this.tabConfigAttachment);
            this.tabConfig.Controls.Add(this.tabConfigAdvance);
            this.tabConfig.Controls.Add(this.tabConfigLog);
            this.tabConfig.Location = new System.Drawing.Point(12, 236);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Drawing.Point(16, 4);
            this.tabConfig.SelectedIndex = 0;
            this.tabConfig.Size = new System.Drawing.Size(1000, 292);
            this.tabConfig.TabIndex = 12;
            // 
            // tabConfigMediaInfo
            // 
            this.tabConfigMediaInfo.Controls.Add(this.txtMediaInfo);
            this.tabConfigMediaInfo.Location = new System.Drawing.Point(4, 24);
            this.tabConfigMediaInfo.Name = "tabConfigMediaInfo";
            this.tabConfigMediaInfo.Size = new System.Drawing.Size(992, 264);
            this.tabConfigMediaInfo.TabIndex = 6;
            this.tabConfigMediaInfo.Text = "Media Info";
            this.tabConfigMediaInfo.UseVisualStyleBackColor = true;
            // 
            // txtMediaInfo
            // 
            this.txtMediaInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMediaInfo.Location = new System.Drawing.Point(0, 0);
            this.txtMediaInfo.Multiline = true;
            this.txtMediaInfo.Name = "txtMediaInfo";
            this.txtMediaInfo.ReadOnly = true;
            this.txtMediaInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMediaInfo.Size = new System.Drawing.Size(992, 264);
            this.txtMediaInfo.TabIndex = 0;
            this.txtMediaInfo.WordWrap = false;
            // 
            // lstFile
            // 
            this.lstFile.AllowDrop = true;
            this.lstFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFile.CheckBoxes = true;
            this.lstFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFileName,
            this.colFileType,
            this.colFileDuration,
            this.colFileSize,
            this.colFileStatus,
            this.colFileProgress});
            this.lstFile.FullRowSelect = true;
            this.lstFile.GridLines = true;
            this.lstFile.HideSelection = false;
            this.lstFile.Location = new System.Drawing.Point(12, 114);
            this.lstFile.Name = "lstFile";
            this.lstFile.Size = new System.Drawing.Size(1000, 116);
            this.lstFile.TabIndex = 11;
            this.lstFile.UseCompatibleStateImageBehavior = false;
            this.lstFile.View = System.Windows.Forms.View.Details;
            this.lstFile.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstFile_ItemChecked);
            this.lstFile.SelectedIndexChanged += new System.EventHandler(this.lstFile_SelectedIndexChanged);
            this.lstFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstFile_DragDrop);
            this.lstFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstFile_DragEnter);
            // 
            // colFileName
            // 
            this.colFileName.Text = "Name";
            this.colFileName.Width = 250;
            // 
            // colFileType
            // 
            this.colFileType.Text = "Type";
            this.colFileType.Width = 80;
            // 
            // colFileDuration
            // 
            this.colFileDuration.Text = "Duration";
            this.colFileDuration.Width = 80;
            // 
            // colFileSize
            // 
            this.colFileSize.Text = "Size";
            this.colFileSize.Width = 80;
            // 
            // colFileStatus
            // 
            this.colFileStatus.Text = "Status";
            this.colFileStatus.Width = 130;
            // 
            // colFileProgress
            // 
            this.colFileProgress.Text = "Progress";
            this.colFileProgress.Width = 354;
            // 
            // btnDonate
            // 
            this.btnDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDonate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDonate.Location = new System.Drawing.Point(820, 76);
            this.btnDonate.Name = "btnDonate";
            this.btnDonate.Size = new System.Drawing.Size(108, 32);
            this.btnDonate.TabIndex = 7;
            this.btnDonate.Text = "&Donate";
            this.btnDonate.UseVisualStyleBackColor = true;
            this.btnDonate.Click += new System.EventHandler(this.btnDonate_Click);
            // 
            // Seperator2
            // 
            this.Seperator2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Seperator2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Seperator2.Location = new System.Drawing.Point(934, 76);
            this.Seperator2.Name = "Seperator2";
            this.Seperator2.Size = new System.Drawing.Size(2, 32);
            this.Seperator2.TabIndex = 8;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(942, 76);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(32, 32);
            this.btnStart.TabIndex = 9;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnStart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnStart_MouseDown);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(980, 76);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(32, 32);
            this.btnStop.TabIndex = 10;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnFileDown
            // 
            this.btnFileDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnFileDown.Location = new System.Drawing.Point(515, 76);
            this.btnFileDown.Name = "btnFileDown";
            this.btnFileDown.Size = new System.Drawing.Size(32, 32);
            this.btnFileDown.TabIndex = 6;
            this.btnFileDown.UseVisualStyleBackColor = true;
            this.btnFileDown.Click += new System.EventHandler(this.btnFileDown_Click);
            // 
            // btnFileUp
            // 
            this.btnFileUp.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnFileUp.Location = new System.Drawing.Point(477, 76);
            this.btnFileUp.Name = "btnFileUp";
            this.btnFileUp.Size = new System.Drawing.Size(32, 32);
            this.btnFileUp.TabIndex = 5;
            this.btnFileUp.UseVisualStyleBackColor = true;
            this.btnFileUp.Click += new System.EventHandler(this.btnFileUp_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOptions.Location = new System.Drawing.Point(96, 76);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(51, 32);
            this.btnOptions.TabIndex = 3;
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // Seperator1
            // 
            this.Seperator1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Seperator1.Location = new System.Drawing.Point(88, 76);
            this.Seperator1.Name = "Seperator1";
            this.Seperator1.Size = new System.Drawing.Size(2, 32);
            this.Seperator1.TabIndex = 2;
            // 
            // btnFileDelete
            // 
            this.btnFileDelete.Location = new System.Drawing.Point(50, 76);
            this.btnFileDelete.Name = "btnFileDelete";
            this.btnFileDelete.Size = new System.Drawing.Size(32, 32);
            this.btnFileDelete.TabIndex = 1;
            this.btnFileDelete.UseVisualStyleBackColor = true;
            this.btnFileDelete.Click += new System.EventHandler(this.btnFileDelete_Click);
            // 
            // btnFileAdd
            // 
            this.btnFileAdd.Location = new System.Drawing.Point(12, 76);
            this.btnFileAdd.Name = "btnFileAdd";
            this.btnFileAdd.Size = new System.Drawing.Size(32, 32);
            this.btnFileAdd.TabIndex = 0;
            this.btnFileAdd.UseVisualStyleBackColor = true;
            this.btnFileAdd.Click += new System.EventHandler(this.btnFileAdd_Click);
            this.btnFileAdd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnFileAdd_MouseUp);
            // 
            // cmsFileAdd
            // 
            this.cmsFileAdd.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmsFileAdd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImportFiles,
            this.tsmiImportFolder,
            this.tsmiImportImgSeq,
            this.toolStripSeparator1,
            this.tsmiImportYouTube});
            this.cmsFileAdd.Name = "cmsFileAdd";
            this.cmsFileAdd.Size = new System.Drawing.Size(190, 98);
            // 
            // tsmiImportFiles
            // 
            this.tsmiImportFiles.Name = "tsmiImportFiles";
            this.tsmiImportFiles.Size = new System.Drawing.Size(189, 22);
            this.tsmiImportFiles.Text = "Import &Files";
            this.tsmiImportFiles.Click += new System.EventHandler(this.tsmiImportFiles_Click);
            // 
            // tsmiImportFolder
            // 
            this.tsmiImportFolder.Enabled = false;
            this.tsmiImportFolder.Name = "tsmiImportFolder";
            this.tsmiImportFolder.Size = new System.Drawing.Size(189, 22);
            this.tsmiImportFolder.Text = "Import Fol&der";
            this.tsmiImportFolder.Click += new System.EventHandler(this.tsmiImportFolder_Click);
            // 
            // tsmiImportImgSeq
            // 
            this.tsmiImportImgSeq.Name = "tsmiImportImgSeq";
            this.tsmiImportImgSeq.Size = new System.Drawing.Size(189, 22);
            this.tsmiImportImgSeq.Text = "Import &Image Sequence";
            this.tsmiImportImgSeq.Click += new System.EventHandler(this.tsmiImportImgSeq_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // tsmiImportYouTube
            // 
            this.tsmiImportYouTube.Enabled = false;
            this.tsmiImportYouTube.Name = "tsmiImportYouTube";
            this.tsmiImportYouTube.Size = new System.Drawing.Size(189, 22);
            this.tsmiImportYouTube.Text = "Import &YouTube";
            this.tsmiImportYouTube.Click += new System.EventHandler(this.tsmiImportYouTube_Click);
            // 
            // cmsProfiles
            // 
            this.cmsProfiles.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmsProfiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiProfilesSave,
            this.tsmiProfilesRename,
            this.tsmiProfilesDelete});
            this.cmsProfiles.Name = "cmsProfiles";
            this.cmsProfiles.Size = new System.Drawing.Size(114, 70);
            // 
            // tsmiProfilesSave
            // 
            this.tsmiProfilesSave.Name = "tsmiProfilesSave";
            this.tsmiProfilesSave.Size = new System.Drawing.Size(113, 22);
            this.tsmiProfilesSave.Text = "&Save As";
            this.tsmiProfilesSave.Click += new System.EventHandler(this.tsmiProfilesSave_Click);
            // 
            // tsmiProfilesRename
            // 
            this.tsmiProfilesRename.Name = "tsmiProfilesRename";
            this.tsmiProfilesRename.Size = new System.Drawing.Size(113, 22);
            this.tsmiProfilesRename.Text = "&Rename";
            this.tsmiProfilesRename.Click += new System.EventHandler(this.tsmiProfilesRename_Click);
            // 
            // tsmiProfilesDelete
            // 
            this.tsmiProfilesDelete.Name = "tsmiProfilesDelete";
            this.tsmiProfilesDelete.Size = new System.Drawing.Size(113, 22);
            this.tsmiProfilesDelete.Text = "&Delete";
            this.tsmiProfilesDelete.Click += new System.EventHandler(this.tsmiProfilesDelete_Click);
            // 
            // cmsPower
            // 
            this.cmsPower.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmsPower.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPowerOff});
            this.cmsPower.Name = "cmsPower";
            this.cmsPower.Size = new System.Drawing.Size(198, 26);
            // 
            // tsmiPowerOff
            // 
            this.tsmiPowerOff.Name = "tsmiPowerOff";
            this.tsmiPowerOff.Size = new System.Drawing.Size(197, 22);
            this.tsmiPowerOff.Text = "Shutdown when complete";
            this.tsmiPowerOff.Click += new System.EventHandler(this.tsmiPowerOff_Click);
            // 
            // cmsFileAddSubs
            // 
            this.cmsFileAddSubs.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmsFileAddSubs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFileAddSubs,
            this.tsmiFileAddSubsEmbed});
            this.cmsFileAddSubs.Name = "cmsFileAddSubs";
            this.cmsFileAddSubs.Size = new System.Drawing.Size(187, 48);
            // 
            // tsmiFileAddSubs
            // 
            this.tsmiFileAddSubs.Name = "tsmiFileAddSubs";
            this.tsmiFileAddSubs.Size = new System.Drawing.Size(186, 22);
            this.tsmiFileAddSubs.Text = "Add &Subtitle";
            this.tsmiFileAddSubs.Click += new System.EventHandler(this.tsmiFileAddSubs_Click);
            // 
            // tsmiFileAddSubsEmbed
            // 
            this.tsmiFileAddSubsEmbed.Name = "tsmiFileAddSubsEmbed";
            this.tsmiFileAddSubsEmbed.Size = new System.Drawing.Size(186, 22);
            this.tsmiFileAddSubsEmbed.Text = "Add Subtitle from &Video";
            this.tsmiFileAddSubsEmbed.Click += new System.EventHandler(this.tsmiFileAddSubsEmbed_Click);
            // 
            // cmsFileAddAttach
            // 
            this.cmsFileAddAttach.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmsFileAddAttach.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFileAddAttach,
            this.tsmiFileAddAttachEmbed});
            this.cmsFileAddAttach.Name = "cmsFileAddAttach";
            this.cmsFileAddAttach.Size = new System.Drawing.Size(178, 48);
            // 
            // tsmiFileAddAttach
            // 
            this.tsmiFileAddAttach.Name = "tsmiFileAddAttach";
            this.tsmiFileAddAttach.Size = new System.Drawing.Size(177, 22);
            this.tsmiFileAddAttach.Text = "Add &Fonts";
            this.tsmiFileAddAttach.Click += new System.EventHandler(this.tsmiFileAddAttach_Click);
            // 
            // tsmiFileAddAttachEmbed
            // 
            this.tsmiFileAddAttachEmbed.Name = "tsmiFileAddAttachEmbed";
            this.tsmiFileAddAttachEmbed.Size = new System.Drawing.Size(177, 22);
            this.tsmiFileAddAttachEmbed.Text = "Add Fonts from &Video";
            this.tsmiFileAddAttachEmbed.Click += new System.EventHandler(this.tsmiFileAddAttachEmbed_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnAbout.Location = new System.Drawing.Point(153, 76);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(51, 32);
            this.btnAbout.TabIndex = 4;
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1024, 600);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnOutputBrowse);
            this.Controls.Add(this.btnProfileSaveLoad);
            this.Controls.Add(this.cboProfile);
            this.Controls.Add(this.cboFormat);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.lblOutputPath);
            this.Controls.Add(this.lblFormatProfile);
            this.Controls.Add(this.PbxBanner);
            this.Controls.Add(this.tabConfig);
            this.Controls.Add(this.lstFile);
            this.Controls.Add(this.btnDonate);
            this.Controls.Add(this.Seperator2);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnFileDown);
            this.Controls.Add(this.btnFileUp);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.Seperator1);
            this.Controls.Add(this.btnFileDelete);
            this.Controls.Add(this.btnFileAdd);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IFME 2020";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.tabConfigSubtitle.ResumeLayout(false);
            this.grpAudioCodec.ResumeLayout(false);
            this.tabConfigAudio.ResumeLayout(false);
            this.tabConfigAttachment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PbxBanner)).EndInit();
            this.tabConfigLog.ResumeLayout(false);
            this.tabConfigAdvance.ResumeLayout(false);
            this.tabConfigAdvance.PerformLayout();
            this.grpAdvTrim.ResumeLayout(false);
            this.grpAdvTrim.PerformLayout();
            this.grpVideoPicture.ResumeLayout(false);
            this.grpVideoInterlace.ResumeLayout(false);
            this.tabConfigVideo.ResumeLayout(false);
            this.grpVideoCodec.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoMultiPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoRateFactor)).EndInit();
            this.tabConfig.ResumeLayout(false);
            this.tabConfigMediaInfo.ResumeLayout(false);
            this.tabConfigMediaInfo.PerformLayout();
            this.cmsFileAdd.ResumeLayout(false);
            this.cmsProfiles.ResumeLayout(false);
            this.cmsPower.ResumeLayout(false);
            this.cmsFileAddSubs.ResumeLayout(false);
            this.cmsFileAddAttach.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboSubLang;
        private System.Windows.Forms.Button btnSubAdd;
        private System.Windows.Forms.Label lblSubLang;
        private System.Windows.Forms.Button btnSubDel;
        private System.Windows.Forms.Button btnSubMoveDown;
        private System.Windows.Forms.ColumnHeader colSubLang;
        private System.Windows.Forms.ColumnHeader colSubFileName;
        private System.Windows.Forms.ColumnHeader colSubId;
        private System.Windows.Forms.Label Seperator6;
        private System.Windows.Forms.CheckBox chkSubHard;
        private System.Windows.Forms.ListView lstSub;
        private System.Windows.Forms.TabPage tabConfigSubtitle;
        private System.Windows.Forms.Button btnSubMoveUp;
        private System.Windows.Forms.ColumnHeader colAudioId;
        private System.Windows.Forms.ComboBox cboAudioLang;
        private System.Windows.Forms.Button btnAudioMoveDown;
        private System.Windows.Forms.Button btnAudioMoveUp;
        private System.Windows.Forms.Label Seperator4;
        private System.Windows.Forms.Button btnAudioDel;
        private System.Windows.Forms.Button btnAudioAdd;
        private System.Windows.Forms.Button btnOutputBrowse;
        private System.Windows.Forms.Label lblAudioLang;
        private System.Windows.Forms.GroupBox grpAudioCodec;
        private System.Windows.Forms.Button btnAudioEnc;
        private System.Windows.Forms.Button btnAudioDec;
        private System.Windows.Forms.Label lblAudioAdv;
        private System.Windows.Forms.ComboBox cboAudioMode;
        private System.Windows.Forms.Label lblAudioMode;
        private System.Windows.Forms.ComboBox cboAudioChannel;
        private System.Windows.Forms.Label lblAudioChannel;
        private System.Windows.Forms.ComboBox cboAudioSampleRate;
        private System.Windows.Forms.Label lblAudioSampleRate;
        private System.Windows.Forms.ComboBox cboAudioQuality;
        private System.Windows.Forms.Label lblAudioQuality;
        private System.Windows.Forms.ComboBox cboAudioEncoder;
        private System.Windows.Forms.Label lblAudioEncoder;
        private System.Windows.Forms.TabPage tabConfigAudio;
        private System.Windows.Forms.ListView lstAudio;
        private System.Windows.Forms.ListView lstVideo;
        private System.Windows.Forms.ColumnHeader colVideoId;
        private System.Windows.Forms.TabPage tabConfigAttachment;
        private System.Windows.Forms.ComboBox cboAttachMime;
        private System.Windows.Forms.Label lblAttachMime;
        private System.Windows.Forms.ListView lstAttach;
        private System.Windows.Forms.ColumnHeader colAttachId;
        private System.Windows.Forms.ColumnHeader colAttachFileName;
        private System.Windows.Forms.ColumnHeader colAttachMime;
        private System.Windows.Forms.Button btnAttachDel;
        private System.Windows.Forms.Button btnAttachAdd;
        private System.Windows.Forms.Button btnProfileSaveLoad;
        private System.Windows.Forms.ComboBox cboFormat;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.Label lblFormatProfile;
        private System.Windows.Forms.PictureBox PbxBanner;
        private System.Windows.Forms.TabPage tabConfigLog;
        private System.Windows.Forms.CheckBox chkAdvTrim;
        private System.Windows.Forms.TabPage tabConfigAdvance;
        private System.Windows.Forms.GroupBox grpAdvTrim;
        private System.Windows.Forms.Label lblAdvTimeEqual;
        private System.Windows.Forms.Label lblAdvTimeUntil;
        private System.Windows.Forms.Label lblAdvTimeEnd;
        private System.Windows.Forms.Label lblAdvTimeDuration;
        private System.Windows.Forms.Label lblAdvTimeStart;
        private System.Windows.Forms.Button btnVideoAdd;
        private System.Windows.Forms.ComboBox cboVideoPixFmt;
        private System.Windows.Forms.ComboBox cboVideoBitDepth;
        private System.Windows.Forms.Label lblVideoPixFmt;
        private System.Windows.Forms.Label lblVideoBitDepth;
        private System.Windows.Forms.Label lblVideoFps;
        private System.Windows.Forms.ComboBox cboVideoFps;
        private System.Windows.Forms.GroupBox grpVideoPicture;
        private System.Windows.Forms.ComboBox cboVideoRes;
        private System.Windows.Forms.Label lblVideoRes;
        private System.Windows.Forms.ComboBox cboVideoDeInterField;
        private System.Windows.Forms.Label lblVideoDeInterField;
        private System.Windows.Forms.ComboBox cboVideoDeInterMode;
        private System.Windows.Forms.Label lblVideoDeInterMode;
        private System.Windows.Forms.Label lblVideoLang;
        private System.Windows.Forms.CheckBox chkVideoDeInterlace;
        private System.Windows.Forms.GroupBox grpVideoInterlace;
        private System.Windows.Forms.TabPage tabConfigVideo;
        private System.Windows.Forms.GroupBox grpVideoCodec;
        private System.Windows.Forms.Button btnVideoEnc;
        private System.Windows.Forms.Label lblVideoAdv;
        private System.Windows.Forms.Button btnVideoDec;
        private System.Windows.Forms.NumericUpDown nudVideoMultiPass;
        private System.Windows.Forms.NumericUpDown nudVideoRateFactor;
        private System.Windows.Forms.Label lblVideoMultiPass;
        private System.Windows.Forms.Label lblVideoRateFactor;
        private System.Windows.Forms.ComboBox cboVideoRateControl;
        private System.Windows.Forms.Label lblVideoRateControl;
        private System.Windows.Forms.ComboBox cboVideoTune;
        private System.Windows.Forms.ComboBox cboVideoPreset;
        private System.Windows.Forms.Label lblVideoTune;
        private System.Windows.Forms.Label lblVideoPreset;
        private System.Windows.Forms.Label lblVideoEncoder;
        private System.Windows.Forms.ComboBox cboVideoLang;
        private System.Windows.Forms.Button btnVideoMoveDown;
        private System.Windows.Forms.Button btnVideoMoveUp;
        private System.Windows.Forms.Label Seperator3;
        private System.Windows.Forms.Button btnVideoDel;
        private System.Windows.Forms.TabControl tabConfig;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colFileType;
        private System.Windows.Forms.ColumnHeader colFileDuration;
        private System.Windows.Forms.ColumnHeader colFileSize;
        private System.Windows.Forms.ColumnHeader colFileStatus;
        private System.Windows.Forms.Button btnDonate;
        private System.Windows.Forms.Label Seperator2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnFileDown;
        private System.Windows.Forms.Button btnFileUp;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Label Seperator1;
        private System.Windows.Forms.Button btnFileDelete;
        private System.Windows.Forms.Button btnFileAdd;
        private System.Windows.Forms.ColumnHeader colVideoLang;
        private System.Windows.Forms.ColumnHeader colVideoRes;
        private System.Windows.Forms.ColumnHeader colAudioLang;
        private System.Windows.Forms.ColumnHeader colAudioBitRate;
        private System.Windows.Forms.ContextMenuStrip cmsFileAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportFiles;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportYouTube;
        internal System.Windows.Forms.RichTextBox rtfConsole;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportImgSeq;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.ContextMenuStrip cmsProfiles;
        private System.Windows.Forms.ToolStripMenuItem tsmiProfilesSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiProfilesRename;
        private System.Windows.Forms.ToolStripMenuItem tsmiProfilesDelete;
        private System.Windows.Forms.ContextMenuStrip cmsPower;
        private System.Windows.Forms.ToolStripMenuItem tsmiPowerOff;
        private System.Windows.Forms.TabPage tabConfigMediaInfo;
        private System.Windows.Forms.TextBox txtMediaInfo;
        private System.Windows.Forms.ContextMenuStrip cmsFileAddSubs;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileAddSubs;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileAddSubsEmbed;
        private System.Windows.Forms.ContextMenuStrip cmsFileAddAttach;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileAddAttach;
        private System.Windows.Forms.ToolStripMenuItem tsmiFileAddAttachEmbed;
        private System.Windows.Forms.ColumnHeader colFileProgress;
        internal System.Windows.Forms.ListView lstFile;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.ComboBox cboVideoEncoder;
        private System.Windows.Forms.ColumnHeader colVideoFps;
        private System.Windows.Forms.ColumnHeader colVideoBitDepth;
        private System.Windows.Forms.ColumnHeader colVideoPixFmt;
        private System.Windows.Forms.ColumnHeader colAudioSampleRate;
        private System.Windows.Forms.ColumnHeader colAudioChannel;
        private System.Windows.Forms.TextBox txtTrimDuration;
        private System.Windows.Forms.TextBox txtTrimEnd;
        private System.Windows.Forms.TextBox txtTrimStart;
        private System.Windows.Forms.GroupBox grpAdvHdr;
    }
}