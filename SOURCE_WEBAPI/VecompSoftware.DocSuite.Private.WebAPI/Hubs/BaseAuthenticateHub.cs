using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;

namespace VecompSoftware.DocSuite.Private.WebAPI.Hubs
{
    [LogCategory(LogCategoryDefinition.SIGNALR)]
    [Authorize]

    public abstract class BaseAuthenticateHub : Hub
    {
        #region [ Fields ]

        private readonly ITopicService _topicService;
        private readonly ILogger _logger;
        private readonly IParameterEnvService _parameterEnvService;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly IMessageConfiguration _messageConfiguration;
        private static readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
        private readonly IDictionary<string, ServiceBusMessageConfiguration> _messageMappings;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Const ]
        private const string TOPIC_CONFIGURATION_NOT_EXIST = "La Topic {0} specificata non è presente nel file di configurazione.";
        #endregion

        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BaseAuthenticateHub));
                }
                return _logCategories;
            }
        }

        protected ILogger Logger => _logger;
        protected ITopicService TopicService => _topicService;
        protected IParameterEnvService ParameterEnvService => _parameterEnvService;
        protected IDataUnitOfWork UnitOfWork => _unitOfWork;
        #endregion

        #region [ Constructor ]

        public BaseAuthenticateHub()
        {
            _topicService = (ITopicService)UnityConfig.GetConfiguredContainer().GetService(typeof(ITopicService));
            _logger = (ILogger)UnityConfig.GetConfiguredContainer().GetService(typeof(ILogger));
            _parameterEnvService = (IParameterEnvService)UnityConfig.GetConfiguredContainer().GetService(typeof(IParameterEnvService));
            _messageConfiguration = (IMessageConfiguration)UnityConfig.GetConfiguredContainer().GetService(typeof(IMessageConfiguration));
            _messageMappings = _messageConfiguration.GetConfigurations();
            _unitOfWork = (IDataUnitOfWork)UnityConfig.GetConfiguredContainer().GetService(typeof(IDataUnitOfWork));
        }
        #endregion

        #region [ Methods ]

        protected abstract IList<string> GetSubscriptionNames();

        protected async Task<bool> SubscriptionConfigurationExistsAsync(string messageConfigurationName, string correlationId)
        {
            string topicName = _messageMappings[messageConfigurationName].TopicName;
            string dynamicSubscriptionName = $"{messageConfigurationName}_{correlationId}";

            return await _topicService.SubscriptionExists(topicName, GetSafeSubscriptionName(dynamicSubscriptionName));
        }

        protected async Task SubscribeTopicAsync(string messageConfigurationName, string correlationId, string commandName, Action<ServiceBusMessage> callback)
        {
            try
            {
                if (!_messageMappings.ContainsKey(messageConfigurationName))
                {
                    LogMessage message = new LogMessage(string.Format(TOPIC_CONFIGURATION_NOT_EXIST, messageConfigurationName));
                    _logger.WriteWarning(message, LogCategories);
                    return;
                }
                string topicName = _messageMappings[messageConfigurationName].TopicName;
                string dynamicSubscriptionName = $"{messageConfigurationName}_{correlationId}";
                await _topicService.SubscribeTopicAsync(topicName, GetSafeSubscriptionName(dynamicSubscriptionName), commandName, correlationId, callback);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("SubscribeTopicAsync - ", ex.Message)), ex, LogCategories);
            }
        }

        protected void SendClientResponse(ServiceBusMessage result, Action<dynamic, ServiceBusMessage> action)
        {
            ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                if (result == null)
                {
                    _logger.WriteWarning(new LogMessage("SendClientResponse result is null"), LogCategories);
                    return;
                }
                dynamic modeCommunication = Clients.All;
                if (string.IsNullOrEmpty(result.CorrelationId) || !_connections.Any(f => f.Value.Equals(result.CorrelationId)) || (modeCommunication = Clients.Client(_connections.Single(f => f.Value.Equals(result.CorrelationId)).Key)) == null)
                {
                    _logger.WriteWarning(new LogMessage("SendClientResponse unknown correlationId. Setted broadcast mode communication"), LogCategories);
                    modeCommunication = Clients.All;
                }
                else
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("SendClientResponse found CorrelationId identifier -> ", result.CorrelationId, ". Setted direct mode communication")), LogCategories);
                }
                action(modeCommunication, result);
            }, _logger, _logCategories);
        }

        protected void SendClientResponse<T>(T result, string correlationId, Action<dynamic, T> action) where T : class
        {
            ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                if (result == null)
                {
                    _logger.WriteWarning(new LogMessage("SendClientResponse result is null"), LogCategories);
                    return;
                }
                dynamic modeCommunication = Clients.All;
                if (string.IsNullOrEmpty(correlationId)
                    || !_connections.Any(f => f.Value.Equals(correlationId))
                    || (modeCommunication = Clients.Client(_connections.Single(f => f.Value.Equals(correlationId)).Key)) == null)
                {
                    _logger.WriteWarning(new LogMessage("SendClientResponse unknown correlationId. Setted broadcast mode communication"), LogCategories);
                    modeCommunication = Clients.All;
                }
                else
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("SendClientResponse found CorrelationId identifier -> ", correlationId, ". Setted direct mode communication")), LogCategories);
                }
                action(modeCommunication, result);
            }, _logger, _logCategories);
        }

        private bool TryAdd(string connectionId, string correlationId)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("Connect with ConnectionId ", Context.ConnectionId, " and CollerationId ", correlationId)), LogCategories);
            if (!_connections.TryAdd(Context.ConnectionId, correlationId))
            {
                _logger.WriteError(new LogMessage(string.Concat("Concurrent Exception in TryAdd ConnectionId ", Context.ConnectionId, " and CollerationId ", correlationId)), LogCategories);
                return false;
            }
            return true;
        }

        private void CheckSecurity()
        {
            if (Context.User == null || Context.User.Identity == null || !Context.User.Identity.IsAuthenticated)
            {
                throw new ArgumentException("WebSocket connection is not authenticated");
            }
        }

        public override Task OnConnected()
        {
            CheckSecurity();
            string userName = Context.User.Identity.Name;
            string correlationId = Context.QueryString["correlationId"];
            _logger.WriteDebug(new LogMessage($"Connect with ConnectionId {Context.ConnectionId} from User {userName} with CorrelationId {correlationId}"), LogCategories);
            if (!TryAdd(Context.ConnectionId, correlationId))
            {
                throw new ArgumentException("WebSocket connection failed");
            }

            return base.OnConnected();
        }

        private string GetSafeSubscriptionName(string name)
        {
            return string.Join(string.Empty, name.Take(50));
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            string correlationId = RemoveConnection();

            if (!string.IsNullOrEmpty(correlationId))
            {
                await RemoveSubscriptionsForAsync(correlationId);
            }

            await base.OnDisconnected(stopCalled);
        }

        //Reason : In Workflow Hub I overwritten the OnDisconnected method because I don't want to use await RemoveSubscriptionsForAsync(correlationId)
        //The service bus subsribers will work continuously until the end of processing. In this case the new OnDisconnected method calls RemoveConnection
        //to only remove the connectionId from _connections.
        protected string RemoveConnection()
        {
            string correlationId = Context.QueryString["correlationId"];

            if (!_connections.TryRemove(Context.ConnectionId, out correlationId))
            {
                _logger.WriteWarning(new LogMessage(string.Concat("Concurrent Exception in TryRemove ConnectionId ", Context.ConnectionId, " to CorrelationId ", correlationId)), LogCategories);
            }

            return correlationId;
        }

        protected async Task RemoveSubscriptionsForAsync(string correlationId)
        {
            IList<string> subscriptionNames = GetSubscriptionNames();
            string subscriptionNameEvaluated;
            string topicName;
            foreach (string subscriptionName in subscriptionNames)
            {
                subscriptionNameEvaluated = $"{subscriptionName}_{correlationId}";
                topicName = _messageMappings[subscriptionName].TopicName;
                await _topicService.UnsubscribeTopicAsync(topicName, GetSafeSubscriptionName(subscriptionNameEvaluated));
            }
        }

        public override Task OnReconnected()
        {
            CheckSecurity();

            string correlationId = Context.QueryString["correlationId"];
            if (!_connections.ContainsKey(Context.ConnectionId))
            {
                if (!TryAdd(Context.ConnectionId, correlationId))
                {
                    throw new ArgumentException("WebSocket connection failed");
                }
            }
            return base.OnReconnected();
        }
        #endregion
    }
}