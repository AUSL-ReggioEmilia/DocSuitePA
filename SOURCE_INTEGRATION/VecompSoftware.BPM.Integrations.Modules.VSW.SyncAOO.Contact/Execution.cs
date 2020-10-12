using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Contact.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Contact.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Models.Commons;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Contact
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
                _logger.WriteError(new LogMessage("VSW.SyncAOO.Contact -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("VSW.SyncAOO.Contact -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.SyncAOO.Contact"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);

                RegisterContactEventSubscriptionsListeners<IEventCompleteContactBuild>(_moduleConfiguration.ContactCreateSubscriptions, EventCreateContactCallback);
                RegisterContactEventSubscriptionsListeners<IEventCompleteContactDelete>(_moduleConfiguration.ContactDeleteSubscriptions, EventDeleteContactCallback);
                RegisterContactEventSubscriptionsListeners<IEventCompleteContactUpdate>(_moduleConfiguration.ContactUpdateSubscriptions, EventUpdateContactCallback);

                _needInitializeModule = false;
            }
        }

        private async Task EventCreateContactCallback(IEventCompleteContactBuild completeContactBuildEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventCreateContactCallback -> received callback with event id {completeContactBuildEvent.Id}"), LogCategories);

            try
            {
                ContactBuildModel contactBuildModel = GetContactBuildModelFromEvent(completeContactBuildEvent, "EventCreateContactCallback");
                string evaluatedSubscriptionName = GetEvaluatedSubscriptionName(messageProperties, completeContactBuildEvent.Id);

                IWebAPIClient webAPIClient = BuildWebAPIClientForSubscription(evaluatedSubscriptionName);
                int? contactAOOParentId = await webAPIClient.GetParameterContactAOOParentIdAsync();

                if (!contactAOOParentId.HasValue)
                {
                    throw new ArgumentNullException($"Parameter ContactAOOParentId is not defined for {evaluatedSubscriptionName}");
                }

                if (!contactBuildModel.Contact.ParentInsertId.HasValue)
                {
                    throw new ArgumentNullException($"Contact.ParentInsertId is not defined. ContactId:{contactBuildModel.Contact.UniqueId} on {evaluatedSubscriptionName}");
                }

                DocSuiteWeb.Entity.Commons.Contact contactToCreate = CreateContactEntityFromModel(contactBuildModel.Contact);
                if (contactBuildModel.Contact.ParentInsertId.Value == Guid.Empty)
                {
                    contactToCreate.IncrementalFather = contactAOOParentId;
                }
                else
                {
                    DocSuiteWeb.Entity.Commons.Contact docSuiteParentContact = (await _webAPIClient.GetContactAsync($"$filter=UniqueId eq {contactBuildModel.Contact.ParentInsertId.Value}")).Single();
                    contactToCreate.IncrementalFather = docSuiteParentContact.EntityId;
                }

                DocSuiteWeb.Entity.Commons.Contact createdContact = await webAPIClient.PostAsync(contactToCreate, retryPolicyEnabled: true);
                ICollection<Tenant> currentAOOTenants = await webAPIClient.GetTenantsAsync();

                await UpdateAOOTenantsContactsAsync(webAPIClient, currentAOOTenants, createdContact);

                _logger.WriteInfo(new LogMessage($"EventCreateContactCallback -> Contact {createdContact.UniqueId}/{createdContact.Description} has been successfully created for subscription {evaluatedSubscriptionName}"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCreateContactCallback -> error occouring when calling WebAPI POST action for event id {completeContactBuildEvent.Id}"), ex, LogCategories);
                throw;
            }
        }
        private async Task EventDeleteContactCallback(IEventCompleteContactDelete completeContactDeleteEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventDeleteContactCallback -> received callback with event id {completeContactDeleteEvent.Id}"), LogCategories);

            try
            {
                ContactBuildModel contactBuildModel = GetContactBuildModelFromEvent(completeContactDeleteEvent, "EventDeleteContactCallback");
                string evaluatedSubscriptionName = GetEvaluatedSubscriptionName(messageProperties, completeContactDeleteEvent.Id);

                DocSuiteWeb.Entity.Commons.Contact contactToDelete = CreateContactEntityFromModel(contactBuildModel.Contact);

                IWebAPIClient webAPIClient = BuildWebAPIClientForSubscription(evaluatedSubscriptionName);
                DocSuiteWeb.Entity.Commons.Contact deletedContact = await webAPIClient.DeleteAsync(contactToDelete, retryPolicyEnabled: true);

                _logger.WriteInfo(new LogMessage($"EventDeleteContactCallback -> Contact {deletedContact.UniqueId}/{deletedContact.Description} has been successfully deleted for subscription {evaluatedSubscriptionName}"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventDeleteContactCallback -> error occouring when calling WebAPI DELETE action for event id {completeContactDeleteEvent.Id}"), ex, LogCategories);
                throw;
            }
        }
        private async Task EventUpdateContactCallback(IEventCompleteContactUpdate completeContactUpdateEvent, IDictionary<string, object> messageProperties)
        {
            _logger.WriteInfo(new LogMessage($"EventUpdateContactCallback -> received callback with event id {completeContactUpdateEvent.Id}"), LogCategories);

            try
            {
                ContactBuildModel contactBuildModel = GetContactBuildModelFromEvent(completeContactUpdateEvent, "EventUpdateContactCallback");
                string evaluatedSubscriptionName = GetEvaluatedSubscriptionName(messageProperties, completeContactUpdateEvent.Id);

                DocSuiteWeb.Entity.Commons.Contact contactToUpdate = CreateContactEntityFromModel(contactBuildModel.Contact);

                IWebAPIClient webAPIClient = BuildWebAPIClientForSubscription(evaluatedSubscriptionName);
                DocSuiteWeb.Entity.Commons.Contact updatedContact = await webAPIClient.DeleteAsync(contactToUpdate, retryPolicyEnabled: true);

                _logger.WriteInfo(new LogMessage($"EventUpdateContactCallback -> Contact {updatedContact.UniqueId}/{updatedContact.Description} has been successfully updated for subscription {evaluatedSubscriptionName}"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventUpdateContactCallback -> error occouring when calling WebAPI PUT action for event id {completeContactUpdateEvent.Id}"), ex, LogCategories);
                throw;
            }
        }

        private void RegisterContactEventSubscriptionsListeners<TEvent>(ICollection<ContactSubscriptionModel> eventSubscriptions, Func<TEvent, IDictionary<string, object>, Task> eventActionHandler)
            where TEvent : IEvent
        {
            foreach (ContactSubscriptionModel subscriptionModel in eventSubscriptions)
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

        private ContactBuildModel GetContactBuildModelFromEvent<TEvent>(TEvent @event, string eventCallbackName)
            where TEvent : IEvent
        {
            ContactBuildModel contactBuildModel = @event.Content.ContentValue as ContactBuildModel;
            if (contactBuildModel == null)
            {
                _logger.WriteError(new LogMessage($"{eventCallbackName} -> ContactBuildModel is null"), LogCategories);
                throw new ArgumentNullException("ContactBuildModel", "ContactBuildModel is null.");
            }

            return contactBuildModel;
        }

        private string GetEvaluatedSubscriptionName(IDictionary<string, object> messageProperties, Guid eventId)
        {
            _logger.WriteInfo(new LogMessage($"Reading {CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property from event with id {eventId}"), LogCategories);
            string evaluatedSubscriptionName = messageProperties[CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME].ToString();

            if (string.IsNullOrEmpty(evaluatedSubscriptionName))
            {
                _logger.WriteError(new LogMessage($"EventSyncContactCallback -> error occouring when reading {CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property for event id {eventId}"), LogCategories);
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
                _logger.WriteError(new LogMessage($"EventSyncContactCallback -> error occouring when reading {CustomPropertyName.EVALUATED_SUBSCRIPTION_NAME} message property"), LogCategories);
                throw new ArgumentException($"WebAPI url not configured for subscription with name {evaluatedSubscriptionName}.");
            }

            string subscriptionWebAPIUrl = _subscriptionsWebAPIUrlsDictionary[evaluatedSubscriptionName];

            if (string.IsNullOrEmpty(subscriptionWebAPIUrl))
            {
                _logger.WriteError(new LogMessage($"EventSyncContactCallback -> error occouring when reading the WebAPIUrl for subscription {evaluatedSubscriptionName}"), LogCategories);
                throw new ArgumentNullException($"WebAPI url for subscription {evaluatedSubscriptionName} is null or empty.");
            }

            return new WebAPIClient(_logger, subscriptionWebAPIUrl);
        }

        private async Task UpdateAOOTenantsContactsAsync(IWebAPIClient webApiClient, ICollection<Tenant> aooTenants, DocSuiteWeb.Entity.Commons.Contact syncedContact)
        {
            foreach (Tenant tenant in aooTenants)
            {
                tenant.Contacts = new List<DocSuiteWeb.Entity.Commons.Contact> { syncedContact };
                await webApiClient.PutAsync(tenant, DocSuiteWeb.Common.Infrastructures.UpdateActionType.TenantContactAdd);

                _logger.WriteInfo(new LogMessage($"UpdateAOOTenantsContactsAsync -> Contact ({syncedContact.EntityId}/{syncedContact.Description}) has been successfully associated with tenant ({tenant.UniqueId}/{tenant.TenantName})"), LogCategories);
            }
        }

        private DocSuiteWeb.Entity.Commons.Contact CreateContactEntityFromModel(ContactModel contactModel)
        {
            DocSuiteWeb.Entity.Commons.Contact contactEntity = new DocSuiteWeb.Entity.Commons.Contact
            {
                UniqueId = contactModel.UniqueId,
                ActiveFrom = contactModel.ActiveFrom,
                ActiveTo = contactModel.ActiveTo,
                Address = contactModel.Address,
                BirthDate = contactModel.BirthDate,
                BirthPlace = contactModel.BirthPlace,
                CertifiedMail = contactModel.CertifiedMail,
                City = contactModel.City,
                CityCode = contactModel.CityCode,
                CivicNumber = contactModel.CivicNumber,
                Code = contactModel.Code,
                Description = contactModel.Description,
                EmailAddress = contactModel.Email,
                EntityId = contactModel.HasId() ? contactModel.Id.Value : 0,
                FaxNumber = contactModel.FaxNumber,
                FiscalCode = contactModel.FiscalCode,
                FullIncrementalPath = contactModel.FullIncrementalPath,
                IdContactType = contactModel.ContactType.ToString(),
                IncrementalFather = contactModel.IncrementalFather,
                IsActive = contactModel.IsActive,
                IsChanged = contactModel.IsChanged,
                IsLocked = contactModel.IsLocked,
                IsNotExpandable = contactModel.IsNotExpandable,
                LastChangedDate = contactModel.RegistrationDate,
                LastChangedUser = contactModel.LastChangedUser,
                Nationality = contactModel.Nationality,
                Note = contactModel.Note,
                PlaceName = contactModel.IdPlaceName.HasValue ? new ContactPlaceName { EntityShortId = contactModel.IdPlaceName.Value } : null,
                RegistrationDate = contactModel.RegistrationDate,
                RegistrationUser = contactModel.RegistrationUser,
                Role = contactModel.IdRole.HasValue ? new Role { EntityShortId = contactModel.IdRole.Value } : null,
                RoleRootContact = contactModel.IdRoleRootContact.HasValue ? new Role { EntityShortId = contactModel.IdRoleRootContact.Value } : null,
                SDIIdentification = contactModel.SDIIndentification,
                SearchCode = contactModel.SearchCode,
                TelephoneNumber = contactModel.TelephoneNumber,
                Title = contactModel.IdTitle.HasValue ? new ContactTitle { EntityId = contactModel.IdTitle.Value } : null,
                ZipCode = contactModel.ZipCode
            };

            return contactEntity;
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
