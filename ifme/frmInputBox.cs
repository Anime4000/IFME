using System;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmInputBox : Form
	{
		public string ReturnValue { get; set; }

		public frmInputBox(string title, string description, string input)
		{
			InitializeComponent();

			Text = title;
			lblDescription.Text = description;
			txtInput.Text = input;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ReturnValue = txtInput.Text;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
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
