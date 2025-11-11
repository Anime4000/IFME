using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;

using NDesk.Options;

namespace IFME
{
    static class Program
    {
        public static bool ArgsHelp = false;
        public static bool ArgsSkipAVX = false;
        public static bool ArgsSkipAVX2 = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var o = new OptionSet()
            {
                { "h|?|help", "Show this message and exit", h => ArgsHelp = h != null },

                { "skip-avx", "Bypass AVX instruction set checks", x => ArgsSkipAVX = x != null },
                { "skip-avx2", "Bypass AVX2 instruction set checks", x => ArgsSkipAVX2 = x != null },
            };

            try
            {
                o.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine($"Try `IFME.exe --help' for more information.");
            }

            if (ArgsHelp)
            {
                Console.Error.WriteLine($"Usage: IFME.exe [OPTIONS]+");
                Console.Error.WriteLine("Mandatory arguments to long option are mandatory for short options too.");
                Console.Error.WriteLine("\nOptions:");
                o.WriteOptionDescriptions(Console.Error);
                Console.Error.WriteLine();

                return;
            }

            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
	}
}
