using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Validation;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public class ServiceBusQueueContext : ServiceBusContext, IServiceBusQueueContext, IDisposable
    {
        #region [ Fields ]
        private const string QUEUE_IS_EMPTY = "Queue has not been defined";
        private const string QUEUE_NOT_EXIST = "Nessuna coda configurata per il nome {0}";
        private const string CONTEXT_NOT_INITIALIZED = "Nessun context inizializzato per la richiesta. Effettuare un BeginContextAsync prima di un PublishAsync.";

        private readonly ILogger _logger;
        private readonly IServiceBusConfiguration _configuration;
        private QueueClient _queueClient;
        private bool _disposed;
        #endregion

        #region [ Properties ]

        private bool IsContextInitialized => _queueClient == null ? false : true;

        public Action<BrokeredMessage> Handler { get; set; }
        public string QueueName { get; set; }
        #endregion

        #region [ Dispose ]

        protected new virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _queueClient.CloseAsync();
                _disposed = true;
            }
        }
        #endregion

        #region [ Constructor ]

        public ServiceBusQueueContext(IServiceBusConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region [ Methods ]

        public IServiceBusContext BeginContext(string queueName, Action<BrokeredMessage> handler)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new DSWException(QUEUE_IS_EMPTY, null, DSWExceptionCode.SS_Mapper);
            }

            QueueName = queueName;
            Handler = handler;

            InitializeQueue();
            _queueClient = QueueClient.CreateFromConnectionString(_configuration.ConnectionString, queueName, ReceiveMode.PeekLock);
            return this;
        }

        /// <summary>
        /// Send a message to a queue
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
            await _queueClient.SendAsync(message);
            return message;
        }

        /// <summary>
        /// Get messages from queue
        /// </summary>
        public override async Task<ICollection<BrokeredMessage>> GetMessagesAsync()
        {
            if (!IsContextInitialized)
            {
                _logger.WriteError(new LogMessage(CONTEXT_NOT_INITIALIZED), LogCategories);
                throw new DSWException(CONTEXT_NOT_INITIALIZED, null, DSWExceptionCode.SS_Anomaly);
            }

            QueueDescription queueDescription = await GetQueueAsync();
            int activeMessageCount = (int)queueDescription.MessageCountDetails.ActiveMessageCount;
            IEnumerable<BrokeredMessage> messages = await _queueClient.PeekBatchAsync(activeMessageCount);
            if (messages == null)
            {
                return new List<BrokeredMessage>();
            }

            return messages.ToList();
        }

        /// <summary>
        /// Get queue/topic status
        /// </summary>
        /// <returns></returns>
        public override async Task<EntityStatus> GetStatusAsync()
        {
            QueueDescription queue = await GetQueueAsync();
            return queue.Status;
        }

        private void InitializeQueue()
        {
            QueueDescription options = new QueueDescription(QueueName)
            {
                MaxSizeInMegabytes = _configuration.MaximumQueueSize,
                DefaultMessageTimeToLive = _configuration.TimeToLive
            };

            if (!NamespaceManager.QueueExistsAsync(QueueName).Result)
            {
                string message = string.Format(QUEUE_NOT_EXIST, QueueName);
                _logger.WriteError(new LogMessage(message), LogCategories);
                throw new DSWValidationException(message,
                    new ReadOnlyCollection<ValidationMessageModel>(new Collection<ValidationMessageModel>() { new ValidationMessageModel() { Message = message, Key = "QUEUE_NOT_EXIST" } }), null, DSWExceptionCode.SS_Mapper);
            }
        }

        private void SetResponseEvent()
        {
            OnMessageOptions options = new OnMessageOptions
            {
                AutoComplete = false
            };

            _queueClient.OnMessage(Handler, options);
        }

        private async Task<QueueDescription> GetQueueAsync()
        {
            return await NamespaceManager.GetQueueAsync(QueueName);
        }

        private async Task<IEnumerable<QueueDescription>> GetQueuesAsync()
        {
            return await NamespaceManager.GetQueuesAsync();
        }
        #endregion
    }
}
