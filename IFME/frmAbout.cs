using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

using IFME.OSManager;

namespace IFME
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = $"{Text} {Version.Title}";
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            lblCodeName.Text = $"'{Version.CodeName}'";
            lblVersion.Text = $"Version {Version.Release}";
            lblCopyRight.Text = $"© 2011-{DateTime.Now.Year} Anime4000 && Contributor, Some Right Reserved.";
            lblArtWork.Text = $"Character illustration by Ray-en aka 53C for IFME Project.";
            lblDevs.Text = $"{Version.Contrib}\n\n{Version.TradeMark}";

#if SAVE_LANG
            i18n.Save(this, Name);
#else
            i18n.Apply(this, Name, Properties.Settings.Default.UILanguage);
#endif
            var font = lblCodeName.Font;
            lblCodeName.Font = new Font(font.FontFamily, 14f);
            lblVersion.Font = new Font(font.FontFamily, 14f, FontStyle.Bold);
            lblCopyRight.Font = new Font(font.FontFamily, 10f);
            lblArtWork.Font = new Font(font.FontFamily, 10f, FontStyle.Bold);
            lnkRayEn.Font = new Font(font.FontFamily, 10f);
            lblDevs.Font = new Font(font.FontFamily, 9f);
        }

        private void frmAbout_Shown(object sender, EventArgs e)
        {
            var ichika = WAD.Resource.LoadImage("Avatar1_Ichika.png");
            var fumiko = WAD.Resource.LoadImage("Avatar2_Fumiko.png");
            var miho = WAD.Resource.LoadImage("Avatar3_Miho.png");
            var erika = WAD.Resource.LoadImage("Avatar4_Erika.png");
            pbCharIchika.Image = Images.Resize(ichika, pbCharIchika.Width, pbCharIchika.Height);
            pbCharFumiko.Image = Images.Resize(fumiko, pbCharFumiko.Width, pbCharFumiko.Height);
            pbCharMiho.Image = Images.Resize(miho, pbCharMiho.Width, pbCharMiho.Height);
            pbCharErika.Image = Images.Resize(erika, pbCharErika.Width, pbCharErika.Height);

            banner.BackgroundImage = WAD.Resource.LoadImage("Banner_About.png");
        }

        private void frmAbout_Resize(object sender, EventArgs e)
        {

        }

        private void lblArtWork_Click(object sender, EventArgs e)
        {
            Process.Start("http://pixiv.me/ray53c");
        }

        private void lnkRayEn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://pixiv.me/ray53c");
        }

        private void lblHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://x265.github.io/");
        }

        private void lnkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Anime4000/IFME");
        }

        private void lnkSourceForge_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://sourceforge.net/projects/ifme/");
        }

        private void lnkFacebook_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.facebook.com/internetfriendlymediaencoder");
        }

        private void lnkDiscord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://discord.gg/4f6MDpfug2");
        }

        private void lnkHitoha_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://hitoha.ga/");
        }

        private void lnkSoraIro_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://sora-iro.nippombashi.net/");
        }
    }
}
