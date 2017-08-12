using System.Windows.Forms;

namespace ifme
{
	public partial class frmCheckUpdate
	{
		public void InitializeUX()
		{
			LoadLanguage();
			DownloadLog();
		}

		private void LoadLanguage()
		{
			if (OS.IsWindows)
				Font = Language.Lang.UIFontWindows;
			else
				Font = Language.Lang.UIFontLinux;

			var frm = Language.Lang.frmCheckUpdate;
			Control ctrl = this;

			do
			{
				ctrl = GetNextControl(ctrl, true);

				if (ctrl != null)
					if (ctrl is Label ||
						ctrl is Button ||
						ctrl is TabPage ||
						ctrl is CheckBox ||
						ctrl is RadioButton ||
						ctrl is GroupBox)
						if (frm.ContainsKey(ctrl.Name))
							ctrl.Text = frm[ctrl.Name];

			} while (ctrl != null);
		}

		private void DownloadLog()
		{
			rtbLog.Text = new Download().GetString("https://github.com/Anime4000/IFME/raw/master/changelog.txt");
		}
	}
}
