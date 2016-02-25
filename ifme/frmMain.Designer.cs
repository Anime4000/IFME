namespace ifme
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
            this.chkDonePowerOff = new System.Windows.Forms.CheckBox();
            this.lblSpacer1 = new System.Windows.Forms.Label();
            this.lblSpacer2 = new System.Windows.Forms.Label();
            this.lstQueue = new System.Windows.Forms.ListView();
            this.colQueueName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colQueueDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colQueueStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsQueue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiQueueNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQueueOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQueueSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQueueSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiQueueDel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQueueSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQueueSelectNone = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQueueSelectInvert = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAviSynth = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQueueAviSynthEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiQueueAviSynthConvertTo = new System.Windows.Forms.ToolStripMenuItem();
            this.tabProp = new System.Windows.Forms.TabControl();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.grpPropFormat = new System.Windows.Forms.GroupBox();
            this.rdoMKV = new System.Windows.Forms.RadioButton();
            this.rdoMP4 = new System.Windows.Forms.RadioButton();
            this.grpPropOutput = new System.Windows.Forms.GroupBox();
            this.txtOutputInfo = new System.Windows.Forms.TextBox();
            this.grpPropInput = new System.Windows.Forms.GroupBox();
            this.txtSourceInfo = new System.Windows.Forms.TextBox();
            this.tabVideo = new System.Windows.Forms.TabPage();
            this.chkVideoDeinterlace = new System.Windows.Forms.CheckBox();
            this.grpVideoDeInterlace = new System.Windows.Forms.GroupBox();
            this.cboVideoDiField = new System.Windows.Forms.ComboBox();
            this.lblVideoDiField = new System.Windows.Forms.Label();
            this.cboVideoDiMode = new System.Windows.Forms.ComboBox();
            this.lblVideoDiMode = new System.Windows.Forms.Label();
            this.grpVideoCodec = new System.Windows.Forms.GroupBox();
            this.btnVideoArgEdit = new System.Windows.Forms.Button();
            this.nudVideoMultipass = new System.Windows.Forms.NumericUpDown();
            this.lblVideoMultipass = new System.Windows.Forms.Label();
            this.lblVideoRateFactor = new System.Windows.Forms.Label();
            this.nudVideoRateFactor = new System.Windows.Forms.NumericUpDown();
            this.lblVideoRateControl = new System.Windows.Forms.Label();
            this.cboVideoTune = new System.Windows.Forms.ComboBox();
            this.lblVideoTune = new System.Windows.Forms.Label();
            this.cboVideoPreset = new System.Windows.Forms.ComboBox();
            this.lblVideoPreset = new System.Windows.Forms.Label();
            this.cboVideoEncodingType = new System.Windows.Forms.ComboBox();
            this.cboVideoEncoder = new System.Windows.Forms.ComboBox();
            this.lblVideoEncoder = new System.Windows.Forms.Label();
            this.grpVideoQuality = new System.Windows.Forms.GroupBox();
            this.cboVideoChroma = new System.Windows.Forms.ComboBox();
            this.lblVideoColourSpace = new System.Windows.Forms.Label();
            this.cboVideoBitDepth = new System.Windows.Forms.ComboBox();
            this.lblVideoBitDepth = new System.Windows.Forms.Label();
            this.cboVideoFrameRate = new System.Windows.Forms.ComboBox();
            this.lblVideoFrameRate = new System.Windows.Forms.Label();
            this.cboVideoResolution = new System.Windows.Forms.ComboBox();
            this.lblVideoResolution = new System.Windows.Forms.Label();
            this.tabAudio = new System.Windows.Forms.TabPage();
            this.btnAudioMoveDown = new System.Windows.Forms.Button();
            this.btnAudioMoveUp = new System.Windows.Forms.Button();
            this.lblSpacer4 = new System.Windows.Forms.Label();
            this.lstAudio = new System.Windows.Forms.ListView();
            this.colAudioName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAudioId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpAudioCodec = new System.Windows.Forms.GroupBox();
            this.lblAudioMode = new System.Windows.Forms.Label();
            this.cboAudioMode = new System.Windows.Forms.ComboBox();
            this.btnAudioEditArg = new System.Windows.Forms.Button();
            this.lblAudioEncoder = new System.Windows.Forms.Label();
            this.lblAudioChannel = new System.Windows.Forms.Label();
            this.cboAudioEncoder = new System.Windows.Forms.ComboBox();
            this.cboAudioChannel = new System.Windows.Forms.ComboBox();
            this.lblAudioQuality = new System.Windows.Forms.Label();
            this.cboAudioFreq = new System.Windows.Forms.ComboBox();
            this.cboAudioQuality = new System.Windows.Forms.ComboBox();
            this.lblAudioFreq = new System.Windows.Forms.Label();
            this.btnAudioRemove = new System.Windows.Forms.Button();
            this.btnAudioAdd = new System.Windows.Forms.Button();
            this.tabSubtitle = new System.Windows.Forms.TabPage();
            this.cboSubLang = new System.Windows.Forms.ComboBox();
            this.lstSub = new System.Windows.Forms.ListView();
            this.colSubId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSubName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSubLang = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblSpacer5 = new System.Windows.Forms.Label();
            this.btnSubMoveDown = new System.Windows.Forms.Button();
            this.btnSubMoveUp = new System.Windows.Forms.Button();
            this.btnSubRemove = new System.Windows.Forms.Button();
            this.btnSubAdd = new System.Windows.Forms.Button();
            this.tabAttachment = new System.Windows.Forms.TabPage();
            this.lstAttach = new System.Windows.Forms.ListView();
            this.colAttachName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAttachMime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAttachDel = new System.Windows.Forms.Button();
            this.btnAttachAdd = new System.Windows.Forms.Button();
            this.lblProfile = new System.Windows.Forms.Label();
            this.cboProfile = new System.Windows.Forms.ComboBox();
            this.btnProfileSave = new System.Windows.Forms.Button();
            this.btnProfileDelete = new System.Windows.Forms.Button();
            this.chkOutput = new System.Windows.Forms.CheckBox();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbxBanner = new System.Windows.Forms.PictureBox();
            this.btnQueueAdd = new System.Windows.Forms.Button();
            this.btnQueueRemove = new System.Windows.Forms.Button();
            this.btnQueueMoveUp = new System.Windows.Forms.Button();
            this.btnQueueMoveDown = new System.Windows.Forms.Button();
            this.btnQueueStop = new System.Windows.Forms.Button();
            this.btnQueuePause = new System.Windows.Forms.Button();
            this.btnQueueStart = new System.Windows.Forms.Button();
            this.colQueueTarget = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsQueue.SuspendLayout();
            this.tabProp.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.grpPropFormat.SuspendLayout();
            this.grpPropOutput.SuspendLayout();
            this.grpPropInput.SuspendLayout();
            this.tabVideo.SuspendLayout();
            this.grpVideoDeInterlace.SuspendLayout();
            this.grpVideoCodec.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoMultipass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoRateFactor)).BeginInit();
            this.grpVideoQuality.SuspendLayout();
            this.tabAudio.SuspendLayout();
            this.grpAudioCodec.SuspendLayout();
            this.tabSubtitle.SuspendLayout();
            this.tabAttachment.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // chkDonePowerOff
            // 
            this.chkDonePowerOff.Location = new System.Drawing.Point(12, 70);
            this.chkDonePowerOff.Name = "chkDonePowerOff";
            this.chkDonePowerOff.Size = new System.Drawing.Size(240, 32);
            this.chkDonePowerOff.TabIndex = 10;
            this.chkDonePowerOff.Text = "&Shutdown computer when done";
            this.chkDonePowerOff.UseVisualStyleBackColor = true;
            // 
            // lblSpacer1
            // 
            this.lblSpacer1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpacer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSpacer1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSpacer1.Location = new System.Drawing.Point(572, 70);
            this.lblSpacer1.Name = "lblSpacer1";
            this.lblSpacer1.Size = new System.Drawing.Size(2, 32);
            this.lblSpacer1.TabIndex = 4;
            // 
            // lblSpacer2
            // 
            this.lblSpacer2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpacer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSpacer2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSpacer2.Location = new System.Drawing.Point(488, 70);
            this.lblSpacer2.Name = "lblSpacer2";
            this.lblSpacer2.Size = new System.Drawing.Size(2, 32);
            this.lblSpacer2.TabIndex = 7;
            // 
            // lstQueue
            // 
            this.lstQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstQueue.CheckBoxes = true;
            this.lstQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colQueueName,
            this.colQueueDuration,
            this.colQueueTarget,
            this.colQueueStatus});
            this.lstQueue.ContextMenuStrip = this.cmsQueue;
            this.lstQueue.FullRowSelect = true;
            this.lstQueue.Location = new System.Drawing.Point(12, 108);
            this.lstQueue.Name = "lstQueue";
            this.lstQueue.Size = new System.Drawing.Size(676, 214);
            this.lstQueue.TabIndex = 11;
            this.lstQueue.UseCompatibleStateImageBehavior = false;
            this.lstQueue.View = System.Windows.Forms.View.Details;
            this.lstQueue.SelectedIndexChanged += new System.EventHandler(this.lstQueue_SelectedIndexChanged);
            this.lstQueue.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstQueue_DragDrop);
            this.lstQueue.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstQueue_DragEnter);
            // 
            // colQueueName
            // 
            this.colQueueName.Tag = "colQueueName";
            this.colQueueName.Text = "Name";
            this.colQueueName.Width = 392;
            // 
            // colQueueDuration
            // 
            this.colQueueDuration.Tag = "colQueueDuration";
            this.colQueueDuration.Text = "Duration";
            this.colQueueDuration.Width = 80;
            // 
            // colQueueStatus
            // 
            this.colQueueStatus.Tag = "colQueueStatus";
            this.colQueueStatus.Text = "Status";
            this.colQueueStatus.Width = 120;
            // 
            // cmsQueue
            // 
            this.cmsQueue.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cmsQueue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiQueueNew,
            this.tsmiQueueOpen,
            this.tsmiQueueSave,
            this.tsmiQueueSaveAs,
            this.toolStripSeparator1,
            this.tsmiQueueDel,
            this.tsmiQueueSelectAll,
            this.tsmiQueueSelectNone,
            this.tsmiQueueSelectInvert,
            this.toolStripSeparator2,
            this.tsmiAviSynth});
            this.cmsQueue.Name = "cmsQueue";
            this.cmsQueue.Size = new System.Drawing.Size(193, 214);
            // 
            // tsmiQueueNew
            // 
            this.tsmiQueueNew.Image = global::ifme.Properties.Resources.document_new;
            this.tsmiQueueNew.Name = "tsmiQueueNew";
            this.tsmiQueueNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmiQueueNew.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueNew.Text = "&New queue";
            // 
            // tsmiQueueOpen
            // 
            this.tsmiQueueOpen.Image = global::ifme.Properties.Resources.document_open;
            this.tsmiQueueOpen.Name = "tsmiQueueOpen";
            this.tsmiQueueOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmiQueueOpen.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueOpen.Text = "&Open queue";
            // 
            // tsmiQueueSave
            // 
            this.tsmiQueueSave.Image = global::ifme.Properties.Resources.document_save;
            this.tsmiQueueSave.Name = "tsmiQueueSave";
            this.tsmiQueueSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmiQueueSave.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueSave.Text = "&Save queue";
            // 
            // tsmiQueueSaveAs
            // 
            this.tsmiQueueSaveAs.Image = global::ifme.Properties.Resources.document_save_as;
            this.tsmiQueueSaveAs.Name = "tsmiQueueSaveAs";
            this.tsmiQueueSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.tsmiQueueSaveAs.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueSaveAs.Text = "Save &as...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(189, 6);
            // 
            // tsmiQueueDel
            // 
            this.tsmiQueueDel.Image = global::ifme.Properties.Resources.edit_delete;
            this.tsmiQueueDel.Name = "tsmiQueueDel";
            this.tsmiQueueDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.tsmiQueueDel.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueDel.Text = "&Delete";
            // 
            // tsmiQueueSelectAll
            // 
            this.tsmiQueueSelectAll.Image = global::ifme.Properties.Resources.edit_select_all;
            this.tsmiQueueSelectAll.Name = "tsmiQueueSelectAll";
            this.tsmiQueueSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.tsmiQueueSelectAll.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueSelectAll.Text = "S&elect All";
            // 
            // tsmiQueueSelectNone
            // 
            this.tsmiQueueSelectNone.Image = global::ifme.Properties.Resources.edit_bomb;
            this.tsmiQueueSelectNone.Name = "tsmiQueueSelectNone";
            this.tsmiQueueSelectNone.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.tsmiQueueSelectNone.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueSelectNone.Text = "Select N&one";
            // 
            // tsmiQueueSelectInvert
            // 
            this.tsmiQueueSelectInvert.Image = global::ifme.Properties.Resources.edit_redo;
            this.tsmiQueueSelectInvert.Name = "tsmiQueueSelectInvert";
            this.tsmiQueueSelectInvert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsmiQueueSelectInvert.Size = new System.Drawing.Size(192, 22);
            this.tsmiQueueSelectInvert.Text = "&Invert selection";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(189, 6);
            // 
            // tsmiAviSynth
            // 
            this.tsmiAviSynth.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiQueueAviSynthEdit,
            this.toolStripSeparator3,
            this.tsmiQueueAviSynthConvertTo});
            this.tsmiAviSynth.Image = global::ifme.Properties.Resources.code_context;
            this.tsmiAviSynth.Name = "tsmiAviSynth";
            this.tsmiAviSynth.Size = new System.Drawing.Size(192, 22);
            this.tsmiAviSynth.Text = "A&viSynth";
            // 
            // tsmiQueueAviSynthEdit
            // 
            this.tsmiQueueAviSynthEdit.Name = "tsmiQueueAviSynthEdit";
            this.tsmiQueueAviSynthEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.tsmiQueueAviSynthEdit.Size = new System.Drawing.Size(172, 22);
            this.tsmiQueueAviSynthEdit.Text = "Edit S&cript";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(169, 6);
            // 
            // tsmiQueueAviSynthConvertTo
            // 
            this.tsmiQueueAviSynthConvertTo.Name = "tsmiQueueAviSynthConvertTo";
            this.tsmiQueueAviSynthConvertTo.Size = new System.Drawing.Size(172, 22);
            this.tsmiQueueAviSynthConvertTo.Text = "&Convert to AviSynth";
            // 
            // tabProp
            // 
            this.tabProp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabProp.Controls.Add(this.tabInfo);
            this.tabProp.Controls.Add(this.tabVideo);
            this.tabProp.Controls.Add(this.tabAudio);
            this.tabProp.Controls.Add(this.tabSubtitle);
            this.tabProp.Controls.Add(this.tabAttachment);
            this.tabProp.Location = new System.Drawing.Point(12, 328);
            this.tabProp.Name = "tabProp";
            this.tabProp.SelectedIndex = 0;
            this.tabProp.Size = new System.Drawing.Size(676, 300);
            this.tabProp.TabIndex = 12;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.grpPropFormat);
            this.tabInfo.Controls.Add(this.grpPropOutput);
            this.tabInfo.Controls.Add(this.grpPropInput);
            this.tabInfo.Location = new System.Drawing.Point(4, 22);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabInfo.Size = new System.Drawing.Size(668, 274);
            this.tabInfo.TabIndex = 0;
            this.tabInfo.Text = "Properties";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // grpPropFormat
            // 
            this.grpPropFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPropFormat.Controls.Add(this.rdoMKV);
            this.grpPropFormat.Controls.Add(this.rdoMP4);
            this.grpPropFormat.Location = new System.Drawing.Point(337, 6);
            this.grpPropFormat.Name = "grpPropFormat";
            this.grpPropFormat.Size = new System.Drawing.Size(325, 50);
            this.grpPropFormat.TabIndex = 1;
            this.grpPropFormat.TabStop = false;
            this.grpPropFormat.Text = "Output &Format";
            // 
            // rdoMKV
            // 
            this.rdoMKV.AutoSize = true;
            this.rdoMKV.Location = new System.Drawing.Point(165, 19);
            this.rdoMKV.Name = "rdoMKV";
            this.rdoMKV.Size = new System.Drawing.Size(45, 17);
            this.rdoMKV.TabIndex = 1;
            this.rdoMKV.TabStop = true;
            this.rdoMKV.Text = "M&KV";
            this.rdoMKV.UseVisualStyleBackColor = true;
            this.rdoMKV.CheckedChanged += new System.EventHandler(this.rdoMKV_CheckedChanged);
            // 
            // rdoMP4
            // 
            this.rdoMP4.AutoSize = true;
            this.rdoMP4.Location = new System.Drawing.Point(114, 19);
            this.rdoMP4.Name = "rdoMP4";
            this.rdoMP4.Size = new System.Drawing.Size(45, 17);
            this.rdoMP4.TabIndex = 0;
            this.rdoMP4.TabStop = true;
            this.rdoMP4.Text = "&MP4";
            this.rdoMP4.UseVisualStyleBackColor = true;
            this.rdoMP4.CheckedChanged += new System.EventHandler(this.rdoMP4_CheckedChanged);
            // 
            // grpPropOutput
            // 
            this.grpPropOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPropOutput.Controls.Add(this.txtOutputInfo);
            this.grpPropOutput.Location = new System.Drawing.Point(337, 62);
            this.grpPropOutput.Name = "grpPropOutput";
            this.grpPropOutput.Size = new System.Drawing.Size(325, 206);
            this.grpPropOutput.TabIndex = 2;
            this.grpPropOutput.TabStop = false;
            this.grpPropOutput.Text = "&Output Info";
            // 
            // txtOutputInfo
            // 
            this.txtOutputInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutputInfo.Location = new System.Drawing.Point(3, 16);
            this.txtOutputInfo.Multiline = true;
            this.txtOutputInfo.Name = "txtOutputInfo";
            this.txtOutputInfo.ReadOnly = true;
            this.txtOutputInfo.Size = new System.Drawing.Size(319, 187);
            this.txtOutputInfo.TabIndex = 0;
            this.txtOutputInfo.Text = "Video:\r\nID 0, HEVC, 1280x720 @ 30fps (8bpc @ YUV420)\r\n\r\nAudio:\r\nID 1, AAC, 128 @ " +
    "44100Hz, 2 Channel\r\nID 2, AAC, 512 @ 44100Hz, 6 Channel\r\n\r\nSubtitle:\r\nID 3, ASS," +
    " eng (English)";
            // 
            // grpPropInput
            // 
            this.grpPropInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPropInput.Controls.Add(this.txtSourceInfo);
            this.grpPropInput.Location = new System.Drawing.Point(6, 6);
            this.grpPropInput.Name = "grpPropInput";
            this.grpPropInput.Size = new System.Drawing.Size(325, 262);
            this.grpPropInput.TabIndex = 0;
            this.grpPropInput.TabStop = false;
            this.grpPropInput.Text = "&Source Info";
            // 
            // txtSourceInfo
            // 
            this.txtSourceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSourceInfo.Location = new System.Drawing.Point(3, 16);
            this.txtSourceInfo.Multiline = true;
            this.txtSourceInfo.Name = "txtSourceInfo";
            this.txtSourceInfo.ReadOnly = true;
            this.txtSourceInfo.Size = new System.Drawing.Size(319, 243);
            this.txtSourceInfo.TabIndex = 0;
            // 
            // tabVideo
            // 
            this.tabVideo.Controls.Add(this.chkVideoDeinterlace);
            this.tabVideo.Controls.Add(this.grpVideoDeInterlace);
            this.tabVideo.Controls.Add(this.grpVideoCodec);
            this.tabVideo.Controls.Add(this.grpVideoQuality);
            this.tabVideo.Location = new System.Drawing.Point(4, 22);
            this.tabVideo.Name = "tabVideo";
            this.tabVideo.Padding = new System.Windows.Forms.Padding(3);
            this.tabVideo.Size = new System.Drawing.Size(668, 274);
            this.tabVideo.TabIndex = 1;
            this.tabVideo.Text = "Video";
            this.tabVideo.UseVisualStyleBackColor = true;
            // 
            // chkVideoDeinterlace
            // 
            this.chkVideoDeinterlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkVideoDeinterlace.AutoSize = true;
            this.chkVideoDeinterlace.Location = new System.Drawing.Point(13, 139);
            this.chkVideoDeinterlace.Name = "chkVideoDeinterlace";
            this.chkVideoDeinterlace.Size = new System.Drawing.Size(86, 17);
            this.chkVideoDeinterlace.TabIndex = 1;
            this.chkVideoDeinterlace.Text = "&Deinterlaced";
            this.chkVideoDeinterlace.UseVisualStyleBackColor = true;
            this.chkVideoDeinterlace.CheckedChanged += new System.EventHandler(this.chkVideoDeinterlace_CheckedChanged);
            // 
            // grpVideoDeInterlace
            // 
            this.grpVideoDeInterlace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVideoDeInterlace.Controls.Add(this.cboVideoDiField);
            this.grpVideoDeInterlace.Controls.Add(this.lblVideoDiField);
            this.grpVideoDeInterlace.Controls.Add(this.cboVideoDiMode);
            this.grpVideoDeInterlace.Controls.Add(this.lblVideoDiMode);
            this.grpVideoDeInterlace.Location = new System.Drawing.Point(6, 140);
            this.grpVideoDeInterlace.Name = "grpVideoDeInterlace";
            this.grpVideoDeInterlace.Size = new System.Drawing.Size(325, 128);
            this.grpVideoDeInterlace.TabIndex = 2;
            this.grpVideoDeInterlace.TabStop = false;
            // 
            // cboVideoDiField
            // 
            this.cboVideoDiField.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboVideoDiField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoDiField.FormattingEnabled = true;
            this.cboVideoDiField.Location = new System.Drawing.Point(40, 82);
            this.cboVideoDiField.Name = "cboVideoDiField";
            this.cboVideoDiField.Size = new System.Drawing.Size(248, 21);
            this.cboVideoDiField.TabIndex = 3;
            this.cboVideoDiField.SelectedIndexChanged += new System.EventHandler(this.cboVideoDiField_SelectedIndexChanged);
            // 
            // lblVideoDiField
            // 
            this.lblVideoDiField.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblVideoDiField.AutoSize = true;
            this.lblVideoDiField.Location = new System.Drawing.Point(37, 66);
            this.lblVideoDiField.Name = "lblVideoDiField";
            this.lblVideoDiField.Size = new System.Drawing.Size(33, 13);
            this.lblVideoDiField.TabIndex = 2;
            this.lblVideoDiField.Text = "&Field:";
            // 
            // cboVideoDiMode
            // 
            this.cboVideoDiMode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboVideoDiMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoDiMode.FormattingEnabled = true;
            this.cboVideoDiMode.Location = new System.Drawing.Point(40, 42);
            this.cboVideoDiMode.Name = "cboVideoDiMode";
            this.cboVideoDiMode.Size = new System.Drawing.Size(248, 21);
            this.cboVideoDiMode.TabIndex = 1;
            this.cboVideoDiMode.SelectedIndexChanged += new System.EventHandler(this.cboVideoDiMode_SelectedIndexChanged);
            // 
            // lblVideoDiMode
            // 
            this.lblVideoDiMode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblVideoDiMode.AutoSize = true;
            this.lblVideoDiMode.Location = new System.Drawing.Point(37, 26);
            this.lblVideoDiMode.Name = "lblVideoDiMode";
            this.lblVideoDiMode.Size = new System.Drawing.Size(37, 13);
            this.lblVideoDiMode.TabIndex = 0;
            this.lblVideoDiMode.Text = "&Mode:";
            // 
            // grpVideoCodec
            // 
            this.grpVideoCodec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVideoCodec.Controls.Add(this.btnVideoArgEdit);
            this.grpVideoCodec.Controls.Add(this.nudVideoMultipass);
            this.grpVideoCodec.Controls.Add(this.lblVideoMultipass);
            this.grpVideoCodec.Controls.Add(this.lblVideoRateFactor);
            this.grpVideoCodec.Controls.Add(this.nudVideoRateFactor);
            this.grpVideoCodec.Controls.Add(this.lblVideoRateControl);
            this.grpVideoCodec.Controls.Add(this.cboVideoTune);
            this.grpVideoCodec.Controls.Add(this.lblVideoTune);
            this.grpVideoCodec.Controls.Add(this.cboVideoPreset);
            this.grpVideoCodec.Controls.Add(this.lblVideoPreset);
            this.grpVideoCodec.Controls.Add(this.cboVideoEncodingType);
            this.grpVideoCodec.Controls.Add(this.cboVideoEncoder);
            this.grpVideoCodec.Controls.Add(this.lblVideoEncoder);
            this.grpVideoCodec.Location = new System.Drawing.Point(337, 6);
            this.grpVideoCodec.Name = "grpVideoCodec";
            this.grpVideoCodec.Size = new System.Drawing.Size(325, 262);
            this.grpVideoCodec.TabIndex = 3;
            this.grpVideoCodec.TabStop = false;
            this.grpVideoCodec.Text = "&Codec";
            // 
            // btnVideoArgEdit
            // 
            this.btnVideoArgEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVideoArgEdit.Location = new System.Drawing.Point(12, 196);
            this.btnVideoArgEdit.Name = "btnVideoArgEdit";
            this.btnVideoArgEdit.Size = new System.Drawing.Size(300, 32);
            this.btnVideoArgEdit.TabIndex = 12;
            this.btnVideoArgEdit.Text = "Edit encoder command-&line";
            this.btnVideoArgEdit.UseVisualStyleBackColor = true;
            this.btnVideoArgEdit.Click += new System.EventHandler(this.btnVideoArgEdit_Click);
            // 
            // nudVideoMultipass
            // 
            this.nudVideoMultipass.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.nudVideoMultipass.Location = new System.Drawing.Point(165, 170);
            this.nudVideoMultipass.Name = "nudVideoMultipass";
            this.nudVideoMultipass.Size = new System.Drawing.Size(147, 20);
            this.nudVideoMultipass.TabIndex = 11;
            this.nudVideoMultipass.ValueChanged += new System.EventHandler(this.nudVideoMultipass_ValueChanged);
            // 
            // lblVideoMultipass
            // 
            this.lblVideoMultipass.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblVideoMultipass.AutoSize = true;
            this.lblVideoMultipass.Location = new System.Drawing.Point(162, 154);
            this.lblVideoMultipass.Name = "lblVideoMultipass";
            this.lblVideoMultipass.Size = new System.Drawing.Size(55, 13);
            this.lblVideoMultipass.TabIndex = 10;
            this.lblVideoMultipass.Text = "&Multipass:";
            // 
            // lblVideoRateFactor
            // 
            this.lblVideoRateFactor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblVideoRateFactor.AutoSize = true;
            this.lblVideoRateFactor.Location = new System.Drawing.Point(9, 154);
            this.lblVideoRateFactor.Name = "lblVideoRateFactor";
            this.lblVideoRateFactor.Size = new System.Drawing.Size(63, 13);
            this.lblVideoRateFactor.TabIndex = 8;
            this.lblVideoRateFactor.Text = "Rate&factor:";
            // 
            // nudVideoRateFactor
            // 
            this.nudVideoRateFactor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.nudVideoRateFactor.Increment = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudVideoRateFactor.Location = new System.Drawing.Point(12, 170);
            this.nudVideoRateFactor.Maximum = new decimal(new int[] {
            10485760,
            0,
            0,
            0});
            this.nudVideoRateFactor.Name = "nudVideoRateFactor";
            this.nudVideoRateFactor.Size = new System.Drawing.Size(147, 20);
            this.nudVideoRateFactor.TabIndex = 9;
            this.nudVideoRateFactor.ValueChanged += new System.EventHandler(this.nudVideoRateFactor_ValueChanged);
            // 
            // lblVideoRateControl
            // 
            this.lblVideoRateControl.AutoSize = true;
            this.lblVideoRateControl.Location = new System.Drawing.Point(9, 114);
            this.lblVideoRateControl.Name = "lblVideoRateControl";
            this.lblVideoRateControl.Size = new System.Drawing.Size(70, 13);
            this.lblVideoRateControl.TabIndex = 6;
            this.lblVideoRateControl.Text = "&Rate control:";
            // 
            // cboVideoTune
            // 
            this.cboVideoTune.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cboVideoTune.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoTune.FormattingEnabled = true;
            this.cboVideoTune.Location = new System.Drawing.Point(165, 90);
            this.cboVideoTune.Name = "cboVideoTune";
            this.cboVideoTune.Size = new System.Drawing.Size(147, 21);
            this.cboVideoTune.TabIndex = 5;
            this.cboVideoTune.SelectedIndexChanged += new System.EventHandler(this.cboVideoTune_SelectedIndexChanged);
            // 
            // lblVideoTune
            // 
            this.lblVideoTune.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblVideoTune.AutoSize = true;
            this.lblVideoTune.Location = new System.Drawing.Point(162, 74);
            this.lblVideoTune.Name = "lblVideoTune";
            this.lblVideoTune.Size = new System.Drawing.Size(35, 13);
            this.lblVideoTune.TabIndex = 4;
            this.lblVideoTune.Text = "&Tune:";
            // 
            // cboVideoPreset
            // 
            this.cboVideoPreset.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cboVideoPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoPreset.FormattingEnabled = true;
            this.cboVideoPreset.Location = new System.Drawing.Point(12, 90);
            this.cboVideoPreset.Name = "cboVideoPreset";
            this.cboVideoPreset.Size = new System.Drawing.Size(147, 21);
            this.cboVideoPreset.TabIndex = 3;
            this.cboVideoPreset.SelectedIndexChanged += new System.EventHandler(this.cboVideoPreset_SelectedIndexChanged);
            // 
            // lblVideoPreset
            // 
            this.lblVideoPreset.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblVideoPreset.AutoSize = true;
            this.lblVideoPreset.Location = new System.Drawing.Point(9, 74);
            this.lblVideoPreset.Name = "lblVideoPreset";
            this.lblVideoPreset.Size = new System.Drawing.Size(42, 13);
            this.lblVideoPreset.TabIndex = 2;
            this.lblVideoPreset.Text = "&Preset:";
            // 
            // cboVideoEncodingType
            // 
            this.cboVideoEncodingType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoEncodingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoEncodingType.FormattingEnabled = true;
            this.cboVideoEncodingType.Location = new System.Drawing.Point(12, 130);
            this.cboVideoEncodingType.Name = "cboVideoEncodingType";
            this.cboVideoEncodingType.Size = new System.Drawing.Size(300, 21);
            this.cboVideoEncodingType.TabIndex = 7;
            this.cboVideoEncodingType.SelectedIndexChanged += new System.EventHandler(this.cboVideoEncodingType_SelectedIndexChanged);
            // 
            // cboVideoEncoder
            // 
            this.cboVideoEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVideoEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoEncoder.FormattingEnabled = true;
            this.cboVideoEncoder.Location = new System.Drawing.Point(12, 50);
            this.cboVideoEncoder.Name = "cboVideoEncoder";
            this.cboVideoEncoder.Size = new System.Drawing.Size(300, 21);
            this.cboVideoEncoder.TabIndex = 1;
            this.cboVideoEncoder.SelectedIndexChanged += new System.EventHandler(this.cboVideoEncoder_SelectedIndexChanged);
            // 
            // lblVideoEncoder
            // 
            this.lblVideoEncoder.AutoSize = true;
            this.lblVideoEncoder.Location = new System.Drawing.Point(9, 34);
            this.lblVideoEncoder.Name = "lblVideoEncoder";
            this.lblVideoEncoder.Size = new System.Drawing.Size(50, 13);
            this.lblVideoEncoder.TabIndex = 0;
            this.lblVideoEncoder.Text = "&Encoder:";
            // 
            // grpVideoQuality
            // 
            this.grpVideoQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVideoQuality.Controls.Add(this.cboVideoChroma);
            this.grpVideoQuality.Controls.Add(this.lblVideoColourSpace);
            this.grpVideoQuality.Controls.Add(this.cboVideoBitDepth);
            this.grpVideoQuality.Controls.Add(this.lblVideoBitDepth);
            this.grpVideoQuality.Controls.Add(this.cboVideoFrameRate);
            this.grpVideoQuality.Controls.Add(this.lblVideoFrameRate);
            this.grpVideoQuality.Controls.Add(this.cboVideoResolution);
            this.grpVideoQuality.Controls.Add(this.lblVideoResolution);
            this.grpVideoQuality.Location = new System.Drawing.Point(6, 6);
            this.grpVideoQuality.Name = "grpVideoQuality";
            this.grpVideoQuality.Size = new System.Drawing.Size(325, 128);
            this.grpVideoQuality.TabIndex = 0;
            this.grpVideoQuality.TabStop = false;
            this.grpVideoQuality.Text = "&Quality";
            // 
            // cboVideoChroma
            // 
            this.cboVideoChroma.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboVideoChroma.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoChroma.FormattingEnabled = true;
            this.cboVideoChroma.Items.AddRange(new object[] {
            "420",
            "422",
            "444"});
            this.cboVideoChroma.Location = new System.Drawing.Point(167, 82);
            this.cboVideoChroma.Name = "cboVideoChroma";
            this.cboVideoChroma.Size = new System.Drawing.Size(121, 21);
            this.cboVideoChroma.TabIndex = 7;
            this.cboVideoChroma.SelectedIndexChanged += new System.EventHandler(this.cboVideoChroma_SelectedIndexChanged);
            // 
            // lblVideoColourSpace
            // 
            this.lblVideoColourSpace.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblVideoColourSpace.AutoSize = true;
            this.lblVideoColourSpace.Location = new System.Drawing.Point(164, 66);
            this.lblVideoColourSpace.Name = "lblVideoColourSpace";
            this.lblVideoColourSpace.Size = new System.Drawing.Size(109, 13);
            this.lblVideoColourSpace.TabIndex = 6;
            this.lblVideoColourSpace.Text = "&Chroma subsampling:";
            // 
            // cboVideoBitDepth
            // 
            this.cboVideoBitDepth.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboVideoBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVideoBitDepth.FormattingEnabled = true;
            this.cboVideoBitDepth.Items.AddRange(new object[] {
            "8",
            "10",
            "12"});
            this.cboVideoBitDepth.Location = new System.Drawing.Point(40, 82);
            this.cboVideoBitDepth.Name = "cboVideoBitDepth";
            this.cboVideoBitDepth.Size = new System.Drawing.Size(121, 21);
            this.cboVideoBitDepth.TabIndex = 5;
            this.cboVideoBitDepth.SelectedIndexChanged += new System.EventHandler(this.cboVideoBitDepth_SelectedIndexChanged);
            // 
            // lblVideoBitDepth
            // 
            this.lblVideoBitDepth.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblVideoBitDepth.AutoSize = true;
            this.lblVideoBitDepth.Location = new System.Drawing.Point(37, 66);
            this.lblVideoBitDepth.Name = "lblVideoBitDepth";
            this.lblVideoBitDepth.Size = new System.Drawing.Size(54, 13);
            this.lblVideoBitDepth.TabIndex = 4;
            this.lblVideoBitDepth.Text = "Bit &depth:";
            // 
            // cboVideoFrameRate
            // 
            this.cboVideoFrameRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboVideoFrameRate.FormattingEnabled = true;
            this.cboVideoFrameRate.Items.AddRange(new object[] {
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
            "100",
            "120"});
            this.cboVideoFrameRate.Location = new System.Drawing.Point(167, 42);
            this.cboVideoFrameRate.Name = "cboVideoFrameRate";
            this.cboVideoFrameRate.Size = new System.Drawing.Size(121, 21);
            this.cboVideoFrameRate.TabIndex = 3;
            this.cboVideoFrameRate.TextChanged += new System.EventHandler(this.cboVideoFrameRate_TextChanged);
            this.cboVideoFrameRate.Leave += new System.EventHandler(this.cboVideoFrameRate_Leave);
            // 
            // lblVideoFrameRate
            // 
            this.lblVideoFrameRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblVideoFrameRate.AutoSize = true;
            this.lblVideoFrameRate.Location = new System.Drawing.Point(164, 26);
            this.lblVideoFrameRate.Name = "lblVideoFrameRate";
            this.lblVideoFrameRate.Size = new System.Drawing.Size(64, 13);
            this.lblVideoFrameRate.TabIndex = 2;
            this.lblVideoFrameRate.Text = "&Frame rate:";
            // 
            // cboVideoResolution
            // 
            this.cboVideoResolution.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboVideoResolution.FormattingEnabled = true;
            this.cboVideoResolution.Items.AddRange(new object[] {
            "auto",
            "640x480",
            "640x360",
            "720x404",
            "800x600",
            "853x480",
            "1024x768",
            "1024x576",
            "1280x720",
            "1920x1080",
            "3840x2160",
            "7680x4320"});
            this.cboVideoResolution.Location = new System.Drawing.Point(40, 42);
            this.cboVideoResolution.Name = "cboVideoResolution";
            this.cboVideoResolution.Size = new System.Drawing.Size(121, 21);
            this.cboVideoResolution.TabIndex = 1;
            this.cboVideoResolution.TextChanged += new System.EventHandler(this.cboVideoResolution_TextChanged);
            this.cboVideoResolution.Leave += new System.EventHandler(this.cboVideoResolution_Leave);
            // 
            // lblVideoResolution
            // 
            this.lblVideoResolution.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblVideoResolution.AutoSize = true;
            this.lblVideoResolution.Location = new System.Drawing.Point(37, 26);
            this.lblVideoResolution.Name = "lblVideoResolution";
            this.lblVideoResolution.Size = new System.Drawing.Size(61, 13);
            this.lblVideoResolution.TabIndex = 0;
            this.lblVideoResolution.Text = "&Resolution:";
            // 
            // tabAudio
            // 
            this.tabAudio.Controls.Add(this.btnAudioMoveDown);
            this.tabAudio.Controls.Add(this.btnAudioMoveUp);
            this.tabAudio.Controls.Add(this.lblSpacer4);
            this.tabAudio.Controls.Add(this.lstAudio);
            this.tabAudio.Controls.Add(this.grpAudioCodec);
            this.tabAudio.Controls.Add(this.btnAudioRemove);
            this.tabAudio.Controls.Add(this.btnAudioAdd);
            this.tabAudio.Location = new System.Drawing.Point(4, 22);
            this.tabAudio.Name = "tabAudio";
            this.tabAudio.Padding = new System.Windows.Forms.Padding(3);
            this.tabAudio.Size = new System.Drawing.Size(668, 274);
            this.tabAudio.TabIndex = 2;
            this.tabAudio.Text = "Audio";
            this.tabAudio.UseVisualStyleBackColor = true;
            // 
            // btnAudioMoveDown
            // 
            this.btnAudioMoveDown.Image = global::ifme.Properties.Resources.go_down;
            this.btnAudioMoveDown.Location = new System.Drawing.Point(128, 6);
            this.btnAudioMoveDown.Name = "btnAudioMoveDown";
            this.btnAudioMoveDown.Size = new System.Drawing.Size(32, 32);
            this.btnAudioMoveDown.TabIndex = 4;
            this.btnAudioMoveDown.UseVisualStyleBackColor = true;
            this.btnAudioMoveDown.Click += new System.EventHandler(this.btnAudioMoveDown_Click);
            // 
            // btnAudioMoveUp
            // 
            this.btnAudioMoveUp.Image = global::ifme.Properties.Resources.go_up;
            this.btnAudioMoveUp.Location = new System.Drawing.Point(90, 6);
            this.btnAudioMoveUp.Name = "btnAudioMoveUp";
            this.btnAudioMoveUp.Size = new System.Drawing.Size(32, 32);
            this.btnAudioMoveUp.TabIndex = 3;
            this.btnAudioMoveUp.UseVisualStyleBackColor = true;
            this.btnAudioMoveUp.Click += new System.EventHandler(this.btnAudioMoveUp_Click);
            // 
            // lblSpacer4
            // 
            this.lblSpacer4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSpacer4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSpacer4.Location = new System.Drawing.Point(82, 6);
            this.lblSpacer4.Name = "lblSpacer4";
            this.lblSpacer4.Size = new System.Drawing.Size(2, 32);
            this.lblSpacer4.TabIndex = 2;
            // 
            // lstAudio
            // 
            this.lstAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAudio.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAudioName,
            this.colAudioId});
            this.lstAudio.Location = new System.Drawing.Point(6, 44);
            this.lstAudio.MultiSelect = false;
            this.lstAudio.Name = "lstAudio";
            this.lstAudio.Size = new System.Drawing.Size(325, 224);
            this.lstAudio.TabIndex = 5;
            this.lstAudio.UseCompatibleStateImageBehavior = false;
            this.lstAudio.View = System.Windows.Forms.View.Details;
            this.lstAudio.SelectedIndexChanged += new System.EventHandler(this.lstAudio_SelectedIndexChanged);
            // 
            // colAudioName
            // 
            this.colAudioName.Tag = "colAudioName";
            this.colAudioName.Text = "Name";
            this.colAudioName.Width = 160;
            // 
            // colAudioId
            // 
            this.colAudioId.Tag = "colAudioId";
            this.colAudioId.Text = "Info";
            this.colAudioId.Width = 160;
            // 
            // grpAudioCodec
            // 
            this.grpAudioCodec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAudioCodec.Controls.Add(this.lblAudioMode);
            this.grpAudioCodec.Controls.Add(this.cboAudioMode);
            this.grpAudioCodec.Controls.Add(this.btnAudioEditArg);
            this.grpAudioCodec.Controls.Add(this.lblAudioEncoder);
            this.grpAudioCodec.Controls.Add(this.lblAudioChannel);
            this.grpAudioCodec.Controls.Add(this.cboAudioEncoder);
            this.grpAudioCodec.Controls.Add(this.cboAudioChannel);
            this.grpAudioCodec.Controls.Add(this.lblAudioQuality);
            this.grpAudioCodec.Controls.Add(this.cboAudioFreq);
            this.grpAudioCodec.Controls.Add(this.cboAudioQuality);
            this.grpAudioCodec.Controls.Add(this.lblAudioFreq);
            this.grpAudioCodec.Location = new System.Drawing.Point(337, 6);
            this.grpAudioCodec.Name = "grpAudioCodec";
            this.grpAudioCodec.Size = new System.Drawing.Size(325, 262);
            this.grpAudioCodec.TabIndex = 6;
            this.grpAudioCodec.TabStop = false;
            this.grpAudioCodec.Text = "&Codec";
            // 
            // lblAudioMode
            // 
            this.lblAudioMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAudioMode.AutoSize = true;
            this.lblAudioMode.Location = new System.Drawing.Point(214, 74);
            this.lblAudioMode.Name = "lblAudioMode";
            this.lblAudioMode.Size = new System.Drawing.Size(37, 13);
            this.lblAudioMode.TabIndex = 2;
            this.lblAudioMode.Text = "Mode:";
            // 
            // cboAudioMode
            // 
            this.cboAudioMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAudioMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioMode.FormattingEnabled = true;
            this.cboAudioMode.Location = new System.Drawing.Point(217, 90);
            this.cboAudioMode.Name = "cboAudioMode";
            this.cboAudioMode.Size = new System.Drawing.Size(96, 21);
            this.cboAudioMode.TabIndex = 3;
            this.cboAudioMode.SelectedIndexChanged += new System.EventHandler(this.cboAudioMode_SelectedIndexChanged);
            // 
            // btnAudioEditArg
            // 
            this.btnAudioEditArg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAudioEditArg.Location = new System.Drawing.Point(13, 157);
            this.btnAudioEditArg.Name = "btnAudioEditArg";
            this.btnAudioEditArg.Size = new System.Drawing.Size(300, 32);
            this.btnAudioEditArg.TabIndex = 10;
            this.btnAudioEditArg.Text = "Edit encoder command-&line";
            this.btnAudioEditArg.UseVisualStyleBackColor = true;
            this.btnAudioEditArg.Click += new System.EventHandler(this.btnAudioEditArg_Click);
            // 
            // lblAudioEncoder
            // 
            this.lblAudioEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAudioEncoder.AutoSize = true;
            this.lblAudioEncoder.Location = new System.Drawing.Point(10, 74);
            this.lblAudioEncoder.Name = "lblAudioEncoder";
            this.lblAudioEncoder.Size = new System.Drawing.Size(50, 13);
            this.lblAudioEncoder.TabIndex = 0;
            this.lblAudioEncoder.Text = "&Encoder:";
            // 
            // lblAudioChannel
            // 
            this.lblAudioChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAudioChannel.AutoSize = true;
            this.lblAudioChannel.Location = new System.Drawing.Point(214, 114);
            this.lblAudioChannel.Name = "lblAudioChannel";
            this.lblAudioChannel.Size = new System.Drawing.Size(50, 13);
            this.lblAudioChannel.TabIndex = 8;
            this.lblAudioChannel.Text = "&Channel:";
            // 
            // cboAudioEncoder
            // 
            this.cboAudioEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAudioEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioEncoder.FormattingEnabled = true;
            this.cboAudioEncoder.Location = new System.Drawing.Point(13, 90);
            this.cboAudioEncoder.Name = "cboAudioEncoder";
            this.cboAudioEncoder.Size = new System.Drawing.Size(198, 21);
            this.cboAudioEncoder.TabIndex = 1;
            this.cboAudioEncoder.SelectedIndexChanged += new System.EventHandler(this.cboAudioEncoder_SelectedIndexChanged);
            // 
            // cboAudioChannel
            // 
            this.cboAudioChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboAudioChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioChannel.FormattingEnabled = true;
            this.cboAudioChannel.Location = new System.Drawing.Point(217, 130);
            this.cboAudioChannel.Name = "cboAudioChannel";
            this.cboAudioChannel.Size = new System.Drawing.Size(96, 21);
            this.cboAudioChannel.TabIndex = 9;
            this.cboAudioChannel.SelectedIndexChanged += new System.EventHandler(this.cboAudioChannel_SelectedIndexChanged);
            // 
            // lblAudioQuality
            // 
            this.lblAudioQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAudioQuality.AutoSize = true;
            this.lblAudioQuality.Location = new System.Drawing.Point(10, 114);
            this.lblAudioQuality.Name = "lblAudioQuality";
            this.lblAudioQuality.Size = new System.Drawing.Size(45, 13);
            this.lblAudioQuality.TabIndex = 4;
            this.lblAudioQuality.Text = "&Quality:";
            // 
            // cboAudioFreq
            // 
            this.cboAudioFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboAudioFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioFreq.FormattingEnabled = true;
            this.cboAudioFreq.Location = new System.Drawing.Point(115, 130);
            this.cboAudioFreq.Name = "cboAudioFreq";
            this.cboAudioFreq.Size = new System.Drawing.Size(96, 21);
            this.cboAudioFreq.TabIndex = 7;
            this.cboAudioFreq.SelectedIndexChanged += new System.EventHandler(this.cboAudioFreq_SelectedIndexChanged);
            // 
            // cboAudioQuality
            // 
            this.cboAudioQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboAudioQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAudioQuality.FormattingEnabled = true;
            this.cboAudioQuality.Location = new System.Drawing.Point(13, 130);
            this.cboAudioQuality.Name = "cboAudioQuality";
            this.cboAudioQuality.Size = new System.Drawing.Size(96, 21);
            this.cboAudioQuality.TabIndex = 5;
            this.cboAudioQuality.SelectedIndexChanged += new System.EventHandler(this.cboAudioQuality_SelectedIndexChanged);
            // 
            // lblAudioFreq
            // 
            this.lblAudioFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAudioFreq.AutoSize = true;
            this.lblAudioFreq.Location = new System.Drawing.Point(112, 114);
            this.lblAudioFreq.Name = "lblAudioFreq";
            this.lblAudioFreq.Size = new System.Drawing.Size(62, 13);
            this.lblAudioFreq.TabIndex = 6;
            this.lblAudioFreq.Text = "&Frequency:";
            // 
            // btnAudioRemove
            // 
            this.btnAudioRemove.Image = global::ifme.Properties.Resources.list_remove;
            this.btnAudioRemove.Location = new System.Drawing.Point(44, 6);
            this.btnAudioRemove.Name = "btnAudioRemove";
            this.btnAudioRemove.Size = new System.Drawing.Size(32, 32);
            this.btnAudioRemove.TabIndex = 1;
            this.btnAudioRemove.UseVisualStyleBackColor = true;
            this.btnAudioRemove.Click += new System.EventHandler(this.btnAudioRemove_Click);
            // 
            // btnAudioAdd
            // 
            this.btnAudioAdd.Image = global::ifme.Properties.Resources.list_add;
            this.btnAudioAdd.Location = new System.Drawing.Point(6, 6);
            this.btnAudioAdd.Name = "btnAudioAdd";
            this.btnAudioAdd.Size = new System.Drawing.Size(32, 32);
            this.btnAudioAdd.TabIndex = 0;
            this.btnAudioAdd.UseVisualStyleBackColor = true;
            this.btnAudioAdd.Click += new System.EventHandler(this.btnAudioAdd_Click);
            // 
            // tabSubtitle
            // 
            this.tabSubtitle.Controls.Add(this.cboSubLang);
            this.tabSubtitle.Controls.Add(this.lstSub);
            this.tabSubtitle.Controls.Add(this.lblSpacer5);
            this.tabSubtitle.Controls.Add(this.btnSubMoveDown);
            this.tabSubtitle.Controls.Add(this.btnSubMoveUp);
            this.tabSubtitle.Controls.Add(this.btnSubRemove);
            this.tabSubtitle.Controls.Add(this.btnSubAdd);
            this.tabSubtitle.Location = new System.Drawing.Point(4, 22);
            this.tabSubtitle.Name = "tabSubtitle";
            this.tabSubtitle.Padding = new System.Windows.Forms.Padding(3);
            this.tabSubtitle.Size = new System.Drawing.Size(668, 274);
            this.tabSubtitle.TabIndex = 3;
            this.tabSubtitle.Text = "Subtitle";
            this.tabSubtitle.UseVisualStyleBackColor = true;
            // 
            // cboSubLang
            // 
            this.cboSubLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSubLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubLang.FormattingEnabled = true;
            this.cboSubLang.Location = new System.Drawing.Point(6, 247);
            this.cboSubLang.Name = "cboSubLang";
            this.cboSubLang.Size = new System.Drawing.Size(656, 21);
            this.cboSubLang.TabIndex = 7;
            this.cboSubLang.SelectedIndexChanged += new System.EventHandler(this.cboSubLang_SelectedIndexChanged);
            // 
            // lstSub
            // 
            this.lstSub.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSubId,
            this.colSubName,
            this.colSubLang});
            this.lstSub.Location = new System.Drawing.Point(6, 44);
            this.lstSub.Name = "lstSub";
            this.lstSub.Size = new System.Drawing.Size(656, 197);
            this.lstSub.TabIndex = 6;
            this.lstSub.UseCompatibleStateImageBehavior = false;
            this.lstSub.View = System.Windows.Forms.View.Details;
            this.lstSub.SelectedIndexChanged += new System.EventHandler(this.lstSub_SelectedIndexChanged);
            // 
            // colSubId
            // 
            this.colSubId.Tag = "colSubId";
            this.colSubId.Text = "Id";
            this.colSubId.Width = 35;
            // 
            // colSubName
            // 
            this.colSubName.Tag = "colSubName";
            this.colSubName.Text = "Name";
            this.colSubName.Width = 300;
            // 
            // colSubLang
            // 
            this.colSubLang.Tag = "colSubLang";
            this.colSubLang.Text = "Language";
            this.colSubLang.Width = 300;
            // 
            // lblSpacer5
            // 
            this.lblSpacer5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSpacer5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSpacer5.Location = new System.Drawing.Point(82, 6);
            this.lblSpacer5.Name = "lblSpacer5";
            this.lblSpacer5.Size = new System.Drawing.Size(2, 32);
            this.lblSpacer5.TabIndex = 3;
            // 
            // btnSubMoveDown
            // 
            this.btnSubMoveDown.Image = global::ifme.Properties.Resources.go_down;
            this.btnSubMoveDown.Location = new System.Drawing.Point(128, 6);
            this.btnSubMoveDown.Name = "btnSubMoveDown";
            this.btnSubMoveDown.Size = new System.Drawing.Size(32, 32);
            this.btnSubMoveDown.TabIndex = 5;
            this.btnSubMoveDown.UseVisualStyleBackColor = true;
            this.btnSubMoveDown.Click += new System.EventHandler(this.btnSubMoveDown_Click);
            // 
            // btnSubMoveUp
            // 
            this.btnSubMoveUp.Image = global::ifme.Properties.Resources.go_up;
            this.btnSubMoveUp.Location = new System.Drawing.Point(90, 6);
            this.btnSubMoveUp.Name = "btnSubMoveUp";
            this.btnSubMoveUp.Size = new System.Drawing.Size(32, 32);
            this.btnSubMoveUp.TabIndex = 4;
            this.btnSubMoveUp.UseVisualStyleBackColor = true;
            this.btnSubMoveUp.Click += new System.EventHandler(this.btnSubMoveUp_Click);
            // 
            // btnSubRemove
            // 
            this.btnSubRemove.Image = global::ifme.Properties.Resources.list_remove;
            this.btnSubRemove.Location = new System.Drawing.Point(44, 6);
            this.btnSubRemove.Name = "btnSubRemove";
            this.btnSubRemove.Size = new System.Drawing.Size(32, 32);
            this.btnSubRemove.TabIndex = 1;
            this.btnSubRemove.UseVisualStyleBackColor = true;
            this.btnSubRemove.Click += new System.EventHandler(this.btnSubRemove_Click);
            // 
            // btnSubAdd
            // 
            this.btnSubAdd.Image = global::ifme.Properties.Resources.list_add;
            this.btnSubAdd.Location = new System.Drawing.Point(6, 6);
            this.btnSubAdd.Name = "btnSubAdd";
            this.btnSubAdd.Size = new System.Drawing.Size(32, 32);
            this.btnSubAdd.TabIndex = 0;
            this.btnSubAdd.UseVisualStyleBackColor = true;
            this.btnSubAdd.Click += new System.EventHandler(this.btnSubAdd_Click);
            // 
            // tabAttachment
            // 
            this.tabAttachment.Controls.Add(this.lstAttach);
            this.tabAttachment.Controls.Add(this.btnAttachDel);
            this.tabAttachment.Controls.Add(this.btnAttachAdd);
            this.tabAttachment.Location = new System.Drawing.Point(4, 22);
            this.tabAttachment.Name = "tabAttachment";
            this.tabAttachment.Padding = new System.Windows.Forms.Padding(3);
            this.tabAttachment.Size = new System.Drawing.Size(668, 274);
            this.tabAttachment.TabIndex = 4;
            this.tabAttachment.Text = "Attachment";
            this.tabAttachment.UseVisualStyleBackColor = true;
            // 
            // lstAttach
            // 
            this.lstAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAttach.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAttachName,
            this.colAttachMime});
            this.lstAttach.Location = new System.Drawing.Point(6, 44);
            this.lstAttach.Name = "lstAttach";
            this.lstAttach.Size = new System.Drawing.Size(656, 224);
            this.lstAttach.TabIndex = 3;
            this.lstAttach.UseCompatibleStateImageBehavior = false;
            this.lstAttach.View = System.Windows.Forms.View.Details;
            this.lstAttach.SelectedIndexChanged += new System.EventHandler(this.lstAttach_SelectedIndexChanged);
            // 
            // colAttachName
            // 
            this.colAttachName.Tag = "colAttachName";
            this.colAttachName.Text = "Name";
            this.colAttachName.Width = 325;
            // 
            // colAttachMime
            // 
            this.colAttachMime.Tag = "colAttachMime";
            this.colAttachMime.Text = "MIME";
            this.colAttachMime.Width = 325;
            // 
            // btnAttachDel
            // 
            this.btnAttachDel.Image = global::ifme.Properties.Resources.list_remove;
            this.btnAttachDel.Location = new System.Drawing.Point(44, 6);
            this.btnAttachDel.Name = "btnAttachDel";
            this.btnAttachDel.Size = new System.Drawing.Size(32, 32);
            this.btnAttachDel.TabIndex = 2;
            this.btnAttachDel.UseVisualStyleBackColor = true;
            this.btnAttachDel.Click += new System.EventHandler(this.btnAttachDel_Click);
            // 
            // btnAttachAdd
            // 
            this.btnAttachAdd.Image = global::ifme.Properties.Resources.list_add;
            this.btnAttachAdd.Location = new System.Drawing.Point(6, 6);
            this.btnAttachAdd.Name = "btnAttachAdd";
            this.btnAttachAdd.Size = new System.Drawing.Size(32, 32);
            this.btnAttachAdd.TabIndex = 1;
            this.btnAttachAdd.UseVisualStyleBackColor = true;
            this.btnAttachAdd.Click += new System.EventHandler(this.btnAttachAdd_Click);
            // 
            // lblProfile
            // 
            this.lblProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProfile.Location = new System.Drawing.Point(12, 634);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(130, 24);
            this.lblProfile.TabIndex = 13;
            this.lblProfile.Text = "Encoding Pro&file:";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboProfile
            // 
            this.cboProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfile.Font = new System.Drawing.Font("Tahoma", 10F);
            this.cboProfile.FormattingEnabled = true;
            this.cboProfile.Location = new System.Drawing.Point(148, 634);
            this.cboProfile.Name = "cboProfile";
            this.cboProfile.Size = new System.Drawing.Size(480, 24);
            this.cboProfile.TabIndex = 14;
            this.cboProfile.SelectedIndexChanged += new System.EventHandler(this.cboProfile_SelectedIndexChanged);
            // 
            // btnProfileSave
            // 
            this.btnProfileSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfileSave.Location = new System.Drawing.Point(634, 634);
            this.btnProfileSave.Name = "btnProfileSave";
            this.btnProfileSave.Size = new System.Drawing.Size(24, 24);
            this.btnProfileSave.TabIndex = 15;
            this.btnProfileSave.UseVisualStyleBackColor = true;
            this.btnProfileSave.Click += new System.EventHandler(this.btnProfileSave_Click);
            // 
            // btnProfileDelete
            // 
            this.btnProfileDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfileDelete.Location = new System.Drawing.Point(664, 634);
            this.btnProfileDelete.Name = "btnProfileDelete";
            this.btnProfileDelete.Size = new System.Drawing.Size(24, 24);
            this.btnProfileDelete.TabIndex = 16;
            this.btnProfileDelete.UseVisualStyleBackColor = true;
            this.btnProfileDelete.Click += new System.EventHandler(this.btnProfileDelete_Click);
            // 
            // chkOutput
            // 
            this.chkOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkOutput.Location = new System.Drawing.Point(12, 664);
            this.chkOutput.Name = "chkOutput";
            this.chkOutput.Size = new System.Drawing.Size(130, 24);
            this.chkOutput.TabIndex = 17;
            this.chkOutput.Text = "&Output:";
            this.chkOutput.UseVisualStyleBackColor = true;
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputFolder.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtOutputFolder.Location = new System.Drawing.Point(148, 664);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(510, 24);
            this.txtOutputFolder.TabIndex = 18;
            this.txtOutputFolder.TextChanged += new System.EventHandler(this.txtOutputFolder_TextChanged);
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutput.Location = new System.Drawing.Point(664, 664);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(24, 24);
            this.btnBrowseOutput.TabIndex = 19;
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.pbxBanner);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(700, 64);
            this.panel1.TabIndex = 0;
            // 
            // pbxBanner
            // 
            this.pbxBanner.BackColor = System.Drawing.Color.Transparent;
            this.pbxBanner.BackgroundImage = global::ifme.Properties.Resources.BannerLeft;
            this.pbxBanner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbxBanner.Location = new System.Drawing.Point(0, 0);
            this.pbxBanner.Name = "pbxBanner";
            this.pbxBanner.Size = new System.Drawing.Size(640, 64);
            this.pbxBanner.TabIndex = 0;
            this.pbxBanner.TabStop = false;
            // 
            // btnQueueAdd
            // 
            this.btnQueueAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueAdd.Image = global::ifme.Properties.Resources.list_add;
            this.btnQueueAdd.Location = new System.Drawing.Point(412, 70);
            this.btnQueueAdd.Name = "btnQueueAdd";
            this.btnQueueAdd.Size = new System.Drawing.Size(32, 32);
            this.btnQueueAdd.TabIndex = 9;
            this.btnQueueAdd.UseVisualStyleBackColor = true;
            this.btnQueueAdd.Click += new System.EventHandler(this.btnQueueAdd_Click);
            // 
            // btnQueueRemove
            // 
            this.btnQueueRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueRemove.Image = global::ifme.Properties.Resources.list_remove;
            this.btnQueueRemove.Location = new System.Drawing.Point(450, 70);
            this.btnQueueRemove.Name = "btnQueueRemove";
            this.btnQueueRemove.Size = new System.Drawing.Size(32, 32);
            this.btnQueueRemove.TabIndex = 8;
            this.btnQueueRemove.UseVisualStyleBackColor = true;
            this.btnQueueRemove.Click += new System.EventHandler(this.btnQueueRemove_Click);
            // 
            // btnQueueMoveUp
            // 
            this.btnQueueMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueMoveUp.Image = global::ifme.Properties.Resources.go_up;
            this.btnQueueMoveUp.Location = new System.Drawing.Point(496, 70);
            this.btnQueueMoveUp.Name = "btnQueueMoveUp";
            this.btnQueueMoveUp.Size = new System.Drawing.Size(32, 32);
            this.btnQueueMoveUp.TabIndex = 6;
            this.btnQueueMoveUp.UseVisualStyleBackColor = true;
            this.btnQueueMoveUp.Click += new System.EventHandler(this.btnQueueMoveUp_Click);
            // 
            // btnQueueMoveDown
            // 
            this.btnQueueMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueMoveDown.Image = global::ifme.Properties.Resources.go_down;
            this.btnQueueMoveDown.Location = new System.Drawing.Point(534, 70);
            this.btnQueueMoveDown.Name = "btnQueueMoveDown";
            this.btnQueueMoveDown.Size = new System.Drawing.Size(32, 32);
            this.btnQueueMoveDown.TabIndex = 5;
            this.btnQueueMoveDown.UseVisualStyleBackColor = true;
            this.btnQueueMoveDown.Click += new System.EventHandler(this.btnQueueMoveDown_Click);
            // 
            // btnQueueStop
            // 
            this.btnQueueStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueStop.Image = global::ifme.Properties.Resources.media_playback_stop;
            this.btnQueueStop.Location = new System.Drawing.Point(580, 70);
            this.btnQueueStop.Name = "btnQueueStop";
            this.btnQueueStop.Size = new System.Drawing.Size(32, 32);
            this.btnQueueStop.TabIndex = 3;
            this.btnQueueStop.UseVisualStyleBackColor = true;
            this.btnQueueStop.Click += new System.EventHandler(this.btnQueueStop_Click);
            // 
            // btnQueuePause
            // 
            this.btnQueuePause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueuePause.Image = global::ifme.Properties.Resources.media_playback_pause;
            this.btnQueuePause.Location = new System.Drawing.Point(618, 70);
            this.btnQueuePause.Name = "btnQueuePause";
            this.btnQueuePause.Size = new System.Drawing.Size(32, 32);
            this.btnQueuePause.TabIndex = 2;
            this.btnQueuePause.UseVisualStyleBackColor = true;
            this.btnQueuePause.Click += new System.EventHandler(this.btnQueuePause_Click);
            // 
            // btnQueueStart
            // 
            this.btnQueueStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueueStart.Image = global::ifme.Properties.Resources.media_playback_start;
            this.btnQueueStart.Location = new System.Drawing.Point(656, 70);
            this.btnQueueStart.Name = "btnQueueStart";
            this.btnQueueStart.Size = new System.Drawing.Size(32, 32);
            this.btnQueueStart.TabIndex = 1;
            this.btnQueueStart.UseVisualStyleBackColor = true;
            this.btnQueueStart.Click += new System.EventHandler(this.btnQueueStart_Click);
            // 
            // colQueueTarget
            // 
            this.colQueueTarget.Tag = "colQueueTarget";
            this.colQueueTarget.Text = "Target";
            this.colQueueTarget.Width = 80;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(700, 700);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnBrowseOutput);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.lstQueue);
            this.Controls.Add(this.chkOutput);
            this.Controls.Add(this.btnProfileDelete);
            this.Controls.Add(this.btnProfileSave);
            this.Controls.Add(this.cboProfile);
            this.Controls.Add(this.lblProfile);
            this.Controls.Add(this.tabProp);
            this.Controls.Add(this.btnQueueAdd);
            this.Controls.Add(this.btnQueueRemove);
            this.Controls.Add(this.lblSpacer2);
            this.Controls.Add(this.btnQueueMoveUp);
            this.Controls.Add(this.btnQueueMoveDown);
            this.Controls.Add(this.lblSpacer1);
            this.Controls.Add(this.btnQueueStop);
            this.Controls.Add(this.btnQueuePause);
            this.Controls.Add(this.btnQueueStart);
            this.Controls.Add(this.chkDonePowerOff);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Internet Friendly Media Encoder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.cmsQueue.ResumeLayout(false);
            this.tabProp.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.grpPropFormat.ResumeLayout(false);
            this.grpPropFormat.PerformLayout();
            this.grpPropOutput.ResumeLayout(false);
            this.grpPropOutput.PerformLayout();
            this.grpPropInput.ResumeLayout(false);
            this.grpPropInput.PerformLayout();
            this.tabVideo.ResumeLayout(false);
            this.tabVideo.PerformLayout();
            this.grpVideoDeInterlace.ResumeLayout(false);
            this.grpVideoDeInterlace.PerformLayout();
            this.grpVideoCodec.ResumeLayout(false);
            this.grpVideoCodec.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoMultipass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudVideoRateFactor)).EndInit();
            this.grpVideoQuality.ResumeLayout(false);
            this.grpVideoQuality.PerformLayout();
            this.tabAudio.ResumeLayout(false);
            this.grpAudioCodec.ResumeLayout(false);
            this.grpAudioCodec.PerformLayout();
            this.tabSubtitle.ResumeLayout(false);
            this.tabAttachment.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxBanner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkDonePowerOff;
        private System.Windows.Forms.Button btnQueueStart;
        private System.Windows.Forms.Button btnQueuePause;
        private System.Windows.Forms.Button btnQueueStop;
        private System.Windows.Forms.Label lblSpacer1;
        private System.Windows.Forms.Button btnQueueMoveDown;
        private System.Windows.Forms.Button btnQueueMoveUp;
        private System.Windows.Forms.Label lblSpacer2;
        private System.Windows.Forms.Button btnQueueRemove;
        private System.Windows.Forms.Button btnQueueAdd;
        private System.Windows.Forms.ListView lstQueue;
        private System.Windows.Forms.ColumnHeader colQueueName;
        private System.Windows.Forms.ColumnHeader colQueueDuration;
        private System.Windows.Forms.ColumnHeader colQueueStatus;
        private System.Windows.Forms.TabControl tabProp;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.TabPage tabVideo;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.Button btnProfileSave;
        private System.Windows.Forms.Button btnProfileDelete;
        private System.Windows.Forms.CheckBox chkOutput;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.TabPage tabAudio;
        private System.Windows.Forms.TabPage tabSubtitle;
        private System.Windows.Forms.TabPage tabAttachment;
        private System.Windows.Forms.GroupBox grpVideoCodec;
        private System.Windows.Forms.GroupBox grpVideoQuality;
        private System.Windows.Forms.ComboBox cboVideoChroma;
        private System.Windows.Forms.Label lblVideoColourSpace;
        private System.Windows.Forms.ComboBox cboVideoBitDepth;
        private System.Windows.Forms.Label lblVideoBitDepth;
        private System.Windows.Forms.ComboBox cboVideoFrameRate;
        private System.Windows.Forms.Label lblVideoFrameRate;
        private System.Windows.Forms.ComboBox cboVideoResolution;
        private System.Windows.Forms.Label lblVideoResolution;
        private System.Windows.Forms.ComboBox cboVideoEncoder;
        private System.Windows.Forms.Label lblVideoEncoder;
        private System.Windows.Forms.ComboBox cboVideoEncodingType;
        private System.Windows.Forms.Label lblVideoPreset;
        private System.Windows.Forms.ComboBox cboVideoTune;
        private System.Windows.Forms.Label lblVideoTune;
        private System.Windows.Forms.ComboBox cboVideoPreset;
        private System.Windows.Forms.NumericUpDown nudVideoMultipass;
        private System.Windows.Forms.Label lblVideoMultipass;
        private System.Windows.Forms.Label lblVideoRateFactor;
        private System.Windows.Forms.NumericUpDown nudVideoRateFactor;
        private System.Windows.Forms.Label lblVideoRateControl;
        private System.Windows.Forms.GroupBox grpVideoDeInterlace;
        private System.Windows.Forms.ComboBox cboVideoDiField;
        private System.Windows.Forms.Label lblVideoDiField;
        private System.Windows.Forms.ComboBox cboVideoDiMode;
        private System.Windows.Forms.Label lblVideoDiMode;
        private System.Windows.Forms.CheckBox chkVideoDeinterlace;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbxBanner;
        private System.Windows.Forms.Label lblAudioEncoder;
        private System.Windows.Forms.ComboBox cboAudioEncoder;
        private System.Windows.Forms.Label lblAudioFreq;
        private System.Windows.Forms.ComboBox cboAudioQuality;
        private System.Windows.Forms.Label lblAudioQuality;
        private System.Windows.Forms.Label lblAudioChannel;
        private System.Windows.Forms.ComboBox cboAudioChannel;
        private System.Windows.Forms.ComboBox cboAudioFreq;
        private System.Windows.Forms.Button btnVideoArgEdit;
        private System.Windows.Forms.GroupBox grpAudioCodec;
        private System.Windows.Forms.Button btnAudioEditArg;
        private System.Windows.Forms.Button btnAudioRemove;
        private System.Windows.Forms.Button btnAudioAdd;
        private System.Windows.Forms.ListView lstAudio;
        private System.Windows.Forms.ColumnHeader colAudioId;
        private System.Windows.Forms.Label lblSpacer4;
        private System.Windows.Forms.Button btnAudioMoveDown;
        private System.Windows.Forms.Button btnAudioMoveUp;
        private System.Windows.Forms.ComboBox cboAudioMode;
        private System.Windows.Forms.Label lblAudioMode;
        private System.Windows.Forms.Button btnSubRemove;
        private System.Windows.Forms.Button btnSubAdd;
        private System.Windows.Forms.Button btnSubMoveDown;
        private System.Windows.Forms.Button btnSubMoveUp;
        private System.Windows.Forms.Label lblSpacer5;
        private System.Windows.Forms.ComboBox cboSubLang;
        private System.Windows.Forms.ListView lstSub;
        private System.Windows.Forms.ColumnHeader colSubId;
        private System.Windows.Forms.ColumnHeader colSubName;
        private System.Windows.Forms.ColumnHeader colSubLang;
        private System.Windows.Forms.Button btnAttachDel;
        private System.Windows.Forms.Button btnAttachAdd;
        private System.Windows.Forms.ListView lstAttach;
        private System.Windows.Forms.ColumnHeader colAttachName;
        private System.Windows.Forms.ColumnHeader colAttachMime;
        private System.Windows.Forms.GroupBox grpPropOutput;
        private System.Windows.Forms.GroupBox grpPropInput;
        private System.Windows.Forms.GroupBox grpPropFormat;
        private System.Windows.Forms.RadioButton rdoMKV;
        private System.Windows.Forms.RadioButton rdoMP4;
        private System.Windows.Forms.ContextMenuStrip cmsQueue;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueNew;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueDel;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueSelectAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueSelectNone;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueSelectInvert;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiAviSynth;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueAviSynthEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tsmiQueueAviSynthConvertTo;
        private System.Windows.Forms.TextBox txtOutputInfo;
        private System.Windows.Forms.TextBox txtSourceInfo;
        private System.Windows.Forms.ColumnHeader colAudioName;
        private System.Windows.Forms.ColumnHeader colQueueTarget;
    }
}

