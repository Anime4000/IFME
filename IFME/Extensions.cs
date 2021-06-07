using System;
using System.ComponentModel;
using System.IO;

static class Extensions
{
	/// <summary>
	/// Check if item is exists
	/// </summary>
	/// <typeparam name="T">Type</typeparam>
	/// <param name="value">Item to check</param>
	/// <param name="items">Items to check</param>
	/// <returns>Return true if item is exist, false otherwise.</returns>
	internal static bool IsOneOf<T>(this T value, params T[] items)
	{
		for (int i = 0; i < items.Length; ++i)
		{
			if (items[i].Equals(value))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Check string if is blank, null, white space or string has "disable", "blank", "none", "non", "off", "no" and "0"
	/// </summary>
	/// <param name="value">String check (Case Insensetive)</param>
	/// <returns>Return true if string is blank/disable, false otherwise</returns>
	internal static bool IsDisable(this string value)
	{
		if (string.Equals("disabled", value, StringComparison.InvariantCultureIgnoreCase) ||
			string.Equals("disable", value, StringComparison.InvariantCultureIgnoreCase) ||
			string.Equals("blank", value, StringComparison.InvariantCultureIgnoreCase) ||
			string.Equals("none", value, StringComparison.InvariantCultureIgnoreCase) ||
			string.Equals("non", value, StringComparison.InvariantCultureIgnoreCase) ||
			string.Equals("off", value, StringComparison.InvariantCultureIgnoreCase) ||
			string.Equals("no", value, StringComparison.InvariantCultureIgnoreCase) ||
			string.Equals("0", value, StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}

		if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// Get language code from a file name
	/// </summary>
	/// <param name="value">File Name</param>
	/// <returns></returns>
	internal static string GetLanguageCodeFromFileName(this string value)
	{
		var file = Path.GetFileNameWithoutExtension(value);
		return file.Substring(file.Length - 3);
	}

	/// <summary>
	/// Copy a directory to new location
	/// </summary>
	/// <param name="sourceDirName">Source Directory</param>
	/// <param name="destDirName">Destination Directory</param>
	/// <param name="copySubDirs">Recursive</param>
	internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
	{
		// Get the subdirectories for the specified directory.
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);

		if (!dir.Exists)
		{
			throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
		}

		DirectoryInfo[] dirs = dir.GetDirectories();

		// If the destination directory doesn't exist, create it.       
		Directory.CreateDirectory(destDirName);

		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files)
		{
			string tempPath = Path.Combine(destDirName, file.Name);
			file.CopyTo(tempPath, true);
		}

		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirs)
		{
			foreach (DirectoryInfo subdir in dirs)
			{
				string tempPath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
			}
		}
	}
}
