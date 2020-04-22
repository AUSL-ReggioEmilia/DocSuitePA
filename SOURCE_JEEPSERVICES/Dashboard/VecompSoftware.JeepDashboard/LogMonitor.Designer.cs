namespace VecompSoftware.JeepDashboard
{
    partial class LogMonitor
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdrefresh = new System.Windows.Forms.Button();
            this.cmdPause = new System.Windows.Forms.Button();
            this.LogSelectorCombo = new System.Windows.Forms.ComboBox();
            this.logTimer = new System.Windows.Forms.Timer(this.components);
            this.LocalLogWatcher = new System.IO.FileSystemWatcher();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LocalLogWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 45);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(992, 528);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdrefresh);
            this.panel1.Controls.Add(this.cmdPause);
            this.panel1.Controls.Add(this.LogSelectorCombo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(992, 45);
            this.panel1.TabIndex = 1;
            // 
            // cmdrefresh
            // 
            this.cmdrefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdrefresh.Location = new System.Drawing.Point(824, 10);
            this.cmdrefresh.Name = "cmdrefresh";
            this.cmdrefresh.Size = new System.Drawing.Size(75, 23);
            this.cmdrefresh.TabIndex = 2;
            this.cmdrefresh.Text = "Refresh";
            this.cmdrefresh.UseVisualStyleBackColor = true;
            this.cmdrefresh.Click += new System.EventHandler(this.CmdrefreshClick);
            // 
            // cmdPause
            // 
            this.cmdPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdPause.Location = new System.Drawing.Point(905, 10);
            this.cmdPause.Name = "cmdPause";
            this.cmdPause.Size = new System.Drawing.Size(75, 23);
            this.cmdPause.TabIndex = 1;
            this.cmdPause.Text = "Avvia";
            this.cmdPause.UseVisualStyleBackColor = true;
            this.cmdPause.Click += new System.EventHandler(this.CmdPauseClick);
            // 
            // LogSelectorCombo
            // 
            this.LogSelectorCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogSelectorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LogSelectorCombo.FormattingEnabled = true;
            this.LogSelectorCombo.Location = new System.Drawing.Point(12, 12);
            this.LogSelectorCombo.Name = "LogSelectorCombo";
            this.LogSelectorCombo.Size = new System.Drawing.Size(806, 21);
            this.LogSelectorCombo.TabIndex = 0;
            this.LogSelectorCombo.SelectedIndexChanged += new System.EventHandler(this.LogSelectorComboSelectedIndexChanged);
            // 
            // logTimer
            // 
            this.logTimer.Interval = 2000;
            this.logTimer.Tick += new System.EventHandler(this.LogTimerTick);
            // 
            // LocalLogWatcher
            // 
            this.LocalLogWatcher.EnableRaisingEvents = true;
            this.LocalLogWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.LocalLogWatcher.SynchronizingObject = this;
            // 
            // LogMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 573);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel1);
            this.Name = "LogMonitor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LogMonitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogMonitorFormClosing);
            this.Load += new System.EventHandler(this.LogMonitorLoad);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LocalLogWatcher)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdPause;
        private System.Windows.Forms.ComboBox LogSelectorCombo;
        private System.Windows.Forms.Button cmdrefresh;
        private System.Windows.Forms.Timer logTimer;
        private System.IO.FileSystemWatcher LocalLogWatcher;
    }
}