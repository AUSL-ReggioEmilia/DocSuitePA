using VecompSoftware.DocSuiteWeb.Common.Loggers;


namespace VecompSoftware.DocSuite.Interceptors.Behaviors.Loggers
{
    public class ServicesLogging : BaseLogging
    {

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ServicesLogging(ILogger logger)
            : base(logger, false)//false.TryParseDefault(ConfigurationManager.AppSettings["VecompSoftware.Logger.Services.InterceptorEnabled"]))
        {
        }
        #endregion



    }
}
