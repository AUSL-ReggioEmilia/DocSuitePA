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
    public partial class UCArchive : BaseAdminControls
    {
        BindingList<Archive> archives;
        RadContextMenu menu;        
        public UCArchive(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();            
            this.Load += new EventHandler(UCArchive_Load);

            // Init RadContextMenu
            menu = new RadContextMenu();
            RadMenuItem itemAddNew = new RadMenuItem();
            itemAddNew.Text = "Insert a new Archive";
            itemAddNew.Click += new EventHandler(itemAddNew_Click);
            menu.Items.Add(itemAddNew);
            
            #region AttributeGroup
            RadMenuItem itemAttributeGroup = new RadMenuItem("Attribute Groups");
            itemAttributeGroup.Click += new EventHandler(itemAddNewAttributeGroup_Click);
            menu.Items.Add(itemAttributeGroup);
            #endregion

            RadMenuItem itemModifica = new RadMenuItem();
            itemModifica.Text = "Modify";
            itemModifica.Click += new EventHandler(itemModifica_Click);
            menu.Items.Add(itemModifica);
            RadMenuItem itemAddRelation = new RadMenuItem();
            itemAddRelation.Text = "Storages";
            itemAddRelation.Click += new EventHandler(itemAddRelation_Click);
            menu.Items.Add(itemAddRelation);
            RadMenuItem itemAddRelationAttributes = new RadMenuItem();
            itemAddRelationAttributes.Text = "Attributes";
            itemAddRelationAttributes.Click += new EventHandler(itemAddRelationAttributes_Click);
            menu.Items.Add(itemAddRelationAttributes);
            RadMenuItem itemDelete = new RadMenuItem();
            itemDelete.Text = "Delete";
            itemDelete.Visibility = ElementVisibility.Collapsed;
            itemDelete.Click += new EventHandler(itemDelete_Click);
            menu.Items.Add(itemDelete);
        }

        void itemModifyAttributeGroup_Click(object sender, EventArgs e)
        {
            //// Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            //ControlName = "UcAttributeGroupDetail";
            //OutputParameters = new Hashtable();
            //OutputParameters.Add("Action", action.update);
            //OutputParameters.Add("IdArchive", this.radGridView.SelectedRows[0].Cells["IdArchive"].Value);
            //// Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            //Close_Click(sender, e);
        }

        void itemAddNewAttributeGroup_Click(object sender, EventArgs e)
        {
            //// Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            //ControlName = "UcAttributeGroupDetail";
            //OutputParameters = new Hashtable();
            //OutputParameters.Add("Action", action.create);
            //OutputParameters.Add("IdArchive", this.radGridView.SelectedRows[0].Cells["IdArchive"].Value);
            //OutputParameters.Add("ArchiveName", this.radGridView.SelectedRows[0].Cells["Name"].Value);
            //// Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            //Close_Click(sender, e);
            if (this.radGridView.SelectedRows.Count > 0)
            {
                ControlName = "UcAttributeGroup";
                OutputParameters = new Hashtable();
                OutputParameters.Add("IdArchive", this.radGridView.SelectedRows[0].Cells["IdArchive"].Value);
                OutputParameters.Add("ArchiveName", this.radGridView.SelectedRows[0].Cells["Name"].Value);
                // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
                Close_Click(sender, e);
            }
            else 
            {
                RadMessageBox.Show(this,
                    "Select an archive to apply an AttributeGroup to it.",
                    "Error",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error);
            }
        }

        void UCArchive_Load(object sender, EventArgs e)
        {
            CreateWaitDialog();
            Client.GetArchivesAsync();
            Client.GetArchivesCompleted += (object pnj, ServiceReferenceAdministration.GetArchivesCompletedEventArgs args) =>
                {
                    if (args.Error == null)
                    {
                        archives = args.Result;
                        this.radGridView.DataSource = archives;
                        if (this.radGridView.Rows.Count > 0) this.radGridView.Rows.FirstOrDefault().IsCurrent = true;
                    }
                    else
                        TrapError(args.Error);
                    CloseWaitDialog();
                };                        
        }              

        #region radGridView Events

        private void radGridView_ContextMenuOpening(object sender, Telerik.WinControls.UI.ContextMenuOpeningEventArgs e)
        {          
            e.ContextMenu = menu.DropDown;
        }

        void itemAddNew_Click(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UCArchiveDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "AddNew");
            OutputParameters.Add("ArchiveName", String.Empty);
            OutputParameters.Add("ID", String.Empty);
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemModifica_Click(object sender, EventArgs e)
        {
            if (this.radGridView.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Select an \"Archive\".", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UCArchiveDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "Modify");
            OutputParameters.Add("ArchiveName", radGridView.SelectedRows[0].Cells["Name"].Value);
            OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdArchive"].Value);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemAddRelation_Click(object sender, EventArgs e)
        {
            if (this.radGridView.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Select an \"Archive\".", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcArchiveStorageRelation";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", "RelationToArchive");
            OutputParameters.Add("Name", radGridView.SelectedRows[0].Cells["Name"].Value);
            OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdArchive"].Value);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemAddRelationAttributes_Click(object sender, EventArgs e)
        {
            if (this.radGridView.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Select an \"Archive\".", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcAttribute";
            OutputParameters = new Hashtable();
            OutputParameters.Add("ID", radGridView.SelectedRows[0].Cells["IdArchive"].Value);
            OutputParameters.Add("ArchiveName", radGridView.SelectedRows[0].Cells["Name"].Value);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }

        void itemDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.radGridView.SelectedRows.Count <= 0)
                {
                    RadMessageBox.Show(this, "Select an \"Archive\".", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                if (RadMessageBox.Show("This operation will remove the archive. Confirm ?", "BiblosDS", MessageBoxButtons.YesNo, Telerik.WinControls.RadMessageIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show("TODO");                 
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

            //// Converto bytes in MB
            //foreach (GridViewDataRowInfo grv in radGridView.Rows)
            //{
            //    grv.Cells["MaxCache"].Value = Convert.ToInt64(grv.Cells["MaxCache"].Value) / (1024 * 1024);
            //    grv.Cells["UpperCache"].Value = Convert.ToInt64(grv.Cells["UpperCache"].Value) / (1024 * 1024);
            //    grv.Cells["LowerCache"].Value = Convert.ToInt64(grv.Cells["LowerCache"].Value) / (1024 * 1024);
            //}
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
