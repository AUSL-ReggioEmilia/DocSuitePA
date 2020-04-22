using System.Configuration;
using System.Web.Hosting;

namespace BiblosDS.LegalExtension.AdminPortal.Helpers
{
    public static class ConfigurationHelper
    {
        public static string GetArchiveConfigurationFilePath(string archiveName)
        {
            return HostingEnvironment.MapPath(string.Format("~/App_Data/{0}.json", archiveName));
        }

        public static string GetAppDataPath()
        {
            return HostingEnvironment.MapPath("~/App_Data");
        }

        public static string PDVArchiveName
        {
            get
            {
                return ConfigurationManager.AppSettings["PDVArchiveName"];
            }
        }

        public static string RDVArchiveName
        {
            get
            {
                return ConfigurationManager.AppSettings["RDVArchiveName"];
            }
        }

        public static string WCFHostWebAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WCFHostWebAPIUrl"];
            }
        }
    }
}