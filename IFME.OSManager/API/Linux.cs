using System;
using System.Diagnostics;
using System.Collections.Generic;

internal partial class API
{
	internal class LinuxProcess
	{
		private List<int> _pgrep = new List<int>();

		internal int[] ListParentChild(int value)
		{
			var args = $"pgrep -P {value}";

			Environment.SetEnvironmentVariable("PGREP_PID", args, EnvironmentVariableTarget.Process);

			var p = new Process()
			{
				StartInfo = new ProcessStartInfo(OS.Console, OS.ConsoleEnv("PGREP_PID"))
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

			return _pgrep.ToArray();
		}

		private void ConsoleStandardHandler(object sender, DataReceivedEventArgs e)
		{
			if (int.TryParse(e.Data, out int pid))
			{
				_pgrep.Add(pid);
			}
		}

		internal void PauseParentChild(int value)
		{
			foreach (var pid in ListParentChild(value))
			{
				Process.Start("kill", $"-STOP {pid} > /dev/null 2>&1");
			}
		}

		internal void ResumeParentChild(int value)
		{
			foreach (var pid in ListParentChild(value))
			{
				Process.Start("kill", $"-CONT {pid} > /dev/null 2>&1");
			}
		}
	}
}