namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcStorageAreaDetail
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.radPanelTitle = new Telerik.WinControls.UI.RadPanel();
            this.btUpdate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.label14 = new System.Windows.Forms.Label();
            this.txtPriority = new System.Windows.Forms.NumericUpDown();
            this.cbStatus = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ckEnabled = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.lbMaxSize = new System.Windows.Forms.Label();
            this.txtMaxSize = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCurrentSize = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCurrentFileNumber = new System.Windows.Forms.NumericUpDown();
            this.lbMaxFileNumber = new System.Windows.Forms.Label();
            this.txtMaxFileNumber = new System.Windows.Forms.NumericUpDown();
            this.cbSetMaxSize = new System.Windows.Forms.CheckBox();
            this.cbSetMaxFileNumber = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbArchive = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPriority)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentFileNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxFileNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // radPanelTitle
            // 
            this.radPanelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanelTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanelTitle.Location = new System.Drawing.Point(6, 3);
            this.radPanelTitle.Name = "radPanelTitle";
            this.radPanelTitle.Size = new System.Drawing.Size(782, 46);
            this.radPanelTitle.TabIndex = 10;
            this.radPanelTitle.Text = "Storage Area";
            // 
            // btUpdate
            // 
            this.btUpdate.AutoSize = true;
            this.btUpdate.Location = new System.Drawing.Point(572, 67);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(104, 23);
            this.btUpdate.TabIndex = 7;
            this.btUpdate.Text = "Update";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.storageUpdate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(551, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 19);
            this.label5.TabIndex = 41;
            this.label5.Text = "*";
            // 
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(572, 96);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 8;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(110, 263);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(50, 13);
            this.label14.TabIndex = 99;
            this.label14.Text = "Priority:";
            // 
            // txtPriority
            // 
            this.txtPriority.Location = new System.Drawing.Point(166, 256);
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.Size = new System.Drawing.Size(120, 20);
            this.txtPriority.TabIndex = 6;
            // 
            // cbStatus
            // 
            this.cbStatus.DisplayMember = "Description";
            this.cbStatus.FormattingEnabled = true;
            this.cbStatus.Location = new System.Drawing.Point(166, 309);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(375, 21);
            this.cbStatus.TabIndex = 4;
            this.cbStatus.ValueMember = "IdStatus";
            this.cbStatus.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(113, 317);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 84;
            this.label7.Text = "Status:";
            this.label7.Visible = false;
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(166, 229);
            this.txtPath.MaxLength = 255;
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(375, 20);
            this.txtPath.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(123, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 82;
            this.label6.Text = "Path:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(106, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 81;
            this.label1.Text = "Enabled:";
            // 
            // ckEnabled
            // 
            this.ckEnabled.AutoSize = true;
            this.ckEnabled.Checked = true;
            this.ckEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckEnabled.Location = new System.Drawing.Point(169, 100);
            this.ckEnabled.Name = "ckEnabled";
            this.ckEnabled.Size = new System.Drawing.Size(15, 14);
            this.ckEnabled.TabIndex = 1;
            this.ckEnabled.UseVisualStyleBackColor = true;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(169, 73);
            this.txtName.MaxLength = 255;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(375, 20);
            this.txtName.TabIndex = 0;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(120, 80);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 78;
            this.label15.Text = "Name:";
            // 
            // lbMaxSize
            // 
            this.lbMaxSize.AutoSize = true;
            this.lbMaxSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxSize.Location = new System.Drawing.Point(326, 155);
            this.lbMaxSize.Name = "lbMaxSize";
            this.lbMaxSize.Size = new System.Drawing.Size(92, 13);
            this.lbMaxSize.TabIndex = 103;
            this.lbMaxSize.Text = "Max Size (MB):";
            // 
            // txtMaxSize
            // 
            this.txtMaxSize.Location = new System.Drawing.Point(424, 148);
            this.txtMaxSize.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.txtMaxSize.Name = "txtMaxSize";
            this.txtMaxSize.Size = new System.Drawing.Size(120, 20);
            this.txtMaxSize.TabIndex = 2;
            this.txtMaxSize.TabStop = false;
            this.txtMaxSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(53, 155);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 13);
            this.label8.TabIndex = 106;
            this.label8.Text = "Current Size (MB):";
            // 
            // txtCurrentSize
            // 
            this.txtCurrentSize.Enabled = false;
            this.txtCurrentSize.Location = new System.Drawing.Point(169, 148);
            this.txtCurrentSize.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.txtCurrentSize.Name = "txtCurrentSize";
            this.txtCurrentSize.ReadOnly = true;
            this.txtCurrentSize.Size = new System.Drawing.Size(120, 20);
            this.txtCurrentSize.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(39, 210);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(123, 13);
            this.label9.TabIndex = 110;
            this.label9.Text = "Current File Number:";
            // 
            // txtCurrentFileNumber
            // 
            this.txtCurrentFileNumber.Enabled = false;
            this.txtCurrentFileNumber.Location = new System.Drawing.Point(169, 203);
            this.txtCurrentFileNumber.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.txtCurrentFileNumber.Name = "txtCurrentFileNumber";
            this.txtCurrentFileNumber.ReadOnly = true;
            this.txtCurrentFileNumber.Size = new System.Drawing.Size(120, 20);
            this.txtCurrentFileNumber.TabIndex = 108;
            // 
            // lbMaxFileNumber
            // 
            this.lbMaxFileNumber.AutoSize = true;
            this.lbMaxFileNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxFileNumber.Location = new System.Drawing.Point(313, 210);
            this.lbMaxFileNumber.Name = "lbMaxFileNumber";
            this.lbMaxFileNumber.Size = new System.Drawing.Size(105, 13);
            this.lbMaxFileNumber.TabIndex = 109;
            this.lbMaxFileNumber.Text = "Max File Number:";
            // 
            // txtMaxFileNumber
            // 
            this.txtMaxFileNumber.Location = new System.Drawing.Point(424, 203);
            this.txtMaxFileNumber.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.txtMaxFileNumber.Name = "txtMaxFileNumber";
            this.txtMaxFileNumber.Size = new System.Drawing.Size(120, 20);
            this.txtMaxFileNumber.TabIndex = 107;
            this.txtMaxFileNumber.TabStop = false;
            this.txtMaxFileNumber.ThousandsSeparator = true;
            this.txtMaxFileNumber.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // cbSetMaxSize
            // 
            this.cbSetMaxSize.AutoSize = true;
            this.cbSetMaxSize.Location = new System.Drawing.Point(169, 126);
            this.cbSetMaxSize.Name = "cbSetMaxSize";
            this.cbSetMaxSize.Size = new System.Drawing.Size(15, 14);
            this.cbSetMaxSize.TabIndex = 2;
            this.cbSetMaxSize.UseVisualStyleBackColor = true;
            this.cbSetMaxSize.CheckedChanged += new System.EventHandler(this.cbSetMaxSize_CheckedChanged);
            // 
            // cbSetMaxFileNumber
            // 
            this.cbSetMaxFileNumber.AutoSize = true;
            this.cbSetMaxFileNumber.Location = new System.Drawing.Point(169, 181);
            this.cbSetMaxFileNumber.Name = "cbSetMaxFileNumber";
            this.cbSetMaxFileNumber.Size = new System.Drawing.Size(15, 14);
            this.cbSetMaxFileNumber.TabIndex = 3;
            this.cbSetMaxFileNumber.UseVisualStyleBackColor = true;
            this.cbSetMaxFileNumber.CheckedChanged += new System.EventHandler(this.cbSetMaxFileNumber_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(82, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 113;
            this.label2.Text = "Set Max Size";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(39, 182);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(124, 13);
            this.label10.TabIndex = 114;
            this.label10.Text = "Set Max File Number";
            // 
            // cbArchive
            // 
            this.cbArchive.DisplayMember = "Name";
            this.cbArchive.FormattingEnabled = true;
            this.cbArchive.Location = new System.Drawing.Point(166, 282);
            this.cbArchive.Name = "cbArchive";
            this.cbArchive.Size = new System.Drawing.Size(375, 21);
            this.cbArchive.TabIndex = 115;
            this.cbArchive.ValueMember = "IdArchive";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(106, 290);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 116;
            this.label3.Text = "Archive:";
            // 
            // UcStorageAreaDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cbArchive);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbSetMaxFileNumber);
            this.Controls.Add(this.cbSetMaxSize);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtCurrentFileNumber);
            this.Controls.Add(this.lbMaxFileNumber);
            this.Controls.Add(this.txtMaxFileNumber);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtCurrentSize);
            this.Controls.Add(this.lbMaxSize);
            this.Controls.Add(this.txtMaxSize);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtPriority);
            this.Controls.Add(this.cbStatus);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ckEnabled);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.radPanelTitle);
            this.Name = "UcStorageAreaDetail";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPriority)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentFileNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxFileNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Telerik.WinControls.UI.RadPanel radPanelTitle;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown txtPriority;
        private System.Windows.Forms.ComboBox cbStatus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckEnabled;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lbMaxSize;
        private System.Windows.Forms.NumericUpDown txtMaxSize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown txtCurrentSize;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown txtCurrentFileNumber;
        private System.Windows.Forms.Label lbMaxFileNumber;
        private System.Windows.Forms.NumericUpDown txtMaxFileNumber;
        private System.Windows.Forms.CheckBox cbSetMaxSize;
        private System.Windows.Forms.CheckBox cbSetMaxFileNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cbArchive;
        private System.Windows.Forms.Label label3;
    }
}
