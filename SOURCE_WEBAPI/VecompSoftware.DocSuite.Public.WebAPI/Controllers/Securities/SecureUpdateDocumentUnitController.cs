using IronPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Protocols;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Public.WebAPI.Controllers.CustomModules;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.ExternalSecurities;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Documents;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Entity.Commons.DSWEnvironmentType;
using VecompSoftware.DocSuite.Public.WebAPI.Handlers;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Securities
{
    [AzureAuthorize]
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class SecureUpdateDocumentUnitController : ApiController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        protected readonly Guid _instanceId;
        private readonly ITopicService _topicService;
        private readonly IQueueService _queueService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IMessageConfiguration _messageConfiguration;
        private readonly Location _workflowLocation;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region [ Constructor ]
        public SecureUpdateDocumentUnitController(ILogger logger, IQueueService queueService, ITopicService topicService, IDecryptedParameterEnvService parameterEnvService,
            IDataUnitOfWork unitOfWork, ICQRSMessageMapper cqrsMapper, IMessageConfiguration messageConfiguration, 
            IPDFDocumentGenerator pdfDocumentGenerator, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
            : base()
        {
            _logger = logger;
            _topicService = topicService;
            _queueService = queueService;
            _parameterEnvService = parameterEnvService;
            _instanceId = Guid.NewGuid();
            _unitOfWork = unitOfWork;
            _cqrsMapper = cqrsMapper;
            _messageConfiguration = messageConfiguration;
            _documentService = documentService;
            short workflowLocationId = _parameterEnvService.WorkflowLocationId;
            _workflowLocation = _unitOfWork.Repository<Location>().Find(workflowLocationId);
            if (_workflowLocation == null)
            {
                throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
            }

        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(TokenSecuritiesController));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Methods ]

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] SecureUpdateDocumentUnit documentUnit)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                _logger.WriteInfo(new LogMessage($"Secure update documentunit {documentUnit?.DocumentUnit}/{documentUnit?.Year}/{documentUnit?.Number} request for authenticationId {documentUnit?.AuthenticationId} and token {documentUnit?.Token}"), LogCategories);
                if (documentUnit == null)
                {
                    _logger.WriteWarning(new LogMessage($"Model is not valid"), LogCategories);
                    throw new DSWSecurityException($"Model is not valid", null, DSWExceptionCode.DB_Validation);
                }
                if (!IsValidAuthenticationId(documentUnit.AuthenticationId))
                {
                    _logger.WriteWarning(new LogMessage($"WorkflowRepository with id '{documentUnit.AuthenticationId}' not found or doesn't have environment '{DSWEnvironmentType.Any}' or the _dsw_p_ExternalViewerIntegrationEnabled is not found"), LogCategories);
                    throw new DSWSecurityException($"AuthenticationId {documentUnit.AuthenticationId} is not valid", null, DSWExceptionCode.DB_Validation);
                }
                if (string.IsNullOrWhiteSpace(documentUnit.Filename))
                {
                    _logger.WriteWarning(new LogMessage($"Model doesn't have valid filename '{documentUnit.Filename}'"), LogCategories);
                    throw new DSWSecurityException($"Model doesn't have valid filename '{documentUnit.Filename}'", null, DSWExceptionCode.DB_Validation);
                }
                if (documentUnit.Stream == null || documentUnit.Stream.Length <= 0)
                {
                    _logger.WriteWarning(new LogMessage($"Model doesn't have valid stream '{documentUnit.Stream?.Length}'"), LogCategories);
                    throw new DSWSecurityException($"Model doesn't have valid filename '{documentUnit.Stream?.Length}'", null, DSWExceptionCode.DB_Validation);
                }
                string topicName = _messageConfiguration.GetConfigurations()["EventTokenSecurity"].TopicName;
                if (!await _topicService.SubscriptionExists(topicName, documentUnit.Token.ToString()))
                {
                    _logger.WriteWarning(new LogMessage($"Subscription {topicName}/{documentUnit.Token} doesn't exists and has no valid token {documentUnit.AuthenticationId}"), LogCategories);
                    throw new DSWSecurityException($"AuthenticationId {documentUnit.AuthenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                }
                ServiceBusMessage serviceBusMessage = (await _topicService.GetMessagesAsync(topicName, documentUnit.Token.ToString())).FirstOrDefault();
                IEventTokenSecurity tokenSecurityModel = null;
                if (serviceBusMessage == null || (tokenSecurityModel = (IEventTokenSecurity)serviceBusMessage.Content) == null)
                {
                    _logger.WriteWarning(new LogMessage($"AuthenticationId {documentUnit.AuthenticationId}:{topicName}/{documentUnit.Token} has no valid token TokenSecurityModel is null"), LogCategories);
                    throw new DSWSecurityException($"Token {documentUnit.Token} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                }
                DocumentUnit reference = null;
                switch (documentUnit.DocumentUnit)
                {
                    case "Protocol":
                        {
                            reference = _unitOfWork.Repository<DocumentUnit>().GetByWorkflowRepositoryId(documentUnit.Year, documentUnit.Number, (int)DSWEnvironmentType.Protocol, documentUnit.AuthenticationId, optimization: true);
                            break;
                        }
                    default:
                        {
                            _logger.WriteWarning(new LogMessage($"AuthenticationId {documentUnit.AuthenticationId} has no valid documentUnit '{documentUnit.DocumentUnit}' name"), LogCategories);
                            throw new DSWSecurityException($"AuthenticationId {documentUnit.AuthenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                        }
                }
                if (reference == null)
                {
                    _logger.WriteWarning(new LogMessage($"DocumentUnit {documentUnit.DocumentUnit} - {documentUnit.Year}/{documentUnit.Number} not found"), LogCategories);
                    throw new DSWSecurityException($"DocumentUnit {documentUnit.DocumentUnit} - {documentUnit.Year}/{documentUnit.Number} not found", null, DSWExceptionCode.DB_Validation);
                }
                Protocol protocol = _unitOfWork.Repository<Protocol>().GetByUniqueIdWithRole(reference.UniqueId).SingleOrDefault();
                if (protocol == null)
                {
                    _logger.WriteWarning(new LogMessage($"Protocol {reference.UniqueId} - {reference.Year}/{reference.Number} not found"), LogCategories);
                    throw new DSWSecurityException($"Protocol {reference.UniqueId} - {reference.Year}/{reference.Number} not found", null, DSWExceptionCode.DB_Validation);
                }

                if (tokenSecurityModel.ContentType.ContentTypeValue.DocumentUnitAuhtorized.UniqueId != reference.UniqueId)
                {
                    _logger.WriteWarning(new LogMessage($"Token {documentUnit.Token} is not valid for protocol {reference.UniqueId}/{documentUnit.Year}/{documentUnit.Number}, was defined for {tokenSecurityModel.ContentType.ContentTypeValue.DocumentUnitAuhtorized.UniqueId}"), LogCategories);
                    throw new DSWSecurityException($"Token is not valid for the protocol {documentUnit.Year}/{documentUnit.Number}", null, DSWExceptionCode.SC_InvalidAccount);
                }
                _logger.WriteDebug(new LogMessage($"SecureToken is {tokenSecurityModel.ContentType.ContentTypeValue.Token} expire {tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate}"), LogCategories);
                if (tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate.HasValue && tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate.Value < DateTimeOffset.UtcNow)
                {
                    _logger.WriteWarning(new LogMessage($"Token {documentUnit.Token} is expired {tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate}"), LogCategories);
                    throw new DSWSecurityException($"Token is expired", null, DSWExceptionCode.SC_InvalidAccount);
                }

                ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel()
                {
                    UniqueId = Guid.NewGuid(),
                    Protocol = new ProtocolModel()
                    {
                        UniqueId = protocol.UniqueId,
                        Year = protocol.Year,
                        Number = protocol.Number,
                        Object = protocol.Object,
                        Roles = protocol.ProtocolRoles.Select(s => new ProtocolRoleModel()
                        {
                            UniqueId = s.UniqueId,
                            Role = new RoleModel() { IdRole = s.Role.EntityShortId }
                        }).ToList(),
                        Users = protocol.ProtocolUsers.Select(s => new ProtocolUserModel()
                        {
                            UniqueId = s.UniqueId,
                            Account = s.Account
                        }).ToList()
                    }
                };
                _logger.WriteInfo(new LogMessage($"Storing document in temporary workflow storage"), LogCategories);

                ArchiveDocument archiveDocument = await _documentService.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = documentUnit.Stream,
                    Name = documentUnit.Filename,
                });

                protocolBuildModel.Protocol.Annexes.Add(new DocumentModel()
                {
                    FileName = documentUnit.Filename,
                    DocumentToStoreId = archiveDocument.IdDocument
                });
                CommandUpdateProtocolData command = new CommandUpdateProtocolData(tokenSecurityModel.TenantName, tokenSecurityModel.TenantId, tokenSecurityModel.TenantAOOId, tokenSecurityModel.Identity, protocolBuildModel);
                ServiceBusMessage message = _cqrsMapper.Map(command, new ServiceBusMessage());
                _logger.WriteInfo(new LogMessage($"Sending command through brokered message service"), LogCategories);
                message = await _queueService.SubscribeQueue(message.ChannelName).SendToQueueAsync(message);
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
