namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcServerDetail
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlRoot = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRole = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btUpdate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.radPanelTitle = new Telerik.WinControls.UI.RadPanel();
            this.pnlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlRoot
            // 
            this.pnlRoot.BackColor = System.Drawing.Color.White;
            this.pnlRoot.Controls.Add(this.label2);
            this.pnlRoot.Controls.Add(this.cbRole);
            this.pnlRoot.Controls.Add(this.label1);
            this.pnlRoot.Controls.Add(this.btCancel);
            this.pnlRoot.Controls.Add(this.btUpdate);
            this.pnlRoot.Controls.Add(this.label5);
            this.pnlRoot.Controls.Add(this.label4);
            this.pnlRoot.Controls.Add(this.txtName);
            this.pnlRoot.Controls.Add(this.radPanelTitle);
            this.pnlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRoot.Location = new System.Drawing.Point(0, 0);
            this.pnlRoot.Name = "pnlRoot";
            this.pnlRoot.Size = new System.Drawing.Size(640, 480);
            this.pnlRoot.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(273, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 19);
            this.label2.TabIndex = 49;
            this.label2.Text = "*";
            // 
            // cbRole
            // 
            this.cbRole.FormattingEnabled = true;
            this.cbRole.Location = new System.Drawing.Point(60, 83);
            this.cbRole.Name = "cbRole";
            this.cbRole.Size = new System.Drawing.Size(207, 21);
            this.cbRole.TabIndex = 48;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "Role:";
            // 
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(533, 81);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 44;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.BackToSenderControl);
            // 
            // btUpdate
            // 
            this.btUpdate.AutoSize = true;
            this.btUpdate.Location = new System.Drawing.Point(533, 52);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(104, 23);
            this.btUpdate.TabIndex = 43;
            this.btUpdate.Text = "Update";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(441, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 19);
            this.label5.TabIndex = 46;
            this.label5.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(60, 55);
            this.txtName.MaxLength = 255;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(375, 20);
            this.txtName.TabIndex = 42;
            // 
            // radPanelTitle
            // 
            this.radPanelTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.radPanelTitle.Location = new System.Drawing.Point(0, 0);
            this.radPanelTitle.Name = "radPanelTitle";
            this.radPanelTitle.Size = new System.Drawing.Size(640, 46);
            this.radPanelTitle.TabIndex = 11;
            this.radPanelTitle.Text = "Server";
            // 
            // UcServerDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlRoot);
            this.Name = "UcServerDetail";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.UcServerDetail_Load);
            this.pnlRoot.ResumeLayout(false);
            this.pnlRoot.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlRoot;
        private Telerik.WinControls.UI.RadPanel radPanelTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbRole;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtName;
    }
}
