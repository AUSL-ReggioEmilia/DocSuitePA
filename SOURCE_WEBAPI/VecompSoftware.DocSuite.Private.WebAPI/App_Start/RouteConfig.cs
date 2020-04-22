using System.Web.Http;

namespace VecompSoftware.DocSuite.Private.WebAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
               name: "ServiceBus_DefaultApi",
               routeTemplate: "api/sb/{controller}/{action}",
               defaults: new { action = RouteParameter.Optional });

            routes.MapHttpRoute(
                name: "Entity_DefaultApi",
                routeTemplate: "api/entity/{controller}/{action}",
                defaults: new { action = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "UDS_DefaultApi",
                routeTemplate: "api/uds/{controller}/{action}",
                defaults: new { action = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "WorkflowManager_DefaultApi",
                routeTemplate: "api/wfm/{controller}/{action}",
                defaults: new { action = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "Builder_DefaultApi",
                routeTemplate: "api/builder/{controller}/{action}",
                defaults: new { action = RouteParameter.Optional }
            );
        }
    }
}