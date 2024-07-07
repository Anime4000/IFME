using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IFME.OSManager;

namespace IFME
{
	internal class ProcessManager
	{
		private static List<int> ProcessId = new List<int>();

		internal static bool IsPause = false;

		private TimeSpan eta = new TimeSpan(0, 0, 0);
        private List<double> recentFps;
        private const int sampleSize = 5;

        public ProcessManager()
        {
            recentFps = new List<double>();
        }

        internal static int Start(string Command)
		{
			return new ProcessManager().Run(Command, string.Empty);
		}

		internal static int Start(string WorkingDirectory, string Command)
		{
			return new ProcessManager().Run(Command, WorkingDirectory);
		}

		private int Run(string Command, string WorkingDirectory)
		{
			var EnvId = RandomGen.String(7);

			// replace double or more space with single space except in quote char (" ' `)
			Command = Regex.Replace(Command, "\\s{2,}(?=(?:[^'\"`]*(['\"`])[^'\"`]*\\1)*[^'\"`]*$)", " ");

			Environment.SetEnvironmentVariable(EnvId, Command, EnvironmentVariableTarget.Process);

			var cmd = OS.IsWindows ? "cmd" : "bash";
			var arg = OS.IsWindows ? $"/c %{EnvId}%" : $"-c 'eval ${EnvId}'";

			Process proc = new Process
			{
				StartInfo = new ProcessStartInfo(cmd, arg)
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					WorkingDirectory = WorkingDirectory,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}
			};

#if DEBUG
			frmMain.PrintLog($"[DEBG] Command Line: {Command}");
#endif

			proc.OutputDataReceived += Proc_DataReceived;
			proc.ErrorDataReceived += Proc_DataReceived;

			proc.Start();

			ProcessId.Add(proc.Id);

			proc.BeginOutputReadLine();
			proc.BeginErrorReadLine();

			proc.WaitForExit();

			ProcessId.Remove(proc.Id);
			
			return proc.ExitCode;
		}

		private void Proc_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (frmMain.frmMainStatic == null)
				return;

			if (!string.IsNullOrEmpty(e.Data))
			{
                var tf = @"(?<=encoded\s) ?\d+(?=> frames in \d+.\d+)?"; //x265 encoded total frame
                var tfm = Regex.Matches(e.Data, tf, RegexOptions.IgnoreCase);
                if (tfm.Count > 0)
                {
                    if (tfm.Count > 0)
                    {
                        if (!int.TryParse(tfm[0].Value, out int rfc))
                            MediaEncoding.RealFrameCount = rfc;
                    }

                    return;
                }

                var regexPattern = @"( \d+ bits )|( \d+ seconds)|(\d+/\d{3})|(size=[ ]{1,}\d+)|(frame[ ]{1,}\d+)|(\d+.\d+[ ]{1,}kb/s)|(\d+.\d+[ ]{1,}fps)|(\d+[ ]{1,}frames:\s\d+.\d+[ ]{1,}fps,\s\d+.\d+[ ]{1,}kb/s,\sGPU\s\d+%,\sVE\s\d+%)";
                Match m = Regex.Match(e.Data, regexPattern, RegexOptions.IgnoreCase);
                if (m.Success)
                    frmMain.PrintProgress(e.Data);
                else
                    frmMain.PrintLog(e.Data);

                var patterns = new[]
				{
                    @"vvenc \[info\]: stats:  frame=\s*(\d+) .* avg_fps=\s*([\d\.]+) .* avg_bitrate=\s*([\d\.]+) kbps", // Fraunhofer VVC
					@"\[\d+\.\d+%\] (\d+)/\d+ frames, ([\d\.]+) fps, ([\d\.]+) kb/s", // x264 & x265
					@"(\d+) frames: ([\d\.]+) fps, ([\d\.]+) kb/s", // Rigaya NVEnc
					@"frame=\s*(\d+) fps=\s*([\d\.]+) .* bitrate=\s*([\d\.]+)kbits/s", // FFmpeg
					@"Encoding frame\s*(\d+)\s* ([\d\.]+) kbps\s* ([\d\.]+) fps" // SVT-AV1
                };

				foreach (var pattern in patterns) 
				{
                    var match = Regex.Match(e.Data, pattern);
                    if (match.Success)
                    {
						int frame;
						double bitrate, speed;

						int.TryParse(match.Groups[1].Value, out int a);
						double.TryParse(match.Groups[2].Value, out double b);
						double.TryParse(match.Groups[3].Value, out double c);

						frame = a;

						if (pattern.EndsWith("fps")) // SVT-AV1 position fps last compared with others encoder
						{
							bitrate = b;
							speed = c;
						}
						else
						{
                            bitrate = c;
                            speed = b;
                        }

                        double percentage = (double)frame / MediaEncoding.RealFrameCount * 100;

                        // Update recent fps list
                        recentFps.Add(speed);
                        if (recentFps.Count > sampleSize)
                        {
                            recentFps.RemoveAt(0);
                        }

                        // Calculate ETA
                        if (recentFps.Count == sampleSize)
                        {
                            double averageFps = 0;
                            foreach (var fps in recentFps)
                            {
                                averageFps += fps;
                            }
                            averageFps /= sampleSize;
                            averageFps = Math.Round(averageFps); // Round to the nearest whole number

                            if (averageFps > 0)
                            {
                                int remainingFrames = MediaEncoding.RealFrameCount - frame;
                                double remainingTime = remainingFrames / averageFps;

                                if (remainingTime > TimeSpan.MaxValue.TotalSeconds)
                                {
                                    remainingTime = TimeSpan.MaxValue.TotalSeconds;
                                }
                                else if (remainingTime < TimeSpan.MinValue.TotalSeconds)
                                {
                                    remainingTime = TimeSpan.MinValue.TotalSeconds;
                                }

                                try
                                {
                                    eta = TimeSpan.FromSeconds(remainingTime);
                                }
                                catch (Exception ex)
                                {
                                    frmMain.PrintLog($"[WARN] ETA Logic is crashed: {ex.Message}");

                                }
                            }
                        }

                        frmMain.PrintProgress($"[{percentage:0.00} %] Frame: {frame}, Bitrate: {bitrate:0} kb/s, Speed: {speed:0.00} fps, ETA: {eta:hh\\:mm\\:ss}");

                        return;
                    }
                }
			}
		}

		internal static void Clear()
		{
			ProcessId.Clear();
		}

		internal static void Stop()
		{
			foreach (var pid in ProcessId)
			{
				ProcessEx.Terminate(pid);
			}
		}

		internal static void Pause()
		{
			foreach (var pid in ProcessId)
			{
				ProcessEx.Pause(pid);
			}

			IsPause = true;
		}

		internal static void Resume()
		{
			foreach (var pid in ProcessId)
			{
				ProcessEx.Resume(pid);
			}

			IsPause = false;
		}

		internal static void Donate()
		{
			Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U");
		}
	}
}
