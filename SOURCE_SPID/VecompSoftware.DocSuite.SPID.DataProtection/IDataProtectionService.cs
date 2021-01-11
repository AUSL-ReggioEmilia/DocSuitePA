using Microsoft.IdentityModel.Tokens;

namespace VecompSoftware.DocSuite.SPID.DataProtection
{
    public interface IDataProtectionService
    {
        string Protect(string content);
        string Unprotect(string protectedContent);
        SymmetricSecurityKey GetSigningKey();
    }
}
