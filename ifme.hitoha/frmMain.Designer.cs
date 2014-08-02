namespace ifme.hitoha
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
			this.btnOptions = new System.Windows.Forms.Button();
			this.btnAbout = new System.Windows.Forms.Button();
			this.btnPause = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.tabEncoding = new System.Windows.Forms.TabControl();
			this.tabQueue = new System.Windows.Forms.TabPage();
			this.btnQueueBrowseDest = new System.Windows.Forms.Button();
			this.txtDestDir = new System.Windows.Forms.TextBox();
			this.chkQueueSaveTo = new System.Windows.Forms.CheckBox();
			this.btnQueueDown = new System.Windows.Forms.Button();
			this.btnQueueUp = new System.Windows.Forms.Button();
			this.btnQueueClear = new System.Windows.Forms.Button();
			this.btnQueueRemove = new System.Windows.Forms.Button();
			this.btnQueueAdd = new System.Windows.Forms.Button();
			this.lstQueue = new System.Windows.Forms.ListView();
			this.colQueueFName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueExt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueCodec = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueRes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueFPS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueueBitDepth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colQueuePath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabVideo = new System.Windows.Forms.TabPage();
			this.txtVideoAdvCmd = new System.Windows.Forms.TextBox();
			this.lblVideoAdvCmd = new System.Windows.Forms.Label();
			this.grpVideoRateCtrl = new System.Windows.Forms.GroupBox();
			this.lblVideoRateFL = new System.Windows.Forms.Label();
			this.lblVideoRateFH = new System.Windows.Forms.Label();
			this.trkVideoRate = new System.Windows.Forms.TrackBar();
			this.txtVideoRate = new System.Windows.Forms.TextBox();
			this.lblVideoRateFactor = new System.Windows.Forms.Label();
			this.cboVideoRateCtrl = new System.Windows.Forms.ComboBox();
			this.grpVideoBasic = new System.Windows.Forms.GroupBox();
			this.cboVideoTune = new System.Windows.Forms.ComboBox();
			this.lblVideoTune = new System.Windows.Forms.Label();
			this.cboVideoPreset = new System.Windows.Forms.ComboBox();
			this.lblVideoPreset = new System.Windows.Forms.Label();
			this.tabAudio = new System.Windows.Forms.TabPage();
			this.txtAudioCmd = new System.Windows.Forms.TextBox();
			this.lblAudioCmdAdv = new System.Windows.Forms.Label();
			this.grpAudioMode = new System.Windows.Forms.GroupBox();
			this.cboAudioMode = new System.Windows.Forms.ComboBox();
			this.grpAudioQuality = new System.Windows.Forms.GroupBox();
			this.lblAudioChan = new System.Windows.Forms.Label();
			this.cboAudioChan = new System.Windows.Forms.ComboBox();
			this.lblAudioFreq = new System.Windows.Forms.Label();
			this.cboAudioFreq = new System.Windows.Forms.ComboBox();
			this.cboAudioBitRate = new System.Windows.Forms.ComboBox();
			this.lblAudioBitRate = new System.Windows.Forms.Label();
			this.grpAudioFormat = new System.Windows.Forms.GroupBox();
			this.lblAudioFInfo = new System.Windows.Forms.Label();
			this.lblAudioFormat = new System.Windows.Forms.Label();
			this.cboAudioFormat = new System.Windows.Forms.ComboBox();
			this.tabSubtitle = new System.Windows.Forms.TabPage();
			this.lblSubtitleNotice = new System.Windows.Forms.Label();
			this.btnSubDown = new System.Windows.Forms.Button();
			this.btnSubUp = new System.Windows.Forms.Button();
			this.chkSubEnable = new System.Windows.Forms.CheckBox();
			this.lblSubLang = new System.Windows.Forms.Label();
			this.cboSubLang = new System.Windows.Forms.ComboBox();
			this.btnSubClear = new System.Windows.Forms.Button();
			this.btnSubRemove = new System.Windows.Forms.Button();
			this.btnSubAdd = new System.Windows.Forms.Button();
			this.lstSubtitle = new System.Windows.Forms.ListView();
			this.colSubFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSubExt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSubLang = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSubPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabAttachment = new System.Windows.Forms.TabPage();
			this.lblAttachNotice = new System.Windows.Forms.Label();
			this.chkAttachEnable = new System.Windows.Forms.CheckBox();
			this.txtAttachDesc = new System.Windows.Forms.TextBox();
			this.lblAttachDesc = new System.Windows.Forms.Label();
			this.lstAttachment = new System.Windows.Forms.ListView();
			this.colAttachFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAttachExt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAttachMime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAttachPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnAttachClear = new System.Windows.Forms.Button();
			this.btnAttachRemove = new System.Windows.Forms.Button();
			this.btnAttachAdd = new System.Windows.Forms.Button();
			this.tabStatus = new System.Windows.Forms.TabPage();
			this.rtfLog = new System.Windows.Forms.RichTextBox();
			this.pictBannerLeft = new System.Windows.Forms.PictureBox();
			this.pictBannerRight = new System.Windows.Forms.PictureBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnResume = new System.Windows.Forms.Button();
			this.BGThread = new System.ComponentModel.BackgroundWorker();
			this.proTip = new System.Windows.Forms.ToolTip(this.components);
			this.tabEncoding.SuspendLayout();
			this.tabQueue.SuspendLayout();
			this.tabVideo.SuspendLayout();
			this.grpVideoRateCtrl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkVideoRate)).BeginInit();
			this.grpVideoBasic.SuspendLayout();
			this.tabAudio.SuspendLayout();
			this.grpAudioMode.SuspendLayout();
			this.grpAudioQuality.SuspendLayout();
			this.grpAudioFormat.SuspendLayout();
			this.tabSubtitle.SuspendLayout();
			this.tabAttachment.SuspendLayout();
			this.tabStatus.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictBannerLeft)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictBannerRight)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOptions
			// 
			this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOptions.Location = new System.Drawing.Point(118, 445);
			this.btnOptions.Name = "btnOptions";
			this.btnOptions.Size = new System.Drawing.Size(100, 23);
			this.btnOptions.TabIndex = 3;
			this.btnOptions.Text = "{0}";
			this.btnOptions.UseVisualStyleBackColor = true;
			this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
			// 
			// btnAbout
			// 
			this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAbout.Location = new System.Drawing.Point(12, 445);
			this.btnAbout.Name = "btnAbout";
			this.btnAbout.Size = new System.Drawing.Size(100, 23);
			this.btnAbout.TabIndex = 4;
			this.btnAbout.Text = "{0}";
			this.btnAbout.UseVisualStyleBackColor = true;
			this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
			// 
			// btnPause
			// 
			this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPause.Location = new System.Drawing.Point(422, 445);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(100, 23);
			this.btnPause.TabIndex = 2;
			this.btnPause.Text = "{0}";
			this.btnPause.UseVisualStyleBackColor = true;
			this.btnPause.Visible = false;
			this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
			// 
			// btnStart
			// 
			this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStart.Enabled = false;
			this.btnStart.Location = new System.Drawing.Point(528, 445);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(100, 23);
			this.btnStart.TabIndex = 1;
			this.btnStart.Text = "{0}";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// tabEncoding
			// 
			this.tabEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabEncoding.Controls.Add(this.tabQueue);
			this.tabEncoding.Controls.Add(this.tabVideo);
			this.tabEncoding.Controls.Add(this.tabAudio);
			this.tabEncoding.Controls.Add(this.tabSubtitle);
			this.tabEncoding.Controls.Add(this.tabAttachment);
			this.tabEncoding.Controls.Add(this.tabStatus);
			this.tabEncoding.Location = new System.Drawing.Point(12, 70);
			this.tabEncoding.Name = "tabEncoding";
			this.tabEncoding.SelectedIndex = 0;
			this.tabEncoding.Size = new System.Drawing.Size(616, 369);
			this.tabEncoding.TabIndex = 0;
			// 
			// tabQueue
			// 
			this.tabQueue.Controls.Add(this.btnQueueBrowseDest);
			this.tabQueue.Controls.Add(this.txtDestDir);
			this.tabQueue.Controls.Add(this.chkQueueSaveTo);
			this.tabQueue.Controls.Add(this.btnQueueDown);
			this.tabQueue.Controls.Add(this.btnQueueUp);
			this.tabQueue.Controls.Add(this.btnQueueClear);
			this.tabQueue.Controls.Add(this.btnQueueRemove);
			this.tabQueue.Controls.Add(this.btnQueueAdd);
			this.tabQueue.Controls.Add(this.lstQueue);
			this.tabQueue.Location = new System.Drawing.Point(4, 22);
			this.tabQueue.Name = "tabQueue";
			this.tabQueue.Padding = new System.Windows.Forms.Padding(3);
			this.tabQueue.Size = new System.Drawing.Size(608, 343);
			this.tabQueue.TabIndex = 0;
			this.tabQueue.Text = "{0}";
			this.tabQueue.UseVisualStyleBackColor = true;
			// 
			// btnQueueBrowseDest
			// 
			this.btnQueueBrowseDest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueBrowseDest.Location = new System.Drawing.Point(575, 315);
			this.btnQueueBrowseDest.Name = "btnQueueBrowseDest";
			this.btnQueueBrowseDest.Size = new System.Drawing.Size(27, 23);
			this.btnQueueBrowseDest.TabIndex = 8;
			this.btnQueueBrowseDest.Text = "...";
			this.btnQueueBrowseDest.UseVisualStyleBackColor = true;
			this.btnQueueBrowseDest.Click += new System.EventHandler(this.btnQueueBrowseDest_Click);
			// 
			// txtDestDir
			// 
			this.txtDestDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDestDir.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDestDir.Location = new System.Drawing.Point(27, 316);
			this.txtDestDir.Name = "txtDestDir";
			this.txtDestDir.ReadOnly = true;
			this.txtDestDir.Size = new System.Drawing.Size(542, 21);
			this.txtDestDir.TabIndex = 0;
			// 
			// chkQueueSaveTo
			// 
			this.chkQueueSaveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkQueueSaveTo.AutoSize = true;
			this.chkQueueSaveTo.Location = new System.Drawing.Point(6, 321);
			this.chkQueueSaveTo.Name = "chkQueueSaveTo";
			this.chkQueueSaveTo.Size = new System.Drawing.Size(15, 14);
			this.chkQueueSaveTo.TabIndex = 7;
			this.chkQueueSaveTo.UseVisualStyleBackColor = true;
			this.chkQueueSaveTo.CheckedChanged += new System.EventHandler(this.chkQueueSaveTo_CheckedChanged);
			// 
			// btnQueueDown
			// 
			this.btnQueueDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnQueueDown.Image = global::ifme.hitoha.Properties.Resources.go_down;
			this.btnQueueDown.Location = new System.Drawing.Point(307, 6);
			this.btnQueueDown.Name = "btnQueueDown";
			this.btnQueueDown.Size = new System.Drawing.Size(24, 24);
			this.btnQueueDown.TabIndex = 5;
			this.btnQueueDown.UseVisualStyleBackColor = true;
			this.btnQueueDown.Click += new System.EventHandler(this.btnQueueDown_Click);
			// 
			// btnQueueUp
			// 
			this.btnQueueUp.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnQueueUp.Image = global::ifme.hitoha.Properties.Resources.go_up;
			this.btnQueueUp.Location = new System.Drawing.Point(277, 6);
			this.btnQueueUp.Name = "btnQueueUp";
			this.btnQueueUp.Size = new System.Drawing.Size(24, 24);
			this.btnQueueUp.TabIndex = 4;
			this.btnQueueUp.UseVisualStyleBackColor = true;
			this.btnQueueUp.Click += new System.EventHandler(this.btnQueueUp_Click);
			// 
			// btnQueueClear
			// 
			this.btnQueueClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueClear.Image = global::ifme.hitoha.Properties.Resources.list_clear;
			this.btnQueueClear.Location = new System.Drawing.Point(578, 6);
			this.btnQueueClear.Name = "btnQueueClear";
			this.btnQueueClear.Size = new System.Drawing.Size(24, 24);
			this.btnQueueClear.TabIndex = 3;
			this.btnQueueClear.UseVisualStyleBackColor = true;
			this.btnQueueClear.Click += new System.EventHandler(this.btnQueueClear_Click);
			// 
			// btnQueueRemove
			// 
			this.btnQueueRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueRemove.Image = global::ifme.hitoha.Properties.Resources.list_remove;
			this.btnQueueRemove.Location = new System.Drawing.Point(548, 6);
			this.btnQueueRemove.Name = "btnQueueRemove";
			this.btnQueueRemove.Size = new System.Drawing.Size(24, 24);
			this.btnQueueRemove.TabIndex = 2;
			this.btnQueueRemove.UseVisualStyleBackColor = true;
			this.btnQueueRemove.Click += new System.EventHandler(this.btnQueueRemove_Click);
			// 
			// btnQueueAdd
			// 
			this.btnQueueAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQueueAdd.Image = global::ifme.hitoha.Properties.Resources.list_add;
			this.btnQueueAdd.Location = new System.Drawing.Point(518, 6);
			this.btnQueueAdd.Name = "btnQueueAdd";
			this.btnQueueAdd.Size = new System.Drawing.Size(24, 24);
			this.btnQueueAdd.TabIndex = 1;
			this.btnQueueAdd.UseVisualStyleBackColor = true;
			this.btnQueueAdd.Click += new System.EventHandler(this.btnQueueAdd_Click);
			// 
			// lstQueue
			// 
			this.lstQueue.AllowDrop = true;
			this.lstQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colQueueFName,
            this.colQueueExt,
            this.colQueueCodec,
            this.colQueueRes,
            this.colQueueFPS,
            this.colQueueBitDepth,
            this.colQueuePath});
			this.lstQueue.FullRowSelect = true;
			this.lstQueue.Location = new System.Drawing.Point(6, 36);
			this.lstQueue.Name = "lstQueue";
			this.lstQueue.Size = new System.Drawing.Size(596, 274);
			this.lstQueue.TabIndex = 6;
			this.lstQueue.UseCompatibleStateImageBehavior = false;
			this.lstQueue.View = System.Windows.Forms.View.Details;
			this.lstQueue.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstQueue_DragDrop);
			this.lstQueue.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstQueue_DragEnter);
			// 
			// colQueueFName
			// 
			this.colQueueFName.Text = "";
			this.colQueueFName.Width = 230;
			// 
			// colQueueExt
			// 
			this.colQueueExt.Text = "";
			this.colQueueExt.Width = 40;
			// 
			// colQueueCodec
			// 
			this.colQueueCodec.Text = "";
			// 
			// colQueueRes
			// 
			this.colQueueRes.Text = "";
			this.colQueueRes.Width = 70;
			// 
			// colQueueFPS
			// 
			this.colQueueFPS.Text = "FPS";
			this.colQueueFPS.Width = 80;
			// 
			// colQueueBitDepth
			// 
			this.colQueueBitDepth.Text = "";
			// 
			// colQueuePath
			// 
			this.colQueuePath.Text = "";
			this.colQueuePath.Width = 400;
			// 
			// tabVideo
			// 
			this.tabVideo.Controls.Add(this.txtVideoAdvCmd);
			this.tabVideo.Controls.Add(this.lblVideoAdvCmd);
			this.tabVideo.Controls.Add(this.grpVideoRateCtrl);
			this.tabVideo.Controls.Add(this.grpVideoBasic);
			this.tabVideo.Location = new System.Drawing.Point(4, 22);
			this.tabVideo.Name = "tabVideo";
			this.tabVideo.Padding = new System.Windows.Forms.Padding(3);
			this.tabVideo.Size = new System.Drawing.Size(608, 343);
			this.tabVideo.TabIndex = 1;
			this.tabVideo.Text = "{0}";
			this.tabVideo.UseVisualStyleBackColor = true;
			// 
			// txtVideoAdvCmd
			// 
			this.txtVideoAdvCmd.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.txtVideoAdvCmd.Location = new System.Drawing.Point(6, 316);
			this.txtVideoAdvCmd.Name = "txtVideoAdvCmd";
			this.txtVideoAdvCmd.Size = new System.Drawing.Size(596, 21);
			this.txtVideoAdvCmd.TabIndex = 6;
			// 
			// lblVideoAdvCmd
			// 
			this.lblVideoAdvCmd.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblVideoAdvCmd.AutoSize = true;
			this.lblVideoAdvCmd.Location = new System.Drawing.Point(6, 300);
			this.lblVideoAdvCmd.Name = "lblVideoAdvCmd";
			this.lblVideoAdvCmd.Size = new System.Drawing.Size(27, 13);
			this.lblVideoAdvCmd.TabIndex = 0;
			this.lblVideoAdvCmd.Text = "{0}:";
			// 
			// grpVideoRateCtrl
			// 
			this.grpVideoRateCtrl.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpVideoRateCtrl.Controls.Add(this.lblVideoRateFL);
			this.grpVideoRateCtrl.Controls.Add(this.lblVideoRateFH);
			this.grpVideoRateCtrl.Controls.Add(this.trkVideoRate);
			this.grpVideoRateCtrl.Controls.Add(this.txtVideoRate);
			this.grpVideoRateCtrl.Controls.Add(this.lblVideoRateFactor);
			this.grpVideoRateCtrl.Controls.Add(this.cboVideoRateCtrl);
			this.grpVideoRateCtrl.Location = new System.Drawing.Point(6, 112);
			this.grpVideoRateCtrl.Name = "grpVideoRateCtrl";
			this.grpVideoRateCtrl.Size = new System.Drawing.Size(596, 185);
			this.grpVideoRateCtrl.TabIndex = 0;
			this.grpVideoRateCtrl.TabStop = false;
			this.grpVideoRateCtrl.Text = "{0}";
			// 
			// lblVideoRateFL
			// 
			this.lblVideoRateFL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblVideoRateFL.Location = new System.Drawing.Point(348, 136);
			this.lblVideoRateFL.Name = "lblVideoRateFL";
			this.lblVideoRateFL.Size = new System.Drawing.Size(200, 14);
			this.lblVideoRateFL.TabIndex = 0;
			this.lblVideoRateFL.Text = "({0}) 51";
			this.lblVideoRateFL.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblVideoRateFH
			// 
			this.lblVideoRateFH.Location = new System.Drawing.Point(45, 136);
			this.lblVideoRateFH.Name = "lblVideoRateFH";
			this.lblVideoRateFH.Size = new System.Drawing.Size(200, 14);
			this.lblVideoRateFH.TabIndex = 0;
			this.lblVideoRateFH.Text = "0 ({0})";
			// 
			// trkVideoRate
			// 
			this.trkVideoRate.BackColor = System.Drawing.Color.White;
			this.trkVideoRate.Location = new System.Drawing.Point(48, 88);
			this.trkVideoRate.Maximum = 51;
			this.trkVideoRate.Name = "trkVideoRate";
			this.trkVideoRate.Size = new System.Drawing.Size(500, 45);
			this.trkVideoRate.TabIndex = 5;
			this.trkVideoRate.Value = 26;
			this.trkVideoRate.ValueChanged += new System.EventHandler(this.trkVideoRate_ValueChanged);
			// 
			// txtVideoRate
			// 
			this.txtVideoRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtVideoRate.Location = new System.Drawing.Point(448, 61);
			this.txtVideoRate.Name = "txtVideoRate";
			this.txtVideoRate.Size = new System.Drawing.Size(100, 21);
			this.txtVideoRate.TabIndex = 4;
			this.txtVideoRate.Text = "26";
			// 
			// lblVideoRateFactor
			// 
			this.lblVideoRateFactor.AutoSize = true;
			this.lblVideoRateFactor.Location = new System.Drawing.Point(45, 64);
			this.lblVideoRateFactor.Name = "lblVideoRateFactor";
			this.lblVideoRateFactor.Size = new System.Drawing.Size(27, 13);
			this.lblVideoRateFactor.TabIndex = 0;
			this.lblVideoRateFactor.Text = "{0}:";
			// 
			// cboVideoRateCtrl
			// 
			this.cboVideoRateCtrl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboVideoRateCtrl.FormattingEnabled = true;
			this.cboVideoRateCtrl.Items.AddRange(new object[] {
            "Single pass, Ratefactor-based - crf",
            "Single pass, Quantizer-based - cqp",
            "Single pass, Bitrate-based - abr"});
			this.cboVideoRateCtrl.Location = new System.Drawing.Point(48, 34);
			this.cboVideoRateCtrl.Name = "cboVideoRateCtrl";
			this.cboVideoRateCtrl.Size = new System.Drawing.Size(500, 21);
			this.cboVideoRateCtrl.TabIndex = 3;
			this.cboVideoRateCtrl.SelectedIndexChanged += new System.EventHandler(this.cboVideoRateCtrl_SelectedIndexChanged);
			// 
			// grpVideoBasic
			// 
			this.grpVideoBasic.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpVideoBasic.Controls.Add(this.cboVideoTune);
			this.grpVideoBasic.Controls.Add(this.lblVideoTune);
			this.grpVideoBasic.Controls.Add(this.cboVideoPreset);
			this.grpVideoBasic.Controls.Add(this.lblVideoPreset);
			this.grpVideoBasic.Location = new System.Drawing.Point(6, 6);
			this.grpVideoBasic.Name = "grpVideoBasic";
			this.grpVideoBasic.Size = new System.Drawing.Size(596, 100);
			this.grpVideoBasic.TabIndex = 0;
			this.grpVideoBasic.TabStop = false;
			this.grpVideoBasic.Text = "{0}";
			// 
			// cboVideoTune
			// 
			this.cboVideoTune.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboVideoTune.FormattingEnabled = true;
			this.cboVideoTune.Items.AddRange(new object[] {
            "off",
            "psnr",
            "ssim",
            "zero-latency"});
			this.cboVideoTune.Location = new System.Drawing.Point(303, 48);
			this.cboVideoTune.Name = "cboVideoTune";
			this.cboVideoTune.Size = new System.Drawing.Size(121, 21);
			this.cboVideoTune.TabIndex = 2;
			// 
			// lblVideoTune
			// 
			this.lblVideoTune.AutoSize = true;
			this.lblVideoTune.Location = new System.Drawing.Point(300, 32);
			this.lblVideoTune.Name = "lblVideoTune";
			this.lblVideoTune.Size = new System.Drawing.Size(27, 13);
			this.lblVideoTune.TabIndex = 0;
			this.lblVideoTune.Text = "{0}:";
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
			this.cboVideoPreset.Location = new System.Drawing.Point(176, 48);
			this.cboVideoPreset.Name = "cboVideoPreset";
			this.cboVideoPreset.Size = new System.Drawing.Size(121, 21);
			this.cboVideoPreset.TabIndex = 1;
			// 
			// lblVideoPreset
			// 
			this.lblVideoPreset.AutoSize = true;
			this.lblVideoPreset.Location = new System.Drawing.Point(173, 32);
			this.lblVideoPreset.Name = "lblVideoPreset";
			this.lblVideoPreset.Size = new System.Drawing.Size(27, 13);
			this.lblVideoPreset.TabIndex = 0;
			this.lblVideoPreset.Text = "{0}:";
			// 
			// tabAudio
			// 
			this.tabAudio.Controls.Add(this.txtAudioCmd);
			this.tabAudio.Controls.Add(this.lblAudioCmdAdv);
			this.tabAudio.Controls.Add(this.grpAudioMode);
			this.tabAudio.Controls.Add(this.grpAudioQuality);
			this.tabAudio.Controls.Add(this.grpAudioFormat);
			this.tabAudio.Location = new System.Drawing.Point(4, 22);
			this.tabAudio.Name = "tabAudio";
			this.tabAudio.Padding = new System.Windows.Forms.Padding(3);
			this.tabAudio.Size = new System.Drawing.Size(608, 343);
			this.tabAudio.TabIndex = 2;
			this.tabAudio.Text = "{0}";
			this.tabAudio.UseVisualStyleBackColor = true;
			// 
			// txtAudioCmd
			// 
			this.txtAudioCmd.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.txtAudioCmd.Location = new System.Drawing.Point(6, 316);
			this.txtAudioCmd.Name = "txtAudioCmd";
			this.txtAudioCmd.Size = new System.Drawing.Size(596, 21);
			this.txtAudioCmd.TabIndex = 6;
			// 
			// lblAudioCmdAdv
			// 
			this.lblAudioCmdAdv.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAudioCmdAdv.AutoSize = true;
			this.lblAudioCmdAdv.Location = new System.Drawing.Point(6, 300);
			this.lblAudioCmdAdv.Name = "lblAudioCmdAdv";
			this.lblAudioCmdAdv.Size = new System.Drawing.Size(27, 13);
			this.lblAudioCmdAdv.TabIndex = 0;
			this.lblAudioCmdAdv.Text = "{0}:";
			// 
			// grpAudioMode
			// 
			this.grpAudioMode.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpAudioMode.Controls.Add(this.cboAudioMode);
			this.grpAudioMode.Location = new System.Drawing.Point(6, 198);
			this.grpAudioMode.Name = "grpAudioMode";
			this.grpAudioMode.Size = new System.Drawing.Size(596, 90);
			this.grpAudioMode.TabIndex = 0;
			this.grpAudioMode.TabStop = false;
			this.grpAudioMode.Text = "{0}";
			// 
			// cboAudioMode
			// 
			this.cboAudioMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioMode.FormattingEnabled = true;
			this.cboAudioMode.Location = new System.Drawing.Point(111, 35);
			this.cboAudioMode.Name = "cboAudioMode";
			this.cboAudioMode.Size = new System.Drawing.Size(375, 21);
			this.cboAudioMode.TabIndex = 5;
			// 
			// grpAudioQuality
			// 
			this.grpAudioQuality.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpAudioQuality.Controls.Add(this.lblAudioChan);
			this.grpAudioQuality.Controls.Add(this.cboAudioChan);
			this.grpAudioQuality.Controls.Add(this.lblAudioFreq);
			this.grpAudioQuality.Controls.Add(this.cboAudioFreq);
			this.grpAudioQuality.Controls.Add(this.cboAudioBitRate);
			this.grpAudioQuality.Controls.Add(this.lblAudioBitRate);
			this.grpAudioQuality.Location = new System.Drawing.Point(6, 102);
			this.grpAudioQuality.Name = "grpAudioQuality";
			this.grpAudioQuality.Size = new System.Drawing.Size(596, 90);
			this.grpAudioQuality.TabIndex = 0;
			this.grpAudioQuality.TabStop = false;
			this.grpAudioQuality.Text = "{0}";
			// 
			// lblAudioChan
			// 
			this.lblAudioChan.AutoSize = true;
			this.lblAudioChan.Location = new System.Drawing.Point(364, 27);
			this.lblAudioChan.Name = "lblAudioChan";
			this.lblAudioChan.Size = new System.Drawing.Size(27, 13);
			this.lblAudioChan.TabIndex = 0;
			this.lblAudioChan.Text = "{0}:";
			// 
			// cboAudioChan
			// 
			this.cboAudioChan.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboAudioChan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioChan.FormattingEnabled = true;
			this.cboAudioChan.Items.AddRange(new object[] {
            "Automatic",
            "Mono",
            "Stereo"});
			this.cboAudioChan.Location = new System.Drawing.Point(365, 43);
			this.cboAudioChan.Name = "cboAudioChan";
			this.cboAudioChan.Size = new System.Drawing.Size(121, 21);
			this.cboAudioChan.TabIndex = 4;
			// 
			// lblAudioFreq
			// 
			this.lblAudioFreq.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAudioFreq.AutoSize = true;
			this.lblAudioFreq.Location = new System.Drawing.Point(237, 27);
			this.lblAudioFreq.Name = "lblAudioFreq";
			this.lblAudioFreq.Size = new System.Drawing.Size(27, 13);
			this.lblAudioFreq.TabIndex = 0;
			this.lblAudioFreq.Text = "{0}:";
			// 
			// cboAudioFreq
			// 
			this.cboAudioFreq.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboAudioFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioFreq.FormattingEnabled = true;
			this.cboAudioFreq.Items.AddRange(new object[] {
            "96000",
            "48000",
            "44100",
            "32000",
            "24000",
            "22050",
            "16000",
            "12000",
            "8000"});
			this.cboAudioFreq.Location = new System.Drawing.Point(238, 43);
			this.cboAudioFreq.Name = "cboAudioFreq";
			this.cboAudioFreq.Size = new System.Drawing.Size(121, 21);
			this.cboAudioFreq.TabIndex = 3;
			// 
			// cboAudioBitRate
			// 
			this.cboAudioBitRate.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboAudioBitRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioBitRate.FormattingEnabled = true;
			this.cboAudioBitRate.Location = new System.Drawing.Point(111, 43);
			this.cboAudioBitRate.Name = "cboAudioBitRate";
			this.cboAudioBitRate.Size = new System.Drawing.Size(121, 21);
			this.cboAudioBitRate.TabIndex = 2;
			// 
			// lblAudioBitRate
			// 
			this.lblAudioBitRate.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAudioBitRate.AutoSize = true;
			this.lblAudioBitRate.Location = new System.Drawing.Point(109, 27);
			this.lblAudioBitRate.Name = "lblAudioBitRate";
			this.lblAudioBitRate.Size = new System.Drawing.Size(27, 13);
			this.lblAudioBitRate.TabIndex = 0;
			this.lblAudioBitRate.Text = "{0}:";
			// 
			// grpAudioFormat
			// 
			this.grpAudioFormat.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.grpAudioFormat.Controls.Add(this.lblAudioFInfo);
			this.grpAudioFormat.Controls.Add(this.lblAudioFormat);
			this.grpAudioFormat.Controls.Add(this.cboAudioFormat);
			this.grpAudioFormat.Location = new System.Drawing.Point(6, 6);
			this.grpAudioFormat.Name = "grpAudioFormat";
			this.grpAudioFormat.Size = new System.Drawing.Size(596, 90);
			this.grpAudioFormat.TabIndex = 0;
			this.grpAudioFormat.TabStop = false;
			this.grpAudioFormat.Text = "{0}";
			// 
			// lblAudioFInfo
			// 
			this.lblAudioFInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAudioFInfo.Location = new System.Drawing.Point(184, 44);
			this.lblAudioFInfo.Name = "lblAudioFInfo";
			this.lblAudioFInfo.Size = new System.Drawing.Size(302, 42);
			this.lblAudioFInfo.TabIndex = 0;
			this.lblAudioFInfo.Text = "{0}\r\n{1}\r\n{2}";
			// 
			// lblAudioFormat
			// 
			this.lblAudioFormat.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAudioFormat.Location = new System.Drawing.Point(108, 44);
			this.lblAudioFormat.Name = "lblAudioFormat";
			this.lblAudioFormat.Size = new System.Drawing.Size(70, 42);
			this.lblAudioFormat.TabIndex = 0;
			this.lblAudioFormat.Text = "{0}:\r\n{1}:\r\n{2}:";
			this.lblAudioFormat.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cboAudioFormat
			// 
			this.cboAudioFormat.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cboAudioFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAudioFormat.FormattingEnabled = true;
			this.cboAudioFormat.Location = new System.Drawing.Point(111, 20);
			this.cboAudioFormat.Name = "cboAudioFormat";
			this.cboAudioFormat.Size = new System.Drawing.Size(375, 21);
			this.cboAudioFormat.TabIndex = 1;
			this.cboAudioFormat.SelectedIndexChanged += new System.EventHandler(this.cboAudioFormat_SelectedIndexChanged);
			// 
			// tabSubtitle
			// 
			this.tabSubtitle.Controls.Add(this.lblSubtitleNotice);
			this.tabSubtitle.Controls.Add(this.btnSubDown);
			this.tabSubtitle.Controls.Add(this.btnSubUp);
			this.tabSubtitle.Controls.Add(this.chkSubEnable);
			this.tabSubtitle.Controls.Add(this.lblSubLang);
			this.tabSubtitle.Controls.Add(this.cboSubLang);
			this.tabSubtitle.Controls.Add(this.btnSubClear);
			this.tabSubtitle.Controls.Add(this.btnSubRemove);
			this.tabSubtitle.Controls.Add(this.btnSubAdd);
			this.tabSubtitle.Controls.Add(this.lstSubtitle);
			this.tabSubtitle.Location = new System.Drawing.Point(4, 22);
			this.tabSubtitle.Name = "tabSubtitle";
			this.tabSubtitle.Padding = new System.Windows.Forms.Padding(3);
			this.tabSubtitle.Size = new System.Drawing.Size(608, 343);
			this.tabSubtitle.TabIndex = 3;
			this.tabSubtitle.Text = "{0}";
			this.tabSubtitle.UseVisualStyleBackColor = true;
			// 
			// lblSubtitleNotice
			// 
			this.lblSubtitleNotice.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblSubtitleNotice.Location = new System.Drawing.Point(104, 148);
			this.lblSubtitleNotice.Name = "lblSubtitleNotice";
			this.lblSubtitleNotice.Size = new System.Drawing.Size(400, 46);
			this.lblSubtitleNotice.TabIndex = 0;
			this.lblSubtitleNotice.Text = "{0}\r\n{1}";
			this.lblSubtitleNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnSubDown
			// 
			this.btnSubDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnSubDown.Image = global::ifme.hitoha.Properties.Resources.go_down;
			this.btnSubDown.Location = new System.Drawing.Point(307, 6);
			this.btnSubDown.Name = "btnSubDown";
			this.btnSubDown.Size = new System.Drawing.Size(24, 24);
			this.btnSubDown.TabIndex = 6;
			this.btnSubDown.UseVisualStyleBackColor = true;
			this.btnSubDown.Visible = false;
			this.btnSubDown.Click += new System.EventHandler(this.btnSubDown_Click);
			// 
			// btnSubUp
			// 
			this.btnSubUp.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnSubUp.Image = global::ifme.hitoha.Properties.Resources.go_up;
			this.btnSubUp.Location = new System.Drawing.Point(277, 6);
			this.btnSubUp.Name = "btnSubUp";
			this.btnSubUp.Size = new System.Drawing.Size(24, 24);
			this.btnSubUp.TabIndex = 5;
			this.btnSubUp.UseVisualStyleBackColor = true;
			this.btnSubUp.Visible = false;
			this.btnSubUp.Click += new System.EventHandler(this.btnSubUp_Click);
			// 
			// chkSubEnable
			// 
			this.chkSubEnable.AutoSize = true;
			this.chkSubEnable.Location = new System.Drawing.Point(6, 11);
			this.chkSubEnable.Name = "chkSubEnable";
			this.chkSubEnable.Size = new System.Drawing.Size(42, 17);
			this.chkSubEnable.TabIndex = 1;
			this.chkSubEnable.Text = "{0}";
			this.chkSubEnable.UseVisualStyleBackColor = true;
			this.chkSubEnable.CheckedChanged += new System.EventHandler(this.chkSubEnable_CheckedChanged);
			// 
			// lblSubLang
			// 
			this.lblSubLang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblSubLang.AutoSize = true;
			this.lblSubLang.Location = new System.Drawing.Point(6, 319);
			this.lblSubLang.Name = "lblSubLang";
			this.lblSubLang.Size = new System.Drawing.Size(27, 13);
			this.lblSubLang.TabIndex = 0;
			this.lblSubLang.Text = "{0}:";
			this.lblSubLang.Visible = false;
			// 
			// cboSubLang
			// 
			this.cboSubLang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cboSubLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSubLang.FormattingEnabled = true;
			this.cboSubLang.Location = new System.Drawing.Point(92, 316);
			this.cboSubLang.Name = "cboSubLang";
			this.cboSubLang.Size = new System.Drawing.Size(510, 21);
			this.cboSubLang.TabIndex = 8;
			this.cboSubLang.Visible = false;
			this.cboSubLang.SelectedIndexChanged += new System.EventHandler(this.cboSubLang_SelectedIndexChanged);
			// 
			// btnSubClear
			// 
			this.btnSubClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubClear.Image = global::ifme.hitoha.Properties.Resources.list_clear;
			this.btnSubClear.Location = new System.Drawing.Point(578, 6);
			this.btnSubClear.Name = "btnSubClear";
			this.btnSubClear.Size = new System.Drawing.Size(24, 24);
			this.btnSubClear.TabIndex = 4;
			this.btnSubClear.UseVisualStyleBackColor = true;
			this.btnSubClear.Visible = false;
			this.btnSubClear.Click += new System.EventHandler(this.btnSubClear_Click);
			// 
			// btnSubRemove
			// 
			this.btnSubRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubRemove.Image = global::ifme.hitoha.Properties.Resources.list_remove;
			this.btnSubRemove.Location = new System.Drawing.Point(548, 6);
			this.btnSubRemove.Name = "btnSubRemove";
			this.btnSubRemove.Size = new System.Drawing.Size(24, 24);
			this.btnSubRemove.TabIndex = 3;
			this.btnSubRemove.UseVisualStyleBackColor = true;
			this.btnSubRemove.Visible = false;
			this.btnSubRemove.Click += new System.EventHandler(this.btnSubRemove_Click);
			// 
			// btnSubAdd
			// 
			this.btnSubAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubAdd.Image = global::ifme.hitoha.Properties.Resources.list_add;
			this.btnSubAdd.Location = new System.Drawing.Point(518, 6);
			this.btnSubAdd.Name = "btnSubAdd";
			this.btnSubAdd.Size = new System.Drawing.Size(24, 24);
			this.btnSubAdd.TabIndex = 2;
			this.btnSubAdd.UseVisualStyleBackColor = true;
			this.btnSubAdd.Visible = false;
			this.btnSubAdd.Click += new System.EventHandler(this.btnSubAdd_Click);
			// 
			// lstSubtitle
			// 
			this.lstSubtitle.AllowDrop = true;
			this.lstSubtitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstSubtitle.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSubFile,
            this.colSubExt,
            this.colSubLang,
            this.colSubPath});
			this.lstSubtitle.FullRowSelect = true;
			this.lstSubtitle.Location = new System.Drawing.Point(6, 36);
			this.lstSubtitle.MultiSelect = false;
			this.lstSubtitle.Name = "lstSubtitle";
			this.lstSubtitle.Size = new System.Drawing.Size(596, 274);
			this.lstSubtitle.TabIndex = 7;
			this.lstSubtitle.UseCompatibleStateImageBehavior = false;
			this.lstSubtitle.View = System.Windows.Forms.View.Details;
			this.lstSubtitle.Visible = false;
			this.lstSubtitle.SelectedIndexChanged += new System.EventHandler(this.lstSubtitle_SelectedIndexChanged);
			this.lstSubtitle.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstSubtitle_DragDrop);
			this.lstSubtitle.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstSubtitle_DragEnter);
			// 
			// colSubFile
			// 
			this.colSubFile.Text = "";
			this.colSubFile.Width = 240;
			// 
			// colSubExt
			// 
			this.colSubExt.Text = "";
			this.colSubExt.Width = 40;
			// 
			// colSubLang
			// 
			this.colSubLang.Text = "";
			this.colSubLang.Width = 140;
			// 
			// colSubPath
			// 
			this.colSubPath.Text = "";
			this.colSubPath.Width = 172;
			// 
			// tabAttachment
			// 
			this.tabAttachment.Controls.Add(this.lblAttachNotice);
			this.tabAttachment.Controls.Add(this.chkAttachEnable);
			this.tabAttachment.Controls.Add(this.txtAttachDesc);
			this.tabAttachment.Controls.Add(this.lblAttachDesc);
			this.tabAttachment.Controls.Add(this.lstAttachment);
			this.tabAttachment.Controls.Add(this.btnAttachClear);
			this.tabAttachment.Controls.Add(this.btnAttachRemove);
			this.tabAttachment.Controls.Add(this.btnAttachAdd);
			this.tabAttachment.Location = new System.Drawing.Point(4, 22);
			this.tabAttachment.Name = "tabAttachment";
			this.tabAttachment.Padding = new System.Windows.Forms.Padding(3);
			this.tabAttachment.Size = new System.Drawing.Size(608, 343);
			this.tabAttachment.TabIndex = 4;
			this.tabAttachment.Text = "{0}";
			this.tabAttachment.UseVisualStyleBackColor = true;
			// 
			// lblAttachNotice
			// 
			this.lblAttachNotice.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblAttachNotice.Location = new System.Drawing.Point(104, 148);
			this.lblAttachNotice.Name = "lblAttachNotice";
			this.lblAttachNotice.Size = new System.Drawing.Size(400, 46);
			this.lblAttachNotice.TabIndex = 0;
			this.lblAttachNotice.Text = "{0}\r\n{1}";
			this.lblAttachNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// chkAttachEnable
			// 
			this.chkAttachEnable.AutoSize = true;
			this.chkAttachEnable.Enabled = false;
			this.chkAttachEnable.Location = new System.Drawing.Point(6, 11);
			this.chkAttachEnable.Name = "chkAttachEnable";
			this.chkAttachEnable.Size = new System.Drawing.Size(42, 17);
			this.chkAttachEnable.TabIndex = 1;
			this.chkAttachEnable.Text = "{0}";
			this.chkAttachEnable.UseVisualStyleBackColor = true;
			this.chkAttachEnable.CheckedChanged += new System.EventHandler(this.chkAttachEnable_CheckedChanged);
			// 
			// txtAttachDesc
			// 
			this.txtAttachDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAttachDesc.Location = new System.Drawing.Point(100, 316);
			this.txtAttachDesc.Name = "txtAttachDesc";
			this.txtAttachDesc.Size = new System.Drawing.Size(502, 21);
			this.txtAttachDesc.TabIndex = 6;
			this.txtAttachDesc.Text = "No";
			this.txtAttachDesc.Visible = false;
			// 
			// lblAttachDesc
			// 
			this.lblAttachDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblAttachDesc.AutoSize = true;
			this.lblAttachDesc.Location = new System.Drawing.Point(6, 319);
			this.lblAttachDesc.Name = "lblAttachDesc";
			this.lblAttachDesc.Size = new System.Drawing.Size(27, 13);
			this.lblAttachDesc.TabIndex = 0;
			this.lblAttachDesc.Text = "{0}:";
			this.lblAttachDesc.Visible = false;
			// 
			// lstAttachment
			// 
			this.lstAttachment.AllowDrop = true;
			this.lstAttachment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstAttachment.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAttachFile,
            this.colAttachExt,
            this.colAttachMime,
            this.colAttachPath});
			this.lstAttachment.FullRowSelect = true;
			this.lstAttachment.Location = new System.Drawing.Point(6, 36);
			this.lstAttachment.Name = "lstAttachment";
			this.lstAttachment.Size = new System.Drawing.Size(596, 274);
			this.lstAttachment.TabIndex = 5;
			this.lstAttachment.UseCompatibleStateImageBehavior = false;
			this.lstAttachment.View = System.Windows.Forms.View.Details;
			this.lstAttachment.Visible = false;
			this.lstAttachment.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstAttachment_DragDrop);
			this.lstAttachment.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstAttachment_DragEnter);
			// 
			// colAttachFile
			// 
			this.colAttachFile.Text = "";
			this.colAttachFile.Width = 240;
			// 
			// colAttachExt
			// 
			this.colAttachExt.Text = "";
			this.colAttachExt.Width = 40;
			// 
			// colAttachMime
			// 
			this.colAttachMime.Text = "";
			this.colAttachMime.Width = 140;
			// 
			// colAttachPath
			// 
			this.colAttachPath.Text = "";
			this.colAttachPath.Width = 172;
			// 
			// btnAttachClear
			// 
			this.btnAttachClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAttachClear.Image = global::ifme.hitoha.Properties.Resources.list_clear;
			this.btnAttachClear.Location = new System.Drawing.Point(578, 6);
			this.btnAttachClear.Name = "btnAttachClear";
			this.btnAttachClear.Size = new System.Drawing.Size(24, 24);
			this.btnAttachClear.TabIndex = 4;
			this.btnAttachClear.UseVisualStyleBackColor = true;
			this.btnAttachClear.Visible = false;
			this.btnAttachClear.Click += new System.EventHandler(this.btnAttachClear_Click);
			// 
			// btnAttachRemove
			// 
			this.btnAttachRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAttachRemove.Image = global::ifme.hitoha.Properties.Resources.list_remove;
			this.btnAttachRemove.Location = new System.Drawing.Point(548, 6);
			this.btnAttachRemove.Name = "btnAttachRemove";
			this.btnAttachRemove.Size = new System.Drawing.Size(24, 24);
			this.btnAttachRemove.TabIndex = 3;
			this.btnAttachRemove.UseVisualStyleBackColor = true;
			this.btnAttachRemove.Visible = false;
			this.btnAttachRemove.Click += new System.EventHandler(this.btnAttachRemove_Click);
			// 
			// btnAttachAdd
			// 
			this.btnAttachAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAttachAdd.Image = global::ifme.hitoha.Properties.Resources.list_add;
			this.btnAttachAdd.Location = new System.Drawing.Point(518, 6);
			this.btnAttachAdd.Name = "btnAttachAdd";
			this.btnAttachAdd.Size = new System.Drawing.Size(24, 24);
			this.btnAttachAdd.TabIndex = 2;
			this.btnAttachAdd.UseVisualStyleBackColor = true;
			this.btnAttachAdd.Visible = false;
			this.btnAttachAdd.Click += new System.EventHandler(this.btnAttachAdd_Click);
			// 
			// tabStatus
			// 
			this.tabStatus.Controls.Add(this.rtfLog);
			this.tabStatus.Location = new System.Drawing.Point(4, 22);
			this.tabStatus.Name = "tabStatus";
			this.tabStatus.Padding = new System.Windows.Forms.Padding(3);
			this.tabStatus.Size = new System.Drawing.Size(608, 343);
			this.tabStatus.TabIndex = 5;
			this.tabStatus.Text = "{0}";
			this.tabStatus.UseVisualStyleBackColor = true;
			// 
			// rtfLog
			// 
			this.rtfLog.BackColor = System.Drawing.Color.Black;
			this.rtfLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtfLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtfLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtfLog.ForeColor = System.Drawing.Color.LightGray;
			this.rtfLog.Location = new System.Drawing.Point(3, 3);
			this.rtfLog.Name = "rtfLog";
			this.rtfLog.ReadOnly = true;
			this.rtfLog.Size = new System.Drawing.Size(602, 337);
			this.rtfLog.TabIndex = 1;
			this.rtfLog.Text = "";
			this.rtfLog.TextChanged += new System.EventHandler(this.rtfLog_TextChanged);
			this.rtfLog.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rtfLog_KeyUp);
			// 
			// pictBannerLeft
			// 
			this.pictBannerLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictBannerLeft.BackColor = System.Drawing.Color.Black;
			this.pictBannerLeft.ErrorImage = null;
			this.pictBannerLeft.Image = global::ifme.hitoha.Properties.Resources.BannerLeft;
			this.pictBannerLeft.InitialImage = null;
			this.pictBannerLeft.Location = new System.Drawing.Point(0, 0);
			this.pictBannerLeft.Name = "pictBannerLeft";
			this.pictBannerLeft.Size = new System.Drawing.Size(640, 64);
			this.pictBannerLeft.TabIndex = 35;
			this.pictBannerLeft.TabStop = false;
			// 
			// pictBannerRight
			// 
			this.pictBannerRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictBannerRight.BackColor = System.Drawing.Color.Transparent;
			this.pictBannerRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictBannerRight.ErrorImage = null;
			this.pictBannerRight.Image = global::ifme.hitoha.Properties.Resources.BannerRight;
			this.pictBannerRight.InitialImage = null;
			this.pictBannerRight.Location = new System.Drawing.Point(6, 0);
			this.pictBannerRight.Name = "pictBannerRight";
			this.pictBannerRight.Size = new System.Drawing.Size(634, 64);
			this.pictBannerRight.TabIndex = 36;
			this.pictBannerRight.TabStop = false;
			// 
			// btnStop
			// 
			this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStop.Location = new System.Drawing.Point(528, 445);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(100, 23);
			this.btnStop.TabIndex = 37;
			this.btnStop.Text = "{0}";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Visible = false;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnResume
			// 
			this.btnResume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnResume.Location = new System.Drawing.Point(422, 445);
			this.btnResume.Name = "btnResume";
			this.btnResume.Size = new System.Drawing.Size(100, 23);
			this.btnResume.TabIndex = 38;
			this.btnResume.Text = "{0}";
			this.btnResume.UseVisualStyleBackColor = true;
			this.btnResume.Visible = false;
			this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
			// 
			// BGThread
			// 
			this.BGThread.WorkerSupportsCancellation = true;
			this.BGThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BGThread_DoWork);
			this.BGThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BGThread_RunWorkerCompleted);
			// 
			// proTip
			// 
			this.proTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.pictBannerRight);
			this.Controls.Add(this.pictBannerLeft);
			this.Controls.Add(this.btnOptions);
			this.Controls.Add(this.btnAbout);
			this.Controls.Add(this.btnPause);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.tabEncoding);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnResume);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(656, 518);
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "{0}";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.Shown += new System.EventHandler(this.frmMain_Shown);
			this.tabEncoding.ResumeLayout(false);
			this.tabQueue.ResumeLayout(false);
			this.tabQueue.PerformLayout();
			this.tabVideo.ResumeLayout(false);
			this.tabVideo.PerformLayout();
			this.grpVideoRateCtrl.ResumeLayout(false);
			this.grpVideoRateCtrl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkVideoRate)).EndInit();
			this.grpVideoBasic.ResumeLayout(false);
			this.grpVideoBasic.PerformLayout();
			this.tabAudio.ResumeLayout(false);
			this.tabAudio.PerformLayout();
			this.grpAudioMode.ResumeLayout(false);
			this.grpAudioQuality.ResumeLayout(false);
			this.grpAudioQuality.PerformLayout();
			this.grpAudioFormat.ResumeLayout(false);
			this.tabSubtitle.ResumeLayout(false);
			this.tabSubtitle.PerformLayout();
			this.tabAttachment.ResumeLayout(false);
			this.tabAttachment.PerformLayout();
			this.tabStatus.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictBannerLeft)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictBannerRight)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOptions;
		private System.Windows.Forms.Button btnAbout;
		private System.Windows.Forms.Button btnPause;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.TabControl tabEncoding;
		private System.Windows.Forms.TabPage tabQueue;
		private System.Windows.Forms.Button btnQueueBrowseDest;
		private System.Windows.Forms.TextBox txtDestDir;
		private System.Windows.Forms.CheckBox chkQueueSaveTo;
		private System.Windows.Forms.Button btnQueueDown;
		private System.Windows.Forms.Button btnQueueUp;
		private System.Windows.Forms.Button btnQueueClear;
		private System.Windows.Forms.Button btnQueueRemove;
		private System.Windows.Forms.Button btnQueueAdd;
		private System.Windows.Forms.ListView lstQueue;
		private System.Windows.Forms.ColumnHeader colQueueFName;
		private System.Windows.Forms.ColumnHeader colQueueExt;
		private System.Windows.Forms.ColumnHeader colQueueCodec;
		private System.Windows.Forms.ColumnHeader colQueueRes;
		private System.Windows.Forms.ColumnHeader colQueueBitDepth;
		private System.Windows.Forms.ColumnHeader colQueuePath;
		private System.Windows.Forms.TabPage tabVideo;
		private System.Windows.Forms.TextBox txtVideoAdvCmd;
		private System.Windows.Forms.Label lblVideoAdvCmd;
		private System.Windows.Forms.GroupBox grpVideoRateCtrl;
		private System.Windows.Forms.Label lblVideoRateFL;
		private System.Windows.Forms.Label lblVideoRateFH;
		private System.Windows.Forms.TrackBar trkVideoRate;
		private System.Windows.Forms.TextBox txtVideoRate;
		private System.Windows.Forms.Label lblVideoRateFactor;
		private System.Windows.Forms.ComboBox cboVideoRateCtrl;
		private System.Windows.Forms.GroupBox grpVideoBasic;
		private System.Windows.Forms.ComboBox cboVideoTune;
		private System.Windows.Forms.Label lblVideoTune;
		private System.Windows.Forms.ComboBox cboVideoPreset;
		private System.Windows.Forms.Label lblVideoPreset;
		private System.Windows.Forms.TabPage tabAudio;
		private System.Windows.Forms.TextBox txtAudioCmd;
		private System.Windows.Forms.Label lblAudioCmdAdv;
		private System.Windows.Forms.GroupBox grpAudioMode;
		private System.Windows.Forms.ComboBox cboAudioMode;
		private System.Windows.Forms.GroupBox grpAudioQuality;
		private System.Windows.Forms.Label lblAudioChan;
		private System.Windows.Forms.ComboBox cboAudioChan;
		private System.Windows.Forms.Label lblAudioFreq;
		private System.Windows.Forms.ComboBox cboAudioFreq;
		private System.Windows.Forms.ComboBox cboAudioBitRate;
		private System.Windows.Forms.Label lblAudioBitRate;
		private System.Windows.Forms.GroupBox grpAudioFormat;
		private System.Windows.Forms.Label lblAudioFInfo;
		private System.Windows.Forms.Label lblAudioFormat;
		private System.Windows.Forms.ComboBox cboAudioFormat;
		private System.Windows.Forms.TabPage tabSubtitle;
		private System.Windows.Forms.Label lblSubtitleNotice;
		private System.Windows.Forms.Button btnSubDown;
		private System.Windows.Forms.Button btnSubUp;
		private System.Windows.Forms.CheckBox chkSubEnable;
		private System.Windows.Forms.Label lblSubLang;
		private System.Windows.Forms.ComboBox cboSubLang;
		private System.Windows.Forms.Button btnSubClear;
		private System.Windows.Forms.Button btnSubRemove;
		private System.Windows.Forms.Button btnSubAdd;
		private System.Windows.Forms.ListView lstSubtitle;
		private System.Windows.Forms.ColumnHeader colSubFile;
		private System.Windows.Forms.ColumnHeader colSubExt;
		private System.Windows.Forms.ColumnHeader colSubLang;
		private System.Windows.Forms.ColumnHeader colSubPath;
		private System.Windows.Forms.TabPage tabAttachment;
		private System.Windows.Forms.Label lblAttachNotice;
		private System.Windows.Forms.CheckBox chkAttachEnable;
		private System.Windows.Forms.TextBox txtAttachDesc;
		private System.Windows.Forms.Label lblAttachDesc;
		private System.Windows.Forms.ListView lstAttachment;
		private System.Windows.Forms.ColumnHeader colAttachFile;
		private System.Windows.Forms.ColumnHeader colAttachExt;
		private System.Windows.Forms.ColumnHeader colAttachMime;
		private System.Windows.Forms.ColumnHeader colAttachPath;
		private System.Windows.Forms.Button btnAttachClear;
		private System.Windows.Forms.Button btnAttachRemove;
		private System.Windows.Forms.Button btnAttachAdd;
		private System.Windows.Forms.TabPage tabStatus;
		private System.Windows.Forms.PictureBox pictBannerLeft;
		private System.Windows.Forms.PictureBox pictBannerRight;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnResume;
		private System.ComponentModel.BackgroundWorker BGThread;
		private System.Windows.Forms.RichTextBox rtfLog;
		private System.Windows.Forms.ToolTip proTip;
		private System.Windows.Forms.ColumnHeader colQueueFPS;
	}
}