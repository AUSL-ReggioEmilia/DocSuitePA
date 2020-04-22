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
    public partial class UcStorageArea : BaseAdminControls
    {
        BindingList<StorageArea> documents;
        RadContextMenu menu;
        public UcStorageArea(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            VerifyInputParameters(new List<string> { "ID", "StorageName", "IdArchive", "ArchiveName" });

            // Init RadContextMenu
            menu = new RadContextMenu();
            RadMenuItem itemAddNew = new RadMenuItem();
            itemAddNew.Text = "Insert a new Storage Area";
            itemAddNew.Click += new EventHandler(itemAddNew_Click);
            menu.Items.Add(itemAddNew);
            RadMenuItem itemModifica = new RadMenuItem();
            itemModifica.Text = "Modify";
            itemModifica.Click += new EventHandler(itemModifica_Click);
            menu.Items.Add(itemModifica);
            RadMenuItem itemStorageAreaRule = new RadMenuItem();
            itemStorageAreaRule.Text = "Storage Area Rule";
            itemStorageAreaRule.Click += new EventHandler(itemStorageAreaRule_Click);
            menu.Items.Add(itemStorageAreaRule);
            RadMenuItem itemDelete = new RadMenuItem();
            itemDelete.Text = "Delete";
            itemDelete.Visibility = ElementVisibility.Collapsed;
            itemDelete.Click += new EventHandler(itemDelete_Click);
            menu.Items.Add(itemDelete);

            if (InputParameters["ArchiveName"].ToString() == String.Empty)
            {
                radPanel1.Text = FormatTitle(new List<string>{
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "StorageArea List"
                                });

                btBack.Text = "Back";
            }
            else
            {
                radPanel1.Text = FormatTitle(new List<string>{
                                            "Archive " + InputParameters["ArchiveName"].ToString(),
                                            "Storage " + InputParameters["StorageName"].ToString(),
                                            "StorageArea List"
                                });
                btBack.Text = "Back";
            }

            this.Load += new EventHandler(UcAttribute_Load);
        }


        void UcAttribute_Load(object sender, EventArgs e)
        {
            CreateWaitDialog();

            // Carica dati
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (InputParameters["ArchiveName"].ToString() == String.Empty)
            {
                // Carico gli storage area relativi a questo storage
                documents = new BindingList<StorageArea>(Client.GetStorageAreas((Guid)InputParameters["ID"]));
            }
            else
            {
                // Carico solo gli storage area relativi a questo storage e che collegati all'archivio di partenza
                documents = new BindingList<StorageArea>(Client.GetStorageAreasFromStorageArchive((Guid)InputParameters["ID"], (Guid)InputParameters["IdArchive"]));
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                TrapError(e.Error);
            else
            {
                this.radGridView.DataSource = documents;
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
            ControlName = "UCStorageAreaDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "AddNew");
            OutputParameters.Add("StorageName", InputParameters["StorageName"]);
            OutputParameters.Add("IdStorage", InputParameters["ID"]);
            OutputParameters.Add("IdArchive", InputParameters["IdArchive"]);
            OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemModifica_Click(object sender, EventArgs e)
        {
            if (radGridView.SelectedRows.Count > 0)
            {
                // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
                ControlName = "UCStorageAreaDetail";
                OutputParameters = new Hashtable();
                OutputParameters.Add("Action", "Modify");
                OutputParameters.Add("StorageName", InputParameters["StorageName"]);
                OutputParameters.Add("IdStorage", InputParameters["ID"]);
                OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdStorageArea"].Value);
                OutputParameters.Add("IdArchive", InputParameters["IdArchive"]);
                OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);

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

        void itemStorageAreaRule_Click(object sender, EventArgs e)
        {
            if (radGridView.SelectedRows.Count > 0)
            {
                // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
                ControlName = "UCStorageAreaRule";
                OutputParameters = new Hashtable();
                OutputParameters.Add("StorageName", InputParameters["StorageName"]);
                OutputParameters.Add("IdStorage", InputParameters["ID"]);
                OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);
                OutputParameters.Add("IdArchive", InputParameters["IdArchive"]);
                OutputParameters.Add("StorageAreaName", radGridView.SelectedRows[0].Cells["Name"].Value);
                OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdStorageArea"].Value);

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
            Close_Click(sender, e);
        }

        private void radGridView_DataBindingComplete(object sender, GridViewBindingCompleteEventArgs e)
        {
            // Converto bytes in MB
            foreach (GridViewDataRowInfo grv in radGridView.Rows)
            {
                grv.Cells["MaxSize"].Value = Convert.ToInt64(grv.Cells["MaxSize"].Value) / (1024 * 1024);
                grv.Cells["CurrentSize"].Value = Convert.ToInt64(grv.Cells["CurrentSize"].Value) / (1024 * 1024);
            }
        }
    }
}
