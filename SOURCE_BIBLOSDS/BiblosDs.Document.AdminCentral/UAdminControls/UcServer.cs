using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UcServer : BaseAdminControls
    {
        public UcServer()
            : this(null)
        {
        }

        public UcServer(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();

            foreach (var itm in cmServers.Items)
                itm.Click += new EventHandler(itm_Click);
        }

        private void itm_Click(object sender, EventArgs e)
        {
            var itm = sender as RadMenuItem;

            if (gvServers.SelectedRows.Count <= 0)
            {
                RadMessageBox.Show(this, "Select a \"Server\".", string.Empty, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            var srv = gvServers.SelectedRows.First().DataBoundItem as Server;

            if (itm.Text.Equals("Delete", StringComparison.InvariantCultureIgnoreCase))
            {
                if (MessageBox.Show(this, "Proceed?", "Biblos", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (Client.DeleteServer(srv))
                    {
                        MessageBox.Show(this, "Server deleted", "Biblos", MessageBoxButtons.OK);
                        UcServer_Load(sender, e);
                    }
                }
            }
            else
            {
                openDetails(itm.Text, srv);
            }
        }

        private void UcServer_Load(object sender, EventArgs e)
        {
            CreateWaitDialog();

            try
            {
                Client.GetServersCompleted += new EventHandler<ServiceReferenceAdministration.GetServersCompletedEventArgs>(Client_GetServersCompleted);
                Client.GetServersAsync();
            }
            catch
            {
                CloseWaitDialog();
            }
        }

        private void Client_GetServersCompleted(object sender, ServiceReferenceAdministration.GetServersCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                TrapError(e.Error);
            }
            else
            {
                gvServers.DataSource = e.Result;
                CloseWaitDialog();
            }
        }

        private void gvServers_ContextMenuOpening(object sender, Telerik.WinControls.UI.ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = cmServers.DropDown;
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            openDetails("AddNew", new Server());
        }

        private void openDetails(string action, Server server = null)
        {
            ControlName = "UcServerDetail";
            OutputParameters = new Hashtable();
            OutputParameters.Add("Action", action);
            OutputParameters.Add("Server", server);

            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(this, EventArgs.Empty);
        }
    }
}
