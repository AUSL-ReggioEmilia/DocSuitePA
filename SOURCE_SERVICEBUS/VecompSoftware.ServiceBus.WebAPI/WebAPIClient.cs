using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Clients.WebAPI.Configuration;
using VecompSoftware.Clients.WebAPI.Exceptions;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.ServiceBus.WebAPI.Exceptions;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.ServiceBus.WebAPI
{
    [LogCategory(LogCategoryDefinition.WEBAPISERVICEBUS)]
    public class WebAPIClient : IWebAPIClient
    {
        #region [ Fields ]
        private readonly WebApiHttpClient _httpClient = null;
        private readonly ILogger _logger = null;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private const int _retry_tentative = 10;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        #endregion

        #region [ Parameter Name ]
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
        private const string PARAMETER_SIGNATURE_TEMPLATE = "SignatureTemplate";
        private const string PARAMETER_MESSAGE_LOCATION = "MessageLocationId";
        private const string PARAMETER_COLLABORATION_LOCATION = "CollaborationLocationId";
        private const string PARAMETER_FASCICLE_MISCELLANEA_LOCATION = "FascicleMiscellaneaLocation";
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
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
        public WebAPIClient(ILogger logger, string addressesJsonConfigPath)
        {
            _logger = logger;
            _httpClient = new WebApiHttpClient(new WebAPIClientConfiguration(addressesJsonConfigPath).HttpConfiguration,
                new WebAPIClientConfiguration(addressesJsonConfigPath).HttpConfiguration, f => _logger.WriteDebug(new LogMessage(f), LogCategories));
        }
        #endregion

        #region [ Methods ]
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

        private async Task<T> ExecuteHelper<T>(Func<Task<T>> func, string errorMessage, string lookingWarningMessage = "", bool retryPolicyEnabled = false)
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

        #region [ CRUD Actions ]
        public async Task<TEntity> PostEntityAsync<TEntity>(TEntity entity, string actionType = "")
            where TEntity : DSWBaseEntity
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<TEntity>();
                return await _httpClient.PostAsync(entity, actionType)
                    .ResponseToModelAsync<TEntity>();
            }, "WebAPIClient.PostEntityAsync -> POST entities error");
        }

        public async Task<TEntity> PutEntityAsync<TEntity>(TEntity entity, string actionType = "")
            where TEntity : DSWBaseEntity
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<TEntity>();
                return await _httpClient.PutAsync(entity, actionType)
                     .ResponseToModelAsync<TEntity>();
            }, "WebAPIClient.PutEntityAsync -> PUT entities error");
        }

        public async Task<TEntity> DeleteEntityAsync<TEntity>(TEntity model, string actionType = "")
            where TEntity : DSWBaseEntity
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<TEntity>();
                return await _httpClient.DeleteAsync(model, actionType)
                    .ResponseToModelAsync<TEntity>();
            }, "WebAPIClient.DeleteEntityAsync -> DELETE entities error");
        }

        public async Task<DocumentUnit> PostDocumentUnitAsync(DocumentUnit entity)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<DocumentUnit>();
                return await _httpClient.PostAsync(entity)
                    .ResponseToModelAsync<DocumentUnit>();
            }, "WebAPIClient.PostDocumentUnitAsync -> POST entities error");
        }

        public async Task<DocumentUnit> PutDocumentUnitAsync(DocumentUnit entity)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<DocumentUnit>();
                return await _httpClient.PutAsync(entity)
                    .ResponseToModelAsync<DocumentUnit>();
            }, "WebAPIClient.PutDocumentUnitAsync -> PUT entities error");
        }

        public async Task<TFascicolable> PostFascicolableEntityAsync<TFascicolable, TEntity>(TFascicolable fascicolableEntity)
            where TFascicolable : DSWBaseEntity, IDSWEntityFascicolable<TEntity>
            where TEntity : DSWBaseEntity
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<TFascicolable>();
                return await _httpClient.PostAsync(fascicolableEntity)
                    .ResponseToModelAsync<TFascicolable>();
            }, $"WebAPIClient.PostFascicolableEntityAsync Error: {typeof(TFascicolable).Name} not sended to WebAPI");
        }

        public async Task<TFascicolable> PutFascicolableEntityAsync<TFascicolable, TEntity>(TFascicolable fascicolableEntity)
            where TFascicolable : DSWBaseEntity, IDSWEntityFascicolable<TEntity>
            where TEntity : DSWBaseEntity
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<TFascicolable>();
                return await _httpClient.PutAsync(fascicolableEntity)
                    .ResponseToModelAsync<TFascicolable>();
            }, $"WebAPIClient.PutFascicolableEntityAsync Error: {typeof(TFascicolable).Name} not sended to WebAPI");
        }

        public async Task<Container> CreateContainerAsync(Container container)
        {
            return await ExecuteHelper(async () =>
            {
                container = await _httpClient.PostAsync(container, "InsertContainer").ResponseToModelAsync();
                return container;
            }, $"WebAPIClient.CreateContainerAsync -> POST entities error");
        }

        public async Task<BuildActionModel> PostBuilderAsync(BuildActionModel entity, Guid IdRepository)
        {
            return await ExecuteHelper(async () =>
            {
                return await _httpClient.PostAsync(entity).WithParameters("idRepository", IdRepository).ResponseToModelAsync<BuildActionModel>();
            }, $"WebAPIClient.PostBuilderAsync -> POST Model error", retryPolicyEnabled: true);
        }

        public async Task<WorkflowResult> StartWorkflowAsync(WorkflowStart entity)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<WorkflowStart>();
                return await _httpClient.PostAsync(entity)
                    .ResponseToModelAsync<WorkflowResult>();
            }, $"WebAPIClient.StartWorkflowAsync -> POST entities error");
        }

        public async Task<WorkflowResult> PushWorkflowNotifyAsync(WorkflowNotify entity)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityRest<WorkflowNotify>();
                return await _httpClient.PostAsync(entity)
                    .ResponseToModelAsync<WorkflowResult>();
            }, $"WebAPIClient.PushWorkflowNotifyAsync -> POST entities error");
        }

        public async Task<bool> PushEventAsync<TEvent>(TEvent evt)
            where TEvent : IEvent
        {
            try
            {
                await _httpClient.PostAsync(evt);
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(ex.Message), ex, LogCategories);
                throw ex;
            }
        }

        public async Task<bool> PushCommandAsync<TCommand>(TCommand command)
           where TCommand : ICommand
        {
            try
            {
                await _httpClient.PostAsync(command);
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(ex.Message), ex, LogCategories);
                throw ex;
            }
        }

        #endregion

        #region [ Finders ]
        public async Task<UDSRepository> GetCurrentUDSRepositoryAsync(string udsRepositoryName)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<UDSRepository>();
                ODataModel<UDSRepository> currentRepository = (await _httpClient.GetAsync<UDSRepository>()
                    .WithOData(string.Concat("$filter=Name eq \'", udsRepositoryName, "\' and ExpiredDate eq null and Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRepositoryStatus\'", DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed, "\'&$orderby=version desc"))
                    .ResponseToModelAsync<ODataModel<UDSRepository>>());

                return currentRepository == null || currentRepository.Value == null || !currentRepository.Value.Any() ? null : currentRepository.Value.Single();
            }, $"WebAPIClient.GetCurrentUDSRepositoryAsync -> GET entities error");
        }

        public async Task<UDSRepository> GetUDSRepositoryAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<UDSRepository>();
                ODataModel<UDSRepository> repository = (await _httpClient.GetAsync<UDSRepository>()
                .WithOData(string.Concat("$filter=UniqueId eq ", uniqueId))
                       .ResponseToModelAsync<ODataModel<UDSRepository>>());
                return repository == null || repository.Value == null || !repository.Value.Any() ? null : repository.Value.Single(); ;
            }, $"WebAPIClient.GetUDSRepositoryAsync -> GET entities error");
        }

        public async Task<PECMail> GetPECMailAsync(int idPECMail)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<PECMail>();
                ODataModel<PECMail> pecMail = (await _httpClient.GetAsync<PECMail>()
                    .WithOData(string.Format("$filter=EntityId eq {0}", idPECMail))
                    .ResponseToModelAsync<ODataModel<PECMail>>());
                return pecMail == null || pecMail.Value == null || !pecMail.Value.Any() ? null : pecMail.Value.Single();
            }, $"WebAPIClient.GetPECMailAsync -> GET entities error");
        }

        public async Task<Collaboration> GetCollaborationAsync(int idCollaboration)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Collaboration>();
                ODataModel<Collaboration> collaboration = (await _httpClient.GetAsync<Collaboration>()
                    .WithOData(string.Format("$filter=EntityId eq {0}", idCollaboration))
                    .ResponseToModelAsync<ODataModel<Collaboration>>());
                return collaboration == null || collaboration.Value == null || !collaboration.Value.Any() ? null : collaboration.Value.Single();
            }, $"WebAPIClient.GetCollaborationAsync -> GET entities error");
        }

        public async Task<UDSSchemaRepository> GetCurrentUDSSchemaRepositoryAsync()
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<UDSSchemaRepository>();
                ODataModel<UDSSchemaRepository> currentSchemaRepository = (await _httpClient.GetAsync<UDSSchemaRepository>()
                       .WithOData("$filter=ExpiredDate eq null")
                       .ResponseToModelAsync<ODataModel<UDSSchemaRepository>>());
                return currentSchemaRepository == null || currentSchemaRepository.Value == null || !currentSchemaRepository.Value.Any() ? null : currentSchemaRepository.Value.Single();
            }, $"WebAPIClient.GetCurrentUDSSchemaRepositoryAsync -> GET entities error");
        }

        public async Task<Fascicle> GetFascicleAsync(Guid documentUnitId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<FascicleDocumentUnit>();
                ODataModel<FascicleDocumentUnit> fascicleDocumentUnit = await _httpClient.GetAsync<FascicleDocumentUnit>()
                .WithOData($"$filter=DocumentUnit/UniqueId eq {documentUnitId} and ReferenceType eq VecompSoftware.DocSuiteWeb.Entity.Fascicles.ReferenceType'{ReferenceType.Fascicle}'&$expand=Fascicle")
                       .ResponseToModelAsync<ODataModel<FascicleDocumentUnit>>();
                return fascicleDocumentUnit == null || fascicleDocumentUnit.Value == null || !fascicleDocumentUnit.Value.Any() ? null : fascicleDocumentUnit.Value.Single().Fascicle;
            }, $"WebAPIClient.GetFascicleAsync -> GET entities error");
        }

        public async Task<Fascicle> GetFascicleByIdAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Fascicle>();
                ODataModel<Fascicle> fascicle = await _httpClient.GetAsync<Fascicle>()
                .WithOData($"$filter=UniqueId eq {uniqueId}&$expand=Category,Contacts,FascicleDocuments,MetadataRepository")
                       .ResponseToModelAsync<ODataModel<Fascicle>>();
                return fascicle == null || fascicle.Value == null || !fascicle.Value.Any() ? null : fascicle.Value.Single();
            }, $"WebAPIClient.GetFascicleByIdAsync -> GET entities error");
        }

        public async Task<Dossier> GetDossierByIdAsync(Guid uniqueId, string optionalFilter = null)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Dossier>();

                string baseOdataFilter = $"$filter=UniqueId eq {uniqueId}";
                string odataQuery = string.IsNullOrEmpty(optionalFilter) ? baseOdataFilter : $"{baseOdataFilter}&{optionalFilter}";

                ODataModel<Dossier> dossier = await _httpClient.GetAsync<Dossier>()
                .WithOData(odataQuery)
                       .ResponseToModelAsync<ODataModel<Dossier>>();
                return dossier == null || dossier.Value == null || !dossier.Value.Any() ? null : dossier.Value.Single();
            }, $"WebAPIClient.GetDossierByIdAsync -> GET entities error");
        }

        public async Task<DocumentUnitFascicleCategory> GetDocumentUnitFascicleCategoryAsync(Guid idDocumentUnit, short idCategory, Guid idFascicle)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<DocumentUnitFascicleCategory>();
                ODataModel<DocumentUnitFascicleCategory> documentUnitFascicleCategory = await _httpClient.GetAsync<DocumentUnitFascicleCategory>()
                .WithOData($"$filter=DocumentUnit/UniqueId eq {idDocumentUnit} and Category/EntityShortId eq {idCategory} and Fascicle/UniqueId eq {idFascicle}")
                    .ResponseToModelAsync<ODataModel<DocumentUnitFascicleCategory>>();
                return documentUnitFascicleCategory == null || documentUnitFascicleCategory.Value == null || !documentUnitFascicleCategory.Value.Any() ? null : documentUnitFascicleCategory.Value.Single();
            }, $"WebAPIClient.GetDocumentUnitFascicleCategoryAsync -> GET entities error");
        }


        public async Task<Location> GetLocationAsync(short idLocation)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Location>();
                ODataModel<Location> location = (await _httpClient.GetAsync<Location>()
                .WithOData(string.Concat("$filter=EntityShortId eq ", idLocation))
                       .ResponseToModelAsync<ODataModel<Location>>());
                return location == null || location.Value == null || !location.Value.Any() ? null : location.Value.Single();
            }, $"WebAPIClient.GetLocationAsync -> GET entities error");
        }

        public async Task<Category> GetCategoryAsync(int idCategory)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Category>();
                ODataModel<Category> category = (await _httpClient.GetAsync<Category>()
                .WithOData(string.Concat("$filter=EntityShortId eq ", idCategory))
                       .ResponseToModelAsync<ODataModel<Category>>());
                return category == null || category.Value == null || !category.Value.Any() ? null : category.Value.Single();
            }, $"WebAPIClient.GetCategoryAsync -> GET entities error");
        }

        public async Task<Container> GetContainerAsync(int idContainer)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Container>();
                ODataModel<Container> container = (await _httpClient.GetAsync<Container>()
                .WithOData(string.Concat("$filter=EntityShortId eq ", idContainer))
                       .ResponseToModelAsync<ODataModel<Container>>());
                return container == null || container.Value == null || !container.Value.Any() ? null : container.Value.Single();
            }, $"WebAPIClient.GetContainerAsync -> GET entities error");
        }

        public async Task<DocumentUnit> GetDocumentUnitAsync(DocumentUnit documentUnit)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<DocumentUnit>();
                ODataModel<DocumentUnit> result = (await _httpClient.GetAsync<DocumentUnit>()
                    .WithOData(string.Concat("$filter=UniqueId eq ", documentUnit.UniqueId, "&$expand=DocumentUnitRoles,DocumentUnitChains,DocumentUnitUsers,Fascicle,Category,Container,UDSRepository"))
                     .ResponseToModelAsync<ODataModel<DocumentUnit>>());
                return result.Value.SingleOrDefault();
            }, $"WebAPIClient.GetDocumentUnitAsync -> GET entities error");
        }

        public async Task<ICollection<Fascicle>> GetPeriodicFasciclesAsync()
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Fascicle>();
                ODataModel<Fascicle> result = (await _httpClient.GetAsync<Fascicle>()
                    .WithOData(string.Concat("$filter=FascicleType eq VecompSoftware.DocSuiteWeb.Entity.Fascicles.FascicleType\'", FascicleType.Period, "\' and Category/isActive eq 1&$expand=Category,FasciclePeriod"))
                     .ResponseToModelAsync<ODataModel<Fascicle>>());
                return result.Value;
            }, $"WebAPIClient.GetPeriodicFasciclesAsync -> GET entities error");
        }

        public async Task<Fascicle> GetAvailablePeriodicFascicleByDocumentUnitAsync(DocumentUnit documentUnit)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Fascicle>();
                ODataModel<Fascicle> result = (await _httpClient.GetAsync<Fascicle>()
                    .WithRowQuery(string.Format("/FascicleService.PeriodicFascicles(uniqueId = {0})", documentUnit.UniqueId))
                     .ResponseToModelAsync<ODataModel<Fascicle>>());
                return result.Value.SingleOrDefault();
            }, $"WebAPIClient.GetAvailablePeriodicFascicleByDocumentUnitAsync -> GET entities error");
        }

        public async Task<Role> GetRoleAsync(RoleModel roleModel)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Role>();

                if (!roleModel.IdRole.HasValue && !roleModel.UniqueId.HasValue)
                {
                    _logger.WriteWarning(new LogMessage("RoleModel non ha nessun id specificato."), LogCategories);
                    return null;
                }
                string odataFilter = roleModel.IdRole.HasValue ? "$filter=EntityShortId eq " : "$filter=UniqueId eq ";
                string idRole = roleModel.IdRole.HasValue ? roleModel.IdRole.Value.ToString() : roleModel.UniqueId.ToString();

                ODataModel<Role> role = (await _httpClient.GetAsync<Role>()
                .WithOData(string.Concat(odataFilter, idRole))
                .ResponseToModelAsync<ODataModel<Role>>());
                return role == null || role.Value == null || !role.Value.Any() ? null : role.Value.Single();
            }, $"WebAPIClient.GetRoleAsync -> GET entities error");
        }

        public async Task<Protocol> GetProtocolAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Protocol>();
                ODataModel<Protocol> protocol = (await _httpClient.GetAsync<Protocol>()
                .WithOData(string.Concat("$filter=UniqueId eq ", uniqueId))
                       .ResponseToModelAsync<ODataModel<Protocol>>());
                return protocol == null || protocol.Value == null || !protocol.Value.Any() ? null : protocol.Value.Single(); ;
            }, $"WebAPIClient.GetProtocolAsync -> GET entities error");
        }

        public async Task<Resolution> GetResolutionAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<Resolution>();
                ODataModel<Resolution> resolution = (await _httpClient.GetAsync<Resolution>()
                .WithOData(string.Concat("$filter=UniqueId eq ", uniqueId))
                       .ResponseToModelAsync<ODataModel<Resolution>>());
                return resolution == null || resolution.Value == null || !resolution.Value.Any() ? null : resolution.Value.Single(); ;
            }, $"WebAPIClient.GetResolutionAsync -> GET entities error");
        }

        public async Task<DocumentSeriesItem> GetDocumentSeriesItemAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<DocumentSeriesItem>();
                ODataModel<DocumentSeriesItem> documentSeriesItem = (await _httpClient.GetAsync<DocumentSeriesItem>()
                .WithOData(string.Concat("$filter=UniqueId eq ", uniqueId))
                       .ResponseToModelAsync<ODataModel<DocumentSeriesItem>>());
                return documentSeriesItem == null || documentSeriesItem.Value == null || !documentSeriesItem.Value.Any() ? null : documentSeriesItem.Value.Single(); ;
            }, $"WebAPIClient.GetDocumentSeriesItemAsync -> GET entities error");
        }


        public async Task<DomainUserModel> GetSignerInformationAsync(string username, string domain)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<DomainUserModel>();
                DomainUserModel result = (await _httpClient.GetAsync<DomainUserModel>()
                    .WithRowQuery(string.Format("/DomainUserService.GetUser(username='{0}',domain='{1}')", username, domain))
                     .ResponseToModelAsync<DomainUserModel>());
                return result;
            }, $"WebAPIClient.GetDefaultCategoryFascicleAsync -> GET entities error", lookingWarningMessage: "not found in ActiveDirectory");
        }

        public async Task<CategoryFascicle> GetPeriodicCategoryFascicleByEnvironmentAsync(short idCategory, int DSWEnvironment)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<CategoryFascicle>();
                ODataModel<CategoryFascicle> result = (await _httpClient.GetAsync<CategoryFascicle>()
                    .WithOData(string.Concat("$filter=Category/EntityShortId eq ", idCategory, " and DSWEnvironment eq ", DSWEnvironment, "&$expand=Category,FasciclePeriod"))
                     .ResponseToModelAsync<ODataModel<CategoryFascicle>>());
                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, $"WebAPIClient.GetPeriodicCategoryFascicleByEnvironmentAsync -> GET entities error");
        }

        public async Task<CategoryFascicle> GetDefaultCategoryFascicleAsync(short idCategory)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<CategoryFascicle>();
                ODataModel<CategoryFascicle> result = (await _httpClient.GetAsync<CategoryFascicle>()
                    .WithOData(string.Concat("$filter=Category/EntityShortId eq ", idCategory, " and DSWEnvironment eq 0 &$expand=Category"))
                     .ResponseToModelAsync<ODataModel<CategoryFascicle>>());
                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, $"WebAPIClient.GetDefaultCategoryFascicleAsync -> GET entities error");
        }

        public async Task<ICollection<ProtocolLog>> GetProtocolLogAsync(string odataFilter)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<ProtocolLog>();
                ODataModel<ProtocolLog> protocolLogs = (await _httpClient.GetAsync<ProtocolLog>()
                .WithOData(odataFilter)
                       .ResponseToModelAsync<ODataModel<ProtocolLog>>());
                return protocolLogs.Value;
            }, $"WebAPIClient.GetProtocolLogAsync -> GET entities error");
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
        public async Task<ICollection<UDSDocumentUnit>> GetUDSDocumentUnitRelationByID(Guid IdUDS)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<UDSDocumentUnit>();
                ODataModel<UDSDocumentUnit> UDSDocumentUnit = await _httpClient.GetAsync<UDSDocumentUnit>()
                .WithOData($"$expand=Relation($expand=UDSRepository($select=UniqueId))&$filter=Relation/Environment ge 100 and IdUDS eq {IdUDS}")
                       .ResponseToModelAsync<ODataModel<UDSDocumentUnit>>();
                return UDSDocumentUnit == null || UDSDocumentUnit.Value == null || !UDSDocumentUnit.Value.Any() ? null : UDSDocumentUnit.Value;
            }, $"WebAPIClient.GetDefaultFascicleFolderAsync -> GET entities error");
        }

        public async Task<WorkflowProperty> GetWorkflowActivityProperty(Guid idWorkflowActivity, string propertyName)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<WorkflowProperty>();
                ODataModel<WorkflowProperty> workflowProperty = await _httpClient.GetAsync<WorkflowProperty>()
                .WithOData($"$filter=WorkflowActivity/UniqueId eq {idWorkflowActivity} and Name eq '{propertyName}'")
                       .ResponseToModelAsync<ODataModel<WorkflowProperty>>();
                return workflowProperty == null || workflowProperty.Value == null || !workflowProperty.Value.Any() ? null : workflowProperty.Value.Single();
            }, $"WebAPIClient.GetWorkflowActivityProperty -> GET entities error");
        }

        public async Task<FascicleFolder> GetFascicleFolderAsync(Guid uniqueId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<FascicleFolder>();
                ODataModel<FascicleFolder> fascicleFolder = await _httpClient.GetAsync<FascicleFolder>()
                .WithOData($"$filter=UniqueId eq {uniqueId}")
                       .ResponseToModelAsync<ODataModel<FascicleFolder>>();
                return fascicleFolder == null || fascicleFolder.Value == null || !fascicleFolder.Value.Any() ? null : fascicleFolder.Value.Single();
            }, $"WebAPIClient.GetFascicleFolderAsync -> GET entities error");
        }

        public async Task<FascicleDocument> GetFascicleDocumentByFolderAsync(Guid fascicleFolderId)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<FascicleDocument>();
                ODataModel<FascicleDocument> fascicleDocument = await _httpClient.GetAsync<FascicleDocument>()
                .WithOData($"$filter=FascicleFolder/UniqueId eq {fascicleFolderId}")
                       .ResponseToModelAsync<ODataModel<FascicleDocument>>();
                return fascicleDocument == null || fascicleDocument.Value == null || !fascicleDocument.Value.Any() ? null : fascicleDocument.Value.Single();
            }, $"WebAPIClient.GetFascicleDocumentByFolderAsync -> GET entities error");
        }

        public async Task<DossierFolder> GetDossierFolderParentAsync(Guid idDossierFolder)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<DossierFolder>();
                ODataModel<DossierFolder> result = (await _httpClient.GetAsync<DossierFolder>()
                    .WithRowQuery($"/DossierFolderService.GetParent(idDossierFolder={idDossierFolder})")
                     .ResponseToModelAsync<ODataModel<DossierFolder>>());
                return result.Value.SingleOrDefault();
            }, $"WebAPIClient.GetDossierFolderParentAsync -> GET entities error");
        }

        public async Task<DossierFolder> GetDossierFolderAsync(Guid idDossierFolder)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<DossierFolder>();
                ODataModel<DossierFolder> dossierFolder = (await _httpClient.GetAsync<DossierFolder>()
                    .WithOData($"$filter=UniqueId eq {idDossierFolder}")
                     .ResponseToModelAsync<ODataModel<DossierFolder>>());
                return dossierFolder == null || dossierFolder.Value == null || !dossierFolder.Value.Any() ? null : dossierFolder.Value.Single(); ;
            }, $"WebAPIClient.GetDossierFolderAsync -> GET entities error");
        }
        #endregion

        #region [ Parameters ]
        private async Task<ODataParameterModel> GetParameterByKeyName(string name)
        {
            return await ExecuteHelper(async () =>
            {
                _httpClient.SetEntityODATA<ODataParameterModel>();
                ODataModel<ODataParameterModel> result = (await _httpClient.GetAsync<ODataParameterModel>()
                    .WithOData(string.Concat("$filter=Key eq '", name, "'"))
                        .ResponseToModelAsync<ODataModel<ODataParameterModel>>());
                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }, $"WebAPIClient.GetParameterByKeyName -> GET Parameter error", lookingWarningMessage: "not found in parameter");
        }

        public async Task<bool> GetAutomaticSecurityGroupsEnabledAsync()
        {
            try
            {
                ODataParameterModel result = await GetParameterByKeyName(PARAMETER_ARCHIVE_SECURITYGROUPS_GENERATION_ENABLED);
                bool.TryParse(result?.Value, out bool parameterValue);
                return parameterValue;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<bool> GetSecureDocumentSignatureEnabledAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SECURE_DOCUMENT_SIGNATURE_ENABLED);
            bool.TryParse(result?.Value, out bool parameterValue);
            return parameterValue;
        }

        public async Task<int> GetParameterSecurePaperServiceIdAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SECURE_PAPER_SERVICE_ID);
            int parameterValue = -1;
            int.TryParse(result?.Value, out parameterValue);
            return parameterValue;
        }

        public async Task<string> GetParameterSecurePaperCertificateThumbprintAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SECURE_PAPER_CERTIFICATE_THUMBPRINT);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<string> GetParameterSecurePaperServiceUrlAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SECURE_PAPER_SERVICE_URL);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<short> GetParameterSignatureProtocolTypeAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SIGNATURE_PROTOCOL_TYPE);
            short.TryParse(result?.Value, out short parameterValue);
            return parameterValue;
        }

        public async Task<string> GetParameterSignatureProtocolStringAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SIGNATURE_PROTOCOL_STRING);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<string> GetParameterSignatureProtocolMainFormatAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SIGNATURE_PROTOCOL_MAIN_FORMAT);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<string> GetParameterSignatureProtocolAttachmentFormatAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SIGNATURE_PROTOCOL_ATTACHMENT_FORMAT);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<string> GetParameterSignatureProtocolAnnexedFormatAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SIGNATURE_PROTOCOL_ANNEXED_FORMAT);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<string> GetParameterCorporateAcronymAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_CORPORATE_ACRONYM);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<string> GetParameterCorporateNameAsync()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_CORPORATE_NAME);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<string> GetParameterSignatureTemplate()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_SIGNATURE_TEMPLATE);
            return JsonConvert.DeserializeObject<string>(result?.Value);
        }

        public async Task<int?> GetParameterCollaborationLocation()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_COLLABORATION_LOCATION);
            return JsonConvert.DeserializeObject<int?>(result?.Value);
        }

        public async Task<short?> GetParameterMessageLocation()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_MESSAGE_LOCATION);
            return JsonConvert.DeserializeObject<short?>(result?.Value);
        }

        public async Task<short?> GetParameterFascicleMiscellaneaLocation()
        {
            ODataParameterModel result = await GetParameterByKeyName(PARAMETER_FASCICLE_MISCELLANEA_LOCATION);
            return JsonConvert.DeserializeObject<short?>(result?.Value);
        }

        #endregion

        #endregion
    }
}
