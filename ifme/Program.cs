using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Mono.Options;

namespace ifme
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var reset = false;
            var help = false;
            var p = new OptionSet()
            {
                { "r|reset", "Reset IFME to factory default", v => reset = v != null  },
                { "h|help", "Show this message and exit", v => help = v != null }
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `ifme --help' for more information.");

                return;
            }

            // force to use "." as decimal
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Console.Title = Get.AppName;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(Get.AppNameLong);
			Console.WriteLine($"Release: {Get.AppNameLib}");
			Console.ResetColor();
			Console.WriteLine();

            if (help)
            {
                Console.Error.WriteLine("Usage: ifme [OPTION]");
                Console.Error.WriteLine("Mandatory arguments to long option are mandatory for short options too.");
                Console.Error.WriteLine("\nOptions:");
                p.WriteOptionDescriptions(Console.Error);
                Console.Error.WriteLine("\nProject home page: < https://x265.github.io/>");
                Console.Error.WriteLine("Report bugs to: <https://github.com/Anime4000/IFME/issues>");

                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"(c) {DateTime.Now.Year} {Branding.CopyRight()}");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Warning, DO NOT close this Terminal/Console, all useful info will be shown here.");
            Console.ResetColor();
			Console.WriteLine();

            if (reset)
            {
                Console.WriteLine("Resetting user settings.");

                Properties.Settings.Default.Reset();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

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
