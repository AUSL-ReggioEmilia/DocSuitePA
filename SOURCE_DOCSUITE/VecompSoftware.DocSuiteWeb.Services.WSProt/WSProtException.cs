using System;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt
{
    public class WSProtException : Exception 
    {
        public WSProtException(string message) : base(message)
        {
        }

        public WSProtException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}