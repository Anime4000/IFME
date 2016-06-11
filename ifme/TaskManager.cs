using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ifme
{
    public class TaskManager
    {
        public static int CurrentId { get; set; }

        public int RunCmd(string Bin, string Arg)
        {
            return Run($"{Bin} {Arg}");
        }

        public int RunCmd(string Bin1, string Arg1, string Bin2, string Arg2)
        {
            return Run($"{Bin1} {Arg1} 2> {OS.NULL} | {Bin2} {Arg2}");
        }

        private int Run(string Command)
        {
            Environment.SetEnvironmentVariable("HOTARU", "Internet Friendly Media Encoder", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("RITSUKO", $"{Command}", EnvironmentVariableTarget.Process);

            var p = new Process();

            if (OS.IsWindows)
            {
                p.StartInfo = new ProcessStartInfo("cmd", "/c title %HOTARU% && echo %HOTARU% && %RITSUKO%")
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = Properties.Settings.Default.TempFolder,
                };
            }
            else
            {
                p.StartInfo = new ProcessStartInfo("xterm", "-xrm 'XTerm.vt100.allowTitleOps: false' -T '$HOTARU' -geometry 120x30 -e 'eval $RITSUKO'")
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = Properties.Settings.Default.TempFolder,
                };
            }

            p.Start();
            CurrentId = p.Id;
            p.WaitForExit();

            return p.ExitCode;
        }
    }
}
