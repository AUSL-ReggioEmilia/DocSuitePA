namespace BiblosDs.Document.AdminCentral.UControls.AlignmentWizard
{
    partial class Step1
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
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.btnVerifySource = new System.Windows.Forms.Button();
            this.txtUrlSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpDest = new System.Windows.Forms.GroupBox();
            this.btnVerifyDest = new System.Windows.Forms.Button();
            this.txtUrlDest = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlContent.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpDest.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.grpDest);
            this.pnlContent.Controls.Add(this.grpSource);
            this.pnlContent.Size = new System.Drawing.Size(632, 361);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.label3);
            this.grpSource.Controls.Add(this.btnVerifySource);
            this.grpSource.Controls.Add(this.txtUrlSource);
            this.grpSource.Controls.Add(this.label1);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 0);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(615, 186);
            this.grpSource.TabIndex = 0;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Indirizzo Sorgente";
            // 
            // btnVerifySource
            // 
            this.btnVerifySource.Location = new System.Drawing.Point(253, 114);
            this.btnVerifySource.Name = "btnVerifySource";
            this.btnVerifySource.Size = new System.Drawing.Size(112, 33);
            this.btnVerifySource.TabIndex = 2;
            this.btnVerifySource.Text = "Verifica";
            this.btnVerifySource.UseVisualStyleBackColor = true;
            this.btnVerifySource.Click += new System.EventHandler(this.btnVerifySource_Click);
            // 
            // txtUrlSource
            // 
            this.txtUrlSource.Location = new System.Drawing.Point(45, 54);
            this.txtUrlSource.Name = "txtUrlSource";
            this.txtUrlSource.Size = new System.Drawing.Size(581, 20);
            this.txtUrlSource.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL:";
            // 
            // grpDest
            // 
            this.grpDest.Controls.Add(this.label4);
            this.grpDest.Controls.Add(this.btnVerifyDest);
            this.grpDest.Controls.Add(this.txtUrlDest);
            this.grpDest.Controls.Add(this.label2);
            this.grpDest.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDest.Location = new System.Drawing.Point(0, 186);
            this.grpDest.Name = "grpDest";
            this.grpDest.Size = new System.Drawing.Size(615, 186);
            this.grpDest.TabIndex = 1;
            this.grpDest.TabStop = false;
            this.grpDest.Text = "Indirizzo Destinazione";
            // 
            // btnVerifyDest
            // 
            this.btnVerifyDest.Location = new System.Drawing.Point(253, 114);
            this.btnVerifyDest.Name = "btnVerifyDest";
            this.btnVerifyDest.Size = new System.Drawing.Size(112, 33);
            this.btnVerifyDest.TabIndex = 2;
            this.btnVerifyDest.Text = "Verifica";
            this.btnVerifyDest.UseVisualStyleBackColor = true;
            this.btnVerifyDest.Click += new System.EventHandler(this.btnVerifyDest_Click);
            // 
            // txtUrlDest
            // 
            this.txtUrlDest.Location = new System.Drawing.Point(45, 54);
            this.txtUrlDest.Name = "txtUrlDest";
            this.txtUrlDest.Size = new System.Drawing.Size(581, 20);
            this.txtUrlDest.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "URL:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "URL esempio: http://localhost:1526/";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(182, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "URL esempio: http://localhost:1600/";
            // 
            // Step1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "Step1";
            this.pnlContent.ResumeLayout(false);
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpDest.ResumeLayout(false);
            this.grpDest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.TextBox txtUrlSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpDest;
        private System.Windows.Forms.Button btnVerifyDest;
        private System.Windows.Forms.TextBox txtUrlDest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnVerifySource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

    }
}
