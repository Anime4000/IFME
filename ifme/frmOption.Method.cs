using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmOption
    {
        public void InitializeUX()
        {
            // Load language
            LoadLanguage();

            // General
            var l = new Dictionary<string, string>();
            foreach (var item in Language.List)
                if (Get.LanguageCode.ContainsKey(item.Key))
                    l.Add(item.Key, Get.LanguageCode[item.Key]);
            cboLanguage.DataSource = new BindingSource(l, null);
            cboLanguage.DisplayMember = "Value";
            cboLanguage.ValueMember = "Key";
            cboLanguage.SelectedValue = Properties.Settings.Default.Language;            

            txtTempPath.Text = Properties.Settings.Default.TempDir;
            txtNamePrefix.Text = Properties.Settings.Default.FileNamePrefix;
            txtNamePostfix.Text = Properties.Settings.Default.FileNamePostfix;

            if (Properties.Settings.Default.FileNamePrefixType == 0)
                rdoNamePrefixNone.Checked = true;
            else if (Properties.Settings.Default.FileNamePrefixType == 1)
                rdoNamePrefixDateTime.Checked = true;
            else
                rdoNamePrefixCustom.Checked = true;

            if (Properties.Settings.Default.FileNamePostfixType == 0)
                rdoNamePostfixNone.Checked = true;
            else
                rdoNamePostfixCustom.Checked = true;

            // Encoding
            if (Properties.Settings.Default.FFmpegArch == 32)
                rdoFFmpeg32.Checked = true;
            else
                rdoFFmpeg64.Checked = true;

            if (AviSynth.IsInstalled)
            {
                lblAviSynthInstall.Text = Language.Lang.frmOption["lblAviSynthInstall"];
                lblAviSynthInstall.ForeColor = Color.Green;
                lblAviSynthVersion.Text = AviSynth.InstalledVersion;
            }
            else
            {
                lblAviSynthInstall.Text = Language.Lang.frmOption["lblAviSynthNoInstall"];
                lblAviSynthInstall.ForeColor = Color.Red;
            }

            nudFrameCountOffset.Value = Properties.Settings.Default.FrameCountOffset;

            // List all plugins
            foreach (var item in Plugin.Items)
            {
                lstModule.Items.Add(new ListViewItem(new[]
                {
                    item.Value.Name,
                    (item.Value.X64 ? "64bit" : "32bit"),
                    item.Value.Author.Developer

                }));
            }
        }

        private void LoadLanguage()
        {
            if (OS.IsWindows)
                Font = Language.Lang.UIFontWindows;
            else
                Font = Language.Lang.UIFontLinux;

            var frm = Language.Lang.frmOption;
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
