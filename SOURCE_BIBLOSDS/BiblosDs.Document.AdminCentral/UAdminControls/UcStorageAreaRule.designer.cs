namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcStorageAreaRule
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
            Telerik.WinControls.UI.GridViewMaskBoxColumn gridViewMaskBoxColumn1 = new Telerik.WinControls.UI.GridViewMaskBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridSortField gridSortField1 = new Telerik.WinControls.UI.GridSortField();
            this.documentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btBack = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.radGridView = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).BeginInit();
            this.SuspendLayout();
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
            this.radPanel1.Controls.Add(this.btBack);
            this.radPanel1.Controls.Add(this.btAdd);
            this.radPanel1.Location = new System.Drawing.Point(6, 3);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(782, 46);
            this.radPanel1.TabIndex = 10;
            this.radPanel1.Text = "Storage Area Rules";
            // 
            // btBack
            // 
            this.btBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBack.AutoSize = true;
            this.btBack.Location = new System.Drawing.Point(534, 12);
            this.btBack.Name = "btBack";
            this.btBack.Size = new System.Drawing.Size(116, 23);
            this.btBack.TabIndex = 12;
            this.btBack.Text = "Back";
            this.btBack.UseVisualStyleBackColor = true;
            this.btBack.Click += new System.EventHandler(this.btBack_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.AutoSize = true;
            this.btAdd.Location = new System.Drawing.Point(656, 12);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(113, 23);
            this.btAdd.TabIndex = 0;
            this.btAdd.Text = "New";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.itemAddNew_Click);
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
            // 
            // 
            this.radGridView.MasterGridViewTemplate.AllowAddNewRow = false;
            this.radGridView.MasterGridViewTemplate.AutoGenerateColumns = false;
            this.radGridView.MasterGridViewTemplate.Caption = "DocumentAttribute";
            gridViewTextBoxColumn1.FieldAlias = "StorageArea.IdStorageArea";
            gridViewTextBoxColumn1.FieldName = "StorageArea.IdStorageArea";
            gridViewTextBoxColumn1.HeaderText = "IdStorageArea";
            gridViewTextBoxColumn1.IsVisible = false;
            gridViewTextBoxColumn1.UniqueName = "StorageArea.IdStorageArea";
            gridViewTextBoxColumn2.FieldAlias = "Attribute.IdAttribute";
            gridViewTextBoxColumn2.FieldName = "Attribute.IdAttribute";
            gridViewTextBoxColumn2.HeaderText = "IdAttribute";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.IsVisible = false;
            gridViewTextBoxColumn2.UniqueName = "Attribute.IdAttribute";
            gridViewTextBoxColumn3.FieldAlias = "Attribute.Archive.IdArchive";
            gridViewTextBoxColumn3.FieldName = "Attribute.Archive.IdArchive";
            gridViewTextBoxColumn3.HeaderText = "IdArchive";
            gridViewTextBoxColumn3.IsVisible = false;
            gridViewTextBoxColumn3.UniqueName = "Attribute.Archive.IdArchive";
            gridViewTextBoxColumn4.FieldAlias = "Attribute.Name";
            gridViewTextBoxColumn4.FieldName = "Attribute.Name";
            gridViewTextBoxColumn4.HeaderText = "Attribute";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.UniqueName = "Attribute.Name";
            gridViewTextBoxColumn4.Width = 138;
            gridViewTextBoxColumn5.FieldAlias = "Attribute.Archive.Name";
            gridViewTextBoxColumn5.FieldName = "Attribute.Archive.Name";
            gridViewTextBoxColumn5.HeaderText = "Archive";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewTextBoxColumn5.UniqueName = "Attribute.Archive.Name";
            gridViewTextBoxColumn5.Width = 171;
            gridViewMaskBoxColumn1.FieldAlias = "RuleOrder";
            gridViewMaskBoxColumn1.FieldName = "RuleOrder";
            gridViewMaskBoxColumn1.HeaderText = "Order";
            gridViewMaskBoxColumn1.MaskType = Telerik.WinControls.UI.MaskType.Numeric;
            gridViewMaskBoxColumn1.ReadOnly = true;
            gridViewMaskBoxColumn1.UniqueName = "Rule_Order";
            gridViewTextBoxColumn6.FieldAlias = "RuleFormat";
            gridViewTextBoxColumn6.FieldName = "RuleFormat";
            gridViewTextBoxColumn6.HeaderText = "Format";
            gridViewTextBoxColumn6.IsAutoGenerated = true;
            gridViewTextBoxColumn6.UniqueName = "RuleFormat";
            gridViewTextBoxColumn6.Width = 116;
            gridViewTextBoxColumn7.FieldAlias = "RuleFilter";
            gridViewTextBoxColumn7.FieldName = "RuleFilter";
            gridViewTextBoxColumn7.HeaderText = "Filter";
            gridViewTextBoxColumn7.IsAutoGenerated = true;
            gridViewTextBoxColumn7.UniqueName = "RuleFilter";
            gridViewTextBoxColumn7.Width = 120;
            gridViewTextBoxColumn8.FieldAlias = "RuleOperator.Descrizione";
            gridViewTextBoxColumn8.FieldName = "RuleOperator.Descrizione";
            gridViewTextBoxColumn8.HeaderText = "Operator";
            gridViewTextBoxColumn8.UniqueName = "RuleOperator.Descrizione";
            gridViewTextBoxColumn8.Width = 112;
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn1);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn2);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn3);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn4);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn5);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewMaskBoxColumn1);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn6);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn7);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn8);
            this.radGridView.MasterGridViewTemplate.EnableFiltering = true;
            gridSortField1.FieldAlias = "Attribute.Archive.Name";
            gridSortField1.FieldName = "Attribute.Archive.Name";
            gridSortField1.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            this.radGridView.MasterGridViewTemplate.SortExpressions.Add(gridSortField1);
            this.radGridView.Name = "radGridView";
            this.radGridView.ReadOnly = true;
            this.radGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridView.ShowGroupPanel = false;
            this.radGridView.Size = new System.Drawing.Size(782, 404);
            this.radGridView.TabIndex = 12;
            this.radGridView.Text = "radGridViewPreview";
            this.radGridView.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.radGridView_ContextMenuOpening);
            // 
            // UcStorageAreaRule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.radGridView);
            this.Controls.Add(this.radPanel1);
            this.Name = "UcStorageAreaRule";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.Button btAdd;
        private Telerik.WinControls.UI.RadGridView radGridView;
        private System.Windows.Forms.Button btBack;
    }
}
