using Microsoft.Extensions.Logging;
using System;

namespace VecompSoftware.DocSuite.SPID.Common.Helpers.Actions
{
    public class ActionHelper
    {
        #region [ Methods ]
        public static TResult TryCatchWithLoggerGeneric<TResult>(Func<TResult> func, ILogger logger)
        {
            try
            {
                return func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    logger.LogError(ie, ie.Message);
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}
