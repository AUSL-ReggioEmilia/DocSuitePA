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
    public partial class UCStoragesDetail : BaseAdminControls
    {

        #region Variabili private
        
        private BindingList<StorageType> storagesType;
        private Storage storage;
        private const string SQL2014Storage = "SQL2014Storage";

        #endregion

        public UCStoragesDetail(): base()
        {
        }

        public UCStoragesDetail(Hashtable parameters): base( parameters)
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
                    this.radPanelTitle.Text = "Create a new Storage";
                    this.btUpdate.Text = "Insert";
                    this.btCancel.Text = "Cancel";
                    break;
                case "modify":
                    this.radPanelTitle.Text = "Modify Storage";
                    this.btUpdate.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
            }
            
            Client.GetStoragesTypeCompleted += new EventHandler<ServiceReferenceAdministration.GetStoragesTypeCompletedEventArgs>(Client_GetStoragesTypeCompleted);
            Client.GetStoragesTypeAsync();
        }

        void Client_GetStoragesTypeCompleted(object sender, ServiceReferenceAdministration.GetStoragesTypeCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.storagesType = e.Result;
                ddlStorageType.DataSource = storagesType;
                backgroundWorker2.RunWorkerAsync();
            }
            else
            {
                TrapError(e.Error);
                CloseWaitDialog();
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    // In inserimento creo uno storage vuoto
                    //(DocumentStorage)e.Argument = new DocumentStorage();
                    storage = new Storage();
                    break;
                case "modify":
                    // Se sto modificando carico i dati da modificare
                    storage = Client.GetStorageWithServer((Guid)InputParameters["ID"]);
                    
                    break;
            }

        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var servers = Client.GetServers();
                cbServers.DataSource = servers;

                cbServers.SelectedItem = (storage.Server != null) ? servers.Single(x => x.IdServer == storage.Server.IdServer) : null;

                SyncroMemoryDB(storage, true);
            }
            else
            {
                TrapError(e.Error);
            }
            CloseWaitDialog();
        }

        private void storageUpdate_Click(object sender, EventArgs e)
        {
            if (!RequiredFieldsArePopulated(true))
                return;
            try
            {
                if (SyncroMemoryDB(storage, false))
                {
                    CreateWaitDialog();
                    backgroundWorker3.RunWorkerAsync(storage);
                }
                else
                {
                    MessageBox.Show("Alcuni valori obbligatori non sono stati popolati. Impossibile proseguire!");
                    return;
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }            
        }

        private bool RequiredFieldsArePopulated(bool showMsgBox)
        {
            bool retval = (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtPath.Text) && ddlStorageType.SelectedItem != null);
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
        private bool SyncroMemoryDB(Storage storage, Boolean fromStorage)
        {
            Boolean campiRequiredPopolati = false;
            if (fromStorage)
            {
                // Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                if (storage.StorageType != null) { ddlStorageType.SelectedValue = storage.StorageType.IdStorageType; }
                txtPath.Text = storage.MainPath;
                txtName.Text = storage.Name;
                txtStorageRuleAssembly.Text = storage.StorageRuleAssembly;
                txtStorageRuleClassName.Text = storage.StorageRuleClassName;
                txtPriority.Text = Convert.ToString(storage.Priority);
                cbFullText.Checked = storage.EnableFulText;
                txtAuthenticationKey.Text = storage.AuthenticationKey;
                txtAuthenticationPassword.Text = storage.AuthenticationPassword;
        
                campiRequiredPopolati = true;
            }
            else
            {
                // Qui controllo che i campi obbligatori siano valorizzati
                if (!RequiredFieldsArePopulated(true))
                    return false;
                //if (String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtPath.Text))
                //{
                //    campiRequiredPopolati = false;
                //}
                //else
                //{
                    // Popolo la classe dell'oggetto DB col valore dei controlli
                    storage.StorageType = (StorageType)ddlStorageType.SelectedItem;
                    storage.MainPath = txtPath.Text;
                    storage.Name = txtName.Text;
                    storage.StorageRuleAssembly = txtStorageRuleAssembly.Text;
                    storage.StorageRuleClassName = txtStorageRuleClassName.Text;
                    if (!String.IsNullOrEmpty(txtPriority.Text)) { storage.Priority = int.Parse(txtPriority.Text); }
                    storage.EnableFulText = cbFullText.Checked;
                    storage.AuthenticationKey = txtAuthenticationKey.Text;
                    storage.AuthenticationPassword = txtAuthenticationPassword.Text;
                    storage.IsVisible = cbVisible.Checked;
                    if (string.IsNullOrWhiteSpace(cbServers.Text))
                        storage.Server = null;
                    else if (cbServers.SelectedItem != null)
                        storage.Server = cbServers.SelectedItem as Server;
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
            ControlName = "UcStorages";
            OutputParameters = null;
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            // Persisto
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    Client.AddStorage((Storage)e.Argument);
                    break;
                case "modify":
                    Client.UpdateStorage((Storage)e.Argument);
                    break;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnEndSubmit(sender, e);
        }

        private void ddlStorageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox storageTypes = (ComboBox)sender;
            StorageType selectedStorageType = (StorageType)storageTypes.SelectedItem;
            bool isSql2014Storage = selectedStorageType.StorageClassName.Equals(SQL2014Storage, StringComparison.InvariantCultureIgnoreCase);
            cbFullText.Checked = isSql2014Storage;
            cbFullText.Enabled = !isSql2014Storage;
            label17.Visible = isSql2014Storage;
        }
    }
}
