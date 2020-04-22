using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.SAML
{
    public interface IAuthRequest
    {
        string PostableAuthRequest(SamlRequestOption requestOption, string xmlPrivateKey = "");
        string RedirectableAuthRequest(SamlRequestOption requestOption, string xmlPrivateKey = "");
        string PostableLogOutRequest(SamlRequestOption requestOption, string xmlPrivateKey = "");
    }
}
