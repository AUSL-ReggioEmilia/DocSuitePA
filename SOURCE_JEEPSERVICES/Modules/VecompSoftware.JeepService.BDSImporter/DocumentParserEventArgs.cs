using System;

namespace VecompSoftware.JeepService
{
    public class DocumentParserEventArgs : EventArgs
    {

        public DocumentParserEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }

    }
}
