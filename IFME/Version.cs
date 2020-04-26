using IFME.OSManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
    public class Version
    {
        public static string Title { get { return "Internet Friendly Media Encoder"; } }
        public static string Name { get { return typeof(Program).Assembly.GetName().Name; } }
        public static string CodeName { get { return Application.ProductName; } }
        public static string Release { get { return Application.ProductVersion; } }
        public static string OSArch { get { return OS.Is64bit ? "amd64" : "x86"; } }
        public static string OSPlatform { get { return OS.IsWindows ? "windows" : "linux"; } }
    }
}
