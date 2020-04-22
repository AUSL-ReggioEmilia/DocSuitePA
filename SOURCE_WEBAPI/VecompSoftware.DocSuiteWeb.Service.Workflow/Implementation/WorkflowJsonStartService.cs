using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Document.Generator.OpenXml.Word;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Workflow;
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
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using StorageDocument = VecompSoftware.DocSuite.Document;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    [LogCategory(LogCategoryDefinition.SERVICEWF)]
    public class WorkflowJsonStartService : WorkflowBaseService<WorkflowStart>, IWorkflowStartService, IDisposable
    {
        #region [ Fields ]
        private readonly IWorkflowInstanceService _workflowInstanceService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IWorkflowArgumentMapper _workflowArgumentMapper;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowJsonStartService(ILogger logger, IWorkflowInstanceService workflowInstanceService, IWorkflowArgumentMapper workflowArgumentMapper,
            IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
            ITopicService topicServiceBus, ITranslationErrorMapper mapper_to_translation_error, ICQRSMessageMapper mapper_eventServiceBusMessage,
            IDataUnitOfWork unitOfWork, StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService,
            ICollaborationService collaborationService, ISecurity security, IParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService, 
            IMessageService messageService, IDossierRoleService dossierRoleService, IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator,
            IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService, IPDFDocumentGenerator pdfDocumentGenerator)
            : base(logger, workflowInstanceService, workflowInstanceRoleService, workflowActivityService, topicServiceBus,
                  mapper_to_translation_error, mapper_eventServiceBusMessage, unitOfWork, documentService,
                  collaborationService, security, parameterEnvService, fascicleRoleService, messageService, dossierRoleService, queueService, 
                  wordOpenXmlDocumentGenerator, messageConfiguration, protocolLogService, pdfDocumentGenerator)
        {
            _unitOfWork = unitOfWork;
            _workflowInstanceService = workflowInstanceService;
            _workflowArgumentMapper = workflowArgumentMapper;
        }
        #endregion

        #region [ Methods ]
        protected override async Task<WorkflowResult> BeforeCreateAsync(WorkflowStart content)
        {
            return await StartWorkflowJson(content);
        }

        private async Task<WorkflowResult> StartWorkflowJson(WorkflowStart content)
        {
            WorkflowResult validationResult = new WorkflowResult();

            WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().GetByName(content.WorkflowName);
            if (workflowRepository == null)
            {
                throw new DSWValidationException("Evaluate start workflow validation error",
                    new List<ValidationMessageModel>() 
                        { 
                            new ValidationMessageModel 
                            { 
                                Key = "WorkflowStart", 
                                Message = $"Impossibile avviare il workflow '{content.WorkflowName}' in quanto esiste nel repositories dei workflow validi." 
                            } 
                        }, null, DSWExceptionCode.VA_RulesetValidation);
            }
            WorkflowInstance workflowInstance = new WorkflowInstance()
            {
                Status = WorkflowStatus.Todo,
                WorkflowRepository = workflowRepository,
                Json = workflowRepository.Json
            };
            WorkflowProperty dsw_p_UriToSendEntity = new WorkflowProperty()
            {
                PropertyType = WorkflowPropertyType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_URI_SEND_ENTITY,
                WorkflowType = WorkflowType.Workflow,
                ValueString = ClientConfig.UriToSendEntity
            };
            WorkflowProperty dsw_p_UriToSendServiceBus = new WorkflowProperty()
            {
                PropertyType = WorkflowPropertyType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_URI_SEND_SERVICE_BUS,
                WorkflowType = WorkflowType.Workflow,
                ValueString = ClientConfig.UriToSendServiceBus
            };
            workflowInstance.WorkflowProperties.Add(dsw_p_UriToSendEntity);
            workflowInstance.WorkflowProperties.Add(dsw_p_UriToSendServiceBus);

            WorkflowProperty prop;
            foreach (KeyValuePair<string, WorkflowArgument> item in content.Arguments)
            {
                prop = _workflowArgumentMapper.Map(item.Value, new WorkflowProperty());
                prop.WorkflowType = WorkflowType.Workflow;
                workflowInstance.WorkflowProperties.Add(prop);
            }

            WorkflowProperty _dsw_p_WorkflowStartMotivationRequired = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_MOTIVATION_REQUIRED);
            if (_dsw_p_WorkflowStartMotivationRequired != null && _dsw_p_WorkflowStartMotivationRequired.ValueBoolean.HasValue && _dsw_p_WorkflowStartMotivationRequired.ValueBoolean.Value)
            {
                WorkflowProperty dsw_p_Subject = workflowInstance.WorkflowProperties.SingleOrDefault(f => f.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT);
                if (dsw_p_Subject == null || string.IsNullOrEmpty(dsw_p_Subject.ValueString))
                {
                    throw new DSWValidationException("Evaluate start workflow validation error",
                            new List<ValidationMessageModel>() 
                            { 
                                new ValidationMessageModel 
                                { 
                                    Key = "WorkflowStart", 
                                    Message = $"Impossibile avviare il flusso di lavoro se non è stata fornita una motivazione" 
                                } 
                            }, null, DSWExceptionCode.VA_RulesetValidation);
                }
            }

            _unitOfWork.BeginTransaction();
            Guid instanceId = Guid.NewGuid();
            workflowInstance = await _workflowInstanceService.CreateAsync(workflowInstance);
            WorkflowActivity workflowActivity = await PopulateActivityAsync(workflowInstance, instanceId, workflowInstance.WorkflowRepository, workflowInstance.WorkflowProperties);
            bool result = await _unitOfWork.SaveAsync();

            _unitOfWork.BeginTransaction();
            if (!workflowInstance.InstanceId.HasValue)
            {
                workflowInstance.InstanceId = instanceId;
            }
            workflowInstance.Status = WorkflowStatus.Progress;
            workflowInstance = await _workflowInstanceService.UpdateAsync(workflowInstance);
            result = await _unitOfWork.SaveAsync();
            validationResult.InstanceId = workflowInstance.InstanceId;
            validationResult.IsValid = true;
            _logger.WriteInfo(new LogMessage($"Assing workflowInstance.InstanceId [{workflowInstance.InstanceId}] for {workflowInstance.WorkflowRepository.Name}"), LogCategories);

            return validationResult;
        }

        #endregion
    }
}
