using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ifme.hitoha
{
	public partial class frmAviSynthEditor : Form
	{
		public string file { get; set; }
		public string content { get; set; }

		public frmAviSynthEditor(string avsfile)
		{
			InitializeComponent();
			Icon = Properties.Resources.ifme_flat;

			file = avsfile;
			content = File.ReadAllText(file);
			rtfEditor.Text = content;
		}

		private void frmAviSynthEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (content != rtfEditor.Text)
			{
				var MsgBox = MessageBox.Show("Save?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
				if (MsgBox == System.Windows.Forms.DialogResult.Yes)
				{
					File.WriteAllText(file, rtfEditor.Text);
				}
				else if (MsgBox == System.Windows.Forms.DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}
	}
}
