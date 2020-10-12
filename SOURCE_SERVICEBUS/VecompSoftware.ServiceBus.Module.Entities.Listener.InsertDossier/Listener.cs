using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Dossiers;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertDossier
{
    public class Listener : ListenerMessageBase<ICommandBuildDossier>, IListenerMessageGeneric<ICommandBuildDossier>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient)
          : base(receiver, logger, new Execution(logger, webApiClient), "CommandBuildDossier", webApiClient)
        {
        }
    }
}
