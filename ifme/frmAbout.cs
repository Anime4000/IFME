using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Media;

namespace ifme
{
	public partial class frmAbout : Form
	{
		HashSet<string> Pro = new HashSet<string>(new string[] { "Anime4000", "Nemu", "Pis" });
		HashSet<string> Art = new HashSet<string>(new string[] { "53C aka Ray-en", "http://53c.deviantart.com/" });
		HashSet<string> Dev = new HashSet<string>();
		HashSet<string> Sup = new HashSet<string>(File.ReadAllLines("metauser.if"));

		SoundPlayer epic = new SoundPlayer("epic.wav");

		public frmAbout()
		{
			InitializeComponent();
			this.Icon = imouto.Properties.Resources.application_lightning;
		}

		private void frmAbout_Load(object sender, EventArgs e)
		{
			foreach (var item in Plugin.List)
				Dev.Add(item.Profile.Dev);

			lblAppName.Text = Global.App.Name;
			lblAppBuild.Text = String.Format("{0} {1} ({2} x64 '{3}')", Global.App.Type, Global.App.VersionRelease, Global.App.Version, Global.App.CodeName);

			lblTitleA.Text = String.Format(lblTitleA.Text, Global.App.Name);

			lblName1.Text = String.Join("\n", Pro);
			lblName1.Height = 14 * Pro.Count;

			lblName2.Text = String.Join("\n", Art);
			lblName2.Height = 14 * Art.Count;

			lblName3.Text = String.Join("\n", Dev);
			lblName3.Height = 14 * Dev.Count;

			lblName4.Text = String.Join("\n", Sup);
			lblName4.Height = 14 * Sup.Count;

			lblTitleA.Top = 0;
			lblName1.Top = lblTitleA.Bottom;
			lblTitleB.Top = lblName1.Bottom;
			lblName2.Top = lblTitleB.Bottom;
			lblTitleC.Top = lblName2.Bottom;
			lblName3.Top = lblTitleC.Bottom;
			lblTitleD.Top = lblName3.Bottom;
			lblName4.Top = lblTitleD.Bottom;
			lblLast.Top = lblName4.Bottom;
			panelCredit.Height = lblLast.Bottom;

			panelCredit.Top = panelRoll.Height;
			pictBanner.Image = imouto.Properties.Resources.AboutBanner;
		}

		private void frmAbout_Shown(object sender, EventArgs e)
		{
			bgThank.RunWorkerAsync();
		}

		private void frmAbout_FormClosing(object sender, FormClosingEventArgs e)
		{
			epic.Stop();
		}

		private void bgThank_DoWork(object sender, DoWorkEventArgs e)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => panelCredit.Top = panelRoll.Height));
			else
				panelCredit.Top = panelRoll.Height;

			epic.Play();

			while (panelCredit.Bottom != 0)
			{
				if (this.InvokeRequired)
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
