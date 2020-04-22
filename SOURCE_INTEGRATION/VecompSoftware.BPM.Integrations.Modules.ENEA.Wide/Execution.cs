using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Clients;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Configurations;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Models;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.ProtocollaService;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.TipoDocumentoService;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Services.Command.CQRS.Events.Models.UDS;
using BiblosDocument = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Document;
using DocumentUnit = VecompSoftware.DocSuiteWeb.Entity.DocumentUnits.DocumentUnit;
using UDSBuildModel = VecompSoftware.DocSuiteWeb.Model.Entities.UDS.UDSBuildModel;
using UDSRepositoryModel = VecompSoftware.DocSuiteWeb.Model.Entities.UDS.UDSRepositoryModel;

namespace VecompSoftware.BPM.Integrations.Modules.ENEA.Wide
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private const string DOCUMENT_TYPE_INGRESSO = "Ingresso";
        private const string DOCUMENT_TYPE_TRA_UFFICI = "Tra uffici";
        private const string DOCUMENT_TYPE_USCITA = "Uscita";

        private const string ESITO_WIDE_SUCCESS = "Protocollato";
        private const string ESITO_SUCCESS = "Protocollato con successo il {0}";
        private const string ESITO_WIDE_ERROR = "In errore";

        private const string META_KEY_PROTOCOL_NR = "NumeroProtocolloWide";
        private const string META_KEY_ESITO_WIDE = "EsitoWide";
        private const string META_KEY_ESITO = "Esito";

        private const string SIGNATURE = @"<Label>
                                              <Text>{0} Pagina (pagina) di (pagine)</Text>
                                              <Font Face=""Arial"" Size=""12"" Style=""Bold"" />
                                          </Label>";
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private readonly IDocumentClient _documentClient;
        private readonly WideClient _wideClient;
        private readonly FTPClient _ftpClient;
        private readonly IdentityContext _identityContext = null;
        private readonly IStampaConformeClient _stampaConformeClient;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient, IDocumentClient documentClient, IStampaConformeClient stampaConformeClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _serviceBusClient = serviceBusClient;
                _stampaConformeClient = stampaConformeClient;
                _documentClient = documentClient;
                string username = "anonymous";
                _needInitializeModule = true;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);

                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _wideClient = new WideClient(webAPIClient, logger);
                _ftpClient = new FTPClient(_documentClient, _logger);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.ENEA.WIDE -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("VecompSoftware.BPM.Integrations.Modules.ENEA.Wide -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }
        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> ENEA.Wide"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCQRSCreateUDSData>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowStartWideEneaProtocolSubscription, EventUDSDataCreatedCallbackAsync));

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

        private async Task EventUDSDataCreatedCallbackAsync(IEventCQRSCreateUDSData evt)
        {
            try
            {
                _logger.WriteDebug(new LogMessage($"EventUDSDataCreatedCallbackAsync -> evaluate event id {evt.Id}"), LogCategories);
                _logger.WriteInfo(new LogMessage($"Notifying new Wide UDS {evt.ContentType?.ContentTypeValue?.UDSId}"), LogCategories);

                //CallTipoDocService 
                //TipoDocDocumentoResponse tipoDocResponse = _wideClient.TipoDocDocumento();
                string tipoDocumento = "Wide";//tipoDocResponse.Documento.FirstOrDefault()?.TipoDocumento ?? string.Empty;
                string wideProtocolNumber = string.Empty;
                bool hasError = false;
                DocumentUnit documentUnit = evt.DocumentUnit;
                UDSRepository udsRepository = (await _webAPIClient.GetUDSRepository(documentUnit.UDSRepository.UniqueId).ConfigureAwait(false)).SingleOrDefault();

                //Prelevare i metadati dell'archivio UDS
                string controllerName = Utils.GetWebAPIControllerName(udsRepository.Name);
                Dictionary<int, Guid> uds_documents = new Dictionary<int, Guid>();
                Dictionary<string, object> uds_metadatas = await _webAPIClient.GetUDS(controllerName, documentUnit.UniqueId, uds_documents).ConfigureAwait(false);
                _logger.WriteDebug(new LogMessage($"Found {uds_metadatas != null} uds main document {uds_documents.ContainsKey(1)} and attachments {uds_documents.ContainsKey(2)}"), LogCategories);
                if (uds_metadatas.ContainsKey(META_KEY_PROTOCOL_NR) && !string.IsNullOrEmpty(uds_metadatas[META_KEY_PROTOCOL_NR] as string))
                {
                    _logger.WriteInfo(new LogMessage($"UDS already has wide potocol number {uds_metadatas[META_KEY_PROTOCOL_NR]}"), LogCategories);
                    wideProtocolNumber = uds_metadatas[META_KEY_PROTOCOL_NR].ToString();
                }
                try
                {
                    List<BiblosDocument> biblosAttachments = new List<BiblosDocument>();
                    //Prelevare i documenti dell'archivio UDS
                    Guid documentChain = uds_documents[1];
                    _logger.WriteDebug(new LogMessage($"Get main document {documentChain}"), LogCategories);
                    List<BiblosDocument> biblosDocuments = await _documentClient.GetDocumentChildrenAsync(documentChain).ConfigureAwait(false);
                    BiblosDocument mainBiblosDocument = biblosDocuments.Single();

                    _logger.WriteDebug(new LogMessage($"Found main document {mainBiblosDocument.IdDocument}"), LogCategories);

                    if (uds_documents.ContainsKey(2))
                    {
                        documentChain = uds_documents[2];
                        _logger.WriteDebug(new LogMessage($"Get attachments for document id {documentChain}"), LogCategories);
                        biblosAttachments = await _documentClient.GetDocumentChildrenAsync(documentChain).ConfigureAwait(false);
                    }

                    //Call ProtocolloService
                    if (string.IsNullOrEmpty(wideProtocolNumber))
                    {
                        Registro_Type registro_Type = GetRegistroType(uds_metadatas);
                        _logger.WriteDebug(new LogMessage($"Calling protocol wide {documentUnit.UniqueId} ..."), LogCategories);
                        protDocumentoResponse wideProtocol = await _wideClient.ProtocolloRegistrazioneAsync(documentUnit, tipoDocumento, documentUnit.UniqueId, registro_Type).ConfigureAwait(false);
                        uds_metadatas[META_KEY_PROTOCOL_NR] = wideProtocol.NumeroProtocollo;
                        wideProtocolNumber = wideProtocol.NumeroProtocollo;
                    }

                    string newSignature = $"Protocollo WIDE {wideProtocolNumber} del {DateTime.Now.ToShortDateString()}";
                    _logger.WriteDebug(new LogMessage($"Updating main document signature to {newSignature} ..."), LogCategories);
                    ArchiveDocument mainDocument = await _documentClient.GetInfoDocumentAsync(mainBiblosDocument.IdDocument);
                    mainDocument.Metadata[_documentClient.SIGNATURE_ATTRIBUTE_NAME] = newSignature;
                    Guid updatedDocumentId = await _documentClient.UpdateDocumentAsync(mainDocument.Archive, mainDocument.IdDocument, mainDocument.Metadata);
                    _logger.WriteInfo(new LogMessage($"main document {mainDocument.IdDocument} has been successfully updated. New Id was setted to {updatedDocumentId}"), LogCategories);

                    Services.BiblosDS.DocumentService.Content content = await _documentClient.GetDocumentContentByIdAsync(mainBiblosDocument.IdDocument).ConfigureAwait(false);
                    _logger.WriteDebug(new LogMessage("Converting main document to PDF/A format ..."), LogCategories);
                    byte[] contentPDFA = await _stampaConformeClient.ConvertToPDFAAsync(content.Blob, string.Format(SIGNATURE, newSignature)).ConfigureAwait(false);

                    string rootFolderName = string.Concat(wideProtocolNumber.Split(Path.GetInvalidFileNameChars()));

                    if (!_ftpClient.DirectoryExist(rootFolderName))
                    {
                        _ftpClient.CreateDirectory(rootFolderName);
                    }

                    #region [ Main document ] 
                    _logger.WriteDebug(new LogMessage($"Saving main file to ftp {rootFolderName} folder ..."), LogCategories);
                    await _ftpClient.StoreFileAsync(rootFolderName, mainBiblosDocument);

                    _logger.WriteDebug(new LogMessage($"Calling main document to protocol {wideProtocolNumber} wide ..."), LogCategories);
                    _wideClient.InserimentoAllegati(new List<BiblosDocument>() { mainBiblosDocument }, InserisciAllegatiService.Tipologia_Type.O, rootFolderName, wideProtocolNumber);

                    #endregion

                    #region [ Attachments documents ]
                    if (biblosAttachments.Any())
                    {
                        if (!_ftpClient.DirectoryExist(rootFolderName))
                        {
                            _ftpClient.CreateDirectory(rootFolderName);
                        }

                        _logger.WriteDebug(new LogMessage($"Saving attachments files to ftp {rootFolderName} folder ..."), LogCategories);
                        foreach (BiblosDocument document in biblosAttachments)
                        {
                            await _ftpClient.StoreFileAsync(rootFolderName, document);
                        }

                        _logger.WriteDebug(new LogMessage($"Calling attachments to protocol {wideProtocolNumber} wide ..."), LogCategories);
                        _wideClient.InserimentoAllegati(biblosAttachments, InserisciAllegatiService.Tipologia_Type.A, rootFolderName, wideProtocolNumber);
                    }

                    #endregion

                    #region [ Signs documents ]
                    if (!_ftpClient.DirectoryExist(rootFolderName))
                    {
                        _ftpClient.CreateDirectory(rootFolderName);
                    }

                    _logger.WriteDebug(new LogMessage($"Saving sign files to ftp {rootFolderName} folder ..."), LogCategories);
                    string ext = mainBiblosDocument.Name.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) ? string.Empty : ".pdf";
                    mainBiblosDocument.Name = $"cc_{mainBiblosDocument.Name}{ext}";
                    _logger.WriteDebug(new LogMessage($"Saving sign document {mainBiblosDocument.Name} converted in PDF/A standard ..."), LogCategories);
                    _ftpClient.StoreFile(rootFolderName, mainBiblosDocument.Name, contentPDFA);

                    _logger.WriteDebug(new LogMessage($"Calling sign to protocol {wideProtocolNumber} wide ..."), LogCategories);
                    _wideClient.InserimentoAllegati(new List<BiblosDocument>() { mainBiblosDocument }, InserisciAllegatiService.Tipologia_Type.S, rootFolderName, wideProtocolNumber);

                    #endregion
                    
                    uds_metadatas[META_KEY_ESITO_WIDE] = JsonConvert.SerializeObject(new List<string>() { ESITO_WIDE_SUCCESS });
                    uds_metadatas[META_KEY_ESITO] = string.Format(ESITO_SUCCESS, DateTime.Now.ToLongDateString());
                }
                catch (Exception ex)
                {
                    _logger.WriteWarning(new LogMessage($"ProtocolWide response an error : {ex.Message}"), ex, LogCategories);
                    uds_metadatas[META_KEY_ESITO_WIDE] = JsonConvert.SerializeObject(new List<string>() { ESITO_WIDE_ERROR });
                    uds_metadatas[META_KEY_ESITO] = $"{ex.Message}";
                    hasError = true;
                }

                await UpdateDocSuiteUDS(documentUnit, udsRepository, uds_documents, uds_metadatas);
                if (hasError)
                {
                    throw new Exception("Manual raise exception to set message into deadletter");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(ex, ModuleConfigurationHelper.JsonSerializerSettings)), LogCategories);
                _logger.WriteError(new LogMessage($"EventUDSDataCreatedCallbackAsync -> error executing event with id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task UpdateDocSuiteUDS(DocumentUnit documentUnit, UDSRepository udsRepository, Dictionary<int, Guid> uds_documents, Dictionary<string, object> uds_metadatas)
        {
            ICollection<UDSRole> udsRoles = await _webAPIClient.GetUDSRoles(documentUnit.UniqueId);
            ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(documentUnit.UniqueId);
            ICollection<UDSMessage> udsMessages = await _webAPIClient.GetUDSMessages(documentUnit.UniqueId);
            ICollection<UDSPECMail> udsPECMails = await _webAPIClient.GetUDSPECMails(documentUnit.UniqueId);
            ICollection<UDSDocumentUnit> udsDocumentUnits = await _webAPIClient.GetUDSDocumentUnits(documentUnit.UniqueId, false, false);
            UDSBuildModel udsBuildModel = PrepareUpdateUDSBuildModel(udsRepository, documentUnit.UniqueId, uds_metadatas, uds_documents, udsRoles, udsContacts, udsMessages, udsPECMails,
                udsDocumentUnits, _identityContext.User);

            CommandUpdateUDSData commandUpdateUDSData = new CommandUpdateUDSData(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _identityContext, udsBuildModel);
            await _webAPIClient.SendCommandAsync(commandUpdateUDSData);
            _logger.WriteInfo(new LogMessage($"Updating metadata {commandUpdateUDSData.Id} has been sended"), LogCategories);
        }

        public static UDSBuildModel PrepareUpdateUDSBuildModel(UDSRepository udsRepository, Guid IdUDS, IDictionary<string, object> uds_metadatas, Dictionary<int, Guid> documents,
            ICollection<UDSRole> udsRoles, ICollection<UDSContact> udsContacts, ICollection<UDSMessage> udsMessages, ICollection<UDSPECMail> udsPECMails,
            ICollection<UDSDocumentUnit> udsDocumentUnits, string registrationUser/*, string mainPDFAfileName, byte[] mainPDFAcontent*/)
        {
            UDSModel model = UDSModel.LoadXml(udsRepository.ModuleXML);

            foreach (Section metadata in model.Model.Metadata)
            {
                foreach (FieldBaseType item in metadata.Items)
                {
                    UDSModelField udsField = new UDSModelField(item);
                    udsField.Value = uds_metadatas.Single(f => f.Key == item.ColumnName).Value;
                }
            }

            model.Model.Title = udsRepository?.Name;
            model.Model.Subject.Value = uds_metadatas["_subject"].ToString();
            model.Model.Category.IdCategory = uds_metadatas["IdCategory"].ToString();

            ICollection<Guid> mainDocuments = documents.Where(x => x.Key == (int)UDSDocumentType.Main).Select(s => s.Value).ToList();
            ICollection<Guid> attachmentsDocuments = documents.Where(x => x.Key == (int)UDSDocumentType.Attachment).Select(s => s.Value).ToList();
            ICollection<Guid> annexedDocuments = documents.Where(x => x.Key == (int)UDSDocumentType.Annexed).Select(s => s.Value).ToList();
            ICollection<Guid> dematerialisationDocuments = documents.Where(x => x.Key == (int)UDSDocumentType.Dematerialisation).Select(s => s.Value).ToList();

            model.FillDocuments(mainDocuments);
            model.FillDocumentAttachments(attachmentsDocuments);
            model.FillDocumentAnnexed(annexedDocuments);
            model.FillDocumentDematerialisation(dematerialisationDocuments);
            //if (!string.IsNullOrEmpty(mainPDFAfileName) && mainPDFAcontent != null && mainPDFAcontent.Length > 0)
            //{
            //    List<DocumentInstance> documentInstances = new List<DocumentInstance>(model.Model.Documents.DocumentAttachment.Instances);
            //    documentInstances.Add(new DocumentInstance()
            //    {
            //        DocumentContent = Convert.ToBase64String(mainPDFAcontent),
            //        DocumentName = mainPDFAfileName
            //    });
            //    model.Model.Documents.DocumentAttachment.Instances = documentInstances.ToArray();
            //}

            IList<UDSContact> contacts;
            foreach (Contacts modelContacts in model.Model.Contacts)
            {
                contacts = udsContacts.Where(x => x.ContactLabel == modelContacts.Label).ToList();
                foreach (UDSContact contact in contacts)
                {
                    if (contact.ContactType.HasValue && contact.ContactType.Value == (short)Helpers.UDS.UDSContactType.Contact)
                    {
                        if (contact.Relation != null)
                        {
                            modelContacts.ContactInstances = (modelContacts.ContactInstances ?? Enumerable.Empty<ContactInstance>()).Concat(new ContactInstance[] { new ContactInstance() { IdContact = contact.Relation.EntityId } }).ToArray();
                        }
                    }
                    else
                    {
                        modelContacts.ContactManualInstances = (modelContacts.ContactManualInstances ?? Enumerable.Empty<ContactManualInstance>()).Concat(new ContactManualInstance[] { new ContactManualInstance() { ContactDescription = contact.ContactManual } }).ToArray();
                    }
                }
            }

            IEnumerable<ReferenceModel> referenceModels = udsRoles.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityShortId, UniqueId = s.UniqueId, AuthorizationType = AuthorizationType.Accounted });
            model.FillAuthorizations(referenceModels, model.Model.Authorizations.Label);

            referenceModels = udsDocumentUnits.Select(s => new ReferenceModel() { UniqueId = s.Relation.UniqueId });
            model.FillProtocols(referenceModels);

            referenceModels = udsMessages.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityId, UniqueId = s.UniqueId });
            model.FillMessages(referenceModels);

            referenceModels = udsPECMails.Select(s => new ReferenceModel() { EntityId = s.Relation.EntityId, UniqueId = s.UniqueId });
            model.FillPECMails(referenceModels);

            UDSBuildModel udsBuildModel = new UDSBuildModel(model.SerializeToXml())
            {
                UDSRepository = new UDSRepositoryModel(udsRepository.UniqueId)
                {
                    ActiveDate = udsRepository.ActiveDate,
                    ExpiredDate = udsRepository.ExpiredDate,
                    ModuleXML = udsRepository.ModuleXML,
                    Name = udsRepository.Name,
                    Status = DocSuiteWeb.Model.Entities.UDS.UDSRepositoryStatus.Confirmed,
                    Version = udsRepository.Version,
                    DSWEnvironment = udsRepository.DSWEnvironment,
                    Alias = udsRepository.Alias
                },
                UniqueId = IdUDS,
                RegistrationUser = registrationUser
            };
            return udsBuildModel;

        }

        private Registro_Type GetRegistroType(Dictionary<string, object> uds_metadatas)
        {
            if (uds_metadatas.ContainsKey("TipoDocumento"))
            {
                string documentType =
                    JsonConvert.DeserializeObject<List<string>>((string)uds_metadatas["TipoDocumento"])
                               .FirstOrDefault();

                if (string.IsNullOrEmpty(documentType))
                {
                    return Registro_Type.A;
                }

                switch (documentType)
                {
                    case DOCUMENT_TYPE_INGRESSO:
                        {
                            return Registro_Type.A;
                        }
                    case DOCUMENT_TYPE_TRA_UFFICI:
                        {
                            return Registro_Type.I;
                        }
                    case DOCUMENT_TYPE_USCITA:
                        {
                            return Registro_Type.P;
                        }
                    default:
                        {
                            return Registro_Type.A;
                        }
                }
            }

            return Registro_Type.A;
        }

        #endregion
    }
}
