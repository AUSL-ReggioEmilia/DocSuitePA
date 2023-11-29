using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Collaborations;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Dossiers;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Fascicles;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Messages;
using VecompSoftware.Core.Command.CQRS.Commands.Models.PECMails;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Protocols;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Core.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Workflows;
using VecompSoftware.Core.Command.CQRS.Events.Models.Fascicle;
using VecompSoftware.Core.Command.CQRS.Events.Models.Integrations.GenericProcesses;
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
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.PECMails;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Model.Documents.Signs;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
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
using VecompSoftware.Services.Command.CQRS.Events.Models.Fascicles;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
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
		public const string WORKFLOW_UDS_ID = "{WorkflowUdsId}";
		public const string WORKFLOW_DOCUMENT_NAME = "{WorkflowDocumentName}";
		public const string WORKFLOW_ID_DOCUMENT = "{WorkflowIdDocument}";
		public const string WORKFLOW_OBJECT = "{WorkflowObject}";

		private const string FILE_EXTENSION_DOCX = ".docx";
		private const string FILE_EXTENSION_PDF = ".pdf";
		private const string GENERATED_FILENAME = "generated";
		private const string DATICERT_FILENAME = "daticert.xml";
		private const string SMIME_FILENAME = "smime.p7s";
		private const string SEGNATURA_FILENAME = "segnatura.xml";

		public const string DSW_FIELD_REFERENCE_MODEL_KEY_NAME = "_dsw_e_ReferenceModelKeyName";

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
		private readonly IDecryptedParameterEnvService _parameterEnvService;
		private readonly IMessageConfiguration _messageConfiguration;
		private readonly StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
		private readonly IWordOpenXmlDocumentGenerator _wordOpenXmlDocumentGenerator;
		private readonly IPDFDocumentGenerator _pdfDocumentGenerator;
		private readonly IFascicleService _fascicleService;
		private readonly IFascicleDocumentService _fascicleDocumentService;
		private readonly IFascicleDocumentUnitService _fascDocumentUnitService;
		private readonly IFascicleLinkService _fascicleLinkService;
		private readonly StorageDocument.DocumentProxy.IDocumentProxyContext _documentProxyContext;
		private readonly IFascicleFolderService _fascicleFolderService;
		private readonly ICQRSMessageMapper _cqrsMapper;
		private readonly string _passwordEncryptionKey;

		private readonly IDictionary<string, ServiceBusMessageConfiguration> _messageMappings;
		#endregion

		#region [ Properties ]   
		protected static IEnumerable<LogCategory> LogCategories
		{
			get
			{
				if (_logCategories == null)
				{
					_logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowStartService));
				}
				return _logCategories;
			}
		}

		protected DomainUserModel CurrentDomainUser { get; }

		public IdentityContext CurrentIdentityContext { get; }
		#endregion

		#region [ Constructor ]
		public WorkflowBaseService(ILogger logger, IWorkflowInstanceService workflowInstanceService,
			IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
			ITopicService topicService, ICQRSMessageMapper mapper_eventServiceBusMessage, IDataUnitOfWork unitOfWork,
			StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService, ICollaborationService collaborationService, ISecurity security,
			IDecryptedParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService, IMessageService messageService, IDossierRoleService dossierRoleService,
			IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator, IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService,
			IPDFDocumentGenerator pdfDocumentGenerator, IFascicleService fascicleService, IFascicleDocumentService fascicleDocumentService, IFascicleFolderService fascicleFolderService,
			IFascicleDocumentUnitService fascDocumentUnitService, IFascicleLinkService fascicleLinkService, IEncryptionKey encryptionKey,
			StorageDocument.DocumentProxy.IDocumentProxyContext documentProxyContext)
		{
			_instanceId = Guid.NewGuid();
			_logger = logger;
			_cqrsMapper = mapper_eventServiceBusMessage;
			_workflowActivityService = workflowActivityService;
			_workflowInstanceService = workflowInstanceService;
			_workflowInstanceRoleService = workflowInstanceRoleService;
			_topicService = topicService;
			_queueService = queueService;
			_unitOfWork = unitOfWork;
			_security = security;
			_documentService = documentService;
			_collaborationService = collaborationService;
			_protocolLogService = protocolLogService;
			_fascicleRoleService = fascicleRoleService;
			_messageService = messageService;
			_dossierRoleService = dossierRoleService;
			_wordOpenXmlDocumentGenerator = wordOpenXmlDocumentGenerator;
			_pdfDocumentGenerator = pdfDocumentGenerator;
			_fascicleService = fascicleService;
			_fascicleDocumentService = fascicleDocumentService;
			_fascDocumentUnitService = fascDocumentUnitService;
			_fascicleLinkService = fascicleLinkService;
			_documentProxyContext = documentProxyContext;
			_fascicleFolderService = fascicleFolderService;
			_parameterEnvService = parameterEnvService;
			_messageConfiguration = messageConfiguration;
			_messageMappings = _messageConfiguration.GetConfigurations();
			_passwordEncryptionKey = encryptionKey.Value;
			CurrentDomainUser = security.GetCurrentUser();
			CurrentIdentityContext = new IdentityContext(CurrentDomainUser.Account);

		}
		#endregion

		#region [ Dispose ]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
		#endregion

		#region [ Methods ]

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

		private async Task CompleteWorkflowInstanceAsync(WorkflowInstance workflowInstance)
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
			WorkflowProperty dsw_p_TenantAOOId = workflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID);

			_logger.WriteInfo(new LogMessage(
				$"WorkflowInstance {workflowInstance.WorkflowRepository.Name} [{workflowInstance.InstanceId}] in Tenant [{dsw_p_TenantName.ValueString}/{dsw_p_TenantId.ValueGuid}] completed."),
				LogCategories);

			IEventCompleteWorkflowInstance evt = new EventCompleteWorkflowInstance(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value,
			   CurrentIdentityContext, workflowInstance);
			ServiceBusMessage message = _cqrsMapper.Map(evt, new ServiceBusMessage());
			ServiceBusMessage response = await _topicService.SendToTopicAsync(message);

			_logger.WriteInfo(new LogMessage(
				$"WorkflowInstance {workflowInstance.WorkflowRepository.Name} [{workflowInstance.InstanceId}] in Tenant [{dsw_p_TenantName.ValueString}/{dsw_p_TenantId.ValueGuid}] sended notification [{response.MessageId}]."),
			   LogCategories);
		}

		protected async Task PopulateActivityAsync(WorkflowInstance workflowInstance, Guid workflowInstanceId, WorkflowRepository workflowRepository,
			IEnumerable<WorkflowProperty> inputArguments, int currentStepNumber = 0)
		{
			try
			{
				IDictionary<int, WorkflowStep> workflowSteps = JsonConvert.DeserializeObject<Dictionary<int, WorkflowStep>>(workflowInstance.Json, ServiceHelper.SerializerSettings);
				List<WorkflowMapping> workflowMappingTags = new List<WorkflowMapping>();

				if (workflowSteps.Count <= currentStepNumber)
				{
					await CompleteWorkflowInstanceAsync(workflowInstance);
					return;
				}
				Guid currentIdWorkflowActivity = Guid.NewGuid();
				WorkflowStep currentStep = workflowSteps[currentStepNumber];

				#region [ Workflow Properties initialization ]
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
				WorkflowProperty dsw_e_ActivityStartMotivation = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION);
				WorkflowProperty dsw_e_ActivityEndReferenceModel = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL);
				WorkflowProperty dsw_a_Parallel_Activity = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_PARALLEL_ACTIVITY);
				WorkflowProperty dsw_a_SetRecipientResponsible = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_SET_RECIPIENT_RESPONSIBLE);
				WorkflowProperty dsw_a_Collaboration_AddChains = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_COLLABORATION_ADD_CHAINS);
				WorkflowProperty dsw_a_Collaboration_AddProposerHierarchySigner = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_COLLABORATION_ADD_PROPOSER_HIERARCHY_SIGNER);
				WorkflowProperty dsw_p_CollaborationToManageModel = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_COLLABORATION_TO_MANAGE_MODEL);
				WorkflowProperty dsw_p_TenantId = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
				WorkflowProperty dsw_p_TenantAOOId = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID);
				WorkflowProperty dsw_p_TenantName = workflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
				WorkflowProperty dsw_p_FolderSelected = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_FOLDER_SELECTED);
				WorkflowProperty dsw_p_ReferenceUniqueId = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_REFERENCE_UNIQUEID);
				WorkflowProperty dsw_p_ReferenceEnvironment = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_REFERENCE_ENVIRONMENT);
				WorkflowProperty dsw_a_Fascicle_CopyDocumentUnits = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_COPY_DOCUMENTUNIT);
				WorkflowProperty dsw_a_Fascicle_CopyDocuments = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_COPY_DOCUMENTS);
				WorkflowProperty dsw_a_Fascicle_CopyMetadatas = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_COPY_METADATAS);
				WorkflowProperty dsw_a_Fascicle_CreateFolders = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_CREATE_FOLDERS);
				WorkflowProperty dsw_a_Fascicle_CloseReference = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_CLOSE_REFERENCE);
				WorkflowProperty dsw_a_FascicleFolderFilterLevel = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLEFOLDER_FILTER_LEVEL);
				WorkflowProperty dsw_a_Fascicle_CreateLink = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_CREATE_LINK);
				WorkflowProperty dsw_e_ActivityEndMotivation = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION);
				WorkflowProperty dsw_e_ActivityIsVisible = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_ISVISIBLE);
				WorkflowProperty dsw_e_Identity_activity = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_IDENTITY);
				WorkflowProperty dsw_p_DocumentFascicleToArchive = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_FASCICLE_TO_ARCHIVE);
				WorkflowProperty dsw_a_Metadata_HandlerDate = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_METADATA_HANDLER_DATE);
				WorkflowProperty dsw_a_Metadata_HandlerEndDate = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_METADATA_HANDLER_ENDDATE);
				WorkflowProperty dsw_p_WorkflowStorageLocationEnabled = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_STORAGELOCAION_ENABLED);
				WorkflowProperty dsw_p_DocumentFilename = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_FILENAME);
				WorkflowProperty dsw_p_RemoteSignInfo = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REMOTE_SIGN_INFO);
				WorkflowProperty dsw_p_RemoteSignLoadCredentials = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REMOTE_SIGN_LOAD_CREDENTIALS);
				WorkflowProperty dsw_p_ProviderSignType = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROVIDER_SIGN_TYPE);
				WorkflowProperty dsw_p_SignRequestType = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SIGN_REQUEST_TYPE);
				WorkflowProperty dsw_a_LoadDocuments = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_LOAD_DOCUMENTS);
				WorkflowProperty dsw_a_ArchiveDocuments = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_ARCHIVE_DOCUMENTS);
				WorkflowProperty dsw_a_CopyIdArchiveChain = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_COPY_ARCHIVE_CHAIN);


				WorkflowReferenceModel workflowReferenceModel = null;
				WorkflowReferenceModel activityReferenceModel = null;
				WorkflowProperty activityReferenceProperty = null;
				Guid? idArchiveChain = null;				
				if (dsw_p_ReferenceModel_evaluate != null && dsw_p_ReferenceModel_evaluate.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ReferenceModel_evaluate.ValueString))
				{
					workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_p_ReferenceModel_evaluate.ValueString, ServiceHelper.SerializerSettings);
				}
				if (dsw_e_ActivityStartReferenceModel != null && dsw_e_ActivityStartReferenceModel.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_ActivityStartReferenceModel.ValueString))
				{
					activityReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_e_ActivityStartReferenceModel.ValueString, ServiceHelper.SerializerSettings);
					activityReferenceProperty = dsw_e_ActivityStartReferenceModel;
				}
				if (dsw_e_ActivityEndReferenceModel != null && dsw_e_ActivityEndReferenceModel.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_ActivityEndReferenceModel.ValueString))
				{
					activityReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_e_ActivityEndReferenceModel.ValueString, ServiceHelper.SerializerSettings);
					activityReferenceProperty = dsw_e_ActivityEndReferenceModel;
				}
				if (dsw_e_ActivityEndMotivation != null && dsw_e_ActivityEndMotivation.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_ActivityEndMotivation.ValueString))
				{
					activityReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_e_ActivityEndMotivation.ValueString, ServiceHelper.SerializerSettings);
				}
				#endregion

				#region [ Validations ]
				if (dsw_p_TenantId == null || !dsw_p_TenantId.ValueGuid.HasValue || dsw_p_TenantId.ValueGuid == Guid.Empty)
				{
					throw new DSWValidationException("Evaluate properties validation error",
						new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "TenantId workflow property not defined" } },
						null, DSWExceptionCode.VA_RulesetValidation);
				}

				if (dsw_p_TenantAOOId == null || !dsw_p_TenantAOOId.ValueGuid.HasValue || dsw_p_TenantAOOId.ValueGuid == Guid.Empty)
				{
					throw new DSWValidationException("Evaluate properties validation error",
						new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "TenantAOOId workflow property not defined" } },
						null, DSWExceptionCode.VA_RulesetValidation);
				}

				if (dsw_p_TenantName == null || string.IsNullOrEmpty(dsw_p_TenantName.ValueString))
				{
					throw new DSWValidationException("Evaluate properties validation error",
						new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "TenantName workflow property not defined" } },
						null, DSWExceptionCode.VA_RulesetValidation);
				}
				#endregion

				workflowMappingTags = EvaluateMappingTags(workflowRepository, inputArguments, workflowMappingTags);

				#region[ Generic Activities ]
				if (dsw_a_CopyIdArchiveChain != null && dsw_a_CopyIdArchiveChain.PropertyType == WorkflowPropertyType.PropertyGuid && dsw_a_CopyIdArchiveChain.ValueGuid.HasValue && dsw_a_CopyIdArchiveChain.ValueGuid != Guid.Empty && currentStep.ActivityType == Model.Workflow.WorkflowActivityType.GenericActivity)
				{
					idArchiveChain = dsw_a_CopyIdArchiveChain.ValueGuid.Value;
				}

				if (!idArchiveChain.HasValue)
				{
					if (dsw_a_LoadDocuments != null && dsw_a_LoadDocuments.ValueBoolean.HasValue && dsw_a_LoadDocuments.ValueBoolean.Value && currentStep.ActivityType == Model.Workflow.WorkflowActivityType.GenericActivity)
					{
						idArchiveChain = await ArchiveAllDocumentUnitDocuments(workflowReferenceModel, idArchiveChain);
					}
					else
					{
						if (workflowReferenceModel != null && workflowReferenceModel.Documents != null && workflowReferenceModel.DocumentUnits != null && currentStep.ActivityType == Model.Workflow.WorkflowActivityType.GenericActivity)
						{
							idArchiveChain = await ArchiveDocumentFromReferences(workflowReferenceModel, idArchiveChain);
							dsw_p_ReferenceModel_evaluate.ValueString = JsonConvert.SerializeObject(workflowReferenceModel, ServiceHelper.SerializerSettings);
						}
					}

					if (activityReferenceModel != null && !string.IsNullOrEmpty(activityReferenceModel.ReferenceModel) && currentStep.ActivityType == Model.Workflow.WorkflowActivityType.GenericActivity)
					{
						idArchiveChain = await ArchiveDocument((f) => activityReferenceProperty.ValueString = f, activityReferenceModel, idArchiveChain);
					}					
				}

				if (idArchiveChain.HasValue && dsw_a_CopyIdArchiveChain != null && (!dsw_a_CopyIdArchiveChain.ValueGuid.HasValue || dsw_a_CopyIdArchiveChain.ValueGuid == Guid.Empty))
				{
					dsw_a_CopyIdArchiveChain.ValueGuid = idArchiveChain;
				}
				#endregion

				#region [ Automation Areas ]
				if (currentStep.ActivityType == Model.Workflow.WorkflowActivityType.AutomaticActivity)
				{
					await EvaluateBuildAreaAsync(workflowInstance, workflowInstanceId, currentIdWorkflowActivity, currentStep, dsw_p_ReferenceModel_evaluate, dsw_p_Model_instance,
						dsw_e_UDSId_instance, dsw_e_UDSRepositoryId_instance, dsw_p_Roles_instance, dsw_a_SetRecipientResponsible, dsw_p_TenantAOOId, dsw_p_TenantId, dsw_p_TenantName,
						dsw_a_Generate_TemplateId, dsw_a_Generate_WordTemplate, dsw_a_Generate_PDFTemplate, dsw_e_Generate_DocumentMetadatas, workflowReferenceModel, dsw_p_FolderSelected, dsw_p_ProposerRole_instance,
						dsw_e_ActivityEndMotivation, dsw_p_Accounts_instance, dsw_p_ProposerUser_instance, dsw_e_Identity_activity, dsw_p_DocumentFascicleToArchive, dsw_p_WorkflowStorageLocationEnabled,
						dsw_p_DocumentFilename, dsw_p_RemoteSignInfo, dsw_p_RemoteSignLoadCredentials, dsw_p_ProviderSignType, dsw_p_SignRequestType, dsw_p_Subject_instance, dsw_e_ActivityStartMotivation, dsw_a_ArchiveDocuments);
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
					if (workflowReferenceModel != null && !string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel))
					{
						try
						{
							DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
							if (documentUnit != null && !string.IsNullOrEmpty(documentUnit.Title))
							{
								workflowActivity.Subject = $"{workflowActivity.Subject} - {documentUnit.DocumentUnitName} n. {documentUnit.Title}-{documentUnit.Subject}";
							}
						}
						catch (Exception)
						{
						}
					}
					if (string.IsNullOrEmpty(workflowInstance.Subject))
					{
						workflowInstance.Subject = workflowActivity.Subject;
						_logger.WriteDebug(new LogMessage($"SET INSTANCE SUBJECT: {dsw_p_Subject_instance.ValueString}"), LogCategories);
					}
					_logger.WriteDebug(new LogMessage($"SET SUBJECT: {dsw_p_Subject_instance.ValueString}"), LogCategories);
				}
				if (dsw_p_Priority != null && dsw_p_Priority.ValueInt.HasValue)
				{
					WorkflowPriorityType workflowPriorityType = (WorkflowPriorityType)dsw_p_Priority.ValueInt;
					workflowActivity.Priority = workflowPriorityType;
					_logger.WriteDebug(new LogMessage($"SET PRIORITY: {dsw_p_Priority.ValueString}"), LogCategories);
				}
				if (currentStep.EvaluationArguments.Any(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_ISVISIBLE && f.ValueBoolean.HasValue && f.ValueBoolean.Value))
				{
					workflowActivity.IsVisible = true;
				}
				if (dsw_e_ActivityIsVisible != null && dsw_e_ActivityIsVisible.ValueBoolean.HasValue)
				{
					workflowActivity.IsVisible = dsw_e_ActivityIsVisible.ValueBoolean.Value;
				}
				_logger.WriteDebug(new LogMessage($"SET VISIBILITY TO {workflowActivity.IsVisible}"), LogCategories);
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
					if (workflowRole == null || workflowRole.UniqueId == Guid.Empty)
					{
						throw new DSWValidationException("Evaluate properties validation error",
							new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile autorizzare una istanza di Workflow se il modello WorkflowRole non ha un valore valido per UniqueID" } },
							null, DSWExceptionCode.VA_RulesetValidation);
					}
					Role role = _unitOfWork.Repository<Role>().GetByUniqueId(workflowRole.UniqueId).SingleOrDefault();
					if (role == null)
					{
						throw new DSWValidationException("Evaluate properties validation error",
							new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Impossibile autorizzare una istanza di Workflow se il settore {workflowRole.UniqueId} non esiste" } },
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
				if (dsw_p_TenantId != null && dsw_p_TenantId.ValueGuid.HasValue)
				{
					workflowActivity.Tenant = new Tenant() { UniqueId = dsw_p_TenantId.ValueGuid.Value };
				}
				#endregion

				workflowActivity.WorkflowActivityLogs.Add(new WorkflowActivityLog()
				{
					LogDescription = $"Richiesta esecuzione {workflowActivity.Name}",
					LogType = WorkflowActivityLogType.Progress,
					LogDate = DateTimeOffset.UtcNow,
					RegistrationUser = CurrentIdentityContext.User,
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
				WorkflowProperty tmpProp = null;
				foreach (WorkflowProperty currentProp in currentWorkflowProperties
					.Where(f => !f.Name.StartsWith(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL) ||
								f.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL) || f.Name.Equals(prop_step_referenceModel)))
				{
					tmpProp = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == currentProp.Name);
					if (tmpProp == null)
					{
						tmpProp = CloneWorkflowProperty(currentProp);
						workflowActivity.WorkflowProperties.Add(tmpProp);
					}
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
				WorkflowProperty dsw_p_CollaborationToManageModel_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_COLLABORATION_TO_MANAGE_MODEL);
				WorkflowProperty dsw_p_Model_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);

				WorkflowProperty dsw_a_Fascicle_PublicEnforcement_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_PUBLIC_ENFORCEMENT);
				WorkflowProperty dsw_a_Fascicle_PublicTemporaryEnforcement_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_PUBLIC_TEMPORARY_ENFORCEMENT);
				WorkflowProperty dsw_a_SetAuditableProperties_activity = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_SET_AUDITABLE_PROPERTIES);

				#region [ Auditable Properties ]
				if (dsw_a_SetAuditableProperties_activity != null && dsw_a_SetAuditableProperties_activity.PropertyType == WorkflowPropertyType.Json &&
					!string.IsNullOrEmpty(dsw_a_SetAuditableProperties_activity.ValueString))
				{
					workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_a_SetAuditableProperties_activity.ValueString, ServiceHelper.SerializerSettings);
					if (workflowReferenceModel != null && !string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel) && workflowReferenceModel.ReferenceType == DSWEnvironmentType.Workflow)
					{
						DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
						if (documentUnit != null && !string.IsNullOrEmpty(documentUnit.RegistrationUser) && documentUnit.RegistrationDate != DateTimeOffset.MinValue)
						{
							workflowActivity.RegistrationDate = documentUnit.RegistrationDate;
							workflowActivity.RegistrationUser = documentUnit.RegistrationUser;
							_logger.WriteDebug(new LogMessage($"set auditable properties from documentUnit {documentUnit.UniqueId}: {documentUnit.RegistrationUser}/{documentUnit.RegistrationDate}"), LogCategories);
						}
					}
				}
				IIdentityContext identity = null;
				if (dsw_e_Identity_activity != null && dsw_e_Identity_activity.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_Identity_activity.ValueString)
					&& (identity = JsonConvert.DeserializeObject<IdentityContext>(dsw_e_Identity_activity.ValueString, ServiceHelper.SerializerSettings)) != null && !string.IsNullOrEmpty(identity.User))
				{
					workflowActivity.RegistrationDate = DateTimeOffset.UtcNow;
					workflowActivity.RegistrationUser = identity.User;
				}

				#endregion

				#region [ Collaboration Managements ]
				collaborationProperties = EvaluateCollaborationArea(workflowActivity.ActivityType, dsw_p_Model_activity, dsw_p_SignerPosition_activity, dsw_p_Accounts_activity, dsw_p_SignerModel_activity, dsw_p_CollaborationToManageModel_activity);
				if (collaborationProperties != null && collaborationProperties.Any())
				{
					collaborationProperties.ForEach(f => workflowActivity.WorkflowProperties.Add(f));
				}
				#endregion

				#region [ Fascicles Managements ]
				await EvaluateAssignmentAreaAsync(currentStep, workflowInstance, workflowRepository, workflowActivity, workflowReferenceModel, dsw_p_ReferenceModel_activity,
					dsw_p_Roles_activity, dsw_p_Accounts_activity, dsw_a_Fascicle_PublicEnforcement_activity, dsw_a_Fascicle_PublicTemporaryEnforcement_activity, identity);

				if (currentStep.ActivityOperation.Area == Model.Workflow.WorkflowActivityArea.Fascicle && currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.CopyFascicleContents)
				{
					_logger.WriteDebug(new LogMessage($"Evaluating {Model.Workflow.WorkflowActivityAction.CopyFascicleContents} workflow activity action"), LogCategories);
					WorkflowProperty dsw_e_FascicleModel = inputArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_FASCICLE_FASCICLEMODEL) ?? dsw_p_ReferenceModel_instance;
					await EvaluateCopyFascicleContentsAreaAsync(dsw_p_ReferenceUniqueId, dsw_p_ReferenceEnvironment, dsw_a_Fascicle_CopyDocumentUnits, dsw_a_Fascicle_CopyDocuments, dsw_a_Fascicle_CopyMetadatas,
						dsw_a_Fascicle_CloseReference, dsw_a_FascicleFolderFilterLevel, dsw_e_FascicleModel, dsw_a_Fascicle_CreateLink, dsw_a_Fascicle_CreateFolders,
						dsw_p_TenantName, dsw_p_TenantId, dsw_p_TenantAOOId, dsw_a_Metadata_HandlerDate, dsw_a_Metadata_HandlerEndDate,
						workflowActivity.UniqueId, workflowInstance.UniqueId, workflowInstance.WorkflowRepository?.Name, identity);
				}

				if (currentStep.ActivityOperation.Area == Model.Workflow.WorkflowActivityArea.Fascicle && currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.Authorize && currentStep.ActivityType == Model.Workflow.WorkflowActivityType.ProtocolUpdate)
				{
					_logger.WriteDebug(new LogMessage($"Evaluating {Model.Workflow.WorkflowActivityAction.Authorize} workflow activity action"), LogCategories);
					await EvaluateFascicleAuthorizeProtocolAreaAsync(workflowInstance, workflowInstanceId, currentIdWorkflowActivity, workflowReferenceModel,
						dsw_p_ProposerUser_instance, dsw_p_Accounts_activity, dsw_p_TenantAOOId, dsw_p_TenantId, dsw_p_TenantName);
				}
				#endregion

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

		/// <summary>
		///     Attaches the idWorkflowActivity to the DestinationLink document unit model which has environment of DSWEnvironmentType.Workflow
		/// </summary>
		/// <typeparam name="TModel">The build model</typeparam>
		/// <param name="idWorkflowActivity">The id of the workflow activity</param>
		/// <param name="model">The model containing the WorkflowActions</param>
		private void AttachWorkflowActivityIdToDocumentUnitLink<TModel>(Guid idWorkflowActivity, TModel model)
			where TModel : IWorkflowContentBase
		{
			foreach (IWorkflowAction workflowAction in model.WorkflowActions)
			{
				WorkflowActionDocumentUnitLinkModel documentUnitLinkModel = workflowAction as WorkflowActionDocumentUnitLinkModel;

				if (documentUnitLinkModel == null)
				{
					continue;
				}

				Model.Entities.DocumentUnits.DocumentUnitModel workflowActivityDestinationLink = documentUnitLinkModel.GetDestinationLink();

				if (workflowActivityDestinationLink == null || workflowActivityDestinationLink.Environment != (int)DSWEnvironmentType.Workflow)
				{
					continue;
				}

				workflowActivityDestinationLink.UniqueId = idWorkflowActivity;
			}
		}

		#region [ Authorizations Methods ]

		protected WorkflowActivity MappingAuhtorizations<T>(WorkflowActivity workflowActivity, IEnumerable<T> results, Func<T, string> getFullAccountName)
			where T : DSWBaseEntity
		{
			if (results == null || !results.Any())
			{
				return workflowActivity;
			}
			foreach (T item in results)
			{
				string fullAccountName = getFullAccountName(item);
				if (!workflowActivity.WorkflowAuthorizations.Any(f => f.Account.Equals(fullAccountName, StringComparison.InvariantCultureIgnoreCase)))
				{
					_logger.WriteDebug(new LogMessage($"{nameof(T)} {item.UniqueId} {fullAccountName}"), LogCategories);
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
							_logger.WriteDebug(new LogMessage($"Model.Workflow.WorkflowAuthorizationType.AllRoleUser {workflowMapping.Role.UniqueId}"), LogCategories);
							IEnumerable<SecurityUser> securityUsers = _unitOfWork.Repository<RoleGroup>().GetRoleGroupsAllAuthorizationType(workflowMapping.Role.UniqueId);
							_logger.WriteDebug(new LogMessage($"Founded {securityUsers.Count()} security users"), LogCategories);
							workflowActivity = MappingAuhtorizations(workflowActivity, securityUsers, x => string.Concat(x.UserDomain, "\\", x.Account));
							break;
						}
					case Model.Workflow.WorkflowAuthorizationType.AllSecretary:
						{
							_logger.WriteDebug(new LogMessage($"Model.Workflow.WorkflowAuthorizationType.AllSecretary {workflowMapping.Role.UniqueId}"), LogCategories);
							IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Secretary, workflowMapping.Role.UniqueId,
								DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Any);
							if (!roleUsers.Any())
							{
								roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Secretary, workflowMapping.Role.UniqueId,
									DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol);
							}
							workflowActivity = MappingAuhtorizations(workflowActivity, roleUsers, (x => x.Account));
							break;
						}
					case Model.Workflow.WorkflowAuthorizationType.AllSigner:
						{
							_logger.WriteDebug(new LogMessage($"Model.Workflow.WorkflowAuthorizationType.AllSigner {workflowMapping.Role.UniqueId}"), LogCategories);
							IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(new List<string>() { RoleUserType.Vice, RoleUserType.Manager },
								workflowMapping.Role.UniqueId, DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Any);
							if (!roleUsers.Any())
							{
								roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(new List<string>() { RoleUserType.Vice, RoleUserType.Manager },
									workflowMapping.Role.UniqueId, DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol);
							}
							workflowActivity = MappingAuhtorizations(workflowActivity, roleUsers, (x => x.Account));
							break;
						}
					case Model.Workflow.WorkflowAuthorizationType.AllManager:
						{
							_logger.WriteDebug(new LogMessage($"Model.Workflow.WorkflowAuthorizationType.AllManager {workflowMapping.Role.UniqueId}"), LogCategories);

							IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Manager, workflowMapping.Role.UniqueId,
								DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Any);
							if (!roleUsers.Any())
							{
								roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Manager, workflowMapping.Role.UniqueId,
									DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Protocol);
							}
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
					case Model.Workflow.WorkflowAuthorizationType.AllPECMailBoxRoleUser:
						{
							_logger.WriteDebug(new LogMessage($"Model.Workflow.WorkflowAuthorizationType.AllPECMailBoxRoleUser {workflowMapping.Role.UniqueId}"), LogCategories);
							IEnumerable<SecurityUser> securityUsers = _parameterEnvService.RoleGroupPECRightEnabled
								? _unitOfWork.Repository<RoleGroup>().GetPECAuthorizedRoleSecurityUsers(workflowMapping.Role.UniqueId)
								: _unitOfWork.Repository<RoleGroup>().GetProtocolAuthorizedRoleSecurityUsers(workflowMapping.Role.UniqueId);
							_logger.WriteDebug(new LogMessage($"Found {securityUsers.Count()} security users"), LogCategories);
							workflowActivity = MappingAuhtorizations(workflowActivity, securityUsers, x => string.Concat(x.UserDomain, "\\", x.Account));
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
						throw new DSWValidationException("Evaluate mapping tags validation error", new List<ValidationMessageModel>()
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
							IdRole = f.Role.EntityShortId,
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


		#endregion

		#region [ Build Methods ]

		private MessageBuildModel CreateMessageBuildModel(Guid workflowInstanceId, string messageBody, string messageSubject, string[] recipientEmails
			, string senderEmail = "", Model.Entities.Messages.MessageContactType contactType = Model.Entities.Messages.MessageContactType.User, int priority = 0, string messageLink = "")
		{
			IList<MessageContactModel> recipients = new List<MessageContactModel>();
			recipientEmails = recipientEmails ?? new string[] { };
			if (recipientEmails.All(f => string.IsNullOrEmpty(f)))
			{
				_logger.WriteWarning(new LogMessage($"Email recipients has not been defined in workflow InstanceId {workflowInstanceId}"), LogCategories);
			}
			if (string.IsNullOrEmpty(senderEmail))
			{
				_logger.WriteWarning(new LogMessage($"Email sender has not been defined in workflow InstanceId {workflowInstanceId}"), LogCategories);
			}

			MessageContactEmailModel contactEmail;
			MessageContactModel contact;

			#region Sender
			if (!string.IsNullOrEmpty(senderEmail))
			{
				contactEmail = new MessageContactEmailModel()
				{
					Description = senderEmail,
					Email = senderEmail,
					User = CurrentIdentityContext.User
				};

				contact = new MessageContactModel()
				{
					ContactPosition = MessageContantTypology.Sender,
					ContactType = contactType,
					Description = senderEmail,
					MessageContactEmail = new List<MessageContactEmailModel>() { contactEmail },
				};
				recipients.Add(contact);
			}

			#endregion

			#region Recipients
			foreach (string recipient in recipientEmails.Where(f => !string.IsNullOrEmpty(f)))
			{
				contactEmail = new MessageContactEmailModel()
				{
					Description = recipient,
					Email = recipient,
					User = CurrentIdentityContext.User
				};
				contact = new MessageContactModel()
				{
					ContactPosition = MessageContantTypology.Recipient,
					ContactType = contactType,
					Description = recipient,
					MessageContactEmail = new List<MessageContactEmailModel>() { contactEmail },
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
			MessageEmailModel email = new MessageEmailModel()
			{
				Body = string.Concat(messageBody, messageLink),
				Subject = messageSubject,
				Priority = currentPriority
			};

			MessageModel message = new MessageModel()
			{
				MessageType = Model.Entities.Messages.MessageType.Email,
				MessageContacts = recipients,
				Status = Model.Entities.Messages.MessageStatus.Active,
				MessageEmails = new List<MessageEmailModel>() { email }
			};

			#endregion

			MessageBuildModel messageBuildModel = new MessageBuildModel
			{
				Message = message,
				WorkflowAutoComplete = true
			};

			return messageBuildModel;
		}

		private async Task EvaluateBuildAreaAsync(WorkflowInstance workflowInstance, Guid workflowInstanceId, Guid idWorkflowActivity, WorkflowStep currentStep,
			WorkflowProperty dsw_p_ReferenceModel, WorkflowProperty dsw_p_Model, WorkflowProperty dsw_e_UDSId, WorkflowProperty dsw_e_UDSRepositoryId, WorkflowProperty dsw_p_Roles,
			WorkflowProperty dsw_a_SetRecipientResponsible, WorkflowProperty dsw_p_TenantAOOId, WorkflowProperty dsw_p_TenantId, WorkflowProperty dsw_p_TenantName,
			WorkflowProperty dsw_a_Generate_TemplateId, WorkflowProperty dsw_a_Generate_WordTemplate, WorkflowProperty dsw_a_Generate_PdfTemplate,
			WorkflowProperty dsw_e_Generate_DocumentMetadatas, WorkflowReferenceModel workflowReferenceModel, WorkflowProperty dsw_p_FolderSelected,
			WorkflowProperty dsw_p_ProposerRole, WorkflowProperty dsw_e_ActivityEndMotivation, WorkflowProperty dsw_p_Accounts, WorkflowProperty dsw_p_ProposerUser,
			WorkflowProperty dsw_e_Identity_activity, WorkflowProperty dsw_p_DocumentFascicleToArchive, WorkflowProperty dsw_p_WorkflowStorageLocationEnabled,
			WorkflowProperty dsw_p_DocumentFilename, WorkflowProperty dsw_p_RemoteSignInfo, WorkflowProperty dsw_p_RemoteSignLoadCredentials,
			WorkflowProperty dsw_p_ProviderSignType, WorkflowProperty dsw_p_SignRequestType, WorkflowProperty dsw_p_Subject, WorkflowProperty dsw_e_ActivityStartMotivation, WorkflowProperty dsw_a_ArchiveDocuments)
		{
			if (currentStep.ActivityOperation.Area == Model.Workflow.WorkflowActivityArea.Build)
			{
				if (!workflowInstance.InstanceId.HasValue && workflowReferenceModel.ReferenceId != Guid.Empty)
				{
					workflowInstanceId = workflowReferenceModel.ReferenceId;
					workflowInstance.InstanceId = workflowReferenceModel.ReferenceId;
				}
				IIdentityContext identity = new IdentityContext(_security.GetCurrentUser().Account);

				if (dsw_e_Identity_activity != null && dsw_e_Identity_activity.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_Identity_activity.ValueString))
				{
					identity = JsonConvert.DeserializeObject<IdentityContext>(dsw_e_Identity_activity.ValueString);
				}

				if (dsw_p_ReferenceModel == null || dsw_p_ReferenceModel.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
				{
					throw new DSWValidationException("Evaluate build area validation error",
						new List<ValidationMessageModel>()
						{
							new ValidationMessageModel
								{
									Key = "WorkflowActivity",
									Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action} da workflow senza aver specificato il modello di referenza"
								}
						}, null, DSWExceptionCode.VA_RulesetValidation);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.UpdateArchive)
				{
					if (dsw_p_Model == null || dsw_e_UDSId == null || dsw_e_UDSRepositoryId == null || dsw_p_Model.PropertyType != WorkflowPropertyType.Json ||
						dsw_e_UDSId.PropertyType != WorkflowPropertyType.PropertyGuid || dsw_e_UDSRepositoryId.PropertyType != WorkflowPropertyType.PropertyGuid ||
						string.IsNullOrEmpty(dsw_p_Model.ValueString) || !dsw_e_UDSId.ValueGuid.HasValue || !dsw_e_UDSRepositoryId.ValueGuid.HasValue)
					{
						throw new DSWValidationException("Evaluate build area validation error",
							new List<ValidationMessageModel>()
							{
								new ValidationMessageModel
								{
									Key = "WorkflowActivity",
									Message = $"Impossibile avviare il proccesso di aggiornamento automatico di {currentStep.ActivityOperation.Action} da workflow senza aver specificato il modello UDS"
								}
							}, null, DSWExceptionCode.VA_RulesetValidation);
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
								IdRole = item.IdRole,
								UniqueId = item.UniqueId
							});
						}
					}

					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ServiceHelper.SerializerSettings);
				}
				if (workflowReferenceModel == null || string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel))
				{
					throw new DSWValidationException("Evaluate build area validation error",
						new List<ValidationMessageModel>()
						{
							new ValidationMessageModel
							{
								Key = "WorkflowActivity",
								Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action} da workflow senza aver specificato il modello di creazione"
							}
						}, null, DSWExceptionCode.VA_RulesetValidation);
				}

				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToMessage)
				{
					MessageBuildModel messageBuildModel = InitializeMessageBuildModel(workflowInstance, currentStep, workflowInstanceId, dsw_p_ProposerRole, dsw_p_Roles, dsw_p_Accounts, dsw_p_ProposerUser,
						workflowReferenceModel, dsw_e_ActivityStartMotivation != null && !string.IsNullOrEmpty(dsw_e_ActivityStartMotivation.ValueString)
						? dsw_e_ActivityStartMotivation.ValueString
						: dsw_e_ActivityEndMotivation != null && !string.IsNullOrEmpty(dsw_e_ActivityEndMotivation.ValueString)
							? dsw_e_ActivityEndMotivation.ValueString
							: " ");
					if (messageBuildModel == null)
					{
						_logger.WriteError(new LogMessage($"Email has not been sended in workflow InstanceId {workflowInstanceId}"), LogCategories);
					}
					WorkflowReferenceModel localReferenceModel = new WorkflowReferenceModel()
					{
						ReferenceType = DSWEnvironmentType.Any,
						ReferenceId = workflowReferenceModel.ReferenceId,
						ReferenceModel = JsonConvert.SerializeObject(messageBuildModel, ServiceHelper.SerializerSettings)
					};
					messageBuildModel = await CompleteBuildActivity<MessageBuildModel>(localReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandBuildMessage(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Message != null);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToCollaboration)
				{
					CollaborationBuildModel collaborationBuildModel = await CompleteBuildActivity<CollaborationBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandBuildCollaboration(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Collaboration != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(collaborationBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToFascicle)
				{
					FascicleBuildModel fascicleBuildModel = await CompleteBuildActivity<FascicleBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandBuildFascicle(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Fascicle != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToDossier)
				{
					DossierBuildModel dossierBuildModel = await CompleteBuildActivity<DossierBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandBuildDossier(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Dossier != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(dossierBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToProtocol)
				{
					ProtocolBuildModel protocolBuildModel = await CompleteBuildActivity<ProtocolBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandBuildProtocol(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Protocol != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.UpdateProtocol)
				{
					ProtocolBuildModel protocolBuildModel = await CompleteBuildActivity<ProtocolBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandUpdateProtocolData(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Protocol != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToPEC)
				{
					PECMailBuildModel pecMailBuildModel = await CompleteBuildActivity<PECMailBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandBuildPECMail(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.PECMail != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(pecMailBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToArchive)
				{
					UDSBuildModel udsBuildModel = null;
					if (dsw_p_DocumentFascicleToArchive != null && dsw_p_DocumentFascicleToArchive.ValueBoolean.HasValue && dsw_p_DocumentFascicleToArchive.ValueBoolean.Value)
					{
						if (workflowReferenceModel.Documents == null || workflowReferenceModel.Documents.Count > 1)
						{
							throw new DSWValidationException("Evaluate build area validation error",
								new List<ValidationMessageModel>()
								{
									new ValidationMessageModel
									{
										Key = "WorkflowActivity",
										Message = $"Impossibile avviare il flusso di lavoro in quanto il workflow prevede la selezione di un singolo elemento. "
									}
								}, null, DSWExceptionCode.VA_RulesetValidation);
						}

						WorkflowReferenceBiblosModel documentModel = workflowReferenceModel.Documents.FirstOrDefault();

						if (documentModel.ArchiveDocumentId == null || documentModel.ArchiveDocumentId == Guid.Empty || documentModel.ArchiveChainId == null || documentModel.ArchiveChainId == Guid.Empty)
						{
							throw new DSWValidationException("Evaluate build area validation error",
								new List<ValidationMessageModel>()
								{
									new ValidationMessageModel
									{
										Key = "WorkflowActivity",
										Message = $"Impossibile avviare il flusso di lavoro in quanto non è stato specificato l'id del documento o della catena documentale."
									}
								}, null, DSWExceptionCode.VA_RulesetValidation);
						}

						Guid udsId = Guid.NewGuid();
						Fascicle fascicleReferenceModel = JsonConvert.DeserializeObject<Fascicle>(workflowReferenceModel.ReferenceModel);
						workflowReferenceModel.ReferenceModel = AddWorkflowDocumentModelChainId(workflowReferenceModel.ReferenceModel, documentModel.ArchiveChainId, documentModel.ArchiveDocumentId);

						string proeprtySetter = string.Empty;
						ICollection<ArchiveDocument> archiveDocuments = await ArchiveDocumentsAsync((f) => proeprtySetter = f, workflowReferenceModel, null);
						Guid? idArchiveChain = null;
						Guid idDocument = Guid.Empty;
						if (archiveDocuments.Any(f => f.IdChain.HasValue))
						{
							idArchiveChain = archiveDocuments.First().IdChain;
							idDocument = archiveDocuments.First().IdDocument;
						}

						dsw_p_Model.ValueString = ReplaceUDSModelTokens(dsw_p_Model.ValueString, udsId.ToString(), workflowReferenceModel.Documents.FirstOrDefault().DocumentName, idDocument.ToString(), dsw_p_Subject.ValueString);
						UDSRepository repository = _unitOfWork.Repository<UDSRepository>().Find(dsw_e_UDSRepositoryId.ValueGuid);
						FascicleFolder selectedFascicleFolder = JsonConvert.DeserializeObject<FascicleFolder>(dsw_p_FolderSelected.ValueString);

						udsBuildModel = new UDSBuildModel(WebUtility.HtmlDecode(dsw_p_Model.ValueString))
						{
							UDSRepository = new UDSRepositoryModel(dsw_e_UDSRepositoryId.ValueGuid.Value)
							{
								DSWEnvironment = repository.DSWEnvironment,
								Name = repository.Name
							},
							RegistrationUser = identity.User,
							Documents = workflowReferenceModel.Documents.Select(x => new UDSDocumentModel()
							{
								BiblosArchive = idDocument,
								DocumentName = x.DocumentName,
								IdChain = idArchiveChain.Value,
								DocumentType = UDSDocumentType.Document
							}).ToList(),
							WorkflowActions = new List<IWorkflowAction>
							{
								new WorkflowActionFascicleModel(
									new FascicleModel { UniqueId = fascicleReferenceModel.UniqueId },
									new Model.Entities.DocumentUnits.DocumentUnitModel { UniqueId = udsId, Environment = repository.DSWEnvironment },
									new FascicleFolderModel { UniqueId = selectedFascicleFolder.UniqueId })
							}
						};

						if (fascicleReferenceModel.FascicleRoles != null && fascicleReferenceModel.FascicleRoles.Count > 0)
						{
							foreach (FascicleRole role in fascicleReferenceModel.FascicleRoles.Where(x => x.IsMaster))
							{
								udsBuildModel.Roles.Add(new RoleModel()
								{
									AuthorizationType = Model.Commons.AuthorizationRoleType.Responsible,
									IdRole = role.Role.EntityShortId,
									UniqueId = role.Role.UniqueId
								});
							}
						}

						workflowReferenceModel = new WorkflowReferenceModel
						{
							ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ServiceHelper.SerializerSettings)
						};
					}
					udsBuildModel = await CompleteBuildActivity<UDSBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandInsertUDSData(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => !string.IsNullOrEmpty(model.XMLContent));
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.UpdateArchive)
				{
					UDSBuildModel udsBuildModel = await CompleteBuildActivity<UDSBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandUpdateUDSData(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => !string.IsNullOrEmpty(model.XMLContent));
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.UpdateFascicle)
				{
					FascicleBuildModel fascicleBuildModel = await CompleteBuildActivity<FascicleBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandUpdateFascicleData(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Fascicle != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(fascicleBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.CancelProtocol)
				{
					ProtocolBuildModel protocolBuildModel = await CompleteBuildActivity<ProtocolBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandDeleteProtocol(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => model.Protocol != null);
					workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ServiceHelper.SerializerSettings);
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.CancelArchive)
				{
					UDSBuildModel udsBuildModel = await CompleteBuildActivity<UDSBuildModel>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new CommandDeleteUDSData(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model),
						(model) => !string.IsNullOrEmpty(model.XMLContent));
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToShare)
				{
					UserLog userLog = _unitOfWork.Repository<UserLog>().GetBySystemUser(CurrentDomainUser.Account);
					if (userLog == null)
					{
						throw new DSWValidationException("Evaluate build area validation error",
							new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"UserLog not found for Account: {CurrentDomainUser.Account}" } },
							null, DSWExceptionCode.VA_RulesetValidation);
					}

					Tenant currentTenant = _unitOfWork.Repository<Tenant>().GetByUniqueId(userLog.CurrentTenantId).FirstOrDefault();
					if (currentTenant == null)
					{
						throw new DSWValidationException("Evaluate build area validation error",
							new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Tenant not found for TenantId: {userLog.CurrentTenantId}" } },
							null, DSWExceptionCode.VA_RulesetValidation);
					}

					DocumentUnit documentUnit = await CompleteBuildEvent<DocumentUnit>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new EventShareDocumentUnit(Guid.NewGuid(), workflowReferenceModel.ReferenceId, currentTenant.TenantName, currentTenant.UniqueId, currentTenant.TenantAOO.UniqueId, identity, model, DateTimeOffset.UtcNow.AddSeconds(5)));
					if (documentUnit.Environment == 1)
					{
						ProtocolLog protocolLog = await _protocolLogService.CreateAsync(new ProtocolLog()
						{
							LogDescription = $"Protocollo {documentUnit.Title} condiviso con l'integrazione {workflowInstance.WorkflowRepository.Name}",
							Entity = new Protocol() { UniqueId = documentUnit.UniqueId }
						}, InsertActionType.ProtocolShared);
					}
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.ToIntegration)
				{
					if (dsw_p_RemoteSignLoadCredentials != null && dsw_p_RemoteSignLoadCredentials.ValueBoolean != null && dsw_p_RemoteSignLoadCredentials.ValueBoolean.Value)
					{
						if (dsw_p_ProviderSignType == null || !dsw_p_ProviderSignType.ValueInt.HasValue)
						{
							throw new DSWValidationException("Evaluate build area validation error",
								new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Missing parameter dsw_p_RemoteSignLoadCredentials" } },
								null, DSWExceptionCode.VA_RulesetValidation);
						}

						UserLog userLog = _unitOfWork.Repository<UserLog>().GetBySystemUser(identity.User);
						if (userLog == null)
						{
							throw new DSWValidationException("Evaluate build area validation error",
								new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"UserLog not found for Account: {CurrentDomainUser.Account}" } },
								null, DSWExceptionCode.VA_RulesetValidation);
						}

						if (string.IsNullOrEmpty(userLog.UserProfile))
						{
							throw new DSWValidationException("Evaluate build area validation error",
								new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"No UserProfile was found for Account: {CurrentDomainUser.Account}" } },
								null, DSWExceptionCode.VA_RulesetValidation);
						}

						ProviderSignType userProviderSignType = (ProviderSignType)dsw_p_ProviderSignType.ValueInt;
						userLog.UserProfile = EncryptionHelper.DecryptString(userLog.UserProfile, _passwordEncryptionKey);
						UserProfile userProfile = JsonConvert.DeserializeObject<UserProfile>(userLog.UserProfile);
						RemoteSignProperty userProfileSignProperty = userProfile.Value[userProviderSignType];

						if (userProfileSignProperty == null)
						{
							throw new DSWValidationException("Evaluate build area validation error",
								new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"User without {userProviderSignType} sign provider configured" } },
								null, DSWExceptionCode.VA_RulesetValidation);
						}

						switch (userProviderSignType)
						{
							case ProviderSignType.ArubaAutomatic:
							case ProviderSignType.InfocertAutomatic:
								userProfileSignProperty.RemoteSignType = RemoteSignType.Automatic;
								break;

							case ProviderSignType.ArubaRemote:
							case ProviderSignType.InfocertRemote:
								userProfileSignProperty.RemoteSignType = RemoteSignType.Remote;
								break;
							case ProviderSignType.ArubaSeal:
							case ProviderSignType.InfocertSeal:
								userProfileSignProperty.RemoteSignType = RemoteSignType.Seal;
								break;
						}

						DocSuiteEvent docsuiteEvent = JsonConvert.DeserializeObject<DocSuiteEvent>(workflowReferenceModel.ReferenceModel);
						DocumentManagementRequestModel documentManagementRequestModel = JsonConvert.DeserializeObject<DocumentManagementRequestModel>(docsuiteEvent.EventModel.CustomProperties[nameof(DocumentManagementRequestModel)]);

						if (!string.IsNullOrEmpty(documentManagementRequestModel.UserProfileRemoteSignProperty?.OTP))
						{
							userProfileSignProperty.OTP = documentManagementRequestModel.UserProfileRemoteSignProperty.OTP;
						}

						documentManagementRequestModel.UserProfileRemoteSignProperty = userProfileSignProperty;
						IDictionary<string, string> signProvider = documentManagementRequestModel.UserProfileRemoteSignProperty.CustomProperties;

						signProvider[RemoteSignProperty.PROVIDER_TYPE] = dsw_p_ProviderSignType.ValueInt.ToString();

						if (dsw_p_SignRequestType != null && dsw_p_SignRequestType.ValueInt.HasValue)
						{
							signProvider[RemoteSignProperty.REQUEST_TYPE] = dsw_p_SignRequestType.ValueInt.ToString();
						}

						if (dsw_a_ArchiveDocuments != null && !string.IsNullOrWhiteSpace(dsw_a_ArchiveDocuments.ValueString))
						{
							documentManagementRequestModel.Documents = workflowReferenceModel.Documents;
						}

						docsuiteEvent.EventModel.CustomProperties[nameof(DocumentManagementRequestModel)] = JsonConvert.SerializeObject(documentManagementRequestModel);
						workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(docsuiteEvent);
					}

					DocSuiteEvent docSuiteEvent = await CompleteBuildEvent<DocSuiteEvent>(workflowReferenceModel, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new EventIntegrationRequest(Guid.NewGuid(), workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model, null));
				}
				if (currentStep.ActivityOperation.Action == Model.Workflow.WorkflowActivityAction.GenerateReport)
				{
					if (dsw_a_Generate_TemplateId == null || dsw_a_Generate_TemplateId.PropertyType != WorkflowPropertyType.PropertyGuid || dsw_a_Generate_TemplateId.ValueGuid == null)
					{
						throw new DSWValidationException("Evaluate build area validation error",
							new List<ValidationMessageModel>()
							{
								new ValidationMessageModel
								{
									Key = "WorkflowActivity",
									Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action} da workflow senza aver specificato il modello di TemplateId"
								}
							},
							null, DSWExceptionCode.VA_RulesetValidation);
					}
					if (dsw_e_Generate_DocumentMetadatas == null || dsw_e_Generate_DocumentMetadatas.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_e_Generate_DocumentMetadatas.ValueString))
					{
						throw new DSWValidationException("Evaluate build area validation error",
							new List<ValidationMessageModel>()
							{
								new ValidationMessageModel
								{
									Key = "WorkflowActivity",
									Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action} da workflow senza aver specificato il modello di DocumentMetadatas"
								}
							}, null, DSWExceptionCode.VA_RulesetValidation);
					}
					if (workflowReferenceModel.ReferenceType == DSWEnvironmentType.Fascicle && (dsw_p_FolderSelected == null || dsw_p_FolderSelected.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_FolderSelected.ValueString)))
					{
						throw new DSWValidationException("Evaluate build area validation error",
							new List<ValidationMessageModel>()
							{
								new ValidationMessageModel
								{
									Key = "WorkflowActivity",
									Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action} da workflow senza aver specificato il modello di SelectedFascicleFolder"
								}
							},
							null, DSWExceptionCode.VA_RulesetValidation);
					}

					bool generateWordTemplateIsMissing = dsw_a_Generate_WordTemplate == null
						|| dsw_a_Generate_WordTemplate.PropertyType != WorkflowPropertyType.PropertyBoolean
						|| !dsw_a_Generate_WordTemplate.ValueBoolean.HasValue;
					bool generatePdfTemplateIsMissing = dsw_a_Generate_PdfTemplate == null
						|| dsw_a_Generate_PdfTemplate.PropertyType != WorkflowPropertyType.PropertyBoolean
						|| !dsw_a_Generate_PdfTemplate.ValueBoolean.HasValue;

					if (generateWordTemplateIsMissing && generatePdfTemplateIsMissing)
					{
						throw new DSWValidationException("Evaluate build area validation error",
							new List<ValidationMessageModel>()
							{
								new ValidationMessageModel
								{
									Key = "WorkflowActivity",
									Message = $"Impossibile avviare il proccesso di creazione automatica di {currentStep.ActivityOperation.Action} da workflow senza aver specificato il modello di WordTemplate o PdfTemplate"
								}
							}, null, DSWExceptionCode.VA_RulesetValidation);
					}

					ArchiveDocument archiveDocument = await GenerateReport(dsw_a_Generate_TemplateId, dsw_a_Generate_WordTemplate, dsw_a_Generate_PdfTemplate,
						dsw_e_Generate_DocumentMetadatas, workflowReferenceModel, dsw_p_FolderSelected, dsw_p_WorkflowStorageLocationEnabled, dsw_p_DocumentFilename);
					workflowReferenceModel.Documents.Add(new WorkflowReferenceBiblosModel
					{
						ArchiveChainId = archiveDocument.IdChain,
						ArchiveDocumentId = archiveDocument.IdDocument,
						ArchiveName = archiveDocument.Archive,
						ChainType = Model.Entities.DocumentUnits.ChainType.Miscellanea,
						Simulation = true,
						DocumentName = archiveDocument.Name
					});

					RemoteSignProperty remoteSign = new RemoteSignProperty();
					if (dsw_p_RemoteSignInfo != null && dsw_p_RemoteSignInfo.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_RemoteSignInfo.ValueString))
					{
						remoteSign = JsonConvert.DeserializeObject<RemoteSignProperty>(dsw_p_RemoteSignInfo.ValueString, ServiceHelper.SerializerSettings);
					}

					DocumentManagementRequestModel documentManagementRequestModel = new DocumentManagementRequestModel
					{
						Documents = workflowReferenceModel.Documents,
						DocumentUnit = workflowReferenceModel,
						UserProfileRemoteSignProperty = remoteSign
					};

					DocSuiteEvent evt = new DocSuiteEvent
					{
						WorkflowAutoComplete = true,
						EventDate = DateTimeOffset.UtcNow,
						WorkflowReferenceId = workflowReferenceModel.ReferenceId,
						EventModel = new DocSuiteModel
						{
							CustomProperties = new Dictionary<string, string>
							{
								{ nameof(DocumentManagementRequestModel), JsonConvert.SerializeObject(documentManagementRequestModel, ServiceHelper.SerializerSettings) }
							}
						}
					};

					DocSuiteEvent docSuiteEvent = await CompleteBuildEvent(evt, workflowInstanceId, idWorkflowActivity, workflowInstance.WorkflowRepository.Name,
						(model) => new EventCompleteGenerateReport(Guid.NewGuid(), workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, model, null));
				}
				dsw_p_ReferenceModel.ValueString = JsonConvert.SerializeObject(workflowReferenceModel, ServiceHelper.SerializerSettings);
			}
		}

		private async Task<IDictionary<int, WorkflowStep>> CreateCollaborationActivityAsync(CollaborationModel collaborationModel, WorkflowProperty dsw_e_CollaborationManaged,
			WorkflowInstance workflowInstance, IDictionary<int, WorkflowStep> workflowSteps, int currentStepNumber, List<WorkflowMapping> workflowMappingTags)
		{
			int localCurrentStepNumber = currentStepNumber;
			int beforeCurrentStep = localCurrentStepNumber;
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
				DocumentType = collaborationModel.DocumentType,
				IdPriority = collaborationModel.IdPriority,
				IdStatus = collaborationModel.IdStatus,
				SignCount = collaborationModel.SignCount,
				Subject = collaborationModel.Subject,
				Location = collaborationLocation,
				TemplateName = collaborationModel.TemplateName,
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
						new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL, PropertyType = ArgumentType.Json },
						new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, PropertyType = ArgumentType.PropertyInt },
						new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, PropertyType = ArgumentType.Json },
						new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, PropertyType = ArgumentType.PropertyInt },
						new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE, PropertyType = ArgumentType.PropertyBoolean },
					},
					EvaluationArguments = new List<WorkflowArgument>()
					{
						new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_ISVISIBLE, PropertyType = ArgumentType.PropertyBoolean, ValueBoolean = true},
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
				ArchiveDocument archiveDocument = new ArchiveDocument
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
					default:
						{
							activityType = Model.Workflow.WorkflowActivityType.CollaborationToProtocol;
							break;
						}
				}
			}

			_logger.WriteDebug(new LogMessage($"Set collaboration managed to {workflowActivityOperation.Action}"), LogCategories);

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
					new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID, PropertyType = ArgumentType.PropertyGuid }
				},
				EvaluationArguments = new List<WorkflowArgument>()
				{
					new WorkflowArgument() { Name = WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_ISVISIBLE, PropertyType = ArgumentType.PropertyBoolean, ValueBoolean = true},
				}
			});

			WorkflowProperty dsw_a_Activity_ManualComplete = workflowInstance.WorkflowProperties.SingleOrDefault(x => x.Name == WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE);
			if (dsw_a_Activity_ManualComplete != null)
			{
				workflowSteps[workflowSteps.Count - 1].OutputArguments.Add(new WorkflowArgument()
				{
					Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE,
					PropertyType = ArgumentType.PropertyBoolean,
					ValueBoolean = dsw_a_Activity_ManualComplete.ValueBoolean
				});
			}
			#endregion

			collaboration.WorkflowInstance = workflowInstance;
			collaboration = await _collaborationService.CreateAsync(collaboration);

			collaborationModel.IdCollaboration = collaboration.EntityId;
			_logger.WriteDebug(new LogMessage($"Collaboration {collaboration.EntityId} [{collaboration.UniqueId}] has been successfully created"), LogCategories);
			for (int i = beforeCurrentStep + 1; i < workflowSteps.Count; i++)
			{
				workflowSteps[i].Name += $" nella collaborazione {collaborationModel.IdCollaboration}";
			}
			return workflowSteps;
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
					if (dsw_a_Collaboration_AddProposerHierarchySigner != null && dsw_a_Collaboration_AddProposerHierarchySigner.ValueBoolean.HasValue && dsw_a_Collaboration_AddProposerHierarchySigner.ValueBoolean.Value &&
						dsw_p_ProposerRole != null && dsw_p_ProposerRole.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ProposerRole.ValueString))
					{
						WorkflowRole workflowRole = JsonConvert.DeserializeObject<WorkflowRole>(dsw_p_ProposerRole.ValueString, ServiceHelper.SerializerSettings);
						if (workflowRole == null || workflowRole.UniqueId == Guid.Empty)
						{
							throw new DSWValidationException("Evaluate collaboration validation error",
								new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowRole", Message = $"Il settore proponente della collaborazione {workflowRole?.Name}({workflowRole?.UniqueId}) non esiste" } },
								null, DSWExceptionCode.VA_RulesetValidation);
						}
						proposerRole = _unitOfWork.Repository<Role>().GetByUniqueId(workflowRole.UniqueId).SingleOrDefault();
						if (proposerRole == null)
						{
							throw new DSWValidationException("Evaluate collaboration validation error",
								new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowRole", Message = $"Il settore proponente della collaborazione {workflowRole?.Name}({workflowRole?.UniqueId}) non esiste" } },
								null, DSWExceptionCode.VA_RulesetValidation);
						}
						proposerSigner = _unitOfWork.Repository<RoleUser>().GetFirstHierarchySigner(proposerRole.FullIncrementalPath, DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Any);
						_logger.WriteDebug(new LogMessage($"Proposer signer hierarchy founded({proposerSigner != null}) {proposerSigner?.Account} by role {proposerRole.Name}"), LogCategories);
					}
					TemplateCollaboration templateCollaboration = _unitOfWork.Repository<TemplateCollaboration>().GetWithRelations(dsw_p_TemplateCollaboration.ValueGuid.Value, optimization: true);
					if (templateCollaboration.DocumentType == CollaborationDocumentType.Protocol && dsw_p_CollaborationToManageModel != null && dsw_p_CollaborationToManageModel.PropertyType == WorkflowPropertyType.Json &&
						!string.IsNullOrEmpty(dsw_p_CollaborationToManageModel.ValueString))
					{
						ProtocolModel protocolModel = JsonConvert.DeserializeObject<ProtocolModel>(dsw_p_CollaborationToManageModel.ValueString, ServiceHelper.SerializerSettings);
						protocolModel.Object = dsw_p_Subject?.ValueString ?? templateCollaboration.Object;
						dsw_p_CollaborationToManageModel.ValueString = JsonConvert.SerializeObject(protocolModel, ServiceHelper.SerializerSettings);
					}
					DomainUserModel domainUserModel;
					collaborationModel = new CollaborationModel()
					{
						DocumentType = templateCollaboration.DocumentType,
						TemplateName = templateCollaboration.Name,
						IdStatus = CollaborationStatusType.Insert,
						IdPriority = "N",
						RegistrationName = CurrentDomainUser.DisplayName,
						Subject = dsw_p_Subject?.ValueString ?? templateCollaboration.Object,
						Note = templateCollaboration.Note
					};
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
					}
					foreach (TemplateCollaborationUser item in templateCollaboration.TemplateCollaborationUsers.Where(f => f.UserType == TemplateCollaborationUserType.Signer).OrderBy(f => f.Incremental))
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
					}
					collaborationModel.SignCount = (short)collaborationModel.CollaborationSigns.Count();
					incremental = 1;
					if (proposerRole != null && !templateCollaboration.TemplateCollaborationUsers.Any(f => f.UserType == TemplateCollaborationUserType.Secretary && f.Role.EntityShortId == proposerRole.EntityShortId))
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
						workflowMappingTags.Add(new WorkflowMapping()
						{
							AuthorizationType = Model.Workflow.WorkflowAuthorizationType.AllSecretary,
							Role = new WorkflowRole()
							{
								EmailAddress = proposerRole.EMailAddress,
								IdRole = proposerRole.EntityShortId,
								UniqueId = proposerRole.UniqueId,
								Name = proposerRole.Name,
								Required = false
							}
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
						workflowMappingTags.Add(new WorkflowMapping()
						{
							AuthorizationType = Model.Workflow.WorkflowAuthorizationType.AllSecretary,
							Role = new WorkflowRole()
							{
								EmailAddress = item.Role.EMailAddress,
								IdRole = item.Role.EntityShortId,
								UniqueId = item.Role.UniqueId,
								Name = item.Role.Name,
								Required = item.IsRequired
							}
						});
					}
				}

				if (dsw_p_Model != null && dsw_p_Model.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_Model.ValueString))
				{
					try
					{
						collaborationModel = JsonConvert.DeserializeObject<CollaborationModel>(dsw_p_Model.ValueString, ServiceHelper.SerializerSettings);
					}
					catch (Exception) { }
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
				if (!string.IsNullOrEmpty(document.Key) && document.Value != null)
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

					if (documentUnit.DocumentUnitChains.Any(f => f.ChainType == ChainType.MainChain))
					{
						documents = await _documentService.GetDocumentLatestVersionFromChainAsync(documentUnit.DocumentUnitChains.Single(f => f.ChainType == ChainType.MainChain).IdArchiveChain);
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
					dsw_p_Model = new WorkflowProperty()
					{
						WorkflowType = WorkflowType.Activity,
						PropertyType = WorkflowPropertyType.Json,
						Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL
					};
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

		private List<WorkflowProperty> EvaluateCollaborationArea(WorkflowActivityType activityType, WorkflowProperty dsw_p_Model, WorkflowProperty dsw_p_SignerPosition,
			WorkflowProperty dsw_p_Accounts, WorkflowProperty dsw_p_SignerModel, WorkflowProperty dsw_p_CollaborationToManageModel)
		{
			List<WorkflowProperty> results = new List<WorkflowProperty>();
			if (activityType == WorkflowActivityType.CollaborationSign)
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
				_logger.WriteDebug(new LogMessage($"Evaluate collaboration sign position {dsw_p_SignerPosition.ValueInt.Value}"), LogCategories);
				CollaborationSignModel collaborationSignModel = collaborationModel.CollaborationSigns.OrderBy(f => f.Incremental).ElementAt((int)dsw_p_SignerPosition.ValueInt.Value);
				_logger.WriteDebug(new LogMessage($"Evaluate collaboration sign model {collaborationSignModel.SignUser}"), LogCategories);
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

		private MessageBuildModel InitializeMessageBuildModel(WorkflowInstance workflowInstance, WorkflowStep currentStep, Guid workflowInstanceId,
			WorkflowProperty dsw_p_ProposerRole, WorkflowProperty dsw_p_Roles, WorkflowProperty dsw_p_Accounts, WorkflowProperty dsw_p_ProposerUser,
			WorkflowReferenceModel workflowReferenceModel, string responseMessage)
		{
			WorkflowArgument dsw_e_MessageBody = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_BODY);
			WorkflowArgument dsw_e_MessageSubject = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_SUBJECT);
			WorkflowArgument dsw_e_MessagePriority = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_PRIORITY);
			WorkflowArgument dsw_e_MessageLinkType = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_MESSAGE_LINK_TYPE);
			WorkflowArgument dsw_e_ReferenceModelKeyName = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == DSW_FIELD_REFERENCE_MODEL_KEY_NAME);
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
				if (workflowRole == null || workflowRole.UniqueId == Guid.Empty || (role = _unitOfWork.Repository<Role>().GetByUniqueId(workflowRole.UniqueId, optimization: true).SingleOrDefault()) == null)
				{
					throw new DSWException(string.Concat("Proposer UniqueId ", workflowRole?.UniqueId, " not defined/found on email generation"), null, DSWExceptionCode.WF_RulesetValidation);
				}
				senderEmail = role.EMailAddress?.Split(';').First();
			}
			if (dsw_p_ProposerUser != null && dsw_p_ProposerUser.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ProposerUser.ValueString))
			{
				proposerUser = JsonConvert.DeserializeObject<WorkflowAccount>(dsw_p_ProposerUser.ValueString, ServiceHelper.SerializerSettings);
				senderEmail = proposerUser.EmailAddress;
			}
			WorkflowProperty tmp;
			if (dsw_e_ReferenceModelKeyName != null && !string.IsNullOrWhiteSpace(dsw_e_ReferenceModelKeyName.ValueString) && (tmp = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == dsw_e_ReferenceModelKeyName.ValueString)) != null &&
				!string.IsNullOrEmpty(tmp.ValueString))
			{
				workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(tmp.ValueString, ServiceHelper.SerializerSettings);
				_logger.WriteDebug(new LogMessage($"Used workflow property {dsw_e_ReferenceModelKeyName.ValueString} instead of current reference model"), LogCategories);
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
									.Select(f => f == null && f.UniqueId == Guid.Empty ? null : _unitOfWork.Repository<Role>().GetByUniqueId(f.UniqueId, optimization: true).SingleOrDefault())
									.Where(f => f != null && !string.IsNullOrEmpty(f.EMailAddress))
									.SelectMany(f => f.EMailAddress.Split(';'))
									.ToArray();
			}
			if (dsw_p_EmailEvaluationRecipient != null && dsw_p_EmailEvaluationRecipient.ValueString == WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS &&
				dsw_p_Accounts != null && dsw_p_Accounts.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_Accounts.ValueString))
			{
				ICollection<WorkflowAccount> accounts = JsonConvert.DeserializeObject<ICollection<WorkflowAccount>>(dsw_p_Accounts.ValueString, ServiceHelper.SerializerSettings);
				foreach (WorkflowAccount item in accounts.Where(f => string.IsNullOrEmpty(f.EmailAddress)))
				{
					UserLog userLog = _unitOfWork.Repository<UserLog>().GetBySystemUser(item.AccountName);
					item.EmailAddress = userLog?.UserMail;
					_logger.WriteDebug(new LogMessage($"Evaluate email {item.AccountName} using UserLog {userLog?.UserMail}"), LogCategories);
				}

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
			if (dsw_p_EmailEvaluationRecipient != null && !string.IsNullOrEmpty(dsw_p_EmailEvaluationRecipient.ValueString) && dsw_p_EmailEvaluationRecipient.ValueString.Contains("@"))
			{
				recipientEmails = dsw_p_EmailEvaluationRecipient.ValueString.Split(';');
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
							if (workflowReferenceModel != null && !string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel))
							{
								Fascicle fascicle = JsonConvert.DeserializeObject<Fascicle>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
								if (fascicle != null)
								{
									linkToken0 = fascicle.UniqueId.ToString();
									linkToken1 = $"Fascicolo n. {fascicle.Title}-{fascicle.FascicleObject} del {fascicle.RegistrationDate.ToLocalTime().Date.ToShortDateString()}";
								}
							}
							break;
						}
					case WorkflowMessageLinkType.ProtocolLink:
						{
							if (workflowReferenceModel != null && !string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel))
							{
								DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
								if (documentUnit != null)
								{
									linkToken0 = documentUnit.UniqueId.ToString();
									linkToken1 = $"{documentUnit.DocumentUnitName} n. {documentUnit.Title}-{documentUnit.Subject} del {documentUnit.RegistrationDate.ToLocalTime().Date.ToShortDateString()}";
								}
							}
							break;
						}
					case WorkflowMessageLinkType.CreatedFascicleLink:
						{
							if (workflowReferenceModel != null)
							{
								Fascicle fascicle = _unitOfWork.Repository<Fascicle>().GetByUniqueId(workflowReferenceModel.ReferenceId);
								if (fascicle != null)
								{
									linkToken0 = fascicle.UniqueId.ToString();
									linkToken1 = $"Fascicolo n. {fascicle.Title}-{fascicle.FascicleObject} del {fascicle.RegistrationDate.ToLocalTime().Date.ToShortDateString()}";
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
							if (workflowReferenceModel != null && !string.IsNullOrEmpty(workflowReferenceModel.ReferenceModel))
							{
								try
								{
									DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
									if (documentUnit != null && !string.IsNullOrEmpty(documentUnit.Title))
									{
										linkToken1 = $"{linkToken1} <br/> <b>{documentUnit.DocumentUnitName} n. {documentUnit.Title}-{documentUnit.Subject} del {documentUnit.RegistrationDate.ToLocalTime().Date.ToShortDateString()}</b>";
									}
								}
								catch (Exception)
								{
								}
							}
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

			MessageBuildModel messageBuildModel = CreateMessageBuildModel(workflowInstanceId, dsw_e_MessageBody?.ValueString,
				dsw_e_MessageSubject?.ValueString, recipientEmails, senderEmail: senderEmail, contactType: Model.Entities.Messages.MessageContactType.Role,
				priority: priority, messageLink: messageLink);
			return messageBuildModel;
		}

		private async Task EvaluateCopyFascicleContentsAreaAsync(WorkflowProperty dsw_p_ReferenceFascicleId, WorkflowProperty dsw_p_ReferenceEnvironment, WorkflowProperty dsw_a_Fascicle_CopyDocumentUnits,
			WorkflowProperty dsw_a_Fascicle_CopyDocuments, WorkflowProperty dsw_a_Fascicle_CopyMetadatas, WorkflowProperty dsw_a_Fascicle_CloseReference, WorkflowProperty dsw_a_FascicleFolderFilterLevel,
			WorkflowProperty dsw_p_ReferenceModel, WorkflowProperty dsw_a_Fascicle_CreateLink, WorkflowProperty dsw_a_Fascicle_CreateFolders, WorkflowProperty dsw_p_TenantName,
			WorkflowProperty dsw_p_TenantId, WorkflowProperty dsw_p_TenantAOOId, WorkflowProperty dsw_a_Metadata_HandlerDate, WorkflowProperty dsw_a_Metadata_HandlerEndDate,
			Guid workflowActivityId, Guid workflowInstanceId, string workflowName, IIdentityContext identity)
		{
			#region [ Validations ]
			if (dsw_p_ReferenceFascicleId == null || !dsw_p_ReferenceFascicleId.ValueGuid.HasValue)
			{
				throw new DSWValidationException("Evaluate copy fascicle contents validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Undefined Reference Fascicle id" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			if (dsw_p_ReferenceEnvironment == null || !dsw_p_ReferenceEnvironment.ValueInt.HasValue)
			{
				throw new DSWValidationException("Evaluate copy fascicle contents validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Undefined Reference Environment" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			if (dsw_p_ReferenceEnvironment.ValueInt.Value != (int)DSWEnvironmentType.Fascicle)
			{
				throw new DSWValidationException("Evaluate copy fascicle contents validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel {
						Key = "WorkflowActivity",
						Message = $"Invalid Reference Environment {dsw_p_ReferenceEnvironment.ValueInt.Value} for fascicle evaluation" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			if (dsw_p_ReferenceModel == null || string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
			{
				throw new DSWValidationException("Evaluate copy fascicle contents validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Undefined Destination Fascicle model" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			FascicleModel destinationFascicleModel = JsonConvert.DeserializeObject<FascicleModel>(dsw_p_ReferenceModel.ValueString);
			Guid referenceFascicleId = dsw_p_ReferenceFascicleId.ValueGuid.Value;

			_logger.WriteDebug(new LogMessage($"Fetching Reference and Destination fascicle entities"), LogCategories);

			Fascicle referenceFascicleEntity = _unitOfWork.Repository<Fascicle>().Find(referenceFascicleId);
			Fascicle destinationFascicleEntity = _unitOfWork.Repository<Fascicle>().Find(destinationFascicleModel.UniqueId);

			if (referenceFascicleEntity == null)
			{
				throw new DSWValidationException("Evaluate copy fascicle contents validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Reference Fascicle with id {referenceFascicleId} not found" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			if (destinationFascicleEntity == null)
			{
				throw new DSWValidationException("Evaluate copy fascicle contents validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Destination Fascicle with id {destinationFascicleModel.UniqueId} not found" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			#endregion

			#region [ Copy FascicleDocumentUnits and FascicleDocuments from Reference fascicle folder into Destination fascicle folder] 
			bool copyFascicleDocumentUnits = dsw_a_Fascicle_CopyDocumentUnits != null && dsw_a_Fascicle_CopyDocumentUnits.ValueBoolean.HasValue && dsw_a_Fascicle_CopyDocumentUnits.ValueBoolean.Value;
			bool copyFascicleDocuments = dsw_a_Fascicle_CopyDocuments != null && dsw_a_Fascicle_CopyDocuments.ValueBoolean.HasValue && dsw_a_Fascicle_CopyDocuments.ValueBoolean.Value;
			int? fascicleFolderFilterLevel = dsw_a_FascicleFolderFilterLevel != null && dsw_a_FascicleFolderFilterLevel.ValueInt.HasValue
				? (int?)dsw_a_FascicleFolderFilterLevel.ValueInt.Value : null;

			await CloneFascicleFoldersContentsAsync(copyFascicleDocumentUnits, copyFascicleDocuments, referenceFascicleEntity.UniqueId, destinationFascicleEntity, fascicleFolderFilterLevel);
			#endregion

			await CloneFascicleMetadataValuesAsync(referenceFascicleEntity, destinationFascicleEntity, dsw_a_Fascicle_CopyMetadatas, dsw_a_Metadata_HandlerDate, dsw_a_Metadata_HandlerEndDate);

			#region [ Evaluate dsw_a_CloseReferenceFascicle Argument ]
			if (dsw_a_Fascicle_CloseReference != null && dsw_a_Fascicle_CloseReference.ValueBoolean.HasValue && dsw_a_Fascicle_CloseReference.ValueBoolean.Value)
			{
				_logger.WriteDebug(new LogMessage($"Closing Reference Fascicle {referenceFascicleEntity.UniqueId}"), LogCategories);

				referenceFascicleEntity.EndDate = DateTimeOffset.UtcNow;
				if (identity != null)
				{
					referenceFascicleEntity.LastChangedUser = identity.User;
				}
				await _fascicleService.UpdateAsync(referenceFascicleEntity);

				_logger.WriteDebug(new LogMessage($"Reference Fascicle {referenceFascicleEntity.UniqueId} closed successfully"), LogCategories);
			}
			#endregion

			#region [ Evaluate dsw_a_Fascicle_CreateLink Argument ]
			if (dsw_a_Fascicle_CreateLink != null && dsw_a_Fascicle_CreateLink.ValueBoolean.HasValue && dsw_a_Fascicle_CreateLink.ValueBoolean.Value)
			{
				_logger.WriteDebug(new LogMessage($"Creating Link between reference fascicle '{referenceFascicleEntity.UniqueId}' and destination fascicle '{destinationFascicleEntity.UniqueId}'"), LogCategories);

				FascicleLink fascicleLink = new FascicleLink
				{
					FascicleLinkType = FascicleLinkType.Automatic,
					Fascicle = new Fascicle { UniqueId = destinationFascicleEntity.UniqueId },
					FascicleLinked = new Fascicle { UniqueId = referenceFascicleEntity.UniqueId }
				};

				await _fascicleLinkService.CreateAsync(fascicleLink);

				_logger.WriteDebug(new LogMessage($"Reference and destination fascicles linked successfully"), LogCategories);
			}
			#endregion

			_logger.WriteDebug(new LogMessage($"Contents of reference fasicle '{referenceFascicleEntity.UniqueId}' copied successfully into destination fasiccle '{destinationFascicleEntity.UniqueId}'"), LogCategories);

			#region [ Send Copy Contents Event ]
			_logger.WriteDebug(new LogMessage($"Sending IEventCompleteFascicleBuild event to servicebus for workflow instance '{workflowInstanceId}'"), LogCategories);

			FascicleBuildModel destinationFascicleBuildModel = new FascicleBuildModel
			{
				WorkflowName = workflowName,
				WorkflowAutoComplete = true,
				IdWorkflowActivity = workflowActivityId,
				Fascicle = destinationFascicleModel
			};

			IEventCompleteFascicleBuild evt = new EventCompleteFascicleBuild(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value,
				CurrentIdentityContext, destinationFascicleBuildModel);
			ServiceBusMessage message = await SendEventToTopicAsync(evt, workflowInstanceId);

			_logger.WriteDebug(new LogMessage($"IEventCompleteFascicleBuild event sent successfully"), LogCategories);
			#endregion
		}

		/// <summary>
		///     Takes all the <see cref="MetadataValue"/>s from the reference fascicle which have the same name as 
		///     metadata values from destination fascicle, and updates the values with ones from reference fascicle
		/// </summary>
		/// <param name="referenceFascicleEntity">The reference fascicle entity</param>
		/// <param name="destinationFascicleEntity">The destination fascicle entity</param>
		/// <param name="dsw_a_CopyMetadatas">The workflow parameter indicating if metadata values update is enabled</param>
		/// <param name="dsw_a_Metadata_HandlerDate">The workflow parameter indicating the KeyName of the HandlerDate metadata </param>
		/// <param name="dsw_a_Metadata_HandlerEndDate">The workflow parameter indicating the KeyName of the HandlerEndDate metadata </param>
		/// <returns></returns>
		private async Task CloneFascicleMetadataValuesAsync(Fascicle referenceFascicleEntity, Fascicle destinationFascicleEntity, WorkflowProperty dsw_a_CopyMetadatas,
			WorkflowProperty dsw_a_Metadata_HandlerDate, WorkflowProperty dsw_a_Metadata_HandlerEndDate)
		{
			if (string.IsNullOrEmpty(referenceFascicleEntity.MetadataValues) || string.IsNullOrEmpty(destinationFascicleEntity.MetadataValues)
				|| (dsw_a_CopyMetadatas == null || !dsw_a_CopyMetadatas.ValueBoolean.HasValue || !dsw_a_CopyMetadatas.ValueBoolean.Value))
			{
				return;
			}

			ICollection<MetadataValueModel> referenceMetadataValues = JsonConvert.DeserializeObject<List<MetadataValueModel>>(referenceFascicleEntity.MetadataValues);
			ICollection<MetadataValueModel> destinationMetadataValues = JsonConvert.DeserializeObject<List<MetadataValueModel>>(destinationFascicleEntity.MetadataValues);

			ICollection<string> metadataToExclude = new List<string>();

			if (!string.IsNullOrEmpty(dsw_a_Metadata_HandlerDate.ValueString))
			{
				metadataToExclude.Add(dsw_a_Metadata_HandlerDate.ValueString);
			}
			if (!string.IsNullOrEmpty(dsw_a_Metadata_HandlerEndDate.ValueString))
			{
				metadataToExclude.Add(dsw_a_Metadata_HandlerEndDate.ValueString);
			}

			ICollection<MetadataValueModel> metadataValuesToUpdate = referenceMetadataValues
				.Where(referenceValue => destinationMetadataValues.Any(destinationValue => referenceValue.KeyName == destinationValue.KeyName && metadataToExclude.Contains(referenceValue.KeyName) == false)).ToList();

			if (metadataValuesToUpdate.Any())
			{
				UpdateDestinationFascicleMetadataValues(metadataValuesToUpdate, destinationMetadataValues, destinationFascicleEntity);

				await _fascicleService.UpdateAsync(destinationFascicleEntity);

				_logger.WriteDebug(new LogMessage($"Destination fascicle Metadata Values updated successfully"), LogCategories);
			}
		}

		/// <summary>
		///     Takes all fascicle folders from reference fascicle which have the same name as fascicle folders from destination fascicle, 
		///     and clones the Fascicle Documents and Fascicle Document Units from reference folders into destination folders
		/// </summary>
		/// <param name="copyFascicleDocumentUnits">Indicates if have to clone <see cref="FascicleDocumentUnit"/>s</param>
		/// <param name="copyFascicleDocuments">Indicates if have to clone <see cref="FascicleDocument"/>s</param>
		/// <param name="referenceFascicleId">The uniqueid of the reference <see cref="Fascicle"/></param>
		/// <param name="destinationFascicle">The destination fascicle entity</param>
		/// <param name="fascicleFolderFilterLevel">The fascicle folder level to start cloning from</param>
		/// <returns></returns>
		private async Task CloneFascicleFoldersContentsAsync(bool copyFascicleDocumentUnits, bool copyFascicleDocuments, Guid referenceFascicleId, Fascicle destinationFascicle, int? fascicleFolderFilterLevel = null)
		{
			if (!copyFascicleDocuments && !copyFascicleDocumentUnits)
			{
				return;
			}

			_logger.WriteDebug(new LogMessage($"Getting fascicle folders with same name from reference fascicle {referenceFascicleId} and destination fascicle {destinationFascicle.UniqueId} starting from hierarchy level {fascicleFolderFilterLevel}"), LogCategories);

			ICollection<FascicleFolderTableValuedModel> fascicleFoldersWithSameName = _unitOfWork.Repository<FascicleFolder>()
				.GetFascicleFoldersWithSameName(referenceFascicleId, destinationFascicle.UniqueId, fascicleFolderFilterLevel);

			_logger.WriteDebug(new LogMessage($"{fascicleFoldersWithSameName.Count} fascicle folders with same name have been found between reference fascicle and destination fascicle"), LogCategories);

			if (!fascicleFoldersWithSameName.Any())
			{
				return;
			}

			_logger.WriteDebug(new LogMessage($"Cloning fascicle document units and fascicle documents from reference fascicle folders to destination fascicle folders"), LogCategories);

			Location fascicleMiscellaneaLocation = _unitOfWork.Repository<Location>().Find(_parameterEnvService.FascicleMiscellaneaLocation);

			short currentMaxSequenceNumber = _unitOfWork.Repository<FascicleDocumentUnit>().GetLatestSequenceNumber(destinationFascicle.UniqueId);
			int successfullyInsertedCount = 0;
			foreach (FascicleFolderTableValuedModel referenceFascicleFolder in fascicleFoldersWithSameName)
			{
				FascicleFolder destinationFascicleFolder = _unitOfWork.Repository<FascicleFolder>()
					.GetByIdFascicleLevelAndName(destinationFascicle.UniqueId, referenceFascicleFolder.FascicleFolderLevel, referenceFascicleFolder.Name);

				if (copyFascicleDocumentUnits)
				{
					currentMaxSequenceNumber = await InsertFascicleDocumentUnitsIntoFolderAsync(referenceFascicleFolder.IdFascicleFolder, destinationFascicleFolder, destinationFascicle, currentMaxSequenceNumber);
				}

				if (copyFascicleDocuments)
				{
					await InsertFascicleDocumentsIntoFolderAsync(referenceFascicleFolder.IdFascicleFolder, destinationFascicleFolder, fascicleMiscellaneaLocation);
				}

				successfullyInsertedCount++;
			}

			_logger.WriteDebug(new LogMessage($"Fascicle Folders contents cloned successfully ({successfullyInsertedCount}/{fascicleFoldersWithSameName.Count})"), LogCategories);
		}

		private void UpdateDestinationFascicleMetadataValues(ICollection<MetadataValueModel> commonMetadataValues, ICollection<MetadataValueModel> destinationMetadataValues, Fascicle destinationFascicle)
		{
			_logger.WriteDebug(new LogMessage($"Updating {commonMetadataValues.Count} Metadata Values from Reference Fascicle with values from Destination Fascicle {destinationFascicle.UniqueId}"), LogCategories);

			foreach (MetadataValueModel commonMetadataValueModel in commonMetadataValues)
			{
				_logger.WriteDebug(new LogMessage($"Updating '{commonMetadataValueModel.KeyName}' Destination Metadata Value with new value '{commonMetadataValueModel.Value}'"), LogCategories);

				MetadataValueModel destinationMetadataValueModel = destinationMetadataValues.SingleOrDefault(refValue => refValue.KeyName == commonMetadataValueModel.KeyName);
				destinationMetadataValueModel.Value = commonMetadataValueModel.Value;
			}

			destinationFascicle.MetadataValues = JsonConvert.SerializeObject(destinationMetadataValues);
		}

		private async Task<short> InsertFascicleDocumentUnitsIntoFolderAsync(Guid referenceFascicleFolderId, FascicleFolder destinationFascicleFolder, Fascicle destinationFascicle, short currentMaxSequenceNumber)
		{
			ICollection<FascicleDocumentUnit> referenceFascicleDocumentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetByIdFascicleFolder(referenceFascicleFolderId, true).ToList();

			_logger.WriteDebug(new LogMessage($"Cloning fascicle document units from reference fascicle folder {referenceFascicleFolderId} into destination folder {destinationFascicleFolder.UniqueId}"), LogCategories);


			foreach (FascicleDocumentUnit referenceFascicleDocUnit in referenceFascicleDocumentUnits.OrderBy(f => f.SequenceNumber))
			{
				_logger.WriteDebug(new LogMessage($"Inserting Fascicle Document Unit '{referenceFascicleDocUnit.DocumentUnit.Subject}' into destination fascicle folder '{destinationFascicleFolder.Name}'"), LogCategories);

				FascicleDocumentUnit copyFascDocUnit = new FascicleDocumentUnit
				{
					DocumentUnit = referenceFascicleDocUnit.DocumentUnit,
					ReferenceType = DocSuiteWeb.Entity.Fascicles.ReferenceType.Reference,
					FascicleFolder = destinationFascicleFolder,
					Fascicle = destinationFascicle,
					SequenceNumber = ++currentMaxSequenceNumber
				};

				await _fascDocumentUnitService.CreateAsync(copyFascDocUnit, InsertActionType.CloneFascicle);
			}

			_logger.WriteDebug(new LogMessage($"Fascicle document unit successfully inserted into destination folder {destinationFascicleFolder.UniqueId} (last currentMaxSequenceNumber used: {currentMaxSequenceNumber})"), LogCategories);
			return currentMaxSequenceNumber;
		}

		private async Task InsertFascicleDocumentsIntoFolderAsync(Guid referenceFascicleFolderId, FascicleFolder destinationFascicleFolder, Location fascicleMiscellaneaLocation)
		{
			ICollection<FascicleDocument> referenceFascicleDocuments = _unitOfWork.Repository<FascicleDocument>().GetByFascicleFolderId(referenceFascicleFolderId, true).ToList();

			_logger.WriteDebug(new LogMessage($"Cloning fascicle documents from reference fascicle folder {referenceFascicleFolderId} into destination folder {destinationFascicleFolder.UniqueId}"), LogCategories);

			foreach (FascicleDocument referenceFascDocument in referenceFascicleDocuments)
			{
				_logger.WriteDebug(new LogMessage($"Inserting Fascicle Document {referenceFascDocument.UniqueId} into destination fascicle folder '{destinationFascicleFolder.Name}'"), LogCategories);

				ICollection<ArchiveDocument> insertedDestinationFascDocuments = await CloneDocumentsFromChainAsync(referenceFascDocument.IdArchiveChain, fascicleMiscellaneaLocation.ProtocolArchive);

				if (insertedDestinationFascDocuments != null && insertedDestinationFascDocuments.Any())
				{
					FascicleDocument copyFascicleDocument = new FascicleDocument
					{
						ChainType = referenceFascDocument.ChainType,
						IdArchiveChain = insertedDestinationFascDocuments.First().IdChain.Value,
						FascicleFolder = destinationFascicleFolder,
						Fascicle = destinationFascicleFolder.Fascicle
					};
					await _fascicleDocumentService.CreateAsync(copyFascicleDocument);
				}
			}

			_logger.WriteDebug(new LogMessage($"Fascicle Documents successfully inserted into destination folder {destinationFascicleFolder.UniqueId}"), LogCategories);
		}

		private async Task EvaluateAssignmentAreaAsync(WorkflowStep currentStep, WorkflowInstance workflowInstance, WorkflowRepository workflowRepository,
			WorkflowActivity workflowActivity, WorkflowReferenceModel workflowReferenceModel, WorkflowProperty dsw_p_ReferenceModel, WorkflowProperty dsw_p_Roles, WorkflowProperty dsw_p_Accounts,
			WorkflowProperty dsw_a_Fascicle_PublicEnforcement, WorkflowProperty dsw_a_Fascicle_PublicTemporaryEnforcement, IIdentityContext identity)
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
					foreach (WorkflowRole workflowRole in roles.Where(f => f.UniqueId != Guid.Empty))
					{
						role = _unitOfWork.Repository<Role>().GetByUniqueId(workflowRole.UniqueId).SingleOrDefault();
						if (role == null)
						{
							throw new DSWValidationException("Evaluate assignment validation error",
								new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Impossibile autorizzare il fascicolo {fascicle.Title} se il settore {workflowRole.UniqueId} non esiste" } },
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
									_logger.WriteDebug(new LogMessage($"Added role {workflowRole.UniqueId} to fascicle {fascicle.Title} with identifier {fascicleRole.UniqueId}"), LogCategories);
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
								_logger.WriteDebug(new LogMessage($"Skipped role ({workflowRole.UniqueId}) to fascicle {fascicle.Title} because already authorized"), LogCategories);
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
							if (identity != null)
							{
								fascicle.LastChangedUser = identity.User;
							}
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

		#endregion

		#region [ Service Bus Methods ]
		private async Task<TModel> CompleteBuildActivity<TModel>(WorkflowReferenceModel workflowReferenceModel, Guid workflowInstanceId, Guid idWorkflowActivity,
			string workflowName, Func<TModel, ICommand> f_initialize, Func<TModel, bool> f_validate)
			where TModel : IWorkflowContentBase
		{
			TModel model = JsonConvert.DeserializeObject<TModel>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);

			AttachWorkflowActivityIdToDocumentUnitLink(idWorkflowActivity, model);

			model.IdWorkflowActivity = idWorkflowActivity;
			model.WorkflowName = workflowName;
			if (!f_validate(model))
			{
				throw new DSWException($"Model [{model.GetType().Name}] is not complete. Check ReferenceModel property has value.", null, DSWExceptionCode.WF_RulesetValidation);
			}
			ICommand command = f_initialize(model);
			ServiceBusMessage message = await SendCommandToQueueAsync(command, workflowInstanceId);
			return model;
		}

		private async Task<ServiceBusMessage> SendCommandToQueueAsync(ICommand command, Guid workflowInstanceId)
		{
			ServiceBusMessage message = _cqrsMapper.Map(command, new ServiceBusMessage());

			if (string.IsNullOrEmpty(message.ChannelName))
			{
				throw new DSWException($"Queue name to command [{command}] is not mapped", null, DSWExceptionCode.SC_Mapper);
			}

			try
			{
				message = await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
			}
			catch (Exception ex)
			{
				_logger.WriteError(new LogMessage($"{command} has not been sended in workflow InstanceId {workflowInstanceId}"), ex, LogCategories);
				throw;
			}

			return message;
		}

		private async Task<TModel> CompleteBuildEvent<TModel>(WorkflowReferenceModel workflowReferenceModel, Guid workflowInstanceId, Guid idWorkflowActivity,
			string workflowName, Func<TModel, IEvent> func)
			where TModel : IWorkflowContentBase
		{
			TModel model = JsonConvert.DeserializeObject<TModel>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);

			return await CompleteBuildEvent(model, workflowInstanceId, idWorkflowActivity, workflowName, func);
		}

		private async Task<TModel> CompleteBuildEvent<TModel>(TModel model, Guid workflowInstanceId, Guid idWorkflowActivity,
			string workflowName, Func<TModel, IEvent> func)
			where TModel : IWorkflowContentBase
		{
			model.IdWorkflowActivity = idWorkflowActivity;
			model.WorkflowName = workflowName;
			IEvent evt = func(model);

			ServiceBusMessage message = await SendEventToTopicAsync(evt, workflowInstanceId);

			return model;
		}

		private async Task<ServiceBusMessage> SendEventToTopicAsync(IEvent @event, Guid workflowInstanceId)
		{
			ServiceBusMessage message = _cqrsMapper.Map(@event, new ServiceBusMessage());

			if (string.IsNullOrEmpty(message.ChannelName))
			{
				throw new DSWException($"Topic name to command [{@event}] is not mapped", null, DSWExceptionCode.SC_Mapper);
			}

			try
			{
				message = await _topicService.SendToTopicAsync(message);
			}
			catch (Exception ex)
			{
				_logger.WriteError(new LogMessage($"{@event} has not been sended in workflow InstanceId {workflowInstanceId}"), ex, LogCategories);
				throw;
			}

			return message;
		}
		#endregion

		#region [ Clone Methods ]

		private async Task<WorkflowActivity> CloneWorkflowActivityAsync(WorkflowActivity currentWorkflowActivity, Location workflowLocation)
		{
			WorkflowActivity workflowActivity = new WorkflowActivity
			{
				ActivityType = currentWorkflowActivity.ActivityType,
				ActivityAction = currentWorkflowActivity.ActivityAction,
				ActivityArea = currentWorkflowActivity.ActivityArea,
				DocumentUnitReferenced = currentWorkflowActivity.DocumentUnitReferenced,
				DueDate = currentWorkflowActivity.DueDate,
				IdArchiveChain = currentWorkflowActivity.IdArchiveChain,
				Name = currentWorkflowActivity.Name,
				Note = currentWorkflowActivity.Note,
				Priority = currentWorkflowActivity.Priority,
				IsVisible = currentWorkflowActivity.IsVisible,
				Subject = currentWorkflowActivity.Subject,
				RegistrationDate = currentWorkflowActivity.RegistrationDate,
				RegistrationUser = currentWorkflowActivity.RegistrationUser,
				Status = currentWorkflowActivity.Status,
				WorkflowInstance = currentWorkflowActivity.WorkflowInstance,
				Tenant = currentWorkflowActivity.Tenant
			};
			if (workflowActivity.IdArchiveChain.HasValue)
			{
				ICollection<ArchiveDocument> clonedChainDocuments = await CloneDocumentsFromChainAsync(workflowActivity.IdArchiveChain.Value, workflowLocation.ProtocolArchive);
				workflowActivity.IdArchiveChain = clonedChainDocuments.First().IdChain.Value;
			}
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

		private async Task<ICollection<ArchiveDocument>> CloneDocumentsFromChainAsync(Guid idArchiveChain, string archiveName)
		{
			IEnumerable<Document> chainDocuments = await _documentService.GetDocumentLatestVersionFromChainAsync(idArchiveChain);

			List<ArchiveDocument> archiveDocuments = new List<ArchiveDocument>();
			foreach (Document document in chainDocuments)
			{
				archiveDocuments.Add(new ArchiveDocument
				{
					Archive = archiveName,
					ContentStream = await _documentService.GetDocumentContentAsync(document.IdDocument),
					Name = document.Name
				});
			}
			ICollection<ArchiveDocument> results = await _documentService.InsertDocumentsAsync(archiveDocuments);

			return results;
		}

		#endregion

		#region [ BiblosDS Methods ]
		protected async Task<Guid?> ArchiveDocument(Action<string> propertySetter, WorkflowReferenceModel referenceModel, Guid? idArchiveChain)
		{
			ICollection<ArchiveDocument> results = await ArchiveDocumentsAsync(propertySetter, referenceModel, idArchiveChain);

			if (results != null && results.Any(f => f.IdChain.HasValue))
			{
				idArchiveChain = results.First().IdChain;
			}

			return idArchiveChain;
		}

		private async Task<ICollection<ArchiveDocument>> ArchiveDocumentsAsync(Action<string> propertySetter, WorkflowReferenceModel referenceModel, Guid? idArchiveChain)
		{
			ICollection<ArchiveDocument> results = null;
			WorkflowDocumentModel workflowDocumentModel = JsonConvert.DeserializeObject<WorkflowDocumentModel>(referenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
			if (workflowDocumentModel != null && workflowDocumentModel.Documents.Count > 0)
			{
				_logger.WriteDebug(new LogMessage($"Initialize archive document {workflowDocumentModel.Documents.Count} to store in workflow storage"), LogCategories);
				short workflowLocationId = _parameterEnvService.WorkflowLocationId;
				Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
				if (workflowLocation == null)
				{
					throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
				}
				results = workflowDocumentModel.Documents
					.Where(f => f.Key == Model.Entities.DocumentUnits.ChainType.Miscellanea && f.Value != null && f.Value.ContentStream != null && !string.IsNullOrEmpty(f.Value.FileName))
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
					foreach (KeyValuePair<Model.Entities.DocumentUnits.ChainType, DocumentModel> document in workflowDocumentModel.Documents.Where(f => f.Value.ChainId.HasValue))
					{
						if (document.Value.DocumentId != null && document.Value.DocumentId != Guid.Empty)
						{
							documents.Add(await _documentService.GetDocumentAsync(document.Value.DocumentId.Value));
						}
						else
						{
							documents.AddRange(await _documentService.GetDocumentLatestVersionFromChainAsync(document.Value.ChainId.Value));
						}
					}
					List<ArchiveDocument> archiveDocuments = new List<ArchiveDocument>();
					foreach (ModelDocument.Document document in documents)
					{
						archiveDocuments.Add(new ArchiveDocument
						{
							Archive = workflowLocation.ProtocolArchive,
							ContentStream = await _documentService.GetDocumentContentAsync(document.IdDocument),
							Name = document.Name,
							Signature = document.AttributeValues.SingleOrDefault(f => f.AttributeName.Equals(AttributeValue.ATTRIBUTE_SIGNATURE, StringComparison.InvariantCultureIgnoreCase))?.ValueString
						});
					}
					if (archiveDocuments.Any())
					{
						_logger.WriteDebug(new LogMessage($"Cloning {archiveDocuments.Count} documents to workflow storage"), LogCategories);
						results = await _documentService.InsertDocumentsAsync(archiveDocuments);
					}
				}
				referenceModel.ReferenceModel = JsonConvert.SerializeObject(workflowDocumentModel);
				propertySetter(JsonConvert.SerializeObject(referenceModel));
			}
			return results;
		}

		protected async Task<Guid?> ArchiveDocumentFromReferences(WorkflowReferenceModel workflowReferenceModel, Guid? idArchiveChain)
		{
			IEnumerable<WorkflowReferenceBiblosModel> referenceBiblosModels;
			if (workflowReferenceModel != null)
			{
				short workflowLocationId = _parameterEnvService.WorkflowLocationId;
				Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
				if (workflowLocation == null)
				{
					throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
				}

				ICollection<ArchiveDocument> documentUnitDocuments = (await GetArchiveDocumentUnitFromReferences(workflowReferenceModel.DocumentUnits)).Select(s => new ArchiveDocument
				{
					Archive = workflowLocation.ProtocolArchive,
					ContentStream = s.ContentStream,
					Name = s.Name,
					Signature = s.Signature,
				}).ToList();
				if (documentUnitDocuments.Any())
				{
					documentUnitDocuments = await _documentService.InsertDocumentsAsync(documentUnitDocuments, idChain: idArchiveChain);
					idArchiveChain = documentUnitDocuments.First().IdChain;
				}

				if ((referenceBiblosModels = workflowReferenceModel.Documents.Where(f => f.ChainType == Model.Entities.DocumentUnits.ChainType.Miscellanea && f.ArchiveChainId.HasValue && f.ArchiveDocumentId.HasValue)).Any())
				{
					ICollection<ArchiveDocument> results = new List<ArchiveDocument>();
					ICollection<Document> existDocument = new List<Document>();
					if (idArchiveChain.HasValue)
					{
						existDocument = await _documentService.GetDocumentsFromChainAsync(idArchiveChain.Value);
					}

					byte[] content;
					foreach (WorkflowReferenceBiblosModel item in referenceBiblosModels.Where(f => !f.ArchiveDocumentId.HasValue || (f.ArchiveDocumentId.HasValue && !existDocument.Any(x => f.ArchiveDocumentId.Value == x.IdDocument))))
					{
						content = await _documentService.GetDocumentContentAsync(item.ArchiveDocumentId.Value);
						Document document = await _documentService.GetDocumentAsync(item.ArchiveDocumentId.Value);
						item.ArchiveName = workflowLocation.ProtocolArchive;
						results.Add(new ArchiveDocument
						{
							Archive = workflowLocation.ProtocolArchive,
							ContentStream = content,
							Name = item.DocumentName,
							Signature = document.AttributeValues.SingleOrDefault(f => f.AttributeName.Equals(AttributeValue.ATTRIBUTE_SIGNATURE, StringComparison.InvariantCultureIgnoreCase))?.ValueString
						});
					}
					results = await _documentService.InsertDocumentsAsync(results, idChain: idArchiveChain);
					if (results.Count > 0)
					{
						await Task.Delay(3000);
						idArchiveChain = results.First().IdChain;
					}
				}
			}

			return idArchiveChain;
		}

		protected async Task<Guid?> ArchiveAllDocumentUnitDocuments(WorkflowReferenceModel workflowReferenceModel, Guid? idArchiveChain)
		{
			if (workflowReferenceModel != null)
			{
				short workflowLocationId = _parameterEnvService.WorkflowLocationId;
				Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
				if (workflowLocation == null)
				{
					throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
				}
				ICollection<ArchiveDocument> documentUnitDocuments = null;
				if (workflowReferenceModel.ReferenceType == DSWEnvironmentType.PECMail)
				{
					PECMail pecMail = JsonConvert.DeserializeObject<PECMail>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
					documentUnitDocuments = (await GetArchiveDocumentsFromPecMail(pecMail)).Select(s => new ArchiveDocument
					{
						Archive = workflowLocation.ProtocolArchive,
						ContentStream = s.ContentStream,
						Name = s.Name,
						Signature = s.Signature,
					}).ToList();
				}
				if (IsDocumentUnit(workflowReferenceModel))
				{
					DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
					documentUnitDocuments = (await GetArchiveDocumentsFromDocumentUnit(documentUnit)).Select(s => new ArchiveDocument
					{
						Archive = workflowLocation.ProtocolArchive,
						ContentStream = s.ContentStream,
						Name = s.Name,
						Signature = s.Signature,
					}).ToList();
				}
				if (documentUnitDocuments != null && documentUnitDocuments.Any())
				{
					documentUnitDocuments = await _documentService.InsertDocumentsAsync(documentUnitDocuments);
					idArchiveChain = documentUnitDocuments.First().IdChain;
				}
			}

			return idArchiveChain;
		}

		protected async Task<ICollection<ArchiveDocument>> GetArchiveDocumentUnitFromReferences(IEnumerable<WorkflowReferenceDocumentUnitModel> referenceDocumentUnitModels)
		{
			ICollection<ArchiveDocument> results = new List<ArchiveDocument>();
			if (referenceDocumentUnitModels != null)
			{
				DocumentUnit documentUnit;
				ICollection<Document> documents;
				byte[] content;
				foreach (WorkflowReferenceDocumentUnitModel item in referenceDocumentUnitModels)
				{
					documentUnit = _unitOfWork.Repository<DocumentUnit>().GetById(item.UniqueId).FirstOrDefault();
					if (documentUnit != null && documentUnit.DocumentUnitChains != null)
					{
						foreach (DocumentUnitChain documentUnitChain in documentUnit.DocumentUnitChains)
						{
							documents = await _documentService.GetDocumentsFromChainAsync(documentUnitChain.IdArchiveChain);
							foreach (Document document in documents)
							{
								content = await _documentService.GetDocumentContentAsync(document.IdDocument);
								results.Add(new ArchiveDocument
								{
									Archive = documentUnitChain.ArchiveName,
									ContentStream = content,
									Name = document.Name,
									IdChain = documentUnitChain.IdArchiveChain,
									IdDocument = document.IdDocument,
									Size = document.Size,
									Version = document.Version,
									Signature = document.AttributeValues.SingleOrDefault(f => f.AttributeName.Equals(AttributeValue.ATTRIBUTE_SIGNATURE, StringComparison.InvariantCultureIgnoreCase))?.ValueString
								});
							}
						}
					}
				}
			}
			return results;
		}

		protected async Task<ICollection<ArchiveDocument>> GetArchiveDocumentsFromDocumentUnit(DocumentUnit documentUnit)
		{
			ICollection<ArchiveDocument> results = new List<ArchiveDocument>();
			if (documentUnit == null || documentUnit.DocumentUnitChains == null)
			{
				return results;
			}
			ICollection<Document> documents;
			byte[] content;
			foreach (DocumentUnitChain documentUnitChain in documentUnit.DocumentUnitChains)
			{
				documents = await _documentService.GetDocumentsFromChainAsync(documentUnitChain.IdArchiveChain);
				foreach (Document document in documents)
				{
					content = await _documentService.GetDocumentContentAsync(document.IdDocument);
					results.Add(new ArchiveDocument
					{
						Archive = documentUnitChain.ArchiveName,
						ContentStream = content,
						Name = document.Name,
						IdChain = documentUnitChain.IdArchiveChain,
						IdDocument = document.IdDocument,
						Size = document.Size,
						Version = document.Version,
						Signature = document.AttributeValues.SingleOrDefault(f => f.AttributeName.Equals(AttributeValue.ATTRIBUTE_SIGNATURE, StringComparison.InvariantCultureIgnoreCase))?.ValueString
					});
				}
			}
			return results;
		}

		protected async Task<ICollection<ArchiveDocument>> GetArchiveDocumentsFromPecMail(PECMail pecMail)
		{
			ICollection<ArchiveDocument> results = new List<ArchiveDocument>();
			if (pecMail == null)
			{
				return results;
			}

			List<PECMailAttachment> attachments = _unitOfWork.Repository<PECMailAttachment>().GetAttachments(pecMail.EntityId).ToList();
			bool isDocumentManger = pecMail.ProcessStatus == DocSuiteWeb.Entity.PECMails.PECMailProcessStatus.ArchivedInDocSuiteNext
				|| pecMail.ProcessStatus == DocSuiteWeb.Entity.PECMails.PECMailProcessStatus.StoredInDocumentManager;

			byte[] content = null;
			string archiveName = string.Empty;
			string fileName = string.Empty;
			Guid? idChain = Guid.Empty;
			Guid idDocument = Guid.Empty;
			long? size = null;
			List<DocumentSuiteInfo> documentProxyInfos = isDocumentManger && !string.IsNullOrEmpty(pecMail.MailContent)
				? JsonConvert.DeserializeObject<List<DocumentSuiteInfo>>(pecMail.MailContent)
				: new List<DocumentSuiteInfo>();
			if (pecMail.IDPostacert.HasValue)
			{
				if (isDocumentManger && documentProxyInfos.Any())
				{
					_logger.WriteDebug(new LogMessage($"getting pecmail postacert from documentproxy: {pecMail.IDPostacert}"), LogCategories);
					string id = pecMail.IDPostacert.Value.ToString();
					DocumentSuiteInfo documentSuiteInfo = documentProxyInfos.SingleOrDefault(f => f.DocumentId.Equals(id, StringComparison.OrdinalIgnoreCase));
					if (documentSuiteInfo != null)
					{
						_logger.WriteDebug(new LogMessage($"pecmail postacert has been found on documentproxy: {documentSuiteInfo.FileName}/{documentSuiteInfo.ReferenceType}/{documentSuiteInfo.DocumentId}"), LogCategories);
						idChain = Guid.Parse(documentSuiteInfo.ReferenceId);
						idDocument = Guid.Parse(documentSuiteInfo.DocumentId);
						content = await _documentProxyContext.GetDocumentContentAsync(idChain.Value, documentSuiteInfo.ReferenceType, idDocument);
						fileName = documentSuiteInfo.FileName;
						size = documentSuiteInfo.Size;
						archiveName = documentSuiteInfo.ReferenceId;
					}
				}
				else
				{
					_logger.WriteDebug(new LogMessage($"getting pecmail postacert from biblosds: {pecMail.IDPostacert}"), LogCategories);
					Document postacert = await _documentService.GetDocumentAsync(pecMail.IDPostacert.Value);
					if (postacert != null)
					{
						_logger.WriteDebug(new LogMessage($"pecmail postacert has been found on biblosds: {postacert.Name}/{postacert.IdDocument}"), LogCategories);
						content = await _documentService.GetDocumentContentAsync(postacert.IdDocument);
						archiveName = postacert.ArchiveName;
						fileName = postacert.Name;
						idChain = postacert.IdChain;
						idDocument = postacert.IdDocument;
						size = postacert.Size;
					}
				}
				if (content != null && content.Length > 0)
				{
					results.Add(new ArchiveDocument
					{
						Archive = archiveName,
						ContentStream = content,
						Name = fileName,
						IdChain = idChain,
						IdDocument = idDocument,
						Size = size
					});
				}
			}

			if (attachments == null)
			{
				return results;
			}

			Document document;
			foreach (PECMailAttachment attachment in attachments.Where(f => f.IDDocument.HasValue && f.IDDocument.Value != Guid.Empty))
			{
				content = null;
				archiveName = string.Empty;
				fileName = string.Empty;
				idChain = Guid.Empty;
				idDocument = Guid.Empty;
				size = null;
				if (isDocumentManger && documentProxyInfos.Any())
				{
					_logger.WriteDebug(new LogMessage($"getting pecmail attachment from documentproxy: {attachment.IDDocument}"), LogCategories);
					string id = attachment.IDDocument.Value.ToString();
					DocumentSuiteInfo documentSuiteInfo = documentProxyInfos.SingleOrDefault(f => f.DocumentId.Equals(id, StringComparison.OrdinalIgnoreCase));
					if (documentSuiteInfo != null)
					{
						_logger.WriteDebug(new LogMessage($"pecmail attachment has been found on documentproxy: {documentSuiteInfo.FileName}/{documentSuiteInfo.DocumentId}"), LogCategories);
						idChain = Guid.Parse(documentSuiteInfo.ReferenceId);
						idDocument = Guid.Parse(documentSuiteInfo.DocumentId);
						content = await _documentProxyContext.GetDocumentContentAsync(idChain.Value, documentSuiteInfo.ReferenceType, idDocument);
						fileName = documentSuiteInfo.FileName;
						size = documentSuiteInfo.Size;
						archiveName = documentSuiteInfo.ReferenceId;
					}
				}
				else
				{
					_logger.WriteDebug(new LogMessage($"getting pecmail attachment from biblosds: {attachment.IDDocument}"), LogCategories);
					document = await _documentService.GetDocumentAsync(attachment.IDDocument.Value);
					if (document != null)
					{
						_logger.WriteDebug(new LogMessage($"pecmail attachment has been found on biblosds: {document.Name}/{document.IdDocument}"), LogCategories);
						content = await _documentService.GetDocumentContentAsync(document.IdDocument);
						archiveName = document.ArchiveName;
						fileName = document.Name;
						idChain = document.IdChain;
						idDocument = document.IdDocument;
						size = document.Size;
					}
				}
				if (content != null && content.Length > 0)
				{
					results.Add(new ArchiveDocument
					{
						Archive = archiveName,
						ContentStream = content,
						Name = fileName,
						IdChain = idChain,
						IdDocument = idDocument,
						Size = size
					});
				}

			}

			return results;
		}

		private async Task EvaluateFascicleAuthorizeProtocolAreaAsync(WorkflowInstance workflowInstance, Guid workflowInstanceId, Guid workflowActivityId, WorkflowReferenceModel workflowReferenceModel,
			WorkflowProperty dsw_p_ProposerUser, WorkflowProperty dsw_p_Accounts, WorkflowProperty dsw_p_TenantAOOId, WorkflowProperty dsw_p_TenantId, WorkflowProperty dsw_p_TenantName)
		{
			if (workflowReferenceModel != null && workflowReferenceModel.ReferenceType == DSWEnvironmentType.Fascicle && workflowReferenceModel.DocumentUnits != null)
			{
				if (dsw_p_Accounts == null || dsw_p_Accounts.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_Accounts.ValueString))
				{
					throw new DSWValidationException("Evaluate validation error",
						new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Impossibile avviare l'attività senza aver specificato il nome utente" } },
						null, DSWExceptionCode.VA_RulesetValidation);
				}

				IIdentityContext identity = new IdentityContext(_security.GetCurrentUser().Account);
				if (dsw_p_ProposerUser != null && dsw_p_ProposerUser.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_p_ProposerUser.ValueString))
				{
					WorkflowAccount proposerUser = JsonConvert.DeserializeObject<WorkflowAccount>(dsw_p_ProposerUser.ValueString, ServiceHelper.SerializerSettings);
					identity = new IdentityContext(proposerUser.AccountName);
				}
				_logger.WriteInfo(new LogMessage($"Set identity {identity.User}"), LogCategories);
				ICollection<WorkflowAccount> toAuthorizeUsers = JsonConvert.DeserializeObject<ICollection<WorkflowAccount>>(dsw_p_Accounts.ValueString);
				ProtocolBuildModel protocolBuildModel;
				Protocol protocol;
				CommandUpdateProtocolData command;
				ServiceBusMessage message;
				foreach (WorkflowReferenceDocumentUnitModel referenceDocumentUnit in workflowReferenceModel.DocumentUnits.Where(f => f.Environment == (int)DSWEnvironmentType.Protocol))
				{
					protocol = _unitOfWork.Repository<Protocol>().GetByUniqueIdWithRole(referenceDocumentUnit.UniqueId).SingleOrDefault();
					if (protocol == null)
					{
						_logger.WriteWarning(new LogMessage($"Protocol [{referenceDocumentUnit.UniqueId}] not found"), LogCategories);
						continue;
					}

					protocolBuildModel = new ProtocolBuildModel()
					{
						UniqueId = Guid.NewGuid(),
						WorkflowName = workflowInstance.WorkflowRepository.Name,
						WorkflowAutoComplete = true,
						IdWorkflowActivity = workflowActivityId,
						Protocol = new ProtocolModel()
						{
							UniqueId = referenceDocumentUnit.UniqueId,
							Year = protocol.Year,
							Number = protocol.Number,
							Object = protocol.Object,
							Roles = protocol.ProtocolRoles.Select(s => new ProtocolRoleModel()
							{
								UniqueId = s.UniqueId,
								Role = new RoleModel() { IdRole = s.Role.EntityShortId, EntityShortId = s.Role.EntityShortId }
							}).ToList(),
							Users = protocol.ProtocolUsers.Select(s => new ProtocolUserModel()
							{
								UniqueId = s.UniqueId,
								Account = s.Account
							}).ToList()
						}
					};

					protocolBuildModel.Protocol.Users = protocolBuildModel.Protocol.Users.Concat(toAuthorizeUsers.Select(s => new ProtocolUserModel()
					{
						Account = s.AccountName,
						Type = Model.Entities.Protocols.ProtocolUserType.Authorization
					})).ToList();

					command = new CommandUpdateProtocolData(workflowReferenceModel.ReferenceId, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, identity, protocolBuildModel);
					message = await SendCommandToQueueAsync(command, workflowInstanceId);
				}
			}
		}

		#endregion

		#region [ Generate Word/PDF Methods ]
		private async Task<KeyValuePair<string, byte[]>> BuildDocument(WorkflowProperty dsw_a_Generate_TemplateId, DocumentGeneratorModel documentGeneratorModel, WorkflowProperty dsw_a_Generate_WordTemplate,
			WorkflowProperty dsw_a_Generate_PDFTemplate, bool appendContent = false, List<DocumentGeneratorModel> extraDocumentParameters = null,
			Func<Guid, List<DocumentGeneratorModel>, DocumentGeneratorModel, Task<byte[]>> AppendContentToWordDocument = null, string documentFilename = null)
		{
			byte[] stream = null;
			string out_filename = string.Empty;
			if (dsw_a_Generate_TemplateId != null && dsw_a_Generate_TemplateId.ValueGuid.HasValue && documentGeneratorModel != null)
			{
				StringParameter filename = new StringParameter("_filename", "document");
				if (documentGeneratorModel.DocumentGeneratorParameters.OfType<StringParameter>().Any(f => f.Name == "_filename"))
				{
					filename = documentGeneratorModel.DocumentGeneratorParameters.OfType<StringParameter>().Single(f => f.Name == "_filename");
				}
				if (!string.IsNullOrEmpty(documentFilename))
				{
					filename = new StringParameter("_filename", documentFilename);
				}

				if (dsw_a_Generate_WordTemplate != null && dsw_a_Generate_WordTemplate.ValueBoolean.HasValue && dsw_a_Generate_WordTemplate.ValueBoolean.Value)
				{
					if (appendContent)
					{
						WordGeneratorBuilderExtension.WordOpenXmlDocumentGenerator = _wordOpenXmlDocumentGenerator;
						stream = await AppendContentToWordDocument(dsw_a_Generate_TemplateId.ValueGuid.Value, extraDocumentParameters, documentGeneratorModel);
					}
					else
					{
						stream = await _wordOpenXmlDocumentGenerator.GenerateDocumentAsync(dsw_a_Generate_TemplateId.ValueGuid.Value, documentGeneratorModel);
					}
					filename.Value = $"{filename.Value}{FILE_EXTENSION_DOCX}";
				}
				if (dsw_a_Generate_PDFTemplate != null && dsw_a_Generate_PDFTemplate.ValueBoolean.HasValue && dsw_a_Generate_PDFTemplate.ValueBoolean.Value)
				{
					stream = await _pdfDocumentGenerator.GenerateDocumentAsync(dsw_a_Generate_TemplateId.ValueGuid.Value, documentGeneratorModel);
					filename.Value = $"{filename.Value}{FILE_EXTENSION_PDF}";
				}
				out_filename = filename.Value;
			}

			return new KeyValuePair<string, byte[]>(out_filename, stream);
		}

		private async Task<KeyValuePair<string, byte[]>> BuildDocument(WorkflowProperty dsw_a_Generate_TemplateId, WorkflowProperty dsw_e_Generate_DocumentMetadatas, WorkflowProperty dsw_a_Generate_WordTemplate,
			WorkflowProperty dsw_a_Generate_PDFTemplate, bool appendContent = false, List<DocumentGeneratorModel> extraDocumentParameters = null,
			Func<Guid, List<DocumentGeneratorModel>, DocumentGeneratorModel, Task<byte[]>> AppendContentToWordDocument = null)
		{
			if (dsw_a_Generate_TemplateId != null && dsw_a_Generate_TemplateId.ValueGuid.HasValue && dsw_e_Generate_DocumentMetadatas != null && !string.IsNullOrEmpty(dsw_e_Generate_DocumentMetadatas.ValueString))
			{
				DocumentGeneratorModel documentGeneratorModel = new DocumentGeneratorModel
				{
					DocumentGeneratorParameters = JsonConvert.DeserializeObject<ICollection<IDocumentGeneratorParameter>>(dsw_e_Generate_DocumentMetadatas.ValueString, ServiceHelper.SerializerSettings)
				};

				return await BuildDocument(dsw_a_Generate_TemplateId, documentGeneratorModel, dsw_a_Generate_WordTemplate, dsw_a_Generate_PDFTemplate,
					appendContent, extraDocumentParameters, AppendContentToWordDocument);
			}

			return new KeyValuePair<string, byte[]>(string.Empty, null);
		}

		private DocumentGeneratorModel GenerateDocumentParametersFromFascicleDocumentUnits(Guid idFascicle, ICollection<IDocumentGeneratorParameter> documentGeneratorParameters)
		{
			ICollection<FascicleDocumentUnit> fascicleDocumentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetByFascicle(idFascicle, optimization: true).ToList();
			DocumentGeneratorModel documentGeneratorModel = new DocumentGeneratorModel
			{
				DocumentGeneratorParameters = new List<IDocumentGeneratorParameter>()
			};
			int fascicleDocumentUnitIndex = 0;
			foreach (StringParameter fascicleDocumentUnitProperty in documentGeneratorParameters)
			{
				documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter(fascicleDocumentUnitProperty.Name.Split('_')[1], fascicleDocumentUnitProperty.Value));
			}
			foreach (FascicleDocumentUnit fascicleDocumentUnit in fascicleDocumentUnits)
			{
				DocumentUnit documentUnit = fascicleDocumentUnit.DocumentUnit;
				documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter($"YearNumber_{fascicleDocumentUnitIndex}", $"{documentUnit.Year}/{documentUnit.Number:0000000}"));
				documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter($"Subject_{fascicleDocumentUnitIndex}", documentUnit.Subject));
				string categoryDescription = documentUnit.Category != null
					? $"{documentUnit.Category.Code}.{documentUnit.Category.Name}"
					: string.Empty;
				documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter($"Category_{fascicleDocumentUnitIndex}", categoryDescription));
				documentGeneratorModel.DocumentGeneratorParameters.Add(new DateTimeParameter($"RegistrationDate_{fascicleDocumentUnitIndex}", documentUnit.RegistrationDate.DateTime));
				fascicleDocumentUnitIndex++;
			}
			return documentGeneratorModel;
		}

		private async Task<DocumentGeneratorModel> GenerateDocumentParametersFromFascicleDocuments(Guid idFascicle, ICollection<IDocumentGeneratorParameter> documentGeneratorParameters)
		{
			ICollection<FascicleDocument> fascicleDocuments = _unitOfWork.Repository<FascicleDocument>().GetByFascicle(idFascicle, optimization: true).ToList();
			DocumentGeneratorModel documentGeneratorModel = new DocumentGeneratorModel
			{
				DocumentGeneratorParameters = new List<IDocumentGeneratorParameter>()
			};
			int fascicleDocumentIndex = 0;
			foreach (StringParameter fascicleDocumentProperty in documentGeneratorParameters)
			{
				documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter(fascicleDocumentProperty.Name.Split('_')[1], fascicleDocumentProperty.Value));
			}
			foreach (FascicleDocument fascicleDocument in fascicleDocuments)
			{
				ICollection<Document> biblosDocuments = await _documentService.GetDocumentsFromChainAsync(fascicleDocument.IdArchiveChain);
				foreach (Document biblosDocument in biblosDocuments)
				{
					documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter($"DocumentName_{fascicleDocumentIndex}", biblosDocument.Name));
					documentGeneratorModel.DocumentGeneratorParameters.Add(new DateTimeParameter($"DocumentCreatedDate_{fascicleDocumentIndex}", biblosDocument.CreatedDate.Value));
					documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter($"DocumentRegistrationUser_{fascicleDocumentIndex}", fascicleDocument.RegistrationUser));
					fascicleDocumentIndex++;
				}
			}
			return documentGeneratorModel;
		}

		private async Task<ArchiveDocument> GenerateReport(WorkflowProperty dsw_a_Generate_TemplateId, WorkflowProperty dsw_a_Generate_WordTemplate, WorkflowProperty dsw_a_Generate_PdfTemplate,
			WorkflowProperty dsw_e_Generate_DocumentMetadatas, WorkflowReferenceModel workflowReferenceModel, WorkflowProperty dsw_p_FolderSelected, WorkflowProperty dsw_p_WorkflowStorageLocationEnabled,
			WorkflowProperty dsw_p_DocumentFilename)
		{
			if (workflowReferenceModel.ReferenceType == DSWEnvironmentType.Fascicle)
			{
				return await GenerateFascicleWordReport(dsw_a_Generate_TemplateId, dsw_a_Generate_WordTemplate, dsw_e_Generate_DocumentMetadatas, workflowReferenceModel, dsw_p_FolderSelected);
			}

			if (workflowReferenceModel.ReferenceType == DSWEnvironmentType.Protocol
				|| workflowReferenceModel.ReferenceType == DSWEnvironmentType.Resolution
				|| workflowReferenceModel.ReferenceType == DSWEnvironmentType.UDS)
			{
				return await GenerateDocumentUnitReport(dsw_a_Generate_TemplateId, dsw_a_Generate_WordTemplate, dsw_a_Generate_PdfTemplate, dsw_e_Generate_DocumentMetadatas, workflowReferenceModel,
					dsw_p_WorkflowStorageLocationEnabled, dsw_p_DocumentFilename);
			}

			throw new DSWValidationException("Evaluate build area validation error",
				new List<ValidationMessageModel>()
				{
					new ValidationMessageModel
					{
						Key = "WorkflowActivity",
						Message = $"Report generation not allowed for ReferenceType {workflowReferenceModel.ReferenceType}"
					}
				},
				null, DSWExceptionCode.VA_RulesetValidation);
		}

		private async Task<ArchiveDocument> GenerateFascicleWordReport(WorkflowProperty dsw_a_Generate_TemplateId, WorkflowProperty dsw_a_Generate_WordTemplate, WorkflowProperty dsw_e_Generate_DocumentMetadatas,
			WorkflowReferenceModel workflowReferenceModel, WorkflowProperty dsw_p_FolderSelected)
		{
			DocumentGeneratorModel documentGeneratorModel = new DocumentGeneratorModel
			{
				DocumentGeneratorParameters = JsonConvert.DeserializeObject<ICollection<IDocumentGeneratorParameter>>(dsw_e_Generate_DocumentMetadatas.ValueString, ServiceHelper.SerializerSettings)
			};

			Fascicle fascicle = JsonConvert.DeserializeObject<Fascicle>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);

			List<DocumentGeneratorModel> extraContentParameters = new List<DocumentGeneratorModel>() {
					GenerateDocumentParametersFromFascicleDocumentUnits(fascicle.UniqueId, documentGeneratorModel.DocumentGeneratorParameters.Where(x=>x.Name.StartsWith("FDU_")).ToList()),
					await GenerateDocumentParametersFromFascicleDocuments(fascicle.UniqueId, documentGeneratorModel.DocumentGeneratorParameters.Where(x=>x.Name.StartsWith("FD_")).ToList())
				};

			KeyValuePair<string, byte[]> wordDocument = await BuildDocument(dsw_a_Generate_TemplateId, dsw_e_Generate_DocumentMetadatas, dsw_a_Generate_WordTemplate, null,
				appendContent: true, extraDocumentParameters: extraContentParameters, AppendContentToWordDocument: AppendFascicleContentToWordDocument);

			Location fascicleMiscellaneaLocation = _unitOfWork.Repository<Location>().Find(_parameterEnvService.FascicleMiscellaneaLocation);
			if (fascicleMiscellaneaLocation == null)
			{
				throw new DSWValidationException("Evaluate build area validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"Workflow location {_parameterEnvService.FascicleMiscellaneaLocation} not found" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}
			string archiveName = fascicleMiscellaneaLocation.ProtocolArchive;

			FascicleFolder selectedFascicleFolder = JsonConvert.DeserializeObject<FascicleFolder>(dsw_p_FolderSelected.ValueString);
			FascicleDocument fascicleDocumentFromSelectedFascicleFolder = _unitOfWork.Repository<FascicleDocument>().GetByIdFascicleFolder(selectedFascicleFolder.UniqueId, optimization: true, includeNavigationProperties: false).FirstOrDefault();

			ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
			{
				Archive = archiveName,
				ContentStream = wordDocument.Value,
				Name = wordDocument.Key,
				IdChain = fascicleDocumentFromSelectedFascicleFolder != null
					? fascicleDocumentFromSelectedFascicleFolder.IdArchiveChain
					: Guid.NewGuid()
			});

			FascicleDocument fascicleDocument = new FascicleDocument()
			{
				ChainType = ChainType.Miscellanea,
				IdArchiveChain = archiveDocument.IdChain.Value,
				Fascicle = new Fascicle() { UniqueId = fascicle.UniqueId },
				FascicleFolder = new FascicleFolder()
				{
					UniqueId = selectedFascicleFolder.UniqueId
				}
			};
			_unitOfWork.Repository<FascicleDocument>().Insert(fascicleDocument);

			return archiveDocument;
		}

		private async Task<ArchiveDocument> GenerateDocumentUnitReport(
					WorkflowProperty dsw_a_Generate_TemplateId,
					WorkflowProperty dsw_a_Generate_WordTemplate,
					WorkflowProperty dsw_a_Generate_PdfTemplate,
					WorkflowProperty dsw_e_Generate_DocumentMetadatas,
					WorkflowReferenceModel workflowReferenceModel,
					WorkflowProperty dsw_p_WorkflowStorageLocationEnabled,
					WorkflowProperty dsw_p_DocumentFilename)
		{
			DocumentUnit documentUnitFromReferenceModel = JsonConvert.DeserializeObject<DocumentUnit>(workflowReferenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
			DocumentUnit documentUnit = _unitOfWork.Repository<DocumentUnit>().GetByIdIncludingContainer(documentUnitFromReferenceModel.UniqueId).FirstOrDefault();
			if (documentUnit == null)
			{
				throw new DSWValidationException("Evaluate build area validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = $"DocumentUnit with id {documentUnitFromReferenceModel.UniqueId} not found" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			string archiveName = GetReferenceTypeArchiveName(workflowReferenceModel, dsw_p_WorkflowStorageLocationEnabled, documentUnit);

			if (string.IsNullOrEmpty(archiveName))
			{
				throw new DSWValidationException("Evaluate build area validation error",
					new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "Invalid ArchiveName" } },
					null, DSWExceptionCode.VA_RulesetValidation);
			}

			DocumentGeneratorModel documentGeneratorModel = await BuildDocumentInfoMetadata(dsw_e_Generate_DocumentMetadatas, documentUnit);

			string documentName = dsw_p_DocumentFilename != null && dsw_p_DocumentFilename.PropertyType == WorkflowPropertyType.PropertyString && !string.IsNullOrEmpty(dsw_p_DocumentFilename.ValueString)
				? dsw_p_DocumentFilename.ValueString
				: GENERATED_FILENAME;
			KeyValuePair<string, byte[]> generatedDocument = await BuildDocument(dsw_a_Generate_TemplateId, documentGeneratorModel, dsw_a_Generate_WordTemplate, dsw_a_Generate_PdfTemplate, documentFilename: documentName);

			ArchiveDocument archiveDocument;
			DocumentUnitChain documentUnitChain = _unitOfWork.Repository<DocumentUnitChain>().GetByDocumentUnitAndChainType(documentUnit, ChainType.DematerialisationChain);

			if (documentUnitChain == null)
			{
				_logger.WriteInfo(new LogMessage($"GenerateDocumentUnitReport: DematerialisationChain not found for document unit {documentUnit.UniqueId}. Adding DematerialisationChain and ArchiveDocument..."), LogCategories);

				archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument
				{
					Archive = archiveName,
					ContentStream = generatedDocument.Value,
					Name = generatedDocument.Key,
					IdChain = Guid.NewGuid()
				});

				if (!archiveDocument.IdChain.HasValue)
				{
					throw new DSWValidationException("Evaluate build area validation error",
						new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowActivity", Message = "IdChain of inserted ArchiveDocument is null" } },
						null, DSWExceptionCode.VA_RulesetValidation);
				}

				documentUnitChain = new DocumentUnitChain
				{
					ArchiveName = archiveName,
					ChainType = ChainType.DematerialisationChain,
					DocumentLabel = null,
					DocumentName = generatedDocument.Key,
					DocumentUnit = documentUnit,
					IdArchiveChain = archiveDocument.IdChain.Value
				};
				_unitOfWork.Repository<DocumentUnitChain>().Insert(documentUnitChain);

				_logger.WriteInfo(new LogMessage($"GenerateDocumentUnitReport: ArchiveDocument and DocumentUnitChain inserted (IdChain {archiveDocument.IdChain}, IdDocument {archiveDocument.IdDocument})."), LogCategories);
			}
			else
			{
				_logger.WriteInfo(new LogMessage($"GenerateDocumentUnitReport: DematerialisationChain found for document unit {documentUnit.UniqueId}. Updating ArchiveDocument..."), LogCategories);

				Document documentLatestVersion = (await _documentService.GetDocumentLatestVersionFromChainAsync(documentUnitChain.IdArchiveChain)).FirstOrDefault();

				Dictionary<string, string> attributes = new Dictionary<string, string>();

				foreach (AttributeValue attribute in documentLatestVersion.AttributeValues)
				{
					attributes.Add(attribute.AttributeName, attribute.ValueString);
				}

				archiveDocument = new ArchiveDocument
				{
					IdDocument = documentLatestVersion.IdDocument,
					Archive = archiveName,
					ContentStream = generatedDocument.Value,
					Name = generatedDocument.Key,
					IdChain = documentUnitChain.IdArchiveChain
				};

				archiveDocument = await _documentService.UpdateDocumentAsync(archiveDocument, attributes);

				_logger.WriteInfo(new LogMessage($"GenerateDocumentUnitReport: ArchiveDocument updated (IdChain {archiveDocument.IdChain}, IdDocument {archiveDocument.IdDocument})."), LogCategories);
			}

			return archiveDocument;
		}

		private string GetReferenceTypeArchiveName(WorkflowReferenceModel workflowReferenceModel, WorkflowProperty dsw_p_WorkflowStorageLocationEnabled, DocumentUnit documentUnit)
		{
			if (dsw_p_WorkflowStorageLocationEnabled != null
				&& dsw_p_WorkflowStorageLocationEnabled.PropertyType == WorkflowPropertyType.PropertyBoolean
				&& dsw_p_WorkflowStorageLocationEnabled.ValueBoolean.HasValue
				&& dsw_p_WorkflowStorageLocationEnabled.ValueBoolean.Value)
			{
				short workflowLocationId = _parameterEnvService.WorkflowLocationId;
				Location workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
				return workflowLocation.ProtocolArchive;
			}

			switch (workflowReferenceModel.ReferenceType)
			{
				case DSWEnvironmentType.Protocol:
					return documentUnit?.Container?.ProtLocation?.ProtocolArchive;
				case DSWEnvironmentType.Resolution:
					return documentUnit?.Container?.ReslLocation?.ResolutionArchive;
				case DSWEnvironmentType.UDS:
					DocumentUnitChain duc = _unitOfWork.Repository<DocumentUnitChain>().GetByDocumentUnit(documentUnit).FirstOrDefault();
					return duc.ArchiveName;
			}

			return string.Empty;
		}

		private async Task<DocumentGeneratorModel> BuildDocumentInfoMetadata(WorkflowProperty dsw_e_Generate_DocumentMetadatas, DocumentUnit documentUnit)
		{
			DocumentGeneratorModel documentGeneratorModel = new DocumentGeneratorModel
			{
				DocumentGeneratorParameters = JsonConvert.DeserializeObject<ICollection<IDocumentGeneratorParameter>>(dsw_e_Generate_DocumentMetadatas.ValueString, ServiceHelper.SerializerSettings)
			};
			IDocumentGeneratorParameter documentsParameter = documentGeneratorModel.DocumentGeneratorParameters.FirstOrDefault(x => x.Name == "Documents");
			if (documentsParameter != null && documentsParameter is BooleanParameter && (documentsParameter as BooleanParameter).Value)
			{
				foreach (DocumentUnitChain chain in documentUnit.DocumentUnitChains)
				{
					ICollection<Document> biblosDocuments = await _documentService.GetDocumentsFromChainAsync(chain.IdArchiveChain);
					List<string> documentsInfos = new List<string>();
					foreach (Document biblosDocument in biblosDocuments)
					{
						documentsInfos.Add($"{biblosDocument.Name}({biblosDocument.CreatedDate:dd/MM/yyyy HH:mm:ss} - {biblosDocument.IdDocument})");
					}
					documentGeneratorModel.DocumentGeneratorParameters.Add(new StringParameter($"Document_{chain.ChainType}_Info", string.Join(", ", documentsInfos)));
				}
			}

			return documentGeneratorModel;
		}

		private async Task<byte[]> AppendFascicleContentToWordDocument(Guid idTemplate, List<DocumentGeneratorModel> extraContentParameters, DocumentGeneratorModel documentGeneratorModel)
		{
			WordDocumentModel wordDocument = await (await WordGeneratorBuilderExtension
				.GetLatestVersionAsync(idTemplate))
				.AppendTable(extraContentParameters.FirstOrDefault())
				.AppendTable(extraContentParameters.LastOrDefault())
				.GenerateDocumentAsync(idTemplate, documentGeneratorModel);
			return wordDocument.Stream;
		}

		private string AddWorkflowDocumentModelChainId(string referenceModel, Guid? chainId, Guid? archiveDocumentId)
		{
			WorkflowDocumentModel workflowDocumentModel = JsonConvert.DeserializeObject<WorkflowDocumentModel>(referenceModel, ServiceHelper.SerializerSettings);
			workflowDocumentModel.Documents.Add(new KeyValuePair<Model.Entities.DocumentUnits.ChainType, DocumentModel>(Model.Entities.DocumentUnits.ChainType.Miscellanea, new DocumentModel() { ChainId = chainId, DocumentId = archiveDocumentId }));
			return JsonConvert.SerializeObject(workflowDocumentModel);
		}

		private string ReplaceUDSModelTokens(string udsModel, string udsId, string documentName, string idDocument, string udsObject)
		{
			return udsModel
				.Replace(WORKFLOW_UDS_ID, udsId)
				.Replace(WORKFLOW_DOCUMENT_NAME, documentName)
				.Replace(WORKFLOW_ID_DOCUMENT, idDocument)
				.Replace(WORKFLOW_OBJECT, udsObject);
		}
		#endregion

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