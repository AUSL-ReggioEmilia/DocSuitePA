namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    partial class Step3
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grpRiepilogo = new System.Windows.Forms.GroupBox();
            this.tabRiepilogo = new System.Windows.Forms.TabControl();
            this.tabAttrs = new System.Windows.Forms.TabPage();
            this.gridAttrs = new System.Windows.Forms.DataGridView();
            this.colSorgente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNomeSorgente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDestinazione = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNomeDestinazione = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabDocs = new System.Windows.Forms.TabPage();
            this.gridDocs = new System.Windows.Forms.DataGridView();
            this.colIdDocument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHash = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIdBiblos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlContent.SuspendLayout();
            this.grpRiepilogo.SuspendLayout();
            this.tabRiepilogo.SuspendLayout();
            this.tabAttrs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAttrs)).BeginInit();
            this.tabDocs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDocs)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.grpRiepilogo);
            this.pnlContent.Size = new System.Drawing.Size(632, 361);
            // 
            // grpRiepilogo
            // 
            this.grpRiepilogo.Controls.Add(this.tabRiepilogo);
            this.grpRiepilogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpRiepilogo.Location = new System.Drawing.Point(0, 0);
            this.grpRiepilogo.Name = "grpRiepilogo";
            this.grpRiepilogo.Size = new System.Drawing.Size(632, 361);
            this.grpRiepilogo.TabIndex = 0;
            this.grpRiepilogo.TabStop = false;
            this.grpRiepilogo.Text = "Riepilogo";
            // 
            // tabRiepilogo
            // 
            this.tabRiepilogo.Controls.Add(this.tabAttrs);
            this.tabRiepilogo.Controls.Add(this.tabDocs);
            this.tabRiepilogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabRiepilogo.Location = new System.Drawing.Point(3, 16);
            this.tabRiepilogo.Name = "tabRiepilogo";
            this.tabRiepilogo.SelectedIndex = 0;
            this.tabRiepilogo.Size = new System.Drawing.Size(626, 342);
            this.tabRiepilogo.TabIndex = 0;
            // 
            // tabAttrs
            // 
            this.tabAttrs.Controls.Add(this.gridAttrs);
            this.tabAttrs.Location = new System.Drawing.Point(4, 22);
            this.tabAttrs.Name = "tabAttrs";
            this.tabAttrs.Padding = new System.Windows.Forms.Padding(3);
            this.tabAttrs.Size = new System.Drawing.Size(618, 316);
            this.tabAttrs.TabIndex = 1;
            this.tabAttrs.Text = "Attributi";
            this.tabAttrs.UseVisualStyleBackColor = true;
            // 
            // gridAttrs
            // 
            this.gridAttrs.AllowUserToAddRows = false;
            this.gridAttrs.AllowUserToDeleteRows = false;
            this.gridAttrs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAttrs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAttrs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSorgente,
            this.colNomeSorgente,
            this.colDestinazione,
            this.colNomeDestinazione});
            this.gridAttrs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAttrs.Location = new System.Drawing.Point(3, 3);
            this.gridAttrs.MultiSelect = false;
            this.gridAttrs.Name = "gridAttrs";
            this.gridAttrs.ReadOnly = true;
            this.gridAttrs.Size = new System.Drawing.Size(612, 310);
            this.gridAttrs.TabIndex = 0;
            // 
            // colSorgente
            // 
            this.colSorgente.HeaderText = "SrcAttr";
            this.colSorgente.Name = "colSorgente";
            this.colSorgente.ReadOnly = true;
            this.colSorgente.Visible = false;
            // 
            // colNomeSorgente
            // 
            this.colNomeSorgente.HeaderText = "Attributo sorgente";
            this.colNomeSorgente.Name = "colNomeSorgente";
            this.colNomeSorgente.ReadOnly = true;
            // 
            // colDestinazione
            // 
            this.colDestinazione.HeaderText = "DestAttr";
            this.colDestinazione.Name = "colDestinazione";
            this.colDestinazione.ReadOnly = true;
            this.colDestinazione.Visible = false;
            // 
            // colNomeDestinazione
            // 
            this.colNomeDestinazione.HeaderText = "Attributo destinazione";
            this.colNomeDestinazione.Name = "colNomeDestinazione";
            this.colNomeDestinazione.ReadOnly = true;
            // 
            // tabDocs
            // 
            this.tabDocs.Controls.Add(this.gridDocs);
            this.tabDocs.Location = new System.Drawing.Point(4, 22);
            this.tabDocs.Name = "tabDocs";
            this.tabDocs.Padding = new System.Windows.Forms.Padding(3);
            this.tabDocs.Size = new System.Drawing.Size(618, 316);
            this.tabDocs.TabIndex = 0;
            this.tabDocs.Text = "Documenti";
            this.tabDocs.UseVisualStyleBackColor = true;
            // 
            // gridDocs
            // 
            this.gridDocs.AllowUserToAddRows = false;
            this.gridDocs.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.BurlyWood;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkRed;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DarkOliveGreen;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Gold;
            this.gridDocs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridDocs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridDocs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridDocs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIdDocument,
            this.colSelect,
            this.colName,
            this.colData,
            this.colHash,
            this.colIdBiblos});
            this.gridDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDocs.Location = new System.Drawing.Point(3, 3);
            this.gridDocs.MultiSelect = false;
            this.gridDocs.Name = "gridDocs";
            this.gridDocs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDocs.Size = new System.Drawing.Size(612, 310);
            this.gridDocs.TabIndex = 9;
            // 
            // colIdDocument
            // 
            this.colIdDocument.DataPropertyName = "IdDocument";
            this.colIdDocument.HeaderText = "IdDocument";
            this.colIdDocument.Name = "colIdDocument";
            this.colIdDocument.ReadOnly = true;
            this.colIdDocument.Visible = false;
            // 
            // colSelect
            // 
            this.colSelect.DataPropertyName = "IsVisible";
            this.colSelect.FalseValue = "false";
            this.colSelect.HeaderText = "Seleziona";
            this.colSelect.Name = "colSelect";
            this.colSelect.TrueValue = "true";
            // 
            // colName
            // 
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Nome";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colData
            // 
            this.colData.DataPropertyName = "DateMain";
            this.colData.HeaderText = "Data";
            this.colData.Name = "colData";
            this.colData.ReadOnly = true;
            // 
            // colHash
            // 
            this.colHash.DataPropertyName = "DocumentHash";
            this.colHash.HeaderText = "Hash";
            this.colHash.Name = "colHash";
            this.colHash.ReadOnly = true;
            // 
            // colIdBiblos
            // 
            this.colIdBiblos.DataPropertyName = "IdBiblos";
            this.colIdBiblos.HeaderText = "Id Biblos";
            this.colIdBiblos.Name = "colIdBiblos";
            this.colIdBiblos.ReadOnly = true;
            // 
            // Step3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "Step3";
            this.pnlContent.ResumeLayout(false);
            this.grpRiepilogo.ResumeLayout(false);
            this.tabRiepilogo.ResumeLayout(false);
            this.tabAttrs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAttrs)).EndInit();
            this.tabDocs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDocs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpRiepilogo;
        private System.Windows.Forms.TabControl tabRiepilogo;
        private System.Windows.Forms.TabPage tabDocs;
        private System.Windows.Forms.TabPage tabAttrs;
        private System.Windows.Forms.DataGridView gridAttrs;
        private System.Windows.Forms.DataGridView gridDocs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSorgente;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNomeSorgente;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDestinazione;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNomeDestinazione;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIdDocument;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colData;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHash;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIdBiblos;
    }
}
