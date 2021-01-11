namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcAttribute
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
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn9 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn10 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn11 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn12 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn13 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn14 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn15 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
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
            this.radPanel1.Text = "Attributes";
            // 
            // btBack
            // 
            this.btBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBack.AutoSize = true;
            this.btBack.Location = new System.Drawing.Point(552, 12);
            this.btBack.Name = "btBack";
            this.btBack.Size = new System.Drawing.Size(104, 23);
            this.btBack.TabIndex = 12;
            this.btBack.Text = "Back";
            this.btBack.UseVisualStyleBackColor = true;
            this.btBack.Click += new System.EventHandler(this.btBack_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.AutoSize = true;
            this.btAdd.Location = new System.Drawing.Point(662, 12);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(107, 23);
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
            gridViewTextBoxColumn1.FieldAlias = "Name";
            gridViewTextBoxColumn1.FieldName = "Name";
            gridViewTextBoxColumn1.HeaderText = "Name";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.UniqueName = "Name";
            gridViewTextBoxColumn1.Width = 112;
            gridViewTextBoxColumn2.DataType = typeof(System.Guid);
            gridViewTextBoxColumn2.FieldAlias = "IdAttribute";
            gridViewTextBoxColumn2.FieldName = "IdAttribute";
            gridViewTextBoxColumn2.HeaderText = "IdAttribute";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.IsVisible = false;
            gridViewTextBoxColumn2.UniqueName = "IdAttribute";
            gridViewTextBoxColumn3.DataType = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Archive);
            gridViewTextBoxColumn3.FieldAlias = "Archive";
            gridViewTextBoxColumn3.FieldName = "Archive";
            gridViewTextBoxColumn3.HeaderText = "Archive";
            gridViewTextBoxColumn3.IsAutoGenerated = true;
            gridViewTextBoxColumn3.IsVisible = false;
            gridViewTextBoxColumn3.UniqueName = "Archive";
            gridViewCheckBoxColumn1.DataType = typeof(bool);
            gridViewCheckBoxColumn1.FieldAlias = "IsRequired";
            gridViewCheckBoxColumn1.FieldName = "IsRequired";
            gridViewCheckBoxColumn1.HeaderText = "Required";
            gridViewCheckBoxColumn1.IsAutoGenerated = true;
            gridViewCheckBoxColumn1.UniqueName = "IsRequired";
            gridViewTextBoxColumn4.DataType = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.AttributeMode);
            gridViewTextBoxColumn4.FieldAlias = "Mode";
            gridViewTextBoxColumn4.FieldName = "Mode";
            gridViewTextBoxColumn4.HeaderText = "Mode";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.IsVisible = false;
            gridViewTextBoxColumn4.ReadOnly = true;
            gridViewTextBoxColumn4.UniqueName = "Mode";
            gridViewTextBoxColumn4.Width = 95;
            gridViewTextBoxColumn5.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn5.FieldAlias = "IsMainDate";
            gridViewTextBoxColumn5.FieldName = "IsMainDate";
            gridViewTextBoxColumn5.HeaderText = "Main Date";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.UniqueName = "IsMainDate";
            gridViewTextBoxColumn5.Width = 67;
            gridViewTextBoxColumn6.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn6.FieldAlias = "IsEnumerator";
            gridViewTextBoxColumn6.FieldName = "IsEnumerator";
            gridViewTextBoxColumn6.HeaderText = "Enumerator";
            gridViewTextBoxColumn6.IsAutoGenerated = true;
            gridViewTextBoxColumn6.UniqueName = "IsEnumerator";
            gridViewTextBoxColumn6.Width = 76;
            gridViewTextBoxColumn7.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn7.FieldAlias = "IsAutoInc";
            gridViewTextBoxColumn7.FieldName = "IsAutoInc";
            gridViewTextBoxColumn7.HeaderText = "Auto Inc";
            gridViewTextBoxColumn7.IsAutoGenerated = true;
            gridViewTextBoxColumn7.UniqueName = "IsAutoInc";
            gridViewTextBoxColumn7.Width = 57;
            gridViewTextBoxColumn8.FieldAlias = "AttributeType";
            gridViewTextBoxColumn8.FieldName = "AttributeType";
            gridViewTextBoxColumn8.HeaderText = "Attribute Type";
            gridViewTextBoxColumn8.IsAutoGenerated = true;
            gridViewTextBoxColumn8.ReadOnly = true;
            gridViewTextBoxColumn8.UniqueName = "AttributeType";
            gridViewTextBoxColumn8.Width = 91;
            gridViewTextBoxColumn9.DataType = typeof(System.Nullable<short>);
            gridViewTextBoxColumn9.FieldAlias = "KeyOrder";
            gridViewTextBoxColumn9.FieldName = "KeyOrder";
            gridViewTextBoxColumn9.HeaderText = "Key Order";
            gridViewTextBoxColumn9.IsAutoGenerated = true;
            gridViewTextBoxColumn9.UniqueName = "KeyOrder";
            gridViewTextBoxColumn9.Width = 137;
            gridViewTextBoxColumn10.DataType = typeof(System.Nullable<short>);
            gridViewTextBoxColumn10.FieldAlias = "ConservationPosition";
            gridViewTextBoxColumn10.FieldName = "ConservationPosition";
            gridViewTextBoxColumn10.HeaderText = "ConservationPosition";
            gridViewTextBoxColumn10.IsAutoGenerated = true;
            gridViewTextBoxColumn10.IsVisible = false;
            gridViewTextBoxColumn10.UniqueName = "ConservationPosition";
            gridViewTextBoxColumn11.FieldAlias = "KeyFilter";
            gridViewTextBoxColumn11.FieldName = "KeyFilter";
            gridViewTextBoxColumn11.HeaderText = "Key Filter";
            gridViewTextBoxColumn11.IsAutoGenerated = true;
            gridViewTextBoxColumn11.UniqueName = "KeyFilter";
            gridViewTextBoxColumn12.FieldAlias = "KeyFormat";
            gridViewTextBoxColumn12.FieldName = "KeyFormat";
            gridViewTextBoxColumn12.HeaderText = "Key Format";
            gridViewTextBoxColumn12.IsAutoGenerated = true;
            gridViewTextBoxColumn12.UniqueName = "KeyFormat";
            gridViewTextBoxColumn13.FieldAlias = "Validation";
            gridViewTextBoxColumn13.FieldName = "Validation";
            gridViewTextBoxColumn13.HeaderText = "Validation";
            gridViewTextBoxColumn13.IsAutoGenerated = true;
            gridViewTextBoxColumn13.UniqueName = "Validation";
            gridViewTextBoxColumn14.FieldAlias = "Format";
            gridViewTextBoxColumn14.FieldName = "Format";
            gridViewTextBoxColumn14.HeaderText = "Format";
            gridViewTextBoxColumn14.IsAutoGenerated = true;
            gridViewTextBoxColumn14.UniqueName = "Format";
            gridViewTextBoxColumn15.FieldName = "AttributeGroup.Description";
            gridViewTextBoxColumn15.HeaderText = "Attribute Group";
            gridViewTextBoxColumn15.UniqueName = "AttributeGroup";
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn1);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn2);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn3);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewCheckBoxColumn1);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn4);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn5);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn6);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn7);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn8);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn9);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn10);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn11);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn12);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn13);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn14);
            this.radGridView.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn15);
            this.radGridView.MasterGridViewTemplate.EnableFiltering = true;
            this.radGridView.Name = "radGridView";
            this.radGridView.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridView.ReadOnly = true;
            this.radGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.radGridView.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridView.ShowGroupPanel = false;
            this.radGridView.Size = new System.Drawing.Size(782, 404);
            this.radGridView.TabIndex = 12;
            this.radGridView.Text = "radGridViewPreview";
            this.radGridView.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.radGridView_ContextMenuOpening);
            // 
            // UcAttribute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.radGridView);
            this.Controls.Add(this.radPanel1);
            this.Name = "UcAttribute";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.Button btAdd;
        private Telerik.WinControls.UI.RadGridView radGridView;
        private System.Windows.Forms.Button btBack;
    }
}
