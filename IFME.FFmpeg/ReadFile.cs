using System;
using System.Diagnostics;
using System.Text;

namespace IFME.FFmpeg
{
    internal class ReadFile
    {
        internal string Media(string filePath, string frameRate = "")
        {
            var fps = string.Empty;

            if (!string.IsNullOrEmpty(frameRate))
                fps = $"-framerate {frameRate}";

            // Quote the probe path: it is absolute (under the install folder) when ffprobe is not
            // in PATH, and install folders such as "Program Files" contain spaces.
            var args = $"\"{MediaInfo.FFmpegProbe}\" -v quiet -print_format json -show_format -show_streams {fps} \"{filePath}\"";

            // Previously the command went through a process-wide "NEMU" variable, which two
            // files probed in parallel could overwrite before the shell read it.
            var info = OS.ShellStartInfo(args);
            info.RedirectStandardOutput = true;
            info.StandardOutputEncoding = Encoding.UTF8; // make Unicode aware

            var output = new StringBuilder();

            using (var p = new Process { StartInfo = info })
            {
                p.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        output.Append(e.Data);
                };

                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }

            return output.ToString();
        }
    }
}
