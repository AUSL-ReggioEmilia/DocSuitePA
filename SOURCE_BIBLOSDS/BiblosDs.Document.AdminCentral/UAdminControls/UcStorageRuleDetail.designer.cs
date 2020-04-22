namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcStorageRuleDetail
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.radPanelTitle = new Telerik.WinControls.UI.RadPanel();
            this.btUpdate = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.cbArchive = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFormat = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.documentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOrder = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAttribute = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbRuleOperators = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // radPanelTitle
            // 
            this.radPanelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanelTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanelTitle.Location = new System.Drawing.Point(6, 3);
            this.radPanelTitle.Name = "radPanelTitle";
            this.radPanelTitle.Size = new System.Drawing.Size(782, 46);
            this.radPanelTitle.TabIndex = 10;
            this.radPanelTitle.Text = "Add new Rule";
            // 
            // btUpdate
            // 
            this.btUpdate.AutoSize = true;
            this.btUpdate.Location = new System.Drawing.Point(572, 72);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(104, 23);
            this.btUpdate.TabIndex = 6;
            this.btUpdate.Text = "Update";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.storageUpdate_Click);
            // 
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(572, 98);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 7;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            // 
            // cbArchive
            // 
            this.cbArchive.DisplayMember = "Name";
            this.cbArchive.FormattingEnabled = true;
            this.cbArchive.Location = new System.Drawing.Point(169, 72);
            this.cbArchive.Name = "cbArchive";
            this.cbArchive.Size = new System.Drawing.Size(375, 21);
            this.cbArchive.TabIndex = 0;
            this.cbArchive.ValueMember = "IdArchive";
            this.cbArchive.SelectedIndexChanged += new System.EventHandler(this.cbArchive_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(109, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 84;
            this.label7.Text = "Archive:";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(170, 150);
            this.txtFilter.MaxLength = 255;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(375, 20);
            this.txtFilter.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(115, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 82;
            this.label6.Text = "Format:";
            // 
            // txtFormat
            // 
            this.txtFormat.Location = new System.Drawing.Point(170, 177);
            this.txtFormat.MaxLength = 255;
            this.txtFormat.Name = "txtFormat";
            this.txtFormat.Size = new System.Drawing.Size(374, 20);
            this.txtFormat.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(125, 153);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(39, 13);
            this.label15.TabIndex = 78;
            this.label15.Text = "Filter:";
            // 
            // documentBindingSource
            // 
            this.documentBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Storage);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(551, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 19);
            this.label4.TabIndex = 101;
            this.label4.Text = "*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(121, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 103;
            this.label2.Text = "Order:";
            // 
            // txtOrder
            // 
            this.txtOrder.Location = new System.Drawing.Point(169, 125);
            this.txtOrder.Name = "txtOrder";
            this.txtOrder.Size = new System.Drawing.Size(120, 20);
            this.txtOrder.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(551, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 19);
            this.label3.TabIndex = 104;
            this.label3.Text = "*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(104, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 81;
            this.label1.Text = "Attribute:";
            // 
            // cbAttribute
            // 
            this.cbAttribute.DisplayMember = "Name";
            this.cbAttribute.FormattingEnabled = true;
            this.cbAttribute.Location = new System.Drawing.Point(169, 100);
            this.cbAttribute.Name = "cbAttribute";
            this.cbAttribute.Size = new System.Drawing.Size(375, 21);
            this.cbAttribute.TabIndex = 1;
            this.cbAttribute.ValueMember = "IdAttribute";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(551, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(15, 19);
            this.label8.TabIndex = 108;
            this.label8.Text = "*";
            // 
            // cbRuleOperators
            // 
            this.cbRuleOperators.DisplayMember = "Descrizione";
            this.cbRuleOperators.FormattingEnabled = true;
            this.cbRuleOperators.Location = new System.Drawing.Point(169, 204);
            this.cbRuleOperators.Name = "cbRuleOperators";
            this.cbRuleOperators.Size = new System.Drawing.Size(375, 21);
            this.cbRuleOperators.TabIndex = 5;
            this.cbRuleOperators.ValueMember = "IdRuleOperator";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(103, 207);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 111;
            this.label5.Text = "Operator:";
            // 
            // UcStorageRuleDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cbRuleOperators);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cbAttribute);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOrder);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbArchive);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFormat);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.radPanelTitle);
            this.Name = "UcStorageRuleDetail";
            this.Size = new System.Drawing.Size(794, 474);
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Telerik.WinControls.UI.RadPanel radPanelTitle;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Button btCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.ComboBox cbArchive;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFormat;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtOrder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAttribute;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbRuleOperators;
        private System.Windows.Forms.Label label5;
    }
}
