using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.SAML
{
    public interface ILogoutResponse
    {
        SamlResponse Deserialize(string spidResponse, string logOutRequestId);
        bool Validate(string spidResponse);
    }
}
