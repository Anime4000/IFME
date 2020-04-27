using System;
using System.Windows.Forms;

namespace IFME.OSManager
{
	public partial class InputBox2 : Form
	{
		public string ReturnValue1 { get; set; }
		public string ReturnValue2 { get; set; }
		private int MinChar { get; set; }

		/// <summary>
		/// Show an Input Box dialog with two input with two returnable value
		/// </summary>
		/// <param name="title">Window Title</param>
		/// <param name="message1">First Message to prompt</param>
		/// <param name="message2">Second Message to prompt</param>
		/// <param name="minChar">Accept how many character for the input</param>
		public InputBox2(string title, string message1, string message2, int minChar = 0)
		{
			MinChar = minChar;
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.Sizable;

			Text = title;
			lblMessage1.Text = message1;
			lblMessage2.Text = message2;
		}

		/// <summary>
		/// Show an Input Box dialog with two input with two returnable value
		/// </summary>
		/// <param name="title">Window Title</param>
		/// <param name="message1">First Message to prompt</param>
		/// <param name="message2">Second Message to prompt</param>
		/// <param name="value1">First default value to prompt</param>
		/// <param name="value2">Second default value to prompt</param>
		/// <param name="minChar">Accept how many character for the input</param>
		public InputBox2(string title, string message1, string message2, string value1, string value2, int minChar = 0)
		{
			MinChar = minChar;
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.Sizable;

			Text = title;
			lblMessage1.Text = message1;
			lblMessage2.Text = message2;
			txtInputBox1.Text = value1;
			txtInputBox2.Text = value2;
		}

		private void InputBox2_Load(object sender, EventArgs e)
		{

		}

		private void InputBox2_Shown(object sender, EventArgs e)
		{

		}

		private void txtInputBox1_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = (txtInputBox1.Text.Length >= MinChar) && (txtInputBox2.Text.Length >= MinChar);
		}

		private void txtInputBox2_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = (txtInputBox1.Text.Length >= MinChar) && (txtInputBox2.Text.Length >= MinChar);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ReturnValue1 = txtInputBox1.Text;
			ReturnValue2 = txtInputBox2.Text;
		}
	}
}
