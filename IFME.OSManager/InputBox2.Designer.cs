namespace IFME.OSManager
{
    partial class InputBox2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputBox2));
            this.btnOK = new System.Windows.Forms.Button();
            this.txtInputBox2 = new System.Windows.Forms.TextBox();
            this.lblMessage2 = new System.Windows.Forms.Label();
            this.txtInputBox1 = new System.Windows.Forms.TextBox();
            this.lblMessage1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(688, 164);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 24);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtInputBox2
            // 
            this.txtInputBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputBox2.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtInputBox2.Location = new System.Drawing.Point(12, 134);
            this.txtInputBox2.Name = "txtInputBox2";
            this.txtInputBox2.Size = new System.Drawing.Size(776, 24);
            this.txtInputBox2.TabIndex = 3;
            this.txtInputBox2.TextChanged += new System.EventHandler(this.txtInputBox2_TextChanged);
            this.txtInputBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInputBox2_KeyDown);
            // 
            // lblMessage2
            // 
            this.lblMessage2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage2.Location = new System.Drawing.Point(12, 85);
            this.lblMessage2.Name = "lblMessage2";
            this.lblMessage2.Size = new System.Drawing.Size(776, 46);
            this.lblMessage2.TabIndex = 2;
            this.lblMessage2.Text = "Message 2";
            this.lblMessage2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtInputBox1
            // 
            this.txtInputBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputBox1.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtInputBox1.Location = new System.Drawing.Point(12, 58);
            this.txtInputBox1.Name = "txtInputBox1";
            this.txtInputBox1.Size = new System.Drawing.Size(776, 24);
            this.txtInputBox1.TabIndex = 1;
            this.txtInputBox1.TextChanged += new System.EventHandler(this.txtInputBox1_TextChanged);
            this.txtInputBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInputBox1_KeyDown);
            // 
            // lblMessage1
            // 
            this.lblMessage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage1.Location = new System.Drawing.Point(12, 9);
            this.lblMessage1.Name = "lblMessage1";
            this.lblMessage1.Size = new System.Drawing.Size(776, 46);
            this.lblMessage1.TabIndex = 0;
            this.lblMessage1.Text = "Message 1";
            this.lblMessage1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // InputBox2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 200);
            this.Controls.Add(this.lblMessage1);
            this.Controls.Add(this.txtInputBox1);
            this.Controls.Add(this.lblMessage2);
            this.Controls.Add(this.txtInputBox2);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InputBox2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Input Box 2";
            this.Load += new System.EventHandler(this.InputBox2_Load);
            this.Shown += new System.EventHandler(this.InputBox2_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtInputBox2;
        private System.Windows.Forms.Label lblMessage2;
        private System.Windows.Forms.TextBox txtInputBox1;
        private System.Windows.Forms.Label lblMessage1;
    }
}