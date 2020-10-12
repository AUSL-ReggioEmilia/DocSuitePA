using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Security.Principal;
using VecompSoftware.BPM.Integrations.Modules.VSW.FascicleCustomActions.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.FascicleCustomActions.Models;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.FascicleCustomActions
{
    public class FascicleCustomAction
    {
        public bool? AutoClose { get; set; }
        public bool? AutoCloseAndClone { get; set; }
    }

    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private bool _needInitializeModule = false;
        private readonly string _username;
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
        public Execution(ILogger logger, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _webAPIClient = webAPIClient;
                _needInitializeModule = true;
                _username = string.Empty;
                if (WindowsIdentity.GetCurrent() != null)
                {
                    _username = WindowsIdentity.GetCurrent().Name;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.FascicleCustomActions -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteInfo(new LogMessage("VSW.FascicleCustomActions -> Starting execution module ..."), LogCategories);

                string filterQuery = string.Empty;
                string expandQuery = "$expand=Contacts,Category";
                string topQuery = $"$top={_moduleConfiguration.TopFascicle}";

                #region [ Managing AutoClose ]
                _logger.WriteInfo(new LogMessage("VSW.FascicleCustomActions -> Starting the close fascicles process..."), LogCategories);
                int? fascicleAutoCloseThresholdDays = _webAPIClient.GetParameterFascicleAutoCloseThresholdDaysAsync().Result;
                if (fascicleAutoCloseThresholdDays.HasValue)
                {
                    DateTimeOffset fascicleAutoCloseThresholdDate = DateTimeOffset.Now.AddDays(-fascicleAutoCloseThresholdDays.Value);
                    _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Getting fascicles with CustomActions not null and LastChangedDate <= {fascicleAutoCloseThresholdDate:dd/MM/yyyy} ..."), LogCategories);
                    filterQuery = $"$filter=CustomActions ne null and LastChangedDate le {fascicleAutoCloseThresholdDate:yyyy-MM-dd}";
                    ICollection<Fascicle> fasciclesWithCustomActions = _webAPIClient.GetFasciclesAsync($@"{filterQuery}&{expandQuery}&{topQuery}").Result;
                    foreach (Fascicle fascicle in fasciclesWithCustomActions)
                    {
                        try
                        {
                            _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Fascicle found with id: {fascicle.UniqueId}"), LogCategories);
                            FascicleCustomAction fascicleCustomAction = JsonConvert.DeserializeObject<FascicleCustomAction>(fascicle.CustomActions);
                            if (fascicleCustomAction.AutoClose.HasValue && fascicleCustomAction.AutoClose.Value)
                            {
                                fascicle.EndDate = DateTimeOffset.UtcNow;
                                Fascicle closedFascicle = _webAPIClient.PutAsync(fascicle, UpdateActionType.FascicleClose).Result;
                                _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Fascicle with id {closedFascicle.UniqueId} was closed."), LogCategories);
                            }
                            else
                            {
                                _logger.WriteInfo(new LogMessage("VSW.FascicleCustomActions -> Fascicle cannot be closed automatically. Fascicle skipped."), LogCategories);
                            }
                        }
                        catch(Exception ex)
                        {
                            _logger.WriteError(new LogMessage("VSW.FascicleCustomActions -> critical error on closing the fascicle. See error details"), ex, LogCategories);
                        }
                    }
                }
                #endregion

                #region [ Managing AutoCloseAndClone ]
                _logger.WriteInfo(new LogMessage("VSW.FascicleCustomActions -> Starting the close and clone fascicles process..."), LogCategories);
                _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Getting fascicles with CustomActions not null and the year of opening < {DateTimeOffset.Now.Year} ..."), LogCategories);
                filterQuery = $"$filter=CustomActions ne null and Year lt {DateTimeOffset.Now.Year}";
                ICollection<Fascicle> oldFasciclesWithCustomActions = _webAPIClient.GetFasciclesAsync($@"{filterQuery}&{expandQuery}&{topQuery}").Result;
                foreach(Fascicle fascicle in oldFasciclesWithCustomActions)
                {
                    try
                    {
                        _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Fascicle found with id: {fascicle.UniqueId}"), LogCategories);
                        FascicleCustomAction fascicleCustomAction = JsonConvert.DeserializeObject<FascicleCustomAction>(fascicle.CustomActions);
                        if (fascicleCustomAction.AutoCloseAndClone.HasValue && fascicleCustomAction.AutoCloseAndClone.Value)
                        {
                            fascicle.EndDate = DateTimeOffset.UtcNow;
                            Fascicle closedFascicle = _webAPIClient.PutAsync(fascicle, UpdateActionType.FascicleClose).Result;
                            _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Fascicle with id {closedFascicle.UniqueId} was closed."), LogCategories);
                            _logger.WriteInfo(new LogMessage("VSW.FascicleCustomActions -> Creating fascicle clone for closed fascicle..."), LogCategories);
                            Fascicle clonedFascicle = fascicle;
                            clonedFascicle.UniqueId = Guid.NewGuid();
                            clonedFascicle.FascicleDocumentUnits.Clear();
                            clonedFascicle.StartDate = DateTimeOffset.UtcNow;
                            clonedFascicle.Number = default(int);
                            clonedFascicle.EndDate = null;
                            clonedFascicle = _webAPIClient.PostAsync(clonedFascicle).Result;
                            _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Fascicle with id {clonedFascicle.UniqueId} was created successfully."), LogCategories);
                            _logger.WriteInfo(new LogMessage("VSW.FascicleCustomActions -> Linking cloned fascicle to closed fascicle..."), LogCategories);
                            FascicleLink fascicleLink = new FascicleLink()
                            {
                                FascicleLinkType = FascicleLinkType.Automatic,
                                Fascicle = clonedFascicle,
                                FascicleLinked = closedFascicle
                            };
                            fascicleLink = _webAPIClient.PostAsync<FascicleLink>(fascicleLink).Result;
                            _logger.WriteInfo(new LogMessage($"VSW.FascicleCustomActions -> Cloned fascicle was linked to closed fascicle successfully. Id {fascicleLink.UniqueId}"), LogCategories);
                        }
                        else
                        {
                            _logger.WriteInfo(new LogMessage("VSW.FascicleCustomActions -> Fascicle cannot be closed automatically and cloned. Fascicle skipped."), LogCategories);
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.WriteError(new LogMessage("VSW.FascicleCustomActions -> critical error on closing and cloning the fascicle. See error details"), ex, LogCategories);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.FascicleCustomActions -> critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.FascicleCustomActions"), LogCategories);
        }
        #endregion
    }
}
