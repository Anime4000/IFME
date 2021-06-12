
namespace IFME
{
    partial class frmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCopyRight = new System.Windows.Forms.Label();
            this.lblArtWork = new System.Windows.Forms.Label();
            this.lblDevs = new System.Windows.Forms.Label();
            this.lblCodeName = new System.Windows.Forms.Label();
            this.lnkGithub = new System.Windows.Forms.LinkLabel();
            this.lnkSourceForge = new System.Windows.Forms.LinkLabel();
            this.lblHome = new System.Windows.Forms.LinkLabel();
            this.lnkFacebook = new System.Windows.Forms.LinkLabel();
            this.lnkHitoha = new System.Windows.Forms.LinkLabel();
            this.lnkSoraIro = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 64);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(600, 64);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Internet Friendly Media Encoder";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(0, 152);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(600, 50);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Version pre-8";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(250, 564);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::IFME.Properties.Resources.Banner_2b;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 64);
            this.panel1.TabIndex = 0;
            // 
            // lblCopyRight
            // 
            this.lblCopyRight.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCopyRight.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblCopyRight.Location = new System.Drawing.Point(0, 202);
            this.lblCopyRight.Name = "lblCopyRight";
            this.lblCopyRight.Size = new System.Drawing.Size(600, 48);
            this.lblCopyRight.TabIndex = 4;
            this.lblCopyRight.Text = "(c)";
            this.lblCopyRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblArtWork
            // 
            this.lblArtWork.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblArtWork.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblArtWork.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArtWork.Location = new System.Drawing.Point(0, 250);
            this.lblArtWork.Name = "lblArtWork";
            this.lblArtWork.Size = new System.Drawing.Size(600, 80);
            this.lblArtWork.TabIndex = 5;
            this.lblArtWork.Text = "Artwork by";
            this.lblArtWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblArtWork.Click += new System.EventHandler(this.lblArtWork_Click);
            // 
            // lblDevs
            // 
            this.lblDevs.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDevs.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblDevs.Location = new System.Drawing.Point(0, 330);
            this.lblDevs.Name = "lblDevs";
            this.lblDevs.Size = new System.Drawing.Size(600, 100);
            this.lblDevs.TabIndex = 6;
            this.lblDevs.Text = "Devs";
            this.lblDevs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCodeName
            // 
            this.lblCodeName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCodeName.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblCodeName.Location = new System.Drawing.Point(0, 128);
            this.lblCodeName.Name = "lblCodeName";
            this.lblCodeName.Size = new System.Drawing.Size(600, 24);
            this.lblCodeName.TabIndex = 7;
            this.lblCodeName.Text = "Imagine Chip";
            this.lblCodeName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkGithub
            // 
            this.lnkGithub.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lnkGithub.Location = new System.Drawing.Point(197, 452);
            this.lnkGithub.Name = "lnkGithub";
            this.lnkGithub.Size = new System.Drawing.Size(100, 30);
            this.lnkGithub.TabIndex = 8;
            this.lnkGithub.TabStop = true;
            this.lnkGithub.Text = "GitHub";
            this.lnkGithub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGithub_LinkClicked);
            // 
            // lnkSourceForge
            // 
            this.lnkSourceForge.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lnkSourceForge.Location = new System.Drawing.Point(303, 452);
            this.lnkSourceForge.Name = "lnkSourceForge";
            this.lnkSourceForge.Size = new System.Drawing.Size(100, 30);
            this.lnkSourceForge.TabIndex = 9;
            this.lnkSourceForge.TabStop = true;
            this.lnkSourceForge.Text = "SourceForge";
            this.lnkSourceForge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkSourceForge.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSourceForge_LinkClicked);
            // 
            // lblHome
            // 
            this.lblHome.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHome.Location = new System.Drawing.Point(91, 452);
            this.lblHome.Name = "lblHome";
            this.lblHome.Size = new System.Drawing.Size(100, 30);
            this.lblHome.TabIndex = 10;
            this.lblHome.TabStop = true;
            this.lblHome.Text = "Homepage";
            this.lblHome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHome_LinkClicked);
            // 
            // lnkFacebook
            // 
            this.lnkFacebook.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lnkFacebook.Location = new System.Drawing.Point(409, 452);
            this.lnkFacebook.Name = "lnkFacebook";
            this.lnkFacebook.Size = new System.Drawing.Size(100, 30);
            this.lnkFacebook.TabIndex = 11;
            this.lnkFacebook.TabStop = true;
            this.lnkFacebook.Text = "Facebook";
            this.lnkFacebook.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkFacebook.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkFacebook_LinkClicked);
            // 
            // lnkHitoha
            // 
            this.lnkHitoha.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lnkHitoha.Location = new System.Drawing.Point(197, 482);
            this.lnkHitoha.Name = "lnkHitoha";
            this.lnkHitoha.Size = new System.Drawing.Size(100, 30);
            this.lnkHitoha.TabIndex = 12;
            this.lnkHitoha.TabStop = true;
            this.lnkHitoha.Text = "Hitoha が";
            this.lnkHitoha.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkHitoha.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHitoha_LinkClicked);
            // 
            // lnkSoraIro
            // 
            this.lnkSoraIro.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lnkSoraIro.Location = new System.Drawing.Point(303, 482);
            this.lnkSoraIro.Name = "lnkSoraIro";
            this.lnkSoraIro.Size = new System.Drawing.Size(100, 30);
            this.lnkSoraIro.TabIndex = 13;
            this.lnkSoraIro.TabStop = true;
            this.lnkSoraIro.Text = "Sora Iro";
            this.lnkSoraIro.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkSoraIro.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSoraIro_LinkClicked);
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(600, 600);
            this.Controls.Add(this.lnkSoraIro);
            this.Controls.Add(this.lnkHitoha);
            this.Controls.Add(this.lnkFacebook);
            this.Controls.Add(this.lblHome);
            this.Controls.Add(this.lnkSourceForge);
            this.Controls.Add(this.lnkGithub);
            this.Controls.Add(this.lblDevs);
            this.Controls.Add(this.lblArtWork);
            this.Controls.Add(this.lblCopyRight);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblCodeName);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblCopyRight;
        private System.Windows.Forms.Label lblArtWork;
        private System.Windows.Forms.Label lblDevs;
        private System.Windows.Forms.Label lblCodeName;
        private System.Windows.Forms.LinkLabel lnkGithub;
        private System.Windows.Forms.LinkLabel lnkSourceForge;
        private System.Windows.Forms.LinkLabel lblHome;
        private System.Windows.Forms.LinkLabel lnkFacebook;
        private System.Windows.Forms.LinkLabel lnkHitoha;
        private System.Windows.Forms.LinkLabel lnkSoraIro;
    }
}