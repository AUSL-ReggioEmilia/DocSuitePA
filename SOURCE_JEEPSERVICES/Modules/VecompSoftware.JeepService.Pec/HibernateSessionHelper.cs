using NHibernate;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.Pec
{
    internal class HibernateSessionHelper
    {
        #region Constuctor/Dispose

        static HibernateSessionHelper()
        {

        }

        private HibernateSessionHelper()
        {

        }
        #endregion Constuctor/Dispose

        #region Properties

        #endregion Properties
        internal static String SessionName { get { return "ProtDB"; } }

        internal static ISession NHibernateSession
        {
            get { return NHibernateSessionManager.Instance.GetSessionFrom(SessionName); }
        }

        #region Properties

        #endregion Properties

        #region Methods

        private static String parseError(Exception ex)
        {
            if (ex is SqlException)
            {
                SqlException ex_sql = ex as SqlException;
                StringBuilder errors = new StringBuilder();
                foreach (SqlError sql_error in ex_sql.Errors)
                {
                    errors.AppendLine(sql_error.ToString());
                }
                return string.Format("{0} -> code: {1} number: {2}, sql messages: \n{3}",
                    ex.GetType().FullName, ex_sql.ErrorCode, ex_sql.Number, errors.ToString());

            }
            return ex.GetType().FullName;
        }
        private static StringBuilder buildErrorMessage(StringBuilder error, Exception ex)
        {
            if (ex == null || error == null)
            {
                return error;
            }
            return buildErrorMessage(error, ex.InnerException)
                .AppendLine(parseError(ex))
                .AppendLine(ex.Message);
        }

        internal static void TryOrRollback(Action action, String LoggerName, String errorMessage = "", bool recursiveMessageError = false)
        {
            TryOrRollback(new List<Action>() { action }, LoggerName, errorMessage, recursiveMessageError, 0, 1);
        }

        internal static void TryOrRollback(ICollection<Action> action, String LoggerName, String errorMessage = "", bool recursiveMessageError = false,
            ushort pass = 0, ushort max = 3)
        {
            ITransaction transaction = null;
            try
            {
                transaction = NHibernateSession.BeginTransaction();
                action.ElementAt(pass)();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                {
                    //todo: prevedere una visione di tentativi specifici per sql error 8152 : es String or binary data would be truncated
                    // deve ad ogni passi si esegue il tentativo successivo previsto dal chiamate 

                    /* if(pass++ < max)
                    {
                        TryOrRollback(action, errorMessage, recursiveMessageError, pass);
                    }
   
                     * http://msdn.microsoft.com/en-us/library/cc645597.aspx
                     * Exception:System.Data.SqlClient.SqlException -> code: -2146232060 number: 8152, sql messages: 
                        System.Data.SqlClient.SqlError: String or binary data would be truncated.
                        System.Data.SqlClient.SqlError: The statement has been terminated.
                     * */


                }
                if (transaction != null && transaction.IsActive)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (ObjectDisposedException){ }
                    
                }
                FileLogger.Error(LoggerName, ex.Message, ex);
                throw new Exception(recursiveMessageError ? buildErrorMessage(new StringBuilder(), ex).ToString() : errorMessage, ex);
            }
            finally
            {
                try
                {
                    transaction.Dispose();
                }
                catch (Exception){}
            }
        }
        #endregion Methods
    }
}
