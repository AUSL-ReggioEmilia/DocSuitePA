using System.IO;
using System.Text;

namespace VecompSoftware.JeepDashboard
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.grid = new System.Windows.Forms.PropertyGrid();
            this.Tree = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aDDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uPDATEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.FileExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editReloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editPreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceInstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceUninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceRestartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceStartConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.jeepServiceConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeepServiceUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateServiceStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateServiceInstallStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateServiceUninstallStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateServiceStartStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateServiceRestartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateStopStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateServiceStartConsoleStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.liveUpdateServiceConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOpenConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOpenFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.jeepServiceStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.liveUpdateStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imgStatusQuestion = new System.Windows.Forms.PictureBox();
            this.imgStatusPause = new System.Windows.Forms.PictureBox();
            this.imgStatusPlay = new System.Windows.Forms.PictureBox();
            this.imgInstallationOFF = new System.Windows.Forms.PictureBox();
            this.imgInstallationON = new System.Windows.Forms.PictureBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.processStarter = new System.ComponentModel.BackgroundWorker();
            this.statusBarRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusPause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusPlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgInstallationOFF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgInstallationON)).BeginInit();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Dock = System.Windows.Forms.DockStyle.Right;
            this.grid.Location = new System.Drawing.Point(385, 24);
            this.grid.MinimumSize = new System.Drawing.Size(210, 0);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(733, 537);
            this.grid.TabIndex = 0;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.GridPropertyValueChanged);
            // 
            // Tree
            // 
            this.Tree.AllowDrop = true;
            this.Tree.ContextMenuStrip = this.contextMenuStrip1;
            this.Tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tree.FullRowSelect = true;
            this.Tree.HideSelection = false;
            this.Tree.ImageKey = "Node";
            this.Tree.ImageList = this.imageList1;
            this.Tree.Location = new System.Drawing.Point(0, 24);
            this.Tree.Name = "Tree";
            this.Tree.SelectedImageKey = "Node";
            this.Tree.Size = new System.Drawing.Size(375, 498);
            this.Tree.TabIndex = 1;
            this.Tree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeBeforeLabelEdit);
            this.Tree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeAfterLabelEdit);
            this.Tree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TreeItemDrag);
            this.Tree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeBeforeSelect);
            this.Tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeAfterSelect);
            this.Tree.DragDrop += new System.Windows.Forms.DragEventHandler(this.TreeDragDrop);
            this.Tree.DragEnter += new System.Windows.Forms.DragEventHandler(this.TreeDragEnter);
            this.Tree.DragOver += new System.Windows.Forms.DragEventHandler(this.TreeDragOver);
            this.Tree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeMouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToolStripMenuItem,
            this.aDDToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.uPDATEToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuStrip1.Size = new System.Drawing.Size(227, 92);
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            this.installToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.installToolStripMenuItem.Text = "Install module from Repository";
            this.installToolStripMenuItem.Click += new System.EventHandler(this.InstallToolStripMenuItemClick);
            // 
            // aDDToolStripMenuItem
            // 
            this.aDDToolStripMenuItem.Name = "aDDToolStripMenuItem";
            this.aDDToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.aDDToolStripMenuItem.Text = "Manual add installed module";
            this.aDDToolStripMenuItem.Click += new System.EventHandler(this.AddToolStripMenuItemClick);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.removeToolStripMenuItem.Text = "Remove module";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItemClick);
            // 
            // uPDATEToolStripMenuItem
            // 
            this.uPDATEToolStripMenuItem.Name = "uPDATEToolStripMenuItem";
            this.uPDATEToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.uPDATEToolStripMenuItem.Text = "Update module from Repository";
            this.uPDATEToolStripMenuItem.Click += new System.EventHandler(this.UpdateToolStripMenuItemClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Timer");
            this.imageList1.Images.SetKeyName(1, "Spool");
            this.imageList1.Images.SetKeyName(2, "Module");
            this.imageList1.Images.SetKeyName(3, "Parameter");
            this.imageList1.Images.SetKeyName(4, "Node");
            this.imageList1.Images.SetKeyName(5, "Disabled");
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.jeepServiceToolStripMenuItem,
            this.liveUpdateServiceStripMenuItem,
            this.logToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1118, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileSaveToolStripMenuItem,
            this.toolStripSeparator1,
            this.FileExitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.fileToolStripMenuItem.Text = "&FILE";
            // 
            // FileSaveToolStripMenuItem
            // 
            this.FileSaveToolStripMenuItem.Name = "FileSaveToolStripMenuItem";
            this.FileSaveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.FileSaveToolStripMenuItem.Text = "&Save";
            this.FileSaveToolStripMenuItem.Click += new System.EventHandler(this.FileSaveToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(95, 6);
            // 
            // FileExitToolStripMenuItem
            // 
            this.FileExitToolStripMenuItem.Name = "FileExitToolStripMenuItem";
            this.FileExitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.FileExitToolStripMenuItem.Text = "&Exit";
            this.FileExitToolStripMenuItem.Click += new System.EventHandler(this.FileExitToolStripMenuItemClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editReloadToolStripMenuItem,
            this.toolStripSeparator2,
            this.editPreferencesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.editToolStripMenuItem.Text = "&EDIT";
            // 
            // editReloadToolStripMenuItem
            // 
            this.editReloadToolStripMenuItem.Name = "editReloadToolStripMenuItem";
            this.editReloadToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.editReloadToolStripMenuItem.Text = "Reload";
            this.editReloadToolStripMenuItem.Click += new System.EventHandler(this.EditReloadToolStripMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(141, 6);
            // 
            // editPreferencesToolStripMenuItem
            // 
            this.editPreferencesToolStripMenuItem.Name = "editPreferencesToolStripMenuItem";
            this.editPreferencesToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.editPreferencesToolStripMenuItem.Text = "Preferences...";
            this.editPreferencesToolStripMenuItem.Click += new System.EventHandler(this.EditPreferencesToolStripMenuItemClick);
            // 
            // jeepServiceToolStripMenuItem
            // 
            this.jeepServiceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jeepServiceInstallToolStripMenuItem,
            this.jeepServiceUninstallToolStripMenuItem,
            this.jeepServiceStartToolStripMenuItem,
            this.jeepServiceRestartToolStripMenuItem,
            this.jeepServiceStopToolStripMenuItem,
            this.jeepServiceStartConsoleToolStripMenuItem,
            this.toolStripSeparator4,
            this.jeepServiceConfigurationToolStripMenuItem,
            this.jeepServiceUpdateToolStripMenuItem});
            this.jeepServiceToolStripMenuItem.Name = "jeepServiceToolStripMenuItem";
            this.jeepServiceToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.jeepServiceToolStripMenuItem.Text = "JEEP &SERVICE";
            // 
            // jeepServiceInstallToolStripMenuItem
            // 
            this.jeepServiceInstallToolStripMenuItem.Name = "jeepServiceInstallToolStripMenuItem";
            this.jeepServiceInstallToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceInstallToolStripMenuItem.Text = "Install";
            this.jeepServiceInstallToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceInstallToolStripMenuItemClick);
            // 
            // jeepServiceUninstallToolStripMenuItem
            // 
            this.jeepServiceUninstallToolStripMenuItem.Name = "jeepServiceUninstallToolStripMenuItem";
            this.jeepServiceUninstallToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceUninstallToolStripMenuItem.Text = "Uninstall";
            this.jeepServiceUninstallToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceUninstallToolStripMenuItemClick);
            // 
            // jeepServiceStartToolStripMenuItem
            // 
            this.jeepServiceStartToolStripMenuItem.Name = "jeepServiceStartToolStripMenuItem";
            this.jeepServiceStartToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceStartToolStripMenuItem.Text = "Start";
            this.jeepServiceStartToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceStartToolStripMenuItemClick);
            // 
            // jeepServiceRestartToolStripMenuItem
            // 
            this.jeepServiceRestartToolStripMenuItem.Name = "jeepServiceRestartToolStripMenuItem";
            this.jeepServiceRestartToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceRestartToolStripMenuItem.Text = "Restart";
            this.jeepServiceRestartToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceRestartToolStripMenuItemClick);
            // 
            // jeepServiceStopToolStripMenuItem
            // 
            this.jeepServiceStopToolStripMenuItem.Name = "jeepServiceStopToolStripMenuItem";
            this.jeepServiceStopToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceStopToolStripMenuItem.Text = "Stop";
            this.jeepServiceStopToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceStopToolStripMenuItemClick);
            // 
            // jeepServiceStartConsoleToolStripMenuItem
            // 
            this.jeepServiceStartConsoleToolStripMenuItem.Name = "jeepServiceStartConsoleToolStripMenuItem";
            this.jeepServiceStartConsoleToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceStartConsoleToolStripMenuItem.Text = "Start Console";
            this.jeepServiceStartConsoleToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceStartConsoleToolStripMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(136, 6);
            // 
            // jeepServiceConfigurationToolStripMenuItem
            // 
            this.jeepServiceConfigurationToolStripMenuItem.Name = "jeepServiceConfigurationToolStripMenuItem";
            this.jeepServiceConfigurationToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceConfigurationToolStripMenuItem.Text = "Configuration";
            this.jeepServiceConfigurationToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceConfigurationToolStripMenuItemClick);
            // 
            // jeepServiceUpdateToolStripMenuItem
            // 
            this.jeepServiceUpdateToolStripMenuItem.Name = "jeepServiceUpdateToolStripMenuItem";
            this.jeepServiceUpdateToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.jeepServiceUpdateToolStripMenuItem.Text = "Update";
            this.jeepServiceUpdateToolStripMenuItem.Click += new System.EventHandler(this.JeepServiceUpdateToolStripMenuItemClick);
            // 
            // liveUpdateServiceStripMenuItem
            // 
            this.liveUpdateServiceStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.liveUpdateServiceInstallStripMenuItem,
            this.liveUpdateServiceUninstallStripMenuItem,
            this.liveUpdateServiceStartStripMenuItem,
            this.liveUpdateServiceRestartToolStripMenuItem,
            this.liveUpdateStopStripMenuItem,
            this.liveUpdateServiceStartConsoleStripMenuItem,
            this.toolStripSeparator3,
            this.liveUpdateServiceConfigurationToolStripMenuItem,
            this.liveUpdateUpdateToolStripMenuItem});
            this.liveUpdateServiceStripMenuItem.Name = "liveUpdateServiceStripMenuItem";
            this.liveUpdateServiceStripMenuItem.Size = new System.Drawing.Size(127, 20);
            this.liveUpdateServiceStripMenuItem.Text = "LIVE UPDATE SER&VICE";
            // 
            // liveUpdateServiceInstallStripMenuItem
            // 
            this.liveUpdateServiceInstallStripMenuItem.Name = "liveUpdateServiceInstallStripMenuItem";
            this.liveUpdateServiceInstallStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateServiceInstallStripMenuItem.Text = "Install";
            this.liveUpdateServiceInstallStripMenuItem.Click += new System.EventHandler(this.LiveUpdateServiceInstallStripMenuItemClick);
            // 
            // liveUpdateServiceUninstallStripMenuItem
            // 
            this.liveUpdateServiceUninstallStripMenuItem.Name = "liveUpdateServiceUninstallStripMenuItem";
            this.liveUpdateServiceUninstallStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateServiceUninstallStripMenuItem.Text = "Uninstall";
            this.liveUpdateServiceUninstallStripMenuItem.Click += new System.EventHandler(this.LiveUpdateServiceUninstallStripMenuItemClick);
            // 
            // liveUpdateServiceStartStripMenuItem
            // 
            this.liveUpdateServiceStartStripMenuItem.Name = "liveUpdateServiceStartStripMenuItem";
            this.liveUpdateServiceStartStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateServiceStartStripMenuItem.Text = "Start";
            this.liveUpdateServiceStartStripMenuItem.Click += new System.EventHandler(this.LiveUpdateServiceStartStripMenuItemClick);
            // 
            // liveUpdateServiceRestartToolStripMenuItem
            // 
            this.liveUpdateServiceRestartToolStripMenuItem.Name = "liveUpdateServiceRestartToolStripMenuItem";
            this.liveUpdateServiceRestartToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateServiceRestartToolStripMenuItem.Text = "Restart";
            // 
            // liveUpdateStopStripMenuItem
            // 
            this.liveUpdateStopStripMenuItem.Name = "liveUpdateStopStripMenuItem";
            this.liveUpdateStopStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateStopStripMenuItem.Text = "Stop";
            this.liveUpdateStopStripMenuItem.Click += new System.EventHandler(this.LiveUpdateStopStripMenuItemClick);
            // 
            // liveUpdateServiceStartConsoleStripMenuItem
            // 
            this.liveUpdateServiceStartConsoleStripMenuItem.Name = "liveUpdateServiceStartConsoleStripMenuItem";
            this.liveUpdateServiceStartConsoleStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateServiceStartConsoleStripMenuItem.Text = "Start Console";
            this.liveUpdateServiceStartConsoleStripMenuItem.Click += new System.EventHandler(this.LiveUpdateServiceStartConsoleStripMenuItemClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(136, 6);
            // 
            // liveUpdateServiceConfigurationToolStripMenuItem
            // 
            this.liveUpdateServiceConfigurationToolStripMenuItem.Name = "liveUpdateServiceConfigurationToolStripMenuItem";
            this.liveUpdateServiceConfigurationToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateServiceConfigurationToolStripMenuItem.Text = "Configuration";
            this.liveUpdateServiceConfigurationToolStripMenuItem.Click += new System.EventHandler(this.LiveUpdateServiceConfigurationToolStripMenuItemClick);
            // 
            // liveUpdateUpdateToolStripMenuItem
            // 
            this.liveUpdateUpdateToolStripMenuItem.Name = "liveUpdateUpdateToolStripMenuItem";
            this.liveUpdateUpdateToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.liveUpdateUpdateToolStripMenuItem.Text = "Update";
            this.liveUpdateUpdateToolStripMenuItem.Click += new System.EventHandler(this.LiveUpdateUpdateToolStripMenuItemClick);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logOpenConfigToolStripMenuItem,
            this.logOpenFolderToolStripMenuItem,
            this.logMonitorToolStripMenuItem,
            this.logClearToolStripMenuItem});
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.logToolStripMenuItem.Text = "&LOG";
            // 
            // logOpenConfigToolStripMenuItem
            // 
            this.logOpenConfigToolStripMenuItem.Name = "logOpenConfigToolStripMenuItem";
            this.logOpenConfigToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.logOpenConfigToolStripMenuItem.Text = "Open Config";
            this.logOpenConfigToolStripMenuItem.Click += new System.EventHandler(this.LogOpenConfigToolStripMenuItemClick);
            // 
            // logOpenFolderToolStripMenuItem
            // 
            this.logOpenFolderToolStripMenuItem.Name = "logOpenFolderToolStripMenuItem";
            this.logOpenFolderToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.logOpenFolderToolStripMenuItem.Text = "Open Folder";
            this.logOpenFolderToolStripMenuItem.Click += new System.EventHandler(this.LogOpenFolderToolStripMenuItemClick);
            // 
            // logMonitorToolStripMenuItem
            // 
            this.logMonitorToolStripMenuItem.Name = "logMonitorToolStripMenuItem";
            this.logMonitorToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.logMonitorToolStripMenuItem.Text = "Monitor";
            this.logMonitorToolStripMenuItem.Click += new System.EventHandler(this.LogMonitorToolStripMenuItemClick);
            // 
            // logClearToolStripMenuItem
            // 
            this.logClearToolStripMenuItem.Name = "logClearToolStripMenuItem";
            this.logClearToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.logClearToolStripMenuItem.Text = "Clear";
            this.logClearToolStripMenuItem.Click += new System.EventHandler(this.LogClearToolStripMenuItemClick);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpUpdatesToolStripMenuItem,
            this.helpAboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.helpToolStripMenuItem.Text = "&HELP";
            // 
            // helpUpdatesToolStripMenuItem
            // 
            this.helpUpdatesToolStripMenuItem.Name = "helpUpdatesToolStripMenuItem";
            this.helpUpdatesToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.helpUpdatesToolStripMenuItem.Text = "Updates";
            this.helpUpdatesToolStripMenuItem.Click += new System.EventHandler(this.HelpUpdatesToolStripMenuItemClick);
            // 
            // helpAboutToolStripMenuItem
            // 
            this.helpAboutToolStripMenuItem.Name = "helpAboutToolStripMenuItem";
            this.helpAboutToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.helpAboutToolStripMenuItem.Text = "About";
            this.helpAboutToolStripMenuItem.Click += new System.EventHandler(this.HelpAboutToolStripMenuItemClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jeepServiceStatusLabel,
            this.liveUpdateStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 561);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1118, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // jeepServiceStatusLabel
            // 
            this.jeepServiceStatusLabel.Name = "jeepServiceStatusLabel";
            this.jeepServiceStatusLabel.Size = new System.Drawing.Size(119, 17);
            this.jeepServiceStatusLabel.Text = "jeepServiceStatusLabel";
            // 
            // liveUpdateStatusLabel
            // 
            this.liveUpdateStatusLabel.Name = "liveUpdateStatusLabel";
            this.liveUpdateStatusLabel.Size = new System.Drawing.Size(114, 17);
            this.liveUpdateStatusLabel.Text = "liveUpdateStatusLabel";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.imgStatusQuestion);
            this.panel1.Controls.Add(this.imgStatusPause);
            this.panel1.Controls.Add(this.imgStatusPlay);
            this.panel1.Controls.Add(this.imgInstallationOFF);
            this.panel1.Controls.Add(this.imgInstallationON);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 522);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 39);
            this.panel1.TabIndex = 6;
            // 
            // imgStatusQuestion
            // 
            this.imgStatusQuestion.Image = global::VecompSoftware.JeepDashboard.Properties.Resources.emotion_question;
            this.imgStatusQuestion.Location = new System.Drawing.Point(41, 3);
            this.imgStatusQuestion.Name = "imgStatusQuestion";
            this.imgStatusQuestion.Size = new System.Drawing.Size(32, 32);
            this.imgStatusQuestion.TabIndex = 1;
            this.imgStatusQuestion.TabStop = false;
            // 
            // imgStatusPause
            // 
            this.imgStatusPause.Image = global::VecompSoftware.JeepDashboard.Properties.Resources.control_pause_blue1;
            this.imgStatusPause.Location = new System.Drawing.Point(41, 3);
            this.imgStatusPause.Name = "imgStatusPause";
            this.imgStatusPause.Size = new System.Drawing.Size(32, 32);
            this.imgStatusPause.TabIndex = 0;
            this.imgStatusPause.TabStop = false;
            // 
            // imgStatusPlay
            // 
            this.imgStatusPlay.Image = global::VecompSoftware.JeepDashboard.Properties.Resources.control_play_blue1;
            this.imgStatusPlay.Location = new System.Drawing.Point(41, 3);
            this.imgStatusPlay.Name = "imgStatusPlay";
            this.imgStatusPlay.Size = new System.Drawing.Size(32, 32);
            this.imgStatusPlay.TabIndex = 0;
            this.imgStatusPlay.TabStop = false;
            // 
            // imgInstallationOFF
            // 
            this.imgInstallationOFF.Image = global::VecompSoftware.JeepDashboard.Properties.Resources.server_delete;
            this.imgInstallationOFF.Location = new System.Drawing.Point(3, 3);
            this.imgInstallationOFF.Name = "imgInstallationOFF";
            this.imgInstallationOFF.Size = new System.Drawing.Size(32, 32);
            this.imgInstallationOFF.TabIndex = 0;
            this.imgInstallationOFF.TabStop = false;
            // 
            // imgInstallationON
            // 
            this.imgInstallationON.Image = global::VecompSoftware.JeepDashboard.Properties.Resources.server;
            this.imgInstallationON.Location = new System.Drawing.Point(3, 3);
            this.imgInstallationON.Name = "imgInstallationON";
            this.imgInstallationON.Size = new System.Drawing.Size(32, 32);
            this.imgInstallationON.TabIndex = 0;
            this.imgInstallationON.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(375, 24);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(10, 537);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // processStarter
            // 
            this.processStarter.WorkerSupportsCancellation = true;
            this.processStarter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ProcessStarterDoWork);
            this.processStarter.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ProcessStarterRunWorkerCompleted);
            // 
            // statusBarRefreshTimer
            // 
            this.statusBarRefreshTimer.Enabled = true;
            this.statusBarRefreshTimer.Interval = 1000;
            this.statusBarRefreshTimer.Tick += new System.EventHandler(this.StatusBarRefreshTimerTick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 583);
            this.Controls.Add(this.Tree);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Jeep Dashboard";
            this.Load += new System.EventHandler(this.Form1Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusPause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusPlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgInstallationOFF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgInstallationON)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid grid;
        private System.Windows.Forms.TreeView Tree;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceInstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceUninstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceStopToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel jeepServiceStatusLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aDDToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logOpenConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logClearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceStartConsoleToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox imgInstallationOFF;
        private System.Windows.Forms.PictureBox imgInstallationON;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.PictureBox imgStatusPause;
        private System.Windows.Forms.PictureBox imgStatusPlay;
        private System.Windows.Forms.PictureBox imgStatusQuestion;
        private System.Windows.Forms.ToolStripMenuItem logOpenFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editPreferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editReloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateServiceStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateServiceInstallStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateServiceUninstallStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateServiceStartStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateStopStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateServiceStartConsoleStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateServiceConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceRestartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uPDATEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeepServiceUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel liveUpdateStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateServiceRestartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpAboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpUpdatesToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker processStarter;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.Timer statusBarRefreshTimer;
    }
}

