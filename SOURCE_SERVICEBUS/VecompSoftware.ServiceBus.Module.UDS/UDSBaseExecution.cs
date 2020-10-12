using Microsoft.Web.Administration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.ServiceBus.Module.UDS.Storage.Relations;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;
using WebApiEntity = VecompSoftware.DocSuiteWeb.Entity.UDS;
using WebApiModel = VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.ServiceBus.Module.UDS
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public abstract class UDSBaseExecution<TCommand> : IListenerExecution<TCommand>
        where TCommand : ICommand
    {
        #region [ Fields ]
        private readonly TimeSpan _threadWaithing = TimeSpan.FromSeconds(1);
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webApiClient;
        private readonly UDSConfig _udsConfig;
        private readonly string _path_JsonConfigModule = Path.Combine(Environment.CurrentDirectory, "Module.UDS.Config.json");
        protected static IEnumerable<LogCategory> _logCategories = null;
        private WebApiEntity.UDSSchemaRepository _udsSchemaRepository;

        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(UDSBaseExecution<>));
                }
                return _logCategories;
            }
        }

        public IDictionary<string, object> Properties { get; set; }

        public EvaluationModel RetryPolicyEvaluation { get; set; }
        public Guid? IdWorkflowActivity { get; set; }

        public string DBSchema => _udsConfig.DbSchema;

        public string ConnectionString => _udsConfig.ConnectionString;

        public string OutputDLLPath => _udsConfig.OutputDLLPath;

        public string SolutionPath => _udsConfig.SolutionPath;

        public string CompilationLoggerPath => _udsConfig.CompilationLoggerPath;

        public string WebAPI_UDS_Path => _udsConfig.WebAPI_UDS_Path;

        public string WebAPI_PoolName => _udsConfig.WebAPI_PoolName;

        public string BiblosDS_Storage_MainPath => _udsConfig.BiblosDS_Storage_MainPath;

        public string BiblosDS_Storage_StorageType => _udsConfig.BiblosDS_Storage_StorageType;

        public string ProjectName => _udsConfig.ProjectName;

        public WebApiEntity.UDSSchemaRepository CurrentUDSSchemaRepository
        {
            get
            {
                if (_udsSchemaRepository == null)
                {
                    _udsSchemaRepository = GetCurrentUDSSchemaRepositoryAsync().Result;
                }
                return _udsSchemaRepository;
            }
        }
        #endregion

        #region [ Constructor ]
        public UDSBaseExecution(ILogger logger, IWebAPIClient webApiClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _udsConfig = JsonConvert.DeserializeObject<UDSConfig>(File.ReadAllText(_path_JsonConfigModule));
        }
        #endregion

        #region [ Methods ]
        public abstract Task ExecuteAsync(TCommand entity);

        private async Task<WebApiEntity.UDSSchemaRepository> GetCurrentUDSSchemaRepositoryAsync()
        {
            try
            {
                return await _webApiClient.GetCurrentUDSSchemaRepositoryAsync();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<CategoryFascicle> GetPeriodicCategoryFascicleByEnvironment(short idCategory, int DSWEnvironment)
        {
            try
            {
                return await _webApiClient.GetPeriodicCategoryFascicleByEnvironmentAsync(idCategory, DSWEnvironment);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<CategoryFascicle> GetDefaultCategoryFascicle(short idCategory)
        {
            try
            {
                return await _webApiClient.GetDefaultCategoryFascicleAsync(idCategory);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<WebApiEntity.UDSRepository> GetCurrentUDSRepositoryAsync(string udsRepositoryName)
        {
            try
            {
                return await _webApiClient.GetCurrentUDSRepositoryAsync(udsRepositoryName);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<WebApiEntity.UDSRepository> GetUDSRepositoryAsync(Guid idUDSRepository)
        {
            try
            {
                return await _webApiClient.GetUDSRepositoryAsync(idUDSRepository);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<ICollection<WebApiEntity.UDSDocumentUnit>> GetUDSDocumentUnitRelationByID(Guid IdUDS)
        {
            try
            {
                return await _webApiClient.GetUDSDocumentUnitRelationByID(IdUDS);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<WebApiEntity.UDSRepository> UpdateUDSRepositoryAsync(WebApiEntity.UDSRepository udsRepository)
        {
            try
            {
                return await _webApiClient.PutEntityAsync(udsRepository);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<WebApiEntity.UDSRepository> DeleteUDSRepositoryAsync(WebApiEntity.UDSRepository udsRepository)
        {
            try
            {
                return await _webApiClient.DeleteEntityAsync(udsRepository);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<PECMail> GetPECMailAsync(int idPECMail)
        {
            try
            {
                return await _webApiClient.GetPECMailAsync(idPECMail);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<Collaboration> GetCollaborationAsync(int idCollaboration)
        {
            try
            {
                return await _webApiClient.GetCollaborationAsync(idCollaboration);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<Protocol> GetProtocolAsync(Guid idProtocol)
        {
            try
            {
                return await _webApiClient.GetProtocolAsync(idProtocol);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<PECMail> UpdatePECMailAsync(PECMail pecMail, string actionType = "")
        {
            try
            {
                return await _webApiClient.PutEntityAsync(pecMail, actionType);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<Collaboration> UpdateCollaborationAsync(Collaboration collaboration)
        {
            try
            {
                return await _webApiClient.PutEntityAsync(collaboration);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<Protocol> UpdateProtocolAsync(Protocol protocol)
        {
            try
            {
                return await _webApiClient.PutEntityAsync(protocol, EnumHelper.GetDescription(UpdateActionType.ProtocolArchivedUpdate));
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<Container> CreateConteainerAsync(Container container)
        {
            try
            {
                return await _webApiClient.CreateContainerAsync(container);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<PECMailLog> CreatePECMailLogAsync(PECMailLog pecMailLog)
        {
            try
            {
                return await _webApiClient.PostEntityAsync(pecMailLog);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task<bool> PushEventAsync<TEvent>(TEvent evt)
            where TEvent : IEvent
        {
            try
            {
                return await _webApiClient.PushEventAsync(evt);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }

        }

        public async Task<bool> PushCommandAsync<TCCommand>(TCCommand command)
            where TCCommand : ICommand
        {
            try
            {
                return await _webApiClient.PushCommandAsync(command);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }

        }

        public T RetryingPolicyAction<T>(Func<T> func, int step = 1, int retry_tentative = 5)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{retry_tentative} in progress..."), LogCategories);
            if (step >= retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.ServiceBus.Module.UDS: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("Module UDS retry policy expired maximum tentatives");
            }
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{retry_tentative} faild. Waithing {_threadWaithing} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaithing).Wait();
                return RetryingPolicyAction(func, ++step);
            }
        }

        public bool InstallUDS()
        {
            try
            {
                return RetryingPolicyAction(() =>
                {
                    _logger.WriteInfo(new LogMessage("Build successful"), LogCategories);
                    using (ServerManager iisManager = new ServerManager())
                    {
                        ApplicationPool appPool = iisManager.ApplicationPools[WebAPI_PoolName];
                        if (appPool != null)
                        {
                            appPool.Stop();
                            int i = 5;
                            while (i >= 0 && appPool.State != ObjectState.Stopped)
                            {
                                Thread.Sleep(2000);
                                i--;
                            }
                            _logger.WriteInfo(new LogMessage($"ApplicationPool stoped - {appPool.Name}"), LogCategories);
                        }
                        string dllName = $"{ProjectName}.dll";
                        string pdbName = $"{ProjectName}.pdb";
                        string dll = Path.Combine(OutputDLLPath, dllName);
                        string dllDest = Path.Combine(WebAPI_UDS_Path, dllName);
                        string pdb = Path.Combine(OutputDLLPath, pdbName);
                        string pdbDest = Path.Combine(WebAPI_UDS_Path, pdbName);

                        _logger.WriteInfo(new LogMessage($"Copy {dll} to {dllDest}"), LogCategories);
                        File.Copy(dll, dllDest, true);
                        _logger.WriteInfo(new LogMessage($"Copy {pdb} to {pdbDest}"), LogCategories);
                        File.Copy(pdb, pdbDest, true);
                        if (appPool != null)
                        {
                            appPool.Start();
                            _logger.WriteInfo(new LogMessage($"ApplicationPool starting {appPool.Name}"), LogCategories);
                        }
                    }

                    return true;
                });
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                return false;
            }
        }

        protected void ResetModelXML(WebApiModel.UDSBuildModel currentModel)
        {
            currentModel.XMLContent = string.Empty;
        }

        protected Container CreateContainerFromArchive(string archiveName, string adminUserName)
        {
            Container container = new Container
            {
                Name = string.Concat("Archivio ", archiveName),
                PrefixSecurityGroupName = string.Concat("uds_", archiveName.Replace(" ", string.Empty).ToLower()),
                SecurityUserAccount = adminUserName
            };
            return container;
        }

        protected async Task InsertLogAsync(UDSEntityModel model, Guid udsRepositoryId, int environment)
        {
            if (model == null || model.Logs == null || !model.Logs.Any())
            {
                return;
            }

            try
            {
                foreach (UDSLogModel log in model.Logs)
                {
                    WebApiEntity.UDSLog webApiLog = MapLog(log);
                    webApiLog.Entity = new WebApiEntity.UDSRepository(udsRepositoryId);
                    webApiLog.Environment = environment;
                    await InsertLogAsync(webApiLog);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Format("InsertLogAsync -> Errore in inserimento log: {0}", ex.Message)), LogCategories);
                throw ex;
            }
        }

        protected async Task<WebApiEntity.UDSLog> InsertLogAsync(WebApiEntity.UDSLog log)
        {
            try
            {
                return await _webApiClient.PostEntityAsync(log);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        private async Task PostRelationsAsync(UDSEntityModel model, Guid udsRepositoryId, int environment, string userName, DateTimeOffset creationTime, BuildActionType buildActionType)
        {
            if (model == null || model.Relations == null || (model.Relations == null && (model.Users == null || !model.Users.Any())))
            {
                return;
            }

            try
            {
                BuildActionModel actionModel = new BuildActionModel
                {
                    ReferenceId = model.IdUDS,
                    BuildType = buildActionType,
                    Model = PrepareRelations(model, udsRepositoryId, environment, userName, creationTime, buildActionType)
                };

                await _webApiClient.PostBuilderAsync(actionModel, udsRepositoryId);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Format("PostRelationsAsync -> Errore in inserimento relazioni: {0}", ex.Message)), LogCategories);
                throw ex;
            }
        }

        protected async Task InsertRelationsAsync(UDSEntityModel model, Guid udsRepositoryId, int environment, string userName, DateTimeOffset creationTime)
        {
            await PostRelationsAsync(model, udsRepositoryId, environment, userName, creationTime, BuildActionType.Build);
        }

        protected async Task UpdateRelationsAsync(UDSEntityModel model, Guid udsRepositoryId, int environment, string userName, DateTimeOffset creationTime)
        {
            await PostRelationsAsync(model, udsRepositoryId, environment, userName, creationTime, BuildActionType.Synchronize);
        }

        #region [ Mappers ]

        protected WebApiEntity.UDSLog MapLog(UDSLogModel localLog)
        {
            WebApiEntity.UDSLog webApiLog = new WebApiEntity.UDSLog
            {
                IdUDS = localLog.UDSId,
                LogDescription = localLog.LogDescription,
                RegistrationDate = localLog.LogDate,
                SystemComputer = localLog.SystemComputer,
                RegistrationUser = localLog.SystemUser,
                LogType = (WebApiEntity.UDSLogType)Enum.Parse(typeof(WebApiEntity.UDSLogType), localLog.LogType.ToString())
            };

            return webApiLog;
        }

        protected WebApiModel.UDSBuildModel MapUDSModel(WebApiModel.UDSBuildModel model, UDSEntityModel entityModel)
        {
            model.Year = entityModel.Year;
            model.Number = entityModel.Number;
            model.RegistrationDate = entityModel.RegistrationDate;
            model.LastChangedDate = entityModel.LastChangedDate;
            model.LastChangedUser = entityModel.LastChangedUser;
            model.Subject = entityModel.Subject;
            model.Title = entityModel.Title;
            model.Category = entityModel.IdCategory.HasValue ? new CategoryModel(entityModel.IdCategory) : null;
            model.Container = entityModel.IdContainer.HasValue ? new ContainerModel(entityModel.IdContainer) : null;
            model.Roles = entityModel.Relations != null ? MapUDSRoles(entityModel.Relations.Authorizations) : new List<RoleModel>();
            model.Users = MapUDSUsers(entityModel.Users);
            model.Documents = entityModel.Relations != null ? MapUDSDocuments(entityModel.Relations.Documents) : new List<WebApiModel.UDSDocumentModel>();

            if (model.WorkflowActions == null)
            {
                model.WorkflowActions = new List<IWorkflowAction>();
            }
            model.WorkflowActions = model.WorkflowActions.Concat(MapWorkflowLinkActions(entityModel)).ToList();
            return model;
        }

        private ICollection<IWorkflowAction> MapWorkflowLinkActions(UDSEntityModel model)
        {
            if (model.Relations == null)
            {
                return new List<IWorkflowAction>();
            }

            List<IWorkflowAction> workflowActions = new List<IWorkflowAction>();
            workflowActions.AddRange(MapWorkflowPECMailLinkActions(model));
            return workflowActions;
        }

        private ICollection<IWorkflowAction> MapWorkflowPECMailLinkActions(UDSEntityModel model)
        {
            if (model.Relations.PECMails == null || !model.Relations.PECMails.Any())
            {
                return new List<IWorkflowAction>();
            }

            ICollection<IWorkflowAction> workflowActions = new List<IWorkflowAction>();
            foreach (UDSPECMail instance in model.Relations.PECMails)
            {
                PECMail pecInstance = GetPECMailAsync(instance.IdPECMail).Result;
                if (pecInstance != null && !pecInstance.Year.HasValue)
                {
                    workflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                        new DocumentUnitModel() { UniqueId = model.IdUDS, Number = model.Number.Value.ToString(), Year = model.Year.Value },
                        new DocumentUnitModel() { UniqueId = instance.UniqueIdPECMail, EntityId = instance.IdPECMail, Environment = (int)DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.PECMail }));
                }
            }
            return workflowActions;
        }

        protected ICollection<UserModel> MapUDSUsers(IEnumerable<UDSUserModel> users)
        {
            ICollection<UserModel> udsUsers = new List<UserModel>();
            UserModel user = new UserModel();
            if (users != null)
            {
                foreach (UDSUserModel userModel in users)
                {
                    user.Account = userModel.Account;
                    user.AuthorizationType = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Accounted;
                    udsUsers.Add(user);
                }
            }
            return udsUsers;
        }

        protected ICollection<RoleModel> MapUDSRoles(IEnumerable<UDSAuthorization> authorizations)
        {
            ICollection<RoleModel> UDSRoles = new List<RoleModel>();
            if (authorizations != null)
            {
                foreach (UDSAuthorization authorization in authorizations)
                {
                    RoleModel role = new RoleModel(Convert.ToInt16(authorization.IdRole))
                    {
                        UniqueId = authorization.UniqueIdRole,
                        RoleLabel = authorization.RoleLabel
                    };
                    UDSRoles.Add(role);
                }
            }
            return UDSRoles;
        }

        protected ICollection<WebApiModel.UDSDocumentModel> MapUDSDocuments(IEnumerable<UDSDocument> documents)
        {
            ICollection<WebApiModel.UDSDocumentModel> UDSDocuments = new List<WebApiModel.UDSDocumentModel>();
            if (documents != null)
            {
                foreach (UDSDocument document in documents)
                {
                    WebApiModel.UDSDocumentModel UDSDocument = new WebApiModel.UDSDocumentModel()
                    {
                        IdChain = document.IdDocument,
                        DocumentType = (WebApiModel.UDSDocumentType)document.DocumentType,
                        DocumentLabel = document.DocumentLabel,
                        DocumentName = document.DocumentName
                    };
                    UDSDocuments.Add(UDSDocument);
                }
            }
            return UDSDocuments;
        }

        protected WebApiModel.UDSRoleModel MapAuthorization(UDSAuthorization udsAuthorization, DateTimeOffset creationTime, string userName, int environment)
        {

            WebApiModel.UDSRoleModel webAPIUDSRoleModel = new WebApiModel.UDSRoleModel
            {
                UniqueId = udsAuthorization.UDSAuthorizationId,
                IdRole = udsAuthorization.IdRole,
                IdUDS = udsAuthorization.UDSId,
                Environment = environment,
                AuthorizationLabel = udsAuthorization.RoleLabel,
                AuthorizationType = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Accounted,
                RegistrationDate = creationTime,
                RegistrationUser = userName,
                RelationType = WebApiModel.UDSRelationType.Role
            };

            return webAPIUDSRoleModel;
        }

        protected WebApiModel.UDSUserModel MapUser(UDSUserModel userModel, DateTimeOffset creationTime, string userName, int environment, Guid idUDS)
        {
            WebApiModel.UDSUserModel webApiUser = new WebApiModel.UDSUserModel
            {
                Account = userModel.Account,
                IdUDS = idUDS,
                Environment = environment,
                AuthorizationType = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Accounted,
                RegistrationDate = creationTime,
                RegistrationUser = userName
            };
            return webApiUser;
        }

        protected WebApiModel.UDSCollaborationModel MapCollaboration(UDSCollaboration collaborationModel, DateTimeOffset creationTime, string userName, int environment, Guid idUDS)
        {
            WebApiModel.UDSCollaborationModel webApiCollaboration = new WebApiModel.UDSCollaborationModel
            {
                RelationType = WebApiModel.UDSRelationType.Collaboration,
                IdUDS = idUDS,
                IdCollaboration = collaborationModel.IdCollaboration,
                Environment = environment,
                RegistrationUser = userName,
                RegistrationDate = creationTime
            };
            webApiCollaboration.RelationType = WebApiModel.UDSRelationType.Collaboration;
            return webApiCollaboration;
        }

        protected WebApiModel.UDSContactModel MapContact(UDSContact contactModel, DateTimeOffset creationTime, string userName, int environment)
        {
            WebApiModel.UDSContactModel webApiContact = new WebApiModel.UDSContactModel
            {
                UniqueId = contactModel.UDSContactId,
                RelationType = WebApiModel.UDSRelationType.Contact,
                IdUDS = contactModel.UDSId,
                IdContact = contactModel.IdContact ?? 0,
                ContactManual = contactModel.ContactManual,
                ContactLabel = contactModel.ContactLabel,
                Environment = environment,
                RegistrationUser = userName,
                RegistrationDate = creationTime
            };
            return webApiContact;
        }

        protected WebApiModel.UDSMessageModel MapMessage(UDSMessage messageModel, DateTimeOffset creationTime, string userName, int environment)
        {
            WebApiModel.UDSMessageModel webApiMessage = new WebApiModel.UDSMessageModel
            {
                UniqueId = messageModel.UDSMessageId,
                RelationType = WebApiModel.UDSRelationType.Messages,
                IdUDS = messageModel.UDSId,
                IdMessage = messageModel.IdMessage,
                Environment = environment,
                RegistrationUser = userName,
                RegistrationDate = creationTime
            };
            return webApiMessage;
        }

        protected WebApiModel.UDSPECMailModel MapPECMail(UDSPECMail pecMailModel, DateTimeOffset creationTime, string userName, int environment)
        {
            WebApiModel.UDSPECMailModel webApiPecMail = new WebApiModel.UDSPECMailModel
            {
                UniqueId = pecMailModel.UDSPECMailId,
                RelationType = WebApiModel.UDSRelationType.PECMail,
                IdUDS = pecMailModel.UDSId,
                IdPECMail = pecMailModel.IdPECMail,
                Environment = environment,
                RegistrationUser = userName,
                RegistrationDate = creationTime
            };
            return webApiPecMail;
        }

        protected WebApiModel.UDSDocumentUnitModel MapProtocol(UDSProtocol protocolModel, DateTimeOffset creationTime, string userName, int environment, BuildActionType buildActionType)
        {
            WebApiModel.UDSDocumentUnitModel webApiDocumentUnit = new WebApiModel.UDSDocumentUnitModel
            {
                UniqueId = protocolModel.UDSProtocolId,
                RelationType = buildActionType == BuildActionType.Build ? WebApiModel.UDSRelationType.ArchiveProtocol : WebApiModel.UDSRelationType.ProtocolArchived,
                IdUDS = protocolModel.UDSId,
                IdDocumentUnit = protocolModel.IdProtocol,
                Environment = environment,
                RegistrationUser = userName,
                RegistrationDate = creationTime
            };
            return webApiDocumentUnit;
        }

        protected WebApiModel.UDSDocumentUnitModel MapResolution(UDSResolution resolutionModel, DateTimeOffset creationTime, string userName, int environment)
        {
            WebApiModel.UDSDocumentUnitModel webApiDocumentUnit = new WebApiModel.UDSDocumentUnitModel
            {
                UniqueId = resolutionModel.UDSResolutionId,
                RelationType = WebApiModel.UDSRelationType.Resolution,
                IdUDS = resolutionModel.UDSId,
                IdDocumentUnit = resolutionModel.UniqueIdResolution,
                Environment = environment,
                RegistrationUser = userName,
                RegistrationDate = creationTime
            };
            return webApiDocumentUnit;
        }

        protected WebApiModel.UDSDocumentUnitModel MapUDSDocumentUnits(WebApiEntity.UDSDocumentUnit UDSDocumentUnit, DateTimeOffset creationTime, string userName)
        {
            WebApiModel.UDSDocumentUnitModel webApiDocumentUnit = new WebApiModel.UDSDocumentUnitModel
            {
                UniqueId = UDSDocumentUnit.UniqueId,
                RelationType = WebApiModel.UDSRelationType.UDS,
                IdUDS = UDSDocumentUnit.IdUDS,
                IdDocumentUnit = UDSDocumentUnit.Relation.UniqueId,
                Environment = UDSDocumentUnit.Environment,
                RegistrationUser = userName,
                RegistrationDate = creationTime
            };
            return webApiDocumentUnit;
        }
        #endregion

        protected string PrepareRelations(UDSEntityModel model, Guid udsRepositoryId, int environment, string userName, DateTimeOffset creationTime, BuildActionType buildActionType)
        {
            WebApiModel.UDSRelationModel relationModel = new WebApiModel.UDSRelationModel
            {
                Roles = PrepareRelationModels(model, () => model.Relations?.Authorizations,
                (item) => MapAuthorization(item, creationTime, userName, environment)),

                Users = PrepareRelationModels(model, () => model.Users,
                (item) => MapUser(item, creationTime, userName, environment, model.IdUDS)),

                Collaborations = PrepareRelationModels(model, () => model.Relations?.Collaborations,
                (item) => MapCollaboration(item, creationTime, userName, environment, model.IdUDS)),

                Contacts = PrepareRelationModels(model, () => model.Relations?.Contacts,
                (item) => MapContact(item, creationTime, userName, environment)),

                Messages = PrepareRelationModels(model, () => model.Relations?.Messages,
                (item) => MapMessage(item, creationTime, userName, environment)),

                PECMails = PrepareRelationModels(model, () => model.Relations?.PECMails,
                (item) => MapPECMail(item, creationTime, userName, environment))
            };

            ICollection<WebApiModel.UDSDocumentUnitModel> documentUnits = new List<WebApiModel.UDSDocumentUnitModel>();
            documentUnits = PrepareRelationModels(model, () => model.Relations?.Protocols,
                (item) => MapProtocol(item, creationTime, userName, environment, buildActionType));

            documentUnits = documentUnits.Concat(PrepareRelationModels(model, () => model.Relations?.Resolutions,
                (item) => MapResolution(item, creationTime, userName, environment))).ToList();

            ICollection<WebApiEntity.UDSDocumentUnit> udsDocumentUnits = GetUDSDocumentUnitRelationByID(model.IdUDS).Result;
            documentUnits = documentUnits.Concat(PrepareRelationModels(model, () => udsDocumentUnits,
                         (item) => MapUDSDocumentUnits(item, creationTime, userName))).ToList();

            relationModel.DocumentUnits = documentUnits;

            return JsonConvert.SerializeObject(relationModel);
        }

        private ICollection<TResult> PrepareRelationModels<T, TResult>(UDSEntityModel model,
            Func<IEnumerable<T>> getSourceFunc, Func<T, TResult> mappingFunc)
        {
            ICollection<TResult> results = new List<TResult>();
            IEnumerable<T> sources = getSourceFunc();
            if (sources != null)
            {
                foreach (T source in sources)
                {
                    results.Add(mappingFunc(source));
                }
            }
            return results;
        }


        #endregion
    }
}