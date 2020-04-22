using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;
using VecompSoftware.JeepDashboard.Code;
using VecompSoftware.JeepDashboard.Properties;
using VecompSoftware.JeepDashboard.Remote;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;
using Vecompsoftware.FileServer.Services;
using Vecompsoftware.FileServer.Services.ActionMessages;
using Configuration = VecompSoftware.JeepService.Common.Configuration;
using Module = VecompSoftware.JeepService.Common.Module;
using Timer = VecompSoftware.JeepService.Common.Timer;

namespace VecompSoftware.JeepDashboard
{
    public partial class MainForm : Form
    {
        #region [ Fields ]

        private const string Logger = "Application";
        private Configuration _configuration;

        #endregion

        #region [ Constructor ]

        public MainForm()
        {
            InitializeComponent();
            CheckRemote();
        }

        #endregion

        #region [ Properties ]

        public static IFileRepositoryService LiveUpdateClient { get; set; }

        public Configuration Cfg
        {
            get { return _configuration ?? (_configuration = ConfigurationHelper.LoadConfiguration(Settings.Default.JeepServiceConfig, LiveUpdateClient)); }
            private set { _configuration = value; }
        }

        private TreeNode ModulesNode { get; set; }

        private TreeNode SpoolsNode { get; set; }

        #endregion

        #region [ Generic Events ]

        private void Form1Load(object sender, EventArgs e)
        {
            Text = String.Format("Jeep Dashboard v.{0}", Application.ProductVersion);
            try
            {
                JeepServiceCheck();
                LiveUpdateServiceCheck();

                // Cerco di rileggere la configurazione, se presente
                Cfg = ConfigurationHelper.LoadConfiguration(Settings.Default.JeepServiceConfig, LiveUpdateClient);
                AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
                BindConfiguration(false);

                // Se sono stati caricati i nodi esco
                if (Tree.Nodes.Count != 0) return;

                // Altrimenti lancio un'eccezione 
                throw new Exception();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + Tools.FullStackTrace(ex));

                // Se non è stato caricato l'albero significa che non riesco a leggere la configurazione, propongo l'impostazione dei parametri
                var form = new Preferences();
                if (form.ShowDialog(this) != DialogResult.OK) return;
                JeepServiceCheck();
                LiveUpdateServiceCheck();
                BindConfiguration(true);
            }


        }

