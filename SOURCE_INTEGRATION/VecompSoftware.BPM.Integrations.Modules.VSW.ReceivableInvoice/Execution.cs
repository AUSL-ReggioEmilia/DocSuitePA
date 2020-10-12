using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Telerik.Windows.Zip;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Exceptions;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Helpers;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.BPM.Integrations.Services.WebAPI.Models;
using VecompSoftware.Commons;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Commands.Models.Messages;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Core.Command.CQRS.Events.Models.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.EInvoice.EInvoice1_2;
using VecompSoftware.Helpers.EInvoice.Models;
using VecompSoftware.Helpers.EInvoice.UDS.Models;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Helpers.XML;
using VecompSoftware.Helpers.XML.Converters.Factory;
using VecompSoftware.Helpers.XML.Converters.Models.InvoicePA.PA1_2;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.Services.Command.CQRS.Events.Models.Protocols;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;
using BiblosDocument = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Document;
using ComunicationType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.ComunicationType;
using Content = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Content;
using DSWEnvironmentType = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType;
using InvoiceFSM1_0 = VecompSoftware.Helpers.XML.Converters.Models.InvoicePA.VFSM1_0;
using ProtocolRoleNoteType = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleNoteType;
using ProtocolRoleStatus = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolRoleStatus;
using ProtocolTypology = VecompSoftware.DocSuiteWeb.Model.Entities.Protocols.ProtocolTypology;
using UDSEInvoiceHelper = VecompSoftware.Helpers.EInvoice.UDS.EInvoice1_2.EInvoiceHelper;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        private static IEnumerable<LogCategory> _logCategories;
        private static readonly byte[] _patternSearch = new byte[] { 60, 63 };
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IDocumentClient _documentClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private Location _workflowLocation;
        private readonly string _username;

        private const string UDSMetadata_Workflow_ApprovazioneTecnica = "ApprovazioneTecnica";
        private const string UDSMetadata_Workflow_ApprovazioneAlPagamento = "ApprovazioneAlPagamento";

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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IDocumentClient documentClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _documentClient = documentClient;
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _needInitializeModule = true;
                _username = string.Empty;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    _username = WindowsIdentity.GetCurrent().Name;
                }

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.ReceivableInvoice -> Critical error in costruction module"), ex, LogCategories);
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

                string workingPath = string.Empty;
                InvoiceMetadata invoiceMetadata;
                IEnumerable<FileInfo> resultFiles;
                if (_moduleConfiguration.PersistInvoiceFilesystemEnabled)
                {
                    foreach (InvoiceFilePersistance metadataInvoiceFolder in _moduleConfiguration.WorkflowConfigurations.SelectMany(f => f.Value.InvoiceTypes)
                        .Where(f => f.Value.InvoicePersistanceConfigurations.Any()).SelectMany(f => f.Value.InvoicePersistanceConfigurations.Values).Where(f => !string.IsNullOrEmpty(f.FolderMetadataInvoiceLooking)))
                    {
                        resultFiles = new DirectoryInfo(metadataInvoiceFolder.FolderMetadataInvoiceLooking).EnumerateFiles("*.json", SearchOption.AllDirectories);
                        _logger.WriteInfo(new LogMessage($"Looking metadata-> found {resultFiles.Count()} json file in path {metadataInvoiceFolder.FolderMetadataInvoiceLooking}"), LogCategories);
                        foreach (FileInfo fileInfo in resultFiles)
                        {
                            workingPath = string.Empty;
                            try
                            {
                                _logger.WriteInfo(new LogMessage($"Evaluating {fileInfo.FullName} ...."), LogCategories);
                                workingPath = Path.Combine(metadataInvoiceFolder.FolderMetadataInvoiceWorking, fileInfo.Name);
                                File.Move(fileInfo.FullName, workingPath);
                                _logger.WriteInfo(new LogMessage($"{fileInfo.FullName} has been moved to {workingPath}"), LogCategories);
                                invoiceMetadata = new InvoiceMetadata(workingPath, metadataInvoiceFolder.UDSRepositoryName, JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(workingPath)));
                                UpdateInvoiceMetadataAsync(invoiceMetadata).Wait();
                                if (string.IsNullOrEmpty(metadataInvoiceFolder.FolderMetadataInvoiceBackup))
                                {
                                    File.Delete(workingPath);
                                }
                                else
                                {
                                    File.Move(workingPath, Path.Combine(metadataInvoiceFolder.FolderMetadataInvoiceBackup, fileInfo.Name));
                                    _logger.WriteInfo(new LogMessage($"{workingPath} has been moved to {Path.Combine(metadataInvoiceFolder.FolderMetadataInvoiceBackup, fileInfo.Name)}"), LogCategories);
                                }
                                if (!fileInfo.Directory.FullName.Equals(metadataInvoiceFolder.FolderMetadataInvoiceLooking, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    fileInfo.Directory.Delete();
                                    _logger.WriteInfo(new LogMessage($"{fileInfo.Directory.FullName} has been deleted"), LogCategories);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.WriteError(new LogMessage($"Error during {fileInfo.FullName} evaluation. This file was skipped"), ex, LogCategories);
                                _logger.WriteError(new LogMessage($"ReceivableInvoice {fileInfo.FullName} metadata file has been skipped to invalid reason: {ex.Message}"), LogCategory.NotifyToEmailCategory);
                                try
                                {
                                    File.Move(fileInfo.FullName, Path.Combine(metadataInvoiceFolder.FolderMetadataInvoiceError, fileInfo.Name));
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        File.Move(workingPath, Path.Combine(metadataInvoiceFolder.FolderMetadataInvoiceError, fileInfo.Name));
                                    }
                                    catch (Exception)
                                    {
                                        _logger.WriteWarning(new LogMessage($"Error occouring {fileInfo.FullName} in moving file to rejected folder"), ex, LogCategories);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ReceivableInvoiceWorkflow -> critical error"), ex, LogCategories);
                throw;
            }

        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.ReceivableInvoice"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreatePECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartReceivableInvoiceSubscription, WorkflowStartReceivableInvoiceCallback));
                if (_moduleConfiguration.InvoiceAdEEnabled)
                {
                    _subscriptions.Add(_serviceBusClient.StartListening<EventDematerialisationRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                        _moduleConfiguration.WorkflowStartReceivableInvoiceAdESubscription, WorkflowStartReceivableInvoiceAdECallback));
                }
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteProtocolBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowReceivableInvoiceProtocolBuildCompleteSubscription, WorkflowReceivableInvoiceProtocolBuildCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteProtocolDelete>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowInvoiceDeleteProtocolDeleteCompleteSubscription, WorkflowInvoiceDeleteProtocolDeleteCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteUDSDelete>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                    _moduleConfiguration.WorkflowInvoiceDeleteUDSDeleteCompleteSubscription, WorkflowInvoiceDeleteUDSDeleteCompleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<EventDematerialisationRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartInvoiceDeleteSubscription, WorkflowStartInvoiceDeleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<EventDematerialisationRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartInvoiceMoveSubscription, WorkflowStartInvoiceMoveCallback));
                if (_moduleConfiguration.PersistInvoiceFilesystemEnabled)
                {
                    _subscriptions.Add(_serviceBusClient.StartListening<IEventCompleteUDSBuild>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicBuilderEvent,
                        _moduleConfiguration.WorkflowReceivableInvoiceUDSBuildCompleteSubscription, WorkflowReceivableInvoiceUDSBuildCompleteCallback));
                }

                int? workflowLocationId = _webAPIClient.GetParameterWorkflowLocationIdAsync().Result;
                if (!workflowLocationId.HasValue)
                {
                    throw new ArgumentNullException("Parameter WorkflowLocationId is not defined");
                }
                _workflowLocation = _webAPIClient.GetLocationAsync(workflowLocationId.Value).Result.Single();

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

        #region [ WorkflowStartReceivableInvoiceCallback ]

        private async Task WorkflowStartReceivableInvoiceCallback(IEventCreatePECMail evt, IDictionary<string, object> properties)
        {
            Content pecmMailBibloContent;
            string xmlContent = string.Empty;
            InvoiceFileModel invoiceUDSFileModel = new InvoiceFileModel();
            InvoiceFileModel invoiceProtocolFileModel = new InvoiceFileModel();
            IDictionary<string, byte[]> invoiceAttachments = new Dictionary<string, byte[]>();
            XmlFactory xmlFactory = new XmlFactory();
            XMLConverterModel xmlConverterModel;
            Dictionary<string, object> invoice_metadatas = new Dictionary<string, object>();
            FileInfo fileInfo = null;
            string mailBoxRecipient = string.Empty;
            ArchiveDocument archiveDocument;
            try
            {
                PECMail pecMail = evt.ContentType.ContentTypeValue;
                _logger.WriteDebug(new LogMessage($"WorkflowStartReceivableInvoiceCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"Evaluate PECMail id {pecMail.UniqueId}"), LogCategories);
                mailBoxRecipient = string.IsNullOrEmpty(pecMail.PECMailBox.MailBoxRecipient) ? evt.CustomProperties[CustomPropertyName.PECMAIL_RECIPIENTS] as string : pecMail.PECMailBox.MailBoxRecipient;

                foreach (PECMailAttachment pecMailAttachment in pecMail.Attachments
                    .Where(f => f.IDDocument.HasValue && f.IDDocument.Value != Guid.Empty &&
                    ((fileInfo = new FileInfo(f.AttachmentName)).Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase) || fileInfo.Extension.Equals(".p7m", StringComparison.InvariantCultureIgnoreCase))))
                {
                    _logger.WriteDebug(new LogMessage($"Evaluate attachment {pecMailAttachment.AttachmentName}"), LogCategories);
                    try
                    {
                        pecmMailBibloContent = await _documentClient.GetDocumentContentByIdAsync(pecMailAttachment.IDDocument.Value);
                        if (fileInfo.Extension.Equals(".p7m", StringComparison.InvariantCultureIgnoreCase))
                        {
                            xmlContent = EInvoiceHelper.TryGetInvoiceSignedContent(pecmMailBibloContent.Blob, (f, ex) => _logger.WriteWarning(new LogMessage(f), ex, LogCategories));
                        }
                        else
                        {
                            xmlContent = EncodingUtil.GetEncoding(pecmMailBibloContent.Blob).GetString(pecmMailBibloContent.Blob);
                        }

                        xmlConverterModel = xmlFactory.BuildXmlModel(xmlContent);
                        if (xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V12 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePR_V12 ||
                            xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V10 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V11)
                        {
                            archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                            {
                                Archive = _workflowLocation.ProtocolArchive,
                                ContentStream = pecmMailBibloContent.Blob,
                                Name = pecMailAttachment.AttachmentName,
                            });
                            invoiceUDSFileModel.InvoiceBiblosDocumentId = archiveDocument.IdDocument;
                            invoiceUDSFileModel.InvoiceFilename = pecMailAttachment.AttachmentName;
                            archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                            {
                                Archive = _workflowLocation.ProtocolArchive,
                                ContentStream = pecmMailBibloContent.Blob,
                                Name = pecMailAttachment.AttachmentName,
                            });
                            invoiceProtocolFileModel.InvoiceBiblosDocumentId = archiveDocument.IdDocument;
                            invoiceProtocolFileModel.InvoiceFilename = pecMailAttachment.AttachmentName;
                        }
                        if (!EInvoiceHelper.TryGetSDIFileMetadatas(xmlContent, (a) => _logger.WriteDebug(new LogMessage(a), LogCategories), invoice_metadatas))
                        {
                            _logger.WriteWarning(new LogMessage($"Skiping {pecMailAttachment.AttachmentName} looking file metadati"), LogCategories);
                        }
                        else
                        {
                            invoiceAttachments.Add(pecMailAttachment.AttachmentName, pecmMailBibloContent.Blob);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage("An error occured during inoivce metadata reading"), ex, LogCategories);
                        throw;
                    }
                }
                await WorkflowStartReceivableInvoice(xmlContent, invoiceUDSFileModel, invoiceProtocolFileModel, invoiceAttachments, mailBoxRecipient, Guid.NewGuid(), invoice_metadatas, pecMail: pecMail);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowStartReceivableInvoiceCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task WorkflowStartReceivableInvoice(string xmlContent, InvoiceFileModel invoiceUDSFileModel, InvoiceFileModel invoiceProtocolFileModel,
            IDictionary<string, byte[]> invoiceAttachments, string mailBoxRecipient, Guid correlationId, Dictionary<string, object> invoice_metadatas, PECMail pecMail = null,
            string tenantName = "", bool checkIfExist = true, bool simulation = false)
        {
            bool isInvoiceB2B = false;
            XMLModelKind xmlModelKind_iteration = XMLModelKind.Invalid;
            InvoiceContactModel invoiceContactModel = null;
            InvoiceConfiguration invoiceConfiguration;
            Guid udsID;
            Guid protocolUniqueId;
            UDSRepository udsRepository;
            Contact contact = null;
            WorkflowResult workflowResult;
            WorkflowReferenceModel workflowReferenceModelProtocol;
            WorkflowReferenceModel workflowReferenceModelUDS;
            ICollection<Contact> contacts;
            List<Role> roles = new List<Role>();
            List<RoleModel> roleModels;
            string workflowInstanceSubject;
            KeyValuePair<string, WorkflowConfigurationModel> workflowConfiguration;
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowStartReceivableInvoice -> evaluate invoice {invoiceUDSFileModel.InvoiceFilename}"), LogCategories);

                try
                {
                    invoice_metadatas = FillIReceivableInvoiceMetadatas(xmlContent, invoice_metadatas, invoiceAttachments, out invoiceContactModel, out xmlModelKind_iteration);
                    isInvoiceB2B |= xmlModelKind_iteration == XMLModelKind.InvoicePR_V12 || xmlModelKind_iteration == XMLModelKind.InvoicePA_V12;
                }
                catch (Exception ex)
                {
                    _logger.WriteError(new LogMessage($"Receivable invoice {invoiceUDSFileModel.InvoiceFilename} An error occured during invoice metadata reading: {ex.Message}"), LogCategory.NotifyToEmailCategory);
                    _logger.WriteError(new LogMessage("An error occured during invoice metadata reading"), ex, LogCategories);
                    throw;
                }

                if (!isInvoiceB2B)
                {
                    _logger.WriteError(new LogMessage($"Receivable invoice {invoiceUDSFileModel.InvoiceFilename} evaluation found unsupport {xmlModelKind_iteration} XmlModelKind. This implementation support only B2B Invoices"), LogCategory.NotifyToEmailCategory);
                    throw new ArgumentException($"Invoice xml evaluation found unsupport {xmlModelKind_iteration} XmlModelKind. This implementation support only B2B Invoices");
                }

                if (_moduleConfiguration.WorkflowConfigurations.Any(f => f.Key.Equals(mailBoxRecipient, StringComparison.InvariantCultureIgnoreCase)))
                {
                    workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.Single(f => f.Key.Equals(mailBoxRecipient, StringComparison.InvariantCultureIgnoreCase));
                    _logger.WriteDebug(new LogMessage($"Specific configuration found from PECMailBox {mailBoxRecipient}"), LogCategories);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage($"Specific configuration not found for PECMailBox {mailBoxRecipient}. Using default configuration"), LogCategories);
                    workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.SingleOrDefault(f => string.IsNullOrEmpty(f.Key));

                    if (!string.IsNullOrEmpty(tenantName))
                    {
                        workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.SingleOrDefault(f => f.Value.InvoiceTypes.Any(x => x.Value.UDSRepositoryName.StartsWith($"{tenantName} - ")));
                        if (workflowConfiguration.Value != null)
                        {
                            _logger.WriteDebug(new LogMessage($"Specific configuration found with TenantName {tenantName}"), LogCategories);
                        }
                        else
                        {
                            workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.Single(f => string.IsNullOrEmpty(f.Key));
                        }
                    }
                }

                if (!invoice_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_IndirizzoPec))
                {
                    invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_IndirizzoPec, mailBoxRecipient);
                }
                if (!workflowConfiguration.Value.InvoiceTypes.ContainsKey(xmlModelKind_iteration))
                {
                    _logger.WriteError(new LogMessage($"Receivable invoice {invoiceUDSFileModel.InvoiceFilename} unsupport {xmlModelKind_iteration} XmlModelKind. This implementation support only B2B Invoices"), LogCategory.NotifyToEmailCategory);
                    throw new ArgumentException($"Invoice xml evaluation found unsupport {xmlModelKind_iteration} XmlModelKind. This implementation support only B2B Invoices");
                }

                invoiceConfiguration = workflowConfiguration.Value.InvoiceTypes[xmlModelKind_iteration];
                if (_moduleConfiguration.InvoiceTenantValidationEnabled && !string.IsNullOrEmpty(tenantName))
                {
                    Tenant currentTenant = (await _webAPIClient.GetTenantsAsync($"$filter=TenantName eq '{tenantName}'&$expand=TenantWorkflowRepositories")).SingleOrDefault();
                    if (currentTenant != null)
                    {
                        XmlFactory xmlFactory = new XmlFactory();
                        XMLConverterModel xmlConverterModel = xmlFactory.BuildXmlModel(xmlContent);
                        FatturaElettronicaType fatturaElettronica = (FatturaElettronicaType)xmlConverterModel.Model;
                        InvoiceValidation validation = new InvoiceValidation(fatturaElettronica, currentTenant);
                        if (!validation.ValidateTenantPiva(out string tenantPiva))
                        {
                            _logger.WriteWarning(new LogMessage($"Invalid VAT {fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.IdFiscaleIVA?.IdPaese}{fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.IdFiscaleIVA?.IdCodice} or FiscalCode {fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.CodiceFiscale} : {currentTenant.CompanyName}. Workflow will be skipped"), LogCategories);
                            throw new ArgumentException($"Invalid company name {fatturaElettronica.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items[0]} : {tenantPiva}. Workflow will be skipped");
                        }
                        //invoiceConfiguration = JsonConvert.DeserializeObject<InvoiceConfiguration>(currentTenant.TenantWorkflowRepositories.First().JsonValue);
                    }
                }

                DocSuiteWeb.Entity.Commons.Container container = (await _webAPIClient.GetContainerAsync(invoiceConfiguration.ProtocolContainerId)).SingleOrDefault();
                if (container == null)
                {
                    throw new ArgumentException($"Container {invoiceConfiguration.ProtocolContainerId} not found");
                }
                if (invoiceContactModel == null || string.IsNullOrEmpty(invoiceContactModel.Description) || string.IsNullOrEmpty(invoiceContactModel.SDIIdentification) || string.IsNullOrEmpty(invoiceContactModel.Pivacf))
                {
                    throw new ArgumentException($"contact description {invoiceContactModel.Description} is empty or SDIIdentification {invoiceContactModel.SDIIdentification} is empty  or Pivacf {invoiceContactModel.Pivacf} is empty");
                }
                contacts = await _webAPIClient.GetContactAsync($"$filter=SDIIdentification eq '{invoiceContactModel.SDIIdentification}' and IncrementalFather eq {invoiceConfiguration.ContactParent}");
                contact = contacts.FirstOrDefault();
                if (contact == null)
                {
                    _logger.WriteDebug(new LogMessage($"Contact '{invoiceContactModel.SDIIdentification}' not found and it's going to be creating."), LogCategories);
                    contact = await CreateContactAsync(invoiceConfiguration.ContactParent, invoiceContactModel);
                }
                foreach (short roleId in invoiceConfiguration.AuthorizationRoles)
                {
                    roles.Add(await _webAPIClient.GetRoleAsync(roleId));
                }
                roleModels = roles.Select(role => new RoleModel()
                {
                    IdRole = role.EntityShortId,
                    Name = role.Name,
                    TenantId = role.TenantId,
                    UniqueId = role.UniqueId,
                    RoleLabel = "Autorizzazione",
                    FullIncrementalPath = role.FullIncrementalPath
                }).ToList();

                Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
                string controllerName;
                _logger.WriteDebug(new LogMessage($"Finding UDS invoice using {invoiceUDSFileModel.InvoiceFilename} filename"), LogCategories);
                Dictionary<string, object> uds_metadatas = null;
                foreach (KeyValuePair<string, WorkflowConfigurationModel> item in _moduleConfiguration.WorkflowConfigurations)
                {
                    udsRepository = (await _webAPIClient.GetUDSRepository(item.Value.InvoiceTypes[xmlModelKind_iteration].UDSRepositoryName)).Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
                    controllerName = Utils.GetWebAPIControllerName(item.Value.InvoiceTypes[xmlModelKind_iteration].UDSRepositoryName);
                    try
                    {
                        uds_metadatas = await _webAPIClient.GetUDSByInvoiceFilename(controllerName, invoiceUDSFileModel.InvoiceFilename, invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura] as string, false, documents);
                    }
                    catch (InvalidOperationException)
                    {
                        throw new ArgumentException($"Invoice {invoiceUDSFileModel.InvoiceFilename} exists more than one unique record expected in UDS archive {item.Value.InvoiceTypes[xmlModelKind_iteration].UDSRepositoryName}");
                    }
                    if (uds_metadatas != null && uds_metadatas.Any())
                    {
                        if (checkIfExist || (!checkIfExist && udsRepository.Name == invoiceConfiguration.UDSRepositoryName))
                        {
                            throw new AlreadyExistsInvoiceException(invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura] as string, invoiceUDSFileModel.InvoiceFilename, udsRepository.Name,
                                uds_metadatas[UDSEInvoiceHelper.UDSMetadata_UDSId] as string, item.Key);
                        }
                    }
                }

                if (simulation)
                {
                    _logger.WriteInfo(new LogMessage($"Simulation request for {invoiceUDSFileModel.InvoiceFilename} invoice. This file has been skipped."), LogCategories);
                    return;
                }
                udsRepository = (await _webAPIClient.GetUDSRepository(invoiceConfiguration.UDSRepositoryName)).Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
                controllerName = Utils.GetWebAPIControllerName(invoiceConfiguration.UDSRepositoryName);
                protocolUniqueId = Guid.NewGuid();
                udsID = Guid.NewGuid();
                _logger.WriteDebug(new LogMessage($"Preaparing starting workflow with correlationId {correlationId}, protocolUniqueId {protocolUniqueId}, udsID {udsID}"), LogCategories);
                workflowReferenceModelUDS = await CreateUDSBuildModelAsync(invoiceUDSFileModel, contact, udsRepository, roleModels, udsID, protocolUniqueId, correlationId, invoice_metadatas,
                    invoiceAttachments, invoiceConfiguration.WorkflowRepositoryName);
                workflowReferenceModelProtocol = await CreateProtocolBuildModelAsync(invoiceProtocolFileModel, container, contact, udsRepository, roleModels, protocolUniqueId, udsID, invoiceConfiguration.ProtocolCategoryId,
                    correlationId, invoice_metadatas, invoiceAttachments, pecMail, invoiceConfiguration.WorkflowRepositoryName);
                workflowInstanceSubject = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()} ({invoiceUDSFileModel.InvoiceFilename})";
                workflowResult = await StartWorkflowAsync(workflowReferenceModelProtocol, workflowReferenceModelUDS, workflowConfiguration.Value, invoiceConfiguration.WorkflowRepositoryName, workflowInstanceSubject);
                if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
                {
                    _logger.WriteError(new LogMessage($"Receivable invoice {invoiceUDSFileModel.InvoiceFilename} an error occured in starting workflow"), LogCategory.NotifyToEmailCategory);
                    _logger.WriteError(new LogMessage("An error occured in start receivable invoice workflow"), LogCategories);
                    throw new Exception(string.Join(", ", workflowResult.Errors));
                }
            }
            catch (AlreadyExistsInvoiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowStartReceivableInvoiceCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private Dictionary<string, object> FillIReceivableInvoiceMetadatas(string xmlContent, Dictionary<string, object> invoice_metadatas,
            IDictionary<string, byte[]> invoiceAttachments, out InvoiceContactModel invoiceContactModel, out XMLModelKind xmlModelKind)
        {
            XmlFactory xmlFactory = new XmlFactory();
            invoiceContactModel = new InvoiceContactModel();
            XMLConverterModel xmlConverterModel = xmlFactory.BuildXmlModel(xmlContent);
            xmlModelKind = XMLModelKind.Invalid;
            _logger.WriteDebug(new LogMessage($"Xml content has {xmlConverterModel.ModelKind} with version {xmlConverterModel.Version}"), LogCategories);
            string tmpNomeAttachment;
            string newtmpNomeAttachment;
            int tmpNomeAttachmentext = 0;
            if (xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V12 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePR_V12 ||
                xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V10 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V11)
            {
                if (xmlConverterModel.Model is FatturaElettronicaType)
                {
                    FatturaElettronicaType fatturaElettronica = (FatturaElettronicaType)xmlConverterModel.Model;
                    if (EInvoiceHelper.TryGetReceivableInvoiceMetadatas(fatturaElettronica, (a) => _logger.WriteDebug(new LogMessage(a), LogCategories),
                        invoice_metadatas, out invoiceContactModel))
                    {
                        xmlModelKind = xmlConverterModel.ModelKind;
                        if (invoiceAttachments != null && fatturaElettronica != null && fatturaElettronica.FatturaElettronicaBody != null && fatturaElettronica.FatturaElettronicaBody.Any())
                        {
                            foreach (AllegatiType item in fatturaElettronica.FatturaElettronicaBody.Where(f => f.Allegati != null)
                                .SelectMany(f => f.Allegati).Where(f => f != null && f.Attachment.Length > 0 && !string.IsNullOrEmpty(f.NomeAttachment)))
                            {
                                item.NomeAttachment = string.Concat(item.NomeAttachment.Split(Path.GetInvalidFileNameChars()));
                                tmpNomeAttachment = Path.HasExtension(item.NomeAttachment) ? item.NomeAttachment : $"{item.NomeAttachment}.{item.FormatoAttachment}";
                                newtmpNomeAttachment = tmpNomeAttachment;
                                while (invoiceAttachments.ContainsKey(newtmpNomeAttachment))
                                {
                                    newtmpNomeAttachment = $"doc_{(++tmpNomeAttachmentext).ToString("000")}_{tmpNomeAttachment}";
                                }
                                invoiceAttachments.Add(newtmpNomeAttachment, item.Attachment);
                            }
                        }
                    }
                }
                if (xmlConverterModel.Model is InvoiceFSM1_0.FatturaElettronicaType)
                {
                    InvoiceFSM1_0.FatturaElettronicaType fatturaElettronica = (InvoiceFSM1_0.FatturaElettronicaType)xmlConverterModel.Model;
                    if (EInvoiceHelper.TryGetReceivableInvoiceMetadatas(fatturaElettronica, (a) => _logger.WriteDebug(new LogMessage(a), LogCategories),
                        invoice_metadatas, out invoiceContactModel))
                    {
                        xmlModelKind = xmlConverterModel.ModelKind;
                        if (invoiceAttachments != null && fatturaElettronica != null && fatturaElettronica.FatturaElettronicaBody != null && fatturaElettronica.FatturaElettronicaBody.Any())
                        {
                            foreach (InvoiceFSM1_0.AllegatiType item in fatturaElettronica.FatturaElettronicaBody.Where(f => f.Allegati != null)
                                .SelectMany(f => f.Allegati).Where(f => f != null && f.Attachment.Length > 0 && !string.IsNullOrEmpty(f.NomeAttachment)))
                            {
                                item.NomeAttachment = string.Concat(item.NomeAttachment.Split(Path.GetInvalidFileNameChars()));
                                tmpNomeAttachment = Path.HasExtension(item.NomeAttachment) ? item.NomeAttachment : $"{item.NomeAttachment}.{item.FormatoAttachment}";
                                newtmpNomeAttachment = tmpNomeAttachment;
                                while (invoiceAttachments.ContainsKey(newtmpNomeAttachment))
                                {
                                    newtmpNomeAttachment = $"doc_{(++tmpNomeAttachmentext).ToString("000")}_{tmpNomeAttachment}";
                                }
                                invoiceAttachments.Add(newtmpNomeAttachment, item.Attachment);
                            }
                        }
                    }
                }
            }
            return invoice_metadatas;
        }

        private async Task<Contact> CreateContactAsync(int contactFatherId, InvoiceContactModel invoiceContactModel)
        {
            Guid uniqueId = Guid.NewGuid();
            Contact contact = new Contact()
            {
                IdContactType = DocSuiteWeb.Entity.Commons.ContactType.AOO,
                IncrementalFather = contactFatherId,
                Description = invoiceContactModel.Description,
                SearchCode = invoiceContactModel.Pivacf,
                SDIIdentification = invoiceContactModel.SDIIdentification,
                FiscalCode = invoiceContactModel.Pivacf,
                Address = invoiceContactModel.Address,
                City = invoiceContactModel.City,
                ZipCode = invoiceContactModel.CAP,
                IsActive = 1,
                IsLocked = 0,
                IsChanged = 0,
                UniqueId = uniqueId
            };
            contact = await _webAPIClient.PostAsync(contact);
            contact = (await _webAPIClient.GetContactAsync($"$filter=UniqueId eq {uniqueId}")).Single();
            _logger.WriteInfo(new LogMessage($"Contact '{invoiceContactModel.Description}' ({contact.EntityId}) has been create succesfully"), LogCategories);
            return contact;
        }

        private async Task<WorkflowReferenceModel> CreateProtocolBuildModelAsync(InvoiceFileModel invoiceFileModel, DocSuiteWeb.Entity.Commons.Container container, Contact contact,
            UDSRepository uDSRepository, List<RoleModel> roles, Guid protocolUniqueId, Guid udsID, short categoryId, Guid correlationId,
            Dictionary<string, object> invoice_metadatas, IDictionary<string, byte[]> invoiceAttachments, PECMail pecMail, string workflowName)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };
            ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                UniqueId = workflowReferenceModel.ReferenceId,
                Protocol = new ProtocolModel
                {
                    Category = new CategoryModel() { IdCategory = categoryId },
                    Container = new ContainerModel()
                    {
                        IdContainer = container.EntityShortId,
                        Name = container.Name,
                        ProtLocation = new LocationModel()
                        {
                            IdLocation = container.ProtLocation.EntityShortId,
                            ProtocolArchive = container.ProtLocation.ProtocolArchive
                        }
                    }
                }
            };
            if (invoice_metadatas.ContainsKey(EInvoiceHelper.Metadata_IdentificativoSdi))
            {
                protocolBuildModel.Protocol.SDIIdentification = invoice_metadatas[EInvoiceHelper.Metadata_IdentificativoSdi].ToString();
            }

            protocolBuildModel.Protocol.UniqueId = protocolUniqueId;
            protocolBuildModel.Protocol.ProtocolType = new ProtocolTypeModel(ProtocolTypology.Inbound);
            protocolBuildModel.Protocol.Object = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura passiva n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}";
            protocolBuildModel.Protocol.DocumentCode = invoiceFileModel.InvoiceFilename;
            protocolBuildModel.Protocol.Contacts.Add(new ProtocolContactModel()
            {
                ComunicationType = ComunicationType.Sender,
                IdContact = contact.EntityId,
                Description = contact.Description
            });
            foreach (RoleModel role in roles)
            {
                protocolBuildModel.Protocol.Roles.Add(new ProtocolRoleModel()
                {
                    NoteType = ProtocolRoleNoteType.Accessible,
                    Status = ProtocolRoleStatus.ToEvaluate,
                    Role = role
                });
            }
            protocolBuildModel.Protocol.MainDocument = new DocumentModel()
            {
                FileName = invoiceFileModel.InvoiceFilename,
                DocumentToStoreId = invoiceFileModel.InvoiceBiblosDocumentId
            };
            ArchiveDocument archiveDocument;
            List<DocumentModel> invoiceAttachmentFiles = new List<DocumentModel>();
            foreach (KeyValuePair<string, byte[]> item in invoiceAttachments)
            {
                archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = item.Value,
                    Name = item.Key,
                });
                invoiceAttachmentFiles.Add(new DocumentModel()
                {
                    FileName = item.Key,
                    DocumentToStoreId = archiveDocument.IdDocument
                });
            }
            protocolBuildModel.Protocol.Attachments = invoiceAttachmentFiles;

            if (pecMail != null)
            {
                protocolBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                   new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
                   new DocumentUnitModel() { UniqueId = pecMail.UniqueId, EntityId = pecMail.EntityId, Environment = (int)DSWEnvironmentType.PECMail }));
            }
            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private async Task<WorkflowReferenceModel> CreateUDSBuildModelAsync(InvoiceFileModel invoiceFileModel, Contact contact, UDSRepository uDSRepository, List<RoleModel> roleModels,
            Guid udsID, Guid protocolUniqueId, Guid correlationId, IDictionary<string, object> invoice_metadatas, IDictionary<string, byte[]> invoiceAttachments, string workflowName)
        {
            WorkflowReferenceModel workflowReferenceModel = new WorkflowReferenceModel
            {
                ReferenceId = correlationId
            };
            UDSModel model = UDSModel.LoadXml(uDSRepository.ModuleXML);
            model.Model.UDSId = udsID.ToString();
            model.Model.Subject.Value = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura passiva n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}";
            IDictionary<string, object> uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceMetadatas(invoice_metadatas, DateTime.UtcNow);
            if (invoice_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi))
            {
                uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi] = invoice_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi];
            }
            if (invoice_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_DataAccettazione))
            {
                uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataAccettazione] = invoice_metadatas[UDSEInvoiceHelper.UDSMetadata_DataAccettazione];
            }
            uds_metadatas[UDSMetadata_Workflow_ApprovazioneTecnica] = string.Empty;
            uds_metadatas[UDSMetadata_Workflow_ApprovazioneAlPagamento] = string.Empty;
            model.FillMetaData(uds_metadatas);
            model = UDSEInvoiceHelper.InitDocumentStructures(model);

            model.Model.Documents.Document.Instances = UDSEInvoiceHelper.FillDocumentInstances(new List<InvoiceFileModel>() { invoiceFileModel });

            List<InvoiceFileModel> invoiceAttachmentFiles = new List<InvoiceFileModel>();
            ArchiveDocument archiveDocument;
            foreach (KeyValuePair<string, byte[]> item in invoiceAttachments)
            {
                archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = item.Value,
                    Name = item.Key,
                });
                invoiceAttachmentFiles.Add(new InvoiceFileModel()
                {
                    InvoiceFilename = item.Key,
                    InvoiceBiblosDocumentId = archiveDocument.IdDocument
                });
            }
            model.Model.Documents.DocumentAttachment.Instances = UDSEInvoiceHelper.FillDocumentInstances(invoiceAttachmentFiles);

            Contacts contacts = model.Model.Contacts.Single();
            if (contacts.ContactInstances == null)
            {
                contacts.ContactInstances = new ContactInstance[]
                {
                    new ContactInstance() { IdContact = contact.EntityId }
                };
            }

            model.Model.Authorizations = model.Model.Authorizations ?? new Authorizations();
            model = UDSEInvoiceHelper.FillAuthorizations(model, roleModels);

            UDSBuildModel udsBuildModel = new UDSBuildModel(model.SerializeToXml())
            {
                WorkflowName = workflowName,
                WorkflowAutoComplete = true,
                WorkflowActions = new List<IWorkflowAction>(),
                Documents = new List<UDSDocumentModel>(),
                Roles = new List<RoleModel>(),
                Users = new List<UserModel>(),
                UniqueId = correlationId,
                UDSRepository = new UDSRepositoryModel(uDSRepository.UniqueId)
                {
                    DSWEnvironment = uDSRepository.DSWEnvironment,
                    Name = uDSRepository.Name
                },
                Subject = model.Model.Subject.Value
            };
            udsBuildModel.Roles = roleModels;
            udsBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                new DocumentUnitModel() { UniqueId = protocolUniqueId, Environment = (int)DSWEnvironmentType.Protocol },
                new DocumentUnitModel() { UniqueId = udsID, Environment = uDSRepository.DSWEnvironment, IdUDSRepository = uDSRepository.UniqueId }));
            workflowReferenceModel.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModel.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            return workflowReferenceModel;
        }

        private async Task<WorkflowResult> StartWorkflowAsync(WorkflowReferenceModel workflowReferenceModelProtocol, WorkflowReferenceModel workflowReferenceModelUDS,
            WorkflowConfigurationModel workflowConfiguration, string workflowName, string subject)
        {
            WorkflowStart workflowStart = new WorkflowStart
            {
                WorkflowName = workflowName
            };

            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"),
                new WorkflowArgument()
                {
                    Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_0"),
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(workflowReferenceModelProtocol)
                });
            workflowStart.Arguments.Add(string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"),
                new WorkflowArgument()
                {
                    Name = string.Concat(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "_1"),
                    PropertyType = ArgumentType.Json,
                    ValueString = JsonConvert.SerializeObject(workflowReferenceModelUDS)
                });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID,
                ValueGuid = _moduleConfiguration.TenantId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, new WorkflowArgument()
            {
                PropertyType = ArgumentType.RelationGuid,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID,
                ValueGuid = workflowConfiguration.TenantAOOId
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME,
                ValueString = _moduleConfiguration.TenantName
            });
            workflowStart.Arguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_INSTANCE_SUBJECT, new WorkflowArgument()
            {
                PropertyType = ArgumentType.PropertyString,
                Name = WorkflowPropertyHelper.DSW_PROPERTY_INSTANCE_SUBJECT,
                ValueString = subject
            });
            WorkflowResult workflowResult = await _webAPIClient.StartWorkflow(workflowStart);
            _logger.WriteInfo(new LogMessage(string.Concat("Workflow started correctly [IsValid: ", workflowResult.IsValid, "] with instanceId ", workflowResult.InstanceId)), LogCategories);
            return workflowResult;
        }

        #endregion

        #region [ WorkflowStartReceivableInvoiceAdECallback ]

        private async Task WorkflowStartReceivableInvoiceAdECallback(EventDematerialisationRequest evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowStartReceivableInvoiceAdECallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying WorkflowStartReceivableInvoiceAdE for CorrelationId {evt.CorrelationId}"), LogCategories);

                DocumentManagementRequestModel documentManagementRequestModel = evt.ContentType.ContentTypeValue;
                WorkflowReferenceBiblosModel workflowReferenceBiblosModel = documentManagementRequestModel.Documents.First(f => f.ArchiveChainId.HasValue);
                _logger.WriteDebug(new LogMessage($"Notifying WorkflowReferenceBiblosModel ChainId {workflowReferenceBiblosModel.ArchiveChainId}"), LogCategories);
                BiblosDocument biblosDocument = (await _documentClient.GetDocumentChildrenAsync(workflowReferenceBiblosModel.ArchiveChainId.Value)).Single(f => f.IsLatestVersion);
                Content content = await _documentClient.GetDocumentContentByIdAsync(biblosDocument.IdDocument);
                using (ZipArchive archive = new ZipArchive(new MemoryStream(content.Blob), ZipArchiveMode.Read, true, null))
                {
                    IEnumerable<ZipArchiveEntry> metadatas = archive.Entries;
                    metadatiFattura metadati;
                    string invoiceFilename;
                    string sdiIdentification;
                    string supplierDescription = string.Empty;
                    ZipArchiveEntry zipFattura;
                    Dictionary<string, object> invoice_metadatas;
                    string xmlMetadata;
                    byte[] metadataContent;
                    byte[] invoiceContent;
                    string xmlInvoice = string.Empty;
                    string mailBoxRecipient = string.Empty;
                    metadatiFatturaMetadato metadato;
                    InvoiceFileModel invoiceUDSFileModel;
                    InvoiceFileModel invoiceProtocolFileModel;
                    List<InvoicePreviewModel> simulationResults = new List<InvoicePreviewModel>();
                    InvoicePreviewModel currentInvoicePreviewModel = new InvoicePreviewModel();
                    IDictionary<string, byte[]> invoiceAttachments;
                    ArchiveDocument archiveDocument;
                    bool workflowSimulation = workflowReferenceBiblosModel.Simulation ?? false;
                    await _webAPIClient.PushCorrelatedNotificationAsync($"L'archivio compresso contiene <b>{metadatas.Count()}</b> elementi da analizzare di cui <b>{metadatas.Count() / 2}</b> sono probabili fatture elettroniche. Analisi in corso...",
                       ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);
                    _logger.WriteDebug(new LogMessage($"Found files {metadatas.Count()}"), LogCategories);
                    int i = 1;
                    int processed = 0;
                    ZipEntryProcessSummary zipEntrySummary;
                    ProcessSummary processSummary = new ProcessSummary(metadatas.Count());

                    foreach (ZipArchiveEntry zipMetadata in metadatas)
                    {
                        invoiceUDSFileModel = new InvoiceFileModel();
                        invoiceProtocolFileModel = new InvoiceFileModel();
                        invoiceAttachments = new Dictionary<string, byte[]>();
                        xmlInvoice = string.Empty;
                        mailBoxRecipient = string.Empty;
                        invoice_metadatas = new Dictionary<string, object>();
                        zipEntrySummary = new ZipEntryProcessSummary();
                        processSummary.Add(zipEntrySummary);
                        zipEntrySummary.ZipMetadataName = zipMetadata.FullName;

                        _logger.WriteDebug(new LogMessage($"Evalauting metadata {zipMetadata.FullName}({i++})"), LogCategories);
                        try
                        {
                            using (Stream sourceStream = zipMetadata.Open())
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                sourceStream.CopyTo(memoryStream);
                                metadataContent = memoryStream.ToArray();
                            }
                            using (Stream metadataStream = zipMetadata.Open())
                            using (StreamReader metadataReader = new StreamReader(metadataStream))
                            {
                                xmlMetadata = metadataReader.ReadToEnd();
                                try
                                {
                                    metadati = XmlConvert.Deserialize<metadatiFattura>(xmlMetadata);
                                }
                                catch (Exception)
                                {
                                    string message = $"Skip not valid metadata file {zipMetadata.FullName}";
                                    zipEntrySummary.ErrorMessage = message;
                                    _logger.WriteWarning(new LogMessage(message), LogCategories);
                                    continue;
                                }

                                invoiceFilename = metadati.metadato.First(f => f.nome == "nomefile").valore;
                                zipEntrySummary.InvoiceFileName = zipMetadata.FullName;
                                sdiIdentification = metadati.metadato.First(f => f.nome == "idfile").valore;
                                if (metadati.metadato.Any(f => f.nome == "cedentedenominazione"))
                                {
                                    supplierDescription = metadati.metadato.First(f => f.nome == "cedentedenominazione").valore;
                                }
                                await _webAPIClient.PushCorrelatedNotificationAsync($"E' stata trovata la fattura elettronica <b>{invoiceFilename}</b> del fornitore <b>{supplierDescription}</b> con identificativo SDI <b>{sdiIdentification}</b>.",
                                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);
                                zipFattura = archive.Entries.Single(f => f.Name == invoiceFilename);
                                invoiceAttachments.Add(zipMetadata.Name, metadataContent);
                                metadato = metadati.metadato.First(f => f.nome == "idfile");
                                _logger.WriteDebug(new LogMessage($"Metadata {metadato.nome} {metadato.valore} -> {EInvoiceHelper.Metadata_IdentificativoSdi}"), LogCategories);
                                invoice_metadatas.Add(EInvoiceHelper.Metadata_IdentificativoSdi, metadato.valore);

                                if (metadati.metadato.Any(f => f.nome == "hashfile"))
                                {
                                    metadato = metadati.metadato.First(f => f.nome == "hashfile");
                                    _logger.WriteDebug(new LogMessage($"Metadata {metadato.nome} {metadato.valore} -> {EInvoiceHelper.Metadata_HashSdi}"), LogCategories);
                                    invoice_metadatas.Add(EInvoiceHelper.Metadata_HashSdi, metadato.valore);
                                }
                                if (metadati.metadato.Any(f => f.nome == "dataaccoglienza"))
                                {
                                    metadato = metadati.metadato.First(f => f.nome == "dataaccoglienza");
                                    _logger.WriteDebug(new LogMessage($"Metadata {metadato.nome} {metadato.valore} -> {UDSEInvoiceHelper.UDSMetadata_DataAccettazione}/{UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi}"), LogCategories);
                                    invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_DataAccettazione, DateTimeOffset.Parse(metadato.valore));
                                    invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi, DateTimeOffset.Parse(metadato.valore));
                                }
                                using (Stream sourceStream = zipFattura.Open())
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    sourceStream.CopyTo(memoryStream);
                                    invoiceContent = memoryStream.ToArray();
                                }
                                archiveDocument = null;
                                if (!workflowSimulation)
                                {
                                    archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                                    {
                                        Archive = _workflowLocation.ProtocolArchive,
                                        ContentStream = invoiceContent,
                                        Name = invoiceFilename,
                                    });
                                }
                                invoiceUDSFileModel.InvoiceFilename = invoiceFilename;
                                invoiceUDSFileModel.InvoiceBiblosDocumentId = workflowSimulation ? Guid.Empty : archiveDocument.IdDocument;

                                if (!workflowSimulation)
                                {
                                    archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                                    {
                                        Archive = _workflowLocation.ProtocolArchive,
                                        ContentStream = invoiceContent,
                                        Name = invoiceFilename,
                                    });
                                }
                                invoiceProtocolFileModel.InvoiceFilename = invoiceFilename;
                                invoiceProtocolFileModel.InvoiceBiblosDocumentId = workflowSimulation ? Guid.Empty : archiveDocument.IdDocument;

                                if (invoiceFilename.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    using (Stream invoiceStream = zipFattura.Open())
                                    using (StreamReader invoiceReader = new StreamReader(invoiceStream))
                                    {
                                        xmlInvoice = invoiceReader.ReadToEnd();
                                    }
                                }
                                if (invoiceFilename.EndsWith(".p7m", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    xmlInvoice = EInvoiceHelper.TryGetInvoiceSignedContent(invoiceContent, (f, ex) => _logger.WriteWarning(new LogMessage(f), ex, LogCategories));
                                }
                                XmlFactory xmlFactory = new XmlFactory();
                                XMLConverterModel xmlConverterModel = xmlFactory.BuildXmlModel(xmlInvoice);
                                if (xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V12 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePR_V12 ||
                                    xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V10 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V11)
                                {
                                    string description = string.Empty;
                                    if (xmlConverterModel.Model is FatturaElettronicaType)
                                    {
                                        FatturaElettronicaType fatturaElettronica = (FatturaElettronicaType)xmlConverterModel.Model;
                                        DatiGeneraliType datiGenerali = fatturaElettronica.FatturaElettronicaBody.Single(f => f.DatiGenerali != null).DatiGenerali;
                                        mailBoxRecipient = fatturaElettronica.FatturaElettronicaHeader.DatiTrasmissione.PECDestinatario;
                                        description = $"Fattura {datiGenerali.DatiGeneraliDocumento.Numero} di {supplierDescription} del {datiGenerali.DatiGeneraliDocumento.Data}";
                                    }
                                    if (xmlConverterModel.Model is InvoiceFSM1_0.FatturaElettronicaType)
                                    {
                                        InvoiceFSM1_0.FatturaElettronicaType fatturaElettronica = (InvoiceFSM1_0.FatturaElettronicaType)xmlConverterModel.Model;
                                        InvoiceFSM1_0.DatiGeneraliType datiGenerali = fatturaElettronica.FatturaElettronicaBody.Single(f => f.DatiGenerali != null).DatiGenerali;
                                        mailBoxRecipient = fatturaElettronica.FatturaElettronicaHeader.DatiTrasmissione.PECDestinatario;
                                        description = $"Fattura {datiGenerali.DatiGeneraliDocumento.Numero} di {supplierDescription} del {datiGenerali.DatiGeneraliDocumento.Data}";
                                    }
                                    currentInvoicePreviewModel = new InvoicePreviewModel()
                                    {
                                        InvoiceMetadataFilename = zipMetadata.Name,
                                        InvoiceFilename = invoiceFilename,
                                        Description = description,
                                        Selectable = true,
                                        Result = "Non presente"
                                    };
                                    simulationResults.Add(currentInvoicePreviewModel);
                                }
                                if (string.IsNullOrEmpty(mailBoxRecipient))
                                {
                                    mailBoxRecipient = workflowReferenceBiblosModel.ArchiveName;
                                }

                                await _webAPIClient.PushCorrelatedNotificationAsync($"Avvio flusso di lavoro per la fattura <b>{invoiceFilename}</b> in corso ...",
                                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);
                                await WorkflowStartReceivableInvoice(xmlInvoice, invoiceUDSFileModel, invoiceProtocolFileModel, invoiceAttachments, mailBoxRecipient,
                                    evt.CorrelationId.HasValue ? evt.CorrelationId.Value : Guid.NewGuid(), invoice_metadatas, tenantName: workflowReferenceBiblosModel.ArchiveName,
                                    simulation: workflowSimulation);
                            }
                        }
                        catch (AlreadyExistsInvoiceException ex)
                        {
                            currentInvoicePreviewModel.Selectable = false;
                            currentInvoicePreviewModel.Result = ex.InvoiceMessage;
                            if (!workflowReferenceBiblosModel.Simulation.HasValue || (workflowReferenceBiblosModel.Simulation.HasValue && !workflowReferenceBiblosModel.Simulation.Value))
                            {
                                string message = $"La fattura è stata scartate per le seguenti motivazioni: <b>{ex.InvoiceMessage}</b>";
                                zipEntrySummary.ErrorMessage = ex.ToString();
                                await _webAPIClient.PushCorrelatedNotificationAsync(message,
                                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationWarning);
                            }
                            _logger.WriteWarning(new LogMessage($"Metadata {zipMetadata.FullName} was skipped"), ex, LogCategories);
                            if (invoiceUDSFileModel != null && invoiceUDSFileModel.InvoiceBiblosDocumentId != Guid.Empty)
                            {
                                await _documentClient.DetachDocumentAsync(invoiceUDSFileModel.InvoiceBiblosDocumentId);
                            }
                            if (invoiceProtocolFileModel != null && invoiceProtocolFileModel.InvoiceBiblosDocumentId != Guid.Empty)
                            {
                                await _documentClient.DetachDocumentAsync(invoiceProtocolFileModel.InvoiceBiblosDocumentId);
                            }
                        }
                        catch (Exception ex)
                        {
                            //COLLECT ERRORS FOR SEND MESSAGE
                            string err = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            string message = $"Il file <b>{zipMetadata.FullName}</b> è stato scartato per le seguenti motivazioni: <b>{err}</b>";
                            zipEntrySummary.ErrorMessage = message;

                            await _webAPIClient.PushCorrelatedNotificationAsync(message,
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationError);
                            _logger.WriteWarning(new LogMessage($"Metadata {zipMetadata.FullName} was skipped"), ex, LogCategories);
                        }
                        zipEntrySummary.Processed = true;
                        processed++;
                    }
                    if (workflowSimulation)
                    {
                        await _webAPIClient.PushCorrelatedNotificationAsync(JsonConvert.SerializeObject(simulationResults), ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName,
                            evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfoAsModel);
                    }
                    else
                    {
                        await _webAPIClient.PushCorrelatedNotificationAsync($"L'importazione delle fatture dal cassetto fiscale si è conclusa con <b>{processed}</b> fatture processate",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                        await _documentClient.DetachDocumentAsync(biblosDocument.IdDocument);

                        CommandBuildMessage command = new CommandBuildMessage(
                            tenantName: _moduleConfiguration.TenantName,
                            tenantId: _moduleConfiguration.TenantId,
                            tenantAOOId: Guid.Empty,
                            evt.Identity,
                            MessageHelpers.CreateMessageBuildModel(_moduleConfiguration, processSummary));

                        await _webAPIClient.SendCommandAsync<CommandBuildMessage>(command);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Import ZIP AdE: Critical error occured during importing invoices: {ex.Message}"), LogCategory.NotifyToEmailCategory);

                await _webAPIClient.PushCorrelatedNotificationAsync("Importazione AdE : Errore critico, contattare l'assistenza.",
                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                _logger.WriteError(new LogMessage("WorkflowStartReceivableInvoiceAdECallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowStartInvoiceDeleteCallback ]

        private async Task WorkflowStartInvoiceDeleteCallback(EventDematerialisationRequest evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowStartInvoiceDeleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying WorkflowStartInvoiceDeleteCallback for CorrelationId {evt.CorrelationId}"), LogCategories);

                DocumentManagementRequestModel documentManagementRequestModel = evt.ContentType.ContentTypeValue;

                foreach (WorkflowReferenceBiblosModel wrbm in documentManagementRequestModel.Documents)
                {
                    if (wrbm.ArchiveChainId.HasValue)
                    {
                        Guid idUDS = wrbm.ArchiveChainId.Value;
                        string cancelMotivation = wrbm.ArchiveName;
                        _logger.WriteDebug(new LogMessage($"Notifying delete invoice {idUDS} with motivation {cancelMotivation}"), LogCategories);
                        DocumentUnit documentUnit = await _webAPIClient.GetDocumentUnitAsync(idUDS);

                        WorkflowResult workflowResult = await BuildInvoiceDelete(idUDS, documentUnit, cancelMotivation, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity);
                        await _webAPIClient.PushCorrelatedNotificationAsync($"Avvio del processo di anullamento della fattura e del protocollo è avvenuto correttamente.",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                        await _webAPIClient.PushCorrelatedNotificationAsync($"Attendere l'esito delle attività previste <b>Identificativo richiesta: {workflowResult.InstanceId}</b>",
                            ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Receivable invoice: Critical error occured during canceling invoice: {ex.Message}"), LogCategory.NotifyToEmailCategory);

                await _webAPIClient.PushCorrelatedNotificationAsync("Annullamento di fattura: Errore critico, contattare l'assistenza.",
                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                _logger.WriteError(new LogMessage("WorkflowStartInvoiceDeleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private async Task<WorkflowResult> BuildInvoiceDelete(Guid idUDS, DocumentUnit documentUnit, string cancelMotivation,
            Guid tenantId, Guid tenantAOOId, string tenantName, Guid? correlationId, IIdentityContext identity, string currentCompany = "")
        {
            if (documentUnit == null || documentUnit.UDSRepository == null)
            {
                await _webAPIClient.PushCorrelatedNotificationAsync($"Annullamento di fattura: La fattura {idUDS} non è stata trovata.",
                ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowStatusError);
                throw new ArgumentNullException("documentUnit", $"Invoice {idUDS} not found");
            }
            WorkflowConfigurationModel workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.Select(s => s.Value).FirstOrDefault(x => x.TenantAOOId == tenantAOOId);
            if (workflowConfiguration == null)
            {
                throw new ArgumentNullException("documentUnit", $"TenantAOOId {tenantAOOId} not found");
            }

            string controllerName = Utils.GetWebAPIControllerName(documentUnit.UDSRepository.Name);
            Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
            Dictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, idUDS, documents);
            _logger.WriteDebug(new LogMessage($"Found {uds_metadatas != null} invoice metadatas {documents.ContainsKey(1)}"), LogCategories);

            await _webAPIClient.PushCorrelatedNotificationAsync($"Preparazione annullamento fattura <b>{uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura]} del {((DateTime)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataFattura]).ToShortDateString()}</b>...",
                ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowNotificationInfo);

            ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(idUDS, false, false);
            if (udsDocumentUnits == null || !udsDocumentUnits.Any())
            {
                await _webAPIClient.PushCorrelatedNotificationAsync($"Annullamento di fattura: Non sono stati trovati protocolli da annullare per la fattura <b>{uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura]}</b>.",
                    ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowStatusError);
                await _webAPIClient.PushCorrelatedNotificationAsync("Procedere con l'annullamento manuale dell'archivio.",
                    ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowStatusError);

                throw new ArgumentNullException("udsDocumentUnits", $"Protocol related to invoice {idUDS} not found");
            }

            WorkflowReferenceModel workflowReferenceModelProtocol = new WorkflowReferenceModel();
            WorkflowReferenceModel workflowReferenceModelUDS = new WorkflowReferenceModel();

            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(idUDS);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(idUDS);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(idUDS);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(idUDS);
            UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(documentUnit.UDSRepository, idUDS, uds_metadatas, documents, udsRoles, udsContacts, udsMessages, udsPECMails,
                udsDocumentUnits, null, _username);
            udsBuildModel.WorkflowName = _moduleConfiguration.WorkflowInvoiceDeleteRepositoryName;
            udsBuildModel.WorkflowAutoComplete = true;
            udsBuildModel.WorkflowActions = new List<IWorkflowAction>();
            udsBuildModel.CancelMotivation = cancelMotivation;
            udsBuildModel.RegistrationUser = identity.User;
            workflowReferenceModelUDS.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModelUDS.ReferenceModel = JsonConvert.SerializeObject(udsBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            workflowReferenceModelUDS.ReferenceId = correlationId.Value;

            UDSDocumentUnit udsDocumentUnit = udsDocumentUnits.First(f => f.RelationType == DocSuiteWeb.Entity.UDS.UDSRelationType.ArchiveProtocol ||
                    f.RelationType == DocSuiteWeb.Entity.UDS.UDSRelationType.ProtocolArchived || f.RelationType == DocSuiteWeb.Entity.UDS.UDSRelationType.Protocol);
            await _webAPIClient.PushCorrelatedNotificationAsync($"Preparazione annullamento protocollo <b>{udsDocumentUnit.Relation.Year}/{udsDocumentUnit.Relation.Number:0000000} del {udsDocumentUnit.Relation.RegistrationDate.ToLocalTime().Date.ToShortDateString()}</b>...",
                ModuleConfigurationHelper.MODULE_NAME, tenantId, tenantAOOId, tenantName, correlationId, identity, NotificationType.EventWorkflowNotificationInfo);

            ProtocolBuildModel protocolBuildModel = new ProtocolBuildModel
            {
                WorkflowName = _moduleConfiguration.WorkflowInvoiceDeleteRepositoryName,
                WorkflowAutoComplete = true,
                UniqueId = correlationId.Value,
                Protocol = new ProtocolModel
                {
                    Number = udsDocumentUnit.Relation.Number,
                    Year = udsDocumentUnit.Relation.Year,
                    Object = udsDocumentUnit.Relation.Subject,
                    UniqueId = udsDocumentUnit.Relation.UniqueId,
                    CancelMotivation = cancelMotivation
                }
            };
            workflowReferenceModelProtocol.ReferenceType = DSWEnvironmentType.Build;
            workflowReferenceModelProtocol.ReferenceModel = JsonConvert.SerializeObject(protocolBuildModel, ModuleConfigurationHelper.JsonSerializerSettings);
            workflowReferenceModelProtocol.ReferenceId = correlationId.Value;
            WorkflowResult workflowResult = await StartWorkflowAsync(workflowReferenceModelProtocol, workflowReferenceModelUDS, workflowConfiguration, _moduleConfiguration.WorkflowInvoiceDeleteRepositoryName,
                $"Annullamento fattura {uds_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} ({idUDS})");
            if (!workflowResult.IsValid || !workflowResult.InstanceId.HasValue)
            {
                _logger.WriteError(new LogMessage($"Receivable invoice an error occured in cancel invoice workflow"), LogCategory.NotifyToEmailCategory);

                _logger.WriteError(new LogMessage("An error occured in start cancel invoice workflow"), LogCategories);
                throw new ArgumentException(string.Join(", ", workflowResult.Errors));
            }

            return workflowResult;
        }

        #endregion

        #region [ WorkflowStartInvoiceMoveCallback ]

        private async Task WorkflowStartInvoiceMoveCallback(EventDematerialisationRequest evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowStartInvoiceMoveCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying WorkflowStartInvoiceMoveCallback for CorrelationId {evt.CorrelationId}"), LogCategories);
                DocumentManagementRequestModel documentManagementRequestModel = evt.ContentType.ContentTypeValue;
                InvoiceFileModel invoiceUDSFileModel = null;
                InvoiceFileModel invoiceProtocolFileModel = null;
                Guid idUDS;
                Guid invoiceDocumentChain;
                string mailBoxRecipient = string.Empty;
                string xmlContent = string.Empty;
                string companyDestination = string.Empty;
                string cancelMotivation = string.Empty;
                string controllerName;
                DocumentUnit documentUnit;
                List<BiblosDocument> invoiceDocuments;
                ICollection<UDSDocumentUnit> udsDocumentUnits;
                IDictionary<string, byte[]> invoiceAttachments;
                Dictionary<int, Guid> uds_documents;
                Dictionary<string, object> uds_metadatas;
                Dictionary<string, object> invoice_metadatas;
                XmlFactory xmlFactory;
                XMLConverterModel xmlConverterModel;
                BiblosDocument invoiceDocument;
                Content invoiceContent;
                ArchiveDocument archiveDocument;

                foreach (WorkflowReferenceBiblosModel wrbm in documentManagementRequestModel.Documents)
                {
                    if (wrbm.ArchiveChainId.HasValue)
                    {
                        try
                        {
                            invoiceUDSFileModel = new InvoiceFileModel();
                            invoiceProtocolFileModel = new InvoiceFileModel();
                            invoiceAttachments = new Dictionary<string, byte[]>();
                            uds_documents = new Dictionary<int, Guid>();
                            invoice_metadatas = new Dictionary<string, object>();
                            xmlFactory = new XmlFactory();

                            idUDS = wrbm.ArchiveChainId.Value;
                            companyDestination = wrbm.DocumentName;
                            cancelMotivation = wrbm.ArchiveName;
                            _logger.WriteDebug(new LogMessage($"Notifying move invoice {idUDS} with motivation {cancelMotivation} and CompanyDestination {companyDestination}"), LogCategories);

                            documentUnit = await _webAPIClient.GetDocumentUnitAsync(idUDS);
                            if (documentUnit == null || documentUnit.UDSRepository == null)
                            {
                                await _webAPIClient.PushCorrelatedNotificationAsync($"Spostamento di fattura: La fattura {idUDS} non è stata trovata.",
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                                throw new ArgumentNullException("documentUnit", $"Invoice {idUDS} not found");
                            }
                            controllerName = Utils.GetWebAPIControllerName(documentUnit.UDSRepository.Name);
                            uds_metadatas = await _webAPIClient.GetUDS(controllerName, idUDS, uds_documents);
                            _logger.WriteDebug(new LogMessage($"Found {uds_metadatas != null} invoice metadatas {uds_documents.ContainsKey(1)}"), LogCategories);
                            udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(idUDS, false, false);
                            if (udsDocumentUnits == null || !udsDocumentUnits.Any())
                            {
                                await _webAPIClient.PushCorrelatedNotificationAsync($"Spostamento di fattura: Non sono stati trovati protocolli da annullare per la fattura <b>{uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura]}</b>. Procedere con l'annullamento manuale dell'archivio.",
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                                throw new ArgumentNullException("udsDocumentUnits", $"Protocol related to invoice {idUDS} not found");
                            }

                            await _webAPIClient.PushCorrelatedNotificationAsync($"Preparazione spostamento fattura <b>{uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura]} del {((DateTime)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataFattura]).ToShortDateString()}</b>.",
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);

                            mailBoxRecipient = companyDestination;
                            xmlContent = string.Empty;
                            invoiceDocumentChain = uds_documents[1];
                            _logger.WriteDebug(new LogMessage($"Get InvoiceDocument {invoiceDocumentChain}"), LogCategories);
                            invoiceDocuments = await _documentClient.GetDocumentChildrenAsync(invoiceDocumentChain);
                            _logger.WriteDebug(new LogMessage($"Found {invoiceDocuments?.Count} documents. Main document has identification {invoiceDocuments?.Single(f => f.IsLatestVersion)?.IdDocument}"), LogCategories);
                            invoiceDocument = invoiceDocuments.Single(f => f.IsLatestVersion);
                            invoiceContent = await _documentClient.GetDocumentContentByIdAsync(invoiceDocument.IdDocument);
                            if (invoiceDocument.Name.EndsWith(".p7m", StringComparison.InvariantCultureIgnoreCase))
                            {
                                xmlContent = EInvoiceHelper.TryGetInvoiceSignedContent(invoiceContent.Blob, (f, ex) => _logger.WriteWarning(new LogMessage(f), ex, LogCategories));
                            }
                            else
                            {
                                xmlContent = EncodingUtil.GetEncoding(invoiceContent.Blob).GetString(invoiceContent.Blob);
                            }

                            xmlConverterModel = xmlFactory.BuildXmlModel(xmlContent);
                            if (xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V12 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePR_V12 ||
                                xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V10 || xmlConverterModel.ModelKind == XMLModelKind.InvoicePA_V11)
                            {
                                archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                                {
                                    Archive = _workflowLocation.ProtocolArchive,
                                    ContentStream = invoiceContent.Blob,
                                    Name = invoiceDocument.Name,
                                });
                                invoiceUDSFileModel.InvoiceBiblosDocumentId = archiveDocument.IdDocument;
                                invoiceUDSFileModel.InvoiceFilename = invoiceDocument.Name;
                                archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                                {
                                    Archive = _workflowLocation.ProtocolArchive,
                                    ContentStream = invoiceContent.Blob,
                                    Name = invoiceDocument.Name,
                                });
                                invoiceProtocolFileModel.InvoiceBiblosDocumentId = archiveDocument.IdDocument;
                                invoiceProtocolFileModel.InvoiceFilename = invoiceDocument.Name;
                            }

                            invoiceDocumentChain = uds_documents[2];
                            foreach (BiblosDocument biblosAttachments in (await _documentClient.GetDocumentChildrenAsync(invoiceDocumentChain))
                                .Where(f => f.IsLatestVersion && f.Name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                _logger.WriteDebug(new LogMessage($"Evaluate attachment {biblosAttachments.Name}"), LogCategories);
                                try
                                {
                                    invoiceContent = await _documentClient.GetDocumentContentByIdAsync(biblosAttachments.IdDocument);

                                    if (!EInvoiceHelper.TryGetSDIFileMetadatas(xmlContent, (a) => _logger.WriteDebug(new LogMessage(a), LogCategories), invoice_metadatas))
                                    {
                                        _logger.WriteWarning(new LogMessage($"Skiping {biblosAttachments.Name} looking file metadati"), LogCategories);
                                    }
                                    invoiceAttachments.Add(biblosAttachments.Name, invoiceContent.Blob);
                                }
                                catch (Exception ex)
                                {
                                    _logger.WriteWarning(new LogMessage("An error occured during inoivce metadata reading"), ex, LogCategories);
                                    throw;
                                }
                            }

                            if (uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi) && uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi] != null)
                            {
                                invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi, uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi]);
                            }
                            if (uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_IndirizzoPec) && uds_metadatas[UDSEInvoiceHelper.UDSMetadata_IndirizzoPec] != null)
                            {
                                invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_IndirizzoPec, uds_metadatas[UDSEInvoiceHelper.UDSMetadata_IndirizzoPec]);
                            }

                            await _webAPIClient.PushCorrelatedNotificationAsync($"Avvio spostamento della fattura <b>{uds_metadatas[UDSEInvoiceHelper.UDSMetadata_NumeroFattura]} del {((DateTime)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataFattura]).ToShortDateString()}</b> nell'azienda <b>{companyDestination}</b>.",
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);
                            await WorkflowStartReceivableInvoice(xmlContent, invoiceUDSFileModel, invoiceProtocolFileModel, invoiceAttachments, mailBoxRecipient, evt.CorrelationId.Value,
                                invoice_metadatas, tenantName: companyDestination, checkIfExist: false);

                            WorkflowResult workflowResult = await BuildInvoiceDelete(idUDS, documentUnit, cancelMotivation, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity);
                            await _webAPIClient.PushCorrelatedNotificationAsync($"Avvio del processo di anullamento della fattura e del protocollo è avvenuto correttamente.",
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                            await _webAPIClient.PushCorrelatedNotificationAsync($"Attendere l'esito delle attività previste <b>Identificativo richiesta: {workflowResult.InstanceId}</b>",
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusDone);
                        }
                        catch (AlreadyExistsInvoiceException ex)
                        {
                            await _webAPIClient.PushCorrelatedNotificationAsync($"Spostamento di fattura: La fattura non è gestibile per le seguenti motivazioni: <b>{ex.InvoiceMessage}</b>",
                                ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationWarning);
                            _logger.WriteWarning(new LogMessage(ex.Message), ex, LogCategories);
                            if (invoiceUDSFileModel != null && invoiceUDSFileModel.InvoiceBiblosDocumentId != Guid.Empty)
                            {
                                await _documentClient.DetachDocumentAsync(invoiceUDSFileModel.InvoiceBiblosDocumentId);
                            }
                            if (invoiceProtocolFileModel != null && invoiceProtocolFileModel.InvoiceBiblosDocumentId != Guid.Empty)
                            {
                                await _documentClient.DetachDocumentAsync(invoiceProtocolFileModel.InvoiceBiblosDocumentId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Receivable invoice an error occured in moving invoice: {ex.Message}"), LogCategory.NotifyToEmailCategory);

                await _webAPIClient.PushCorrelatedNotificationAsync("Spostamento di fattura: Errore critico, contattare l'assistenza.",
                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowStatusError);
                _logger.WriteError(new LogMessage("WorkflowStartInvoiceMoveCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowInvoiceDeleteProtocolDeleteCompleteCallback ]

        private async Task WorkflowInvoiceDeleteProtocolDeleteCompleteCallback(IEventCompleteProtocolDelete evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowInvoiceDeleteProtocolDeleteCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying ProtocolDeleteComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);

                ProtocolBuildModel protocolBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying ProtocolDeleteComplete for IdWorkflowActivity {protocolBuildModel.IdWorkflowActivity}"), LogCategories);
                await _webAPIClient.PushCorrelatedNotificationAsync($"Protocollo di fattura <b>{protocolBuildModel.Protocol.Year}/{protocolBuildModel.Protocol.Number:0000000}</b> annullato correttamente.",
                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowInvoiceDeleteProtocolDeleteCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowInvoiceDeleteUDSDeleteCompleteCallback ]

        private async Task WorkflowInvoiceDeleteUDSDeleteCompleteCallback(IEventCompleteUDSDelete evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowInvoiceDeleteUDSDeleteCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying UDSDeleteComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);
                UDSBuildModel udsBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying UDSDeleteComplete for IdWorkflowActivity {udsBuildModel.IdWorkflowActivity} and IdUDS {udsBuildModel.UniqueId}"), LogCategories);
                DocumentUnit documentUnit = await _webAPIClient.GetDocumentUnitAsync(udsBuildModel.UniqueId);
                await _webAPIClient.PushCorrelatedNotificationAsync($"Archivio di fattura <b>{documentUnit?.Year}/{documentUnit?.Number:0000000}-{documentUnit.Subject}</b> annullato correttamente.",
                    ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowInvoiceDeleteUDSDeleteCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowReceivableInvoiceProtocolBuildCompleteCallback ]

        private async Task WorkflowReceivableInvoiceProtocolBuildCompleteCallback(IEventCompleteProtocolBuild evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowReceivableInvoiceProtocolBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying ProtocolBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);

                ProtocolBuildModel protocolBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying ProtocolBuildComplete for IdWorkflowActivity {protocolBuildModel.IdWorkflowActivity}"), LogCategories);
                if (_moduleConfiguration.AdEProtocolNotificationEnabled)
                {
                    await _webAPIClient.PushCorrelatedNotificationAsync($"Protocollo di fattura <b>{protocolBuildModel.Protocol.Year}/{protocolBuildModel.Protocol.Number.ToString("0000000")}</b> creato correttamente.",
                        ModuleConfigurationHelper.MODULE_NAME, evt.TenantId, evt.TenantAOOId, evt.TenantName, evt.CorrelationId, evt.Identity, NotificationType.EventWorkflowNotificationInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowReceivableInvoiceProtocolBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ WorkflowReceivableInvoiceUDSBuildCompleteCallback ]
        private async Task<T> RetryingPolicyActionAsync<T>(Func<Task<T>> func, int step = 1, int retry_tentative = 10)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyActionAsync : tentative {step}/{retry_tentative} in progress..."), LogCategories);
            if (step >= retry_tentative)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
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

        private async Task WorkflowReceivableInvoiceUDSBuildCompleteCallback(IEventCompleteUDSBuild evt, IDictionary<string, object> properties)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"WorkflowReceivableInvoiceUDSBuildCompleteCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for WorkflowInstanceId {evt.CorrelationId}"), LogCategories);

                UDSDocumentUnit protocolDocumentUnit;
                Protocol protocol;
                PECMail pecMail;
                Dictionary<string, string> metadataSdi = null;
                KeyValuePair<string, InvoiceFilePersistance>? persistanceInvoiceConfiguration = null;
                KeyValuePair<XMLModelKind, InvoiceConfiguration>? invoiceConfiguration = null;
                DirectoryInfo directoryInvoice;
                Content invoiceContent;
                BiblosDocument invoiceDocument;
                List<BiblosDocument> invoiceDocuments;
                string xmlContent;
                string controllerName;
                string mailBoxRecipient;
                Guid invoiceDocumentChain;
                Dictionary<int, Guid> documents;
                UDSBuildModel udsBuildModel = evt.ContentType.ContentTypeValue;
                _logger.WriteInfo(new LogMessage($"Notifying UDSBuildComplete for WorkflowActivityId {udsBuildModel.IdWorkflowActivity}"), LogCategories);
                protocolDocumentUnit = await RetryingPolicyActionAsync(async () =>
                {
                    UDSDocumentUnit result = (await _webAPIClient.GetUDSDocumentUnits(udsBuildModel.UniqueId, false, true)).FirstOrDefault(f => f.Relation.Environment == (int)DSWEnvironmentType.Protocol);
                    if (result == null)
                    {
                        throw new ArgumentNullException($"Received a wrong message invalid UDSId {udsBuildModel.UniqueId}: Not found UDS");
                    }
                    return result;
                });
                invoiceConfiguration = _moduleConfiguration.WorkflowConfigurations.SelectMany(f => f.Value.InvoiceTypes).SingleOrDefault(f => f.Value.UDSRepositoryName == protocolDocumentUnit.Repository.Name);
                if (!invoiceConfiguration.HasValue)
                {
                    throw new ArgumentNullException($"Received a wrong message UDSId {protocolDocumentUnit.IdUDS} {protocolDocumentUnit.Repository.Name}: Not related to valid invoice configuration environment");
                }
                _logger.WriteDebug(new LogMessage($"Found protocol {protocolDocumentUnit.Relation.UniqueId} associated to UDS {protocolDocumentUnit.IdUDS}"), LogCategories);
                if (!invoiceConfiguration.Value.Value.InvoicePersistanceConfigurations.Any() || !invoiceConfiguration.Value.Value.InvoicePersistanceConfigurations.Any(f => !string.IsNullOrEmpty(f.Value.FolderReceivedInvoice)))
                {
                    _logger.WriteInfo(new LogMessage($"UDSBuildComplete for WorkflowInstanceId {evt.CorrelationId} in event id {evt.Id} was ingored to not configurated FolderReceivedInvoice parameter"), LogCategories);
                    return;
                }
                protocol = (await _webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {protocolDocumentUnit.Relation.UniqueId}&$expand=AdvancedProtocol")).SingleOrDefault();
                if (protocol == null)
                {
                    throw new ArgumentNullException($"protocol not found for identification {protocolDocumentUnit.Relation.Year}/ {protocolDocumentUnit.Relation.Number}");
                }
                _logger.WriteDebug(new LogMessage($"Found protocol {protocol.UniqueId} associated to UDS {protocolDocumentUnit.Relation.UniqueId}"), LogCategories);
                mailBoxRecipient = string.Empty;
                pecMail = (await _webAPIClient.GetPECMailFromProtocol(protocol.UniqueId)).FirstOrDefault();
                if (pecMail != null)
                {
                    _logger.WriteDebug(new LogMessage($"Found PECMail {pecMail.UniqueId} associated to protocol {protocolDocumentUnit.Relation.UniqueId}"), LogCategories);
                    mailBoxRecipient = pecMail.PECMailBox.MailBoxRecipient;
                }

                persistanceInvoiceConfiguration = invoiceConfiguration.Value.Value.InvoicePersistanceConfigurations.LastOrDefault(f => string.IsNullOrEmpty(f.Key) || f.Key.Equals(mailBoxRecipient, StringComparison.InvariantCultureIgnoreCase));
                if (!persistanceInvoiceConfiguration.HasValue || persistanceInvoiceConfiguration.Value.Key == null)
                {
                    throw new ArgumentNullException($"Persistance invoice configuration not found for {mailBoxRecipient} MailBoxRecipient");
                }
                if (string.IsNullOrEmpty(persistanceInvoiceConfiguration.Value.Value.FolderReceivedInvoice) || !Directory.Exists(persistanceInvoiceConfiguration.Value.Value.FolderReceivedInvoice))
                {
                    throw new ArgumentNullException($"FolderReceivedInvoice parameter are empty or not exist {persistanceInvoiceConfiguration.Value.Value.FolderReceivedInvoice}");
                }
                controllerName = Utils.GetWebAPIControllerName(persistanceInvoiceConfiguration.Value.Value.UDSRepositoryName);
                documents = new Dictionary<int, Guid>();
                _logger.WriteDebug(new LogMessage($"Get UDS {protocolDocumentUnit.IdUDS} invoice metadatas"), LogCategories);
                Dictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, protocolDocumentUnit.IdUDS, documents);
                _logger.WriteDebug(new LogMessage($"Found {uds_metadatas != null} invoice metadatas {documents.ContainsKey(1)}"), LogCategories);

                xmlContent = string.Empty;
                invoiceDocumentChain = documents[1];
                _logger.WriteDebug(new LogMessage($"Get InvoiceDocument {invoiceDocumentChain}"), LogCategories);
                invoiceDocuments = await _documentClient.GetDocumentChildrenAsync(invoiceDocumentChain);
                _logger.WriteDebug(new LogMessage($"Found {invoiceDocuments?.Count} documents. Main document has identificaiton {invoiceDocuments?.SingleOrDefault(f => f.IsLatestVersion)?.IdDocument}"), LogCategories);
                invoiceDocument = invoiceDocuments.Single(f => f.IsLatestVersion);
                invoiceContent = await _documentClient.GetDocumentContentByIdAsync(invoiceDocument.IdDocument);
                if (new FileInfo(invoiceDocument.Name).Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlContent = EncodingUtil.GetEncoding(invoiceContent.Blob).GetString(invoiceContent.Blob);
                }
                if (new FileInfo(invoiceDocument.Name).Extension.Equals(".p7m", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlContent = EInvoiceHelper.TryGetInvoiceSignedContent(invoiceContent.Blob, (f, ex) => _logger.WriteWarning(new LogMessage(f), ex, LogCategories));
                }

                directoryInvoice = new DirectoryInfo(persistanceInvoiceConfiguration.Value.Value.FolderReceivedInvoice);
                string sessionId = string.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);
                if (_moduleConfiguration.PersistSDIMetadataEnabled)
                {
                    DateTime? date = uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi) ? (DateTime?)uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi] : null;
                    string destinationPath = Path.Combine(persistanceInvoiceConfiguration.Value.Value.FolderReceivedInvoice, $"{Path.GetFileNameWithoutExtension(protocol.DocumentCode.Replace(".p7m", string.Empty).Replace(".P7M", string.Empty))}_{sessionId}");
                    directoryInvoice = Directory.CreateDirectory(destinationPath);
                    metadataSdi = new Dictionary<string, string>
                        {
                            { "Esito", "Consegna" },
                            { "Descrizione esito", string.Empty },
                            { "Data notifica", !date.HasValue ? null : DateTime.SpecifyKind(date.Value.ToUniversalTime(), DateTimeKind.Utc).ToString("o") },
                            { "Identificazione SDI", uds_metadatas.ContainsKey(UDSEInvoiceHelper.UDSMetadata_IdentificativoSdi) ? uds_metadatas[UDSEInvoiceHelper.UDSMetadata_IdentificativoSdi] as string : null },
                            { "Anno protocollo", protocolDocumentUnit.Relation.Year.ToString() },
                            { "Numero protocollo", protocolDocumentUnit.Relation.Number.ToString("0000000") }
                        };
                    File.WriteAllText(Path.Combine(directoryInvoice.FullName, $"{directoryInvoice.Name.Replace($"_{sessionId}", string.Empty)}.json"), JsonConvert.SerializeObject(metadataSdi));
                }
                string filePath = Path.Combine(directoryInvoice.FullName, $"{invoiceDocument.Name.Replace(".p7m", string.Empty).Replace(".P7M", string.Empty)}");
                File.WriteAllText(filePath, xmlContent);
                _logger.WriteInfo(new LogMessage($"Invoice files has been successfully stored to folder {directoryInvoice.FullName}"), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Receivable invoice an error occured persisting filesystem metadata: {ex.Message}"), LogCategory.NotifyToEmailCategory);

                _logger.WriteError(new LogMessage("WorkflowReceivableInvoiceUDSBuildCompleteCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        #endregion

        #region [ UpdateInvoiceMetadataAsync ]
        private async Task UpdateInvoiceMetadataAsync(InvoiceMetadata invoiceMetadata)
        {
            string controllerName = Utils.GetWebAPIControllerName(invoiceMetadata.UDSRepositoryName);
            Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
            _logger.WriteDebug(new LogMessage($"Finding UDS invoice using {invoiceMetadata.InvoiceFileNameXML} filename"), LogCategories);
            IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDSByInvoiceFilename(controllerName, invoiceMetadata.InvoiceFileNameXML, false, documents);
            if (uds_metadatas == null || !uds_metadatas.Any())
            {
                throw new ArgumentNullException($"Wrong invoice metadata {invoiceMetadata.MetadataFileName} {invoiceMetadata.InvoiceFileNameXML}/{invoiceMetadata.InvoiceFileNameP7M} not related to valid UDSId");
            }

            UDSRepository udsRepository = (await _webAPIClient.GetUDSRepository(invoiceMetadata.UDSRepositoryName)).Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
            if (udsRepository == null)
            {
                throw new ArgumentNullException($"udsRepository {invoiceMetadata.UDSRepositoryName} not found");
            }
            WorkflowConfigurationModel workflowConfiguration = _moduleConfiguration.WorkflowConfigurations.Select(f => f.Value).SingleOrDefault(f => f.InvoiceTypes.Any(x => x.Value.UDSRepositoryName == udsRepository.Name));
            uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceFiscalMetadatas(invoiceMetadata.InvoiceFiscalMetadata, uds_metadatas);
            Guid idUDS = Guid.Parse(uds_metadatas[UDSEInvoiceHelper.UDSMetadata_UDSId] as string);
            _logger.WriteInfo(new LogMessage($"UDS {idUDS} updating invoice metadata"), LogCategories);
            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(idUDS);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(idUDS);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(idUDS);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(idUDS);
            ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(idUDS, false, false);
            UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(udsRepository, idUDS, uds_metadatas, documents, udsRoles, udsContacts, udsMessages, udsPECMails,
                udsDocumentUnits, null, _username);

            CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, workflowConfiguration.TenantAOOId, new IdentityContext(_username), udsBuildModel);
            await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
            _logger.WriteInfo(new LogMessage($"Updating metadata invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
        }
        #endregion

        #endregion
    }
}
