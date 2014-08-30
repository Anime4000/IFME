using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
// Asset
using ProgressDialogs;
using IniParser;
using IniParser.Model;

namespace ifme.hitoha
{
	public partial class frmAbout : Form
	{
		WebClient client = new WebClient();
		ProgressDialog progressDialog = new ProgressDialog();

		string tmp = System.IO.Path.GetTempPath();
		string Title = Globals.AppInfo.Name;
		string Version = Globals.AppInfo.Version;
		string CPU = Globals.AppInfo.CPU;
		string BuildDate = Globals.AppInfo.BuildDate;
		string Info = null;

		string Names = null;
		string NamesFormat = "{0}\n";

		public frmAbout()
		{
			this.Icon = Properties.Resources.aruuie_ifme;

			client.DownloadProgressChanged += client_DownloadProgressChanged;
			client.DownloadFileCompleted += client_DownloadFileCompleted;

			InitializeComponent();
		}

		private void LoadLang()
		{
			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(Language.Path.Folder + "\\" + Language.Default + ".ini");

			lblUpdateInfo.Text = String.Format(lblUpdateInfo.Text, Globals.AppInfo.Name, data[Language.Section.Abt]["Latest"]);

			btnUpdate.Text = data[Language.Section.Abt]["btnUpdate"];
			Info = data[Language.Section.Abt]["Description"];
			//lnkEndUser.Text = data[Language.Section.Abt]["EndUserRight"];
			//lnkLicense.Text = data[Language.Section.Abt]["LicenseInfo"];
			//lnkPrivacy.Text = data[Language.Section.Abt]["PrivacyPolicy"];
			//lblMascot.Text = String.Format(lblMascot.Text, data[Language.Section.Abt]["ByWho"]);

			lblInfo.Text = Info;
		}

		private void frmAbout_Load(object sender, EventArgs e)
		{
			LoadLang();
			lblTitle.Text = String.Format("{0} {1}",Title, Version);
			lblAuthorInfo.Text = String.Format("Compiled on: {0} ({1} build)\nCopyleft (ɔ) 2013 - {2} Anime4000, GNU General Public License v2", BuildDate, CPU, DateTime.Today.Year.ToString());
			this.Text = String.Format(this.Text, "About", Globals.AppInfo.Name);

			if (!Globals.AppInfo.VersionEqual)
			{
				lblUpdateInfo.Visible = false;
				btnUpdate.Visible = true;
				lnkChangeLog.Visible = true;
			}

			// Get Names for Langauge author
			for (int i = 0; i < Language.Installed.Data.GetLength(0); i++)
			{
				if (Language.Installed.Data[i, 0] == null)
					break;

				if (String.Equals(Language.Installed.Data[i, 2], "Anime4000"))
					continue;

				Names += String.Format(NamesFormat, Language.Installed.Data[i, 2]);
			}

			// Get Names for Addons author
			for (int i = 0; i < Addons.Installed.Data.GetLength(0); i++)
			{
				if (Addons.Installed.Data[i, 3] == null)
					break;

				if (String.Equals(Addons.Installed.Data[i, 3], "MulticoreWare"))
					continue;

				if (Addons.Installed.Data[i, 3].Contains("Xiph"))
					continue;

				Names += String.Format(NamesFormat, Addons.Installed.Data[i, 3]);
			}

			lblNames.Text = "People @ MulticoreWare\nPeople @ United Rig Hunter\n" + Names + "Xiph.Org Foundation";

			// Use for to capture height
			lblNames.AutoSize = true;
			int h = lblNames.Height;
			lblNames.AutoSize = false;
			lblNames.Height = h;

			lblNames.Top = panel3.Height;
			tmrScroll.Start();
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			try
			{
				progressDialog.Show();		// Using build-in progress dialog, Windows 8 still have Windows Vista style.
				progressDialog.AutoClose = true;
				progressDialog.Title = "Updating";
				timer.Start();	// Start timer to detect user cancel download updates

				string LATEST = client.DownloadString("http://ifme.sourceforge.net/update/version.txt");
				client.DownloadFileAsync(new Uri("http://master.dl.sourceforge.net/project/ifme/encoder-gui/" + LATEST + "/x265ui.7z"), tmp + "\\ifme\\saishin.jp");
			}
			catch (Exception ex)
			{
				progressDialog.Close();
				MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			progressDialog.Value = e.ProgressPercentage;

			progressDialog.Line1 = String.Format("Downloading {0} KB", Math.Round(Convert.ToDouble(e.TotalBytesToReceive / 1024)));
			progressDialog.Line2 = "From: master.dl.sourceforge.net";
		}

		private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			if (!System.IO.Directory.Exists(tmp + "\\ifme"))
				System.IO.Directory.CreateDirectory(tmp + "\\ifme");

			if (!System.IO.File.Exists(Globals.AppInfo.CurrentFolder + "\\unpack.exe"))
			{
				MessageBox.Show("Error: unpack.exe missing!");
				return;
			}
			else
			{
				System.IO.File.Copy("unpack.exe", tmp + "\\ifme\\7za.exe", true);
			}

			if (System.IO.File.Exists(Globals.AppInfo.CurrentFolder + "\\unins000.exe"))
				System.IO.File.Copy("unins000.exe", tmp + "\\ifme\\unins000.exe", true);

			if (System.IO.File.Exists(Globals.AppInfo.CurrentFolder + "\\unins000.dat"))
				System.IO.File.Copy("unins000.dat", tmp + "\\ifme\\unins000.dat", true);

			foreach (var item in System.IO.Directory.GetDirectories(Globals.AppInfo.CurrentFolder))
			{
				System.IO.Directory.Delete(item, true);
			}

			System.Diagnostics.Process P = new System.Diagnostics.Process();
			P.StartInfo.FileName = "cmd.exe";
			P.StartInfo.Arguments = String.Format("/c title Update in progress! PLEASE WAIT! & TIMEOUT /T 3 /NOBREAK & del /F /S /Q *.* & \"{0}\\ifme\\7za.exe\" x -y -o\"{1}\" \"{0}\\ifme\\saishin.jp\" & copy \"{0}\\ifme\\unins000.exe\" \"{1}\\unins000.exe\" & copy \"{0}\\ifme\\unins000.dat\" \"{1}\\unins000.dat\" & del /F /S /Q \"{0}\\ifme\\*.*\" & TIMEOUT /T 5 /NOBREAK & start \"\" \"{1}\\ifme.exe\"", tmp, Globals.AppInfo.CurrentFolder);
			P.StartInfo.CreateNoWindow = true;
			P.StartInfo.WorkingDirectory = Globals.AppInfo.CurrentFolder;

			P.Start();
			Application.ExitThread();
		}

		private void lnkChangeLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://raw.githubusercontent.com/Anime4000/IFME/master/installer/text_changelog.txt");
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (progressDialog.HasUserCancelled)
			{
				timer.Stop();
				progressDialog.Close();
			}
		}

		private void tmrScroll_Tick(object sender, EventArgs e)
		{
			if (lblNames.Bottom == 0)
				lblNames.Top = panel3.Height;
			else
				lblNames.Top -= 1;
		}

		private void lblWhoChar_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.pixiv.net/member.php?id=6206705");
		}

		private void lblWhoDraw_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://ray-en.deviantart.com/");
		}

		private void panel2_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://ifme.sourceforge.net/");
		}
	}
}
