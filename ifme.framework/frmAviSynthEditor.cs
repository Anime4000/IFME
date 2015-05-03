using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ifme.framework
{
	public partial class frmAviSynthEditor : Form
	{
		public string file { get; set; }
		public string content { get; set; }

		public frmAviSynthEditor(string avsfile)
		{
			InitializeComponent();
			Icon = Properties.Resources.Bug;

			file = avsfile;
			content = File.ReadAllText(file);
			txtEditor.Text = content;
		}

		private void frmAviSynthEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (content != txtEditor.Text)
			{
				var MsgBox = MessageBox.Show("Save?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
				if (MsgBox == System.Windows.Forms.DialogResult.Yes)
				{
					var utf8WithoutBom = new System.Text.UTF8Encoding(false);
					using (var data = new StreamWriter(file, false,utf8WithoutBom))
					{
						data.Write(txtEditor.Text);
					}
				}
				else if (MsgBox == System.Windows.Forms.DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}
	}
}
