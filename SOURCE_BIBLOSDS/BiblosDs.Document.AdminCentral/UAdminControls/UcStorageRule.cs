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
    public partial class UcStorageRule : BaseAdminControls
    {
        BindingList<StorageRule> documents;
        RadContextMenu menu;

        public UcStorageRule(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            // Init RadContextMenu
            menu = new RadContextMenu();
            RadMenuItem itemAddNew = new RadMenuItem();
            itemAddNew.Text = "Insert a new Storage Rule";
            itemAddNew.Click += new EventHandler(itemAddNew_Click);
            menu.Items.Add(itemAddNew);
            RadMenuItem itemModifica = new RadMenuItem();
            itemModifica.Text = "Modify";
            itemModifica.Click += new EventHandler(itemModifica_Click);
            menu.Items.Add(itemModifica);
            RadMenuItem itemDelete = new RadMenuItem();
            itemDelete.Text = "Delete";
            itemDelete.Visibility = ElementVisibility.Collapsed;
            itemDelete.Click += new EventHandler(itemDelete_Click);
            menu.Items.Add(itemDelete);

            VerifyInputParameters(new List<string> { "StorageName", "ID", "ArchiveName", "IdArchive" });

            if (InputParameters["ArchiveName"].ToString()==String.Empty)
            {
                this.radPanel1.Text = FormatTitle(new List<string>{
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "Storage Rule List"
                                });


                btBack.Text = "Back";
            }
            else
            {
                this.radPanel1.Text = FormatTitle(new List<string>{
                                            "Archive " + InputParameters["ArchiveName"].ToString(),
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "Storage Rule List"
                                });


                btBack.Text = "Back";
            }
            this.Load += new EventHandler(UcAttribute_Load);
        }

        void UcAttribute_Load(object sender, EventArgs e)
        {
            // Carica dati
            Client.GetStorageRulesFromStorageCompleted += new EventHandler<ServiceReferenceAdministration.GetStorageRulesFromStorageCompletedEventArgs>(Client_GetStorageRulesFromStorageCompleted);
            CreateWaitDialog();
            Client.GetStorageRulesFromStorageAsync((Guid)InputParameters["ID"]);
        }

        void Client_GetStorageRulesFromStorageCompleted(object sender, ServiceReferenceAdministration.GetStorageRulesFromStorageCompletedEventArgs e)
        {
            if (e.Error != null)
                TrapError(e.Error);
            else
            {
                documents = e.Result;
                this.radGridView.DataSource = documents;
                if (this.radGridView.Rows.Count > 0)
                    this.radGridView.CurrentRow = this.radGridView.Rows.First();
            }
            CloseWaitDialog();
        }

        #region radGridView Events

        private void radGridView_ContextMenuOpening(object sender, Telerik.WinControls.UI.ContextMenuOpeningEventArgs e)
        {
            //GridDataCellElement cell = e.ContextMenuProvider as GridDataCellElement;
            //_IdItem = Convert.ToInt32(this.radGridView.Rows[cell.RowIndex].Cells["IdArchive"].Value);

            e.ContextMenu = menu.DropDown;
        }

        void itemAddNew_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UCStorageRuleDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "AddNew");
            OutputParameters.Add("StorageName", InputParameters["StorageName"]);
            OutputParameters.Add("IdStorage", InputParameters["ID"]);
            OutputParameters.Add("IdAttribute", String.Empty);
            OutputParameters.Add("IdArchive", InputParameters["IdArchive"]);
            OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);
            // Determino se permettere o meno la modifica dell'archive
            if (InputParameters["ArchiveName"].ToString() == String.Empty)
                OutputParameters.Add("ChangeArchive", "True");
            else
                OutputParameters.Add("ChangeArchive", "False");

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemModifica_Click(object sender, EventArgs e)
        {
            if (radGridView.SelectedRows.Count > 0)
            {
                // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
                ControlName = "UCStorageRuleDetail";
                OutputParameters = new Hashtable();
                OutputParameters.Add("Action", "Modify");
                OutputParameters.Add("StorageName", InputParameters["StorageName"]);
                OutputParameters.Add("IdStorage", InputParameters["ID"]);
                OutputParameters.Add("IdAttribute", radGridView.SelectedRows[0].Cells["Attribute.IdAttribute"].Value);
                OutputParameters.Add("IdArchive", radGridView.SelectedRows[0].Cells["Attribute.Archive.IdArchive"].Value);
                OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);
                // Determino se permettere o meno la modifica dell'archive
                if (InputParameters["ArchiveName"].ToString() == String.Empty)
                    OutputParameters.Add("ChangeArchive", "True");
                else
                    OutputParameters.Add("ChangeArchive", "False");


                // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
                Close_Click(sender, e);
            }
            else
            {
                RadMessageBox.Show(this,
                    "Please select a row first.",
                    "BiblosDS",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error);
            }
        }

        void itemDelete_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (RadMessageBox.Show("Sei sicuro di volere eliminare l'attributo", "BiblosDS", MessageBoxButtons.YesNo, Telerik.WinControls.RadMessageIcon.Question) == DialogResult.Yes)
            //    {
            //        ws.DeleteAttribute((Guid)radGridView.SelectedRows[0].Cells["IdAttribute"].Value);
            //    }
            //}
            //catch 
            //{
            //    String msg = "Non è possibile cancellare l'oggetto perchè viene utilizzato in almeno in un documento";
            //    RadMessageBox.Show(msg, "Biblos DS", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            //}
        }

        #endregion

        private void btBack_Click(object sender, EventArgs e)
        {
            BackToSenderControl(sender, e);
        }

        protected override void BackToSenderControl(object sender, EventArgs e)
        {
            // Determino il Chiamante
            if (InputParameters["ArchiveName"].ToString() == String.Empty)
            {
                // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
                ControlName = "UcStorages";
                OutputParameters = null;
            }
            else
            {
                ControlName = "UcArchiveStorageRelation";
                OutputParameters = new Hashtable();
                OutputParameters.Add("Action", "RelationToArchive");
                OutputParameters.Add("Name", InputParameters["ArchiveName"]);
                OutputParameters.Add("ID", InputParameters["IdArchive"]);
            }
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }
    }
}
