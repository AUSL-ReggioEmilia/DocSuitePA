using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VecompSoftware.JeepDashboard.Code;

namespace VecompSoftware.JeepDashboard
{
    public partial class Preferences : Form
    {
        #region [ Constructor ]

        public Preferences()
        {
            InitializeComponent();
        }

        #endregion

        #region [ Events ]

        private void Preferences_Load(object sender, EventArgs e)
        {
            JeepServiceConfigPath.Text = Properties.Settings.Default.JeepServiceConfig;
            JeepServiceExePath.Text = Properties.Settings.Default.JeepServicePath;
            JeepServiceName.Text = Properties.Settings.Default.ServiceName;
            Log4NetConfigPath.Text = Properties.Settings.Default.log4netConfig;
            LiveUpdateServiceName.Text = Properties.Settings.Default.LiveUpdateServiceName;
            LiveUpdateExePath.Text = Properties.Settings.Default.LiveUpdatePath;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.JeepServiceConfig = JeepServiceConfigPath.Text.Trim();
            Properties.Settings.Default.JeepServicePath = JeepServiceExePath.Text.Trim();
            Properties.Settings.Default.ServiceName = JeepServiceName.Text.Trim();
            Properties.Settings.Default.log4netConfig = Log4NetConfigPath.Text.Trim();
            Properties.Settings.Default.LiveUpdateServiceName = LiveUpdateServiceName.Text;
            Properties.Settings.Default.LiveUpdatePath = LiveUpdateExePath.Text;

            Properties.Settings.Default.Save();
            DialogResult = DialogResult.OK;
        }

        private void FindExecutable_Click(object sender, EventArgs e)
        {
            SetPath(ref JeepServiceExePath, lblJeepServiceExePath.Text);
        }

        private void Log4Net_Click(object sender, EventArgs e)
        {
            SetPath(ref Log4NetConfigPath, lblLog4NetConfigPath.Text);
        }

        private void FindConfigPath_Click(object sender, EventArgs e)
        {
            SetPath(ref JeepServiceConfigPath, lblJeepServiceConfigPath.Text);
        }

        private void FindLiveUpdatePath_Click(object sender, EventArgs e)
        {
            SetPath(ref LiveUpdateExePath, lblLiveUpdateExePath.Text);
        }

        private void ServiceName_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox) sender;
            if (ServicesHelper.Exists(textBox.Text, JeepServiceExePath.Text, null))
            {
                textBox.BackColor = Color.Coral;
                toolTip.Show("Nome servizio già in uso.", textBox);
            }
            else
            {
                textBox.BackColor = Color.White;
                toolTip.Hide(textBox);
            }
        }

        #endregion

        #region [ Methods ]

        private void SetPath(ref TextBox textBox, string title)
        {
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = title;
            if (File.Exists(textBox.Text))
            {
                var info = new FileInfo(textBox.Text);
                if (!string.IsNullOrWhiteSpace(info.DirectoryName))
                {
                    openFileDialog.InitialDirectory = info.DirectoryName;
                }
                if (!string.IsNullOrWhiteSpace(info.Name))
                {
                    openFileDialog.FileName = info.Name;
                }
            }
            else
            {
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                openFileDialog.FileName = "";
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = openFileDialog.FileName;
            }
        }

        #endregion
    }
}
