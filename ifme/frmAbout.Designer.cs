namespace ifme
{
    partial class frmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.pbBanner = new System.Windows.Forms.PictureBox();
            this.lblMainTitle = new System.Windows.Forms.Label();
            this.lblMainVersion = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblDevTtitle = new System.Windows.Forms.Label();
            this.lblDonateETH = new System.Windows.Forms.Label();
            this.lblTechInfo = new System.Windows.Forms.Label();
            this.lblTechTitle = new System.Windows.Forms.Label();
            this.lblArtInfo = new System.Windows.Forms.Label();
            this.lblArtTitle = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblDonateBTC = new System.Windows.Forms.Label();
            this.lblDonatePP = new System.Windows.Forms.Label();
            this.lblDevInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbBanner)).BeginInit();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbBanner
            // 
            this.pbBanner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbBanner.Location = new System.Drawing.Point(0, 0);
            this.pbBanner.Name = "pbBanner";
            this.pbBanner.Size = new System.Drawing.Size(600, 200);
            this.pbBanner.TabIndex = 0;
            this.pbBanner.TabStop = false;
            // 
            // lblMainTitle
            // 
            this.lblMainTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMainTitle.Font = new System.Drawing.Font("Tahoma", 16F);
            this.lblMainTitle.Location = new System.Drawing.Point(1, 1);
            this.lblMainTitle.Name = "lblMainTitle";
            this.lblMainTitle.Size = new System.Drawing.Size(598, 48);
            this.lblMainTitle.TabIndex = 0;
            this.lblMainTitle.Text = "IFME";
            this.lblMainTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMainVersion
            // 
            this.lblMainVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMainVersion.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblMainVersion.Location = new System.Drawing.Point(1, 48);
            this.lblMainVersion.Name = "lblMainVersion";
            this.lblMainVersion.Size = new System.Drawing.Size(598, 24);
            this.lblMainVersion.TabIndex = 1;
            this.lblMainVersion.Text = "Version";
            this.lblMainVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.lblDevInfo);
            this.panelMain.Controls.Add(this.lblDonatePP);
            this.panelMain.Controls.Add(this.lblDonateBTC);
            this.panelMain.Controls.Add(this.lblInfo);
            this.panelMain.Controls.Add(this.lblArtInfo);
            this.panelMain.Controls.Add(this.lblArtTitle);
            this.panelMain.Controls.Add(this.lblTechInfo);
            this.panelMain.Controls.Add(this.lblTechTitle);
            this.panelMain.Controls.Add(this.lblDonateETH);
            this.panelMain.Controls.Add(this.lblDevTtitle);
            this.panelMain.Controls.Add(this.lblMainVersion);
            this.panelMain.Controls.Add(this.lblMainTitle);
            this.panelMain.Location = new System.Drawing.Point(0, 200);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(600, 400);
            this.panelMain.TabIndex = 1;
            // 
            // lblDevTtitle
            // 
            this.lblDevTtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDevTtitle.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lblDevTtitle.Location = new System.Drawing.Point(1, 80);
            this.lblDevTtitle.Name = "lblDevTtitle";
            this.lblDevTtitle.Size = new System.Drawing.Size(598, 20);
            this.lblDevTtitle.TabIndex = 2;
            this.lblDevTtitle.Text = "Developed by";
            this.lblDevTtitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lblDonateETH
            // 
            this.lblDonateETH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDonateETH.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblDonateETH.ForeColor = System.Drawing.Color.Blue;
            this.lblDonateETH.Location = new System.Drawing.Point(1, 371);
            this.lblDonateETH.Name = "lblDonateETH";
            this.lblDonateETH.Size = new System.Drawing.Size(598, 20);
            this.lblDonateETH.TabIndex = 11;
            this.lblDonateETH.Text = "Ethereum: 0xAdd9ba89B601e7CB5B3602643337B9db8c90EFe0";
            this.lblDonateETH.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDonateETH.Click += new System.EventHandler(this.lblDonateETH_Click);
            // 
            // lblTechInfo
            // 
            this.lblTechInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTechInfo.Location = new System.Drawing.Point(1, 140);
            this.lblTechInfo.Name = "lblTechInfo";
            this.lblTechInfo.Size = new System.Drawing.Size(598, 20);
            this.lblTechInfo.TabIndex = 5;
            this.lblTechInfo.Text = "FFmpeg, MulticoreWare, VideoLAN, Xiph.org, Google, Nero AG, Mkvtoolnix, GPAC";
            this.lblTechInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblTechTitle
            // 
            this.lblTechTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTechTitle.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lblTechTitle.Location = new System.Drawing.Point(1, 120);
            this.lblTechTitle.Name = "lblTechTitle";
            this.lblTechTitle.Size = new System.Drawing.Size(598, 20);
            this.lblTechTitle.TabIndex = 4;
            this.lblTechTitle.Text = "Technologies used";
            this.lblTechTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lblArtInfo
            // 
            this.lblArtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblArtInfo.Location = new System.Drawing.Point(1, 180);
            this.lblArtInfo.Name = "lblArtInfo";
            this.lblArtInfo.Size = new System.Drawing.Size(598, 48);
            this.lblArtInfo.TabIndex = 7;
            this.lblArtInfo.Text = "Adeq @ http://fb.com/liyana.0426\r\n53C aka Ray-en @ http://53c.deviantart.com/\r\nOx" +
    "ygen Icon @ https://github.com/pasnox/oxygen-icons-png";
            this.lblArtInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblArtTitle
            // 
            this.lblArtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblArtTitle.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lblArtTitle.Location = new System.Drawing.Point(1, 160);
            this.lblArtTitle.Name = "lblArtTitle";
            this.lblArtTitle.Size = new System.Drawing.Size(598, 20);
            this.lblArtTitle.TabIndex = 6;
            this.lblArtTitle.Text = "Artwork by";
            this.lblArtTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblInfo.Location = new System.Drawing.Point(1, 228);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(598, 103);
            this.lblInfo.TabIndex = 8;
            this.lblInfo.Text = resources.GetString("lblInfo.Text");
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDonateBTC
            // 
            this.lblDonateBTC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDonateBTC.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblDonateBTC.ForeColor = System.Drawing.Color.Blue;
            this.lblDonateBTC.Location = new System.Drawing.Point(1, 351);
            this.lblDonateBTC.Name = "lblDonateBTC";
            this.lblDonateBTC.Size = new System.Drawing.Size(598, 20);
            this.lblDonateBTC.TabIndex = 10;
            this.lblDonateBTC.Text = "Bitcoin: 12LWHDCPShFvYh6vxxeMsntejAUm8y8rFN";
            this.lblDonateBTC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDonateBTC.Click += new System.EventHandler(this.lblDonateBTC_Click);
            // 
            // lblDonatePP
            // 
            this.lblDonatePP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDonatePP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblDonatePP.ForeColor = System.Drawing.Color.Blue;
            this.lblDonatePP.Location = new System.Drawing.Point(1, 331);
            this.lblDonatePP.Name = "lblDonatePP";
            this.lblDonatePP.Size = new System.Drawing.Size(598, 20);
            this.lblDonatePP.TabIndex = 9;
            this.lblDonatePP.Text = "Paypal: ilham92_sakura@yahoo.com";
            this.lblDonatePP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDonatePP.Click += new System.EventHandler(this.lblDonatePP_Click);
            // 
            // lblDevInfo
            // 
            this.lblDevInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDevInfo.Location = new System.Drawing.Point(1, 100);
            this.lblDevInfo.Name = "lblDevInfo";
            this.lblDevInfo.Size = new System.Drawing.Size(598, 20);
            this.lblDevInfo.TabIndex = 3;
            this.lblDevInfo.Text = "Anime4000";
            this.lblDevInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 600);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.pbBanner);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbBanner)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbBanner;
        private System.Windows.Forms.Label lblMainTitle;
        private System.Windows.Forms.Label lblMainVersion;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label lblDevTtitle;
        private System.Windows.Forms.Label lblDonateETH;
        private System.Windows.Forms.Label lblArtInfo;
        private System.Windows.Forms.Label lblArtTitle;
        private System.Windows.Forms.Label lblTechInfo;
        private System.Windows.Forms.Label lblTechTitle;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblDonatePP;
        private System.Windows.Forms.Label lblDonateBTC;
        private System.Windows.Forms.Label lblDevInfo;
    }
}