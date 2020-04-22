using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public abstract class ServiceBusContext : IServiceBusContext
    {
        #region [ Fields ]        
        private const string MESSAGE_SIZE_EXCEEDS = "Message size {0}KB exceeds the maximum size limit of {1}KB : {2}";
        private readonly ILogger _logger;
        private readonly IServiceBusConfiguration _configuration;
        private readonly NamespaceManager _namespaceManager;
        private bool _disposed;
        private static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ServiceBusContext));
                }
                return _logCategories;
            }
        }

        protected NamespaceManager NamespaceManager => _namespaceManager;

        protected IServiceBusConfiguration ServiceBusConfiguration => _configuration;

        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }
        #endregion

        #region [ Constructor ]
        public ServiceBusContext(IServiceBusConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            //Initialize the queue with custom settings
            _namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// Send a message to a queue/topic
        /// </summary>
        public abstract Task<BrokeredMessage> PublishAsync(BrokeredMessage message);

        /// <summary>
        /// Get messages from queue/topic
        /// </summary>
        public abstract Task<ICollection<BrokeredMessage>> GetMessagesAsync();

        /// <summary>
        /// Get queue/topic status
        /// </summary>
        /// <returns></returns>
        public abstract Task<EntityStatus> GetStatusAsync();

        protected void InitializeMessage(BrokeredMessage message)
        {
            if (message.Size > _configuration.MaximumMessageSize)
            {
                LogMessage warningMessage = new LogMessage(string.Format(MESSAGE_SIZE_EXCEEDS, message.Size / 1024, _configuration.MaximumMessageSize / 1024, message));
                _logger.WriteWarning(warningMessage, LogCategories);
            }
        }
        #endregion
    }
}
