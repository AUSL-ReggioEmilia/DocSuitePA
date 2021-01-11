namespace BiblosDs.Document.AdminCentral.UControls
{
    partial class UcLoadFileOnStorage
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn9 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn10 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn11 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn12 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn13 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlArchives = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gwAttributes = new Telerik.WinControls.UI.RadGridView();
            this.btnUpload = new System.Windows.Forms.Button();
            this.radGridStatus = new Telerik.WinControls.UI.RadGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gwAttributes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(355, 75);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(36, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "::";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(76, 78);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(273, 20);
            this.txtPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Directory:";
            // 
            // ddlArchives
            // 
            this.ddlArchives.FormattingEnabled = true;
            this.ddlArchives.Location = new System.Drawing.Point(76, 105);
            this.ddlArchives.Name = "ddlArchives";
            this.ddlArchives.Size = new System.Drawing.Size(273, 21);
            this.ddlArchives.TabIndex = 3;
            this.ddlArchives.SelectedIndexChanged += new System.EventHandler(this.ddlArchives_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Archive:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1047, 55);
            this.panel1.TabIndex = 5;
            // 
            // gwAttributes
            // 
            this.gwAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gwAttributes.BackColor = System.Drawing.SystemColors.Control;
            this.gwAttributes.Cursor = System.Windows.Forms.Cursors.Default;
            this.gwAttributes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.gwAttributes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gwAttributes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gwAttributes.Location = new System.Drawing.Point(76, 132);
            // 
            // 
            // 
            this.gwAttributes.MasterGridViewTemplate.AllowAddNewRow = false;
            this.gwAttributes.MasterGridViewTemplate.AutoGenerateColumns = false;
            gridViewTextBoxColumn1.FieldAlias = "AttributeType";
            gridViewTextBoxColumn1.FieldName = "AttributeType";
            gridViewTextBoxColumn1.HeaderText = "Type";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.ReadOnly = true;
            gridViewTextBoxColumn1.UniqueName = "AttributeType";
            gridViewTextBoxColumn1.Width = 134;
            gridViewTextBoxColumn2.FieldAlias = "Format";
            gridViewTextBoxColumn2.FieldName = "Format";
            gridViewTextBoxColumn2.HeaderText = "Format";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.IsVisible = false;
            gridViewTextBoxColumn2.UniqueName = "Format";
            gridViewTextBoxColumn2.Width = 82;
            gridViewTextBoxColumn3.DataType = typeof(System.Guid);
            gridViewTextBoxColumn3.FieldAlias = "IdAttribute";
            gridViewTextBoxColumn3.FieldName = "IdAttribute";
            gridViewTextBoxColumn3.HeaderText = "IdAttribute";
            gridViewTextBoxColumn3.IsAutoGenerated = true;
            gridViewTextBoxColumn3.IsVisible = false;
            gridViewTextBoxColumn3.UniqueName = "IdAttribute";
            gridViewTextBoxColumn4.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn4.FieldAlias = "IsAutoInc";
            gridViewTextBoxColumn4.FieldName = "IsAutoInc";
            gridViewTextBoxColumn4.HeaderText = "IsAutoInc";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.IsVisible = false;
            gridViewTextBoxColumn4.UniqueName = "IsAutoInc";
            gridViewTextBoxColumn4.Width = 113;
            gridViewTextBoxColumn5.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn5.FieldAlias = "IsEnumerator";
            gridViewTextBoxColumn5.FieldName = "IsEnumerator";
            gridViewTextBoxColumn5.HeaderText = "IsEnumerator";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.IsVisible = false;
            gridViewTextBoxColumn5.UniqueName = "IsEnumerator";
            gridViewTextBoxColumn6.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn6.FieldAlias = "IsMainDate";
            gridViewTextBoxColumn6.FieldName = "IsMainDate";
            gridViewTextBoxColumn6.HeaderText = "IsMainDate";
            gridViewTextBoxColumn6.IsAutoGenerated = true;
            gridViewTextBoxColumn6.IsVisible = false;
            gridViewTextBoxColumn6.UniqueName = "IsMainDate";
            gridViewCheckBoxColumn1.DataType = typeof(bool);
            gridViewCheckBoxColumn1.FieldAlias = "IsRequired";
            gridViewCheckBoxColumn1.FieldName = "IsRequired";
            gridViewCheckBoxColumn1.HeaderText = "Required";
            gridViewCheckBoxColumn1.IsAutoGenerated = true;
            gridViewCheckBoxColumn1.ReadOnly = true;
            gridViewCheckBoxColumn1.UniqueName = "IsRequired";
            gridViewCheckBoxColumn1.Width = 118;
            gridViewTextBoxColumn7.FieldAlias = "Mode";
            gridViewTextBoxColumn7.FieldName = "Mode";
            gridViewTextBoxColumn7.HeaderText = "Mode";
            gridViewTextBoxColumn7.IsAutoGenerated = true;
            gridViewTextBoxColumn7.IsVisible = false;
            gridViewTextBoxColumn7.UniqueName = "Mode";
            gridViewTextBoxColumn8.FieldAlias = "Name";
            gridViewTextBoxColumn8.FieldName = "Name";
            gridViewTextBoxColumn8.HeaderText = "Name";
            gridViewTextBoxColumn8.IsAutoGenerated = true;
            gridViewTextBoxColumn8.ReadOnly = true;
            gridViewTextBoxColumn8.UniqueName = "Name";
            gridViewTextBoxColumn8.Width = 200;
            gridViewTextBoxColumn9.FieldAlias = "Validation";
            gridViewTextBoxColumn9.FieldName = "Validation";
            gridViewTextBoxColumn9.HeaderText = "Validation";
            gridViewTextBoxColumn9.IsAutoGenerated = true;
            gridViewTextBoxColumn9.IsVisible = false;
            gridViewTextBoxColumn9.UniqueName = "Validation";
            gridViewTextBoxColumn10.AllowGroup = false;
            gridViewTextBoxColumn10.AllowSort = false;
            gridViewTextBoxColumn10.FieldAlias = "column1";
            gridViewTextBoxColumn10.HeaderText = "Name to Map";
            gridViewTextBoxColumn10.IsVisible = false;
            gridViewTextBoxColumn10.UniqueName = "colValue";
            gridViewTextBoxColumn10.Width = 153;
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn1);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn2);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn3);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn4);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn5);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn6);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewCheckBoxColumn1);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn7);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn8);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn9);
            this.gwAttributes.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn10);
            this.gwAttributes.MasterGridViewTemplate.EnableGrouping = false;
            this.gwAttributes.Name = "gwAttributes";
            this.gwAttributes.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.gwAttributes.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gwAttributes.ShowGroupPanel = false;
            this.gwAttributes.Size = new System.Drawing.Size(962, 221);
            this.gwAttributes.TabIndex = 8;
            this.gwAttributes.Text = "radGridViewPreview";
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(507, 359);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(108, 23);
            this.btnUpload.TabIndex = 9;
            this.btnUpload.Text = "Process";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // radGridStatus
            // 
            this.radGridStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radGridStatus.BackColor = System.Drawing.SystemColors.Control;
            this.radGridStatus.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.radGridStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radGridStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridStatus.Location = new System.Drawing.Point(76, 388);
            // 
            // 
            // 
            this.radGridStatus.MasterGridViewTemplate.AllowAddNewRow = false;
            this.radGridStatus.MasterGridViewTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewTextBoxColumn11.HeaderText = "Filename";
            gridViewTextBoxColumn11.UniqueName = "column1";
            gridViewTextBoxColumn11.Width = 200;
            gridViewTextBoxColumn12.HeaderText = "Status";
            gridViewTextBoxColumn12.UniqueName = "column2";
            gridViewTextBoxColumn12.Width = 100;
            gridViewTextBoxColumn13.HeaderText = "Details";
            gridViewTextBoxColumn13.UniqueName = "column3";
            gridViewTextBoxColumn13.Width = 500;
            this.radGridStatus.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn11);
            this.radGridStatus.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn12);
            this.radGridStatus.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn13);
            this.radGridStatus.MasterGridViewTemplate.EnableGrouping = false;
            this.radGridStatus.Name = "radGridStatus";
            this.radGridStatus.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridStatus.ReadOnly = true;
            this.radGridStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridStatus.ShowGroupPanel = false;
            this.radGridStatus.Size = new System.Drawing.Size(962, 108);
            this.radGridStatus.TabIndex = 9;
            this.radGridStatus.Text = "radGridViewPreview";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(407, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "File Suffix.:";
            // 
            // txtSuffix
            // 
            this.txtSuffix.Location = new System.Drawing.Point(471, 77);
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new System.Drawing.Size(67, 20);
            this.txtSuffix.TabIndex = 10;
            this.txtSuffix.Text = "_1";
            // 
            // UcLoadFileOnStorage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSuffix);
            this.Controls.Add(this.radGridStatus);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.gwAttributes);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ddlArchives);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnBrowse);
            this.Name = "UcLoadFileOnStorage";
            this.Size = new System.Drawing.Size(1054, 496);
            this.Load += new System.EventHandler(this.UcLoadFileOnStorage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gwAttributes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlArchives;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadGridView gwAttributes;
        private System.Windows.Forms.Button btnUpload;
        private Telerik.WinControls.UI.RadGridView radGridStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSuffix;
    }
}
