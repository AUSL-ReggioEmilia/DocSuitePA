using VecompSoftware.DocSuite.SPID.Token;

namespace VecompSoftware.DocSuite.SPID.JWT
{
    public interface IJwtService : ITokenService
    {
        string Clone(string referenceCode, bool unprotectData);
    }
}