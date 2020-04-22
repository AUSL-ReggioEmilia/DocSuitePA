using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VecompSoftware.DocSuite.SPID.SAML;
using VecompSoftware.DocSuite.SPID.AuthEngine.Auth;
using VecompSoftware.DocSuite.SPID.AuthEngine.Models;
using VecompSoftware.DocSuite.SPID.Model.IDP;
using VecompSoftware.DocSuite.SPID.AuthEngine.Helpers;

namespace VecompSoftware.DocSuite.SPID.AuthEngine
{
    public static class SAMLAuthProvider
    {
        public static void AddSAMLAuth(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection authSettings = configuration.GetSection(AuthConfiguration.CONFIGURATION_SECTION_NAME);
            services.AddSaml(authSettings.GetValue<IdpType>(nameof(AuthConfiguration.IdpType)));

            services.AddSingleton<RequestOptionFactory>();
            services.AddSingleton<IdpHelper>();
            
            services.AddOptions();

            services.Configure<AuthConfiguration>(configuration.GetSection(AuthConfiguration.CONFIGURATION_SECTION_NAME));
        }
    }
}
