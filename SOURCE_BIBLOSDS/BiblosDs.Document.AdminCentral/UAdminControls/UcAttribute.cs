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


namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UcAttribute : BaseAdminControls
    {
        BindingList<ServiceReferenceAdministration.Attribute> documents;
        RadContextMenu menu;

        public UcAttribute(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            // Init RadContextMenu
            menu = new RadContextMenu();
            RadMenuItem itemAddNew = new RadMenuItem();
            itemAddNew.Text = "Add a new Attribute";
            itemAddNew.Click += new EventHandler(itemAddNew_Click);
            menu.Items.Add(itemAddNew);
            RadMenuItem itemModifica = new RadMenuItem();
            itemModifica.Text = "Modify";
            itemModifica.Click += new EventHandler(itemModifica_Click);
            menu.Items.Add(itemModifica);
            RadMenuItem itemDelete = new RadMenuItem();
            itemDelete.Text = "Delete";
            itemDelete.Click += new EventHandler(itemDelete_Click);
            menu.Items.Add(itemDelete);

            this.radPanel1.Text = FormatTitle(new List<string>{
                                            "Archive " + InputParameters["ArchiveName"].ToString(),
                                            "Attribute List"
                                });


            this.Load += new EventHandler(UcAttribute_Load);
        }

        void UcAttribute_Load(object sender, EventArgs e)
        {
            // Carica dati
            CreateWaitDialog();
            Client.GetAttributesFromArchiveCompleted += new EventHandler<ServiceReferenceAdministration.GetAttributesFromArchiveCompletedEventArgs>(Client_GetAttributesFromArchiveCompleted);
            Client.GetAttributesFromArchiveAsync((Guid)InputParameters["ID"]);
        }

        void Client_GetAttributesFromArchiveCompleted(object sender, ServiceReferenceAdministration.GetAttributesFromArchiveCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.documents = e.Result;
                this.radGridView.DataSource = documents;
            }
            else 
            {
                TrapError(e.Error);
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
            ControlName = "UCAttributeDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "AddNew");
            OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);
            OutputParameters.Add("IdArchive", InputParameters["ID"]);
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemModifica_Click(object sender, EventArgs e)
        {
            if (radGridView.SelectedRows.Count > 0)
            {
                // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
                ControlName = "UCAttributeDetail";
                OutputParameters = new Hashtable();
                OutputParameters.Add("Action", "Modify");
                OutputParameters.Add("ArchiveName", InputParameters["ArchiveName"]);
                OutputParameters.Add("IdArchive", InputParameters["ID"]);
                OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdAttribute"].Value);

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
            try
            {
                if (radGridView.SelectedRows.Count > 0)
                {
                    if (RadMessageBox.Show("This operation will remove the attribute. Confirm ?", "BiblosDS", MessageBoxButtons.YesNo, Telerik.WinControls.RadMessageIcon.Question) == DialogResult.Yes)
                    {
                        CreateWaitDialog();
                        Client.DeleteAttributeCompleted += (object sender1, AsyncCompletedEventArgs e1) =>
                        {
                            CloseWaitDialog();
                            UcAttribute_Load(sender, e);
                        };
                        Client.DeleteAttributeAsync((Guid)radGridView.SelectedRows[0].Cells["IdAttribute"].Value);
                    }
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
            catch 
            {
                String msg = "It's impossible to remove the object because it's used in almost one document.";
                RadMessageBox.Show(msg, "Biblos DS", MessageBoxButtons.OK, Telerik.WinControls.RadMessageIcon.Error);
            }
        }

        #endregion

        private void btBack_Click(object sender, EventArgs e)
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
    }
}
