using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.SAML
{
    public interface IAuthResponse
    {
        SamlResponse Deserialize(string SpidResponse, string authnRequestId);
        bool Validate(string spidResponse);
    }
}
