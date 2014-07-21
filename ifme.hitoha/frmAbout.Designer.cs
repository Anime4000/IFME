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
			this.lblMascot = new System.Windows.Forms.Label();
			this.lnkPrivacy = new System.Windows.Forms.LinkLabel();
			this.lnkLicense = new System.Windows.Forms.LinkLabel();
			this.lnkEndUser = new System.Windows.Forms.LinkLabel();
			this.lblInfo = new System.Windows.Forms.Label();
			this.lblUpdateInfo = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.lnkChangeLog = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// lblMascot
			// 
			this.lblMascot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMascot.BackColor = System.Drawing.Color.Transparent;
			this.lblMascot.Enabled = false;
			this.lblMascot.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblMascot.Location = new System.Drawing.Point(12, 273);
			this.lblMascot.Name = "lblMascot";
			this.lblMascot.Size = new System.Drawing.Size(624, 24);
			this.lblMascot.TabIndex = 26;
			this.lblMascot.Text = "{0} Aruuie Francoise";
			this.lblMascot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lnkPrivacy
			// 
			this.lnkPrivacy.BackColor = System.Drawing.Color.Transparent;
			this.lnkPrivacy.LinkColor = System.Drawing.Color.White;
			this.lnkPrivacy.Location = new System.Drawing.Point(450, 248);
			this.lnkPrivacy.Name = "lnkPrivacy";
			this.lnkPrivacy.Size = new System.Drawing.Size(186, 15);
			this.lnkPrivacy.TabIndex = 25;
			this.lnkPrivacy.TabStop = true;
			this.lnkPrivacy.Text = "Privacy Policy";
			this.lnkPrivacy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lnkPrivacy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPrivacy_LinkClicked);
			// 
			// lnkLicense
			// 
			this.lnkLicense.BackColor = System.Drawing.Color.Transparent;
			this.lnkLicense.LinkColor = System.Drawing.Color.White;
			this.lnkLicense.Location = new System.Drawing.Point(204, 248);
			this.lnkLicense.Name = "lnkLicense";
			this.lnkLicense.Size = new System.Drawing.Size(240, 15);
			this.lnkLicense.TabIndex = 24;
			this.lnkLicense.TabStop = true;
			this.lnkLicense.Text = "Lisensing Information";
			this.lnkLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lnkLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLicense_LinkClicked);
			// 
			// lnkEndUser
			// 
			this.lnkEndUser.BackColor = System.Drawing.Color.Transparent;
			this.lnkEndUser.LinkColor = System.Drawing.Color.White;
			this.lnkEndUser.Location = new System.Drawing.Point(12, 248);
			this.lnkEndUser.Name = "lnkEndUser";
			this.lnkEndUser.Size = new System.Drawing.Size(186, 15);
			this.lnkEndUser.TabIndex = 23;
			this.lnkEndUser.TabStop = true;
			this.lnkEndUser.Text = "End-User Rights";
			this.lnkEndUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lnkEndUser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkEndUser_LinkClicked);
			// 
			// lblInfo
			// 
			this.lblInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblInfo.ForeColor = System.Drawing.Color.White;
			this.lblInfo.Location = new System.Drawing.Point(265, 117);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(381, 50);
			this.lblInfo.TabIndex = 22;
			this.lblInfo.Text = "IFME is designed for encode media to internet friendly, let\'s together make this " +
    "application awesome and useful.";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUpdateInfo
			// 
			this.lblUpdateInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblUpdateInfo.ForeColor = System.Drawing.Color.White;
			this.lblUpdateInfo.Location = new System.Drawing.Point(265, 85);
			this.lblUpdateInfo.Name = "lblUpdateInfo";
			this.lblUpdateInfo.Size = new System.Drawing.Size(381, 32);
			this.lblUpdateInfo.TabIndex = 21;
			this.lblUpdateInfo.Text = "{0} {1}";
			this.lblUpdateInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblVersion
			// 
			this.lblVersion.BackColor = System.Drawing.Color.Transparent;
			this.lblVersion.ForeColor = System.Drawing.Color.White;
			this.lblVersion.Location = new System.Drawing.Point(265, 53);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(381, 32);
			this.lblVersion.TabIndex = 20;
			this.lblVersion.Text = "{0} ({1} build)";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.BackColor = System.Drawing.Color.Transparent;
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
			this.lblTitle.ForeColor = System.Drawing.Color.White;
			this.lblTitle.Location = new System.Drawing.Point(262, 21);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(42, 32);
			this.lblTitle.TabIndex = 19;
			this.lblTitle.Text = "{0}";
			// 
			// btnUpdate
			// 
			this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
			this.btnUpdate.Location = new System.Drawing.Point(268, 85);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(200, 32);
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
			this.lnkChangeLog.AutoSize = true;
			this.lnkChangeLog.BackColor = System.Drawing.Color.Transparent;
			this.lnkChangeLog.LinkColor = System.Drawing.Color.White;
			this.lnkChangeLog.Location = new System.Drawing.Point(474, 94);
			this.lnkChangeLog.Name = "lnkChangeLog";
			this.lnkChangeLog.Size = new System.Drawing.Size(65, 15);
			this.lnkChangeLog.TabIndex = 28;
			this.lnkChangeLog.TabStop = true;
			this.lnkChangeLog.Text = "Changelog";
			this.lnkChangeLog.Visible = false;
			this.lnkChangeLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkChangeLog_LinkClicked);
			// 
			// frmAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::ifme.hitoha.Properties.Resources.ImgAbout;
			this.ClientSize = new System.Drawing.Size(648, 297);
			this.Controls.Add(this.lnkChangeLog);
			this.Controls.Add(this.btnUpdate);
			this.Controls.Add(this.lblMascot);
			this.Controls.Add(this.lnkPrivacy);
			this.Controls.Add(this.lnkLicense);
			this.Controls.Add(this.lnkEndUser);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.lblUpdateInfo);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblTitle);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(664, 335);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(664, 335);
			this.Name = "frmAbout";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "{0} {1}";
			this.Load += new System.EventHandler(this.frmAbout_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		internal System.Windows.Forms.Label lblMascot;
		internal System.Windows.Forms.LinkLabel lnkPrivacy;
		internal System.Windows.Forms.LinkLabel lnkLicense;
		internal System.Windows.Forms.LinkLabel lnkEndUser;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label lblUpdateInfo;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblTitle;
		internal System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.LinkLabel lnkChangeLog;
	}
}