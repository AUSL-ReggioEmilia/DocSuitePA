using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt.ErrorHandler
{
    public sealed class GlobalExceptionHandler : IErrorHandler
    {

        #region [ Constants ]

        public const string LoggerName = "WSProtLog";

        #endregion

        #region IErrorHandler Members

        public bool HandleError(Exception ex)
        {
            return true;
        }

        /// <summary>
        /// Metododo invocato in caso di eccezione
        /// </summary>
        public void ProvideFault(Exception ex, MessageVersion version, ref Message msg)
        {
            // Gestione del tipo di eccezione
            if (ex.GetType() == typeof(ArgumentException) || ex.GetType() == typeof(InvalidOperationException)) 
            {
                FileLogger.Warn(LoggerName, ex.Message, ex);
            }
            else 
            {
                FileLogger.Error(LoggerName, ex.Message, ex);
            }

            var newEx = new FaultException(string.Format("Il Sistema di gestione eccezioni è stato chiamato dal metodo {0}.\n{1}", 
                ex.TargetSite.Name, RecursiveExceptionLog(new StringBuilder(), ex).ToString()));

            MessageFault msgFault = newEx.CreateMessageFault();
            msg = Message.CreateMessage(version, msgFault, newEx.Action);
        }

        /// <summary>
        /// Metododo di logger ricorsivo
        /// </summary>
        private StringBuilder RecursiveExceptionLog(StringBuilder messages, Exception ex)
        {
            if (ex == null)
            {
                return messages ?? new StringBuilder();
            }
            messages = RecursiveExceptionLog(messages, ex.InnerException);
            messages.AppendLine(ex.Message);
            return messages;
        }
        #endregion
    }
}