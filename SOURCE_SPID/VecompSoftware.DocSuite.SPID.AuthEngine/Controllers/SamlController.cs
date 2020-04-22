using System;
using Microsoft.AspNetCore.Mvc;
using VecompSoftware.DocSuite.SPID.AuthEngine.Helpers;
using VecompSoftware.DocSuite.SPID.Model.SAML;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VecompSoftware.DocSuite.SPID.AuthEngine.Models;
using VecompSoftware.DocSuite.SPID.AuthEngine.Auth;
using VecompSoftware.DocSuite.SPID.SAML;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Actions;
using VecompSoftware.DocSuite.SPID.Common.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using VecompSoftware.DocSuite.SPID.Model.Auth;
using System.Linq;
using VecompSoftware.DocSuite.SPID.Model.Tokens;
using VecompSoftware.DocSuite.SPID.Model.Tokens.Types;
using VecompSoftware.DocSuite.SPID.DataProtection;
using VecompSoftware.DocSuite.SPID.Token;

namespace VecompSoftware.DocSuite.SPID.AuthEngine.Controllers
{
    public class SamlController : Controller
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly ILogger _traceLogger;
        private readonly ILogger _sessionAuthLogger;
        private readonly AuthConfiguration _spidConfiguration;
        private readonly RequestOptionFactory _requestOptionFactory;
        private readonly IAuthRequest _authRequest;
        private readonly IAuthResponse _authResponse;
        private readonly ILogoutResponse _logoutResponse;
        private readonly ITokenService _tokenService;
        private readonly IdpHelper _idpHelper;
        private readonly IDataProtectionService _dataProtectionService;
        #endregion

        #region [ Constructor ]
        public SamlController(ILoggerFactory logger, IOptions<AuthConfiguration> spidConfiguration, 
            RequestOptionFactory requestOptionFactory, IAuthRequest authRequest, IAuthResponse authResponse, ILogoutResponse logoutResponse,
            ITokenService tokenService, IdpHelper idpHelper, IDataProtectionService dataProtectionService)
        {
            _logger = logger.CreateLogger(LogCategories.AUTHENGINE);
            _traceLogger = logger.CreateLogger(LogCategories.SAMLTRACE);
            _sessionAuthLogger = logger.CreateLogger(LogCategories.AUTHSESSION);
            _spidConfiguration = spidConfiguration?.Value;
            _requestOptionFactory = requestOptionFactory;
            _authRequest = authRequest;
            _authResponse = authResponse;
            _logoutResponse = logoutResponse;
            _tokenService = tokenService;
            _idpHelper = idpHelper;
            _dataProtectionService = dataProtectionService;
        }
        #endregion

        #region [ Methods ]
        private void ClearCookies()
        {
            this.RemoveCookie("SpidAuthnRequestId");
            this.RemoveCookie("SpidLogoutRequestId");
        }

        public IActionResult Auth(string idp)
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                if(string.IsNullOrEmpty(idp))
                {
                    throw new ArgumentNullException(nameof(idp), "Auth -> parameter idp is null");
                }

                string idpUrl = _idpHelper.GetSingleSignOnUrl(idp);
                if(string.IsNullOrEmpty(idpUrl))
                {
                    throw new Exception(string.Concat("Auth -> idp url not found for idp ", idpUrl));
                }

                SamlRequestOption samlRequestOption = _requestOptionFactory.GenerateAuthRequestOption(idp);
                if(samlRequestOption == null)
                {
                    throw new Exception("Auth -> error on generate saml model option");
                }

                string samlrequest = _authRequest.PostableAuthRequest(samlRequestOption, _spidConfiguration.CertificatePrivateKey);

                ClearCookies();
                this.SetCookie("SpidAuthnRequestId", samlRequestOption.Id.ToString(), 20);

