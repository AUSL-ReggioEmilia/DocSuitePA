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
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.Helpers.Workflow;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Securities
{
    [AzureAuthorize]
    [LogCategory(LogCategoryDefinition.SECURITY)]
    public class TokenSecuritiesController : ApiController
    {
        #region [ Fields ]

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        private readonly ITopicService _topicService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly ICQRSMessageMapper _cqrsMapper;
        private readonly IMessageConfiguration _messageConfiguration;
        #endregion

        #region [ Constructor ]
        public TokenSecuritiesController(ILogger logger, ITopicService topicService, IDecryptedParameterEnvService parameterEnvService,
            IDataUnitOfWork unitOfWork, ICQRSMessageMapper cqrsMapper, IMessageConfiguration messageConfiguration)
            : base()
        {
            _logger = logger;
            _topicService = topicService;
            _parameterEnvService = parameterEnvService;
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
        public async Task<IHttpActionResult> Post(Guid authenticationId, string documentUnit, short? year, string number)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                _logger.WriteInfo(new LogMessage($"Token securities receive a request for authenticationId {authenticationId}, documentUnit {documentUnit}, year {year} and number {number}"), LogCategories);
                if (!IsValidAuthenticationId(authenticationId))
                {
                    _logger.WriteWarning(new LogMessage($"WorkflowRepository with id '{authenticationId}' not found or doesn't have environment '{DSWEnvironmentType.Any}' or the _dsw_p_ExternalViewerIntegrationEnabled is not found"), LogCategories);
                    throw new DSWSecurityException($"AuthenticationId {authenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                }
                int tokenModelEnvironment = 0;
                short tokenModelYear = 0;
                string tokenModelNumber = string.Empty;
                Guid tokenModelUniqueId = Guid.Empty;
                string tokenModelTitle = string.Empty;
                switch (documentUnit)
                {
                    case "Protocol":
                        {
                            int protocolNumber = 0;
                            if (!year.HasValue || !int.TryParse(number, out protocolNumber))
                            {
                                _logger.WriteWarning(new LogMessage($"Year '{year}' and number {number} are not valid."), LogCategories);
                                throw new DSWSecurityException($"Year '{year}' and number {number} are not valid.", null, DSWExceptionCode.SC_InvalidAccount);
                            }
                            DocumentUnit reference = _unitOfWork.Repository<DocumentUnit>().GetByWorkflowRepositoryId(year.Value, protocolNumber, (int)DSWEnvironmentType.Protocol, authenticationId, optimization: true);
                            if (reference == null)
                            {
                                _logger.WriteWarning(new LogMessage($"DocumentUnit {documentUnit} - {year}/{number} not found"), LogCategories);
                                throw new DSWSecurityException($"DocumentUnit {documentUnit} - {year}/{number} not found", null, DSWExceptionCode.SC_InvalidAccount);
                            }
                            tokenModelEnvironment = reference.Environment;
                            tokenModelUniqueId = reference.UniqueId;
                            tokenModelYear = reference.Year;
                            tokenModelNumber = reference.Number.ToString();
                            tokenModelTitle = reference.Title;
                            break;
                        }
                    case "Collaboration":
                        {
                            Guid instanceId;
                            if (!Guid.TryParse(number, out instanceId))
                            {
                                _logger.WriteWarning(new LogMessage($"Number {number} is not valid."), LogCategories);
                                throw new DSWSecurityException($"Number {number} is not valid.", null, DSWExceptionCode.SC_InvalidAccount);
                            }
                            WorkflowInstance workflowInstance  = _unitOfWork.Repository<WorkflowInstance>().GetByColllaborationReferenceId(instanceId, authenticationId).SingleOrDefault();
                            if (workflowInstance == null)
                            {
                                _logger.WriteWarning(new LogMessage($"ReferenceId {documentUnit} - {number} not found"), LogCategories);
                                throw new DSWSecurityException($"DocumentUnit {documentUnit} - {number} not found", null, DSWExceptionCode.SC_InvalidAccount);
                            }
                            tokenModelEnvironment = (int)DSWEnvironmentType.Collaboration;
                            tokenModelUniqueId = workflowInstance.UniqueId;
                            tokenModelNumber = workflowInstance.InstanceId.ToString();
                            tokenModelTitle = workflowInstance.Subject;
                            break;
                        }
                    default:
                        {
                            _logger.WriteWarning(new LogMessage($"AuthenticationId {authenticationId} has no valid documentUnit '{documentUnit}' name"), LogCategories);
                            throw new DSWSecurityException($"AuthenticationId {authenticationId} is not valid", null, DSWExceptionCode.SC_InvalidAccount);
                        }
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
                        Environment = tokenModelEnvironment,
                        UniqueId = tokenModelUniqueId,
                        Year = tokenModelYear,
                        Number = tokenModelNumber,
                        Title = tokenModelTitle
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

        private bool IsValidAuthenticationId(Guid authenticationId)
        {
            WorkflowRepository workflowRepository = _unitOfWork.Repository<WorkflowRepository>().GetIncludingEvaluationProperties(authenticationId);

            if (workflowRepository == null || workflowRepository.DSWEnvironment != DSWEnvironmentType.Any)
            {
                return false;
            }

            WorkflowEvaluationProperty externalIntegrationProperty = workflowRepository.WorkflowEvaluationProperties.FirstOrDefault(p => p.Name == WorkflowPropertyHelper.DSW_PROPERTY_EXTERNALVIEWER_INTEGRATION_ENABLED);
            bool isExternalIntegrationEnabled = externalIntegrationProperty != null && externalIntegrationProperty.ValueBoolean.HasValue && externalIntegrationProperty.ValueBoolean.Value;

            return isExternalIntegrationEnabled;
        }

        #endregion
    }
}
