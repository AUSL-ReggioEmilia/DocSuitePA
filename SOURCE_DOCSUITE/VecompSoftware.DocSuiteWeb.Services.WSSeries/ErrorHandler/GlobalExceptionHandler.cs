using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.WSSeries.ErrorHandler
{
    public sealed class GlobalExceptionHandler : IErrorHandler
    {

        #region [ Constants ]

        public const string LoggerName = "WSSeriesLog";

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

            var newEx = new FaultException(string.Format("Il Sistema di gestione eccezioni è stato chiamato dal metodo {0}", ex.TargetSite.Name));

            MessageFault msgFault = newEx.CreateMessageFault();
            msg = Message.CreateMessage(version, msgFault, newEx.Action);
        }

        #endregion
    }
}