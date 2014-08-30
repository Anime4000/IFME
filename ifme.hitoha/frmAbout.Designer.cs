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
			this.lblUpdateInfo = new System.Windows.Forms.Label();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.lnkChangeLog = new System.Windows.Forms.LinkLabel();
			this.lblAuthorInfo = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.lblNames = new System.Windows.Forms.Label();
			this.lblWhoDraw = new System.Windows.Forms.Label();
			this.lblSpecialThx = new System.Windows.Forms.Label();
			this.lblWhoChar = new System.Windows.Forms.Label();
			this.lblMascotName = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.tmrScroll = new System.Windows.Forms.Timer(this.components);
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblUpdateInfo
			// 
			this.lblUpdateInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblUpdateInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.lblUpdateInfo.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblUpdateInfo.Location = new System.Drawing.Point(3, 440);
			this.lblUpdateInfo.Name = "lblUpdateInfo";
			this.lblUpdateInfo.Size = new System.Drawing.Size(393, 28);
			this.lblUpdateInfo.TabIndex = 21;
			this.lblUpdateInfo.Text = "{0} {1}";
			this.lblUpdateInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnUpdate
			// 
			this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
			this.btnUpdate.Location = new System.Drawing.Point(114, 440);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(171, 28);
			this.btnUpdate.TabIndex = 27;
			this.btnUpdate.Text = "&Apply Update";
			this.btnUpdate.UseVisualStyleBackColor = false;
			this.btnUpdate.Visible = false;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// timer
			// 
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// lnkChangeLog
			// 
			this.lnkChangeLog.BackColor = System.Drawing.Color.Transparent;
			this.lnkChangeLog.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lnkChangeLog.LinkColor = System.Drawing.Color.Blue;
			this.lnkChangeLog.Location = new System.Drawing.Point(3, 424);
			this.lnkChangeLog.Name = "lnkChangeLog";
			this.lnkChangeLog.Size = new System.Drawing.Size(393, 13);
			this.lnkChangeLog.TabIndex = 28;
			this.lnkChangeLog.TabStop = true;
			this.lnkChangeLog.Text = "See Changelog";
			this.lnkChangeLog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lnkChangeLog.Visible = false;
			this.lnkChangeLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkChangeLog_LinkClicked);
			// 
			// lblAuthorInfo
			// 
			this.lblAuthorInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblAuthorInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblAuthorInfo.Location = new System.Drawing.Point(3, 94);
			this.lblAuthorInfo.Name = "lblAuthorInfo";
			this.lblAuthorInfo.Size = new System.Drawing.Size(393, 45);
			this.lblAuthorInfo.TabIndex = 30;
			this.lblAuthorInfo.Text = "A\r\nB";
			this.lblAuthorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblTitle
			// 
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
			this.lblTitle.Location = new System.Drawing.Point(3, 9);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(393, 30);
			this.lblTitle.TabIndex = 31;
			this.lblTitle.Text = "A";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.lblWhoDraw);
			this.panel1.Controls.Add(this.lblSpecialThx);
			this.panel1.Controls.Add(this.lblWhoChar);
			this.panel1.Controls.Add(this.lblMascotName);
			this.panel1.Controls.Add(this.lblInfo);
			this.panel1.Controls.Add(this.btnUpdate);
			this.panel1.Controls.Add(this.lblAuthorInfo);
			this.panel1.Controls.Add(this.lblTitle);
			this.panel1.Controls.Add(this.lnkChangeLog);
			this.panel1.Controls.Add(this.lblUpdateInfo);
			this.panel1.Location = new System.Drawing.Point(241, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(399, 480);
			this.panel1.TabIndex = 32;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.lblNames);
			this.panel3.Location = new System.Drawing.Point(0, 282);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(399, 139);
			this.panel3.TabIndex = 39;
			// 
			// lblNames
			// 
			this.lblNames.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNames.Location = new System.Drawing.Point(99, 56);
			this.lblNames.Name = "lblNames";
			this.lblNames.Size = new System.Drawing.Size(200, 26);
			this.lblNames.TabIndex = 36;
			this.lblNames.Text = "All name will scroll here!";
			this.lblNames.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblWhoDraw
			// 
			this.lblWhoDraw.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblWhoDraw.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblWhoDraw.Location = new System.Drawing.Point(3, 209);
			this.lblWhoDraw.Name = "lblWhoDraw";
			this.lblWhoDraw.Size = new System.Drawing.Size(393, 40);
			this.lblWhoDraw.TabIndex = 38;
			this.lblWhoDraw.Text = "Illustration by ray-en\r\nhttp://ray-en.deviantart.com/";
			this.lblWhoDraw.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblWhoDraw.Click += new System.EventHandler(this.lblWhoDraw_Click);
			// 
			// lblSpecialThx
			// 
			this.lblSpecialThx.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSpecialThx.Location = new System.Drawing.Point(3, 249);
			this.lblSpecialThx.Name = "lblSpecialThx";
			this.lblSpecialThx.Size = new System.Drawing.Size(393, 30);
			this.lblSpecialThx.TabIndex = 35;
			this.lblSpecialThx.Text = "Special Thanks";
			this.lblSpecialThx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblWhoChar
			// 
			this.lblWhoChar.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblWhoChar.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblWhoChar.Location = new System.Drawing.Point(3, 169);
			this.lblWhoChar.Name = "lblWhoChar";
			this.lblWhoChar.Size = new System.Drawing.Size(393, 40);
			this.lblWhoChar.TabIndex = 34;
			this.lblWhoChar.Text = "Character by Aruuie Francoise\r\nhttp://www.pixiv.net/member.php?id=6206705";
			this.lblWhoChar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblWhoChar.Click += new System.EventHandler(this.lblWhoChar_Click);
			// 
			// lblMascotName
			// 
			this.lblMascotName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.lblMascotName.Location = new System.Drawing.Point(3, 139);
			this.lblMascotName.Name = "lblMascotName";
			this.lblMascotName.Size = new System.Drawing.Size(393, 30);
			this.lblMascotName.TabIndex = 33;
			this.lblMascotName.Text = "イフミーちゃん　「ifme-chan」";
			this.lblMascotName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblInfo
			// 
			this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInfo.Location = new System.Drawing.Point(3, 39);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(393, 55);
			this.lblInfo.TabIndex = 32;
			this.lblInfo.Text = "A";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tmrScroll
			// 
			this.tmrScroll.Tick += new System.EventHandler(this.tmrScroll_Tick);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.Transparent;
			this.panel2.BackgroundImage = global::ifme.hitoha.Properties.Resources.AboutLeft;
			this.panel2.Cursor = System.Windows.Forms.Cursors.Hand;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(242, 480);
			this.panel2.TabIndex = 33;
			this.panel2.Click += new System.EventHandler(this.panel2_Click);
			// 
			// frmAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BackgroundImage = global::ifme.hitoha.Properties.Resources.AboutBackground;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
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
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblUpdateInfo;
		internal System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.LinkLabel lnkChangeLog;
		private System.Windows.Forms.Label lblAuthorInfo;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label lblMascotName;
		private System.Windows.Forms.Label lblWhoChar;
		private System.Windows.Forms.Label lblSpecialThx;
		private System.Windows.Forms.Label lblNames;
		private System.Windows.Forms.Timer tmrScroll;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label lblWhoDraw;
		private System.Windows.Forms.Panel panel3;
	}
}