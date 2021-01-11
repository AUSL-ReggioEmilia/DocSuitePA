using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Core.Command.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuite.Private.WebAPI.Hubs.UDS
{
    public class UDSEventHub : BaseAuthenticateHub
    {
        #region [ Fields ]

        private const string UDS_DATAERROR_CONFIGURATION = "EventError";
        private const string UDS_DATA_CONFIGURATION = "UDSDataEvent";
        private const string UDS_SCHEMA_CONFIGURATION = "UDSSchemaEvent";
        private static readonly IList<string> _subscriptionNames = new List<string> { UDS_DATAERROR_CONFIGURATION, UDS_DATA_CONFIGURATION };

        private static readonly string TYPE_UDS_COMMAND_INSERTDATA = typeof(CommandInsertUDSData).Name;
        private static readonly string TYPE_UDS_COMMAND_UPDATEDATA = typeof(CommandUpdateUDSData).Name;
        private static readonly string TYPE_UDS_COMMAND_DELETEDATA = typeof(CommandDeleteUDSData).Name;
        private static readonly string TYPE_UDS_COMMAND_CREATE = typeof(CommandCreateUDS).Name;
        private static readonly string TYPE_UDS_COMMAND_UPDATE = typeof(CommandUpdateUDS).Name;
        private static readonly string TYPE_EVENT_ERROR = typeof(EventError).Name;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public UDSEventHub()
            : base()
        {
        }
        #endregion

        #region [ Methods ]
        protected override IList<string> GetSubscriptionNames()
        {
            return _subscriptionNames;
        }

        public async Task SubscribeUDSInsertEvents(string correlationId)
        {
            await SubscribeTopicAsync(UDS_DATA_CONFIGURATION, correlationId, TYPE_UDS_COMMAND_INSERTDATA, SendResponseDataEvent);
            await SubscribeTopicAsync(UDS_DATAERROR_CONFIGURATION, correlationId, TYPE_EVENT_ERROR, SendResponseDataErrorEvent);
        }

        public async Task SubscribeUDSUpdateEvents(string correlationId)
        {
            await SubscribeTopicAsync(UDS_DATA_CONFIGURATION, correlationId, TYPE_UDS_COMMAND_UPDATEDATA, SendResponseDataEvent);
            await SubscribeTopicAsync(UDS_DATAERROR_CONFIGURATION, correlationId, TYPE_EVENT_ERROR, SendResponseDataErrorEvent);
        }

        public async Task SubscribeUDSDeleteEvents(string correlationId)
        {
            await SubscribeTopicAsync(UDS_DATA_CONFIGURATION, correlationId, TYPE_UDS_COMMAND_DELETEDATA, SendResponseDataEvent);
            await SubscribeTopicAsync(UDS_DATAERROR_CONFIGURATION, correlationId, TYPE_EVENT_ERROR, SendResponseDataErrorEvent);
        }

        public async Task SubscribeUDSSchemaCreateEvents(string colleratedId)
        {
            await SubscribeTopicAsync(UDS_SCHEMA_CONFIGURATION, colleratedId, TYPE_UDS_COMMAND_CREATE, SendResponseSchemaEvent);
        }

        private void SendResponseDataErrorEvent(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.udsDataError(result.Content));
        }

        private void SendResponseDataEvent(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.udsDataEvent(result.Content));
        }

        private void SendResponseSchemaEvent(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.udsSchemaEvent(result.Content));
        }

        #endregion
    }
}