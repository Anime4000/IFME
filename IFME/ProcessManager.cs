﻿using IFME.OSManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
			if (!string.IsNullOrEmpty(e.Data))
			{
				var regexPattern = @"(\d\%\] )|(time=\d)|(\| \(\d+\/\d+\))|(\x08)";
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
