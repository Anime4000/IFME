using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ifme.hitoha
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Console.Write("Starting {0}\n", Globals.AppInfo.Name);

			if (OS.IsLinux)
				Console.Write("For {0} version, all encoding log will display on this terminal\n", OS.Name);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}
	}
}
