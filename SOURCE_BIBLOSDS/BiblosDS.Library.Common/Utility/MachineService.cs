using System.Configuration;

namespace BiblosDS.Library.Common.Objects.UtilityService
{
    public class MachineService
    {
        public static string GetServerName()
        {
            if (ConfigurationManager.AppSettings["ServerName"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"].ToString()))
                return ConfigurationManager.AppSettings["ServerName"].ToString();
            else
                return System.Environment.MachineName;
        }
        
    }
}
