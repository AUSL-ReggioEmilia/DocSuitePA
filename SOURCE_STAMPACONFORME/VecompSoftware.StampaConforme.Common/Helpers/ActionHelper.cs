using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.StampaConforme.Common.Helpers
{
    public static class ActionHelper
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        public static TResult TryCatchWithLogger<TResult>(Func<TResult> func, ILogger logger,
            IEnumerable<LogCategory> logCategories)
        {
            try
            {
                return func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {                    
                    logger.WriteError(ie, logCategories);
                    throw new Exception(ae.Message);
                }
                throw new Exception(ae.Message);
            }
            catch (Exception ex)
            {
                logger.WriteError(ex, logCategories);
                throw ex;
            }
        }
        #endregion
    }
}
