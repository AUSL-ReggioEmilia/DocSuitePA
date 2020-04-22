using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;

namespace VecompSoftware.ServiceBus.Module.UDS.Listener.Create
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Listener : ListenerMessageBase<ICommandCreateUDS>, IListenerMessageGeneric<ICommandCreateUDS>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
            : base(receiver, logger, new Execution(logger, webApiClient, biblosClient), "CommandCreateUDS")
        {

        }
    }
}
