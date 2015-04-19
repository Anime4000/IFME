using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

namespace ifme
{
	class TaskManager
	{
		public static class Mod
		{
			[Flags()]
			public enum ThreadAccess : int
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
			private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, System.UInt32 dwThreadId);
			[DllImport("kernel32.dll")]
			private static extern System.UInt32 SuspendThread(IntPtr hThread);
			[DllImport("kernel32.dll")]
			private static extern System.UInt32 ResumeThread(IntPtr hThread);
			[DllImport("kernel32.dll")]
			private static extern bool CloseHandle(IntPtr hHandle);

			public static void SuspendProcess(System.Diagnostics.Process process)
			{
				foreach (ProcessThread t in process.Threads)
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

			public static void ResumeProcess(System.Diagnostics.Process process)
			{
				foreach (ProcessThread t in process.Threads)
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
		}

		public static class ModLinux
		{
			public static void SuspendProcess(int pid)
			{
				Process P = new Process();
				var SI = P.StartInfo;

				SI.FileName = "bash";
				SI.Arguments = String.Format("-c \"kill -STOP {0}\"", pid);
				SI.UseShellExecute = false;
				SI.CreateNoWindow = true;
				SI.RedirectStandardError = true;
				SI.RedirectStandardOutput = true;

				P.Start();
			}

			public static void ResumeProcess(int pid)
			{
				Process P = new Process();
				var SI = P.StartInfo;

				SI.FileName = "bash";
				SI.Arguments = String.Format("-c \"kill -CONT {0}\"", pid);
				SI.UseShellExecute = false;
				SI.CreateNoWindow = true;
				SI.RedirectStandardError = true;
				SI.RedirectStandardOutput = true;

				P.Start();
			}
		}

		public static class ImageName
		{
			private static int _Id = 0;
			private static string _SIM = "";
			private static int _LV = 3;

			public static int Id
			{
				get { return _Id; }
				set { _Id = value; }
			}

			public static string Current
			{
				get { return _SIM; }
				set { _SIM = value; }
			}

			public static int Level
			{
				get { return _LV; }
				set { _LV = value; }
			}
		}

		public static class CPU
		{
			public static bool[] Affinity = new bool[Environment.ProcessorCount];

			public static string DefaultAll(bool yes)
			{
				string x = "";
				for (int i = 0; i < Environment.ProcessorCount; i++)
				{
					Affinity[i] = yes;
					x += yes.ToString() + ",";
				}
				x = x.Remove(x.Length - 1);

				return x;
			}

			public static string GetAffinity()
			{
				BitArray bin = new BitArray(TaskManager.CPU.Affinity);
				byte[] data = new byte[1];
				bin.CopyTo(data, 0);
				return BitConverter.ToString(data, 0);
			}

			public static void Kill(string s)
			{
				Process[] Task = Process.GetProcessesByName(s);
				foreach (Process P in Task)
				{
					P.Kill();
				}
			}

			public static void Set(string app)
			{
				var Nice = Properties.Settings.Default.Nice;

				try
				{
					Process[] Task = Process.GetProcessesByName(app);
					foreach (Process P in Task)
					{
						P.ProcessorAffinity = (IntPtr)Int32.Parse(GetAffinity(), System.Globalization.NumberStyles.HexNumber);
						ImageName.Id = P.Id;

						if (Nice == 0)
							P.PriorityClass = ProcessPriorityClass.RealTime;
						else if (Nice == 1)
							P.PriorityClass = ProcessPriorityClass.High;
						else if (Nice == 2)
							P.PriorityClass = ProcessPriorityClass.AboveNormal;
						else if (Nice == 3)
							P.PriorityClass = ProcessPriorityClass.Normal;
						else if (Nice == 4)
							P.PriorityClass = ProcessPriorityClass.BelowNormal;
						else if (Nice == 5)
							P.PriorityClass = ProcessPriorityClass.Idle;
						else
							P.PriorityClass = ProcessPriorityClass.Normal;
					}
				}
				catch (Exception ex)
				{
					EventLog.WriteEntry("IFME", ex.Message);
				}
			}
		}

		public static void SetPerformance(string exe, string args)
		{
			System.Threading.Thread.Sleep(100);

			string hevclo = Path.GetFileNameWithoutExtension(Addons.BuildIn.HEVCLO);
			string hevchi = Path.GetFileNameWithoutExtension(Addons.BuildIn.HEVCHI);

			if (args.Contains(hevclo))
			{
				ImageName.Current = hevclo;
				CPU.Set(hevclo);
			}
			else if (args.Contains(hevchi))
			{
				ImageName.Current = hevchi;
				CPU.Set(hevchi);
			}
			else
			{
				ImageName.Current = Path.GetFileNameWithoutExtension(exe);
				CPU.Set(Path.GetFileNameWithoutExtension(exe));
			}
		}
	}
}
