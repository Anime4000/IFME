using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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

			Console.Title = "IFME console - plugins activity and report";

			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("{0} - compiled on {1}", Global.App.Name, Global.App.BuildDate);
			Console.WriteLine("Version: {0} (x64 {1} build)\n", Global.App.Version, Global.App.Type);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(" ________________________________________");
			Console.Write("/ ");

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("  All encoding activity and progress  ");

			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(" \\\n");
			Console.Write("\\ ");

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("         will be display here         ");

			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(" /\n");
			Console.WriteLine(" ----------------------------------------");

			Console.ResetColor();
			Console.WriteLine(@"        \   ^__^");
			Console.WriteLine(@"         \  (oo)\_______");
			Console.WriteLine(@"            (__)\       )\/\");
			Console.WriteLine(@"                ||----w |");
			Console.WriteLine(@"                ||     ||");

			Console.WriteLine();

			Application.Run(new frmMain());

			Properties.Settings.Default.Save();
		}
	}
}
