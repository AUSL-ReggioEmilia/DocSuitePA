using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.Integrations.GenericProcesses;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Dematerialisation
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {

        #region [ Fields ]
        private const string PROGRAM_NAME = "Integrations";
        private const string DEMATERIALISATION_COMPLETED_LOG_TYPE = "ST";
        private const string DEMATERIALISATION_COMPLETED_LOG_DESCRIPTION = "Terminata con successo attività di attestazione di conformità";
        private const string ODATA_FILTER_LOGTYPE = "$filter=LogType eq 'SC' and Entity/UniqueId eq ";
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IDocumentClient _documentClient;
        private readonly IWebAPIClient _webAPIClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IDocumentClient documentClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _documentClient = documentClient;
                _webAPIClient = webAPIClient;
                _needInitializeModule = true;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    _userName = WindowsIdentity.GetCurrent().Name;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.Dematerialisation -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("Dematerialisation -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.Dematerialisation"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventDematerialisationResponse>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.Topic_Workflow_Integration,
                    _moduleConfiguration.Subscription_Dematerialisation, WorkflowSuccessCallbackAsync));

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

        private async Task WorkflowSuccessCallbackAsync(IEventDematerialisationResponse evt)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage(string.Format("EventCompleteWorkflowInstanceCallbackAsync -> received callback with event id {0}", evt.Id)), LogCategories);

            try
            {
                if (evt != null && evt.ContentType != null && evt.ContentType.ContentTypeValue != null)
                {
                    DocumentManagementRequestModel model = evt.ContentType.ContentTypeValue;
                    if (model == null)
                    {
                        _logger.WriteError(new LogMessage("EventCompleteWorkflowInstanceCallbackAsync -> DematerialisationRequestModel is null"), LogCategories);
                        throw new Exception("Il modello passato nell'evento è vuoto.");
                    }

                    WorkflowReferenceBiblosModel dematerialisation = model.Documents.Where(d => d.ChainType == ChainType.DematerialisationChain).FirstOrDefault();
                    if (dematerialisation == null)
                    {
                        _logger.WriteError(new LogMessage("EventCompleteWorkflowInstanceCallbackAsync -> DematerialisationDocumentModel is null"), LogCategories);
                        throw new Exception("Il documento di attestato di conformità passato nel modello è vuoto.");
                    }

                    string archiveName = string.Empty;
                    if (dematerialisation.ArchiveChainId.HasValue)
                    {
                        List<Document> documents = await _documentClient.GetDocumentChildrenAsync(dematerialisation.ArchiveChainId.Value);

                        if (documents == null || documents.Count == 0)
                        {
                            _logger.WriteError(new LogMessage(string.Concat("EventCompleteWorkflowInstanceCallbackAsync -> Documents of chain with id ", dematerialisation.ArchiveChainId.Value, " not found")), LogCategories);
                            throw new Exception(string.Concat("Nessun documento trovato nella catena ", dematerialisation.ArchiveChainId.Value));
                        }

                        Document document = documents.FirstOrDefault();
                        if (model.DocumentUnit != null)
                        {
                            AttributeValue signature = document.AttributeValues.Where(t => t.Attribute.Name == _documentClient.SIGNATURE_ATTRIBUTE_NAME).FirstOrDefault();

                            _logger.WriteDebug(new LogMessage("EventCompleteWorkflowInstanceCallbackAsync -> deserializing ReferenceModel of WorkflowReferenceModel"), LogCategories);
                            DocSuiteWeb.Entity.DocumentUnits.DocumentUnit documentUnit = JsonConvert.DeserializeObject<DocSuiteWeb.Entity.DocumentUnits.DocumentUnit>(model.DocumentUnit.ReferenceModel);

                            if (documentUnit == null)
                            {
                                _logger.WriteError(new LogMessage("EventCompleteWorkflowInstanceCallbackAsync -> DocumentUnit not found"), LogCategories);
                                throw new Exception("L'unità documentaria passata nel modello è vuota.");
                            }

                            archiveName = model.Documents.FirstOrDefault().ArchiveName;

                            switch (model.DocumentUnit.ReferenceType)
                            {
                                case DSWEnvironmentType.Protocol:
                                    {
                                        string partialSignature = string.Concat(_moduleConfiguration.ProtocolSignature, documentUnit.Title);
                                        await SetSignature(signature, archiveName, document, partialSignature);
                                        await SaveDematerialisationDocumentAsync(document, archiveName, dematerialisation);
                                        Protocol protocol = new Protocol(documentUnit.UniqueId)
                                        {
                                            DematerialisationChainId = dematerialisation.ArchiveChainId
                                        };
                                        protocol = _webAPIClient.PutAsync(protocol, actionType: UpdateActionType.CompleteDematerialisationWorkflow).Result;
                                        _logger.WriteInfo(new LogMessage(string.Concat("EventCompleteWorkflowInstanceCallbackAsync -> Protocol with uniqueid ", documentUnit.UniqueId, " updated with success")), LogCategories);

                                        await ProtocolLogCompleteDematerialisationAsync(protocol, documentUnit);
                                        break;
                                    }
                                case DSWEnvironmentType.Resolution:
                                    {
                                        string partialSignature = string.Format(_moduleConfiguration.ResolutionSignature, documentUnit.DocumentUnitName, documentUnit.Title);
                                        await SetSignature(signature, archiveName, document, partialSignature);
                                        await SaveDematerialisationDocumentAsync(document, archiveName, dematerialisation);
                                        Resolution resolution = new Resolution(documentUnit.UniqueId)
                                        {
                                            FileResolution = new FileResolution
                                            {
                                                DematerialisationChainId = dematerialisation.ArchiveChainId
                                            }
                                        };
                                        resolution = _webAPIClient.PutAsync(resolution, actionType: UpdateActionType.CompleteDematerialisationWorkflow).Result;
                                        _logger.WriteInfo(new LogMessage(string.Concat("EventCompleteWorkflowInstanceCallbackAsync -> Resolution with uniqueid ", documentUnit.UniqueId, " updated with success")), LogCategories);
                                        await ResolutionLogCompleteDematerialisationAsync(resolution, documentUnit);
                                        break;
                                    }
                                case DSWEnvironmentType.DocumentSeries:
                                    {
                                        DocumentSeriesItem item = new DocumentSeriesItem(documentUnit.UniqueId);
                                        await SaveDematerialisationDocumentAsync(document, archiveName, dematerialisation);
                                        item.DematerialisationChainId = dematerialisation.ArchiveChainId;
                                        item = _webAPIClient.PutAsync(item, actionType: UpdateActionType.CompleteDematerialisationWorkflow).Result;
                                        _logger.WriteInfo(new LogMessage(string.Concat("EventCompleteWorkflowInstanceCallbackAsync -> DocumentSeriesItem with uniqueid ", documentUnit.UniqueId, " updated with success")), LogCategories);
                                        await DocumentSeriesItemLogCompleteDematerialisationAsync(item, documentUnit);
                                        break;
                                    }

                                    //Per il momento le UDS non sono gestite, nel momento in cui verranno gestite servono questi elementi: UDSModel e IdUDSRepository
                                    //case DSWEnvironmentType.UDS:
                                    //    {
                                    //        UDSModel udsModel = JsonConvert.DeserializeObject<UDSModel>(model.DocumentUnit.ReferenceModel);

                                    //        if (udsModel != null && udsModel.Model != null)
                                    //        {
                                    //            FillDematerialisationInstances(udsModel.Model, document);
                                    //        }

                                    //        IdentityContext identity = new IdentityContext(_userName);
                                    //        string tenantName = _moduleConfiguration.TenantName;
                                    //        Guid tenantId = _moduleConfiguration.TenantId;


                                    //        UDSCommandModel commandModel = CreateUDSCommandModel(idUDSRepository, model.DocumentUnit.ReferenceId, model);
                                    //        CommandInsertUDSData commandInsert = new CommandInsertUDSData(tenantName, tenantId, identity, commandModel);

                                    //        _updateUDSSubscriptionName = await _serviceBusClient.CreateSubscriptionAsync(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.Topic_UDS, string.Concat("CorrelationId = '", commandInsert.CorrelationId, "'"));
                                    //        _subscriptions.Add(_serviceBusClient.StartListening<IEventUpdateUDSData>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.Topic_UDS, _updateUDSSubscriptionName, UDSUpdateSuccessCallbackAsync));
                                    //        commandInsert = _webAPIClient.PostAsync(commandInsert).Result;

                                    //        break;
                                    //    }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventCompleteWorkflowInstanceCallbackAsync -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        //Metodo di callback di uodate di UDS
        //private async Task UDSUpdateSuccessCallbackAsync(IEventUpdateUDSData evt)
        //{
        //    if (Cancel)
        //    {
        //        return;
        //    }
        //    _logger.WriteInfo(new LogMessage(string.Format("UDSUpdateSuccessCallbackAsync -> received callback with event id {0}", evt.Id)), LogCategories);

        //    await _serviceBusClient.DeleteSubscriptionAsync(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.Topic_UDS, _updateUDSSubscriptionName);
        //}

        //Creazione comando UDS
        //private UDSCommandModel CreateUDSCommandModel(Guid idUDSRepository, Guid idUDS, UDSModel model)
        //{
        //    UDSCommandModel commandModel = new UDSCommandModel(model.SerializeToXml());
        //    commandModel.UDSRepository = new UDSRepositoryModel(idUDSRepository);
        //    commandModel.UniqueId = idUDS;
        //    commandModel.RegistrationUser = _userName;
        //    return commandModel;
        //}

        //Aggiunta catena dematerializzazione nell'UDS
        //private void FillDematerialisationInstances(UnitaDocumentariaSpecifica model, Services.BiblosDS.DocumentService.Document dematerialisationDocument)
        //{
        //    if (model.Documents == null)
        //    {
        //        model.Documents = new Documents();
        //    }

        //    if (model.Documents.DocumentDematerialisation == null)
        //    {
        //        model.Documents.DocumentDematerialisation = new Helpers.UDS.Document();
        //    }

        //    IList<DocumentInstance> instances = new List<DocumentInstance>();
        //    instances.Add(new DocumentInstance()
        //    {
        //        DocumentName = dematerialisationDocument.Name,
        //        DocumentContent = Convert.ToBase64String(dematerialisationDocument.Content.Blob)
        //    });
        //    model.Documents.DocumentDematerialisation.Instances = instances.ToArray();
        //}

        private async Task SetSignature(AttributeValue signature, string archiveName, Document document, string partialSignatureValue)
        {
            if (signature == null)
            {
                IList<Services.BiblosDS.DocumentService.Attribute> attributes = await _documentClient.GetAttributesDefinitionAsync(archiveName);
                Services.BiblosDS.DocumentService.Attribute signatureAttribute = attributes.Where(a => a.Name == _documentClient.SIGNATURE_ATTRIBUTE_NAME).FirstOrDefault();
                if (signatureAttribute != null)
                {
                    signature = new AttributeValue
                    {
                        Attribute = signatureAttribute
                    };
                    document.AttributeValues.Add(signature);
                }
            }

            signature.Value = partialSignatureValue;
            if (!string.IsNullOrEmpty(_moduleConfiguration.CorporateAcronym))
            {
                signature.Value = string.Concat(_moduleConfiguration.CorporateAcronym, " ", signature.Value);
            }
        }

        private async Task SaveDematerialisationDocumentAsync(Document document, string archiveName, WorkflowReferenceBiblosModel dematerialisation)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("SaveDematerialisationDocument -> Start saving the dematerialisation document in archive ", archiveName)), LogCategories);
            IList<Archive> archives = await _documentClient.GetArchivesAsync();
            Archive archive = archives.Where(a => a.Name.Equals(archiveName)).FirstOrDefault();
            if (archive == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("SaveDematerialisationDocumentAsync -> Biblos Archive ", archiveName, " not found")), LogCategories);
                throw new Exception(string.Concat("Archivio Biblos ", archiveName, " non trovato."));
            }

            Document savedDocument = new Document
            {
                Name = document.Name,
                Archive = archive,
                AttributeValues = document.AttributeValues,
                Content = await _documentClient.GetDocumentContentByIdAsync(document.IdDocument)
            };

            if (savedDocument.Content == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("SaveDematerialisationDocumentAsync -> Content of document with id ", document.IdDocument, " not found")), LogCategories);
                throw new Exception(string.Concat("Contenuto del documento con id ", document.IdDocument, " non trovato."));
            }

            savedDocument = await _documentClient.AddDocumentToChainAsync(savedDocument);
            _logger.WriteInfo(new LogMessage(string.Concat("SaveDematerialisationDocumentAsync -> Document with id ", savedDocument.IdDocument, " saved in archive ", archiveName)), LogCategories);
            dematerialisation.ArchiveChainId = savedDocument.DocumentParent.IdDocument;
            dematerialisation.ArchiveDocumentId = savedDocument.IdDocument;
        }

        private async Task ProtocolLogCompleteDematerialisationAsync(Protocol protocol, DocSuiteWeb.Entity.DocumentUnits.DocumentUnit documentUnit)
        {
            ProtocolLog deleteLog = _webAPIClient.GetProtocolLogAsync(string.Concat(ODATA_FILTER_LOGTYPE, protocol.UniqueId.ToString())).Result.FirstOrDefault();
            if (deleteLog != null)
            {
                await _webAPIClient.DeleteAsync(deleteLog, actionType: DeleteActionType.DematerialisationLogDelete);
                _logger.WriteInfo(new LogMessage(string.Concat("ProtocolLogCompleteDematerialisationAsync -> Deleted Protocol log with uniqueid ", deleteLog.UniqueId)), LogCategories);
            }

            ProtocolLog log = new ProtocolLog
            {
                Entity = protocol,
                LogDate = DateTime.UtcNow,
                RegistrationUser = _userName,
                Program = PROGRAM_NAME,
                LogType = DEMATERIALISATION_COMPLETED_LOG_TYPE,
                LogDescription = DEMATERIALISATION_COMPLETED_LOG_DESCRIPTION
            };
            await _webAPIClient.PostAsync(log, actionType: InsertActionType.DematerialisationLogInsert);
            _logger.WriteInfo(new LogMessage(string.Concat("ProtocolLogCompleteDematerialisationAsync -> Protocollog with uniqueid ", log.UniqueId, " created with success")), LogCategories);
        }

        private async Task ResolutionLogCompleteDematerialisationAsync(Resolution resolution, DocSuiteWeb.Entity.DocumentUnits.DocumentUnit documentUnit)
        {
            ResolutionLog deleteLog = _webAPIClient.GetResolutionLogAsync(string.Concat(ODATA_FILTER_LOGTYPE, resolution.UniqueId.ToString())).Result.FirstOrDefault();
            if (deleteLog != null)
            {
                await _webAPIClient.DeleteAsync(deleteLog, actionType: DeleteActionType.DematerialisationLogDelete);
                _logger.WriteInfo(new LogMessage(string.Concat("ResolutionLogCompleteDematerialisationAsync -> Deleted Resolution log with uniqueid ", deleteLog.UniqueId)), LogCategories);
            }

            ResolutionLog log = new ResolutionLog
            {
                Entity = resolution,
                LogDate = DateTime.UtcNow,
                RegistrationUser = _userName,
                Program = PROGRAM_NAME,
                LogType = DEMATERIALISATION_COMPLETED_LOG_TYPE,
                LogDescription = DEMATERIALISATION_COMPLETED_LOG_DESCRIPTION
            };
            await _webAPIClient.PostAsync(log, actionType: InsertActionType.DematerialisationLogInsert);
            _logger.WriteInfo(new LogMessage(string.Concat("ResolutionLogCompleteDematerialisationAsync -> Resolution log with uniqueid ", log.UniqueId, " created with success")), LogCategories);
        }

        private async Task DocumentSeriesItemLogCompleteDematerialisationAsync(DocumentSeriesItem item, DocSuiteWeb.Entity.DocumentUnits.DocumentUnit documentUnit)
        {
            DocumentSeriesItemLog deleteLog = _webAPIClient.GetDocumentSeriesItemLogAsync(string.Concat(ODATA_FILTER_LOGTYPE, item.UniqueId.ToString())).Result.FirstOrDefault();
            if (deleteLog != null)
            {
                await _webAPIClient.DeleteAsync(deleteLog, actionType: DeleteActionType.DematerialisationLogDelete);
                _logger.WriteInfo(new LogMessage(string.Concat("DocumentSeriesItemLogCompleteDematerialisationAsync -> Deleted DocumentSeriesItem log with uniqueid ", deleteLog.UniqueId)), LogCategories);

            }
            DocumentSeriesItemLog log = new DocumentSeriesItemLog
            {
                Entity = item,
                LogDate = DateTime.UtcNow,
                RegistrationUser = _userName,
                Program = PROGRAM_NAME,
                LogType = DEMATERIALISATION_COMPLETED_LOG_TYPE,
                LogDescription = DEMATERIALISATION_COMPLETED_LOG_DESCRIPTION
            };
            await _webAPIClient.PostAsync(log, actionType: InsertActionType.DematerialisationLogInsert);
            _logger.WriteInfo(new LogMessage(string.Concat("DocumentSeriesItemLogCompleteDematerialisationAsync -> DocumentSeriesItem log with uniqueid ", log.UniqueId, " created with success")), LogCategories);
        }

        #endregion
    }
}
