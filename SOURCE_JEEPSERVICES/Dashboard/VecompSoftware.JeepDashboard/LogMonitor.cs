using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VecompSoftware.JeepDashboard.Code;
using VecompSoftware.JeepDashboard.Properties;
using Vecompsoftware.FileServer.Services;

namespace VecompSoftware.JeepDashboard
{
    public partial class LogMonitor : Form
    {
        private FileInfo _log;
        private readonly IFileRepositoryService _liveUpdateClient;
        private bool _enableRaisingEvents;

        public LogMonitor(FileInfo log, IFileRepositoryService liveUpdateClient)
        {
            _log = log;
            _liveUpdateClient = liveUpdateClient;
            InitializeComponent();
            if (liveUpdateClient != null) return;
            LocalLogWatcher.SynchronizingObject = this;
            LocalLogWatcher.Changed += FileWatcherChanged;
            ReadLog();
        }

        public LogMonitor(IFileRepositoryService liveUpdateClient)
        {
            _liveUpdateClient = liveUpdateClient;
            InitializeComponent();
            if (liveUpdateClient != null) return;
            LocalLogWatcher.SynchronizingObject = this;
            LocalLogWatcher.Changed += FileWatcherChanged;
        }

        private void LoadLogs()
        {
            LogSelectorCombo.Items.Clear();
            foreach (var file in LogsHelper.GetFiles(Path.Combine(Path.GetDirectoryName(Settings.Default.JeepServicePath), "logs"), "*.log", _liveUpdateClient))
            {
                LogSelectorCombo.Items.Add(file);
            }
            if (_log != null)
            {
                LogSelectorCombo.Text = _log.Name;
            }
        }

        private void LogMonitorLoad(object sender, EventArgs e)
        {
            if (_log != null)
            {
                Text = _log.Name;
            }
            // popolo l'elenco dei Log
            LoadLogs();
            ReadLog();
        }

        private void ReadLog()
        {
            if (_log == null) return;
            try
            {
                using (var stream = LogsHelper.GetFile(_log, _liveUpdateClient))
                {
                    richTextBox1.LoadFile(stream, RichTextBoxStreamType.PlainText);
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Errore in accesso a documento.");
                // Errore in accesso a file bloccato, attendo 1secondo e continuo
                Thread.Sleep(500);
            }
        }

        private void CmdPauseClick(object sender, EventArgs e)
        {
            // Se non ho un log da leggere torno indietro
            if (_log == null)
            {
                MessageBox.Show("Selezionare un log da leggere!");
                return;
            }

            logTimer.Enabled = false;
            var enable = !_enableRaisingEvents;
            cmdPause.Text = enable ? "Pausa" : "Avvia";

            _enableRaisingEvents = enable;
            if (_liveUpdateClient == null) LocalLogWatcher.EnableRaisingEvents = _enableRaisingEvents;
            else logTimer.Enabled = _enableRaisingEvents;
        }

        private void LogSelectorComboSelectedIndexChanged(object sender, EventArgs e)
        {
            _enableRaisingEvents = false;
            if (_liveUpdateClient == null) LocalLogWatcher.EnableRaisingEvents = _enableRaisingEvents;
            else logTimer.Enabled = _enableRaisingEvents;

            var exe = new FileInfo(Settings.Default.JeepServicePath);
            if (exe.DirectoryName != null)
            {
                var folder = Path.Combine(exe.DirectoryName, "logs");

                _log = new FileInfo(Path.Combine(folder, LogSelectorCombo.SelectedItem.ToString()));
            }
            Text = _log.Name;

            if (_liveUpdateClient == null)
            {
                LocalLogWatcher.Path = _log.DirectoryName;
                LocalLogWatcher.Filter = _log.Name;
            }

            cmdPause.Text = "Avvia";
            ReadLog();
        }

        private void LogMonitorFormClosing(object sender, FormClosingEventArgs e)
        {
            _enableRaisingEvents = false;
            if (_liveUpdateClient == null) LocalLogWatcher.EnableRaisingEvents = _enableRaisingEvents;
            else logTimer.Enabled = _enableRaisingEvents;
        }

        private void CmdrefreshClick(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void LogTimerTick(object sender, EventArgs e)
        {
            ReadLog();
        }

        private void FileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            var watcher = (FileSystemWatcher)sender;
            watcher.EnableRaisingEvents = false;
            ReadLog();
            Thread.Sleep(500);
            watcher.EnableRaisingEvents = true;
        }
    }
}
