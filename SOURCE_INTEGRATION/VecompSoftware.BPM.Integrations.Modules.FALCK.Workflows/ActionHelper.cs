using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows
{
    public class ActionHelper
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        #endregion

        #region [ Methods ]
        public static async Task TryCatchWithLoggerAsync(Func<Task> func, ILogger logger, IEnumerable<LogCategory> logCategories)
        {
            try
            {
                await func();
            }
            catch (OperationCanceledException)
            {
                logger.WriteInfo(new LogMessage("Operation canceled by user"), logCategories);
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    logger.WriteError(ie, logCategories);
                }
                throw;
            }
            catch (Exception ex)
            {
                logger.WriteError(ex, logCategories);
                throw;
            }
        }
        #endregion
    }
}
