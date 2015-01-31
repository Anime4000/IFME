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
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblAuthorInfo = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.lblWhoChar = new System.Windows.Forms.Label();
			this.lblWhoDraw = new System.Windows.Forms.Label();
			this.lblUpdateInfo = new System.Windows.Forms.Label();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.tmrScroll = new System.Windows.Forms.Timer(this.components);
			this.lblNames = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.lblTitle.Location = new System.Drawing.Point(0, 300);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(600, 40);
			this.lblTitle.TabIndex = 40;
			this.lblTitle.Text = "A";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblAuthorInfo
			// 
			this.lblAuthorInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblAuthorInfo.BackColor = System.Drawing.Color.White;
			this.lblAuthorInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblAuthorInfo.Location = new System.Drawing.Point(0, 340);
			this.lblAuthorInfo.Name = "lblAuthorInfo";
			this.lblAuthorInfo.Size = new System.Drawing.Size(600, 60);
			this.lblAuthorInfo.TabIndex = 39;
			this.lblAuthorInfo.Text = "A\r\nB";
			this.lblAuthorInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblInfo
			// 
			this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInfo.Location = new System.Drawing.Point(12, 400);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(576, 50);
			this.lblInfo.TabIndex = 41;
			this.lblInfo.Text = "A";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblWhoChar
			// 
			this.lblWhoChar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblWhoChar.BackColor = System.Drawing.Color.Transparent;
			this.lblWhoChar.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblWhoChar.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblWhoChar.ForeColor = System.Drawing.Color.Blue;
			this.lblWhoChar.Location = new System.Drawing.Point(12, 490);
			this.lblWhoChar.Name = "lblWhoChar";
			this.lblWhoChar.Size = new System.Drawing.Size(576, 40);
			this.lblWhoChar.TabIndex = 43;
			this.lblWhoChar.Text = "Character by Aruuie Francoise\r\nhttp://www.pixiv.net/member.php?id=6206705";
			this.lblWhoChar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblWhoChar.Click += new System.EventHandler(this.lblWhoChar_Click);
			// 
			// lblWhoDraw
			// 
			this.lblWhoDraw.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblWhoDraw.BackColor = System.Drawing.Color.Transparent;
			this.lblWhoDraw.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblWhoDraw.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblWhoDraw.ForeColor = System.Drawing.Color.Blue;
			this.lblWhoDraw.Location = new System.Drawing.Point(12, 450);
			this.lblWhoDraw.Name = "lblWhoDraw";
			this.lblWhoDraw.Size = new System.Drawing.Size(576, 40);
			this.lblWhoDraw.TabIndex = 45;
			this.lblWhoDraw.Text = "Character Illustration by ray-en\r\nhttp://ray-en.deviantart.com/";
			this.lblWhoDraw.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblWhoDraw.Click += new System.EventHandler(this.lblWhoDraw_Click);
			// 
			// lblUpdateInfo
			// 
			this.lblUpdateInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblUpdateInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblUpdateInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.lblUpdateInfo.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblUpdateInfo.Location = new System.Drawing.Point(12, 603);
			this.lblUpdateInfo.Name = "lblUpdateInfo";
			this.lblUpdateInfo.Size = new System.Drawing.Size(576, 28);
			this.lblUpdateInfo.TabIndex = 46;
			this.lblUpdateInfo.Text = "{0} {1}";
			this.lblUpdateInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnUpdate
			// 
			this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
			this.btnUpdate.Location = new System.Drawing.Point(211, 603);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(178, 28);
			this.btnUpdate.TabIndex = 47;
			this.btnUpdate.Text = "&Apply Update";
			this.btnUpdate.UseVisualStyleBackColor = false;
			this.btnUpdate.Visible = false;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// tmrScroll
			// 
			this.tmrScroll.Interval = 1500;
			this.tmrScroll.Tick += new System.EventHandler(this.tmrScroll_Tick);
			// 
			// lblNames
			// 
			this.lblNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblNames.BackColor = System.Drawing.Color.Transparent;
			this.lblNames.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNames.Location = new System.Drawing.Point(12, 538);
			this.lblNames.Name = "lblNames";
			this.lblNames.Size = new System.Drawing.Size(576, 50);
			this.lblNames.TabIndex = 36;
			this.lblNames.Text = "Thank You for using :)";
			this.lblNames.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// frmAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(600, 662);
			this.Controls.Add(this.lblWhoDraw);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.lblNames);
			this.Controls.Add(this.lblWhoChar);
			this.Controls.Add(this.lblAuthorInfo);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.btnUpdate);
			this.Controls.Add(this.lblUpdateInfo);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "{0} {1}";
			this.Load += new System.EventHandler(this.frmAbout_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblAuthorInfo;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label lblWhoChar;
		private System.Windows.Forms.Label lblWhoDraw;
		private System.Windows.Forms.Label lblUpdateInfo;
		internal System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Timer tmrScroll;
		private System.Windows.Forms.Label lblNames;


	}
}