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
	public partial class frmInputBox : Form
	{
		public string ReturnValue { get; set; }

		public frmInputBox(string Title, string Message)
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			Text = Title;
			FormBorderStyle = FormBorderStyle.Sizable;

			lblMessage.Text = Message;
		}

		public frmInputBox(string Title, string Message, string Value)
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			Text = Title;
			FormBorderStyle = FormBorderStyle.Sizable;

			lblMessage.Text = Message;

			txtInput.Text = Value;
		}

		private void frmInputBox_Load(object sender, EventArgs e)
		{

		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ReturnValue = txtInput.Text;
		}

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (txtInput.Text.Length > 0);
        }
    }
}
