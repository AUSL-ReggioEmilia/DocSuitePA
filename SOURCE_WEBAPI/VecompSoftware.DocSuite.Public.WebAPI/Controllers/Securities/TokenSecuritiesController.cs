using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Models.ExternalSecurities;
using VecompSoftware.DocSuite.Public.WebAPI.Handlers;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Securities
{
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class TokenSecuritiesController : ApiController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        protected readonly Guid _instanceId;
        private readonly ITopicService _topicService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IParameterEnvService _parameterEnvService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IMessageConfiguration _messageConfiguration;
        #endregion

        #region [ Constructor ]
        public TokenSecuritiesController(ILogger logger, ITopicService topicService, IParameterEnvService parameterEnvService,
            IDataUnitOfWork unitOfWork, ICQRSMessageMapper cqrsMapper, IMessageConfiguration messageConfiguration)
            : base()
        {
            _logger = logger;
            _topicService = topicService;
            _parameterEnvService = parameterEnvService;
            _instanceId = Guid.NewGuid();
            _unitOfWork = unitOfWork;
            _cqrsMapper = cqrsMapper;
            _messageConfiguration = messageConfiguration;
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
        public async Task<IHttpActionResult> Post(Guid authenticationId, string documentUnit, short year, int number)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                _logger.WriteInfo(new LogMessage($"Token securities receive a request for authenticationId {authenticationId}, documentUnit {documentUnit}, year {year} and number {number}"), LogCategories);
                if (!DSWAuthorizationServerProvider.ValidAuthenticationList.Any(f => f == authenticationId))
                {
                    _logger.WriteWarning(new LogMessage($"AuthenticationId {authenticationId} is not valid"), LogCategories);
                    throw new DSWSecurityException($"AuthenticationId {authenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                }
                DocumentUnit reference = null;
                switch (documentUnit)
                {
                    case "Protocol":
                        {
                            reference = _unitOfWork.Repository<DocumentUnit>().GetByNumbering(year, number, (int)DSWEnvironmentType.Protocol, optimization: true);
                            break;
                        }
                    default:
                        {
                            _logger.WriteWarning(new LogMessage($"AuthenticationId {authenticationId} has no valid documentUnit '{documentUnit}' name"), LogCategories);
                            throw new DSWSecurityException($"AuthenticationId {authenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                        }
                }
                if (reference == null)
                {
                    _logger.WriteWarning(new LogMessage($"DocumentUnit {documentUnit} - {year}/{number} not found"), LogCategories);
                    throw new DSWSecurityException($"DocumentUnit {documentUnit} - {year}/{number} not found", null, DSWExceptionCode.SC_InvalidAccount);
                }
                Guid currentToken = Guid.NewGuid();
                DateTimeOffset creationDate = DateTimeOffset.UtcNow;
                DateTimeOffset expiryDate = creationDate.AddMilliseconds(30 * 1000);
                HostIdentify hostIdentify = new HostIdentify(Environment.MachineName, "Public WebAPI");
                TokenSecurityModel tokenModel = new TokenSecurityModel()
                {
                    AuthenticationId = authenticationId,
                    ExpiryDate = expiryDate,
                    Token = currentToken,
                    Host = hostIdentify,
                    DocumentUnitAuhtorized = new DocSuiteWeb.Model.Entities.DocumentUnits.DocumentUnitModel()
                    {
                        Environment = reference.Environment,
                        UniqueId = reference.UniqueId,
                        Year = reference.Year,
                        Number = reference.Number.ToString(),
                        Title = reference.Title
                    }
                };
                string username = "localmachine\\anonymous_api";
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                IdentityContext identityContext = new IdentityContext(username);
                EventTokenSecurity eventTokenSecurity = new EventTokenSecurity(Guid.NewGuid(), currentToken,_parameterEnvService.CurrentTenantName, _parameterEnvService.CurrentTenantId, 
                    Guid.Empty, identityContext, tokenModel, null);
                _logger.WriteDebug(new LogMessage($"Generated token {currentToken} by {username} expiry on {expiryDate}"), LogCategories);
                string topicName = _messageConfiguration.GetConfigurations()[eventTokenSecurity.EventName].TopicName;
                string dynamicSubscriptionName = currentToken.ToString();
                await _topicService.CreateSubscriptionAsync(topicName, dynamicSubscriptionName, currentToken.ToString(), eventTokenSecurity.EventName);
                
                ServiceBusMessage message = _cqrsMapper.Map(eventTokenSecurity, new ServiceBusMessage());
                ServiceBusMessage response = await _topicService.SendToTopicAsync(message);
                return Ok(currentToken);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
