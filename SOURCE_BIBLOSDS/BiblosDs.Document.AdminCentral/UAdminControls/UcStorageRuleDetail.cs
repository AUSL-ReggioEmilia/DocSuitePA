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
    public partial class UcStorageRuleDetail : BaseAdminControls
    {
        #region Variabili private
        private StorageRule storageRule;
        private Storage storage;

        private BindingList<Archive> archives;
        private BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute> attributes;
        private BindingList<RuleOperator> ruleOperators;

        private Guid currentIdStorage = Guid.Empty;
        private Dictionary<Guid, BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute>> archiveAtributes;
        #endregion

        public UcStorageRuleDetail(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            // Verifica Parametri
            VerifyInputParameters(new List<string> { "Action", "StorageName", "IdStorage", "IdArchive", "IdAttribute", "ChangeArchive" });

            //Inizializzazioni
            this.archiveAtributes = new Dictionary<Guid, BindingList<ServiceReferenceAdministration.Attribute>>();
            this.attributes = new BindingList<ServiceReferenceAdministration.Attribute>();
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
                                            "Creazione Storage Rule"
                                });
                    this.btUpdate.Text = "Insert";
                    this.btCancel.Text = "Cancel";
                    break;
                case "modify":
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "Modify Storage Rule"
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
            this.currentIdStorage = (Guid)InputParameters["IdStorage"];

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    // In inserimento creo uno storage vuoto
                    //(DocumentArchive)e.Argument = new DocumentArchive();
                    storageRule = new StorageRule();
                    if (InputParameters["IdArchive"].ToString() != String.Empty)
                    {
                        attributes = Client.GetAttributesFromArchive((Guid)InputParameters["IdArchive"]);
                    }
                    break;
                case "modify":
                    // Se sto modificando carico i dati da modificare
                    storageRule = Client.GetStorageRule(this.currentIdStorage, (Guid)InputParameters["IdAttribute"]);
                    //attributes = Client.GetAttributesFromArchive((Guid)InputParameters["IdArchive"]);
                    break;
            }
            archives = Client.GetArchivesFromStorage(this.currentIdStorage);
            if (archives.Count <= 0)
            {
                RadMessageBox.Show("No archives associated with this storage.",
                    "Error",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error);
                BackToSenderControl(null, null);
            }

            ruleOperators = Client.GetRuleOperators();

            storage = Client.GetStorage(this.currentIdStorage);

            if (InputParameters["Action"].ToString().ToLower() == "modify")
            {
                BindingList<StorageRule> rules;
                this.attributes = new BindingList<ServiceReferenceAdministration.Attribute>();
                bool foundedSomething = false, invalidArchive;
                Archive arc;
                for (int i = 0; i < archives.Count; i++)
                {
                    arc = archives[i];
                    invalidArchive = true;
                    rules = Client.GetStorageRules(this.currentIdStorage, arc.IdArchive);

                    this.archiveAtributes.Add(arc.IdArchive, new BindingList<ServiceReferenceAdministration.Attribute>());

                    foreach (StorageRule r in rules)
                    {
                        if (r.Attribute != null && (!r.Attribute.IsVisible.HasValue || r.Attribute.IsVisible.Value))
                        {
                            r.Attribute.Archive = arc;
                            this.archiveAtributes[arc.IdArchive].Add(r.Attribute);
                            this.attributes.Add(r.Attribute);
                            foundedSomething = true;
                            invalidArchive = false;
                        }
                    }
                    if (invalidArchive)
                    {
                        this.archives.RemoveAt(i--);
                    }
                }

                if (!foundedSomething)
                {
                    RadMessageBox.Show("No Archives or Attributes was founded valid for this operation.",
                        "Error",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Error);
                    BackToSenderControl(null, null);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                cbArchive.DataSource = archives;
                cbAttribute.DataSource = attributes;
                cbRuleOperators.DataSource = ruleOperators;
                if (InputParameters["Action"].ToString().ToLower() == "addnew")
                {
                    for (int i = 0; i < archives.Count; i++)
                    {
                        Archive arc = archives[i];
                        this.archiveAtributes.Add(arc.IdArchive,
                            Client.GetAttributesFromArchive(arc.IdArchive));
                        if (this.archiveAtributes[arc.IdArchive].Count <= 0)
                        {
                            this.archiveAtributes.Remove(arc.IdArchive);
                            this.archives.RemoveAt(i--);
                        }
                        else
                        {
                            foreach (ServiceReferenceAdministration.Attribute att in this.archiveAtributes[arc.IdArchive])
                            {
                                this.attributes.Add(att);
                            }
                        }
                    }
                    this.storageRule.Attribute = this.attributes.FirstOrDefault();
                }
                else
                {
                    this.RetrieveAttributes((Guid)InputParameters["IdArchive"]);
                }
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

                SyncroMemoryDB(storageRule, true);
            }
            else
            {
                TrapError(e.Error);
            }
            CloseWaitDialog();
        }

        private void storageUpdate_Click(object sender, EventArgs e)
        {
            if (SyncroMemoryDB(storageRule, false))
            {
                CreateWaitDialog();
                backgroundWorker3.RunWorkerAsync(storageRule);
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
        private bool SyncroMemoryDB(StorageRule storageRule, Boolean fromStorage)
        {
            Boolean campiRequiredPopolati = false;
            if (fromStorage)
            {
                // Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                if (storageRule.Attribute != null) 
                {
                    cbArchive.SelectedValue = storageRule.Attribute.Archive.IdArchive;
                    cbAttribute.DataSource = attributes.Where(x => x.Archive.IdArchive == (Guid)cbArchive.SelectedValue).ToList();
                    cbAttribute.SelectedItem = storageRule.Attribute; 
                }
                txtOrder.Text = Convert.ToString(storageRule.RuleOrder);
                txtFilter.Text = storageRule.RuleFilter;
                txtFormat.Text = storageRule.RuleFormat;
                cbRuleOperators.SelectedItem = storageRule.RuleOperator;

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
                    storageRule.Storage = storage;
                    storageRule.Attribute = (BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute)cbAttribute.SelectedItem;
                    storageRule.RuleOrder = (short)txtOrder.Value;
                    storageRule.RuleFilter = txtFilter.Text;
                    storageRule.RuleFormat = txtFormat.Text;
                    storageRule.RuleOperator = (RuleOperator)cbRuleOperators.SelectedItem;
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
            ControlName = "UcStorageRule";
            OutputParameters = new Hashtable();
            OutputParameters.Add("ID", InputParameters["IdStorage"]);
            OutputParameters.Add("StorageName", InputParameters["StorageName"]);
            OutputParameters.Add("IdArchive", InputParameters["IdArchive"]);
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
                    Client.AddStorageRule((StorageRule)e.Argument);
                    break;
                case "modify":
                    Client.UpdateStorageRule((StorageRule)e.Argument);
                    break;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnEndSubmit(sender, e);
        }

        private void cbArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbArchive.Enabled)
            {
                this.RetrieveAttributes((Guid)cbArchive.SelectedValue);
            }
        }

        private void RetrieveAttributes(Guid idArchive)
        {
            if (idArchive != Guid.Empty)
            {
                //Client.GetAttributesFromArchiveCompleted += new EventHandler<ServiceReferenceAdministration.GetAttributesFromArchiveCompletedEventArgs>(Client_GetAttributesFromArchiveCompleted);
                //CreateWaitDialog();
                //Client.GetAttributesFromArchiveAsync(idArchive);
                this.txtOrder.Value = 0;
                this.txtFormat.Text = string.Empty;
                this.txtFilter.Text = string.Empty;
                this.AssignAttributes(this.archiveAtributes[idArchive]);
            }
        }

        private void AssignAttributes(BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Attribute> attribs)
        {
            attributes = attribs;

            ServiceReferenceAdministration.Attribute attr = attributes.First();

            cbAttribute.DataSource = attributes;            
            cbAttribute.Text = attr.Name;
            cbAttribute.SelectedItem = attr;
            
            cbRuleOperators.SelectedIndex = 0;

            if (InputParameters["Action"].ToString().ToLower() == "modify")
            {
                storageRule = Client.GetStorageRule(currentIdStorage, attr.IdAttribute);

                txtFilter.Text = storageRule.RuleFilter;
                txtFormat.Text = storageRule.RuleFormat;
                txtOrder.Value = storageRule.RuleOrder;
                cbRuleOperators.SelectedItem = storageRule.RuleOperator;
                cbRuleOperators.Text = storageRule.RuleOperator.Descrizione;
            }
        }

        [Obsolete("Ormai non più usata")]
        void Client_GetAttributesFromArchiveCompleted(object sender, ServiceReferenceAdministration.GetAttributesFromArchiveCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.AssignAttributes(e.Result);
            }
            else
            {
                TrapError(e.Error);
            }
            CloseWaitDialog();
        }
    }
}
