using BiblosDs.Document.AdminCentral.ServiceReferenceDocument;

namespace BiblosDs.Document.AdminCentral.UControls
{
    partial class UcFileUploader
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
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn9 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn10 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnAdd = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.gwAttributes = new Telerik.WinControls.UI.RadGridView();
            this.attributeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.radComboBoxArchive = new Telerik.WinControls.UI.RadComboBox();
            this.archiveBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel2 = new Telerik.WinControls.UI.RadPanel();
            this.documentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gwAttributes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.attributeBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxArchive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.archiveBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).BeginInit();
            this.radPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(11, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(117, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Aggiungi documento";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Attributes:";
            // 
            // gwAttributes
            // 
            this.gwAttributes.BackColor = System.Drawing.SystemColors.Control;
            this.gwAttributes.Cursor = System.Windows.Forms.Cursors.Default;
            this.gwAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gwAttributes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.gwAttributes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gwAttributes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gwAttributes.Location = new System.Drawing.Point(0, 106);
            // 
            // gwAttributes
            // 
            this.gwAttributes.MasterTemplate.AllowAddNewRow = false;
            this.gwAttributes.MasterTemplate.AutoGenerateColumns = false;
            gridViewTextBoxColumn1.FieldName = "AttributeType";
            gridViewTextBoxColumn1.HeaderText = "Type";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.Name = "AttributeType";
            gridViewTextBoxColumn1.ReadOnly = true;
            gridViewTextBoxColumn1.Width = 134;
            gridViewTextBoxColumn2.FieldName = "Format";
            gridViewTextBoxColumn2.HeaderText = "Format";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.IsVisible = false;
            gridViewTextBoxColumn2.Name = "Format";
            gridViewTextBoxColumn2.Width = 82;
            gridViewTextBoxColumn3.DataType = typeof(System.Guid);
            gridViewTextBoxColumn3.FieldName = "IdAttribute";
            gridViewTextBoxColumn3.HeaderText = "IdAttribute";
            gridViewTextBoxColumn3.IsAutoGenerated = true;
            gridViewTextBoxColumn3.IsVisible = false;
            gridViewTextBoxColumn3.Name = "IdAttribute";
            gridViewTextBoxColumn4.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn4.FieldName = "IsAutoInc";
            gridViewTextBoxColumn4.HeaderText = "IsAutoInc";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.IsVisible = false;
            gridViewTextBoxColumn4.Name = "IsAutoInc";
            gridViewTextBoxColumn4.Width = 113;
            gridViewTextBoxColumn5.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn5.FieldName = "IsEnumerator";
            gridViewTextBoxColumn5.HeaderText = "IsEnumerator";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.IsVisible = false;
            gridViewTextBoxColumn5.Name = "IsEnumerator";
            gridViewTextBoxColumn6.DataType = typeof(System.Nullable<bool>);
            gridViewTextBoxColumn6.FieldName = "IsMainDate";
            gridViewTextBoxColumn6.HeaderText = "IsMainDate";
            gridViewTextBoxColumn6.IsAutoGenerated = true;
            gridViewTextBoxColumn6.IsVisible = false;
            gridViewTextBoxColumn6.Name = "IsMainDate";
            gridViewCheckBoxColumn1.FieldName = "IsRequired";
            gridViewCheckBoxColumn1.HeaderText = "Required";
            gridViewCheckBoxColumn1.IsAutoGenerated = true;
            gridViewCheckBoxColumn1.Name = "IsRequired";
            gridViewCheckBoxColumn1.ReadOnly = true;
            gridViewCheckBoxColumn1.Width = 118;
            gridViewTextBoxColumn7.DataType = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceDocument.AttributeMode);
            gridViewTextBoxColumn7.FieldName = "Mode";
            gridViewTextBoxColumn7.HeaderText = "Mode";
            gridViewTextBoxColumn7.IsAutoGenerated = true;
            gridViewTextBoxColumn7.IsVisible = false;
            gridViewTextBoxColumn7.Name = "Mode";
            gridViewTextBoxColumn8.FieldName = "Name";
            gridViewTextBoxColumn8.HeaderText = "Name";
            gridViewTextBoxColumn8.IsAutoGenerated = true;
            gridViewTextBoxColumn8.Name = "Name";
            gridViewTextBoxColumn8.ReadOnly = true;
            gridViewTextBoxColumn8.Width = 200;
            gridViewTextBoxColumn9.FieldName = "Validation";
            gridViewTextBoxColumn9.HeaderText = "Validation";
            gridViewTextBoxColumn9.IsAutoGenerated = true;
            gridViewTextBoxColumn9.IsVisible = false;
            gridViewTextBoxColumn9.Name = "Validation";
            gridViewTextBoxColumn10.AllowGroup = false;
            gridViewTextBoxColumn10.AllowSort = false;
            gridViewTextBoxColumn10.HeaderText = "Value";
            gridViewTextBoxColumn10.Name = "colValue";
            gridViewTextBoxColumn10.Width = 153;
            this.gwAttributes.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5,
            gridViewTextBoxColumn6,
            gridViewCheckBoxColumn1,
            gridViewTextBoxColumn7,
            gridViewTextBoxColumn8,
            gridViewTextBoxColumn9,
            gridViewTextBoxColumn10});
            this.gwAttributes.MasterTemplate.DataSource = this.attributeBindingSource;
            this.gwAttributes.MasterTemplate.EnableGrouping = false;
            this.gwAttributes.Name = "gwAttributes";
            this.gwAttributes.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.gwAttributes.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.gwAttributes.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.gwAttributes.ShowGroupPanel = false;
            this.gwAttributes.Size = new System.Drawing.Size(873, 400);
            this.gwAttributes.TabIndex = 7;
            this.gwAttributes.Text = "radGridViewPreview";
            // 
            // attributeBindingSource
            // 
            this.attributeBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Attribute);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(873, 33);
            this.panel1.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Insert new document";
            // 
            // radComboBoxArchive
            // 
            this.radComboBoxArchive.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.archiveBindingSource, "Name", true));
            this.radComboBoxArchive.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.archiveBindingSource, "Name", true));
            this.radComboBoxArchive.FormatString = "{0}";
            this.radComboBoxArchive.Location = new System.Drawing.Point(86, 12);
            this.radComboBoxArchive.Name = "radComboBoxArchive";
            // 
            // 
            // 
            this.radComboBoxArchive.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radComboBoxArchive.Size = new System.Drawing.Size(245, 20);
            this.radComboBoxArchive.TabIndex = 9;
            this.radComboBoxArchive.TabStop = false;
            this.radComboBoxArchive.Text = "-Select one-";
            this.radComboBoxArchive.SelectedIndexChanged += new System.EventHandler(this.radComboBoxArchive_SelectedIndexChanged);
            // 
            // archiveBindingSource
            // 
            this.archiveBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Archive);
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(10, 18);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(49, 16);
            this.radLabel1.TabIndex = 10;
            this.radLabel1.Text = "Archive:";
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.radComboBoxArchive);
            this.radPanel1.Controls.Add(this.radLabel1);
            this.radPanel1.Controls.Add(this.label2);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.radPanel1.Location = new System.Drawing.Point(0, 33);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(873, 73);
            this.radPanel1.TabIndex = 11;
            // 
            // radPanel2
            // 
            this.radPanel2.Controls.Add(this.btnAdd);
            this.radPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radPanel2.Location = new System.Drawing.Point(0, 506);
            this.radPanel2.Name = "radPanel2";
            this.radPanel2.Size = new System.Drawing.Size(873, 42);
            this.radPanel2.TabIndex = 12;
            // 
            // documentBindingSource
            // 
            this.documentBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceDocument.Document);
            // 
            // UcFileUploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gwAttributes);
            this.Controls.Add(this.radPanel2);
            this.Controls.Add(this.radPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "UcFileUploader";
            this.Size = new System.Drawing.Size(873, 548);
            this.Load += new System.EventHandler(this.UcFileUploader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gwAttributes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.attributeBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxArchive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.archiveBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).EndInit();
            this.radPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnAdd;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.UI.RadGridView gwAttributes;
        private System.Windows.Forms.BindingSource attributeBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private Telerik.WinControls.UI.RadComboBox radComboBoxArchive;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.BindingSource archiveBindingSource;
    }
}
