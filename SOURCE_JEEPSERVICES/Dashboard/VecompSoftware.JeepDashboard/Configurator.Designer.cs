namespace VecompSoftware.JeepDashboard
{
    partial class Configurator
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Parameters = new System.Windows.Forms.DataGridView();
            this.chkEncrypt = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.OptionsPanel = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ConnectionStrings = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Clients = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.Parameters)).BeginInit();
            this.OptionsPanel.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConnectionStrings)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Clients)).BeginInit();
            this.SuspendLayout();
            // 
            // Parameters
            // 
            this.Parameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.Parameters.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Parameters.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Parameters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Parameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Parameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Parameters.Location = new System.Drawing.Point(3, 3);
            this.Parameters.MultiSelect = false;
            this.Parameters.Name = "Parameters";
            this.Parameters.Size = new System.Drawing.Size(947, 492);
            this.Parameters.TabIndex = 2;
            // 
            // chkEncrypt
            // 
            this.chkEncrypt.AutoSize = true;
            this.chkEncrypt.Location = new System.Drawing.Point(152, 7);
            this.chkEncrypt.Name = "chkEncrypt";
            this.chkEncrypt.Size = new System.Drawing.Size(130, 17);
            this.chkEncrypt.TabIndex = 5;
            this.chkEncrypt.Text = "Cifra la configurazione";
            this.chkEncrypt.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(134, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Salva configurazione";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSaveClick);
            // 
            // OptionsPanel
            // 
            this.OptionsPanel.Controls.Add(this.btnSave);
            this.OptionsPanel.Controls.Add(this.chkEncrypt);
            this.OptionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.OptionsPanel.Location = new System.Drawing.Point(0, 0);
            this.OptionsPanel.Name = "OptionsPanel";
            this.OptionsPanel.Size = new System.Drawing.Size(953, 30);
            this.OptionsPanel.TabIndex = 6;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 730);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(953, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(953, 700);
            this.panel1.TabIndex = 8;
            // 
            // ConnectionStrings
            // 
            this.ConnectionStrings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ConnectionStrings.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ConnectionStrings.BackgroundColor = System.Drawing.SystemColors.Control;
            this.ConnectionStrings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConnectionStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConnectionStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConnectionStrings.Location = new System.Drawing.Point(3, 501);
            this.ConnectionStrings.MultiSelect = false;
            this.ConnectionStrings.Name = "ConnectionStrings";
            this.ConnectionStrings.Size = new System.Drawing.Size(947, 98);
            this.ConnectionStrings.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.Clients, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ConnectionStrings, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Parameters, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.72425F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.27575F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(953, 700);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // Clients
            // 
            this.Clients.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.Clients.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Clients.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Clients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Clients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Clients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Clients.Location = new System.Drawing.Point(3, 605);
            this.Clients.MultiSelect = false;
            this.Clients.Name = "Clients";
            this.Clients.Size = new System.Drawing.Size(947, 92);
            this.Clients.TabIndex = 4;
            // 
            // Configurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 752);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.OptionsPanel);
            this.Name = "Configurator";
            this.Text = "Impostazione parametri Config";
            this.Load += new System.EventHandler(this.ParametersConfigGuiLoad);
            ((System.ComponentModel.ISupportInitialize)(this.Parameters)).EndInit();
            this.OptionsPanel.ResumeLayout(false);
            this.OptionsPanel.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConnectionStrings)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Clients)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Parameters;
        private System.Windows.Forms.CheckBox chkEncrypt;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel OptionsPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView ConnectionStrings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView Clients;
    }
}