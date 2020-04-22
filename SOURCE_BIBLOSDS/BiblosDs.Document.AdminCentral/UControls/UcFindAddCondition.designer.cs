namespace BiblosDs.Document.AdminCentral.UControls
{
    partial class UcFindAddCondition
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
            this.radComboBoxCondition = new Telerik.WinControls.UI.RadComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.radComboBoxOperator = new Telerik.WinControls.UI.RadComboBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCancell = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxCondition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxOperator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.SuspendLayout();
            // 
            // radComboBoxCondition
            // 
            this.radComboBoxCondition.FormatString = "{0}";
            this.radComboBoxCondition.Location = new System.Drawing.Point(20, 90);
            this.radComboBoxCondition.Name = "radComboBoxCondition";
            // 
            // 
            // 
            this.radComboBoxCondition.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radComboBoxCondition.Size = new System.Drawing.Size(245, 20);
            this.radComboBoxCondition.TabIndex = 32;
            this.radComboBoxCondition.Text = "-Select one-";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(456, 44);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(405, 20);
            this.txtValue.TabIndex = 31;
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.Location = new System.Drawing.Point(456, 24);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(38, 14);
            this.radLabel3.TabIndex = 29;
            this.radLabel3.Text = "Value:";
            this.radLabel3.TextWrap = true;
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(20, 24);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(39, 14);
            this.radLabel4.TabIndex = 30;
            this.radLabel4.Text = "Name:";
            this.radLabel4.TextWrap = true;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(20, 44);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(178, 20);
            this.txtName.TabIndex = 25;
            // 
            // radComboBoxOperator
            // 
            this.radComboBoxOperator.FormatString = "{0}";
            this.radComboBoxOperator.Location = new System.Drawing.Point(204, 44);
            this.radComboBoxOperator.Name = "radComboBoxOperator";
            // 
            // 
            // 
            this.radComboBoxOperator.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radComboBoxOperator.Size = new System.Drawing.Size(245, 20);
            this.radComboBoxOperator.TabIndex = 26;
            this.radComboBoxOperator.Text = "-Select one-";
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(204, 24);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(54, 14);
            this.radLabel2.TabIndex = 27;
            this.radLabel2.Text = "Operator:";
            this.radLabel2.TextWrap = true;
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(20, 70);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(58, 14);
            this.radLabel1.TabIndex = 25;
            this.radLabel1.Text = "Condition:";
            this.radLabel1.TextWrap = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 126);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 33;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancell
            // 
            this.btnCancell.Location = new System.Drawing.Point(101, 126);
            this.btnCancell.Name = "btnCancell";
            this.btnCancell.Size = new System.Drawing.Size(75, 23);
            this.btnCancell.TabIndex = 34;
            this.btnCancell.Text = "Cancel";
            this.btnCancell.UseVisualStyleBackColor = true;
            this.btnCancell.Click += new System.EventHandler(this.btnCancell_Click);
            // 
            // UcFindAddCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 162);
            this.Controls.Add(this.btnCancell);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radComboBoxCondition);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.radComboBoxOperator);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.Name = "UcFindAddCondition";
            this.Text = "Add condition";
            this.Load += new System.EventHandler(this.UcFindAddCondition_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxCondition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radComboBoxOperator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadComboBox radComboBoxCondition;
        private System.Windows.Forms.TextBox txtValue;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private System.Windows.Forms.TextBox txtName;
        private Telerik.WinControls.UI.RadComboBox radComboBoxOperator;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCancell;
    }
}
