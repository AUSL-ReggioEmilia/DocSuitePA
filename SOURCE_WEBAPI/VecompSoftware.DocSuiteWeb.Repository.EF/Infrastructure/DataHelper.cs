using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.Infrastructure
{
    public static class DataHelper
    {
        #region [ Fields ]
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            MaxDepth = 2,
            TypeNameHandling = TypeNameHandling.Objects,
            NullValueHandling = NullValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public static async Task<T> TryCatchWithLogger<T>(Func<Task<T>> func, ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, DSWExceptionCode code)
        {
            try
            {
                return await func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    if (ie is DbEntityValidationException)
                    {
                        EvaluateExeption(logger, logCategories, methodName, ie as DbEntityValidationException);
                    }
                    else
                    {
                        if (ie is DbUpdateConcurrencyException)
                        {
                            EvaluateExeption(logger, logCategories, methodName, ie as DbUpdateConcurrencyException);
                        }
                        else
                        {
                            if (ie is DbUpdateException)
                            {
                                EvaluateExeption(logger, logCategories, methodName, ie as DbUpdateException);
                            }
                            else
                            {
                                EvaluateExeption(logger, logCategories, methodName, ie as Exception, code);
                            }
                        }
                    }
                }
                throw new DSWException(string.Concat(methodName, ".Exception -> ", ae.Message), ae, code);
            }
            catch (DbEntityValidationException dbv_ex)
            {
                EvaluateExeption(logger, logCategories, methodName, dbv_ex);
            }
            catch (DbUpdateConcurrencyException dbc_ex)
            {
                EvaluateExeption(logger, logCategories, methodName, dbc_ex);
            }
            catch (DbUpdateException dbu_ex)
            {
                EvaluateExeption(logger, logCategories, methodName, dbu_ex);
            }
            catch (Exception ex)
            {
                EvaluateExeption(logger, logCategories, methodName, ex, code);
            }
            throw new DSWException(string.Concat(methodName, ".Exception -> <TryCatchWithLogger no path found>"), null, code);
        }

        public static T TryCatchWithLogger<T>(Func<T> func, ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, DSWExceptionCode code)
        {
            try
            {
                return func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    if (ie is DbEntityValidationException)
                    {
                        EvaluateExeption(logger, logCategories, methodName, ie as DbEntityValidationException);
                    }
                    else
                    {
                        if (ie is DbUpdateConcurrencyException)
                        {
                            EvaluateExeption(logger, logCategories, methodName, ie as DbUpdateConcurrencyException);
                        }
                        else
                        {
                            if (ie is DbUpdateException)
                            {
                                EvaluateExeption(logger, logCategories, methodName, ie as DbUpdateException);
                            }
                            else
                            {
                                EvaluateExeption(logger, logCategories, methodName, ie as Exception, code);
                            }
                        }
                    }
                }
                throw new DSWException(string.Concat(methodName, ".Exception -> ", ae.Message), ae, code);
            }
            catch (DbEntityValidationException dbv_ex)
            {
                EvaluateExeption(logger, logCategories, methodName, dbv_ex);
            }
            catch (DbUpdateConcurrencyException dbc_ex)
            {
                EvaluateExeption(logger, logCategories, methodName, dbc_ex);
            }
            catch (DbUpdateException dbu_ex)
            {
                EvaluateExeption(logger, logCategories, methodName, dbu_ex);
            }
            catch (Exception ex)
            {
                EvaluateExeption(logger, logCategories, methodName, ex, code);
            }
            throw new DSWException(string.Concat(methodName, ".Exception -> <TryCatchWithLogger no path found>"), null, code);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, Exception ex, DSWExceptionCode code)
        {
            logger.WriteError(ex, logCategories);
            throw new DSWException(string.Concat(methodName, ".Exception -> ", ex.Message), ex, code);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, UpdateException dbu_ex)
        {
            if (dbu_ex.StateEntries != null)
            {
                foreach (ObjectStateEntry result in dbu_ex.StateEntries.Where(f => f.Entity != null))
                {
                    logger.WriteWarning(new LogMessage(string.Concat("Type: ", result.Entity.GetType().Name, " was part of the problem")), logCategories);
                }
            }

            logger.WriteError(dbu_ex, logCategories);
            throw new DSWException(string.Concat(methodName, ".UpdateException -> ", dbu_ex.Message), dbu_ex, DSWExceptionCode.DB_EntityStateError);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, DbUpdateException dbu_ex)
        {
            if (dbu_ex.Entries != null)
            {
                foreach (DbEntityEntry result in dbu_ex.Entries.Where(f => f.Entity != null))
                {
                    logger.WriteWarning(new LogMessage(string.Concat("Type: ", result.Entity.GetType().Name, " was part of the problem")), logCategories);
                }
            }

            if (dbu_ex.InnerException != null && dbu_ex.InnerException is UpdateException)
            {
                EvaluateExeption(logger, logCategories, methodName, dbu_ex.InnerException as UpdateException);
            }
            logger.WriteError(dbu_ex, logCategories);
            throw new DSWException(string.Concat(methodName, ".UpdateException -> ", dbu_ex.Message), dbu_ex, DSWExceptionCode.DB_EntityStateError);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, DbUpdateConcurrencyException dbc_ex)
        {
            if (dbc_ex.Entries != null)
            {
                foreach (DbEntityEntry result in dbc_ex.Entries.Where(f => f.Entity != null))
                {
                    logger.WriteWarning(new LogMessage(string.Concat("Type: ", result.Entity.GetType().Name, " was part of the problem")), logCategories);
                }
            }

            logger.WriteError(dbc_ex, logCategories);
            throw new DSWException(string.Concat(methodName, ".UpdateConcurrencyException -> ", dbc_ex.Message), dbc_ex, DSWExceptionCode.DB_ConcurrencyError);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, DbEntityValidationException dbv_ex)
        {
            foreach (DbEntityValidationResult errors in dbv_ex.EntityValidationErrors)
            {
                if (errors.Entry != null && errors.Entry.Entity != null)
                {
                    logger.WriteWarning(new LogMessage(JsonConvert.SerializeObject(errors.Entry.Entity, Formatting.Indented, _jsonSerializerSettings)), logCategories);
                }

                foreach (DbValidationError error in errors.ValidationErrors)
                {
                    logger.WriteWarning(new LogMessage(string.Concat(error.PropertyName, " -> ", error.ErrorMessage)), logCategories);
                }
            }
            logger.WriteError(dbv_ex, logCategories);
            throw new DSWException(string.Concat(methodName, ".ValidationException -> ", dbv_ex.Message), dbv_ex, DSWExceptionCode.DB_Validation);
        }
        #endregion
    }
}
