namespace ifme.framework
{
	partial class frmAviSynthEditor
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
			this.rtfEditor = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// rtfEditor
			// 
			this.rtfEditor.BackColor = System.Drawing.Color.White;
			this.rtfEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtfEditor.Font = new System.Drawing.Font("Courier New", 10F);
			this.rtfEditor.Location = new System.Drawing.Point(0, 0);
			this.rtfEditor.Name = "rtfEditor";
			this.rtfEditor.Size = new System.Drawing.Size(640, 480);
			this.rtfEditor.TabIndex = 1;
			this.rtfEditor.Text = "";
			this.rtfEditor.WordWrap = false;
			// 
			// frmAviSynthEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.rtfEditor);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.MinimumSize = new System.Drawing.Size(656, 518);
			this.Name = "frmAviSynthEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AviSynth: Script Editor (EXPERIMENTAL)";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAviSynthEditor_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox rtfEditor;
	}
}