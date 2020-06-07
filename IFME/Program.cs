using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			SetDefaultCulture(new CultureInfo("en-us"));

			if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

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
	}
}
