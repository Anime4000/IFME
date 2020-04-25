using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFME.OSManager;

namespace IFME.FFmpeg
{
    internal class ReadFile
    {
        private string _output = string.Empty;

        internal string Media(string FilePath)
        {
            var args = $"{MediaInfo.FFmpegProbe} -v quiet -print_format json -show_format -show_streams \"{FilePath}\"";

            Environment.SetEnvironmentVariable("NEMU", args, EnvironmentVariableTarget.Process);

            var p = new Process()
            {
                StartInfo = new ProcessStartInfo(OS.Console, OS.ConsoleEnv("NEMU"))
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };

            p.OutputDataReceived += new DataReceivedEventHandler(ConsoleStandardHandler);

            p.Start();

            p.BeginOutputReadLine();

            p.WaitForExit();

            return _output;
        }

        private void ConsoleStandardHandler(object sender, DataReceivedEventArgs e)
        {
            _output += e.Data;
        }
    }
}
