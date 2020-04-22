using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Integrations.GenericProcesses;

namespace VecompSoftware.ServiceBus.Module.Workflow.Listener.SecureDocument
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Listener : ListenerMessageBase<ICommandSecureDocumentRequest>, IListenerMessageGeneric<ICommandSecureDocumentRequest>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient, StampaConforme.StampaConformeClient stampaConformeClient)
            : base(receiver, logger, new Execution(logger, webApiClient, biblosClient, stampaConformeClient), "CommandSecureDocumentRequest")
        {

        }
    }
}
