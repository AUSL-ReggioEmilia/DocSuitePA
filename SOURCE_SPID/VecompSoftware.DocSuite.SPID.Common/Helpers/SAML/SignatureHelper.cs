using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System;
using Microsoft.Extensions.Logging;
using VecompSoftware.DocSuite.SPID.Common.Extensions;
using VecompSoftware.DocSuite.SPID.Common.Logging;

namespace VecompSoftware.DocSuite.SPID.Common.Helpers.SAML
{
    public class SignatureHelper : ISignatureHelper
    {
        #region [ Fields ]
        public const string SIGNATURE_ALGORITHM_SHA256 = "http://www.w3.org/2001/04/xmldsig#rsa-sha256";
        public const string SIGNATURE_ALGORITHM_MORE_SHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        public const string DIGEST_METHOD_SHA256 = "http://www.w3.org/2001/04/xmlenc#sha256";
        public const string XML_DSIG_ENVELOP_SIGNATURE = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
        public const string XML_DSIG_EXC_C14N = "http://www.w3.org/2001/10/xml-exc-c14n#";

        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public SignatureHelper(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger(LogCategories.GENERAL);
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// Genera l'xml della segnatura da appendere alla richiesta di login Saml
        /// </summary>
        /// <param name="xmlRequest"></param>
        /// <param name="certificate"></param>
        /// <param name="xmlPrivateKey"></param>
        /// <returns></returns>
        public XmlElement GetXmlAuthRequestSignature(XmlDocument xmlRequest, X509Certificate2 certificate, string xmlPrivateKey)
        {
            if (xmlRequest == null)
                throw new ArgumentNullException(nameof(xmlRequest), "GetXmlAuthRequestSignature -> parameter xmlRequest is null");

            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate), "GetXmlAuthRequestSignature -> certificate not found");

            _logger.LogInformation("GetXmlAuthRequestSignature -> generating signature xml for request...");
            if (string.IsNullOrEmpty(xmlPrivateKey))
            {
                xmlPrivateKey = (certificate.GetRSAPrivateKey()).ToXmlStringExt();
            }
            SignedXml signedRequest = SignAuthRequest(xmlRequest, certificate, xmlPrivateKey);
            _logger.LogInformation("GetXmlAuthRequestSignature -> signature xml created correctly");
            return signedRequest.GetXml();
        }

        /// <summary>
        /// Crea l'oggetto SignedXml per la richiesta di login Saml
        /// </summary>
        /// <param name="xmlRequest"></param>
        /// <param name="certificate"></param>
        /// <param name="xmlPrivateKey"></param>
        /// <returns></returns>
        private SignedXml SignAuthRequest(XmlDocument xmlRequest, X509Certificate2 certificate, string xmlPrivateKey)
        {
            using (RSACryptoServiceProvider rSACryptoService = new RSACryptoServiceProvider(new CspParameters(24)))
            {
                rSACryptoService.PersistKeyInCsp = false;
                rSACryptoService.FromXmlStringExt(xmlPrivateKey);

                SignedXml signedXml = new SignedXml(xmlRequest)
                {
                    SigningKey = rSACryptoService
                };
                signedXml.SignedInfo.SignatureMethod = rSACryptoService.GetSigningAlgorithmName();
                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

                Reference reference = new Reference()
                {
                    Uri = string.Concat("#", xmlRequest.DocumentElement.GetAttribute("ID")),
                    DigestMethod = DIGEST_METHOD_SHA256
                };
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigExcC14NTransform());

                signedXml.AddReference(reference);

                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(certificate));
                signedXml.KeyInfo = keyInfo;
                signedXml.ComputeSignature();

                return signedXml;
            }
        }

        /// <summary>
        /// Firma un messaggio tramite certificato X509
        /// </summary>
        /// <param name="message"></param>
        /// <param name="certificate"></param>
        /// <param name="xmlPrivateKey"></param>
        /// <returns></returns>
        public string SignMessage(string message, X509Certificate2 certificate, string xmlPrivateKey)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "SignMessage -> no message to sign");

            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate), "SignMessage -> certificate not found");

            if (string.IsNullOrEmpty(xmlPrivateKey))
            {
                xmlPrivateKey = (certificate.GetRSAPrivateKey()).ToXmlStringExt();
            }
            using (RSACryptoServiceProvider rSACryptoService = new RSACryptoServiceProvider(new CspParameters(24)))
            {
                rSACryptoService.PersistKeyInCsp = false;
                rSACryptoService.FromXmlStringExt(xmlPrivateKey);
                byte[] signedMessage = rSACryptoService.SignData(Encoding.UTF8.GetBytes(message), new SHA256CryptoServiceProvider());
                return Convert.ToBase64String(signedMessage);
            }
        }

        /// <summary>
        /// Validazione tramite chiave pubblica dell'xml di risposta dall'IDP
        /// </summary>
        /// <param name="xmlResponse"></param>
        /// <returns></returns>
        public bool ValidateSignature(XmlDocument xmlResponse)
        {
            if (xmlResponse == null)
                throw new ArgumentNullException(nameof(xmlResponse), "Parameter xmlResponse not defined");

            using (RSACryptoServiceProvider rSACryptoService = new RSACryptoServiceProvider(new CspParameters(24)))
            {
                XmlNamespaceManager manager = new XmlNamespaceManager(xmlResponse.NameTable);
                manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
                manager.AddNamespace("saml", SamlNamespaceHelper.SAML_ASSERTION_NAMESPACE);
                manager.AddNamespace("saml2p", SamlNamespaceHelper.SAML_PROTOCOL_NAMESPACE);
                XmlNodeList signatureNodes = xmlResponse.SelectNodes("//ds:Signature", manager);

                SignedXml signedXml = new SignedXml(xmlResponse);
                signedXml.LoadXml((XmlElement)signatureNodes[0]);

                return signedXml.CheckSignature();
            }
        }
        #endregion
    }
}
