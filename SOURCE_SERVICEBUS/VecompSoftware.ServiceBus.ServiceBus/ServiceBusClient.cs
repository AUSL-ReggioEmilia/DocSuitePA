using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.ServiceBus.ServiceBus
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class ServiceBusClient : IServiceBusClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private static Lazy<string> _workflowAggregationTopicName => new Lazy<string>(() => ConfigurationManager.AppSettings["WorkflowAggregationTopicName"]);
        public const string SERVICE_BUS_CORRELATIONID_FILTER = "sys.CorrelationId = '{0}'";
        #endregion

        #region [ Properties ]
        public static string WorkflowAggregationTopicName => _workflowAggregationTopicName.Value;
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ServiceBusClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public ServiceBusClient(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public async Task CreateSubscriptionAsync(string topicName, string subscriptionName, string filter, bool enableSession = false)
        {
            try
            {
                _logger.WriteInfo(new LogMessage($"Creating subscription..."), LogCategories);
                NamespaceManager manager = NamespaceManager.Create();
                if (manager.SubscriptionExists(topicName, subscriptionName))
                {
                    _logger.WriteInfo(new LogMessage($"Subscription {subscriptionName} already exists in topic {topicName}. No action required."), LogCategories);
                    return;
                }
                SqlFilter sqlFilter;
                if (!string.IsNullOrEmpty(filter))
                {
                    sqlFilter = new SqlFilter(filter);
                    _logger.WriteInfo(new LogMessage($"Create subscription name {subscriptionName} with filters {sqlFilter.SqlExpression}"), LogCategories);
                    await manager.CreateSubscriptionAsync(new SubscriptionDescription(topicName, subscriptionName) { RequiresSession = enableSession }, sqlFilter);
                }
                else
                {
                    _logger.WriteInfo(new LogMessage($"Create subscription name {subscriptionName}"), LogCategories);
                    await manager.CreateSubscriptionAsync(new SubscriptionDescription(topicName, subscriptionName) { RequiresSession = enableSession });
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task DeleteSubscriptionAsync(string topicName, string subscriptionName)
        {
            try
            {
                _logger.WriteInfo(new LogMessage("Deleting subscription..."), LogCategories);
                NamespaceManager manager = NamespaceManager.Create();

                if (string.IsNullOrEmpty(subscriptionName))
                {
                    _logger.WriteWarning(new LogMessage("Cannot delete a subscription without specify subscription name"), LogCategories);
                    return;
                }

                _logger.WriteInfo(new LogMessage($"Delete subscription name {subscriptionName}"), LogCategories);
                await manager.DeleteSubscriptionAsync(topicName, subscriptionName);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }
        #endregion
    }
}
