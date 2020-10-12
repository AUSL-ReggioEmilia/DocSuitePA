using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Tenants.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Tenants.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Models.Tenants;
using TenantTypologyType = VecompSoftware.DocSuiteWeb.Entity.Tenants.TenantTypologyType;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Tenants
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
        private readonly IDictionary<string, string> _subscriptionsWebAPIUrlsDictionary;

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
                _subscriptionsWebAPIUrlsDictionary = new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.SyncAOO.Tenants -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("VSW.SyncAOO.Tenants -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.SyncAOO.Tenants"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);

                RegisterTenantsEventSubscriptionsListeners<IEventCompleteTenantAOOBuild>(_moduleConfiguration.TenantAOOCreateSubscriptions, EventCreateTenantAOOCallback);
                RegisterTenantsEventSubscriptionsListeners<IEventCompleteTenantBuild>(_moduleConfiguration.TenantCreateSubscriptions, EventCreateTenantCallback);

                _needInitializeModule = false;
            }
        }

        private async Task EventCreateTenantAOOCallback(IEventCompleteTenantAOOBuild completeTenantAOOBuildEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventCreateTenantAOOCallback -> received callback with event id {completeTenantAOOBuildEvent.Id}"), LogCategories);

            try
            {
                TenantAOOModel tenantAOOModel = GetTenantAOOModelFromEvent(completeTenantAOOBuildEvent, "EventCreateTenantAOOCallback");
                string evaluatedSubscriptionName = GetEvaluatedSubscriptionName(messageProperties, completeTenantAOOBuildEvent.Id);

                IWebAPIClient externalWebAPIClient = BuildWebAPIClientForSubscription(evaluatedSubscriptionName);
                TenantAOO result = await externalWebAPIClient.GetTenantAOOAsync(tenantAOOModel.IdTenantAOO);
                if (result != null)
                {
                    _logger.WriteInfo(new LogMessage($"EventCreateTenantAOOCallback -> TenantAOO {result.UniqueId}/{result.Name} for subscription {evaluatedSubscriptionName} already exist and it is going to be skip"), LogCategories);
                    return;
                }
                TenantAOO tenantAOOToCreate = CreateTenantAOOEntityFromModel(tenantAOOModel);
                TenantAOO createdTenantAOO = await externalWebAPIClient.PostAsync(tenantAOOToCreate, retryPolicyEnabled: true);
                _logger.WriteInfo(new LogMessage($"EventCreateTenantAOOCallback -> TenantAOO {createdTenantAOO.UniqueId}/{createdTenantAOO.Name} has been successfully created for subscription {evaluatedSubscriptionName}"), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCreateTenantAOOCallback -> error occouring when calling WebAPI POST action for event id {completeTenantAOOBuildEvent.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventCreateTenantCallback(IEventCompleteTenantBuild completeTenantBuildEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventCreateTenantCallback -> received callback with event id {completeTenantBuildEvent.Id}"), LogCategories);

            try
            {
                TenantModel tenantModel = GetTenantModelFromEvent(completeTenantBuildEvent, "EventCreateTenantCallback");
                string evaluatedSubscriptionName = GetEvaluatedSubscriptionName(messageProperties, completeTenantBuildEvent.Id);

                IWebAPIClient externalWebAPIClient = BuildWebAPIClientForSubscription(evaluatedSubscriptionName);
                Tenant result = await externalWebAPIClient.GetTenantAsync(tenantModel.TenantId);
                if (result != null)
                {
                    _logger.WriteInfo(new LogMessage($"EventCreateTenantAOOCallback -> Tenant {result.UniqueId}/{result.TenantName} for subscription {evaluatedSubscriptionName} already exist and it is going to be skip"), LogCategories);
                    return;
                }
                Tenant tenantToCreate = CreateTenantEntityFromModel(tenantModel);
                Tenant createdTenant = await externalWebAPIClient.PostAsync(tenantToCreate, retryPolicyEnabled: true);

                _logger.WriteInfo(new LogMessage($"EventCreateTenantCallback -> Tenant {createdTenant.UniqueId}/{createdTenant.TenantName} has been successfully created for subscription {evaluatedSubscriptionName}"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCreateTenantCallback -> error occouring when calling WebAPI POST action for event id {completeTenantBuildEvent.Id}"), ex, LogCategories);
                throw;
            }
        }

        private void RegisterTenantsEventSubscriptionsListeners<TEvent>(ICollection<TenantSubscriptionModel> eventSubscriptions, Func<TEvent, IDictionary<string, object>, Task> eventActionHandler)
            where TEvent : IEvent
        {
            foreach (TenantSubscriptionModel subscriptionModel in eventSubscriptions)
            {
                if (!_subscriptionsWebAPIUrlsDictionary.ContainsKey(subscriptionModel.SubscriptionName))
                {
                    _subscriptionsWebAPIUrlsDictionary.Add(subscriptionModel.SubscriptionName, subscriptionModel.WebAPIUrl);
                }

                _subscriptions.Add(_serviceBusClient.StartListening(
                    ModuleConfigurationHelper.MODULE_NAME,
                    _moduleConfiguration.SyncTopicName,
                    subscriptionModel.SubscriptionName,
                    eventActionHandler));
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
        private string GetEvaluatedSubscriptionName(IDictionary<string, object> messageProperties, Guid eventId)
        {
            _logger.WriteInfo(new LogMessage($"Reading {CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property from event with id {eventId}"), LogCategories);
            string evaluatedSubscriptionName = messageProperties[CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME].ToString();

            if (string.IsNullOrEmpty(evaluatedSubscriptionName))
            {
                _logger.WriteError(new LogMessage($"EventSyncTenantsCallback -> error occouring when reading {CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property for event id {eventId}"), LogCategories);
                throw new ArgumentNullException(CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME, $"{CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property is null or empty.");
            }

            _logger.WriteInfo(new LogMessage($"Readed {CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property with value {evaluatedSubscriptionName}"), LogCategories);
            return evaluatedSubscriptionName;
        }
        private IWebAPIClient BuildWebAPIClientForSubscription(string evaluatedSubscriptionName)
        {
            _logger.WriteInfo(new LogMessage($"Reading webapi url from module configuration, for subscription {evaluatedSubscriptionName}"), LogCategories);
            bool subscriptionWebAPIUrlIsConfigured = _subscriptionsWebAPIUrlsDictionary.ContainsKey(evaluatedSubscriptionName);

            if (!subscriptionWebAPIUrlIsConfigured)
            {
                _logger.WriteError(new LogMessage($"EventSyncTenantsCallback -> error occouring when reading {CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property"), LogCategories);
                throw new ArgumentException($"WebAPI url not configured for subscription with name {evaluatedSubscriptionName}.");
            }

            string subscriptionWebAPIUrl = _subscriptionsWebAPIUrlsDictionary[evaluatedSubscriptionName];

            if (string.IsNullOrEmpty(subscriptionWebAPIUrl))
            {
                _logger.WriteError(new LogMessage($"EventSyncTenantsCallback -> error occouring when reading the WebAPIUrl for subscription {evaluatedSubscriptionName}"), LogCategories);
                throw new ArgumentNullException($"WebAPI url for subscription {evaluatedSubscriptionName} is null or empty.");
            }

            return new WebAPIClient(_logger, subscriptionWebAPIUrl);
        }

        private TenantAOO CreateTenantAOOEntityFromModel(TenantAOOModel tenantAOOModel)
        {
            TenantAOO tenantAOOEntity = new TenantAOO
            {
                UniqueId = tenantAOOModel.IdTenantAOO,
                Name = tenantAOOModel.Name,
                Note = tenantAOOModel.Note,
                CategorySuffix = tenantAOOModel.CategorySuffix,
                TenantTypology = TenantTypologyType.ExternalTenant,
                RegistrationUser = tenantAOOModel.RegistrationUser,
                RegistrationDate = tenantAOOModel.RegistrationDate,
                LastChangedDate = tenantAOOModel.LastChangedDate,
                LastChangedUser = tenantAOOModel.LastChangedUser
            };

            return tenantAOOEntity;
        }
        private Tenant CreateTenantEntityFromModel(TenantModel tenantModel)
        {
            Tenant tenantEntity = new Tenant
            {
                UniqueId = tenantModel.TenantId,
                TenantAOO = new TenantAOO { UniqueId = tenantModel.TenantAOOId },
                Note = tenantModel.Note,
                TenantName = tenantModel.TenantName,
                CompanyName = tenantModel.CompanyName,
                TenantTypology = TenantTypologyType.ExternalTenant,
                RegistrationUser = tenantModel.RegistrationUser,
                RegistrationDate = tenantModel.RegistrationDate,
                LastChangedDate = tenantModel.LastChangedDate,
                LastChangedUser = tenantModel.LastChangedUser
            };

            return tenantEntity;
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
