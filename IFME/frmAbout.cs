using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = $"{Text} {Version.Title}";
            FormBorderStyle = FormBorderStyle.Sizable;

            
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            lblCodeName.Text = $"'{Version.CodeName}'";
            lblVersion.Text = $"Version {Version.Release}";
            lblCopyRight.Text = $"© 2011-{DateTime.Now.Year} Anime4000 && Contributor, Some Right Reserved.";
            lblArtWork.Text = $"Character illustration by Ray-en aka 53C for IFME Project.";
            lblDevs.Text = Version.TradeMark;
        }

        private void frmAbout_Shown(object sender, EventArgs e)
        {
            pbCharIchika.Image = Images.Resize(Properties.Resources.Avatar1_Ichika, pbCharIchika.Width, pbCharIchika.Height);
            pbCharFumiko.Image = Images.Resize(Properties.Resources.Avatar2_Fumiko, pbCharFumiko.Width, pbCharFumiko.Height);
            pbCharMiho.Image = Images.Resize(Properties.Resources.Avatar3_Miho, pbCharMiho.Width, pbCharMiho.Height);
            pbCharErika.Image = Images.Resize(Properties.Resources.Avatar4_Erika, pbCharErika.Width, pbCharErika.Height);
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
