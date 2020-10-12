using log4net;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BiblosDS.LegalExtension.AdminPortal
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
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