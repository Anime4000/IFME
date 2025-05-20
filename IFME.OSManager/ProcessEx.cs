using System.Diagnostics;

public class ProcessEx
{
	/// <summary>
	/// Get a Parent process id and retrive it's id of child procces
	/// </summary>
	/// <param name="ParentPID">Parent process id</param>
	/// <returns>List of parent-child process id's</returns>
	public static int[] GetChildProcess(int ParentPID)
	{
		if (OS.IsWindows)
		{
			var proc = new API.WindowsProcess();
			return proc.ListParentChild(ParentPID);
		}
		else
		{
			var proc = new API.LinuxProcess();
			return proc.ListParentChild(ParentPID);
		}
	}

	/// <summary>
	/// Kill parent and it's child process
	/// </summary>
	/// <param name="ParentPID">Parent process id</param>
	public static void Terminate(int ParentPID)
	{
		foreach (var item in GetChildProcess(ParentPID))
		{
			var pid = Process.GetProcessById(item);
			pid.Kill();
		}
	}

	/// <summary>
	/// Pause a process including childs
	/// </summary>
	/// <param name="ParentID">Parent process id</param>
	public static void Pause(int ParentID)
	{
		if (OS.IsWindows)
		{
			var proc = new API.WindowsProcess();
			proc.PauseParentChild(ParentID);
		}
		else
		{
			var proc = new API.LinuxProcess();
			proc.PauseParentChild(ParentID);
		}
	}

	/// <summary>
	/// Resume a process including childs
	/// </summary>
	/// <param name="ParentID">Parent process id</param>
	public static void Resume(int ParentID)
	{
		if (OS.IsWindows)
		{
			var proc = new API.WindowsProcess();
			proc.ResumeParentChild(ParentID);
		}
		else
		{
			var proc = new API.LinuxProcess();
			proc.ResumeParentChild(ParentID);
		}
	}
}