using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VecompSoftware.DocSuite.SPID.Token;

namespace VecompSoftware.DocSuite.SPID.JWT
{
    public static class JwtSPIDProvider
    {
        public static void AddJwtSPID(this IServiceCollection services, IConfiguration configuration, SymmetricSecurityKey signingKey)
        {
            services.AddSingleton<ITokenService, JwtService>();
            services.AddSingleton<IJwtService, JwtService>();
            //TODO: da rivedere con logiche migliori di persistenza
            services.AddDistributedMemoryCache();

            IConfigurationSection jwtAppSettings = configuration.GetSection(nameof(JwtConfiguration));

            services.Configure<JwtConfiguration>(options =>
            {
                options.Issuer = jwtAppSettings.GetValue<string>(nameof(JwtConfiguration.Issuer));
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });
        }
    }
}
