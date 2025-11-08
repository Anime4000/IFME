using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

static class Extensions
{
    /// <summary>
    /// Retrieves the string value associated with the <see cref="EnumMemberAttribute"/> of an enum value. 
    /// If the attribute is not present, the default string representation of the enum value is returned.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>The attributed string value, or the default string if the attribute is missing.</returns>
    public static string GetEnumMemberValue(this Enum value)
    {
        Type type = value.GetType();
        // Get the field info for the specific enum value
        FieldInfo field = type.GetField(value.ToString());

        if (field != null)
        {
            // Try to retrieve the EnumMemberAttribute
            var attribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute), false);

            if (attribute != null)
            {
                // Return the Value from the attribute
                return attribute.Value;
            }
        }

        // If no EnumMemberAttribute is found, return the field name as a string (default behavior)
        return value.ToString();
    }

    /// <summary>
    /// Checks whether the input string contains format specifiers like {0}, {1}, {2}, up to {n}.
    /// </summary>
    /// <param name="format">The string to check for format specifiers.</param>
    /// <returns>True if the string contains format specifiers, otherwise false.</returns>
    internal static bool HasFormatSpecifiers(this string format)
    {
        // Regular expression pattern to match format specifiers like {0}, {1}, {2}, etc.
        string pattern = @"\{\d+\}";

        // Returns true if the string contains any format specifier matching the pattern.
        return Regex.IsMatch(format, pattern);
    }

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
            string.Equals("null", value, StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals("nan", value, StringComparison.InvariantCultureIgnoreCase) ||
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
			string tempPath = AppPath.Combine(destDirName, file.Name);
			file.CopyTo(tempPath, true);
		}

		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirs)
		{
			foreach (DirectoryInfo subdir in dirs)
			{
				string tempPath = AppPath.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
			}
		}
	}
}
