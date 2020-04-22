using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuite.Interceptors.Behaviors.Loggers
{
    public class ValidationsLogging : BaseLogging
    {

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ValidationsLogging(ILogger logger)
            : base(logger, false)//false.TryParseDefault(ConfigurationManager.AppSettings["VecompSoftware.Logger.Validations.InterceptorEnabled"]))
        {
        }
        #endregion

    }
}
