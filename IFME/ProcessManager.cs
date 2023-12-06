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

				var p = @"(frame[ ]{1,}\d+)|(\d+.\d+[ ]{1,}kbps)|(\d+.\d+[ ]{1,}fps)";
				var x = Regex.Matches(e.Data, p, RegexOptions.IgnoreCase);
				if (x.Count >= 3)
                {
					int.TryParse(x[0].ToString().Substring(6), out int cf);

					frmMain.PrintProgress($"[{(float)cf / MediaEncoding.RealFrameCount * 100:0.0} %] Frame: {cf}, Bitrate: {x[1]}, Speed: {x[2]}");
					return;
				}
				
				var regexPattern = @"(frame[ ]{1,}\d+)|(\d+.\d+[ ]{1,}kb/s)|(\d+.\d+[ ]{1,}fps)|(\d+[ ]{1,}frames:\s\d+.\d+[ ]{1,}fps,\s\d+.\d+[ ]{1,}kb/s,\sGPU\s\d+%,\sVE\s\d+%)";
				Match m = Regex.Match(e.Data, regexPattern, RegexOptions.IgnoreCase);
				if (m.Success)
					frmMain.PrintProgress(e.Data);
				else
					frmMain.PrintLog(e.Data);
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
