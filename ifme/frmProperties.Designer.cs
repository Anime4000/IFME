namespace ifme
{
	partial class frmProperties
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
			this.cboScreenRes = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// cboScreenRes
			// 
			this.cboScreenRes.FormattingEnabled = true;
			this.cboScreenRes.Items.AddRange(new object[] {
            "--- 4:3 ---",
            "640x480",
            "800x600",
            "1024x768",
            "--- 16:9 ---",
            "640x360",
            "853x480",
            "1024x576",
            "1280x720",
            "1920x1080",
            "3840x2160",
            "--- other ---",
            "720x400"});
			this.cboScreenRes.Location = new System.Drawing.Point(82, 31);
			this.cboScreenRes.Name = "cboScreenRes";
			this.cboScreenRes.Size = new System.Drawing.Size(121, 21);
			this.cboScreenRes.TabIndex = 0;
			this.cboScreenRes.Text = "1280x720";
			this.cboScreenRes.SelectedIndexChanged += new System.EventHandler(this.cboScreenRes_SelectedIndexChanged);
			// 
			// frmProperties
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 82);
			this.Controls.Add(this.cboScreenRes);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(300, 120);
			this.MinimumSize = new System.Drawing.Size(300, 120);
			this.Name = "frmProperties";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProperties_FormClosing);
			this.Load += new System.EventHandler(this.frmProperties_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cboScreenRes;
	}
}