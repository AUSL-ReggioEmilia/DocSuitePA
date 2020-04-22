using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.ServiceBus.Module.CQRS.Listener.ModelUpdate
{
    public class Listener : ListenerMessageBase<ICommandCQRSUpdate>, IListenerMessageGeneric<ICommandCQRSUpdate>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
            : base(receiver, logger, new Execution(logger, webApiClient, biblosClient),
                  string.Empty, commandNameFilterEnabled: false)
        {

        }
    }
}
