using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmShutdown
    {
        public void InitializeUX()
        {
            // Load language
            LoadLanguage();

            // Default
            try
            {
                cboShutdown.SelectedIndex = Properties.Settings.Default.ShutdownType;
            }
            catch
            {
                cboShutdown.SelectedIndex = 0;
            }
        }

        private void LoadLanguage()
        {
            if (OS.IsWindows)
                Font = Language.Lang.UIFontWindows;
            else
                Font = Language.Lang.UIFontLinux;

            var frm = Language.Lang.frmShutdown;
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

            cboShutdown.Items.Clear();
            cboShutdown.Items.AddRange(Language.Lang.ComboBoxShutdown.ToArray());
        }
    }
}
