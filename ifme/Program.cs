using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			Console.Title = Get.AppName;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(Get.AppNameLong);
			Console.WriteLine($"Release: {Get.AppNameLib}");
			Console.ResetColor();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"(c) {DateTime.Now.Year} Anime4000, FFmpeg team, MulticoreWare, x264 team,\nXiph.Org Foundation, Google Inc., Nero AG, Moritz Bunkus, et al.");
			Console.ResetColor();
			Console.WriteLine();

			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
