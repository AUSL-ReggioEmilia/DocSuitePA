using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Workflows;
using VecompSoftware.Core.Command.CQRS.Events.Models.ExternalViewer;
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
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
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
    public class WorkflowNotifyService : WorkflowBaseService<WorkflowNotify>, IWorkflowNotifyService, IDisposable
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

        public WorkflowNotifyService(ILogger logger, IWorkflowInstanceService workflowInstanceService, ICQRSMessageMapper mapper_cqrsMessageMapper,
            IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
            ITopicService topicService, ICQRSMessageMapper mapper_eventServiceBusMessage, IDataUnitOfWork unitOfWork, 
            StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService, ICollaborationService collaborationService, 
            ISecurity security, IParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService, IMessageService messageService,
            IDossierRoleService dossierRoleService, IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator,
            IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService, IPDFDocumentGenerator pdfDocumentGenerator,
            IFascicleService fascicleService, IFascicleDocumentService fascicleDocumentService, IFascicleFolderService fascicleFolderService,
            IFascicleDocumentUnitService fascDocumentUnitService, IFascicleLinkService fascicleLinkService)
            : base(logger, workflowInstanceService, workflowInstanceRoleService, workflowActivityService, topicService, mapper_eventServiceBusMessage, 
                  unitOfWork, documentService, collaborationService, security, parameterEnvService, fascicleRoleService,
                  messageService, dossierRoleService, queueService, wordOpenXmlDocumentGenerator, messageConfiguration,
                  protocolLogService, pdfDocumentGenerator, fascicleService, fascicleDocumentService, fascicleFolderService, fascDocumentUnitService,
                  fascicleLinkService)
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

        private WorkflowProperty UpdateWorkflowProperty(WorkflowArgument workflowArgument, WorkflowProperty workflowProperty)
        {
            if (workflowProperty.ObjectState == Repository.Infrastructure.ObjectState.Added)
            {
                return workflowProperty;
            }
            switch (workflowProperty.PropertyType)
            {
                case WorkflowPropertyType.PropertyString:
                case WorkflowPropertyType.Json:
                    {
                        workflowProperty.ValueString = workflowArgument.ValueString;
                        break;
                    }
                case WorkflowPropertyType.PropertyGuid:
                case WorkflowPropertyType.RelationGuid:
                    {
                        workflowProperty.ValueGuid = workflowArgument.ValueGuid;
                        break;
                    }
                case WorkflowPropertyType.PropertyInt:
                case WorkflowPropertyType.RelationInt:
                    {
                        workflowProperty.ValueInt = workflowArgument.ValueInt;
                        break;
                    }
                case WorkflowPropertyType.PropertyDate:
                    {
                        workflowProperty.ValueDate = workflowArgument.ValueDate;
                        break;
                    }
                case WorkflowPropertyType.PropertyDouble:
                    {
                        workflowProperty.ValueDouble = workflowArgument.ValueDouble;
                        break;
                    }
                case WorkflowPropertyType.PropertyBoolean:
                    {
                        workflowProperty.ValueBoolean = workflowArgument.ValueBoolean;
                        break;
                    }
                default:
                    break;
            }
            _unitOfWork.Repository<WorkflowProperty>().Update(workflowProperty);
            _logger.WriteDebug(new LogMessage($"WorkflowNotify update {workflowProperty.Name}"), LogCategories);
            return workflowProperty;
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
                    WorkflowType = WorkflowType.Activity
                };
                _unitOfWork.Repository<WorkflowProperty>().Insert(workflowProperty);
                _logger.WriteDebug(new LogMessage($"WorkflowNotify add new {workflowProperty.Name}"), LogCategories);

                workflowActivity.WorkflowProperties.Add(workflowProperty);
                evaluationWorkflowProperties.Add(workflowProperty);
            }

            WorkflowProperty dsw_p_CurrentStep = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_CURRENT_STEP);
            WorkflowStep currentStep = JsonConvert.DeserializeObject<WorkflowStep>(dsw_p_CurrentStep.ValueString, ServiceHelper.SerializerSettings);
            WorkflowProperty workflowPropertyToUpdate;
            foreach (WorkflowArgument item in currentStep.OutputArguments.Where(f => content.OutputArguments.Any(x => x.Key == f.Name)))
            {
                workflowPropertyToUpdate = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == item.Name);
                if (workflowPropertyToUpdate != null)
                {
                    workflowPropertyToUpdate = UpdateWorkflowProperty(content.OutputArguments[workflowPropertyToUpdate.Name], workflowPropertyToUpdate);
                }
            }

            workflowActivity.Status = WorkflowStatus.Progress;
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

            evaluationWorkflowProperties.AddRange(workflowActivity.WorkflowInstance.WorkflowProperties.Where(f => !f.Name.Equals(WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE) && !evaluationWorkflowProperties.Any(p => p.Name == f.Name)));

            _unitOfWork.BeginTransaction();

            if (currentStep.EvaluationArguments.Any(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_TO_HANDLER))
            {
                EvaluateFascicleHandleDate(evaluationWorkflowProperties, workflowActivity);
            }

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
                    await PopulateActivityAsync(workflowActivity.WorkflowInstance, workflowActivity.WorkflowInstance.InstanceId.Value,
                        workflowActivity.WorkflowInstance.WorkflowRepository,
                        evaluationWorkflowProperties.Where(f => !f.Name.Equals(WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE)),
                        currentStepNumber: currentStep.Position + 1, idArchiveChain: workflowActivity.IdArchiveChain);
                }
                WorkflowArgument dsw_p_Subject = content.OutputArguments.SingleOrDefault(f => f.Value != null && f.Value.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT).Value;
                if (dsw_p_Subject != null && !string.IsNullOrEmpty(dsw_p_Subject.ValueString))
                {
                    workflowActivity.Subject = dsw_p_Subject.ValueString;
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
                await PopulateActivityAsync(workflowActivity.WorkflowInstance, workflowActivity.WorkflowInstance.InstanceId.Value,
                    workflowActivity.WorkflowInstance.WorkflowRepository, evaluationWorkflowProperties, currentStepNumber: (int)retrostep.Value);
            }

            if (currentStep.EvaluationArguments.Any(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL))
            {
                WorkflowProperty dsw_e_ActivityEndReferenceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_REFERENCE_MODEL);
                if (dsw_e_ActivityEndReferenceModel != null && dsw_e_ActivityEndReferenceModel.PropertyType == WorkflowPropertyType.Json && !string.IsNullOrEmpty(dsw_e_ActivityEndReferenceModel.ValueString))
                {
                    WorkflowReferenceModel activityReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_e_ActivityEndReferenceModel.ValueString, ServiceHelper.SerializerSettings);
                    if (activityReferenceModel != null && !string.IsNullOrEmpty(activityReferenceModel.ReferenceModel))
                    {
                        workflowActivity.IdArchiveChain = await ArchiveDocument(dsw_e_ActivityEndReferenceModel, activityReferenceModel, workflowActivity.IdArchiveChain);
                    }
                }
            }            

            KeyValuePair<string, WorkflowArgument> dsw_a_Collaboration_ChangeSigner = content.OutputArguments.SingleOrDefault(f => f.Value.Name == WorkflowPropertyHelper.DSW_ACTION_COLLABORATION_CHANGE_SIGNER);
            if (dsw_a_Collaboration_ChangeSigner.Key != null && dsw_a_Collaboration_ChangeSigner.Value.PropertyType == ArgumentType.Json && !string.IsNullOrEmpty(dsw_a_Collaboration_ChangeSigner.Value.ValueString))
            {
                workflowActivity.Status = WorkflowStatus.Todo;
                workflowActivity.Name = EvaluateCollaborationChangeSigner(dsw_a_Collaboration_ChangeSigner.Value.ValueString, workflowActivity);
            }

            _unitOfWork.Repository<WorkflowActivity>().Update(workflowActivity);
            await _unitOfWork.SaveAsync();
            if (workflowActivity.Status == WorkflowStatus.Done)
            {
                WorkflowProperty dsw_p_TenantId = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID);
                WorkflowProperty dsw_p_TenantName = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME);
                WorkflowProperty dsw_p_TenantAOOId = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID);
                WorkflowArgument dsw_a_Activity_AutoComplete = currentStep.EvaluationArguments.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_AUTO_COMPLETE);
                IEventCompleteWorkflowActivity evt = new EventCompleteWorkflowActivity(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value,
                    CurrentIdentityContext, workflowActivity, dsw_a_Activity_AutoComplete == null || !dsw_a_Activity_AutoComplete.ValueBoolean.HasValue ? false : dsw_a_Activity_AutoComplete.ValueBoolean.Value);
                ServiceBusMessage message = _mapper_cqrsMessageMapper.Map(evt, new ServiceBusMessage());
                ServiceBusMessage response = await _topicService.SendToTopicAsync(message);
                _logger.WriteInfo(
                    new LogMessage($"WorkflowActivity ${workflowActivity.Name}[{workflowActivity.WorkflowInstance.InstanceId}] in Tenant [{dsw_p_TenantName.ValueString}/{dsw_p_TenantId.ValueGuid}] sended notification [{response.MessageId}]."),
                    LogCategories);
            }
            validationResult.IsValid = true;
            validationResult.InstanceId = workflowActivity.WorkflowInstance.InstanceId;

            if (currentStep.ActivityType == Model.Workflow.WorkflowActivityType.CollaborationToProtocol || currentStep.ActivityType == Model.Workflow.WorkflowActivityType.ProtocolCreate || currentStep.ActivityType == Model.Workflow.WorkflowActivityType.UDSToProtocol)
            {
                await EvaluateEventProtocolExternalViewerAsync(content.WorkflowName, currentStep.ActivityType, currentStep.ActivityOperation, workflowActivity);
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
                    _logger.WriteInfo(new LogMessage($"Collaboration WorkflowActivity {workflowActivity.Name}/{workflowActivity.Subject} was done"), LogCategories);
                }
                return res;
            }
            return true;
        }

        private string EvaluateCollaborationChangeSigner(string collaboration_ChangeSigner, WorkflowActivity workflowActivity)
        {
            WorkflowProperty dsw_p_Model = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_MODEL);
            if (dsw_p_Model == null || dsw_p_Model.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_Model.ValueString))
            {
                throw new DSWException($"Unable to change Signer in a collaboration activity {workflowActivity.UniqueId} without specifying the collaboration model", null, DSWExceptionCode.WF_RulesetValidation);
            }

            CollaborationModel collaborationModel = JsonConvert.DeserializeObject<CollaborationModel>(dsw_p_Model.ValueString, ServiceHelper.SerializerSettings);
            CollaborationSignModel collaborationSingModel = JsonConvert.DeserializeObject<CollaborationSignModel>(collaboration_ChangeSigner, ServiceHelper.SerializerSettings);
            WorkflowProperty dsw_p_SignerPosition = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION);
            if (dsw_p_SignerPosition == null || dsw_p_SignerPosition.PropertyType != WorkflowPropertyType.PropertyInt || !dsw_p_SignerPosition.ValueInt.HasValue)
            {
                throw new DSWException($"Unable to change Signer in a collaboration activity {workflowActivity.UniqueId}  without specifying the signer position", null, DSWExceptionCode.WF_RulesetValidation);
            }
            int signerPosition = (int)dsw_p_SignerPosition.ValueInt.Value;
            int newsignerPosition = signerPosition + 1;
            int newIncremetalPosition = newsignerPosition + 1;
            dsw_p_SignerPosition.ValueInt = newsignerPosition;
            _unitOfWork.Repository<WorkflowProperty>().Update(dsw_p_SignerPosition);
            _logger.WriteInfo(new LogMessage($"Collaboration WorkflowActivity {workflowActivity.Name}/{workflowActivity.Subject} SignerPosition "), LogCategories);

            List<CollaborationSignModel> collaborationSignModels = collaborationModel.CollaborationSigns.OrderBy(f => f.Incremental).ToList();
            CollaborationSignModel collaborationSign = collaborationSignModels[signerPosition];
            string collaborationSignToRemoveAccount = collaborationSign.SignUser;
            WorkflowProperty dsw_p_SignerModel = workflowActivity.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL);
            List<CollaborationSignerWorkflowModel> collaborationSigners = JsonConvert.DeserializeObject<List<CollaborationSignerWorkflowModel>>(dsw_p_SignerModel.ValueString, ServiceHelper.SerializerSettings);

            CollaborationSignerWorkflowModel deferedCollaborationSigneModel = new CollaborationSignerWorkflowModel()
            {
                UserName = collaborationSign.SignUser,
                HasApproved = true,
                ExecuteDate = DateTimeOffset.UtcNow
            };
            collaborationSigners.Add(deferedCollaborationSigneModel);
            dsw_p_SignerModel.ValueString = JsonConvert.SerializeObject(collaborationSigners);
            _unitOfWork.Repository<WorkflowProperty>().Update(dsw_p_SignerModel);
            _logger.WriteInfo(new LogMessage($"Collaboration WorkflowActivity {workflowActivity.Name}/{workflowActivity.Subject} add Signermodel"), LogCategories);

            foreach (CollaborationSignModel colSignModel in collaborationSignModels.Where(x => x.Incremental >= newIncremetalPosition))
            {
                colSignModel.Incremental = (short)(colSignModel.Incremental + 1);
            }

            collaborationSign = new CollaborationSignModel();
            collaborationSign.IdCollaborationSign = collaborationSingModel.IdCollaborationSign;
            collaborationSign.SignEmail = collaborationSingModel.SignEmail;
            collaborationSign.SignName = collaborationSingModel.SignName;
            collaborationSign.SignUser = collaborationSingModel.SignUser;
            collaborationSign.IsRequired = collaborationSingModel.IsRequired;
            collaborationSign.Incremental = (short)(newIncremetalPosition);
            collaborationSignModels.Add(collaborationSign);

            collaborationModel.CollaborationSigns = collaborationSignModels;

            dsw_p_Model.ValueString = JsonConvert.SerializeObject(collaborationModel);
            _unitOfWork.Repository<WorkflowProperty>().Update(dsw_p_Model);

            _logger.WriteInfo(new LogMessage($"Added collaborationSing {collaborationSign.SignName} after {collaborationSignToRemoveAccount} for workflowActivity {workflowActivity.UniqueId}"), LogCategories);

            List<WorkflowAccount> workflowAccounts = new List<WorkflowAccount>();
            workflowAccounts.Add(new WorkflowAccount
            {
                AccountName = collaborationSign.SignUser,
                DisplayName = collaborationSign.SignName,
                EmailAddress = collaborationSign.SignEmail,
                Required = collaborationSign.IsRequired.Value
            });

            WorkflowProperty workflowProperty_accounts = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS);
            workflowProperty_accounts.ValueString = JsonConvert.SerializeObject(workflowAccounts);
            _unitOfWork.Repository<WorkflowProperty>().Update(workflowProperty_accounts);

            WorkflowAuthorization workflowAuthorization = _unitOfWork.Repository<WorkflowAuthorization>().GetByUser(collaborationSignToRemoveAccount, workflowActivity.UniqueId);
            workflowAuthorization.Account = collaborationSign.SignUser;
            _unitOfWork.Repository<WorkflowAuthorization>().Update(workflowAuthorization);

            WorkflowInstanceLog workflowInstanceLog = new WorkflowInstanceLog()
            {
                LogType = WorkflowInstanceLogType.Information,
                LogDescription = $"Cambio responsabile da {collaborationSignToRemoveAccount} a {collaborationSign.SignName}",
                SystemComputer = Environment.MachineName,
                Entity = workflowActivity.WorkflowInstance

            };
            _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLog);
            WorkflowActivityLog workflowActivityLog = new WorkflowActivityLog()
            {
                LogType = WorkflowStatus.Todo,
                LogDescription = $"Cambio responsabile da {collaborationSignToRemoveAccount} a {collaborationSign.SignName}",
                SystemComputer = Environment.MachineName,
                RegistrationUser = _security.GetCurrentUser().Account,
                Entity = workflowActivity
            };
            _unitOfWork.Repository<WorkflowActivityLog>().Insert(workflowActivityLog);
            return $"Documento '{collaborationModel.Subject}' in firma a {collaborationSignToRemoveAccount} nella collaborazione { collaborationModel.IdCollaboration}            ";
        }

        /// <summary>
        ///     If _dsw_a_ToHandler output workflow argument exists, updates the current user's workflow activity authorization
        ///     and sets IsHandler to true
        /// </summary>
        /// <param name="toHandlerWorkflowProperty">The _dsw_a_ToHandler workflow property</param>
        /// <param name="workflowActivity">The current workflow activity</param>
        /// <returns>True if current user has been set as handler, otherwise false</returns>
        private bool TryAuthorizeWorkflowActivityForCurrentUser(WorkflowProperty toHandlerWorkflowProperty, WorkflowActivity workflowActivity)
        {
            if (toHandlerWorkflowProperty == null || !toHandlerWorkflowProperty.ValueBoolean.HasValue || !toHandlerWorkflowProperty.ValueBoolean.Value)
            {
                return false;
            }

            _logger.WriteDebug(new LogMessage($"Evaluating {WorkflowPropertyHelper.DSW_ACTION_TO_HANDLER} workflow evaluation property"), LogCategories);

            string currentUserAccount = _security.GetCurrentUser().Account;
            WorkflowAuthorization currentUserWorkflowAuthorization = _unitOfWork.Repository<WorkflowAuthorization>().GetByUser(currentUserAccount, workflowActivity.UniqueId);

            if (currentUserWorkflowAuthorization == null)
            {
                _logger.WriteWarning(new LogMessage($"Autorizzazione del flusso di lavoro '{workflowActivity.Name}' non trovata per l'utente corrente '{currentUserAccount}'"), LogCategories);
                return false;
            }

            _logger.WriteDebug(new LogMessage($"Updating the workflow authorization handler property for current user '{currentUserAccount}'"), LogCategories);

            currentUserWorkflowAuthorization.IsHandler = true;
            _unitOfWork.Repository<WorkflowAuthorization>().Update(currentUserWorkflowAuthorization);

            _logger.WriteDebug(new LogMessage($"Workflow authorization handler property updated correctly"), LogCategories);

            WorkflowInstanceLog workflowInstanceLog = new WorkflowInstanceLog()
            {
                LogType = WorkflowInstanceLogType.WFTakeCharge,
                LogDescription = $"Assegnata a '{currentUserAccount}', l'attività di presa in carico '{workflowActivity.Name}'",
                SystemComputer = Environment.MachineName,
                Entity = workflowActivity.WorkflowInstance
            };

            _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLog);
            return true;
        }

        /// <summary>
        ///     If _dsw_a_Metadata_HandlerDate output argument exists and current user is the authorized handler for the workflow activity, 
        ///     update fascicle's handle date metadata value with current datetime
        /// </summary>
        /// <param name="evaluationProperties">The evaluation properties of the current workflow</param>
        /// <param name="workflowActivity">The current workflow activity</param>
        private void EvaluateFascicleHandleDate(List<WorkflowProperty> evaluationProperties, WorkflowActivity workflowActivity)
        {
            WorkflowProperty dsw_p_ReferenceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
            WorkflowProperty dsw_a_ToHandler = evaluationProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_TO_HANDLER);
            WorkflowProperty dsw_a_Metadata_HandlerDate = evaluationProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_METADATA_HANDLER_DATE);

            if (dsw_p_ReferenceModel == null || dsw_p_ReferenceModel.PropertyType != WorkflowPropertyType.Json || string.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString))
            {
                throw new DSWValidationException("Assignment validation error",
                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico senza aver specificato il modello di referenza" } },
                    null, DSWExceptionCode.VA_RulesetValidation);
            }

            if ((dsw_a_ToHandler == null || !dsw_a_ToHandler.ValueBoolean.HasValue || !dsw_a_ToHandler.ValueBoolean.Value)
                || (dsw_a_Metadata_HandlerDate == null || string.IsNullOrEmpty(dsw_a_Metadata_HandlerDate.ValueString)) 
                || !TryAuthorizeWorkflowActivityForCurrentUser(dsw_a_ToHandler, workflowActivity))
            {
                return;
            }
            
            _logger.WriteDebug(new LogMessage($"Evaluating fascicle {WorkflowPropertyHelper.DSW_ACTION_METADATA_HANDLER_DATE} workflow evaluation property"), LogCategories);

            WorkflowReferenceModel workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(dsw_p_ReferenceModel.ValueString, ServiceHelper.SerializerSettings);
            
            if (workflowReferenceModel.ReferenceType != Model.Entities.Commons.DSWEnvironmentType.Fascicle)
            {
                return;
            }

            Fascicle fascicle = null;
            if (workflowReferenceModel == null || (fascicle = _unitOfWork.Repository<Fascicle>().GetWithRoles(workflowReferenceModel.ReferenceId).SingleOrDefault()) == null)
            {
                throw new DSWValidationException("Assignment validation error",
                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico se l'entità non esiste" } },
                    null, DSWExceptionCode.VA_RulesetValidation);
            }

            MetadataDesignerModel metadataModel = JsonConvert.DeserializeObject<MetadataDesignerModel>(fascicle.MetadataDesigner);
            if (metadataModel == null || metadataModel.DateFields == null || !metadataModel.DateFields.Any(d => d.KeyName == dsw_a_Metadata_HandlerDate.ValueString))
            {
                throw new DSWValidationException("Assignment validation error",
                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = $"Impossibile completare una presa in carico se l'entità non prevede il metadata specifico per la data di presa in carico" } },
                    null, DSWExceptionCode.VA_RulesetValidation);
            }

            ICollection<MetadataValueModel> fascicleMetadataValues = new List<MetadataValueModel>();
            if (!string.IsNullOrEmpty(fascicle.MetadataValues))
            {
                fascicleMetadataValues = JsonConvert.DeserializeObject<ICollection<MetadataValueModel>>(fascicle.MetadataValues);
            }

            MetadataValueModel handleDateMetadata = fascicleMetadataValues.SingleOrDefault(metadata => metadata.KeyName == dsw_a_Metadata_HandlerDate.ValueString);
            DateTime currentDatetime = DateTime.UtcNow;
            handleDateMetadata.Value = currentDatetime.ToString("yyyy-MM-dd");

            MetadataValue handleDateMetadataValue = _unitOfWork.Repository<MetadataValue>().GetByNameAndFascicle(dsw_a_Metadata_HandlerDate.ValueString, fascicle.UniqueId, true).SingleOrDefault();
            handleDateMetadataValue.ValueDate = currentDatetime;
            _unitOfWork.Repository<MetadataValue>().Update(handleDateMetadataValue);

            fascicle.MetadataValues = JsonConvert.SerializeObject(fascicleMetadataValues);
            _unitOfWork.Repository<Fascicle>().Update(fascicle);

            _logger.WriteDebug(new LogMessage($"Fascicle {dsw_a_Metadata_HandlerDate.ValueString} metadata updated correctly"), LogCategories);

            WorkflowInstanceLog workflowInstanceLog = new WorkflowInstanceLog()
            {
                LogType = WorkflowInstanceLogType.WFTakeCharge,
                LogDescription = $"Aggiornato il metadato '{dsw_a_Metadata_HandlerDate.ValueString}' con la data di presa in carico nell'attività '{workflowActivity.Name}'",
                SystemComputer = Environment.MachineName,
                Entity = workflowActivity.WorkflowInstance
            };

            _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLog);
        }


        private async Task EvaluateAssignmentAsync(WorkflowStep currentStep, WorkflowActivity workflowActivity)
        {
            if (workflowActivity.ActivityType == DocSuiteWeb.Entity.Workflows.WorkflowActivityType.Assignment)
            {
                WorkflowProperty dsw_p_WorkflowEndMotivationRequired = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_END_MOTIVATION_REQUIRED);
                WorkflowProperty dsw_p_ReferenceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
                WorkflowProperty dsw_p_Roles = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLES);
                WorkflowProperty dsw_p_AcceptanceModel = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE);
                WorkflowProperty dsw_a_PublicFascicle_Temporary = workflowActivity.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_PUBLIC_TEMPORARY_ENFORCEMENT);
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
                bool forceRemoveRole = false;
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
                            MetadataDesignerModel metadataModel = JsonConvert.DeserializeObject<MetadataDesignerModel>(fascicle.MetadataDesigner);
                            if (metadataModel == null || metadataModel.DiscussionFields == null || !metadataModel.DiscussionFields.Any(d => d.Label == dsw_a_Metadata_Motivation_Label.ValueString))
                            {
                                throw new DSWValidationException("Assignment validation error",
                                    new List<ValidationMessageModel>() { new ValidationMessageModel { Key = "WorkflowNotify", Message = "Impossibile completare una presa in carico se l'entità non prevede il metadata specifico per la motivazione" } },
                                    null, DSWExceptionCode.VA_RulesetValidation);
                            }

                            ICollection<MetadataValueModel> metadataValues = new List<MetadataValueModel>();
                            if (!string.IsNullOrEmpty(fascicle.MetadataValues))
                            {
                                metadataValues = JsonConvert.DeserializeObject<ICollection<MetadataValueModel>>(fascicle.MetadataValues);
                            }

                            metadataMotivationLabel = dsw_a_Metadata_Motivation_Label.ValueString;
                            DiscussionFieldModel discussion = metadataModel.DiscussionFields.First(d => d.Label == dsw_a_Metadata_Motivation_Label.ValueString);
                            MetadataValueModel discussionLastValue = metadataValues.SingleOrDefault(d => d.KeyName == dsw_a_Metadata_Motivation_Label.ValueString);
                            if (discussion.Comments == null)
                            {
                                discussion.Comments = new List<CommentFieldModel>();
                            }
                            if (discussionLastValue == null)
                            {
                                discussionLastValue = new MetadataValueModel() { KeyName = dsw_a_Metadata_Motivation_Label.ValueString };
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
                            discussionLastValue.Value = acceptanceModel.AcceptanceReason;

                            MetadataValue acceptanceMetadataValue = _unitOfWork.Repository<MetadataValue>().GetByNameAndFascicle(dsw_a_Metadata_Motivation_Label.ValueString, fascicle.UniqueId, true).SingleOrDefault();
                            Action<MetadataValue> dbActionMetadata = (m) => _unitOfWork.Repository<MetadataValue>().Update(m);
                            if (acceptanceMetadataValue == null)
                            {
                                dbActionMetadata = (m) => _unitOfWork.Repository<MetadataValue>().Insert(m);
                                acceptanceMetadataValue = MetadataValueService.CreateMetadataValue(metadataModel, discussionLastValue);
                                acceptanceMetadataValue.Fascicle = fascicle;
                            }
                            acceptanceMetadataValue.ValueString = discussionLastValue.Value;
                            dbActionMetadata(acceptanceMetadataValue);

                            fascicle.MetadataDesigner = JsonConvert.SerializeObject(metadataModel);
                            fascicle.MetadataValues = JsonConvert.SerializeObject(metadataValues);                            
                            _unitOfWork.Repository<Fascicle>().Update(fascicle);
                        }
                    }

                    string endWorkflowDescription = string.Empty;
                    if (acceptanceModel.Status == AcceptanceStatus.Refused)
                    {
                        forceRemoveRole = true;
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
                    bool needRemoveRole = workflowActivity.WorkflowInstance.WorkflowProperties.Any(f => f.Name.Equals(WorkflowPropertyHelper.DSW_ACTION_FASCICLE_REMOVE_EVALUATED_AUTHORIZATION) && f.ValueBoolean.HasValue && f.ValueBoolean.Value);
                    _logger.WriteDebug(new LogMessage($"Request remove({needRemoveRole}) authorization from fascicle {fascicle.Title}"), LogCategories);
                    if (forceRemoveRole)
                    {
                        _logger.WriteDebug(new LogMessage($"Force remove authorization by refused activity from fascicle {fascicle.Title}"), LogCategories);
                        needRemoveRole = true;
                    }

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
                WorkflowProperty dsw_p_TenantAOOId = workflowActivity.WorkflowInstance.WorkflowProperties.Single(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID);
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

                EventProtocolExternalViewer evt = new EventProtocolExternalViewer(dsw_p_TenantName.ValueString, dsw_p_TenantId.ValueGuid.Value, dsw_p_TenantAOOId.ValueGuid.Value, workflowName,
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

        #endregion
    }
}
