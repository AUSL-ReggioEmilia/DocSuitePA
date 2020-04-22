using Microsoft.Extensions.Logging;
using System;
using System.Xml;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Common.Logging;
using VecompSoftware.DocSuite.SPID.Model.SAML;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Common;

namespace VecompSoftware.DocSuite.SPID.SAML
{
    public class AuthResponse : IAuthResponse
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly ILogger _traceLogger;
        private readonly ILogger _sessionAuthLogger;
        private readonly ISignatureHelper _signatureHelper;
        private readonly ISamlUserMapper _samlUserMapper;
        private readonly ISamlResponseStatusMapper _samlResponseStatusMapper;
        private readonly ISamlResponseStatusMessageMapper _samlResponseStatusMessageMapper;
        #endregion

        #region [ Constructor ]
        public AuthResponse(ILoggerFactory logger, ISignatureHelper signatureHelper,
            ISamlUserMapper samlUserMapper, ISamlResponseStatusMapper samlResponseStatusMapper,
            ISamlResponseStatusMessageMapper samlResponseStatusMessageMapper)
        {
            _logger = logger.CreateLogger(LogCategories.SAML);
            _traceLogger = logger.CreateLogger(LogCategories.SAMLTRACE);
            _sessionAuthLogger = logger.CreateLogger(LogCategories.AUTHSESSION);
            _signatureHelper = signatureHelper;
            _samlUserMapper = samlUserMapper;
            _samlResponseStatusMapper = samlResponseStatusMapper;
            _samlResponseStatusMessageMapper = samlResponseStatusMessageMapper;
        }
        #endregion

        #region [ Methods ]
        public SamlResponse Deserialize(string spidResponse, string authnRequestId)
        {
            ResponseType response = new ResponseType();
            try
            {
                _logger.LogInformation("Deserialize -> deserializing response...");
                _logger.LogDebug("Deserialize -> SAML response: {0}", spidResponse);
                response = XmlHelper.Deserialize<ResponseType>(spidResponse);
                _traceLogger.LogInformation("AuthnResp_ID: {0}|AuthnResp_IssueInstant: {1}|AuthnResp_Issuer: {2}|AuthnResp_SAML: {3}", response.ID, response.IssueInstant, response.Issuer, spidResponse);                

                SamlResponse model = new SamlResponse()
                {
                    Version = response.Version,
                    Id = response.ID,
                    SPRequestId = response.InResponseTo,
                    Issuer = response.Issuer.Value,
                    ResponseDate = response.IssueInstant
                };

                _logger.LogInformation("Deserialize -> validate response with id {0}...", model.Id);
                bool signatureIsValid = Validate(spidResponse);
                if (!signatureIsValid)
                {
                    _logger.LogWarning("Deserialize -> SAML response is not valid");
                    model.Status = SamlResponseStatus.ValidationError;
                    return model;
                }
                _logger.LogInformation("Deserialize -> response validated");

                _logger.LogInformation("Deserialize -> mapping status code for response {0}...", model.Id);
                model.Status = _samlResponseStatusMapper.Map(response.Status.StatusCode);
                _logger.LogInformation("Deserialize -> response status code {0}", model.Status);
                if (!model.IsValid)
                {
                    if (!string.IsNullOrEmpty(response.Status.StatusMessage))
                    {
                        model.StatusMessage = _samlResponseStatusMessageMapper.Map(response.Status.StatusMessage);
                    }

                    _logger.LogWarning("Deserialize -> response is wrong", model.Status.ToString());
                    _logger.LogWarning("Deserialize -> request error status code: {0}", model.Status.ToString());
                    _logger.LogWarning("Deserialize -> request error status message: {0}", model.StatusMessage);
                    return model;
                }

                if (response.InResponseTo != $"_{authnRequestId}")
                {
                    _logger.LogWarning("Deserialize -> response is not valid. InResponseTo: {0} - AuthnRequest Id: {1}", response.InResponseTo, authnRequestId);
                    model.Status = SamlResponseStatus.ValidationError;
                    return model;
                }

                _logger.LogInformation("Deserialize -> mapping user informations for response {0}...", model.Id);
                model.User = _samlUserMapper.Map(response);
                _traceLogger.LogInformation("AuthnResp_ID: {0}|AuthnResp_IdpReferenceId: {1}", response.ID, model.User.IdpReferenceId);
                _sessionAuthLogger.LogInformation("Autenticazione effettuata correttamente dall'utente {0} (AuthnResp_ID: {1})", model.User.IdpReferenceId, response.ID);
                _logger.LogInformation("Deserialize -> SAML response {0} deserialized correctly", model.Id);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deserialize -> error on deserialize response");
                throw ex;
            }
        }

        public bool Validate(string spidResponse)
        {
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(spidResponse);
            bool signatureIsValid = _signatureHelper.ValidateSignature(xmlResponse);

            //DateTime? notBefore = NotBefore(xmlResponse);
            //_logger.LogDebug("notBefore {0} - Now {1}", notBefore, DateTime.Now);
            //signatureIsValid = signatureIsValid && (!notBefore.HasValue || (notBefore <= DateTime.Now));

            DateTime? notOnOrAfter = NotOnOrAfter(xmlResponse);
            _logger.LogDebug("notOnOrAfter {0} - Now {1}", notOnOrAfter, DateTime.Now);
            signatureIsValid = signatureIsValid && (!notOnOrAfter.HasValue || (notOnOrAfter > DateTime.Now));

            return signatureIsValid;
        }

        private DateTime? NotBefore(XmlDocument xmlResponse)
        {
            return CheckConditionDate(xmlResponse, "NotBefore");
        }

        private DateTime? NotOnOrAfter(XmlDocument xmlResponse)
        {
            return CheckConditionDate(xmlResponse, "NotOnOrAfter");
        }

        private DateTime? CheckConditionDate(XmlDocument xmlResponse, string conditionName)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlResponse.NameTable);
            manager.AddNamespace("saml", SamlNamespaceHelper.SAML_ASSERTION_NAMESPACE);
            manager.AddNamespace("saml2p", SamlNamespaceHelper.SAML_PROTOCOL_NAMESPACE);

            var nodes = xmlResponse.SelectNodes("/saml2p:Response/saml:Assertion/saml:Conditions", manager);
            string value = null;
            if (nodes != null && nodes.Count > 0 && nodes[0] != null && nodes[0].Attributes != null && nodes[0].Attributes[conditionName] != null)
            {
                value = nodes[0].Attributes[conditionName].Value;
            }
            return value != null ? DateTime.Parse(value) : (DateTime?)null;
        }
        #endregion
    }
}
