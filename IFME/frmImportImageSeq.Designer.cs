
namespace IFME
{
    partial class frmImportImageSeq
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
            this.lblFrameRate = new System.Windows.Forms.Label();
            this.txtFrameRate = new System.Windows.Forms.TextBox();
            this.lblInformation = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSourceFile = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblSourceFileParse = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.lblBitDepth = new System.Windows.Forms.Label();
            this.txtBitDepth = new System.Windows.Forms.TextBox();
            this.txtPixelFormat = new System.Windows.Forms.TextBox();
            this.lblPixelFormat = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFrameRate
            // 
            this.lblFrameRate.Location = new System.Drawing.Point(212, 27);
            this.lblFrameRate.Name = "lblFrameRate";
            this.lblFrameRate.Size = new System.Drawing.Size(200, 18);
            this.lblFrameRate.TabIndex = 2;
            this.lblFrameRate.Text = "Frame Rate:";
            this.lblFrameRate.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtFrameRate
            // 
            this.txtFrameRate.Location = new System.Drawing.Point(212, 48);
            this.txtFrameRate.Name = "txtFrameRate";
            this.txtFrameRate.Size = new System.Drawing.Size(200, 20);
            this.txtFrameRate.TabIndex = 3;
            this.txtFrameRate.Text = "25";
            // 
            // lblInformation
            // 
            this.lblInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInformation.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblInformation.Location = new System.Drawing.Point(436, 184);
            this.lblInformation.Name = "lblInformation";
            this.lblInformation.Size = new System.Drawing.Size(352, 123);
            this.lblInformation.TabIndex = 3;
            this.lblInformation.Text = "You must set Frame Rate for this image sequence (target/original/actual)\r\n\r\nAccep" +
    "ted value in Interger, Float or Fraction, example:\r\nInteger: 25\r\nFloat: 23.976 \r" +
    "\nFraction: 1/5 (image per 5 seconds)";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblSourceFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source File";
            // 
            // lblSourceFile
            // 
            this.lblSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSourceFile.Location = new System.Drawing.Point(6, 16);
            this.lblSourceFile.Name = "lblSourceFile";
            this.lblSourceFile.Size = new System.Drawing.Size(764, 56);
            this.lblSourceFile.TabIndex = 0;
            this.lblSourceFile.Text = "C:\\Users\\Nemu\\Pictures\\Anime\\Blend S\\image-0001.png";
            this.lblSourceFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblSourceFileParse);
            this.groupBox2.Location = new System.Drawing.Point(12, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 80);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Source File (Parsed)";
            // 
            // lblSourceFileParse
            // 
            this.lblSourceFileParse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSourceFileParse.Location = new System.Drawing.Point(6, 16);
            this.lblSourceFileParse.Name = "lblSourceFileParse";
            this.lblSourceFileParse.Size = new System.Drawing.Size(764, 56);
            this.lblSourceFileParse.TabIndex = 0;
            this.lblSourceFileParse.Text = "C:\\Users\\Nemu\\Pictures\\Anime\\Blend S\\image-%04d.png";
            this.lblSourceFileParse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.lblPixelFormat);
            this.groupBox3.Controls.Add(this.txtPixelFormat);
            this.groupBox3.Controls.Add(this.txtBitDepth);
            this.groupBox3.Controls.Add(this.lblBitDepth);
            this.groupBox3.Controls.Add(this.txtResolution);
            this.groupBox3.Controls.Add(this.lblResolution);
            this.groupBox3.Controls.Add(this.lblFrameRate);
            this.groupBox3.Controls.Add(this.txtFrameRate);
            this.groupBox3.Location = new System.Drawing.Point(12, 184);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(418, 150);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Properties";
            // 
            // lblResolution
            // 
            this.lblResolution.Location = new System.Drawing.Point(6, 27);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(200, 18);
            this.lblResolution.TabIndex = 0;
            this.lblResolution.Text = "Resolution:";
            this.lblResolution.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtResolution
            // 
            this.txtResolution.Location = new System.Drawing.Point(6, 48);
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.ReadOnly = true;
            this.txtResolution.Size = new System.Drawing.Size(200, 20);
            this.txtResolution.TabIndex = 1;
            this.txtResolution.Text = "1920x1080";
            // 
            // lblBitDepth
            // 
            this.lblBitDepth.Location = new System.Drawing.Point(6, 71);
            this.lblBitDepth.Name = "lblBitDepth";
            this.lblBitDepth.Size = new System.Drawing.Size(200, 18);
            this.lblBitDepth.TabIndex = 4;
            this.lblBitDepth.Text = "Bit Depth:";
            this.lblBitDepth.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtBitDepth
            // 
            this.txtBitDepth.Location = new System.Drawing.Point(6, 92);
            this.txtBitDepth.Name = "txtBitDepth";
            this.txtBitDepth.ReadOnly = true;
            this.txtBitDepth.Size = new System.Drawing.Size(200, 20);
            this.txtBitDepth.TabIndex = 5;
            this.txtBitDepth.Text = "8";
            // 
            // txtPixelFormat
            // 
            this.txtPixelFormat.Location = new System.Drawing.Point(212, 92);
            this.txtPixelFormat.Name = "txtPixelFormat";
            this.txtPixelFormat.ReadOnly = true;
            this.txtPixelFormat.Size = new System.Drawing.Size(200, 20);
            this.txtPixelFormat.TabIndex = 7;
            this.txtPixelFormat.Text = "RGB888";
            // 
            // lblPixelFormat
            // 
            this.lblPixelFormat.Location = new System.Drawing.Point(212, 71);
            this.lblPixelFormat.Name = "lblPixelFormat";
            this.lblPixelFormat.Size = new System.Drawing.Size(200, 18);
            this.lblPixelFormat.TabIndex = 6;
            this.lblPixelFormat.Text = "Pixel Format:";
            this.lblPixelFormat.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAdd.Location = new System.Drawing.Point(713, 310);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 24);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnCancel.Location = new System.Drawing.Point(632, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmImportImageSeq
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 346);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblInformation);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmImportImageSeq";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import image sequence";
            this.Load += new System.EventHandler(this.frmImportImageSeq_Load);
            this.Shown += new System.EventHandler(this.frmImportImageSeq_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFrameRate;
        private System.Windows.Forms.TextBox txtFrameRate;
        private System.Windows.Forms.Label lblInformation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblSourceFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblSourceFileParse;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.TextBox txtBitDepth;
        private System.Windows.Forms.Label lblBitDepth;
        private System.Windows.Forms.Label lblPixelFormat;
        private System.Windows.Forms.TextBox txtPixelFormat;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
    }
}