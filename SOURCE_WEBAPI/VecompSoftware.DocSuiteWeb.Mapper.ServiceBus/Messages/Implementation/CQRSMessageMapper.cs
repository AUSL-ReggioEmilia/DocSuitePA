using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.Services.Command.CQRS;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages
{
    public class CQRSMessageMapper : BaseServiceBusMapper<IMessage, ServiceBusMessage>, ICQRSMessageMapper
    {
        #region [ Fields ]

        private readonly IDictionary<string, ServiceBusMessageConfiguration> _configurations;

        #endregion

        public CQRSMessageMapper(IMessageConfiguration messageConfiguration)
        {
            _configurations = messageConfiguration.GetConfigurations();
        }

        #region [ Methods ]

        public override ServiceBusMessage Map(IMessage message, ServiceBusMessage serviceBusMessage)
        {
            #region [ Base ]

            serviceBusMessage.ContentType = message is ICommand ? ServiceBusMessageType.Command : ServiceBusMessageType.Event;
            if (_configurations.ContainsKey(message.Name))
            {
                ServiceBusMessageConfiguration serviceBusMessageConfiguration = _configurations[message.Name];
                switch (serviceBusMessage.ContentType)
                {
                    case ServiceBusMessageType.Command:
                        {
                            serviceBusMessage.ChannelName = serviceBusMessageConfiguration.QueueName;
                        }
                        break;
                    case ServiceBusMessageType.Event:
                        {
                            serviceBusMessage.ChannelName = serviceBusMessageConfiguration.TopicName;
                        }
                        break;
                    case ServiceBusMessageType.Message:
                    default:
                        break;
                }
            }
            serviceBusMessage.Content = message;

            if (message.CorrelationId.HasValue)
            {
                serviceBusMessage.CorrelationId = message.CorrelationId.ToString();
            }

            if (message.ScheduledTime.HasValue)
            {
                serviceBusMessage.ScheduledEnqueueTimeUtc = message.ScheduledTime.Value.DateTime;
            }
            #endregion

            #region [ Navigation Properties ]

            serviceBusMessage.Properties = message.CustomProperties
                .Where(f => f.Value != null)
                .ToDictionary(f => f.Key, f => f.Value);

            #endregion

            return serviceBusMessage;
        }

        #endregion
    }
}
