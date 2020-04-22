using System.Data;
using System.Windows.Forms;

namespace VecompSoftware.JeepDashboard
{
    public partial class ManageUpdates : Form
    {
        public ManageUpdates()
        {
            InitializeComponent();

            // Connessione al service
            var liveUpdateServiceClient = new LiveUpdateServiceReference.LiveUpdateClient();
            var releases = liveUpdateServiceClient.GetLastVersions();
            var dataTable = new DataTable("LatestReleases");
            dataTable.Columns.Add("Nome modulo");
            dataTable.Columns.Add("Versione");
            dataTable.Columns.Add("Note rilascio");
            foreach (var updateDefinition in releases)
            {
                var row = dataTable.NewRow();
                row["Nome modulo"] = updateDefinition.ModuleName;
                row["Versione"] = updateDefinition.LatestVersion;
                row["Note rilascio"] = updateDefinition.LatestReleaseNotes;
                dataTable.Rows.Add(row);
            }

            latestPackagesGridView.DataSource = dataTable;
        }
    }
}
