using System.Windows.Forms;

namespace ifme
{
	public partial class frmAbout
	{
		public void InitializeUX()
		{
			pbBanner.Image = Branding.About();

			lblMainTitle.Text = Get.AppName;
			lblMainVersion.Text = $"{Get.AppNameLib} ('{Properties.Resources.AppCodeName}')";

			if (!string.IsNullOrEmpty(Branding.Author()))
				lblInfo.Text = Branding.Author();

			LoadLanguage();
		}

		private void LoadLanguage()
		{
			if (OS.IsWindows)
				Font = Language.Lang.UIFontWindows;
			else
				Font = Language.Lang.UIFontLinux;

			lblDevTtitle.Font = new System.Drawing.Font(Font.Name, 9, System.Drawing.FontStyle.Bold);
			lblTechTitle.Font = lblDevTtitle.Font;
			lblArtTitle.Font = lblDevTtitle.Font;

			var frm = Language.Lang.frmAbout;
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
	}
}
