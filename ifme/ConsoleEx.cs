using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

enum LogLevel
{
	Normal,
	Warning,
	Error
}

namespace ifme
{
	static class ConsoleEx
	{
		internal static void Write(LogLevel level, string tag, string message)
		{
			switch (level)
			{
				case LogLevel.Normal:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case LogLevel.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogLevel.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				default:
					Console.ForegroundColor = ConsoleColor.White;
					break;
			}

			Console.Write($"[{tag}]");
			Console.ResetColor();
			Console.Write($" {message}\n");
		}
	}
}
