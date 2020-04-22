using BiblosDs.Document.AdminCentral.UControls;

namespace BiblosDs.Document.AdminCentral.Forms
{
    partial class AttributeEdit
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
            this.ucAttributeEdit1 = new UcAttributeEdit();
            this.SuspendLayout();
            // 
            // ucAttributeEdit1
            // 
            this.ucAttributeEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucAttributeEdit1.Location = new System.Drawing.Point(0, 0);
            this.ucAttributeEdit1.Name = "ucAttributeEdit1";
            this.ucAttributeEdit1.Size = new System.Drawing.Size(640, 264);
            this.ucAttributeEdit1.TabIndex = 0;
            // 
            // AttributeEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 264);
            this.Controls.Add(this.ucAttributeEdit1);
            this.Name = "AttributeEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modifica di attributi";
            this.Load += new System.EventHandler(this.AttributeEdit_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UcAttributeEdit ucAttributeEdit1;
    }
}