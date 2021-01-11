using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.ServiceBus
{
    public class ServiceBusTopicMessage
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the identifier of the message.
        /// </summary>
        public Guid? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the session.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the correlation.
        /// </summary>
        public string CorrelationId { get; set; }

        public DateTime? ExpiresAtUtc { get; set; }

        public string Label { get; set; }

        public string ReplyTo { get; set; }

        public string ReplyToSessionId { get; set; }

        /// <summary>
        /// Gets the unique number assigned to a message by the Service Bus.
        /// </summary>
        public long? SequenceNumber { get; set; }

        public ServiceBusMessageType ContentType { get; set; }

        /// <summary>
        /// Gets the size of the message in bytes.
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Gets or sets the message of the state.
        /// </summary>
        public ServiceBusResponseState State { get; set; }

        public string ChannelName { get; set; }

        public string SubscriptionName { get; set; }

        /// <summary>
        /// Gets or sets the message’s time to live value
        /// </summary>
        public TimeSpan? TimeToLive { get; set; }

        public DateTime? ScheduledEnqueueTimeUtc { get; set; }

        public string Content { get; set; }

        public IDictionary<string, string> Properties { get; set; }
        #endregion

        #region [ Constructor ]
        public ServiceBusTopicMessage()
        {
            MessageId = Guid.NewGuid();
            SessionId = "0";
            ContentType = ServiceBusMessageType.Message;
            Properties = new Dictionary<string, string>();
        }
        #endregion
    }

}
