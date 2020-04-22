using System;
using System.Collections.Generic;

namespace VecompSoftware.JeepService.Common
{
    public class MessageEventArgs
    {
        #region [ Fields ]

        private readonly String _message;
        private List<string> _recipients = new List<string>();

        #endregion

        #region [ Properties ]

        public String Message
        {
            get { return this._message; }
        }

        public List<string> Recipients
        {
            get { return this._recipients; }
        }

        #endregion

        #region [ Constructors ]

        public MessageEventArgs(String message)
        {
            this._message = message;
        }

        #endregion

        #region [ Delegates ]

        public delegate void MessageEventHandler(object sender, MessageEventArgs args);

        #endregion
    }
}
