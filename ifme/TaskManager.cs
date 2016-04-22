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
        static string Temp { get { return Properties.Settings.Default.TempFolder; } }
        static int MaxLine { get { return Properties.Settings.Default.LogMaxLines; } }

        public static int Run(string command, string workDir)
        {
            Environment.SetEnvironmentVariable("RITSUKO", $"{command}", EnvironmentVariableTarget.Process);

            var enc = new Process();

            if (OS.IsWindows)
            {
                enc.StartInfo = new ProcessStartInfo("cmd", "/c %RITSUKO%")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = workDir,
                };
            }
            else
            {
                enc.StartInfo = new ProcessStartInfo("bash", "-c 'eval $RITSUKO'")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = workDir,
                };
            }

            enc.Start();
            enc.WaitForExit();

            return enc.ExitCode;
        }

        public static int Run(string command)
        {
            return Run(command, Temp);
        }

        public static int RunLogs(string command)
        {
            return Run($"{command} >> encoding.log 2>&1");
        }

        public static int RunTail()
        {
            var tail = new Process();

            if (OS.IsWindows)
            {
                tail.StartInfo = new ProcessStartInfo("tail", $"--retry -n {MaxLine} -qf encoding.log")
                {
                    UseShellExecute = false,
                    WorkingDirectory = Temp,
                };
            }
            else
            {
                tail.StartInfo = new ProcessStartInfo("xterm", $"-geometry 120x30 -e 'tail --retry -n {MaxLine} -qf encoding.log'")
                {
                    UseShellExecute = false,
                    WorkingDirectory = Temp,
                };
            }

            tail.Start();
            tail.WaitForExit();

            return tail.ExitCode;
        }
    }
}
