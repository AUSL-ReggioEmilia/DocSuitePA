using IronPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
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
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.ExternalSecurities;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Securities
{
    [AzureAuthorize]
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class SecureDocumentsController : ApiController
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
        #endregion

        #region [ Constructor ]
        public SecureDocumentsController(ILogger logger, ITopicService topicService, IDecryptedParameterEnvService parameterEnvService,
            IDataUnitOfWork unitOfWork, ICQRSMessageMapper cqrsMapper, IMessageConfiguration messageConfiguration, 
            IPDFDocumentGenerator pdfDocumentGenerator, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
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
            IronPdf.License.LicenseKey = "IRONPDF.DGROOVESRL.IRO210316.2898.42160.613012-F8C227A020-DX63ZNOLKZB42AM-5QIKPMZP5CCL-WDTTLNDKAYXG-JRKQMHANBMER-KSWYCZCYZVKM-Q6X5PR-LLUJSELMCFSEUA-OEM.1PRO.1YR-LQ4E7V.RENEW.SUPPORT.16.MAR.2022";
            if (!IronPdf.License.IsLicensed)
            {
                throw new InvalidOperationException("IronPdf licence expired");
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
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(SecureDocumentsController));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Methods ]

        [HttpPost]
        public async Task<IHttpActionResult> Post(Guid authenticationId, Guid token, string documentUnit, short year, int number)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                _logger.WriteInfo(new LogMessage($"Secure document request for authenticationId {authenticationId} and token {token}"), LogCategories);
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
                DocumentUnit reference = null;
                switch (documentUnit)
                {
                    case "Protocol":
                        {
                            reference = _unitOfWork.Repository<DocumentUnit>().GetByWorkflowRepositoryId(year, number, (int)DSWEnvironmentType.Protocol, authenticationId, optimization: true);
                            break;
                        }
                    default:
                        {
                            _logger.WriteWarning(new LogMessage($"AuthenticationId {authenticationId} has no valid documentUnit '{documentUnit}' name"), LogCategories);
                            throw new DSWSecurityException($"AuthenticationId {authenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                        }
                }
                if (reference == null)
                {
                    _logger.WriteWarning(new LogMessage($"DocumentUnit {documentUnit} - {year}/{number} not found"), LogCategories);
                    throw new DSWSecurityException($"DocumentUnit {documentUnit} - {year}/{number} not found", null, DSWExceptionCode.SC_InvalidAccount);
                }
                DocumentUnitChain documentUnitChain = reference.DocumentUnitChains.SingleOrDefault(f => f.ChainType == ChainType.MainChain);
                if (documentUnitChain == null)
                {
                    _logger.WriteWarning(new LogMessage($"DocumentUnit {documentUnit} - {year}/{number} doesn't have main document"), LogCategories);
                    throw new DSWSecurityException($"DocumentUnit {documentUnit} - {year}/{number} doesn't have main document", null, DSWExceptionCode.SC_InvalidAccount);
                }
                if (tokenSecurityModel.ContentType.ContentTypeValue.DocumentUnitAuhtorized.UniqueId != reference.UniqueId)
                {
                    _logger.WriteWarning(new LogMessage($"Token {token} is not valid for protocol {reference.UniqueId}/{year}/{number}, was defined for {tokenSecurityModel.ContentType.ContentTypeValue.DocumentUnitAuhtorized.UniqueId}"), LogCategories);
                    throw new DSWSecurityException($"Token is not valid for the protocol {year}/{number}", null, DSWExceptionCode.SC_InvalidAccount);
                }
                _logger.WriteDebug(new LogMessage($"SecureToken is {tokenSecurityModel.ContentType.ContentTypeValue.Token} expire {tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate}"), LogCategories);
                if (tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate.HasValue && tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate.Value < DateTimeOffset.UtcNow)
                {
                    _logger.WriteWarning(new LogMessage($"Token {token} is expired {tokenSecurityModel.ContentType.ContentTypeValue.ExpiryDate}"), LogCategories);
                    throw new DSWSecurityException($"Token is expired", null, DSWExceptionCode.SC_InvalidAccount);
                }

                ModelDocument.Document document = (await _documentService.GetDocumentsFromChainAsync(documentUnitChain.IdArchiveChain)).SingleOrDefault();
                byte[] content = await _documentService.GetDocumentContentAsync(document.IdDocument);
                string signature = document.AttributeValues.SingleOrDefault(f => f.AttributeName.Equals(AttributeValue.ATTRIBUTE_SIGNATURE, StringComparison.InvariantCultureIgnoreCase))?.ValueString;
                _logger.WriteDebug(new LogMessage($"Generating PDF/A of {document.Name}{document.IdDocument}"), LogCategories);
                PdfDocument mergedPdfDocument = new PdfDocument(await _pdfDocumentGenerator.GeneratePdfAsync(content, document.Name, signature));
                byte[] returnStream = mergedPdfDocument.BinaryData;
                DocumentUnitChain documentUnitChainAttachment = reference.DocumentUnitChains.SingleOrDefault(f => f.ChainType == ChainType.AttachmentsChain);
                if (documentUnitChainAttachment != null && Guid.Empty != documentUnitChainAttachment.IdArchiveChain)
                {
                    foreach (ModelDocument.Document item in (await _documentService.GetDocumentsFromChainAsync(documentUnitChainAttachment.IdArchiveChain)))
                    {
                        content = await _documentService.GetDocumentContentAsync(item.IdDocument);
                        signature = item.AttributeValues.SingleOrDefault(f => f.AttributeName.Equals(AttributeValue.ATTRIBUTE_SIGNATURE, StringComparison.InvariantCultureIgnoreCase))?.ValueString;
                        _logger.WriteDebug(new LogMessage($"Generating PDF/A of {item.Name}{item.IdDocument}"), LogCategories);
                        mergedPdfDocument = new PdfDocument(returnStream);
                        returnStream = IronPdf.PdfDocument.Merge(mergedPdfDocument, new PdfDocument(await _pdfDocumentGenerator.GeneratePdfAsync(content, item.Name, signature))).BinaryData;
                    }
                }

                documentUnitChainAttachment = reference.DocumentUnitChains.SingleOrDefault(f => f.ChainType == ChainType.AnnexedChain);
                if (documentUnitChainAttachment != null && Guid.Empty != documentUnitChainAttachment.IdArchiveChain)
                {
                    foreach (ModelDocument.Document item in (await _documentService.GetDocumentsFromChainAsync(documentUnitChainAttachment.IdArchiveChain)))
                    {
                        content = await _documentService.GetDocumentContentAsync(item.IdDocument);
                        signature = item.AttributeValues.SingleOrDefault(f => f.AttributeName.Equals(AttributeValue.ATTRIBUTE_SIGNATURE, StringComparison.InvariantCultureIgnoreCase))?.ValueString;
                        _logger.WriteDebug(new LogMessage($"Generating PDF/A of {item.Name}{item.IdDocument}"), LogCategories);
                        mergedPdfDocument = new PdfDocument(returnStream);
                        returnStream = IronPdf.PdfDocument.Merge(mergedPdfDocument, new PdfDocument(await _pdfDocumentGenerator.GeneratePdfAsync(content, item.Name, signature))).BinaryData;
                    }
                }

                return Ok(Convert.ToBase64String(returnStream));
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
