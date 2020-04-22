using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.Common.Extensions
{
    public static class AuthnRequestTypeExtension
    {
        public static string ToXmlString(this RequestAbstractType request)
        {
            string result = string.Empty;
            XmlSerializer serializer = new XmlSerializer(request.GetType());
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("saml2p", SamlNamespaceHelper.SAML_PROTOCOL_NAMESPACE);
                    namespaces.Add("saml", SamlNamespaceHelper.SAML_ASSERTION_NAMESPACE);
                    serializer.Serialize(writer, request, namespaces);
                    result = stringWriter.ToString();
                }
            }
            return result;
        }
    }
}
