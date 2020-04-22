namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcLog
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn11 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn12 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn13 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn14 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn15 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridSortField gridSortField3 = new Telerik.WinControls.UI.GridSortField();
            this.radTitlePanel = new Telerik.WinControls.UI.RadPanel();
            this.splitPnl = new System.Windows.Forms.SplitContainer();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.radBtSearch = new Telerik.WinControls.UI.RadButton();
            this.radCheckIdArch = new Telerik.WinControls.UI.RadCheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radCbArchives = new Telerik.WinControls.UI.RadComboBox();
            this.radCheckDataTo = new Telerik.WinControls.UI.RadCheckBox();
            this.radDtTo = new Telerik.WinControls.UI.RadDateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.radDtFrom = new Telerik.WinControls.UI.RadDateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.radGridLogs = new Telerik.WinControls.UI.RadGridView();
            this.ucPager = new BiblosDs.Document.AdminCentral.UControls.Pager.UcPager();
            ((System.ComponentModel.ISupportInitialize)(this.radTitlePanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPnl)).BeginInit();
            this.splitPnl.Panel1.SuspendLayout();
            this.splitPnl.Panel2.SuspendLayout();
            this.splitPnl.SuspendLayout();
            this.grpSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radBtSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCheckIdArch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCbArchives)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCheckDataTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radDtFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridLogs)).BeginInit();
            this.SuspendLayout();
            // 
            // radTitlePanel
            // 
            this.radTitlePanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radTitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.radTitlePanel.Location = new System.Drawing.Point(0, 0);
            this.radTitlePanel.Name = "radTitlePanel";
            this.radTitlePanel.Size = new System.Drawing.Size(794, 46);
            this.radTitlePanel.TabIndex = 11;
            this.radTitlePanel.Text = "Logs";
            // 
            // splitPnl
            // 
            this.splitPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitPnl.Location = new System.Drawing.Point(0, 46);
            this.splitPnl.Name = "splitPnl";
            this.splitPnl.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitPnl.Panel1
            // 
            this.splitPnl.Panel1.Controls.Add(this.grpSearch);
            // 
            // splitPnl.Panel2
            // 
            this.splitPnl.Panel2.Controls.Add(this.radGridLogs);
            this.splitPnl.Panel2.Controls.Add(this.ucPager);
            this.splitPnl.Size = new System.Drawing.Size(794, 428);
            this.splitPnl.SplitterDistance = 90;
            this.splitPnl.TabIndex = 12;
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.radBtSearch);
            this.grpSearch.Controls.Add(this.radCheckIdArch);
            this.grpSearch.Controls.Add(this.label3);
            this.grpSearch.Controls.Add(this.radCbArchives);
            this.grpSearch.Controls.Add(this.radCheckDataTo);
            this.grpSearch.Controls.Add(this.radDtTo);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Controls.Add(this.radDtFrom);
            this.grpSearch.Controls.Add(this.label1);
            this.grpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSearch.Location = new System.Drawing.Point(0, 0);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(794, 90);
            this.grpSearch.TabIndex = 0;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Opzioni di visualizzazione";
            // 
            // radBtSearch
            // 
            this.radBtSearch.Location = new System.Drawing.Point(658, 58);
            this.radBtSearch.Name = "radBtSearch";
            this.radBtSearch.Size = new System.Drawing.Size(130, 24);
            this.radBtSearch.TabIndex = 7;
            this.radBtSearch.Text = "Aggiorna";
            this.radBtSearch.Click += new System.EventHandler(this.radBtSearch_Click);
            // 
            // radCheckIdArch
            // 
            this.radCheckIdArch.Location = new System.Drawing.Point(278, 64);
            this.radCheckIdArch.Name = "radCheckIdArch";
            this.radCheckIdArch.Size = new System.Drawing.Size(53, 18);
            this.radCheckIdArch.TabIndex = 5;
            this.radCheckIdArch.Text = "Includi";
            this.radCheckIdArch.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.radCheckIdArch_ToggleStateChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Archivio:";
            // 
            // radCbArchives
            // 
            this.radCbArchives.Enabled = false;
            this.radCbArchives.Location = new System.Drawing.Point(71, 64);
            this.radCbArchives.Name = "radCbArchives";
            // 
            // 
            // 
            this.radCbArchives.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radCbArchives.Size = new System.Drawing.Size(201, 20);
            this.radCbArchives.TabIndex = 5;
            this.radCbArchives.TabStop = false;
            this.radCbArchives.SelectedIndexChanged += new System.EventHandler(this.radCbArchivesID_SelectedIndexChanged);
            // 
            // radCheckDataTo
            // 
            this.radCheckDataTo.Location = new System.Drawing.Point(278, 42);
            this.radCheckDataTo.Name = "radCheckDataTo";
            this.radCheckDataTo.Size = new System.Drawing.Size(53, 18);
            this.radCheckDataTo.TabIndex = 4;
            this.radCheckDataTo.Text = "Includi";
            this.radCheckDataTo.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.radCheckDataTo_ToggleStateChanged);
            // 
            // radDtTo
            // 
            this.radDtTo.Culture = new System.Globalization.CultureInfo("it-IT");
            this.radDtTo.Enabled = false;
            this.radDtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.radDtTo.Location = new System.Drawing.Point(71, 42);
            this.radDtTo.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.radDtTo.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.radDtTo.Name = "radDtTo";
            this.radDtTo.NullDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.radDtTo.Size = new System.Drawing.Size(201, 20);
            this.radDtTo.TabIndex = 2;
            this.radDtTo.TabStop = false;
            this.radDtTo.Value = new System.DateTime(2010, 6, 14, 11, 30, 45, 94);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Data a:";
            // 
            // radDtFrom
            // 
            this.radDtFrom.Culture = new System.Globalization.CultureInfo("it-IT");
            this.radDtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.radDtFrom.Location = new System.Drawing.Point(71, 19);
            this.radDtFrom.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.radDtFrom.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.radDtFrom.Name = "radDtFrom";
            this.radDtFrom.NullDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.radDtFrom.Size = new System.Drawing.Size(201, 20);
            this.radDtFrom.TabIndex = 1;
            this.radDtFrom.TabStop = false;
            this.radDtFrom.Value = new System.DateTime(2010, 6, 14, 11, 30, 45, 94);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data da:";
            // 
            // radGridLogs
            // 
            this.radGridLogs.BackColor = System.Drawing.SystemColors.Control;
            this.radGridLogs.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.radGridLogs.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radGridLogs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridLogs.Location = new System.Drawing.Point(0, 0);
            // 
            // radGridLogs
            // 
            this.radGridLogs.MasterTemplate.AllowAddNewRow = false;
            this.radGridLogs.MasterTemplate.AutoGenerateColumns = false;
            this.radGridLogs.MasterTemplate.Caption = "DocumentAttribute";
            gridViewTextBoxColumn11.FieldName = "IdEntry";
            gridViewTextBoxColumn11.FormatString = "";
            gridViewTextBoxColumn11.HeaderText = "IdEntry";
            gridViewTextBoxColumn11.IsAutoGenerated = true;
            gridViewTextBoxColumn11.Name = "IdEntry";
            gridViewTextBoxColumn11.Width = 200;
            gridViewTextBoxColumn12.FieldName = "TimeStamp";
            gridViewTextBoxColumn12.FormatString = "";
            gridViewTextBoxColumn12.HeaderText = "Time Stamp";
            gridViewTextBoxColumn12.IsAutoGenerated = true;
            gridViewTextBoxColumn12.Name = "TimeStamp";
            gridViewTextBoxColumn12.Width = 130;
            gridViewTextBoxColumn13.FieldName = "Message";
            gridViewTextBoxColumn13.FormatString = "";
            gridViewTextBoxColumn13.HeaderText = "Message";
            gridViewTextBoxColumn13.Multiline = true;
            gridViewTextBoxColumn13.Name = "Message";
            gridViewTextBoxColumn13.StretchVertically = false;
            gridViewTextBoxColumn13.Width = 375;
            gridViewTextBoxColumn13.WrapText = true;
            gridViewTextBoxColumn14.FieldName = "Server";
            gridViewTextBoxColumn14.FormatString = "";
            gridViewTextBoxColumn14.HeaderText = "Server";
            gridViewTextBoxColumn14.Name = "Server";
            gridViewTextBoxColumn15.FieldName = "Client";
            gridViewTextBoxColumn15.FormatString = "";
            gridViewTextBoxColumn15.HeaderText = "Client";
            gridViewTextBoxColumn15.Name = "Client";
            this.radGridLogs.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn11,
            gridViewTextBoxColumn12,
            gridViewTextBoxColumn13,
            gridViewTextBoxColumn14,
            gridViewTextBoxColumn15});
            this.radGridLogs.MasterTemplate.EnableFiltering = true;
            gridSortField3.FieldAlias = "Name";
            gridSortField3.FieldName = "Name";
            gridSortField3.PropertyName = "Name";
            gridSortField3.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            this.radGridLogs.MasterTemplate.SortDescriptors.AddRange(new Telerik.WinControls.Data.SortDescriptor[] {
            gridSortField3});
            this.radGridLogs.Name = "radGridLogs";
            this.radGridLogs.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridLogs.ReadOnly = true;
            this.radGridLogs.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.radGridLogs.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridLogs.ShowGroupPanel = false;
            this.radGridLogs.Size = new System.Drawing.Size(794, 303);
            this.radGridLogs.TabIndex = 13;
            this.radGridLogs.Text = "radGridViewPreview";
            // 
            // ucPager
            // 
            this.ucPager.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucPager.Location = new System.Drawing.Point(0, 303);
            this.ucPager.Name = "ucPager";
            this.ucPager.Size = new System.Drawing.Size(794, 31);
            this.ucPager.TabIndex = 14;
            this.ucPager.FirstPage += new BiblosDs.Document.AdminCentral.UControls.Pager.UcPager.PageChangingHandler(this.ucPager_ChangePage);
            this.ucPager.NexPage += new BiblosDs.Document.AdminCentral.UControls.Pager.UcPager.PageChangingHandler(this.ucPager_ChangePage);
            this.ucPager.PrevPage += new BiblosDs.Document.AdminCentral.UControls.Pager.UcPager.PageChangingHandler(this.ucPager_ChangePage);
            this.ucPager.LastPage += new BiblosDs.Document.AdminCentral.UControls.Pager.UcPager.PageChangingHandler(this.ucPager_ChangePage);
            this.ucPager.GotoPage += new BiblosDs.Document.AdminCentral.UControls.Pager.UcPager.PageChangingHandler(this.ucPager_ChangePage);
            // 
            // UcLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitPnl);
            this.Controls.Add(this.radTitlePanel);
            this.Name = "UcLog";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radTitlePanel)).EndInit();
            this.splitPnl.Panel1.ResumeLayout(false);
            this.splitPnl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPnl)).EndInit();
            this.splitPnl.ResumeLayout(false);
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radBtSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCheckIdArch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCbArchives)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCheckDataTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radDtFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridLogs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radTitlePanel;
        private System.Windows.Forms.SplitContainer splitPnl;
        private System.Windows.Forms.GroupBox grpSearch;
        private Telerik.WinControls.UI.RadCheckBox radCheckDataTo;
        private Telerik.WinControls.UI.RadDateTimePicker radDtTo;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.UI.RadDateTimePicker radDtFrom;
        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadGridView radGridLogs;
        private Telerik.WinControls.UI.RadCheckBox radCheckIdArch;
        private System.Windows.Forms.Label label3;
        private Telerik.WinControls.UI.RadComboBox radCbArchives;
        private Telerik.WinControls.UI.RadButton radBtSearch;
        private UControls.Pager.UcPager ucPager;
    }
}
