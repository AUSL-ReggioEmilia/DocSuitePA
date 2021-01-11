using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

using System.Security.Cryptography;
using Telerik.WinControls.UI;
using System.Threading;
using Telerik.WinControls;
using System.Data.SqlClient;
using System.Collections;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;


namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UcAttributeDetail : BaseAdminControls
    {
        #region Variabili private
        
        private BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute attribute;
        private Archive archive;
        private AttributeGroup attributeGroup;
        private BindingList<AttributeGroup> lsAttributeGroup;
        private BindingList<AttributeMode> attributeMode;

        #endregion

        public UcAttributeDetail(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            // Carica dati
            this.Load += new System.EventHandler(this.UCStorageDetail_Load);
        }

        private void UCStorageDetail_Load(object sender, EventArgs e)
        {
            CreateWaitDialog();

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Archive " + InputParameters["ArchiveName"].ToString(),
                                            "Attribute Creation"
                                });
                    this.btUpdate.Text = "Insert";
                    this.btCancel.Text = "Cancel";
                    break;
                case "modify":
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Archive " + InputParameters["ArchiveName"].ToString(),
                                            "Modify attribute"
                                });
                    this.btUpdate.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
            }
            
            // Caricamento tipo dato
            comboBoxType.Items.Clear();
            comboBoxType.Items.Add("DateTime");
            comboBoxType.Items.Add("Double");
            comboBoxType.Items.Add("Int64");
            comboBoxType.Items.Add("String");
            
            // Caricamento attributo
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    // In inserimento creo uno storage vuoto
                    //(DocumentArchive)e.Argument = new DocumentArchive();
                    attribute = new BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute();
                    break;
                case "modify":
                    // Se sto modificando carico i dati da modificare
                    attribute = Client.GetAttribute((Guid)InputParameters["ID"]);
                    break;
            }

            attributeMode = Client.GetAttributeModes();

            archive = Client.GetArchive((Guid)InputParameters["IdArchive"]);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                comboBoxMode.DataSource = attributeMode;

                SyncroMemoryDB(attribute, true);
            }
            else
            {
                TrapError(e.Error);
            }
            CloseWaitDialog();
        }

        private void storageUpdate_Click(object sender, EventArgs e)
        {
            //Controlla se i campi required sono popolati.
            if (!RequiredFieldsArePopulated(true))
            {
                return;
            }
            //Controlla se la stringa di formato è valida.
            if (!string.IsNullOrEmpty(this.txtFormat.Text) && this.txtFormat.Text.Contains('{'))
            {
                try
                {
                    string.Format(this.txtFormat.Text, "test");
                }
                catch 
                {
                    RadMessageBox.Show(this,
                        "Format string isn't valid. Please retry.",
                        "Error",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Error);
                    return;
                }
            }

            if (SyncroMemoryDB(attribute, false))
            {
                CreateWaitDialog();
                backgroundWorker3.RunWorkerAsync(attribute);
            }
            else
            {
                MessageBox.Show("Some needed values aren't present: cannot continue!");
                return;
            }
        }
        private bool RequiredFieldsArePopulated(bool showMsgBox)
        {
            bool retval = (!string.IsNullOrWhiteSpace(txtName.Text) && comboBoxType.SelectedItem != null && comboBoxMode.SelectedItem != null && cbAttributeGroup.SelectedItem != null);
            if (!retval && showMsgBox)
            {
                RadMessageBox.Show(this,
                    "Some required informations are missing. Please retry.",
                    "Error",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error);
            }
            return retval;
        }
        /// <summary>
        /// Sincronizza i valori dei campi del Controllo con i valori della classe dell'oggetto del DB
        /// In caso di passaggio dati verso il database ( false ) restituisce false se i campi obbligatori non sono valorizzati
        /// </summary>
        /// <param name="storage">classe oggetto del db</param>
        /// <param name="fromStorage">true o false per la direzione del sincronismo dati</param>
        /// <returns>bool</returns>
        private bool SyncroMemoryDB(BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute attribute, Boolean fromStorage)
        {
            Boolean campiRequiredPopolati = false;
            if (fromStorage)
            {
                // Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                txtName.Text = attribute.Name;
                if (!string.IsNullOrEmpty(attribute.AttributeType))
                    comboBoxType.SelectedItem = attribute.AttributeType.Substring(attribute.AttributeType.LastIndexOf('.')+1);
                if (attribute.Mode != null) { comboBoxMode.SelectedValue = attribute.Mode.IdMode; }
                ckRequired.Checked = attribute.IsRequired;
                ckPrinaryKey.Checked = attribute.KeyOrder.HasValue && attribute.KeyOrder.Value > 0;
                txtKeyFilter.Text = attribute.KeyFilter;
                txtKeyFormat.Text = attribute.KeyFormat;
                if (attribute.KeyOrder != null) { txtKeyOrder.Value = (short)attribute.KeyOrder; }
                txtValidation.Text = attribute.Validation;
                txtFormat.Text = attribute.Format;
                ckMainDate.Checked = attribute.IsMainDate != null ? (bool)attribute.IsMainDate : false;
                ckAutoInc.Checked = attribute.IsAutoInc != null ? (bool)attribute.IsAutoInc : false;
                ckUnique.Checked = attribute.IsUnique != null ? (bool)attribute.IsUnique : false;
                txtMaxLenght.Value = attribute.MaxLenght.HasValue ? attribute.MaxLenght.Value : 0;
                txtDefaultValue.Text = attribute.DefaultValue;
                txtDescription.Text = (string.IsNullOrWhiteSpace(attribute.Description)) ? attribute.Name : attribute.Description;
                ckIsSectional.Checked = attribute.IsSectional.GetValueOrDefault();
                attributeGroup = attribute.AttributeGroup;
                lsAttributeGroup = Client.GetAttributeGroup(archive.IdArchive);
                lsAttributeGroup.Select(x => x.Description).ToList().ForEach(
                    x => cbAttributeGroup.Items.Add(x.ToString())
                    );
                cbAttributeGroup.Text = attributeGroup != null ? attributeGroup.Description : string.Empty;

                ckRequiredForPreservation.Checked = attribute.IsRequiredForPreservation.GetValueOrDefault();
                ckVisibleForUser.Checked = attribute.IsVisibleForUser.GetValueOrDefault();
                campiRequiredPopolati = true;

            }
            else
            {
                // Qui controllo che i campi obbligatori siano valorizzati
                if (!RequiredFieldsArePopulated(true))
                    return false;
                //if (String.IsNullOrEmpty(txtName.Text) || comboBoxType.SelectedItem == null || comboBoxMode.SelectedItem==null || cbAttributeGroup.SelectedItem==null)
                //{
                //    campiRequiredPopolati = false;
                //}
                //else
                //{
                    // Popolo la classe dell'oggetto DB col valore dei controlli
                    attribute.Name = txtName.Text.Trim();
                    attribute.Archive = archive;
                    attribute.AttributeType = "System." + comboBoxType.SelectedItem.ToString();
                    attribute.Format = txtFormat.Text;
                    attribute.IsMainDate = ckMainDate.Checked;
                    attribute.IsRequired = ckRequired.Checked;
                    attribute.KeyFilter = txtKeyFilter.Text;
                    attribute.KeyFormat = txtKeyFormat.Text;
                    attribute.KeyOrder = (short)txtKeyOrder.Value;
                    attribute.Mode = (AttributeMode)comboBoxMode.SelectedItem;
                    attribute.Validation = txtValidation.Text;
                    attribute.IsUnique = ckUnique.Checked;
                    attribute.AttributeGroup = attributeGroup;
                    attribute.IsVisible = ckVisible.Checked;
                    attribute.DefaultValue = txtDefaultValue.Text;
                    attribute.Description = (string.IsNullOrWhiteSpace(txtDescription.Text)) ? attribute.Name : txtDescription.Text.Trim();
                    attribute.IsSectional = ckIsSectional.Checked;
                    if (txtMaxLenght.Visible) attribute.MaxLenght = (int)txtMaxLenght.Value;

                    attribute.IsRequiredForPreservation = ckRequiredForPreservation.Checked;
                    attribute.IsVisibleForUser = ckVisibleForUser.Checked;
                    campiRequiredPopolati = true;
                //}
            }

            return campiRequiredPopolati;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            BackToSenderControl(sender, e);
        }

        protected override void BackToSenderControl(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcAttribute";
            OutputParameters = new Hashtable();
            OutputParameters.Add("ID", InputParameters["IdArchive"]);
            OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            // Persisto
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    Client.AddAttribute((BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute)e.Argument);
                    break;
                case "modify":
                    Client.UpdateAttribute((BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute)e.Argument);
                    break;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnEndSubmit(sender, e);
        }

        private void ckRequired_CheckedChanged(object sender, EventArgs e)
        {
            if (!ckRequired.Checked) ckPrinaryKey.Checked = false;
            ckPrinaryKey.Enabled = ckRequired.Checked;
            txtKeyFormat.Enabled = ckPrinaryKey.Checked;
            txtKeyFilter.Enabled = ckPrinaryKey.Checked;            
            txtKeyOrder.Enabled = ckPrinaryKey.Checked;
            if (!ckPrinaryKey.Checked)
            {
                txtKeyOrder.Minimum = 0;
                txtKeyOrder.Value = 0;
            }
            else
            {
                txtKeyOrder.Value = 1;
                txtKeyOrder.Minimum = 1;
                if (string.IsNullOrEmpty(this.txtKeyFormat.Text) || !this.txtKeyFormat.Text.Contains("{0"))
                {
                    this.txtKeyFormat.Text = "{0}_" + this.txtKeyFormat.Text;
                }
            }
        }

        private void ckPrinaryKey_CheckedChanged(object sender, EventArgs e)
        {
            ckRequired_CheckedChanged(sender, e);
        }

        private void comboBoxType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBoxType.SelectedItem != null)
            {
                ckMainDate.Enabled = comboBoxType.SelectedItem.ToString().Equals("DateTime");
                ckAutoInc.Enabled = comboBoxType.SelectedItem.ToString().Equals("Int64");
                if (comboBoxType.SelectedItem.ToString() == "String")
                {
                    lblMaxLenght.Visible = txtMaxLenght.Visible = true;
                }
                else 
                {
                    lblMaxLenght.Visible = txtMaxLenght.Visible = false;
                }
            }
        }

        private void cbAttributeGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            attributeGroup = lsAttributeGroup.Where(x => x.Description == cbAttributeGroup.Text).First();
        }       
    }
}
