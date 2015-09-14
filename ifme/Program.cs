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
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmSplashScreen());

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

			Properties.Settings.Default.Save();
		}
	}
}
