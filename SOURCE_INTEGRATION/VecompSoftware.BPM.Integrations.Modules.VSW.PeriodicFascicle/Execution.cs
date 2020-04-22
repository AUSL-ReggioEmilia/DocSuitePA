using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.PeriodicFascicle.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.PeriodicFascicle.CustomExceptions;
using VecompSoftware.BPM.Integrations.Modules.VSW.PeriodicFascicle.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.PeriodicFascicle
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
                _logger.WriteError(new LogMessage("VSW.PeriodicFascicle -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("PeriodicFascicle -> Critical Error"), ex, LogCategories);
                throw;
            }
        }
        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.PeriodicFascicle"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCQRSFascicolable>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, 
                    _moduleConfiguration.PeriodicFascicolableDocumentUnitSubscription, FascicolateDocumentUnit));

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

        private async Task FascicolateDocumentUnit(IEventCQRSFascicolable evt)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("FascicolateDocumentUnit -> evaluate event id ", evt.Id)), LogCategories);

            if (evt.DocumentUnit == null)
            {
                _logger.WriteError(new LogMessage("FascicolateDocumentUnit -> DocumentUnit is null"), LogCategories);
                throw new Exception("Non e' presente una DocumentUnit nell'evento di fascicolazione");
            }

            Fascicle periodicFascicle = await GetPeriodicFascicle(evt);

            DocumentUnit documentUnit = evt.DocumentUnit;

            _logger.WriteInfo(new LogMessage($"FascicolateDocumentUnit -> Evaluating DocumentUnitId {documentUnit.UniqueId}"), LogCategories);
            await FascicolateDocumentUnitAsync(documentUnit.UniqueId, periodicFascicle);
        }

        private async Task<Fascicle> GetPeriodicFascicle(IEventCQRSFascicolable evt)
        {
            if (evt.CategoryFascicle == null)
            {
                _logger.WriteError(new LogMessage("GetPeriodicFascicle -> CategoryFascicle is null"), LogCategories);
                throw new Exception("Non e' presente un CategoryFascicle nell'evento di fascicolazione");
            }

            CategoryFascicle categoryFascicle = evt.CategoryFascicle;

            ICollection<Fascicle> periodicFascicles = await _webAPIClient.GetFasciclesAsync(string.Concat("$filter=Category/EntityShortId eq ", categoryFascicle.Category.EntityShortId, " and (Container/EntityShortId eq ", evt.DocumentUnit.Container.EntityShortId, " or Container eq null)",
                " and DSWEnvironment eq ", categoryFascicle.DSWEnvironment, " and EndDate eq null and FascicleType eq 'Period'&$expand=Category,Container,Contacts,FascicleRoles($expand=Role),MetadataRepository,FascicleFolders"));

            if (periodicFascicles.Count() < 1)
            {
                _logger.WriteError(new LogMessage("GetPeriodicFascicle -> No PeriodicFascicleFounded"), LogCategories);
                throw new PeriodicFascicleNotFoundException("Non sono stati trovati fascicoli periodici corrispondenti al piano di fascicolaizone");
            }

            if (periodicFascicles.Count() > 1)
            {
                _logger.WriteError(new LogMessage("GetPeriodicFascicle -> Too many PeriodicFascicleFounded"), LogCategories);
                throw new TooManyPeriodicFasciclesException("Sono stati trovati piu' fascicoli periodici corrispondenti al piano di fascicolaizone");
            }

            _logger.WriteInfo(new LogMessage(string.Concat("GetPeriodicFascicle -> Founded a single FascicleProtocol", periodicFascicles.First().UniqueId)), LogCategories);

            Fascicle currentPeriodicFascicle = periodicFascicles.First();
            FascicleFolder folder = currentPeriodicFascicle.FascicleFolders.FirstOrDefault(f => f.Typology == FascicleFolderTypology.Fascicle);
            if (folder == null)
            {
                _logger.WriteError(new LogMessage("GetPeriodicFascicle -> FascicleFolder is null"), LogCategories);
                throw new Exception("Il fascicolo non ha nessuna cartella di tipo Fascicolo.");
            }

            DateTimeOffset currentDate = DateTimeOffset.Now;

            if (IsPeriodicFascicleOutdated(currentPeriodicFascicle.StartDate, categoryFascicle.FasciclePeriod))
            {
                _logger.WriteInfo(new LogMessage(string.Concat("GetPeriodicFascicle -> Current PeriodicFascicle is outdated, creating a new one ", periodicFascicles.First().UniqueId)), LogCategories);
                DateTimeOffset currentDatePrevMonth = currentDate.AddMonths(-1);
                currentPeriodicFascicle.EndDate = new DateTime(currentDatePrevMonth.Year, currentDatePrevMonth.Month, DateTime.DaysInMonth(currentDatePrevMonth.Year, currentDatePrevMonth.Month));

                await _webAPIClient.PutAsync(currentPeriodicFascicle, actionType: UpdateActionType.PeriodicFascicleClose);
                Fascicle newPeriodicFascicle = new Fascicle()
                {
                    Category = currentPeriodicFascicle.Category,
                    Container = currentPeriodicFascicle.Container,
                    Conservation = currentPeriodicFascicle.Conservation,
                    Contacts = currentPeriodicFascicle.Contacts,
                    DSWEnvironment = currentPeriodicFascicle.DSWEnvironment,
                    FascicleRoles = currentPeriodicFascicle.FascicleRoles,
                    FascicleType = currentPeriodicFascicle.FascicleType,
                    Manager = currentPeriodicFascicle.Manager,
                    FascicleObject = currentPeriodicFascicle.FascicleObject,
                    MetadataRepository = currentPeriodicFascicle.MetadataRepository,
                    MetadataValues = currentPeriodicFascicle.MetadataValues,
                    Name = currentPeriodicFascicle.Name,
                    Note = currentPeriodicFascicle.Note,
                    Rack = currentPeriodicFascicle.Rack,
                    StartDate = new DateTime(currentDate.Year, currentDate.Month, 1),
                    Title = currentPeriodicFascicle.Title
                };

                return await _webAPIClient.PostAsync(newPeriodicFascicle, actionType: InsertActionType.InsertPeriodicFascicle);
            }

            return currentPeriodicFascicle;
        }

        private async Task FascicolateDocumentUnitAsync(Guid documentUnitId, Fascicle fascicle)
        {
            try
            {
                if (documentUnitId == Guid.Empty)
                {
                    _logger.WriteError(new LogMessage("FascicolateDocumentUnit -> DocumentUnitId is empty"), LogCategories);
                    throw new Exception("DocumentUnitId is empty.");
                }

                FascicleDocumentUnit fascicleDocumentUnit = new FascicleDocumentUnit
                {
                    Fascicle = fascicle,
                    DocumentUnit = new DocumentUnit(documentUnitId),
                    FascicleFolder = fascicle.FascicleFolders.First(f => f.Typology == FascicleFolderTypology.Fascicle)
                };

                await _webAPIClient.PostAsync(fascicleDocumentUnit);
                _logger.WriteInfo(new LogMessage($"The DocumentUnit {documentUnitId} has been successfully inserted."), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("FascicolateDocumentUnit -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        protected bool IsPeriodicFascicleOutdated(DateTimeOffset fascicleStartDate, FasciclePeriod period)
        {
            if (DateTimeOffset.Now.Year > fascicleStartDate.Year)
            {
                return true;
            }
            switch (period.PeriodName)
            {
                case "Semestrale":
                    {
                        if (DateTimeOffset.Now.Month >= fascicleStartDate.Month + 6)
                        {
                            return true;
                        }
                        break;
                    }
                case "Trimestrale":
                    {
                        if (DateTimeOffset.Now.Month >= fascicleStartDate.Month + 3)
                        {
                            return true;
                        }
                        break;
                    }
                case "Mensile":
                    {
                        if (DateTimeOffset.Now.Month >= fascicleStartDate.Month + 1)
                        {
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }

        #endregion
    }
}
