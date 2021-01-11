namespace BiblosDs.Document.AdminCentral.UControls
{
    partial class UcFindFile
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
            this.btnFind = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.radContextMenu1 = new Telerik.WinControls.UI.RadContextMenu(this.components);
            this.addChildCond = new Telerik.WinControls.UI.RadMenuItem();
            this.remCond = new Telerik.WinControls.UI.RadMenuItem();
            this.editCond = new Telerik.WinControls.UI.RadMenuItem();
            this.radGridFindResults = new Telerik.WinControls.UI.RadGridView();
            this.radContextMenu2 = new Telerik.WinControls.UI.RadContextMenu(this.components);
            this.showDocument = new Telerik.WinControls.UI.RadMenuItem();
            this.showMetadata = new Telerik.WinControls.UI.RadMenuItem();
            this.radTreeView1 = new Telerik.WinControls.UI.RadTreeView();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.radGridFindResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(20, 304);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 6;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(20, 19);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 16;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // radContextMenu1
            // 
            this.radContextMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.addChildCond,
            this.remCond,
            this.editCond});
            // 
            // addChildCond
            // 
            this.addChildCond.AccessibleDescription = "Add child condition";
            this.addChildCond.AccessibleName = "Add child condition";
            this.addChildCond.Name = "addChildCond";
            this.addChildCond.Text = "Add child condition";
            this.addChildCond.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // remCond
            // 
            this.remCond.AccessibleDescription = "Remove condition(s)";
            this.remCond.AccessibleName = "Remove condition(s)";
            this.remCond.Name = "remCond";
            this.remCond.PopupDirection = Telerik.WinControls.UI.RadDirection.Right;
            this.remCond.Text = "Remove condition(s)";
            this.remCond.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // editCond
            // 
            this.editCond.AccessibleDescription = "Edit condition";
            this.editCond.AccessibleName = "Edit condition";
            this.editCond.Name = "editCond";
            this.editCond.Text = "Edit condition";
            this.editCond.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // radGridFindResults
            // 
            this.radGridFindResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radGridFindResults.BackColor = System.Drawing.SystemColors.Control;
            this.radGridFindResults.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridFindResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.radGridFindResults.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radGridFindResults.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridFindResults.Location = new System.Drawing.Point(20, 336);
            // 
            // radGridFindResults
            // 
            this.radGridFindResults.MasterTemplate.AllowAddNewRow = false;
            this.radGridFindResults.MasterTemplate.AllowCellContextMenu = false;
            this.radGridFindResults.MasterTemplate.AllowColumnChooser = false;
            this.radGridFindResults.MasterTemplate.AllowColumnHeaderContextMenu = false;
            this.radGridFindResults.MasterTemplate.AllowDeleteRow = false;
            this.radGridFindResults.MasterTemplate.AllowEditRow = false;
            this.radGridFindResults.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewTextBoxColumn1.HeaderText = "N.";
            gridViewTextBoxColumn1.Name = "column1";
            gridViewTextBoxColumn1.Width = 24;
            gridViewTextBoxColumn2.HeaderText = "Filename";
            gridViewTextBoxColumn2.Name = "filename";
            gridViewTextBoxColumn2.Width = 68;
            gridViewTextBoxColumn3.HeaderText = "Status";
            gridViewTextBoxColumn3.Name = "status";
            gridViewTextBoxColumn3.Width = 49;
            gridViewTextBoxColumn4.HeaderText = "Details";
            gridViewTextBoxColumn4.Name = "details";
            gridViewTextBoxColumn4.Width = 601;
            gridViewTextBoxColumn5.HeaderText = "IdDocument";
            gridViewTextBoxColumn5.IsVisible = false;
            gridViewTextBoxColumn5.Name = "iddocument";
            this.radGridFindResults.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5});
            this.radGridFindResults.MasterTemplate.EnableGrouping = false;
            this.radGridFindResults.Name = "radGridFindResults";
            this.radGridFindResults.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridFindResults.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.radGridFindResults.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridFindResults.ShowGroupPanel = false;
            this.radGridFindResults.Size = new System.Drawing.Size(759, 169);
            this.radGridFindResults.TabIndex = 18;
            this.radGridFindResults.Text = "radGridViewPreview";
            // 
            // radContextMenu2
            // 
            this.radContextMenu2.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.showDocument,
            this.showMetadata});
            // 
            // showDocument
            // 
            this.showDocument.AccessibleDescription = "Show &document";
            this.showDocument.AccessibleName = "Show &document";
            this.showDocument.Name = "showDocument";
            this.showDocument.Text = "Show &document";
            this.showDocument.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // showMetadata
            // 
            this.showMetadata.AccessibleDescription = "Show &metadata";
            this.showMetadata.AccessibleName = "Show &metadata";
            this.showMetadata.Name = "showMetadata";
            this.showMetadata.Text = "Show &metadata";
            this.showMetadata.Visibility = Telerik.WinControls.ElementVisibility.Visible;
            // 
            // radTreeView1
            // 
            this.radTreeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radTreeView1.BackColor = System.Drawing.SystemColors.Window;
            this.radTreeView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.radTreeView1.Font = new System.Drawing.Font("Tahoma", 8.6F);
            this.radTreeView1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radTreeView1.Location = new System.Drawing.Point(20, 48);
            this.radTreeView1.Name = "radTreeView1";
            this.radTreeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radTreeView1.Size = new System.Drawing.Size(759, 250);
            this.radTreeView1.SpacingBetweenNodes = -1;
            this.radTreeView1.TabIndex = 17;
            this.radTreeView1.Text = "radTreeView1";
            // 
            // UcFindFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radGridFindResults);
            this.Controls.Add(this.radTreeView1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnFind);
            this.Name = "UcFindFile";
            this.Size = new System.Drawing.Size(812, 513);
            this.Load += new System.EventHandler(this.UcFindFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radGridFindResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnAdd;
        private Telerik.WinControls.UI.RadContextMenu radContextMenu1;
        private Telerik.WinControls.UI.RadMenuItem addChildCond;
        private Telerik.WinControls.UI.RadMenuItem remCond;
        private Telerik.WinControls.UI.RadMenuItem editCond;
        private Telerik.WinControls.UI.RadGridView radGridFindResults;
        private Telerik.WinControls.UI.RadContextMenu radContextMenu2;
        private Telerik.WinControls.UI.RadMenuItem showDocument;
        private Telerik.WinControls.UI.RadMenuItem showMetadata;
        private Telerik.WinControls.UI.RadTreeView radTreeView1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
