using Microsoft.IdentityModel.Tokens;
using VecompSoftware.DocSuite.SPID.Model.Common;

namespace VecompSoftware.DocSuite.SPID.JWT
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }        
        public SigningCredentials SigningCredentials { get; set; }
    }
}
