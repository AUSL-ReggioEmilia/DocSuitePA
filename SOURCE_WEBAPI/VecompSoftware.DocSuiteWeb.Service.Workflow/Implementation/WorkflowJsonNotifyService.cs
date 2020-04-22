using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Workflows;
using VecompSoftware.Core.Command.CQRS.Events.Models.ExternalViewer;
using VecompSoftware.Core.Command.CQRS.Events.Models.Integrations.GenericProcesses;
using VecompSoftware.DocSuite.Document.Generator.OpenXml.Word;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
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
using VecompSoftware.Services.Command.CQRS.Events.Entities.Workflow;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using StorageDocument = VecompSoftware.DocSuite.Document;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    [LogCategory(LogCategoryDefinition.SERVICEWF)]
    public class WorkflowJsonNotifyService : WorkflowBaseService<WorkflowNotify>, IWorkflowNotifyService, IDisposable
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ICQRSMessageMapper _mapper_cqrsMessageMapper;
        private readonly ITopicService _topicService;
        private readonly IFascicleRoleService _fascicleRoleService;
        private readonly IDossierRoleService _dossierRoleService;
        private readonly IParameterEnvService _parameterEnvService;
        private readonly ISecurity _security;
        private readonly StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public WorkflowJsonNotifyService(ILogger logger, IWorkflowInstanceService workflowInstanceService, ICQRSMessageMapper mapper_cqrsMessageMapper,
            IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
            ITopicService topicService, ITranslationErrorMapper mapper_to_translation_error, ICQRSMessageMapper mapper_eventServiceBusMessage,
            IDataUnitOfWork unitOfWork, StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService,
            ICollaborationService collaborationService, ISecurity security, IParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService, 
            IMessageService messageService, IDossierRoleService dossierRoleService, IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator, 
            IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService, IPDFDocumentGenerator pdfDocumentGenerator)
            : base(logger, workflowInstanceService, workflowInstanceRoleService, workflowActivityService, topicService,
                  mapper_to_translation_error, mapper_eventServiceBusMessage, unitOfWork, documentService,
                  collaborationService, security, parameterEnvService, fascicleRoleService,
                  messageService, dossierRoleService, queueService, wordOpenXmlDocumentGenerator, messageConfiguration,
                  protocolLogService, pdfDocumentGenerator)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            _mapper_cqrsMessageMapper = mapper_cqrsMessageMapper;
            _topicService = topicService;
            _fascicleRoleService = fascicleRoleService;
            _dossierRoleService = dossierRoleService;
            _documentService = documentService;
            _parameterEnvService = parameterEnvService;
        }

        #endregion        

        #region [ Methods ]

        protected override async Task<WorkflowResult> BeforeCreateAsync(WorkflowNotify content)
        {
            _logger.WriteInfo(new LogMessage($"Workflow notifying for WorkflowActivityId {content.WorkflowActivityId}"), LogCategories);
            return await WorkflowJsonPushNotification(content);
        }

        private void ValidatePushNotification(WorkflowActivity workflowActivity)
        {
            WorkflowProperty dsw_p_WorkflowEndMotivationRequired = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_END_MOTIVATION_REQUIRED);
            if (dsw_p_WorkflowEndMotivationRequired != null && dsw_p_WorkflowEndMotivationRequired.ValueBoolean.HasValue && dsw_p_WorkflowEndMotivationRequired.ValueBoolean.Value)
            {
                WorkflowProperty dsw_p_AcceptanceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE);
                if (dsw_p_AcceptanceModel == null || string.IsNullOrEmpty(dsw_p_AcceptanceModel.ValueString))
                {
                    throw new DSWValidationException("WorkflowNotify validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare l'attività se non è stata fornita una motivazione" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                WorkflowAcceptanceModel acceptanceModel = JsonConvert.DeserializeObject<WorkflowAcceptanceModel>(dsw_p_AcceptanceModel.ValueString, ServiceHelper.SerializerSettings);
                if (acceptanceModel == null || string.IsNullOrEmpty(acceptanceModel.AcceptanceReason))
                {
                    throw new DSWValidationException("WorkflowNotify validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare l'attività se non è stata fornita una motivazione" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
            }
        }

        private async Task<WorkflowResult> WorkflowJsonPushNotification(WorkflowNotify content)
        {
            _logger.WriteDebug(new LogMessage($"Incoming notification on WorkflowActivity {content.WorkflowActivityId} related to WorkflowRepository {content.WorkflowName}/{content.ModuleName}"), LogCategories);

            WorkflowResult validationResult = new WorkflowResult();
            content.OutputArguments = content.OutputArguments ?? new Dictionary<string, WorkflowArgument>();
            WorkflowActivity workflowActivity = _unitOfWork.Repository<WorkflowActivity>().Get(content.WorkflowActivityId).Single();
            if (workflowActivity == null)
            {
                throw new DSWValidationException("Evaluate push notification",
                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = $"L'attività {content.WorkflowActivityId} specificata sul workflow {content.WorkflowName} non esiste" } },
                    null, DSWExceptionCode.VA_RulesetValidation);
            }
            if (workflowActivity.Status == WorkflowStatus.Done)
            {
                throw new DSWValidationException("Evaluate push notification",
                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = $"Impossibile inviare la notifica l'attività {workflowActivity.Name} se è stata completata {workflowActivity.Status}" } },
                    null, DSWExceptionCode.VA_RulesetValidation);
            }

            List<WorkflowProperty> evaluationWorkflowProperties = workflowActivity.WorkflowProperties
                .Where(f => !content.OutputArguments.Any(x => x.Value.Name == f.Name) &&
                       f.Name != WorkflowPropertyHelper.DSW_PROPERTY_OPERATION && f.Name != WorkflowPropertyHelper.DSW_PROPERTY_CURRENT_STEP).ToList();
            WorkflowProperty workflowProperty;
            foreach (KeyValuePair<string, WorkflowArgument> item in content.OutputArguments.Where(f => !workflowActivity.WorkflowProperties.Any(x => x.Name == f.Value.Name)))
            {
                workflowProperty = new WorkflowProperty()
                {
                    Name = item.Value.Name,
                    PropertyType = (WorkflowPropertyType)(int)item.Value.PropertyType,
                    ValueBoolean = item.Value.ValueBoolean,
                    ValueDate = item.Value.ValueDate,
                    ValueDouble = item.Value.ValueDouble,
                    ValueGuid = item.Value.ValueGuid,
                    ValueInt = item.Value.ValueInt,
                    ValueString = item.Value.ValueString,
                    WorkflowType = WorkflowType.Activity,
                    ObjectState = Repository.Infrastructure.ObjectState.Added
                };
                workflowActivity.WorkflowProperties.Add(workflowProperty);
                evaluationWorkflowProperties.Add(workflowProperty);
            }

            WorkflowProperty dsw_p_CurrentStep = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_CURRENT_STEP);
            WorkflowStep currentStep = JsonConvert.DeserializeObject<WorkflowStep>(dsw_p_CurrentStep.ValueString, ServiceHelper.SerializerSettings);
            workflowActivity.Status = WorkflowStatus.Progress;
            WorkflowActivity nextWorkflowActivity = null;
            foreach (KeyValuePair<string, WorkflowArgument> item in content.OutputArguments.Where(f => !evaluationWorkflowProperties.Any(x => x.Name == f.Value.Name)))
            {
                workflowProperty = new WorkflowProperty()
                {
                    Name = item.Value.Name,
                    PropertyType = (WorkflowPropertyType)(int)item.Value.PropertyType,
                    ValueBoolean = item.Value.ValueBoolean,
                    ValueDate = item.Value.ValueDate,
                    ValueDouble = item.Value.ValueDouble,
                    ValueGuid = item.Value.ValueGuid,
                    ValueInt = item.Value.ValueInt,
                    ValueString = item.Value.ValueString,
                    WorkflowType = WorkflowType.Activity,
                    ObjectState = Repository.Infrastructure.ObjectState.Added
                };
                evaluationWorkflowProperties.Add(workflowProperty);
            }
            evaluationWorkflowProperties.AddRange(workflowActivity.WorkflowInstance.WorkflowProperties.Where(f => !evaluationWorkflowProperties.Any(p => p.Name == f.Name)));

            _unitOfWork.BeginTransaction();
            bool hasAllArguments = currentStep.OutputArguments.All(f => evaluationWorkflowProperties.Any(x => x.Name == f.Name));
            _logger.WriteDebug(new LogMessage($"Current WorkflowActivity has all {hasAllArguments} OutputArguments"), LogCategories);
            if (!hasAllArguments)
            {
                foreach (KeyValuePair<string, WorkflowArgument> item in content.OutputArguments)
                {
                    _logger.WriteDebug(new LogMessage($"WorkflowNotify send {item.Key} OutputArgument"), LogCategories);
                }
                foreach (WorkflowArgument item in currentStep.OutputArguments)
                {
                    _logger.WriteDebug(new LogMessage($"Step need {item.Name} argument but not found in evaluation workflow properties [{evaluationWorkflowProperties.Any(f => f.Name == item.Name)}]"), LogCategories);
                }
            }
            if (hasAllArguments)
            {
                _logger.WriteDebug(new LogMessage($"Evalauting next step {currentStep.Position + 1}"), LogCategories);
                ValidatePushNotification(workflowActivity);
                await EvaluateAssignmentAsync(currentStep, workflowActivity);
                bool hasToContinue = EvaluateCollaborationSigns(currentStep, workflowActivity);
                if (hasToContinue)
                {
                    nextWorkflowActivity = await PopulateActivityAsync(workflowActivity.WorkflowInstance, workflowActivity.WorkflowInstance.InstanceId.Value,
                    workflowActivity.WorkflowInstance.WorkflowRepository, evaluationWorkflowProperties, currentStepNumber: currentStep.Position + 1,
                    idArchiveChain: workflowActivity.IdArchiveChain);
                }
                workflowActivity.Status = WorkflowStatus.Done;
            }
            WorkflowProperty dsw_a_Shared_To = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_SHARED_TO);

            if (workflowActivity.Status != WorkflowStatus.Done && dsw_a_Shared_To != null)
            {
                long? retrostep = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_SHARED_TO_STEP)?.ValueInt;
                retrostep = retrostep ?? currentStep.Position;
                WorkflowProperty dsw_p_Accounts_instance = evaluationWorkflowProperties.SingleOrDefault(f => f.Name.Equals(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS));
                dsw_p_Accounts_instance.ValueString = string.Copy(dsw_a_Shared_To.ValueString);
                nextWorkflowActivity = await PopulateActivityAsync(workflowActivity.WorkflowInstance, workflowActivity.WorkflowInstance.InstanceId.Value,
                    workflowActivity.WorkflowInstance.WorkflowRepository, evaluationWorkflowProperties, currentStepNumber: (int)retrostep.Value);
            }
            WorkflowProperty dsw_e_ActivityEndReferenceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL);

            if (dsw_e_ActivityEndReferenceModel != null && dsw_e_ActivityEndReferenceModel.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_ActivityEndReferenceModel.ValueString))
            {
                WorkflowReferenceModel activityReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_e_ActivityEndReferenceModel.ValueString, ServiceHelper.SerializerSettings);
                if (activityReferenceModel != null && !string.IsNullOrEmpty(activityReferenceModel.ReferenceModel))
                {
                    workflowActivity.IdArchiveChain = await ArchiveDocument(dsw_e_ActivityEndReferenceModel, activityReferenceModel, workflowActivity.IdArchiveChain);
                }
            }

            _unitOfWork.Repository<WorkflowActivity>().Update(workflowActivity);
            await _unitOfWork.SaveAsync();
            if (workflowActivity.Status == WorkflowStatus.Done)
            {
                WorkflowProperty dsw_p_TenantId = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
                WorkflowProperty dsw_p_TenantName = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
                IEventCompleteWorkflowActivity evt = new EventCompleteWorkflowActivity(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value,
                    CurrentIdentityContext, workflowActivity);
                ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(evt, new ServiceBusMessage());
                ServiceBusMessage response = await _topicService.SendToTopicAsync(message);
                _logger.WriteInfo(
                    new LogMessage($"WorkflowActivity ${workflowActivity.Name}[{workflowActivity.WorkflowInstance.InstanceId}] in Tenant [{dsw_p_TenantName.ValueString}/{dsw_p_TenantId.ValueGuid}] sended notification [{response.MessageId}]."),
                    LogCategories);
            }
            validationResult.IsValid = true;

            if (currentStep.ActivityType == Model.Workflow.WorkflowActivityType.CollaborationToProtocol || currentStep.ActivityType == Model.Workflow.WorkflowActivityType.ProtocolCreate || currentStep.ActivityType == Model.Workflow.WorkflowActivityType.UDSToProtocol)
            {
                await EvaluateEventProtocolExternalViewerAsync(content.WorkflowName, currentStep.ActivityType, currentStep.ActivityOperation, workflowActivity);
            }

            if (nextWorkflowActivity != null && nextWorkflowActivity.ActivityType == DocSuiteWeb.Entity.Workflows.WorkflowActivityType.DematerialisationStatement)
            {
                await EvaluateEventDematerialisationResponseAsync(workflowActivity);
            }

            if (nextWorkflowActivity != null && nextWorkflowActivity.ActivityType == DocSuiteWeb.Entity.Workflows.WorkflowActivityType.SecureDocumentCreate)
            {
                await EvaluateEventSecureDocumentResponseAsync(workflowActivity);
            }

            return validationResult;
        }

        private bool EvaluateCollaborationSigns(WorkflowStep currentStep, WorkflowActivity workflowActivity)
        {
            if (currentStep.ActivityType == Model.Workflow.WorkflowActivityType.CollaborationSign)
            {
                WorkflowProperty dsw_p_SignerModel = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL);
                List<CollaborationSignerWorkflowModel> collaborationSigners = JsonConvert.DeserializeObject<List<CollaborationSignerWorkflowModel>>(dsw_p_SignerModel.ValueString, ServiceHelper.SerializerSettings);
                bool res = collaborationSigners.All(f => f.HasApproved);
                if (!res)
                {
                    workflowActivity.WorkflowInstance.Status = WorkflowStatus.Done;
                    _unitOfWork.Repository<WorkflowInstance>().Update(workflowActivity.WorkflowInstance);
                }
                return res;
            }
            return true;
        }

        private async Task EvaluateAssignmentAsync(WorkflowStep currentStep, WorkflowActivity workflowActivity)
        {
            if (workflowActivity.ActivityType == DocSuiteWeb.Entity.Workflows.WorkflowActivityType.Assignment)
            {
                WorkflowProperty dsw_p_WorkflowEndMotivationRequired = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_END_MOTIVATION_REQUIRED);
                WorkflowProperty dsw_p_ReferenceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
                WorkflowProperty dsw_p_Roles = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLES);
                WorkflowProperty dsw_p_AcceptanceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE);
                WorkflowProperty dsw_a_PublicFascicle_Temporary = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_PUBLIC_FASCICLE_TEMPORARY_ENFORCEMENT);
                WorkflowProperty dsw_a_Metadata_Motivation_Label = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_METADATA_MOTIVATION_LABEL);
                if (dsw_p_ReferenceModel == null || dsw_p_ReferenceModel.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
                {
                    throw new DSWValidationException("Assignment validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico senza aver specificato il modello di referenza" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                if (IsRoleAuthorization(currentStep) && (dsw_p_Roles == null || dsw_p_Roles.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_Roles.ValueString)))
                {
                    throw new DSWValidationException("Assignment validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico senza aver specificato almeno un settore" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                WorkflowReferenceModel workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_p_ReferenceModel.ValueString, ServiceHelper.SerializerSettings);
                Fascicle fascicle = null;
                if (workflowReferenceModel.ReferenceType == Model.Entities.Commons.DSWEnvironmentType.Fascicle)
                {
                    if (workflowReferenceModel == null || (fascicle = _unitOfWork.Repository<Fascicle>().GetWithRoles(workflowReferenceModel.ReferenceId).SingleOrDefault()) == null)
                    {
                        throw new DSWValidationException("Assignment validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico se l'entità non esiste" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }

                    if (dsw_p_WorkflowEndMotivationRequired != null && dsw_p_WorkflowEndMotivationRequired.ValueBoolean.HasValue && dsw_p_WorkflowEndMotivationRequired.ValueBoolean.Value && 
                        (dsw_a_Metadata_Motivation_Label == null || fascicle.MetadataValues == null))
                    {
                        throw new DSWValidationException("Assignment validation error",
                            new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico se la motivazione è obbligatoria e l'entità non prevede metadati dinamici" } },
                            null, DSWExceptionCode.VA_RulesetValidation);
                    }
                }

                if (dsw_p_AcceptanceModel != null && !string.IsNullOrEmpty(dsw_p_AcceptanceModel.ValueString))
                {
                    string metadataMotivationLabel = string.Empty;
                    WorkflowAcceptanceModel acceptanceModel = JsonConvert.DeserializeObject<WorkflowAcceptanceModel>(dsw_p_AcceptanceModel.ValueString, ServiceHelper.SerializerSettings);
                    DomainUserModel userModel = _security.GetCurrentUser();
                    if (acceptanceModel != null && !string.IsNullOrEmpty(acceptanceModel.AcceptanceReason))
                    {
                        if (dsw_a_Metadata_Motivation_Label == null || string.IsNullOrEmpty(dsw_a_Metadata_Motivation_Label.ValueString) || fascicle.MetadataValues == null)
                        {
                            throw new DSWValidationException("Assignment validation error",
                                new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico se l'entità non prevede metadati dinamici" } },
                                null, DSWExceptionCode.VA_RulesetValidation);
                        }
                        if (workflowReferenceModel.ReferenceType == Model.Entities.Commons.DSWEnvironmentType.Fascicle)
                        {
                            MetadataModel metadataModel = JsonConvert.DeserializeObject<MetadataModel>(fascicle.MetadataValues);

                            if (metadataModel == null || metadataModel.DiscussionFields == null || !metadataModel.DiscussionFields.Any(d => d.Label == dsw_a_Metadata_Motivation_Label.ValueString))
                            {
                                throw new DSWValidationException("Assignment validation error",
                                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico se l'entità non prevede il metadata specifico per la motivazione" } },
                                    null, DSWExceptionCode.VA_RulesetValidation);
                            }

                            metadataMotivationLabel = dsw_a_Metadata_Motivation_Label.ValueString;
                            DiscussionFieldModel discussion = metadataModel.DiscussionFields.First(d => d.Label == dsw_a_Metadata_Motivation_Label.ValueString);
                            if (discussion.Comments == null)
                            {
                                discussion.Comments = new List<CommentFieldModel>();
                            }
                            if (userModel == null)
                            {
                                throw new DSWValidationException("Assignment validation error",
                                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile identificare l'utente che sta completando la presa in carico" } },
                                    null, DSWExceptionCode.VA_RulesetValidation);
                            }

                            CommentFieldModel comment = new CommentFieldModel()
                            {
                                Author = string.Concat(userModel.Domain, "\\", userModel.Name),
                                RegistrationDate = DateTimeOffset.UtcNow,
                                Comment = acceptanceModel.AcceptanceReason
                            };
                            discussion.Comments.Add(comment);
                            discussion.Comments = discussion.Comments.OrderByDescending(c => c.RegistrationDate).ToList();
                            fascicle.MetadataValues = JsonConvert.SerializeObject(metadataModel);
                            _unitOfWork.Repository<Fascicle>().Update(fascicle);
                        }
                    }

                    string endWorkflowDescription = string.Empty;
                    if (acceptanceModel.Status == AcceptanceStatus.Refused)
                    {
                        endWorkflowDescription = $"Rifiutato il flusso di Lavoro '{workflowActivity.WorkflowInstance.WorkflowRepository.Name}'";
                        if (!string.IsNullOrEmpty(metadataMotivationLabel))
                        {
                            endWorkflowDescription = $"{endWorkflowDescription} con motivazione salvata nel metadato '{metadataMotivationLabel}'";
                        }
                        if (workflowReferenceModel.ReferenceType == Model.Entities.Commons.DSWEnvironmentType.Fascicle)
                        {
                            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(fascicle, FascicleLogType.Workflow, endWorkflowDescription, CurrentDomainUser.Account));
                        }

                        WorkflowInstanceLog wfILog = new WorkflowInstanceLog()
                        {
                            LogType = WorkflowInstanceLogType.WFRefused,
                            LogDescription = endWorkflowDescription,
                            SystemComputer = Environment.MachineName,
                            Entity = workflowActivity.WorkflowInstance

                        };
                        _unitOfWork.Repository<WorkflowInstanceLog>().Insert(wfILog);

                    }
                    if (acceptanceModel != null && acceptanceModel.Status == AcceptanceStatus.Accepted)
                    {
                        endWorkflowDescription = $"Il flusso di Lavoro '{workflowActivity.WorkflowInstance.WorkflowRepository.Name}' è stato completato con parere positivo da '{userModel.Account}'";
                        if (!string.IsNullOrEmpty(metadataMotivationLabel))
                        {
                            endWorkflowDescription = $"{endWorkflowDescription} con motivazione salvata nel metadato '{metadataMotivationLabel}'";
                        }
                        if (workflowReferenceModel.ReferenceType == Model.Entities.Commons.DSWEnvironmentType.Fascicle)
                        {
                            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(fascicle, FascicleLogType.Workflow, endWorkflowDescription, CurrentDomainUser.Account));
                        }

                        WorkflowInstanceLog workflowInstanceLog = new WorkflowInstanceLog()
                        {
                            LogType = WorkflowInstanceLogType.WFCompleted,
                            LogDescription = endWorkflowDescription,
                            SystemComputer = Environment.MachineName,
                            Entity = workflowActivity.WorkflowInstance

                        };
                        _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLog);
                    }
                }

                if (workflowReferenceModel.ReferenceType == Model.Entities.Commons.DSWEnvironmentType.Fascicle && IsRoleAuthorization(currentStep))
                {
                    bool needRemoveRole = workflowActivity.WorkflowInstance.WorkflowProperties.Any(f => f.Name.Equals(WorkflowPropertyHelper.DSW_ACTION_REMOVE_FASCICLE_EVALUATED_AUTHORIZATION) && f.ValueBoolean.HasValue && f.ValueBoolean.Value);
                    _logger.WriteDebug(new LogMessage($"Request remove({needRemoveRole}) authorization from fascicle {fascicle.Title}"), LogCategories);

                    ICollection<WorkflowMapping> workflowMappings = JsonConvert.DeserializeObject<ICollection<WorkflowMapping>>(dsw_p_Roles.ValueString, ServiceHelper.SerializerSettings);
                    FascicleRole fascicleRole;
                    foreach (WorkflowMapping workflowMapping in workflowMappings.Where(f => f.Role != null && f.Role.IdRole != 0))
                    {
                        fascicleRole = fascicle.FascicleRoles.SingleOrDefault(f => f.Role.EntityShortId == workflowMapping.Role.IdRole && f.AuthorizationRoleType == AuthorizationRoleType.Responsible);
                        if (fascicleRole == null)
                        {
                            _logger.WriteDebug(new LogMessage($"Role {workflowMapping.Role.IdRole} not founded in fascicle{fascicle.Title}"), LogCategories);
                        }
                        else
                        {
                            if (!fascicleRole.IsMaster)
                            {
                                if (needRemoveRole)
                                {
                                    await _fascicleRoleService.DeleteAsync(fascicleRole);
                                    _logger.WriteDebug(new LogMessage($"Role {fascicleRole.UniqueId} ({workflowMapping.Role.IdRole}) from fascicle {fascicle.Title} has been successfully removed."), LogCategories);
                                }
                                else
                                {
                                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                                    await _fascicleRoleService.UpdateAsync(fascicleRole);
                                    _logger.WriteDebug(new LogMessage($"Role {fascicleRole.UniqueId} ({workflowMapping.Role.IdRole}) from fascicle {fascicle.Title} has been successfully setted to Accounted."), LogCategories);
                                }
                            }
                            else
                            {
                                _logger.WriteDebug(new LogMessage($"Not removed role authorization identified {fascicleRole.UniqueId} ({workflowMapping.Role.IdRole}) because it's master for fascicle {fascicle.Title}"), LogCategories);
                            }
                        }
                    }
                }

                if (workflowReferenceModel.ReferenceType == Model.Entities.Commons.DSWEnvironmentType.Fascicle && dsw_a_PublicFascicle_Temporary != null && 
                    dsw_a_PublicFascicle_Temporary.ValueBoolean.HasValue && dsw_a_PublicFascicle_Temporary.ValueBoolean.Value)
                {
                    fascicle.VisibilityType = VisibilityType.Confidential;
                    _unitOfWork.Repository<Fascicle>().Update(fascicle);
                    _logger.WriteDebug(new LogMessage($"Fascicle visivility changed to confidential during activity '{workflowActivity.Name}' of workflow '{workflowActivity.WorkflowInstance.WorkflowRepository.Name}'"), LogCategories);
                    _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(fascicle, FascicleLogType.Workflow, $"Fascicolo reso privato durante completamento attività '{workflowActivity.Name}' - '{workflowActivity.WorkflowInstance.WorkflowRepository.Name}'", CurrentDomainUser.Account));

                    foreach (DossierFolder dossierFolder in fascicle.DossierFolders)
                    {
                        _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(dossierFolder.Dossier, dossierFolder, DossierLogType.Workflow,
                            $"Il flusso di lavoro '{workflowActivity.WorkflowInstance.WorkflowRepository.Name}' è stato completato sulla cartella {dossierFolder.Name}", CurrentDomainUser.Account));
                    }
                }
            }
        }

        private async Task EvaluateEventProtocolExternalViewerAsync(string workflowName, Model.Workflow.WorkflowActivityType activityType, WorkflowActivityOperation activityOperation, WorkflowActivity workflowActivity)
        {
            if (workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_EXTERNALVIEWER_URL)
                && workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR)
                && workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER))
            {
                WorkflowProperty dsw_p_TenantId = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
                WorkflowProperty dsw_p_TenantName = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
                WorkflowProperty dsw_e_ExternalViewerUrl = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_EXTERNALVIEWER_URL);
                WorkflowProperty dsw_e_ProtocolYear = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR);
                WorkflowProperty dsw_e_ProtocolNumber = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER);
                WorkflowProperty dsw_p_ExternalIdentifier = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER);

                ExternalViewerModel externalViewerModel = new ExternalViewerModel
                {
                    Url = dsw_e_ExternalViewerUrl.ValueString,
                    Sender = new ExternalViewerContactModel() { Name = dsw_e_ProtocolYear.RegistrationUser },
                    RegistrationUser = dsw_e_ProtocolYear.RegistrationUser,
                    RegistrationDate = dsw_e_ProtocolYear.RegistrationDate
                };

                if (dsw_e_ProtocolYear.ValueInt.HasValue)
                {
                    externalViewerModel.Year = (short)dsw_e_ProtocolYear.ValueInt.Value;
                }
                if (dsw_e_ProtocolNumber.ValueInt.HasValue)
                {
                    externalViewerModel.Number = (short)dsw_e_ProtocolNumber.ValueInt.Value;
                }
                if (dsw_p_ExternalIdentifier != null && dsw_p_ExternalIdentifier.ValueGuid.HasValue)
                {
                    externalViewerModel.UniqueId = dsw_p_ExternalIdentifier.ValueGuid.Value;
                }

                EventProtocolExternalViewer evt = new EventProtocolExternalViewer(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, workflowName,
                    activityType, activityOperation.Action, activityOperation.Area, CurrentIdentityContext, externalViewerModel);
                ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(evt, new ServiceBusMessage());
                ServiceBusMessage response = await _topicService.SendToTopicAsync(message);

                _logger.WriteInfo(new LogMessage(string.Concat("ProtocolExternalViewer : WorkflowActivity ", workflowActivity.Name,
                    " [", workflowActivity.UniqueId, "] in Tenant [", dsw_p_TenantName.ValueString, "/", dsw_p_TenantId.ValueGuid,
                    "] sended notification [", response.MessageId, "]",
                    externalViewerModel.UniqueId == Guid.Empty ? "." : string.Concat("with ExternalIdentifier [", externalViewerModel.UniqueId, "]."))),
                    LogCategories);
            }
        }

        private async Task EvaluateEventDematerialisationResponseAsync(WorkflowActivity workflowActivity)
        {
            if (workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_SIGNED)
                && workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL)
                && workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL))
            {
                WorkflowProperty dsw_p_TenantId = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
                WorkflowProperty dsw_p_TenantName = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
                WorkflowProperty dsw_p_ReferenceModel = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
                WorkflowProperty dsw_p_CollaborationModel = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);

                if (string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
                {
                    throw new DSWException("Impossibile completare la richiesta di Attestazione di conformità da Workflow senza aver specificato il modello di referenza", null, DSWExceptionCode.WF_RulesetValidation);
                }

                if (string.IsNullOrEmpty(dsw_p_CollaborationModel.ValueString))
                {
                    throw new DSWException("Impossibile completare la richiesta di Attestazione di conformità da Workflow senza aver specificato il modello della collaborazione", null, DSWExceptionCode.WF_RulesetValidation);
                }

                short collaborationLocationId = _parameterEnvService.CollaborationLocationId;
                Location collaborationLocation = _unitOfWork.Repository<Location>().Find(collaborationLocationId);
                if (collaborationLocation == null)
                {
                    throw new DSWException($"Collaboration Location {collaborationLocationId} not found", null, DSWExceptionCode.WF_Mapper);
                }

                CollaborationModel collaboration = JsonConvert.DeserializeObject<CollaborationModel>(dsw_p_CollaborationModel.ValueString);

                CollaborationVersioningModel lastVersion = collaboration.CollaborationVersionings.Where(c => c.CollaborationIncremental == 0).OrderByDescending(t => t.Incremental).First();

                Guid dematerialisationChainId = await _documentService.GetDocumentIdAsync(collaborationLocation.ProtocolArchive, lastVersion.IdDocument);

                DocumentManagementRequestModel dematerialisationModel = JsonConvert.DeserializeObject<DocumentManagementRequestModel>(dsw_p_ReferenceModel.ValueString);
                WorkflowReferenceBiblosModel dematerialisationDocument = new WorkflowReferenceBiblosModel
                {
                    ArchiveChainId = dematerialisationChainId,
                    ChainType = ChainType.DematerialisationChain
                };
                dematerialisationModel.Documents.Add(dematerialisationDocument);

                EventDematerialisationResponse evt = new EventDematerialisationResponse(null, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, CurrentIdentityContext, dematerialisationModel);
                ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(evt, new ServiceBusMessage());
                ServiceBusMessage response = await _topicService.SendToTopicAsync(message);

                _logger.WriteInfo(new LogMessage(string.Concat("Dematerialisation : WorkflowActivity ", workflowActivity.Name,
                    " [", workflowActivity.UniqueId, "] in Tenant [", dsw_p_TenantName.ValueString, "/", dsw_p_TenantId.ValueGuid,
                    "] sended notification [", response.MessageId, "]",
                    dematerialisationModel.UniqueId == Guid.Empty ? "." : string.Concat("with DematerialisationModel UniqueId [", dematerialisationModel.UniqueId, "]."))),
                    LogCategories);
            }
        }

        private async Task EvaluateEventSecureDocumentResponseAsync(WorkflowActivity workflowActivity)
        {
            if (workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_SIGNED)
                && workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL)
                && workflowActivity.WorkflowProperties.Any(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL))
            {
                WorkflowProperty dsw_p_TenantId = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
                WorkflowProperty dsw_p_TenantName = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
                WorkflowProperty dsw_p_ReferenceModel = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
                WorkflowProperty dsw_p_CollaborationModel = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);
                WorkflowProperty dsw_p_ExternalIdentifier = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_EXTERNAL_IDENTIFIER);

                if (string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
                {
                    throw new DSWException("Impossibile completare la richiesta di Securizzazione documento da Workflow senza aver specificato il modello di referenza", null, DSWExceptionCode.WF_RulesetValidation);
                }

                if (string.IsNullOrEmpty(dsw_p_CollaborationModel.ValueString))
                {
                    throw new DSWException("Impossibile completare la richiesta di Securizzazione documento da Workflow senza aver specificato il modello della collaborazione", null, DSWExceptionCode.WF_RulesetValidation);
                }

                short collaborationLocationId = _parameterEnvService.CollaborationLocationId;
                Location collaborationLocation = _unitOfWork.Repository<Location>().Find(collaborationLocationId);
                if (collaborationLocation == null)
                {
                    throw new DSWException(string.Concat("Collaboration Location ", collaborationLocationId, " not found"), null, DSWExceptionCode.WF_Mapper);
                }

                CollaborationModel collaboration = JsonConvert.DeserializeObject<CollaborationModel>(dsw_p_CollaborationModel.ValueString);

                CollaborationVersioningModel lastVersion = collaboration.CollaborationVersionings.Where(c => c.CollaborationIncremental == 0).OrderByDescending(t => t.Incremental).First();

                Guid secureDocumentChainId = await _documentService.GetDocumentIdAsync(collaborationLocation.ProtocolArchive, lastVersion.IdDocument);

                DocumentManagementRequestModel secureDocumentModel = JsonConvert.DeserializeObject<DocumentManagementRequestModel>(dsw_p_ReferenceModel.ValueString);
                WorkflowReferenceBiblosModel referenceDocument = secureDocumentModel.Documents.FirstOrDefault();
                if (referenceDocument == null)
                {
                    throw new DSWException("Impossibile completare la richiesta di Securizzazione documento da Workflow senza aver specificato il documento di riferimento", null, DSWExceptionCode.WF_RulesetValidation);
                }
                WorkflowReferenceBiblosModel secureDocument = new WorkflowReferenceBiblosModel
                {
                    ArchiveChainId = secureDocumentChainId,
                    ChainType = ChainType.MainChain,
                    ReferenceDocument = referenceDocument
                };
                secureDocumentModel.Documents.Clear();
                secureDocumentModel.Documents.Add(secureDocument);

                EventSecureDocumentResponse evt = new EventSecureDocumentResponse(null, dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, CurrentIdentityContext, secureDocumentModel, dsw_p_ExternalIdentifier.ValueString);
                ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(evt, new ServiceBusMessage());
                ServiceBusMessage response = await _topicService.SendToTopicAsync(message);

                _logger.WriteInfo(new LogMessage(string.Concat("SecureDocument : WorkflowActivity ", workflowActivity.Name,
                    " [", workflowActivity.UniqueId, "] in Tenant [", dsw_p_TenantName.ValueString, "/", dsw_p_TenantId.ValueGuid,
                    "] sended notification [", response.MessageId, "]",
                    secureDocumentModel.UniqueId == Guid.Empty ? "." : string.Concat("with DematerialisationModel UniqueId [", secureDocumentModel.UniqueId, "]."))),
                    LogCategories);
            }
        }

        #endregion
    }
}
