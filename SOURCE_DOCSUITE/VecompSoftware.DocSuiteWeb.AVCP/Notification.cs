using System;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class Notification
    {
        #region ND_Error enum

        public enum ND_Error
        {
            OK = 0,
            ER_FIELD_REQUIRED,
            ER_FIELD_ERROR,
            ER_FILE_NOT_FOUND,
            ER_IMPORT_ERROR,
            ER_TEMPLATE_NOT_FOUND,
            ER_FIELD_COUNTERROR,
            ER_FILE_FORMAT,
            ER_STORAGE_GET_ERROR,
            ER_STORAGE_ADD_ERROR,
            ER_STORAGE_UPDATE_ERROR,
            ER_LOTTO_NOT_FOUND,
            ER_LOTTO_MULTIPLE_FOUND
        }

        #endregion

        public Notification()
        {
            ErrorID = (int) ND_Error.OK;
            Message = String.Empty;
            ExceptionMessage = String.Empty;
        }

        public int ErrorID { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
    }

    public class LogEventArg : EventArgs
    {
      public string Info = String.Empty;
    }

}