using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace VecompSoftware.DocSuite.SPID.Common.Helpers.SAML
{
    public interface ISignatureHelper
    {
        XmlElement GetXmlAuthRequestSignature(XmlDocument xmlRequest, X509Certificate2 certificate, string xmlPrivateKey);
        string SignMessage(string message, X509Certificate2 certificate, string xmlPrivateKey);
        bool ValidateSignature(XmlDocument xmlResponse);
    }
}