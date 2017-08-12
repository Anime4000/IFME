using System;
using System.Windows.Forms;

namespace ifme
{
	public partial class frmInputBox : Form
	{
		public string ReturnValue { get; set; }

		private int MinLength = 0;

		/// <summary>
		/// Show an Input dialog box
		/// </summary>
		/// <param name="Title">Window title</param>
		/// <param name="Message">Message to prompt</param>
		/// <param name="MinChar">Accept how many character for the input</param>
		public frmInputBox(string Title, string Message, int MinChar)
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			Text = Title;
			FormBorderStyle = FormBorderStyle.Sizable;

			lblMessage.Text = Message;
			MinLength = MinChar;
		}

		/// <summary>
		/// Show an Input dialog box
		/// </summary>
		/// <param name="Title">Window title</param>
		/// <param name="Message">Message to prompt</param>
		/// <param name="Value">Default value to prompt</param>
		/// <param name="MinChar">Accept how many character for the input</param>
		public frmInputBox(string Title, string Message, string Value, int MinChar)
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			Text = Title;
			FormBorderStyle = FormBorderStyle.Sizable;

			lblMessage.Text = Message;

			txtInput.Text = Value;
			MinLength = MinChar;
		}

		private void frmInputBox_Load(object sender, EventArgs e)
		{
			if (OS.IsWindows)
				Font = Language.Lang.UIFontWindows;
			else
				Font = Language.Lang.UIFontLinux;

			btnOK.Enabled = (txtInput.Text.Length >= MinLength);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ReturnValue = txtInput.Text;
		}

		private void txtInput_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = (txtInput.Text.Length >= MinLength);
		}
	}
}
