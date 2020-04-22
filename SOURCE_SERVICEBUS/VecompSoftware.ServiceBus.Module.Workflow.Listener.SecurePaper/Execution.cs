using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Integrations.GenericProcesses;

namespace VecompSoftware.ServiceBus.Module.Workflow.Listener.SecurePaper
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : IListenerExecution<ICommandSecurePaperRequest>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosDS.BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly StampaConforme.StampaConformeClient _stampaConformeClient;
        private readonly SecurePaperService _securePaperService;
        private readonly SecurePaperConfig _securePaperConfig;
        private readonly string _userName;
        private const string _biblos_attribute_filename = "filename";
        private const string _biblos_attribute_signature = "signature";
        private const string _biblos_attribute_securepaper_archivekey = "securepaperarchivekey";
        private const string _biblos_checkout_userId = "ServiceBusService";
        private const string PROGRAM_NAME = "SBReceiver";
        private const string SECUREDOCUMENT_COMPLETED_LOG_TYPE = "CT";
        private const string SECUREDOCUMENT_COMPLETED_LOG_DESCRIPTION = "Terminata con successo attività di applicazione del contrassegno a stampa del documento {0}[{1}]";
        private const string ODATA_FILTER_LOGTYPE = "$filter=LogType eq 'CS' and Entity/UniqueId eq {0} and contains(LogDescription, '{1}')";
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }

        public IDictionary<string, object> Properties { get; set; }

        public EvaluationModel RetryPolicyEvaluation { get; set; }
        #endregion

        #region [ Constructor ]
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient, StampaConforme.StampaConformeClient stampaConformeClient)
        {
            _logger = logger;
            _biblosClient = biblosClient;
            _webApiClient = webApiClient;
            _stampaConformeClient = stampaConformeClient;

            if (WindowsIdentity.GetCurrent() != null)
            {
                _userName = WindowsIdentity.GetCurrent().Name;
            }

            _securePaperConfig = new SecurePaperConfig
            {
                SecurePaperServiceId = _webApiClient.GetParameterSecurePaperServiceIdAsync().Result
            };
            _logger.WriteDebug(new LogMessage(string.Concat("Parameter SecurePaperServiceId -> ", _securePaperConfig.SecurePaperServiceId)), LogCategories);
            if (_securePaperConfig.SecurePaperServiceId == -1)
            {
                _logger.WriteWarning(new LogMessage("Parameter SecurePaperServiceId is not correct."), LogCategories);
                throw new Exception("Parameter SecurePaperServiceId is not correct.");
            }

            _securePaperConfig.CertificateThumbprint = _webApiClient.GetParameterSecurePaperCertificateThumbprintAsync().Result;
            _logger.WriteDebug(new LogMessage(string.Concat("Parameter SecurePaperCertificateThumbprint -> ", _securePaperConfig.CertificateThumbprint)), LogCategories);
            if (string.IsNullOrEmpty(_securePaperConfig.CertificateThumbprint))
            {
                _logger.WriteWarning(new LogMessage("Parameter SecurePaperCertificateThumbprint is null."), LogCategories);
                throw new Exception("Parameter SecurePaperCertificateThumbprint is null.");
            }

            _securePaperConfig.SecurePaperWSUrl = _webApiClient.GetParameterSecurePaperServiceUrlAsync().Result;
            _logger.WriteDebug(new LogMessage($"Parameter SecurePaperServiceUrl -> {_securePaperConfig.SecurePaperWSUrl}"), LogCategories);
            if (string.IsNullOrEmpty(_securePaperConfig.SecurePaperWSUrl))
            {
                _logger.WriteWarning(new LogMessage("Parameter SecurePaperServiceUrl is null."), LogCategories);
                throw new Exception("Parameter SecurePaperServiceUrl is null.");
            }

            _securePaperService = new SecurePaperService(logger, _securePaperConfig.SecurePaperWSUrl, _securePaperConfig.SecurePaperServiceId, _securePaperConfig.CertificateThumbprint);
        }
        #endregion

        #region [ Methods ]
        public async Task ExecuteAsync(ICommandSecurePaperRequest command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command, " is arrived")), LogCategories);
            byte[] content = null;
            string convertedDocumentName = string.Empty;
            string archiveKey = string.Empty;
            string signatureValue = string.Empty;
            StampaConforme.StampaConforme.stDoc toSecure;
            StampaConforme.StampaConforme.stDoc response;
            byte[] secureDocumentContent;
            Protocol protocol;
            BiblosDS.BiblosDS.Document infoDocument;
            BiblosDS.BiblosDS.AttributeValue signature;
            bool appendSignature;
            DocumentUnit documentUnit = null;
            try
            {
                _logger.WriteInfo(new LogMessage(string.Concat(" Evaluating: ", command.ContentType.UniqueId)), LogCategories);

                if (command.ContentType.ContentTypeValue.DocumentUnit == null)
                {
                    _logger.WriteWarning(new LogMessage("Document Unit is not configured"), LogCategories);
                    throw new Exception("Document Unit is not configured");
                }

                if (command.ContentType.ContentTypeValue.DocumentUnit.ReferenceModel == null)
                {
                    _logger.WriteWarning(new LogMessage("Reference Model is not configured"), LogCategories);
                    throw new Exception("Reference Model is not configured");
                }

                _logger.WriteDebug(new LogMessage(string.Concat(" Document Unit uniqueid: ", command.ContentType.ContentTypeValue.DocumentUnit.ReferenceId)), LogCategories);

                try
                {
                    documentUnit = JsonConvert.DeserializeObject<DocumentUnit>(command.ContentType.ContentTypeValue.DocumentUnit.ReferenceModel);
                }
                catch (Exception)
                {
                    _logger.WriteWarning(new LogMessage("ReferenceModel is not correctly deserialized"), LogCategories);
                    throw new Exception("ReferenceModel is not correctly deserialized");
                }

                appendSignature = await _webApiClient.GetSecureDocumentSignatureEnabledAsync();
                foreach (WorkflowReferenceBiblosModel document in command.ContentType.ContentTypeValue.Documents)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("Creating secure document for id ", document.ArchiveDocumentId, "...")), LogCategories);
                    content = _biblosClient.Document.GetDocumentContentById(document.ArchiveDocumentId.Value).Blob;
                    toSecure = new StampaConforme.StampaConforme.stDoc()
                    {
                        Blob = Convert.ToBase64String(content),
                        FileExtension = Path.GetExtension(document.DocumentName)
                    };

                    if (appendSignature)
                    {
                        _logger.WriteDebug(new LogMessage("Append signature to document"), LogCategories);
                        infoDocument = _biblosClient.Document.GetDocumentInfoById(document.ArchiveDocumentId.Value);
                        signature = infoDocument.AttributeValues.SingleOrDefault(f => f.Attribute.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase));
                        if (signature != null && signature.Value != null)
                        {
                            signatureValue = StampaConforme.StampaConformeClient.GetLabel(signature.Value.ToString());
                        }
                    }

                    _logger.WriteInfo(new LogMessage(string.Concat("Convert document ", document.DocumentName, "(", document.ArchiveDocumentId, ") to PDF")), LogCategories);
                    convertedDocumentName = string.Concat(document.DocumentName, ".pdf");
                    response = _stampaConformeClient.StampaConforme.ToRasterFormatEx(toSecure, Path.GetExtension(document.DocumentName), signatureValue);
                    _logger.WriteInfo(new LogMessage(string.Concat("Call SecurePaper WS for pdf document ", convertedDocumentName, "(rif -> ", document.ArchiveDocumentId, ")")), LogCategories);
                    secureDocumentContent = _securePaperService.Create(convertedDocumentName, Convert.FromBase64String(response.Blob), out archiveKey);
                    _logger.WriteDebug(new LogMessage(string.Concat("Document ", document.ArchiveDocumentId, " secure correctly")), LogCategories);
                    UpdateDocumentAsync(document.ArchiveDocumentId.Value, convertedDocumentName, secureDocumentContent, archiveKey);
                    protocol = await _webApiClient.GetProtocolAsync(documentUnit.UniqueId);
                    await CompleteSecureDocumentLogAsync(protocol, document.ArchiveDocumentId.Value, document.DocumentName);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public Guid UpdateDocumentAsync(Guid documentId, string documentName, byte[] content, string archiveKey)
        {
            try
            {
                BiblosDS.BiblosDS.Document document = new BiblosDS.BiblosDS.Document();
                BiblosDS.BiblosDS.Document checkedout = _biblosClient.Document.DocumentCheckOut(documentId, true, _biblos_checkout_userId);

                ICollection<BiblosDS.BiblosDS.Attribute> attributes = _biblosClient.Document.GetAttributesDefinition(checkedout.Archive.Name);

                _logger.WriteDebug(new LogMessage(string.Concat("Document ", documentId, " set filename attribute -> ", documentName)), LogCategories);
                BiblosDS.BiblosDS.AttributeValue fileName = checkedout.AttributeValues.SingleOrDefault(f => f.Attribute.Name.Equals(_biblos_attribute_filename, StringComparison.InvariantCultureIgnoreCase));
                fileName.Value = documentName;

                BiblosDS.BiblosDS.Attribute archiveKeyAttribute = attributes.SingleOrDefault(f => f.Name.Equals(_biblos_attribute_securepaper_archivekey, StringComparison.InvariantCultureIgnoreCase));
                if (archiveKeyAttribute != null)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("Document ", documentId, " set securepaper archivekey attribute -> ", archiveKey)), LogCategories);
                    BiblosDS.BiblosDS.AttributeValue securePaperArchiveKey = checkedout.AttributeValues.SingleOrDefault(f => f.Attribute.Name.Equals(_biblos_attribute_securepaper_archivekey, StringComparison.InvariantCultureIgnoreCase));
                    if (securePaperArchiveKey == null)
                    {
                        securePaperArchiveKey = new BiblosDS.BiblosDS.AttributeValue()
                        {
                            Attribute = archiveKeyAttribute
                        };
                        checkedout.AttributeValues.Add(securePaperArchiveKey);
                    }
                    securePaperArchiveKey.Value = archiveKey;
                }

                checkedout.Content = new BiblosDS.BiblosDS.Content()
                {
                    Blob = content,
                };

                Guid savedId = _biblosClient.Document.DocumentCheckIn(checkedout, _biblos_checkout_userId);
                _logger.WriteInfo(new LogMessage(string.Concat("Document ", documentId, " updated correctly")), LogCategories);
                _biblosClient.Document.ConfirmDocumentAsync(checkedout.IdDocument);
                return savedId;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                _biblosClient.Document.UndoCheckOutDocumentAsync(documentId, _biblos_checkout_userId);
                throw new Exception(string.Concat("Unexpected exception was thrown while invoking operation: ", ex.Message), ex);
            }
        }

        private async Task CompleteSecureDocumentLogAsync(Protocol protocol, Guid documentId, string documentName)
        {
            ProtocolLog deleteLog = _webApiClient.GetProtocolLogAsync(string.Format(ODATA_FILTER_LOGTYPE, protocol.UniqueId.ToString(), documentId)).Result.FirstOrDefault();
            if (deleteLog != null)
            {
                await _webApiClient.DeleteEntityAsync(deleteLog, DeleteActionType.SecureDocumentLogDelete.ToString());
                _logger.WriteInfo(new LogMessage(string.Concat("CompleteSecureDocumentLogAsync -> Deleted Protocol log with uniqueid ", deleteLog.UniqueId)), LogCategories);
            }

            ProtocolLog log = new ProtocolLog()
            {
                Entity = protocol,
                LogDate = DateTime.UtcNow,
                RegistrationUser = _userName,
                Program = PROGRAM_NAME,
                LogType = SECUREDOCUMENT_COMPLETED_LOG_TYPE,
                LogDescription = string.Format(SECUREDOCUMENT_COMPLETED_LOG_DESCRIPTION, documentName, documentId)
            };
            await _webApiClient.PostEntityAsync(log, InsertActionType.SecureDocumentLogInsert.ToString());
            _logger.WriteInfo(new LogMessage(string.Concat("CompleteSecureDocumentLogAsync -> Protocollog with uniqueid ", log.UniqueId, " created with success")), LogCategories);
        }
        #endregion
    }
}
