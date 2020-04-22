using System;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public class ServiceBusConfiguration : IServiceBusConfiguration
    {
        #region [ Properties ]
        /// <summary>
        /// The Service Bus connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the message’s time to live value. This is the duration after
        /// which the message expires, starting from when the message is sent to the
        /// Service Bus. Messages older than their TimeToLive value will expire and no
        /// longer be retained in the message store. Subscribers will be unable to receive
        /// expired messages.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }

        /// <summary>
        /// The needed time for the session to automatically renew.
        /// </summary>
        public TimeSpan AutoRenewTimeout { get; set; }

        /// <summary>
        /// Gets or Sets the operation timeout for all Service Bus operations 
        /// </summary>
        public TimeSpan? OperationTimeout { get; set; }

        /// <summary>
        /// Gets or Sets the maximum message size (in bytes) that can be sent or received
        /// Default value is set to 256KB which is the maximum recommended size for Service Bus operations
        /// </summary>
        public int MaximumMessageSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum queue size (in MB)
        /// </summary>
        public int MaximumQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the server waits for processing messages before it times out
        /// </summary>
        public TimeSpan ServerWaitTime { get; set; }

        /// <summary>
        ///     Gets or sets the System.TimeSpan idle interval after which the subscription is
        ///     automatically deleted. The minimum duration is 5 minutes.
        ///
        /// Returns:
        ///     The auto delete on idle time span for the subscription.
        /// </summary>
        public TimeSpan AutoDeleteOnIdle { get; set; }

        /// <summary>
        ///     Gets or sets the default message time to live value. This is the duration after
        ///     which the message expires, starting from when the message is sent to the Service
        ///     Bus. This is the default value used when Microsoft.ServiceBus.Messaging.BrokeredMessage.TimeToLive
        ///     is not set on a message itself. Messages older than their TimeToLive value will
        ///     expire and no longer be retained in the message store. Subscribers will be unable
        ///     to receive expired messages.
        ///
        /// Returns:
        ///     The default message time to live for a subscription.
        /// </summary>
        public TimeSpan DefaultMessageTimeToLive { get; set; }

        /// <summary>
        ///     Gets or sets the lock duration time span for the subscription.
        ///
        /// Returns:
        ///     The lock duration time span for the subscription.
        /// </summary>
        public TimeSpan LockDuration { get; set; }

        /// <summary>
        /// Summary:
        ///     Gets or sets the number of maximum deliveries.
        ///
        /// Returns:
        ///     The number of maximum deliveries.
        /// </summary>
        public int MaxDeliveryCount { get; set; }
        #endregion

        #region [ Constructor ]
        public ServiceBusConfiguration()
        {
        }
        public ServiceBusConfiguration(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            ConnectionString = connectionString;
            TimeToLive = TimeSpan.FromDays(1);
            AutoRenewTimeout = TimeSpan.FromMinutes(1);
            MaximumMessageSize = 8192 * 1024;
            MaximumQueueSize = 1024;
            OperationTimeout = null;
            ServerWaitTime = TimeSpan.FromMinutes(5);
        }
        #endregion
    }
}
