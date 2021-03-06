﻿using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command.CQRS.Events.Models.ExternalSecurities;

namespace VecompSoftware.DocSuite.Public.WebAPI.Handlers
{
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class DSWAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        #region [ Fields ]
        public const string CLAIM_ExternalViewer_AuthenticationRule = "http://schemas.vecompsoftware.it/beral/2016/09/identity/claims/DocSuite/Authentication/rule";
        public const string VALUE_ExternalViewer_AuthenticationRule_OAuth2 = "oauth2";
        public const string VALUE_ExternalViewer_AuthenticationRule_Token = "token";
        public const string CLAIM_ExternalViewer_OAuth2_Kind = "http://schemas.vecompsoftware.it/beral/2016/09/identity/claims/DocSuite/OAuth2/kind";
        public const string CLAIM_ExternalViewer_OAuth2_Year = "http://schemas.vecompsoftware.it/beral/2016/09/identity/claims/DocSuite/OAuth2/year";
        public const string CLAIM_ExternalViewer_OAuth2_Number = "http://schemas.vecompsoftware.it/beral/2016/09/identity/claims/DocSuite/OAuth2/number";
        public const string CLAIM_ExternalViewer_OAuth2_UniqueId = "http://schemas.vecompsoftware.it/beral/2016/09/identity/claims/DocSuite/OAuth2/uniqueId";
        public const string CLAIM_ExternalViewer_Token_Hash = "http://schemas.vecompsoftware.it/beral/2016/09/identity/claims/DocSuite/Token/hash";
        public const string CLAIM_ExternalViewer_Token_User = "http://schemas.vecompsoftware.it/beral/2016/09/identity/claims/DocSuite/Token/user";

        public const string ExternalViewerSecurityTokenSubscriptionName = "ExternalViewerSecurityToken";
        private const string _filter_tokenAuthenticationId = "TokenAuthenticationId";
        private const string _filter_tokenExpiryDate = "TokenExpiryDate";

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ITopicService _topicService;
        private readonly IMessageConfiguration _messageConfiguration;

        private static readonly object _lockObject = new object();
        private static IEnumerable<LogCategory> _logCategories = null;
        private static IReadOnlyList<Guid> _validAuthenticationList = null;
        private static IReadOnlyDictionary<string, KeyValuePair<Guid, string>> _mapper_authentication_integration = null;
        #endregion

        #region [ Constructor ]
        public DSWAuthorizationServerProvider(IDataUnitOfWork dataUnitOfWork, ILogger logger, ITopicService topicService,
            IMessageConfiguration messageConfiguration)
        {
            _unitOfWork = dataUnitOfWork;
            _logger = logger;
            _topicService = topicService;
            _messageConfiguration = messageConfiguration;
        }

        #endregion

        #region [ Properties ]
        public static IReadOnlyList<Guid> ValidAuthenticationList
        {
            get
            {
                if (_validAuthenticationList == null)
                {
                    lock (_lockObject)
                    {
                        if (_validAuthenticationList == null)
                        {
                            Guid **REMOVE**_ERPContract = Guid.Parse("51DAE147-3E30-4434-A73F-DCB5E062CA5E");
                            Guid **REMOVE**_SAPInvoice = Guid.Parse("49DC7004-EFC1-454A-9174-CEA12D06061E");
                            Guid auslre_AVELCO = Guid.Parse("7AF08013-7018-4583-942C-73441C680994");
                            Guid auslre_ZEN = Guid.Parse("C39ADF96-AAEF-4237-BA56-A22A8C6BD4EF");
                            _validAuthenticationList = new List<Guid>
                            {
                                auslre_ZEN, **REMOVE**_ERPContract, **REMOVE**_ERPContract, auslre_AVELCO, **REMOVE**_SAPInvoice
                            };
                            Dictionary<string, KeyValuePair<Guid, string>> local = new Dictionary<string, KeyValuePair<Guid, string>>
                            {
                                { "**REMOVE**.ERP_CONTRATTI", new KeyValuePair<Guid, string>(**REMOVE**_ERPContract, VALUE_ExternalViewer_AuthenticationRule_Token) },
                                { "**REMOVE**.SAP_FATTURE", new KeyValuePair<Guid, string>(**REMOVE**_SAPInvoice, VALUE_ExternalViewer_AuthenticationRule_Token) },
                                { "AUSLRE.AVELCO", new KeyValuePair<Guid, string>(auslre_AVELCO, VALUE_ExternalViewer_AuthenticationRule_Token) },
                                { "AUSLRE.ZEN", new KeyValuePair<Guid, string>(auslre_ZEN, VALUE_ExternalViewer_AuthenticationRule_Token) }
                            };
                            _mapper_authentication_integration = local;
                        }
                    }
                }
                return _validAuthenticationList;
            }
        }

