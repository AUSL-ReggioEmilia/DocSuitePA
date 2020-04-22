namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UCArchiveDetail
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCommandColumn gridViewCommandColumn1 = new Telerik.WinControls.UI.GridViewCommandColumn();
            Telerik.WinControls.UI.GridViewCommandColumn gridViewCommandColumn2 = new Telerik.WinControls.UI.GridViewCommandColumn();
            this.radPanelTitle = new Telerik.WinControls.UI.RadPanel();
            this.btUpdate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btCancel = new System.Windows.Forms.Button();
            this.cbSecurity = new System.Windows.Forms.CheckBox();
            this.lbTransitRequired = new System.Windows.Forms.Label();
            this.lbPathPreservationRequired = new System.Windows.Forms.Label();
            this.ckTransitEnabled = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMaxCache = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbAutoVersion = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAuthorizationAssembly = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAuthorizationClassName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbLegal = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUpperCACHE = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLowerCache = new System.Windows.Forms.NumericUpDown();
            this.txtLastIdBiblos = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPathTransito = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtPathPreservation = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbDocumentsType = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.grpServers = new Telerik.WinControls.UI.RadGroupBox();
            this.gvServers = new Telerik.WinControls.UI.RadGridView();
            this.btnAddServer = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxCache)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpperCACHE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowerCache)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastIdBiblos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpServers)).BeginInit();
            this.grpServers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvServers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddServer)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanelTitle
            // 
            this.radPanelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radPanelTitle.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.radPanelTitle.Location = new System.Drawing.Point(6, 3);
            this.radPanelTitle.Name = "radPanelTitle";
            this.radPanelTitle.Size = new System.Drawing.Size(782, 46);
            this.radPanelTitle.TabIndex = 10;
            this.radPanelTitle.Text = "Archive";
            // 
            // btUpdate
            // 
            this.btUpdate.AutoSize = true;
            this.btUpdate.Location = new System.Drawing.Point(585, 58);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(104, 23);
            this.btUpdate.TabIndex = 10;
            this.btUpdate.Text = "Update";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.storageUpdate_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(551, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 19);
            this.label5.TabIndex = 41;
            this.label5.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(121, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(170, 65);
            this.txtName.MaxLength = 255;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(375, 20);
            this.txtName.TabIndex = 0;
            // 
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(585, 87);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 11;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // cbSecurity
            // 
            this.cbSecurity.AutoSize = true;
            this.cbSecurity.Location = new System.Drawing.Point(170, 391);
            this.cbSecurity.Name = "cbSecurity";
            this.cbSecurity.Size = new System.Drawing.Size(15, 14);
            this.cbSecurity.TabIndex = 9;
            this.cbSecurity.UseVisualStyleBackColor = true;
            // 
            // lbTransitRequired
            // 
            this.lbTransitRequired.AutoSize = true;
            this.lbTransitRequired.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTransitRequired.ForeColor = System.Drawing.Color.Red;
            this.lbTransitRequired.Location = new System.Drawing.Point(551, 108);
            this.lbTransitRequired.Name = "lbTransitRequired";
            this.lbTransitRequired.Size = new System.Drawing.Size(15, 19);
            this.lbTransitRequired.TabIndex = 65;
            this.lbTransitRequired.Text = "*";
            // 
            // lbPathPreservationRequired
            // 
            this.lbPathPreservationRequired.AutoSize = true;
            this.lbPathPreservationRequired.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPathPreservationRequired.ForeColor = System.Drawing.Color.Red;
            this.lbPathPreservationRequired.Location = new System.Drawing.Point(551, 161);
            this.lbPathPreservationRequired.Name = "lbPathPreservationRequired";
            this.lbPathPreservationRequired.Size = new System.Drawing.Size(15, 19);
            this.lbPathPreservationRequired.TabIndex = 68;
            this.lbPathPreservationRequired.Text = "*";
            // 
            // ckTransitEnabled
            // 
            this.ckTransitEnabled.AutoSize = true;
            this.ckTransitEnabled.Location = new System.Drawing.Point(171, 91);
            this.ckTransitEnabled.Name = "ckTransitEnabled";
            this.ckTransitEnabled.Size = new System.Drawing.Size(15, 14);
            this.ckTransitEnabled.TabIndex = 69;
            this.ckTransitEnabled.UseVisualStyleBackColor = true;
            this.ckTransitEnabled.CheckedChanged += new System.EventHandler(this.ckTransitEnabled_CheckedChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(63, 92);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 13);
            this.label16.TabIndex = 70;
            this.label16.Text = "Transit Enabled:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(52, 224);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Max Cache ( MB ):";
            // 
            // txtMaxCache
            // 
            this.txtMaxCache.Location = new System.Drawing.Point(170, 217);
            this.txtMaxCache.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.txtMaxCache.Name = "txtMaxCache";
            this.txtMaxCache.Size = new System.Drawing.Size(94, 20);
            this.txtMaxCache.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(75, 297);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Last Id Biblos:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(80, 322);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Auto Version:";
            // 
            // cbAutoVersion
            // 
            this.cbAutoVersion.AutoSize = true;
            this.cbAutoVersion.Location = new System.Drawing.Point(170, 321);
            this.cbAutoVersion.Name = "cbAutoVersion";
            this.cbAutoVersion.Size = new System.Drawing.Size(15, 14);
            this.cbAutoVersion.TabIndex = 6;
            this.cbAutoVersion.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(22, 348);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(141, 13);
            this.label12.TabIndex = 51;
            this.label12.Text = "Authorization Assembly:";
            // 
            // txtAuthorizationAssembly
            // 
            this.txtAuthorizationAssembly.Location = new System.Drawing.Point(170, 341);
            this.txtAuthorizationAssembly.MaxLength = 255;
            this.txtAuthorizationAssembly.Name = "txtAuthorizationAssembly";
            this.txtAuthorizationAssembly.Size = new System.Drawing.Size(375, 20);
            this.txtAuthorizationAssembly.TabIndex = 7;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(9, 370);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(155, 13);
            this.label11.TabIndex = 53;
            this.label11.Text = "Authorization Class Name:";
            // 
            // txtAuthorizationClassName
            // 
            this.txtAuthorizationClassName.Location = new System.Drawing.Point(170, 367);
            this.txtAuthorizationClassName.MaxLength = 50;
            this.txtAuthorizationClassName.Name = "txtAuthorizationClassName";
            this.txtAuthorizationClassName.Size = new System.Drawing.Size(375, 20);
            this.txtAuthorizationClassName.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(121, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "Legal:";
            // 
            // cbLegal
            // 
            this.cbLegal.AutoSize = true;
            this.cbLegal.Location = new System.Drawing.Point(170, 139);
            this.cbLegal.Name = "cbLegal";
            this.cbLegal.Size = new System.Drawing.Size(15, 14);
            this.cbLegal.TabIndex = 1;
            this.cbLegal.UseVisualStyleBackColor = true;
            this.cbLegal.CheckedChanged += new System.EventHandler(this.cbLegal_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(40, 250);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 57;
            this.label3.Text = "Upper Cache ( MB ):";
            // 
            // txtUpperCACHE
            // 
            this.txtUpperCACHE.Location = new System.Drawing.Point(170, 243);
            this.txtUpperCACHE.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.txtUpperCACHE.Name = "txtUpperCACHE";
            this.txtUpperCACHE.Size = new System.Drawing.Size(94, 20);
            this.txtUpperCACHE.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(40, 276);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 13);
            this.label7.TabIndex = 59;
            this.label7.Text = "Lower Cache ( MB ):";
            // 
            // txtLowerCache
            // 
            this.txtLowerCache.Location = new System.Drawing.Point(170, 269);
            this.txtLowerCache.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.txtLowerCache.Name = "txtLowerCache";
            this.txtLowerCache.Size = new System.Drawing.Size(94, 20);
            this.txtLowerCache.TabIndex = 4;
            // 
            // txtLastIdBiblos
            // 
            this.txtLastIdBiblos.Location = new System.Drawing.Point(170, 295);
            this.txtLastIdBiblos.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.txtLastIdBiblos.Name = "txtLastIdBiblos";
            this.txtLastIdBiblos.Size = new System.Drawing.Size(94, 20);
            this.txtLastIdBiblos.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(106, 392);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 62;
            this.label8.Text = "Security:";
            // 
            // txtPathTransito
            // 
            this.txtPathTransito.Location = new System.Drawing.Point(169, 111);
            this.txtPathTransito.MaxLength = 255;
            this.txtPathTransito.Name = "txtPathTransito";
            this.txtPathTransito.Size = new System.Drawing.Size(375, 20);
            this.txtPathTransito.TabIndex = 63;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(83, 118);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 13);
            this.label13.TabIndex = 64;
            this.label13.Text = "Path Transit:";
            // 
            // txtPathPreservation
            // 
            this.txtPathPreservation.Location = new System.Drawing.Point(169, 159);
            this.txtPathPreservation.MaxLength = 255;
            this.txtPathPreservation.Name = "txtPathPreservation";
            this.txtPathPreservation.Size = new System.Drawing.Size(375, 20);
            this.txtPathPreservation.TabIndex = 66;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(51, 165);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(112, 13);
            this.label15.TabIndex = 67;
            this.label15.Text = "Path Preservation:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(20, 192);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 13);
            this.label9.TabIndex = 71;
            this.label9.Text = "Fiscal Documents Type:";
            // 
            // cbDocumentsType
            // 
            this.cbDocumentsType.FormattingEnabled = true;
            this.cbDocumentsType.Location = new System.Drawing.Point(169, 189);
            this.cbDocumentsType.Name = "cbDocumentsType";
            this.cbDocumentsType.Size = new System.Drawing.Size(376, 21);
            this.cbDocumentsType.TabIndex = 72;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Red;
            this.label14.Location = new System.Drawing.Point(166, 423);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(581, 13);
            this.label14.TabIndex = 73;
            this.label14.Text = "N.B. The path must be a local path or a full specify shared path like \\\\192.168.1" +
    ".1\\c$\\BiblosDS2010";
            // 
            // grpServers
            // 
            this.grpServers.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.grpServers.Controls.Add(this.gvServers);
            this.grpServers.Controls.Add(this.btnAddServer);
            this.grpServers.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpServers.FooterImageIndex = -1;
            this.grpServers.FooterImageKey = "";
            this.grpServers.HeaderImageIndex = -1;
            this.grpServers.HeaderImageKey = "";
            this.grpServers.HeaderMargin = new System.Windows.Forms.Padding(0);
            this.grpServers.HeaderText = "Servers";
            this.grpServers.Location = new System.Drawing.Point(0, 439);
            this.grpServers.Name = "grpServers";
            this.grpServers.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            // 
            // 
            // 
            this.grpServers.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.grpServers.Size = new System.Drawing.Size(794, 163);
            this.grpServers.TabIndex = 74;
            this.grpServers.Text = "Servers";
            // 
            // gvServers
            // 
            this.gvServers.BackColor = System.Drawing.Color.White;
            this.gvServers.Cursor = System.Windows.Forms.Cursors.Default;
            this.gvServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvServers.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.gvServers.ForeColor = System.Drawing.Color.Black;
            this.gvServers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gvServers.Location = new System.Drawing.Point(2, 42);
            // 
            // gvServers
            // 
            this.gvServers.MasterTemplate.AllowAddNewRow = false;
            this.gvServers.MasterTemplate.AllowColumnReorder = false;
            this.gvServers.MasterTemplate.AutoGenerateColumns = false;
            gridViewTextBoxColumn1.FieldName = "IdServer";
            gridViewTextBoxColumn1.HeaderText = "ServerId";
            gridViewTextBoxColumn1.IsVisible = false;
            gridViewTextBoxColumn1.Name = "colServerId";
            gridViewTextBoxColumn2.FieldName = "Server.ServerName";
            gridViewTextBoxColumn2.HeaderText = "Name";
            gridViewTextBoxColumn2.Name = "colServerName";
            gridViewTextBoxColumn2.ReadOnly = true;
            gridViewTextBoxColumn2.Width = 300;
            gridViewCheckBoxColumn1.FieldName = "TransitEnabled";
            gridViewCheckBoxColumn1.HeaderText = "Transit Enabled";
            gridViewCheckBoxColumn1.MinWidth = 20;
            gridViewCheckBoxColumn1.Name = "colEnabled";
            gridViewCheckBoxColumn1.ReadOnly = true;
            gridViewCheckBoxColumn1.Width = 85;
            gridViewTextBoxColumn3.FieldName = "TransitPath";
            gridViewTextBoxColumn3.HeaderText = "Transit Path";
            gridViewTextBoxColumn3.Name = "colTransitPath";
            gridViewTextBoxColumn3.ReadOnly = true;
            gridViewTextBoxColumn3.Width = 300;
            gridViewCommandColumn1.HeaderText = "Modify";
            gridViewCommandColumn1.Name = "colModify";
            gridViewCommandColumn1.Width = 43;
            gridViewCommandColumn2.HeaderText = "Delete";
            gridViewCommandColumn2.Name = "colDelete";
            gridViewCommandColumn2.Width = 40;
            this.gvServers.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewCheckBoxColumn1,
            gridViewTextBoxColumn3,
            gridViewCommandColumn1,
            gridViewCommandColumn2});
            this.gvServers.MasterTemplate.EnableAlternatingRowColor = true;
            this.gvServers.Name = "gvServers";
            this.gvServers.ReadOnly = true;
            this.gvServers.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.gvServers.RootElement.ForeColor = System.Drawing.Color.Black;
            this.gvServers.ShowGroupPanel = false;
            this.gvServers.Size = new System.Drawing.Size(790, 119);
            this.gvServers.TabIndex = 2;
            this.gvServers.CommandCellClick += new Telerik.WinControls.UI.CommandCellClickEventHandler(this.gvServers_CommandCellClick);
            // 
            // btnAddServer
            // 
            this.btnAddServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddServer.Image = global::BiblosDs.Document.AdminCentral.Properties.Resources.BindingNavigatorAddNewItem_Image1;
            this.btnAddServer.Location = new System.Drawing.Point(2, 18);
            this.btnAddServer.Name = "btnAddServer";
            this.btnAddServer.Size = new System.Drawing.Size(790, 24);
            this.btnAddServer.TabIndex = 0;
            this.btnAddServer.Text = "Add New Server Configuration";
            this.btnAddServer.Click += new System.EventHandler(this.btnAddServer_Click);
            // 
            // UCArchiveDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpServers);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.cbDocumentsType);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ckTransitEnabled);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lbPathPreservationRequired);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtPathPreservation);
            this.Controls.Add(this.lbTransitRequired);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtPathTransito);
            this.Controls.Add(this.cbSecurity);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtLastIdBiblos);
            this.Controls.Add(this.txtLowerCache);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtUpperCACHE);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbLegal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAuthorizationClassName);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtAuthorizationAssembly);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.cbAutoVersion);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMaxCache);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.radPanelTitle);
            this.Name = "UCArchiveDetail";
            this.Size = new System.Drawing.Size(794, 602);
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxCache)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpperCACHE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowerCache)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastIdBiblos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpServers)).EndInit();
            this.grpServers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvServers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddServer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanelTitle;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.CheckBox cbSecurity;
        private System.Windows.Forms.Label lbTransitRequired;
        private System.Windows.Forms.Label lbPathPreservationRequired;
        private System.Windows.Forms.CheckBox ckTransitEnabled;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtMaxCache;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbAutoVersion;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAuthorizationAssembly;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtAuthorizationClassName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbLegal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown txtUpperCACHE;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown txtLowerCache;
        private System.Windows.Forms.NumericUpDown txtLastIdBiblos;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPathTransito;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtPathPreservation;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbDocumentsType;
        private System.Windows.Forms.Label label14;
        private Telerik.WinControls.UI.RadGroupBox grpServers;
        private Telerik.WinControls.UI.RadGridView gvServers;
        private Telerik.WinControls.UI.RadButton btnAddServer;
    }
}
