using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme.framework
{
	public class ShellScript
	{
		private static string _ScriptWin = "@echo off\n" +
			"@title IFME Update\n" +
			"REM Usage:\n" +
			"REM   update.cmd \"parm 1\" \"parm 2\" \"parm 3\"\n" +
			"REM example:\n" +
			"REM   update.cmd \"http://update.example.com/myapp/1.22/file.zip\" \"C:\\Program Files\\myapp\" \"myapp.exe\"\n" +
			"echo.\n" +
			"echo This console will perfom IFME update to the latest version.\n" +
			"echo Please be patient this may take a while depending your\n" +
			"echo internet configuration, you can do other thing while updating.\n" +
			"echo.\n" +
			"TIMEOUT /T 10 /NOBREAK\n" +
			"copy %2\\unins000.exe .\\unins000.exe\n" +
			"copy %2\\unins000.dat .\\unins000.dat\n" +
			"rmdir /s /q %2\n" +
			"mkdir %2\n" +
			"wget --no-check-certificate -O .\\saishin.jp %1\n" +
			"7za x -y -o%2 .\\saishin.jp\n" +
			"copy .\\unins000.exe %2\\unins000.exe\n" +
			"copy .\\unins000.dat %2\\unins000.dat\n" +
			"echo.\n" +
			"echo.\n" +
			"@title Update Complete!\n" +
			"echo IFME will start after 3 second...\n" +
			"echo.\n" +
			"TIMEOUT /T 3 /NOBREAK\n" +
			"cd %2\n" +
			"start \"IFME\" /D %2 %2\\%3\n" +
			"exit";

		private static string _ScripLin = "#!/bin/sh\n" +
			"# Usage:\n" +
			"#   update.sh \"parm 1\" \"parm 2\" \"parm 3\"\n" +
			"# example:\n" +
			"#   update.sh \"http://update.example.com/myapp/1.22/file.zip\" \"/home/me/Desktop/myapp\" \"myapp.sh\"\n" +
			"echo \" \"\n" +
			"echo \"This terminal will perfom IFME update to the latest version.\"\n" +
			"echo \"Please be patient this may take a while depending your\"\n" +
			"echo \"internet configuration, you can do other thing while updating.\"\n" +
			"echo \" \"\n" +
			"sleep 5\n" +
			"rm -r -f \"$2\"\n" +
			"mkdir $2\n" +
			"wget --no-check-certificate -O \"./saishin.jp\" $1\n" +
			"./7za x -so \"./saishin.jp\" | tar xf - -C \"$2\"\n" +
			"echo \" \"\n" +
			"echo \" \"\n" +
			"echo \"Update complete!\"\n" +
			"echo \"IFME will start after 3 seconds...\"\n" +
			"sleep 3\n" +
			"cd \"$2\"\n" +
			"sh $3";

		public static string ScriptWin
		{
			get { return _ScriptWin; }
		}

		public static string ScriptLinux
		{
			get { return _ScripLin; }
		}
	}
}
