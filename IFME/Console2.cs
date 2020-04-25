using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
	internal class Console2
	{
		private static RichTextBox rtf = (Application.OpenForms["frmMain"] as frmMain).rtfConsole;

		internal static void Write(string value)
		{
			rtf.Invoke((Action)delegate
			{
				rtf.AppendText(value);
				rtf.ScrollToCaret();
			});
		}

		internal static void WriteLine(string value)
		{
			rtf.Invoke((Action)delegate
			{
				rtf.AppendText(value + Environment.NewLine);
				rtf.ScrollToCaret();
			});
		}
	}
}
