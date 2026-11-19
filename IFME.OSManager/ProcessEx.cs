using System;
using System.Collections.Generic;
using System.Diagnostics;

public class ProcessEx
{
	/// <summary>
	/// One row of a process table snapshot.
	/// </summary>
	internal struct ProcessEntry
	{
		internal int Id;
		internal int ParentId;

		/// <summary>
		/// Platform-specific start time used to reject stale parent links; only compared with
		/// values from the same snapshot. 0 when unknown.
		/// </summary>
		internal long StartTime;
	}

	/// <summary>
	/// Get a Parent process id and retrive it's id of child procces
	/// </summary>
	/// <param name="ParentPID">Parent process id</param>
	/// <returns>Every descendant of the parent (children, grandchildren, ...), not including the parent itself</returns>
	public static int[] GetChildProcess(int ParentPID)
	{
		var tree = GetProcessTree(ParentPID);
		var result = new int[Math.Max(0, tree.Length - 1)];
		Array.Copy(tree, 1, result, 0, result.Length);
		return result;
	}

	/// <summary>
	/// The process and all of its descendants, parents listed before their children.
	/// </summary>
	/// <param name="RootPID">Process id at the top of the tree</param>
	public static int[] GetProcessTree(int RootPID)
	{
		List<ProcessEntry> table;

		try
		{
			table = OS.IsWindows
				? API.WindowsProcess.Snapshot()
				: API.LinuxProcess.Snapshot();
		}
		catch
		{
			table = new List<ProcessEntry>();
		}

		var children = new Dictionary<int, List<ProcessEntry>>();
		long rootStart = 0;

		foreach (var p in table)
		{
			if (p.Id == RootPID)
				rootStart = p.StartTime;

			if (p.Id == p.ParentId)
				continue; // PID 0 on Windows reports itself as its own parent

			if (!children.TryGetValue(p.ParentId, out var list))
				children[p.ParentId] = list = new List<ProcessEntry>();

			list.Add(p);
		}

		var result = new List<int> { RootPID };
		var seen = new HashSet<int> { RootPID };
		var queue = new Queue<KeyValuePair<int, long>>();
		queue.Enqueue(new KeyValuePair<int, long>(RootPID, rootStart));

		while (queue.Count > 0)
		{
			var parent = queue.Dequeue();

			if (!children.TryGetValue(parent.Key, out var list))
				continue;

			foreach (var child in list)
			{
				// A process cannot start before its parent. If it appears to, the "parent" is a
				// newer process that reused the PID of the real (dead) parent; on Windows this
				// used to make IFME suspend or kill unrelated programs.
				if (parent.Value != 0 && child.StartTime != 0 && child.StartTime < parent.Value)
					continue;

				if (!seen.Add(child.Id))
					continue;

				result.Add(child.Id);
				queue.Enqueue(new KeyValuePair<int, long>(child.Id, child.StartTime));
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// Kill a process together with every descendant
	/// </summary>
	/// <param name="ParentPID">Parent process id</param>
	public static void Terminate(int ParentPID)
	{
		var tree = GetProcessTree(ParentPID);

		// Children first, then the shell that launched them. The root is included because bash
		// may exec() a single command in place, leaving no children to kill at all.
		for (int i = tree.Length - 1; i >= 0; i--)
		{
			try
			{
				using (var p = Process.GetProcessById(tree[i]))
					p.Kill();
			}
			catch
			{
				// Already gone, or not ours to kill.
			}
		}
	}

	/// <summary>
	/// Pause a process including childs
	/// </summary>
	/// <param name="ParentID">Parent process id</param>
	public static void Pause(int ParentID)
	{
		var tree = GetProcessTree(ParentID);

		if (OS.IsWindows)
			API.WindowsProcess.Suspend(tree);
		else
			API.LinuxProcess.Suspend(tree);
	}

	/// <summary>
	/// Resume a process including childs
	/// </summary>
	/// <param name="ParentID">Parent process id</param>
	public static void Resume(int ParentID)
	{
		var tree = GetProcessTree(ParentID);
		Array.Reverse(tree);

		if (OS.IsWindows)
			API.WindowsProcess.Resume(tree);
		else
			API.LinuxProcess.Resume(tree);
	}
}
