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
    public partial class UcStorageAreaRuleDetail : BaseAdminControls
    {
        #region Variabili private

        private StorageAreaRule storageAreaRule;
        private StorageArea storageArea;

        private BindingList<Archive> archives;
        private BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute> attributes;
        private BindingList<RuleOperator> ruleOperators;

        #endregion

        public UcStorageAreaRuleDetail(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            // Verifica Parametri
            VerifyInputParameters(new List<string> { "Action", "StorageName", "IdStorage", "IdArchive", "IdAttribute", "StorageAreaName", "IdStorageArea", "ChangeArchive","ArchiveName" });

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
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "Storage Area " + InputParameters["StorageAreaName"].ToString(),
                                            "New Storage Area Rule"
                                });
                    this.btUpdate.Text = "Insert";
                    this.btCancel.Text = "Cancel";
                    break;
                case "modify":
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "Storage Area " + InputParameters["StorageAreaName"].ToString(),
                                            "Modify Storage Area Rule"
                                });
                    this.btUpdate.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
            }
            
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
                    storageAreaRule = new StorageAreaRule();
                    if (InputParameters["IdArchive"].ToString()!=String.Empty)
                        attributes = Client.GetAttributesFromArchive((Guid)InputParameters["IdArchive"]);

                    break;
                case "modify":
                    // Se sto modificando carico i dati da modificare
                    storageAreaRule = Client.GetStorageAreaRule((Guid)InputParameters["IdStorageArea"], (Guid)InputParameters["IdAttribute"]);
                    attributes = Client.GetAttributesFromArchive((Guid)InputParameters["IdArchive"]);
                    break;
            }

            archives = Client.GetArchivesFromStorage((Guid)InputParameters["IdStorage"]);

            if (InputParameters["IdArchive"].ToString() == String.Empty && archives.Count() > 0)
                attributes = Client.GetAttributesFromArchive(archives[0].IdArchive);
            ruleOperators = Client.GetRuleOperators();

            storageArea = Client.GetStorageArea((Guid)InputParameters["IdStorageArea"]);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                cbArchive.DataSource = archives;
                cbAttribute.DataSource = attributes;
                cbRuleOperators.DataSource = ruleOperators;

                // Blocco il cambio archivio se è il caso
                if (InputParameters["ChangeArchive"].ToString().ToLower() != "true")
                {
                    if (InputParameters["Action"].ToString().ToLower() == "addnew")
                    {
                        cbArchive.SelectedValue = InputParameters["IdArchive"].ToString();
                        cbArchive.Text = InputParameters["ArchiveName"].ToString();
                    }
                    cbArchive.Enabled = false;
                }


                SyncroMemoryDB(storageAreaRule, true);
            }
            else
            {
                TrapError(e.Error);
            }
            CloseWaitDialog();
        }

        private void storageUpdate_Click(object sender, EventArgs e)
        {
            if (SyncroMemoryDB(storageAreaRule, false))
            {
                CreateWaitDialog();
                backgroundWorker3.RunWorkerAsync(storageAreaRule);
            }
            else
            {
                MessageBox.Show("Alcuni valori obbligatori non sono stati popolati. Impossibile proseguire!");
                return;
            }
        }

        /// <summary>
        /// Sincronizza i valori dei campi del Controllo con i valori della classe dell'oggetto del DB
        /// In caso di passaggio dati verso il database ( false ) restituisce false se i campi obbligatori non sono valorizzati
        /// </summary>
        /// <param name="storage">classe oggetto del db</param>
        /// <param name="fromStorage">true o false per la direzione del sincronismo dati</param>
        /// <returns>bool</returns>
        private bool SyncroMemoryDB(StorageAreaRule storageAreaRule, Boolean fromStorage)
        {
            Boolean campiRequiredPopolati = false;
            if (fromStorage)
            {
                // Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                if (storageAreaRule.Attribute != null) 
                {
                    cbArchive.SelectedValue = storageAreaRule.Attribute.Archive.IdArchive;
                    cbAttribute.SelectedValue = storageAreaRule.Attribute.IdAttribute; 
                }
                txtOrder.Text = Convert.ToString(storageAreaRule.RuleOrder);
                txtFilter.Text = storageAreaRule.RuleFilter;
                txtFormat.Text = storageAreaRule.RuleFormat;
                cbRuleOperators.SelectedItem = storageAreaRule.RuleOperator;
                ckCalculated.Checked = storageAreaRule.IsCalculated.GetValueOrDefault();
                campiRequiredPopolati = true;
            }
            else
            {
                // Qui controllo che i campi obbligatori siano valorizzati
                if (String.IsNullOrEmpty(txtFilter.Text) || String.IsNullOrEmpty(txtOrder.Text) || cbAttribute.SelectedValue == null)
                {
                    campiRequiredPopolati = false;
                }
                else
                {
                    // Popolo la classe dell'oggetto DB col valore dei controlli
                    storageAreaRule.StorageArea = storageArea;
                    storageAreaRule.Attribute = (BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute)cbAttribute.SelectedItem;
                    storageAreaRule.RuleOrder = (short)txtOrder.Value;
                    storageAreaRule.RuleFilter = txtFilter.Text;
                    storageAreaRule.RuleFormat = txtFormat.Text;
                    storageAreaRule.RuleOperator = (RuleOperator)cbRuleOperators.SelectedItem;
                    storageAreaRule.IsCalculated = ckCalculated.Checked;
                    campiRequiredPopolati = true;
                }
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
            ControlName = "UcStorageAreaRule";
            OutputParameters = new Hashtable();
            OutputParameters.Add("StorageName", InputParameters["StorageName"]);
            OutputParameters.Add("IdStorage", InputParameters["IdStorage"]);
            OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);
            OutputParameters.Add("IdArchive", InputParameters["IdArchive"]);
            OutputParameters.Add("StorageAreaName", InputParameters["StorageAreaName"]);
            OutputParameters.Add("ID", InputParameters["IdStorageArea"]);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            // Persisto
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    Client.AddStorageAreaRule((StorageAreaRule)e.Argument);
                    break;
                case "modify":
                    Client.UpdateStorageAreaRule((StorageAreaRule)e.Argument);
                    break;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnEndSubmit(sender, e);
        }

        private void cbArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbArchive.Enabled && (Guid)cbArchive.SelectedValue != Guid.Empty)
            {
                CreateWaitDialog();
                Client.GetAttributesFromArchiveCompleted += new EventHandler<ServiceReferenceAdministration.GetAttributesFromArchiveCompletedEventArgs>(Client_GetAttributesFromArchiveCompleted);
                Client.GetAttributesFromArchiveAsync((Guid)cbArchive.SelectedValue);
            }
        }

        void Client_GetAttributesFromArchiveCompleted(object sender, ServiceReferenceAdministration.GetAttributesFromArchiveCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                cbAttribute.Text = String.Empty;
                attributes = e.Result;
                cbAttribute.DataSource = attributes;
            }
            else
            {
                TrapError(e.Error);
            }
            CloseWaitDialog();
        }
    }
}
