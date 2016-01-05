using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Media;

namespace ifme
{
    public partial class frmAbout : Form
	{
		HashSet<string> Pro = new HashSet<string>(new string[] { "Anime4000", "Nemu", "Pis", "SailorOnDaTea", "Bvzcoder" });
		HashSet<string> Art = new HashSet<string>(new string[] { "53C", "http://53c.deviantart.com/", "Adeq", "https://www.facebook.com/liyana.0426" });
		HashSet<string> Dev = new HashSet<string>();
		HashSet<string> Lng = new HashSet<string>();
		HashSet<string> Sup = new HashSet<string>(File.ReadAllLines("metauser.if"));

		public frmAbout()
		{
			InitializeComponent();

			Icon = Properties.Resources.application_lightning;
			BackgroundImage = Properties.Resources.AboutBanner;
		}

		private void frmAbout_Load(object sender, EventArgs e)
		{
			foreach (var item in Plugin.List)
				Dev.Add(item.Value.Profile.Dev);

			foreach (var item in Language.Lists)
			{
				if (string.Equals(item.Author, "Anime4000"))
					continue;

				Lng.Add($"{item.Name} by {item.Author}");
			}
			
			lblAppName.Text = Global.App.Name;
            lblAppBuild.Text = $"{Global.App.ReleaseType} {Global.App.VersionRelease} ({Global.App.Version} x64 '{Global.App.CodeName}')";

            lblTitleA.Text = string.Format(lblTitleA.Text, Global.App.Name);

			lblName1.Text = string.Join("\n", Pro);
			lblName1.Height = 14 * Pro.Count;

			lblName2.Text = string.Join("\n", Art);
			lblName2.Height = 14 * Art.Count;

			lblName3.Text = string.Join("\n", Dev);
			lblName3.Height = 14 * Dev.Count;

			lblName4.Text = string.Join("\n", Lng);
			lblName4.Height = 14 * Lng.Count;

			lblName5.Text = string.Join("\n", Sup);
			lblName5.Height = 14 * Sup.Count;

			lblTitleA.Top = 0;
			lblName1.Top = lblTitleA.Bottom;
			lblTitleB.Top = lblName1.Bottom;
			lblName2.Top = lblTitleB.Bottom;
			lblTitleC.Top = lblName2.Bottom;
			lblName3.Top = lblTitleC.Bottom;
			lblTitleD.Top = lblName3.Bottom;
			lblName4.Top = lblTitleD.Bottom;
			lblTitleE.Top = lblName4.Bottom;
			lblName5.Top = lblTitleE.Bottom;
			lblLast.Top = lblName5.Bottom;
			panelCredit.Height = lblLast.Bottom;

			panelCredit.Top = panelRoll.Height;
		}

		private void frmAbout_Shown(object sender, EventArgs e)
		{
			bgThank.RunWorkerAsync();
		}

		private void frmAbout_FormClosing(object sender, FormClosingEventArgs e)
		{
			bgThank.Dispose();
		}

		private void bgThank_DoWork(object sender, DoWorkEventArgs e)
		{
			

			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() => panelCredit.Top = panelRoll.Height));
			else
				panelCredit.Top = panelRoll.Height;

			while (panelCredit.Bottom != 0)
			{
				if (InvokeRequired)
					BeginInvoke(new MethodInvoker(() => panelCredit.Top -= 1));
				else
					panelCredit.Top -= 1;

				Thread.Sleep(35);
			}
		}

		private void bgThank_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			bgThank.RunWorkerAsync();
		}
	}
}
