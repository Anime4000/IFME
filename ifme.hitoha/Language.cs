using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Asset
using ifme.hitoha;
using IniParser;
using IniParser.Model;

namespace ifme.hitoha
{
	class Language
	{
		private static string _Default = Properties.Settings.Default.DefaultLang;
		public static string Default
		{
			get { return _Default; }
			set { _Default = value; }
		}

		public static class Path
		{
			public static string Folder = Globals.AppInfo.CurrentFolder + "\\lang";
			public static string FileENG = Folder + "\\eng.ini";
		}

		public static class Section
		{
			public static string Info = "Info";
			public static string Root = "Root";
			public static string Amtd = "AudioMethod";
			public static string Lst = "TheList";
			public static string Msg = "TheMessage";
			public static string Ops = "TheOptions";
			public static string Abt = "AboutApp";
			public static string Pro = "TheProTip";
		}

		public static class Installed
		{
			private static string[,] _Data = new string[500, 5];

			public static string[,] Data
			{
				get { return _Data; }
				set { _Data = value; }
			}

			public static int Get()
			{
				int i = 0;
				foreach (var item in System.IO.Directory.GetFiles(Path.Folder))
				{
					var parser = new FileIniDataParser();
					IniData data = parser.ReadFile(item);

					Data[i, 0] = System.IO.Path.GetFileNameWithoutExtension(item);
					Data[i, 1] = FullName(Data[i, 0]);
					Data[i, 2] = data[Section.Info]["Name"];
					Data[i, 3] = data[Section.Info]["Version"];
					Data[i, 4] = data[Section.Info]["Contact"];

					i++;
				}
				return i;
			}

			public static string FullName(string code)
			{
				foreach (var item in System.IO.File.ReadAllLines(Globals.Files.ISO))
				{
					if (code == item.Substring(0, 3))
						return item.Substring(5, item.Length - 6);
				}
				return "Undetermined";
			}
		}

		public static class IMessage
		{
			public static string OpenFile = "";
			public static string OpenFolder = "";
			public static string Invalid = "";
			public static string MoveItem = "";
			public static string WrongCodec = "";
			public static string EmptySave = "";
			public static string EmptyQueue = "";
			public static string EmptySubtitle = "";
			public static string EmptyAttachment = "";
			public static string NotEqual = "";
			public static string Quit = "";
			public static string Halt = "";
			public static string InstallMsg = "";
			public static string RemoveMsg = "";
			public static string RemoveMsgErr = "";
			public static string Restart = "";
			public static string ResetSettingsAsk = "";
			public static string ResetSettingsOK = "";
			public static string ProTipTitle = "";
			public static string ProTipUpdate = "";
		}

		public static class IOptions
		{
			public static string MainWin = "";
			public static string TabGeneral = "";
			public static string TabPerf = "";
			public static string TabAddons = "";
			public static string GrpLang = "";
			public static string GrpTemp = "";
			public static string GrpFormat = "";
			public static string GrpUpdate = "";
			public static string lblLang = "";
			public static string chkUpdate = "";
			public static string lblCPUPriority = "";
			public static string lblCPUAffinity = "";
			public static string btnCPUBoost = "";
			public static string lblCPUInfo = "";
			public static string btnAddonInstall = "";
			public static string btnAddonRemove = "";
			public static string colID = "";
			public static string colName = "";
			public static string colVer = "";
			public static string colDev = "";
			public static string colProvider = "";
		}
	}
}
