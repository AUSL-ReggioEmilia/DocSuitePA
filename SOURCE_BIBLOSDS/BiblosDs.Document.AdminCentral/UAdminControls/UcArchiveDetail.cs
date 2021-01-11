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
using System.Collections;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;


namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UCArchiveDetail : BaseAdminControls
    {
        #region Variabili private
        
        private Archive archive;

        #endregion

        public UCArchiveDetail(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            // Carica dati
            this.Load += new System.EventHandler(this.UCStorageDetail_Load);
        }

        private void UCStorageDetail_Load(object sender, EventArgs e)
        {
            VerifyInputParameters(new List<string> { "Action", "ID", "ArchiveName" });

            CreateWaitDialog();
            cbDocumentsType.DataSource = Client.GetPreservationFiscalDocumentsTypes();
            CloseWaitDialog();

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    this.radPanelTitle.Text = "New Archive";
                    this.btUpdate.Text = "Insert";
                    this.btCancel.Text = "Cancel";
                    break;
                case "modify":
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Archive " + InputParameters["ArchiveName"].ToString(),
                                            "Modify"
                                });

                    this.btUpdate.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
            }

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    // In inserimento creo uno storage vuoto
                    //(DocumentArchive)e.Argument = new DocumentArchive();
                    archive = new Archive();
                    ckTransitEnabled_CheckedChanged(this, EventArgs.Empty);
                    cbLegal_CheckedChanged(this, EventArgs.Empty);
                    break;
                case "modify":
                    // Se sto modificando carico i dati da modificare
                    Client.GetArchiveWithServerConfigsCompleted += new EventHandler<GetArchiveWithServerConfigsCompletedEventArgs>(Client_GetArchiveWithServerConfigsCompleted);
                    CreateWaitDialog();
                    Client.GetArchiveWithServerConfigsAsync((Guid)InputParameters["ID"]);
                    break;
            }
        }

        void Client_GetArchiveWithServerConfigsCompleted(object sender, GetArchiveWithServerConfigsCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.archive = e.Result;
                SyncroMemoryDB(archive, true);
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
            if (SyncroMemoryDB(archive, false))
            {
                CreateWaitDialog();
                switch (InputParameters["Action"].ToString().ToLower())
                {
                    case "addnew":
                        Client.AddArchiveCompleted +=new EventHandler<ServiceReferenceAdministration.AddArchiveCompletedEventArgs>(Client_AddArchiveCompleted);
                        Client.AddArchiveAsync(archive);
                        break;
                    case "modify":
                        Client.UpdateArchiveCompleted +=new EventHandler<AsyncCompletedEventArgs>(Client_UpdateArchiveCompleted);
                        Client.UpdateArchiveAsync(archive);
                        break;
                }
            }
            else
            {
                RadMessageBox.Show(this, "Some values are required: can't go on!", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
        }

        void  Client_AddArchiveCompleted(object sender, ServiceReferenceAdministration.AddArchiveCompletedEventArgs e)
        {
            this.StorageUpdateCompleted(sender, e.Error);
        }

        void  Client_UpdateArchiveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.StorageUpdateCompleted(sender, e.Error);
        }

        private bool RequiredFieldsArePopulated(bool showMsgBox)
        {
            bool retval = !string.IsNullOrEmpty(txtName.Text);
            if (retval && ckTransitEnabled.Checked)
                retval = !string.IsNullOrEmpty(txtPathTransito.Text);
            if (retval && cbLegal.Checked)
                retval = !string.IsNullOrEmpty(txtPathPreservation.Text);
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
        private bool SyncroMemoryDB(Archive archive, Boolean fromStorage)
        {
            Boolean campiRequiredPopolati = false;
            if (fromStorage)
            {
                // Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                txtName.Text = archive.Name;
                txtPathPreservation.Text = archive.PathPreservation;
                cbDocumentsType.SelectedItem = archive.FiscalDocumentType;
                txtPathTransito.Text = archive.PathTransito;
                cbLegal.Checked = archive.IsLegal;
                txtMaxCache.Text = Convert.ToString(archive.MaxCache / (1024 * 1024));
                txtUpperCACHE.Text = Convert.ToString(archive.UpperCache / (1024 * 1024));
                txtLowerCache.Text = Convert.ToString(archive.LowerCache / (1024 * 1024));
                cbAutoVersion.Checked = archive.AutoVersion;
                txtLastIdBiblos.Value = archive.LastIdBiblos;
                txtAuthorizationAssembly.Text = archive.AuthorizationAssembly;
                txtAuthorizationClassName.Text = archive.AuthorizationClassName;
                cbSecurity.Checked = archive.EnableSecurity;
                ckTransitEnabled.Checked = archive.TransitoEnabled;
                ckTransitEnabled_CheckedChanged(this, EventArgs.Empty);
                cbLegal_CheckedChanged(this, EventArgs.Empty);
                // Valori di default
                if (archive.IdArchive == Guid.Empty)
                {
                    txtMaxCache.Text = Convert.ToString("100");
                    txtUpperCACHE.Text = Convert.ToString("80");
                    txtLowerCache.Text = Convert.ToString("50");
                }

                gvServers.DataSource = archive.ServerConfigs ?? new BindingList<ArchiveServerConfig>();

                campiRequiredPopolati = true;
            }
            else
            {
                // Qui controllo che i campi obbligatori siano valorizzati
                if (!RequiredFieldsArePopulated(true))
                    return false;
                //if (String.IsNullOrEmpty(txtName.Text) )
                //{
                //    campiRequiredPopolati = false;
                //}
                //else
                //{
                    // Popolo la classe dell'oggetto DB col valore dei controlli
                    archive.Name=txtName.Text;
                    archive.PathTransito = txtPathTransito.Text;
                    //archive.PathCache = txtPathPreservation.Text;
                    archive.PathPreservation = txtPathPreservation.Text;
                    archive.FiscalDocumentType = (cbDocumentsType.SelectedItem != null) ? cbDocumentsType.SelectedItem.ToString() : string.Empty;
                    archive.IsLegal=cbLegal.Checked;
                    if (!String.IsNullOrEmpty(txtMaxCache.Text)) { archive.MaxCache = Convert.ToInt64(txtMaxCache.Text) * (1024 * 1024); }
                    if (!String.IsNullOrEmpty(txtUpperCACHE.Text)) { archive.UpperCache = Convert.ToInt64(txtUpperCACHE.Text) * (1024 * 1024); }
                    if (!String.IsNullOrEmpty(txtLowerCache.Text)) { archive.LowerCache = Convert.ToInt64(txtLowerCache.Text) * (1024 * 1024); }
                    archive.AutoVersion = cbAutoVersion.Checked;
                    if (!String.IsNullOrEmpty(txtLastIdBiblos.Text)) { archive.LastIdBiblos = Convert.ToInt32(txtLastIdBiblos.Text); }
                    archive.AuthorizationAssembly = txtAuthorizationAssembly.Text;
                    archive.AuthorizationClassName = txtAuthorizationClassName.Text;
                    archive.EnableSecurity = cbSecurity.Checked;
                    archive.TransitoEnabled = ckTransitEnabled.Checked;
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
            ControlName = "UcArchive";
            OutputParameters = null;
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        private void StorageUpdateCompleted(object sender, Exception error)
        {
            OnEndSubmit(sender, new RunWorkerCompletedEventArgs(sender, error, false));
        }

        private void ckTransitEnabled_CheckedChanged(object sender, EventArgs e)
        {
            txtPathTransito.Enabled = ckTransitEnabled.Checked;
            lbTransitRequired.Visible = ckTransitEnabled.Checked;
        }

        private void cbLegal_CheckedChanged(object sender, EventArgs e)
        {
            lbPathPreservationRequired.Visible = cbLegal.Checked;
            txtPathPreservation.Enabled = cbLegal.Checked;
        }

        private void modifyServerConfig(ArchiveServerConfig item = null)
        {
            this.Enabled = false;

            try
            {
                var frm = new ArchiveServerConfiguration(ref Client, archive, item != null, item);
                var result = frm
                    .ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    if (!frm.EditMode)
                    {
                        var toAdd = Client.AddArchiveServerConfig(frm.Result);
                        (gvServers.DataSource as BindingList<ArchiveServerConfig>).Add(toAdd);
                    }
                    gvServers.Refresh();
                }
            }
            finally
            {
                this.Enabled = true;
            }
        }

        private void btnAddServer_Click(object sender, EventArgs e)
        {
            modifyServerConfig();
        }

        private void gvServers_CommandCellClick(object sender, EventArgs e)
        {
            var args = e as Telerik.WinControls.UI.GridViewCellEventArgs;
            var item = args.Row.DataBoundItem as ArchiveServerConfig;

            switch (args.Column.Name.ToUpper())
            {
                case "COLMODIFY":
                    modifyServerConfig(item);
                    break;
                case "COLDELETE":
                    CreateWaitDialog();
                    try
                    {
                        Client.DeleteArchiveServerConfig(item);
                        args.Row.Delete();
                    }
                    finally
                    {
                        CloseWaitDialog();
                    }
                    break;
            }

            gvServers.Refresh();
        }
   }
}
