using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.SecureDocument.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.SecureDocument.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS.Events.Models.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.Integrations.GenericProcesses;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.SecureDocument
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private const string PROGRAM_NAME = "Integrations";
        private const string SECUREDOCUMENT_COMPLETED_LOG_TYPE = "LT";
        private const string SECUREDOCUMENT_COMPLETED_LOG_DESCRIPTION = "Terminata con successo attività di securizzazione del documento {0}[{1}]";
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IDocumentClient _documentClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IStampaConformeClient _stampaConformeClient;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private const string SIGNATURE_ATTRIBUTE_NAME = "Signature";
        private const string REFERENCE_SECUREDOCUMENT_ATTRIBUTE_NAME = "SecureDocumentId";
        private const string PROTOCOL_ODATA_FILTER = "$filter=UniqueId eq {0}&$expand=Location,Container($expand=ProtAttachLocation)";
        private const string ODATA_FILTER_LOGTYPE = "$filter=LogType eq 'LC' and Entity/UniqueId eq {0} and contains(LogDescription, '{1}')";
        private readonly string _userName;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IDocumentClient documentClient, IWebAPIClient webAPIClient, IStampaConformeClient stampaConformeClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _documentClient = documentClient;
                _webAPIClient = webAPIClient;
                _stampaConformeClient = stampaConformeClient;
                _needInitializeModule = true;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    _userName = WindowsIdentity.GetCurrent().Name;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.SecureDocument -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("SecureDocument -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.SecureDocument"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventSecureDocumentResponse>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.Topic_Workflow_Integration, 
                    _moduleConfiguration.Subscription_SecureDocument, WorkflowSuccessCallbackAsync));

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

        private async Task WorkflowSuccessCallbackAsync(IEventSecureDocumentResponse evt)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage(string.Format("WorkflowSuccessCallbackAsync -> received callback with event id {0}", evt.Id)), LogCategories);

            try
            {
                if (evt != null && evt.ContentType != null && evt.ContentType.ContentTypeValue != null)
                {
                    DocumentManagementRequestModel model = evt.ContentType.ContentTypeValue;
                    if (model == null)
                    {
                        _logger.WriteError(new LogMessage("WorkflowSuccessCallbackAsync -> DematerialisationRequestModel is null"), LogCategories);
                        throw new Exception("Il modello passato nell'evento è vuoto.");
                    }

                    WorkflowReferenceBiblosModel secureDocument = model.Documents.SingleOrDefault();
                    if (secureDocument == null)
                    {
                        _logger.WriteError(new LogMessage("WorkflowSuccessCallbackAsync -> WorkflowReferenceBiblosModel is null"), LogCategories);
                        throw new Exception("Il documento securizzato passato nel modello è vuoto.");
                    }

                    if (secureDocument.ArchiveChainId.HasValue)
                    {
                        ICollection<ArchiveDocument> documents = await _documentClient.GetChildrenAsync(secureDocument.ArchiveChainId.Value);
                        if (documents == null || documents.Count == 0)
                        {
                            _logger.WriteError(new LogMessage(string.Concat("WorkflowSuccessCallbackAsync -> Documents of chain with id ", secureDocument.ArchiveChainId.Value, " not found")), LogCategories);
                            throw new Exception(string.Concat("Nessun documento trovato nella catena ", secureDocument.ArchiveChainId.Value));
                        }

                        ArchiveDocument document = documents.FirstOrDefault();
                        if (model.DocumentUnit != null)
                        {
                            _logger.WriteDebug(new LogMessage("WorkflowSuccessCallbackAsync -> deserializing ReferenceModel of WorkflowReferenceModel"), LogCategories);
                            DocSuiteWeb.Entity.DocumentUnits.DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocSuiteWeb.Entity.DocumentUnits.DocumentUnit>(model.DocumentUnit.ReferenceModel);

                            if (documentUnit == null)
                            {
                                _logger.WriteError(new LogMessage("WorkflowSuccessCallbackAsync -> DocumentUnit not found"), LogCategories);
                                throw new Exception("L'unità documentaria passata nel modello è vuota.");
                            }

                            ICollection<Protocol> results = await _webAPIClient.GetProtocolAsync(string.Format(PROTOCOL_ODATA_FILTER, documentUnit.UniqueId));
                            if (results.Count == 0)
                            {
                                _logger.WriteWarning(new LogMessage(string.Concat("WorkflowSuccessCallbackAsync -> protocol with uniqueid ", documentUnit.UniqueId, " not found")), LogCategories);
                                throw new Exception(string.Concat("Non è stato trovato nessun protocollo con UniqueId ", documentUnit.UniqueId));
                            }

                            Protocol protocol = results.Single();
                            string signature = string.Format(_moduleConfiguration.ProtocolSignature, documentUnit.Title, documentUnit.RegistrationDate);
                            SetSignature(document, signature);
                            Location currentLocation = protocol.Location;
                            if (_moduleConfiguration.ProtocolAttachLocationEnabled)
                            {
                                currentLocation = protocol.Container.ProtAttachLocation;
                            }

                            if (currentLocation == null)
                            {
                                _logger.WriteWarning(new LogMessage(string.Concat("WorkflowSuccessCallbackAsync -> location not found for protocol with uniqueid ", documentUnit.UniqueId)), LogCategories);
                                throw new Exception(string.Concat("Non è stata trovata nessuna location per il protocollo con UniqueId ", documentUnit.UniqueId));
                            }

                            EventSecureDocumentResponse eventSecure = evt as EventSecureDocumentResponse;
                            await SaveSecureDocumentAsync(document, currentLocation.ProtocolArchive, protocol, secureDocument, eventSecure.ExternalReferenceId);
                            ArchiveDocument referenceDocument = await _documentClient.GetInfoDocumentAsync(secureDocument.ReferenceDocument.ArchiveDocumentId.Value);
                            await UpdateSecureDocumentReferenceAsync(referenceDocument, secureDocument);
                            protocol.IdAnnexed = secureDocument.ArchiveChainId;
                            protocol = _webAPIClient.PutAsync(protocol, actionType: UpdateActionType.CompleteSecureDocumentWorkflow).Result;
                            _logger.WriteInfo(new LogMessage(string.Concat("WorkflowSuccessCallbackAsync -> Protocol with uniqueid ", documentUnit.UniqueId, " updated with success")), LogCategories);

                            await CompleteSecureDocumentLogAsync(protocol, referenceDocument);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowSuccessCallbackAsync -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private void SetSignature(ArchiveDocument document, string signature)
        {
            if (!document.Metadata.ContainsKey(SIGNATURE_ATTRIBUTE_NAME))
            {
                document.Metadata.Add(SIGNATURE_ATTRIBUTE_NAME, string.Empty);
            }
            document.Metadata[SIGNATURE_ATTRIBUTE_NAME] = signature;
        }

        private async Task UpdateSecureDocumentReferenceAsync(ArchiveDocument document, WorkflowReferenceBiblosModel secureDocument)
        {
            if (!document.Metadata.ContainsKey(REFERENCE_SECUREDOCUMENT_ATTRIBUTE_NAME))
            {
                document.Metadata.Add(REFERENCE_SECUREDOCUMENT_ATTRIBUTE_NAME, string.Empty);
            }
            document.Metadata[REFERENCE_SECUREDOCUMENT_ATTRIBUTE_NAME] = secureDocument.ArchiveDocumentId;
            Guid updatedDocumentId = await _documentClient.UpdateDocumentAsync(document.Archive, document.IdDocument, document.Metadata);
        }

        private async Task SaveSecureDocumentAsync(ArchiveDocument document, string archiveName, Protocol protocol, WorkflowReferenceBiblosModel secureDocument, string externalReferenceId)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("SaveSecureDocumentAsync -> Start saving the secure document in archive ", archiveName)), LogCategories);
            document.Archive = archiveName;
            document.ContentStream = await _documentClient.GetDocumentStreamAsync(document.IdDocument);

            if (document.ContentStream == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("SaveSecureDocumentAsync -> Content of document with id ", document.IdDocument, " not found")), LogCategories);
                throw new Exception(string.Concat("Contenuto del documento con id ", document.IdDocument, " non trovato."));
            }

            await _stampaConformeClient.UploadSecureDocumentAsync(document.ContentStream, externalReferenceId);
            ArchiveDocument savedDocument = null;
            if (protocol.IdAnnexed.HasValue && protocol.IdAnnexed.Value != Guid.Empty)
            {
                savedDocument = await _documentClient.AddDocumentToChainAsync(protocol.IdAnnexed.Value, document);
            }
            else
            {
                savedDocument = await _documentClient.InsertDocumentAsync(document);
            }
            _logger.WriteInfo(new LogMessage(string.Concat("SaveSecureDocumentAsync -> Document with id ", savedDocument.IdDocument, " saved in archive ", archiveName)), LogCategories);
            secureDocument.ArchiveChainId = savedDocument.IdChain.Value;
            secureDocument.ArchiveDocumentId = savedDocument.IdDocument;
        }

        private async Task CompleteSecureDocumentLogAsync(Protocol protocol, ArchiveDocument referenceDocument)
        {
            ProtocolLog deleteLog = _webAPIClient.GetProtocolLogAsync(string.Format(ODATA_FILTER_LOGTYPE, protocol.UniqueId.ToString(), referenceDocument.IdDocument)).Result.FirstOrDefault();
            if (deleteLog != null)
            {
                await _webAPIClient.DeleteAsync(deleteLog, actionType: DeleteActionType.SecureDocumentLogDelete);
                _logger.WriteInfo(new LogMessage(string.Concat("CompleteSecureDocumentLogAsync -> Deleted Protocol log with uniqueid ", deleteLog.UniqueId)), LogCategories);
            }

            ProtocolLog log = new ProtocolLog()
            {
                Entity = protocol,
                LogDate = DateTime.UtcNow,
                RegistrationUser = _userName,
                Program = PROGRAM_NAME,
                LogType = SECUREDOCUMENT_COMPLETED_LOG_TYPE,
                LogDescription = string.Format(SECUREDOCUMENT_COMPLETED_LOG_DESCRIPTION, referenceDocument.Name, referenceDocument.IdDocument)
            };
            await _webAPIClient.PostAsync(log, actionType: InsertActionType.SecureDocumentLogInsert);
            _logger.WriteInfo(new LogMessage(string.Concat("CompleteSecureDocumentLogAsync -> Protocollog with uniqueid ", log.UniqueId, " created with success")), LogCategories);
        }

        
        #endregion
    }
}
