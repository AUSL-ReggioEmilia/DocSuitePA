using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowLogging.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowLogging.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowLogging
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
                _logger.WriteError(new LogMessage("VSW.WorkflowLogging -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("WorkflowLogging -> critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.WorkflowLogging"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowNotificationError>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowLoggingNotificationErrorSubscription, EventWorkflowNotificationErrorCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowNotificationInfo>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowLoggingNotificationInfoSubscription, EventWorkflowNotificationInfoCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowNotificationWarning>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowLoggingNotificationWarningSubscription, EventWorkflowNotificationWarningCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowStartRequestDone>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowLoggingStartRequestDoneSubscription, EventWorkflowStartRequestDoneCallback));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowStartRequestError>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration,
                    _moduleConfiguration.WorkflowLoggingNotificationErrorSubscription, EventWorkflowStartRequestErrorCallback));

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

        private async Task EventWorkflowNotificationErrorCallback(IEventWorkflowNotificationError evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("EventWorkflowNotificationErrorCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                WorkflowNotification workflowNotification = evt.ContentType.ContentTypeValue;

                WorkflowActivity wfActivity = await GetWorkflowActivityFromNotificationAsync(workflowNotification, "EventWorkflowNotificationErrorCallback");

                await CreateWorkflowInstanceLog(wfActivity.WorkflowInstance, WorkflowInstanceLogType.WFRefused, SeverityLog.Error, workflowNotification.Description);
                await CreateWorkflowActivityLog(wfActivity, WorkflowStatus.Error, SeverityLog.Error, workflowNotification.Description);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowNotificationErrorCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventWorkflowNotificationInfoCallback(IEventWorkflowNotificationInfo evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("EventWorkflowNotificationInfoCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                WorkflowNotification workflowNotification = evt.ContentType.ContentTypeValue;

                WorkflowActivity wfActivity = await GetWorkflowActivityFromNotificationAsync(workflowNotification, "EventWorkflowNotificationInfoCallback");

                await CreateWorkflowInstanceLog(wfActivity.WorkflowInstance, WorkflowInstanceLogType.Information, SeverityLog.Info, workflowNotification.Description);
                await CreateWorkflowActivityLog(wfActivity, WorkflowStatus.Done, SeverityLog.Info, workflowNotification.Description);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowNotificationInfoCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventWorkflowNotificationWarningCallback(IEventWorkflowNotificationWarning evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("EventWorkflowNotificationWarningCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                WorkflowNotification workflowNotification = evt.ContentType.ContentTypeValue;

                WorkflowActivity wfActivity = await GetWorkflowActivityFromNotificationAsync(workflowNotification, "EventWorkflowNotificationWarningCallback");

                await CreateWorkflowInstanceLog(wfActivity.WorkflowInstance, WorkflowInstanceLogType.Information, SeverityLog.Warning, workflowNotification.Description);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowNotificationWarningCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventWorkflowStartRequestDoneCallback(IEventWorkflowStartRequestDone evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("EventWorkflowStartRequestDoneCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                WorkflowRequestStatus workflowRequestStatus = evt.ContentType.ContentTypeValue;

                await CreateWorkflowInstanceLog(new WorkflowInstance(workflowRequestStatus.WorkflowInstanceId), WorkflowInstanceLogType.WFStarted, SeverityLog.Info, workflowRequestStatus.Description);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowStartRequestDoneCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventWorkflowStartRequestErrorCallback(IEventWorkflowStartRequestError evt, IDictionary<string, object> properties)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("EventWorkflowStartRequestErrorCallback -> evaluate event id ", evt.Id)), LogCategories);

            try
            {
                WorkflowRequestStatus workflowRequestStatus = evt.ContentType.ContentTypeValue;

                await CreateWorkflowInstanceLog(new WorkflowInstance(workflowRequestStatus.WorkflowInstanceId), WorkflowInstanceLogType.WFRefused, SeverityLog.Error, workflowRequestStatus.Description);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventWorkflowStartRequestErrorCallback -> error complete call"), ex, LogCategories);
                throw;
            }
        }

        private async Task CreateWorkflowInstanceLog(WorkflowInstance workflowInstance, WorkflowInstanceLogType logType, SeverityLog severity, string description)
        {
            WorkflowInstanceLog workflowInstanceLog = new WorkflowInstanceLog
            {
                Entity = workflowInstance,
                SystemComputer = Environment.MachineName,
                LogType = logType,
                Severity = severity,
                LogDescription = description
            };

            await _webAPIClient.PostAsync(workflowInstanceLog, retryPolicyEnabled: false);
            _logger.WriteInfo(new LogMessage($"WorkflowInstanceLog {workflowInstanceLog.UniqueId} has been successfully inserted"), LogCategories);
        }

        private async Task CreateWorkflowActivityLog(WorkflowActivity workflowActivity, WorkflowStatus logType, SeverityLog severityLog, string description)
        {
            WorkflowActivityLog workflowActivityLog = new WorkflowActivityLog
            {
                Entity = workflowActivity,
                SystemComputer = Environment.MachineName,
                LogType = logType,
                Severity = severityLog,
                LogDescription = description
            };

            await _webAPIClient.PostAsync(workflowActivityLog, retryPolicyEnabled: false);
            _logger.WriteInfo(new LogMessage($"WorkflowActivityLog {workflowActivityLog.Entity.UniqueId} referenced {workflowActivityLog.Entity.WorkflowInstance?.UniqueId} to has been successfully inserted"), LogCategories);
        }

        private async Task<WorkflowActivity> GetWorkflowActivityFromNotificationAsync(WorkflowNotification workflowNotification, string callbackName)
        {
            if (workflowNotification == null)
            {
                _logger.WriteError(new LogMessage($"{callbackName} -> WorkflowNotification is null"), LogCategories);
                throw new ArgumentNullException("WorkflowNotification", "WorkflowNotification is null.");
            }

            WorkflowActivity workflowActivity = await _webAPIClient.GetWorkflowActivityAsync(workflowNotification.IdWorkflowActivity, "$expand=WorkflowInstance");

            if (workflowActivity == null)
            {
                _logger.WriteError(new LogMessage($"{callbackName} -> WorkflowActivity with id {workflowNotification.IdWorkflowActivity} not found"), LogCategories);
                throw new ArgumentNullException("WorkflowActivity", "WorkflowActivity is null.");
            }

            return workflowActivity;
        }

        #endregion
    }
}
