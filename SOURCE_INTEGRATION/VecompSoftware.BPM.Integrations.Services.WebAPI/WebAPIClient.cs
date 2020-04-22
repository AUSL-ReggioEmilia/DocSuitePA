using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.WebAPI.Exceptions;
using VecompSoftware.BPM.Integrations.Services.WebAPI.Models;
using VecompSoftware.Clients.WebAPI.Configuration;
using VecompSoftware.Clients.WebAPI.Exceptions;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Services.Command;
namespace VecompSoftware.BPM.Integrations.Services.WebAPI
{
    [LogCategory(LogCategoryDefinition.WEBAPIWORKFLOW)]
    public class WebAPIClient : IWebAPIClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly WebApiHttpClient _httpClient = null;
        private readonly IHttpClientConfiguration _originalHttpClientConfiguration;
        private const string ADDRESSES_CONFIG_NAME = "WebApi.Client.Config.Addresses.json";
        private const int _retry_tentative = 10;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        #endregion

        #region [ ODATA Controller Name ]
        private const string ODATA_DOCUMENTUNIT_CONTROLLER_NAME = "DocumentUnits";
        private const string ODATA_TEMPLATE_DOCUMENT_REPOSITORY_CONTROLLER_NAME = "TemplateDocumentRepositories";
        private const string ODATA_DOCUMENTUNITCHAIN_CONTROLLER_NAME = "DocumentUnitChains";
        private const string ODATA_USER_CONTROLLER_NAME = "DocumentUnitUsers";
        private const string ODATA_FASCICLE_CONTROLLER_NAME = "Fascicles";
        private const string ODATA_FASCICLE_DOCUMENTUNIT_CONTROLLER_NAME = "FascicleDocumentUnits";
        private const string ODATA_PROTOCOLADVANCED_CONTROLLER_NAME = "AdvancedProtocols";
        private const string ODATA_COLLABORATION_CONTROLLER_NAME = "Collaborations";
        private const string ODATA_PROTOCOL_CONTROLLER_NAME = "Protocols";
        private const string ODATA_PROTOCOL_LOG_CONTROLLER_NAME = "ProtocolLogs";
        private const string ODATA_RESOLUTION_CONTROLLER_NAME = "Resolutions";
        private const string ODATA_RESOLUTION_LOG_CONTROLLER_NAME = "ResolutionLogs";
        private const string ODATA_DOCUMENT_SERIES_ITEM_CONTROLLER_NAME = "DocumentSeriesItems";
        private const string ODATA_DOCUMENT_SERIES_ITEM_LOG_CONTROLLER_NAME = "DocumentSeriesItemLogs";
        private const string ODATA_UDS_REPOSITORY_CONTROLLER_NAME = "UDSRepositories";
        private const string ODATA_PECMAIL_CONTROLLER_NAME = "PECMails";
        private const string ODATA_PECMAILATTACHMENT_CONTROLLER_NAME = "PECMailAttachments";
        private const string ODATA_PECMAILRECEIPT_CONTROLLER_NAME = "PECMailReceipts";
        private const string ODATA_PECMAILBOXES_CONTROLLER_NAME = "PECMailBoxes";
        private const string ODATA_UDS_SCHEMA_REPOSITORY_CONTROLLER_NAME = "UDSSchemaRepositories";
        private const string ODATA_CATEGORY_CONTROLLER_NAME = "Categories";
        private const string ODATA_CATEGORYFASCICLE_CONTROLLER_NAME = "CategoryFascicles";
        private const string ODATA_LOCATION_CONTROLLER_NAME = "Locations";
        private const string ODATA_CONTACT_CONTROLLER_NAME = "Contacts";
        private const string ODATA_CONTAINER_CONTROLLER_NAME = "Containers";
        private const string ODATA_ROLE_CONTROLLER_NAME = "Roles";
        private const string ODATA_WORKFLOWROLEMAPPING_CONTROLLER_NAME = "WorkflowRoleMappings";
        private const string ODATA_METADATAREPOSITORY_CONTROLLER_NAME = "MetadataRepositories";
        private const string ODATA_COLLABORATIONAGGREGATE_CONTROLLER_NAME = "CollaborationAggregates";
        private const string ODATA_UDS_DOCUMENTUNIT_CONTROLLER_NAME = "UDSDocumentUnits";
        private const string ODATA_UDS_CONTACT_CONTROLLER_NAME = "UDSContacts";
        private const string ODATA_UDS_PECMAIL_CONTROLLER_NAME = "UDSPECMails";
        private const string ODATA_UDS_MESSAGE_CONTROLLER_NAME = "UDSMessages";
        private const string ODATA_UDS_ROLE_CONTROLLER_NAME = "UDSRoles";
        private const string ODATA_TENANT_CONTROLLER_NAME = "Tenants";
        private const string ODATA_PARAMETER_CONTROLLER_NAME = "Parameters";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WebAPIClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public WebAPIClient(ILogger logger)
        {
            _logger = logger;
            _originalHttpClientConfiguration = new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration;
            _httpClient = new WebApiHttpClient(new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration,
                _originalHttpClientConfiguration, f => _logger.WriteDebug(new LogMessage(f), LogCategories));
        }
        #endregion

        #region [ Methods ]

        public void SetUDSEndpoints(string endpointName, string controllerName)
        {
            IWebApiControllerEndpoint webApiControllerEndpoint = _httpClient.Configuration.EndPoints.SingleOrDefault(f => f.EndpointName.Equals(endpointName, StringComparison.InvariantCultureIgnoreCase));
            if (webApiControllerEndpoint == null)
            {
                webApiControllerEndpoint = new WebApiControllerEndpoint()
                {
                    AddressName = WebApiHttpClient.UDS_ADDRESS_NAME,
                    ControllerName = controllerName,
                    EndpointName = endpointName
                };
                _httpClient.Configuration.EndPoints.Add(webApiControllerEndpoint);
            }
            webApiControllerEndpoint.ControllerName = controllerName;
            webApiControllerEndpoint.EndpointName = endpointName;
        }

