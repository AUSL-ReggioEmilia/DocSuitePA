namespace VecompSoftware.JeepDashboard
{
    partial class ManageUpdates
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
            this.latestPackagesGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.latestPackagesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // latestPackagesGridView
            // 
            this.latestPackagesGridView.AllowUserToAddRows = false;
            this.latestPackagesGridView.AllowUserToDeleteRows = false;
            this.latestPackagesGridView.AllowUserToOrderColumns = true;
            this.latestPackagesGridView.AllowUserToResizeColumns = false;
            this.latestPackagesGridView.AllowUserToResizeRows = false;
            this.latestPackagesGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.latestPackagesGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.latestPackagesGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.latestPackagesGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.latestPackagesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.latestPackagesGridView.Location = new System.Drawing.Point(12, 12);
            this.latestPackagesGridView.Name = "latestPackagesGridView";
            this.latestPackagesGridView.RowHeadersVisible = false;
            this.latestPackagesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.latestPackagesGridView.Size = new System.Drawing.Size(570, 436);
            this.latestPackagesGridView.TabIndex = 0;
            // 
            // ManageUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 460);
            this.Controls.Add(this.latestPackagesGridView);
            this.Name = "ManageUpdates";
            this.Text = "Updates";
            ((System.ComponentModel.ISupportInitialize)(this.latestPackagesGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView latestPackagesGridView;
    }
}