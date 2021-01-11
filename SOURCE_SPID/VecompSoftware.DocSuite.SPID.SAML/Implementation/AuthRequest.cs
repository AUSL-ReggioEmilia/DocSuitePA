using System;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Common.Extensions;
using VecompSoftware.DocSuite.SPID.Common.Logging;
using VecompSoftware.DocSuite.SPID.Model.SAML;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;

namespace VecompSoftware.DocSuite.SPID.SAML
{
    public class AuthRequest : IAuthRequest
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly ILogger _traceLogger;
        private readonly ISignatureHelper _signatureHelper;
        private readonly IAuthRequestTypeMapper _authRequestTypeMapper;
        private readonly ILogOutRequestTypeMapper _logOutRequestTypeMapper;
        #endregion

        #region [ Constructor ]
        public AuthRequest(ILoggerFactory logger, ISignatureHelper signatureHelper, 
            IAuthRequestTypeMapper authRequestTypeMapper, ILogOutRequestTypeMapper logOutRequestTypeMapper)
        {
            _logger = logger.CreateLogger(LogCategories.SAML);
            _traceLogger = logger.CreateLogger(LogCategories.SAMLTRACE);
            _signatureHelper = signatureHelper;
            _authRequestTypeMapper = authRequestTypeMapper;
            _logOutRequestTypeMapper = logOutRequestTypeMapper;
        }

        #endregion

        #region [ Methods ]
        public string PostableAuthRequest(SamlRequestOption requestOption, string xmlPrivateKey = "")
        {
            try
            {
                _logger.LogInformation("PostableAuthRequest -> initialize request for IDP {0}", requestOption.Destination);
                _logger.LogInformation("PostableAuthRequest -> creating request for authentication...");
                AuthnRequestType authnRequest = _authRequestTypeMapper.Map(requestOption);
                _logger.LogInformation("PostableAuthRequest -> request created with id {0}", authnRequest.ID);

                XmlDocument xmlRequest = new XmlDocument();
                xmlRequest.LoadXml(authnRequest.ToXmlString());

                _logger.LogInformation("PostableAuthRequest -> generating signature for id {0}...", authnRequest.ID);
                XmlElement signatureElement = _signatureHelper.GetXmlAuthRequestSignature(xmlRequest, requestOption.Certificate, xmlPrivateKey);
                xmlRequest.DocumentElement.InsertAfter(signatureElement, xmlRequest.DocumentElement.ChildNodes[0]);
                _logger.LogInformation("PostableAuthRequest -> signature generated correctly");
                
                _logger.LogDebug("PostableAuthRequest -> request id: {0} xml: {1}", authnRequest.ID, xmlRequest.OuterXml);
                _traceLogger.LogInformation("AuthnReq_ID: {0}|AuthnReq_IssueInstant: {1}|AuthnReq_SAML: {2}", authnRequest.ID, authnRequest.IssueInstant, xmlRequest.OuterXml);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlRequest.OuterXml));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PostableAuthRequest -> error on creating postable auth request");
                throw;
            }            
        }

        public string RedirectableAuthRequest(SamlRequestOption requestOption, string xmlPrivateKey = "")
        {
            try
            {
                _logger.LogInformation("RedirectableAuthRequest -> initialize request for IDP {0}", requestOption.Destination);
                AuthnRequestType authnRequest = _authRequestTypeMapper.Map(requestOption);
                authnRequest.ProtocolBinding = SamlNamespaceHelper.SAML_PROTOCOL_BINDING_REDIRECT_NAMESPACE;
                _logger.LogInformation("RedirectableAuthRequest -> request created with id {0}", authnRequest.ID);

                string compressedRequest = WebUtility.UrlEncode(CompressRequest(authnRequest));
                string sigAlg = WebUtility.UrlEncode(SignatureHelper.SIGNATURE_ALGORITHM_SHA256);

                _logger.LogInformation("RedirectableAuthRequest -> generating signature for id {0}...", authnRequest.ID);
                string tmpRequest = string.Concat("SAMLRequest=", compressedRequest, "&SigAlg=", sigAlg);
                string requestSignature = _signatureHelper.SignMessage(tmpRequest, requestOption.Certificate, xmlPrivateKey);
                _logger.LogInformation("RedirectableAuthRequest -> signature generated correctly");
                
                string requestQueryString = string.Concat(tmpRequest, "&Signature=", requestSignature);
                _logger.LogDebug("RedirectableAuthRequest -> request id {0} - query string: {1}", authnRequest.ID, requestQueryString);
                _traceLogger.LogInformation("AuthnReq_ID: {0}|AuthnReq_IssueInstant: {1}|AuthnReq_QS: {2}", authnRequest.ID, authnRequest.IssueInstant, requestQueryString);
                return requestQueryString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RedirectableAuthRequest -> error on creating redirectable auth request");
                throw;
            }            
        }

        public string PostableLogOutRequest(SamlRequestOption requestOption, string xmlPrivateKey = "")
        {
            try
            {
                _logger.LogInformation("PostableLogOutRequest -> initialize logout request for IDP {0}", requestOption.Destination);
                LogoutRequestType logOutRequest = _logOutRequestTypeMapper.Map(requestOption);
                _logger.LogInformation("PostableLogOutRequest -> request created with id {0}", logOutRequest.ID);
                XmlDocument xmlRequest = new XmlDocument();
                xmlRequest.LoadXml(logOutRequest.ToXmlString());

                _logger.LogInformation("PostableLogOutRequest -> generating signature for id {0}...", logOutRequest.ID);
                XmlElement signatureElement = _signatureHelper.GetXmlAuthRequestSignature(xmlRequest, requestOption.Certificate, xmlPrivateKey);
                xmlRequest.DocumentElement.InsertAfter(signatureElement, xmlRequest.DocumentElement.ChildNodes[0]);
                _logger.LogInformation("PostableLogOutRequest -> signature generated correctly");
                
                _logger.LogDebug("PostableLogOutRequest -> request id {0} - xml: {1}", logOutRequest.ID, xmlRequest.OuterXml);
                _traceLogger.LogInformation("LogoutReq_ID: {0}|LogouteReq_IssueInstant: {1}|LogouteReq_SAML: {2}", logOutRequest.ID, logOutRequest.IssueInstant, xmlRequest.OuterXml);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlRequest.OuterXml));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PostableLogOutRequest -> error on creating postable logout request");
                throw;
            }
        }

        private string CompressRequest(RequestAbstractType request)
        {
            string xmlRequest = request.ToXmlString();
            using (MemoryStream compressedStream = new MemoryStream())
            using (MemoryStream uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlRequest)))
            {
                using (var compressorStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
                {
                    uncompressedStream.CopyTo(compressorStream);
                }
                return Convert.ToBase64String(compressedStream.ToArray());
            }
        }
        #endregion
    }
}
