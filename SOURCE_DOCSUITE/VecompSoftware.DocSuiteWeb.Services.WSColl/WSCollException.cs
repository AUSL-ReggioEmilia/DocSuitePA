using System;

namespace VecompSoftware.DocSuiteWeb.Services.WSColl
{
    public class WSCollException : Exception 
    {
        public WSCollException(string message) : base(message) {}

        public WSCollException(string message, Exception innerException) : base(message, innerException) { }
    }
}