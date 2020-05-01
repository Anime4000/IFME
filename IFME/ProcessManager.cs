using IFME.OSManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

			proc.OutputDataReceived += Proc_OutputDataReceived;
			proc.ErrorDataReceived += Proc_ErrorDataReceived;

			proc.Start();

			ProcessId.Add(proc.Id);

			proc.BeginOutputReadLine();
			proc.BeginErrorReadLine();

			proc.WaitForExit();

			ProcessId.Remove(proc.Id);
			
			return proc.ExitCode;
		}

		private void Proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{

				Console2.WriteLine(e.Data);
			}
		}

		private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				Console2.WriteLine(e.Data);
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
