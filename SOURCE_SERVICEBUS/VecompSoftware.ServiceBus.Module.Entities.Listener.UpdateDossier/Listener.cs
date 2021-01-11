using VecompSoftware.ServiceBus.Receiver.Base;
using Microsoft.ServiceBus.Messaging;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.UpdateDossier
{
    public class Listener : ListenerMessageBase<ICommandUpdateDossierData>, IListenerMessageGeneric<ICommandUpdateDossierData>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient)
            : base(receiver, logger, new Execution(logger, webApiClient), "CommandUpdateDossierData", webApiClient)
        {
        }
    }
}
