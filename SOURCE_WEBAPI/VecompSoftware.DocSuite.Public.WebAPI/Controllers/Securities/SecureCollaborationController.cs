using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Collaborations;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Public.WebAPI.Handlers;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Collaborations;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.ExternalSecurities;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Securities
{
    [AzureAuthorize]
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class SecureCollaborationController : ApiController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        protected readonly Guid _instanceId;
        private readonly ITopicService _topicService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IMessageConfiguration _messageConfiguration;
        private readonly IPDFDocumentGenerator _pdfDocumentGenerator;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        private readonly ICollaborationService _collaborationService;
        #endregion

        #region [ Constructor ]
        public SecureCollaborationController(ILogger logger, ITopicService topicService, IDecryptedParameterEnvService parameterEnvService,
            IDataUnitOfWork unitOfWork, ICQRSMessageMapper cqrsMapper, IMessageConfiguration messageConfiguration, 
            IPDFDocumentGenerator pdfDocumentGenerator, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService,
            ICollaborationService collaborationService)
            : base()
        {
            _logger = logger;
            _topicService = topicService;
            _parameterEnvService = parameterEnvService;
            _instanceId = Guid.NewGuid();
            _unitOfWork = unitOfWork;
            _cqrsMapper = cqrsMapper;
            _messageConfiguration = messageConfiguration;
            _pdfDocumentGenerator = pdfDocumentGenerator;
            _documentService = documentService;
            _collaborationService = collaborationService;
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(SecureCollaborationController));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Methods ]

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Guid authenticationId, Guid token, Guid instanceId)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                _logger.WriteInfo(new LogMessage($"Secure collaboration request for authenticationId {authenticationId}, instanceid {instanceId} and token {token}"), LogCategories);
                if (!IsValidAuthenticationId(authenticationId))
                {
                    _logger.WriteWarning(new LogMessage($"WorkflowRepository with id '{authenticationId}' not found or doesn't have environment '{DSWEnvironmentType.Any}' or the _dsw_p_ExternalViewerIntegrationEnabled is not found"), LogCategories);
                    throw new DSWSecurityException($"AuthenticationId {authenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                }
                string topicName = _messageConfiguration.GetConfigurations()["EventTokenSecurity"].TopicName;
                if (!await _topicService.SubscriptionExists(topicName, token.ToString()))
                {
                    _logger.WriteWarning(new LogMessage($"Subscription {topicName}/{token} doesn't exists and has no valid token {authenticationId}"), LogCategories);
                    throw new DSWSecurityException($"AuthenticationId {authenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                }
                ServiceBusMessage serviceBusMessage = (await _topicService.GetMessagesAsync(topicName, token.ToString())).FirstOrDefault();
                IEventTokenSecurity tokenSecurityModel = null;
                if (serviceBusMessage == null || (tokenSecurityModel = (IEventTokenSecurity)serviceBusMessage.Content) == null)
                {
                    _logger.WriteWarning(new LogMessage($"AuthenticationId {authenticationId}:{topicName}/{token} has no valid token TokenSecurityModel is null"), LogCategories);
                    throw new DSWSecurityException($"Token {token} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                }
                WorkflowInstance workflowInstance = _unitOfWork.Repository<WorkflowInstance>().GetByColllaborationReferenceId(instanceId, authenticationId).SingleOrDefault();
                if (workflowInstance == null)
                {
                    _logger.WriteWarning(new LogMessage($"ReferenceId {instanceId} not found"), LogCategories);
                    throw new DSWSecurityException($"ReferenceId {instanceId} not found", null, DSWExceptionCode.SC_InvalidAccount);
                }
                if (tokenSecurityModel.ContentType.ContentTypeValue.DocumentUnitAuhtorized.UniqueId != workflowInstance.UniqueId)
                {
                    _logger.WriteWarning(new LogMessage($"Token {token} is not valid for collaboration {workflowInstance.UniqueId}/{instanceId}, was defined for {tokenSecurityModel.ContentType.ContentTypeValue.DocumentUnitAuhtorized.UniqueId}"), LogCategories);
                    throw new DSWSecurityException($"Token {token} is not valid for collaboration {workflowInstance.UniqueId}/{instanceId}", null, DSWExceptionCode.SC_InvalidAccount);
                }
                _logger.WriteDebug(new LogMessage($"SecureToken is {tokenSecurityModel.ContentType.ContentTypeValue.Token} expire {tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate}"), LogCategories);
                if (tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate.HasValue && tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate.Value < DateTimeOffset.UtcNow)
                {
                    _logger.WriteWarning(new LogMessage($"Token {token} is expired {tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate}"), LogCategories);
                    throw new DSWSecurityException($"Token is expired", null, DSWExceptionCode.SC_InvalidAccount);
                }
                WorkflowProperty collaborationIdProperty = _unitOfWork.Repository<WorkflowProperty>().GetByInstanceIdAndPropertyName(workflowInstance.UniqueId, WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID);
                if (collaborationIdProperty == null || !collaborationIdProperty.ValueInt.HasValue)
                {
                    _logger.WriteWarning(new LogMessage($"WorkflowPropety {WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID} not found for WorkflowInstance {workflowInstance.UniqueId}"), LogCategories);
                    throw new DSWException($"WorkflowPropety {WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID} not found for WorkflowInstance {workflowInstance.UniqueId}", null, DSWExceptionCode.Invalid);
                }
                Collaboration collaboration = _unitOfWork.Repository<Collaboration>().GetById((int)collaborationIdProperty.ValueInt.Value).SingleOrDefault();
                if(collaboration == null)
                {
                    _logger.WriteWarning(new LogMessage($"Collaboration with di {collaborationIdProperty.ValueInt.Value} not found."), LogCategories);
                    throw new DSWException($"Collaboration with di {collaborationIdProperty.ValueInt.Value} not found.", null, DSWExceptionCode.Invalid);
                }
                if (collaboration.IdStatus == CollaborationStatusType.Registered)
                {
                    _logger.WriteWarning(new LogMessage($"Cannot delete collaboration in status {CollaborationStatusType.Registered}."), LogCategories);
                    throw new DSWException($"Cannot delete collaboration in status {CollaborationStatusType.Registered}.", null, DSWExceptionCode.Invalid);
                }
                _logger.WriteInfo(new LogMessage($"Detaching biblos documents"), LogCategories);
                foreach (CollaborationVersioning collaborationVersioning in collaboration.CollaborationVersionings.Where(x => x.IsActive))
                {
                    Guid idDocument = await _documentService.GetDocumentIdAsync(collaborationVersioning.IdDocument, collaboration.Location.ProtocolArchive);
                    await _documentService.DetachDocumentAsync(idDocument);
                }
                _logger.WriteInfo(new LogMessage($"Deleting collaboration with UniqueId {collaboration.UniqueId}"), LogCategories);
                _unitOfWork.BeginTransaction();
                await _collaborationService.DeleteAsync(collaboration, DocSuiteWeb.Common.Infrastructures.DeleteActionType.DeleteCollaboration);
                await _unitOfWork.SaveAsync();

                collaboration.WorkflowName = workflowInstance.WorkflowRepository.Name;
                EventDeleteCollaboration @event = new EventDeleteCollaboration(tokenSecurityModel.TenantName, tokenSecurityModel.TenantId, tokenSecurityModel.TenantAOOId, tokenSecurityModel.Identity, collaboration);
                ServiceBusMessage message = _cqrsMapper.Map(@event, new ServiceBusMessage());
                _logger.WriteInfo(new LogMessage($"Sending EventDeleteCollaboration through brokered message service"), LogCategories);
                message = await _topicService.SendToTopicAsync(message);
                return Ok();
            }, _logger, LogCategories);
        }

        private bool IsValidAuthenticationId(Guid authenticationId)
        {
            WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().GetIncludingEvaluationProperties(authenticationId);

            if (workflowRepository == null || workflowRepository.DSWEnvironment != DSWEnvironmentType.Any)
            {
                return false;
            }

            WorkflowEvaluationProperty externalIntegrationProperty = workflowRepository.WorkflowEvaluationProperties.FirstOrDefault(p => p.Name == WorkflowPropertyHelper.DSW_PROPERTY_EXTERNALVIEWER_INTEGRATION_ENABLED);
            bool isExternalIntegrationEnabled = externalIntegrationProperty != null && externalIntegrationProperty.ValueBoolean.HasValue && externalIntegrationProperty.ValueBoolean.Value;

            return isExternalIntegrationEnabled;
        }

        #endregion
    }
}
