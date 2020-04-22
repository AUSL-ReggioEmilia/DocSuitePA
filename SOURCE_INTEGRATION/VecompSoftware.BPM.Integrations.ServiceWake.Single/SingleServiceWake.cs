using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.ServiceWake.Single
{

    public class SingleServiceWake : ServiceWakeBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public SingleServiceWake(ILogger logger) : base(logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public override void OnAddModule()
        {
            _logger.WriteInfo(new LogMessage($"SingleServiceWake.OnAddModule() -> module '{CurrentModule.Name}' loaded."), LogCategories);
        }

        public override void OnStart(Action executeAction, Action closeAction)
        {
            _logger.WriteInfo(new LogMessage("SingleServiceWake.OnStart() -> start timer"), LogCategories);
            executeAction();
        }

        public override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("SingleServiceWake.OnStop() -> stop timer"), LogCategories);
        }

        #endregion
    }
}
