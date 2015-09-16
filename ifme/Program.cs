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
		static int Main(string[] args)
		{
			// Essential Stuff
			Title = $"{Global.App.Name} Console";
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			// Command
			if (args[0] == "-h" || args[0] == "--help")
			{
				Help();
				return 0;
			}

			if (args[0] == "-r" || args[0] == "--reset")
			{
				Properties.Settings.Default.Reset();
				Properties.Settings.Default.Save();

				WriteLine("Settings has been reset!");
			}

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
			return 0;
		}

		static void Head()
		{
			ForegroundColor = ConsoleColor.Yellow;
			WriteLine(Global.App.NameFull);
			WriteLine($"Compiled release: {Global.App.Version}-{(OS.Is64bit ? "x64" : "x86")}-{Global.App.Type}\n");
			ResetColor();
		}

		static void Help()
		{
			Head();
			WriteLine("Usage: ifme [OPTION]");
			WriteLine();
			WriteLine("Mandatory arguments to long options are mandatory for short options too.");
			WriteLine("  -h, --help                   show help (implies -r)");
			WriteLine("  -r, --reset                  reset IFME configuration");
			WriteLine();
			WriteLine("Report bugs to: <https://github.com/Anime4000/IFME/issues>");
			WriteLine("IFME home page: <https://x265.github.io/>");
			WriteLine("IFME fb page  : <https://www.facebook.com/internetfriendlymediaencoder/>");
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
			Head();

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
