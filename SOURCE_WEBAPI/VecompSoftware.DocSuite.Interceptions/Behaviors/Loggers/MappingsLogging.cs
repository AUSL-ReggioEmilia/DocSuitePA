using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuite.Interceptors.Behaviors.Loggers
{
    public class MappingsLogging : BaseLogging
    {

        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public MappingsLogging(ILogger logger)
            : base(logger, false)//false.TryParseDefault(ConfigurationManager.AppSettings["VecompSoftware.Logger.Mappings.InterceptorEnabled"]))
        {
        }
        #endregion



    }
}
