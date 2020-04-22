using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Fascicles;
using VecompSoftware.Core.Command.CQRS.Commands.Models.PECMails;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Protocols;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Core.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Workflows;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuite.Document.Generator.OpenXml.Word;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Workflow;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Client;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using MessageType = VecompSoftware.DocSuiteWeb.Entity.Messages.MessageType;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using StorageDocument = VecompSoftware.DocSuite.Document;
using WorkflowActivityType = VecompSoftware.DocSuiteWeb.Entity.Workflows.WorkflowActivityType;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    [LogCategory(LogCategoryDefinition.SERVICEWF)]
    public abstract class WorkflowBaseService<TWorkflowEntity> : IWorkflowBaseService<TWorkflowEntity>
        where TWorkflowEntity : class, new()
    {
        #region [ Fields ]
        protected WorkflowClientConfiguration _workflowClientConfiguration;
        private readonly IdentityContext _identityContext;
        private readonly DomainUserModel _currentDomainUser;

        public const string WORKFLOW_STATUS_DONE = "WorkflowStatusDone";
        public const string WORKFLOW_STATUS_ERROR = "WorkflowStatusError";
        public const string WORKFLOW_NOTIFICATION_INFO = "WorkflowNotificationInfo";
        public const string WORKFLOW_NOTIFICATION_INFO_MODEL = "WorkflowNotificationInfoModel";
        public const string WORKFLOW_NOTIFICATION_WARNING = "WorkflowNotificationWarning";
        public const string WORKFLOW_NOTIFICATION_ERROR = "WorkflowNotificationError";
        public static readonly string TYPE_WORKFLOW_STATUS_DONE = typeof(EventWorkflowStartRequestDone).Name;
        public static readonly string TYPE_WORKFLOW_STATUS_ERROR = typeof(EventWorkflowStartRequestError).Name;
        public static readonly string TYPE_WORKFLOW_NOTIFICATION_INFO = typeof(EventWorkflowNotificationInfo).Name;
        public static readonly string TYPE_WORKFLOW_NOTIFICATION_INFO_AS_MODEL = typeof(EventWorkflowNotificationInfoAsModel).Name;
        public static readonly string TYPE_WORKFLOW_NOTIFICATION_WARNING = typeof(EventWorkflowNotificationWarning).Name;
        public static readonly string TYPE_WORKFLOW_NOTIFICATION_ERROR = typeof(EventWorkflowNotificationError).Name;

        protected readonly Guid _instanceId;
        protected static IEnumerable<LogCategory> _logCategories = null;

        protected readonly ILogger _logger;
        private readonly IWorkflowActivityService _workflowActivityService;
        private readonly IWorkflowInstanceService _workflowInstanceService;
        private readonly IWorkflowInstanceRoleService _workflowInstanceRoleService;
        private readonly ISecurity _security;
        private readonly ICollaborationService _collaborationService;
        private readonly IProtocolLogService _protocolLogService;
        private readonly IFascicleRoleService _fascicleRoleService;
        private readonly IMessageService _messageService;
        private readonly IDossierRoleService _dossierRoleService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ITopicService _topicService;
        private readonly IQueueService _queueService;
        private readonly IParameterEnvService _parameterEnvService;
        private readonly IMessageConfiguration _messageConfiguration;
        private readonly StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        private readonly IWordOpenXmlDocumentGenerator _wordOpenXmlDocumentGenerator;
        private readonly IPDFDocumentGenerator _pdfDocumentGenerator;
        private readonly ICQRSMessageMapper _mapper_cqrsMessageMapper;

        private readonly IDictionary<string, ServiceBusMessageConfiguration> _messageMappings;
        protected readonly ITranslationErrorMapper _mapper_to_translation_error;
        #endregion

        #region [ Properties ]   
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowJsonStartService));
                }
                return _logCategories;
            }
        }

        protected DomainUserModel CurrentDomainUser => _currentDomainUser;

        public string WorkflowConfig { get; set; }

        public WorkflowClientConfiguration ClientConfig
        {
            get
            {
                if (_workflowClientConfiguration == null)
                {
                    _workflowClientConfiguration = JsonConvert.DeserializeObject<WorkflowClientConfiguration>(WorkflowConfig);
                }
                return _workflowClientConfiguration;
            }
        }

        public IdentityContext CurrentIdentityContext => _identityContext;
        #endregion

        #region [ Constructor ]
        public WorkflowBaseService(ILogger logger, IWorkflowInstanceService workflowInstanceService,
            IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
            ITopicService topicServiceBus, ITranslationErrorMapper mapper_to_translation_error, ICQRSMessageMapper mapper_eventServiceBusMessage,
            IDataUnitOfWork unitOfWork, StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService,
            ICollaborationService collaborationService, ISecurity security, IParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService, 
            IMessageService messageService, IDossierRoleService dossierRoleService, IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator,
            IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService, IPDFDocumentGenerator pdfDocumentGenerator)
        {
            _instanceId = Guid.NewGuid();
            _workflowClientConfiguration = null;
            _logger = logger;
            _mapper_to_translation_error = mapper_to_translation_error;
            _mapper_cqrsMessageMapper = mapper_eventServiceBusMessage;
            _workflowActivityService = workflowActivityService;
            _workflowInstanceService = workflowInstanceService;
            _workflowInstanceRoleService = workflowInstanceRoleService;
            _topicService = topicServiceBus;
            _queueService = queueService;
            _unitOfWork = unitOfWork;
            _security = security;
            _documentService = documentService;
            _collaborationService = collaborationService;
            _protocolLogService = protocolLogService;
            _fascicleRoleService = fascicleRoleService;
            _messageService = messageService;
            _dossierRoleService = dossierRoleService;
            _currentDomainUser = security.GetCurrentUser();
            _wordOpenXmlDocumentGenerator = wordOpenXmlDocumentGenerator;
            _pdfDocumentGenerator = pdfDocumentGenerator;
            _identityContext = new IdentityContext(_currentDomainUser.Account);
            _parameterEnvService = parameterEnvService;
            _messageConfiguration = messageConfiguration;
            _messageMappings = _messageConfiguration.GetConfigurations();
        }
        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region [ Methos ]

        public async Task<WorkflowResult> CreateAsync(TWorkflowEntity content)
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                return await BeforeCreateAsync(content);
            }, _logger, LogCategories);
        }

        public WorkflowResult Create(TWorkflowEntity content)
        {
            return CreateAsync(content).Result;
        }

        protected bool IsDocumentUnit(WorkflowReferenceModel workflowReferenceModel)
        {
            if (workflowReferenceModel == null || string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel))
            {
                return false;
            }
            return workflowReferenceModel.ReferenceType == DSWEnvironmentType.Protocol || workflowReferenceModel.ReferenceType == DSWEnvironmentType.DocumentSeries ||
                    workflowReferenceModel.ReferenceType == DSWEnvironmentType.Resolution || workflowReferenceModel.ReferenceType == DSWEnvironmentType.UDS;
        }

        protected bool IsRoleAuthorization(WorkflowStep currentStep)
        {
            switch (currentStep.AuthorizationType)
            {
                case Model.Workflow.WorkflowAuthorizationType.AllRoleUser:
                case Model.Workflow.WorkflowAuthorizationType.AllSecretary:
                case Model.Workflow.WorkflowAuthorizationType.AllSigner:
                case Model.Workflow.WorkflowAuthorizationType.AllManager:
                case Model.Workflow.WorkflowAuthorizationType.AllOChartRoleUser:
                case Model.Workflow.WorkflowAuthorizationType.AllOChartManager:
                case Model.Workflow.WorkflowAuthorizationType.AllOChartHierarchyManager:
                case Model.Workflow.WorkflowAuthorizationType.AllDematerialisationManager:
                    {
                        return true;
                    }
                case Model.Workflow.WorkflowAuthorizationType.UserName:
                case Model.Workflow.WorkflowAuthorizationType.ADGroup:
                case Model.Workflow.WorkflowAuthorizationType.MappingTags:
                    {
                        return false;
                    }
            }
            return false;
        }

        protected WorkflowActivity MappingAuhtorizations<T>(WorkflowActivity workflowActivity, IEnumerable<T> results, Func<T, string> getFullAccountName)
            where T : DSWBaseEntity
        {
            if (results == null || !results.Any())
            {
                throw new DSWException(string.Concat("Workflow Authorization decoration Failed : ", typeof(T).Name, " not found"), null, DSWExceptionCode.SS_RulesetValidation);
            }
            foreach (T item in results)
            {
                string fullAccountName = getFullAccountName(item);
                if (!workflowActivity.WorkflowAuthorizations.Any(f => f.Account.Equals(fullAccountName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _logger.WriteDebug(new LogMessage($"Security user {item.UniqueId} {fullAccountName}"), LogCategories);
                    workflowActivity.WorkflowAuthorizations.Add(new WorkflowAuthorization()
                    {
                        Account = fullAccountName
                    });
                }
            }
            return workflowActivity;
        }

        protected WorkflowActivity DecorateAuthorizationActivity(WorkflowActivity workflowActivity, WorkflowStep currentStep)
        {
            if (workflowActivity == null || currentStep == null || currentStep.AuthorizationType == Model.Workflow.WorkflowAuthorizationType.None)
            {
                return workflowActivity;
            }
            WorkflowProperty dsw_p_RoleId = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLE_ID);
            WorkflowProperty dsw_p_Roles = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLES);
            WorkflowProperty dsw_p_Accounts = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS);

            List<WorkflowMapping> workflowMappings = null;
            if (dsw_p_Roles != null && !string.IsNullOrEmpty(dsw_p_Roles.ValueString) && workflowActivity.ActivityType != DocSuiteWeb.Entity.Workflows.WorkflowActivityType.CollaborationSign)
            {
                _logger.WriteDebug(new LogMessage("DecorateAuthorizationActivity DSW_PROPERTY_ROLES"), LogCategories);
                workflowMappings = JsonConvert.DeserializeObject<List<WorkflowMapping>>(dsw_p_Roles.ValueString, ServiceHelper.SerializerSettings);
            }
            if (workflowMappings == null && dsw_p_RoleId != null && dsw_p_RoleId.ValueInt.HasValue)
            {
                _logger.WriteDebug(new LogMessage($"DecorateAuthorizationActivity DSW_PROPERTY_ROLE_ID {dsw_p_RoleId.ValueInt}"), LogCategories);
                workflowMappings = new List<WorkflowMapping>()
                {
                    new WorkflowMapping()
                    {
                        AuthorizationType = currentStep.AuthorizationType,
                        MappingTag = string.Empty,
                        Role = new WorkflowRole() { IdRole = (short)dsw_p_RoleId.ValueInt.Value }
                    }
                };
            }
            if (workflowMappings == null && dsw_p_Accounts != null && !string.IsNullOrEmpty(dsw_p_Accounts.ValueString))
            {
                _logger.WriteDebug(new LogMessage("DecorateAuthorizationActivity DSW_PROPERTY_ACCOUNTS"), LogCategories);
                ICollection<WorkflowAccount> workflowAccounts = JsonConvert.DeserializeObject<ICollection<WorkflowAccount>>(dsw_p_Accounts.ValueString);
                workflowMappings = new List<WorkflowMapping>();
                workflowMappings.AddRange(workflowAccounts.Select(f => new WorkflowMapping()
                {
                    AuthorizationType = currentStep.AuthorizationType,
                    MappingTag = string.Empty,
                    Account = f
                }));
            }

            foreach (WorkflowMapping workflowMapping in workflowMappings ?? new List<WorkflowMapping>())
            {
                switch (workflowMapping.AuthorizationType)
                {
                    case Model.Workflow.WorkflowAuthorizationType.AllRoleUser:
                        {
                            _logger.WriteDebug(new LogMessage($"Model.Workflow.WorkflowAuthorizationType.AllRoleUser {workflowMapping.Role.IdRole}"), LogCategories);
                            IEnumerable<SecurityUser> securityUsers = _unitOfWork.Repository<RoleGroup>().GetRoleGroupsAllAuthorizationType(workflowMapping.Role.IdRole);
                            _logger.WriteDebug(new LogMessage($"Founded {securityUsers.Count()} security users"), LogCategories);
                            workflowActivity = MappingAuhtorizations(workflowActivity, securityUsers, x => string.Concat(x.UserDomain, "\\", x.Account));
                            break;
                        }
                    case Model.Workflow.WorkflowAuthorizationType.AllSecretary:
                        {
                            _logger.WriteDebug(new LogMessage("Model.Workflow.WorkflowAuthorizationType.AllSecretary"), LogCategories);
                            IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Secretary, workflowMapping.Role.IdRole,
                                DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol);

                            workflowActivity = MappingAuhtorizations(workflowActivity, roleUsers, (x => x.Account));
                            break;
                        }
                    case Model.Workflow.WorkflowAuthorizationType.AllSigner:
                        {
                            _logger.WriteDebug(new LogMessage("Model.Workflow.WorkflowAuthorizationType.AllSigner"), LogCategories);
                            IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(new List<string>() { RoleUserType.Vice, RoleUserType.Manager },
                                workflowMapping.Role.IdRole, DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol);

                            workflowActivity = MappingAuhtorizations(workflowActivity, roleUsers, (x => x.Account));
                            break;
                        }
                    case Model.Workflow.WorkflowAuthorizationType.AllManager:
                        {
                            _logger.WriteDebug(new LogMessage("Model.Workflow.WorkflowAuthorizationType.AllManager"), LogCategories);

                            IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Manager, workflowMapping.Role.IdRole,
                                DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol);

                            workflowActivity = MappingAuhtorizations(workflowActivity, roleUsers, (x => x.Account));
                            break;
                        }
                    case Model.Workflow.WorkflowAuthorizationType.UserName:
                        {
                            _logger.WriteDebug(new LogMessage("Model.Workflow.WorkflowAuthorizationType.UserName"), LogCategories);
                            workflowActivity.WorkflowAuthorizations.Add(new WorkflowAuthorization()
                            {
                                Account = workflowMapping.Account.AccountName
                            });
                            break;
                        }
                    case Model.Workflow.WorkflowAuthorizationType.AllOChartRoleUser:
                        throw new DSWException("Workflow Authorization AllOChartRoleUser: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    case Model.Workflow.WorkflowAuthorizationType.AllOChartManager:
                        throw new DSWException("Workflow Authorization AllOChartManager: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    case Model.Workflow.WorkflowAuthorizationType.AllOChartHierarchyManager:
                        throw new DSWException("Workflow Authorization AllOChartHierarchyManager: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    case Model.Workflow.WorkflowAuthorizationType.ADGroup:
                        throw new DSWException("Workflow Authorization ADGroup: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    default:
                        throw new DSWException("Workflow Authorization AuthorizationType argument not defined", null, DSWExceptionCode.SS_RulesetValidation);
                }
            }
            if (!workflowActivity.WorkflowAuthorizations.Any())
            {
                throw new DSWException("Workflow engine has not been found valid accounts to authorize activity", null, DSWExceptionCode.SS_RulesetValidation);
            }
            return workflowActivity;
        }

        private async Task<IDictionary<int, WorkflowStep>> CreateCollaborationActivityAsync(CollaborationModel collaborationModel, WorkflowProperty dsw_e_CollaborationManaged,
            WorkflowInstance workflowInstance, IDictionary<int, WorkflowStep> workflowSteps, int currentStepNumber, List<WorkflowMapping> workflowMappingTags)
        {
            int localCurrentStepNumber = currentStepNumber;
            _logger.WriteDebug(new LogMessage("CreateCollaborationActivityAsync"), LogCategories);

            #region Validations
            if (collaborationModel == null)
            {
                throw new DSWException("CollaborationModel is not valid", null, DSWExceptionCode.WF_Mapper);
            }
            if (workflowInstance == null)
            {
                throw new DSWException("WorkflowInstance is not valid", null, DSWExceptionCode.WF_Mapper);
            }
            short collaborationLocationId = _parameterEnvService.CollaborationLocationId;
            Location collaborationLocation = _unitOfWork.Repository<Location>().Find(collaborationLocationId);
            if (collaborationLocation == null)
            {
                throw new DSWException($"Collaboration Location {collaborationLocationId} not found", null, DSWExceptionCode.WF_Mapper);
            }
            if (!collaborationModel.CollaborationSigns.Any())
            {
                throw new DSWValidationException("Evaluate create collaboration activity validation error", new List<ValidationMessageModel>()
                {
                    new ValidationMessageModel
                    {
                        Key = "CollaborationSigns",
                        Message = $"Impossibile avviare la collaborazione senza aver specificato almeno un firmatario"
                    }
                },
                null, DSWExceptionCode.VA_RulesetValidation);
            }
            if (!collaborationModel.CollaborationUsers.Any() && !workflowMappingTags.Any())
            {
                throw new DSWValidationException("Evaluate create collaboration activity validation error", new List<ValidationMessageModel>()
                {
                    new ValidationMessageModel
                    {
                        Key = "CollaborationUsers",
                        Message = $"Impossibile avviare la collaborazione senza aver specificato almeno una segreteria"
                    }
                },
                null, DSWExceptionCode.VA_RulesetValidation);
            }
            if (!collaborationModel.CollaborationVersionings.Any())
            {
                throw new DSWValidationException("Evaluate create collaboration activity validation error", new List<ValidationMessageModel>()
                {
                    new ValidationMessageModel
                    {
                        Key = "CollaborationVersionings",
                        Message = $"Impossibile avviare la collaborazione senza aver specificato almeno un documento"
                    }
                },
                null, DSWExceptionCode.VA_RulesetValidation);
            }
            #endregion

            #region Decorate Collaboration base properties
            Collaboration collaboration = new Collaboration()
            {
                DocumentType = "W",
                IdPriority = collaborationModel.IdPriority,
                IdStatus = collaborationModel.IdStatus,
                SignCount = collaborationModel.SignCount,
                Subject = collaborationModel.Subject,
                Location = collaborationLocation,
                TemplateName = workflowInstance.WorkflowRepository?.Name ?? "Workflow",
                RegistrationDate = DateTimeOffset.UtcNow,
                RegistrationUser = collaborationModel.RegistrationUser,
                RegistrationName = collaborationModel.RegistrationName
            };
            #endregion

            #region Collaboration Signs
            foreach (CollaborationSignModel item in collaborationModel.CollaborationSigns.OrderBy(f => f.Incremental))
            {
                collaboration.CollaborationSigns.Add(new CollaborationSign()
                {
                    Incremental = item.Incremental,
                    IsActive = item.Incremental == 1,
                    IsRequired = item.IsRequired,
                    SignUser = item.SignUser,
                    SignName = item.SignName,
                    SignEmail = item.SignEmail,
                    IdStatus = CollaborationStatusType.ToRead
                });

                #region CollaborationSign Step
                workflowSteps.Add(workflowSteps.Count, new WorkflowStep()
                {
                    ActivityOperation = new WorkflowActivityOperation() 
                    { 
                        Action = Model.Workflow.WorkflowActivityAction.ToSign, 
                        Area = Model.Workflow.WorkflowActivityArea.Collaboration 
                    },
                    ActivityType = Model.Workflow.WorkflowActivityType.CollaborationSign,
                    AuthorizationType = Model.Workflow.WorkflowAuthorizationType.UserName,
                    Name = $"Documento '{collaborationModel.Subject}' in firma a {item.SignName}",
                    Position = ++localCurrentStepNumber,
                    InputArguments = new List<WorkflowArgument>()
                    {
                        new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, PropertyType = ArgumentType.PropertyInt },
                        new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, PropertyType = ArgumentType.Json },
                        new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, PropertyType = ArgumentType.PropertyString },
                        new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, PropertyType = ArgumentType.PropertyInt }
                    },
                    OutputArguments = new List<WorkflowArgument>()
                    {
                        new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, PropertyType = ArgumentType.Json },
                        new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, PropertyType = ArgumentType.PropertyInt },
                        new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, PropertyType = ArgumentType.PropertyInt },
                    }
                });
                #endregion
            }
            #endregion

            #region Collaboration Users
            if (!collaborationModel.CollaborationUsers.Any())
            {
                DomainUserModel domainUserModel = null;
                string destinationName = string.Empty;
                string destinationEmail = string.Empty;
                string destinationType = string.Empty;
                short incremental = 1;
                short? idRole = null;
                foreach (WorkflowMapping workflowMapping in workflowMappingTags)
                {
                    domainUserModel = null;
                    destinationName = string.IsNullOrEmpty(workflowMapping.Account.DisplayName) ? workflowMapping.Account.AccountName : workflowMapping.Account.DisplayName;
                    destinationEmail = workflowMapping.Account.EmailAddress;
                    destinationType = CollaborationDestinationType.P.ToString();
                    idRole = null;

                    if (!string.IsNullOrEmpty(workflowMapping.Account.AccountName))
                    {
                        domainUserModel = _security.GetUser(workflowMapping.Account.AccountName);
                        destinationName = domainUserModel.DisplayName;
                        destinationEmail = domainUserModel.EmailAddress;
                    }

                    if (workflowMapping.Role != null)
                    {
                        idRole = workflowMapping.Role.IdRole;
                        destinationName = workflowMapping.Role.Name;
                        destinationEmail = workflowMapping.Role.EmailAddress;
                        destinationType = CollaborationDestinationType.S.ToString();
                    }

                    collaborationModel.CollaborationUsers.Add(new CollaborationUserModel()
                    {
                        Account = workflowMapping.Account.AccountName,
                        DestinationFirst = true,
                        DestinationEmail = destinationEmail,
                        DestinationName = destinationName,
                        DestinationType = destinationType,
                        Incremental = incremental++,
                        IdRole = idRole,
                    });
                }
            }

            foreach (CollaborationUserModel item in collaborationModel.CollaborationUsers.OrderBy(f => f.Incremental.Value))
            {
                collaboration.CollaborationUsers.Add(new CollaborationUser()
                {
                    Incremental = item.Incremental.Value,
                    DestinationType = item.DestinationType,
                    DestinationFirst = item.DestinationFirst,
                    Account = item.Account,
                    Role = item.IdRole.HasValue ? new Role() { EntityShortId = item.IdRole.Value } : null,
                    DestinationName = item.DestinationName,
                    DestinationEmail = item.DestinationEmail
                });
            }
            #endregion

            #region Collaboration Versionings
            foreach (CollaborationVersioningModel item in collaborationModel.CollaborationVersionings
                .OrderBy(f => f.CollaborationIncremental)
                .ThenBy(f => f.Incremental))
            {
                ModelDocument.ArchiveDocument archiveDocument = new ModelDocument.ArchiveDocument
                {
                    Archive = collaborationLocation.ProtocolArchive,
                    ContentStream = item.DocumentContent,
                    Name = item.DocumentName
                };
                archiveDocument = await _documentService.InsertDocumentAsync(archiveDocument);
                item.DocumentContent = null;
                collaboration.CollaborationVersionings.Add(new CollaborationVersioning()
                {
                    Incremental = item.Incremental,
                    CollaborationIncremental = item.CollaborationIncremental,
                    DocumentName = item.DocumentName,
                    IdDocument = archiveDocument.IdLegacyChain,
                    DocumentGroup = item.DocumentGroup,
                    IsActive = item.IsActive.Value,
                });
            }
            #endregion

            #region CollaborationToProtocol Step 
            WorkflowActivityOperation workflowActivityOperation = new WorkflowActivityOperation() 
            { 
                Action = Model.Workflow.WorkflowActivityAction.ToProtocol, 
                Area = Model.Workflow.WorkflowActivityArea.Collaboration 
            };
            Model.Workflow.WorkflowActivityType activityType = Model.Workflow.WorkflowActivityType.CollaborationToProtocol;
            Model.Workflow.WorkflowAuthorizationType workflowAuthorizationType = Model.Workflow.WorkflowAuthorizationType.AllSecretary;
            string name = $"Documento '{collaborationModel.Subject}' da protocollare";

            if (dsw_e_CollaborationManaged != null && dsw_e_CollaborationManaged.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_CollaborationManaged.ValueString))
            {
                _logger.WriteDebug(new LogMessage("Found dsw_e_CollaborationManaged property"), LogCategories);
                workflowActivityOperation = JsonConvert.DeserializeObject<WorkflowActivityOperation>(dsw_e_CollaborationManaged.ValueString);
                switch (workflowActivityOperation.Action)
                {
                    case Model.Workflow.WorkflowActivityAction.ToAssignment:
                        {
                            activityType = Model.Workflow.WorkflowActivityType.DematerialisationStatement;
                            name = $"Attestazione di conformità : '{collaborationModel.Subject}'";
                            break;
                        }
                    case Model.Workflow.WorkflowActivityAction.ToSecure:
                        {
                            activityType = Model.Workflow.WorkflowActivityType.SecureDocumentCreate;
                            name = $"Securizzazione documento : '{collaborationModel.Subject}'";
                            break;
                        }
                    default:
                        {
                            activityType = Model.Workflow.WorkflowActivityType.CollaborationToProtocol;
                            break;
                        }
                }
            }

            _logger.WriteDebug(new LogMessage($"Set collaboration managed to {workflowActivityOperation.Action.ToString()}"), LogCategories);

            workflowSteps.Add(workflowSteps.Count, new WorkflowStep()
            {
                ActivityOperation = workflowActivityOperation,
                ActivityType = activityType,
                AuthorizationType = workflowAuthorizationType,
                Name = name,
                Position = ++localCurrentStepNumber,
                InputArguments = new List<WorkflowArgument>()
                {
                    new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, PropertyType = ArgumentType.PropertyInt },
                    new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, PropertyType = ArgumentType.Json },
                    new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_ROLES, PropertyType = ArgumentType.Json }
                },
                OutputArguments = new List<WorkflowArgument>()
                {
                    new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR, PropertyType = ArgumentType.PropertyInt },
                    new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER, PropertyType = ArgumentType.PropertyInt }                    ,
                    new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_CHAINID_MAIN, PropertyType = ArgumentType.PropertyGuid }
                }
            });
            #endregion

            collaboration.WorkflowInstance = workflowInstance;
            collaboration = await _collaborationService.CreateAsync(collaboration);

            collaborationModel.IdCollaboration = collaboration.EntityId;
            _logger.WriteDebug(new LogMessage(string.Concat("Collaboration ", collaboration.EntityId, " [", collaboration.UniqueId, "] has been successfully created")), LogCategories);
            return workflowSteps;
        }

        private ContactBagDraftModel DecorateContactBagDraft(IEnumerable<ProtocolContactManualModel> protocolContactManualModels)
        {
            ContactBagDraftModel contactBagDraftModel = new ContactBagDraftModel();
            foreach (ProtocolContactManualModel protocolContactManualModel in protocolContactManualModels)
            {
                contactBagDraftModel.Contacts.Add(new ContactDraftModel()
                {
                    Description = protocolContactManualModel.Description,
                    CertifiedMail = protocolContactManualModel.CertifiedEmail,
                    StandardMail = protocolContactManualModel.EMail,
                    Type = "A",
                    Address = string.IsNullOrEmpty(protocolContactManualModel.Address) ? null : new ContactAddressDraftModel() { Name = protocolContactManualModel.Address }
                });
            }

            return contactBagDraftModel;
        }

        private async Task<Message> CreateMessageAsync(Guid uniqueId, string messageBody, string messageSubject, string[] recipientEmails
            , string senderEmail = "", MessageContactType contactType = MessageContactType.User, int priority = 0, string messageLink = "")
        {
            IList<MessageContact> recipients = new List<MessageContact>();
            if (recipientEmails == null || recipientEmails.All(f => string.IsNullOrEmpty(f)))
            {
                _logger.WriteWarning(new LogMessage($"Email can't be generated: recipients has not been defined in workflow InstanceId {uniqueId}"), LogCategories);
                return null;
            }
            if (string.IsNullOrEmpty(senderEmail))
            {
                _logger.WriteWarning(new LogMessage($"Email sender has not been defined in workflow InstanceId {uniqueId}"), LogCategories);
            }

            MessageContactEmail contactEmail;
            MessageContact contact;

            #region Sender
            if (!string.IsNullOrEmpty(senderEmail))
            {
                contactEmail = new MessageContactEmail()
                {
                    Description = senderEmail,
                    Email = senderEmail,
                    User = CurrentIdentityContext.User
                };

                contact = new MessageContact()
                {
                    ContactPosition = MessageContactPosition.Sender,
                    ContactType = contactType,
                    Description = senderEmail,
                    MessageContactEmail = new List<MessageContactEmail>() { contactEmail },
                };
                recipients.Add(contact);
            }

            #endregion

            #region Recipients
            foreach (string recipient in recipientEmails.Where(f => !string.IsNullOrEmpty(f)))
            {
                contactEmail = new MessageContactEmail()
                {
                    Description = recipient,
                    Email = recipient,
                    User = CurrentIdentityContext.User
                };
                contact = new MessageContact()
                {
                    ContactPosition = MessageContactPosition.Recipient,
                    ContactType = contactType,
                    Description = recipient,
                    MessageContactEmail = new List<MessageContactEmail>() { contactEmail },
                };
                recipients.Add(contact);
            }

            #endregion

            #region Message
            string currentPriority = MessagePriorityType.Normal;
            switch (priority)
            {
                case 0:
                    {
                        currentPriority = MessagePriorityType.Normal;
                        break;
                    }
                case 1:
                    {
                        currentPriority = MessagePriorityType.Low;
                        break;
                    }
                case 2:
                    {
                        currentPriority = MessagePriorityType.High;
                        break;
                    }
                default:
                    break;
            }
            MessageEmail email = new MessageEmail()
            {
                Body = string.Concat(messageBody, messageLink),
                Subject = messageSubject,
                Priority = currentPriority
            };

            Message message = new Message()
            {
                MessageType = MessageType.Email,
                MessageContacts = recipients,
                Status = MessageStatus.Active,
                MessageEmails = new List<MessageEmail>() { email }
            };

            #endregion

            message = await _messageService.CreateAsync(message);
            _logger.WriteInfo(new LogMessage(string.Concat("Email has been correctly inserted into the processing queue with id ", message.UniqueId, " to uniqueId ", uniqueId)), LogCategories);

            return message;
        }

        private async Task<TModel> CompleteBuildActivity<TModel>(WorkflowReferenceModel workflowReferenceModel, Guid workflowInstanceId, Guid idWorkflowActivity,
            Func<TModel, ICommand> func)
            where TModel : IWorkflowContentBase
        {
            TModel model = JsonConvert.DeserializeObject<TModel>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
            model.IdWorkflowActivity = idWorkflowActivity;
            ICommand command = func(model);
            ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(command, new ServiceBusMessage());
            if (string.IsNullOrEmpty(message.ChannelName))
            {
                throw new DSWException($"Queue name to command [{command.ToString()}] is not mapped", null, DSWExceptionCode.SC_Mapper);
            }
            try
            {
                message = await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"{command.ToString()} has not been sended in workflow InstanceId {workflowInstanceId}"), ex, LogCategories);
                throw;
            }
            return model;
        }

        private async Task<TModel> CompleteBuildEvent<TModel>(WorkflowReferenceModel workflowReferenceModel, Guid workflowInstanceId, Guid idWorkflowActivity,
            Func<TModel, IEvent> func)
            where TModel : IWorkflowContentBase
        {
            TModel model = JsonConvert.DeserializeObject<TModel>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
            model.IdWorkflowActivity = idWorkflowActivity;
            IEvent evt = func(model);
            ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(evt, new ServiceBusMessage());
            if (string.IsNullOrEmpty(message.ChannelName))
            {
                throw new DSWException($"Topic name to command [{evt.ToString()}] is not mapped", null, DSWExceptionCode.SC_Mapper);
            }
            try
            {
                message = await _topicService.SendToTopicAsync(message);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"{evt.ToString()} has not been sended in workflow InstanceId {workflowInstanceId}"), ex, LogCategories);
                throw;
            }
            return model;
        }

        private async Task<WorkflowActivity> CompleteWorkflowInstanceAsync(WorkflowInstance workflowInstance, IDictionary<int, WorkflowStep> workflowSteps, int currentStepNumber)
        {
            workflowInstance.Status = WorkflowStatus.Done;
            if (!string.IsNullOrEmpty(workflowInstance.RegistrationUser))
            {
                WorkflowRepository workflowRepository = workflowInstance.WorkflowRepository;
                workflowInstance = await _workflowInstanceService.UpdateAsync(workflowInstance);
                workflowInstance.WorkflowRepository = workflowRepository;
            }
            WorkflowProperty dsw_p_TenantId = workflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
            WorkflowProperty dsw_p_TenantName = workflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
            _logger.WriteInfo(new LogMessage(string.Concat("WorkflowInstance ", workflowInstance.WorkflowRepository.Name,
                " [", workflowInstance.InstanceId, "] in Tenant [", dsw_p_TenantName.ValueString, "/", dsw_p_TenantId.ValueGuid, "] completed.")),
                LogCategories);

            IEventCompleteWorkflowInstance evt = new EventCompleteWorkflowInstance(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value,
               _identityContext, workflowInstance);
            ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(evt, new ServiceBusMessage());
            ServiceBusMessage response = await _topicService.SendToTopicAsync(message);

            _logger.WriteInfo(new LogMessage(string.Concat("WorkflowInstance ", workflowInstance.WorkflowRepository.Name,
               " [", workflowInstance.InstanceId, "] in Tenant [", dsw_p_TenantName.ValueString, "/", dsw_p_TenantId.ValueGuid, "] sended notification [", response.MessageId, "].")),
               LogCategories);

            //TODO: remove this really  bad code!!!!! We need use notification pattern like used in InvoiceWorkflows
            if (workflowSteps.Count - 1 >= 0 && workflowSteps[currentStepNumber - 1].ActivityType == Model.Workflow.WorkflowActivityType.DematerialisationStatement)
            {
                return new WorkflowActivity()
                {
                    ActivityType = DocSuiteWeb.Entity.Workflows.WorkflowActivityType.DematerialisationStatement,
                    ActivityAction = DocSuiteWeb.Entity.Workflows.WorkflowActivityAction.ToSecure,
                    ActivityArea = DocSuiteWeb.Entity.Workflows.WorkflowActivityArea.Build
                };
            }
            if (workflowSteps.Count - 1 >= 0 && workflowSteps[currentStepNumber - 1].ActivityType == Model.Workflow.WorkflowActivityType.SecureDocumentCreate)
            {
                return new WorkflowActivity()
                {
                    ActivityType = DocSuiteWeb.Entity.Workflows.WorkflowActivityType.SecureDocumentCreate,
                    ActivityAction = DocSuiteWeb.Entity.Workflows.WorkflowActivityAction.ToSecure,
                    ActivityArea = DocSuiteWeb.Entity.Workflows.WorkflowActivityArea.Build
                };
            }
            return null;
        }

        private List<WorkflowMapping> EvaluateMappingTags(WorkflowRepository workflowRepository, IEnumerable<WorkflowProperty> inputArguments, List<WorkflowMapping> workflowMappingTags)
        {
            if (inputArguments.Any(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG))
            {
                WorkflowProperty dsw_p_MappingTag = inputArguments.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MAPPING_TAG);
                WorkflowProperty dsw_p_Accounts = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS);

                if (dsw_p_Accounts == null || string.IsNullOrEmpty(dsw_p_Accounts.ValueString) || string.IsNullOrWhiteSpace(dsw_p_Accounts.ValueString))
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("SET MAPPING TAG: ", dsw_p_MappingTag.ValueString)), LogCategories);
                    string mappingTag = dsw_p_MappingTag.ValueString.ToLower();
                    IEnumerable<WorkflowRoleMapping> workflowRoleMappings = _unitOfWork.Repository<WorkflowRoleMapping>().GetByMappingTag(mappingTag, workflowRepository.UniqueId);

                    if (!workflowRoleMappings.Any())
                    {
                        throw new DSWValidationException("Evaluate mapping tags validation error",new List<ValidationMessageModel>() 
                        { 
                            new ValidationMessageModel 
                            { 
                                Key = "WorkflowRoleMapping", 
                                Message = $"Impossibile avviare il workflow {workflowRepository.Name} in quanto il mapping tag {mappingTag} non è stato definito" 
                            }
                        },
                        null, DSWExceptionCode.VA_RulesetValidation);
                    }
                    workflowMappingTags = workflowRoleMappings.Select(f => new WorkflowMapping()
                    {
                        AuthorizationType = (Model.Workflow.WorkflowAuthorizationType)f.AuthorizationType,
                        MappingTag = f.MappingTag,
                        Role = new WorkflowRole()
                        {
                            IdRole = f.Role.IdRoleTenant,
                            EmailAddress = f.Role.EMailAddress,
                            Name = f.Role.Name,
                            UniqueId = f.Role.UniqueId
                        }
                    }).ToList();
                }
                else
                {
                    ICollection<WorkflowAccount> workflowAccounts = JsonConvert.DeserializeObject<ICollection<WorkflowAccount>>(dsw_p_Accounts.ValueString);
                    if (workflowAccounts != null && workflowAccounts.Count > 0)
                    {
                        _logger.WriteDebug(new LogMessage(string.Concat("MAPPING TAG: ", dsw_p_MappingTag.ValueString, " WAS IGNORED BECAUSE FOUND SPECIFIC ACCOUNT NAME")), LogCategories);
                        workflowMappingTags.Add(new WorkflowMapping()
                        {
                            AuthorizationType = Model.Workflow.WorkflowAuthorizationType.UserName,
                            MappingTag = dsw_p_MappingTag.Name,
                            Account = workflowAccounts.First()

                        });
                    }
                }
            }

            return workflowMappingTags;
        }

        private async Task EvaluateBuildAreaAsync(WorkflowInstance workflowInstance, Guid workflowInstanceId, Guid idWorkflowActivity, WorkflowStep currentStep,
            WorkflowProperty dsw_p_ReferenceModel, WorkflowProperty dsw_p_Model, WorkflowProperty dsw_e_UDSId, WorkflowProperty dsw_e_UDSRepositoryId,
            WorkflowProperty dsw_p_Roles, WorkflowProperty dsw_a_SetRecipientResponsible, WorkflowReferenceModel workflowReferenceModel)
        {
            if (currentStep.ActivityOperation.Area == Model.Workflow.WorkflowActivityArea.Build)
            {
                if (!workflowInstance.InstanceId.HasValue)
                {
                    workflowInstanceId = workflowReferenceModel.ReferenceId;
                    workflowInstance.InstanceId = workflowReferenceModel.ReferenceId;
                }
                IIdentityContext identity = new IdentityContext(_security.GetCurrentUser().Account);

                if (dsw_p_ReferenceModel == null || dsw_p_ReferenceModel.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
                {
                    throw new DSWValidationException("Evaluate build area validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action.ToString()} da workflow senza aver specificato il modello di referenza" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }

                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.UpdateArchive)
                {
                    if (dsw_p_Model == null || dsw_e_UDSId == null || dsw_e_UDSRepositoryId == null || dsw_p_Model.PropertyType != WorkflowPropertyType.Json ||
                        dsw_e_UDSId.PropertyType != WorkflowPropertyType.PropertyGuid || dsw_e_UDSRepositoryId.PropertyType != WorkflowPropertyType.PropertyGuid ||
                        string.IsNullOrEmpty(dsw_p_Model.ValueString) || !dsw_e_UDSId.ValueGuid.HasValue || !dsw_e_UDSRepositoryId.ValueGuid.HasValue)
                    {
                        throw new DSWValidationException("Evaluate build area validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Impossibile avviare il proccesso di aggiornamento automatico di {currentStep.ActivityOperation.Action.ToString()} da workflow senza aver specificato il modello UDS" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }

                    UDSBuildModel udsBuildModel = new UDSBuildModel(WebUtility.HtmlDecode(dsw_p_Model.ValueString))
                    {
                        UDSRepository = new UDSRepositoryModel(dsw_e_UDSRepositoryId.ValueGuid.Value),
                        UniqueId = dsw_e_UDSId.ValueGuid.Value,
                        RegistrationUser = identity.User
                    };
                    if (dsw_p_Roles != null && dsw_p_Roles.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_Roles.ValueString) && dsw_a_SetRecipientResponsible != null &&
                        dsw_a_SetRecipientResponsible.ValueBoolean.HasValue && dsw_a_SetRecipientResponsible.ValueBoolean.Value)
                    {
                        _logger.WriteDebug(new LogMessage($"Evaluate roles {dsw_p_Roles.ValueString} to set responsible"), LogCategories);

                        foreach (WorkflowRole item in JsonConvert.DeserializeObject<ICollection<WorkflowRole>>(dsw_p_Roles.ValueString, ServiceHelper.SerializerSettings))
                        {
                            udsBuildModel.Roles.Add(new RoleModel()
                            {
                                AuthorizationType = Model.Commons.AuthorizationRoleType.Responsible,
                                TenantId = item.TenantId,
                                IdRole = item.IdRole,
                                IdRoleTenant = item.IdRole
                            });
                        }
                    }

                    workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ServiceHelper.SerializerSettings);
                }

                if (workflowReferenceModel == null || string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel))
                {
                    throw new DSWValidationException("Evaluate build area validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action.ToString()} da workflow senza aver specificato il modello di creazione" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToFascicle)
                {
                    FascicleBuildModel fascicleBuildModel = await CompleteBuildActivity<FascicleBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new CommandBuildFascicle(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToProtocol)
                {
                    ProtocolBuildModel protocolBuildModel = await CompleteBuildActivity<ProtocolBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new CommandBuildProtocol(workflowReferenceModel.ReferenceId, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                    protocolBuildModel.Protocol.MainDocument.ContentStream = null;
                    foreach (DocumentModel item in protocolBuildModel.Protocol.Attachments)
                    {
                        item.ContentStream = null;
                    }
                    foreach (DocumentModel item in protocolBuildModel.Protocol.Annexes)
                    {
                        item.ContentStream = null;
                    }
                    workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ServiceHelper.SerializerSettings);
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.CancelProtocol)
                {
                    ProtocolBuildModel protocolBuildModel = await CompleteBuildActivity<ProtocolBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new CommandDeleteProtocol(workflowReferenceModel.ReferenceId, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                    if (protocolBuildModel.Protocol.MainDocument != null)
                    {
                        protocolBuildModel.Protocol.MainDocument.ContentStream = null;
                    }
                    foreach (DocumentModel item in protocolBuildModel.Protocol.Attachments)
                    {
                        item.ContentStream = null;
                    }
                    foreach (DocumentModel item in protocolBuildModel.Protocol.Annexes)
                    {
                        item.ContentStream = null;
                    }
                    workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ServiceHelper.SerializerSettings);
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToPEC)
                {
                    PECMailBuildModel pecMailBuildModel = await CompleteBuildActivity<PECMailBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new CommandBuildPECMail(workflowReferenceModel.ReferenceId, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                    foreach (DocumentModel item in pecMailBuildModel.PECMail.Attachments)
                    {
                        item.ContentStream = null;
                    }
                    workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(pecMailBuildModel, ServiceHelper.SerializerSettings);
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToArchive)
                {
                    UDSBuildModel udsBuildModel = await CompleteBuildActivity<UDSBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new CommandInsertUDSData(workflowReferenceModel.ReferenceId, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.UpdateArchive)
                {

                    UDSBuildModel udsBuildModel = await CompleteBuildActivity<UDSBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new CommandUpdateUDSData(workflowReferenceModel.ReferenceId, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.CancelArchive)
                {
                    UDSBuildModel udsBuildModel = await CompleteBuildActivity<UDSBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new CommandDeleteUDSData(workflowReferenceModel.ReferenceId, _parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                }
                if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToShare)
                {
                    DocumentUnit documentUnit = await CompleteBuildEvent<DocumentUnit>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity,
                        (model) => new EventShareDocumentUnit(_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, identity, model));
                    if (documentUnit.Environment == 1)
                    {
                        ProtocolLog protocolLog = await _protocolLogService.CreateAsync(new ProtocolLog()
                        {
                            LogDescription = $"Protocollo {documentUnit.Title} condiviso con l'integrazione {workflowInstance.WorkflowRepository.Name}",
                            Entity = new Protocol() { UniqueId = documentUnit.UniqueId }
                        }, InsertActionType.ProtocolShared);
                    }
                }

                dsw_p_ReferenceModel.ValueString = JsonConvert.SerializeObject(workflowReferenceModel, ServiceHelper.SerializerSettings);
            }
        }

        private async Task<List<WorkflowProperty>> EvaluateCollaborationAreaAsync(Model.Workflow.WorkflowActivityType activityType, WorkflowInstance workflowInstance, List<WorkflowMapping> workflowMappingTags,
            IDictionary<int, WorkflowStep> workflowSteps, int currentStepNumber, WorkflowProperty dsw_p_Model, WorkflowProperty dsw_e_CollaborationManaged, WorkflowReferenceModel workflowReferenceModel,
            WorkflowProperty dsw_p_TemplateCollaboration, WorkflowProperty dsw_a_Generate_TemplateId, WorkflowProperty dsw_e_Generate_DocumentMetadatas, WorkflowProperty dsw_a_Generate_WordTemplate,
            WorkflowProperty dsw_a_Generate_PDFTemplate, WorkflowProperty dsw_p_ProposerRole, WorkflowProperty dsw_a_Collaboration_AddChains, WorkflowProperty dsw_p_Subject, 
            WorkflowProperty dsw_a_Collaboration_AddProposerHierarchySigner, WorkflowProperty dsw_p_CollaborationToManageModel)
        {
            List<WorkflowProperty> collaborationProperties = new List<WorkflowProperty>();
            if (activityType == Model.Workflow.WorkflowActivityType.CollaborationCreate)
            {
                CollaborationModel collaborationModel = null;
                if (dsw_p_TemplateCollaboration != null && dsw_p_TemplateCollaboration.ValueGuid.HasValue)
                {
                    RoleUser proposerSigner = null;
                    Role proposerRole = null;
                    if (dsw_a_Collaboration_AddProposerHierarchySigner !=null && dsw_a_Collaboration_AddProposerHierarchySigner.ValueBoolean.HasValue && dsw_a_Collaboration_AddProposerHierarchySigner.ValueBoolean.Value &&
                        dsw_p_ProposerRole != null && dsw_p_ProposerRole.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ProposerRole.ValueString))
                    {
                        WorkflowRole workflowRole = JsonConvert.DeserializeObject<WorkflowRole>(dsw_p_ProposerRole.ValueString, ServiceHelper.SerializerSettings);
                        proposerRole = _unitOfWork.Repository<Role>().Find(workflowRole?.IdRole);
                        if (proposerRole == null)
                        {
                            throw new DSWValidationException("Evaluate collaboration validation error",
                                new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowRole", Message = $"Il settore proponente della collaborazione {workflowRole?.Name}({workflowRole?.IdRole}) non esiste" } },
                                null, DSWExceptionCode.VA_RulesetValidation);
                        }
                        proposerSigner = _unitOfWork.Repository<RoleUser>().GetFirstHierarchySigner(proposerRole.FullIncrementalPath, DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Any);
                        _logger.WriteDebug(new LogMessage($"Proposer signer hierarchy founded({proposerSigner != null}) {proposerSigner?.Account} by role {proposerRole.Name}"), LogCategories);
                    }
                    TemplateCollaboration templateCollaboration = _unitOfWork.Repository<TemplateCollaboration>().GetWithRelations(dsw_p_TemplateCollaboration.ValueGuid.Value, optimization: true);
                    if (templateCollaboration.DocumentType == "P" && dsw_p_CollaborationToManageModel != null && dsw_p_CollaborationToManageModel.PropertyType == WorkflowPropertyType.Json && 
                        !string.IsNullOrEmpty(dsw_p_CollaborationToManageModel.ValueString))
                    {
                        ProtocolModel protocolModel = JsonConvert.DeserializeObject<ProtocolModel>(dsw_p_CollaborationToManageModel.ValueString, ServiceHelper.SerializerSettings);
                        protocolModel.Object = dsw_p_Subject?.ValueString ?? templateCollaboration.Object;
                        dsw_p_CollaborationToManageModel.ValueString = JsonConvert.SerializeObject(protocolModel, ServiceHelper.SerializerSettings);
                    }
                    DomainUserModel domainUserModel;
                    collaborationModel = new CollaborationModel();
                    collaborationModel.DocumentType = templateCollaboration.DocumentType;
                    collaborationModel.IdStatus = CollaborationStatusType.Insert;
                    collaborationModel.IdPriority = "N";
                    collaborationModel.RegistrationName = _currentDomainUser.DisplayName;
                    collaborationModel.Subject = dsw_p_Subject?.ValueString ?? templateCollaboration.Object;
                    collaborationModel.Note = templateCollaboration.Note;
                    collaborationModel.IdPriority = templateCollaboration.IdPriority;
                    short incremental = 1;
                    if (proposerSigner != null)
                    {
                        collaborationModel.CollaborationSigns.Add(new CollaborationSignModel()
                        {
                            Incremental = incremental++,
                            IsRequired = false,
                            SignUser = proposerSigner.Account,
                            SignName = proposerSigner.Description,
                            SignEmail = proposerSigner.Email,
                        });
                        workflowMappingTags.Add(new WorkflowMapping()
                        {
                            AuthorizationType = Model.Workflow.WorkflowAuthorizationType.UserName,
                            Account = new WorkflowAccount()
                            {
                                AccountName = proposerSigner.Account,
                                DisplayName = proposerSigner.Description,
                                EmailAddress = proposerSigner.Email,
                                Required = false,
                            }
                        });
                    }
                    foreach (TemplateCollaborationUser item in templateCollaboration.TemplateCollaborationUsers.Where(f => f.UserType == TemplateCollaborationUserType.Signer).OrderBy(f=>f.Incremental))
                    {
                        domainUserModel = _security.GetUser(item.Account);
                        collaborationModel.CollaborationSigns.Add(new CollaborationSignModel()
                        {
                            Incremental = incremental++,
                            IsRequired = item.IsRequired,
                            SignUser = item.Account,
                            SignName = domainUserModel.DisplayName,
                            SignEmail = domainUserModel.EmailAddress
                        });
                        workflowMappingTags.Add(new WorkflowMapping()
                        {
                            AuthorizationType = Model.Workflow.WorkflowAuthorizationType.UserName,
                            Account = new WorkflowAccount()
                            {
                                AccountName = item.Account,
                                DisplayName = domainUserModel.DisplayName,
                                EmailAddress = domainUserModel.EmailAddress,
                                Required = item.IsRequired,
                            }
                        });
                    }
                    collaborationModel.SignCount = (short)collaborationModel.CollaborationSigns.Count();
                    incremental = 1;
                    if (proposerRole != null && !templateCollaboration.TemplateCollaborationUsers.Any(f=> f.UserType == TemplateCollaborationUserType.Secretary && f.Role.EntityShortId == proposerRole.EntityShortId))
                    {
                        collaborationModel.CollaborationUsers.Add(new CollaborationUserModel()
                        {
                            Incremental = incremental++,
                            IdRole = proposerRole.EntityShortId,
                            DestinationType = CollaborationDestinationType.S.ToString(),
                            DestinationFirst = false,
                            DestinationName = proposerRole.Name,
                            DestinationEmail = proposerRole.EMailAddress,
                        });
                    }
                    foreach (TemplateCollaborationUser item in templateCollaboration.TemplateCollaborationUsers.Where(f => f.UserType == TemplateCollaborationUserType.Secretary).OrderBy(f => f.Incremental))
                    {
                        collaborationModel.CollaborationUsers.Add(new CollaborationUserModel()
                        {
                            Incremental = incremental++,
                            IdRole = item.Role.EntityShortId,
                            DestinationType = CollaborationDestinationType.S.ToString(),
                            DestinationFirst = item.IsRequired,
                            DestinationName = item.Role.Name,
                            DestinationEmail = item.Role.EMailAddress,
                        });
                    }
                }

                if (dsw_p_Model != null && dsw_p_Model.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_Model.ValueString))
                {
                    try
                    {
                        collaborationModel = JsonConvert.DeserializeObject<CollaborationModel>(dsw_p_Model.ValueString, ServiceHelper.SerializerSettings);
                    }
                    catch (Exception){}
                }
                if (collaborationModel == null)
                {
                    throw new DSWValidationException("Evaluate collaboration validation error",
                        new List<ValidationMessageModel>() 
                        { 
                            new ValidationMessageModel 
                            { 
                                Key = "WorkflowActivity", 
                                Message = "Impossibile avviare una collaborazione da Workflow senza aver specificato il modello di collaborazione" 
                            } 
                        }, null, DSWExceptionCode.VA_RulesetValidation);
                }
                KeyValuePair<string, byte[]> document = await BuildDocument(dsw_a_Generate_TemplateId, dsw_e_Generate_DocumentMetadatas, dsw_a_Generate_WordTemplate, dsw_a_Generate_PDFTemplate);
                short collaborationIncremental = 0;
                if (!string.IsNullOrEmpty(document.Key) && document.Key != null)
                {
                    collaborationModel.CollaborationVersionings.Add(new CollaborationVersioningModel()
                    {
                        Incremental = 1,
                        DocumentContent = document.Value,
                        CollaborationIncremental = collaborationIncremental++,
                        DocumentName = document.Key,
                        DocumentGroup = CollaborationDocumentGroupName.MainDocument,
                        IsActive = true,
                    });
                }
                if (IsDocumentUnit(workflowReferenceModel) && dsw_a_Collaboration_AddChains != null && dsw_a_Collaboration_AddChains.ValueBoolean.HasValue && dsw_a_Collaboration_AddChains.ValueBoolean.Value)
                {
                    DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
                    _logger.WriteDebug(new LogMessage($"Clone documents of {documentUnit?.Title} to collaboration versioning ({documentUnit?.DocumentUnitChains?.Count})"), LogCategories);
                    IEnumerable<ModelDocument.Document> documents;
                    
                    if (documentUnit.DocumentUnitChains.Any(f=> f.ChainType == ChainType.MainChain))
                    {
                        documents = await _documentService.GetDocumentLatestVersionFromChainAsync(documentUnit.DocumentUnitChains.Single(f=> f.ChainType == ChainType.MainChain).IdArchiveChain);
                        foreach (ModelDocument.Document item in documents)
                        {
                            collaborationModel.CollaborationVersionings.Add(new CollaborationVersioningModel()
                            {
                                Incremental = 1,
                                DocumentContent = await _documentService.GetDocumentContentAsync(item.IdDocument),
                                CollaborationIncremental = collaborationIncremental++,
                                DocumentName = item.Name,
                                DocumentGroup = CollaborationDocumentGroupName.MainDocument,
                                IsActive = true,
                            });
                            _logger.WriteDebug(new LogMessage($"Cloned main document {item.Name} from chain {item.IdChain} with id {item.IdDocument}"), LogCategories);
                        }
                    }
                    if (documentUnit.DocumentUnitChains.Any(f => f.ChainType == ChainType.AttachmentsChain))
                    {
                        documents = await _documentService.GetDocumentLatestVersionFromChainAsync(documentUnit.DocumentUnitChains.Single(f => f.ChainType == ChainType.AttachmentsChain).IdArchiveChain);
                        foreach (ModelDocument.Document item in documents)
                        {
                            collaborationModel.CollaborationVersionings.Add(new CollaborationVersioningModel()
                            {
                                Incremental = 1,
                                DocumentContent = await _documentService.GetDocumentContentAsync(item.IdDocument),
                                CollaborationIncremental = collaborationIncremental++,
                                DocumentName = item.Name,
                                DocumentGroup = CollaborationDocumentGroupName.Attachment,
                                IsActive = true,
                            });
                            _logger.WriteDebug(new LogMessage($"Cloned attachments document {item.Name} from chain {item.IdChain} with id {item.IdDocument}"), LogCategories);
                        }
                    }
                }
                workflowSteps = await CreateCollaborationActivityAsync(collaborationModel, dsw_e_CollaborationManaged, workflowInstance, workflowSteps, currentStepNumber, workflowMappingTags);
                if (dsw_p_Model == null)
                {
                    dsw_p_Model = new WorkflowProperty() { WorkflowType = WorkflowType.Activity, PropertyType = WorkflowPropertyType.Json, Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL };
                    collaborationProperties.Add(dsw_p_Model);
                }
                dsw_p_Model.ValueString = JsonConvert.SerializeObject(collaborationModel, ServiceHelper.SerializerSettings);
                currentStepNumber = ++currentStepNumber;
                collaborationProperties.Add(new WorkflowProperty()
                {
                    Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID,
                    PropertyType = WorkflowPropertyType.PropertyInt,
                    ValueInt = collaborationModel.IdCollaboration.Value
                });
                collaborationProperties.Add(new WorkflowProperty()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION,
                    PropertyType = WorkflowPropertyType.PropertyInt,
                    ValueInt = 0
                });
                collaborationProperties.Add(new WorkflowProperty()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_OPERATION,
                    PropertyType = WorkflowPropertyType.Json,
                    ValueString = JsonConvert.SerializeObject(workflowSteps[currentStepNumber], ServiceHelper.SerializerSettings)
                });
            }
            return collaborationProperties;
        }

        private async Task<KeyValuePair<string, byte[]>> BuildDocument(WorkflowProperty dsw_a_Generate_TemplateId, WorkflowProperty dsw_e_Generate_DocumentMetadatas, WorkflowProperty dsw_a_Generate_WordTemplate,
            WorkflowProperty dsw_a_Generate_PDFTemplate)
        {
            byte[] stream = null;
            string out_filename = string.Empty;
            if (dsw_a_Generate_TemplateId != null && dsw_a_Generate_TemplateId.ValueGuid.HasValue && dsw_e_Generate_DocumentMetadatas != null && !string.IsNullOrEmpty(dsw_e_Generate_DocumentMetadatas.ValueString))
            {
                DocumentGeneratorModel documentGeneratorModel = new DocumentGeneratorModel
                {
                    DocumentGeneratorParameters = JsonConvert.DeserializeObject<ICollection<IDocumentGeneratorParameter>>(dsw_e_Generate_DocumentMetadatas.ValueString, ServiceHelper.SerializerSettings)
                };
                StringParameter filename = documentGeneratorModel.DocumentGeneratorParameters.OfType<StringParameter>().SingleOrDefault(f => f.Name == "_filename");
                if (dsw_a_Generate_WordTemplate != null && dsw_a_Generate_WordTemplate.ValueBoolean.HasValue && dsw_a_Generate_WordTemplate.ValueBoolean.Value)
                {
                    stream = await _wordOpenXmlDocumentGenerator.GenerateDocumentAsync(dsw_a_Generate_TemplateId.ValueGuid.Value, documentGeneratorModel);
                    if (filename == null)
                    {
                        filename = new StringParameter("_filename", "document");
                    }
                    filename.Value = $"{filename.Value}.docx";
                }
                if (dsw_a_Generate_PDFTemplate != null && dsw_a_Generate_PDFTemplate.ValueBoolean.HasValue && dsw_a_Generate_PDFTemplate.ValueBoolean.Value)
                {
                    stream = await _pdfDocumentGenerator.GenerateDocumentAsync(dsw_a_Generate_TemplateId.ValueGuid.Value, documentGeneratorModel);
                    if (filename == null)
                    {
                        filename = new StringParameter("_filename", "document");
                    }
                    filename.Value = $"{filename.Value}.pdf";
                }
                out_filename = filename.Value;
            }

            return new KeyValuePair<string, byte[]>(out_filename, stream);
        }

        private async Task<WorkflowActivity> EvaluateMessageAreaAsync(WorkflowStep currentStep, WorkflowInstance workflowInstance, Guid workflowInstanceId,
            WorkflowRepository workflowRepository, int currentStepNumber, IEnumerable<WorkflowProperty> inputArguments, WorkflowProperty dsw_p_ProposerRole,
            WorkflowProperty dsw_p_Roles, WorkflowProperty dsw_p_Accounts, WorkflowProperty dsw_p_ProposerUser, WorkflowReferenceModel workflowReferenceModel,
            string responseMessage, Guid? idArchiveChain)
        {
            WorkflowArgument dsw_e_MessageBody = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_BODY);
            WorkflowArgument dsw_e_MessageSubject = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_SUBJECT);
            WorkflowArgument dsw_e_MessagePriority = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_PRIORITY);
            WorkflowArgument dsw_e_MessageLinkType = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_LINK_TYPE);
            WorkflowArgument dsw_e_MessageLink = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_LINK);
            WorkflowArgument dsw_p_EmailEvaluationRecipient = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_EMAIL_EVALUATION_RECIPIENT);
            string senderEmail = string.Empty;
            string[] recipientEmails = null;
            WorkflowAccount proposerUser = null;
            int priority = dsw_e_MessagePriority != null && dsw_e_MessagePriority.ValueInt.HasValue ? (int)dsw_e_MessagePriority.ValueInt.Value : 0;

            if (dsw_p_ProposerRole != null && dsw_p_ProposerRole.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ProposerRole.ValueString))
            {
                WorkflowRole workflowRole = JsonConvert.DeserializeObject<WorkflowRole>(dsw_p_ProposerRole.ValueString, ServiceHelper.SerializerSettings);
                Role role;
                if (workflowRole == null || workflowRole.IdRole == 0 || (role = _unitOfWork.Repository<Role>().GetByRoleId(workflowRole.IdRole, optimization: true).SingleOrDefault()) == null)
                {
                    throw new DSWException(string.Concat("Proposer RoleID ", workflowRole?.IdRole, " not defined/found on email generation"), null, DSWExceptionCode.WF_RulesetValidation);
                }
                senderEmail = role.EMailAddress?.Split(';').First();
            }
            if (dsw_p_ProposerUser != null && dsw_p_ProposerUser.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ProposerUser.ValueString))
            {
                proposerUser = JsonConvert.DeserializeObject<WorkflowAccount>(dsw_p_ProposerUser.ValueString, ServiceHelper.SerializerSettings);
            }
            if (dsw_p_Roles != null && dsw_p_Roles.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_Roles.ValueString))
            {
                _logger.WriteDebug(new LogMessage($"Evaluate email roles {dsw_p_Roles.ValueString}"), LogCategories);

                ICollection<WorkflowRole> roles = null;
                try
                {
                    roles = JsonConvert.DeserializeObject<ICollection<WorkflowRole>>(dsw_p_Roles.ValueString, ServiceHelper.SerializerSettings);
                }
                catch
                {
                    ICollection<WorkflowMapping> workflowMappingTags = JsonConvert.DeserializeObject<ICollection<WorkflowMapping>>(dsw_p_Roles.ValueString, ServiceHelper.SerializerSettings);
                    roles = workflowMappingTags.Select(f => f.Role).ToList();
                }

                recipientEmails = roles
                    .Select(f => f == null && f.IdRole == 0 ? null : _unitOfWork.Repository<Role>().GetByRoleId(f.IdRole, optimization: true).SingleOrDefault())
                    .Where(f => f != null && !string.IsNullOrEmpty(f.EMailAddress))
                    .SelectMany(f => f.EMailAddress.Split(';'))
                    .ToArray();
            }
            if (dsw_p_EmailEvaluationRecipient != null && dsw_p_EmailEvaluationRecipient.ValueString == WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS &&
                dsw_p_Accounts != null && dsw_p_Accounts.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_Accounts.ValueString))
            {
                ICollection<WorkflowAccount> accounts = JsonConvert.DeserializeObject<ICollection<WorkflowAccount>>(dsw_p_Accounts.ValueString, ServiceHelper.SerializerSettings);
                _logger.WriteDebug(new LogMessage($"Evaluate email accounts {dsw_p_Accounts.ValueString}"), LogCategories);
                recipientEmails = accounts
                    .Where(f => f != null && !string.IsNullOrEmpty(f.EmailAddress))
                    .SelectMany(f => f.EmailAddress.Split(';'))
                    .ToArray();
            }
            if (dsw_p_EmailEvaluationRecipient != null && dsw_p_EmailEvaluationRecipient.ValueString == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE)
            {
                string localSender = senderEmail;
                senderEmail = recipientEmails.First();
                recipientEmails = new string[] { localSender };
                _logger.WriteDebug(new LogMessage("Email : switch recipient with sender"), LogCategories);
            }
            if (dsw_p_EmailEvaluationRecipient != null && dsw_p_EmailEvaluationRecipient.ValueString == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER &&
                proposerUser != null && !string.IsNullOrEmpty(proposerUser.EmailAddress))
            {
                recipientEmails = new string[] { proposerUser.EmailAddress };
                _logger.WriteDebug(new LogMessage("Email recipients has been setted with ProposerUser"), LogCategories);
            }
            if (string.IsNullOrEmpty(senderEmail))
            {
                senderEmail = _security.CurrentUserEmail;
                _logger.WriteDebug(new LogMessage($"Email sender {senderEmail} has been automatically detected by CurrentSecurityUser"), LogCategories);
            }
            if (string.IsNullOrEmpty(dsw_e_MessageBody?.ValueString))
            {
                _logger.WriteWarning(new LogMessage($"Email body has not been defined in workflow InstanceId {workflowInstanceId}"), LogCategories);
            }
            if (string.IsNullOrEmpty(dsw_e_MessageSubject?.ValueString))
            {
                _logger.WriteWarning(new LogMessage($"Email subject has not been defined in workflow InstanceId {workflowInstanceId}"), LogCategories);
            }
            string messageLink = string.Empty;
            if (dsw_e_MessageLinkType != null && dsw_e_MessageLinkType.PropertyType == ArgumentType.PropertyInt && dsw_e_MessageLinkType.ValueInt.HasValue &&
                !string.IsNullOrEmpty(dsw_e_MessageLink?.ValueString))
            {
                WorkflowMessageLinkType messageLinkType = (WorkflowMessageLinkType)dsw_e_MessageLinkType.ValueInt.Value;
                string linkToken1 = string.Empty;
                string linkToken0 = string.Empty;
                switch (messageLinkType)
                {
                    case WorkflowMessageLinkType.FascicleLink:
                        {
                            if (workflowReferenceModel != null)
                            {
                                Fascicle fascicle = JsonConvert.DeserializeObject<Fascicle>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
                                if (fascicle != null)
                                {
                                    linkToken0 = fascicle.UniqueId.ToString();
                                    linkToken1 = $"Fascicolo n. {fascicle.Title} del {fascicle.RegistrationDate.ToLocalTime().Date.ToShortDateString()}";
                                }
                            }
                            break;
                        }
                    case WorkflowMessageLinkType.ExternalViewerLink:
                        {
                            linkToken0 = " ";
                            linkToken1 = " ";
                            break;
                        }
                    case WorkflowMessageLinkType.DocSuiteGenericActivity:
                        {
                            linkToken0 = CurrentDomainUser.DisplayName;
                            linkToken1 = responseMessage;
                            break;
                        }
                    case WorkflowMessageLinkType.DocumentUnitLink:
                        {
                            if (workflowReferenceModel != null)
                            {
                                DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
                                if (documentUnit != null)
                                {
                                    linkToken0 = documentUnit.UniqueId.ToString();
                                    linkToken1 = documentUnit.UDSRepository?.UniqueId.ToString();
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(linkToken0))
                {
                    messageLink = string.Empty;
                    _logger.WriteWarning(new LogMessage($"Message link has not been defined in workflow InstanceId {workflowInstanceId} to undeterminate referenceId value"), LogCategories);
                }
                if (string.IsNullOrEmpty(linkToken1))
                {
                    messageLink = string.Empty;
                    _logger.WriteWarning(new LogMessage($"Message link has not been defined in workflow InstanceId {workflowInstanceId} to undeterminate title value"), LogCategories);
                }
                if (!string.IsNullOrEmpty(linkToken0) && !string.IsNullOrEmpty(linkToken1))
                {
                    messageLink = string.Format(dsw_e_MessageLink.ValueString, linkToken0, linkToken1);
                    _logger.WriteInfo(new LogMessage($"Message link has been defined in workflow InstanceId {workflowInstanceId}"), LogCategories);
                }
            }

            Message message = await CreateMessageAsync(workflowInstanceId, dsw_e_MessageBody?.ValueString, dsw_e_MessageSubject?.ValueString, recipientEmails,
                senderEmail: senderEmail, contactType: MessageContactType.Role, priority: priority, messageLink: messageLink);
            if (message == null)
            {
                _logger.WriteError(new LogMessage($"Email has not been sended in workflow InstanceId {workflowInstanceId}"), LogCategories);
            }
            return await PopulateActivityAsync(workflowInstance, workflowInstanceId, workflowRepository, inputArguments, currentStepNumber: ++currentStepNumber, idArchiveChain: idArchiveChain);
        }

        private async Task<WorkflowActivity> EvaluateDematerialisationAreaAsync(WorkflowInstance workflowInstance, Guid workflowInstanceId, WorkflowRepository workflowRepository,
            IEnumerable<WorkflowProperty> inputArguments, int currentStepNumber, Guid? idArchiveChain)
        {
            WorkflowActivity createCollaborationActivity = workflowInstance.WorkflowActivities.Where(f => f.WorkflowProperties.Any(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID)).FirstOrDefault();
            if (createCollaborationActivity == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("CollaborationActivity has not found in workflow InstanceId ", workflowInstanceId)), LogCategories);
                throw new ArgumentNullException(string.Concat("CollaborationActivity has not found in workflow InstanceId ", workflowInstanceId));
            }
            WorkflowProperty dsw_e_CollaborationId = createCollaborationActivity.WorkflowProperties.FirstOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID);
            if (dsw_e_CollaborationId == null || !dsw_e_CollaborationId.ValueInt.HasValue)
            {
                _logger.WriteError(new LogMessage(string.Concat("CollaborationId has not found  in workflow InstanceId ", workflowInstanceId, " / WorkflowActivityId ", createCollaborationActivity.UniqueId)), LogCategories);
                throw new ArgumentNullException(string.Concat("CollaborationId has not found  in workflow InstanceId ", workflowInstanceId, " / WorkflowActivityId ", createCollaborationActivity.UniqueId));
            }

            Collaboration collaboration = _unitOfWork.Repository<Collaboration>().Find(dsw_e_CollaborationId.ValueInt.Value);
            if (collaboration == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("CollaborationId ", dsw_e_CollaborationId.ValueInt.Value, " has not found")), LogCategories);
                throw new ArgumentNullException(string.Concat("CollaborationId ", dsw_e_CollaborationId.ValueInt.Value, " has not found"));
            }
            collaboration.IdStatus = "WM";
            collaboration = await _collaborationService.UpdateAsync(collaboration);

            return await PopulateActivityAsync(workflowInstance, workflowInstanceId, workflowRepository, inputArguments, currentStepNumber: ++currentStepNumber, idArchiveChain: idArchiveChain);
        }

        private async Task EvaluateAssignmentAreaAsync(WorkflowStep currentStep, WorkflowInstance workflowInstance, WorkflowRepository workflowRepository,
            WorkflowActivity workflowActivity, WorkflowReferenceModel workflowReferenceModel, WorkflowProperty dsw_p_ReferenceModel, WorkflowProperty dsw_p_Roles, WorkflowProperty dsw_p_Accounts,
            WorkflowProperty dsw_a_Fascicle_PublicEnforcement, WorkflowProperty dsw_a_Fascicle_PublicTemporaryEnforcement)
        {
            if (currentStep.ActivityType == Model.Workflow.WorkflowActivityType.Assignment)
            {
                if (dsw_p_ReferenceModel == null || dsw_p_ReferenceModel.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
                {
                    throw new DSWValidationException("Evaluate assignment validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile avviare una presa in carico senza aver specificato il modello di referenza" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                if (IsRoleAuthorization(currentStep) && (dsw_p_Roles == null || dsw_p_Roles.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_Roles.ValueString)))
                {
                    throw new DSWValidationException("Evaluate assignment validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile avviare una presa in carico senza aver specificato almeno un settore" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                if (currentStep.AuthorizationType == Model.Workflow.WorkflowAuthorizationType.UserName && (dsw_p_Accounts == null || dsw_p_Accounts.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_Accounts.ValueString)))
                {
                    throw new DSWValidationException("Evaluate assignment validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile avviare una presa senza aver specificato il nome utente" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_p_ReferenceModel.ValueString, ServiceHelper.SerializerSettings);
                #region [ Fascile assignment]

                Fascicle fascicle = null;
                if (workflowReferenceModel.ReferenceType == DSWEnvironmentType.Fascicle)
                {
                    if (workflowReferenceModel == null || (fascicle = _unitOfWork.Repository<Fascicle>().GetWithRoles(workflowReferenceModel.ReferenceId).SingleOrDefault()) == null)
                    {
                        throw new DSWValidationException("Evaluate assignment validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile avviare una presa in carico se il fascicolo non esiste" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                }
                if (IsRoleAuthorization(currentStep))
                {
                    ICollection<WorkflowRole> roles = JsonConvert.DeserializeObject<ICollection<WorkflowRole>>(dsw_p_Roles.ValueString, ServiceHelper.SerializerSettings);
                    FascicleRole fascicleRole;
                    Role role;
                    List<WorkflowMapping> workflowMappings = new List<WorkflowMapping>();
                    foreach (WorkflowRole workflowRole in roles.Where(f => f.IdRole != 0))
                    {
                        role = _unitOfWork.Repository<Role>().Find(workflowRole.IdRole);
                        if (role == null)
                        {
                            throw new DSWValidationException("Evaluate assignment validation error",
                                new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Impossibile autorizzare il fascicolo {fascicle.Title} se il settore {workflowRole.IdRole} non esiste" } },
                                null, DSWExceptionCode.VA_RulesetValidation);
                        }
                        if (fascicle != null)
                        {
                            if (!fascicle.FascicleRoles.Any(f => f.Role.EntityShortId == role.EntityShortId && f.AuthorizationRoleType == AuthorizationRoleType.Responsible))
                            {
                                WorkflowInstanceLog workflowInstanceLogs = new WorkflowInstanceLog()
                                {
                                    LogType = WorkflowInstanceLogType.WFRoleAssigned,
                                    LogDescription = $"Assegnato al settore '{role.Name}' ({role.EntityShortId}), per competenza (responsabile), il flusso di attività '{workflowRepository.Name}'",
                                    SystemComputer = Environment.MachineName,
                                    Entity = workflowInstance
                                };
                                _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLogs);
                                fascicleRole = fascicle.FascicleRoles.FirstOrDefault(f => f.Role.EntityShortId == role.EntityShortId);
                                if (fascicleRole == null)
                                {
                                    fascicleRole = await _fascicleRoleService.CreateAsync(new FascicleRole()
                                    {
                                        Fascicle = fascicle,
                                        Role = role,
                                        AuthorizationRoleType = AuthorizationRoleType.Responsible
                                    });
                                    _logger.WriteDebug(new LogMessage($"Added role {workflowRole.IdRole} to fascicle {fascicle.Title} with identifier {fascicleRole.UniqueId}"), LogCategories);
                                }
                                else
                                {
                                    _logger.WriteDebug(new LogMessage($"Role {fascicleRole.Role?.Name} ({fascicleRole.Role?.EntityShortId}) from fascicle {fascicle.Title} has been successfully setted to Responsible"), LogCategories);
                                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                                    fascicleRole = await _fascicleRoleService.UpdateAsync(fascicleRole);
                                }
                            }
                            else
                            {
                                _logger.WriteDebug(new LogMessage($"Skipped role ({workflowRole.IdRole}) to fascicle {fascicle.Title} because already authorized"), LogCategories);
                            }
                        }
                        workflowMappings.Add(new WorkflowMapping() { AuthorizationType = currentStep.AuthorizationType, Role = workflowRole });
                    }

                    dsw_p_Roles.ValueString = JsonConvert.SerializeObject(workflowMappings, ServiceHelper.SerializerSettings);
                }
                if (fascicle != null)
                {
                    if ((dsw_a_Fascicle_PublicEnforcement != null && dsw_a_Fascicle_PublicEnforcement.ValueBoolean.HasValue && dsw_a_Fascicle_PublicEnforcement.ValueBoolean.Value) ||
                        (dsw_a_Fascicle_PublicTemporaryEnforcement != null && dsw_a_Fascicle_PublicTemporaryEnforcement.ValueBoolean.HasValue && dsw_a_Fascicle_PublicTemporaryEnforcement.ValueBoolean.Value))
                    {
                        if (fascicle.VisibilityType == DocSuiteWeb.Entity.Fascicles.VisibilityType.Confidential)
                        {
                            fascicle.VisibilityType = DocSuiteWeb.Entity.Fascicles.VisibilityType.Accessible;
                            _unitOfWork.Repository<Fascicle>().Update(fascicle);
                            _logger.WriteDebug(new LogMessage($"Fascicle visivility changed to accessible during activity '{workflowActivity.Name}' of workflow '{workflowRepository.Name}'"), LogCategories);
                            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(fascicle, FascicleLogType.Workflow, $"Fascicolo reso pubblico durante attività '{workflowActivity.Name}' - '{workflowRepository.Name}'", CurrentDomainUser.Account));
                        }
                    }
                
                    _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(fascicle, FascicleLogType.Workflow, $"Avviato correttamente il flusso di Lavoro '{workflowRepository.Name}'", CurrentDomainUser.Account));
                    foreach (DossierFolder dossierFolder in fascicle.DossierFolders)
                    {
                        _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(dossierFolder.Dossier, dossierFolder, DossierLogType.Workflow,
                             $"Avviato correttamente flusso di lavoro '{workflowRepository.Name}' sulla cartella {dossierFolder.Name}", CurrentDomainUser.Account));
                    }
                    workflowInstance.Fascicles.Add(fascicle);
                }
                #endregion

                workflowActivity.WorkflowProperties.Add(new WorkflowProperty()
                {
                    Name = WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE,
                    PropertyType = WorkflowPropertyType.Json,
                    WorkflowType = WorkflowType.Activity,
                    ValueString = "{}"
                });
            }
        }

        private List<WorkflowProperty> EvaluateCollaborationArea(WorkflowActivityType activityType, WorkflowProperty dsw_p_Model, WorkflowProperty dsw_p_SignerPosition,
            WorkflowProperty dsw_p_Accounts, WorkflowProperty dsw_p_SignerModel, WorkflowProperty dsw_p_CollaborationToManageModel)
        {
            List<WorkflowProperty> results = new List<WorkflowProperty>();
            if (activityType == DocSuiteWeb.Entity.Workflows.WorkflowActivityType.CollaborationSign)
            {
                if (dsw_p_Model == null || dsw_p_Model.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_Model.ValueString))
                {
                    throw new DSWException("Unable to start a collaboration activity without specifying the collaboration model", null, DSWExceptionCode.WF_RulesetValidation);
                }
                if (dsw_p_SignerPosition == null || dsw_p_SignerPosition.PropertyType != WorkflowPropertyType.PropertyInt || !dsw_p_SignerPosition.ValueInt.HasValue)
                {
                    throw new DSWException("Unable to start a collaboration activity without specifying the signer position", null, DSWExceptionCode.WF_RulesetValidation);
                }
                CollaborationModel collaborationModel = JsonConvert.DeserializeObject<CollaborationModel>(dsw_p_Model.ValueString, ServiceHelper.SerializerSettings);

                if (dsw_p_Accounts == null)
                {
                    dsw_p_Accounts = new WorkflowProperty()
                    {
                        Name = WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS,
                        PropertyType = WorkflowPropertyType.PropertyString,
                    };
                    results.Add(dsw_p_Accounts);
                }
                CollaborationSignModel collaborationSignModel = collaborationModel.CollaborationSigns.ElementAt((int)dsw_p_SignerPosition.ValueInt.Value);
                List<WorkflowAccount> signerWorkflowAccount = new List<WorkflowAccount>() {
                    new WorkflowAccount()
                    {
                        AccountName = collaborationSignModel.SignUser,
                        DisplayName = collaborationSignModel.SignName,
                        EmailAddress = collaborationSignModel.SignEmail,
                        Required = collaborationSignModel.IsRequired.HasValue ? collaborationSignModel.IsRequired.Value : false
                    }
                };
                dsw_p_Accounts.ValueString = JsonConvert.SerializeObject(signerWorkflowAccount);

                if (dsw_p_SignerModel == null)
                {
                    results.Add(new WorkflowProperty()
                    {
                        Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL,
                        PropertyType = WorkflowPropertyType.Json,
                        WorkflowType = WorkflowType.Activity,
                        ValueString = JsonConvert.SerializeObject(new List<CollaborationSignerWorkflowModel>(), ServiceHelper.SerializerSettings)
                    });
                }
            }

            if (activityType == WorkflowActivityType.CollaborationToProtocol && dsw_p_CollaborationToManageModel != null &&
                dsw_p_CollaborationToManageModel.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_CollaborationToManageModel.ValueString))
            {
                dsw_p_Model.ValueString = dsw_p_CollaborationToManageModel.ValueString;
            }
            return results;
        }

        protected async Task<WorkflowActivity> PopulateActivityAsync(WorkflowInstance workflowInstance, Guid workflowInstanceId, WorkflowRepository workflowRepository,
            IEnumerable<WorkflowProperty> inputArguments, int currentStepNumber = 0, Guid? idArchiveChain = null)
        {
            try
            {
                IDictionary<int, WorkflowStep> workflowSteps = JsonConvert.DeserializeObject<Dictionary<int, WorkflowStep>>(workflowInstance.Json, ServiceHelper.SerializerSettings);
                List<WorkflowMapping> workflowMappingTags = new List<WorkflowMapping>();

                if (workflowSteps.Count <= currentStepNumber)
                {
                    return await CompleteWorkflowInstanceAsync(workflowInstance, workflowSteps, currentStepNumber);
                }
                Guid currentIdWorkflowActivity = Guid.NewGuid();
                WorkflowStep currentStep = workflowSteps[currentStepNumber];

                string prop_step_referencePosition = $"_{currentStep.Position}";
                string prop_step_referenceModel = $"{WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL}{prop_step_referencePosition}";

                WorkflowProperty dsw_p_Model_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);
                WorkflowProperty dsw_e_UDSId_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_UDS_ID);
                WorkflowProperty dsw_e_UDSRepositoryId_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID);
                WorkflowProperty dsw_p_DueDate_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE);
                WorkflowProperty dsw_p_Subject_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT);
                WorkflowProperty dsw_p_ActivityName_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME);
                WorkflowProperty dsw_p_ProposerRole_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE);
                WorkflowProperty dsw_p_Roles_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLES);
                WorkflowProperty dsw_p_Accounts_instance = inputArguments.SingleOrDefault(f => f.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS));
                WorkflowProperty dsw_p_ProposerUser_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER);
                WorkflowProperty dsw_p_ReferenceModel_instance = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
                WorkflowProperty dsw_p_ReferenceModel_currentStep = inputArguments.SingleOrDefault(f => f.Name == prop_step_referenceModel);
                WorkflowProperty dsw_p_ReferenceModel_evaluate = dsw_p_ReferenceModel_currentStep ?? dsw_p_ReferenceModel_instance;
                WorkflowProperty dsw_e_CollaborationManaged = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_MANAGED);
                WorkflowProperty dsw_p_TemplateCollaboration = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION);
                WorkflowProperty dsw_a_Generate_TemplateId = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_GENERATE_TEMPLATE_ID);
                WorkflowProperty dsw_e_Generate_DocumentMetadatas = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_GENERATE_DOCUMENT_METADATAS);
                WorkflowProperty dsw_a_Generate_WordTemplate = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_GENERATE_WORD_TEMPLATE);
                WorkflowProperty dsw_a_Generate_PDFTemplate = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_GENERATE_PDF_TEMPLATE);
                WorkflowProperty dsw_p_Priority = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY);
                WorkflowProperty dsw_e_ActivityStartReferenceModel = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_REFERENCE_MODEL);
                WorkflowProperty dsw_a_Parallel_Activity = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_PARALLEL_ACTIVITY);
                WorkflowProperty dsw_a_SetRecipientResponsible = inputArguments.SingleOrDefault(f => f.Name == "_dsw_a_SetRecipientResponsible");
                WorkflowProperty dsw_a_Collaboration_AddChains = inputArguments.SingleOrDefault(f => f.Name == "_dsw_a_Collaboration_AddChains");
                WorkflowProperty dsw_a_Collaboration_AddProposerHierarchySigner = inputArguments.SingleOrDefault(f => f.Name == "_dsw_a_Collaboration_AddProposerHierarchySigner");
                WorkflowProperty dsw_p_CollaborationToManageModel = inputArguments.SingleOrDefault(f => f.Name == "_dsw_p_CollaborationToManageModel");
                
                WorkflowReferenceModel workflowReferenceModel = null;
                WorkflowReferenceModel activityReferenceModel = null;
                if (dsw_p_ReferenceModel_evaluate != null && dsw_p_ReferenceModel_evaluate.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ReferenceModel_evaluate.ValueString))
                {
                    workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_p_ReferenceModel_evaluate.ValueString, ServiceHelper.SerializerSettings);
                }
                if (dsw_e_ActivityStartReferenceModel != null && dsw_e_ActivityStartReferenceModel.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_ActivityStartReferenceModel.ValueString))
                {
                    activityReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_e_ActivityStartReferenceModel.ValueString, ServiceHelper.SerializerSettings);
                }
                workflowMappingTags = EvaluateMappingTags(workflowRepository, inputArguments, workflowMappingTags);

                #region [ Automation Areas ]
                if (currentStep.ActivityType == Model.Workflow.WorkflowActivityType.AutomaticActivity)
                {
                    if (currentStep.ActivityOperation.Area == Model.Workflow.WorkflowActivityArea.Message && currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.Create)
                    {
                        string responseMessage = string.Empty;
                        if (inputArguments != null)
                        {
                            WorkflowProperty dsw_e_ActivityEndMotivation = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION);
                            responseMessage = dsw_e_ActivityEndMotivation?.ValueString;
                        }
                        return await EvaluateMessageAreaAsync(currentStep, workflowInstance, workflowInstanceId, workflowRepository, currentStepNumber, inputArguments,
                            dsw_p_ProposerRole_instance, dsw_p_Roles_instance, dsw_p_Accounts_instance, dsw_p_ProposerUser_instance, workflowReferenceModel,
                            responseMessage, idArchiveChain);
                    }
                    await EvaluateBuildAreaAsync(workflowInstance, workflowInstanceId, currentIdWorkflowActivity, currentStep, dsw_p_ReferenceModel_evaluate, dsw_p_Model_instance,
                        dsw_e_UDSId_instance, dsw_e_UDSRepositoryId_instance, dsw_p_Roles_instance, dsw_a_SetRecipientResponsible, workflowReferenceModel);
                }

                if (currentStep.ActivityType == Model.Workflow.WorkflowActivityType.DematerialisationStatement || currentStep.ActivityType == Model.Workflow.WorkflowActivityType.SecureDocumentCreate)
                {
                    return await EvaluateDematerialisationAreaAsync(workflowInstance, workflowInstanceId, workflowRepository, inputArguments, currentStepNumber, idArchiveChain);
                }

                List<WorkflowProperty> collaborationProperties = await EvaluateCollaborationAreaAsync(currentStep.ActivityType, workflowInstance, workflowMappingTags, workflowSteps, currentStepNumber,
                    dsw_p_Model_instance, dsw_e_CollaborationManaged, workflowReferenceModel, dsw_p_TemplateCollaboration, dsw_a_Generate_TemplateId, dsw_e_Generate_DocumentMetadatas,
                    dsw_a_Generate_WordTemplate, dsw_a_Generate_PDFTemplate, dsw_p_ProposerRole_instance, dsw_a_Collaboration_AddChains, dsw_p_Subject_instance, dsw_a_Collaboration_AddProposerHierarchySigner,
                    dsw_p_CollaborationToManageModel);
                WorkflowProperty workflowProperty = collaborationProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_OPERATION);
                if (workflowProperty != null && workflowProperty.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(workflowProperty.ValueString))
                {
                    currentStep = JsonConvert.DeserializeObject<WorkflowStep>(workflowProperty.ValueString, ServiceHelper.SerializerSettings);
                    collaborationProperties.Remove(workflowProperty);
                }
                workflowProperty = collaborationProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);
                if (workflowProperty != null && workflowProperty.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(workflowProperty.ValueString))
                {
                    dsw_p_Model_instance = workflowProperty;
                }
                #endregion

                #region[ Generic Activities ]
                if (activityReferenceModel != null && !string.IsNullOrEmpty(activityReferenceModel.ReferenceModel) && currentStep.ActivityType == Model.Workflow.WorkflowActivityType.GenericActivity)
                {
                    idArchiveChain = await ArchiveDocument(dsw_e_ActivityStartReferenceModel, activityReferenceModel, idArchiveChain);
                }
                if (workflowReferenceModel != null && workflowReferenceModel.Documents != null && currentStep.ActivityType == Model.Workflow.WorkflowActivityType.GenericActivity)
                {
                    idArchiveChain = await ArchiveDocumentFromReferences(workflowReferenceModel, idArchiveChain);
                    dsw_p_ReferenceModel_evaluate.ValueString = JsonConvert.SerializeObject(workflowReferenceModel, ServiceHelper.SerializerSettings);
                }

                #endregion

                #region Decorate WorkflowActivity base properties
                WorkflowActivity workflowActivity = new WorkflowActivity(currentIdWorkflowActivity) { Name = currentStep.Name };
                workflowActivity.ActivityType = (DocSuiteWeb.Entity.Workflows.WorkflowActivityType)currentStep.ActivityType;
                workflowActivity.ActivityAction = (DocSuiteWeb.Entity.Workflows.WorkflowActivityAction)currentStep.ActivityOperation.Action;
                workflowActivity.ActivityArea = (DocSuiteWeb.Entity.Workflows.WorkflowActivityArea)currentStep.ActivityOperation.Area;
                workflowActivity.Status = WorkflowStatus.Todo;
                workflowActivity.WorkflowInstance = workflowInstance;

                #region Dynamic properites
                if (IsDocumentUnit(workflowReferenceModel) && workflowReferenceModel.ReferenceId != Guid.Empty)
                {
                    workflowActivity.DocumentUnitReferenced = _unitOfWork.Repository<DocumentUnit>().Find(workflowReferenceModel.ReferenceId);
                    _logger.WriteDebug(new LogMessage($"SET ACTIVITY REFERENCED: {workflowActivity.DocumentUnitReferenced?.Title}"), LogCategories);
                }
                if (dsw_p_ActivityName_instance != null && !string.IsNullOrEmpty(dsw_p_ActivityName_instance.ValueString))
                {
                    workflowActivity.Name = dsw_p_ActivityName_instance.ValueString;
                    _logger.WriteDebug(new LogMessage($"SET ACTIVITY NAME: {dsw_p_ActivityName_instance.ValueString}"), LogCategories);
                }
                if (dsw_p_DueDate_instance != null && dsw_p_DueDate_instance.ValueDate.HasValue)
                {
                    workflowActivity.DueDate = dsw_p_DueDate_instance.ValueDate.Value;
                    _logger.WriteDebug(new LogMessage($"SET DUE DATE: {dsw_p_DueDate_instance.ValueDate.Value}"), LogCategories);
                }
                if (dsw_p_Subject_instance != null && !string.IsNullOrEmpty(dsw_p_Subject_instance.ValueString))
                {
                    workflowActivity.Subject = dsw_p_Subject_instance.ValueString;
                    _logger.WriteDebug(new LogMessage($"SET SUBJECT: {dsw_p_Subject_instance.ValueString}"), LogCategories);
                }
                if (dsw_p_Priority != null && dsw_p_Priority.ValueInt.HasValue)
                {
                    WorkflowPriorityType workflowPriorityType = (WorkflowPriorityType)dsw_p_Priority.ValueInt;
                    workflowActivity.Priority = workflowPriorityType;
                    _logger.WriteDebug(new LogMessage($"SET PRIORITY: {dsw_p_Priority.ValueString}"), LogCategories);
                }
                if (idArchiveChain.HasValue)
                {
                    workflowActivity.IdArchiveChain = idArchiveChain.Value;
                }
                if (inputArguments.Any(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE))
                {
                    if (dsw_p_ProposerRole_instance == null || dsw_p_ProposerRole_instance.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_ProposerRole_instance.ValueString))
                    {
                        throw new DSWValidationException("Evaluate properties validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile autorizzare una istanza di Workflow senza aver specificato il modello di RoleModel" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                    WorkflowRole workflowRole = JsonConvert.DeserializeObject<WorkflowRole>(dsw_p_ProposerRole_instance.ValueString, ServiceHelper.SerializerSettings);
                    if (workflowRole == null || workflowRole.IdRole == 0)
                    {
                        throw new DSWValidationException("Evaluate properties validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile autorizzare una istanza di Workflow se il modello WorkflowRole non ha un valore valido per l'IDRole" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                    Role role = _unitOfWork.Repository<Role>().Find(workflowRole.IdRole);
                    if (role == null)
                    {
                        throw new DSWValidationException("Evaluate properties validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Impossibile autorizzare una istanza di Workflow se il settore {workflowRole.IdRole} non esiste" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }

                    WorkflowInstanceRole workflowInstanceRole = new WorkflowInstanceRole
                    {
                        AuthorizationType = AuthorizationRoleType.Accounted,
                        Role = role,
                        WorkflowInstance = workflowInstance
                    };
                    workflowInstanceRole = await _workflowInstanceRoleService.CreateAsync(workflowInstanceRole);

                    workflowRole.Name = role.Name;
                    dsw_p_ProposerRole_instance.ValueString = JsonConvert.SerializeObject(workflowRole, ServiceHelper.SerializerSettings);
                    _logger.WriteDebug(new LogMessage(string.Concat("SET INSTANCE ROLE: ", role.EntityShortId, " ", role.Name)), LogCategories);
                }
                #endregion

                workflowActivity.WorkflowActivityLogs.Add(new WorkflowActivityLog()
                {
                    LogDescription = string.Concat("Richiesta esecuzione ", workflowActivity.Name),
                    LogType = WorkflowStatus.Progress,
                    LogDate = DateTimeOffset.UtcNow,
                    RegistrationUser = _identityContext.User,
                    SystemComputer = Environment.MachineName
                });

                #endregion

                #region Populate WorkflowProperties

                if (workflowMappingTags.Any())
                {
                    workflowActivity.WorkflowProperties.Add(new WorkflowProperty()
                    {
                        Name = WorkflowPropertyHelper.DSW_PROPERTY_ROLES,
                        PropertyType = WorkflowPropertyType.Json,
                        WorkflowType = WorkflowType.Activity,
                        ValueString = JsonConvert.SerializeObject(workflowMappingTags, ServiceHelper.SerializerSettings)
                    });
                }

                if (collaborationProperties != null && collaborationProperties.Any())
                {
                    collaborationProperties.ForEach(f => workflowActivity.WorkflowProperties.Add(f));
                }

                List<WorkflowProperty> currentWorkflowProperties = inputArguments.ToList();
                //if (inputArguments != null && inputArguments.Any())
                //{
                //    currentWorkflowProperties.AddRange(inputArguments.Where(f => currentStep.InputArguments.Any(i => i.Name == f.Name)));
                //}
                foreach (WorkflowProperty currentProp in currentWorkflowProperties
                    .Where(f => !f.Name.StartsWith(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL) ||
                                f.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL) || f.Name.Equals(prop_step_referenceModel)))
                {
                    workflowActivity.WorkflowProperties.Add(CloneWorkflowProperty(currentProp));
                }
                workflowActivity.WorkflowProperties.Add(new WorkflowProperty()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_OPERATION,
                    PropertyType = WorkflowPropertyType.Json,
                    WorkflowType = WorkflowType.Activity,
                    ValueString = JsonConvert.SerializeObject(new WorkflowActivityOperation()
                    {
                        Action = currentStep.ActivityOperation.Action,
                        Area = currentStep.ActivityOperation.Area,
                    }, ServiceHelper.SerializerSettings)
                });
                workflowActivity.WorkflowProperties.Add(new WorkflowProperty()
                {
                    Name = WorkflowPropertyHelper.DSW_PROPERTY_CURRENT_STEP,
                    PropertyType = WorkflowPropertyType.Json,
                    WorkflowType = WorkflowType.Activity,
                    ValueString = JsonConvert.SerializeObject(currentStep, ServiceHelper.SerializerSettings)
                });

                WorkflowProperty dsw_p_SignerPosition_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION);
                WorkflowProperty dsw_p_Accounts_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS));
                WorkflowProperty dsw_p_SignerModel_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL));
                WorkflowProperty dsw_p_ReferenceModel_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
                WorkflowProperty dsw_p_Roles_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLES);
                WorkflowProperty dsw_a_Fascicle_PublicEnforcement_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_PUBLIC_FASCICLE_ENFORCEMENT);
                WorkflowProperty dsw_a_Fascicle_PublicTemporaryEnforcement_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_PUBLIC_FASCICLE_TEMPORARY_ENFORCEMENT);
                WorkflowProperty dsw_a_SetAuditableProperties_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_SET_AUDITABLE_PROPERTIES);
                WorkflowProperty dsw_p_CollaborationToManageModel_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == "_dsw_p_CollaborationToManageModel");
                WorkflowProperty dsw_p_Model_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);


                if (dsw_a_SetAuditableProperties_activity != null && dsw_a_SetAuditableProperties_activity.PropertyType == WorkflowPropertyType.Json
                    && !string.IsNullOrEmpty(dsw_a_SetAuditableProperties_activity.ValueString))
                {
                    workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_a_SetAuditableProperties_activity.ValueString, ServiceHelper.SerializerSettings);
                    if (workflowReferenceModel != null && !string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel) && workflowReferenceModel.ReferenceType == DSWEnvironmentType.Workflow)
                    {
                        DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
                        if (documentUnit != null && !string.IsNullOrEmpty(documentUnit.RegistrationUser) && documentUnit.RegistrationDate != DateTimeOffset.MinValue)
                        {
                            workflowActivity.RegistrationDate = documentUnit.RegistrationDate;
                            workflowActivity.RegistrationUser = documentUnit.RegistrationUser;
                            _logger.WriteDebug(new LogMessage($"set auditable properties: {documentUnit.RegistrationUser}/{documentUnit.RegistrationDate}"), LogCategories);
                        }
                    }
                }

                collaborationProperties = EvaluateCollaborationArea(workflowActivity.ActivityType, dsw_p_Model_activity, dsw_p_SignerPosition_activity, dsw_p_Accounts_activity, dsw_p_SignerModel_activity, dsw_p_CollaborationToManageModel_activity);
                if (collaborationProperties != null && collaborationProperties.Any())
                {
                    collaborationProperties.ForEach(f => workflowActivity.WorkflowProperties.Add(f));
                }

                await EvaluateAssignmentAreaAsync(currentStep, workflowInstance, workflowRepository, workflowActivity, workflowReferenceModel, dsw_p_ReferenceModel_activity,
                    dsw_p_Roles_activity, dsw_p_Accounts_activity, dsw_a_Fascicle_PublicEnforcement_activity, dsw_a_Fascicle_PublicTemporaryEnforcement_activity);


                #endregion

                WorkflowInstanceLog workflowInstanceLog = new WorkflowInstanceLog()
                {
                    LogType = WorkflowInstanceLogType.WFStarted,
                    LogDescription = $"Avviato il Flusso di Lavoro '{workflowRepository.Name}'",
                    SystemComputer = Environment.MachineName,
                    Entity = workflowInstance
                };
                _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLog);

                _logger.WriteDebug(new LogMessage("DecorateAuhtorizationActivity"), LogCategories);
                workflowActivity = DecorateAuthorizationActivity(workflowActivity, currentStep);
                if (dsw_a_Parallel_Activity != null && dsw_a_Parallel_Activity.ValueBoolean.HasValue && dsw_a_Parallel_Activity.ValueBoolean.Value && workflowActivity.WorkflowAuthorizations.Count > 1)
                {
                    _logger.WriteInfo(new LogMessage("Parallel Activity required"), LogCategories);
                    WorkflowAuthorization workflowAuthorization = workflowActivity.WorkflowAuthorizations.First();
                    WorkflowActivity clonedWorkflowActivity;
                    short workflowLocationId = _parameterEnvService.WorkflowLocationId;
                    Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
                    if (workflowLocation == null)
                    {
                        throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
                    }

                    for (int i = 1; i < workflowActivity.WorkflowAuthorizations.Count; i++)
                    {
                        clonedWorkflowActivity = await CloneWorkflowActivityAsync(workflowActivity, workflowLocation);
                        clonedWorkflowActivity.WorkflowAuthorizations.Add(workflowActivity.WorkflowAuthorizations.ElementAt(i));
                        clonedWorkflowActivity.WorkflowProperties.Add(new WorkflowProperty()
                        {
                            Name = WorkflowPropertyHelper.DSW_FIELD_RECIPIENT_POSITION,
                            PropertyType = WorkflowPropertyType.PropertyInt,
                            WorkflowType = WorkflowType.Activity,
                            ValueInt = i
                        });
                        clonedWorkflowActivity = await _workflowActivityService.CreateAsync(clonedWorkflowActivity);
                        _logger.WriteInfo(new LogMessage($"New cloned activity {clonedWorkflowActivity.Name} for {workflowRepository.Name}"), LogCategories);
                    }
                    workflowActivity.WorkflowAuthorizations.Clear();
                    workflowActivity.WorkflowAuthorizations.Add(workflowAuthorization);
                }
                workflowActivity = await _workflowActivityService.CreateAsync(workflowActivity);
                workflowInstance.Json = JsonConvert.SerializeObject(workflowSteps, ServiceHelper.SerializerSettings);
                _logger.WriteInfo(new LogMessage($"New activity {workflowActivity.Name} for {workflowRepository.Name}"), LogCategories);

                return workflowActivity;
            }
            catch (DSWValidationException ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException($"Service workflow layer - unexpected exception was thrown while invoking PopulateActivityAsync : {ex.Message}", ex, DSWExceptionCode.SS_Anomaly);
            }
        }

        private async Task<WorkflowActivity> CloneWorkflowActivityAsync(WorkflowActivity currentWorkflowActivity, Location workflowLocation)
        {
            WorkflowActivity workflowActivity = new WorkflowActivity
            {
                ActivityType = currentWorkflowActivity.ActivityType,
                ActivityAction = currentWorkflowActivity.ActivityAction,
                ActivityArea = currentWorkflowActivity.ActivityArea,
                DocumentUnitReferenced = currentWorkflowActivity.DocumentUnitReferenced,
                DueDate = currentWorkflowActivity.DueDate,
                IdArchiveChain = currentWorkflowActivity.IdArchiveChain
            };
            if (workflowActivity.IdArchiveChain.HasValue && workflowLocation != null)
            {
                IEnumerable<ModelDocument.Document> documents = await _documentService.GetDocumentLatestVersionFromChainAsync(workflowActivity.IdArchiveChain.Value);
                List<ArchiveDocument> archiveDocuments = new List<ArchiveDocument>();
                foreach (ModelDocument.Document document in documents)
                {
                    archiveDocuments.Add(new ArchiveDocument
                    {
                        Archive = workflowLocation.ProtocolArchive,
                        ContentStream = await _documentService.GetDocumentContentAsync(document.IdDocument),
                        Name = document.Name
                    });
                }
                ICollection<ModelDocument.ArchiveDocument> results = await _documentService.InsertDocumentsAsync(archiveDocuments);
                workflowActivity.IdArchiveChain = results.First().IdChain.Value;
            }
            workflowActivity.Name = currentWorkflowActivity.Name;
            workflowActivity.Note = currentWorkflowActivity.Note;
            workflowActivity.RegistrationDate = currentWorkflowActivity.RegistrationDate;
            workflowActivity.RegistrationUser = currentWorkflowActivity.RegistrationUser;
            workflowActivity.Status = currentWorkflowActivity.Status;
            workflowActivity.Subject = currentWorkflowActivity.Subject;
            workflowActivity.WorkflowInstance = currentWorkflowActivity.WorkflowInstance;
            foreach (WorkflowProperty currentProp in currentWorkflowActivity.WorkflowProperties)
            {
                workflowActivity.WorkflowProperties.Add(CloneWorkflowProperty(currentProp));
            }
            return workflowActivity;
        }

        private static WorkflowProperty CloneWorkflowProperty(WorkflowProperty currentProp)
        {
            return new WorkflowProperty()
            {
                Name = currentProp.Name,
                PropertyType = currentProp.PropertyType,
                ValueBoolean = currentProp.ValueBoolean,
                ValueDate = currentProp.ValueDate,
                ValueDouble = currentProp.ValueDouble,
                ValueGuid = currentProp.ValueGuid,
                ValueInt = currentProp.ValueInt,
                ValueString = currentProp.ValueString,
                WorkflowType = WorkflowType.Activity
            };
        }

        protected async Task<Guid?> ArchiveDocument(WorkflowProperty dsw_e_ActivityStartReferenceModel, WorkflowReferenceModel activityReferenceModel, Guid? idArchiveChain)
        {
            WorkflowDocumentModel workflowDocumentModel = JsonConvert.DeserializeObject<WorkflowDocumentModel>(activityReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
            if (workflowDocumentModel != null)
            {
                _logger.WriteDebug(new LogMessage($"Initialize archive document {workflowDocumentModel.Documents.Count} to store in workflow storage"), LogCategories);
                short workflowLocationId = _parameterEnvService.WorkflowLocationId;
                Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
                if (workflowLocation == null)
                {
                    throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
                }
                ICollection<ArchiveDocument> results = workflowDocumentModel.Documents
                    .Where(f => f.Key == Model.Entities.DocumentUnits.ChainType.Miscellanea)
                    .Select(f => new ArchiveDocument
                    {
                        Archive = workflowLocation.ProtocolArchive,
                        ContentStream = f.Value.ContentStream,
                        Name = f.Value.FileName
                    }).ToList();
                if (results.Any())
                {
                    _logger.WriteDebug(new LogMessage($"archiving {results.Count} documents to workflow storage"), LogCategories);
                    results = await _documentService.InsertDocumentsAsync(results, idChain: idArchiveChain);
                    foreach (KeyValuePair<Model.Entities.DocumentUnits.ChainType, DocumentModel> item in workflowDocumentModel.Documents.Where(f => f.Key == Model.Entities.DocumentUnits.ChainType.Miscellanea))
                    {
                        item.Value.ContentStream = null;
                    }
                }
                else
                {
                    List<ModelDocument.Document> documents = new List<Document>();
                    foreach (Guid chainId in workflowDocumentModel.Documents.Where(f => f.Value.ChainId.HasValue).Select(f => f.Value.ChainId))
                    {
                        documents.AddRange(await _documentService.GetDocumentLatestVersionFromChainAsync(chainId));
                    }
                    List<ArchiveDocument> archiveDocuments = new List<ArchiveDocument>();
                    foreach (ModelDocument.Document document in documents)
                    {
                        archiveDocuments.Add(new ArchiveDocument
                        {
                            Archive = workflowLocation.ProtocolArchive,
                            ContentStream = await _documentService.GetDocumentContentAsync(document.IdDocument),
                            Name = document.Name
                        });
                    }
                    if (archiveDocuments.Any())
                    {
                        _logger.WriteDebug(new LogMessage($"Cloning {archiveDocuments.Count} documents to workflow storage"), LogCategories);
                        results = await _documentService.InsertDocumentsAsync(archiveDocuments);
                    }
                }
                activityReferenceModel.ReferenceModel = JsonConvert.SerializeObject(workflowDocumentModel);
                dsw_e_ActivityStartReferenceModel.ValueString = JsonConvert.SerializeObject(activityReferenceModel);
                idArchiveChain = results.First().IdChain;
            }

            return idArchiveChain;
        }

        protected async Task<Guid?> ArchiveDocumentFromReferences(WorkflowReferenceModel workflowReferenceModel, Guid? idArchiveChain)
        {
            IEnumerable<WorkflowReferenceBiblosModel> referenceBiblosModels;
            if (workflowReferenceModel != null &&
                (referenceBiblosModels = workflowReferenceModel.Documents.Where(f => f.ChainType == Model.Entities.DocumentUnits.ChainType.Miscellanea && f.ArchiveChainId.HasValue && f.ArchiveDocumentId.HasValue && string.IsNullOrEmpty(f.ArchiveName))).Any())
            {
                short workflowLocationId = _parameterEnvService.WorkflowLocationId;
                Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
                if (workflowLocation == null)
                {
                    throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
                }
                ICollection<ArchiveDocument> results = new List<ArchiveDocument>();
                byte[] content;
                foreach (WorkflowReferenceBiblosModel item in referenceBiblosModels)
                {
                    content = await _documentService.GetDocumentContentAsync(item.ArchiveDocumentId.Value);
                    item.ArchiveName = workflowLocation.ProtocolArchive;
                    results.Add(new ArchiveDocument
                    {
                        Archive = workflowLocation.ProtocolArchive,
                        ContentStream = content,
                        Name = item.DocumentName
                    });
                }
                results = await _documentService.InsertDocumentsAsync(results, idChain: idArchiveChain);
                idArchiveChain = results.First().IdChain;
            }

            return idArchiveChain;
        }


        #endregion

        #region [ NotImplemented ]
        public bool Delete(TWorkflowEntity content)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(TWorkflowEntity content)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TWorkflowEntity> Queryable(bool optimization = false)
        {
            throw new NotImplementedException();
        }

        public WorkflowResult Update(TWorkflowEntity content)
        {
            throw new NotImplementedException();
        }

        public Task<WorkflowResult> UpdateAsync(TWorkflowEntity content)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [ TriggerEntity ]
        protected abstract Task<WorkflowResult> BeforeCreateAsync(TWorkflowEntity content);

        #endregion
    }
}
