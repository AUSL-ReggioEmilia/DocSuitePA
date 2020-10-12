using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data.Helpers;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Tenants.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Tenants.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS.Events.Models.Audits;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Audits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Models.Audits;
using VecompSoftware.Services.Command.CQRS.Events.Models.Tenants;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Tenants
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private ISQLDbManager _dbManager;

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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.Management.Tenants -> Critical error in construction module"), ex, LogCategories);
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
                InitializeModule();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.Management.Tenants -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.Management.Tenants"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _dbManager = new SQLDbManager(_moduleConfiguration.DatabaseConnectionString);

                #region [ Tenant Events Listeners ]
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteTenantAOOBuild>(
                    ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.ServiceBusConfiguration.TopicName,
                    _moduleConfiguration.ServiceBusConfiguration.CreateTenantAOOSubscription,
                    EventCreateTenantAOOCallback));

                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteTenantBuild>(
                    ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.ServiceBusConfiguration.TopicName,
                    _moduleConfiguration.ServiceBusConfiguration.CreateTenantSubscription,
                    EventCreateTenantCallback));
                #endregion

                _needInitializeModule = false;
            }
        }
        private async Task EventCreateTenantAOOCallback(IEventCompleteTenantAOOBuild createTenantAOOEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventCreateTenantAOOCallback -> received callback with event id {createTenantAOOEvent.Id}"), LogCategories);

            try
            {
                TenantAOOModel tenantAOOModel = GetTenantAOOModelFromEvent(createTenantAOOEvent, "EventCreateTenantAOOCallback");
                SqlParameter[] insertContactSPParameters = InitializeInsertSPParameters(tenantAOOModel.IdTenantAOO, tenantAOOModel.TenantType, tenantAOOModel.Name, tenantAOOModel.Note,
                    tenantAOOModel.RegistrationUser, tenantAOOModel.RegistrationDate, tenantAOOModel.LastChangedUser, tenantAOOModel.LastChangedDate, categorySuffix: tenantAOOModel.CategorySuffix);

                int affectedRows = await _dbManager.ExecuteSqlCommandAsync(CommonDefinitionModel.SQL_SP_Tenant_Insert, CommandType.StoredProcedure, insertContactSPParameters);

                if (affectedRows == 0)
                {
                    throw new Exception($"Insert failed for tenantaoo with id {tenantAOOModel.IdTenantAOO}, affected rows count: {affectedRows}");
                }

                string insertSuccessMessage = $"TenantAOO {tenantAOOModel.IdTenantAOO}/{tenantAOOModel.Name} has been successfully created";

                _logger.WriteInfo(new LogMessage($"EventCreateTenantAOOCallback -> {insertSuccessMessage}"), LogCategories);

                await SendCreateAuditEvent(createTenantAOOEvent, tenantAOOModel.RegistrationUser, nameof(TenantAOO), tenantAOOModel.IdTenantAOO, tenantAOOModel.LastChangedDate, tenantAOOModel.LastChangedUser, insertSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCreateTenantAOOCallback -> error occouring when performing tenantaoo insert for event id {createTenantAOOEvent.Id}"), ex, LogCategories);
                throw;
            }
        }
        private async Task EventCreateTenantCallback(IEventCompleteTenantBuild createTenantEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventCreateTenantCallback -> received callback with event id {createTenantEvent.Id}"), LogCategories);

            try
            {
                TenantModel tenantModel = GetTenantModelFromEvent(createTenantEvent, "EventCreateTenantCallback");
                SqlParameter[] insertContactSPParameters = InitializeInsertSPParameters(tenantModel.TenantId, tenantModel.TenantType, tenantModel.TenantName, tenantModel.Note,
                    tenantModel.RegistrationUser, tenantModel.RegistrationDate, tenantModel.LastChangedUser, tenantModel.LastChangedDate, companyName: tenantModel.CompanyName, parentInsertId: tenantModel.TenantAOOId);

                int affectedRows = await _dbManager.ExecuteSqlCommandAsync(CommonDefinitionModel.SQL_SP_Tenant_Insert, CommandType.StoredProcedure, insertContactSPParameters);

                if (affectedRows == 0)
                {
                    throw new Exception($"Insert failed for tenant with id {tenantModel.TenantId}, affected rows count: {affectedRows}");
                }

                string insertSuccessMessage = $"Tenant {tenantModel.TenantId}/{tenantModel.TenantName} has been successfully created";

                _logger.WriteInfo(new LogMessage($"EventCreateTenantCallback -> {insertSuccessMessage}"), LogCategories);

                await SendCreateAuditEvent(createTenantEvent, tenantModel.RegistrationUser, nameof(Tenant), tenantModel.TenantId, tenantModel.LastChangedDate, tenantModel.LastChangedUser, insertSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCreateTenantCallback -> error occouring when performing tenant insert for event id {createTenantEvent.Id}"), ex, LogCategories);
                throw;
            }
        }
        private TenantAOOModel GetTenantAOOModelFromEvent(IEventCompleteTenantAOOBuild createTenantAOOEvent, string eventCallbackName)
        {
            TenantAOOBuildModel tenantAOOBuildModel = createTenantAOOEvent.ContentType.ContentTypeValue;

            if (tenantAOOBuildModel == null)
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> TenantAOOBuildModel is null"), LogCategories);
                throw new ArgumentNullException("TenantAOOBuildModel", "TenantAOOBuildModel is null.");
            }

            TenantAOOModel tenantAOOModel = tenantAOOBuildModel.TenantAOO;

            if (tenantAOOModel == null)
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> TenantAOOModel is null"), LogCategories);
                throw new ArgumentNullException("TenantAOOModel", "TenantAOOModel is null.");
            }

            return tenantAOOModel;
        }
        private TenantModel GetTenantModelFromEvent(IEventCompleteTenantBuild createTenantEvent, string eventCallbackName)
        {
            TenantBuildModel tenantBuildModel = createTenantEvent.ContentType.ContentTypeValue;

            if (tenantBuildModel == null)
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> TenantBuildModel is null"), LogCategories);
                throw new ArgumentNullException("TenantBuildModel", "TenantBuildModel is null.");
            }

            TenantModel tenantModel = tenantBuildModel.Tenant;

            if (tenantModel == null)
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> TenantModel is null"), LogCategories);
                throw new ArgumentNullException("TenantModel", "TenantModel is null.");
            }

            if (tenantModel.TenantAOOId == Guid.Empty)
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> Invalid TenantAOOId value for tenant, the value is: {tenantModel.TenantAOOId}"), LogCategories);
                throw new ArgumentNullException("TenantAOOModel", "Invalid TenantAOOId property value.");
            }

            return tenantModel;
        }
        private SqlParameter[] InitializeInsertSPParameters(Guid tenantId, TenantType tenantType, string name, string note,
            string registrationUser, DateTimeOffset registrationDate, string lastChangedUser, DateTimeOffset? lastChangedDate, string categorySuffix = null, string companyName = null, Guid? parentInsertId = null)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_TenantId, tenantId, DbType.Guid),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_TenantType, tenantType, DbType.Int16),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_Name, name, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_CompanyName, companyName, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_CategorySuffix, categorySuffix, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_Note, note, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_RegistrationUser, registrationUser, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_RegistrationDate, registrationDate, DbType.DateTimeOffset),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_LastChangedUser, lastChangedUser, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_LastChangedDate, lastChangedDate, DbType.DateTimeOffset),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Tenant_ParentInsertId, parentInsertId, DbType.Guid)
            };

            return sqlParameters.ToArray();
        }
        private async Task SendCreateAuditEvent<TEvent>(TEvent initiatorEvent, string registrationUser, string entityName,
            Guid entityUniqueId, DateTimeOffset? lastChangedDate, string lastChangedUser, string description) where TEvent : IEvent
        {
            AuditBuildModel auditBuildModel = new AuditBuildModel
            {
                RegistrationUser = registrationUser,
                Audit = new AuditModel
                {
                    Description = $"Audit initiated by event ({initiatorEvent.EventName}/{initiatorEvent.Id}). {description}.",
                    Account = registrationUser,
                    EntityName = entityName,
                    EntityUniqueId = entityUniqueId,
                    LogDate = DateTime.UtcNow,
                    LastChangedDate = lastChangedDate,
                    LastChangedUser = lastChangedUser,
                    Type = 1, // TODO: insert the right value
                    UserHost = Environment.MachineName
                }
            };

            IEventCompleteAuditBuild createAuditEvent = new EventCompleteAuditBuild(initiatorEvent.TenantName, initiatorEvent.TenantId, initiatorEvent.TenantAOOId, initiatorEvent.Identity, auditBuildModel);
            await _serviceBusClient.SendEventAsync(createAuditEvent, _moduleConfiguration.ServiceBusConfiguration.TopicName, null);
        }
        internal void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }
        #endregion
    }
}