        private static IReadOnlyDictionary<string, KeyValuePair<Guid, string>> ValidIntegrations
        {
            get
            {
                if (_mapper_authentication_integration == null)
                {
                    IReadOnlyList<Guid> tmp = ValidAuthenticationList;
                }
                return _mapper_authentication_integration;
            }
        }

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DSWAuthorizationServerProvider));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Methods ]

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            string appId, kind, username, @params, authRule, year, number, uniqueId, token, user;
            appId = kind = username = @params = authRule = year = number = uniqueId = token = user = string.Empty;
            try
            {
                _logger.WriteDebug(new LogMessage("GrantResourceOwnerCredentials"), LogCategories);
                IFormCollection forms = await context.OwinContext.Request.ReadFormAsync();

                appId = forms.Single(f => f.Key == "appId").Value.Single();
                _logger.WriteDebug(new LogMessage(string.Concat("AppID-> ", appId)), LogCategories);

                kind = forms.Single(f => f.Key == "kind").Value.Single();
                _logger.WriteDebug(new LogMessage(string.Concat("Kind-> ", kind)), LogCategories);

                username = forms.Single(f => f.Key == "username").Value.Single();
                _logger.WriteDebug(new LogMessage(string.Concat("Username-> ", username)), LogCategories);

                authRule = forms.Single(f => f.Key == "authRule").Value.Single();
                _logger.WriteDebug(new LogMessage(string.Concat("AuthRule-> ", authRule)), LogCategories);

                @params = forms.Single(f => f.Key == "params").Value.SingleOrDefault();
                _logger.WriteDebug(new LogMessage(string.Concat("Params-> ", @params)), LogCategories);

                if (kind == "Protocol")
                {
                    string[] keys = @params.Split('|');
                    year = keys.Single(f => f.StartsWith("year=")).Split('=').Last();
                    number = keys.Single(f => f.StartsWith("number=")).Split('=').Last();
                    uniqueId = _unitOfWork.Repository<Protocol>().GetByCompositeKey(short.Parse(year), int.Parse(number)).Single().UniqueId.ToString();
                }
                if (kind == "Protocol" && authRule == VALUE_ExternalViewer_AuthenticationRule_Token)
                {
                    string[] keys = @params.Split('|');
                    token = keys.Single(f => f.StartsWith("token=")).Split('=').Last();
                    user = keys.Single(f => f.StartsWith("user=")).Split('=').Last();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage(ex.Message), ex, LogCategories);
            }
            Guid authenticationId = Guid.Empty;
            KeyValuePair<Guid, string> currentIntegration;
            if (string.IsNullOrEmpty(appId) || !Guid.TryParse(appId, out authenticationId) || !ValidAuthenticationList.Any(f => f == authenticationId))
            {
                _logger.WriteWarning(new LogMessage($"The AppId '{appId}' is not valid."), LogCategories);
                context.SetError("invalid_grant", $"The AppId {appId} is not valid.");
                context.Rejected();
                return;
            }
            if (string.IsNullOrEmpty(kind))
            {
                _logger.WriteWarning(new LogMessage($"The Kind '{kind}' is not valid."), LogCategories);
                context.SetError("invalid_grant", "The Kind is not valid.");
                context.Rejected();
                return;
            }
            if (string.IsNullOrEmpty(username) || !ValidIntegrations.Any(f => f.Key == username))
            {
                _logger.WriteWarning(new LogMessage($"The Username '{username}' is not valid."), LogCategories);
                context.SetError("invalid_grant", $"The Username {username} is not valid.");
                context.Rejected();
                return;
            }

            if (string.IsNullOrEmpty(authRule))
            {
                _logger.WriteWarning(new LogMessage($"The AuthRule '{authRule}' is not valid."), LogCategories);
                context.SetError("invalid_grant", "The AuthRule is not valid.");
                context.Rejected();
                return;
            }
            currentIntegration = ValidIntegrations[username];
            if (currentIntegration.Key != authenticationId || currentIntegration.Value != authRule)
            {
                _logger.WriteWarning(new LogMessage($"{username}: The configuration is not valid."), LogCategories);
                context.SetError("invalid_grant", "The configuration is not valid.");
                context.Rejected();
                return;
            }

            if (kind == "Protocol" && authRule == VALUE_ExternalViewer_AuthenticationRule_OAuth2 &&
                (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(number)))
            {
                _logger.WriteWarning(new LogMessage($"The protocols oauth2 params {year}/{number} is not valid."), LogCategories);
                context.SetError("invalid_grant", "The protocols oauth2 params is not valid.");
                context.Rejected();
                return;
            }
            if (kind == "Protocol" && authRule == VALUE_ExternalViewer_AuthenticationRule_Token &&
                (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(year) || string.IsNullOrEmpty(number)))
            {
                _logger.WriteWarning(new LogMessage($"The protocols token params {user}/{token}/{year}/{number} is not valid."), LogCategories);
                context.SetError("invalid_grant", "The protocols token params is not valid.");
                context.Rejected();
                return;
            }
            if (kind == "Protocol" && authRule == VALUE_ExternalViewer_AuthenticationRule_Token)
            {
                string topicName = _messageConfiguration.GetConfigurations()["EventTokenSecurity"].TopicName;
                if (!await _topicService.SubscriptionExists(topicName, token))
                {
                    _logger.WriteWarning(new LogMessage($"AppId {appId} has no valid token {authenticationId}"), LogCategories);
                    context.SetError($"AppId {appId} has not validate token {authenticationId}", "The protocols params is not valid.");
                    context.Rejected();
                    return;
                }
                ServiceBusMessage serviceBusMessage = (await _topicService.GetMessagesAsync(topicName, token)).FirstOrDefault();
                IEventTokenSecurity tokenSecurityModel = null;
                if (serviceBusMessage == null || (tokenSecurityModel = (IEventTokenSecurity)serviceBusMessage.Content) == null)
                {
                    _logger.WriteWarning(new LogMessage($"AppId {appId} has no valid token"), LogCategories);
                    context.SetError($"AppId {appId} has not validate token {token}", "The protocols params is not valid.");
                    context.Rejected();
                    return;
                }
                if (tokenSecurityModel.ContentType.ContentTypeValue.DocumentUnitAuhtorized.UniqueId.ToString() != uniqueId)
                {
                    _logger.WriteWarning(new LogMessage($"AppId {appId} token is not valid for protocol {uniqueId}-{year}/{number}"), LogCategories);
                    context.SetError($"AppId {appId} token is not valid for protocol {uniqueId}-{year}/{number}", "The protocols params is not valid.");
                    context.Rejected();
                    return;
                }
                Guid secureToken = tokenSecurityModel.ContentType.ContentTypeValue.Token;
                _logger.WriteDebug(new LogMessage($"SecureToken is {secureToken}"), LogCategories);
            }
            ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(CLAIM_ExternalViewer_OAuth2_Kind, kind, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer, string.Empty));
            identity.AddClaim(new Claim(CLAIM_ExternalViewer_AuthenticationRule, authRule, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer, string.Empty));

            if (kind == "Protocol")
            {
                identity.AddClaim(new Claim(CLAIM_ExternalViewer_OAuth2_Year, year, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer, string.Empty));
                identity.AddClaim(new Claim(CLAIM_ExternalViewer_OAuth2_Number, number, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer, string.Empty));
                identity.AddClaim(new Claim(CLAIM_ExternalViewer_OAuth2_UniqueId, uniqueId, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer, string.Empty));

                if (authRule == VALUE_ExternalViewer_AuthenticationRule_Token)
                {
                    identity.AddClaim(new Claim(CLAIM_ExternalViewer_Token_Hash, token, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer, string.Empty));
                    identity.AddClaim(new Claim(CLAIM_ExternalViewer_Token_User, user, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer, string.Empty));
                }
            }
            context.Validated(identity);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            string accessToken = context.AccessToken;
            return Task.FromResult<object>(null);
        }

        #endregion
    }
}