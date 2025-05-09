﻿namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcServer
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
            Telerik.WinControls.Data.SortDescriptor sortDescriptor1 = new Telerik.WinControls.Data.SortDescriptor();
            this.pnlRoot = new System.Windows.Forms.Panel();
            this.gvServers = new Telerik.WinControls.UI.RadGridView();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btAdd = new System.Windows.Forms.Button();
            this.cmServers = new Telerik.WinControls.UI.RadContextMenu(this.components);
            this.itmModify = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuDelete = new Telerik.WinControls.UI.RadMenuItem();
            this.pnlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvServers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRoot
            // 
            this.pnlRoot.BackColor = System.Drawing.Color.White;
            this.pnlRoot.Controls.Add(this.gvServers);
            this.pnlRoot.Controls.Add(this.radPanel1);
            this.pnlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRoot.Location = new System.Drawing.Point(0, 0);
            this.pnlRoot.Name = "pnlRoot";
            this.pnlRoot.Size = new System.Drawing.Size(640, 480);
            this.pnlRoot.TabIndex = 0;
            // 
            // gvServers
            // 
            this.gvServers.BackColor = System.Drawing.SystemColors.Control;
            this.gvServers.Cursor = System.Windows.Forms.Cursors.Default;
            this.gvServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.gvServers.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gvServers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gvServers.Location = new System.Drawing.Point(0, 46);
            // 
            // gvServers
            // 
            this.gvServers.MasterTemplate.AllowAddNewRow = false;
            this.gvServers.MasterTemplate.AllowDeleteRow = false;
            this.gvServers.MasterTemplate.AllowEditRow = false;
            this.gvServers.MasterTemplate.AutoGenerateColumns = false;
            this.gvServers.MasterTemplate.Caption = "Storage";
            gridViewTextBoxColumn1.FieldName = "IdServer";
            gridViewTextBoxColumn1.FormatString = "";
            gridViewTextBoxColumn1.HeaderText = "IdServer";
            gridViewTextBoxColumn1.IsVisible = false;
            gridViewTextBoxColumn1.Name = "IdServer";
            gridViewTextBoxColumn2.FieldName = "ServerName";
            gridViewTextBoxColumn2.FormatString = "";
            gridViewTextBoxColumn2.HeaderText = "Name";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.Name = "ServerName";
            gridViewTextBoxColumn2.Width = 200;
            gridViewTextBoxColumn3.FieldName = "ServerRole";
            gridViewTextBoxColumn3.FormatString = "";
            gridViewTextBoxColumn3.HeaderText = "Role";
            gridViewTextBoxColumn3.Name = "ServerRole";
            gridViewTextBoxColumn3.Width = 200;
            this.gvServers.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3});
            this.gvServers.MasterTemplate.EnableFiltering = true;
            sortDescriptor1.PropertyName = "Name";
            this.gvServers.MasterTemplate.SortDescriptors.AddRange(new Telerik.WinControls.Data.SortDescriptor[] {
            sortDescriptor1});
            this.gvServers.Name = "gvServers";
            this.gvServers.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.gvServers.ReadOnly = true;
            this.gvServers.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.gvServers.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.gvServers.ShowGroupPanel = false;
            this.gvServers.Size = new System.Drawing.Size(640, 434);
            this.gvServers.TabIndex = 12;
            this.gvServers.Text = "radGridViewPreview";
            this.gvServers.ContextMenuOpening += new Telerik.WinControls.UI.ContextMenuOpeningEventHandler(this.gvServers_ContextMenuOpening);
            // 
            // radPanel1
            // 
            this.radPanel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanel1.Controls.Add(this.btAdd);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(640, 46);
            this.radPanel1.TabIndex = 11;
            this.radPanel1.Text = "Servers";
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.AutoSize = true;
            this.btAdd.Location = new System.Drawing.Point(523, 12);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(104, 23);
            this.btAdd.TabIndex = 0;
            this.btAdd.Text = "New Server";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // cmServers
            // 
            this.cmServers.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.itmModify,
            this.mnuDelete});
            // 
            // itmModify
            // 
            this.itmModify.AccessibleDescription = "Modify";
            this.itmModify.AccessibleName = "Modify";
            this.itmModify.Name = "itmModify";
            this.itmModify.Text = "Modify";
            this.itmModify.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // mnuDelete
            // 
            this.mnuDelete.AccessibleDescription = "Delete";
            this.mnuDelete.AccessibleName = "Delete";
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Text = "Delete";
            this.mnuDelete.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // UcServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlRoot);
            this.Name = "UcServer";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.UcServer_Load);
            this.pnlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvServers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlRoot;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private System.Windows.Forms.Button btAdd;
        private Telerik.WinControls.UI.RadGridView gvServers;
        private Telerik.WinControls.UI.RadContextMenu cmServers;
        private Telerik.WinControls.UI.RadMenuItem itmModify;
        private Telerik.WinControls.UI.RadMenuItem mnuDelete;

    }
}
