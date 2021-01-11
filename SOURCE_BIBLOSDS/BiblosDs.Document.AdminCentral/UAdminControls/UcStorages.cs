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
    public partial class UCStorages : BaseAdminControls
    {
        BindingList<Storage> documents;
        RadContextMenu menu;

        public UCStorages(): base()
        {
            InitializeComponent();

            Inizialize();
        }

        public UCStorages(Hashtable parameters):base(parameters)
        {
            InitializeComponent();

            // Init RadContextMenu           
            Inizialize();
        }

        private void Inizialize()
        {
            menu = new RadContextMenu();
            RadMenuItem itemAddNew = new RadMenuItem();
            itemAddNew.Text = "Insert a new Storage";
            itemAddNew.Click += new EventHandler(itemAddNew_Click);
            menu.Items.Add(itemAddNew);
            RadMenuItem itemModifica = new RadMenuItem();
            itemModifica.Text = "Modify";
            itemModifica.Click += new EventHandler(itemModifica_Click);
            menu.Items.Add(itemModifica);
            RadMenuItem itemAddRelation = new RadMenuItem();
            itemAddRelation.Text = "Archives";
            itemAddRelation.Click += new EventHandler(itemAddRelation_Click);
            menu.Items.Add(itemAddRelation);
            RadMenuItem itemStorageArea = new RadMenuItem();
            itemStorageArea.Text = "Storage Areas";
            itemStorageArea.Click += new EventHandler(itemStorageArea_Click);
            menu.Items.Add(itemStorageArea);
            RadMenuItem itemStorageRule = new RadMenuItem();
            itemStorageRule.Text = "Storage Rules";
            itemStorageRule.Click += new EventHandler(itemStorageRule_Click);
            menu.Items.Add(itemStorageRule);
            RadMenuItem itemDelete = new RadMenuItem();
            itemDelete.Text = "Delete";
            itemDelete.Visibility = ElementVisibility.Collapsed;
            itemDelete.Click += new EventHandler(itemDelete_Click);
            menu.Items.Add(itemDelete);

            this.Load += new EventHandler(UCStorage_Load);
        }

        void UCStorage_Load(object sender, EventArgs e)
        {

            // Carica dati
            Client.GetAllStoragesWithServerCompleted += new EventHandler<GetAllStoragesWithServerCompletedEventArgs>(Client_GetAllStoragesWithServerCompleted);
            CreateWaitDialog();
            Client.GetAllStoragesWithServerAsync();
        }

        void Client_GetAllStoragesWithServerCompleted(object sender, GetAllStoragesWithServerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                TrapError(e.Error);
            }
            else
            {
                documents = e.Result;
                //this.radGridView.DataSource = documents.OrderBy(x => x.IdStorage);
                this.radGridView.DataSource = documents;
                if (this.radGridView.Rows.Count > 0) this.radGridView.Rows.FirstOrDefault().IsCurrent = true;
            }
            CloseWaitDialog();
        }

        #region radGridView Events

        private void radGridViewStorage_ContextMenuOpening(object sender, Telerik.WinControls.UI.ContextMenuOpeningEventArgs e)
        {          
            e.ContextMenu = menu.DropDown;
        }

        void itemAddNew_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UCStoragesDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "AddNew");
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemModifica_Click(object sender, EventArgs e)
        {
            if (this.radGridView.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Non è stato selezionato alcun archivio.", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UCStoragesDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "Modify");
            OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdStorage"].Value);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemAddRelation_Click(object sender, EventArgs e)
        {
            if (this.radGridView.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Non è stato selezionato alcun archivio.", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcArchiveStorageRelation";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "RelationToStorage");
            OutputParameters.Add("Name", radGridView.SelectedRows[0].Cells["Name"].Value);
            OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdStorage"].Value);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemStorageArea_Click(object sender, EventArgs e)
        {
            if (this.radGridView.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Non è stato selezionato alcun archivio.", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcStorageArea";
            OutputParameters = new Hashtable();
            OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdStorage"].Value);
            OutputParameters.Add("StorageName", radGridView.SelectedRows[0].Cells["Name"].Value);
            OutputParameters.Add("IdArchive", String.Empty);
            OutputParameters.Add("ArchiveName", String.Empty);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemStorageRule_Click(object sender, EventArgs e)
        {
            if (this.radGridView.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Non è stato selezionato alcun archivio.", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcStorageRule";
            OutputParameters = new Hashtable();
            OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdStorage"].Value);
            OutputParameters.Add("StorageName", radGridView.SelectedRows[0].Cells["Name"].Value);
            OutputParameters.Add("IdArchive", String.Empty);
            OutputParameters.Add("ArchiveName", String.Empty);
            

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }


        void itemDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.radGridView.SelectedRows.Count <= 0)
                {
                    RadMessageBox.Show(this, "Non è stato selezionato alcun archivio.", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                if (RadMessageBox.Show("This operation will remove the storage. Confirm ?", "BiblosDS", MessageBoxButtons.YesNo, Telerik.WinControls.RadMessageIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show("TODO");
                    // Controllo la cancellibilità
                    //Document document = null;
                    //using (ServiceReferenceDocument.ServiceDocumentClient client = new ServiceDocumentClient())
                    //{
                    //    document = client.DocumentCheckOut((Guid)radGridView2.SelectedRows[0].Cells["IdDocument"].Value, true, System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                    //}
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            }
        }

        #endregion     

        private void radGridView_DataBindingComplete(object sender, GridViewBindingCompleteEventArgs e)
        {
            this.ShowGuid(radGridView, ConfigurationManager.AppSettings["ShowGUID"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGUID"]));
        }

        private void btnArchives_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcArchive";
            OutputParameters = null;
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }
    }
}
