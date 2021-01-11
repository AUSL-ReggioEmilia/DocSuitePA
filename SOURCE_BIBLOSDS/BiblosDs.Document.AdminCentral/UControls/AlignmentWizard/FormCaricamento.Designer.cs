namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    partial class FormCaricamento
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
            this.radPnlRoot = new Telerik.WinControls.UI.RadPanel();
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.radLstMsg = new Telerik.WinControls.UI.RadListControl();
            this.radBtnAnnulla = new Telerik.WinControls.UI.RadButton();
            this.radPb = new Telerik.WinControls.UI.RadProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.radPnlRoot)).BeginInit();
            this.radPnlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLstMsg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnAnnulla)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPb)).BeginInit();
            this.SuspendLayout();
            // 
            // radPnlRoot
            // 
            this.radPnlRoot.Controls.Add(this.radGroupBox1);
            this.radPnlRoot.Controls.Add(this.radBtnAnnulla);
            this.radPnlRoot.Controls.Add(this.radPb);
            this.radPnlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPnlRoot.Location = new System.Drawing.Point(0, 0);
            this.radPnlRoot.Name = "radPnlRoot";
            this.radPnlRoot.Size = new System.Drawing.Size(314, 212);
            this.radPnlRoot.TabIndex = 0;
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Controls.Add(this.radLstMsg);
            this.radGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGroupBox1.FooterImageIndex = -1;
            this.radGroupBox1.FooterImageKey = "";
            this.radGroupBox1.HeaderImageIndex = -1;
            this.radGroupBox1.HeaderImageKey = "";
            this.radGroupBox1.HeaderMargin = new System.Windows.Forms.Padding(0);
            this.radGroupBox1.HeaderText = "Messaggi";
            this.radGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.radGroupBox1.Name = "radGroupBox1";
            this.radGroupBox1.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            // 
            // 
            // 
            this.radGroupBox1.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.radGroupBox1.Size = new System.Drawing.Size(314, 165);
            this.radGroupBox1.TabIndex = 6;
            this.radGroupBox1.Text = "Messaggi";
            // 
            // radLstMsg
            // 
            this.radLstMsg.CaseSensitiveSort = true;
            this.radLstMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radLstMsg.ItemHeight = 18;
            this.radLstMsg.Location = new System.Drawing.Point(2, 18);
            this.radLstMsg.Name = "radLstMsg";
            this.radLstMsg.Size = new System.Drawing.Size(310, 145);
            this.radLstMsg.TabIndex = 0;
            this.radLstMsg.Text = "Lista messaggi";
            // 
            // radBtnAnnulla
            // 
            this.radBtnAnnulla.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radBtnAnnulla.Location = new System.Drawing.Point(0, 165);
            this.radBtnAnnulla.Name = "radBtnAnnulla";
            this.radBtnAnnulla.Size = new System.Drawing.Size(314, 24);
            this.radBtnAnnulla.TabIndex = 5;
            this.radBtnAnnulla.Text = "&Interrompi operazione";
            this.radBtnAnnulla.Click += new System.EventHandler(this.radBtnAnnulla_Click);
            // 
            // radPb
            // 
            this.radPb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.radPb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radPb.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPb.ImageIndex = -1;
            this.radPb.ImageKey = "";
            this.radPb.ImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.radPb.Location = new System.Drawing.Point(0, 189);
            this.radPb.Name = "radPb";
            this.radPb.SeparatorColor1 = System.Drawing.Color.White;
            this.radPb.SeparatorColor2 = System.Drawing.Color.White;
            this.radPb.SeparatorColor3 = System.Drawing.Color.White;
            this.radPb.SeparatorColor4 = System.Drawing.Color.White;
            this.radPb.Size = new System.Drawing.Size(314, 23);
            this.radPb.Step = 1;
            this.radPb.TabIndex = 4;
            // 
            // FormCaricamento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 212);
            this.Controls.Add(this.radPnlRoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCaricamento";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormCaricamento";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.radPnlRoot)).EndInit();
            this.radPnlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radLstMsg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnAnnulla)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPnlRoot;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadButton radBtnAnnulla;
        private Telerik.WinControls.UI.RadProgressBar radPb;
        private Telerik.WinControls.UI.RadListControl radLstMsg;

    }
}