        public async Task PushCorrelatedNotificationAsync(string message, string moduleName, Guid tenantId, string tenantName, Guid? correlationId, IIdentityContext identity, NotificationType notificationType)
        {
            if (correlationId.HasValue)
            {
                WorkflowRequestStatus workflowRequestStatus = new WorkflowRequestStatus()
                {
                    CorrelationId = correlationId,
                    Description = message,
                    LogDate = DateTimeOffset.UtcNow,
                    ModuleName = moduleName
                };
                WorkflowNotification workflowNotification = new WorkflowNotification()
                {
                    CorrelationId = correlationId,
                    Description = message,
                    LogDate = DateTimeOffset.UtcNow,
                    ModuleName = moduleName
                };
                dynamic @event = null;
                switch (notificationType)
                {
                    case NotificationType.EventWorkflowStatusDone:
                        {
                            @event = new EventWorkflowStartRequestDone(Guid.NewGuid(), correlationId, tenantName, tenantId, identity, workflowRequestStatus, null);
                            break;
                        }
                    case NotificationType.EventWorkflowStatusError:
                        {
                            @event = new EventWorkflowStartRequestError(Guid.NewGuid(), correlationId, tenantName, tenantId, identity, workflowRequestStatus, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationInfo:
                        {
                            @event = new EventWorkflowNotificationInfo(Guid.NewGuid(), correlationId, tenantName, tenantId, identity, workflowNotification, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationInfoAsModel:
                        {
                            @event = new EventWorkflowNotificationInfoAsModel(Guid.NewGuid(), correlationId, tenantName, tenantId, identity, workflowNotification, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationWarning:
                        {
                            @event = new EventWorkflowNotificationWarning(Guid.NewGuid(), correlationId, tenantName, tenantId, identity, workflowNotification, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationError:
                        {
                            @event = new EventWorkflowNotificationError(Guid.NewGuid(), correlationId, tenantName, tenantId, identity, workflowNotification, null);
                            break;
                        }
                }
                @event.CustomProperties.Add(CustomPropertyName.EVALUATED_MODULE_NAME, moduleName);
                ServiceBusMessage serviceBusMessage = await SendEventAsync(@event);
                _logger.WriteDebug(new LogMessage($"Correlated notification {correlationId}/{notificationType} has been sended with {serviceBusMessage.MessageId}"), LogCategories);
            }
        }

        public async Task<DocumentUnit> GetDocumentUnitAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<DocumentUnit>(ODATA_DOCUMENTUNIT_CONTROLLER_NAME);
                ODataModel<DocumentUnit> result = (await _httpClient.GetAsync<DocumentUnit>()
                    .WithOData(string.Concat("$filter=UniqueId eq ", uniqueId, "&$expand=UDSRepository"))
                     .ResponseToModelAsync<ODataModel<DocumentUnit>>());
                return result.Value.SingleOrDefault();
            }, $"WebAPIClient.GetDocumentUnitAsync -> GET entities error");
        }

        public async Task<DocumentUnit> GetDocumentUnitAsync(short year, int number, bool expandChains = false)
        {
            return await ExecuteHelper(async () =>
            {
                string query = $"$filter = Year eq {year} and Number eq {number}";
                if (expandChains)
                {
                    query = $"{query}&$expand=DocumentUnitChains";
                }
                SetEntityODATA<DocumentUnit>(ODATA_DOCUMENTUNIT_CONTROLLER_NAME);
                ODataModel<DocumentUnit> result = (await _httpClient.GetAsync<DocumentUnit>()
                    .WithOData(query)
                     .ResponseToModelAsync<ODataModel<DocumentUnit>>());
                return result.Value.SingleOrDefault();
            }, $"WebAPIClient.GetDocumentUnitAsync -> GET entities error");
        }

        public async Task<TemplateDocumentRepository> GetTemplateDocumentRepositoryAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<TemplateDocumentRepository>(ODATA_TEMPLATE_DOCUMENT_REPOSITORY_CONTROLLER_NAME);
                ODataModel<TemplateDocumentRepository> result = (await _httpClient.GetAsync<TemplateDocumentRepository>()
                    .WithOData($"$filter=UniqueId eq {uniqueId}")
                     .ResponseToModelAsync<ODataModel<TemplateDocumentRepository>>());
                return result.Value.SingleOrDefault();
            }, $"WebAPIClient.GetTemplateDocumentRepositoryAsync -> GET entities error");
        }

        public async Task<ICollection<DocumentUnitChain>> GetDocumentUnitChainsAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<DocumentUnitChain>(ODATA_DOCUMENTUNITCHAIN_CONTROLLER_NAME);
                ODataModel<DocumentUnitChain> result = await _httpClient.GetAsync<DocumentUnitChain>()
                    .WithOData($"$filter=DocumentUnit/UniqueId eq {uniqueId}")
                     .ResponseToModelAsync<ODataModel<DocumentUnitChain>>();
                return result.Value;
            }, $"WebAPIClient.GetDocumentUnitChainsAsync -> GET entities error");
        }

        public async Task<Role> GetRoleAsync(short entityShortId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Role>(ODATA_ROLE_CONTROLLER_NAME);
                ODataModel<Role> result = await _httpClient.GetAsync<Role>()
                    .WithOData($"$filter=EntityShortId eq {entityShortId}")
                     .ResponseToModelAsync<ODataModel<Role>>();

                return result == null ? null : result.Value.SingleOrDefault();
            }, "WebAPIClient.GetRolesAsync -> GET entities error");
        }

        public async Task<ICollection<WorkflowRoleMapping>> GetWorkflowRoleMappingAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<WorkflowRoleMapping>(ODATA_WORKFLOWROLEMAPPING_CONTROLLER_NAME);
                ODataModel<WorkflowRoleMapping> result = await _httpClient.GetAsync<WorkflowRoleMapping>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<WorkflowRoleMapping>>();

                return result.Value;
            }, "WebAPIClient.GetWorkflowRoleMappingAsync -> GET entities error");
        }

        public async Task<ICollection<Tenant>> GetTenantsAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Tenant>(ODATA_TENANT_CONTROLLER_NAME);
                ODataModel<Tenant> result = await _httpClient.GetAsync<Tenant>()
                    .WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<Tenant>>();

                return result.Value;
            }, "WebAPIClient.GetTenants -> GET entities error");
        }

        public async Task<Fascicle> GetFascicleAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Fascicle>(ODATA_FASCICLE_CONTROLLER_NAME);
                ODataModel<Fascicle> result = await _httpClient.GetAsync<Fascicle>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<Fascicle>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, "WebAPIClient.GetFascicleAsync -> GET entities error");
        }

        public async Task<FascicleFolder> GetDefaultFascicleFolderAsync(Guid idFascicle)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<FascicleFolder>();
                ODataModel<FascicleFolder> fascicleFolder = await _httpClient.GetAsync<FascicleFolder>()
                .WithOData($"$filter=Fascicle/UniqueId eq {idFascicle} and Name eq 'Fascicolo' and FascicleFolderLevel eq 2")
                       .ResponseToModelAsync<ODataModel<FascicleFolder>>();
                return fascicleFolder == null || fascicleFolder.Value == null || !fascicleFolder.Value.Any() ? null : fascicleFolder.Value.Single();
            }, $"WebAPIClient.GetDefaultFascicleFolderAsync -> GET entities error");
        }

        public async Task<ICollection<Fascicle>> GetFasciclesAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Fascicle>(ODATA_FASCICLE_CONTROLLER_NAME);
                ODataModel<Fascicle> result = await _httpClient.GetAsync<Fascicle>().
                    WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<Fascicle>>();

                return result == null ? new List<Fascicle>() : result.Value;
            }, "WebAPIClient.GetFasciclesAsync -> GET entities error");
        }

        public async Task<ICollection<FascicleDocumentUnit>> GetFascicleDocumentUnitAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<FascicleDocumentUnit>(ODATA_FASCICLE_DOCUMENTUNIT_CONTROLLER_NAME);
                ODataModel<FascicleDocumentUnit> result = await _httpClient.GetAsync<FascicleDocumentUnit>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<FascicleDocumentUnit>>();

                return result == null ? new List<FascicleDocumentUnit>() : result.Value;
            }, "WebAPIClient.GetFascicleDocumentUnitAsync -> GET entities error");
        }

        public async Task<Category> GetCategoryAsync(int idCategory)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Category>(ODATA_CATEGORY_CONTROLLER_NAME);
                ODataModel<Category> result = await _httpClient.GetAsync<Category>()
                    .WithOData($"filter=EntityShortId eq {idCategory}")
                     .ResponseToModelAsync<ODataModel<Category>>();

                return result.Value.FirstOrDefault();

            }, "WebAPIClient.GetCategoryAsync(int) -> GET entities error");
        }

        public async Task<ICollection<Category>> GetCategoryAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Category>(ODATA_CATEGORY_CONTROLLER_NAME);
                ODataModel<Category> result = await _httpClient.GetAsync<Category>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<Category>>();

                return result == null ? new List<Category>() : result.Value;

            }, "WebAPIClient.GetCategoryAsync -> GET entities error");
        }

        public async Task<ICollection<Contact>> GetContactAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Contact>(ODATA_CONTACT_CONTROLLER_NAME);
                ODataModel<Contact> result = await _httpClient.GetAsync<Contact>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<Contact>>();

                return result == null ? new List<Contact>() : result.Value;
            }, "WebAPIClient.GetContactAsync -> GET entities error");
        }

        public async Task<ICollection<Container>> GetContainerAsync(short containerId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Container>(ODATA_CONTAINER_CONTROLLER_NAME);
                ODataModel<Container> result = await _httpClient.GetAsync<Container>()
                    .WithRowQuery($"?$filter=EntityShortId eq {containerId}&$expand=ProtLocation")
                     .ResponseToModelAsync<ODataModel<Container>>();

                return result == null ? new List<Container>() : result.Value;
            }, "WebAPIClient.GetContainerAsync -> GET entities error");
        }

        public async Task<ICollection<PECMailBox>> GetPECMailBoxAsync(short pecMailBoxId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<PECMailBox>(ODATA_PECMAILBOXES_CONTROLLER_NAME);
                ODataModel<PECMailBox> result = await _httpClient.GetAsync<PECMailBox>()
                    .WithRowQuery($"?$filter=EntityShortId eq {pecMailBoxId}&$expand=Location")
                     .ResponseToModelAsync<ODataModel<PECMailBox>>();

                return result == null ? new List<PECMailBox>() : result.Value;
            }, "WebAPIClient.GetPECMailBoxAsync -> GET entities error");
        }

        public async Task<ICollection<Protocol>> GetProtocolAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<Protocol>(ODATA_PROTOCOL_CONTROLLER_NAME);
                ODataModel<Protocol> result = await _httpClient.GetAsync<Protocol>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<Protocol>>();

                return result == null ? new List<Protocol>() : result.Value;
            }, "WebAPIClient.GetProtocolAsync -> GET entities error");
        }

        public async Task<ICollection<ProtocolContact>> GetProtocolContactsAsync(Guid uniqueId, string comunicationType)
        {
            return await ExecuteHelper(async () =>
            {
                string odataQuery = $"$filter=Protocol/UniqueId eq {uniqueId} and ComunicationType eq '{comunicationType}'&$expand=Contact";
                _httpClient.SetEntityODATA<ProtocolContact>();
                ODataModel<ProtocolContact> result = await _httpClient.GetAsync<ProtocolContact>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<ProtocolContact>>();

                return result == null ? new List<ProtocolContact>() : result.Value;
            }, "WebAPIClient.GetProtocolContactsAsync -> GET entities error");
        }

        public async Task<ICollection<Contact>> GetProtocolContactSendersAsync(Protocol protocol)
        {
            return (await GetProtocolContactsAsync(protocol.UniqueId, "M")).Select(s => s.Contact).ToList();
        }

        public async Task<ICollection<ProtocolContactManual>> GetProtocolContactManualSendersAsync(Protocol protocol)
        {
            return (await GetProtocolContactManualsAsync(protocol.UniqueId, "M")).ToList();
        }

        public async Task<ICollection<ProtocolContactManual>> GetProtocolContactManualsAsync(Guid uniqueId, string comunicationType)
        {
            return await ExecuteHelper(async () =>
            {
                string odataQuery = $"$filter=Protocol/UniqueId eq {uniqueId} and ComunicationType eq '{comunicationType}'";
                _httpClient.SetEntityODATA<ProtocolContactManual>();
                ODataModel<ProtocolContactManual> result = await _httpClient.GetAsync<ProtocolContactManual>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<ProtocolContactManual>>();

                return result == null ? new List<ProtocolContactManual>() : result.Value;
            }, "WebAPIClient.GetProtocolContactsAsync -> GET entities error");
        }


        public async Task<AdvancedProtocol> GetAdvancedProtocolAsync(short year, int number)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<AdvancedProtocol>(ODATA_PROTOCOLADVANCED_CONTROLLER_NAME);
                ODataModel<AdvancedProtocol> result = await _httpClient.GetAsync<AdvancedProtocol>()
                    .WithRowQuery($"?$filter=Year eq {year} and Number eq {number}")
                    .ResponseToModelAsync<ODataModel<AdvancedProtocol>>();

                return result?.Value.Single();
            }, "WebAPIClient.GetAdvancedProtocolAsync -> GET entities error");
        }

        public async Task<ICollection<ProtocolLog>> GetProtocolLogAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<ProtocolLog>(ODATA_PROTOCOL_LOG_CONTROLLER_NAME);
                ODataModel<ProtocolLog> result = await _httpClient.GetAsync<ProtocolLog>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<ProtocolLog>>();

                return result == null ? new List<ProtocolLog>() : result.Value;
            }, "WebAPIClient.GetProtocolLogAsync -> GET entities error");
        }

        public async Task<ICollection<ResolutionLog>> GetResolutionLogAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<ResolutionLog>(ODATA_RESOLUTION_LOG_CONTROLLER_NAME);
                ODataModel<ResolutionLog> result = await _httpClient.GetAsync<ResolutionLog>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<ResolutionLog>>();

                return result == null ? new List<ResolutionLog>() : result.Value;
            }, "WebAPIClient.GetResolutionLogAsync -> GET entities error");
        }

        public async Task<ICollection<DocumentSeriesItemLog>> GetDocumentSeriesItemLogAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<DocumentSeriesItemLog>(ODATA_DOCUMENT_SERIES_ITEM_LOG_CONTROLLER_NAME);
                ODataModel<DocumentSeriesItemLog> result = await _httpClient.GetAsync<DocumentSeriesItemLog>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<DocumentSeriesItemLog>>();

                return result == null ? new List<DocumentSeriesItemLog>() : result.Value;
            }, "WebAPIClient.GetProtocolLogAsync -> GET entities error");
        }

        public async Task<ICollection<MetadataRepository>> GetMetadataRepositoryAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<MetadataRepository>(ODATA_METADATAREPOSITORY_CONTROLLER_NAME);
                ODataModel<MetadataRepository> result = await _httpClient.GetAsync<MetadataRepository>()
                    .WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<MetadataRepository>>();

                return result == null ? new List<MetadataRepository>() : result.Value;
            }, "WebAPIClient.GetMetadataRepositoryAsync -> GET entities error");
        }

        public async Task<ICollection<UDSRepository>> GetUDSRepository(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<UDSRepository>(ODATA_UDS_REPOSITORY_CONTROLLER_NAME);
                ODataModel<UDSRepository> result = await _httpClient.GetAsync<UDSRepository>()
                    .WithRowQuery($"?$filter=UniqueId eq {uniqueId}")
                    .ResponseToModelAsync<ODataModel<UDSRepository>>();

                return result == null ? new List<UDSRepository>() : result.Value;
            }, "WebAPIClient.GetUDSRepository -> GET entities error");
        }

        public async Task<ICollection<UDSRepository>> GetUDSRepository(string name)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<UDSRepository>(ODATA_UDS_REPOSITORY_CONTROLLER_NAME);
                ODataModel<UDSRepository> result = await _httpClient.GetAsync<UDSRepository>()
                    .WithRowQuery($"?$filter=Name eq '{name}' and ExpiredDate eq null and Status eq 'Confirmed'")
                    .ResponseToModelAsync<ODataModel<UDSRepository>>();

                return result == null ? new List<UDSRepository>() : result.Value;
            }, "WebAPIClient.GetUDSRepository -> GET entities error");
        }

        public async Task<ICollection<PECMail>> GetPECMailFromProtocol(short year, int number)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<PECMail>(ODATA_PECMAIL_CONTROLLER_NAME);
                ODataModel<PECMail> result = await _httpClient.GetAsync<PECMail>()
                    .WithRowQuery($"?$filter=Year eq {year} and Number eq {number} and DocumentUnitType eq 'Protocol'&$expand=PECMailBox,Attachments")
                    .ResponseToModelAsync<ODataModel<PECMail>>();

                return result == null ? new List<PECMail>() : result.Value;
            }, "WebAPIClient.GetPECMailFromProtocol -> GET entities error");
        }

        public async Task<ICollection<PECMailAttachment>> GetPECMailAttachmentFromPECMailId(int entityId, string attachmentName)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<PECMailAttachment>(ODATA_PECMAILATTACHMENT_CONTROLLER_NAME);
                string odata = $"$filter=PECMail/EntityId eq {entityId}";
                if (!string.IsNullOrEmpty(attachmentName))
                {
                    odata = $"{odata} and contains(AttachmentName,'{attachmentName}')";
                }
                ODataModel<PECMailAttachment> result = await _httpClient.GetAsync<PECMailReceipt>()
                    .WithRowQuery(odata)
                    .ResponseToModelAsync<ODataModel<PECMailAttachment>>();

                return result == null ? new List<PECMailAttachment>() : result.Value;
            }, "WebAPIClient.GetPECMailAttachmentFromPECMailId -> GET entities error");
        }

        public async Task<ICollection<PECMail>> GetPECMailFromReceiptIdentification(string referenceToPECMessageId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<PECMailReceipt>(ODATA_PECMAILRECEIPT_CONTROLLER_NAME);
                ODataModel<PECMailReceipt> result = await _httpClient.GetAsync<PECMailReceipt>()
                    .WithRowQuery($"?$filter=Identification eq '{referenceToPECMessageId}' and ReceiptType eq 'avvenuta-consegna'&$expand=PECMail")
                    .ResponseToModelAsync<ODataModel<PECMailReceipt>>();

                if (result != null && result.Value != null && result.Value.Any(f => f.PECMail != null))
                {
                    return result.Value.Where(f => f.PECMail != null).Select(f => f.PECMail).ToList();
                }
                return new List<PECMail>();
            }, "WebAPIClient.GetPECMailFromReceiptIdentification -> GET entities error");
        }

        public async Task<ICollection<UDSContact>> GetUDSContacts(Guid idUDS)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<UDSContact>(ODATA_UDS_CONTACT_CONTROLLER_NAME);
                ODataModel<UDSContact> result = await _httpClient.GetAsync<UDSContact>()
                    .WithRowQuery($"?$filter=IdUDS eq {idUDS}&$expand=Relation")
                    .ResponseToModelAsync<ODataModel<UDSContact>>();

                return result == null ? new List<UDSContact>() : result.Value;
            }, "WebAPIClient.UDSCGetUDSContactsontact -> GET entities error");
        }

        public async Task<ICollection<UDSPECMail>> GetUDSPECMails(Guid idUDS)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<UDSPECMail>(ODATA_UDS_PECMAIL_CONTROLLER_NAME);
                ODataModel<UDSPECMail> result = await _httpClient.GetAsync<UDSPECMail>()
                    .WithRowQuery($"?$filter=IdUDS eq {idUDS}&$expand=Relation")
                    .ResponseToModelAsync<ODataModel<UDSPECMail>>();

                return result == null ? new List<UDSPECMail>() : result.Value;
            }, "WebAPIClient.GetUDSPECMails -> GET entities error");
        }

        public async Task<ICollection<UDSMessage>> GetUDSMessages(Guid idUDS)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<UDSMessage>(ODATA_UDS_MESSAGE_CONTROLLER_NAME);
                ODataModel<UDSMessage> result = await _httpClient.GetAsync<UDSMessage>()
                    .WithRowQuery($"?$filter=IdUDS eq {idUDS}&$expand=Relation")
                    .ResponseToModelAsync<ODataModel<UDSMessage>>();

                return result == null ? new List<UDSMessage>() : result.Value;
            }, "WebAPIClient.GetUDSMessages -> GET entities error");
        }

        public async Task<ICollection<UDSRole>> GetUDSRoles(Guid idUDS)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<UDSRole>(ODATA_UDS_ROLE_CONTROLLER_NAME);
                ODataModel<UDSRole> result = await _httpClient.GetAsync<UDSRole>()
                    .WithRowQuery($"?$filter=IdUDS eq {idUDS}&$expand=Relation")
                    .ResponseToModelAsync<ODataModel<UDSRole>>();

                return result == null ? new List<UDSRole>() : result.Value;
            }, "WebAPIClient.GetUDSRoles -> GET entities error");
        }

        public async Task<ICollection<UDSDocumentUnit>> GetUDSDocumentUnits(Guid idUDS, bool expandFascicle, bool expandUDSRepository)
        {
            return await ExecuteHelper(async () =>
            {
                string expand = expandFascicle ? "($expand=Fascicle)" : string.Empty;
                expand = expandUDSRepository ? $"{expand},Repository" : expand;
                SetEntityODATA<UDSDocumentUnit>(ODATA_UDS_DOCUMENTUNIT_CONTROLLER_NAME);
                ODataModel<UDSDocumentUnit> result = await _httpClient.GetAsync<UDSDocumentUnit>()
                    .WithRowQuery($"?$filter=IdUDS eq {idUDS}&$expand=Relation{expand}")
                    .ResponseToModelAsync<ODataModel<UDSDocumentUnit>>();

                return result == null ? new List<UDSDocumentUnit>() : result.Value;
            }, "WebAPIClient.GetUDSDocumentUnits -> GET entities error");
        }

        public async Task<ICollection<UDSDocumentUnit>> GetUDSDocumentUnitFromDocumentUnitId(Guid idDocumentUnit)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<UDSDocumentUnit>(ODATA_UDS_DOCUMENTUNIT_CONTROLLER_NAME);
                ODataModel<UDSDocumentUnit> result = await _httpClient.GetAsync<UDSDocumentUnit>()
                    .WithRowQuery($"?$filter=Relation/UniqueId eq {idDocumentUnit}&$expand=Repository")
                    .ResponseToModelAsync<ODataModel<UDSDocumentUnit>>();

                return result == null ? new List<UDSDocumentUnit>() : result.Value;
            }, "WebAPIClient.GetUDSDocumentUnitFromDocumentUnitId -> GET entities error");
        }

        public async Task<Dictionary<string, object>> GetUDSByInvoiceFilename(string controllerName, string invoiceFilename, string invoiceNumber, bool onlyValidInvoiceState, Dictionary<int, Guid> documents)
        {
            return await ExecuteHelper(async () =>
            {
                Dictionary<string, object> results = new Dictionary<string, object>();
                string optionalFilter = string.Empty;
                if (onlyValidInvoiceState)
                {
                    optionalFilter = $"StatoFattura in ('Accettata','Mancato recapito','Recapito in corso','Consegnata','PEC inviata con consegna allo SDI','PEC inviata con accettazione')";
                }
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    if (!string.IsNullOrEmpty(optionalFilter))
                    {
                        optionalFilter = $"{optionalFilter} and ";
                    }
                    invoiceNumber = invoiceNumber.Replace("#", "%23");
                    invoiceNumber = invoiceNumber.Replace("%", "%25");
                    invoiceNumber = invoiceNumber.Replace("/", "%2F");
                    optionalFilter = $"{optionalFilter}NumeroFattura eq '{invoiceNumber}'";
                }
                results = await GetUDSByDocumentName(controllerName, invoiceFilename, optionalFilter, documents, results);
                if (results == null && !invoiceFilename.EndsWith(".p7m", StringComparison.InvariantCultureIgnoreCase))
                {
                    results = new Dictionary<string, object>();
                    _logger.WriteDebug(new LogMessage($"Finding UDS invoice using {invoiceFilename}.p7m filename"), LogCategories);
                    results = await GetUDSByDocumentName(controllerName, $"{invoiceFilename}.p7m", optionalFilter, documents, results);
                }
                return results;
            }, "WebAPIClient.GetUDSByInvoiceFilename -> GET entities error");
        }

        public async Task<Dictionary<string, object>> GetUDSByInvoiceFilename(string controllerName, string invoiceFilename, bool onlyValidInvoiceState, Dictionary<int, Guid> documents)
        {
            return await GetUDSByInvoiceFilename(controllerName, invoiceFilename, string.Empty, onlyValidInvoiceState, documents);
        }

        public async Task<Dictionary<string, object>> GetUDSByDocumentFilename(string controllerName, string documentName, Dictionary<int, Guid> documents)
        {
            return await ExecuteHelper(async () =>
            {
                Dictionary<string, object> results = new Dictionary<string, object>();
                return await GetUDSByDocumentName(controllerName, documentName, string.Empty, documents, results);
            }, "WebAPIClient.GetUDSByDocumentFilename -> GET entities error");
        }

        private async Task<Dictionary<string, object>> GetUDSByDocumentName(string controllerName, string documentName, string optionalFilter,
            Dictionary<int, Guid> documents, Dictionary<string, object> results)
        {
            IBaseAddress webApiAddress = _httpClient.Configuration.Addresses.FirstOrDefault(x => x.AddressName.Equals(WebApiHttpClient.UDS_ADDRESS_NAME, StringComparison.InvariantCultureIgnoreCase));
            IWebApiControllerEndpoint udsEndpoint = _httpClient.Configuration.EndPoints.SingleOrDefault(f => f.EndpointName == "UDSBuildModel");
            if (udsEndpoint == null)
            {
                udsEndpoint = new WebApiControllerEndpoint
                {
                    AddressName = WebApiHttpClient.UDS_ADDRESS_NAME,
                    ControllerName = controllerName,
                    EndpointName = "UDSBuildModel"
                };
                _httpClient.Configuration.EndPoints.Add(udsEndpoint);
            }
            udsEndpoint.ControllerName = controllerName;
            StringBuilder odata = new StringBuilder();
            odata.Append($"?$filter=_status eq 1 and Documents/any(doc:doc/DocumentName eq '{documentName}' and doc/DocumentType eq 1)");
            if (!string.IsNullOrEmpty(optionalFilter))
            {
                odata.Append($" and {optionalFilter}");
            }
            odata.Append("&applySecurity='0'");
            string jsonSource = await _httpClient.GetAsync<UDSBuildModel>().WithRowQuery(odata.ToString()).ResponseToStringAsync();
            JToken values = JObject.Parse(jsonSource)["Items"]["$values"].SingleOrDefault();
            if (values == null)
            {
                return null;
            }
            foreach (JProperty item in values.OfType<JProperty>().Where(f => !f.Name.StartsWith("$")))
            {
                if (!results.ContainsKey(item.Name))
                {
                    results.Add(item.Name, item.Value.ToObject<object>());
                }
            }

            int documentType;
            foreach (JToken item in values["Documents"]["$values"])
            {
                documentType = item.OfType<JProperty>().Single(f => f.Name == "DocumentType").Value.ToObject<int>();
                if (!documents.ContainsKey(documentType))
                {
                    documents.Add(documentType, item.OfType<JProperty>().Single(f => f.Name == "IdDocument").Value.ToObject<Guid>());
                }
            }
            return results;
        }

        public async Task<Dictionary<string, object>> GetUDS(string controllerName, Guid idUds, Dictionary<int, Guid> documents)
        {
            return await ExecuteHelper(async () =>
            {
                Dictionary<string, object> results = new Dictionary<string, object>();
                IBaseAddress webApiAddress = _httpClient.Configuration.Addresses.FirstOrDefault(x => x.AddressName.Equals(WebApiHttpClient.UDS_ADDRESS_NAME, StringComparison.InvariantCultureIgnoreCase));
                IWebApiControllerEndpoint udsEndpoint = _httpClient.Configuration.EndPoints.SingleOrDefault(f => f.EndpointName == "UDSBuildModel");
                if (udsEndpoint == null)
                {
                    udsEndpoint = new WebApiControllerEndpoint
                    {
                        AddressName = WebApiHttpClient.UDS_ADDRESS_NAME,
                        ControllerName = controllerName,
                        EndpointName = "UDSBuildModel"
                    };
                    _httpClient.Configuration.EndPoints.Add(udsEndpoint);
                }
                udsEndpoint.ControllerName = controllerName;
                string jsonSource = await _httpClient.GetAsync<UDSBuildModel>().WithRowQuery($"?$filter=UDSId eq {idUds}&applySecurity='0'").ResponseToStringAsync();
                JToken values = JObject.Parse(jsonSource)["Items"]["$values"].SingleOrDefault();
                if (values == null)
                {
                    return null;
                }
                foreach (JProperty item in values.OfType<JProperty>().Where(f => !f.Name.StartsWith("$")))
                {
                    if (!results.ContainsKey(item.Name))
                    {
                        results.Add(item.Name, item.Value.ToObject<object>());
                    }
                }
                int documentType;
                foreach (JToken item in values["Documents"]["$values"])
                {
                    documentType = item.OfType<JProperty>().Single(f => f.Name == "DocumentType").Value.ToObject<int>();
                    if (!documents.ContainsKey(documentType))
                    {
                        documents.Add(documentType, item.OfType<JProperty>().Single(f => f.Name == "IdDocument").Value.ToObject<Guid>());
                    }
                }
                return results;
            }, "WebAPIClient.GetUDS -> GET entities error");
        }

        public async Task<ICollection<Collaboration>> GetCollaborationAggregatesAsync(int collaborationId)
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityODATA<CollaborationAggregate>(ODATA_COLLABORATIONAGGREGATE_CONTROLLER_NAME);
                ODataModel<CollaborationAggregate> result = await _httpClient.GetAsync<CollaborationAggregate>()
                    .WithOData($"$filter=CollaborationFather/EntityId eq {collaborationId}&$expand=CollaborationChild")
                    .ResponseToModelAsync<ODataModel<CollaborationAggregate>>();

                if (result != null && result.Value != null && result.Value.Any(f => f.CollaborationChild != null))
                {
                    return result.Value.Where(f => f.CollaborationChild != null).Select(f => f.CollaborationChild).ToList();
                }
                return new List<Collaboration>();
            }, "WebAPIClient.GetCollaborationAggregatesAsync -> GET entities error");
        }

        public async Task<Collaboration> GetCollaborationAsync(int collaborationId, bool expandWorkflowActivities = false)
        {
            return await ExecuteHelper(async () =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"$filter=EntityId eq {collaborationId}");
                if (expandWorkflowActivities)
                {
                    stringBuilder.Append("&$Expand=WorkflowInstance($Expand=WorkflowActivities($Expand=DocumentUnitReferenced($Expand=UDSRepository($select=Name,UniqueId,DSWEnvironment))))");
                }
                
                _httpClient.SetEntityODATA<Collaboration>();
                ODataModel<Collaboration> result = await _httpClient.GetAsync<Collaboration>()
                    .WithOData(stringBuilder.ToString())
                    .ResponseToModelAsync<ODataModel<Collaboration>>();

                return result == null && result.Value != null ? null : result.Value.SingleOrDefault();
            }, "WebAPIClient.GetCollaborationAsync -> GET entities error");
        }
        

        public async Task<ICollection<WorkflowActivity>> GetWorkflowAuthorizedActivitiesByDocumentUnitAsync(Guid documentUnitId, string account, string workflowName)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<WorkflowActivity>();
                ODataModel<WorkflowActivity> result = await _httpClient.GetAsync<WorkflowActivity>()
                    .WithOData($@"$expand=DocumentUnitReferenced,WorkflowAuthorizations&$filter=DocumentUnitReferenced/UniqueId eq {documentUnitId} and WorkflowAuthorizations/any(wc:wc/Account in ('{account.Replace(@"\", @"\\")}')) and (Status eq 'Todo' or Status eq 'Progress') and WorkflowInstance/WorkflowRepository/Name eq '{workflowName}'")
                    .ResponseToModelAsync<ODataModel<WorkflowActivity>>();

                return result == null && result.Value != null ? new List<WorkflowActivity>() : result.Value;
            }, "WebAPIClient.GetWorkflowAuthorizedActivitiesByDocumentUnitAsync -> GET entities error");
        }

        public async Task<WorkflowResult> StartWorkflow(WorkflowStart workflowStart)
        {
            return await ExecuteHelper(async () =>
            {
                return await _httpClient.PostAsync(workflowStart).ResponseToModelAsync<WorkflowResult>();
            }, "WebAPIClient.StartWorkflow -> POST model error", retryPolicyEnabled: false);
        }

        public async Task<WorkflowResult> WorkflowNotify(WorkflowNotify workflowNotify)
        {
            return await ExecuteHelper(async () =>
            {
                return await _httpClient.PostAsync(workflowNotify).ResponseToModelAsync<WorkflowResult>();
            }, "WebAPIClient.WorkflowNotify -> POST model error", retryPolicyEnabled: false);
        }

        public async Task<DomainUserModel> GetUserAsync(string user)
        {
            return await ExecuteHelper(async () =>
            {
                if (!user.Contains("\\"))
                {
                    throw new ArgumentException("user parameter cannot contains formal formato <domain>\\<samaccount>");
                }
                string[] tokens = user.Split('\\');
                _httpClient.SetEntityODATA<DomainUserModel>();
                DomainUserModel currentUser = await _httpClient.GetAsync<DomainUserModel>()
                    .WithRowQuery($"/DomainUserService.GetUser(username='{tokens.Last()}',domain='{tokens.First()}')")
                    .ResponseToModelAsync<DomainUserModel>();

                return currentUser;
            }, "WebAPIClient.FindUserAsync -> GET entities error");
        }

        public async Task<ICollection<T>> GetAsync<T>() where T : class
        {
            return await ExecuteHelper(async () =>
            {
                return await _httpClient.GetAsync<T>().ResponseToModelAsync<ICollection<T>>();
            }, "WebAPIClient.GetAsync -> GET entities error");
        }

        public async Task<ICollection<T>> GetAsync<T>(string odataQuery) where T : class
        {
            return await ExecuteHelper(async () =>
            {
                return await _httpClient.GetAsync<T>()
                    .WithOData(odataQuery)
                    .ResponseToModelAsync<ICollection<T>>();
            }, "WebAPIClient.GetAsync -> GET entities error");
        }

        public async Task<TResult> GetAsync<T, TResult>(string controllerName, string odataQuery)
            where T : class
            where TResult : class
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityRest<T>(controllerName);
                return await _httpClient.GetAsync<T>().WithOData(odataQuery).ResponseToModelAsync<TResult>();
            }, "WebAPIClient.GetAsync -> GET entities error");
        }

        public async Task SendCommandAsync<T>(T model)
            where T : Command
        {
            try
            {
                SetEntityRest<T>();
                await _httpClient.PostAsync(model);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WebAPIClient.SendCommandAsync -> POST command error"), ex, LogCategories);
                throw ex;
            }
        }
        public async Task<ServiceBusMessage> SendEventAsync<T>(T model) 
            where T : Event
        {
            try
            {
                SetEntityRest<T>();
                return await _httpClient.PostAsync(model).ResponseToModelAsync<ServiceBusMessage>();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WebAPIClient.SendEventAsync -> POST event error"), ex, LogCategories);
                throw ex;
            }
        }

        public async Task<T> PostAsync<T>(T model, InsertActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityRest<T>();
                return await _httpClient.PostAsync(model, actionType: actionType.HasValue ? actionType.Value.ToString() : string.Empty).ResponseToModelAsync<T>();
            }, "WebAPIClient.PostAsync -> POST entity error", retryPolicyEnabled: retryPolicyEnabled);
        }

        public async Task<TResult> PostAsync<T, TResult>(T model, bool retryPolicyEnabled = false)
            where T : class
            where TResult : class
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityRest<T>();
                return await _httpClient.PostAsync(model).ResponseToModelAsync<TResult>();
            }, "WebAPIClient.PostAsync -> POST entity error", retryPolicyEnabled: retryPolicyEnabled);
        }

        public async Task<T> PutAsync<T>(T model, UpdateActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityRest<T>();
                return await _httpClient.PutAsync(model, actionType: actionType.HasValue ? actionType.Value.ToString() : string.Empty).ResponseToModelAsync<T>();
            }, "WebAPIClient.PutAsync -> PUT entity error", retryPolicyEnabled: retryPolicyEnabled);
        }

        public async Task<T> DeleteAsync<T>(T model, DeleteActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class
        {
            return await ExecuteHelper(async () =>
            {
                SetEntityRest<T>();
                return await _httpClient.DeleteAsync(model, actionType: actionType.HasValue ? actionType.Value.ToString() : string.Empty).ResponseToModelAsync<T>();
            }, "WebAPIClient.DeleteAsync -> DELETE entity error", retryPolicyEnabled: retryPolicyEnabled);
        }

        private async Task<T> RetryingPolicyAction<T>(Func<Task<T>> func, int step = 1, Exception lastException = null)
            where T : class
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                throw lastException ?? new Exception("retry policy expired maximum tentatives");
            }
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaiting).Wait();
                return await RetryingPolicyAction(func, step: ++step, lastException: ex);
            }
        }

        private async Task<T> ExecuteHelper<T>(Func<Task<T>> func, string errorMessage, string lookingWarningMessage = "", bool retryPolicyEnabled = true)
            where T : class
        {
            try
            {
                if (retryPolicyEnabled)
                {
                    return await RetryingPolicyAction(func);
                }
                return await func();
            }
            catch (WebAPIValidationException v_ex)
            {
                _logger.WriteError(new LogMessage($"WebAPI validation exception occurred {v_ex.Message}({v_ex.ValidationErrors.ValidationCode})"), v_ex, LogCategories);
                foreach (ValidationMessageModel item in v_ex.ValidationErrors.ValidationMessages)
                {
                    _logger.WriteWarning(new LogMessage($"{item.Key}({item.MessageCode}): {item.Message}"), LogCategories);
                }
                throw new ValidationException(v_ex.Message, null, v_ex.ValidationErrors.ValidationMessages.ToList());
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(lookingWarningMessage) && ex.Message != null && ex.Message.Contains(lookingWarningMessage))
                {
                    return null;
                }
                _logger.WriteError(new LogMessage(errorMessage), ex, LogCategories);
                throw ex;
            }
        }

        private void SetEntityRest<T>()
            where T : class
        {
            string entityName = typeof(T).Name;
            IWebApiControllerEndpoint controller = _originalHttpClientConfiguration.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            SetEntityRest<T>(controller.ControllerName);
        }

        private void SetEntityRest<T>(string controllerName)
            where T : class
        {
            string entityName = typeof(T).Name;
            IWebApiControllerEndpoint controller = _httpClient.Configuration.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = WebApiHttpClient.API_ADDRESS_NAME;
            controller.ControllerName = controllerName;
        }

        private void SetEntityODATA<T>(string controllerName)
            where T : class
        {
            string entityName = typeof(T).Name;
            IWebApiControllerEndpoint controller = _httpClient.Configuration.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = WebApiHttpClient.ODATA_ADDRESS_NAME;
            controller.ControllerName = controllerName;
        }
        #endregion
    }
}
