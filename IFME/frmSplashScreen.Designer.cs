namespace IFME
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblLoading = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblContrib = new System.Windows.Forms.Label();
            this.lblLog = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(16, 241);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(400, 32);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Loading...";
            // 
            // lblLoading
            // 
            this.lblLoading.BackColor = System.Drawing.Color.Transparent;
            this.lblLoading.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblLoading.ForeColor = System.Drawing.Color.White;
            this.lblLoading.Location = new System.Drawing.Point(16, 217);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(400, 24);
            this.lblLoading.TabIndex = 1;
            this.lblLoading.Text = "Initializing...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(16, 171);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(600, 46);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "Version";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblContrib
            // 
            this.lblContrib.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblContrib.BackColor = System.Drawing.Color.Transparent;
            this.lblContrib.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContrib.ForeColor = System.Drawing.Color.White;
            this.lblContrib.Location = new System.Drawing.Point(16, 450);
            this.lblContrib.Name = "lblContrib";
            this.lblContrib.Size = new System.Drawing.Size(440, 133);
            this.lblContrib.TabIndex = 3;
            this.lblContrib.Text = "Contrib";
            this.lblContrib.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblLog
            // 
            this.lblLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLog.BackColor = System.Drawing.Color.Transparent;
            this.lblLog.Font = new System.Drawing.Font("Tahoma", 7F);
            this.lblLog.ForeColor = System.Drawing.Color.White;
            this.lblLog.Location = new System.Drawing.Point(17, 280);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(439, 160);
            this.lblLog.TabIndex = 4;
            this.lblLog.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // frmSplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::IFME.Properties.Resources.SplashScreen11;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1066, 600);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.lblContrib);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.lblStatus);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IFME";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmSplashScreen_Load);
            this.Shown += new System.EventHandler(this.frmSplashScreen_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblContrib;
        private System.Windows.Forms.Label lblLog;
    }
}