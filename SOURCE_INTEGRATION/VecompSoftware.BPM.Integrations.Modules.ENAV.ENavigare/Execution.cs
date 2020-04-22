using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare.Configurations;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare.Data;
using VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare.Data.Entities;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.DocumentArchives;
using VecompSoftware.Services.Command.CQRS.Events.Entities;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentArchives;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private static ICollection<LogCategory> _logValidationCategories;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private ENavigareDbContext _dbContext;
        private readonly IDocumentClient _documentClient;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IWebAPIClient _webAPIClient;
        private readonly string _username;

        private const string _attribute_filename = "Filename";
        private const string _attribute_position = "Position";
        private const string _message_attribute_module_name = "ModuleName";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogValidationCategories
        {
            get
            {
                if (_logValidationCategories == null && !LogCategories.Any(f => f.Category == LogCategoryDefinition.VALIDATION))
                {
                    _logValidationCategories = Enumerable.ToList(_logCategories);
                    _logValidationCategories.Add(new LogCategory(LogCategoryDefinition.VALIDATION));
                }

                return _logValidationCategories;
            }
        }

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
                _serviceBusClient = serviceBusClient;
                _documentClient = documentClient;
                _webAPIClient = webAPIClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _needInitializeModule = true;
                _username = string.Empty;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    _username = WindowsIdentity.GetCurrent().Name;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ENAV.ENavigare -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("Errore: Eccezione nella ricezione di eventi dal ServiceBus nel database di enavigare"), LogValidationCategories);
                _logger.WriteError(new LogMessage("ENAV E_Navigare -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _dbContext.Dispose();
            _logger.WriteInfo(new LogMessage("OnStop -> ENAV.ENavigare"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _dbContext = new ENavigareDbContext(_logger, ModuleConfigurationHelper.JsonSerializerSettings, _moduleConfiguration.ConnectionString);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreateDocumentSeriesItem>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.DocumentSeriesItemTopic,
                    _moduleConfiguration.DocumentSeriesItemSubscriptionCreate, EventDocumentSeriesItemCreateCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventUpdateDocumentSeriesItem>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.DocumentSeriesItemTopic,
                    _moduleConfiguration.DocumentSeriesItemSubscriptionUpdate, EventDocumentSeriesItemUpdateCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventRetireDocumentSeriesItem>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.DocumentSeriesItemSubscriptionRetired, EventDocumentSeriesItemRetiredCallbackAsync));

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

        private async Task EventDocumentSeriesItemCreateCallbackAsync(IEventCreateDocumentSeriesItem evt)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("EventDocumentSeriesItemCreateCallbackAsync -> received callback with event id ", evt.Id)), LogCategories);

            try
            {
                ICommandCreateDocumentSeriesItem correlatedCommmand = evt.CorrelatedCommands.FirstOrDefault() as ICommandCreateDocumentSeriesItem;
                if (correlatedCommmand == null || correlatedCommmand.CustomProperties["Metadatas"] == null)
                {
                    _logger.WriteError(new LogMessage("EventDocumentSeriesItemCreateCallbackAsync -> CorrelatedCommands is empty"), LogCategories);
                    _logger.WriteError(new LogMessage("Errore: la serie non ha tutte le informazioni necessarie alla corretta gestione del dato verso enavigare"), LogValidationCategories);
                    throw new Exception("Errore: la serie non ha tutte le informazioni necessarie alla corretta gestione del dato verso enavigare");
                }
                DocumentSeriesItem documentSeriesItem = evt.ContentType.ContentTypeValue;
                IDictionary<string, string> metadatas = JsonConvert.DeserializeObject<IDictionary<string, string>>(correlatedCommmand.CustomProperties["Metadatas"] as string);
                ENavigareDocumentSeriesItem entity = MappingSkyDoc_DocumentSeries(documentSeriesItem, metadatas, new ENavigareDocumentSeriesItem());
                _dbContext.DocumentSeries.Add(entity);
                await _dbContext.SaveChangesAsync();
                await EvaluateRetiredDateAsync(evt, null);

                _logger.WriteDebug(new LogMessage("EventDocumentSeriesItemCreateCallbackAsync -> Saved item"), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Errore: Eccezione nella procedura di gestione dell'evento ", evt.Id, " nel database di enavigare")), LogValidationCategories);
                _logger.WriteError(new LogMessage("EventDocumentSeriesItemCreateCallbackAsync -> error complete call ENavigare Services"), ex, LogCategories);
                throw;
            }
            try
            {
                if (_dbContext.GetValidationErrors().Any())
                {
                    _dbContext.DocumentSeries.Local.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage(string.Concat("ignore remove entity object state: ", ex.Message)), LogCategories);
            }
        }

        private async Task EventDocumentSeriesItemUpdateCallbackAsync(IEventUpdateDocumentSeriesItem evt)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage(string.Concat("EventDocumentSeriesItemUpdateCallbackAsync-> received callback with event id ", evt.Id)), LogCategories);

            try
            {
                ICommandUpdateDocumentSeriesItem correlatedCommmand = evt.CorrelatedCommands.FirstOrDefault() as ICommandUpdateDocumentSeriesItem;
                if (correlatedCommmand == null || correlatedCommmand.CustomProperties["Metadatas"] == null)
                {
                    _logger.WriteError(new LogMessage("EventDocumentSeriesItemUpdateCallbackAsync -> CorrelatedCommands is empty"), LogCategories);
                    _logger.WriteError(new LogMessage("Errore: la serie non ha tutte le informazioni necessarie alla corretta gestione del dato verso enavigare"), LogValidationCategories);
                    throw new Exception("Errore: la serie non ha tutte le informazioni necessarie alla corretta gestione del dato verso enavigare");
                }
                IDictionary<string, string> metadatas = JsonConvert.DeserializeObject<IDictionary<string, string>>(correlatedCommmand.CustomProperties["Metadatas"] as string);
                DocumentSeriesItem documentSeriesItem = evt.ContentType.ContentTypeValue;
                ENavigareDocumentSeriesItem skyDocDocumentSeries = await _dbContext.DocumentSeries.FindAsync(documentSeriesItem.UniqueId);
                DateTime? dataRitiro = skyDocDocumentSeries.DataRitiro;
                ENavigareDocumentSeriesItem entity = MappingSkyDoc_DocumentSeries(documentSeriesItem, metadatas, skyDocDocumentSeries);
                _dbContext.DocumentSeries.Attach(entity);
                await _dbContext.SaveChangesAsync();
                await EvaluateRetiredDateAsync(evt, dataRitiro);

                _logger.WriteDebug(new LogMessage("IEventUpdateDocumentSeriesItem -> Saved item"), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Errore: Eccezione nella procedura di gestione dell'evento ", evt.Id, " nel database di enavigare")), LogValidationCategories);
                _logger.WriteError(new LogMessage("IEventUpdateDocumentSeriesItem -> error complete call ENavigare Services"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventDocumentSeriesItemRetiredCallbackAsync(IEventEntity<DocumentSeriesItem> evt)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage(string.Concat("EventDocumentSeriesItemRetiredCallbackAsync-> received callback with event id ", evt.Id)), LogCategories);

            try
            {
                if (evt == null || evt.ContentType == null || evt.ContentType.ContentTypeValue == null)
                {
                    _logger.WriteError(new LogMessage("EventDocumentSeriesItemRetiredCallbackAsync -> received event is empty"), LogCategories);
                    _logger.WriteError(new LogMessage("Errore: l'evento ricevuto non contiene le informazioni necessarie alla corretta gestione del dato."), LogCategories);
                    throw new Exception("Errore: l'evento ricevuto non contiene le informazioni necessarie alla corretta gestione del dato.");
                }

                DocumentSeriesItem documentSeriesItem = evt.ContentType.ContentTypeValue;
                ENavigareDocumentSeriesItem skyDocDocumentSeries = await _dbContext.DocumentSeries.FindAsync(documentSeriesItem.UniqueId);
                if (documentSeriesItem.RetireDate != skyDocDocumentSeries.DataRitiro)
                {
                    return;
                }

                if (documentSeriesItem.IdMain.HasValue && documentSeriesItem.IdMain != Guid.Empty)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("EvaluateRetiredDateAsync -> Remove FullText IdMain ", documentSeriesItem.IdMain.Value, " series item ", documentSeriesItem.UniqueId)), LogCategories);
                    await _documentClient.RemoveFullTextDataAsync(documentSeriesItem.IdMain.Value);
                }

                if (documentSeriesItem.IdAnnexed.HasValue && documentSeriesItem.IdAnnexed != Guid.Empty)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("EvaluateRetiredDateAsync -> Remove FullText IdAnnexed ", documentSeriesItem.IdAnnexed.Value, " series item ", documentSeriesItem.UniqueId)), LogCategories);
                    await _documentClient.RemoveFullTextDataAsync(documentSeriesItem.IdAnnexed.Value);
                }

                if (documentSeriesItem.IdUnpublishedAnnexed.HasValue && documentSeriesItem.IdUnpublishedAnnexed != Guid.Empty)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("EvaluateRetiredDateAsync -> Remove FullText IdUnpublishedAnnexed ", documentSeriesItem.IdUnpublishedAnnexed.Value, " series item ", documentSeriesItem.UniqueId)), LogCategories);
                    await _documentClient.RemoveFullTextDataAsync(documentSeriesItem.IdUnpublishedAnnexed.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Errore: Eccezione nella procedura di gestione dell'evento ", evt.Id)), ex, LogCategories);
                throw;
            }
        }

        private ENavigareDocumentSeriesItem MappingSkyDoc_DocumentSeries(DocumentSeriesItem documentSeriesItem, IDictionary<string, string> metadatas, ENavigareDocumentSeriesItem skyDOC_DocumentSeries)
        {
            Guid idMainChain = documentSeriesItem.IdMain ?? Guid.Empty;
            Guid idAnnexedChain = documentSeriesItem.IdAnnexed ?? Guid.Empty;
            Guid idUnpublishedChain = documentSeriesItem.IdUnpublishedAnnexed ?? Guid.Empty;
            bool invalidateEvaluation = idMainChain == Guid.Empty || skyDOC_DocumentSeries == null;

            if (!invalidateEvaluation)
            {
                ArchiveDocument documentInfo = _documentClient.GetInfoDocumentAsync(idMainChain).Result;
                try
                {
                    skyDOC_DocumentSeries.Abstract = documentInfo.Metadata.Where(x => x.Key.Equals("Abstract", StringComparison.InvariantCultureIgnoreCase)).First().Value.ToString();
                    KeyValuePair<string, object> attributeValueDataValiditaScheda = documentInfo.Metadata.Where(x => x.Key.Equals("DataValiditaScheda", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (attributeValueDataValiditaScheda.Value != null)
                    {
                        skyDOC_DocumentSeries.DataValiditaScheda = DateTime.Parse(attributeValueDataValiditaScheda.Value.ToString());
                    }
                    skyDOC_DocumentSeries.Codice = documentInfo.Metadata.Where(x => x.Key.Equals("Codice", StringComparison.InvariantCultureIgnoreCase)).First().Value.ToString();
                    if (metadatas.ContainsKey("trasparenza_container_archive") && _moduleConfiguration.ArchiveMappingUrls.ContainsKey(metadatas["trasparenza_container_archive"]))
                    {
                        skyDOC_DocumentSeries.Url = string.Format(_moduleConfiguration.ArchiveMappingUrls[metadatas["trasparenza_container_archive"]], documentSeriesItem.EntityId);
                    }
                    if (string.IsNullOrEmpty(skyDOC_DocumentSeries.Url))
                    {
                        _logger.WriteWarning(new LogMessage(string.Concat("MappingSkyDoc_DocumentSeries -> Missing url attribute ", metadatas.ContainsKey("trasparenza_container_archive"),
                            ", ", metadatas.ContainsKey("trasparenza_container_archive") ? metadatas["trasparenza_container_archive"] : string.Empty)), LogCategories);
                        invalidateEvaluation = true;
                    }

                    skyDOC_DocumentSeries.Oggetto = documentSeriesItem.Subject;
                    skyDOC_DocumentSeries.UniqueId = documentSeriesItem.UniqueId;
                    skyDOC_DocumentSeries.DataPubblicazione = documentSeriesItem.PublishingDate;
                    skyDOC_DocumentSeries.DataUltimoAggiornamento = null;
                    if (documentSeriesItem.LastChangedDate.HasValue)
                    {
                        skyDOC_DocumentSeries.DataUltimoAggiornamento = documentSeriesItem.LastChangedDate.Value;
                    }

                    skyDOC_DocumentSeries.DataRitiro = documentSeriesItem.RetireDate;
                    skyDOC_DocumentSeries.InEvidenza = documentSeriesItem.Priority;
                }
                catch (Exception ex)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("MappingSkyDoc_DocumentSeries -> Missing attributes on series ", ex.Message)), LogCategories);
                    invalidateEvaluation = true;
                }
                bool localInvalidateEvaluation = false;
                skyDOC_DocumentSeries.ProceduraPosizioni = metadatas["trasparenza_path"];
                skyDOC_DocumentSeries = DecorateDocumentProperties(skyDOC_DocumentSeries, "main", idMainChain,
                    (flattered, entity) => entity.ProceduraNomiFile = flattered, out localInvalidateEvaluation);
                invalidateEvaluation |= localInvalidateEvaluation;

                skyDOC_DocumentSeries = DecorateDocumentProperties(skyDOC_DocumentSeries, "annexed", idAnnexedChain,
                    (flattered, entity) => entity.LineeGuidaNomiFile = flattered, out localInvalidateEvaluation);
                skyDOC_DocumentSeries.LineeGuidaPosizioni = metadatas["trasparenza_path"];
                invalidateEvaluation |= localInvalidateEvaluation;

                skyDOC_DocumentSeries = DecorateDocumentProperties(skyDOC_DocumentSeries, "annexed unpublished", idUnpublishedChain,
                    (flattered, entity) => entity.ModulisticaNomiFile = flattered, out localInvalidateEvaluation);
                skyDOC_DocumentSeries.ModulusticaPosizioni = metadatas["trasparenza_path"];
                invalidateEvaluation |= localInvalidateEvaluation;
            }

            if (invalidateEvaluation)
            {
                _logger.WriteError(new LogMessage(string.Concat("MappingSkyDoc_DocumentSeries -> Invalidation Results : idMainChain is Empty", idMainChain == Guid.Empty, ", skyDOC_DocumentSeries not found ", skyDOC_DocumentSeries == null)), LogCategories);
                _logger.WriteError(new LogMessage("Errore: la serie non ha tutte le informazioni necessarie alla corretta gestione del dato verso enavigare"), LogValidationCategories);
                throw new Exception("Errore: la serie non ha tutte le informazioni necessarie alla corretta gestione del dato verso enavigare");
            }

            return skyDOC_DocumentSeries;
        }
        
        private string FlatteringAttributes(ICollection<ArchiveDocument> documents, string attribute)
        {
            if (documents == null || !documents.Any() || string.IsNullOrEmpty(attribute))
            {
                return string.Empty;
            }

            return string.Join("|", documents.Select(f => f.Metadata.Where(x => x.Key.Equals(attribute, StringComparison.InvariantCultureIgnoreCase)).First().Value.ToString()).ToArray());
        }

        private ENavigareDocumentSeriesItem DecorateDocumentProperties(ENavigareDocumentSeriesItem entity, string chainName, Guid idDocumentChain,
            Action<string, ENavigareDocumentSeriesItem> setterName, out bool invalidateEvaluation)
        {
            invalidateEvaluation = false;
            if (idDocumentChain != Guid.Empty)
            {
                try
                {
                    ICollection<ArchiveDocument> documents = _documentClient.GetChildrenAsync(idDocumentChain).Result;
                    setterName(FlatteringAttributes(documents, _attribute_filename), entity);
                }
                catch (Exception ex)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("MappingSkyDoc_DocumentSeries -> Missing attribute on ", chainName, " document }", ex.Message)), LogCategories);
                    invalidateEvaluation = true;
                }
            }

            return entity;
        }

        private async Task EvaluateRetiredDateAsync(IEventEntity<DocumentSeriesItem> documentSeriesItemEvent, DateTime? skyDocRetiredDate)
        {
            DocumentSeriesItem documentSeriesItem = documentSeriesItemEvent.ContentType.ContentTypeValue;
            if ((!documentSeriesItem.RetireDate.HasValue && !skyDocRetiredDate.HasValue)
                || (documentSeriesItem.RetireDate.HasValue && skyDocRetiredDate.HasValue && documentSeriesItem.RetireDate.Value == skyDocRetiredDate.Value))
            {
                return;
            }

            if ((documentSeriesItem.RetireDate.HasValue && !skyDocRetiredDate.HasValue) || ((documentSeriesItem.RetireDate.HasValue && skyDocRetiredDate.HasValue)))
            {
                _logger.WriteDebug(new LogMessage(string.Concat("EvaluateRetiredDateAsync -> Scheduled retired ", documentSeriesItem.RetireDate.Value.Date, " series item ", documentSeriesItem.UniqueId)), LogCategories);
                await SendRetiredDocumentSeriesItemEventAsync(documentSeriesItem, documentSeriesItem.RetireDate.Value.Date);
                return;
            }

            if (!documentSeriesItem.RetireDate.HasValue && skyDocRetiredDate.HasValue)
            {
                if (documentSeriesItem.IdMain.HasValue && documentSeriesItem.IdMain.Value != Guid.Empty)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("EvaluateRetiredDateAsync -> Align FullText IdMain ", documentSeriesItem.IdMain.Value, " series item ", documentSeriesItem.UniqueId)), LogCategories);
                    await _documentClient.AlignFullTextDataAsync(documentSeriesItem.IdMain.Value);
                }

                if (documentSeriesItem.IdAnnexed.HasValue && documentSeriesItem.IdAnnexed.Value != Guid.Empty)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("EvaluateRetiredDateAsync -> Align FullText IdAnnexed ", documentSeriesItem.IdAnnexed.Value, " series item ", documentSeriesItem.UniqueId)), LogCategories);
                    await _documentClient.AlignFullTextDataAsync(documentSeriesItem.IdAnnexed.Value);
                }

                if (documentSeriesItem.IdUnpublishedAnnexed.HasValue && documentSeriesItem.IdUnpublishedAnnexed.Value != Guid.Empty)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("EvaluateRetiredDateAsync -> Align FullText IdUnpublishedAnnexed ", documentSeriesItem.IdUnpublishedAnnexed.Value, " series item ", documentSeriesItem.UniqueId)), LogCategories);
                    await _documentClient.AlignFullTextDataAsync(documentSeriesItem.IdUnpublishedAnnexed.Value);
                }

                return;
            }
        }

        private async Task SendRetiredDocumentSeriesItemEventAsync(DocumentSeriesItem documentSeriesItem, DateTime scheduledTime)
        {
            IdentityContext identity = new IdentityContext(_username);
            string tenantName = _moduleConfiguration.TenantName;
            Guid tenantId = _moduleConfiguration.TenantId;

            EventRetireDocumentSeriesItem eventRetireDocumentSeriesItem = new EventRetireDocumentSeriesItem(Guid.NewGuid(), null, tenantName, tenantId, identity, documentSeriesItem, scheduledTime, null, null);
            eventRetireDocumentSeriesItem.CustomProperties.Add(_message_attribute_module_name, ModuleConfigurationHelper.MODULE_NAME);
            await _webAPIClient.PostAsync(eventRetireDocumentSeriesItem);
        }

        #endregion

    }
}
