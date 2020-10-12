using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Commons;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose
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
        private readonly IDictionary<FascicleType, UpdateActionType> _closeFascicleActionType = new Dictionary<FascicleType, UpdateActionType>
        {
            { FascicleType.Activity, UpdateActionType.ActivityFascicleClose },
            { FascicleType.Period, UpdateActionType.PeriodicFascicleClose },
            { FascicleType.Procedure, UpdateActionType.FascicleClose }
        };
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
                _logger.WriteError(new LogMessage("VSW.FascicleClose -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("FascicleClose -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.FascicleClose"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventDeleteCategoryFascicle>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.CategoryFascicleDeleteSubscription, CategoryFascicleDeleteCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowActionFascicleClose>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowFascicleCloseSubscription, WorkflowFascicleCloseCallback));

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

        private async Task CategoryFascicleDeleteCallback(IEventDeleteCategoryFascicle evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("CategoryFascicleDeleteCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                if (evt.ContentType == null || evt.ContentType.ContentTypeValue == null)
                {
                    _logger.WriteError(new LogMessage("CategoryFascicleDeleteCallback -> CategoryFascicle is null"), LogCategories);
                    throw new Exception("Non e' presente un CategoryFascicle nell'evento di chiusura");
                }
                CategoryFascicle categoryFascicle = evt.ContentType.ContentTypeValue;
                ICollection<Fascicle> activeFascicles = await GetActiveFasciclesAsync(categoryFascicle);

                _logger.WriteInfo(new LogMessage($"CategoryFascicleDeleteCallback -> found {activeFascicles.Count} active fascicle(s) to close"), LogCategories);
                await CloseFasciclesAsync(activeFascicles);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("CategoryFascicleDeleteCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private async Task<ICollection<Fascicle>> GetActiveFasciclesAsync(CategoryFascicle categoryFascicle)
        {
            if (categoryFascicle == null)
            {
                _logger.WriteError(new LogMessage("GetPeriodicFascicle -> CategoryFascicle is null"), LogCategories);
                throw new Exception("Non e' presente un CategoryFascicle nell'evento di fascicolazione");
            }

            ICollection<Fascicle> activeFascicles = await _webAPIClient.GetFasciclesAsync($@"$filter=Category/EntityShortId eq {categoryFascicle.Category.EntityShortId} 
                and DSWEnvironment eq {categoryFascicle.DSWEnvironment} and EndDate eq null and FascicleType eq '{categoryFascicle.FascicleType}'&$expand=Contacts");
            return activeFascicles;
        }

        private async Task CloseFasciclesAsync(IEnumerable<Fascicle> fascicles)
        {
            foreach (Fascicle fascicle in fascicles)
            {
                fascicle.EndDate = DateTimeOffset.Now;
                await _webAPIClient.PutAsync(fascicle, GetCloseActionTypeByFascicle(fascicle));
                _logger.WriteInfo(new LogMessage($"CloseFasciclesAsync -> fascicle {fascicle.Title} closed"), LogCategories);
            }
        }

        private UpdateActionType GetCloseActionTypeByFascicle(Fascicle fascicle)
        {
            if (!_closeFascicleActionType.ContainsKey(fascicle.FascicleType))
            {
                return UpdateActionType.FascicleClose;
            }
            return _closeFascicleActionType[fascicle.FascicleType];
        }

        private async Task WorkflowFascicleCloseCallback(IEventWorkflowActionFascicleClose evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("WorkflowFascicleCloseCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                if (evt.ContentType == null || evt.ContentType.ContentTypeValue == null)
                {
                    _logger.WriteError(new LogMessage("WorkflowFascicleCloseCallback -> FascicleModel is null"), LogCategories);
                    throw new Exception("Non e' presente un FascicleModel nell'evento di chiusura");
                }

                DocSuiteWeb.Model.Entities.Fascicles.FascicleModel fascicleModel = (evt.ContentType.ContentTypeValue as WorkflowActionFascicleCloseModel).GetReferenced();                
                Fascicle fascicle = await _webAPIClient.GetFascicleAsync($"$filter=UniqueId eq {fascicleModel.UniqueId}&$expand=Contacts");
                if (fascicle == null)
                {
                    _logger.WriteError(new LogMessage($"WorkflowFascicleCloseCallback -> Fascicle {fascicleModel.UniqueId} not found"), LogCategories);
                    throw new Exception($"Non e' stato trovato un fascicolo con id {fascicleModel.UniqueId}");
                }
                await CloseFasciclesAsync(new List<Fascicle>() { fascicle });
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowFascicleCloseCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }
        #endregion
    }
}
