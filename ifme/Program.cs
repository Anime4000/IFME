using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ifme
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // force to use "." as decimal
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Console.Title = Get.AppName;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(Get.AppNameLong);
			Console.WriteLine($"Release: {Get.AppNameLib}");
			Console.ResetColor();
			Console.WriteLine();

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"(c) {DateTime.Now.Year} {Branding.CopyRight()}");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Warning, DO NOT close this Terminal/Console, all useful info will be shown here.");
            Console.ResetColor();
			Console.WriteLine();

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Console.WriteLine("Updating user settings.");

                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
