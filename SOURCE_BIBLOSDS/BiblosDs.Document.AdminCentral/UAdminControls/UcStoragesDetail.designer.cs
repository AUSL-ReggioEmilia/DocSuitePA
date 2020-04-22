namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UCStoragesDetail
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
            this.components = new System.ComponentModel.Container();
            this.radPanelTitle = new Telerik.WinControls.UI.RadPanel();
            this.btUpdate = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPriority = new System.Windows.Forms.NumericUpDown();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ddlStorageType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.btCancel = new System.Windows.Forms.Button();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.txtStorageRuleAssembly = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtStorageRuleClassName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbFullText = new System.Windows.Forms.CheckBox();
            this.txtAuthenticationPassword = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAuthenticationKey = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cbVisible = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.documentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.cbServers = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPriority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanelTitle
            // 
            this.radPanelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanelTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanelTitle.Location = new System.Drawing.Point(6, 3);
            this.radPanelTitle.Name = "radPanelTitle";
            // 
            // 
            // 
            this.radPanelTitle.RootElement.AccessibleDescription = null;
            this.radPanelTitle.RootElement.AccessibleName = null;
            this.radPanelTitle.RootElement.ControlBounds = new System.Drawing.Rectangle(6, 3, 200, 100);
            this.radPanelTitle.Size = new System.Drawing.Size(782, 46);
            this.radPanelTitle.TabIndex = 10;
            this.radPanelTitle.Text = "Add new Storage";
            // 
            // btUpdate
            // 
            this.btUpdate.AutoSize = true;
            this.btUpdate.Location = new System.Drawing.Point(572, 63);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(104, 23);
            this.btUpdate.TabIndex = 9;
            this.btUpdate.Text = "Update";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.storageUpdate_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(551, 125);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(15, 19);
            this.label8.TabIndex = 43;
            this.label8.Text = "*";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(551, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 19);
            this.label7.TabIndex = 42;
            this.label7.Text = "*";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(551, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 19);
            this.label5.TabIndex = 41;
            this.label5.Text = "*";
            // 
            // txtPriority
            // 
            this.txtPriority.Location = new System.Drawing.Point(170, 201);
            this.txtPriority.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.Size = new System.Drawing.Size(94, 20);
            this.txtPriority.TabIndex = 5;
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(170, 120);
            this.txtPath.MaxLength = 255;
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(375, 20);
            this.txtPath.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(127, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Path:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(121, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Name:";
            // 
            // ddlStorageType
            // 
            this.ddlStorageType.DisplayMember = "StorageClassName";
            this.ddlStorageType.FormattingEnabled = true;
            this.ddlStorageType.Location = new System.Drawing.Point(170, 65);
            this.ddlStorageType.Name = "ddlStorageType";
            this.ddlStorageType.Size = new System.Drawing.Size(375, 21);
            this.ddlStorageType.TabIndex = 0;
            this.ddlStorageType.ValueMember = "IdStorageType";
            this.ddlStorageType.SelectedIndexChanged += new System.EventHandler(this.ddlStorageType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(114, 203);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Priority:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(81, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "StorageType:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(170, 93);
            this.txtName.MaxLength = 255;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(375, 20);
            this.txtName.TabIndex = 1;
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(572, 92);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 10;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            // 
            // txtStorageRuleAssembly
            // 
            this.txtStorageRuleAssembly.Location = new System.Drawing.Point(170, 147);
            this.txtStorageRuleAssembly.MaxLength = 255;
            this.txtStorageRuleAssembly.Name = "txtStorageRuleAssembly";
            this.txtStorageRuleAssembly.Size = new System.Drawing.Size(375, 20);
            this.txtStorageRuleAssembly.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Storage Rule Assembly:";
            // 
            // txtStorageRuleClassName
            // 
            this.txtStorageRuleClassName.Location = new System.Drawing.Point(170, 174);
            this.txtStorageRuleClassName.MaxLength = 255;
            this.txtStorageRuleClassName.Name = "txtStorageRuleClassName";
            this.txtStorageRuleClassName.Size = new System.Drawing.Size(375, 20);
            this.txtStorageRuleClassName.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(9, 177);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(155, 13);
            this.label9.TabIndex = 47;
            this.label9.Text = "Storage Rule Class Name:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(65, 228);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Enable Full Text";
            // 
            // cbFullText
            // 
            this.cbFullText.AutoSize = true;
            this.cbFullText.Location = new System.Drawing.Point(170, 228);
            this.cbFullText.Name = "cbFullText";
            this.cbFullText.Size = new System.Drawing.Size(15, 14);
            this.cbFullText.TabIndex = 6;
            this.cbFullText.UseVisualStyleBackColor = true;
            // 
            // txtAuthenticationPassword
            // 
            this.txtAuthenticationPassword.Location = new System.Drawing.Point(170, 276);
            this.txtAuthenticationPassword.MaxLength = 250;
            this.txtAuthenticationPassword.Name = "txtAuthenticationPassword";
            this.txtAuthenticationPassword.Size = new System.Drawing.Size(375, 20);
            this.txtAuthenticationPassword.TabIndex = 8;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(13, 276);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(151, 13);
            this.label11.TabIndex = 53;
            this.label11.Text = "Authentication Password:";
            // 
            // txtAuthenticationKey
            // 
            this.txtAuthenticationKey.Location = new System.Drawing.Point(170, 249);
            this.txtAuthenticationKey.MaxLength = 250;
            this.txtAuthenticationKey.Name = "txtAuthenticationKey";
            this.txtAuthenticationKey.Size = new System.Drawing.Size(375, 20);
            this.txtAuthenticationKey.TabIndex = 7;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(46, 252);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(118, 13);
            this.label12.TabIndex = 51;
            this.label12.Text = "Authentication Key:";
            // 
            // cbVisible
            // 
            this.cbVisible.AutoSize = true;
            this.cbVisible.Checked = true;
            this.cbVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVisible.Location = new System.Drawing.Point(170, 302);
            this.cbVisible.Name = "cbVisible";
            this.cbVisible.Size = new System.Drawing.Size(15, 14);
            this.cbVisible.TabIndex = 54;
            this.cbVisible.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(114, 302);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 13);
            this.label13.TabIndex = 55;
            this.label13.Text = "Visible:";
            // 
            // documentBindingSource
            // 
            this.documentBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Storage);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Red;
            this.label14.Location = new System.Drawing.Point(167, 380);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(581, 13);
            this.label14.TabIndex = 74;
            this.label14.Text = "N.B. The path must be a local path or a full specify shared path like \\\\192.168.1" +
    ".1\\c$\\BiblosDS2010";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(114, 325);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 13);
            this.label15.TabIndex = 76;
            this.label15.Text = "Server:";
            // 
            // cbServers
            // 
            this.cbServers.DisplayMember = "ServerName";
            this.cbServers.FormattingEnabled = true;
            this.cbServers.Location = new System.Drawing.Point(170, 325);
            this.cbServers.Name = "cbServers";
            this.cbServers.Size = new System.Drawing.Size(375, 21);
            this.cbServers.TabIndex = 77;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Red;
            this.label17.Location = new System.Drawing.Point(167, 406);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(317, 13);
            this.label17.TabIndex = 79;
            this.label17.Text = "N.B. Set SQL Server local path for FileGroup definition";
            this.label17.Visible = false;
            // 
            // UCStoragesDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label17);
            this.Controls.Add(this.cbServers);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.cbVisible);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtAuthenticationPassword);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtAuthenticationKey);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.cbFullText);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtStorageRuleClassName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtStorageRuleAssembly);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPriority);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ddlStorageType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.radPanelTitle);
            this.Name = "UCStoragesDetail";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPriority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanelTitle;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown txtPriority;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddlStorageType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Button btCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.TextBox txtStorageRuleAssembly;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtStorageRuleClassName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbFullText;
        private System.Windows.Forms.TextBox txtAuthenticationPassword;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtAuthenticationKey;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbVisible;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cbServers;
        private System.Windows.Forms.Label label17;
    }
}
