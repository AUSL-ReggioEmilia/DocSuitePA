using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.SPID.DataProtection;
using VecompSoftware.DocSuite.SPID.JWT;

namespace VecompSoftware.DocSuite.SPID.Portal.Code
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly IJwtService _jwtService;
        private readonly IDataProtectionService _dataProtectionService;
        public CustomJwtBearerEvents(IJwtService jwtService, IDataProtectionService dataProtectionService)
        {
            _dataProtectionService = dataProtectionService;
            _jwtService = jwtService;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await Task.Run(() =>
            {
                context.NoResult();
                if (context.SecurityToken is JwtSecurityToken accessToken)
                {
                    string cachedToken = _jwtService.Find(accessToken.Id);
                    if (!string.IsNullOrEmpty(cachedToken))
                    {
                        context.Success();
                    }
                    else
                    {
                        context.Fail("Cannot find token in identity server");
                    }
                }
            });         
        }
    }

}
