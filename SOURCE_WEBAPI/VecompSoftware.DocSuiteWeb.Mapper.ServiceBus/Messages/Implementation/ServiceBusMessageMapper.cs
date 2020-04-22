using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages
{
    public class ServiceBusMessageMapper : BaseServiceBusMapper<ServiceBusMessage, BrokeredMessage>, IServiceBusMessageMapper
    {
        #region [ Fields ]

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        #endregion

        #region [ Methods ]

        public override BrokeredMessage Map(ServiceBusMessage serviceBusMessage, BrokeredMessage brokeredMessage)
        {
            #region [ Base ]
            string jsonContent;
            switch (serviceBusMessage.ContentType)
            {
                case ServiceBusMessageType.Command:
                case ServiceBusMessageType.Event:
                    jsonContent = JsonConvert.SerializeObject(serviceBusMessage.Content, _serializerSettings);
                    brokeredMessage = new BrokeredMessage(jsonContent);
                    break;
                default:
                    brokeredMessage = new BrokeredMessage(serviceBusMessage.Content);
                    break;
            }

            if (serviceBusMessage.MessageId.HasValue)
            {
                brokeredMessage.MessageId = serviceBusMessage.MessageId.ToString();
            }

            brokeredMessage.ContentType = serviceBusMessage.ContentType.ToString();

            if (!string.IsNullOrEmpty(serviceBusMessage.SessionId))
            {
                brokeredMessage.SessionId = serviceBusMessage.SessionId;
            }

            if (!string.IsNullOrEmpty(serviceBusMessage.CorrelationId))
            {
                brokeredMessage.CorrelationId = serviceBusMessage.CorrelationId;
            }

            if (!string.IsNullOrEmpty(serviceBusMessage.Label))
            {
                brokeredMessage.Label = serviceBusMessage.Label;
            }

            if (!string.IsNullOrEmpty(serviceBusMessage.ReplyTo))
            {
                brokeredMessage.ReplyTo = serviceBusMessage.ReplyTo;
            }

            if (!string.IsNullOrEmpty(serviceBusMessage.ReplyToSessionId))
            {
                brokeredMessage.ReplyToSessionId = serviceBusMessage.ReplyToSessionId;
            }

            if (serviceBusMessage.TimeToLive.HasValue)
            {
                brokeredMessage.TimeToLive = serviceBusMessage.TimeToLive.Value;
            }

            if (serviceBusMessage.ScheduledEnqueueTimeUtc.HasValue)
            {
                brokeredMessage.ScheduledEnqueueTimeUtc = serviceBusMessage.ScheduledEnqueueTimeUtc.Value;
            }

            #endregion

            #region [ Navigation Properties ]

            foreach (KeyValuePair<string, object> property in serviceBusMessage.Properties)
            {
                brokeredMessage.Properties.Add(property.Key, property.Value);
            }

            #endregion

            return brokeredMessage;
        }

        #endregion
    }
}
