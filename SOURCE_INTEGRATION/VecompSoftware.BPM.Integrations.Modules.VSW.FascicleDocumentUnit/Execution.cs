using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.FascicleDocumentUnit.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.FascicleDocumentUnit.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.FascicleDocumentUnit
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.FascicleDocumentUnit -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("ActionFascicle -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.FascicleDocumentUnit"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowActionFascicle>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, 
                    _moduleConfiguration.DocumentUnitIntoFascicleSubscription, DocumentUnitIntoFascicleCallback));

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

        private async Task DocumentUnitIntoFascicleCallback(IEventWorkflowActionFascicle evt)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("DocumentUnitIntoFascicleCallback -> evaluate event id ", evt.Id)),
                LogCategories);

            WorkflowActionFascicleModel item = (WorkflowActionFascicleModel)evt.ContentType.ContentTypeValue;
            FascicleModel fascicleModel = item.GetFascicle();
            DocumentUnitModel referenced = item.GetReferenced();
            FascicleFolderModel fascicleFolderModel = item.GetFascicleFolder();
            Guid? fascicleFolderId = null;
            if (fascicleFolderModel != null)
            {
                fascicleFolderId = fascicleFolderModel.UniqueId;
            }
            Fascicle fascicle = await _webAPIClient.GetFascicleAsync($"$filter=UniqueId eq {fascicleModel.UniqueId}");
            if (fascicle == null)
            {
                throw new ArgumentException($"Fascicle {fascicleModel.UniqueId}/{fascicleModel.Title} not found");
            }

            await FascicolateDocumentUnitAsync(referenced.UniqueId, fascicle, fascicleFolderId);
        }

        private async Task FascicolateDocumentUnitAsync(Guid documentUnitId, Fascicle fascicle, Guid? fascicleFolderId)
        {
            try
            {
                if (documentUnitId == Guid.Empty)
                {
                    _logger.WriteError(new LogMessage("FascicolateDocumentUnit -> DocumentUnitId is empty"), LogCategories);
                    throw new Exception("DocumentUnitId is empty.");
                }

                DocSuiteWeb.Entity.Fascicles.FascicleDocumentUnit fascicleDocumentUnit = new DocSuiteWeb.Entity.Fascicles.FascicleDocumentUnit
                {
                    Fascicle = fascicle,
                    DocumentUnit = new DocumentUnit(documentUnitId),
                };
                if (!fascicleFolderId.HasValue) 
                {
                    fascicleFolderId = (await _webAPIClient.GetDefaultFascicleFolderAsync(fascicle.UniqueId)).UniqueId;
                }
                fascicleDocumentUnit.FascicleFolder = new FascicleFolder(fascicleFolderId.Value);
                await _webAPIClient.PostAsync(fascicleDocumentUnit, actionType: InsertActionType.AutomaticIntoFascicleDetection);
                _logger.WriteInfo(new LogMessage($"The DocumentUnit {documentUnitId} has been successfully inserted."), LogCategories);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("FascicolateDocumentUnit -> error complete call"), ex, LogCategories);
                throw;
            }
        }
        #endregion
    }
}
