namespace ifme.framework
{
	partial class frmScriptEditor
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
			this.txtEditor = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtEditor
			// 
			this.txtEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtEditor.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtEditor.Location = new System.Drawing.Point(0, 0);
			this.txtEditor.Multiline = true;
			this.txtEditor.Name = "txtEditor";
			this.txtEditor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtEditor.Size = new System.Drawing.Size(640, 480);
			this.txtEditor.TabIndex = 2;
			this.txtEditor.WordWrap = false;
			// 
			// frmScriptEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(640, 480);
			this.Controls.Add(this.txtEditor);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.MinimumSize = new System.Drawing.Size(656, 518);
			this.Name = "frmScriptEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Script Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAviSynthEditor_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtEditor;

	}
}