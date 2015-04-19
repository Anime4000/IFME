using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme
{
	public partial class frmProperties : Form
	{
		char ScanType { get; set; }
		string OldScreenRes { get; set; }
		public string NewScreenRes { get; set; }

		public frmProperties(string ScreenRes)
		{
			InitializeComponent();
			this.Icon = Properties.Resources.ifme_flat;

			OldScreenRes = ScreenRes.Contains('p') || ScreenRes.Contains('i') ? ScreenRes.Remove(ScreenRes.Length - 1) : ScreenRes;
			NewScreenRes = OldScreenRes;
			ScanType = ScreenRes.Contains('i') ? 'i' : 'p';
		}

		private void frmProperties_Load(object sender, EventArgs e)
		{
			cboScreenRes.Text = OldScreenRes;
		}

		private void frmProperties_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.DialogResult = String.Equals(OldScreenRes, NewScreenRes) ? DialogResult.Cancel : DialogResult.OK;
		}

		private void cboScreenRes_SelectedIndexChanged(object sender, EventArgs e)
		{
			NewScreenRes = cboScreenRes.Text.Contains('-') ? OldScreenRes + ScanType : cboScreenRes.Text + ScanType;
		}
	}
}
