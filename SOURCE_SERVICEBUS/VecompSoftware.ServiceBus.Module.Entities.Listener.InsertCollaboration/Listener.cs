using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Collaborations;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertCollaboration
{
    public class Listener : ListenerMessageBase<ICommandBuildCollaboration>, IListenerMessageGeneric<ICommandBuildCollaboration>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
            : base(receiver, logger, new Execution(logger, webApiClient, biblosClient), "CommandBuildCollaboration")
        {

        }
    }
}
