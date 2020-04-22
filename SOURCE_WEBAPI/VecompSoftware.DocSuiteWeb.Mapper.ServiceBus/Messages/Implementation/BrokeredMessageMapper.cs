using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.ExtensionMethods;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages
{
    public class BrokeredMessageMapper : BaseServiceBusMapper<BrokeredMessage, ServiceBusMessage>, IBrokeredMessageMapper
    {
        #region [ Fields ]

        private const string COMMAND_TYPE_NAME = "Command";
        private const string EVENT_TYPE_NAME = "Event";

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        #endregion

        #region [ Methods ]

        public override ServiceBusMessage Map(BrokeredMessage entity, ServiceBusMessage entityTransformed)
        {
            #region [ Base ]
            string jsonContent;
            switch (entity.ContentType)
            {
                case COMMAND_TYPE_NAME:
                    entityTransformed = new ServiceBusMessage();
                    jsonContent = entity.GetBody<string>();
                    entityTransformed.Content = JsonConvert.DeserializeObject<ICommand>(jsonContent, _serializerSettings);
                    entityTransformed.ContentType = ServiceBusMessageType.Command;
                    break;
                case EVENT_TYPE_NAME:
                    entityTransformed = new ServiceBusMessage();
                    jsonContent = entity.GetBody<string>();
                    entityTransformed.Content = JsonConvert.DeserializeObject<IEvent>(jsonContent, _serializerSettings);
                    entityTransformed.ContentType = ServiceBusMessageType.Event;
                    break;
                default:
                    entityTransformed = new ServiceBusMessage
                    {
                        Content = entity.GetBody<string>(),
                        ContentType = ServiceBusMessageType.Message
                    };
                    break;
            }

            if (entity.MessageId != null)
            {
                Guid messageId;
                if (Guid.TryParse(entity.MessageId, out messageId))
                {
                    entityTransformed.MessageId = messageId;
                }
            }

            if (!string.IsNullOrEmpty(entity.SessionId))
            {
                entityTransformed.SessionId = entity.SessionId;
            }

            if (!string.IsNullOrEmpty(entity.CorrelationId))
            {
                entityTransformed.CorrelationId = entity.CorrelationId;
            }

            DateTime? expiresAtUtc = entity.GetPropertyValueOrDefault<DateTime?>(() => entity.ExpiresAtUtc, null);
            if (expiresAtUtc.HasValue)
            {
                entityTransformed.ExpiresAtUtc = expiresAtUtc.Value;
            }

            string label = entity.GetPropertyValueOrDefault(() => entity.Label, string.Empty);
            if (!string.IsNullOrEmpty(label))
            {
                entityTransformed.Label = label;
            }

            string replayTo = entity.GetPropertyValueOrDefault(() => entity.ReplyTo, string.Empty);
            if (!string.IsNullOrEmpty(replayTo))
            {
                entityTransformed.ReplyTo = replayTo;
            }

            string replayToSessionId = entity.GetPropertyValueOrDefault(() => entity.ReplyToSessionId, string.Empty);
            if (!string.IsNullOrEmpty(replayToSessionId))
            {
                entityTransformed.ReplyToSessionId = replayToSessionId;
            }

            long? sequenceNumber = entity.GetPropertyValueOrDefault<long?>(() => entity.SequenceNumber, null);
            if (sequenceNumber.HasValue)
            {
                entityTransformed.SequenceNumber = sequenceNumber.Value;
            }

            long? size = entity.GetPropertyValueOrDefault<long?>(() => entity.Size, null);
            if (size.HasValue)
            {
                entityTransformed.Size = size.Value;
            }

            entityTransformed.State = (ServiceBusResponseState)entity.State;

            TimeSpan? timeToLive = entity.GetPropertyValueOrDefault<TimeSpan?>(() => entity.TimeToLive, null);
            if (timeToLive.HasValue)
            {
                entityTransformed.TimeToLive = timeToLive.Value;
            }

            DateTime? scheduledEnqueueTimeUtc = entity.GetPropertyValueOrDefault<DateTime?>(() => entity.ScheduledEnqueueTimeUtc, null);
            if (scheduledEnqueueTimeUtc.HasValue)
            {
                entityTransformed.ScheduledEnqueueTimeUtc = scheduledEnqueueTimeUtc.Value;
            }

            #endregion

            #region [ Navigation Properties ]

            foreach (KeyValuePair<string, object> property in entity.Properties)
            {
                entityTransformed.Properties.Add(property.Key, property.Value);
            }

            #endregion

            return entityTransformed;
        }

        #endregion
    }
}
