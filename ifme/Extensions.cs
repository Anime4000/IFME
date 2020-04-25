﻿using System;
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
}
