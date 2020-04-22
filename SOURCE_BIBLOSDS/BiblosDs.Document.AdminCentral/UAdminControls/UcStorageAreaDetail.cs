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
    public partial class UcStorageAreaDetail : BaseAdminControls
    {
        #region Variabili private
        
        private StorageArea storageArea;
        private Storage storage;

        BindingList<BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Status> storageAreaStatus;

        #endregion

        public UcStorageAreaDetail(Hashtable parameters)
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
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "Creazione Storage Area"
                                });

                    this.btUpdate.Text = "Insert";
                    this.btCancel.Text = "Cancel";
                    break;
                case "modify":
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "Modify Storage Area"
                                });
                    this.btUpdate.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
            }
            this.cbStatus.ValueMember = "IdStatus";
            this.cbStatus.DisplayMember = "Description";
          
            // Caricamento attributo
            backgroundWorker1.RunWorkerAsync();
            Client.GetStorageArchiveRelationsFromStorageCompleted += new EventHandler<GetStorageArchiveRelationsFromStorageCompletedEventArgs>(Client_GetStorageArchiveRelationsFromStorageCompleted);
            Client.GetStorageArchiveRelationsFromStorageAsync((Guid)InputParameters["IdStorage"]);
        }

        void Client_GetStorageArchiveRelationsFromStorageCompleted(object sender, GetStorageArchiveRelationsFromStorageCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }

            var archives = e.Result.Select(x => x.Archive).ToList();
            archives.Insert(0, new Archive { IdArchive = Guid.Empty });
            cbArchive.DataSource = archives;
        }        

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    // In inserimento creo uno storage vuoto
                    //(DocumentArchive)e.Argument = new DocumentArchive();
                    storageArea = new StorageArea();
                    break;
                case "modify":
                    // Se sto modificando carico i dati da modificare
                    storageArea = Client.GetStorageArea((Guid)InputParameters["ID"]);
                    break;
            }

            storageAreaStatus = Client.GetAllStorageAreaStatus();

            storage = Client.GetStorage((Guid)InputParameters["IdStorage"]);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseWaitDialog();
            if (e.Error == null)
            {
                cbStatus.DataSource = storageAreaStatus;

                SyncroMemoryDB(storageArea, true);
            }
            else
            {
                TrapError(e.Error);
            }            
        }

        private void storageUpdate_Click(object sender, EventArgs e)
        {
            if (SyncroMemoryDB(storageArea, false))
            {
                CreateWaitDialog();
                backgroundWorker3.RunWorkerAsync(storageArea);
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
        private bool SyncroMemoryDB(StorageArea storageArea, Boolean fromStorage)
        {
            Boolean campiRequiredPopolati = false;
            if (fromStorage)
            {
                // Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                txtName.Text = storageArea.Name;
                ckEnabled.Checked = storageArea.Enable;
                if (storageArea.Status != null) { cbStatus.SelectedValue = storageArea.Status.IdStatus; }
                txtPath.Text = storageArea.Path;
                txtPriority.Text = Convert.ToString(storageArea.Priority);
                txtMaxSize.Text = Convert.ToString(storageArea.MaxSize / (1024 * 1024));
                txtCurrentSize.Text = Convert.ToString(storageArea.CurrentSize / (1024 * 1024));
                txtMaxFileNumber.Text = Convert.ToString(storageArea.MaxFileNumber);
                txtCurrentFileNumber.Text = Convert.ToString(storageArea.CurrentFileNumber);
                
                  // Valori di default
                //if (storageArea.IdStorageArea == Guid.Empty)
                //{
                //}

                // Logica personalizzata
                cbSetMaxFileNumber.Checked = (txtMaxFileNumber.Value != 0);
                cbSetMaxFileNumber_CheckedChanged(cbSetMaxFileNumber, new EventArgs());
                cbSetMaxSize.Checked = (txtMaxSize.Value != 0);
                cbSetMaxSize_CheckedChanged(cbSetMaxSize, new EventArgs());

                if (storageArea.Archive != null)
                    cbArchive.SelectedValue = storageArea.Archive.IdArchive;
                campiRequiredPopolati = true;
            }
            else
            {
                // Qui controllo che i campi obbligatori siano valorizzati
                if (String.IsNullOrEmpty(txtName.Text) )
                {
                    campiRequiredPopolati = false;
                }
                else
                {
                    // Popolo la classe dell'oggetto DB col valore dei controlli
                    storageArea.Name = txtName.Text;
                    storageArea.Storage = storage;
                    storageArea.Path = txtPath.Text;
                    storageArea.Priority = (int)txtPriority.Value;
                    storageArea.Status = (BiblosDs.Document.AdminCentral.ServiceReferenceAdministration.Status)cbStatus.SelectedItem;
                    storageArea.MaxSize = (long)txtMaxSize.Value * (1024 * 1024);
                    storageArea.CurrentSize = (long)txtCurrentSize.Value * (1024 * 1024);
                    storageArea.Enable = ckEnabled.Checked;
                    storageArea.CurrentFileNumber = (long)txtCurrentFileNumber.Value;
                    storageArea.MaxFileNumber = (long)txtMaxFileNumber.Value;

                    if (cbArchive.SelectedValue != null && (Guid)cbArchive.SelectedValue != Guid.Empty)
                        storageArea.Archive = new Archive { IdArchive = (Guid)cbArchive.SelectedValue };
                    else
                        storageArea.Archive = null;
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
            ControlName = "UcStorageArea";
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
                    Client.AddStorageArea((StorageArea)e.Argument);
                    break;
                case "modify":
                    Client.UpdateStorageArea((StorageArea)e.Argument);
                    break;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnEndSubmit(sender, e);
        }

        private void cbSetMaxSize_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                lbMaxSize.Visible =
                txtMaxSize.Visible = true;
            }
            else
            {
                lbMaxSize.Visible =
                txtMaxSize.Visible = false;
                txtMaxSize.Value = 0;
            }
        }

        private void cbSetMaxFileNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                lbMaxFileNumber.Visible = 
                txtMaxFileNumber.Visible = true;
            }
            else
            {
                lbMaxFileNumber.Visible = 
                txtMaxFileNumber.Visible = false;
                txtMaxFileNumber.Value = 0;
            }
        }


    }
}
