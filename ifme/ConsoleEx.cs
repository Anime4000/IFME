using System;

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
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write($"{tag}");

			Console.ResetColor();
			Console.Write(" [");

			switch (level)
			{
				case LogLevel.Normal:
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write("info");
					break;
				case LogLevel.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write("warn");
					break;
				case LogLevel.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write("erro");
					break;
				default:
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("data");
					break;
			}

			Console.ResetColor();
			Console.Write($"]: {message}\n");
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
