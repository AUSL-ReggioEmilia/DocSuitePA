using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReadAOO.Tenants.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReadAOO.Tenants.Models;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using TenantAOO = VecompSoftware.DocSuiteWeb.Entity.Tenants.TenantAOO;
namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReadAOO.Tenants
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.Management.ReadAOO.Tenants -> Critical error in construction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ Methods ]
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }

            try
            {
                ReadAOOTenantsEvents();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.Management.ReadAOO.Tenants -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private void ReadAOOTenantsEvents()
        {
            try
            {
                _logger.WriteInfo(new LogMessage("Start processing..."), LogCategories);

                IEnumerable<TenantAOO> tenantsAOO = new HashSet<TenantAOO>();
                IEnumerable<Tenant> tenants = new HashSet<Tenant>();

                #region TenantAOO management
                GetFromEndpoints<TenantAOO>("$filter=TenantTypology eq 'InternalTenant'", ref tenantsAOO, ref tenants);
                SendToManagementAOOWebAPI(tenantsAOO);
                #endregion

                #region Tenant management
                GetFromEndpoints<Tenant>("$filter=TenantTypology eq 'InternalTenant'&$expand=TenantAOO", ref tenantsAOO, ref tenants);
                SendToManagementAOOWebAPI(tenants);
                #endregion

                _logger.WriteInfo(new LogMessage("Processing ended."), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"ReadAOOTenantsEvents -> VSW.Management.ReadAOO.Tenants: {ex.Message}"), LogCategories);
                throw;
            }
        }

        private void GetFromEndpoints<T>(string odataFilter, ref IEnumerable<TenantAOO> tenantsAOO, ref IEnumerable<Tenant> tenants) where T : DSWBaseEntity
        {
            string entityName = typeof(T).Name;
            _logger.WriteDebug(new LogMessage($"Read {entityName} -> Initialize reading..."), LogCategories);
            foreach (Endpoint endpoint in _moduleConfiguration.Endpoints)
            {
                _logger.WriteDebug(new LogMessage($"Read {entityName} -> Reading from {endpoint.Name}."), LogCategories);

                if (entityName == "TenantAOO")
                {
                    PopulateTenantsAOO(endpoint.WebAPIUrl, odataFilter, ref tenantsAOO);
                }
                else
                {
                    PopulateTenants(endpoint.WebAPIUrl, odataFilter, ref tenants);
                }
            }
        }

        private void PopulateTenantsAOO(string externalWebAPIUrl, string odataFilter, ref IEnumerable<TenantAOO> tenantsAOO)
        {
            IWebAPIClient webAPIClient = new WebAPIClient(_logger, externalWebAPIUrl);
            ICollection<TenantAOO> result = webAPIClient.GetTenantsAOOAsync(odataFilter).Result;
            tenantsAOO = tenantsAOO.Concat(result);
        }

        private void PopulateTenants(string odataUrl, string odataFilter, ref IEnumerable<Tenant> tenants)
        {
            IWebAPIClient webAPIClient = new WebAPIClient(_logger, odataUrl);
            ICollection<Tenant> result = webAPIClient.GetTenantsAsync(odataFilter).Result;
            tenants = tenants.Concat(result);
        }

        private void SendToManagementAOOWebAPI<T>(IEnumerable<T> data)
        {
            string entityName = typeof(T).Name;
            _logger.WriteDebug(new LogMessage($"Send {entityName} -> Initialize sending..."), LogCategories);
            using (HttpClient client = new HttpClient())
            {
                string postUrl = $"{_moduleConfiguration.ManagementAOOUrl}api/{entityName}";
                HttpContent content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.PostAsync(postUrl, content).Wait();
            }
            _logger.WriteInfo(new LogMessage($"Send {entityName} -> All {entityName} collection successfully sent."), LogCategories);
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.Management.ReadAOO.Tenants"), LogCategories);
        }
        #endregion
    }
}
