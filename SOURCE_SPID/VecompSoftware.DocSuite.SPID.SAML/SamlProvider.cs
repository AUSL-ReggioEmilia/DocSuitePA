using Microsoft.Extensions.DependencyInjection;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using SPIDIdpMapper = VecompSoftware.DocSuite.SPID.Mapper.SPIDIdp;
using FedERaIdpMapper = VecompSoftware.DocSuite.SPID.Mapper.FederaIdp;
using VecompSoftware.DocSuite.SPID.Model.IDP;

namespace VecompSoftware.DocSuite.SPID.SAML
{
    public static class SamlProvider
    {
        public static void AddSaml(this IServiceCollection services, IdpType idpType)
        {
            services.AddScoped<IAuthRequest, AuthRequest>();
            services.AddScoped<IAuthResponse, AuthResponse>();
            services.AddScoped<ILogoutResponse, LogoutResponse>();
            services.AddScoped<ISignatureHelper, SignatureHelper>();

            switch (idpType)
            {
                case IdpType.SPID:
                    AddSPIDProvider(services);
                    break;
                case IdpType.FedERa:
                    AddFedERaProvider(services);
                    break;
            }            
        }

        private static void AddSPIDProvider(IServiceCollection services)
        {
            services.AddScoped<IAuthRequestTypeMapper, SPIDIdpMapper.AuthRequestTypeMapper>();
            services.AddScoped<ILogOutRequestTypeMapper, SPIDIdpMapper.LogOutRequestTypeMapper>();
            services.AddScoped<ISamlUserMapper, SPIDIdpMapper.SamlUserMapper>();
            services.AddScoped<ISamlResponseStatusMapper, SPIDIdpMapper.SamlResponseStatusMapper>();
            services.AddScoped<ISamlResponseStatusMessageMapper, SPIDIdpMapper.SamlResponseStatusMessageMapper>();
        }

        private static void AddFedERaProvider(IServiceCollection services)
        {
            services.AddScoped<IAuthRequestTypeMapper, FedERaIdpMapper.AuthRequestTypeMapper>();
            services.AddScoped<ILogOutRequestTypeMapper, FedERaIdpMapper.LogOutRequestTypeMapper>();
            services.AddScoped<ISamlUserMapper, FedERaIdpMapper.SamlUserMapper>();
            services.AddScoped<ISamlResponseStatusMapper, FedERaIdpMapper.SamlResponseStatusMapper>();
            services.AddScoped<ISamlResponseStatusMessageMapper, FedERaIdpMapper.SamlResponseStatusMessageMapper>();
        }
    }
}
