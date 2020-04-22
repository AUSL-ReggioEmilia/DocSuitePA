using System;
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
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using StorageDocument = VecompSoftware.DocSuite.Document;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    [LogCategory(LogCategoryDefinition.SERVICEWF)]
    public class WorkflowManagerNotifyService : WorkflowBaseService<WorkflowNotify>, IWorkflowNotifyService, IDisposable
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public WorkflowManagerNotifyService(ILogger logger, IWorkflowInstanceService workflowInstanceService,
            IWorkflowInstanceRoleService workflowInstanceRoleService, IWorkflowActivityService workflowActivityService,
            ITopicService topicServiceBus, ITranslationErrorMapper mapper_to_translation_error, ICQRSMessageMapper mapper_eventServiceBusMessage,
            IDataUnitOfWork unitOfWork, StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService,
            ICollaborationService collaborationService, ISecurity security, IParameterEnvService parameterEnvService, IFascicleRoleService fascicleRoleService, 
            IMessageService messageService, IDossierRoleService dossierRoleService, IQueueService queueService, IWordOpenXmlDocumentGenerator wordOpenXmlDocumentGenerator,
            IMessageConfiguration messageConfiguration, IProtocolLogService protocolLogService, IPDFDocumentGenerator pdfDocumentGenerator)
            : base(logger, workflowInstanceService, workflowInstanceRoleService, workflowActivityService, topicServiceBus,
                  mapper_to_translation_error, mapper_eventServiceBusMessage, unitOfWork, documentService,
                  collaborationService, security, parameterEnvService, fascicleRoleService,
                  messageService, dossierRoleService, queueService, wordOpenXmlDocumentGenerator, messageConfiguration, 
                  protocolLogService, pdfDocumentGenerator)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion        

        #region [ Methods ]

        protected override async Task<WorkflowResult> BeforeCreateAsync(WorkflowNotify content)
        {
            return await WorkflowManagerPushNotification(content);
        }

        private async Task<WorkflowResult> WorkflowManagerPushNotification(WorkflowNotify content)
        {
            WorkflowResult validationResult = new WorkflowResult();
            WorkflowManagement wf = new WorkflowManagement(ClientConfig);
            WorkflowActivity workflowActivity = (await _unitOfWork.Repository<WorkflowActivity>()
                .Query(f => f.UniqueId == content.WorkflowActivityId)
                .Include(f => f.WorkflowInstance.WorkflowRepository)
                .Include(f => f.WorkflowProperties)
                .SelectAsync()).Single();
            validationResult.IsValid = wf.PushNotification(content, workflowActivity);
            validationResult.Errors = _mapper_to_translation_error.MapCollection(wf.GetLastValidationErrors());
            foreach (TranslationError error in validationResult.Errors)
            {
                _logger.WriteError(new LogMessage(string.Concat("WorkflowManagerPushNotification -> ", error.Message)), LogCategories);
            }
            return validationResult;
        }
        #endregion
    }
}
