using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME.OSManager
{
    public class OS
    {
        public static bool IsWindows
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT;
            }
        }

        public static bool IsUnix
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Unix;
            }
        }

        public static bool IsMacOSX
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.MacOSX;
            }
        }

        public static bool IsLinux
        {
            get
            {
                return (IsUnix) || (IsMacOSX) || (Environment.OSVersion.Platform == (PlatformID)128);
            }
        }

        public static bool Is64bit
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }

        public static string Null
        {
            get
            {
                if (IsWindows)
                    return "nul";
                else
                    return "/dev/null";
            }
        }

        public static string Console
        {
            get
            {
                if (IsWindows)
                    return "cmd";
                else
                    return "bash";
            }
        }

        public static string ConsoleCmd
        {
            get
            {
                if (IsWindows)
                    return "/c";

                return "-c";
            }
        }

        public static string ConsoleEnv(string EnvironmentVariable)
        {
            if (IsWindows)
                return $"/c %{EnvironmentVariable}%";

            return $"-c 'eval ${EnvironmentVariable}'";
        }

        public static void PowerOff(int delay = 0)
        {
            if (IsWindows)
                Process.Start("shutdown.exe", $"-s -f -t {delay}");
            else if (IsLinux)
                Process.Start("bash", "-c 'poweroff'");
        }

        public static string PrintFileSize(ulong value)
        {
            if (IsWindows)
            {
                string[] IEC = { "Bytes", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

                if (value == 0)
                    return $"0 {IEC[0]}";

                long bytes = Math.Abs((long)value);
                int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                return $"{(Math.Sign((long)value) * num)} {IEC[place]}";
            }
            else
            {
                string[] DEC = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };

                if (value == 0)
                    return $"0 {DEC[0]}";

                long bytes = Math.Abs((long)value);
                int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
                double num = Math.Round(bytes / Math.Pow(1000, place), 1);
                return $"{(Math.Sign((long)value) * num)} {DEC[place]}";
            }
        }

        public static List<string> FilesRecursive()
        {
            var files = new List<string>();
            var fbd = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var dirs = new List<string>(Directory.GetDirectories(fbd.SelectedPath));

                if (dirs.Count == 0)
                    dirs.Add(fbd.SelectedPath);

                foreach (var d in dirs)
                {
                    foreach (var f in Directory.GetFiles(d))
                    {
                        files.Add(f);
                    }
                }
            }

            return files;
        }
    }
}
