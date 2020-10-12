using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Messages;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertMessage
{
    public class Listener : ListenerMessageBase<ICommandBuildMessage>, IListenerMessageGeneric<ICommandBuildMessage>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
            : base(receiver, logger, new Execution(logger, webApiClient, biblosClient), "CommandBuildMessage", webApiClient)
        {
        }
    }
}
