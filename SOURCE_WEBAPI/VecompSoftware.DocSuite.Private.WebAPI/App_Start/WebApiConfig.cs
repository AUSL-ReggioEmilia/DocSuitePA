using Microsoft.AspNet.OData.Extensions;
using System.Web.Http;
using VecompSoftware.DocSuite.Private.WebAPI.Filters;
using VecompSoftware.DocSuite.Private.WebAPI.Handlers;

namespace VecompSoftware.DocSuite.Private.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new DSWExceptionFilterAttribute());


            // Web API configuration and services
            config.MapHttpAttributeRoutes(new DSWWebApiRouteProvider());

            config.EnableCors();

            ODataConfig.Register(config);
            config.EnableDependencyInjection();
            config.EnsureInitialized();


        }

    }
}
