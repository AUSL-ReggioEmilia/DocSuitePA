using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using VecompSoftware.DocSuite.Private.WebAPI.Middleware;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;

namespace VecompSoftware.DocSuite.Private.WebAPI
{
    public partial class Startup
    {
        static Startup()
        {
        }

        public void ConfigureAPI(IAppBuilder appBuilder)
        {
            if (WebApiConfiguration.ShibbolethEnabled)
            {
                appBuilder.Use(typeof(DSWShibbolethMiddleware));
            }

            appBuilder.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);

                if (WebApiConfiguration.SignalRMaxIncomingWebSocketMessageSize.HasValue && WebApiConfiguration.SignalRMaxIncomingWebSocketMessageSize.Value > 0) 
                {
					GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = WebApiConfiguration.SignalRMaxIncomingWebSocketMessageSize;
				}                

                HubConfiguration hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    EnableJSONP = true,
                    EnableDetailedErrors = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
            appBuilder.UseCors(CorsOptions.AllowAll);
        }
    }
}
