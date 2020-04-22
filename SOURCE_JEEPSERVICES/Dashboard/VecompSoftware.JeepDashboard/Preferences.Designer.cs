namespace VecompSoftware.JeepDashboard
{
    partial class Preferences
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
            this.components = new System.ComponentModel.Container();
            this.lblJeepServiceExePath = new System.Windows.Forms.Label();
            this.JeepServiceExePath = new System.Windows.Forms.TextBox();
            this.lblJeepServiceName = new System.Windows.Forms.Label();
            this.JeepServiceName = new System.Windows.Forms.TextBox();
            this.lblLog4NetConfigPath = new System.Windows.Forms.Label();
            this.Log4NetConfigPath = new System.Windows.Forms.TextBox();
            this.Save = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FindExePath = new System.Windows.Forms.Button();
            this.FindLog4NetPath = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblJeepServiceConfigPath = new System.Windows.Forms.Label();
            this.JeepServiceConfigPath = new System.Windows.Forms.TextBox();
            this.FindConfigPath = new System.Windows.Forms.Button();
            this.lblLiveUpdateExePath = new System.Windows.Forms.Label();
            this.LiveUpdateExePath = new System.Windows.Forms.TextBox();
            this.lblLiveUpdateName = new System.Windows.Forms.Label();
            this.LiveUpdateServiceName = new System.Windows.Forms.TextBox();
            this.FindLiveUpdatePath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblJeepServiceExePath
            // 
            this.lblJeepServiceExePath.AutoSize = true;
            this.lblJeepServiceExePath.Location = new System.Drawing.Point(28, 41);
            this.lblJeepServiceExePath.Name = "lblJeepServiceExePath";
            this.lblJeepServiceExePath.Size = new System.Drawing.Size(115, 13);
            this.lblJeepServiceExePath.TabIndex = 0;
            this.lblJeepServiceExePath.Text = "Jeep Service Exe Path";
            // 
            // JeepServiceExePath
            // 
            this.JeepServiceExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JeepServiceExePath.Location = new System.Drawing.Point(149, 38);
            this.JeepServiceExePath.Name = "JeepServiceExePath";
            this.JeepServiceExePath.Size = new System.Drawing.Size(530, 20);
            this.JeepServiceExePath.TabIndex = 1;
            // 
            // lblJeepServiceName
            // 
            this.lblJeepServiceName.AutoSize = true;
            this.lblJeepServiceName.Location = new System.Drawing.Point(43, 15);
            this.lblJeepServiceName.Name = "lblJeepServiceName";
            this.lblJeepServiceName.Size = new System.Drawing.Size(100, 13);
            this.lblJeepServiceName.TabIndex = 0;
            this.lblJeepServiceName.Text = "Jeep Service Name";
            // 
            // JeepServiceName
            // 
            this.JeepServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JeepServiceName.Location = new System.Drawing.Point(149, 12);
            this.JeepServiceName.Name = "JeepServiceName";
            this.JeepServiceName.Size = new System.Drawing.Size(530, 20);
            this.JeepServiceName.TabIndex = 1;
            this.JeepServiceName.TextChanged += new System.EventHandler(this.ServiceName_TextChanged);
            // 
            // lblLog4NetConfigPath
            // 
            this.lblLog4NetConfigPath.AutoSize = true;
            this.lblLog4NetConfigPath.Location = new System.Drawing.Point(37, 93);
            this.lblLog4NetConfigPath.Name = "lblLog4NetConfigPath";
            this.lblLog4NetConfigPath.Size = new System.Drawing.Size(106, 13);
            this.lblLog4NetConfigPath.TabIndex = 0;
            this.lblLog4NetConfigPath.Text = "Log4Net Config Path";
            // 
            // Log4NetConfigPath
            // 
            this.Log4NetConfigPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Log4NetConfigPath.Location = new System.Drawing.Point(149, 90);
            this.Log4NetConfigPath.Name = "Log4NetConfigPath";
            this.Log4NetConfigPath.Size = new System.Drawing.Size(530, 20);
            this.Log4NetConfigPath.TabIndex = 1;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(68, 172);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 2;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // FindExePath
            // 
            this.FindExePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindExePath.Location = new System.Drawing.Point(685, 36);
            this.FindExePath.Name = "FindExePath";
            this.FindExePath.Size = new System.Drawing.Size(29, 23);
            this.FindExePath.TabIndex = 3;
            this.FindExePath.Text = "...";
            this.FindExePath.UseVisualStyleBackColor = true;
            this.FindExePath.Click += new System.EventHandler(this.FindExecutable_Click);
            // 
            // FindLog4NetPath
            // 
            this.FindLog4NetPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindLog4NetPath.Location = new System.Drawing.Point(685, 88);
            this.FindLog4NetPath.Name = "FindLog4NetPath";
            this.FindLog4NetPath.Size = new System.Drawing.Size(29, 23);
            this.FindLog4NetPath.TabIndex = 3;
            this.FindLog4NetPath.Text = "...";
            this.FindLog4NetPath.UseVisualStyleBackColor = true;
            this.FindLog4NetPath.Click += new System.EventHandler(this.Log4Net_Click);
            // 
            // lblJeepServiceConfigPath
            // 
            this.lblJeepServiceConfigPath.AutoSize = true;
            this.lblJeepServiceConfigPath.Location = new System.Drawing.Point(16, 67);
            this.lblJeepServiceConfigPath.Name = "lblJeepServiceConfigPath";
            this.lblJeepServiceConfigPath.Size = new System.Drawing.Size(127, 13);
            this.lblJeepServiceConfigPath.TabIndex = 0;
            this.lblJeepServiceConfigPath.Text = "Jeep Service Config Path";
            // 
            // JeepServiceConfigPath
            // 
            this.JeepServiceConfigPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JeepServiceConfigPath.Location = new System.Drawing.Point(149, 64);
            this.JeepServiceConfigPath.Name = "JeepServiceConfigPath";
            this.JeepServiceConfigPath.Size = new System.Drawing.Size(530, 20);
            this.JeepServiceConfigPath.TabIndex = 1;
            // 
            // FindConfigPath
            // 
            this.FindConfigPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindConfigPath.Location = new System.Drawing.Point(685, 62);
            this.FindConfigPath.Name = "FindConfigPath";
            this.FindConfigPath.Size = new System.Drawing.Size(29, 23);
            this.FindConfigPath.TabIndex = 3;
            this.FindConfigPath.Text = "...";
            this.FindConfigPath.UseVisualStyleBackColor = true;
            this.FindConfigPath.Click += new System.EventHandler(this.FindConfigPath_Click);
            // 
            // lblLiveUpdateExePath
            // 
            this.lblLiveUpdateExePath.AutoSize = true;
            this.lblLiveUpdateExePath.Location = new System.Drawing.Point(32, 145);
            this.lblLiveUpdateExePath.Name = "lblLiveUpdateExePath";
            this.lblLiveUpdateExePath.Size = new System.Drawing.Size(111, 13);
            this.lblLiveUpdateExePath.TabIndex = 0;
            this.lblLiveUpdateExePath.Text = "Live Update Exe Path";
            // 
            // LiveUpdateExePath
            // 
            this.LiveUpdateExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LiveUpdateExePath.Location = new System.Drawing.Point(149, 142);
            this.LiveUpdateExePath.Name = "LiveUpdateExePath";
            this.LiveUpdateExePath.Size = new System.Drawing.Size(530, 20);
            this.LiveUpdateExePath.TabIndex = 1;
            // 
            // lblLiveUpdateName
            // 
            this.lblLiveUpdateName.AutoSize = true;
            this.lblLiveUpdateName.Location = new System.Drawing.Point(11, 119);
            this.lblLiveUpdateName.Name = "lblLiveUpdateName";
            this.lblLiveUpdateName.Size = new System.Drawing.Size(135, 13);
            this.lblLiveUpdateName.TabIndex = 0;
            this.lblLiveUpdateName.Text = "Live Update Service Name";
            // 
            // LiveUpdateServiceName
            // 
            this.LiveUpdateServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LiveUpdateServiceName.Location = new System.Drawing.Point(149, 116);
            this.LiveUpdateServiceName.Name = "LiveUpdateServiceName";
            this.LiveUpdateServiceName.Size = new System.Drawing.Size(530, 20);
            this.LiveUpdateServiceName.TabIndex = 1;
            this.LiveUpdateServiceName.TextChanged += new System.EventHandler(this.ServiceName_TextChanged);
            // 
            // FindLiveUpdatePath
            // 
            this.FindLiveUpdatePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindLiveUpdatePath.Location = new System.Drawing.Point(685, 140);
            this.FindLiveUpdatePath.Name = "FindLiveUpdatePath";
            this.FindLiveUpdatePath.Size = new System.Drawing.Size(29, 23);
            this.FindLiveUpdatePath.TabIndex = 3;
            this.FindLiveUpdatePath.Text = "...";
            this.FindLiveUpdatePath.UseVisualStyleBackColor = true;
            this.FindLiveUpdatePath.Click += new System.EventHandler(this.FindLiveUpdatePath_Click);
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 206);
            this.Controls.Add(this.FindLog4NetPath);
            this.Controls.Add(this.FindConfigPath);
            this.Controls.Add(this.FindLiveUpdatePath);
            this.Controls.Add(this.FindExePath);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Log4NetConfigPath);
            this.Controls.Add(this.lblLog4NetConfigPath);
            this.Controls.Add(this.LiveUpdateServiceName);
            this.Controls.Add(this.lblLiveUpdateName);
            this.Controls.Add(this.JeepServiceName);
            this.Controls.Add(this.lblJeepServiceName);
            this.Controls.Add(this.JeepServiceConfigPath);
            this.Controls.Add(this.LiveUpdateExePath);
            this.Controls.Add(this.lblJeepServiceConfigPath);
            this.Controls.Add(this.lblLiveUpdateExePath);
            this.Controls.Add(this.JeepServiceExePath);
            this.Controls.Add(this.lblJeepServiceExePath);
            this.Name = "Preferences";
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblJeepServiceExePath;
        private System.Windows.Forms.TextBox JeepServiceExePath;
        private System.Windows.Forms.Label lblJeepServiceName;
        private System.Windows.Forms.TextBox JeepServiceName;
        private System.Windows.Forms.Label lblLog4NetConfigPath;
        private System.Windows.Forms.TextBox Log4NetConfigPath;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button FindExePath;
        private System.Windows.Forms.Button FindLog4NetPath;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lblJeepServiceConfigPath;
        private System.Windows.Forms.TextBox JeepServiceConfigPath;
        private System.Windows.Forms.Button FindConfigPath;
        private System.Windows.Forms.Label lblLiveUpdateExePath;
        private System.Windows.Forms.TextBox LiveUpdateExePath;
        private System.Windows.Forms.Label lblLiveUpdateName;
        private System.Windows.Forms.TextBox LiveUpdateServiceName;
        private System.Windows.Forms.Button FindLiveUpdatePath;
    }
}