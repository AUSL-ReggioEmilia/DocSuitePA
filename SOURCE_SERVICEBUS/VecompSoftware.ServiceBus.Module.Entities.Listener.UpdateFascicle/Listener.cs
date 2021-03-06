﻿using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Fascicles;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.UpdateFascicle
{
    public class Listener : ListenerMessageBase<ICommandUpdateFascicleData>, IListenerMessageGeneric<ICommandUpdateFascicleData>
    {
        public Listener(MessageReceiver receiver, ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
          : base(receiver, logger, new Execution(logger, webApiClient, biblosClient), "CommandUpdateFascicleData", webApiClient)
        {
        }
    }
}
