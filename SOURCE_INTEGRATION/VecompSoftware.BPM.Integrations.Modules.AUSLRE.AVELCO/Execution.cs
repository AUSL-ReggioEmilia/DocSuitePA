using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Configurations;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;
using Newtonsoft.Json;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private readonly AVELCOClient _avelcoClient;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _needInitializeModule = true;
                _avelcoClient = new AVELCOClient(logger, _moduleConfiguration.AVELCOWebApiUrl);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLRE.AVELCO -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("AUSLRE.AVELCO -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AUSLRE.AVELCO"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventShareDocumentUnit>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartAvelcoShareDocumentUnitSubscription, EventShareDocumentUnitCallbackAsync));

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

        private async Task EventShareDocumentUnitCallbackAsync(IEventShareDocumentUnit evt)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventShareDocumentUnitCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                DocumentUnit documentUnit = evt.ContentType.ContentTypeValue;
                _logger.WriteDebug(new LogMessage($"Evaluating documentUnit {documentUnit.Title}/{documentUnit.UniqueId}"), LogCategories);
                DocSuiteEvent docSuiteEvent = new DocSuiteEvent();
                docSuiteEvent.EventDate = documentUnit.RegistrationDate;
                docSuiteEvent.WorkflowReferenceId = null; //Fase 0 da non impostare
                docSuiteEvent.EventModel = new DocSuiteModel()
                {
                    Title = documentUnit.Title,
                    UniqueId = documentUnit.UniqueId,
                    Year = documentUnit.Year,
                    Number = documentUnit.Number,
                    ModelType = DocSuiteType.Protocol,
                    ModelStatus = DocSuiteStatus.Activated
                };
                docSuiteEvent.EventModel.CustomProperties = new Dictionary<string, string>();
                docSuiteEvent.EventModel.CustomProperties.Add("AMBITO", JsonConvert.SerializeObject(documentUnit.DocumentUnitRoles.Select(f => f.UniqueIdRole)));
                docSuiteEvent.EventModel.CustomProperties.Add("UFFICIO_TERRITORIALE", JsonConvert.SerializeObject(documentUnit.Container.EntityShortId));
                await _avelcoClient.SendEventAsync(docSuiteEvent);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventShareDocumentUnitCallbackAsync -> error occouring when calling AVELCO service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        #endregion
    }
}
