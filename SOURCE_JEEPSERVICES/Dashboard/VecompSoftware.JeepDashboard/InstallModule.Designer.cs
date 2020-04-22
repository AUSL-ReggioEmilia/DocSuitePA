namespace VecompSoftware.JeepDashboard
{
    partial class InstallModule
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
            this.txtReleaseNotes = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtModuleLatestVersion = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ddlModule = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtReleaseNotes
            // 
            this.txtReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReleaseNotes.Location = new System.Drawing.Point(67, 85);
            this.txtReleaseNotes.Multiline = true;
            this.txtReleaseNotes.Name = "txtReleaseNotes";
            this.txtReleaseNotes.ReadOnly = true;
            this.txtReleaseNotes.Size = new System.Drawing.Size(275, 130);
            this.txtReleaseNotes.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Version";
            // 
            // txtModuleLatestVersion
            // 
            this.txtModuleLatestVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModuleLatestVersion.Location = new System.Drawing.Point(67, 59);
            this.txtModuleLatestVersion.Name = "txtModuleLatestVersion";
            this.txtModuleLatestVersion.ReadOnly = true;
            this.txtModuleLatestVersion.Size = new System.Drawing.Size(275, 20);
            this.txtModuleLatestVersion.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Module";
            // 
            // ddlModule
            // 
            this.ddlModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlModule.FormattingEnabled = true;
            this.ddlModule.Location = new System.Drawing.Point(67, 32);
            this.ddlModule.Name = "ddlModule";
            this.ddlModule.Size = new System.Drawing.Size(275, 21);
            this.ddlModule.TabIndex = 19;
            this.ddlModule.SelectedIndexChanged += new System.EventHandler(this.DdlModuleSelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(67, 6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(275, 20);
            this.txtName.TabIndex = 17;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(264, 221);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(78, 23);
            this.cmdCancel.TabIndex = 16;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOk
            // 
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdOk.Enabled = false;
            this.cmdOk.Location = new System.Drawing.Point(180, 221);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(78, 23);
            this.cmdOk.TabIndex = 15;
            this.cmdOk.Text = "Install";
            this.cmdOk.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 32);
            this.label4.TabIndex = 24;
            this.label4.Text = "Release notes";
            // 
            // InstallModule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 251);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtReleaseNotes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtModuleLatestVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ddlModule);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.Name = "InstallModule";
            this.Text = "Install new Module";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtReleaseNotes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtModuleLatestVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ddlModule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Label label4;
    }
}