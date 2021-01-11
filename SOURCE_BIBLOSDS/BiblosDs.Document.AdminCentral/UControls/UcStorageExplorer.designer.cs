namespace BiblosDs.Document.AdminCentral.UControls
{
    partial class UcStorageExplorer
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
            this.radComboBoxStorage = new Telerik.WinControls.UI.RadComboBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.radComboBoxStorageArea = new Telerik.WinControls.UI.RadComboBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxStorage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxStorageArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            this.SuspendLayout();
            // 
            // radComboBoxStorage
            // 
            this.radComboBoxStorage.FormatString = "{0}";
            this.radComboBoxStorage.Location = new System.Drawing.Point(105, 21);
            this.radComboBoxStorage.Name = "radComboBoxStorage";
            // 
            // 
            // 
            this.radComboBoxStorage.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radComboBoxStorage.Size = new System.Drawing.Size(245, 20);
            this.radComboBoxStorage.TabIndex = 11;
            this.radComboBoxStorage.Text = "-Select one-";
            this.radComboBoxStorage.SelectedIndexChanged += new System.EventHandler(this.radComboBoxStorage_SelectedIndexChanged);
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(50, 27);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(49, 14);
            this.radLabel1.TabIndex = 12;
            this.radLabel1.Text = "Storage:";
            this.radLabel1.TextWrap = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(105, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Seleziona";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(187, 75);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "Annulla";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // radComboBoxStorageArea
            // 
            this.radComboBoxStorageArea.FormatString = "{0}";
            this.radComboBoxStorageArea.Location = new System.Drawing.Point(105, 50);
            this.radComboBoxStorageArea.Name = "radComboBoxStorageArea";
            // 
            // 
            // 
            this.radComboBoxStorageArea.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radComboBoxStorageArea.Size = new System.Drawing.Size(245, 20);
            this.radComboBoxStorageArea.TabIndex = 13;
            this.radComboBoxStorageArea.Text = "-Select one-";
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(23, 56);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(76, 14);
            this.radLabel2.TabIndex = 14;
            this.radLabel2.Text = "Storage area:";
            this.radLabel2.TextWrap = true;
            // 
            // UcStorageExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 152);
            this.Controls.Add(this.radComboBoxStorageArea);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radComboBoxStorage);
            this.Controls.Add(this.radLabel1);
            this.Name = "UcStorageExplorer";
            this.Text = "Storage";
            this.Load += new System.EventHandler(this.UcStorageExplorer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxStorage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxStorageArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadComboBox radComboBoxStorage;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private Telerik.WinControls.UI.RadComboBox radComboBoxStorageArea;
        private Telerik.WinControls.UI.RadLabel radLabel2;
    }
}