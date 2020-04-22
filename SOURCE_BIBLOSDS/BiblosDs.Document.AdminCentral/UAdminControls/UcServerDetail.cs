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

namespace BiblosDs.Document.AdminCentral.UAdminControls
{
    public partial class UcServerDetail : BaseAdminControls
    {
        private Server server;
        private bool insertMode;

        public UcServerDetail()
            : this(null)
        {
        }

        public UcServerDetail(Hashtable parameters)
            : base(parameters)
        {
            InitializeComponent();
        }

        private void UcServerDetail_Load(object sender, EventArgs e)
        {
            VerifyInputParameters(new List<string> { "Action", "Server" });

            cbRole.DataSource = Enum.GetNames(typeof(ServerRole));
            server = InputParameters["Server"] as Server;
            insertMode = false;

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    this.radPanelTitle.Text = "New Server";
                    this.btUpdate.Text = "Insert";
                    this.btCancel.Text = "Cancel";
                    this.insertMode = true;
                    break;
                case "modify":
                    this.radPanelTitle.Text = FormatTitle(new List<string>{
                                            "Server " + server.ServerName,
                                            "Modify"
                                });

                    this.btUpdate.Text = "Update";
                    this.btCancel.Text = "Cancel";
                    break;
            }

            switch (InputParameters["Action"].ToString().ToLower())
            {
                case "addnew":
                    break;
                case "modify":
                    txtName.Text = server.ServerName;
                    cbRole.SelectedItem = server.ServerRole.ToString();
                    break;
            }
        }

        protected override void BackToSenderControl(object sender, EventArgs e)
        {
            // Imposto il nome del nuovo controllo, comprensivo dei parametri, che dovrà essere caricato successivamente
            ControlName = "UcServer";
            OutputParameters = null;
            // Scateno l'evento che verrà intercettato dal container del controllo e si occuperà di creare il nuovo controllo
            Close_Click(sender, e);
        }


        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (canSave())
            {
                server.ServerName = txtName.Text;
                server.ServerRole = (ServerRole)Enum.Parse(typeof(ServerRole), cbRole.SelectedItem.ToString());
                server = (insertMode) ? Client.AddServer(server) : Client.UpdateServer(server);
                BackToSenderControl(sender, e);
            }
            else
            {
                MessageBox.Show(this, "Alcuni parametri non sono validi: impossibile procedere.", "Biblos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool canSave()
        {
            ServerRole role;

            try
            {
                role = (ServerRole)Enum.Parse(typeof(ServerRole), cbRole.SelectedItem.ToString());
            }
            catch { role = ServerRole.Undefined; }

            return !string.IsNullOrWhiteSpace(txtName.Text)
                && role != ServerRole.Undefined;
        }
    }
}
