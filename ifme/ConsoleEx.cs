using System;
using System.Media;

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
		internal static void Write(string message)
		{
			Console.Error.Write(message);
		}

		internal static void Write(ConsoleColor color, string message)
		{
			Console.ForegroundColor = color;
			Console.Error.Write(message);
			Console.ResetColor();
		}
 
		internal static void Write(LogLevel level, string message)
		{
			Write(level, "ifme", message);
		}

		internal static void Write(LogLevel level, string tag, string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.Error.Write($"{tag}");

			Console.ResetColor();
			Console.Error.Write(" [");

			switch (level)
			{
				case LogLevel.Normal:
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Error.Write("info");
					break;
				case LogLevel.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Error.Write("warn");
					try { SystemSounds.Exclamation.Play(); } catch { }
					break;
				case LogLevel.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Error.Write("erro");
					try { SystemSounds.Asterisk.Play(); } catch { }
					break;
				default:
					Console.ForegroundColor = ConsoleColor.White;
					Console.Error.Write("data");
					break;
			}

			Console.ResetColor();
			Console.Error.Write($"]: {message}");
		}

		internal static void ClearCurrentLine()
		{
			int currentLineCursor = Console.CursorTop;
			Console.SetCursorPosition(0, Console.CursorTop);
			Console.Write(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(0, currentLineCursor);
		}
	}
}
