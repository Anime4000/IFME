using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;

using Mono.Options;

namespace ifme
{
	static class Program
	{
		/// <summary>
		/// Change program localisation, allow to cast properly
		/// </summary>
		/// <param name="culture">Culture Id, example: en-us</param>
		static void SetDefaultCulture(CultureInfo culture)
		{
			Type type = typeof(CultureInfo);

			try
			{
				type.InvokeMember("s_userDefaultCulture",
					BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
					null,
					culture,
					new object[] { culture });

				type.InvokeMember("s_userDefaultUICulture",
					BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
					null,
					culture,
					new object[] { culture });
			}
			catch { }

			try
			{
				type.InvokeMember("m_userDefaultCulture",
					BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
					null,
					culture,
					new object[] { culture });

				type.InvokeMember("m_userDefaultUICulture",
					BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
					null,
					culture,
					new object[] { culture });
			}
			catch { }
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			SetDefaultCulture(new CultureInfo("en-us"));

			var reset = false;
			var help = false;
            var input = string.Empty;
            var start = false;

			var p = new OptionSet()
			{
				{ "r|reset", "Reset IFME to factory default", v => reset = v != null  },
				{ "h|help", "Show this message and exit", v => help = v != null },
                { "i|input=", "Load a project file", (string i) => input = i },
                { "s", "Start encoding immediately (require input)", s => start = s != null }
			};

			try
			{
				p.Parse(args);
			}
			catch (OptionException e)
			{
				Console.Error.WriteLine(e.Message);
				Console.Error.WriteLine("Try `ifme --help' for more information.");

				return;
			}

			Console.Title = Get.AppNameLongAdmin;
			Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(Get.AppNameLong);
			Console.WriteLine($"Release: {Get.AppNameLib}");
			Console.ResetColor();
			Console.WriteLine();

			if (help)
			{
				Console.Error.WriteLine("Usage: ifme [OPTION]");
				Console.Error.WriteLine("Mandatory arguments to long option are mandatory for short options too.");
				Console.Error.WriteLine("\nOptions:");
				p.WriteOptionDescriptions(Console.Error);
				Console.Error.WriteLine("\nProject home page: < https://x265.github.io/>");
				Console.Error.WriteLine("Report bugs to: <https://github.com/Anime4000/IFME/issues>");

				return;
			}

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"(c) {DateTime.Now.Year} {Branding.CopyRight()}");
			Console.ResetColor();
			Console.WriteLine();

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Warning, DO NOT close this Terminal/Console, all useful info will be shown here.");
			Console.ResetColor();
			Console.WriteLine();

			if (!OS.Is64bit)
			{
				Console.Error.WriteLine("Wrong CPU architecture, resetting!");
				Properties.Settings.Default.FFmpegArch = 32;
				Properties.Settings.Default.Save();
			}

			if (reset)
			{
				Console.Error.WriteLine("Resetting user settings.");

				Properties.Settings.Default.Reset();
				Properties.Settings.Default.UpgradeRequired = false;
				Properties.Settings.Default.Save();
			}

			if (Properties.Settings.Default.UpgradeRequired)
			{
				Console.Error.WriteLine("Updating user settings.");

				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.UpgradeRequired = false;
				Properties.Settings.Default.Save();
			}

            if (!string.IsNullOrEmpty(input))
            {
                MediaProject.ProjectFile = input;
                MediaProject.StartEncode = start;
            }

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}
	}
}
