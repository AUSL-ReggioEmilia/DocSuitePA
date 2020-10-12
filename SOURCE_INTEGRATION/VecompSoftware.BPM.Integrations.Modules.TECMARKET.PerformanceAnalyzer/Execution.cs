using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.PerformanceAnalyzer.Configuration;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.PerformanceAnalyzer.Models;
using VecompSoftware.Core.Command;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.PerformanceAnalyzer
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [Fields]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private bool _needInitializeModule = false;
        private readonly IdentityContext _identityContext = null;

        private const string PHYSICAL_DISK = "PhysicalDisk";
        private const string PROCESSOR = "Processor";
        private const string MEMORY = "Memory";

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

        private readonly Dictionary<string, EventAttributeName> _eventDataType;

        #endregion

        #region [Constructor]
        [ImportingConstructor]
        public Execution(ILogger logger)
                : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                string username = "anonymous";
                _needInitializeModule = true;

                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);

                _eventDataType = new Dictionary<string, EventAttributeName>
                {
                    {PROCESSOR, EventAttributeName.Processor},
                    {MEMORY, EventAttributeName.Memory},
                    {PHYSICAL_DISK, EventAttributeName.PhysicalDisk}
                };
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TECMARKET.PerformanceAnalyzer -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }


        }
        #endregion

        #region [Methods]
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }
            try
            {
                InitializeModule();
                foreach (PerformanceCounterModel performanceCounterModel in _moduleConfiguration.PerformanceCounters)
                {
                    if (performanceCounterModel.Counter.CategoryName.Equals(PHYSICAL_DISK))
                    {
                        EvaluateDiskPerformance(performanceCounterModel.Threshold);
                    }
                    else
                    {
                        EvaluatePerformance(performanceCounterModel.Counter, performanceCounterModel.Threshold);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TECMARKET.PerformanceAnalyzer -> Execute critical error"), ex, LogCategories);
                throw;
            }

        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _needInitializeModule = false;
            }
        }

        private void EvaluatePerformance(PerformanceCounter currentUsage, int threshold)
        {
            _logger.WriteDebug(new LogMessage($"Evaluate performance: {currentUsage.CategoryName}"), LogCategories);
            float currentPerformance = currentUsage.NextValue();
            if (currentPerformance > threshold)
            {
                //Write EventLog in EventViewer
                _logger.WriteDebug(new LogMessage("TECMARKET.PerformanceAnalyzer -> Create new event log"), LogCategories);

                string errorMessage = $"{currentUsage.CategoryName} usage ( {string.Format("{0:0.00}", currentPerformance)} %) is bigger than expected: {string.Format("{0:0.00}", threshold)} %";

                EventInstance eventInstance = new EventInstance(0, 0, EventLogEntryType.Error);
                EventLog.WriteEvent(_moduleConfiguration.DestinationEventSource, eventInstance, new object[] { errorMessage, _eventDataType[currentUsage.CategoryName] });

                _logger.WriteInfo(new LogMessage("Event Log successfully written in Event Viewer"), LogCategories);
            }
        }

        private void EvaluateDiskPerformance(int threshold)
        {
            _logger.WriteDebug(new LogMessage($"Evaluate performance: Disk "), LogCategories);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                {
                    double availableFreeSpace = Math.Round(drive.AvailableFreeSpace / 1024d / 2024d / 1024d, 2);
                    double totalSize = Math.Round(drive.TotalFreeSpace / 1024d / 1024d / 1024d, 2);
                    double usedSpace = Math.Round(totalSize - availableFreeSpace, 2);
                    double percentageUsedSpace = Math.Round((usedSpace * 100d) / totalSize, 2);
                    if (percentageUsedSpace > threshold)
                    {
                        //Write EventLog in EventViewer 
                        _logger.WriteDebug(new LogMessage("TECMARKET.PerformanceAnalyzer -> Create new event log"), LogCategories);

                        string errorMessage = $"Disk usage ({string.Format("{0:0.00}", percentageUsedSpace)} %) of {drive.Name} is bigger than expected: {string.Format("{0:0.00}", threshold)} %";
                        EventInstance eventInstance = new EventInstance(0, 0, EventLogEntryType.Error);
                        EventLog.WriteEvent(_moduleConfiguration.DestinationEventSource, eventInstance, new object[] { errorMessage, _eventDataType[PHYSICAL_DISK] });
                        _logger.WriteInfo(new LogMessage("Event Log successfully written in Event Viewer"), LogCategories);
                        return;
                    }
                }
            }
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> TECMARKET.PerformanceAnalyzer"), LogCategories);
        }
        #endregion
    }
}
