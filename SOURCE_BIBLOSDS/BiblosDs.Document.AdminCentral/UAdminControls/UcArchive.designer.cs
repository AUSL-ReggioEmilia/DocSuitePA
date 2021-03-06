﻿namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UCArchive
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
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn2 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn3 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.Data.SortDescriptor sortDescriptor1 = new Telerik.WinControls.Data.SortDescriptor();
            this.radGridView = new Telerik.WinControls.UI.RadGridView();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btnStorages = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).BeginInit();
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
            gridViewTextBoxColumn1.FieldName = "IdArchive";
            gridViewTextBoxColumn1.FormatString = "";
            gridViewTextBoxColumn1.HeaderText = "IdArchive";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.Name = "IdArchive";
            gridViewTextBoxColumn1.Width = 243;
            gridViewTextBoxColumn2.FieldName = "Name";
            gridViewTextBoxColumn2.FormatString = "";
            gridViewTextBoxColumn2.HeaderText = "Name";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.Name = "Name";
            gridViewTextBoxColumn2.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewTextBoxColumn2.Width = 185;
            gridViewCheckBoxColumn1.FieldName = "IsLegal";
            gridViewCheckBoxColumn1.FormatString = "";
            gridViewCheckBoxColumn1.HeaderText = "Is Legal";
            gridViewCheckBoxColumn1.IsAutoGenerated = true;
            gridViewCheckBoxColumn1.Name = "IsLegal";
            gridViewCheckBoxColumn1.Width = 90;
            gridViewCheckBoxColumn2.FieldName = "EnableSecurity";
            gridViewCheckBoxColumn2.HeaderText = "Enable Security";
            gridViewCheckBoxColumn2.Name = "EnableSecurity";
            gridViewCheckBoxColumn3.FieldName = "TransitoEnabled";
            gridViewCheckBoxColumn3.FormatString = "";
            gridViewCheckBoxColumn3.HeaderText = "Transit";
            gridViewCheckBoxColumn3.Name = "TransitoEnabled";
            gridViewTextBoxColumn3.FieldName = "PathTransito";
            gridViewTextBoxColumn3.FormatString = "";
            gridViewTextBoxColumn3.HeaderText = "Path Transito";
            gridViewTextBoxColumn3.Name = "PathTransito";
            gridViewTextBoxColumn3.Width = 150;
            gridViewTextBoxColumn4.FieldName = "PathPreservation";
            gridViewTextBoxColumn4.FormatString = "";
            gridViewTextBoxColumn4.HeaderText = "Path Preservation";
            gridViewTextBoxColumn4.Name = "PathPreservation";
            gridViewTextBoxColumn4.Width = 150;
            this.radGridView.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewCheckBoxColumn1,
            gridViewCheckBoxColumn2,
            gridViewCheckBoxColumn3,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4});
            this.radGridView.MasterTemplate.EnableFiltering = true;
            sortDescriptor1.PropertyName = "Name";
            this.radGridView.MasterTemplate.SortDescriptors.AddRange(new Telerik.WinControls.Data.SortDescriptor[] {
            sortDescriptor1});
            this.radGridView.Name = "radGridView";
            this.radGridView.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridView.ReadOnly = true;
            this.radGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.radGridView.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridView.ShowGroupPanel = false;
            this.radGridView.Size = new System.Drawing.Size(785, 398);
            this.radGridView.TabIndex = 9;
            this.radGridView.Text = "radGridViewPreview";
            this.radGridView.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.radGridView_ContextMenuOpening);
            this.radGridView.DataBindingComplete += new Telerik.WinControls.UI.GridViewBindingCompleteEventHandler(this.radGridView_DataBindingComplete);
            // 
            // radPanel1
            // 
            this.radPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanel1.Controls.Add(this.btnStorages);
            this.radPanel1.Controls.Add(this.btAdd);
            this.radPanel1.Location = new System.Drawing.Point(6, 3);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(782, 46);
            this.radPanel1.TabIndex = 10;
            this.radPanel1.Text = "Archives";
            // 
            // btnStorages
            // 
            this.btnStorages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStorages.AutoSize = true;
            this.btnStorages.Location = new System.Drawing.Point(574, 12);
            this.btnStorages.Name = "btnStorages";
            this.btnStorages.Size = new System.Drawing.Size(85, 23);
            this.btnStorages.TabIndex = 14;
            this.btnStorages.TabStop = false;
            this.btnStorages.Text = "Storages";
            this.btnStorages.UseVisualStyleBackColor = true;
            this.btnStorages.Click += new System.EventHandler(this.btnStorages_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.AutoSize = true;
            this.btAdd.Location = new System.Drawing.Point(665, 12);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(104, 23);
            this.btAdd.TabIndex = 0;
            this.btAdd.Text = "New Archive";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.itemAddNew_Click);
            // 
            // UCArchive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.radPanel1);
            this.Controls.Add(this.radGridView);
            this.Name = "UCArchive";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btnStorages;
    }
}
