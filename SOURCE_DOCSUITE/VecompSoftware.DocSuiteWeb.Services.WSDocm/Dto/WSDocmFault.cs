using System.Runtime.Serialization;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm.Dto
{
    [DataContract]
    public class WSDocmFault
    {
        #region Fields
        private string _title;
        private string _message;
        #endregion

        #region Constructor
        public WSDocmFault(string message)
        {
            _message = message;
        }

        public WSDocmFault(string title, string message)
        {
            _title = title;
            _message = message;
        }
        #endregion

        #region Properties
        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        public string Message
        {
            get { return this._message; }
            set { this._message = value; }
        }
        #endregion
    }
}