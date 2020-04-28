using System;
using System.Windows.Forms;

namespace IFME.OSManager
{
	public partial class InputBox : Form
	{
		public string ReturnValue { get; set; }
		private int MinChar { get; set; }

		/// <summary>
		/// Show an Input Box dialog with returnable value
		/// </summary>
		/// <param name="title">Window Title</param>
		/// <param name="message">Message to prompt</param>
		/// <param name="minChar">Accept how many character for the input</param>
		public InputBox(string title, string message, int minChar = 0)
		{
			MinChar = minChar;
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.Sizable;

			Text = title;
			lblMessage.Text = message;
		}

		/// <summary>
		/// Show an Input Box dialog with returnable value
		/// </summary>
		/// <param name="title">Window Title</param>
		/// <param name="message">Message to prompt</param>
		/// <param name="value">Default value to prompt</param>
		/// <param name="minChar">Accept how many character for the input</param>
		public InputBox(string title, string message, string value, int minChar = 0)
		{
			MinChar = minChar;
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.Sizable;

			Text = title;
			lblMessage.Text = message;
			txtInputBox.Text = value;
		}

		private void InputBox_Load(object sender, EventArgs e)
		{

		}

		private void InputBox_Shown(object sender, EventArgs e)
		{

		}

		private void txtInputBox_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = (txtInputBox.Text.Length >= MinChar);
		}

		private void txtInputBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnOK.PerformClick();
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ReturnValue = txtInputBox.Text;
		}
	}
}
