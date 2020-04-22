namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    partial class Step2
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
            this.grpArchives = new System.Windows.Forms.GroupBox();
            this.flowArchivi = new System.Windows.Forms.FlowLayoutPanel();
            this.splitArchivi = new System.Windows.Forms.SplitContainer();
            this.cbArchiviSorgente = new System.Windows.Forms.ComboBox();
            this.cbArchiviDestinazione = new System.Windows.Forms.ComboBox();
            this.pnlLoad = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.grpAttributi = new System.Windows.Forms.GroupBox();
            this.gridAttributi = new System.Windows.Forms.DataGridView();
            this.colSorgente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNomeSorgente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDestinazione = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.pnlContent.SuspendLayout();
            this.grpArchives.SuspendLayout();
            this.flowArchivi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitArchivi)).BeginInit();
            this.splitArchivi.Panel1.SuspendLayout();
            this.splitArchivi.Panel2.SuspendLayout();
            this.splitArchivi.SuspendLayout();
            this.pnlLoad.SuspendLayout();
            this.grpAttributi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAttributi)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.grpAttributi);
            this.pnlContent.Controls.Add(this.grpArchives);
            this.pnlContent.Size = new System.Drawing.Size(632, 361);
            // 
            // grpArchives
            // 
            this.grpArchives.AutoSize = true;
            this.grpArchives.Controls.Add(this.flowArchivi);
            this.grpArchives.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpArchives.Location = new System.Drawing.Point(0, 0);
            this.grpArchives.Name = "grpArchives";
            this.grpArchives.Size = new System.Drawing.Size(632, 85);
            this.grpArchives.TabIndex = 0;
            this.grpArchives.TabStop = false;
            this.grpArchives.Text = "Selezione archivi";
            // 
            // flowArchivi
            // 
            this.flowArchivi.AutoSize = true;
            this.flowArchivi.Controls.Add(this.splitArchivi);
            this.flowArchivi.Controls.Add(this.pnlLoad);
            this.flowArchivi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowArchivi.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowArchivi.Location = new System.Drawing.Point(3, 16);
            this.flowArchivi.Name = "flowArchivi";
            this.flowArchivi.Size = new System.Drawing.Size(626, 66);
            this.flowArchivi.TabIndex = 0;
            // 
            // splitArchivi
            // 
            this.splitArchivi.Location = new System.Drawing.Point(3, 3);
            this.splitArchivi.Name = "splitArchivi";
            // 
            // splitArchivi.Panel1
            // 
            this.splitArchivi.Panel1.Controls.Add(this.cbArchiviSorgente);
            // 
            // splitArchivi.Panel2
            // 
            this.splitArchivi.Panel2.Controls.Add(this.cbArchiviDestinazione);
            this.splitArchivi.Size = new System.Drawing.Size(626, 25);
            this.splitArchivi.SplitterDistance = 306;
            this.splitArchivi.TabIndex = 1;
            // 
            // cbArchiviSorgente
            // 
            this.cbArchiviSorgente.DisplayMember = "Name";
            this.cbArchiviSorgente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbArchiviSorgente.FormattingEnabled = true;
            this.cbArchiviSorgente.Location = new System.Drawing.Point(0, 0);
            this.cbArchiviSorgente.Name = "cbArchiviSorgente";
            this.cbArchiviSorgente.Size = new System.Drawing.Size(306, 21);
            this.cbArchiviSorgente.TabIndex = 1;
            // 
            // cbArchiviDestinazione
            // 
            this.cbArchiviDestinazione.DisplayMember = "Name";
            this.cbArchiviDestinazione.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbArchiviDestinazione.FormattingEnabled = true;
            this.cbArchiviDestinazione.Location = new System.Drawing.Point(0, 0);
            this.cbArchiviDestinazione.Name = "cbArchiviDestinazione";
            this.cbArchiviDestinazione.Size = new System.Drawing.Size(316, 21);
            this.cbArchiviDestinazione.TabIndex = 0;
            // 
            // pnlLoad
            // 
            this.pnlLoad.Controls.Add(this.btnLoad);
            this.pnlLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLoad.Location = new System.Drawing.Point(3, 34);
            this.pnlLoad.Name = "pnlLoad";
            this.pnlLoad.Size = new System.Drawing.Size(626, 29);
            this.pnlLoad.TabIndex = 2;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(237, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(151, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Carica / Ricarica";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // grpAttributi
            // 
            this.grpAttributi.Controls.Add(this.gridAttributi);
            this.grpAttributi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAttributi.Location = new System.Drawing.Point(0, 85);
            this.grpAttributi.Name = "grpAttributi";
            this.grpAttributi.Size = new System.Drawing.Size(632, 276);
            this.grpAttributi.TabIndex = 1;
            this.grpAttributi.TabStop = false;
            this.grpAttributi.Text = "Elenco attributi";
            // 
            // gridAttributi
            // 
            this.gridAttributi.AllowUserToAddRows = false;
            this.gridAttributi.AllowUserToDeleteRows = false;
            this.gridAttributi.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAttributi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAttributi.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSorgente,
            this.colNomeSorgente,
            this.colDestinazione});
            this.gridAttributi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAttributi.Location = new System.Drawing.Point(3, 16);
            this.gridAttributi.Name = "gridAttributi";
            this.gridAttributi.Size = new System.Drawing.Size(626, 257);
            this.gridAttributi.TabIndex = 0;
            // 
            // colSorgente
            // 
            this.colSorgente.HeaderText = "AttributoSorgente";
            this.colSorgente.Name = "colSorgente";
            this.colSorgente.ReadOnly = true;
            this.colSorgente.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSorgente.Visible = false;
            // 
            // colNomeSorgente
            // 
            this.colNomeSorgente.HeaderText = "Sorgente";
            this.colNomeSorgente.Name = "colNomeSorgente";
            this.colNomeSorgente.ReadOnly = true;
            // 
            // colDestinazione
            // 
            this.colDestinazione.DataPropertyName = "Name";
            this.colDestinazione.HeaderText = "Destinazione";
            this.colDestinazione.Name = "colDestinazione";
            this.colDestinazione.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Step2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "Step2";
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.grpArchives.ResumeLayout(false);
            this.grpArchives.PerformLayout();
            this.flowArchivi.ResumeLayout(false);
            this.splitArchivi.Panel1.ResumeLayout(false);
            this.splitArchivi.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitArchivi)).EndInit();
            this.splitArchivi.ResumeLayout(false);
            this.pnlLoad.ResumeLayout(false);
            this.grpAttributi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAttributi)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpArchives;
        private System.Windows.Forms.FlowLayoutPanel flowArchivi;
        private System.Windows.Forms.SplitContainer splitArchivi;
        private System.Windows.Forms.ComboBox cbArchiviSorgente;
        private System.Windows.Forms.ComboBox cbArchiviDestinazione;
        private System.Windows.Forms.Panel pnlLoad;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox grpAttributi;
        private System.Windows.Forms.DataGridView gridAttributi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSorgente;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNomeSorgente;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDestinazione;
    }
}
