using Microsoft.Activities;
using Microsoft.Workflow.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Clients.Workflow;
using VecompSoftware.DocSuite.Document.Generator.OpenXml.Word;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Helpers.Workflow;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using StorageDocument = VecompSoftware.DocSuite.Document;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    [LogCategory(LogCategoryDefinition.SERVICEWF)]
    public class WorkflowManagerStartService : WorkflowBaseService<WorkflowStart>, IWorkflowStartService, IDisposable
    {
        #region [ Fields ]
        private readonly IWorkflowInstanceService _workflowInstanceService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IWorkflowArgumentMapper _workflowArgumentMapper;
        private readonly IParameterEnvService _parameterEnvService;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowManagerStartService(ILogger logger, IWorkflowInstanceService workflowInstanceService, IWorkflowArgumentMapper workflowArgumentMapper,
            IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
            ITopicService topicServiceBus, ITranslationErrorMapper mapper_to_translation_error, ICQRSMessageMapper mapper_eventServiceBusMessage,
            IDataUnitOfWork unitOfWork, StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService,
            ICollaborationService collaborationService, ISecurity security, IParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService, 
            IMessageService messageService, IDossierRoleService dossierRoleService, IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator,
            IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService, IPDFDocumentGenerator pdfDocumentGenerator)
            : base(logger, workflowInstanceService, workflowInstanceRoleService, workflowActivityService, topicServiceBus,
                  mapper_to_translation_error, mapper_eventServiceBusMessage, unitOfWork, documentService,
                  collaborationService, security, parameterEnvService, fascicleRoleService, messageService, dossierRoleService, 
                  queueService, wordOpenXmlDocumentGenerator, messageConfiguration, protocolLogService, pdfDocumentGenerator)
        {
            _unitOfWork = unitOfWork;
            _workflowInstanceService = workflowInstanceService;
            _workflowArgumentMapper = workflowArgumentMapper;
            _parameterEnvService = parameterEnvService;
        }
        #endregion

        #region [ Methods ]
        protected override async Task<WorkflowResult> BeforeCreateAsync(WorkflowStart content)
        {
            return await StartWorkflowManager(content);
        }

        private async Task<WorkflowResult> StartWorkflowManager(WorkflowStart content)
        {
            WorkflowResult validationResult = new WorkflowResult();
            WorkflowManagement wf = new WorkflowManagement(ClientConfig);

            #region DocSuite Entities
            WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().Queryable()
                            .Single(f => f.Name.Equals(content.WorkflowName, StringComparison.InvariantCultureIgnoreCase));
            WorkflowInstance instance = new WorkflowInstance()
            {
                Status = WorkflowStatus.Todo,
                WorkflowRepository = workflowRepository,
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
            WorkflowProperty _dsw_p_UriToSendWorkflow = new WorkflowProperty()
            {
                PropertyType = WorkflowPropertyType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_URI_SEND_WORKFLOW_MANAGER,
                WorkflowType = WorkflowType.Workflow,
                ValueString = ClientConfig.UriToSendWorkflowManager
            };
            WorkflowProperty _dsw_p_TenantId = new WorkflowProperty()
            {
                PropertyType = WorkflowPropertyType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                WorkflowType = WorkflowType.Workflow,
                ValueGuid = _parameterEnvService.CurrentTenantId
            };
            WorkflowProperty _dsw_p_TenantName = new WorkflowProperty()
            {
                PropertyType = WorkflowPropertyType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                WorkflowType = WorkflowType.Workflow,
                ValueString = _parameterEnvService.CurrentTenantName
            };

            instance.WorkflowProperties.Add(dsw_p_UriToSendEntity);
            instance.WorkflowProperties.Add(dsw_p_UriToSendServiceBus);
            instance.WorkflowProperties.Add(_dsw_p_UriToSendWorkflow);
            instance.WorkflowProperties.Add(_dsw_p_TenantId);
            instance.WorkflowProperties.Add(_dsw_p_TenantName);

            Dictionary<string, DynamicValue> workflowProperties = new Dictionary<string, DynamicValue>
            {
                { WorkflowPropertyHelper.DSW_PROPERTY_URI_SEND_ENTITY, DynamicValue.Parse(JsonConvert.SerializeObject(dsw_p_UriToSendEntity, ServiceHelper.SerializerSettings)) },
                { WorkflowPropertyHelper.DSW_PROPERTY_URI_SEND_SERVICE_BUS, DynamicValue.Parse(JsonConvert.SerializeObject(dsw_p_UriToSendServiceBus, ServiceHelper.SerializerSettings)) },
                { WorkflowPropertyHelper.DSW_PROPERTY_URI_SEND_WORKFLOW_MANAGER, DynamicValue.Parse(JsonConvert.SerializeObject(_dsw_p_UriToSendWorkflow, ServiceHelper.SerializerSettings)) },
                { WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, DynamicValue.Parse(JsonConvert.SerializeObject(_dsw_p_TenantId, ServiceHelper.SerializerSettings)) },
                { WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, DynamicValue.Parse(JsonConvert.SerializeObject(_dsw_p_TenantName, ServiceHelper.SerializerSettings)) }
            };

            WorkflowProperty prop;
            foreach (KeyValuePair<string, WorkflowArgument> item in content.Arguments)
            {
                prop = _workflowArgumentMapper.Map(item.Value, new WorkflowProperty());
                instance.WorkflowProperties.Add(prop);
                workflowProperties.Add(item.Key, DynamicValue.Parse(JsonConvert.SerializeObject(prop, ServiceHelper.SerializerSettings)));
            }

            _unitOfWork.BeginTransaction();
            await _workflowInstanceService.CreateAsync(instance);
            bool result = await _unitOfWork.SaveAsync();
            #endregion

            WorkflowProperty dsw_p_WorkflowInstanceId = new WorkflowProperty()
            {
                PropertyType = WorkflowPropertyType.PropertyGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_INSTANCE_ID,
                WorkflowType = WorkflowType.Workflow,
                ValueGuid = instance.UniqueId
            };
            workflowProperties.Add(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_INSTANCE_ID, DynamicValue.Parse(JsonConvert.SerializeObject(dsw_p_WorkflowInstanceId, ServiceHelper.SerializerSettings)));

            WorkflowStartParameters startParameters = new WorkflowStartParameters();
            startParameters.Content.Add("WorkflowInstanceId", instance.UniqueId);
            startParameters.Content.Add("WorkflowProperties", workflowProperties);

            foreach (KeyValuePair<string, object> customStartParameter in content.StartParameters)
            {
                startParameters.Content.Add(customStartParameter.Key, customStartParameter.Value is long ? Convert.ToInt32(customStartParameter.Value) : customStartParameter.Value);
            }

            string instanceId = wf.Start(content.WorkflowName, startParameters.Content);
            _logger.WriteInfo(new LogMessage(string.Format("Workflow instanceId = {0}", instanceId)), LogCategories);
            if (!string.IsNullOrEmpty(instanceId))
            {
                instance.InstanceId = Guid.Parse(instanceId);
                instance.Status = WorkflowStatus.Progress;
                _unitOfWork.BeginTransaction();
                await _workflowInstanceService.UpdateAsync(instance);
                await _unitOfWork.SaveAsync();
                validationResult.InstanceId = instance.InstanceId;
            }
            validationResult.Errors = _mapper_to_translation_error.MapCollection(wf.GetLastValidationErrors());
            validationResult.IsValid = validationResult.Errors.Count == 0;
            // TODO: implentare metodi asyn nel client del WF

            return validationResult;
        }
        #endregion
    }
}

