using System.Collections.Generic;

namespace VecompSoftware.DocSuite.SPID.Model.Logs
{
    public class LogRequest
    {
        public string UserAgent { get; set; }
        public string UserHostAddress { get; set; }
        public string Url { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
    }
}
