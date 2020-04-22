using System;
using System.Threading.Tasks;
using VecompSoftware.Clients.Workflow;
using VecompSoftware.DocSuite.Document.Generator.OpenXml.Word;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
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
    public class WorkflowPublishService : WorkflowBaseService<WorkflowPublish>, IWorkflowPublishService, IDisposable
    {
        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowPublishService(ILogger logger, IWorkflowInstanceService workflowInstanceService,
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
        { }
        #endregion

        #region [ Methods ]


        protected override async Task<WorkflowResult> BeforeCreateAsync(WorkflowPublish content)
        {
            // TODO: implentare metodi asyn nel client del WF
            await Task.Delay(1);
            WorkflowResult validationResult = new WorkflowResult();
            WorkflowManagement wf = new WorkflowManagement(ClientConfig);
            validationResult.IsValid = wf.Publish(content.WorkflowName, content.WorkflowXaml, null);
            validationResult.Errors = _mapper_to_translation_error.MapCollection(wf.GetLastValidationErrors());
            return validationResult;
        }
        #endregion
    }
}