using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

using static System.Console;

namespace ifme
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			// Essential Stuff
			Title = $"{Global.App.Name} Console";
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			// Make WinForms much pretty
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Upgrade settings
			UpgradeSettings();

			// Splash Screen, loading and update
			SplashScreen();

			// Main Form
			MainForm();

			// Save settings and exit
			Properties.Settings.Default.Save();
		}

		static void UpgradeSettings()
		{
			if (!string.Equals(Properties.Settings.Default.Version, Global.App.VersionRelease))
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.Version = Global.App.VersionRelease;

				if (string.IsNullOrEmpty(Properties.Settings.Default.Language))
					Properties.Settings.Default.Language = "en";

				if (OS.IsLinux)
					Properties.Settings.Default.Compiler = "gcc";
				else
					Properties.Settings.Default.Compiler = "msvc";

				WriteLine("Settings has been upgraded!");
			}
		}

		static void SplashScreen()
		{
			Application.Run(new frmSplashScreen());
		}

		static void MainForm()
		{
			Title = $"{Global.App.Name} Console";

			Clear();
			ForegroundColor = ConsoleColor.Yellow;
			WriteLine($"{Global.App.Name} - compiled on {Global.App.BuildDate}");
			WriteLine($"Version: {Global.App.Version} ({(OS.Is64bit ? "x64" : "x86")} {Global.App.Type} build)\n");

			ForegroundColor = ConsoleColor.Green;
			WriteLine(" ________________________________________");
			Write("/ ");

			ForegroundColor = ConsoleColor.Cyan;
			Write("  All encoding activity and progress  ");

			ForegroundColor = ConsoleColor.Green;
			Write(" \\\n");
			Write("\\ ");

			ForegroundColor = ConsoleColor.Cyan;
			Write("         will be display here         ");

			ForegroundColor = ConsoleColor.Green;
			Write(" /\n");
			WriteLine(" ----------------------------------------");

			ResetColor();
			WriteLine(@"        \   ^__^");
			WriteLine(@"         \  (oo)\_______");
			WriteLine(@"            (__)\       )\/\");
			WriteLine(@"                ||----w |");
			WriteLine(@"                ||     ||");

			WriteLine();

			Application.Run(new frmMain());
		}
	}
}
