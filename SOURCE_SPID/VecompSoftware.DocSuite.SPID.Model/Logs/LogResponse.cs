using System;
using System.Collections.Generic;
using System.Text;

namespace VecompSoftware.DocSuite.SPID.Model.Logs
{
    public class LogResponse
    {
        public LogResponse()
        {
            Headers = new Dictionary<string, string>();
        }

        public int StatusCode { get; set; }
        public IDictionary<string, string> Headers { get; set; }
    }
}
