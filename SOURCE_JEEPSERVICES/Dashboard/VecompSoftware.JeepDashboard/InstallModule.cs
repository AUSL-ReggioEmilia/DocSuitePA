using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VecompSoftware.JeepDashboard.LiveUpdateServiceReference;

namespace VecompSoftware.JeepDashboard
{
    public partial class InstallModule : Form
    {
        private Dictionary<Guid, UpdateDefinition> Releases {get; set;}
        
        public InstallModule()
        {
            InitializeComponent();

            // Connessione al service
            var liveUpdateServiceClient = new LiveUpdateClient();
            Releases = liveUpdateServiceClient.GetLastVersions().ToDictionary(updateDefinition => updateDefinition.ModuleId, updateDefinition => updateDefinition);

            ddlModule.DataSource = new BindingSource(Releases.ToDictionary(release => release.Key, release => release.Value.ModuleName), null);
            ddlModule.DisplayMember = "Value";
            ddlModule.ValueMember = "Key";

            ModuleDefinitionUpdate(Releases.First().Key);
        }

        private void DdlModuleSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = ((ComboBox) sender).SelectedValue;
            ModuleDefinitionUpdate((Guid) selectedValue);
        }

        private void ModuleDefinitionUpdate(Guid guid)
        {
            var updateDefinition = Releases[guid];

            txtModuleLatestVersion.Text = updateDefinition.LatestVersion;
            txtReleaseNotes.Text = updateDefinition.LatestReleaseNotes;
        }
    }
}
