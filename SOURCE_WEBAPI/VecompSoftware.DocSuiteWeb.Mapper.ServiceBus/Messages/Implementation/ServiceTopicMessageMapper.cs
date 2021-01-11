using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages
{
    public class ServiceTopicMessageMapper : BaseServiceBusMapper<ServiceBusMessage, ServiceBusTopicMessage>, IServiceTopicMessageMapper
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

        public override ServiceBusTopicMessage Map(ServiceBusMessage serviceBusMessage, ServiceBusTopicMessage serviceBusTopicMessage)
        {
            #region [ Base ]

            serviceBusTopicMessage = new ServiceBusTopicMessage
            {
                Content = JsonConvert.SerializeObject(serviceBusMessage.Content, _serializerSettings),
                ContentType = ServiceBusMessageType.Event,


                MessageId = serviceBusMessage.MessageId,
                SessionId = serviceBusMessage.SessionId,
                CorrelationId = serviceBusMessage.CorrelationId,
                ExpiresAtUtc = serviceBusMessage.ExpiresAtUtc,
                Label = serviceBusMessage.Label,
                ReplyTo = serviceBusMessage.ReplyTo,
                ReplyToSessionId = serviceBusMessage.ReplyToSessionId,
                SequenceNumber = serviceBusMessage.SequenceNumber,
                Size = serviceBusMessage.Size,
                State = serviceBusMessage.State,
                TimeToLive = serviceBusMessage.TimeToLive,
                ScheduledEnqueueTimeUtc = serviceBusMessage.ScheduledEnqueueTimeUtc
            };

            #endregion

            #region [ Navigation Properties ]

            foreach (KeyValuePair<string, object> property in serviceBusMessage.Properties.Where(f => f.Value != null))
            {
                serviceBusTopicMessage.Properties.Add(property.Key, property.Value.ToString());
            }

            #endregion

            return serviceBusTopicMessage;
        }

        #endregion
    }
}
