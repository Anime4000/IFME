namespace ifme.hitoha
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
			this.components = new System.ComponentModel.Container();
			this.lblNames = new System.Windows.Forms.Label();
			this.tmrScroll = new System.Windows.Forms.Timer(this.components);
			this.lblWhoDraw = new System.Windows.Forms.Label();
			this.lblWhoChar = new System.Windows.Forms.Label();
			this.lblMascotName = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.lblAuthorInfo = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.lnkChangeLog = new System.Windows.Forms.LinkLabel();
			this.lblUpdateInfo = new System.Windows.Forms.Label();
			this.lblSThx = new System.Windows.Forms.Label();
			this.pictIfme = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictIfme)).BeginInit();
			this.SuspendLayout();
			// 
			// lblNames
			// 
			this.lblNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblNames.BackColor = System.Drawing.Color.Transparent;
			this.lblNames.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNames.Location = new System.Drawing.Point(308, 350);
			this.lblNames.Name = "lblNames";
			this.lblNames.Size = new System.Drawing.Size(320, 30);
			this.lblNames.TabIndex = 36;
			this.lblNames.Text = "Thank You for using :)";
			this.lblNames.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tmrScroll
			// 
			this.tmrScroll.Interval = 1000;
			this.tmrScroll.Tick += new System.EventHandler(this.tmrScroll_Tick);
			// 
			// lblWhoDraw
			// 
			this.lblWhoDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblWhoDraw.BackColor = System.Drawing.Color.Transparent;
			this.lblWhoDraw.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblWhoDraw.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblWhoDraw.Location = new System.Drawing.Point(308, 240);
			this.lblWhoDraw.Name = "lblWhoDraw";
			this.lblWhoDraw.Size = new System.Drawing.Size(320, 40);
			this.lblWhoDraw.TabIndex = 45;
			this.lblWhoDraw.Text = "Illustration by ray-en\r\nhttp://ray-en.deviantart.com/";
			this.lblWhoDraw.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblWhoDraw.Click += new System.EventHandler(this.lblWhoDraw_Click);
			// 
			// lblWhoChar
			// 
			this.lblWhoChar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblWhoChar.BackColor = System.Drawing.Color.Transparent;
			this.lblWhoChar.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblWhoChar.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblWhoChar.Location = new System.Drawing.Point(308, 200);
			this.lblWhoChar.Name = "lblWhoChar";
			this.lblWhoChar.Size = new System.Drawing.Size(320, 40);
			this.lblWhoChar.TabIndex = 43;
			this.lblWhoChar.Text = "Character by Aruuie Francoise\r\nhttp://www.pixiv.net/member.php?id=6206705";
			this.lblWhoChar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblWhoChar.Click += new System.EventHandler(this.lblWhoChar_Click);
			// 
			// lblMascotName
			// 
			this.lblMascotName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMascotName.BackColor = System.Drawing.Color.Transparent;
			this.lblMascotName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.lblMascotName.Location = new System.Drawing.Point(308, 170);
			this.lblMascotName.Name = "lblMascotName";
			this.lblMascotName.Size = new System.Drawing.Size(320, 30);
			this.lblMascotName.TabIndex = 42;
			this.lblMascotName.Text = "Ifme-chan";
			this.lblMascotName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblInfo
			// 
			this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInfo.Location = new System.Drawing.Point(308, 50);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(320, 70);
			this.lblInfo.TabIndex = 41;
			this.lblInfo.Text = "A";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblAuthorInfo
			// 
			this.lblAuthorInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblAuthorInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblAuthorInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblAuthorInfo.Location = new System.Drawing.Point(308, 120);
			this.lblAuthorInfo.Name = "lblAuthorInfo";
			this.lblAuthorInfo.Size = new System.Drawing.Size(320, 50);
			this.lblAuthorInfo.TabIndex = 39;
			this.lblAuthorInfo.Text = "A\r\nB";
			this.lblAuthorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.BackColor = System.Drawing.Color.Transparent;
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.lblTitle.Location = new System.Drawing.Point(308, 9);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(320, 30);
			this.lblTitle.TabIndex = 40;
			this.lblTitle.Text = "A";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnUpdate
			// 
			this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
			this.btnUpdate.Location = new System.Drawing.Point(381, 443);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(178, 28);
			this.btnUpdate.TabIndex = 47;
			this.btnUpdate.Text = "&Apply Update";
			this.btnUpdate.UseVisualStyleBackColor = false;
			this.btnUpdate.Visible = false;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// lnkChangeLog
			// 
			this.lnkChangeLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkChangeLog.BackColor = System.Drawing.Color.Transparent;
			this.lnkChangeLog.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lnkChangeLog.LinkColor = System.Drawing.Color.Blue;
			this.lnkChangeLog.Location = new System.Drawing.Point(308, 427);
			this.lnkChangeLog.Name = "lnkChangeLog";
			this.lnkChangeLog.Size = new System.Drawing.Size(320, 13);
			this.lnkChangeLog.TabIndex = 48;
			this.lnkChangeLog.TabStop = true;
			this.lnkChangeLog.Text = "See Changelog";
			this.lnkChangeLog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lnkChangeLog.Visible = false;
			this.lnkChangeLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkChangeLog_LinkClicked);
			// 
			// lblUpdateInfo
			// 
			this.lblUpdateInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblUpdateInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblUpdateInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.lblUpdateInfo.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblUpdateInfo.Location = new System.Drawing.Point(308, 443);
			this.lblUpdateInfo.Name = "lblUpdateInfo";
			this.lblUpdateInfo.Size = new System.Drawing.Size(320, 28);
			this.lblUpdateInfo.TabIndex = 46;
			this.lblUpdateInfo.Text = "{0} {1}";
			this.lblUpdateInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblSThx
			// 
			this.lblSThx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSThx.BackColor = System.Drawing.Color.Transparent;
			this.lblSThx.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.lblSThx.Location = new System.Drawing.Point(308, 320);
			this.lblSThx.Name = "lblSThx";
			this.lblSThx.Size = new System.Drawing.Size(320, 30);
			this.lblSThx.TabIndex = 49;
			this.lblSThx.Text = "Special Thanks";
			this.lblSThx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictIfme
			// 
			this.pictIfme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.pictIfme.BackColor = System.Drawing.Color.Transparent;
			this.pictIfme.Image = global::ifme.Properties.Resources.AboutIFME;
			this.pictIfme.Location = new System.Drawing.Point(0, 0);
			this.pictIfme.Name = "pictIfme";
			this.pictIfme.Size = new System.Drawing.Size(250, 800);
			this.pictIfme.TabIndex = 50;
			this.pictIfme.TabStop = false;
			// 
			// frmAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::ifme.Properties.Resources.AboutBackground;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.pictIfme);
			this.Controls.Add(this.lblSThx);
			this.Controls.Add(this.lblNames);
			this.Controls.Add(this.btnUpdate);
			this.Controls.Add(this.lnkChangeLog);
			this.Controls.Add(this.lblUpdateInfo);
			this.Controls.Add(this.lblWhoDraw);
			this.Controls.Add(this.lblWhoChar);
			this.Controls.Add(this.lblMascotName);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.lblAuthorInfo);
			this.Controls.Add(this.lblTitle);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(656, 518);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(656, 518);
			this.Name = "frmAbout";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "{0} {1}";
			this.Load += new System.EventHandler(this.frmAbout_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictIfme)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblNames;
		private System.Windows.Forms.Timer tmrScroll;
		private System.Windows.Forms.Label lblWhoDraw;
		private System.Windows.Forms.Label lblWhoChar;
		private System.Windows.Forms.Label lblMascotName;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label lblAuthorInfo;
		private System.Windows.Forms.Label lblTitle;
		internal System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.LinkLabel lnkChangeLog;
		private System.Windows.Forms.Label lblUpdateInfo;
		private System.Windows.Forms.Label lblSThx;
		private System.Windows.Forms.PictureBox pictIfme;
	}
}