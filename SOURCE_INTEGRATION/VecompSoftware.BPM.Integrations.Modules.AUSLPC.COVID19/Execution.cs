using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Builders;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Configurations;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Mappers;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Models.Integrations.GenericProcesses;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private readonly AccountModelBuilder _accountModelBuilder;
        private readonly PortaleCovidClient _portaleCovidClient;
        private bool _needInitializeModule = false;        
        private const string ACCOUNT_FISCALCODE_PROPERTY = "FiscalCode";
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
        public Execution(ILogger logger, IWebAPIClient webAPIClient, IDocumentClient documentClient, IServiceBusClient serviceBusClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _webAPIClient = webAPIClient;
                _documentClient = documentClient;
                _serviceBusClient = serviceBusClient;
                _accountModelBuilder = new AccountModelBuilder(webAPIClient);
                _portaleCovidClient = new PortaleCovidClient(logger, _moduleConfiguration);
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLPC.COVID19 -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("AUSLPC.COVID19 -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventIntegrationRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartCovidPortalNotificationSubscription, EventCovidPortalNotificationCallbackAsync));
                _needInitializeModule = false;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AUSLPC.COVID19"), LogCategories);
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

        private async Task EventCovidPortalNotificationCallbackAsync(IEventIntegrationRequest evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventCovidPortalNotificationCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                DocSuiteEvent @event = evt.ContentType.ContentTypeValue;
                if (@event.EventModel == null)
                {
                    _logger.WriteWarning(new LogMessage($"Event id {evt.Id} is not evaluated correctly, EventModel is null."), LogCategories);
                    throw new Exception($"Event id {evt.Id} is not evaluated correctly, EventModel is null.");
                }

                DocumentUnit documentUnit = await _webAPIClient.GetDocumentUnitAsync(@event.EventModel.UniqueId);
                if (documentUnit == null)
                {
                    _logger.WriteWarning(new LogMessage($"DocumentUnit with id {@event.EventModel.UniqueId} not found."), LogCategories);
                    throw new Exception($"DocumentUnit with id {@event.EventModel.UniqueId} not found.");
                }

                string fiscalCode = @event.EventModel.CustomProperties.Where(x => x.Key.Equals(ACCOUNT_FISCALCODE_PROPERTY)
                        && !string.IsNullOrEmpty(x.Value)).Select(s => s.Value).SingleOrDefault();
                if (string.IsNullOrEmpty(fiscalCode))
                {
                    _logger.WriteWarning(new LogMessage($"Event id {evt.Id} is not evaluated correctly, EventModel has not custom property [{ACCOUNT_FISCALCODE_PROPERTY}]."), LogCategories);
                    throw new Exception($"Event id {evt.Id} is not evaluated correctly, EventModel has not custom property [{ACCOUNT_FISCALCODE_PROPERTY}].");
                }

                AccountModel account = _accountModelBuilder.CreateAccountModel(documentUnit, fiscalCode);
                DocumentUnitModel documentUnitModel = await BuildDocumentUnitModelAsync(documentUnit);
                RequestModel request = new RequestModel() { User = account, DocumentUnits = new List<DocumentUnitModel>() { documentUnitModel } };
                _logger.WriteDebug(new LogMessage($"Send request to COVID portal"), LogCategories);
                await _portaleCovidClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventCovidPortalNotificationCallbackAsync -> error occouring when calling COVID portal service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        public async Task<DocumentUnitModel> BuildDocumentUnitModelAsync(DocumentUnit documentUnit)
        {
            DocumentUnitModel documentUnitModel = DocumentUnitModelMapper.Map(documentUnit);
            ICollection<DocumentUnitChain> documentUnitChains = await _webAPIClient.GetDocumentUnitChainsAsync(documentUnit.UniqueId);
            if (documentUnitChains.Count == 0)
            {
                throw new Exception($"Not found chains for DocumentUnit {documentUnit.UniqueId}");
            }

            DocumentUnitChain mainChain = documentUnitChains.Single(x => x.ChainType == ChainType.MainChain);
            DocumentModel mainDocument = await GetMainDocumentModelAsync(mainChain);
            documentUnitModel.DocumentUnitChains.Add(mainDocument);
            documentUnitModel.MainDocumentName = mainDocument.FileName;
            if (documentUnitChains.Any(x => x.ChainType == ChainType.AttachmentsChain))
            {
                DocumentUnitChain attachmentChain = documentUnitChains.SingleOrDefault(x => x.ChainType == ChainType.AttachmentsChain);
                documentUnitModel.DocumentUnitChains = documentUnitModel.DocumentUnitChains.Concat(await GetAttachmentDocumentModelsAsync(attachmentChain)).ToList();
            }
            return documentUnitModel;
        }

        private async Task<ICollection<ArchiveDocument>> FindDocumentChildrenAsync(Guid idChain)
        {
            return await _documentClient.GetChildrenAsync(idChain);
        }

        private async Task<DocumentModel> GetMainDocumentModelAsync(DocumentUnitChain mainChain)
        {
            ArchiveDocument document = (await FindDocumentChildrenAsync(mainChain.IdArchiveChain)).SingleOrDefault();
            if (document == null)
            {
                throw new Exception($"Not found document in chain {mainChain.IdArchiveChain}");
            }

            return DocumentModelMapper.Map(mainChain, document);
        }

        private async Task<ICollection<DocumentModel>> GetAttachmentDocumentModelsAsync(DocumentUnitChain attachmentChain)
        {
            ICollection<ArchiveDocument> documents = await FindDocumentChildrenAsync(attachmentChain.IdArchiveChain);
            return documents.Select(s => DocumentModelMapper.Map(attachmentChain, s)).ToList();
        }
        #endregion

    }
}
