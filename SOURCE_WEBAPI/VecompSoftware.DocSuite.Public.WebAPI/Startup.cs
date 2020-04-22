using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;
using System.Web.Http.Dependencies;
using VecompSoftware.DocSuite.Public.WebAPI.Handlers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;

[assembly: OwinStartup(typeof(VecompSoftware.DocSuite.Public.WebAPI.Startup))]

namespace VecompSoftware.DocSuite.Public.WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            IDependencyResolver resolver = UnityConfig.GetConfiguredContainer();
            IDataUnitOfWork dataUnitOfWork = resolver.GetService(typeof(IDataUnitOfWork)) as IDataUnitOfWork;
            ILogger logger = resolver.GetService(typeof(ILogger)) as ILogger;
            ITopicService topicService = resolver.GetService(typeof(ITopicService)) as ITopicService;
            config.DependencyResolver = resolver;

            ConfigureOAuth(appBuilder, dataUnitOfWork, logger, topicService);
            WebApiConfig.Register(config);
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app, IDataUnitOfWork dataUnitOfWork, ILogger logger, ITopicService topicService)
        {
            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/Auth/Token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(8),
                Provider = new DSWAuthorizationServerProvider(dataUnitOfWork, logger, topicService)
            };

            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}

