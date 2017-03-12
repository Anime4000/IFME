using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ifme
{
	public class ProcessManager
	{
		[Flags()]
		enum ThreadAccess : int
		{
			TERMINATE = 1,
			SUSPEND_RESUME = 2,
			GET_CONTEXT = 8,
			SET_CONTEXT = 16,
			SET_INFORMATION = 32,
			QUERY_INFORMATION = 64,
			SET_THREAD_TOKEN = 128,
			IMPERSONATE = 256,
			DIRECT_IMPERSONATION = 512,
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
		[DllImport("kernel32.dll")]
		private static extern uint SuspendThread(IntPtr hThread);
		[DllImport("kernel32.dll")]
		private static extern uint ResumeThread(IntPtr hThread);
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hHandle);

		Process Proc = new Process();
		string EnvCmd = string.Empty;
		string CrProc = string.Empty;

		public ProcessManager(string ExePath, string Args)
		{
			CrProc = Path.GetFileNameWithoutExtension(ExePath);
			EnvCmd = $"\"{ExePath}\" {Args}";
		}

		public ProcessManager(string ExePath, string Args, string PipeExePath, string PipeArgs)
		{
			CrProc = Path.GetFileNameWithoutExtension(PipeExePath);
			EnvCmd = $"\"{ExePath}\" {Args} | \"{PipeExePath}\" {PipeArgs}";
		}

		public void Run()
		{
			var cmd = string.Empty;
			var arg = string.Empty;

			Environment.SetEnvironmentVariable("HITOHA", EnvCmd, EnvironmentVariableTarget.Process);

			if (OS.IsWindows)
			{
				cmd = "cmd";
				arg = "/c %HITOHA%";
			}
			else
			{
				cmd = "bash";
				arg = "-c 'eval $HITOHA'";
			}

			Proc.StartInfo = new ProcessStartInfo("", "")
			{
				UseShellExecute = false,
				WorkingDirectory = Properties.Settings.Default.TempDir
			};

			Proc.Start();
		}

		public int Wait()
		{
			Proc.WaitForExit();
			return Proc.ExitCode;
		}

		public void Stop()
		{
			Process[] Task = Process.GetProcessesByName(CrProc);
			foreach (Process p in Task)
			{
				p.Kill();
			}
		}

		public void Pause()
		{
			foreach (var item in Process.GetProcessesByName(CrProc))
			{
				if (OS.IsWindows)
				{
					foreach (ProcessThread t in item.Threads)
					{
						IntPtr th;
						th = OpenThread(ThreadAccess.SUSPEND_RESUME, false, Convert.ToUInt32(t.Id));
						if ((th != IntPtr.Zero))
						{
							SuspendThread(th);
							CloseHandle(th);
						}
					}
				}
				else
				{
					Process.Start("kill", $"-STOP {item.Id} > /dev/null 2>&1");
				}
			}
		}

		public void Resume()
		{
			foreach (var item in Process.GetProcessesByName(CrProc))
			{
				if (OS.IsWindows)
				{
					foreach (ProcessThread t in item.Threads)
					{
						IntPtr th;
						th = OpenThread(ThreadAccess.SUSPEND_RESUME, false, Convert.ToUInt32(t.Id));
						if ((th != IntPtr.Zero))
						{
							ResumeThread(th);
							CloseHandle(th);
						}
					}
				}
				else
				{
					Process.Start("kill", $"-CONT {item.Id} > /dev/null 2>&1");
				}
			}
		}
	}
}
