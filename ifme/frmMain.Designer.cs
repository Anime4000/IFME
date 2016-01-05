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
			this.pbxLeft = new System.Windows.Forms.PictureBox();
			this.pbxRight = new System.Windows.Forms.PictureBox();
			this.btnQueueAdd = new System.Windows.Forms.Button();
			this.btnQueueRemove = new System.Windows.Forms.Button();
			this.btnQueueMoveUp = new System.Windows.Forms.Button();
			this.btnQueueMoveDown = new System.Windows.Forms.Button();
			this.btnQueueStart = new System.Windows.Forms.Button();
			this.btnQueueStop = new System.Windows.Forms.Button();
			this.lstQueue = new System.Windows.Forms.ListView();
			this.colQueueName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cmsQueueMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiQueuePreview = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiBenchmark = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiQueueNew = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiQueueOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiQueueSave = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiQueueSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiQueueDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiQueueSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiQueueSelectNone = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiQueueSelectInvert = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiQueueAviSynth = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiQueueAviSynthEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiQueueAviSynthGenerate = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiFFmpeg = new System.Windows.Forms.ToolStripMenuItem();
			this.btnQueuePause = new System.Windows.Forms.Button();
			this.tabConfig = new System.Windows.Forms.TabControl();
			this.tabPicture = new System.Windows.Forms.TabPage();
			this.chkPictureVideoCopy = new System.Windows.Forms.CheckBox();
			this.chkPictureYadif = new System.Windows.Forms.CheckBox();
			this.grpPictureYadif = new System.Windows.Forms.GroupBox();
			this.cboPictureYadifFlag = new System.Windows.Forms.ComboBox();
			this.lblPictureYadifFlag = new System.Windows.Forms.Label();
			this.cboPictureYadifField = new System.Windows.Forms.ComboBox();
			this.lblPictureYadifField = new System.Windows.Forms.Label();
			this.cboPictureYadifMode = new System.Windows.Forms.ComboBox();
			this.lblPictureYadifMode = new System.Windows.Forms.Label();
			this.grpPictureQuality = new System.Windows.Forms.GroupBox();
			this.cboPictureYuv = new System.Windows.Forms.ComboBox();
			this.lblPictureYuv = new System.Windows.Forms.Label();
			this.cboPictureBit = new System.Windows.Forms.ComboBox();
			this.lblPictureBit = new System.Windows.Forms.Label();
			this.grpPictureBasic = new System.Windows.Forms.GroupBox();
			this.lblPictureFps = new System.Windows.Forms.Label();
			this.cboPictureFps = new System.Windows.Forms.ComboBox();
			this.cboPictureRes = new System.Windows.Forms.ComboBox();
			this.lblPictureRes = new System.Windows.Forms.Label();
			this.grpPictureFormat = new System.Windows.Forms.GroupBox();
			this.rdoMP4 = new System.Windows.Forms.RadioButton();
			this.rdoMKV = new System.Windows.Forms.RadioButton();
			this.tabVideo = new System.Windows.Forms.TabPage();
			this.txtVideoCmd = new System.Windows.Forms.TextBox();
			this.lblVideoCmd = new System.Windows.Forms.Label();
			this.grpVideoRateCtrl = new System.Windows.Forms.GroupBox();
			this.lblVideoRateL = new System.Windows.Forms.Label();
			this.lblVideoRateH = new System.Windows.Forms.Label();
			this.trkVideoRate = new System.Windows.Forms.TrackBar();
			this.txtVideoValue = new System.Windows.Forms.TextBox();
			this.lblVideoRateValue = new System.Windows.Forms.Label();
			this.cboVideoType = new System.Windows.Forms.ComboBox();
			this.grpVideoBasic = new System.Windows.Forms.GroupBox();
			this.cboVideoTune = new System.Windows.Forms.ComboBox();
			this.lblVideoTune = new System.Windows.Forms.Label();
			this.cboVideoPreset = new System.Windows.Forms.ComboBox();
			this.lblVideoPreset = new System.Windows.Forms.Label();
			this.tabAudio = new System.Windows.Forms.TabPage();
			this.btnAudioRemove = new System.Windows.Forms.Button();
			this.btnAudioAdd = new System.Windows.Forms.Button();
			this.lblAudioChannel = new System.Windows.Forms.Label();
			this.cboAudioChannel = new System.Windows.Forms.ComboBox();
			this.chkAudioMerge = new System.Windows.Forms.CheckBox();
			this.clbAudioTracks = new System.Windows.Forms.CheckedListBox();
			this.lblAudioFreq = new System.Windows.Forms.Label();
			this.cboAudioFreq = new System.Windows.Forms.ComboBox();
			this.txtAudioCmd = new System.Windows.Forms.TextBox();
			this.lblAudioCmd = new System.Windows.Forms.Label();
			this.lblAudioBit = new System.Windows.Forms.Label();
			this.cboAudioBit = new System.Windows.Forms.ComboBox();
			this.cboAudioEncoder = new System.Windows.Forms.ComboBox();
			this.lblAudioEncoder = new System.Windows.Forms.Label();
			this.tabSubtitles = new System.Windows.Forms.TabPage();
			this.lblSubNote = new System.Windows.Forms.Label();
			this.cboSubLang = new System.Windows.Forms.ComboBox();
			this.lblSubLang = new System.Windows.Forms.Label();
			this.lstSub = new System.Windows.Forms.ListView();
			this.colSubFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSubLang = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnSubRemove = new System.Windows.Forms.Button();
			this.btnSubAdd = new System.Windows.Forms.Button();
			this.chkSubEnable = new System.Windows.Forms.CheckBox();
			this.tabAttachments = new System.Windows.Forms.TabPage();
			this.lblAttachNote = new System.Windows.Forms.Label();
			this.txtAttachDescription = new System.Windows.Forms.TextBox();
			this.lblAttachDescription = new System.Windows.Forms.Label();
			this.lstAttach = new System.Windows.Forms.ListView();
			this.colAttachName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAttachMime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAttachDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnAttachRemove = new System.Windows.Forms.Button();
			this.btnAttachAdd = new System.Windows.Forms.Button();
			this.chkAttachEnable = new System.Windows.Forms.CheckBox();
			this.tabLogs = new System.Windows.Forms.TabPage();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.lblProfiles = new System.Windows.Forms.Label();
			this.cboProfile = new System.Windows.Forms.ComboBox();
			this.btnProfileSave = new System.Windows.Forms.Button();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.sptVert1 = new System.Windows.Forms.Label();
			this.sptVert2 = new System.Windows.Forms.Label();
			this.chkShutdown = new System.Windows.Forms.CheckBox();
			this.sptVert3 = new System.Windows.Forms.Label();
			this.btnConfig = new System.Windows.Forms.Button();
			this.btnAbout = new System.Windows.Forms.Button();
			this.txtDestination = new System.Windows.Forms.TextBox();
			this.bgwEncoding = new System.ComponentModel.BackgroundWorker();
			this.tipUpdate = new System.Windows.Forms.ToolTip(this.components);
			this.btnDonate = new System.Windows.Forms.Button();
			this.tipNotify = new System.Windows.Forms.ToolTip(this.components);
			this.sptVert4 = new System.Windows.Forms.Label();
			this.chkDestination = new System.Windows.Forms.CheckBox();
			this.btnFacebook = new System.Windows.Forms.Button();
			this.btnProfileDelete = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pbxLeft)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbxRight)).BeginInit();
			this.cmsQueueMenu.SuspendLayout();
			this.tabConfig.SuspendLayout();
			this.tabPicture.SuspendLayout();
			this.grpPictureYadif.SuspendLayout();
			this.grpPictureQuality.SuspendLayout();
			this.grpPictureBasic.SuspendLayout();
			this.grpPictureFormat.SuspendLayout();
			this.tabVideo.SuspendLayout();
			this.grpVideoRateCtrl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkVideoRate)).BeginInit();
			this.grpVideoBasic.SuspendLayout();
			this.tabAudio.SuspendLayout();
			this.tabSubtitles.SuspendLayout();
			this.tabAttachments.SuspendLayout();
			this.tabLogs.SuspendLayout();
			this.SuspendLayout();
			// 
			// pbxLeft
			// 
			this.pbxLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pbxLeft.BackColor = System.Drawing.Color.Black;
			this.pbxLeft.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbxLeft.Location = new System.Drawing.Point(0, 0);
			this.pbxLeft.Name = "pbxLeft";
			this.pbxLeft.Size = new System.Drawing.Size(684, 64);
			this.pbxLeft.TabIndex = 0;
			this.pbxLeft.TabStop = false;
			this.pbxLeft.Click += new System.EventHandler(this.pbxLeft_Click);
			// 
			// pbxRight
			// 
			this.pbxRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pbxRight.BackColor = System.Drawing.Color.Transparent;
			this.pbxRight.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbxRight.Location = new System.Drawing.Point(60, 0);
			this.pbxRight.Name = "pbxRight";
			this.pbxRight.Size = new System.Drawing.Size(624, 64);
			this.pbxRight.TabIndex = 1;
			this.pbxRight.TabStop = false;
			this.pbxRight.Click += new System.EventHandler(this.pbxRight_Click);
			// 
			// btnQueueAdd
			// 
			this.btnQueueAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueAdd.Image = global::ifme.Properties.Resources.film_add;
			this.btnQueueAdd.Location = new System.Drawing.Point(482, 70);
			this.btnQueueAdd.Name = "btnQueueAdd";
			this.btnQueueAdd.Size = new System.Drawing.Size(24, 24);
			this.btnQueueAdd.TabIndex = 4;
			this.btnQueueAdd.UseVisualStyleBackColor = true;
			this.btnQueueAdd.Click += new System.EventHandler(this.btnQueueAdd_Click);
			// 
			// btnQueueRemove
			// 
			this.btnQueueRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueRemove.Image = global::ifme.Properties.Resources.film_delete;
			this.btnQueueRemove.Location = new System.Drawing.Point(512, 70);
			this.btnQueueRemove.Name = "btnQueueRemove";
			this.btnQueueRemove.Size = new System.Drawing.Size(24, 24);
			this.btnQueueRemove.TabIndex = 5;
			this.btnQueueRemove.UseVisualStyleBackColor = true;
			this.btnQueueRemove.Click += new System.EventHandler(this.btnQueueRemove_Click);
			// 
			// btnQueueMoveUp
			// 
			this.btnQueueMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueMoveUp.Image = global::ifme.Properties.Resources.arw_up;
			this.btnQueueMoveUp.Location = new System.Drawing.Point(550, 70);
			this.btnQueueMoveUp.Name = "btnQueueMoveUp";
			this.btnQueueMoveUp.Size = new System.Drawing.Size(24, 24);
			this.btnQueueMoveUp.TabIndex = 7;
			this.btnQueueMoveUp.UseVisualStyleBackColor = true;
			this.btnQueueMoveUp.Click += new System.EventHandler(this.btnQueueMoveUp_Click);
			// 
			// btnQueueMoveDown
			// 
			this.btnQueueMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueMoveDown.Image = global::ifme.Properties.Resources.arw_dn;
			this.btnQueueMoveDown.Location = new System.Drawing.Point(580, 70);
			this.btnQueueMoveDown.Name = "btnQueueMoveDown";
			this.btnQueueMoveDown.Size = new System.Drawing.Size(24, 24);
			this.btnQueueMoveDown.TabIndex = 8;
			this.btnQueueMoveDown.UseVisualStyleBackColor = true;
			this.btnQueueMoveDown.Click += new System.EventHandler(this.btnQueueMoveDown_Click);
			// 
			// btnQueueStart
			// 
			this.btnQueueStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueStart.Image = global::ifme.Properties.Resources.control_play_blue;
			this.btnQueueStart.Location = new System.Drawing.Point(648, 70);
			this.btnQueueStart.Name = "btnQueueStart";
			this.btnQueueStart.Size = new System.Drawing.Size(24, 24);
			this.btnQueueStart.TabIndex = 11;
			this.btnQueueStart.UseVisualStyleBackColor = true;
			this.btnQueueStart.Click += new System.EventHandler(this.btnQueueStart_Click);
			// 
			// btnQueueStop
			// 
			this.btnQueueStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueStop.Image = global::ifme.Properties.Resources.control_stop_blue;
			this.btnQueueStop.Location = new System.Drawing.Point(618, 70);
			this.btnQueueStop.Name = "btnQueueStop";
			this.btnQueueStop.Size = new System.Drawing.Size(24, 24);
			this.btnQueueStop.TabIndex = 10;
			this.btnQueueStop.UseVisualStyleBackColor = true;
			this.btnQueueStop.Click += new System.EventHandler(this.btnQueueStop_Click);
			// 
			// lstQueue
			// 
			this.lstQueue.AllowDrop = true;
			this.lstQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstQueue.CheckBoxes = true;
			this.lstQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colQueueName,
            this.colQueueSize,
            this.colQueueType,
            this.colQueueTo,
            this.colQueueStatus});
			this.lstQueue.ContextMenuStrip = this.cmsQueueMenu;
			this.lstQueue.FullRowSelect = true;
			this.lstQueue.HideSelection = false;
			this.lstQueue.Location = new System.Drawing.Point(12, 100);
			this.lstQueue.Name = "lstQueue";
			this.lstQueue.Size = new System.Drawing.Size(660, 234);
			this.lstQueue.TabIndex = 13;
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
			this.colQueueName.Width = 296;
			// 
			// colQueueSize
			// 
			this.colQueueSize.Tag = "colQueueSize";
			this.colQueueSize.Text = "Size";
			// 
			// colQueueType
			// 
			this.colQueueType.Tag = "colQueueType";
			this.colQueueType.Text = "Type";
			this.colQueueType.Width = 100;
			// 
			// colQueueTo
			// 
			this.colQueueTo.Tag = "colQueueTo";
			this.colQueueTo.Text = "To";
			this.colQueueTo.Width = 100;
			// 
			// colQueueStatus
			// 
			this.colQueueStatus.Tag = "colQueueStatus";
			this.colQueueStatus.Text = "Status";
			this.colQueueStatus.Width = 100;
			// 
			// cmsQueueMenu
			// 
			this.cmsQueueMenu.Font = new System.Drawing.Font("Tahoma", 8F);
			this.cmsQueueMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiQueuePreview,
            this.tsmiBenchmark,
            this.toolStripSeparator1,
            this.tsmiQueueNew,
            this.tsmiQueueOpen,
            this.tsmiQueueSave,
            this.tsmiQueueSaveAs,
            this.toolStripSeparator4,
            this.tsmiQueueDelete,
            this.tsmiQueueSelectAll,
            this.tsmiQueueSelectNone,
            this.tsmiQueueSelectInvert,
            this.toolStripSeparator2,
            this.tsmiQueueAviSynth,
            this.tsmiFFmpeg});
			this.cmsQueueMenu.Name = "cmsQueueMenu";
			this.cmsQueueMenu.Size = new System.Drawing.Size(226, 286);
			// 
			// tsmiQueuePreview
			// 
			this.tsmiQueuePreview.Image = global::ifme.Properties.Resources.magnifier;
			this.tsmiQueuePreview.Name = "tsmiQueuePreview";
			this.tsmiQueuePreview.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.tsmiQueuePreview.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueuePreview.Text = "&Preview";
			this.tsmiQueuePreview.Click += new System.EventHandler(this.tsmiQueuePreview_Click);
			// 
			// tsmiBenchmark
			// 
			this.tsmiBenchmark.Image = global::ifme.Properties.Resources.server_lightning;
			this.tsmiBenchmark.Name = "tsmiBenchmark";
			this.tsmiBenchmark.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.tsmiBenchmark.Size = new System.Drawing.Size(225, 22);
			this.tsmiBenchmark.Text = "&Benchmark";
			this.tsmiBenchmark.Click += new System.EventHandler(this.tsmiBenchmark_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
			// 
			// tsmiQueueNew
			// 
			this.tsmiQueueNew.Image = global::ifme.Properties.Resources.newstuff;
			this.tsmiQueueNew.Name = "tsmiQueueNew";
			this.tsmiQueueNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.tsmiQueueNew.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueNew.Text = "&New queue";
			this.tsmiQueueNew.Click += new System.EventHandler(this.tsmiQueueNew_Click);
			// 
			// tsmiQueueOpen
			// 
			this.tsmiQueueOpen.Image = global::ifme.Properties.Resources.page;
			this.tsmiQueueOpen.Name = "tsmiQueueOpen";
			this.tsmiQueueOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.tsmiQueueOpen.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueOpen.Text = "&Open queue";
			this.tsmiQueueOpen.Click += new System.EventHandler(this.tsmiQueueOpen_Click);
			// 
			// tsmiQueueSave
			// 
			this.tsmiQueueSave.Image = global::ifme.Properties.Resources.page_save;
			this.tsmiQueueSave.Name = "tsmiQueueSave";
			this.tsmiQueueSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.tsmiQueueSave.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueSave.Text = "&Save queue";
			this.tsmiQueueSave.Click += new System.EventHandler(this.tsmiQueueSave_Click);
			// 
			// tsmiQueueSaveAs
			// 
			this.tsmiQueueSaveAs.Image = global::ifme.Properties.Resources.asterisk_orange;
			this.tsmiQueueSaveAs.Name = "tsmiQueueSaveAs";
			this.tsmiQueueSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.tsmiQueueSaveAs.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueSaveAs.Text = "Save queue as...";
			this.tsmiQueueSaveAs.Click += new System.EventHandler(this.tsmiQueueSaveAs_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(222, 6);
			// 
			// tsmiQueueDelete
			// 
			this.tsmiQueueDelete.Name = "tsmiQueueDelete";
			this.tsmiQueueDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.tsmiQueueDelete.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueDelete.Text = "&Delete";
			this.tsmiQueueDelete.Click += new System.EventHandler(this.tsmiQueueDelete_Click);
			// 
			// tsmiQueueSelectAll
			// 
			this.tsmiQueueSelectAll.Name = "tsmiQueueSelectAll";
			this.tsmiQueueSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.tsmiQueueSelectAll.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueSelectAll.Text = "Select &all";
			this.tsmiQueueSelectAll.Click += new System.EventHandler(this.tsmiQueueSelectAll_Click);
			// 
			// tsmiQueueSelectNone
			// 
			this.tsmiQueueSelectNone.Name = "tsmiQueueSelectNone";
			this.tsmiQueueSelectNone.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.tsmiQueueSelectNone.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueSelectNone.Text = "S&elect none";
			this.tsmiQueueSelectNone.Click += new System.EventHandler(this.tsmiQueueSelectNone_Click);
			// 
			// tsmiQueueSelectInvert
			// 
			this.tsmiQueueSelectInvert.Name = "tsmiQueueSelectInvert";
			this.tsmiQueueSelectInvert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.tsmiQueueSelectInvert.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueSelectInvert.Text = "&Invert selection";
			this.tsmiQueueSelectInvert.Click += new System.EventHandler(this.tsmiQueueSelectInvert_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
			// 
			// tsmiQueueAviSynth
			// 
			this.tsmiQueueAviSynth.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiQueueAviSynthEdit,
            this.toolStripSeparator3,
            this.tsmiQueueAviSynthGenerate});
			this.tsmiQueueAviSynth.Image = global::ifme.Properties.Resources.plugin;
			this.tsmiQueueAviSynth.Name = "tsmiQueueAviSynth";
			this.tsmiQueueAviSynth.Size = new System.Drawing.Size(225, 22);
			this.tsmiQueueAviSynth.Text = "A&viSynth";
			// 
			// tsmiQueueAviSynthEdit
			// 
			this.tsmiQueueAviSynthEdit.Image = global::ifme.Properties.Resources.script_edit;
			this.tsmiQueueAviSynthEdit.Name = "tsmiQueueAviSynthEdit";
			this.tsmiQueueAviSynthEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.tsmiQueueAviSynthEdit.Size = new System.Drawing.Size(265, 22);
			this.tsmiQueueAviSynthEdit.Text = "&Edit script";
			this.tsmiQueueAviSynthEdit.Click += new System.EventHandler(this.tsmiQueueAviSynthEdit_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(262, 6);
			// 
			// tsmiQueueAviSynthGenerate
			// 
			this.tsmiQueueAviSynthGenerate.Image = global::ifme.Properties.Resources.script_add;
			this.tsmiQueueAviSynthGenerate.Name = "tsmiQueueAviSynthGenerate";
			this.tsmiQueueAviSynthGenerate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.tsmiQueueAviSynthGenerate.Size = new System.Drawing.Size(265, 22);
			this.tsmiQueueAviSynthGenerate.Text = "Generate &simple AviSynth script";
			this.tsmiQueueAviSynthGenerate.Click += new System.EventHandler(this.tsmiQueueAviSynthGenerate_Click);
			// 
			// tsmiFFmpeg
			// 
			this.tsmiFFmpeg.Image = global::ifme.Properties.Resources.application_xp_terminal;
			this.tsmiFFmpeg.Name = "tsmiFFmpeg";
			this.tsmiFFmpeg.Size = new System.Drawing.Size(225, 22);
			this.tsmiFFmpeg.Text = "&FFmpeg";
			this.tsmiFFmpeg.Click += new System.EventHandler(this.tsmiFFmpeg_Click);
			// 
			// btnQueuePause
			// 
			this.btnQueuePause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueuePause.Image = global::ifme.Properties.Resources.control_pause_blue;
			this.btnQueuePause.Location = new System.Drawing.Point(648, 70);
			this.btnQueuePause.Name = "btnQueuePause";
			this.btnQueuePause.Size = new System.Drawing.Size(24, 24);
			this.btnQueuePause.TabIndex = 12;
			this.btnQueuePause.UseVisualStyleBackColor = true;
			this.btnQueuePause.Visible = false;
			this.btnQueuePause.Click += new System.EventHandler(this.btnQueuePause_Click);
			// 
			// tabConfig
			// 
			this.tabConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabConfig.Controls.Add(this.tabPicture);
			this.tabConfig.Controls.Add(this.tabVideo);
			this.tabConfig.Controls.Add(this.tabAudio);
			this.tabConfig.Controls.Add(this.tabSubtitles);
			this.tabConfig.Controls.Add(this.tabAttachments);
			this.tabConfig.Controls.Add(this.tabLogs);
			this.tabConfig.Location = new System.Drawing.Point(12, 340);
			this.tabConfig.Name = "tabConfig";
			this.tabConfig.SelectedIndex = 0;
			this.tabConfig.Size = new System.Drawing.Size(660, 250);
			this.tabConfig.TabIndex = 14;
			// 
			// tabPicture
			// 
			this.tabPicture.Controls.Add(this.chkPictureVideoCopy);
			this.tabPicture.Controls.Add(this.chkPictureYadif);
			this.tabPicture.Controls.Add(this.grpPictureYadif);
			this.tabPicture.Controls.Add(this.grpPictureQuality);
			this.tabPicture.Controls.Add(this.grpPictureBasic);
			this.tabPicture.Controls.Add(this.grpPictureFormat);
			this.tabPicture.Location = new System.Drawing.Point(4, 22);
			this.tabPicture.Name = "tabPicture";
			this.tabPicture.Padding = new System.Windows.Forms.Padding(3);
			this.tabPicture.Size = new System.Drawing.Size(652, 224);
			this.tabPicture.TabIndex = 0;
			this.tabPicture.Text = "Picture";
			this.tabPicture.UseVisualStyleBackColor = true;
			// 
			// chkPictureVideoCopy
			// 
			this.chkPictureVideoCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chkPictureVideoCopy.AutoSize = true;
			this.chkPictureVideoCopy.Enabled = false;
			this.chkPictureVideoCopy.Location = new System.Drawing.Point(329, 201);
			this.chkPictureVideoCopy.Name = "chkPictureVideoCopy";
			this.chkPictureVideoCopy.Size = new System.Drawing.Size(195, 17);
			this.chkPictureVideoCopy.TabIndex = 5;
			this.chkPictureVideoCopy.Text = "&Video passthrough (do not encode)";
			this.chkPictureVideoCopy.UseVisualStyleBackColor = true;
			this.chkPictureVideoCopy.CheckedChanged += new System.EventHandler(this.chkPictureVideoCopy_CheckedChanged);
			// 
			// chkPictureYadif
			// 
			this.chkPictureYadif.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.chkPictureYadif.AutoSize = true;
			this.chkPictureYadif.Location = new System.Drawing.Point(336, 5);
			this.chkPictureYadif.Name = "chkPictureYadif";
			this.chkPictureYadif.Size = new System.Drawing.Size(119, 17);
			this.chkPictureYadif.TabIndex = 3;
			this.chkPictureYadif.Text = "De-interlace (&yadif)";
			this.chkPictureYadif.UseVisualStyleBackColor = true;
			this.chkPictureYadif.CheckedChanged += new System.EventHandler(this.chkPictureYadif_CheckedChanged);
			// 
			// grpPictureYadif
			// 
			this.grpPictureYadif.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpPictureYadif.Controls.Add(this.cboPictureYadifFlag);
			this.grpPictureYadif.Controls.Add(this.lblPictureYadifFlag);
			this.grpPictureYadif.Controls.Add(this.cboPictureYadifField);
			this.grpPictureYadif.Controls.Add(this.lblPictureYadifField);
			this.grpPictureYadif.Controls.Add(this.cboPictureYadifMode);
			this.grpPictureYadif.Controls.Add(this.lblPictureYadifMode);
			this.grpPictureYadif.Enabled = false;
			this.grpPictureYadif.Location = new System.Drawing.Point(329, 6);
			this.grpPictureYadif.Name = "grpPictureYadif";
			this.grpPictureYadif.Size = new System.Drawing.Size(317, 189);
			this.grpPictureYadif.TabIndex = 4;
			this.grpPictureYadif.TabStop = false;
			// 
			// cboPictureYadifFlag
			// 
			this.cboPictureYadifFlag.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPictureYadifFlag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPictureYadifFlag.FormattingEnabled = true;
			this.cboPictureYadifFlag.Items.AddRange(new object[] {
            "Deinterlace all frames",
            "Deinterlace marked frames"});
			this.cboPictureYadifFlag.Location = new System.Drawing.Point(55, 133);
			this.cboPictureYadifFlag.Name = "cboPictureYadifFlag";
			this.cboPictureYadifFlag.Size = new System.Drawing.Size(210, 21);
			this.cboPictureYadifFlag.TabIndex = 5;
			this.cboPictureYadifFlag.SelectedIndexChanged += new System.EventHandler(this.cboPictureYadifFlag_SelectedIndexChanged);
			// 
			// lblPictureYadifFlag
			// 
			this.lblPictureYadifFlag.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblPictureYadifFlag.AutoSize = true;
			this.lblPictureYadifFlag.Location = new System.Drawing.Point(52, 117);
			this.lblPictureYadifFlag.Name = "lblPictureYadifFlag";
			this.lblPictureYadifFlag.Size = new System.Drawing.Size(31, 13);
			this.lblPictureYadifFlag.TabIndex = 4;
			this.lblPictureYadifFlag.Text = "Fl&ag:";
			// 
			// cboPictureYadifField
			// 
			this.cboPictureYadifField.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPictureYadifField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPictureYadifField.FormattingEnabled = true;
			this.cboPictureYadifField.Items.AddRange(new object[] {
            "Top Field First",
            "Bottom Field First"});
			this.cboPictureYadifField.Location = new System.Drawing.Point(55, 93);
			this.cboPictureYadifField.Name = "cboPictureYadifField";
			this.cboPictureYadifField.Size = new System.Drawing.Size(210, 21);
			this.cboPictureYadifField.TabIndex = 3;
			this.cboPictureYadifField.SelectedIndexChanged += new System.EventHandler(this.cboPictureYadifField_SelectedIndexChanged);
			// 
			// lblPictureYadifField
			// 
			this.lblPictureYadifField.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblPictureYadifField.AutoSize = true;
			this.lblPictureYadifField.Location = new System.Drawing.Point(52, 77);
			this.lblPictureYadifField.Name = "lblPictureYadifField";
			this.lblPictureYadifField.Size = new System.Drawing.Size(33, 13);
			this.lblPictureYadifField.TabIndex = 2;
			this.lblPictureYadifField.Text = "Fi&eld:";
			// 
			// cboPictureYadifMode
			// 
			this.cboPictureYadifMode.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPictureYadifMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPictureYadifMode.FormattingEnabled = true;
			this.cboPictureYadifMode.Items.AddRange(new object[] {
            "Deinterlace only frame",
            "Deinterlace each field",
            "Skips spatial interlacing frame check",
            "Skips spatial interlacing field check"});
			this.cboPictureYadifMode.Location = new System.Drawing.Point(55, 53);
			this.cboPictureYadifMode.Name = "cboPictureYadifMode";
			this.cboPictureYadifMode.Size = new System.Drawing.Size(210, 21);
			this.cboPictureYadifMode.TabIndex = 1;
			this.cboPictureYadifMode.SelectedIndexChanged += new System.EventHandler(this.cboPictureYadifMode_SelectedIndexChanged);
			// 
			// lblPictureYadifMode
			// 
			this.lblPictureYadifMode.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblPictureYadifMode.AutoSize = true;
			this.lblPictureYadifMode.Location = new System.Drawing.Point(52, 37);
			this.lblPictureYadifMode.Name = "lblPictureYadifMode";
			this.lblPictureYadifMode.Size = new System.Drawing.Size(37, 13);
			this.lblPictureYadifMode.TabIndex = 0;
			this.lblPictureYadifMode.Text = "&Mode:";
			// 
			// grpPictureQuality
			// 
			this.grpPictureQuality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.grpPictureQuality.Controls.Add(this.cboPictureYuv);
			this.grpPictureQuality.Controls.Add(this.lblPictureYuv);
			this.grpPictureQuality.Controls.Add(this.cboPictureBit);
			this.grpPictureQuality.Controls.Add(this.lblPictureBit);
			this.grpPictureQuality.Location = new System.Drawing.Point(6, 143);
			this.grpPictureQuality.Name = "grpPictureQuality";
			this.grpPictureQuality.Size = new System.Drawing.Size(317, 75);
			this.grpPictureQuality.TabIndex = 2;
			this.grpPictureQuality.TabStop = false;
			this.grpPictureQuality.Text = "&Quality";
			// 
			// cboPictureYuv
			// 
			this.cboPictureYuv.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPictureYuv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPictureYuv.FormattingEnabled = true;
			this.cboPictureYuv.Items.AddRange(new object[] {
            "420",
            "422",
            "444"});
			this.cboPictureYuv.Location = new System.Drawing.Point(163, 35);
			this.cboPictureYuv.Name = "cboPictureYuv";
			this.cboPictureYuv.Size = new System.Drawing.Size(121, 21);
			this.cboPictureYuv.TabIndex = 3;
			this.cboPictureYuv.SelectedIndexChanged += new System.EventHandler(this.cboPictureYuv_SelectedIndexChanged);
			// 
			// lblPictureYuv
			// 
			this.lblPictureYuv.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblPictureYuv.AutoSize = true;
			this.lblPictureYuv.Location = new System.Drawing.Point(160, 19);
			this.lblPictureYuv.Name = "lblPictureYuv";
			this.lblPictureYuv.Size = new System.Drawing.Size(109, 13);
			this.lblPictureYuv.TabIndex = 2;
			this.lblPictureYuv.Text = "&Chroma subsampling:";
			// 
			// cboPictureBit
			// 
			this.cboPictureBit.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPictureBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPictureBit.FormattingEnabled = true;
			this.cboPictureBit.Items.AddRange(new object[] {
            "8",
            "10"});
			this.cboPictureBit.Location = new System.Drawing.Point(36, 35);
			this.cboPictureBit.Name = "cboPictureBit";
			this.cboPictureBit.Size = new System.Drawing.Size(121, 21);
			this.cboPictureBit.TabIndex = 1;
			this.cboPictureBit.SelectedIndexChanged += new System.EventHandler(this.cboPictureBit_SelectedIndexChanged);
			// 
			// lblPictureBit
			// 
			this.lblPictureBit.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblPictureBit.AutoSize = true;
			this.lblPictureBit.Location = new System.Drawing.Point(33, 19);
			this.lblPictureBit.Name = "lblPictureBit";
			this.lblPictureBit.Size = new System.Drawing.Size(54, 13);
			this.lblPictureBit.TabIndex = 0;
			this.lblPictureBit.Text = "&Bit depth:";
			// 
			// grpPictureBasic
			// 
			this.grpPictureBasic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.grpPictureBasic.Controls.Add(this.lblPictureFps);
			this.grpPictureBasic.Controls.Add(this.cboPictureFps);
			this.grpPictureBasic.Controls.Add(this.cboPictureRes);
			this.grpPictureBasic.Controls.Add(this.lblPictureRes);
			this.grpPictureBasic.Location = new System.Drawing.Point(6, 62);
			this.grpPictureBasic.Name = "grpPictureBasic";
			this.grpPictureBasic.Size = new System.Drawing.Size(317, 75);
			this.grpPictureBasic.TabIndex = 1;
			this.grpPictureBasic.TabStop = false;
			this.grpPictureBasic.Text = "Ba&sic";
			// 
			// lblPictureFps
			// 
			this.lblPictureFps.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblPictureFps.AutoSize = true;
			this.lblPictureFps.Location = new System.Drawing.Point(160, 19);
			this.lblPictureFps.Name = "lblPictureFps";
			this.lblPictureFps.Size = new System.Drawing.Size(64, 13);
			this.lblPictureFps.TabIndex = 2;
			this.lblPictureFps.Text = "&Frame rate:";
			// 
			// cboPictureFps
			// 
			this.cboPictureFps.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPictureFps.FormattingEnabled = true;
			this.cboPictureFps.Items.AddRange(new object[] {
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
			this.cboPictureFps.Location = new System.Drawing.Point(163, 35);
			this.cboPictureFps.Name = "cboPictureFps";
			this.cboPictureFps.Size = new System.Drawing.Size(121, 21);
			this.cboPictureFps.TabIndex = 3;
			this.cboPictureFps.TextChanged += new System.EventHandler(this.cboPictureFps_TextChanged);
			this.cboPictureFps.Leave += new System.EventHandler(this.cboPictureFps_Leave);
			// 
			// cboPictureRes
			// 
			this.cboPictureRes.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboPictureRes.FormattingEnabled = true;
			this.cboPictureRes.Items.AddRange(new object[] {
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
			this.cboPictureRes.Location = new System.Drawing.Point(36, 35);
			this.cboPictureRes.Name = "cboPictureRes";
			this.cboPictureRes.Size = new System.Drawing.Size(121, 21);
			this.cboPictureRes.TabIndex = 1;
			this.cboPictureRes.TextChanged += new System.EventHandler(this.cboPictureRes_TextChanged);
			this.cboPictureRes.Leave += new System.EventHandler(this.cboPictureRes_Leave);
			// 
			// lblPictureRes
			// 
			this.lblPictureRes.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblPictureRes.AutoSize = true;
			this.lblPictureRes.Location = new System.Drawing.Point(33, 19);
			this.lblPictureRes.Name = "lblPictureRes";
			this.lblPictureRes.Size = new System.Drawing.Size(61, 13);
			this.lblPictureRes.TabIndex = 0;
			this.lblPictureRes.Text = "&Resolution:";
			// 
			// grpPictureFormat
			// 
			this.grpPictureFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.grpPictureFormat.Controls.Add(this.rdoMP4);
			this.grpPictureFormat.Controls.Add(this.rdoMKV);
			this.grpPictureFormat.Location = new System.Drawing.Point(6, 6);
			this.grpPictureFormat.Name = "grpPictureFormat";
			this.grpPictureFormat.Size = new System.Drawing.Size(317, 50);
			this.grpPictureFormat.TabIndex = 0;
			this.grpPictureFormat.TabStop = false;
			this.grpPictureFormat.Text = "F&ormat";
			// 
			// rdoMP4
			// 
			this.rdoMP4.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.rdoMP4.AutoSize = true;
			this.rdoMP4.Location = new System.Drawing.Point(161, 19);
			this.rdoMP4.Name = "rdoMP4";
			this.rdoMP4.Size = new System.Drawing.Size(45, 17);
			this.rdoMP4.TabIndex = 1;
			this.rdoMP4.TabStop = true;
			this.rdoMP4.Text = "MP&4";
			this.rdoMP4.UseVisualStyleBackColor = true;
			this.rdoMP4.CheckedChanged += new System.EventHandler(this.rdoMP4_CheckedChanged);
			// 
			// rdoMKV
			// 
			this.rdoMKV.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.rdoMKV.AutoSize = true;
			this.rdoMKV.Location = new System.Drawing.Point(110, 19);
			this.rdoMKV.Name = "rdoMKV";
			this.rdoMKV.Size = new System.Drawing.Size(45, 17);
			this.rdoMKV.TabIndex = 0;
			this.rdoMKV.TabStop = true;
			this.rdoMKV.Text = "M&KV";
			this.rdoMKV.UseVisualStyleBackColor = true;
			this.rdoMKV.CheckedChanged += new System.EventHandler(this.rdoMKV_CheckedChanged);
			// 
			// tabVideo
			// 
			this.tabVideo.Controls.Add(this.txtVideoCmd);
			this.tabVideo.Controls.Add(this.lblVideoCmd);
			this.tabVideo.Controls.Add(this.grpVideoRateCtrl);
			this.tabVideo.Controls.Add(this.grpVideoBasic);
			this.tabVideo.Location = new System.Drawing.Point(4, 22);
			this.tabVideo.Name = "tabVideo";
			this.tabVideo.Padding = new System.Windows.Forms.Padding(3);
			this.tabVideo.Size = new System.Drawing.Size(652, 224);
			this.tabVideo.TabIndex = 1;
			this.tabVideo.Text = "Video";
			this.tabVideo.UseVisualStyleBackColor = true;
			// 
			// txtVideoCmd
			// 
			this.txtVideoCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtVideoCmd.Location = new System.Drawing.Point(6, 198);
			this.txtVideoCmd.Name = "txtVideoCmd";
			this.txtVideoCmd.Size = new System.Drawing.Size(640, 20);
			this.txtVideoCmd.TabIndex = 3;
			this.txtVideoCmd.TextChanged += new System.EventHandler(this.txtVideoCmd_TextChanged);
			// 
			// lblVideoCmd
			// 
			this.lblVideoCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblVideoCmd.AutoSize = true;
			this.lblVideoCmd.Location = new System.Drawing.Point(6, 182);
			this.lblVideoCmd.Name = "lblVideoCmd";
			this.lblVideoCmd.Size = new System.Drawing.Size(105, 13);
			this.lblVideoCmd.TabIndex = 2;
			this.lblVideoCmd.Text = "&Extra command-line:";
			// 
			// grpVideoRateCtrl
			// 
			this.grpVideoRateCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpVideoRateCtrl.Controls.Add(this.lblVideoRateL);
			this.grpVideoRateCtrl.Controls.Add(this.lblVideoRateH);
			this.grpVideoRateCtrl.Controls.Add(this.trkVideoRate);
			this.grpVideoRateCtrl.Controls.Add(this.txtVideoValue);
			this.grpVideoRateCtrl.Controls.Add(this.lblVideoRateValue);
			this.grpVideoRateCtrl.Controls.Add(this.cboVideoType);
			this.grpVideoRateCtrl.Location = new System.Drawing.Point(329, 6);
			this.grpVideoRateCtrl.Name = "grpVideoRateCtrl";
			this.grpVideoRateCtrl.Size = new System.Drawing.Size(317, 173);
			this.grpVideoRateCtrl.TabIndex = 1;
			this.grpVideoRateCtrl.TabStop = false;
			this.grpVideoRateCtrl.Text = "&Rate control";
			// 
			// lblVideoRateL
			// 
			this.lblVideoRateL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblVideoRateL.Location = new System.Drawing.Point(162, 130);
			this.lblVideoRateL.Name = "lblVideoRateL";
			this.lblVideoRateL.Size = new System.Drawing.Size(149, 40);
			this.lblVideoRateL.TabIndex = 5;
			this.lblVideoRateL.Text = "Low quality\r\nSmall file size";
			this.lblVideoRateL.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblVideoRateH
			// 
			this.lblVideoRateH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.lblVideoRateH.Location = new System.Drawing.Point(6, 130);
			this.lblVideoRateH.Name = "lblVideoRateH";
			this.lblVideoRateH.Size = new System.Drawing.Size(150, 40);
			this.lblVideoRateH.TabIndex = 4;
			this.lblVideoRateH.Text = "High quality\r\nHuge file size";
			// 
			// trkVideoRate
			// 
			this.trkVideoRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trkVideoRate.BackColor = System.Drawing.Color.White;
			this.trkVideoRate.Location = new System.Drawing.Point(6, 82);
			this.trkVideoRate.Maximum = 510;
			this.trkVideoRate.Name = "trkVideoRate";
			this.trkVideoRate.Size = new System.Drawing.Size(305, 45);
			this.trkVideoRate.TabIndex = 3;
			this.trkVideoRate.TickFrequency = 10;
			this.trkVideoRate.Value = 260;
			this.trkVideoRate.ValueChanged += new System.EventHandler(this.trkVideoRate_ValueChanged);
			// 
			// txtVideoValue
			// 
			this.txtVideoValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtVideoValue.Location = new System.Drawing.Point(211, 56);
			this.txtVideoValue.Name = "txtVideoValue";
			this.txtVideoValue.Size = new System.Drawing.Size(100, 20);
			this.txtVideoValue.TabIndex = 2;
			this.txtVideoValue.TextChanged += new System.EventHandler(this.txtVideoValue_TextChanged);
			this.txtVideoValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVideoValue_KeyPress);
			this.txtVideoValue.Leave += new System.EventHandler(this.txtVideoValue_Leave);
			// 
			// lblVideoRateValue
			// 
			this.lblVideoRateValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblVideoRateValue.Location = new System.Drawing.Point(6, 56);
			this.lblVideoRateValue.Name = "lblVideoRateValue";
			this.lblVideoRateValue.Size = new System.Drawing.Size(196, 20);
			this.lblVideoRateValue.TabIndex = 1;
			this.lblVideoRateValue.Text = "Rate&factor:";
			this.lblVideoRateValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cboVideoType
			// 
			this.cboVideoType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboVideoType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboVideoType.FormattingEnabled = true;
			this.cboVideoType.Items.AddRange(new object[] {
            "Single pass, Ratefactor-based",
            "Single pass, Quantizer-based",
            "Single pass, Bitrate-based",
            "Multipass, 2 pass",
            "Multipass, 3 pass",
            "Multipass, 4 pass",
            "Multipass, 5 pass",
            "Multipass, 6 pass",
            "Multipass, 7 pass",
            "Multipass, 8 pass",
            "Multipass, 9 pass"});
			this.cboVideoType.Location = new System.Drawing.Point(6, 29);
			this.cboVideoType.Name = "cboVideoType";
			this.cboVideoType.Size = new System.Drawing.Size(305, 21);
			this.cboVideoType.TabIndex = 0;
			this.cboVideoType.SelectedIndexChanged += new System.EventHandler(this.cboVideoType_SelectedIndexChanged);
			// 
			// grpVideoBasic
			// 
			this.grpVideoBasic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.grpVideoBasic.Controls.Add(this.cboVideoTune);
			this.grpVideoBasic.Controls.Add(this.lblVideoTune);
			this.grpVideoBasic.Controls.Add(this.cboVideoPreset);
			this.grpVideoBasic.Controls.Add(this.lblVideoPreset);
			this.grpVideoBasic.Location = new System.Drawing.Point(6, 6);
			this.grpVideoBasic.Name = "grpVideoBasic";
			this.grpVideoBasic.Size = new System.Drawing.Size(317, 173);
			this.grpVideoBasic.TabIndex = 0;
			this.grpVideoBasic.TabStop = false;
			this.grpVideoBasic.Text = "&Basic";
			// 
			// cboVideoTune
			// 
			this.cboVideoTune.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboVideoTune.FormattingEnabled = true;
			this.cboVideoTune.Items.AddRange(new object[] {
            "off",
            "psnr",
            "ssim",
            "grain",
            "zerolatency",
            "fastdecode"});
			this.cboVideoTune.Location = new System.Drawing.Point(163, 84);
			this.cboVideoTune.Name = "cboVideoTune";
			this.cboVideoTune.Size = new System.Drawing.Size(121, 21);
			this.cboVideoTune.TabIndex = 3;
			this.cboVideoTune.SelectedIndexChanged += new System.EventHandler(this.cboVideoTune_SelectedIndexChanged);
			// 
			// lblVideoTune
			// 
			this.lblVideoTune.AutoSize = true;
			this.lblVideoTune.Location = new System.Drawing.Point(160, 68);
			this.lblVideoTune.Name = "lblVideoTune";
			this.lblVideoTune.Size = new System.Drawing.Size(35, 13);
			this.lblVideoTune.TabIndex = 2;
			this.lblVideoTune.Text = "&Tune:";
			// 
			// cboVideoPreset
			// 
			this.cboVideoPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboVideoPreset.FormattingEnabled = true;
			this.cboVideoPreset.Items.AddRange(new object[] {
            "ultrafast",
            "superfast",
            "veryfast",
            "faster",
            "fast",
            "medium",
            "slow",
            "slower",
            "veryslow",
            "placebo"});
			this.cboVideoPreset.Location = new System.Drawing.Point(36, 84);
			this.cboVideoPreset.Name = "cboVideoPreset";
			this.cboVideoPreset.Size = new System.Drawing.Size(121, 21);
			this.cboVideoPreset.TabIndex = 1;
			this.cboVideoPreset.SelectedIndexChanged += new System.EventHandler(this.cboVideoPreset_SelectedIndexChanged);
			// 
			// lblVideoPreset
			// 
			this.lblVideoPreset.AutoSize = true;
			this.lblVideoPreset.Location = new System.Drawing.Point(33, 68);
			this.lblVideoPreset.Name = "lblVideoPreset";
			this.lblVideoPreset.Size = new System.Drawing.Size(42, 13);
			this.lblVideoPreset.TabIndex = 0;
			this.lblVideoPreset.Text = "Pre&set:";
			// 
			// tabAudio
			// 
			this.tabAudio.Controls.Add(this.btnAudioRemove);
			this.tabAudio.Controls.Add(this.btnAudioAdd);
			this.tabAudio.Controls.Add(this.lblAudioChannel);
			this.tabAudio.Controls.Add(this.cboAudioChannel);
			this.tabAudio.Controls.Add(this.chkAudioMerge);
			this.tabAudio.Controls.Add(this.clbAudioTracks);
			this.tabAudio.Controls.Add(this.lblAudioFreq);
			this.tabAudio.Controls.Add(this.cboAudioFreq);
			this.tabAudio.Controls.Add(this.txtAudioCmd);
			this.tabAudio.Controls.Add(this.lblAudioCmd);
			this.tabAudio.Controls.Add(this.lblAudioBit);
			this.tabAudio.Controls.Add(this.cboAudioBit);
			this.tabAudio.Controls.Add(this.cboAudioEncoder);
			this.tabAudio.Controls.Add(this.lblAudioEncoder);
			this.tabAudio.Location = new System.Drawing.Point(4, 22);
			this.tabAudio.Name = "tabAudio";
			this.tabAudio.Padding = new System.Windows.Forms.Padding(3);
			this.tabAudio.Size = new System.Drawing.Size(652, 224);
			this.tabAudio.TabIndex = 2;
			this.tabAudio.Text = "Audio";
			this.tabAudio.UseVisualStyleBackColor = true;
			// 
			// btnAudioRemove
			// 
			this.btnAudioRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAudioRemove.Image = global::ifme.Properties.Resources.delete;
			this.btnAudioRemove.Location = new System.Drawing.Point(622, 6);
			this.btnAudioRemove.Name = "btnAudioRemove";
			this.btnAudioRemove.Size = new System.Drawing.Size(24, 24);
			this.btnAudioRemove.TabIndex = 2;
			this.btnAudioRemove.UseVisualStyleBackColor = true;
			this.btnAudioRemove.Click += new System.EventHandler(this.btnAudioRemove_Click);
			// 
			// btnAudioAdd
			// 
			this.btnAudioAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAudioAdd.Image = global::ifme.Properties.Resources.add;
			this.btnAudioAdd.Location = new System.Drawing.Point(592, 6);
			this.btnAudioAdd.Name = "btnAudioAdd";
			this.btnAudioAdd.Size = new System.Drawing.Size(24, 24);
			this.btnAudioAdd.TabIndex = 1;
			this.btnAudioAdd.UseVisualStyleBackColor = true;
			this.btnAudioAdd.Click += new System.EventHandler(this.btnAudioAdd_Click);
			// 
			// lblAudioChannel
			// 
			this.lblAudioChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblAudioChannel.AutoSize = true;
			this.lblAudioChannel.Location = new System.Drawing.Point(207, 173);
			this.lblAudioChannel.Name = "lblAudioChannel";
			this.lblAudioChannel.Size = new System.Drawing.Size(50, 13);
			this.lblAudioChannel.TabIndex = 10;
			this.lblAudioChannel.Text = "&Channel:";
			// 
			// cboAudioChannel
			// 
			this.cboAudioChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboAudioChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioChannel.FormattingEnabled = true;
			this.cboAudioChannel.Items.AddRange(new object[] {
            "auto",
            "mono",
            "stereo"});
			this.cboAudioChannel.Location = new System.Drawing.Point(210, 189);
			this.cboAudioChannel.Name = "cboAudioChannel";
			this.cboAudioChannel.Size = new System.Drawing.Size(96, 21);
			this.cboAudioChannel.TabIndex = 11;
			this.cboAudioChannel.SelectedIndexChanged += new System.EventHandler(this.cboAudioChannel_SelectedIndexChanged);
			// 
			// chkAudioMerge
			// 
			this.chkAudioMerge.AutoSize = true;
			this.chkAudioMerge.Location = new System.Drawing.Point(6, 11);
			this.chkAudioMerge.Name = "chkAudioMerge";
			this.chkAudioMerge.Size = new System.Drawing.Size(356, 17);
			this.chkAudioMerge.TabIndex = 0;
			this.chkAudioMerge.Text = "&Compile all stream into single stream (not applicable for Passthrough)";
			this.chkAudioMerge.UseVisualStyleBackColor = true;
			this.chkAudioMerge.CheckedChanged += new System.EventHandler(this.chkAudioMerge_CheckedChanged);
			// 
			// clbAudioTracks
			// 
			this.clbAudioTracks.AllowDrop = true;
			this.clbAudioTracks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clbAudioTracks.FormattingEnabled = true;
			this.clbAudioTracks.Location = new System.Drawing.Point(6, 36);
			this.clbAudioTracks.Name = "clbAudioTracks";
			this.clbAudioTracks.Size = new System.Drawing.Size(640, 94);
			this.clbAudioTracks.TabIndex = 3;
			this.clbAudioTracks.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbAudioTracks_ItemCheck);
			this.clbAudioTracks.SelectedIndexChanged += new System.EventHandler(this.clbAudioTracks_SelectedIndexChanged);
			this.clbAudioTracks.DragDrop += new System.Windows.Forms.DragEventHandler(this.clbAudioTracks_DragDrop);
			this.clbAudioTracks.DragEnter += new System.Windows.Forms.DragEventHandler(this.clbAudioTracks_DragEnter);
			this.clbAudioTracks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.clbAudioTracks_KeyDown);
			// 
			// lblAudioFreq
			// 
			this.lblAudioFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblAudioFreq.AutoSize = true;
			this.lblAudioFreq.Location = new System.Drawing.Point(105, 173);
			this.lblAudioFreq.Name = "lblAudioFreq";
			this.lblAudioFreq.Size = new System.Drawing.Size(62, 13);
			this.lblAudioFreq.TabIndex = 8;
			this.lblAudioFreq.Text = "&Frequency:";
			// 
			// cboAudioFreq
			// 
			this.cboAudioFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboAudioFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioFreq.FormattingEnabled = true;
			this.cboAudioFreq.Items.AddRange(new object[] {
            "auto",
            "8000",
            "12000",
            "16000",
            "22050",
            "24000",
            "32000",
            "44100",
            "48000",
            "96000",
            "192000"});
			this.cboAudioFreq.Location = new System.Drawing.Point(108, 189);
			this.cboAudioFreq.Name = "cboAudioFreq";
			this.cboAudioFreq.Size = new System.Drawing.Size(96, 21);
			this.cboAudioFreq.TabIndex = 9;
			this.cboAudioFreq.SelectedIndexChanged += new System.EventHandler(this.cboAudioFreq_SelectedIndexChanged);
			// 
			// txtAudioCmd
			// 
			this.txtAudioCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioCmd.Location = new System.Drawing.Point(312, 149);
			this.txtAudioCmd.Multiline = true;
			this.txtAudioCmd.Name = "txtAudioCmd";
			this.txtAudioCmd.Size = new System.Drawing.Size(334, 61);
			this.txtAudioCmd.TabIndex = 13;
			this.txtAudioCmd.TextChanged += new System.EventHandler(this.txtAudioCmd_TextChanged);
			// 
			// lblAudioCmd
			// 
			this.lblAudioCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblAudioCmd.AutoSize = true;
			this.lblAudioCmd.Location = new System.Drawing.Point(309, 133);
			this.lblAudioCmd.Name = "lblAudioCmd";
			this.lblAudioCmd.Size = new System.Drawing.Size(105, 13);
			this.lblAudioCmd.TabIndex = 12;
			this.lblAudioCmd.Text = "&Extra command-line:";
			// 
			// lblAudioBit
			// 
			this.lblAudioBit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblAudioBit.AutoSize = true;
			this.lblAudioBit.Location = new System.Drawing.Point(3, 173);
			this.lblAudioBit.Name = "lblAudioBit";
			this.lblAudioBit.Size = new System.Drawing.Size(72, 13);
			this.lblAudioBit.TabIndex = 6;
			this.lblAudioBit.Text = "&Bit rate/level:";
			// 
			// cboAudioBit
			// 
			this.cboAudioBit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboAudioBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioBit.FormattingEnabled = true;
			this.cboAudioBit.Location = new System.Drawing.Point(6, 189);
			this.cboAudioBit.Name = "cboAudioBit";
			this.cboAudioBit.Size = new System.Drawing.Size(96, 21);
			this.cboAudioBit.TabIndex = 7;
			this.cboAudioBit.SelectedIndexChanged += new System.EventHandler(this.cboAudioBit_SelectedIndexChanged);
			// 
			// cboAudioEncoder
			// 
			this.cboAudioEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cboAudioEncoder.DisplayMember = "Value";
			this.cboAudioEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioEncoder.FormattingEnabled = true;
			this.cboAudioEncoder.Location = new System.Drawing.Point(6, 149);
			this.cboAudioEncoder.Name = "cboAudioEncoder";
			this.cboAudioEncoder.Size = new System.Drawing.Size(300, 21);
			this.cboAudioEncoder.TabIndex = 5;
			this.cboAudioEncoder.ValueMember = "Key";
			this.cboAudioEncoder.SelectedIndexChanged += new System.EventHandler(this.cboAudioEncoder_SelectedIndexChanged);
			// 
			// lblAudioEncoder
			// 
			this.lblAudioEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblAudioEncoder.AutoSize = true;
			this.lblAudioEncoder.Location = new System.Drawing.Point(3, 133);
			this.lblAudioEncoder.Name = "lblAudioEncoder";
			this.lblAudioEncoder.Size = new System.Drawing.Size(50, 13);
			this.lblAudioEncoder.TabIndex = 4;
			this.lblAudioEncoder.Text = "&Encoder:";
			// 
			// tabSubtitles
			// 
			this.tabSubtitles.Controls.Add(this.lblSubNote);
			this.tabSubtitles.Controls.Add(this.cboSubLang);
			this.tabSubtitles.Controls.Add(this.lblSubLang);
			this.tabSubtitles.Controls.Add(this.lstSub);
			this.tabSubtitles.Controls.Add(this.btnSubRemove);
			this.tabSubtitles.Controls.Add(this.btnSubAdd);
			this.tabSubtitles.Controls.Add(this.chkSubEnable);
			this.tabSubtitles.Location = new System.Drawing.Point(4, 22);
			this.tabSubtitles.Name = "tabSubtitles";
			this.tabSubtitles.Padding = new System.Windows.Forms.Padding(3);
			this.tabSubtitles.Size = new System.Drawing.Size(652, 224);
			this.tabSubtitles.TabIndex = 3;
			this.tabSubtitles.Text = "Subtitles";
			this.tabSubtitles.UseVisualStyleBackColor = true;
			this.tabSubtitles.Leave += new System.EventHandler(this.tabSubtitles_Leave);
			// 
			// lblSubNote
			// 
			this.lblSubNote.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblSubNote.Location = new System.Drawing.Point(126, 82);
			this.lblSubNote.Name = "lblSubNote";
			this.lblSubNote.Size = new System.Drawing.Size(400, 60);
			this.lblSubNote.TabIndex = 6;
			this.lblSubNote.Text = "This tab allow you to add subtitles for each selected video\r\nclick \"Enable\" above" +
    " to start adding\r\n(embaded subtitle will ignored)";
			this.lblSubNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cboSubLang
			// 
			this.cboSubLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboSubLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSubLang.FormattingEnabled = true;
			this.cboSubLang.Location = new System.Drawing.Point(6, 198);
			this.cboSubLang.Name = "cboSubLang";
			this.cboSubLang.Size = new System.Drawing.Size(640, 21);
			this.cboSubLang.TabIndex = 5;
			this.cboSubLang.Visible = false;
			this.cboSubLang.SelectedIndexChanged += new System.EventHandler(this.cboSubLang_SelectedIndexChanged);
			// 
			// lblSubLang
			// 
			this.lblSubLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblSubLang.AutoSize = true;
			this.lblSubLang.Location = new System.Drawing.Point(3, 182);
			this.lblSubLang.Name = "lblSubLang";
			this.lblSubLang.Size = new System.Drawing.Size(58, 13);
			this.lblSubLang.TabIndex = 4;
			this.lblSubLang.Text = "&Language:";
			this.lblSubLang.Visible = false;
			// 
			// lstSub
			// 
			this.lstSub.AllowDrop = true;
			this.lstSub.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstSub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSubFile,
            this.colSubLang});
			this.lstSub.FullRowSelect = true;
			this.lstSub.HideSelection = false;
			this.lstSub.Location = new System.Drawing.Point(6, 36);
			this.lstSub.Name = "lstSub";
			this.lstSub.Size = new System.Drawing.Size(640, 143);
			this.lstSub.TabIndex = 3;
			this.lstSub.UseCompatibleStateImageBehavior = false;
			this.lstSub.View = System.Windows.Forms.View.Details;
			this.lstSub.Visible = false;
			this.lstSub.SelectedIndexChanged += new System.EventHandler(this.lstSub_SelectedIndexChanged);
			this.lstSub.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstSub_DragDrop);
			this.lstSub.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstSub_DragEnter);
			// 
			// colSubFile
			// 
			this.colSubFile.Tag = "colSubFile";
			this.colSubFile.Text = "Name";
			this.colSubFile.Width = 350;
			// 
			// colSubLang
			// 
			this.colSubLang.Tag = "colSubLang";
			this.colSubLang.Text = "Language";
			this.colSubLang.Width = 286;
			// 
			// btnSubRemove
			// 
			this.btnSubRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubRemove.Image = global::ifme.Properties.Resources.delete;
			this.btnSubRemove.Location = new System.Drawing.Point(622, 6);
			this.btnSubRemove.Name = "btnSubRemove";
			this.btnSubRemove.Size = new System.Drawing.Size(24, 24);
			this.btnSubRemove.TabIndex = 2;
			this.btnSubRemove.UseVisualStyleBackColor = true;
			this.btnSubRemove.Visible = false;
			this.btnSubRemove.Click += new System.EventHandler(this.btnSubRemove_Click);
			// 
			// btnSubAdd
			// 
			this.btnSubAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubAdd.Image = global::ifme.Properties.Resources.add;
			this.btnSubAdd.Location = new System.Drawing.Point(592, 6);
			this.btnSubAdd.Name = "btnSubAdd";
			this.btnSubAdd.Size = new System.Drawing.Size(24, 24);
			this.btnSubAdd.TabIndex = 1;
			this.btnSubAdd.UseVisualStyleBackColor = true;
			this.btnSubAdd.Visible = false;
			this.btnSubAdd.Click += new System.EventHandler(this.btnSubAdd_Click);
			// 
			// chkSubEnable
			// 
			this.chkSubEnable.AutoSize = true;
			this.chkSubEnable.Location = new System.Drawing.Point(6, 11);
			this.chkSubEnable.Name = "chkSubEnable";
			this.chkSubEnable.Size = new System.Drawing.Size(58, 17);
			this.chkSubEnable.TabIndex = 0;
			this.chkSubEnable.Text = "&Enable";
			this.chkSubEnable.UseVisualStyleBackColor = true;
			this.chkSubEnable.CheckedChanged += new System.EventHandler(this.chkSubEnable_CheckedChanged);
			// 
			// tabAttachments
			// 
			this.tabAttachments.Controls.Add(this.lblAttachNote);
			this.tabAttachments.Controls.Add(this.txtAttachDescription);
			this.tabAttachments.Controls.Add(this.lblAttachDescription);
			this.tabAttachments.Controls.Add(this.lstAttach);
			this.tabAttachments.Controls.Add(this.btnAttachRemove);
			this.tabAttachments.Controls.Add(this.btnAttachAdd);
			this.tabAttachments.Controls.Add(this.chkAttachEnable);
			this.tabAttachments.Location = new System.Drawing.Point(4, 22);
			this.tabAttachments.Name = "tabAttachments";
			this.tabAttachments.Padding = new System.Windows.Forms.Padding(3);
			this.tabAttachments.Size = new System.Drawing.Size(652, 224);
			this.tabAttachments.TabIndex = 4;
			this.tabAttachments.Text = "Attachments";
			this.tabAttachments.UseVisualStyleBackColor = true;
			this.tabAttachments.Leave += new System.EventHandler(this.tabAttachments_Leave);
			// 
			// lblAttachNote
			// 
			this.lblAttachNote.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAttachNote.Location = new System.Drawing.Point(126, 82);
			this.lblAttachNote.Name = "lblAttachNote";
			this.lblAttachNote.Size = new System.Drawing.Size(400, 60);
			this.lblAttachNote.TabIndex = 6;
			this.lblAttachNote.Text = "This tab allow you to add attachment for each selected video\r\nclick \"Enable\" abov" +
    "e to start adding\r\n(embaded attachment will ignored)";
			this.lblAttachNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtAttachDescription
			// 
			this.txtAttachDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAttachDescription.Location = new System.Drawing.Point(6, 198);
			this.txtAttachDescription.Name = "txtAttachDescription";
			this.txtAttachDescription.Size = new System.Drawing.Size(640, 20);
			this.txtAttachDescription.TabIndex = 5;
			this.txtAttachDescription.Visible = false;
			this.txtAttachDescription.TextChanged += new System.EventHandler(this.txtAttachDescription_TextChanged);
			// 
			// lblAttachDescription
			// 
			this.lblAttachDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblAttachDescription.AutoSize = true;
			this.lblAttachDescription.Location = new System.Drawing.Point(3, 182);
			this.lblAttachDescription.Name = "lblAttachDescription";
			this.lblAttachDescription.Size = new System.Drawing.Size(64, 13);
			this.lblAttachDescription.TabIndex = 4;
			this.lblAttachDescription.Text = "&Description:";
			this.lblAttachDescription.Visible = false;
			// 
			// lstAttach
			// 
			this.lstAttach.AllowDrop = true;
			this.lstAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstAttach.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAttachName,
            this.colAttachMime,
            this.colAttachDescription});
			this.lstAttach.FullRowSelect = true;
			this.lstAttach.HideSelection = false;
			this.lstAttach.Location = new System.Drawing.Point(6, 36);
			this.lstAttach.Name = "lstAttach";
			this.lstAttach.Size = new System.Drawing.Size(640, 143);
			this.lstAttach.TabIndex = 3;
			this.lstAttach.UseCompatibleStateImageBehavior = false;
			this.lstAttach.View = System.Windows.Forms.View.Details;
			this.lstAttach.Visible = false;
			this.lstAttach.SelectedIndexChanged += new System.EventHandler(this.lstAttach_SelectedIndexChanged);
			this.lstAttach.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstAttach_DragDrop);
			this.lstAttach.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstAttach_DragEnter);
			// 
			// colAttachName
			// 
			this.colAttachName.Tag = "colAttachName";
			this.colAttachName.Text = "Name";
			this.colAttachName.Width = 350;
			// 
			// colAttachMime
			// 
			this.colAttachMime.Tag = "colAttachMime";
			this.colAttachMime.Text = "MIME";
			this.colAttachMime.Width = 130;
			// 
			// colAttachDescription
			// 
			this.colAttachDescription.Tag = "colAttachDescription";
			this.colAttachDescription.Text = "Description";
			this.colAttachDescription.Width = 156;
			// 
			// btnAttachRemove
			// 
			this.btnAttachRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAttachRemove.Image = global::ifme.Properties.Resources.delete;
			this.btnAttachRemove.Location = new System.Drawing.Point(622, 6);
			this.btnAttachRemove.Name = "btnAttachRemove";
			this.btnAttachRemove.Size = new System.Drawing.Size(24, 24);
			this.btnAttachRemove.TabIndex = 2;
			this.btnAttachRemove.UseVisualStyleBackColor = true;
			this.btnAttachRemove.Visible = false;
			this.btnAttachRemove.Click += new System.EventHandler(this.btnAttachRemove_Click);
			// 
			// btnAttachAdd
			// 
			this.btnAttachAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAttachAdd.Image = global::ifme.Properties.Resources.add;
			this.btnAttachAdd.Location = new System.Drawing.Point(592, 6);
			this.btnAttachAdd.Name = "btnAttachAdd";
			this.btnAttachAdd.Size = new System.Drawing.Size(24, 24);
			this.btnAttachAdd.TabIndex = 1;
			this.btnAttachAdd.UseVisualStyleBackColor = true;
			this.btnAttachAdd.Visible = false;
			this.btnAttachAdd.Click += new System.EventHandler(this.btnAttachAdd_Click);
			// 
			// chkAttachEnable
			// 
			this.chkAttachEnable.AutoSize = true;
			this.chkAttachEnable.Location = new System.Drawing.Point(6, 11);
			this.chkAttachEnable.Name = "chkAttachEnable";
			this.chkAttachEnable.Size = new System.Drawing.Size(58, 17);
			this.chkAttachEnable.TabIndex = 0;
			this.chkAttachEnable.Text = "&Enable";
			this.chkAttachEnable.UseVisualStyleBackColor = true;
			this.chkAttachEnable.CheckedChanged += new System.EventHandler(this.chkAttachEnable_CheckedChanged);
			// 
			// tabLogs
			// 
			this.tabLogs.Controls.Add(this.txtLog);
			this.tabLogs.Location = new System.Drawing.Point(4, 22);
			this.tabLogs.Name = "tabLogs";
			this.tabLogs.Padding = new System.Windows.Forms.Padding(3);
			this.tabLogs.Size = new System.Drawing.Size(652, 224);
			this.tabLogs.TabIndex = 5;
			this.tabLogs.Text = "Logs";
			this.tabLogs.UseVisualStyleBackColor = true;
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtLog.Location = new System.Drawing.Point(3, 3);
			this.txtLog.MaxLength = 2147483647;
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(646, 218);
			this.txtLog.TabIndex = 0;
			this.txtLog.WordWrap = false;
			// 
			// lblProfiles
			// 
			this.lblProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblProfiles.Location = new System.Drawing.Point(12, 596);
			this.lblProfiles.Name = "lblProfiles";
			this.lblProfiles.Size = new System.Drawing.Size(110, 24);
			this.lblProfiles.TabIndex = 15;
			this.lblProfiles.Text = "&Encoding Preset:";
			this.lblProfiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cboProfile
			// 
			this.cboProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboProfile.Font = new System.Drawing.Font("Tahoma", 10F);
			this.cboProfile.FormattingEnabled = true;
			this.cboProfile.Items.AddRange(new object[] {
            "<new>"});
			this.cboProfile.Location = new System.Drawing.Point(128, 596);
			this.cboProfile.Name = "cboProfile";
			this.cboProfile.Size = new System.Drawing.Size(484, 24);
			this.cboProfile.TabIndex = 16;
			this.cboProfile.SelectedIndexChanged += new System.EventHandler(this.cboProfile_SelectedIndexChanged);
			// 
			// btnProfileSave
			// 
			this.btnProfileSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnProfileSave.Image = global::ifme.Properties.Resources.pencil;
			this.btnProfileSave.Location = new System.Drawing.Point(618, 596);
			this.btnProfileSave.Name = "btnProfileSave";
			this.btnProfileSave.Size = new System.Drawing.Size(24, 24);
			this.btnProfileSave.TabIndex = 17;
			this.btnProfileSave.UseVisualStyleBackColor = true;
			this.btnProfileSave.Click += new System.EventHandler(this.btnProfileSave_Click);
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Image = global::ifme.Properties.Resources.folder_explore;
			this.btnBrowse.Location = new System.Drawing.Point(580, 626);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(24, 24);
			this.btnBrowse.TabIndex = 20;
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// sptVert1
			// 
			this.sptVert1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.sptVert1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sptVert1.Location = new System.Drawing.Point(610, 70);
			this.sptVert1.Name = "sptVert1";
			this.sptVert1.Size = new System.Drawing.Size(2, 24);
			this.sptVert1.TabIndex = 9;
			// 
			// sptVert2
			// 
			this.sptVert2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.sptVert2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sptVert2.Location = new System.Drawing.Point(542, 70);
			this.sptVert2.Name = "sptVert2";
			this.sptVert2.Size = new System.Drawing.Size(2, 24);
			this.sptVert2.TabIndex = 6;
			// 
			// chkShutdown
			// 
			this.chkShutdown.AutoSize = true;
			this.chkShutdown.Location = new System.Drawing.Point(12, 75);
			this.chkShutdown.Name = "chkShutdown";
			this.chkShutdown.Size = new System.Drawing.Size(169, 17);
			this.chkShutdown.TabIndex = 0;
			this.chkShutdown.Text = "&Turn off computer when done";
			this.chkShutdown.UseVisualStyleBackColor = true;
			// 
			// sptVert3
			// 
			this.sptVert3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.sptVert3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sptVert3.Location = new System.Drawing.Point(610, 626);
			this.sptVert3.Name = "sptVert3";
			this.sptVert3.Size = new System.Drawing.Size(2, 24);
			this.sptVert3.TabIndex = 21;
			// 
			// btnConfig
			// 
			this.btnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnConfig.Image = global::ifme.Properties.Resources.wrench;
			this.btnConfig.Location = new System.Drawing.Point(618, 626);
			this.btnConfig.Name = "btnConfig";
			this.btnConfig.Size = new System.Drawing.Size(24, 24);
			this.btnConfig.TabIndex = 22;
			this.btnConfig.UseVisualStyleBackColor = true;
			this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
			// 
			// btnAbout
			// 
			this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAbout.Image = global::ifme.Properties.Resources.information;
			this.btnAbout.Location = new System.Drawing.Point(648, 626);
			this.btnAbout.Name = "btnAbout";
			this.btnAbout.Size = new System.Drawing.Size(24, 24);
			this.btnAbout.TabIndex = 23;
			this.btnAbout.UseVisualStyleBackColor = true;
			this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
			// 
			// txtDestination
			// 
			this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDestination.Font = new System.Drawing.Font("Tahoma", 10F);
			this.txtDestination.Location = new System.Drawing.Point(128, 626);
			this.txtDestination.Name = "txtDestination";
			this.txtDestination.Size = new System.Drawing.Size(446, 24);
			this.txtDestination.TabIndex = 19;
			this.txtDestination.TextChanged += new System.EventHandler(this.txtDestination_TextChanged);
			// 
			// bgwEncoding
			// 
			this.bgwEncoding.WorkerSupportsCancellation = true;
			this.bgwEncoding.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwEncoding_DoWork);
			this.bgwEncoding.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwEncoding_RunWorkerCompleted);
			// 
			// tipUpdate
			// 
			this.tipUpdate.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.tipUpdate.ToolTipTitle = "Info";
			// 
			// btnDonate
			// 
			this.btnDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDonate.Image = global::ifme.Properties.Resources.donate;
			this.btnDonate.Location = new System.Drawing.Point(444, 70);
			this.btnDonate.Name = "btnDonate";
			this.btnDonate.Size = new System.Drawing.Size(24, 24);
			this.btnDonate.TabIndex = 2;
			this.btnDonate.UseVisualStyleBackColor = true;
			this.btnDonate.Click += new System.EventHandler(this.btnDonate_Click);
			// 
			// sptVert4
			// 
			this.sptVert4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.sptVert4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sptVert4.Location = new System.Drawing.Point(474, 70);
			this.sptVert4.Name = "sptVert4";
			this.sptVert4.Size = new System.Drawing.Size(2, 24);
			this.sptVert4.TabIndex = 3;
			// 
			// chkDestination
			// 
			this.chkDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkDestination.Checked = true;
			this.chkDestination.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkDestination.Location = new System.Drawing.Point(15, 626);
			this.chkDestination.Name = "chkDestination";
			this.chkDestination.Size = new System.Drawing.Size(107, 24);
			this.chkDestination.TabIndex = 18;
			this.chkDestination.Text = "&Destination:";
			this.chkDestination.UseVisualStyleBackColor = true;
			this.chkDestination.CheckedChanged += new System.EventHandler(this.chkDestination_CheckedChanged);
			// 
			// btnFacebook
			// 
			this.btnFacebook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFacebook.Image = global::ifme.Properties.Resources.fb;
			this.btnFacebook.Location = new System.Drawing.Point(414, 70);
			this.btnFacebook.Name = "btnFacebook";
			this.btnFacebook.Size = new System.Drawing.Size(24, 24);
			this.btnFacebook.TabIndex = 1;
			this.btnFacebook.UseVisualStyleBackColor = true;
			this.btnFacebook.Click += new System.EventHandler(this.btnFacebook_Click);
			// 
			// btnProfileDelete
			// 
			this.btnProfileDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnProfileDelete.Image = global::ifme.Properties.Resources.cross;
			this.btnProfileDelete.Location = new System.Drawing.Point(648, 596);
			this.btnProfileDelete.Name = "btnProfileDelete";
			this.btnProfileDelete.Size = new System.Drawing.Size(24, 24);
			this.btnProfileDelete.TabIndex = 24;
			this.btnProfileDelete.UseVisualStyleBackColor = true;
			this.btnProfileDelete.Click += new System.EventHandler(this.btnProfileDelete_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(684, 662);
			this.Controls.Add(this.btnProfileDelete);
			this.Controls.Add(this.btnFacebook);
			this.Controls.Add(this.chkDestination);
			this.Controls.Add(this.btnDonate);
			this.Controls.Add(this.sptVert4);
			this.Controls.Add(this.txtDestination);
			this.Controls.Add(this.btnAbout);
			this.Controls.Add(this.btnConfig);
			this.Controls.Add(this.sptVert3);
			this.Controls.Add(this.chkShutdown);
			this.Controls.Add(this.sptVert2);
			this.Controls.Add(this.sptVert1);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.btnProfileSave);
			this.Controls.Add(this.cboProfile);
			this.Controls.Add(this.lblProfiles);
			this.Controls.Add(this.tabConfig);
			this.Controls.Add(this.lstQueue);
			this.Controls.Add(this.btnQueueStart);
			this.Controls.Add(this.btnQueueMoveDown);
			this.Controls.Add(this.btnQueueMoveUp);
			this.Controls.Add(this.btnQueueRemove);
			this.Controls.Add(this.btnQueueAdd);
			this.Controls.Add(this.pbxRight);
			this.Controls.Add(this.pbxLeft);
			this.Controls.Add(this.btnQueueStop);
			this.Controls.Add(this.btnQueuePause);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.MinimumSize = new System.Drawing.Size(692, 700);
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Internet Friendly Media Encoder";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.Shown += new System.EventHandler(this.frmMain_Shown);
			((System.ComponentModel.ISupportInitialize)(this.pbxLeft)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbxRight)).EndInit();
			this.cmsQueueMenu.ResumeLayout(false);
			this.tabConfig.ResumeLayout(false);
			this.tabPicture.ResumeLayout(false);
			this.tabPicture.PerformLayout();
			this.grpPictureYadif.ResumeLayout(false);
			this.grpPictureYadif.PerformLayout();
			this.grpPictureQuality.ResumeLayout(false);
			this.grpPictureQuality.PerformLayout();
			this.grpPictureBasic.ResumeLayout(false);
			this.grpPictureBasic.PerformLayout();
			this.grpPictureFormat.ResumeLayout(false);
			this.grpPictureFormat.PerformLayout();
			this.tabVideo.ResumeLayout(false);
			this.tabVideo.PerformLayout();
			this.grpVideoRateCtrl.ResumeLayout(false);
			this.grpVideoRateCtrl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkVideoRate)).EndInit();
			this.grpVideoBasic.ResumeLayout(false);
			this.grpVideoBasic.PerformLayout();
			this.tabAudio.ResumeLayout(false);
			this.tabAudio.PerformLayout();
			this.tabSubtitles.ResumeLayout(false);
			this.tabSubtitles.PerformLayout();
			this.tabAttachments.ResumeLayout(false);
			this.tabAttachments.PerformLayout();
			this.tabLogs.ResumeLayout(false);
			this.tabLogs.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pbxLeft;
		private System.Windows.Forms.PictureBox pbxRight;
		private System.Windows.Forms.Button btnQueueAdd;
		private System.Windows.Forms.Button btnQueueRemove;
		private System.Windows.Forms.Button btnQueueMoveUp;
		private System.Windows.Forms.Button btnQueueMoveDown;
		private System.Windows.Forms.Button btnQueueStart;
		private System.Windows.Forms.Button btnQueueStop;
		private System.Windows.Forms.ListView lstQueue;
		private System.Windows.Forms.ColumnHeader colQueueName;
		private System.Windows.Forms.Button btnQueuePause;
		private System.Windows.Forms.TabControl tabConfig;
		private System.Windows.Forms.TabPage tabPicture;
		private System.Windows.Forms.TabPage tabVideo;
		private System.Windows.Forms.Label lblProfiles;
		private System.Windows.Forms.ComboBox cboProfile;
		private System.Windows.Forms.Button btnProfileSave;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TabPage tabAudio;
		private System.Windows.Forms.TabPage tabSubtitles;
		private System.Windows.Forms.TabPage tabAttachments;
		private System.Windows.Forms.TabPage tabLogs;
		private System.Windows.Forms.GroupBox grpPictureFormat;
		private System.Windows.Forms.GroupBox grpPictureQuality;
		private System.Windows.Forms.ComboBox cboPictureYuv;
		private System.Windows.Forms.Label lblPictureYuv;
		private System.Windows.Forms.ComboBox cboPictureBit;
		private System.Windows.Forms.Label lblPictureBit;
		private System.Windows.Forms.GroupBox grpPictureBasic;
		private System.Windows.Forms.Label lblPictureFps;
		private System.Windows.Forms.ComboBox cboPictureFps;
		private System.Windows.Forms.ComboBox cboPictureRes;
		private System.Windows.Forms.Label lblPictureRes;
		private System.Windows.Forms.RadioButton rdoMP4;
		private System.Windows.Forms.RadioButton rdoMKV;
		private System.Windows.Forms.GroupBox grpPictureYadif;
		private System.Windows.Forms.CheckBox chkPictureYadif;
		private System.Windows.Forms.ComboBox cboPictureYadifMode;
		private System.Windows.Forms.Label lblPictureYadifMode;
		private System.Windows.Forms.ComboBox cboPictureYadifFlag;
		private System.Windows.Forms.Label lblPictureYadifFlag;
		private System.Windows.Forms.ComboBox cboPictureYadifField;
		private System.Windows.Forms.Label lblPictureYadifField;
		private System.Windows.Forms.ColumnHeader colQueueSize;
		private System.Windows.Forms.ColumnHeader colQueueType;
		private System.Windows.Forms.ColumnHeader colQueueTo;
		private System.Windows.Forms.ColumnHeader colQueueStatus;
		private System.Windows.Forms.GroupBox grpVideoBasic;
		private System.Windows.Forms.Label lblVideoPreset;
		private System.Windows.Forms.ComboBox cboVideoTune;
		private System.Windows.Forms.Label lblVideoTune;
		private System.Windows.Forms.ComboBox cboVideoPreset;
		private System.Windows.Forms.GroupBox grpVideoRateCtrl;
		private System.Windows.Forms.ComboBox cboVideoType;
		private System.Windows.Forms.TextBox txtVideoValue;
		private System.Windows.Forms.Label lblVideoRateValue;
		private System.Windows.Forms.Label lblVideoRateL;
		private System.Windows.Forms.Label lblVideoRateH;
		private System.Windows.Forms.TrackBar trkVideoRate;
		private System.Windows.Forms.Label lblVideoCmd;
		private System.Windows.Forms.TextBox txtVideoCmd;
		private System.Windows.Forms.Label lblAudioEncoder;
		private System.Windows.Forms.ComboBox cboAudioEncoder;
		private System.Windows.Forms.Label lblAudioBit;
		private System.Windows.Forms.Label lblAudioFreq;
		private System.Windows.Forms.ComboBox cboAudioBit;
		private System.Windows.Forms.ComboBox cboAudioFreq;
		private System.Windows.Forms.Label lblAudioChannel;
		private System.Windows.Forms.ComboBox cboAudioChannel;
		private System.Windows.Forms.CheckBox chkAudioMerge;
		private System.Windows.Forms.TextBox txtAudioCmd;
		private System.Windows.Forms.CheckBox chkSubEnable;
		private System.Windows.Forms.Button btnSubAdd;
		private System.Windows.Forms.Button btnSubRemove;
		private System.Windows.Forms.ListView lstSub;
		private System.Windows.Forms.ColumnHeader colSubFile;
		private System.Windows.Forms.ColumnHeader colSubLang;
		private System.Windows.Forms.ComboBox cboSubLang;
		private System.Windows.Forms.Label lblSubLang;
		private System.Windows.Forms.Label lblSubNote;
		private System.Windows.Forms.CheckBox chkAttachEnable;
		private System.Windows.Forms.Button btnAttachRemove;
		private System.Windows.Forms.Button btnAttachAdd;
		private System.Windows.Forms.ListView lstAttach;
		private System.Windows.Forms.ColumnHeader colAttachName;
		private System.Windows.Forms.ColumnHeader colAttachMime;
		private System.Windows.Forms.TextBox txtAttachDescription;
		private System.Windows.Forms.Label lblAttachDescription;
		private System.Windows.Forms.Label lblAttachNote;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.Label sptVert1;
		private System.Windows.Forms.Label sptVert2;
		private System.Windows.Forms.CheckBox chkShutdown;
		private System.Windows.Forms.Label sptVert3;
		private System.Windows.Forms.Button btnConfig;
		private System.Windows.Forms.Button btnAbout;
		private System.Windows.Forms.ColumnHeader colAttachDescription;
		private System.Windows.Forms.TextBox txtDestination;
		private System.ComponentModel.BackgroundWorker bgwEncoding;
		private System.Windows.Forms.ContextMenuStrip cmsQueueMenu;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueuePreview;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueSelectAll;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueSelectNone;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueSelectInvert;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueAviSynth;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueAviSynthEdit;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueAviSynthGenerate;
		private System.Windows.Forms.ToolStripMenuItem tsmiBenchmark;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueSave;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueOpen;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueSaveAs;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueNew;
		private System.Windows.Forms.CheckedListBox clbAudioTracks;
		private System.Windows.Forms.CheckBox chkPictureVideoCopy;
		private System.Windows.Forms.ToolTip tipUpdate;
		private System.Windows.Forms.ToolTip tipNotify;
		private System.Windows.Forms.Label sptVert4;
		private System.Windows.Forms.Button btnDonate;
		private System.Windows.Forms.CheckBox chkDestination;
		private System.Windows.Forms.ToolStripMenuItem tsmiFFmpeg;
		private System.Windows.Forms.Button btnFacebook;
		private System.Windows.Forms.ToolStripMenuItem tsmiQueueDelete;
		private System.Windows.Forms.Button btnProfileDelete;
		private System.Windows.Forms.Label lblAudioCmd;
		private System.Windows.Forms.Button btnAudioAdd;
		private System.Windows.Forms.Button btnAudioRemove;
	}
}