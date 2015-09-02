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

			Title = "IFME console - plugins activity and report";

			Clear();
			ForegroundColor = ConsoleColor.Yellow;
			WriteLine("{0} - compiled on {1}", Global.App.Name, Global.App.BuildDate);
			WriteLine("Version: {0} (x64 {1} build)\n", Global.App.Version, Global.App.Type);

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
