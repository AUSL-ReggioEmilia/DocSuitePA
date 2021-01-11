using Microsoft.Owin.Security.OAuth;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using VecompSoftware.DocSuite.WebAPI.Common;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;

namespace VecompSoftware.DocSuite.Public.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            if (WebApiConfiguration.SecurityContextType != DocSuiteWeb.Model.Parameters.SecurityContextType.OAuth2)
            {
                cors.SupportsCredentials = true;
            }
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            if (WebApiConfiguration.SecurityContextType == DocSuiteWeb.Model.Parameters.SecurityContextType.OAuth2)
            {
                config.SuppressDefaultHostAuthentication();
                config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            }
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter()
            {
                SerializerSettings = Defaults.DefaultJsonSerializer,
            });

            ODataConfig.Register(config);
            config.EnsureInitialized();
        }
    }
}
