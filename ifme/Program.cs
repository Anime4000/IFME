﻿using System;
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
			if (OS.IsLinux)
				Console.Write("[info] All encoding log will display on this terminal\n");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}
	}
}