                ViewData["SAMLRequest"] = samlrequest;
                ViewData["FormUrlAction"] = idpUrl;
                ViewData["RelayState"] = Guid.NewGuid();
                return View();
            }, _logger);
        }

        [HttpPost]
        public IActionResult ACS(IFormCollection collection)
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                string idpEncodedResponse = collection["SAMLResponse"];
                if(string.IsNullOrEmpty(idpEncodedResponse))
                {
                    throw new Exception("ACS -> SAMLResponse not found on request");
                }

                string authnRequestId = this.GetCookie("SpidAuthnRequestId");
                ClearCookies();

                ViewData["Callback"] = _spidConfiguration.ACSCallback;

                byte[] idpResponseBytes = Convert.FromBase64String(idpEncodedResponse);
                string samlResponse = Encoding.UTF8.GetString(idpResponseBytes);
                SamlResponse model = _authResponse.Deserialize(samlResponse, authnRequestId);                
                if(!model.IsValid)
                {
                    _logger.LogWarning("ACS -> SAMLResponse is not valid");
                    ViewData["AuthenticationResult"] = new AuthenticationResult();
                    ViewData["IsAuthenticated"] = false;
                    return View();
                }

                string idpName = _idpHelper.GetIdpNameFromIssuerId(model.Issuer);
                TokenResult jwtToken = _tokenService.Create(model);
                TokenRequest tokenRequest = new TokenRequest()
                {
                    IdpName = idpName,
                    Token = (jwtToken.Token as JwtToken).Token,
                    ReferenceCode = jwtToken.ReferenceCode,
                    TokenGrantType = TokenGrantType.SamlAuth
                };
                return CreateToken(tokenRequest);
            }, _logger);
        }

        [HttpPost]
        public IActionResult IdpLogout(IFormCollection collection)
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                string idpEncodedResponse = collection["SAMLResponse"];
                if (string.IsNullOrEmpty(idpEncodedResponse))
                {
                    throw new Exception("ACS -> SAMLResponse not found on request");
                }

                string logOutRequestId = this.GetCookie("SpidLogoutRequestId");
                ClearCookies();

                byte[] idpResponseBytes = Convert.FromBase64String(idpEncodedResponse);
                string samlResponse = Encoding.UTF8.GetString(idpResponseBytes);
                SamlResponse model = _logoutResponse.Deserialize(samlResponse, logOutRequestId);
                if (!model.IsValid)
                {
                    _logger.LogWarning("ACS -> SAMLResponse is not valid");
                }
                return LocalRedirect("~/pages/login");
            }, _logger);
        }

        [HttpGet]
        public IActionResult IdpLogout()
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                ViewData["LogoutCallback"] = _spidConfiguration.LogoutCallback;
                return View("IdpLogout");
            }, _logger);
        }

        [Route("Saml/LogOut")]
        public IActionResult LogOut(string ReferenceCode, string IdpName)
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                if (string.IsNullOrEmpty(ReferenceCode))
                {
                    _logger.LogWarning("LogOut -> parameter reference code is null");
                    return BadRequest();
                }                

                string idpUrl = _idpHelper.GetSingleLogoutUrl(IdpName);
                if (string.IsNullOrEmpty(idpUrl))
                {
                    throw new Exception(string.Concat("Auth -> idp url not found for idp ", idpUrl));
                }

                string token = _tokenService.Find(_dataProtectionService.Unprotect(ReferenceCode));
                string idpReferenceId = string.Empty;
                if (!string.IsNullOrEmpty(token))
                {
                    JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                    idpReferenceId = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId)?.Value;
                }
                _tokenService.Remove(ReferenceCode);
                _sessionAuthLogger.LogInformation("Disconnessione effettuata correttamente dall' utente {0}", idpReferenceId);

                ViewData["FormUrlAction"] = idpUrl;
                if (_spidConfiguration.IdpType == Model.IDP.IdpType.FedERa)
                {
                    ViewData["SPID"] = _spidConfiguration.SPDomain;
                    ViewData["SPURL"] = _spidConfiguration.LogoutCallback;
                    _traceLogger.LogInformation("LogoutReq_SPID: {0}|LogoutReq_SPURL: {1}", _spidConfiguration.SPDomain, _spidConfiguration.LogoutCallback);                    
                    return View("LogOutFedera");
                }

                SamlRequestOption samlRequestOption = _requestOptionFactory.GenerateLogoutRequestOption(IdpName);
                if (samlRequestOption == null)
                {
                    throw new Exception("Auth -> error on generate saml model option");
                }

                string samlrequest = _authRequest.PostableLogOutRequest(samlRequestOption, _spidConfiguration.CertificatePrivateKey);

                ClearCookies();

                this.SetCookie("SpidLogoutRequestId", samlRequestOption.Id.ToString(), 20);

                ViewData["RelayState"] = Guid.NewGuid();
                ViewData["SAMLRequest"] = samlrequest;                
                return View("LogOutSPID");
            }, _logger);
        }

        [HttpPost]
        [Route("Saml/Token")]
        public IActionResult CreateToken([FromBody]TokenRequest request)
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                if (request == null)
                {
                    _logger.LogWarning("CreateToken -> parameter request is null");
                    return Unauthorized();
                }

                if (string.IsNullOrEmpty(request.Token))
                {
                    _logger.LogWarning("CreateToken -> parameter token is null");
                    return Unauthorized();
                }

                if (string.IsNullOrEmpty(request.ReferenceCode))
                {
                    _logger.LogWarning("CreateToken -> parameter reference code is null");
                    return Unauthorized();
                }

                if (request.TokenGrantType == TokenGrantType.SamlAuth)
                {
                    AuthenticationResult result = new AuthenticationResult() { TokenResult = new TokenResult() };
                    result.TokenResult.Token = new JwtToken(request.Token);
                    result.TokenResult.ReferenceCode = request.ReferenceCode;
                    ViewData["Token"] = request.Token;
                    ViewData["ReferenceCode"] = request.ReferenceCode;
                    ViewData["IdpName"] = request.IdpName;
                    ViewData["IsAuthenticated"] = true;
                    ViewData["AuthenticationResult"] = JsonConvert.SerializeObject(result);
                    return View("ACS");
                }
                else
                {
                    string refreshedToken = _tokenService.Refresh(request.ReferenceCode, request.Token);
                    if (string.IsNullOrEmpty(refreshedToken))
                    {
                        _logger.LogWarning("CreateToken -> cannot refresh Token expiration date");
                        return Unauthorized();
                    }
                    return Json(refreshedToken);
                }                
            }, _logger);
        }
        #endregion        
    }
}