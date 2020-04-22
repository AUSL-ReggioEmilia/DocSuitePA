using System;
using System.Linq;

namespace VecompSoftware.DocSuite.SPID.Logging.Configurations
{
    public class LoggingUrlHelpers
    {
        internal const string LOGGING_URL = "/spid.logging";

        public static bool IsLoggingUrl(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), "IsLoggingUrl -> parameter url is null");
            }

            return UrlMatchesAppenderUrl(url);
        }

        private static bool UrlMatchesAppenderUrl(string url)
        {
            string[] urlParts = url.Split(new char[] { '?' });
            string urlWithoutQuery = urlParts.First();

            bool result = urlWithoutQuery.EndsWith(LOGGING_URL);
            return result;
        }
    }
}
