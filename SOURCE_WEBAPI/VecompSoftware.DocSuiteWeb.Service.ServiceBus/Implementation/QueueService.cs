using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class QueueService : IQueueService, IDisposable
    {
        #region [ Fields ]
        private const string MESSAGE_IS_UNDEFINED = "Message has not been defined";
        private const string QUEUE_SUBSCRIPTION_IS_EMPTY = "Queue has not been defined";
        private const string SERVICE_BASE_CONNECTIONSTRING_IS_EMPTY = "Service Bus ConnectionString is empty";

        private readonly Guid _instanceId;
        private readonly IServiceBusConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IServiceBusMessageMapper _mapper_to_brokered;
        private readonly IBrokeredMessageMapper _mapper_to_message;
        private readonly IServiceBusQueueContext _queueContext;
        private static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(QueueService));
                }
                return _logCategories;
            }
        }

        public Action<ServiceBusMessage> Handler { get; set; }
        #endregion

        #region [ Constructor ]
        public QueueService(IServiceBusConfiguration configuration, ILogger logger,
            IServiceBusMessageMapper mapper_to_brokered, IBrokeredMessageMapper mapper_to_message,
            IServiceBusQueueContext queueContext)
        {
            if (configuration == null || string.IsNullOrEmpty(configuration.ConnectionString))
            {
                throw new DSWException(SERVICE_BASE_CONNECTIONSTRING_IS_EMPTY, null, DSWExceptionCode.SS_Mapper);
            }

            _instanceId = Guid.NewGuid();
            _configuration = configuration;
            _logger = logger;
            _mapper_to_brokered = mapper_to_brokered;
            _mapper_to_message = mapper_to_message;
            _queueContext = queueContext;
        }
        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Send a ServiceBusMessage to a queue with custom callback
        /// </summary>
        public async Task<ServiceBusMessage> SendToQueueAsync(ServiceBusMessage message)
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                if (message == null)
                {
                    throw new DSWException(MESSAGE_IS_UNDEFINED, null, DSWExceptionCode.SS_Mapper);
                }

                BrokeredMessage requestMessage = _mapper_to_brokered.Map(message, new BrokeredMessage());
                ServiceBusMessage serviceBusMessage = new ServiceBusMessage();
                BrokeredMessage responseMessage = await _queueContext.PublishAsync(requestMessage);
                serviceBusMessage = _mapper_to_message.Map(responseMessage, serviceBusMessage);
                serviceBusMessage.ChannelName = _queueContext.QueueName;
                return serviceBusMessage;
            }, _logger, LogCategories);
        }

        /// <summary>
        /// Get ServiceBusMessages from queue
        /// </summary>
        public async Task<ICollection<ServiceBusMessage>> GetMessagesFromQueueAsync()
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                ICollection<ServiceBusMessage> serviceBusMessages = new List<ServiceBusMessage>();
                ICollection<BrokeredMessage> messages = await _queueContext.GetMessagesAsync();
                foreach (BrokeredMessage message in messages)
                {
                    ServiceBusMessage serviceBusMessage = new ServiceBusMessage();
                    serviceBusMessage = _mapper_to_message.Map(message, serviceBusMessage);
                    serviceBusMessage.ChannelName = _queueContext.QueueName;
                    serviceBusMessages.Add(serviceBusMessage);
                }
                return serviceBusMessages;
            }, _logger, LogCategories);
        }

        /// <summary>
        /// Get queue status
        /// </summary>
        public async Task<ServiceBusMessageState> GetQueueStatusAsync()
        {
            return await ServiceHelper.TryCatchWithLogger(async () =>
            {
                return ServiceBusMessageStateHelper.ConvertState(await _queueContext.GetStatusAsync());
            }, _logger, LogCategories);
        }

        public IQueueService SubscribeQueue(string queueName)
        {
            return SubscribeQueue(queueName, null);
        }

        public IQueueService SubscribeQueue(string queueName, Action<ServiceBusMessage> Callback)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new DSWException(QUEUE_SUBSCRIPTION_IS_EMPTY, null, DSWExceptionCode.SS_Mapper);
            }

            Handler = Callback;
            _queueContext.BeginContext(queueName, OnMessage);
            return this;
        }

        private void OnMessage(BrokeredMessage message)
        {
            if (Handler == null)
            {
                return;
            }

            Handler(_mapper_to_message.Map(message, new ServiceBusMessage()));
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
            return await SendToQueueAsync(content);
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
