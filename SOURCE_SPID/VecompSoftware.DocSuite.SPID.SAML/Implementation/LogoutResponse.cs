using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Common;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Common.Logging;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.SAML
{
    public class LogoutResponse : ILogoutResponse
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly ILogger _traceLogger;
        private readonly ISignatureHelper _signatureHelper;
        private readonly ISamlResponseStatusMapper _samlResponseStatusMapper;
        private readonly ISamlResponseStatusMessageMapper _samlResponseStatusMessageMapper;
        #endregion

        #region [ Constructor ]
        public LogoutResponse(ILoggerFactory logger, ISignatureHelper signatureHelper,
            ISamlResponseStatusMapper samlResponseStatusMapper,
            ISamlResponseStatusMessageMapper samlResponseStatusMessageMapper)
        {
            _logger = logger.CreateLogger(LogCategories.SAML);
            _traceLogger = logger.CreateLogger(LogCategories.SAMLTRACE);
            _signatureHelper = signatureHelper;
            _samlResponseStatusMapper = samlResponseStatusMapper;
            _samlResponseStatusMessageMapper = samlResponseStatusMessageMapper;
        }
        #endregion

        #region [ Methods ]
        public SamlResponse Deserialize(string spidResponse, string logOutRequestId)
        {
            ResponseType response = new ResponseType();
            try
            {
                string toDeserializeSpidResponse = spidResponse.Replace(":LogoutResponse", ":Response");
                _logger.LogInformation("Deserialize -> deserializing response...");
                _logger.LogDebug("Deserialize -> SAML response: {0}", spidResponse);
                response = XmlHelper.Deserialize<ResponseType>(toDeserializeSpidResponse);
                _traceLogger.LogInformation("LogoutResp_ID: {0}|LogoutResp_IssueInstant: {1}|LogoutResp_Issuer: {2}|LogoutResp_SAML: {3}", response.ID, response.IssueInstant, response.Issuer.Value, spidResponse);

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

                if (response.InResponseTo != $"_{logOutRequestId}")
                {
                    _logger.LogWarning("Deserialize -> response is not valid. InResponseTo: {0} - LogOutRequest Id: {1}", response.InResponseTo, logOutRequestId);
                    model.Status = SamlResponseStatus.ValidationError;
                    return model;
                }

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
            return signatureIsValid;
        }
        #endregion
    }
}