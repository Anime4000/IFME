namespace ifme
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
			this.bgwThread = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// bgwThread
			// 
			this.bgwThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwThread_DoWork);
			this.bgwThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwThread_RunWorkerCompleted);
			// 
			// frmSplashScreen
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(600, 320);
			this.ControlBox = false;
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MinimumSize = new System.Drawing.Size(600, 320);
			this.Name = "frmSplashScreen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "IFME Loading";
			this.Load += new System.EventHandler(this.frmSplashScreen_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.ComponentModel.BackgroundWorker bgwThread;
	}
}