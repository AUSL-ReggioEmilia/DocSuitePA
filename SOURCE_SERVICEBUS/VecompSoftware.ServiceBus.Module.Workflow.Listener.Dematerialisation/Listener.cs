using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Integrations.GenericProcesses;

namespace VecompSoftware.ServiceBus.Module.Workflow.Listener.Dematerialisation
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Listener : ListenerMessageBase<ICommandDematerialisationRequest>, IListenerMessageGeneric<ICommandDematerialisationRequest>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
            : base(receiver, logger, new Execution(logger, webApiClient, biblosClient), "CommandDematerialisationRequest")
        {

        }
    }
}
