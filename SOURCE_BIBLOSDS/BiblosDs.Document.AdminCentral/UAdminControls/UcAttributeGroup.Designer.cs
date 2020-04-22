namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcAttributeGroup
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
            Telerik.WinControls.Data.SortDescriptor sortDescriptor1 = new Telerik.WinControls.Data.SortDescriptor();
            this.btInsertAg = new System.Windows.Forms.Button();
            this.radTitlePanel = new Telerik.WinControls.UI.RadPanel();
            this.btBack = new System.Windows.Forms.Button();
            this.radGridViewAG = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.radTitlePanel)).BeginInit();
            this.radTitlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewAG)).BeginInit();
            this.SuspendLayout();
            // 
            // btInsertAg
            // 
            this.btInsertAg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btInsertAg.AutoSize = true;
            this.btInsertAg.Location = new System.Drawing.Point(659, 12);
            this.btInsertAg.Name = "btInsertAg";
            this.btInsertAg.Size = new System.Drawing.Size(122, 23);
            this.btInsertAg.TabIndex = 0;
            this.btInsertAg.Text = "New AttributeGroup";
            this.btInsertAg.UseVisualStyleBackColor = true;
            this.btInsertAg.Click += new System.EventHandler(this.btInsertAg_Click);
            // 
            // radTitlePanel
            // 
            this.radTitlePanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radTitlePanel.Controls.Add(this.btBack);
            this.radTitlePanel.Controls.Add(this.btInsertAg);
            this.radTitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.radTitlePanel.Location = new System.Drawing.Point(0, 0);
            this.radTitlePanel.Name = "radTitlePanel";
            this.radTitlePanel.Size = new System.Drawing.Size(794, 46);
            this.radTitlePanel.TabIndex = 11;
            this.radTitlePanel.Text = "Attribute Group";
            // 
            // btBack
            // 
            this.btBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBack.AutoSize = true;
            this.btBack.Location = new System.Drawing.Point(543, 12);
            this.btBack.Name = "btBack";
            this.btBack.Size = new System.Drawing.Size(110, 23);
            this.btBack.TabIndex = 1;
            this.btBack.Text = "Back";
            this.btBack.UseVisualStyleBackColor = true;
            this.btBack.Click += new System.EventHandler(this.btBack_Click);
            // 
            // radGridViewAG
            // 
            this.radGridViewAG.AutoScroll = true;
            this.radGridViewAG.BackColor = System.Drawing.SystemColors.Control;
            this.radGridViewAG.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridViewAG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridViewAG.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.radGridViewAG.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radGridViewAG.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridViewAG.Location = new System.Drawing.Point(0, 46);
            // 
            // radGridViewAG
            // 
            this.radGridViewAG.MasterTemplate.AllowAddNewRow = false;
            this.radGridViewAG.MasterTemplate.AllowDeleteRow = false;
            this.radGridViewAG.MasterTemplate.AllowEditRow = false;
            this.radGridViewAG.MasterTemplate.AutoGenerateColumns = false;
            this.radGridViewAG.MasterTemplate.Caption = "AttributeGroup";
            gridViewTextBoxColumn1.DataType = typeof(System.Guid);
            gridViewTextBoxColumn1.FieldName = "IdArchive";
            gridViewTextBoxColumn1.HeaderText = "IdArchive";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.IsVisible = false;
            gridViewTextBoxColumn1.Name = "IdArchive";
            gridViewTextBoxColumn1.Width = 243;
            gridViewTextBoxColumn2.FieldName = "Name";
            gridViewTextBoxColumn2.HeaderText = "Name";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.Name = "Name";
            gridViewTextBoxColumn2.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewTextBoxColumn2.Width = 185;
            gridViewTextBoxColumn3.FieldName = "IdAttributeGroup";
            gridViewTextBoxColumn3.HeaderText = "IdAttributeGroup";
            gridViewTextBoxColumn3.IsVisible = false;
            gridViewTextBoxColumn3.Name = "IdAttributeGroup";
            gridViewTextBoxColumn3.Width = 185;
            gridViewTextBoxColumn4.FieldName = "Description";
            gridViewTextBoxColumn4.HeaderText = "Description";
            gridViewTextBoxColumn4.Name = "Description";
            gridViewTextBoxColumn4.Width = 185;
            gridViewTextBoxColumn5.FieldName = "GroupType";
            gridViewTextBoxColumn5.HeaderText = "Type";
            gridViewTextBoxColumn5.Name = "GroupType";
            gridViewTextBoxColumn5.Width = 185;
            this.radGridViewAG.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5});
            this.radGridViewAG.MasterTemplate.EnableFiltering = true;
            this.radGridViewAG.MasterTemplate.HorizontalScrollState = Telerik.WinControls.UI.ScrollState.AlwaysShow;
            sortDescriptor1.PropertyName = "Name";
            this.radGridViewAG.MasterTemplate.SortDescriptors.AddRange(new Telerik.WinControls.Data.SortDescriptor[] {
            sortDescriptor1});
            this.radGridViewAG.Name = "radGridViewAG";
            this.radGridViewAG.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridViewAG.ReadOnly = true;
            this.radGridViewAG.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.radGridViewAG.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridViewAG.ShowGroupPanel = false;
            this.radGridViewAG.Size = new System.Drawing.Size(794, 428);
            this.radGridViewAG.TabIndex = 12;
            this.radGridViewAG.Text = "radGridViewPreview";
            // 
            // UcAttributeGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.radGridViewAG);
            this.Controls.Add(this.radTitlePanel);
            this.Name = "UcAttributeGroup";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radTitlePanel)).EndInit();
            this.radTitlePanel.ResumeLayout(false);
            this.radTitlePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewAG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btInsertAg;
        private Telerik.WinControls.UI.RadPanel radTitlePanel;
        private Telerik.WinControls.UI.RadGridView radGridViewAG;
        private System.Windows.Forms.Button btBack;
    }
}
