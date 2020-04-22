using System;
using System.Configuration;
using System.Data;
using System.ServiceModel.Configuration;
using System.Windows.Forms;
using VecompSoftware.JeepDashboard.Code;
using Vecompsoftware.FileServer.Services;

namespace VecompSoftware.JeepDashboard
{
    public partial class Configurator : Form
    {
        #region [ Fields ]

        private readonly Configuration _currentConfiguration;
        private readonly bool _currentConfigurationIsProtected;
        private readonly IFileRepositoryService _liveUpdateClient;

        #endregion

        #region [ Constructors ]

        public Configurator(string path, IFileRepositoryService client)
        {
            InitializeComponent();
            _liveUpdateClient = client;
            var isProtected = false;
            _currentConfiguration = ConfigurationManagerHelper.LoadConfiguration(path, client, ref isProtected);
            _currentConfigurationIsProtected = isProtected;
        }

        #endregion

        #region [ Events ]

        private void ParametersConfigGuiLoad(object sender, EventArgs e)
        {
            BindConfiguration();
        }

        private void BtnSaveClick(object sender, EventArgs e)
        {
            //Ricarica i parametri e li salva cifrati
            if (_currentConfiguration == null) return;

            // appSettings
            // Resetto i parametri
            _currentConfiguration.AppSettings.Settings.Clear();

            foreach (DataRow dr in ((DataTable)(((BindingSource)Parameters.DataSource).DataSource)).Rows)
            {
                var key = dr["Key"].ToString();
                var value = dr["Value"].ToString();
                _currentConfiguration.AppSettings.Settings.Add(key, value);
            }

            //connectionStrings
            _currentConfiguration.ConnectionStrings.ConnectionStrings.Clear();

            foreach (DataRow dr in ((DataTable)(((BindingSource)ConnectionStrings.DataSource).DataSource)).Rows)
            {
                var key = dr["Key"].ToString();
                var value = dr["Value"].ToString();
                _currentConfiguration.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(key, value, "System.Data.SqlClient"));
            }

            // Endpoints
            var serviceModelSectionGroup = ServiceModelSectionGroup.GetSectionGroup(_currentConfiguration);
            if (serviceModelSectionGroup != null)
            {
                var endpointCollection = serviceModelSectionGroup.Client.Endpoints;
                endpointCollection.Clear();

                foreach (DataRow dr in ((DataTable)(((BindingSource)Clients.DataSource).DataSource)).Rows)
                {
                    var address = dr["address"].ToString();
                    var binding = dr["binding"].ToString();
                    var bindingConfiguration = dr["bindingConfiguration"].ToString();
                    var contract = dr["contract"].ToString();
                    var name = dr["name"].ToString();
                    endpointCollection.Add(new ChannelEndpointElement
                                               {
                                                   Address = new Uri(address),
                                                   Binding = binding,
                                                   BindingConfiguration = bindingConfiguration,
                                                   Contract = contract,
                                                   Name = name
                                               });
                }
            }

            // Salvo tutto il file
            ConfigurationManagerHelper.SaveConfiguration(_currentConfiguration, _liveUpdateClient, chkEncrypt.Checked);
        }

        #endregion

        #region [ Methods ]

        private void BindConfiguration()
        {
            toolStripStatusLabel1.Text = _currentConfiguration.FilePath;

            // Gestione appSettings
            chkEncrypt.Checked = _currentConfigurationIsProtected;

            // Carico i parametri su lista
            var dt = new DataTable();
            dt.Columns.Add("Key");
            dt.Columns.Add("Value");

            //BindingSource to sync DataTable and DataGridView
            var bindingSourceBottomLevel = new BindingSource { DataSource = dt };

            foreach (var key in _currentConfiguration.AppSettings.Settings.AllKeys)
            {
                dt.Rows.Add(key, _currentConfiguration.AppSettings.Settings[key].Value);
            }

            Parameters.DataSource = bindingSourceBottomLevel;

            // Gestione ConnectionStrings
            // Carico i parametri su lista
            var dt1 = new DataTable();
            dt1.Columns.Add("Key");
            dt1.Columns.Add("Value");

            //BindingSource to sync DataTable and DataGridView
            var bindingSourceBottomLevel1 = new BindingSource { DataSource = dt1 };

            foreach (ConnectionStringSettings key in _currentConfiguration.ConnectionStrings.ConnectionStrings)
            {
                dt1.Rows.Add(key.Name, key.ConnectionString);
            }

            ConnectionStrings.DataSource = bindingSourceBottomLevel1;

            // Endpoints
            var serviceModelSectionGroup = ServiceModelSectionGroup.GetSectionGroup(_currentConfiguration);
            if (serviceModelSectionGroup != null)
            {
                var endpointCollection = serviceModelSectionGroup.Client.Endpoints;

                // Carico i parametri su lista
                var dt2 = new DataTable();
                dt2.Columns.Add("address");
                dt2.Columns.Add("binding");
                dt2.Columns.Add("bindingConfiguration");
                dt2.Columns.Add("contract");
                dt2.Columns.Add("name");

                foreach(ChannelEndpointElement endpointItem in endpointCollection)
                {
                    dt2.Rows.Add(endpointItem.Address, endpointItem.Binding, endpointItem.BindingConfiguration,
                                 endpointItem.Contract, endpointItem.Name);
                }

                //BindingSource to sync DataTable and DataGridView
                var bindingSourceBottomLevel2 = new BindingSource { DataSource = dt2 };

                Clients.DataSource = bindingSourceBottomLevel2;
            }
        }

        #endregion
    }
}
