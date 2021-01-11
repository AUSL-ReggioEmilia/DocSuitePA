using System.Web.Http;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { action = RouteParameter.Optional }
            );
        }
    }
}
