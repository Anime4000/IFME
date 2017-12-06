using System.Windows.Forms;

namespace ifme
{
    public partial class frmCheckUpdate
    {
		public void InitializeUX()
		{
			LoadLanguage();
		}

		private void LoadLanguage()
		{
			if (OS.IsWindows)
            {
                Font = Language.Lang.UIFontWindows;
                rtbLog.Font = new System.Drawing.Font("Lucida Console", 8F);
            }
			else
            {
                Font = Language.Lang.UIFontLinux;
                rtbLog.Font = new System.Drawing.Font("FreeMono", 8F);
            }

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
	}
}
