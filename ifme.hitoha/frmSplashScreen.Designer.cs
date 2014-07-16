namespace ifme.hitoha
{
	partial class frmSplashScreen
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
			this.BGThread = new System.ComponentModel.BackgroundWorker();
			this.pictSS = new System.Windows.Forms.PictureBox();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblProgress = new System.Windows.Forms.Label();
			this.tmrFadeIn = new System.Windows.Forms.Timer(this.components);
			this.tmrFadeOut = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictSS)).BeginInit();
			this.SuspendLayout();
			// 
			// BGThread
			// 
			this.BGThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BGThread_DoWork);
			this.BGThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BGThread_RunWorkerCompleted);
			// 
			// pictSS
			// 
			this.pictSS.BackColor = System.Drawing.Color.Transparent;
			this.pictSS.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictSS.ErrorImage = null;
			this.pictSS.Image = global::ifme.hitoha.Properties.Resources.SplashScreen4;
			this.pictSS.InitialImage = null;
			this.pictSS.Location = new System.Drawing.Point(0, 0);
			this.pictSS.Name = "pictSS";
			this.pictSS.Size = new System.Drawing.Size(520, 320);
			this.pictSS.TabIndex = 0;
			this.pictSS.TabStop = false;
			// 
			// lblVersion
			// 
			this.lblVersion.BackColor = System.Drawing.Color.Transparent;
			this.lblVersion.ForeColor = System.Drawing.Color.White;
			this.lblVersion.Location = new System.Drawing.Point(388, 298);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(120, 13);
			this.lblVersion.TabIndex = 1;
			this.lblVersion.Text = "Version: {0}";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblStatus
			// 
			this.lblStatus.BackColor = System.Drawing.Color.Transparent;
			this.lblStatus.ForeColor = System.Drawing.Color.White;
			this.lblStatus.Location = new System.Drawing.Point(12, 259);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(496, 26);
			this.lblStatus.TabIndex = 2;
			// 
			// lblProgress
			// 
			this.lblProgress.BackColor = System.Drawing.Color.Transparent;
			this.lblProgress.ForeColor = System.Drawing.Color.White;
			this.lblProgress.Location = new System.Drawing.Point(12, 285);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(496, 13);
			this.lblProgress.TabIndex = 3;
			// 
			// tmrFadeIn
			// 
			this.tmrFadeIn.Interval = 1;
			this.tmrFadeIn.Tick += new System.EventHandler(this.tmrFadeIn_Tick);
			// 
			// tmrFadeOut
			// 
			this.tmrFadeOut.Interval = 1;
			this.tmrFadeOut.Tick += new System.EventHandler(this.tmrFadeOut_Tick);
			// 
			// frmSplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(520, 320);
			this.ControlBox = false;
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.pictSS);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximumSize = new System.Drawing.Size(520, 320);
			this.MinimumSize = new System.Drawing.Size(520, 320);
			this.Name = "frmSplashScreen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Internet Friendly Media Encoder";
			this.Load += new System.EventHandler(this.frmSplashScreen_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictSS)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.ComponentModel.BackgroundWorker BGThread;
		private System.Windows.Forms.PictureBox pictSS;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label lblProgress;
		private System.Windows.Forms.Timer tmrFadeIn;
		private System.Windows.Forms.Timer tmrFadeOut;
	}
}