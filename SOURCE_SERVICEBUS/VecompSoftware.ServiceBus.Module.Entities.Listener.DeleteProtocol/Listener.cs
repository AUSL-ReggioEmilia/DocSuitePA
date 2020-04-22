using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Protocols;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.DeleteProtocol
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Listener : ListenerMessageBase<ICommandDeleteProtocol>, IListenerMessageGeneric<ICommandDeleteProtocol>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient)
            : base(receiver, logger, new Execution(logger, webApiClient, biblosClient), "CommandDeleteProtocol")
        {

        }
    }
}
