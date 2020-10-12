using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Audit.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Audit.Models;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data.Helpers;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Audits;
using VecompSoftware.Services.Command.CQRS.Events.Models.Audits;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Audit
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
                _logger.WriteError(new LogMessage("VSW.Management.Audit -> Critical error in construction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("VSW.Management.Audit -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.Management.Audit"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _dbManager = new SQLDbManager(_moduleConfiguration.DatabaseConnectionString);

                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteAuditBuild>(
                    ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.ServiceBusConfiguration.TopicName,
                    _moduleConfiguration.ServiceBusConfiguration.SubscriptionName,
                    EventCreateAuditCallback));

                _needInitializeModule = false;
            }
        }

        private async Task EventCreateAuditCallback(IEventCompleteAuditBuild completeAuditBuildEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventCreateAuditCallback -> received callback with event id {completeAuditBuildEvent.Id}"), LogCategories);

            try
            {
                AuditModel auditModel = GetAuditModelFromEvent(completeAuditBuildEvent);
                SqlParameter[] insertAuditSPParameters = InitializeInsertSPParameters(auditModel);

                int affectedRows = await _dbManager.ExecuteSqlCommandAsync(CommonDefinitionModel.SQL_SP_Audit_Insert, CommandType.StoredProcedure, insertAuditSPParameters);

                if (affectedRows == 0)
                {
                    throw new Exception($"Insert failed for audit with id {auditModel.AuditId}, affected rows count: {affectedRows}");
                }

                _logger.WriteInfo(new LogMessage($"EventCreateAuditCallback -> Audit {auditModel.AuditId}/{auditModel.Description} has been successfully created"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCreateAuditCallback -> error occouring when performing audit insert for event id {completeAuditBuildEvent.Id}"), ex, LogCategories);
                throw;
            }
        }

        private SqlParameter[] InitializeInsertSPParameters(AuditModel auditModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_AuditId, auditModel.AuditId, DbType.Guid),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_EntityUniqueId, auditModel.EntityUniqueId, DbType.Guid),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_EntityName, auditModel.EntityName, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_LogDate, auditModel.LogDate, DbType.DateTimeOffset),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_UserHost, auditModel.UserHost, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_Account, auditModel.Account, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_Type, auditModel.Type, DbType.Int16),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_Description, auditModel.Description, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_LastChangedUser, auditModel.LastChangedUser, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Audit_LastChangedDate, auditModel.LastChangedDate, DbType.DateTimeOffset)
            };

            return sqlParameters.ToArray();
        }

        private AuditModel GetAuditModelFromEvent(IEventCompleteAuditBuild completeAuditBuildEvent)
        {
            AuditBuildModel auditBuildModel = completeAuditBuildEvent.ContentType.ContentTypeValue;

            if (auditBuildModel == null)
            {
                _logger.WriteError(new LogMessage($"EventCreateAuditCallback -> AuditBuildModel is null"), LogCategories);
                throw new ArgumentNullException("AuditBuildModel", "AuditBuildModel is null.");
            }

            AuditModel auditModel = auditBuildModel.Audit;

            if (auditModel == null)
            {
                _logger.WriteError(new LogMessage($"EventCreateAuditCallback -> AuditModel is null"), LogCategories);
                throw new ArgumentNullException("AuditModel", "AuditModel is null.");
            }

            return auditModel;
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
