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
    public partial class UcArchiveStorageRelation : BaseAdminControls
    {

        #region Variabili private

        BindingList<ArchiveStorage> documentsArchiveStorage;

        BindingList<Archive> documentsArchive;
        BindingList<Storage> documentsStorage;
        RadContextMenu menu;
        ArchiveStorage relation;
        #endregion

        public UcArchiveStorageRelation(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            Action = action.nothing;

            // Init RadContextMenu
            menu = new RadContextMenu();
            RadMenuItem itemAddNew = new RadMenuItem();
            itemAddNew.Text = "Insert a new association";
            itemAddNew.Click += new EventHandler(itemAddNew_Click);
            menu.Items.Add(itemAddNew);
            RadMenuItem itemModifica = new RadMenuItem();
            itemModifica.Text = "Modify";
            itemModifica.Click += new EventHandler(itemModifica_Click);
            menu.Items.Add(itemModifica);
            RadMenuItem itemStorageRule = new RadMenuItem();
            itemStorageRule.Text = "Storage Rules";
            itemStorageRule.Click += new EventHandler(itemStorageRule_Click);
            menu.Items.Add(itemStorageRule);
            RadMenuItem itemStorageArea = new RadMenuItem();
            itemStorageArea.Text = "Storage Areas";
            itemStorageArea.Click += new EventHandler(itemStorageArea_Click);
            menu.Items.Add(itemStorageArea);
            RadMenuItem itemDelete = new RadMenuItem();
            itemDelete.Text = "Delete";
            itemDelete.Visibility = ElementVisibility.Collapsed;
            itemDelete.Click += new EventHandler(itemDelete_Click);
            menu.Items.Add(itemDelete);


            // Carica dati
            this.Load += new System.EventHandler(this.UCStorageDetail_Load);
        }

        private void UCStorageDetail_Load(object sender, EventArgs e)
        {
            CreateWaitDialog();

            this.radPanelDetail.Visible = false;

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "relationtoarchive":
                    // Nascondo la colonna archive
                    this.radGridView.Columns[0].IsVisible = false;
                    // Personalizzo la detail per selezionare lo storage
                    this.lbRelatedEntity.Text = "Storage:";
                    this.ddlRelatedEntity.DisplayMember = "Name";
                    this.ddlRelatedEntity.ValueMember = "IdStorage";
                    // Personalizzo i testi del controllo
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Archive " + InputParameters["Name"].ToString(),
                                            "Associated Storage Lists"
                                });
                    this.btConfirm.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
                case "relationtostorage":
                    // Nascondo le colonne Storage
                    this.radGridView.Columns[1].IsVisible = false;
                    this.radGridView.Columns[2].IsVisible = false;
                    // Personalizzo la detail per selezionare lo storage
                    this.lbRelatedEntity.Text = "Archive:";
                    this.ddlRelatedEntity.DisplayMember = "Name";
                    this.ddlRelatedEntity.ValueMember = "IdArchive";
                    // Personalizzo i testi del controllo
                    this.lbRelatedEntity.Text = "Archive:";
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Storage " + InputParameters["Name"].ToString(),
                                            "Associated Archive Lists"
                                });

                    this.btConfirm.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
            }

            // Carico l'elenco delle relazioni presenti
            // Carico l'elenco degli elementi della combobox utilizzabile ( archives o storages ) per l'inserimento 
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "relationtoarchive":
                    {
                        Client.GetStorageArchiveRelationsFromArchiveCompleted += new EventHandler<ServiceReferenceAdministration.GetStorageArchiveRelationsFromArchiveCompletedEventArgs>(Client_GetStorageArchiveRelationsFromArchiveCompleted);
                        Client.GetStorageArchiveRelationsFromArchiveAsync((Guid)InputParameters["ID"]);
                        Client.GetStoragesNotRelatedToArchiveCompleted+=new EventHandler<ServiceReferenceAdministration.GetStoragesNotRelatedToArchiveCompletedEventArgs>(Client_GetStoragesNotRelatedToArchiveCompleted);
                        Client.GetStoragesNotRelatedToArchiveAsync((Guid)InputParameters["ID"]);
                        break;
                    }
                case "relationtostorage":
                    {
                        Client.GetStorageArchiveRelationsFromStorageCompleted+=new EventHandler<ServiceReferenceAdministration.GetStorageArchiveRelationsFromStorageCompletedEventArgs>(Client_GetStorageArchiveRelationsFromStorageCompleted);
                        Client.GetStorageArchiveRelationsFromStorageAsync((Guid)InputParameters["ID"]);
                        Client.GetArchivesNotRelatedToStorageCompleted += new EventHandler<ServiceReferenceAdministration.GetArchivesNotRelatedToStorageCompletedEventArgs>(Client_GetArchivesNotRelatedToStorageCompleted);
                        Client.GetArchivesNotRelatedToStorageAsync((Guid)InputParameters["ID"]);
                        break;
                    }
            }
        }

        #region Async operations completion event handlers

        void Client_GetArchivesNotRelatedToStorageCompleted(object sender, ServiceReferenceAdministration.GetArchivesNotRelatedToStorageCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.documentsArchive = e.Result;
            }
            else
            {
                TrapError(e.Error);
            }
            StorageRelationsRetrieved(e.Error);
        }

        void Client_GetStorageArchiveRelationsFromStorageCompleted(object sender, ServiceReferenceAdministration.GetStorageArchiveRelationsFromStorageCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.documentsArchiveStorage= e.Result;
            }
            else
            {
                TrapError(e.Error);
            }
            StorageRelationsRetrieved(e.Error);
        }

        void Client_GetStoragesNotRelatedToArchiveCompleted(object sender, ServiceReferenceAdministration.GetStoragesNotRelatedToArchiveCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.documentsStorage = e.Result;
            }
            else
            {
                TrapError(e.Error);
            }
            StorageRelationsRetrieved(e.Error);
        }

        void Client_GetStorageArchiveRelationsFromArchiveCompleted(object sender, ServiceReferenceAdministration.GetStorageArchiveRelationsFromArchiveCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.documentsArchiveStorage = e.Result;
            }
            else 
            {
                TrapError(e.Error);
            }
            StorageRelationsRetrieved(e.Error);
        }

        #endregion

        private void StorageRelationsRetrieved(Exception e)
        {
            if (e == null)
            {
                this.radGridView.DataSource = documentsArchiveStorage;

                switch (InputParameters["Action"].ToString().ToLower())
                {
                    case "relationtoarchive":
                        {
                            ddlRelatedEntity.DataSource = documentsStorage;
                            break;
                        }
                    case "relationtostorage":
                        {
                            ddlRelatedEntity.DataSource = documentsArchive;
                            break;
                        }
                }
            }
            else
            {
                TrapError(e);
                btConfirm.Enabled = false;
            }
            CloseWaitDialog();
        }

        private void archivestorageUpdate_Click(object sender, EventArgs e)
        {            
            if (SyncroMemoryDB(relation, false))
            {
                // Persisto
                CreateWaitDialog();
                switch (Action)
                {
                    case action.create:
                          Client.AddArchiveStorageCompleted += (object sender1, AsyncCompletedEventArgs e1) => { this.OperationCompleted(sender1, e1.Error); };
                          Client.AddArchiveStorageAsync(relation);
                        break;
                    case action.update:
                          Client.UpdateArchiveStorageCompleted += (object sender1, AsyncCompletedEventArgs e1) => { this.OperationCompleted(sender1, e1.Error); };
                          Client.UpdateArchiveStorageAsync(relation);
                        break;
                    case action.delete:                        
                        break;
                }
                // Persisto                            
                this.radPanelDetail.Visible = false;
            }
            else
            {
                MessageBox.Show("Some required values aren't valid: cannot continue!");
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
        private bool SyncroMemoryDB(ArchiveStorage relation, Boolean fromStorage)
        {
            Boolean campiRequiredPopolati = false;
            if (fromStorage)
            {                
                //// Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                if (ddlRelatedEntity.Enabled)
                {                    
                    switch (InputParameters["Action"].ToString().ToLower())
                    {
                        case "relationtoarchive":
                            {
                                if (relation.Storage != null) { ddlRelatedEntity.SelectedValue = relation.Storage.IdStorage; }
                                break;
                            }
                        case "relationtostorage":
                            {
                                if (relation.Archive != null) { ddlRelatedEntity.SelectedValue = relation.Archive.IdArchive; }
                                break;
                            }
                    }
                }

                cbActive.Checked = relation.Active;
                campiRequiredPopolati = true;
            }
            else
            {
                if (Action == action.create)
                {
                    if (ddlRelatedEntity.SelectedItem == null)
                    {
                        campiRequiredPopolati = false;
                    }
                    else
                    {
                        campiRequiredPopolati = true;
                        // Popolo la classe dell'oggetto DB col valore dei controlli
                        // Popolo i controlli dai valori ottenuti dalla classe dell'oggetto DB
                        switch (InputParameters["Action"].ToString().ToLower())
                        {
                            case "relationtoarchive":
                                {
                                    relation.Archive = new Archive();
                                    relation.Archive.IdArchive = (Guid)InputParameters["ID"];
                                    relation.Storage = new Storage();
                                    relation.Storage.IdStorage = (Guid)ddlRelatedEntity.SelectedValue;
                                    break;
                                }
                            case "relationtostorage":
                                {
                                    relation.Archive = new Archive();
                                    relation.Archive.IdArchive = (Guid)ddlRelatedEntity.SelectedValue;
                                    relation.Storage = new Storage();
                                    relation.Storage.IdStorage = (Guid)InputParameters["ID"];
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    campiRequiredPopolati = true;                    
                }
                relation.Active = cbActive.Checked;
            }
            return campiRequiredPopolati;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.radPanelDetail.Visible = false;
        }

        protected override void BackToSenderControl(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "relationtoarchive":
                        ControlName = "UcArchive";
                        break;
                case "relationtostorage":
                        ControlName = "UcStorages";
                        break;
            }

            OutputParameters = null;
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        private void OperationCompleted(object sender, Exception e)
        {
            CloseWaitDialog();
            OnEndSubmit(sender, new RunWorkerCompletedEventArgs(sender, e, false));
        }

        private void btBackToList_Click(object sender, EventArgs e)
        {
            BackToSenderControl(sender, e);
        }

        private void itemAddNew_Click(object sender, EventArgs e)
        {
            relation = new ArchiveStorage();
            Action = action.create;
            ddlRelatedEntity.Text = null;
            ddlRelatedEntity.Enabled = true;
            this.radPanelDetail.Visible = true;
        }

        void itemDelete_Click(object sender, EventArgs e)
        {
            if (RadMessageBox.Show("L'associazione selezionata verrà rimossa. Confermi ?","BiblosDS",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CreateWaitDialog();
                Client.DeleteArchiveStorageCompleted += (object sender1, AsyncCompletedEventArgs e1) => { this.OperationCompleted(sender, e1.Error); };
                Client.DeleteArchiveStorageAsync(relation);
            }
        }

        void itemModifica_Click(object sender, EventArgs e)
        {
            SyncroMemoryDB(relation, true);
            Action = action.update;
            this.radPanelDetail.Visible = true;
        }

        void itemStorageRule_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcStorageRule";
            OutputParameters = new Hashtable();
            OutputParameters.Add("ID", relation.Storage.IdStorage);
            OutputParameters.Add("StorageName", relation.Storage.Name);
            OutputParameters.Add("IdArchive", relation.Archive.IdArchive);
            OutputParameters.Add("ArchiveName", relation.Archive.Name);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemStorageArea_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UCStorageArea";
            OutputParameters = new Hashtable();
            OutputParameters.Add("StorageName", relation.Storage.Name);
            OutputParameters.Add("ID", relation.Storage.IdStorage);
            OutputParameters.Add("IdArchive", relation.Archive.IdArchive);
            OutputParameters.Add("ArchiveName", relation.Archive.Name);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        private void radGridView_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            GridDataCellElement cell = e.ContextMenuProvider as GridDataCellElement;
            if (cell == null || this.radGridView.Rows.Count <=0 )
                return;
            // Memorizzo l'elemento selezionato
            relation = new ArchiveStorage();
            relation.Active = (bool)this.radGridView.Rows[cell.RowIndex].Cells["Active"].Value;
            relation.Archive = new Archive();
            relation.Archive = (Archive)this.radGridView.Rows[cell.RowIndex].Cells["Archive"].Value;
            relation.Storage = new Storage();
            relation.Storage = (Storage)this.radGridView.Rows[cell.RowIndex].Cells["Storage"].Value;
            ddlRelatedEntity.Enabled = false;

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "relationtoarchive":
                    {
                        ddlRelatedEntity.Text = this.radGridView.Rows[cell.RowIndex].Cells["Storage.Name"].Value.ToString();
                        break;
                    }
                case "relationtostorage":
                    {
                        ddlRelatedEntity.Text = this.radGridView.Rows[cell.RowIndex].Cells["Archive.Name"].Value.ToString();
                        break;
                    }
            }

            e.ContextMenu = menu.DropDown;
        }

        private void radGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.radPanelDetail.Visible = false;
        }

        private void btnArchives_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcArchive";
            OutputParameters = null;
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        private void btnStorages_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcStorages";
            OutputParameters = null;
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }        

    }
}
