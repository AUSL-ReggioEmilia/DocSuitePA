using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Validation;

namespace VecompSoftware.DocSuiteWeb.Service.Workflow
{
    public static class ServiceHelper
    {
        #region [ Fields ]
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };
        #endregion

        #region [ Properties ]
        public static JsonSerializerSettings SerializerSettings => _serializerSettings;

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
                throw new DSWException(string.Concat("Service workflow layer - AggregateException in invoke operation: ", string.Join(", ", ae.InnerExceptions.Select(f => f.Message))),
                    ae.Flatten(), DSWExceptionCode.SS_Anomaly);

            }
            catch (DSWValidationException) { throw; }
            catch (DSWException ex)
            {
                logger.WriteError(ex, logCategories);
                throw ex;
            }
            catch (Exception ex)
            {
                logger.WriteError(ex, logCategories);
                throw new DSWException(string.Concat("Service workflow layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.SS_Anomaly);
            }
        }

        #endregion
    }
}
