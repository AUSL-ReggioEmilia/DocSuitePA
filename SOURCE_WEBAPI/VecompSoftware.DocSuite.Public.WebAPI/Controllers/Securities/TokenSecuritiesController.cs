using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.WebAPI.Handlers;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Services.Command.CQRS.Events.Models.ExternalSecurities;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Securities
{
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class TokenSecuritiesController : ApiController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        protected readonly Guid _instanceId;
        private readonly ITopicService _service;

        #endregion

        #region [ Constructor ]
        public TokenSecuritiesController(ILogger logger, ITopicService service)
            : base()
        {
            _logger = logger;
            _service = service;
            _instanceId = Guid.NewGuid();
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(TokenSecuritiesController));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Methods ]
        private const string _filter_tokenAuthenticationId = "TokenAuthenticationId";
        private const string _filter_tokenExpiryDate = "TokenExpiryDate";
        [HttpPost]
        public async Task<IHttpActionResult> Post(Guid authenticationId)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                _logger.WriteInfo(new LogMessage(string.Concat("Token securities receive a request for authenticationId ", authenticationId)), LogCategories);
                if (!DSWAuthorizationServerProvider.ValidAuthenticationList.Any(f => f == authenticationId))
                {
                    _logger.WriteInfo(new LogMessage(string.Concat("AuthenticationId ", authenticationId, " is not valid")), LogCategories);
                    throw new DSWSecurityException(string.Concat("AuthenticationId ", authenticationId, " is not valid"), null, DSWExceptionCode.SC_InvalidAccount);
                }
                IEventTokenSecurity tokenSecurityModel = await DSWAuthorizationServerProvider.TryGetValidToken(_service, authenticationId, f => _logger.WriteDebug(new LogMessage(f), LogCategories));
                if (tokenSecurityModel == null)
                {
                    _logger.WriteInfo(new LogMessage(string.Concat("AuthenticationId ", authenticationId, " has no valid token")), LogCategories);
                    throw new DSWSecurityException(string.Concat("AuthenticationId ", authenticationId, " has no valid token"), null, DSWExceptionCode.SC_InvalidAccount);
                }
                return Ok(tokenSecurityModel.ContentType.ContentTypeValue.Token);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
