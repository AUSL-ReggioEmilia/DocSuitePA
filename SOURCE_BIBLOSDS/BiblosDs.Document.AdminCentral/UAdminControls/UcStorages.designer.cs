namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UCStorages
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn9 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn10 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.Data.SortDescriptor sortDescriptor1 = new Telerik.WinControls.Data.SortDescriptor();
            Telerik.WinControls.Data.SortDescriptor sortDescriptor2 = new Telerik.WinControls.Data.SortDescriptor();
            Telerik.WinControls.Data.SortDescriptor sortDescriptor3 = new Telerik.WinControls.Data.SortDescriptor();
            this.radGridView = new Telerik.WinControls.UI.RadGridView();
            this.documentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btnArchives = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radGridView
            // 
            this.radGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radGridView.BackColor = System.Drawing.SystemColors.Control;
            this.radGridView.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.radGridView.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radGridView.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridView.Location = new System.Drawing.Point(6, 55);
            // 
            // radGridView
            // 
            this.radGridView.MasterTemplate.AllowAddNewRow = false;
            this.radGridView.MasterTemplate.AllowDeleteRow = false;
            this.radGridView.MasterTemplate.AllowEditRow = false;
            this.radGridView.MasterTemplate.AutoGenerateColumns = false;
            this.radGridView.MasterTemplate.Caption = "Storage";
            gridViewTextBoxColumn1.DataType = typeof(System.Guid);
            gridViewTextBoxColumn1.FieldName = "IdStorage";
            gridViewTextBoxColumn1.HeaderText = "IdStorage";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.Name = "IdStorage";
            gridViewTextBoxColumn1.Width = 243;
            gridViewTextBoxColumn2.FieldName = "Name";
            gridViewTextBoxColumn2.HeaderText = "Name";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.Name = "Name";
            gridViewTextBoxColumn2.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewTextBoxColumn2.Width = 185;
            gridViewTextBoxColumn3.FieldName = "StorageType.StorageClassName";
            gridViewTextBoxColumn3.HeaderText = "Type";
            gridViewTextBoxColumn3.IsAutoGenerated = true;
            gridViewTextBoxColumn3.Name = "StorageType.StorageClassName";
            gridViewTextBoxColumn3.Width = 256;
            gridViewTextBoxColumn4.FieldName = "MainPath";
            gridViewTextBoxColumn4.HeaderText = "Path";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.Name = "MainPath";
            gridViewTextBoxColumn4.Width = 252;
            gridViewTextBoxColumn5.FieldName = "StorageRuleAssembly";
            gridViewTextBoxColumn5.HeaderText = "Rule Assembly";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.Name = "StorageRuleAssembly";
            gridViewTextBoxColumn5.Width = 171;
            gridViewTextBoxColumn6.FieldName = "StorageRuleClassName";
            gridViewTextBoxColumn6.HeaderText = "Rule Class Name";
            gridViewTextBoxColumn6.IsAutoGenerated = true;
            gridViewTextBoxColumn6.Name = "StorageRuleClassName";
            gridViewTextBoxColumn6.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewTextBoxColumn6.Width = 143;
            gridViewTextBoxColumn7.DataType = typeof(System.Nullable<int>);
            gridViewTextBoxColumn7.FieldName = "Priority";
            gridViewTextBoxColumn7.HeaderText = "Priority";
            gridViewTextBoxColumn7.IsAutoGenerated = true;
            gridViewTextBoxColumn7.Name = "Priority";
            gridViewCheckBoxColumn1.FieldName = "EnableFulText";
            gridViewCheckBoxColumn1.HeaderText = "Enable Full Text";
            gridViewCheckBoxColumn1.IsAutoGenerated = true;
            gridViewCheckBoxColumn1.Name = "EnableFulText";
            gridViewCheckBoxColumn1.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewCheckBoxColumn1.Width = 90;
            gridViewTextBoxColumn8.FieldName = "AuthenticationKey";
            gridViewTextBoxColumn8.HeaderText = "Authentication Key";
            gridViewTextBoxColumn8.IsAutoGenerated = true;
            gridViewTextBoxColumn8.Name = "AuthenticationKey";
            gridViewTextBoxColumn8.Width = 113;
            gridViewTextBoxColumn9.FieldName = "AuthenticationPassword";
            gridViewTextBoxColumn9.HeaderText = "Authentication Password";
            gridViewTextBoxColumn9.IsAutoGenerated = true;
            gridViewTextBoxColumn9.Name = "AuthenticationPassword";
            gridViewTextBoxColumn9.Width = 148;
            gridViewTextBoxColumn10.DataType = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Server);
            gridViewTextBoxColumn10.FieldName = "Server.ServerName";
            gridViewTextBoxColumn10.HeaderText = "Server";
            gridViewTextBoxColumn10.Name = "Server.ServerName";
            gridViewTextBoxColumn10.Width = 148;
            this.radGridView.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5,
            gridViewTextBoxColumn6,
            gridViewTextBoxColumn7,
            gridViewCheckBoxColumn1,
            gridViewTextBoxColumn8,
            gridViewTextBoxColumn9,
            gridViewTextBoxColumn10});
            this.radGridView.MasterTemplate.DataSource = this.documentBindingSource;
            this.radGridView.MasterTemplate.EnableFiltering = true;
            sortDescriptor1.PropertyName = "Name";
            sortDescriptor2.PropertyName = "StorageRuleClassName";
            sortDescriptor3.PropertyName = "EnableFulText";
            this.radGridView.MasterTemplate.SortDescriptors.AddRange(new Telerik.WinControls.Data.SortDescriptor[] {
            sortDescriptor1,
            sortDescriptor2,
            sortDescriptor3});
            this.radGridView.Name = "radGridView";
            this.radGridView.ReadOnly = true;
            this.radGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridView.ShowGroupPanel = false;
            this.radGridView.Size = new System.Drawing.Size(785, 398);
            this.radGridView.TabIndex = 9;
            this.radGridView.Text = "radGridViewPreview";
            this.radGridView.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.radGridViewStorage_ContextMenuOpening);
            this.radGridView.DataBindingComplete += new Telerik.WinControls.UI.GridViewBindingCompleteEventHandler(this.radGridView_DataBindingComplete);
            // 
            // documentBindingSource
            // 
            this.documentBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Storage);
            // 
            // radPanel1
            // 
            this.radPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanel1.Controls.Add(this.btnArchives);
            this.radPanel1.Controls.Add(this.btAdd);
            this.radPanel1.Location = new System.Drawing.Point(6, 3);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(782, 46);
            this.radPanel1.TabIndex = 10;
            this.radPanel1.Text = "Storages";
            // 
            // btnArchives
            // 
            this.btnArchives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArchives.AutoSize = true;
            this.btnArchives.Location = new System.Drawing.Point(565, 12);
            this.btnArchives.Name = "btnArchives";
            this.btnArchives.Size = new System.Drawing.Size(85, 23);
            this.btnArchives.TabIndex = 13;
            this.btnArchives.TabStop = false;
            this.btnArchives.Text = "Archives";
            this.btnArchives.UseVisualStyleBackColor = true;
            this.btnArchives.Click += new System.EventHandler(this.btnArchives_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.AutoSize = true;
            this.btAdd.Location = new System.Drawing.Point(656, 12);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(113, 23);
            this.btAdd.TabIndex = 0;
            this.btAdd.Text = "Insert new storage";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.itemAddNew_Click);
            // 
            // UCStorages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.radPanel1);
            this.Controls.Add(this.radGridView);
            this.Name = "UCStorages";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btnArchives;
    }
}
