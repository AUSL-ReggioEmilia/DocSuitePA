using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
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
        private const string WM_ADDRESS_BASE_ROUTE = "/api/wfm/";
        private const string API_ADDRESS_BASE_ROUTE = "/api/entity/";
        private const string SB_ADDRESS_BASE_ROUTE = "/api/sb/";
        private const string UDS_ADDRESS_BASE_ROUTE = "/api/uds/";
        private const string ODATA_ADDRESS_BASE_ROUTE = "/ODATA/";
        private const string ODATA_AUTHORIZEDFASCICLES_ROUTE = "/FascicleService.AuthorizedFascicles";
        private const string ODATA_AUTHORIZEDDOSSIERS_ROUTE = "/DossierService.GetAuthorizedDossiers";
        private const string TENANTS_COMPOSITECACHEKEY_FORMAT = "{0}|{1}";
        private const int _retry_tentative = 10;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        private readonly ConcurrentDictionary<string, Tenant> _tenantsCache = new ConcurrentDictionary<string, Tenant>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, string> _addressRouteMapping = new Dictionary<string, string>()
        {
            { WebApiHttpClient.WM_ADDRESS_NAME, WM_ADDRESS_BASE_ROUTE },
            { WebApiHttpClient.API_ADDRESS_NAME, API_ADDRESS_BASE_ROUTE },
            { WebApiHttpClient.SB_ADDRESS_NAME, SB_ADDRESS_BASE_ROUTE },
            { WebApiHttpClient.UDS_ADDRESS_NAME, UDS_ADDRESS_BASE_ROUTE },
            { WebApiHttpClient.ODATA_ADDRESS_NAME, ODATA_ADDRESS_BASE_ROUTE }
        };
        #endregion

        #region [Parameter Name]
        private const string PARAMETER_ARCHIVE_SECURITYGROUPS_GENERATION_ENABLED = "ArchiveSecurityGroupsGenerationEnabled";
        private const string PARAMETER_SECURE_DOCUMENT_SIGNATURE_ENABLED = "SecureDocumentSignatureEnabled";
        private const string PARAMETER_SECURE_PAPER_SERVICE_ID = "SecurePaperServiceId";
        private const string PARAMETER_SECURE_PAPER_CERTIFICATE_THUMBPRINT = "SecurePaperCertificateThumbprint";
        private const string PARAMETER_SECURE_PAPER_SERVICE_URL = "SecurePaperServiceUrl";
        private const string PARAMETER_SIGNATURE_PROTOCOL_TYPE = "SignatureProtocolType";
        private const string PARAMETER_SIGNATURE_PROTOCOL_STRING = "SignatureProtocolString";
        private const string PARAMETER_SIGNATURE_PROTOCOL_MAIN_FORMAT = "SignatureProtocolMainFormat";
        private const string PARAMETER_SIGNATURE_PROTOCOL_ATTACHMENT_FORMAT = "SignatureProtocolAttachmentFormat";
        private const string PARAMETER_SIGNATURE_PROTOCOL_ANNEXED_FORMAT = "SignatureProtocolAnnexedFormat";
        private const string PARAMETER_CORPORATE_ACRONYM = "CorporateAcronym";
        private const string PARAMETER_CORPORATE_NAME = "CorporateName";
        private const string PARAMETER_WORFKLOW_LOCATION_ID = "WorkflowLocationId";
        private const string PARAMETER_CONTACTAOOPARENT_ID = "ContactAOOParentId";
        private const string PARAMETER_SIGNATURE_TEMPLATE = "SignatureTemplate";
        private const string PARAMETER_FASCICLE_AUTOCLOSE_THRESHOLD_DAYS = "FascicleAutoCloseThresholdDays";
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
            : this(logger, string.Empty)
        {

        }

        public WebAPIClient(ILogger logger, string webAPIUrl)
        {
            _logger = logger;
            _originalHttpClientConfiguration = new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration;
            IHttpClientConfiguration configuration = new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration;
            _httpClient = new WebApiHttpClient(configuration, _originalHttpClientConfiguration,
                f => _logger.WriteDebug(new LogMessage(f), LogCategories));

            if (!string.IsNullOrEmpty(webAPIUrl))
            {
                StringBuilder sb;
                foreach (IBaseAddress baseAddress in _httpClient.Configuration.Addresses)
                {
                    sb = new StringBuilder();
                    sb.Append(webAPIUrl);
                    if (!_addressRouteMapping.ContainsKey(baseAddress.AddressName))
                    {
                        continue;
                    }
                    sb.Append(_addressRouteMapping[baseAddress.AddressName]);
                    baseAddress.Address = new Uri(sb.ToString());
                }
                _httpClient = new WebApiHttpClient(_httpClient.Configuration, _originalHttpClientConfiguration,
                    f => _logger.WriteDebug(new LogMessage(f), LogCategories));
            }
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

        public async Task PushCorrelatedNotificationAsync(string message, string moduleName, Guid tenantId, Guid tenantAOOId, string tenantName, Guid? correlationId, IIdentityContext identity, NotificationType notificationType)
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
                            @event = new EventWorkflowStartRequestDone(Guid.NewGuid(), correlationId, tenantName, tenantId, tenantAOOId, identity, workflowRequestStatus, null);
                            break;
                        }
                    case NotificationType.EventWorkflowStatusError:
                        {
                            @event = new EventWorkflowStartRequestError(Guid.NewGuid(), correlationId, tenantName, tenantId, tenantAOOId, identity, workflowRequestStatus, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationInfo:
                        {
                            @event = new EventWorkflowNotificationInfo(Guid.NewGuid(), correlationId, tenantName, tenantId, tenantAOOId, identity, workflowNotification, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationInfoAsModel:
                        {
                            @event = new EventWorkflowNotificationInfoAsModel(Guid.NewGuid(), correlationId, tenantName, tenantId, tenantAOOId, identity, workflowNotification, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationWarning:
                        {
                            @event = new EventWorkflowNotificationWarning(Guid.NewGuid(), correlationId, tenantName, tenantId, tenantAOOId, identity, workflowNotification, null);
                            break;
                        }
                    case NotificationType.EventWorkflowNotificationError:
                        {
                            @event = new EventWorkflowNotificationError(Guid.NewGuid(), correlationId, tenantName, tenantId, tenantAOOId, identity, workflowNotification, null);
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
                _httpClient.SetEntityODATA<DocumentUnit>();
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
                _httpClient.SetEntityODATA<DocumentUnit>();
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
                _httpClient.SetEntityODATA<TemplateDocumentRepository>();
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
                _httpClient.SetEntityODATA<DocumentUnitChain>();
                ODataModel<DocumentUnitChain> result = await _httpClient.GetAsync<DocumentUnitChain>()
                    .WithOData($"$filter=DocumentUnit/UniqueId eq {uniqueId}")
                     .ResponseToModelAsync<ODataModel<DocumentUnitChain>>();
                return result.Value;
            }, $"WebAPIClient.GetDocumentUnitChainsAsync -> GET entities error");
        }

        public async Task<ICollection<WorkflowProperty>> GetWorkflowPropertiesAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<WorkflowProperty>();
                ODataModel<WorkflowProperty> result = await _httpClient.GetAsync<WorkflowProperty>()
                    .WithOData($"$filter=WorkflowActivity/UniqueId eq {uniqueId}")
                     .ResponseToModelAsync<ODataModel<WorkflowProperty>>();
                return result.Value;
            }, $"WebAPIClient.GetWorkflowPropertiesAsync -> GET entities error");
        }

        public async Task<ICollection<Location>> GetLocationAsync(int id)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Location>();
                ODataModel<Location> result = await _httpClient.GetAsync<Location>()
                    .WithOData($"$filter=EntityShortId eq {id}")
                     .ResponseToModelAsync<ODataModel<Location>>();
                return result.Value;
            }, $"WebAPIClient.GetLocationAsync -> GET entities error");
        }

        public async Task<Role> GetRoleAsync(short entityShortId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Role>();
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
                _httpClient.SetEntityODATA<WorkflowRoleMapping>();
                ODataModel<WorkflowRoleMapping> result = await _httpClient.GetAsync<WorkflowRoleMapping>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<WorkflowRoleMapping>>();

                return result.Value;
            }, "WebAPIClient.GetWorkflowRoleMappingAsync -> GET entities error");
        }

        public async Task<TenantAOO> GetTenantAOOAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<TenantAOO>();
                ODataModel<TenantAOO> result = await _httpClient.GetAsync<TenantAOO>()
                    .WithOData($"$filter=UniqueId eq {uniqueId}")
                    .ResponseToModelAsync<ODataModel<TenantAOO>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, "WebAPIClient.GetTenantAOOAsync -> GET entities error");
        }

        public async Task<ICollection<TenantAOO>> GetTenantsAOOAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<TenantAOO>();
                ODataModel<TenantAOO> result = await _httpClient.GetAsync<TenantAOO>()
                .WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<TenantAOO>>();

                return result.Value;
            }, "WebAPIClient.GetTenants -> GET entities error");
        }

        public async Task<Tenant> GetTenantAsync(Guid uniqueId, string optionalFilter = null)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Tenant>();

                string tenantCacheKey = string.IsNullOrEmpty(optionalFilter) 
                    ? uniqueId.ToString() 
                    : string.Format(TENANTS_COMPOSITECACHEKEY_FORMAT, uniqueId, optionalFilter);

                if (_tenantsCache.ContainsKey(tenantCacheKey) && _tenantsCache.TryGetValue(tenantCacheKey, out Tenant cachedTenantValue))
                {
                    return cachedTenantValue;
                }

                string baseFilterQuery = $"$filter=UniqueId eq {uniqueId}";
                string odataQuery = string.IsNullOrEmpty(optionalFilter) ? baseFilterQuery : $"{baseFilterQuery}&{optionalFilter}";

                ODataModel<Tenant> result = await _httpClient.GetAsync<Tenant>()
                    .WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<Tenant>>();

                Tenant tenant = result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();

                if (tenant != null && !_tenantsCache.ContainsKey(tenantCacheKey))
                {
                    _tenantsCache.TryAdd(tenantCacheKey, tenant);
                }

                return tenant;
            }, "WebAPIClient.GetTenantAsync -> GET entities error");
        }

        public async Task<Dossier> GetDossierAsync(Guid uniqueId, string optionalFilter = null)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Dossier>();

                string baseFilterQuery = $"$filter=UniqueId eq {uniqueId}";
                string odataQuery = string.IsNullOrEmpty(optionalFilter) ? baseFilterQuery : $"{baseFilterQuery}&{optionalFilter}";

                ODataModel<Dossier> result = await _httpClient.GetAsync<Dossier>()
                    .WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<Dossier>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, "WebAPIClient.GetDossierAsync -> GET entities error");
        }

        public async Task<ICollection<Tenant>> GetTenantsAsync()
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Tenant>();
                ODataModel<Tenant> result = await _httpClient.GetAsync<Tenant>()
                    .ResponseToModelAsync<ODataModel<Tenant>>();

                return result.Value;
            }, "WebAPIClient.GetTenantsAsync -> GET entities error");
        }

        public async Task<ICollection<Tenant>> GetTenantsAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Tenant>();
                ODataModel<Tenant> result = await _httpClient.GetAsync<Tenant>()
                    .WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<Tenant>>();

                return result.Value;
            }, "WebAPIClient.GetTenantsAsync -> GET entities error");
        }

        public async Task<WorkflowActivity> GetWorkflowActivityAsync(Guid uniqueId, string optionalFilter = null)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<WorkflowActivity>();

                string baseFilterQuery = $"$filter=UniqueId eq {uniqueId}";
                string odataQuery = string.IsNullOrEmpty(optionalFilter) ? baseFilterQuery : $"{baseFilterQuery}&{optionalFilter}";
                ODataModel<WorkflowActivity> result = await _httpClient.GetAsync<WorkflowActivity>()
                    .WithOData(odataQuery)
                    .ResponseToModelAsync<ODataModel<WorkflowActivity>>();

                return result == null || result.Value == null ? null : result.Value.SingleOrDefault();
            }, "WebAPIClient.GetWorkflowActivityAsync -> GET entities error");
        }

        public async Task<Fascicle> GetFascicleAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Fascicle>();
                ODataModel<Fascicle> result = await _httpClient.GetAsync<Fascicle>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<Fascicle>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, "WebAPIClient.GetFascicleAsync -> GET entities error");
        }

        public async Task<Fascicle> FindFascicle(FascicleFinderModel finderModel)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Fascicle>();

                Dictionary<string, FascicleFinderModel> finders = new Dictionary<string, FascicleFinderModel>();
                finders.Add("finder", finderModel);
                string bodyQuery = JsonConvert.SerializeObject(finders, new StringEnumConverter());

                ODataModel<Fascicle> result = await _httpClient.PostStringAsync<Fascicle>(ODATA_AUTHORIZEDFASCICLES_ROUTE, bodyQuery)
                    .ResponseToModelAsync<ODataModel<Fascicle>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, "WebAPIClient.FindFascicle -> GET entities error");
        }

        public async Task<Dossier> FindDossier(DossierFinderModel finderModel)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Dossier>();

                Dictionary<string, DossierFinderModel> finders = new Dictionary<string, DossierFinderModel>();
                finders.Add("finder", finderModel);
                string bodyQuery = JsonConvert.SerializeObject(finders, new StringEnumConverter());

                ODataModel<Dossier> result = await _httpClient.PostStringAsync<Dossier>(ODATA_AUTHORIZEDDOSSIERS_ROUTE, bodyQuery)
                    .ResponseToModelAsync<ODataModel<Dossier>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, "WebAPIClient.FindDossier -> GET entities error");
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
                _httpClient.SetEntityODATA<Fascicle>();
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
                _httpClient.SetEntityODATA<FascicleDocumentUnit>();
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
                _httpClient.SetEntityODATA<Category>();
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
                _httpClient.SetEntityODATA<Category>();
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
                _httpClient.SetEntityODATA<Contact>();
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
                _httpClient.SetEntityODATA<Container>();
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
                _httpClient.SetEntityODATA<PECMailBox>();
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
                _httpClient.SetEntityODATA<Protocol>();
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

        public async Task<ICollection<ProtocolLog>> GetProtocolLogAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<ProtocolLog>();
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
                _httpClient.SetEntityODATA<ResolutionLog>();
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
                _httpClient.SetEntityODATA<DocumentSeriesItemLog>();
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
                _httpClient.SetEntityODATA<MetadataRepository>();
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
                _httpClient.SetEntityODATA<UDSRepository>();
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
                _httpClient.SetEntityODATA<UDSRepository>();
                ODataModel<UDSRepository> result = await _httpClient.GetAsync<UDSRepository>()
                    .WithRowQuery($"?$filter=Name eq '{name}' and ExpiredDate eq null and Status eq 'Confirmed'")
                    .ResponseToModelAsync<ODataModel<UDSRepository>>();

                return result == null ? new List<UDSRepository>() : result.Value;
            }, "WebAPIClient.GetUDSRepository -> GET entities error");
        }

        public async Task<ICollection<PECMail>> GetPECMailFromProtocol(Guid uniqueIdProtocol)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<PECMail>();
                ODataModel<PECMail> result = await _httpClient.GetAsync<PECMail>()
                    .WithRowQuery($"?$filter=DocumentUnit/UniqueId eq {uniqueIdProtocol} and DocumentUnit/Environment eq 1&$expand=PECMailBox,Attachments")
                    .ResponseToModelAsync<ODataModel<PECMail>>();

                return result == null ? new List<PECMail>() : result.Value;
            }, "WebAPIClient.GetPECMailFromProtocol -> GET entities error");
        }

        public async Task<ICollection<PECMailAttachment>> GetPECMailAttachmentFromPECMailId(int entityId, string attachmentName)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<PECMailAttachment>();
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
                _httpClient.SetEntityODATA<PECMailReceipt>();
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
                _httpClient.SetEntityODATA<UDSContact>();
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
                _httpClient.SetEntityODATA<UDSPECMail>();
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
                _httpClient.SetEntityODATA<UDSMessage>();
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
                _httpClient.SetEntityODATA<UDSRole>();
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
                _httpClient.SetEntityODATA<UDSDocumentUnit>();
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
                _httpClient.SetEntityODATA<UDSDocumentUnit>();
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
                optionalFilter = AddInvoiceNumberToUDSOdataQuery(invoiceNumber, optionalFilter);
                results = await GetUDSByDocumentName(controllerName, invoiceFilename, optionalFilter, documents, results);
                return results;
            }, "WebAPIClient.GetUDSByInvoiceFilename -> GET entities error");
        }

        public async Task<Dictionary<string, object>> GetRejectedUDSByInvoiceFilename(string controllerName, string invoiceFilename, Dictionary<int, Guid> documents)
        {
            return await ExecuteHelper(async () =>
            {
                Dictionary<string, object> results = new Dictionary<string, object>();
                string optionalFilter = AddInvoiceNumberToUDSOdataQuery(string.Empty, "StatoFattura eq 'Rifiutata dallo SDI'");
                results = await GetUDSByDocumentName(controllerName, invoiceFilename, optionalFilter, documents, results);
                return results;
            }, "WebAPIClient.GetRejectedUDSByInvoiceFilename -> GET entities error");
        }

        private string AddInvoiceNumberToUDSOdataQuery(string invoiceNumber, string optionalFilter)
        {
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
            return optionalFilter;
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
            documentName = $"{Path.GetFileNameWithoutExtension(documentName.Replace(".p7m", string.Empty).Replace(".P7M", string.Empty))}.";
            StringBuilder odata = new StringBuilder();
            odata.Append($"?$filter=_status eq 1 and Documents/any(doc:startswith(doc/DocumentName,'{documentName}') and doc/DocumentType eq 1)");
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
                _httpClient.SetEntityODATA<CollaborationAggregate>();
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
            try
            {
                return await ExecuteHelper(async () =>
                {
                    return await _httpClient.PostAsync(workflowStart).ResponseToModelAsync<WorkflowResult>();
                }, "WebAPIClient.StartWorkflow -> POST model error", retryPolicyEnabled: false);
            }
            catch (ValidationException ex)
            {
                return new WorkflowResult()
                {
                    IsValid = false,
                    Errors = ex.ValidationErrors.Select(f => f.Message).ToList()
                };
            }
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

        public async Task<ODataModel<ProtocolContactManual>> GetProtocolManualContactsWithFiscalCodeAsync(short containerId, DateTimeOffset fromDate, short docTypeCode, int protocolType, string comunicationType, int skip, int top)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<ProtocolContactManual>();
                return await _httpClient.GetAsync<ProtocolContactManual>()
                    .WithOData($"$count=true&$filter=Protocol/RegistrationDate ge {fromDate:s}Z and Protocol/DocType/EntityShortId eq {docTypeCode} and Protocol/ProtocolType/EntityShortId eq {protocolType} and Protocol/Container/EntityShortId eq {containerId} and Protocol/IdStatus eq 0 and FiscalCode ne '' and TelephoneNumber ne '' and ComunicationType eq '{comunicationType}'&$skip={skip}&$top={top}&$apply=groupby((Protocol/RegistrationDate,Protocol/DocType/EntityShortId,Protocol/ProtocolType/EntityShortId,Protocol/IdStatus,Protocol/Container/EntityShortId,ComunicationType,EMailAddress,CertifydMail,Description,FiscalCode,TelephoneNumber))")
                    .ResponseToModelAsync<ODataModel<ProtocolContactManual>>();
            }, "WebAPIClient.GetSendableProtocolManualContacts -> GET entities error");
        }

        public async Task<ODataModel<Protocol>> GetProtocolsByManualContactWithFiscalCodeAsync(string fiscalCode, short containerId, DateTimeOffset fromDate, short doctypeCode, int protocolType, string comunicationType, int skip, int top)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Protocol>();
                return await _httpClient.GetAsync<Protocol>()
                    .WithOData($"$count=true&$filter=RegistrationDate ge {fromDate:s}Z and DocType/EntityShortId eq {doctypeCode} and IdStatus eq 0 and ProtocolType/EntityShortId eq {protocolType} and Container/EntityShortId eq {containerId} and ProtocolContactManuals/any(a:a/FiscalCode eq '{fiscalCode}' and a/ComunicationType eq '{comunicationType}')&$skip={skip}&$top={top}")
                    .ResponseToModelAsync<ODataModel<Protocol>>();
            }, "WebAPIClient.GetProtocolsSendableByManualContactAsync -> GET entities error");
        }

        public async Task<ICollection<WorkflowActivity>> GetWorkflowActivitiesByPropertyAsync(string propertyName, int value)
        {
            return await InternalGetWorkflowActivitiesByPropertyAsync(propertyName, $"ValueInt eq {value}");
        }

        public async Task<ICollection<WorkflowActivity>> GetWorkflowActivitiesByPropertyAsync(string propertyName, string value)
        {
            return await InternalGetWorkflowActivitiesByPropertyAsync(propertyName, $"ValueString eq '{value}'");
        }

        public async Task<ICollection<WorkflowActivity>> GetWorkflowActivitiesByPropertyAsync(string propertyName, Guid value)
        {
            return await GetWorkflowActivitiesByPropertyAsync(propertyName, $"ValueGuid eq {value}");
        }

        private async Task<ICollection<WorkflowActivity>> InternalGetWorkflowActivitiesByPropertyAsync(string propertyName, string filterValue)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<WorkflowActivity>();
                ODataModel<WorkflowActivity> result = await _httpClient.GetAsync<WorkflowActivity>()
                    .WithOData($"$filter=WorkflowProperties/any(wf:wf/Name eq '{propertyName}' and wf/{filterValue})&$expand=WorkflowInstance")
                     .ResponseToModelAsync<ODataModel<WorkflowActivity>>();
                return result.Value;
            }, $"WebAPIClient.GetWorkflowPropertiesAsync -> GET entities error");
        }

        public async Task<int?> GetParameterWorkflowLocationIdAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_WORFKLOW_LOCATION_ID);
            return JsonConvert.DeserializeObject<int?>(result?.Value);
        }

        public async Task<int?> GetParameterContactAOOParentIdAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_CONTACTAOOPARENT_ID);
            return JsonConvert.DeserializeObject<int?>(result?.Value);
        }

        public async Task<string> GetParameterSignatureTemplate()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SIGNATURE_TEMPLATE);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<int?> GetParameterFascicleAutoCloseThresholdDaysAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_FASCICLE_AUTOCLOSE_THRESHOLD_DAYS);
            return JsonConvert.DeserializeObject<int?>(result?.Value);
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
                _httpClient.SetEntityRest<T>();
                return await _httpClient.GetAsync<T>().WithOData(odataQuery).ResponseToModelAsync<TResult>();
            }, "WebAPIClient.GetAsync -> GET entities error");
        }

        public async Task SendCommandAsync<T>(T model)
            where T : Command
        {
            try
            {
                _httpClient.SetEntityRest<T>();
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
                _httpClient.SetEntityRest<T>();
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
                _httpClient.SetEntityRest<T>();
                return await _httpClient.PostAsync(model, actionType: actionType.HasValue ? actionType.Value.ToString() : string.Empty).ResponseToModelAsync<T>();
            }, "WebAPIClient.PostAsync -> POST entity error", retryPolicyEnabled: retryPolicyEnabled);
        }

        public async Task<TResult> PostAsync<T, TResult>(T model, bool retryPolicyEnabled = false)
            where T : class
            where TResult : class
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<T>();
                return await _httpClient.PostAsync(model).ResponseToModelAsync<TResult>();
            }, "WebAPIClient.PostAsync -> POST entity error", retryPolicyEnabled: retryPolicyEnabled);
        }

        public async Task<T> PutAsync<T>(T model, UpdateActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<T>();
                return await _httpClient.PutAsync(model, actionType: actionType.HasValue ? actionType.Value.ToString() : string.Empty).ResponseToModelAsync<T>();
            }, "WebAPIClient.PutAsync -> PUT entity error", retryPolicyEnabled: retryPolicyEnabled);
        }

        public async Task<T> DeleteAsync<T>(T model, DeleteActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<T>();
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

        private async Task<ODataParameterModel> GetParameterByKeyName(string name)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<ODataParameterModel>();
                ODataModel<ODataParameterModel> result = (await _httpClient.GetAsync<ODataParameterModel>()
                    .WithOData(string.Concat("$filter=Key eq '", name, "'"))
                        .ResponseToModelAsync<ODataModel<ODataParameterModel>>());
                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, $"WebAPIClient.GetParameterByKeyName -> GET Parameter error ", lookingWarningMessage: " not found in parameter");
        }

        public async Task<ICollection<ProtocolContactManual>> GetProtocolContactManualsAsync(string odataQuery)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<ProtocolContactManual>();
                ODataModel<ProtocolContactManual> result = await _httpClient.GetAsync<ProtocolContactManual>()
                    .WithOData(odataQuery)
                     .ResponseToModelAsync<ODataModel<ProtocolContactManual>>();

                return result == null ? new List<ProtocolContactManual>() : result.Value;
            }, "WebAPIClient.GetProtocolContactManualsAsync -> GET entities error");
        }

        public async Task<TaskHeaderProtocol> GetTaskHeaderProtocolAsync(Protocol protocol)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<TaskHeaderProtocol>();
                ODataModel<TaskHeaderProtocol> result = await _httpClient.GetAsync<TaskHeaderProtocol>()
                    .WithOData($"$filter=Protocol/UniqueId eq {protocol.UniqueId}&$expand=TaskHeader")
                     .ResponseToModelAsync<ODataModel<TaskHeaderProtocol>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, "WebAPIClient.GetTaskHeaderProtocolAsync -> GET entities error");
        }

        #endregion
    }
}