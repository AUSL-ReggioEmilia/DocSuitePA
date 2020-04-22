using VecompSoftware.DocSuiteWeb.Common.Loggers;
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
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
            : base(logger, webApiClient, biblosClient)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
