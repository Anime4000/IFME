using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme.hitoha
{
	public partial class InputBox : Form
	{
		public string ReturnValue { get; set; }

		public InputBox(string title, string description, string input)
		{
			InitializeComponent();

			this.Text = title;
			lblDescription.Text = description;
			txtInput.Text = input;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ReturnValue = txtInput.Text;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void txtInput_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnOK.PerformClick();
			}
		}

		private void InputBox_Load(object sender, EventArgs e)
		{

		}
	}
}
