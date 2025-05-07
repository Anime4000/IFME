using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            cboLanguage.Items.Add("English");
            cboLanguage.SelectedIndex = 0;

#if SAVE_LANG
            LocaliserUI.Save(this, Name);
#else
            LocaliserUI.Apply(this, Name, Properties.Settings.Default.UILanguage);
#endif
        }

        private void frmOptions_Shown(object sender, EventArgs e)
        {
            txtTempPath.Text = Properties.Settings.Default.FolderTemporary;
            txtPrefix.Text = Properties.Settings.Default.PrefixText;
            txtPostfix.Text = Properties.Settings.Default.PostfixText;

            switch (Properties.Settings.Default.PrefixMode)
            {
                case 0:
                    rdoPrefixNone.Checked = true;
                    break;
                case 1:
                    rdoPrefixDateTime.Checked = true;
                    break;
                default:
                    rdoPrefixCustom.Checked = true;
                    break;
            }

            switch (Properties.Settings.Default.PostfixMode)
            {
                case 0:
                    rdoPostfixNone.Checked = true;
                    break;
                case 1:
                    rdoPostfixDateTime.Checked = true;
                    break;
                default:
                    rdoPostfixCustom.Checked = true;
                    break;
            }

            var disabled = Properties.Settings.Default.PluginsDisabled.Split(',').Select(Guid.Parse).ToList();
            foreach (var item in Plugins.Items.Lists)
            {
                lstPlugins.Items.Add(new ListViewItem(new[]
                {
                    item.Value.Name,
                    item.Value.X64 ? "64-bit" : "32-bit",
                    item.Value.Version,
                    item.Value.Author.Developer
                })
                {
                    Tag = item.Key,
                    Checked = !disabled.Contains(item.Key)
                });
            }

            chkSkipTest.Checked = Properties.Settings.Default.TestEncoder;

            FileNameExample();

            // show muxer options
            Mp4MuxFlags mp4MuxFlags = (Mp4MuxFlags)Properties.Settings.Default.Mp4MuxFlags;
            chkMuxMp4FK.Checked = mp4MuxFlags.HasFlag(Mp4MuxFlags.FragKeyframe);
            chkMuxMp4EM.Checked = mp4MuxFlags.HasFlag(Mp4MuxFlags.EmptyMoov);
            chkMuxMp4SM.Checked = mp4MuxFlags.HasFlag(Mp4MuxFlags.SeparateMoof);

            // in case encoder test failed, show this to user
            int checkedItemCount = lstPlugins.Items.Cast<ListViewItem>().Count(item => item.Checked);
            if (checkedItemCount == 0)
            {
                tabSetting.SelectedTab = tabPlugins;
                chkSkipTest.ForeColor = Color.Red;
            }

            // show profile lists
            foreach (var item in Profiles.Items)
            {
                lstProfiles.Items.Add(new ListViewItem(new[]
                {
                    item.ProfileName,
                    Path.GetFileName(item.ProfilePath),
                    item.ProfileAuthor,
                    item.ProfileWebUrl
                })
                {
                    Tag = item.ProfileWebUrl
                });
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FolderTemporary = txtTempPath.Text;
            Properties.Settings.Default.PrefixText = txtPrefix.Text;
            Properties.Settings.Default.PostfixText = txtPostfix.Text;

            if (rdoPrefixNone.Checked)
                Properties.Settings.Default.PrefixMode = 0;

            if (rdoPrefixDateTime.Checked)
                Properties.Settings.Default.PrefixMode = 1;

            if (rdoPrefixCustom.Checked)
                Properties.Settings.Default.PrefixMode = 2;

            if (rdoPostfixNone.Checked)
                Properties.Settings.Default.PostfixMode = 0;

            if (rdoPostfixDateTime.Checked)
                Properties.Settings.Default.PostfixMode = 1;

            if (rdoPostfixCustom.Checked)
                Properties.Settings.Default.PostfixMode = 2;

            // save muxer options
            Mp4MuxFlags mp4MuxFlags = Mp4MuxFlags.None;
            if (chkMuxMp4FK.Checked)
                mp4MuxFlags |= Mp4MuxFlags.FragKeyframe;
            if (chkMuxMp4EM.Checked)
                mp4MuxFlags |= Mp4MuxFlags.EmptyMoov;
            if (chkMuxMp4SM.Checked)
                mp4MuxFlags |= Mp4MuxFlags.SeparateMoof;
            Properties.Settings.Default.Mp4MuxFlags = (int)mp4MuxFlags;

            var disabled = new List<Guid>();
            foreach (ListViewItem item in lstPlugins.Items)
            {
                if (!item.Checked)
                    disabled.Add((Guid)item.Tag);
            }
            Properties.Settings.Default.PluginsDisabled = string.Join(",", disabled);

            if (string.IsNullOrEmpty(Properties.Settings.Default.PluginsDisabled))
                Properties.Settings.Default.PluginsDisabled = "00000000-0000-0000-0000-000000000000,aaaaaaaa-0000-0000-0000-000000000000,ffffffff-ffff-ffff-ffff-ffffffffffff";

            Properties.Settings.Default.TestEncoder = chkSkipTest.Checked;

            Properties.Settings.Default.Save();
            
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnTempBrowse_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = false,
                Title = "Select desire temporary folder (must be empty!)",
                FileName = "TEMP",
                InitialDirectory = txtTempPath.Text,
                AutoUpgradeEnabled = true
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var fPath = Path.GetDirectoryName(fbd.FileName);
                txtTempPath.Text = fPath;
            }
        }

        private void rdoPrePostFixFilename_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                FileNameExample();
            }
        }

        private void txtPrefix_TextChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text))
                {
                    rdoPrefixNone.Checked = true;
                }
                else
                {
                    rdoPrefixCustom.Checked = true;
                    FileNameExample();
                }
            }
        }

        private void txtPostfix_TextChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text))
                {
                    rdoPostfixNone.Checked = true;
                }
                else
                {
                    rdoPostfixCustom.Checked = true;
                    FileNameExample();
                }
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstPlugins.Items)
            {
                item.Checked = true;
            }
        }

        private void btnOnly265_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstPlugins.Items)
            {
                if (((Guid)item.Tag).Equals(new Guid("deadbeef-0265-0265-0265-026502650265")))
                    item.Checked = true;
                else
                    item.Checked = false;
            }
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstPlugins.Items)
            {
                item.Checked = false;
            }
        }

        private void chkSkipTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as CheckBox).Checked)
            {
                var msg = MessageBox.Show("Disable encoder test could lead to broken results, glitch, instability, incompatible host & CPU\n\nUSE AT YOUR OWN RISK, NO SUPPORT AFTER THIS!", "You are about to disable encoder test!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (msg == DialogResult.No)
                {
                    chkSkipTest.Checked = true;
                }
                else
                {
                    var final = MessageBox.Show("ARE YOU SURE?", "FINAL WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                    if (final == DialogResult.No)
                    {
                        chkSkipTest.Checked = true;
                    }
                }
            }
        }

        private void FileNameExample()
        {
            var preFix = string.Empty;
            var postFix = string.Empty;

            if (rdoPrefixCustom.Checked)
            {
                preFix = txtPrefix.Text;
            }
            else if (rdoPrefixDateTime.Checked)
            {
                preFix = $"[{DateTime.Now:yyyy-MM-dd_HH-mm-ss}] ";
            }

            if (rdoPostfixCustom.Checked)
            {
                postFix = txtPostfix.Text;
            }
            else if (rdoPostfixDateTime.Checked)
            {
                postFix = $" [{DateTime.Now:yyyy-MM-dd_HH-mm-ss}]";
            }

            lblFileNameEx.Text = $"{preFix}{Properties.Settings.Default.FileNameExample}{postFix}.mkv";
        }

        private void lblFileNameEx_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/watch?v=V47hPO7gSok");
        }

        private void btnFactoryReset_Click(object sender, EventArgs e)
        {
            var msg = MessageBox.Show("This will reset to factory settings, all custom settings will be deleted!\n\nProceed such action?", "Factory Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (msg == DialogResult.Yes)
                Properties.Settings.Default.Reset();
        }
    }
}
