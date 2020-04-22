using System;

namespace VecompSoftware.DocSuiteWeb.Services.WSSeries
{
    public class WSSeriesException : Exception 
    {
        public WSSeriesException(string message) : base(message) { }

        public WSSeriesException(string message, Exception innerException) : base(message, innerException) { }
    }
}