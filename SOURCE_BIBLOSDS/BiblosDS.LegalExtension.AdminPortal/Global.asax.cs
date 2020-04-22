using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using log4net;
using System.Configuration;

namespace BiblosDS.LegalExtension.AdminPortal
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                bool isRoleEnviromentAvailable = false;
                try
                {
                    isRoleEnviromentAvailable = RoleEnvironment.IsAvailable;
                }
                catch
                {
                    isRoleEnviromentAvailable = false;                    
                }

                string value = "";
                if (isRoleEnviromentAvailable)
                    value = RoleEnvironment.GetConfigurationSettingValue(configName);
                else
                    value = ConfigurationManager.AppSettings[configName];
                configSetter(value);
            });

            if (!LogManager.GetRepository().Configured)
                log4net.Config.XmlConfigurator.Configure();
            logger.Info("Application_Start");
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}