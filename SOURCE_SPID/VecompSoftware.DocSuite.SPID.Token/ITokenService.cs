using VecompSoftware.DocSuite.SPID.Model.SAML;
using VecompSoftware.DocSuite.SPID.Model.Tokens;

namespace VecompSoftware.DocSuite.SPID.Token
{
    public interface ITokenService
    {
        TokenResult Create(SamlResponse samlResponse);
        string Find(string referenceId);
        string Refresh(string referenceId, string tokenExpired);
        void Remove(string referenceId);
    }
}
