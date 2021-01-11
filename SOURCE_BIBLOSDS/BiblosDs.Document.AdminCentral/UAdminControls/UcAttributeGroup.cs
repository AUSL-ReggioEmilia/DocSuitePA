using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;
using Telerik.WinControls.UI;
using System.Configuration;
using Telerik.WinControls;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UcAttributeGroup : BaseAdminControls
    {
        private Guid IdArchive;
        private string ArchiveName;
        private RadContextMenu contextMenu;

        public UcAttributeGroup(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();
            this.contextMenu = new RadContextMenu();

            RadMenuItem itmUpd = new RadMenuItem("Update");
            itmUpd.Click += this.btUpdateAg_Click;
            this.contextMenu.Items.Add(itmUpd);

            RadMenuItem itmDel = new RadMenuItem("Delete");
            itmDel.Click += new EventHandler(btDeleteAg_Click);
            this.contextMenu.Items.Add(itmDel);

            this.radGridViewAG.ContextMenuOpening+=new ContextMenuOpeningEventHandler(radGridViewAG_ContextMenuOpening);

            this.radGridViewAG.Columns["IdAttributeGroup"].DataType = typeof(Guid);
            this.radGridViewAG.Columns["IdArchive"].DataType = typeof(Guid);
            // Caricamento dati da DB.
            this.Load += new EventHandler(UcAttributeGroup_Load);
        }

        void btDeleteAg_Click(object sender, EventArgs e)
        {
            if (this.radGridViewAG.SelectedRows.Count > 0)
            {
                if (RadMessageBox.Show("This operation will remove the attribute group. Confirm?", "BiblosDS", MessageBoxButtons.YesNo, Telerik.WinControls.RadMessageIcon.Question) == DialogResult.Yes)
                {
                    Client.DeleteAttributeGroupCompleted += (s, args) =>
                    {
                        base.CloseWaitDialog();
                        this.LoadAttributeGroupGrid();
                    };
                    base.CreateWaitDialog();
                    Client.DeleteAttributeGroupAsync((Guid)this.radGridViewAG.SelectedRows[0].Cells["IdAttributeGroup"].Value);
                }
            }
        }

        void radGridViewAG_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = this.contextMenu.DropDown;
        }

        protected override void BackToSenderControl(object sender, EventArgs e)
        {
            ControlName = "UcArchive";
            base.OutputParameters = null;
            base.Close_Click(sender, e);
        }

        void UcAttributeGroup_Load(object sender, EventArgs e)
        {
            VerifyInputParameters(new List<string> { "IdArchive" });
            this.IdArchive = (Guid)InputParameters["IdArchive"];
            object archName = InputParameters["ArchiveName"];
            this.ArchiveName = (archName != null) ? archName.ToString() : string.Empty;
            this.radTitlePanel.Text = string.Format("[Archive {0}] - Attribute Groups", this.ArchiveName);
            this.LoadAttributeGroupGrid();
        }

        public void LoadAttributeGroupGrid()
        {
            BindingList<AttributeGroup> agList = Client.GetAttributeGroup(IdArchive);
            this.radGridViewAG.Rows.Clear();
            foreach (AttributeGroup ag in agList)
            {
                this.radGridViewAG.Rows.Add(IdArchive, this.ArchiveName, ag.IdAttributeGroup, ag.Description, ag.GroupType.ToString());
            }
            if (this.radGridViewAG.Rows.Count > 0)
            {
                base.ShowGuid(radGridViewAG, ConfigurationManager.AppSettings["ShowGUID"] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["ShowGUID"]));
            }
        }

        private void btUpdateAg_Click(object sender, EventArgs e)
        {
            if (this.radGridViewAG.SelectedRows.Count > 0)
            {
                base.ControlName = "UcAttributeGroupDetail";
                base.OutputParameters = new Hashtable();
                base.OutputParameters.Add("Action", action.update);
                this.OutputParameters.Add("IdAttributeGroup", this.radGridViewAG.SelectedRows[0].Cells["IdAttributeGroup"].Value);
                base.OutputParameters.Add("IdArchive", this.radGridViewAG.SelectedRows[0].Cells["IdArchive"].Value);
                base.OutputParameters.Add("ArchiveName", this.radGridViewAG.SelectedRows[0].Cells["Name"].Value);
                base.Close_Click(sender, e);
            }
        }

        private void btInsertAg_Click(object sender, EventArgs e)
        {
            base.ControlName = "UcAttributeGroupDetail";
            base.OutputParameters = new Hashtable();
            base.OutputParameters.Add("Action", action.create);
            base.OutputParameters.Add("IdArchive", this.IdArchive);
            base.OutputParameters.Add("ArchiveName", this.ArchiveName);
            base.Close_Click(sender, e);
        }

        private void btBack_Click(object sender, EventArgs e)
        {
            this.BackToSenderControl(sender, e);
        }
    }
}
