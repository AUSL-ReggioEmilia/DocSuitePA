using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BiblosDs.Document.AdminCentral.ServiceReferenceAdministration;

namespace BiblosDs.Document.AdminCentral
{
    public partial class ArchiveServerConfiguration : Form
    {
        private AdministrationClient client;
        private Archive serverArchive;
        public bool EditMode { get; private set; }
        private ArchiveServerConfig editedConfig;
        public ArchiveServerConfig Result
        {
            get
            {
                return editedConfig;
            }
            set
            {
                editedConfig = value;

                txtTransitPath.Text = editedConfig.TransitPath;
                ckEnabled.Checked = editedConfig.TransitEnabled;
            }
        }

        public ArchiveServerConfiguration(ref AdministrationClient adminClient, Archive archive, bool isEditing = false, ArchiveServerConfig editConfig = null)
        {
            InitializeComponent();

            client = adminClient;
            serverArchive = archive;
            EditMode = isEditing;
            editedConfig = editConfig ?? new ArchiveServerConfig { Archive = archive, TransitEnabled = true };
            ckEnabled.Checked = editedConfig.TransitEnabled;

            ckEnabled_CheckedChanged(null, EventArgs.Empty);

            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (browser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                txtTransitPath.Text = browser.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ckEnabled.Checked && string.IsNullOrWhiteSpace(txtTransitPath.Text))
            {
                MessageBox.Show(this, "Input the transit path in order to proceed", "Biblos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbServer.SelectedItem == null)
            {
                MessageBox.Show(this, "Selecte a server to proceed", "Biblos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            editedConfig.Server = cbServer.SelectedItem as Server;
            editedConfig.TransitEnabled = ckEnabled.Checked;
            editedConfig.TransitPath = string.IsNullOrWhiteSpace(txtTransitPath.Text) ? null : txtTransitPath.Text;

            DialogResult = System.Windows.Forms.DialogResult.OK;

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;

            Close();
        }

        private void ArchiveServerConfiguration_Load(object sender, EventArgs e)
        {
            cbServer.DataSource = client.GetServers();

            if (editedConfig.Server != null)
                cbServer.SelectedItem = editedConfig.Server;

            txtTransitPath.Text = editedConfig.TransitPath;
            ckEnabled.Checked = editedConfig.TransitEnabled;
        }

        private void ckEnabled_CheckedChanged(object sender, EventArgs e)
        {
            txtTransitPath.Enabled = ckEnabled.Checked;
            //Ripulisce la textbox (niente path del transito!)
            if (!txtTransitPath.Enabled)
                txtTransitPath.Clear();
        }
    }
}
