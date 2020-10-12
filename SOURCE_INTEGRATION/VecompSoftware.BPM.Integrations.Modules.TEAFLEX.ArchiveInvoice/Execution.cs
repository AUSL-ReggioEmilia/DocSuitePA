using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.TEAFLEX.ArchiveInvoice.Configurations;
using VecompSoftware.BPM.Integrations.Modules.TEAFLEX.ArchiveInvoice.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.EInvoice.EInvoice1_2;
using VecompSoftware.Helpers.EInvoice.UDS.Models;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Protocols;
using UDSEInvoiceHelper = VecompSoftware.Helpers.EInvoice.UDS.EInvoice1_2.EInvoiceHelper;

namespace VecompSoftware.BPM.Integrations.Modules.TEAFLEX.ArchiveInvoice
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IDocumentClient _documentClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private UDSRepository _receivableInvoiceUDSName;
        private readonly IdentityContext _identityContext = null;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        private const string LOOKIN_EXTENSION_PDF = "*.pdf";
        private Location _workflowLocation;

        private const string METADATA_DATAFATTURA = "DataFattura";
        private const string METADATA_NUMEROFATTURA = "NumeroFattura";
        private const string METADATA_ANNOIVA = "AnnoIVA";
        private const string METADATA_SEZIONALENUMERICO = "SezionaleNumerico";
        private const string METADATA_PROTOCOLLOIVA = "ProtocolloIVA";
        private const string METADATA_DATAIVA = "DataIVA";
        private const string METADATA_PARTITAIVA = "PartitaIVA";
        private const string METADATA_CODICEFISCALE = "CodiceFiscale";
        private static readonly string VALUE_TIPOFATTURA = JsonConvert.SerializeObject(new List<string>() { "Fattura" });
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _documentClient = documentClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
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
                _logger.WriteError(new LogMessage("TEAFLEX.ArchiveInvoice -> Critical error in costruction module"), ex, LogCategories);
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

                _logger.WriteInfo(new LogMessage($"Looking attachments invoice in folder {_moduleConfiguration.FolderLookingAttachmentInvoice}"), LogCategories);
                IEnumerable<FileInfo> resultFiles = new DirectoryInfo(_moduleConfiguration.FolderLookingAttachmentInvoice).EnumerateFiles(LOOKIN_EXTENSION_PDF, SearchOption.AllDirectories).ToList();
                string movedPath;
                string workingPath = string.Empty;
                FileInfo currentFileInfo = null;
                foreach (FileInfo fileInfo in resultFiles)
                {
                    try
                    {
                        currentFileInfo = fileInfo;
                        _logger.WriteInfo(new LogMessage($"Waiting {_threadWaiting.TotalSeconds} seconds ...."), LogCategories);
                        Task.Delay(_threadWaiting).Wait();
                        _logger.WriteInfo(new LogMessage($"Evaluating {currentFileInfo.FullName} ...."), LogCategories);
                        workingPath = Path.Combine(_moduleConfiguration.FolderWorkingAttachmentInvoice, currentFileInfo.Name);
                        if (File.Exists(workingPath))
                        {
                            File.Delete(workingPath);
                        }
                        File.Move(currentFileInfo.FullName, workingPath);
                        _logger.WriteInfo(new LogMessage($"{fileInfo.FullName} has been moved to {workingPath}"), LogCategories);
                        currentFileInfo = new FileInfo(workingPath);
                        UpdateReceivableInvoiceAsync(currentFileInfo).Wait();
                        movedPath = Path.Combine(_moduleConfiguration.FolderBackupAttachmentInvoice, currentFileInfo.Name);
                        if (Directory.Exists(Path.Combine(_moduleConfiguration.FolderBackupAttachmentInvoice)))
                        {
                            if (File.Exists(movedPath))
                            {
                                File.Delete(movedPath);
                            }
                            File.Move(currentFileInfo.FullName, movedPath);
                            _logger.WriteInfo(new LogMessage($"{currentFileInfo.Name} has been moved to {movedPath}"), LogCategories);
                        }
                        else
                        {
                            File.Delete(currentFileInfo.FullName);
                        }
                        _logger.WriteInfo(new LogMessage($"{currentFileInfo.FullName} has been successfully completed."), LogCategories);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"Error during {currentFileInfo.FullName} evaluation. This file was skipped"), ex, LogCategories);
                        if (Directory.Exists(_moduleConfiguration.FolderRejectedAttachmentInvoice))
                        {
                            File.Move(currentFileInfo.FullName, _moduleConfiguration.FolderRejectedAttachmentInvoice);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.TEAFLEX.ArchiveInvoice -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> TEAFLEX.ArchiveInvoice"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreateProtocol>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartArchiveInvoiceSubscription, WorkflowStartArchiveInvoiceCallback));

                _receivableInvoiceUDSName = _webAPIClient.GetUDSRepository(_moduleConfiguration.ReceivableInvoiceUDSName).Result.Last(f => f.Status == DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed);
                if (_receivableInvoiceUDSName == null)
                {
                    throw new ArgumentException($"UDSRepository {_moduleConfiguration.ReceivableInvoiceUDSName} not found");
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

        private async Task WorkflowStartArchiveInvoiceCallback(IEventCreateProtocol evt, IDictionary<string, object> properties)
        {
            try
            {
                Protocol protocol = evt.ContentType.ContentTypeValue;
                _logger.WriteDebug(new LogMessage($"WorkflowStartArchiveInvoiceCallback -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"Evaluate Protocol id {protocol.UniqueId}"), LogCategories);

                DocumentUnit documentUnit = await _webAPIClient.GetDocumentUnitAsync(protocol.UniqueId);
                if (documentUnit == null)
                {
                    throw new ArgumentException($"Document unit {protocol.UniqueId} not found");
                }

                ICollection<DocumentUnitChain> documentUnitChains = await _webAPIClient.GetDocumentUnitChainsAsync(documentUnit.UniqueId);
                if (!HasMainDocument(documentUnitChains))
                {
                    throw new ArgumentException($"Main document for protocol {protocol.UniqueId} not defined");
                }

                Guid mainChain = documentUnitChains.Single(s => s.ChainType == DocSuiteWeb.Entity.DocumentUnits.ChainType.MainChain).IdArchiveChain;
                ArchiveDocument mainDocument = (await _documentClient.GetChildrenAsync(mainChain)).SingleOrDefault();
                if (mainDocument == null)
                {
                    throw new ArgumentException($"Main document for protocol {protocol.UniqueId} not found");
                }

                byte[] mainDocumentContent = await _documentClient.GetDocumentStreamAsync(mainDocument.IdDocument);
                ArchiveDocument archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
                {
                    Archive = _workflowLocation.ProtocolArchive,
                    ContentStream = mainDocumentContent,
                    Name = mainDocument.Name,
                });
                InvoiceFileModel mainInvoiceFileModel = new InvoiceFileModel()
                {
                    InvoiceBiblosDocumentId = archiveDocument.IdDocument,
                    InvoiceFilename = mainDocument.Name
                };

                _logger.WriteDebug(new LogMessage("Read invoice metadatas from protocol document"), LogCategories);
                IDictionary<string, object> invoice_metadatas = GetInvoiceMetadatasFromDocument(mainDocument);
                foreach (KeyValuePair<string, object> invoice_metadata in invoice_metadatas)
                {
                    _logger.WriteDebug(new LogMessage($"Invoice metadata [{invoice_metadata.Key}] with value [{invoice_metadata.Value?.ToString()}]"), LogCategories);
                }

                IDictionary<string, byte[]> invoiceAttachments = new Dictionary<string, byte[]>();
                if (HasAttachments(documentUnitChains))
                {
                    Guid attachmentsChain = documentUnitChains.Single(s => s.ChainType == DocSuiteWeb.Entity.DocumentUnits.ChainType.AttachmentsChain).IdArchiveChain;
                    ICollection<ArchiveDocument> attachments = await _documentClient.GetChildrenAsync(attachmentsChain);
                    byte[] attachmentContent;
                    foreach (ArchiveDocument attachment in attachments)
                    {
                        attachmentContent = await _documentClient.GetDocumentStreamAsync(attachment.IdDocument);
                        invoiceAttachments.Add(attachment.Name, attachmentContent);
                    }
                }

                Contact protocolContact = (await _webAPIClient.GetProtocolContactSendersAsync(protocol)).FirstOrDefault();
                ProtocolContactManual protocolContactManual = null;
                string description = string.Empty;
                string pivacf = string.Empty;
                if (protocolContact == null)
                {
                    protocolContactManual = (await _webAPIClient.GetProtocolContactManualSendersAsync(protocol)).FirstOrDefault();
                    if (protocolContactManual == null)
                    {
                        throw new ArgumentException($"Sender for protocol {protocol.UniqueId} not found");
                    }
                    description = protocolContactManual.Description.Replace('|', ' ');
                    pivacf = protocolContactManual.FiscalCode;
                }
                else
                {
                    description = protocolContact.Description.Replace('|', ' ');
                    pivacf = protocolContact.FiscalCode;
                }

                if (string.IsNullOrEmpty(pivacf) && mainDocument.Metadata.Any(x => (x.Key == METADATA_PARTITAIVA || x.Key == METADATA_CODICEFISCALE) && x.Value != null))
                {
                    pivacf = mainDocument.Metadata.Single(x => (x.Key == METADATA_PARTITAIVA || x.Key == METADATA_CODICEFISCALE)).Value.ToString();
                }

                invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_Denominazione, description);
                invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_Pivacf, pivacf);
                invoice_metadatas.Add(UDSEInvoiceHelper.UDSMetadata_TipoFattura, VALUE_TIPOFATTURA);

                ICollection<RoleModel> protocolRoles = protocol.ProtocolRoles?.Select(role => new RoleModel()
                {
                    IdRole = role.Role.EntityShortId,
                    Name = role.Role.Name,
                    TenantId = role.Role.TenantId,
                    UniqueId = role.Role.UniqueId,
                    RoleLabel = "Autorizzazione",
                    FullIncrementalPath = role.Role.FullIncrementalPath
                }).ToList();

                _logger.WriteDebug(new LogMessage("Create UDS build model for insert"), LogCategories);
                UDSBuildModel buildModel = await CreateUDSBuildModelAsync(mainInvoiceFileModel, protocolContact, protocolRoles, protocol, invoice_metadatas, invoiceAttachments);
                CommandInsertUDSData commandInsertUDSData = new CommandInsertUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _moduleConfiguration.TenantAOOId, _identityContext, buildModel);

                await _webAPIClient.SendCommandAsync(commandInsertUDSData);
                _logger.WriteInfo(new LogMessage($"Insert invoice command {commandInsertUDSData.Id} has been sended"), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowStartReceivableInvoiceCallback -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private bool HasMainDocument(ICollection<DocumentUnitChain> documentUnitChains)
        {
            return documentUnitChains.Any(x => x.ChainType == DocSuiteWeb.Entity.DocumentUnits.ChainType.MainChain);
        }

        private bool HasAttachments(ICollection<DocumentUnitChain> documentUnitChains)
        {
            return documentUnitChains.Any(x => x.ChainType == DocSuiteWeb.Entity.DocumentUnits.ChainType.AttachmentsChain);
        }

        private IDictionary<string, object> GetInvoiceMetadatasFromDocument(ArchiveDocument document)
        {
            IDictionary<string, object> invoice_metadatas = new Dictionary<string, object>
            {
                { UDSEInvoiceHelper.UDSMetadata_DataFattura, document.Metadata.Where(s => s.Key == METADATA_DATAFATTURA).Select(s => DateTimeOffset.Parse(s.Value.ToString())).SingleOrDefault() },
                { UDSEInvoiceHelper.UDSMetadata_NumeroFattura, document.Metadata.Where(s => s.Key == METADATA_NUMEROFATTURA).Select(s => s.Value.ToString()).SingleOrDefault() },
                { UDSEInvoiceHelper.UDSMetadata_AnnoIva, document.Metadata.Where(s => s.Key == METADATA_ANNOIVA).Select(s => int.Parse(s.Value.ToString())).SingleOrDefault() },
                { UDSEInvoiceHelper.UDSMetadata_SezionaleIva, document.Metadata.Where(s => s.Key == METADATA_SEZIONALENUMERICO).Select(s => s.Value.ToString()).SingleOrDefault() },
                { UDSEInvoiceHelper.UDSMetadata_ProtocolloIva, document.Metadata.Where(s => s.Key == METADATA_PROTOCOLLOIVA).Select(s => int.Parse(s.Value.ToString())).SingleOrDefault() },
                { UDSEInvoiceHelper.UDSMetadata_DataIva, document.Metadata.Where(s => s.Key == METADATA_DATAIVA).Select(s => DateTime.Parse(s.Value.ToString())).SingleOrDefault() }
            };
            return invoice_metadatas;
        }

        private async Task<UDSBuildModel> CreateUDSBuildModelAsync(InvoiceFileModel invoiceFileModel, Contact contact, ICollection<RoleModel> roleModels,
            Protocol protocol, IDictionary<string, object> invoice_metadatas, IDictionary<string, byte[]> invoiceAttachments)
        {
            Guid udsID = Guid.NewGuid();
            Guid correlationId = Guid.NewGuid();
            UDSModel model = UDSModel.LoadXml(_receivableInvoiceUDSName.ModuleXML);
            model.Model.UDSId = udsID.ToString();
            model.Model.Subject.Value = $"{invoice_metadatas[EInvoiceHelper.Metadata_Denominazione]} - Fattura passiva n° {invoice_metadatas[EInvoiceHelper.Metadata_NumeroFattura]} del {((DateTimeOffset)invoice_metadatas[EInvoiceHelper.Metadata_DataFattura]).LocalDateTime.ToShortDateString()}";
            IDictionary<string, object> uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceMetadatas(invoice_metadatas, protocol.RegistrationDate.DateTime);
            ReceivableInvoiceMetadata receivableInvoiceMetadata = new ReceivableInvoiceMetadata
            {
                DateVAT = (DateTime)invoice_metadatas[UDSEInvoiceHelper.UDSMetadata_DataIva],
                ProtocolNumberVAT = (int)invoice_metadatas[UDSEInvoiceHelper.UDSMetadata_ProtocolloIva],
                SectionalVAT = (string)invoice_metadatas[UDSEInvoiceHelper.UDSMetadata_SezionaleIva],
                YearVAT = (int)invoice_metadatas[UDSEInvoiceHelper.UDSMetadata_AnnoIva]
            };
            uds_metadatas = UDSEInvoiceHelper.MappingReceivableInvoiceFiscalMetadatas(receivableInvoiceMetadata, uds_metadatas);
            uds_metadatas[UDSEInvoiceHelper.UDSMetadata_StatoFattura] = UDSEInvoiceHelper.UDSInvoiceStatusAccounted;
            uds_metadatas[UDSEInvoiceHelper.UDSMetadata_DataRicezioneSdi] = null;
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
            if (roleModels != null)
            {
                model = UDSEInvoiceHelper.FillAuthorizations(model, roleModels.ToList());
            }

            UDSBuildModel udsBuildModel = new UDSBuildModel(model.SerializeToXml())
            {
                WorkflowActions = new List<IWorkflowAction>(),
                Documents = new List<UDSDocumentModel>(),
                Roles = roleModels ?? new List<RoleModel>(),
                Users = new List<UserModel>(),
                UniqueId = correlationId,
                UDSRepository = new UDSRepositoryModel(_receivableInvoiceUDSName.UniqueId)
                {
                    DSWEnvironment = _receivableInvoiceUDSName.DSWEnvironment,
                    Name = _receivableInvoiceUDSName.Name
                },
                Subject = model.Model.Subject.Value
            };

            udsBuildModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                new DocumentUnitModel() { UniqueId = protocol.UniqueId, Environment = (int)DocSuiteWeb.Model.Entities.Commons.DSWEnvironmentType.Protocol },
                new DocumentUnitModel() { UniqueId = udsID, Environment = _receivableInvoiceUDSName.DSWEnvironment, IdUDSRepository = _receivableInvoiceUDSName.UniqueId }));
            return udsBuildModel;
        }

        private async Task UpdateReceivableInvoiceAsync(FileInfo fileInfo)
        {
            _logger.WriteDebug(new LogMessage($"LookingA invoice UDS using filename {fileInfo.Name}"), LogCategories);
            string controllerName = Utils.GetWebAPIControllerName(_receivableInvoiceUDSName.Name);
            Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
            IDictionary<string, object> uds_metadatas = await _webAPIClient.GetUDSByInvoiceFilename(controllerName, $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}.xml", false, documents);
            if (uds_metadatas == null || !uds_metadatas.Any())
            {
                throw new ArgumentNullException($"Invoice not found with filename {fileInfo.Name}");
            }

            Guid idUDS = Guid.Parse(uds_metadatas[UDSEInvoiceHelper.UDSMetadata_UDSId] as string);

            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(idUDS);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(idUDS);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(idUDS);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(idUDS);
            ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(idUDS, false, false);
            ArchiveDocument archiveDocument = await _documentClient.InsertDocumentAsync(new ArchiveDocument()
            {
                Archive = _workflowLocation.ProtocolArchive,
                ContentStream = File.ReadAllBytes(fileInfo.FullName),
                Name = fileInfo.Name,
            });
            List<InvoiceFileModel> invoiceFiles = new List<InvoiceFileModel>()
            {
                new InvoiceFileModel()
                {
                    InvoiceBiblosDocumentId = archiveDocument.IdDocument,
                    InvoiceFilename = fileInfo.Name
                }
            };
            UDSBuildModel udsBuildModel = UDSEInvoiceHelper.PrepareUpdateUDSBuildModel(_receivableInvoiceUDSName, idUDS, uds_metadatas, documents, udsRoles, udsContacts,
                udsMessages, udsPECMails, udsDocumentUnits, invoiceFiles, _identityContext.User);
            CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _moduleConfiguration.TenantAOOId, _identityContext, udsBuildModel);
            await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
            _logger.WriteInfo(new LogMessage($"Updating invoice {commandUpdateUDSData.Id} has been sended"), LogCategories);
        }

        #endregion
    }
}
