namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    partial class UcAttributeDetail
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
            this.components = new System.ComponentModel.Container();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.radPanelTitle = new Telerik.WinControls.UI.RadPanel();
            this.btUpdate = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.label14 = new System.Windows.Forms.Label();
            this.txtKeyOrder = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.ckAutoInc = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ckMainDate = new System.Windows.Forms.CheckBox();
            this.txtFormat = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtValidation = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtKeyFormat = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtKeyFilter = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ckRequired = new System.Windows.Forms.CheckBox();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.documentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.ckUnique = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.ckPrinaryKey = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.cbAttributeGroup = new System.Windows.Forms.ComboBox();
            this.ckVisible = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtDefaultValue = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.lblMaxLenght = new System.Windows.Forms.Label();
            this.txtMaxLenght = new System.Windows.Forms.NumericUpDown();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.ckIsSectional = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.ckRequiredForPreservation = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.ckVisibleForUser = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLenght)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
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
            this.radPanelTitle.Text = "Attribute";
            // 
            // btUpdate
            // 
            this.btUpdate.AutoSize = true;
            this.btUpdate.Location = new System.Drawing.Point(572, 67);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(104, 23);
            this.btUpdate.TabIndex = 13;
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
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(572, 96);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 14;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(93, 275);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 13);
            this.label14.TabIndex = 99;
            this.label14.Text = "Key Order:";
            // 
            // txtKeyOrder
            // 
            this.txtKeyOrder.Enabled = false;
            this.txtKeyOrder.Location = new System.Drawing.Point(170, 268);
            this.txtKeyOrder.Name = "txtKeyOrder";
            this.txtKeyOrder.Size = new System.Drawing.Size(120, 20);
            this.txtKeyOrder.TabIndex = 6;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(94, 446);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 13);
            this.label13.TabIndex = 97;
            this.label13.Text = "Auto inc.:";
            // 
            // ckAutoInc
            // 
            this.ckAutoInc.AutoSize = true;
            this.ckAutoInc.Location = new System.Drawing.Point(170, 447);
            this.ckAutoInc.Name = "ckAutoInc";
            this.ckAutoInc.Size = new System.Drawing.Size(15, 14);
            this.ckAutoInc.TabIndex = 14;
            this.ckAutoInc.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(87, 427);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 13);
            this.label11.TabIndex = 94;
            this.label11.Text = "Main Date:";
            // 
            // ckMainDate
            // 
            this.ckMainDate.AutoSize = true;
            this.ckMainDate.Location = new System.Drawing.Point(170, 427);
            this.ckMainDate.Name = "ckMainDate";
            this.ckMainDate.Size = new System.Drawing.Size(15, 14);
            this.ckMainDate.TabIndex = 12;
            this.ckMainDate.UseVisualStyleBackColor = true;
            // 
            // txtFormat
            // 
            this.txtFormat.Location = new System.Drawing.Point(170, 320);
            this.txtFormat.MaxLength = 255;
            this.txtFormat.Name = "txtFormat";
            this.txtFormat.Size = new System.Drawing.Size(375, 20);
            this.txtFormat.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(111, 327);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 13);
            this.label10.TabIndex = 90;
            this.label10.Text = "Format:";
            // 
            // txtValidation
            // 
            this.txtValidation.Location = new System.Drawing.Point(170, 294);
            this.txtValidation.MaxLength = 255;
            this.txtValidation.Name = "txtValidation";
            this.txtValidation.Size = new System.Drawing.Size(375, 20);
            this.txtValidation.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(93, 301);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 88;
            this.label9.Text = "Validation:";
            // 
            // txtKeyFormat
            // 
            this.txtKeyFormat.Enabled = false;
            this.txtKeyFormat.Location = new System.Drawing.Point(170, 242);
            this.txtKeyFormat.MaxLength = 255;
            this.txtKeyFormat.Name = "txtKeyFormat";
            this.txtKeyFormat.Size = new System.Drawing.Size(375, 20);
            this.txtKeyFormat.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(86, 249);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 86;
            this.label8.Text = "Key Format:";
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.DisplayMember = "Description";
            this.comboBoxMode.FormattingEnabled = true;
            this.comboBoxMode.Location = new System.Drawing.Point(170, 149);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(375, 21);
            this.comboBoxMode.TabIndex = 2;
            this.comboBoxMode.ValueMember = "IdMode";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(114, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 84;
            this.label7.Text = "Mode:";
            // 
            // txtKeyFilter
            // 
            this.txtKeyFilter.Enabled = false;
            this.txtKeyFilter.Location = new System.Drawing.Point(170, 216);
            this.txtKeyFilter.MaxLength = 255;
            this.txtKeyFilter.Name = "txtKeyFilter";
            this.txtKeyFilter.Size = new System.Drawing.Size(375, 20);
            this.txtKeyFilter.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(96, 223);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 82;
            this.label6.Text = "Key Filter:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(98, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 81;
            this.label1.Text = "Required:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(118, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 76;
            this.label3.Text = "Type:";
            // 
            // ckRequired
            // 
            this.ckRequired.AutoSize = true;
            this.ckRequired.Location = new System.Drawing.Point(170, 176);
            this.ckRequired.Name = "ckRequired";
            this.ckRequired.Size = new System.Drawing.Size(15, 14);
            this.ckRequired.TabIndex = 3;
            this.ckRequired.UseVisualStyleBackColor = true;
            this.ckRequired.CheckedChanged += new System.EventHandler(this.ckRequired_CheckedChanged);
            // 
            // comboBoxType
            // 
            this.comboBoxType.DisplayMember = "Name";
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(170, 122);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(375, 21);
            this.comboBoxType.TabIndex = 1;
            this.comboBoxType.ValueMember = "IdArchive";
            this.comboBoxType.SelectedValueChanged += new System.EventHandler(this.comboBoxType_SelectedValueChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(170, 70);
            this.txtName.MaxLength = 255;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(375, 20);
            this.txtName.TabIndex = 0;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(114, 77);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 78;
            this.label15.Text = "Name:";
            // 
            // documentBindingSource
            // 
            this.documentBindingSource.DataSource = typeof(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Storage);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(551, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 19);
            this.label2.TabIndex = 100;
            this.label2.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(551, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 19);
            this.label4.TabIndex = 101;
            this.label4.Text = "*";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(105, 467);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(51, 13);
            this.label16.TabIndex = 102;
            this.label16.Text = "Unique:";
            // 
            // ckUnique
            // 
            this.ckUnique.AutoSize = true;
            this.ckUnique.Location = new System.Drawing.Point(170, 467);
            this.ckUnique.Name = "ckUnique";
            this.ckUnique.Size = new System.Drawing.Size(15, 14);
            this.ckUnique.TabIndex = 15;
            this.ckUnique.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(83, 196);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(77, 13);
            this.label17.TabIndex = 104;
            this.label17.Text = "Primary Key:";
            // 
            // ckPrinaryKey
            // 
            this.ckPrinaryKey.AutoSize = true;
            this.ckPrinaryKey.Enabled = false;
            this.ckPrinaryKey.Location = new System.Drawing.Point(170, 196);
            this.ckPrinaryKey.Name = "ckPrinaryKey";
            this.ckPrinaryKey.Size = new System.Drawing.Size(15, 14);
            this.ckPrinaryKey.TabIndex = 103;
            this.ckPrinaryKey.UseVisualStyleBackColor = true;
            this.ckPrinaryKey.CheckedChanged += new System.EventHandler(this.ckPrinaryKey_CheckedChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(71, 351);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(89, 13);
            this.label18.TabIndex = 106;
            this.label18.Text = "AttributeGoup:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.Red;
            this.label19.Location = new System.Drawing.Point(551, 345);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(15, 19);
            this.label19.TabIndex = 107;
            this.label19.Text = "*";
            // 
            // cbAttributeGroup
            // 
            this.cbAttributeGroup.DisplayMember = "Description";
            this.cbAttributeGroup.FormattingEnabled = true;
            this.cbAttributeGroup.Location = new System.Drawing.Point(170, 346);
            this.cbAttributeGroup.Name = "cbAttributeGroup";
            this.cbAttributeGroup.Size = new System.Drawing.Size(375, 21);
            this.cbAttributeGroup.TabIndex = 9;
            this.cbAttributeGroup.ValueMember = "IdMode";
            this.cbAttributeGroup.SelectedValueChanged += new System.EventHandler(this.cbAttributeGroup_SelectedValueChanged);
            // 
            // ckVisible
            // 
            this.ckVisible.AutoSize = true;
            this.ckVisible.Checked = true;
            this.ckVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckVisible.Location = new System.Drawing.Point(170, 487);
            this.ckVisible.Name = "ckVisible";
            this.ckVisible.Size = new System.Drawing.Size(15, 14);
            this.ckVisible.TabIndex = 16;
            this.ckVisible.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(105, 488);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(48, 13);
            this.label20.TabIndex = 110;
            this.label20.Text = "Visible:";
            // 
            // txtDefaultValue
            // 
            this.txtDefaultValue.Location = new System.Drawing.Point(170, 376);
            this.txtDefaultValue.MaxLength = 255;
            this.txtDefaultValue.Name = "txtDefaultValue";
            this.txtDefaultValue.Size = new System.Drawing.Size(375, 20);
            this.txtDefaultValue.TabIndex = 10;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(68, 379);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(88, 13);
            this.label21.TabIndex = 116;
            this.label21.Text = "Default Value:";
            // 
            // lblMaxLenght
            // 
            this.lblMaxLenght.AutoSize = true;
            this.lblMaxLenght.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxLenght.Location = new System.Drawing.Point(83, 405);
            this.lblMaxLenght.Name = "lblMaxLenght";
            this.lblMaxLenght.Size = new System.Drawing.Size(73, 13);
            this.lblMaxLenght.TabIndex = 118;
            this.lblMaxLenght.Text = "MaxLenght:";
            this.lblMaxLenght.Visible = false;
            // 
            // txtMaxLenght
            // 
            this.txtMaxLenght.Location = new System.Drawing.Point(170, 401);
            this.txtMaxLenght.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txtMaxLenght.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.txtMaxLenght.Name = "txtMaxLenght";
            this.txtMaxLenght.Size = new System.Drawing.Size(120, 20);
            this.txtMaxLenght.TabIndex = 11;
            this.txtMaxLenght.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtMaxLenght.Visible = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(170, 96);
            this.txtDescription.MaxLength = 255;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(375, 20);
            this.txtDescription.TabIndex = 119;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(82, 101);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 120;
            this.label12.Text = "Description:";
            // 
            // ckIsSectional
            // 
            this.ckIsSectional.AutoSize = true;
            this.ckIsSectional.Location = new System.Drawing.Point(170, 507);
            this.ckIsSectional.Name = "ckIsSectional";
            this.ckIsSectional.Size = new System.Drawing.Size(15, 14);
            this.ckIsSectional.TabIndex = 121;
            this.ckIsSectional.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(75, 508);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(78, 13);
            this.label22.TabIndex = 122;
            this.label22.Text = "Is Sectional:";
            // 
            // ckRequiredForPreservation
            // 
            this.ckRequiredForPreservation.AutoSize = true;
            this.ckRequiredForPreservation.Location = new System.Drawing.Point(170, 529);
            this.ckRequiredForPreservation.Name = "ckRequiredForPreservation";
            this.ckRequiredForPreservation.Size = new System.Drawing.Size(15, 14);
            this.ckRequiredForPreservation.TabIndex = 123;
            this.ckRequiredForPreservation.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(-3, 530);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(155, 13);
            this.label23.TabIndex = 124;
            this.label23.Text = "Required for preservation:";
            // 
            // ckVisibleForUser
            // 
            this.ckVisibleForUser.AutoSize = true;
            this.ckVisibleForUser.Checked = true;
            this.ckVisibleForUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckVisibleForUser.Location = new System.Drawing.Point(170, 551);
            this.ckVisibleForUser.Name = "ckVisibleForUser";
            this.ckVisibleForUser.Size = new System.Drawing.Size(15, 14);
            this.ckVisibleForUser.TabIndex = 125;
            this.ckVisibleForUser.UseVisualStyleBackColor = true;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(57, 551);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(95, 13);
            this.label24.TabIndex = 126;
            this.label24.Text = "Visible for user:";
            // 
            // UcAttributeDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ckVisibleForUser);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.ckRequiredForPreservation);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.ckIsSectional);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtMaxLenght);
            this.Controls.Add(this.lblMaxLenght);
            this.Controls.Add(this.txtDefaultValue);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.ckVisible);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.cbAttributeGroup);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.ckPrinaryKey);
            this.Controls.Add(this.ckUnique);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtKeyOrder);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.ckAutoInc);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.ckMainDate);
            this.Controls.Add(this.txtFormat);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtValidation);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtKeyFormat);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxMode);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtKeyFilter);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ckRequired);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.radPanelTitle);
            this.Name = "UcAttributeDetail";
            this.Size = new System.Drawing.Size(794, 607);
            ((System.ComponentModel.ISupportInitialize)(this.radPanelTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.documentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLenght)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Telerik.WinControls.UI.RadPanel radPanelTitle;
        private System.Windows.Forms.BindingSource documentBindingSource;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown txtKeyOrder;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox ckAutoInc;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox ckMainDate;
        private System.Windows.Forms.TextBox txtFormat;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtValidation;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtKeyFormat;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtKeyFilter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ckRequired;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox ckUnique;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox ckPrinaryKey;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox cbAttributeGroup;
        private System.Windows.Forms.CheckBox ckVisible;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtDefaultValue;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblMaxLenght;
        private System.Windows.Forms.NumericUpDown txtMaxLenght;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox ckIsSectional;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox ckRequiredForPreservation;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.CheckBox ckVisibleForUser;
        private System.Windows.Forms.Label label24;
    }
}
