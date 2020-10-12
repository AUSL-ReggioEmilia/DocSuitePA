using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.TECHNOSKY.SAP.Invoice.Configurations;
using VecompSoftware.BPM.Integrations.Modules.TECHNOSKY.SAP.Invoice.Models;
using VecompSoftware.BPM.Integrations.Modules.TECHNOSKY.SAP.Invoice.PayableInvoiceService;
using VecompSoftware.BPM.Integrations.Modules.TECHNOSKY.SAP.Invoice.ReceivableInvoiceService;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.SignServices;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.Helpers.EInvoice.UDS.Models;
using VecompSoftware.Helpers.PEC.PA;
using VecompSoftware.Helpers.PEC.PA.Models;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;
using BiblosDocument = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Document;
using Content = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Content;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using UDSEInvoiceHelper = VecompSoftware.Helpers.EInvoice.UDS.EInvoice1_2.EInvoiceHelper;

namespace VecompSoftware.BPM.Integrations.Modules.TECHNOSKY.SAP.Invoice
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        private static readonly HostIdentify _hostIdentify = new HostIdentify(Environment.MachineName, ModuleConfigurationHelper.MODULE_NAME);
        private static readonly byte[] _patternSearch = new byte[] { 60, 63 };
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly ISignServiceClient _signServiceClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IdentityContext _identityContext = null;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private readonly IEnumerable<KeyValuePair<XMLModelKind, InvoiceConfiguration>> _receivableWorkflowConfigurations;
        private readonly IEnumerable<KeyValuePair<XMLModelKind, InvoiceConfiguration>> _payableWorkflowConfigurations;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
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
        #endregion

        #region [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient, ISignServiceClient signServiceClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _receivableWorkflowConfigurations = _moduleConfiguration.ReceivableWorkflowConfigurations.SelectMany(f => f.Value);
                _payableWorkflowConfigurations = _moduleConfiguration.PayableWorkflowConfigurations.SelectMany(f => f.Value);
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _documentClient = documentClient;
                _signServiceClient = signServiceClient;
                string username = "anonymous";
                _needInitializeModule = true;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SAP.Invoice -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }

        }
        #endregion

        #region [ Methods ]
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }

            try
            {
                InitializeModule();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SAP.Invoice -> Critical error in initialize module"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> TECHNOSKY.SAP.Invoice"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteUDSBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowReceivableInvoiceUDSBuildCompleteSubscription, WorkflowReceivableInvoiceUDSBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventReceivedReceiptPECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartUpdateReceiptMetadataInvoiceSubscription, WorkflowPECMailReceiptCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreatePECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartUpdateMetadataInvoiceSubscription, WorkflowCreatePECMailCallback));

                _needInitializeModule = false;
            }
        }

        internal void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }

        #region [ WorkflowReceivableInvoiceUDSBuildCompleteCallback ]
        private async Task WorkflowReceivableInvoiceUDSBuildCompleteCallback(IEventCompleteUDSBuild evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowReceivableInvoiceUDSBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);

                UDSDocumentUnit protocolDocumentUnit;
                Protocol protocol;
                PECMail pecMail;
                DateTime? comunicationDate = null;
                KeyValuePair<string, InvoiceFilePersistance> persistanceInvoiceConfiguration;
                KeyValuePair<XMLModelKind, InvoiceConfiguration> invoiceConfiguration;
                BiblosDocument invoiceDocument;
                List<BiblosDocument> invoiceDocuments;
                string controllerName;
                string mailBoxRecipient;
                Guid invoiceDocumentChain;
                Dictionary<int, Guid> documents;
                Dictionary<string, object> uds_metadatas;
                UDSBuildModel udsBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for WorkflowActivityId {udsBuildModel.IdWorkflowActivity}"), LogCategories);
                protocolDocumentUnit = await RetryingPolicyActionAsync(async () =>
                {
                    UDSDocumentUnit result = (await _webAPIClient.GetUDSDocumentUnits(udsBuildModel.UniqueId, false, true)).FirstOrDefault(f => f.Relation.Environment == (int)DSWEnvironmentType.Protocol);
                    if (result == null)
                    {
                        throw new ArgumentNullException($"Received a wrong message invalid UDSId {udsBuildModel.UniqueId}: UDS not found");
                    }
                    return result;
                });
                if (!_receivableWorkflowConfigurations.Any(f => f.Value.UDSRepositoryName == protocolDocumentUnit.Repository.Name))
                {
                    throw new ArgumentNullException($"Received a wrong message UDSId {protocolDocumentUnit.IdUDS} {protocolDocumentUnit.Repository.Name}: Not related to valid invoice configuration environment");
                }
                invoiceConfiguration = _receivableWorkflowConfigurations.SingleOrDefault(f => f.Value.UDSRepositoryName == protocolDocumentUnit.Repository.Name);
                _logger.WriteDebug(new LogMessage($"Found protocol {protocolDocumentUnit.Relation.UniqueId} associated to UDS {protocolDocumentUnit.IdUDS}"), LogCategories);

                protocol = (await _webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {protocolDocumentUnit.Relation.UniqueId}&$expand=AdvancedProtocol")).SingleOrDefault();
                if (protocol == null)
                {
                    throw new ArgumentNullException($"protocol not found for identification {protocolDocumentUnit.Relation.Year}/{protocolDocumentUnit.Relation.Number}");
                }
                _logger.WriteDebug(new LogMessage($"Found protocol {protocol.UniqueId}/{protocol.Year}/{protocol.Number.ToString("000000")} associated to UDS {protocolDocumentUnit.Relation.UniqueId}"), LogCategories);
                mailBoxRecipient = string.Empty;
                pecMail = (await _webAPIClient.GetPECMailFromProtocol(protocol.UniqueId)).FirstOrDefault();
                if (pecMail != null)
                {
                    _logger.WriteDebug(new LogMessage($"Found PECMail {pecMail.UniqueId} associated to protocol {protocolDocumentUnit.Relation.UniqueId}"), LogCategories);
                    mailBoxRecipient = pecMail.PECMailBox.MailBoxRecipient;
                    comunicationDate = pecMail.MailDate;
                }
                if (invoiceConfiguration.Value.InvoicePersistanceConfigurations.Any(f => f.Key.Equals(mailBoxRecipient, StringComparison.InvariantCultureIgnoreCase)))
                {
                    persistanceInvoiceConfiguration = invoiceConfiguration.Value.InvoicePersistanceConfigurations.Single(f => f.Key.Equals(mailBoxRecipient, StringComparison.InvariantCultureIgnoreCase));
                    _logger.WriteDebug(new LogMessage($"Specific configuration found by MailBoxRecipient {mailBoxRecipient}"), LogCategories);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage($"Specific configuration not found for {mailBoxRecipient}. Using default configuration"), LogCategories);
                    persistanceInvoiceConfiguration = invoiceConfiguration.Value.InvoicePersistanceConfigurations.Single(f => string.IsNullOrEmpty(f.Key));

                }
                if (persistanceInvoiceConfiguration.Value == null || string.IsNullOrEmpty(persistanceInvoiceConfiguration.Value.UDSRepositoryName))
                {
                    throw new ArgumentNullException($"Persistance invoice configuration not found for {mailBoxRecipient} MailBoxRecipient");
                }

                controllerName = Utils.GetWebAPIControllerName(persistanceInvoiceConfiguration.Value.UDSRepositoryName);
                documents = new Dictionary<int, Guid>();
                _logger.WriteDebug(new LogMessage($"Get UDS {protocolDocumentUnit.IdUDS} invoice metadatas"), LogCategories);
                uds_metadatas = await _webAPIClient.GetUDS(controllerName, protocolDocumentUnit.IdUDS, documents);
                _logger.WriteDebug(new LogMessage($"Found {uds_metadatas != null} invoice metadatas {documents.ContainsKey(1)}"), LogCategories);
                if (!comunicationDate.HasValue && uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi))
                {
                    comunicationDate = (DateTime?)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi];
                    _logger.WriteDebug(new LogMessage($"Read {UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi} from metadatas {comunicationDate}"), LogCategories);
                }

                invoiceDocumentChain = documents[1];
                _logger.WriteDebug(new LogMessage($"Get InvoiceDocument {invoiceDocumentChain}"), LogCategories);
                invoiceDocuments = await _documentClient.GetDocumentChildrenAsync(invoiceDocumentChain);
                _logger.WriteDebug(new LogMessage($"Found {invoiceDocuments?.Count} documents. Main document has identificaiton {invoiceDocuments?.Single(f => f.IsLatestVersion)?.IdDocument}"), LogCategories);
                invoiceDocument = invoiceDocuments.Single(f => f.IsLatestVersion);
                InsertSAPInvoice(protocolDocumentUnit, protocol, comunicationDate, invoiceDocument.Name, persistanceInvoiceConfiguration, udsBuildModel, uds_metadatas);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoiceUDSBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private void InsertSAPInvoice(UDSDocumentUnit protocolDocumentUnit, Protocol protocol, DateTime? comunicationDate, string invoiceFilename,
            KeyValuePair<string, InvoiceFilePersistance> persistanceInvoiceConfiguration, UDSBuildModel udsBuildModel, Dictionary<string, object> uds_metadatas)
        {
            using (ZFI_FATTEL_UPDATE_PASS insertInvoiceService = new ZFI_FATTEL_UPDATE_PASS())
            {
                ZfiFattelUpdatePass zfiFattelUpdatePass = new ZfiFattelUpdatePass()
                {
                    IvDataPec = comunicationDate.Value.ToString("s"),
                    IvEsercizio = protocolDocumentUnit.Relation.Year.ToString(),
                    IvNumero = protocolDocumentUnit.Relation.Number.ToString(),
                    IvIdSdi = uds_metadatas[UDSEInvoiceHelper.UDSMetadata_IdentificativoSdi] as string,
                    IvIdsky = protocolDocumentUnit.IdUDS.ToString(),
                    IvNomefile = invoiceFilename.Replace(".p7m", string.Empty).Replace(".P7M", string.Empty),
                    IvPath = persistanceInvoiceConfiguration.Value.FolderReceivedInvoice,
                    IvDenominazione = uds_metadatas[UDSEInvoiceHelper.UDSMetadata_Denominazione] as string,
                    IvUrl = string.Format(_moduleConfiguration.ExternalViewerURIFormat, protocolDocumentUnit.Relation.Year, protocolDocumentUnit.Relation.Number),
                    IvFdp = string.Empty,
                    IvFpc = string.Empty,
                    IvFlec = string.Empty
                };
                if (persistanceInvoiceConfiguration.Value.FolderReceivedInvoice.EndsWith("FDP"))
                {
                    zfiFattelUpdatePass.IvFdp = "x";
                }
                if (persistanceInvoiceConfiguration.Value.FolderReceivedInvoice.EndsWith("FPC"))
                {
                    zfiFattelUpdatePass.IvFpc = "x";
                }
                if (persistanceInvoiceConfiguration.Value.FolderReceivedInvoice.EndsWith("FLEC"))
                {
                    zfiFattelUpdatePass.IvFlec = "x";
                }
                _logger.WriteDebug(new LogMessage($"Sending datas : {JsonConvert.SerializeObject(zfiFattelUpdatePass)}"), LogCategories);
                ZfiFattelUpdatePassResponse zfiFattelUpdatePassResponse = insertInvoiceService.ZfiFattelUpdatePass(zfiFattelUpdatePass);
                _logger.WriteInfo(new LogMessage($"SAP WebService response {zfiFattelUpdatePassResponse.EvResult}"), LogCategories);
                if (zfiFattelUpdatePassResponse.EvResult != "XML correttamente registrato in tabella")
                {
                    throw new Exception($"Error occoured in SAP WebService invocation : {zfiFattelUpdatePassResponse.EvResult}");
                }
            }
            _logger.WriteInfo(new LogMessage($"Invoice {udsBuildModel.UniqueId} has been successfully sended to SAP WebService"), LogCategories);
        }

        private async Task<T> RetryingPolicyActionAsync<T>(Func<Task<T>> func, int step = 1, int retry_tentative = 10)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{retry_tentative} in progress..."), LogCategories);
            if (step >= retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.TECHNOSKY.SAP.Invoice.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("ReceivableInvoice retry policy expired maximum tentatives");
            }
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaiting).Wait();
                return await RetryingPolicyActionAsync(func, ++step);
            }
        }

        #endregion

        #region [ WorkflowPECMailReceiptCallback ]

        private async Task WorkflowPECMailReceiptCallback(IEventReceivedReceiptPECMail evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowReceiptPECMailCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying PECMailReceipt"), LogCategories);
                Guid protocolUniqueId = Guid.Empty;
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PROTOCOL_UNIQUE_ID) ||
                    !Guid.TryParse(evt.CustomProperties[CustomPropertyName.PROTOCOL_UNIQUE_ID].ToString(), out protocolUniqueId))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_UNIQUE_ID} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PROTOCOL_YEAR))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_YEAR} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PROTOCOL_NUMBER))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_NUMBER} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PECMAIL_RECEIPT_TYPE))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PECMAIL_RECEIPT_TYPE} property in event properties");
                }
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.PECMAIL_RECEIPT_DATE))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.PECMAIL_RECEIPT_DATE} property in event properties");
                }
                string receiptType = evt.CustomProperties[CustomPropertyName.PECMAIL_RECEIPT_TYPE].ToString();
                DateTime receiptDate = (DateTime)evt.CustomProperties[CustomPropertyName.PECMAIL_RECEIPT_DATE];
                UDSDocumentUnit udsDocumentUnit = (await _webAPIClient.GetUDSDocumentUnitFromDocumentUnitId(protocolUniqueId)).SingleOrDefault();

                if (udsDocumentUnit == null || !_payableWorkflowConfigurations.Any(f => f.Value.UDSRepositoryName == udsDocumentUnit.Repository.Name))
                {
                    throw new ArgumentNullException($"Wrong message received ProtocolUniqueId {protocolUniqueId} {evt.CustomProperties[CustomPropertyName.PROTOCOL_YEAR]}/{evt.CustomProperties[CustomPropertyName.PROTOCOL_NUMBER]} not related to valid Invoice Environment");
                }

                string controllerName = Utils.GetWebAPIControllerName(udsDocumentUnit.Repository.Name);
                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, udsDocumentUnit.IdUDS, documents);
                if (uds_metadatas == null)
                {
                    throw new ArgumentNullException($"Not found UDS invoice  {udsDocumentUnit.IdUDS}/{controllerName}");
                }
                _logger.WriteInfo(new LogMessage($"Updating PECMailReceipt metadatas for UDSId {udsDocumentUnit.IdUDS}"), LogCategories);
                uds_metadatas = UDSEInvoiceHelper.MappingPayablePECMailReceiptMetadatas(receiptDate, receiptType, uds_metadatas);

                ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(udsDocumentUnit.IdUDS);
                ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(udsDocumentUnit.IdUDS);
                ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(udsDocumentUnit.IdUDS);
                ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(udsDocumentUnit.IdUDS);
                ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(udsDocumentUnit.IdUDS, false, false);

                UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsDocumentUnit.Repository, udsDocumentUnit.IdUDS, uds_metadatas, documents,
                    udsRoles, udsContacts, udsMessages, udsPECMails, udsDocumentUnits, null, evt.Identity.User);
                CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(evt.Name, evt.TenantId, evt.TenantAOOId, evt.Identity, udsBuildModel);
                await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
                _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoicePECMailBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowCreatePECMailCallback ]

        private async Task WorkflowCreatePECMailCallback(IEventCreatePECMail evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowCreatePECMailCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying CreatePECMail"), LogCategories);

                PECMail pecMail = evt.ContentType.ContentTypeValue;
                if (pecMail.DocumentUnit == null)
                {
                    throw new ArgumentNullException($"Undefined protocol in PECMail {pecMail.EntityId}");
                }
                IEnumerable<PECMailAttachment> attachments = pecMail.Attachments
                    .Where(f => f.IDDocument.HasValue && f.IDDocument.Value != Guid.Empty && new FileInfo(f.AttachmentName).Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase));
                if (!attachments.Any())
                {
                    throw new ArgumentNullException($"Undefined valid SDI attachment to evaluate in PECMail {pecMail.EntityId}");
                }

                Protocol protocol = (await _webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {pecMail.DocumentUnit.UniqueId}&$expand=AdvancedProtocol")).SingleOrDefault();
                if (protocol == null)
                {
                    throw new ArgumentNullException($"Protocol not found for identification {pecMail.Year}/{pecMail.Number}");
                }
                UDSDocumentUnit udsDocumentUnit = (await _webAPIClient.GetUDSDocumentUnitFromDocumentUnitId(protocol.UniqueId)).SingleOrDefault();

                if (udsDocumentUnit == null || !_payableWorkflowConfigurations.Any(f => f.Value.UDSRepositoryName == udsDocumentUnit.Repository.Name))
                {
                    throw new ArgumentNullException($"Wrong message received ProtocolUniqueId {protocol.UniqueId} {protocol.Year}/{protocol.Number} not related to valid Invoice Environment");
                }
                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                string controllerName = Utils.GetWebAPIControllerName(udsDocumentUnit.Repository.Name);
                IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, udsDocumentUnit.IdUDS, documents);
                if (uds_metadatas == null)
                {
                    throw new ArgumentNullException($"Not found UDS invoice  {udsDocumentUnit.IdUDS}/{controllerName}");
                }
                _logger.WriteInfo(new LogMessage($"Updating Invoice {udsDocumentUnit.IdUDS} related to Protocol {protocol.UniqueId} {protocol.Year}/{protocol.Number} looking Status from PEC {pecMail.EntityId}"), LogCategories);
                SDIMessage sdiMessage = new SDIMessage();
                Content content;
                object sdiMessageObj = null;
                foreach (PECMailAttachment item in attachments)
                {
                    content = await _documentClient.GetDocumentContentByIdAsync(item.IDDocument.Value);
                    using (MemoryStream stream = new MemoryStream(content.Blob))
                    {
                        sdiMessage = PAMessageHelper.GetSIDMessage(stream, out sdiMessageObj);
                        if (sdiMessage.Status != MessageStatus.None)
                        {
                            break;
                        }
                    }
                }
                if (sdiMessageObj == null || sdiMessage.Status == MessageStatus.None)
                {
                    throw new ArgumentNullException($"Undefined valid SDI receipttype {sdiMessageObj}/{sdiMessage.Status} in evaluating PECMail {pecMail.EntityId}");
                }
                uds_metadatas = UDSEInvoiceHelper.MappingPayablePECMailSIDMetadatas(sdiMessage, uds_metadatas);
                ReceivableInvoiceMetadata receivableInvoiceMetadata = null;
                using (ZFI_FATTEL_UPDATE_ATT updateInvoiceService = new ZFI_FATTEL_UPDATE_ATT())
                {
                    ZfiFattelUpdateAtt zfiFattelUpdatePass = new ZfiFattelUpdateAtt()
                    {
                        IvAnnoFisc = ((DateTime)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataFattura]).Year.ToString(),
                        IvNumDoc = uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura].ToString(),
                        IvLink = string.Format(_moduleConfiguration.ExternalViewerURIFormat, protocol.Year, protocol.Number),
                        IvDescr = sdiMessage.GetDescriptionMessageStatus(),
                        IvIdSdi = sdiMessage.SDIIdentification,
                        IvNumProt = protocol.Number.ToString(),
                        IvEseProt = protocol.Year.ToString(),
                        IvNotifica = sdiMessage.Status == MessageStatus.PAInvoiceRefused ? "0" :
                        sdiMessage.Status == MessageStatus.PAInvoiceAccepted ? "2" :
                        sdiMessage.Status == MessageStatus.PAInvoiceReceipt ? "1" :
                        sdiMessage.Status == MessageStatus.PAInvoiceRefused || sdiMessage.Status == MessageStatus.PAInvoiceSdiRefused || sdiMessage.Status == MessageStatus.PAInvoiceFailedDelivery ? "-1" : string.Empty,
                        IvDataAcc = string.Empty,
                        IvDataRif = string.Empty
                    };
                    if (sdiMessage.Status == MessageStatus.PAInvoiceRefused)
                    {
                        zfiFattelUpdatePass.IvDataRif = (sdiMessage.SDIDate.HasValue ? sdiMessage.SDIDate.Value : pecMail.MailDate.Value).ToString("s");
                    }
                    if (sdiMessage.Status == MessageStatus.PAInvoiceAccepted)
                    {
                        zfiFattelUpdatePass.IvDataAcc = (sdiMessage.SDIDate.HasValue ? sdiMessage.SDIDate.Value : pecMail.MailDate.Value).ToString("s");
                    }
                    ZfiFattelUpdateAttResponse zfiFattelUpdateAttResponse = updateInvoiceService.ZfiFattelUpdateAtt(zfiFattelUpdatePass);
                    _logger.WriteInfo(new LogMessage($"PECMailReceipt {evt.ContentType.ContentTypeValue.EntityId} has been successfully sended to SAP WebService"), LogCategories);
                    _logger.WriteInfo(new LogMessage($"SAP WebService response {JsonConvert.SerializeObject(zfiFattelUpdateAttResponse)}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"metadata YearVAT {zfiFattelUpdateAttResponse.EvAnnoIva}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"metadata DateVAT {zfiFattelUpdateAttResponse.EvDataIva}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"metadata ProtocolNumberVAT {zfiFattelUpdateAttResponse.EvNumIva}"), LogCategories);
                    _logger.WriteDebug(new LogMessage($"metadata SectionalVAT {zfiFattelUpdateAttResponse.EvRegistroIva}"), LogCategories);

                    try
                    {
                        receivableInvoiceMetadata = new ReceivableInvoiceMetadata
                        {
                            DateVAT = DateTime.Parse(zfiFattelUpdateAttResponse.EvDataIva),
                            ProtocolNumberVAT = Convert.ToInt32(zfiFattelUpdateAttResponse.EvNumIva),
                            SectionalVAT = zfiFattelUpdateAttResponse.EvRegistroIva,
                            YearVAT = Convert.ToInt32(zfiFattelUpdateAttResponse.EvAnnoIva)
                        };
                        uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceFiscalMetadatas(receivableInvoiceMetadata, uds_metadatas);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage($"Skip SAP invalid metadata {JsonConvert.SerializeObject(zfiFattelUpdateAttResponse)}"), ex, LogCategories);
                    }
                }

                ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(udsDocumentUnit.IdUDS);
                ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(udsDocumentUnit.IdUDS);
                ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(udsDocumentUnit.IdUDS);
                ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(udsDocumentUnit.IdUDS);
                ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(udsDocumentUnit.IdUDS, false, false);
                _logger.WriteInfo(new LogMessage($"Updating PEC PA metadatas for UDSId {udsDocumentUnit.IdUDS}"), LogCategories);

                UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsDocumentUnit.Repository, udsDocumentUnit.IdUDS, uds_metadatas, documents,
                    udsRoles, udsContacts, udsMessages, udsPECMails, udsDocumentUnits, null, evt.Identity.User);
                CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(evt.Name, evt.TenantId, evt.TenantAOOId, evt.Identity, udsBuildModel);
                await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
                _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowCreatePECMailCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #endregion

    }
}
