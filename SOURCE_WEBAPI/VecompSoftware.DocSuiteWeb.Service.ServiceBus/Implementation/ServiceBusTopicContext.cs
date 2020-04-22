using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Validation;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public class ServiceBusTopicContext : ServiceBusContext, IServiceBusTopicContext, IDisposable
    {
        #region [ Fields ]
        private const string SUBSCRIPTION_CORRELATION_FILTER = "sys.CorrelationId='{0}'";
        private const string TOPIC_IS_EMPTY = "ServiceBusTopicContext : Topic has not been defined";
        private const string TOPIC_NOT_EXIST = "Nessuna topic configurata con il nome {0}";
        private const string CONTEXT_NOT_INITIALIZED = "Nessun context inizializzato per la richiesta. Effettuare un BeginContextAsync prima di un PublishAsync.";
        private const string SUBSCRIPTION_NOT_INITIALIZE = "ServiceBusTopicContext: Subscription has not been defined";
        private const string CORRELATIONID_NOT_INITIALIZE = "ServiceBusTopicContext: CorrelationId has not been defined";
        private const string DEFAULTFILTER_NOT_INITIALIZE = "ServiceBusTopicContext: DefaultFilter has not been defined";

        private readonly ILogger _logger;
        private TopicClient _topicClient = null;
        private readonly Dictionary<string, SubscriptionClient> _subscriptionClient = new Dictionary<string, SubscriptionClient>();
        private readonly MessagingFactory _messagingFactory;
        private static readonly OnMessageOptions _messageOptions = new OnMessageOptions() { AutoComplete = false };
        private bool _disposed;
        #endregion

        #region [ Properties ]

        private bool IsContextInitialized => _topicClient == null ? false : !_topicClient.IsClosed;

        private bool IsSubscriptionInitialized => _subscriptionClient.Count > 0;

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        #endregion

        #region [ Dispose ]

        protected new virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_topicClient != null)
                {
                    _topicClient.Close();
                }

                if (_subscriptionClient != null)
                {
                    foreach (KeyValuePair<string, SubscriptionClient> subscriptionClient in _subscriptionClient)
                    {
                        _logger.WriteInfo(new LogMessage(string.Concat("Subscription ", subscriptionClient.Key, " has been deactivated")), LogCategories);
                        subscriptionClient.Value.Close();
                    }
                    _subscriptionClient.Clear();
                }
                _disposed = true;
            }
        }
        #endregion

        #region [ Constructor ]

        public ServiceBusTopicContext(IServiceBusConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
            _logger = logger;
            _messagingFactory = MessagingFactory.CreateFromConnectionString(configuration.ConnectionString);
        }
        #endregion

        #region [ Methods ]
        public async Task<IServiceBusContext> BeginContextAsync(string topicName, string subscriptionName = "", string filter = "")
        {
            TopicName = topicName;
            if (string.IsNullOrEmpty(TopicName))
            {
                throw new DSWException(TOPIC_IS_EMPTY, null, DSWExceptionCode.SS_Mapper);
            }
            if (!await NamespaceManager.TopicExistsAsync(topicName))
            {
                string message = string.Format(TOPIC_NOT_EXIST, topicName);
                _logger.WriteError(new LogMessage(message), LogCategories);
                throw new DSWValidationException(message,
                    new ReadOnlyCollection<ValidationMessageModel>(new Collection<ValidationMessageModel>() { new ValidationMessageModel() { Message = message, Key = "TOPIC_NOT_EXIST" } }), null, DSWExceptionCode.SS_Mapper);
            }

            if (!IsContextInitialized || !_topicClient.Path.EndsWith(string.Concat("/", topicName)))
            {
                if (IsContextInitialized)
                {
                    _topicClient.Close();
                    _topicClient = null;
                }
                _topicClient = TopicClient.CreateFromConnectionString(ServiceBusConfiguration.ConnectionString, TopicName);

                if (!string.IsNullOrEmpty(subscriptionName))
                {
                    SubscriptionName = subscriptionName;
                    await InitializeSubscriptionAsync(subscriptionName, filter);
                }

            }
            return this;
        }

        public async Task<IServiceBusContext> CreateSubscriptionAsync(string subscriptionName, string correlationId, string defaultFilter, Action<BrokeredMessage> callback)
        {
            if (string.IsNullOrEmpty(subscriptionName))
            {
                throw new DSWException(SUBSCRIPTION_NOT_INITIALIZE, null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(correlationId))
            {
                throw new DSWException(CORRELATIONID_NOT_INITIALIZE, null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(defaultFilter))
            {
                throw new DSWException(DEFAULTFILTER_NOT_INITIALIZE, null, DSWExceptionCode.SS_Mapper);
            }

            if (callback != null)
            {
                await InitializeSubscriptionAsync(subscriptionName, correlationId, defaultFilter, callback);
            }
            return this;
        }

        public async Task<IServiceBusContext> RemoveSubscriptionAsync(string subscriptionName)
        {
            if (string.IsNullOrEmpty(subscriptionName))
            {
                throw new DSWException(SUBSCRIPTION_NOT_INITIALIZE, null, DSWExceptionCode.SS_Mapper);
            }

            if (_subscriptionClient.ContainsKey(subscriptionName))
            {
                SubscriptionClient subscriptionClient = _subscriptionClient[subscriptionName];
                if (!subscriptionClient.IsClosed)
                {
                    await subscriptionClient.CloseAsync();
                    _logger.WriteInfo(new LogMessage(string.Concat("Subscription ", subscriptionName, " in topic ", TopicName, " has been removed")), LogCategories);

                    _subscriptionClient.Remove(subscriptionName);
                }
            }
            return this;
        }

        /// <summary>
        /// Send a message to a topic
        /// </summary>
        public override async Task<BrokeredMessage> PublishAsync(BrokeredMessage message)
        {
            if (_disposed)
            {
                return null;
            }

            if (!IsContextInitialized)
            {
                _logger.WriteError(new LogMessage(CONTEXT_NOT_INITIALIZED), LogCategories);
                throw new DSWException(CONTEXT_NOT_INITIALIZED, null, DSWExceptionCode.SS_Anomaly);
            }

            InitializeMessage(message);
            KeyValuePair<string, object> eventName = message.Properties.SingleOrDefault(f => CustomPropertyName.EVENT_NAME.Equals(f.Key));
            _logger.WriteDebug(new LogMessage($"Sending message '{eventName.Value}' to topic {TopicName}"), LogCategories);

            await _topicClient.SendAsync(message);
            return message;
        }

        /// <summary>
        /// Get messages from topic into specific subscription
        /// </summary>
        public override async Task<ICollection<BrokeredMessage>> GetMessagesAsync()
        {
            if (!IsContextInitialized)
            {
                _logger.WriteError(new LogMessage(CONTEXT_NOT_INITIALIZED), LogCategories);
                throw new DSWException(CONTEXT_NOT_INITIALIZED, null, DSWExceptionCode.SS_Anomaly);
            }

            if (!IsSubscriptionInitialized)
            {
                _logger.WriteError(new LogMessage(CONTEXT_NOT_INITIALIZED), LogCategories);
                throw new DSWException(SUBSCRIPTION_NOT_INITIALIZE, null, DSWExceptionCode.SS_Anomaly);

            }
            string subscriptionName = _subscriptionClient.Keys.First();
            SubscriptionDescription subscriptionDescription = await GetSubscriptionAsync(subscriptionName);
            int activeMessageCount = (int)subscriptionDescription.MessageCountDetails.ActiveMessageCount;
            IEnumerable<BrokeredMessage> messages = await _subscriptionClient[subscriptionName].PeekBatchAsync(activeMessageCount);
            if (messages == null)
            {
                return new List<BrokeredMessage>();
            }

            return messages.ToList();
        }

        /// <summary>
        /// Get topic status
        /// </summary>
        /// <returns></returns>
        public override async Task<EntityStatus> GetStatusAsync()
        {
            TopicDescription topic = await GetTopicAsync();
            return topic.Status;
        }

        private async Task InitializeSubscriptionAsync(string subscriptionName, string filter = "")
        {
            TopicDescription topic = await NamespaceManager.GetTopicAsync(TopicName);
            if (!await NamespaceManager.SubscriptionExistsAsync(topic.Path, subscriptionName))
            {
                return;
            }

            if (!_subscriptionClient.ContainsKey(subscriptionName))
            {
                SubscriptionClient client = SubscriptionClient.Create(TopicName, subscriptionName, ReceiveMode.PeekLock);
                if (!string.IsNullOrEmpty(filter))
                {
                    client.AddRule(new RuleDescription
                    {
                        Name = "localrule",
                        Filter = new SqlFilter(filter),
                    });
                }
                _logger.WriteInfo(new LogMessage(string.Concat("Subscription ", subscriptionName, " has been activated")), LogCategories);
                _subscriptionClient.Add(subscriptionName, client);
            }
        }

        private async Task InitializeSubscriptionAsync(string subscriptionName, string correlationId, string defaultFilter, Action<BrokeredMessage> callback)
        {
            TopicDescription topic = await NamespaceManager.GetTopicAsync(TopicName);
            if (await NamespaceManager.SubscriptionExistsAsync(topic.Path, subscriptionName))
            {
                return;
            }

            if (!_subscriptionClient.ContainsKey(subscriptionName))
            {
                RuleDescription defaultRuleDescription = new RuleDescription()
                {
                    Filter = new SqlFilter(string.Concat(string.Format(SUBSCRIPTION_CORRELATION_FILTER, correlationId), " AND ", defaultFilter)),
                    Name = RuleDescription.DefaultRuleName
                };
                SubscriptionDescription subscriptionDescription = new SubscriptionDescription(topic.Path, subscriptionName)
                {
                    AutoDeleteOnIdle = ServiceBusConfiguration.AutoDeleteOnIdle,
                    DefaultMessageTimeToLive = ServiceBusConfiguration.DefaultMessageTimeToLive,
                    EnableBatchedOperations = false,
                    EnableDeadLetteringOnFilterEvaluationExceptions = true,
                    EnableDeadLetteringOnMessageExpiration = false,
                    LockDuration = ServiceBusConfiguration.LockDuration,
                    MaxDeliveryCount = ServiceBusConfiguration.MaxDeliveryCount,
                    Name = subscriptionName
                };
                subscriptionDescription = await NamespaceManager.CreateSubscriptionAsync(subscriptionDescription, defaultRuleDescription);
                _logger.WriteInfo(new LogMessage(string.Concat("Subscription ", subscriptionName, " in topic ", TopicName, " has been created")),
                    LogCategories);
                SubscriptionClient client = SubscriptionClient.Create(TopicName, subscriptionName, ReceiveMode.PeekLock);
                client.OnMessageAsync(async (message) => await EvaluateMessageAsync(message, callback), _messageOptions);
                _logger.WriteInfo(new LogMessage(string.Concat("Subscription ", subscriptionName, " has been activated")), LogCategories);

                _subscriptionClient.Add(subscriptionName, client);
            }
        }

        private async Task EvaluateMessageAsync(BrokeredMessage brokeredMessage, Action<BrokeredMessage> action)
        {
            try
            {
                if (brokeredMessage != null)
                {
                    _logger.WriteInfo(new LogMessage(string.Concat("receive event ->", brokeredMessage.MessageId)), LogCategories);
                    action(brokeredMessage);
                }

                await brokeredMessage.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                await brokeredMessage.DeadLetterAsync();
            }
        }

        private async Task<TopicDescription> GetTopicAsync()
        {
            return await NamespaceManager.GetTopicAsync(TopicName);
        }
        private async Task<SubscriptionDescription> GetSubscriptionAsync(string subscriptionName)
        {
            return await NamespaceManager.GetSubscriptionAsync(TopicName, subscriptionName);
        }

        private async Task<IEnumerable<TopicDescription>> GetTopicsAsync()
        {
            return await NamespaceManager.GetTopicsAsync();
        }

        #endregion
    }
}
