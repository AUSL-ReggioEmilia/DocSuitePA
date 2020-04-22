using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Services.BiblosDS
{
    public static class DocumentHelper
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public static async Task<T> TryCatchWithLogger<T>(Func<Task<T>> func, ILogger logger, IEnumerable<LogCategory> logCategories)
        {
            try
            {
                return await func();
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is Exception)
                    {
                        logger.WriteError(x, logCategories);
                    }
                    return false;
                });
                throw new DSWException(string.Concat("Document BiblosDS layer - AggregateException in invoke operation: ", string.Join(", ", ae.InnerExceptions.Select(f => f.Message))),
                    ae.Flatten(), DSWExceptionCode.DM_Anomaly);
            }
            catch (DSWException) { throw; }
            catch (Exception ex)
            {
                logger.WriteError(ex, logCategories);
                throw new DSWException(string.Concat("Document BiblosDS layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }

        public static async Task TryCatchWithLogger(Func<Task> func, ILogger logger, IEnumerable<LogCategory> logCategories)
        {
            try
            {
                await func();
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is Exception)
                    {
                        logger.WriteError(x, logCategories);
                    }
                    return false;
                });
                throw new DSWException(string.Concat("Document BiblosDS layer - AggregateException in invoke operation: ", string.Join(", ", ae.InnerExceptions.Select(f => f.Message))),
                    ae.Flatten(), DSWExceptionCode.DM_Anomaly);
            }
            catch (DSWException) { throw; }
            catch (Exception ex)
            {
                logger.WriteError(ex, logCategories);
                throw new DSWException(string.Concat("Document BiblosDS layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }

        #endregion
    }
}
