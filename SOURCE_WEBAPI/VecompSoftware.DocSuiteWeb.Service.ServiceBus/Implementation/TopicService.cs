using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Helpers;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class TopicService : ITopicService, IDisposable
    {
        #region [ Fields ]
        private const string MESSAGE_IS_UNDEFINED = "Message has not been defined";
        private const string TOPIC_SUBSCRIPTION_IS_EMPTY = "TopicName has not been defined";
        private const string SUBSCRIPTION_IS_EMPTY = "SubscriptionName has not been defined";
        private const string COMMANDNAME_IS_EMPTY = "CommandName has not been defined";
        private const string COMMANDNAME_IS_NOT_MAPPED = "CommandName has not been mapped";
        private const string DEFAULTFILTER_IS_EMPTY = "Default Filter has not been defined";
        private const string SERVICE_BASE_CONNECTIONSTRING_IS_EMPTY = "Service Bus ConnectionString is empty";

        private readonly Guid _instanceId;
        private readonly IServiceBusConfiguration _configuration;
        private readonly IDictionary<string, ServiceBusMessageConfiguration> _messageConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusMessageMapper _mapper_to_brokered;
        private readonly IBrokeredMessageMapper _mapper_to_message;
        private readonly IServiceBusTopicContext _topicContext;
        private static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(TopicService));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public TopicService(IServiceBusConfiguration configuration, ILogger logger,
            IServiceBusMessageMapper mapper_to_brokered, IBrokeredMessageMapper mapper_to_message,
            IServiceBusTopicContext topicContext, IMessageConfiguration messageConfiguration)
        {
            if (configuration == null || string.IsNullOrEmpty(configuration.ConnectionString))
            {
                throw new DSWException(SERVICE_BASE_CONNECTIONSTRING_IS_EMPTY, null, DSWExceptionCode.SS_Mapper);
            }

            _instanceId = Guid.NewGuid();
            _configuration = configuration;
            _messageConfiguration = messageConfiguration.GetConfigurations();
            _logger = logger;
            _mapper_to_brokered = mapper_to_brokered;
            _mapper_to_message = mapper_to_message;
            _topicContext = topicContext;
        }
        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region [ Methods ]
        public async Task InitializeAsync(string topicName, string subscriptionName, string filter = "")
        {
            await _topicContext.BeginContextAsync(topicName, subscriptionName, filter);
        }

        /// <summary>
        /// Send a ServiceBusMessage to a queue
        /// </summary>
        public async Task<ServiceBusMessage> SendToTopicAsync(ServiceBusMessage message)
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                //message.ChannelName
                if (message == null)
                {
                    throw new DSWException(string.Concat("SendToTopicAsync: ", MESSAGE_IS_UNDEFINED), null, DSWExceptionCode.SS_Mapper);
                }

                if (string.IsNullOrEmpty(message.ChannelName))
                {
                    throw new DSWException(string.Concat("SendToTopicAsync: ", TOPIC_SUBSCRIPTION_IS_EMPTY), null, DSWExceptionCode.SS_Mapper);
                }
                await _topicContext.BeginContextAsync(message.ChannelName);
                BrokeredMessage requestMessage = _mapper_to_brokered.Map(message, new BrokeredMessage());
                ServiceBusMessage serviceBusMessage = new ServiceBusMessage();
                BrokeredMessage responseMessage = await _topicContext.PublishAsync(requestMessage);
                serviceBusMessage = _mapper_to_message.Map(responseMessage, serviceBusMessage);
                serviceBusMessage.ChannelName = message.ChannelName;
                serviceBusMessage.SubscriptionName = string.Empty;
                return serviceBusMessage;
            }, _logger, LogCategories);
        }

        /// <summary>
        /// Get ServiceBusMessages from topic
        /// </summary>
        public async Task<ICollection<ServiceBusMessage>> GetMessagesFromTopicAsync()
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                ICollection<ServiceBusMessage> serviceBusMessages = new List<ServiceBusMessage>();
                ICollection<BrokeredMessage> messages = await _topicContext.GetMessagesAsync();
                foreach (BrokeredMessage message in messages)
                {
                    ServiceBusMessage serviceBusMessage = new ServiceBusMessage();
                    serviceBusMessage = _mapper_to_message.Map(message, serviceBusMessage);
                    serviceBusMessage.ChannelName = _topicContext.TopicName;
                    serviceBusMessage.SubscriptionName = _topicContext.SubscriptionName;
                    serviceBusMessages.Add(serviceBusMessage);
                }
                return serviceBusMessages;
            }, _logger, LogCategories);
        }

        /// <summary>
        /// Get ServiceBusMessages from queue by id. Returns null if doesn't find it
        /// </summary>
        /// <param name="messageId"></param>
        public async Task<ServiceBusMessage> GetMessageByIdFromTopicAsync(Guid messageId)
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                BrokeredMessage message = (await _topicContext.GetMessagesAsync()).FirstOrDefault(x => x.MessageId == messageId.ToString());
                if (message == null)
                {
                    return null;
                }

                ServiceBusMessage serviceBusMessage = new ServiceBusMessage();
                serviceBusMessage = _mapper_to_message.Map(message, serviceBusMessage);
                serviceBusMessage.ChannelName = _topicContext.TopicName;
                serviceBusMessage.SubscriptionName = _topicContext.SubscriptionName;
                return serviceBusMessage;
            }, _logger, LogCategories);
        }

        public async Task<ICollection<ServiceBusMessage>> GetMessagesAsync(string topicName, string subscriptionName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new DSWException($"SubscribeTopicAsync: {TOPIC_SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(subscriptionName))
            {
                throw new DSWException($"SubscribeTopicAsync: {SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            await _topicContext.BeginContextAsync(topicName);
            return _mapper_to_message.MapCollection(await _topicContext.GetMessagesAsync(subscriptionName));
        }

        /// <summary>
        /// Get queue status
        /// </summary>
        public async Task<ServiceBusMessageState> GetTopicStatusAsync()
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                return ServiceBusMessageStateHelper.ConvertState(await _topicContext.GetStatusAsync());
            }, _logger, LogCategories);
        }
        public async Task<bool> SubscriptionExists(string topicName, string subscriptionName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new DSWException($"SubscribeTopicAsync: {TOPIC_SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(subscriptionName))
            {
                throw new DSWException($"SubscribeTopicAsync: {SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            await _topicContext.BeginContextAsync(topicName);
            return _topicContext.SubscriptionExists(subscriptionName);
        }

        public async Task<ITopicService> CreateSubscriptionAsync(string topicName, string subscriptionName, string correlationId, string commandName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new DSWException($"SubscribeTopicAsync: {TOPIC_SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(subscriptionName))
            {
                throw new DSWException($"SubscribeTopicAsync: {SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(commandName))
            {
                throw new DSWException(COMMANDNAME_IS_EMPTY, null, DSWExceptionCode.SS_Mapper);
            }
            if (!_messageConfiguration.ContainsKey(commandName))
            {
                throw new DSWException(COMMANDNAME_IS_NOT_MAPPED, null, DSWExceptionCode.SS_Mapper);
            }

            await _topicContext.BeginContextAsync(topicName);
            await _topicContext.CreateSubscriptionAsync(subscriptionName, correlationId, _messageConfiguration[commandName].DefaultFilterEvent, true);
            return this;
        }

        public async Task<ITopicService> SubscribeTopicAsync(string topicName, string subscriptionName, string commandName, string correlationId,
            Action<ServiceBusMessage> callback)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new DSWException($"SubscribeTopicAsync: {TOPIC_SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(subscriptionName))
            {
                throw new DSWException($"SubscribeTopicAsync: {SUBSCRIPTION_IS_EMPTY}", null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(commandName))
            {
                throw new DSWException(COMMANDNAME_IS_EMPTY, null, DSWExceptionCode.SS_Mapper);
            }
            if (!_messageConfiguration.ContainsKey(commandName))
            {
                throw new DSWException(COMMANDNAME_IS_NOT_MAPPED, null, DSWExceptionCode.SS_Mapper);
            }

            await _topicContext.BeginContextAsync(topicName);
            await _topicContext.CreateSubscriptionAsync(subscriptionName, correlationId, _messageConfiguration[commandName].DefaultFilterEvent, (message) => OnMessage(callback, message));
            return this;
        }

        public async Task<ITopicService> UnsubscribeTopicAsync(string topicName, string subscriptionName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new DSWException(string.Concat("UnsubscribeTopicAsync: ", TOPIC_SUBSCRIPTION_IS_EMPTY), null, DSWExceptionCode.SS_Mapper);
            }
            if (string.IsNullOrEmpty(subscriptionName))
            {
                throw new DSWException(string.Concat("UnsubscribeTopicAsync: ", SUBSCRIPTION_IS_EMPTY), null, DSWExceptionCode.SS_Mapper);
            }

            await _topicContext.BeginContextAsync(topicName);
            await _topicContext.RemoveSubscriptionAsync(subscriptionName);
            return this;
        }

        private void OnMessage(Action<ServiceBusMessage> callback, BrokeredMessage message)
        {
            if (callback == null || message == null)
            {
                _logger.WriteWarning(new LogMessage(string.Concat("TopicService : OnMessage receive empty BrokeredMessage (", message == null, ") or callback not initalized (", callback == null, ")")), LogCategories);
                return;
            }
            callback(_mapper_to_message.Map(message, new ServiceBusMessage()));
        }

        private async Task OnReceived(BrokeredMessage message)
        {
            await message.CompleteAsync();
        }

        private async Task OnError(BrokeredMessage message, IDictionary<string, object> propertyToModified)
        {
            await message.AbandonAsync(propertyToModified);
        }

        public async Task<ServiceBusMessage> CreateAsync(ServiceBusMessage content)
        {
            return await SendToTopicAsync(content);
        }

        public ServiceBusMessage Create(ServiceBusMessage content)
        {
            return CreateAsync(content).Result;
        }

        #region [ NotImplemented ]


        public Task<ServiceBusMessage> UpdateAsync(ServiceBusMessage content)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ServiceBusMessage content)
        {
            throw new NotImplementedException();
        }

        public ServiceBusMessage Update(ServiceBusMessage content)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ServiceBusMessage content)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ServiceBusMessage> Queryable(bool optimization = false)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
