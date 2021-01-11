namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class ServerAdmin
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
            this._pnl = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // _pnl
            // 
            this._pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnl.Location = new System.Drawing.Point(0, 0);
            this._pnl.Name = "_pnl";
            this._pnl.Size = new System.Drawing.Size(150, 150);
            this._pnl.TabIndex = 0;
            // 
            // ServerAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._pnl);
            this.Name = "ServerAdmin";
            this.Load += new System.EventHandler(this.ServerAdmin_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pnl;
    }
}
