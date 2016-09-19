using System;
using System.IO;
using System.Windows.Forms;

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
            Console.Title = "Internet Friendly Media Encoder";
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
