namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcArchiveStorageRelation
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
            Telerik.WinControls.Data.SortDescriptor sortDescriptor1 = new Telerik.WinControls.Data.SortDescriptor();
            this.radPanelTitle = new Telerik.WinControls.UI.RadPanel();
            this.btnStorages = new System.Windows.Forms.Button();
            this.btnArchives = new System.Windows.Forms.Button();
            this.btBackToList = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.btConfirm = new System.Windows.Forms.Button();
            this.lbRq = new System.Windows.Forms.Label();
            this.ddlRelatedEntity = new System.Windows.Forms.ComboBox();
            this.lbRelatedEntity = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.cbActive = new System.Windows.Forms.CheckBox();
            this.radPanelDetail = new Telerik.WinControls.UI.RadPanel();
            this.radGridView = new Telerik.WinControls.UI.RadGridView();
            this.documentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).BeginInit();
            this.radPanelTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelDetail)).BeginInit();
            this.radPanelDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanelTitle
            // 
            this.radPanelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanelTitle.BackColor = System.Drawing.SystemColors.Control;
            this.radPanelTitle.Controls.Add(this.btnStorages);
            this.radPanelTitle.Controls.Add(this.btnArchives);
            this.radPanelTitle.Controls.Add(this.btBackToList);
            this.radPanelTitle.Controls.Add(this.btAdd);
            this.radPanelTitle.Location = new System.Drawing.Point(6, 3);
            this.radPanelTitle.Name = "radPanelTitle";
            this.radPanelTitle.Size = new System.Drawing.Size(782, 46);
            this.radPanelTitle.TabIndex = 10;
            this.radPanelTitle.Text = "Archive-Storage Association";
            // 
            // btnStorages
            // 
            this.btnStorages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStorages.AutoSize = true;
            this.btnStorages.Location = new System.Drawing.Point(377, 6);
            this.btnStorages.Name = "btnStorages";
            this.btnStorages.Size = new System.Drawing.Size(85, 23);
            this.btnStorages.TabIndex = 13;
            this.btnStorages.TabStop = false;
            this.btnStorages.Text = "Storages";
            this.btnStorages.UseVisualStyleBackColor = true;
            this.btnStorages.Click += new System.EventHandler(this.btnStorages_Click);
            // 
            // btnArchives
            // 
            this.btnArchives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArchives.AutoSize = true;
            this.btnArchives.Location = new System.Drawing.Point(468, 6);
            this.btnArchives.Name = "btnArchives";
            this.btnArchives.Size = new System.Drawing.Size(85, 23);
            this.btnArchives.TabIndex = 12;
            this.btnArchives.TabStop = false;
            this.btnArchives.Text = "Archives";
            this.btnArchives.UseVisualStyleBackColor = true;
            this.btnArchives.Click += new System.EventHandler(this.btnArchives_Click);
            // 
            // btBackToList
            // 
            this.btBackToList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBackToList.AutoSize = true;
            this.btBackToList.Location = new System.Drawing.Point(563, 6);
            this.btBackToList.Name = "btBackToList";
            this.btBackToList.Size = new System.Drawing.Size(104, 23);
            this.btBackToList.TabIndex = 11;
            this.btBackToList.Text = "Back";
            this.btBackToList.UseVisualStyleBackColor = true;
            this.btBackToList.Click += new System.EventHandler(this.btBackToList_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.AutoSize = true;
            this.btAdd.Location = new System.Drawing.Point(675, 6);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(104, 23);
            this.btAdd.TabIndex = 10;
            this.btAdd.TabStop = false;
            this.btAdd.Text = "New";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.itemAddNew_Click);
            // 
            // btConfirm
            // 
            this.btConfirm.AutoSize = true;
            this.btConfirm.Location = new System.Drawing.Point(577, 20);
            this.btConfirm.Name = "btConfirm";
            this.btConfirm.Size = new System.Drawing.Size(104, 23);
            this.btConfirm.TabIndex = 9;
            this.btConfirm.Text = "Ok";
            this.btConfirm.UseVisualStyleBackColor = true;
            this.btConfirm.Click += new System.EventHandler(this.archivestorageUpdate_Click);
            // 
            // lbRq
            // 
            this.lbRq.AutoSize = true;
            this.lbRq.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRq.ForeColor = System.Drawing.Color.Red;
            this.lbRq.Location = new System.Drawing.Point(556, 28);
            this.lbRq.Name = "lbRq";
            this.lbRq.Size = new System.Drawing.Size(15, 19);
            this.lbRq.TabIndex = 41;
            this.lbRq.Text = "*";
            // 
            // ddlRelatedEntity
            // 
            this.ddlRelatedEntity.DisplayMember = "Name";
            this.ddlRelatedEntity.FormattingEnabled = true;
            this.ddlRelatedEntity.Location = new System.Drawing.Point(175, 22);
            this.ddlRelatedEntity.Name = "ddlRelatedEntity";
            this.ddlRelatedEntity.Size = new System.Drawing.Size(375, 21);
            this.ddlRelatedEntity.TabIndex = 0;
            this.ddlRelatedEntity.ValueMember = "IdArchive";
            // 
            // lbRelatedEntity
            // 
            this.lbRelatedEntity.AutoSize = true;
            this.lbRelatedEntity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRelatedEntity.Location = new System.Drawing.Point(115, 25);
            this.lbRelatedEntity.Name = "lbRelatedEntity";
            this.lbRelatedEntity.Size = new System.Drawing.Size(83, 13);
            this.lbRelatedEntity.TabIndex = 36;
            this.lbRelatedEntity.Text = "RelatedEntity";
            // 
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(577, 49);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 10;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(122, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Active:";
            // 
            // cbActive
            // 
            this.cbActive.AutoSize = true;
            this.cbActive.Location = new System.Drawing.Point(175, 56);
            this.cbActive.Name = "cbActive";
            this.cbActive.Size = new System.Drawing.Size(15, 14);
            this.cbActive.TabIndex = 6;
            this.cbActive.UseVisualStyleBackColor = true;
            // 
            // radPanelDetail
            // 
            this.radPanelDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanelDetail.Controls.Add(this.ddlRelatedEntity);
            this.radPanelDetail.Controls.Add(this.lbRelatedEntity);
            this.radPanelDetail.Controls.Add(this.cbActive);
            this.radPanelDetail.Controls.Add(this.label10);
            this.radPanelDetail.Controls.Add(this.lbRq);
            this.radPanelDetail.Controls.Add(this.btCancel);
            this.radPanelDetail.Controls.Add(this.btConfirm);
            this.radPanelDetail.Location = new System.Drawing.Point(9, 294);
            this.radPanelDetail.Name = "radPanelDetail";
            this.radPanelDetail.Size = new System.Drawing.Size(782, 151);
            this.radPanelDetail.TabIndex = 55;
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
            this.radGridView.Location = new System.Drawing.Point(5, 55);
            // 
            // radGridView
            // 
            this.radGridView.MasterTemplate.AllowAddNewRow = false;
            this.radGridView.MasterTemplate.AllowDeleteRow = false;
            this.radGridView.MasterTemplate.AllowEditRow = false;
            this.radGridView.MasterTemplate.AutoGenerateColumns = false;
            this.radGridView.MasterTemplate.Caption = "Archive for Storage";
            gridViewTextBoxColumn1.FieldName = "Archive.Name";
            gridViewTextBoxColumn1.HeaderText = "Archive";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.Name = "Archive.Name";
            gridViewTextBoxColumn1.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewTextBoxColumn1.Width = 243;
            gridViewTextBoxColumn2.FieldName = "Storage.Name";
            gridViewTextBoxColumn2.HeaderText = "Storage";
            gridViewTextBoxColumn2.Name = "Storage.Name";
            gridViewTextBoxColumn2.Width = 209;
            gridViewTextBoxColumn3.FieldName = "Storage.StorageType.Type";
            gridViewTextBoxColumn3.HeaderText = "Storage Type";
            gridViewTextBoxColumn3.Name = "Storage.StorageType.Type";
            gridViewTextBoxColumn3.Width = 178;
            gridViewCheckBoxColumn1.FieldName = "Active";
            gridViewCheckBoxColumn1.HeaderText = "Active";
            gridViewCheckBoxColumn1.IsAutoGenerated = true;
            gridViewCheckBoxColumn1.Name = "Active";
            gridViewCheckBoxColumn1.Width = 63;
            gridViewTextBoxColumn4.FieldName = "Archive";
            gridViewTextBoxColumn4.HeaderText = "Archive";
            gridViewTextBoxColumn4.IsVisible = false;
            gridViewTextBoxColumn4.Name = "Archive";
            gridViewTextBoxColumn5.FieldName = "Storage";
            gridViewTextBoxColumn5.HeaderText = "Storage";
            gridViewTextBoxColumn5.IsVisible = false;
            gridViewTextBoxColumn5.Name = "Storage";
            this.radGridView.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewCheckBoxColumn1,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5});
            this.radGridView.MasterTemplate.DataSource = this.documentBindingSource;
            this.radGridView.MasterTemplate.EnableFiltering = true;
            sortDescriptor1.PropertyName = "Archive.Name";
            this.radGridView.MasterTemplate.SortDescriptors.AddRange(new Telerik.WinControls.Data.SortDescriptor[] {
            sortDescriptor1});
            this.radGridView.Name = "radGridView";
            this.radGridView.ReadOnly = true;
            this.radGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridView.ShowGroupPanel = false;
            this.radGridView.Size = new System.Drawing.Size(785, 233);
            this.radGridView.TabIndex = 56;
            this.radGridView.Text = "radGridViewPreview";
            this.radGridView.SelectionChanged += new System.EventHandler(this.radGridView_SelectionChanged);
            this.radGridView.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.radGridView_ContextMenuOpening);
            // 
            // documentBindingSource
            // 
            this.documentBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Storage);
            // 
            // UcArchiveStorageRelation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.radGridView);
            this.Controls.Add(this.radPanelTitle);
            this.Controls.Add(this.radPanelDetail);
            this.Name = "UcArchiveStorageRelation";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).EndInit();
            this.radPanelTitle.ResumeLayout(false);
            this.radPanelTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelDetail)).EndInit();
            this.radPanelDetail.ResumeLayout(false);
            this.radPanelDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanelTitle;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.Button btConfirm;
        private System.Windows.Forms.Label lbRq;
        private System.Windows.Forms.ComboBox ddlRelatedEntity;
        private System.Windows.Forms.Label lbRelatedEntity;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbActive;
        private Telerik.WinControls.UI.RadPanel radPanelDetail;
        private Telerik.WinControls.UI.RadGridView radGridView;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btBackToList;
        private System.Windows.Forms.Button btnArchives;
        private System.Windows.Forms.Button btnStorages;
    }
}
