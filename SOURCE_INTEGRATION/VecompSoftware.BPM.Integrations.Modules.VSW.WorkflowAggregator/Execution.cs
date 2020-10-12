using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowAggregator.Configuration;
using VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowAggregator.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.Services.Command.CQRS;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowAggregator
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
        private bool _needInitializeModule = false;
        private readonly IDictionary<string, Func<IMessage, Task<ServiceBusMessage>>> _workflowActionFunctions;
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

                _workflowActionFunctions = new Dictionary<string, Func<IMessage, Task<ServiceBusMessage>>>();
                _workflowActionFunctions.Add(nameof(EventWorkflowActionDocumentUnitLink), (@event) => _webAPIClient.SendEventAsync(@event as EventWorkflowActionDocumentUnitLink));
                _workflowActionFunctions.Add(nameof(EventWorkflowActionFascicle), (@event) => _webAPIClient.SendEventAsync(@event as EventWorkflowActionFascicle));
                _workflowActionFunctions.Add(nameof(EventWorkflowActionFascicleClose), (@event) => _webAPIClient.SendEventAsync(@event as EventWorkflowActionFascicleClose));
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.WorkflowAggregator -> Critical error in costruction module"), ex, LogCategories);
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

                ICollection<string> subscriptions = _serviceBusClient.GetSubscriptionsAsync(_moduleConfiguration.TopicWorkflowAggregation).Result;
                _logger.WriteInfo(new LogMessage($"Found {subscriptions.Count} subscriptions to process"), LogCategories);
                long activeMessages;
                int subscriptionLimit;
                foreach (string subscription in subscriptions)
                {
                    try
                    {
                        _logger.WriteInfo(new LogMessage($"Process subscription {subscription}"), LogCategories);
                        activeMessages = SubscriptionActiveMessageCountAsync(subscription).Result;
                        subscriptionLimit = SubscriptionMessageLimit(subscription);
                        _logger.WriteInfo(new LogMessage($"Found {activeMessages} of {subscriptionLimit} active messages in subscription {subscription}"), LogCategories);
                        if (activeMessages < subscriptionLimit)
                        {
                            _logger.WriteInfo(new LogMessage($"Aggregation operations {subscription} not completed. No operations yet."), LogCategories);
                            continue;
                        }

                        IEventWorkflowActionAggregation @event = _serviceBusClient.GetDeadLetterEventAsync<IEventWorkflowActionAggregation>(_moduleConfiguration.TopicWorkflowAggregation, subscription).Result;
                        foreach (IMessage item in @event.CorrelatedMessages)
                        {
                            if (!_workflowActionFunctions.ContainsKey(item.GetType().Name))
                            {
                                _logger.WriteWarning(new LogMessage($"Message type {item.GetType().Name} is not manageable."), LogCategories);
                                continue;
                            }
                            _workflowActionFunctions[item.GetType().Name](item).Wait();
                            _logger.WriteInfo(new LogMessage($"Callback message sended correctly."), LogCategories);
                        }
                        _serviceBusClient.DeleteSubscriptionAsync(_moduleConfiguration.TopicWorkflowAggregation, subscription).Wait();
                        _logger.WriteInfo(new LogMessage($"Subscription {subscription} deleted correctly."), LogCategories);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"WorkflowAggregator -> Error on process subscription {subscription}"), ex, LogCategories);
                        throw ex;
                    }                    
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("WorkflowAggregator -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        private int SubscriptionMessageLimit(string subscriptionName)
        {
            string[] splittedSubscriptionName = subscriptionName.Split('_');
            if (!int.TryParse(splittedSubscriptionName[0], out int subscriptionLimit))
            {
                throw new Exception($"Il nome della sottoscrizione {subscriptionName} non è nella forma corretta");
            }
            return subscriptionLimit;
        }

        private async Task<long> SubscriptionActiveMessageCountAsync(string subscriptionName)
        {
            return await _serviceBusClient.CountSubscriptionActiveMessageAsync(_moduleConfiguration.TopicWorkflowAggregation, subscriptionName);
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.WorkflowAggregator"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _needInitializeModule = false;
            }
        }
        #endregion
    }
}
