using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Contact.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Contact.Models;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data;
using VecompSoftware.BPM.Integrations.Modules.VSW.Management.Data.Helpers;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS.Events.Models.Audits;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Audits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Models.Audits;
using VecompSoftware.Services.Command.CQRS.Events.Models.Commons;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Contact
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
                _logger.WriteError(new LogMessage("VSW.Management.Contact -> Critical error in construction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("VSW.Management.Contact -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.Management.Contact"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _dbManager = new SQLDbManager(_moduleConfiguration.DatabaseConnectionString);

                #region [ ContactEventsListeners ]
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteContactBuild>(
                    ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.ServiceBusConfiguration.TopicName,
                    _moduleConfiguration.ServiceBusConfiguration.CreateContactSubscription,
                    EventCreateContactCallback));

                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteContactDelete>(
                    ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.ServiceBusConfiguration.TopicName,
                    _moduleConfiguration.ServiceBusConfiguration.DeleteContactSubscription,
                    EventDeleteContactCallback));

                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteContactUpdate>(
                    ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.ServiceBusConfiguration.TopicName,
                    _moduleConfiguration.ServiceBusConfiguration.UpdateContactSubscription,
                    EventUpdateContactCallback));
                #endregion

                _needInitializeModule = false;
            }
        }

        private async Task EventCreateContactCallback(IEventCompleteContactBuild completeContactBuildEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventCreateContactCallback -> received callback with event id {completeContactBuildEvent.Id}"), LogCategories);

            try
            {
                ContactModel contactModel = GetContactModelFromEvent(completeContactBuildEvent, "EventCreateContactCallback");
                SqlParameter[] insertContactSPParameters = InitializeInsertUpdateSPParameters(contactModel);

                int affectedRows = await _dbManager.ExecuteSqlCommandAsync(CommonDefinitionModel.SQL_SP_Contact_Insert, CommandType.StoredProcedure, insertContactSPParameters);

                if (affectedRows == 0)
                {
                    throw new Exception($"Insert failed for contact with id {contactModel.UniqueId}, affected rows count: {affectedRows}");
                }

                string insertSuccessMessage = $"Contact {contactModel.UniqueId}/{contactModel.Description} has been successfully created";

                _logger.WriteInfo(new LogMessage($"EventCreateContactCallback -> {insertSuccessMessage}"), LogCategories);

                await SendCreateAuditEvent(completeContactBuildEvent, contactModel, insertSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCreateContactCallback -> error occouring when performing contact insert for event id {completeContactBuildEvent.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventDeleteContactCallback(IEventCompleteContactDelete completeContactDeleteEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventDeleteContactCallback -> received callback with event id {completeContactDeleteEvent.Id}"), LogCategories);

            try
            {
                ContactModel contactModel = GetContactModelFromEvent(completeContactDeleteEvent, "EventDeleteContactCallback");
                SqlParameter[] sqlParameters = InitializeDeleteSPParameters(contactModel);

                int affectedRows = await _dbManager.ExecuteSqlCommandAsync(CommonDefinitionModel.SQL_SP_Contact_Delete, CommandType.StoredProcedure, sqlParameters);

                if (affectedRows == 0)
                {
                    throw new Exception($"Delete failed for contact with id {contactModel.UniqueId}, affected rows count: {affectedRows}");
                }

                string deleteSuccessMessage = $"Contact {contactModel.UniqueId}/{contactModel.Description} has been successfully deleted";

                _logger.WriteInfo(new LogMessage($"EventDeleteContactCallback -> {deleteSuccessMessage}"), LogCategories);

                await SendCreateAuditEvent(completeContactDeleteEvent, contactModel, deleteSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventDeleteContactCallback -> error occouring when performing contact delete for event id {completeContactDeleteEvent.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventUpdateContactCallback(IEventCompleteContactUpdate completeContactUpdateEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventUpdateContactCallback -> received callback with event id {completeContactUpdateEvent.Id}"), LogCategories);

            try
            {
                ContactModel contactModel = GetContactModelFromEvent(completeContactUpdateEvent, "EventUpdateContactCallback");
                SqlParameter[] updateSqlParameters = InitializeInsertUpdateSPParameters(contactModel, isCreateMethod: false);

                int affectedRows = await _dbManager.ExecuteSqlCommandAsync(CommonDefinitionModel.SQL_SP_Contact_Update, CommandType.StoredProcedure, updateSqlParameters);

                if (affectedRows == 0)
                {
                    throw new Exception($"Update failed for contact with id {contactModel.UniqueId}, affected rows count: {affectedRows}");
                }

                string updateSuccessMessage = $"Contact {contactModel.UniqueId}/{contactModel.Description} has been successfully updated";

                _logger.WriteInfo(new LogMessage($"EventUpdateContactCallback -> {updateSuccessMessage}"), LogCategories);

                await SendCreateAuditEvent(completeContactUpdateEvent, contactModel, updateSuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventUpdateContactCallback -> error occouring when performing contact update for event id {completeContactUpdateEvent.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task SendCreateAuditEvent<TEvent>(TEvent initiatorEvent, ContactModel contactModel, string description)
            where TEvent : IEvent
        {
            AuditBuildModel auditBuildModel = new AuditBuildModel
            {
                RegistrationUser = contactModel.RegistrationUser,
                Audit = new AuditModel
                {
                    Description = $"Audit initiated by event ({initiatorEvent.EventName}/{initiatorEvent.Id}). {description}.",
                    Account = contactModel.RegistrationUser,
                    EntityName = nameof(Contact),
                    EntityUniqueId = contactModel.UniqueId,
                    LogDate = DateTime.UtcNow,
                    LastChangedDate = contactModel.LastChangedDate,
                    LastChangedUser = contactModel.LastChangedUser,
                    Type = 1, // TODO: insert the right value
                    UserHost = Environment.MachineName
                }
            };

            IEventCompleteAuditBuild createAuditEvent = new EventCompleteAuditBuild(initiatorEvent.TenantName, initiatorEvent.TenantId, initiatorEvent.TenantAOOId, initiatorEvent.Identity, auditBuildModel);
            await _serviceBusClient.SendEventAsync(createAuditEvent, _moduleConfiguration.ServiceBusConfiguration.TopicName, null);
        }

        private SqlParameter[] InitializeDeleteSPParameters(ContactModel contactModel)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_ContactId, contactModel.UniqueId, DbType.Guid),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_IdTitle, contactModel.IdTitle, DbType.Int32),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_IdPlaceName, contactModel.IdPlaceName, DbType.Int16)
            };

            return sqlParameters.ToArray();
        }

        private SqlParameter[] InitializeInsertUpdateSPParameters(ContactModel contactModel, bool isCreateMethod = true)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_ContactId, contactModel.UniqueId, DbType.Guid),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_IdContactType, contactModel.ContactType.ToString(), DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_IdTitle, contactModel.IdTitle, DbType.Int32),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_IdPlaceName, contactModel.IdPlaceName, DbType.Int16),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_Description, contactModel.Description, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_BirthDate, contactModel.BirthDate, DbType.DateTime),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_Code, contactModel.Code, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_SearchCode, contactModel.SearchCode, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_FiscalCode, contactModel.FiscalCode, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_Address, contactModel.Address, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_CivicNumber, contactModel.CivicNumber, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_ZipCode, contactModel.ZipCode, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_City, contactModel.City, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_CityCode, contactModel.CityCode, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_TelephoneNumber, contactModel.TelephoneNumber, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_FaxNumber, contactModel.FaxNumber, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_EMailAddress, contactModel.Email, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_CertifiedMail, contactModel.CertifiedMail, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_Note, contactModel.Note, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_Language, contactModel.Language, DbType.Int32),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_Nationality, contactModel.Nationality, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_BirthPlace, contactModel.BirthPlace, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_SDIIdentification, contactModel.SDIIndentification, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_RegistrationUser, contactModel.RegistrationUser, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_RegistrationDate, contactModel.RegistrationDate, DbType.DateTimeOffset),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_LastChangedUser, contactModel.LastChangedUser, DbType.String),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_LastChangedDate, contactModel.LastChangedDate, DbType.DateTimeOffset),
                SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_IsActive, contactModel.IsActive, DbType.Boolean)
            };

            if (isCreateMethod)
            {
                sqlParameters.Add(SqlHelper.CreateSqlParameter(CommonDefinitionModel.SQL_Param_Contact_ParentInsertId, contactModel.ParentInsertId, DbType.Guid));
            }

            return sqlParameters.ToArray();
        }

        private ContactModel GetContactModelFromEvent<TEvent>(TEvent @event, string eventCallbackName)
            where TEvent : IEvent
        {
            if (!(@event.Content.ContentValue is ContactBuildModel contactBuildModel))
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> ContactBuildModel is null"), LogCategories);
                throw new ArgumentNullException("ContactBuildModel", "ContactBuildModel is null.");
            }

            ContactModel contactModel = contactBuildModel.Contact;

            if (contactModel == null)
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> ContactModel is null"), LogCategories);
                throw new ArgumentNullException("ContactModel", "ContactModel is null.");
            }

            return contactModel;
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