        private Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AssemblyResolver(args);
        }

        private void GridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor == null) return;

            // Carico l'aggiornamento dalla grafica
            UpdateModules();

            // Aggiorno la configurazione
            ConfigurationHelper.SaveConfiguration(Cfg, LiveUpdateClient);

            // Ricarico la schermata
            BindConfiguration(false);
        }

        #endregion

        #region [ TreeView Events ]

        private void TreeAfterSelect(object sender, TreeViewEventArgs e)
        {
            grid.SelectedObject = e.Node.Tag;
        }

        private void TreeDragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            var targetPoint = Tree.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            var targetNode = Tree.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            var draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (!(targetNode.Tag is Timer)) return;
            var timer = targetNode.Tag as Timer;
            var spool = targetNode.Parent.Tag as SpoolXml;
            var module = draggedNode.Tag as Module;
            if (module != null)
            {
                module.Timer = timer.Id;
                if (spool != null) module.Spool = spool.Id;
            }

            RefreshTimerNodes();
            Tree.SelectedNode = draggedNode;
        }

        private void TreeItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = e.Item as TreeNode;
            if (node != null && node.Tag is Module)
            {
                DoDragDrop(e.Item, DragDropEffects.Link);
            }
        }

        private void TreeDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void TreeDragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.
            var targetPoint = Tree.PointToClient(new Point(e.X, e.Y));
            // Select the node at the mouse position.
            Tree.SelectedNode = Tree.GetNodeAt(targetPoint);

            if (Tree.SelectedNode != null && Tree.SelectedNode.Tag is Timer) e.Effect = e.AllowedEffect;
            else e.Effect = DragDropEffects.None;
        }

        private void TreeMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var node = Tree.GetNodeAt(e.X, e.Y);
            Tree.SelectedNode = node;
        }

        private void TreeBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag is Timer)
            {
                RefreshTimerNode(e.Node);
                return;
            }

            if (e.Node.Tag is Module)
            {
                Tools.RefreshModuleNode(e.Node);
                return;
            }

            if (e.Node.Tag is SpoolXml)
            {
                Tools.RefreshSpoolNode(e.Node);
            }
        }

        private void TreeAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                return;
            }

            if (e.Node.Tag is Timer)
            {
                var t = e.Node.Tag as Timer;
                t.Id = e.Label;
                RefreshTimerNode(e.Node);
            }

            if (e.Node.Tag is Module)
            {
                var m = e.Node.Tag as Module;
                m.Id = e.Label;
                Tools.RefreshModuleNode(e.Node);
            }

            if (e.Node.Tag is SpoolXml)
            {
                var s = e.Node.Tag as SpoolXml;
                s.Id = e.Label;
                Tools.RefreshSpoolNode(e.Node);
            }

            if (e.Node.Tag != null)
            {
                grid.SelectedObject = e.Node.Tag;
            }
        }

        private void TreeBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is Timer) && !(e.Node.Tag is Module) && !(e.Node.Tag is SpoolXml))
            {
                e.CancelEdit = true;
            }

            if (e.Node.Tag is Timer)
            {
                e.Node.Text = (e.Node.Tag as Timer).Id;
            }
        }

        #endregion

        #region [ Context Menu Events ]

        private void RemoveToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Tree.SelectedNode == null)
            {
                return;
            }

            if (Tree.SelectedNode.Tag is Module)
            {
                var module = Tree.SelectedNode.Tag as Module;
                Cfg.Modules.Remove(module);
            }
            else if (Tree.SelectedNode.Tag is SpoolXml)
            {
                var spool = Tree.SelectedNode.Tag as SpoolXml;
                Cfg.Spools.Remove(spool);
            }
            else if (Tree.SelectedNode.Tag is Timer)
            {
                var spool = Tree.SelectedNode.Parent.Tag as SpoolXml;
                var timer = Tree.SelectedNode.Tag as Timer;
                if (spool != null) spool.Timers.Remove(timer);
            }
            BindConfiguration(false);
        }

        private void InstallToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Tree.SelectedNode == null)
            {
                return;
            }

            switch (Tree.SelectedNode.Text)
            {
                case "MODULES":
                    {
                        var form = new InstallModule();
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            //Cfg.Modules.Add(form.Module);
                        }
                        break;
                    }
            }
        }

        private void AddToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Tree.SelectedNode == null)
            {
                return;
            }

            switch (Tree.SelectedNode.Text)
            {
                case "MODULES":
                    {
                        var form = new AddModule();
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            Cfg.Modules.Add(form.Module);
                        }
                        break;
                    }
                case "SPOOLS":
                    {
                        var spool = new SpoolXml { Id = "New Spool" };
                        Cfg.Spools.Add(spool);
                        break;
                    }
                default:
                    {
                        // Aggiungo un Timer
                        if (Tree.SelectedNode.Tag is SpoolXml)
                        {
                            var timer = new Timer { Id = "New Timer" };
                            var spool = Tree.SelectedNode.Tag as SpoolXml;
                            if (spool.Timers == null)
                            {
                                spool.Timers = new List<Timer>();
                            }
                            spool.Timers.Add(timer);
                        }
                        break;
                    }
            }

            BindConfiguration(false);
        }

        /// <summary>
        /// Questo metodo si occupa di gestire l'aggiornamento del modulo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Tree.SelectedNode == null)
            {
                return;
            }

            // Verifico se il JeepService è acceso e in tal caso lo spengo
            if (ServicesHelper.ServiceStatus(Settings.Default.ServiceName, Settings.Default.JeepServicePath, LiveUpdateClient) != ServiceControllerStatus.Stopped)
            {
                if (MessageBox.Show("Per aggiornare il modulo è necessario prima fermare il servizio. Procedere?", "Servizio attivo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    JeepServiceStop();
                }
                else
                {
                    return;
                }
            }

            var module = Tree.SelectedNode.Tag as Module;
            if (module == null) return;

            BackupAndUpdateModule(module.Id, module.Assembly);

            // Ricarico la configurazione
            BindConfiguration(true);
        }

        #endregion

        #region [ JeepService Menu Events ]

        private void JeepServiceInstallToolStripMenuItemClick(object sender, EventArgs e)
        {
            JeepServiceInstall(false);
        }

        private void JeepServiceUninstallToolStripMenuItemClick(object sender, EventArgs e)
        {
            JeepServiceInstall(true);
        }

        private void JeepServiceStartToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (processStarter.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    processStarter.RunWorkerAsync(ProcessStarterActivity.JeepServiceStart);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void JeepServiceStopToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (processStarter.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    processStarter.RunWorkerAsync(ProcessStarterActivity.JeepServiceStop);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void JeepServiceRestartToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (processStarter.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    processStarter.RunWorkerAsync(ProcessStarterActivity.JeepServiceRestart);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void JeepServiceStartConsoleToolStripMenuItemClick(object sender, EventArgs e)
        {
            JeepServiceConsole();
        }

        private void JeepServiceConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenConfigurator(Settings.Default.JeepServicePath);
        }

        /// <summary>
        /// Aggiorna il JeepService
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JeepServiceUpdateToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Verifico se il JeepService è acceso e in tal caso lo spengo
            if (ServicesHelper.ServiceStatus(Settings.Default.ServiceName, Settings.Default.JeepServicePath, LiveUpdateClient) != ServiceControllerStatus.Stopped)
            {
                if (MessageBox.Show("Per aggiornare il modulo è necessario prima fermare il servizio. Procedere?", "Servizio attivo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    JeepServiceStop();
                }
                else
                {
                    return;
                }
            }

            BackupAndUpdateModule("JeepService", Settings.Default.JeepServicePath);

            JeepServiceCheck();
        }

        #endregion

        #region [ LiveUpdateService Menu Events ]

        private void LiveUpdateServiceInstallStripMenuItemClick(object sender, EventArgs e)
        {
            LiveUpdateServiceInstall(false);
        }

        private void LiveUpdateServiceUninstallStripMenuItemClick(object sender, EventArgs e)
        {
            LiveUpdateServiceInstall(true);
        }

        private void LiveUpdateServiceStartStripMenuItemClick(object sender, EventArgs e)
        {
            if (!ServicesHelper.Exists(Settings.Default.LiveUpdateServiceName, Settings.Default.LiveUpdatePath, null))
            {
                MessageBox.Show(this, "Il servizio non è installato.");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                var service = new ServiceController(Settings.Default.LiveUpdateServiceName);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(60000));
                LiveUpdateServiceCheck();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LiveUpdateStopStripMenuItemClick(object sender, EventArgs e)
        {
            if (!ServicesHelper.Exists(Settings.Default.LiveUpdateServiceName, Settings.Default.LiveUpdatePath, null))
            {
                MessageBox.Show(this, "Il servizio non è installato.");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                var service = new ServiceController(Settings.Default.LiveUpdateServiceName);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(60000));
                LiveUpdateServiceCheck();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LiveUpdateServiceStartConsoleStripMenuItemClick(object sender, EventArgs e)
        {
            LiveUpdateConsole();
        }

        private void LiveUpdateServiceConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenConfigurator(Settings.Default.LiveUpdatePath);
        }

        private void LiveUpdateUpdateToolStripMenuItemClick(object sender, EventArgs e)
        {
            BackupAndUpdateModule("JeepService.LiveUpdate", Settings.Default.LiveUpdatePath);
            var promptValue = Prompt.ShowDialog("Nome del file:", "Inserire il nome del file che deve essere eseguito");
            var returnMessage = LiveUpdateClient.ExecuteFile(new ActionMessage { Message = promptValue }).Message;
            var exit = false;
            while (!String.IsNullOrEmpty(returnMessage) && !exit)
            {
                if (
                    MessageBox.Show(String.Format("{0} Riprovare?", returnMessage), "Errore in esecuzione file",
                                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    promptValue = Prompt.ShowDialog("Nome del file:",
                                                    "Inserire il nome del file che deve essere eseguito");
                    returnMessage = LiveUpdateClient.ExecuteFile(new ActionMessage { Message = promptValue }).Message;
                }
                else
                {
                    exit = true;
                }
            }
        }

        #endregion

        #region [ Log Menu Events ]

        private void LogOpenConfigToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!File.Exists(Settings.Default.log4netConfig))
            {
                MessageBox.Show(this, string.Format("Impossibile trovare il file [{0}].", Settings.Default.log4netConfig));
                return;
            }

            var pi = new ProcessStartInfo { UseShellExecute = true, FileName = Settings.Default.log4netConfig };
            using (var p = new Process { StartInfo = pi })
            {
                try
                {
                    p.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LogMonitorToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Tree.SelectedNode != null && (Tree.SelectedNode.Tag is Module || Tree.SelectedNode.Parent.Tag is Module))
            {
                Module module;
                if (Tree.SelectedNode.Tag is Module)
                {
                    // Seleziono il modulo
                    module = Tree.SelectedNode.Tag as Module;
                }
                else
                {
                    // Seleziono il modulo a partire dai parametri
                    module = Tree.SelectedNode.Parent.Tag as Module;
                }

                var filename = string.Concat(module.Id, ".log");
                var exe = new FileInfo(Settings.Default.JeepServicePath);
                if (exe.DirectoryName != null)
                {
                    var folder = Path.Combine(exe.DirectoryName, "logs");
                    var log = new FileInfo(Path.Combine(folder, filename));
                    var form = new LogMonitor(log, LiveUpdateClient);
                    form.Show();
                }
            }
            else
            {
                var form = new LogMonitor(LiveUpdateClient);
                form.Show();

            }
        }

        private void LogClearToolStripMenuItemClick(object sender, EventArgs e)
        {
            var exe = new FileInfo(Settings.Default.JeepServicePath);
            if (exe.DirectoryName != null)
            {
                var folder = Path.Combine(exe.DirectoryName, "logs");
                var directory = new DirectoryInfo(folder);
                try
                {
                    foreach (var file in directory.GetFiles()) file.Delete();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LogOpenFolderToolStripMenuItemClick(object sender, EventArgs e)
        {
            var exe = new FileInfo(Settings.Default.JeepServicePath);
            if (exe.DirectoryName == null) return;
            var folder = Path.Combine(exe.DirectoryName, "logs");
            Process.Start(folder);
        }

        #endregion

        #region [ Generic Menu Events ]

        private void FileSaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                UpdateModules();

                // Elimino il namespace
                ConfigurationHelper.SaveConfiguration(Cfg, LiveUpdateClient);

                // Effettuo il backup
                Tools.BackupConfiguration();

                MessageBox.Show("Salvataggio avvenuto con successo", "Salvataggio", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void EditReloadToolStripMenuItemClick(object sender, EventArgs e)
        {
            BindConfiguration(true);
        }

        private void EditPreferencesToolStripMenuItemClick(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                this,
                string.Format("Cambiare il file di configurazione potrebbe eliminare eventuali modifiche apportate.{0}Continuare?", Environment.NewLine),
                "Attenzione",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                return;
            }

            var form = new Preferences();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                JeepServiceCheck();
                LiveUpdateServiceCheck();
                BindConfiguration(true);
            }
        }

        private void FileExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void HelpAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format("Dgroove Srl - JeepDashBoard v.{0}", Application.ProductVersion), "About");
        }

        private void HelpUpdatesToolStripMenuItemClick(object sender, EventArgs e)
        {
            var form = new ManageUpdates();
            form.Show();
        }

        #endregion

        #region [ JeepService Methods ]

        private void JeepServiceStart()
        {
            if (LiveUpdateClient != null)
            {
                LiveUpdateClient.StartService(Settings.Default.ServiceName);
            }
            else
            {
                var service = new ServiceController(Settings.Default.ServiceName);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(600000));
            }
            JeepServiceCheck();
        }

        private void JeepServiceStop()
        {
            if (LiveUpdateClient != null)
            {
                LiveUpdateClient.StopService(Settings.Default.ServiceName);
            }
            else
            {
                var service = new ServiceController(Settings.Default.ServiceName);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(60000));
            }
            JeepServiceCheck();
        }

        private void JeepServiceRestart()
        {
            if (LiveUpdateClient != null)
            {
                LiveUpdateClient.RestartService(Settings.Default.ServiceName);
            }
            else
            {
                var service = new ServiceController(Settings.Default.ServiceName);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(60000));
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(60000));
            }
            JeepServiceCheck();
        }

        private void JeepServiceInstall(bool uninstall)
        {
            if (!File.Exists(Settings.Default.JeepServicePath))
            {
                MessageBox.Show(this, string.Format("Impossibile trovare il file [{0}]", Settings.Default.JeepServicePath));
                return;
            }

            var startInfo = new ProcessStartInfo
                                {
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    FileName = Settings.Default.JeepServicePath,
                                    Arguments = string.Format("{0} /ServiceName={1}", uninstall ? " -u" : " -i", Settings.Default.ServiceName)
                                };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
            }

            JeepServiceCheck();
        }

        private void JeepServiceConsole()
        {
            if (!File.Exists(Settings.Default.JeepServicePath))
            {
                MessageBox.Show(this, string.Format("Impossibile trovare il file [{0}]", Settings.Default.JeepServicePath));
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = Settings.Default.JeepServicePath,
                Arguments = " -c"
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
            }
        }

        private void JeepServiceCheck()
        {
            try
            {
                ServicesHelper.Exists(Settings.Default.ServiceName, Settings.Default.JeepServicePath, LiveUpdateClient);
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Il servizio non esiste!", ex);
            }

            // Servizio presente
            // Versione servizio
            var jeepServiceVersion = ServicesHelper.ServiceVersion(Settings.Default.ServiceName,
                                                                   Settings.Default.JeepServicePath,
                                                                   LiveUpdateClient);

            string serviceExist;
            string serviceStatus;
            if (ServicesHelper.Installed(Settings.Default.ServiceName, Settings.Default.JeepServicePath,
                                         LiveUpdateClient))
            {
                // Il servizio è installato
                jeepServiceInstallToolStripMenuItem.Enabled = false;
                jeepServiceUninstallToolStripMenuItem.Enabled = true;
                serviceExist = string.Format("Servizio [{0} v.{1}] installato.", Settings.Default.ServiceName,
                                             jeepServiceVersion);
                imgInstallationOFF.Visible = false;
                imgInstallationON.Visible = true;

                var serviceControllerStatus = ServicesHelper.ServiceStatus(Settings.Default.ServiceName,
                                                                           Settings.Default.JeepServicePath,
                                                                           LiveUpdateClient);
                switch (serviceControllerStatus)
                {
                    case ServiceControllerStatus.Running:
                        jeepServiceStartToolStripMenuItem.Enabled = false;
                        jeepServiceStopToolStripMenuItem.Enabled = true;
                        jeepServiceRestartToolStripMenuItem.Enabled = true;
                        jeepServiceStartConsoleToolStripMenuItem.Enabled = false;
                        serviceStatus = "The Service is Running.";
                        imgStatusPlay.Visible = true;
                        imgStatusPause.Visible = false;
                        imgStatusQuestion.Visible = false;
                        break;
                    case ServiceControllerStatus.Stopped:
                        jeepServiceStartToolStripMenuItem.Enabled = true;
                        jeepServiceStartConsoleToolStripMenuItem.Enabled = true;
                        jeepServiceStopToolStripMenuItem.Enabled = false;
                        jeepServiceRestartToolStripMenuItem.Enabled = false;
                        serviceStatus = "The Service is Stopped.";
                        imgStatusPlay.Visible = false;
                        imgStatusPause.Visible = true;
                        imgStatusQuestion.Visible = false;
                        break;
                    default:
                        jeepServiceStartToolStripMenuItem.Enabled = false;
                        serviceStatus = string.Format("The Service is {0}.", serviceControllerStatus);
                        jeepServiceStopToolStripMenuItem.Enabled = false;
                        jeepServiceRestartToolStripMenuItem.Enabled = false;
                        imgStatusPlay.Visible = false;
                        imgStatusPause.Visible = false;
                        imgStatusQuestion.Visible = true;
                        break;
                }
            }
            else
            {
                // Servizio non installato
                jeepServiceInstallToolStripMenuItem.Enabled = true;
                jeepServiceUninstallToolStripMenuItem.Enabled = false;
                editToolStripMenuItem.Enabled = true;
                serviceExist = string.Format("Servizio [{0} v.{1}] NON installato.", Settings.Default.ServiceName,
                                             jeepServiceVersion);
                serviceStatus = "";
                imgInstallationOFF.Visible = true;
                imgInstallationON.Visible = false;

                jeepServiceStartToolStripMenuItem.Enabled = false;
                jeepServiceStartConsoleToolStripMenuItem.Enabled = File.Exists(Settings.Default.JeepServicePath);
                jeepServiceStopToolStripMenuItem.Enabled = false;
                jeepServiceRestartToolStripMenuItem.Enabled = false;

                imgStatusPlay.Visible = false;
                imgStatusPause.Visible = false;
                imgStatusQuestion.Visible = false;
            }
            var jeepServiceStatus = string.Format("{0} {1}", serviceExist, serviceStatus);
            jeepServiceStatusLabel.Text = jeepServiceStatus;
            FileLogger.Info(Logger, jeepServiceStatus);
        }

        #endregion

        #region [ LiveUpdateService Methods ]

        private void LiveUpdateServiceInstall(bool uninstall)
        {
            if (!File.Exists(Settings.Default.LiveUpdatePath))
            {
                MessageBox.Show(this, string.Format("Impossibile trovare il file [{0}]", Settings.Default.LiveUpdatePath));
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Settings.Default.LiveUpdatePath,
                Arguments = string.Format("{0} /ServiceName={1}", uninstall ? " -u" : " -i", Settings.Default.LiveUpdateServiceName)
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                process.WaitForExit();
            }

            LiveUpdateServiceCheck();
        }

        private void LiveUpdateConsole()
        {
            if (!File.Exists(Settings.Default.LiveUpdatePath))
            {
                MessageBox.Show(this, string.Format("Impossibile trovare il file [{0}]", Settings.Default.LiveUpdatePath));
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = Settings.Default.LiveUpdatePath,
                Arguments = " -c"
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
            }
        }

        private void LiveUpdateServiceCheck()
        {
            var liveUpdateServiceVersion = ServicesHelper.ServiceVersion(Settings.Default.LiveUpdateServiceName,
                                                                   Settings.Default.LiveUpdatePath,
                                                                   LiveUpdateClient);
            string serviceExist;
            string serviceStatus;
            // Servizio non installato
            if (!ServicesHelper.Exists(Settings.Default.LiveUpdateServiceName, Settings.Default.LiveUpdatePath, LiveUpdateClient))
            {
                // Servizio non installato
                liveUpdateServiceInstallStripMenuItem.Enabled = true;
                liveUpdateServiceUninstallStripMenuItem.Enabled = false;
                editToolStripMenuItem.Enabled = true;
                serviceExist = string.Format("Service [{0} v.{1}] NOT installed.", Settings.Default.LiveUpdateServiceName,
                                             liveUpdateServiceVersion);
                serviceStatus = "";

                liveUpdateServiceStartStripMenuItem.Enabled = false;
                liveUpdateServiceStartConsoleStripMenuItem.Enabled = File.Exists(Settings.Default.JeepServicePath);
                liveUpdateStopStripMenuItem.Enabled = false;
                liveUpdateServiceRestartToolStripMenuItem.Enabled = false;
            }
            else
            {
                // Servizio installato
                liveUpdateServiceInstallStripMenuItem.Enabled = false;
                liveUpdateServiceUninstallStripMenuItem.Enabled = true;
                serviceExist = string.Format("Service [{0} v.{1}] installed.", Settings.Default.LiveUpdateServiceName, liveUpdateServiceVersion);
                imgInstallationOFF.Visible = false;
                imgInstallationON.Visible = true;

                var serviceControllerStatus = ServicesHelper.ServiceStatus(Settings.Default.LiveUpdateServiceName,
                                                                           Settings.Default.LiveUpdatePath,
                                                                           LiveUpdateClient);
                switch (serviceControllerStatus)
                {
                    case ServiceControllerStatus.Running:
                        liveUpdateServiceStartStripMenuItem.Enabled = false;
                        liveUpdateStopStripMenuItem.Enabled = true;
                        liveUpdateServiceStartConsoleStripMenuItem.Enabled = false;
                        liveUpdateServiceRestartToolStripMenuItem.Enabled = false;
                        serviceStatus = string.Format("The Service is running.");
                        break;
                    case ServiceControllerStatus.Stopped:
                        liveUpdateServiceStartStripMenuItem.Enabled = true;
                        liveUpdateServiceStartConsoleStripMenuItem.Enabled = true;
                        liveUpdateStopStripMenuItem.Enabled = false;
                        liveUpdateServiceRestartToolStripMenuItem.Enabled = true;
                        serviceStatus = string.Format("The Service is stopped");
                        break;
                    default:
                        liveUpdateServiceStartStripMenuItem.Enabled = false;
                        liveUpdateStopStripMenuItem.Enabled = false;
                        liveUpdateServiceRestartToolStripMenuItem.Enabled = false;
                        serviceStatus = string.Format("The Service is {0}.", serviceControllerStatus);
                        break;
                }
            }
            var liveUpdateServiceStatus = string.Format("{0} {1}", serviceExist, serviceStatus);
            liveUpdateStatusLabel.Text = liveUpdateServiceStatus;
            FileLogger.Info(Logger, liveUpdateServiceStatus);
        }

        #endregion

        #region [ Private Methods ]

        private Assembly AssemblyResolver(ResolveEventArgs args)
        {
            if (Cfg == null)
            {
                return null;
            }

            foreach (var temp in Cfg.Modules.Select(module => new FileInfo(module.FullAssemblyPath)).Select(info => FindAssemblyInDirectory(args.Name, info.Directory)).Where(temp => temp != null))
            {
                return temp;
            }
            throw new FileNotFoundException("Impossibile trovare un assembly.", args.Name);
        }

        private static Assembly FindAssemblyInDirectory(string name, DirectoryInfo dir)
        {
            var searchingAssembly = new AssemblyName(name);
            return dir.GetFiles("*.dll").Select(file => Assembly.LoadFile(file.FullName)).FirstOrDefault(myAssembly => searchingAssembly.Name == myAssembly.GetName().Name && (searchingAssembly.Version == null || searchingAssembly.Version.CompareTo(myAssembly.GetName().Version) <= 0));
        }

        private void UpdateModules()
        {
            foreach (var mod in ModulesNode.Nodes.Cast<TreeNode>())
            {
                var module = mod.Tag as Module;
                if (module == null || string.IsNullOrEmpty(module.ParametersType) || mod.Nodes.Count != 1)
                {
                    continue;
                }

                // Recupero l'istanza IJeepParameter
                var parameter = mod.Nodes[0].Tag as IJeepParameter;
                // Faccio in modo che non vada a cancellare eventuali parametri non caricabili
                if (parameter == null || parameter is EmptyJeepParameter) continue;
                var items = parameter.Serialize();
                module.Parameters = items;
            }
        }

        private static void CheckRemote()
        {
            var remoteAddress = ConfigurationManager.AppSettings["LiveUpdateServiceAddress"];
            if (!String.IsNullOrEmpty(remoteAddress))
            {
                //Inizializzo la connessione con il servizio WCF
                LiveUpdateClient = new FileRepositoryServiceClient();
            }
            // Altrimenti leggo solamente da file
        }

        private int CountModulesInTimer(Timer timer)
        {
            return Cfg.Modules.Count(mod => mod.Timer == timer.Id);
        }

        private void RefreshTimerNode(TreeNode node)
        {
            var timer = node.Tag as Timer;
            if (timer != null) node.Text = string.Format("{0} ({1})", timer.Id, CountModulesInTimer(timer));
        }

        private void RefreshTimerNodes()
        {
            foreach (var timer in from TreeNode sn in SpoolsNode.Nodes from TreeNode timer in sn.Nodes select timer)
            {
                RefreshTimerNode(timer);
            }
        }

        private void BindConfiguration(bool reloadConfig)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (reloadConfig)
                {
                    Cfg = ConfigurationHelper.LoadConfiguration(Settings.Default.JeepServiceConfig, LiveUpdateClient);
                }

                var spools = new TreeNode("SPOOLS") { Name = "spoolUnique" };
                foreach (var spoolXml in Cfg.Spools)
                {
                    var item = new TreeNode(spoolXml.Id)
                    {
                        Name = spoolXml.Id,
                        ImageKey = "Spool",
                        SelectedImageKey = "Spool",
                        Tag = spoolXml
                    };

                    spools.Nodes.Add(item);

                    foreach (var t in spoolXml.Timers.Select(timer => new TreeNode
                    {
                        Name = timer.Id,
                        Text = string.Format("{0} ({1})", timer.Id, CountModulesInTimer(timer)),
                        ImageKey = "Timer",
                        SelectedImageKey = "Timer",
                        Tag = timer
                    }))
                    {
                        item.Nodes.Add(t);
                    }
                }
                SpoolsNode = spools;

                var modules = new TreeNode("MODULES") { Name = "modulesUnique" };
                // Carico i moduli
                foreach (var module in Cfg.Modules)
                {
                    var imagekey = module.Enabled ? "Module" : "Disabled";
                    var item = new TreeNode(String.Format("{0} [{1}]", module.Id, module.Version))
                    {
                        Name = module.Id,
                        ImageKey = imagekey,
                        SelectedImageKey = imagekey,
                        Tag = module
                    };
                    modules.Nodes.Add(item);

                    var count = module.Parameters == null ? 0 : module.Parameters.Count;
                    var pNode = new TreeNode(string.Format("PARAMETERS ({0})", count))
                    {
                        Name = "p" + module.Id,
                        ImageKey = "Parameter",
                        SelectedImageKey = "Parameter",
                        Tag = Tools.ParamBuilder(LiveUpdateClient, module.FullAssemblyPath, module.ParametersType, module.Parameters)
                    };
                    item.Nodes.Add(pNode);
                }
                ModulesNode = modules;

                var nodes = new List<TreeNode> { SpoolsNode, ModulesNode };

                var selectedNode = Tree.SelectedNode;

                Tree.BeginUpdate();
                Tree.Nodes.Clear();
                Tree.Nodes.AddRange(nodes.ToArray());
                Tree.ExpandAll();
                Tree.EndUpdate();

                if (selectedNode == null) return;
                var findedNodes = Tree.Nodes.Find(selectedNode.Name, true);
                if (findedNodes.Length > 0)
                {
                    Tree.SelectedNode = findedNodes[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void OpenConfigurator(string path)
        {
            if (LiveUpdateClient == null && !File.Exists(path))
            {
                MessageBox.Show(string.Format(
                    "Impossibile trovare il percorso al file di configurazione [{0}].{1}Correggere le in {2}/{3}.", path, Environment.NewLine, editToolStripMenuItem.Text, editPreferencesToolStripMenuItem.Text),
                    "Errore caricamento",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var configParameter = new Configurator(path, LiveUpdateClient);
            configParameter.ShowDialog(this);
        }

        /// <summary>
        /// Effettua l'aggiornamento di un modulo
        /// </summary>
        /// <param name="moduleName">Nome del modulo che deve essere aggiornato</param>
        /// <param name="workingFile">Path del file di riferimento da aggiornare (dll/exe)</param>
        private void BackupAndUpdateModule(string moduleName, string workingFile)
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Selezionare il pacchetto di aggiornamento",
                RestoreDirectory = false,
                CheckFileExists = true,
                Filter = String.Format("{0}.zip|*.zip", moduleName)
            };

            openDialog.ShowDialog();

            if (string.IsNullOrEmpty(openDialog.FileName)) return;

            // ReSharper disable AssignNullToNotNullAttribute
            var updateZipFilePath = Path.Combine(Path.GetDirectoryName(workingFile), "UpdateTemp", openDialog.SafeFileName);
            // ReSharper restore AssignNullToNotNullAttribute
            using (Stream uploadStream = new FileStream(openDialog.FileName, FileMode.Open))
            {
                LiveUpdateClient.PutFile(new FileUploadMessage { VirtualPath = updateZipFilePath, DataStream = uploadStream });
            }

            // Effettua l'effettivo aggiornamento
            var backupPath = LiveUpdateClient.Update(moduleName, updateZipFilePath, ZipExtraction.FullInternalDirectionExtraction, "UpdateTemp",
                                    Path.GetDirectoryName(workingFile), true, "BackupUpdate", String.Empty).Message;

            // Scarica il backup se è stato generato
            if (String.IsNullOrEmpty(backupPath)) return;
            var saveDialog = new SaveFileDialog
            {
                RestoreDirectory = false,
                OverwritePrompt = false,
                Title = String.Format("Salvare il backup del modulo {0}", moduleName),
                FileName = Path.GetFileName(backupPath),
                Filter = String.Format("{0}.zip|*.zip", Path.GetFileName(backupPath))
            };

            // Se premo OK salvo il backup in locale e lo cancello da remoto, altrimenti lo lascio in remoto
            if (saveDialog.ShowDialog(this) != DialogResult.OK || string.IsNullOrEmpty(saveDialog.FileName)) return;

            // Get the file from the server
            using (var output = new FileStream(saveDialog.FileName, FileMode.Create))
            {
                LiveUpdateClient.GetFile(backupPath).CopyTo(output);
            }

            // Elimina il backup in remoto
            LiveUpdateClient.DeleteFile(backupPath);
        }

        #endregion

        private void ProcessStarterDoWork(object sender, DoWorkEventArgs e)
        {
            if (!(e.Argument is ProcessStarterActivity)) return;

            var activity = (ProcessStarterActivity)e.Argument;
            switch (activity)
            {
                case ProcessStarterActivity.JeepServiceStart:
                    JeepServiceStart();
                    break;
                case ProcessStarterActivity.JeepServiceStop:
                    JeepServiceStop();
                    break;
                case ProcessStarterActivity.JeepServiceRestart:
                    JeepServiceRestart();
                    break;
            }

            e.Result = activity;
        }

        private void ProcessStarterRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //NOOP
        }

        private void StatusBarRefreshTimerTick(object sender, EventArgs e)
        {
            JeepServiceCheck();
            LiveUpdateServiceCheck();
        }
    }
}
