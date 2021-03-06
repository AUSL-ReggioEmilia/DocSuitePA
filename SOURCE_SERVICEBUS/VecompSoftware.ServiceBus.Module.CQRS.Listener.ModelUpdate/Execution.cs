﻿using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;


namespace VecompSoftware.ServiceBus.Module.CQRS.Listener.ModelUpdate
{
    public class Execution : CQRSBaseExecution<ICommandCQRSUpdate>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]


        #endregion

        #region [ Constructor ]
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
            : base(logger, webApiClient, biblosClient, serviceBusClient)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
