using IFME.OSManager;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace IFME
{
    public class Version
    {
        private static readonly string[] version = Application.ProductVersion.Split('.');

        public static string Title { get { return "Internet Friendly Media Encoder"; } }
        public static string Name { get { return typeof(Program).Assembly.GetName().Name; } }
        public static string CodeName { get { return Application.ProductName; } }
        public static string Release { get { return $"{version[0]}.{version[1]} beta {version[2]}"; } }
        public static string Contrib { get { return "Nemu, Jel42, Robin Lawrie, Ron Mitschke, Project Angel"; } }
        public static string TradeMark { get { return FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).LegalTrademarks; } }
        public static string March { get { return "znver2"; } }
        public static string OSArch { get { return OS.Is64bit ? "amd64" : "x86"; } }
        public static string OSPlatform { get { return OS.IsWindows ? "windows" : "linux"; } }
    }
}
