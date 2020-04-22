using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuiteWeb.WebAPI.Controllers.OData.Messages
{
    public class MessageContactsController : BaseODataController<MessageContact, IMessageContactService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IWorkflowAuthorizationService _workflowAuthorizationService;
        private readonly ISecurity _security;
        private readonly IDomainUserModelMessageContactModelMapper _domainUserModelMessageContactModelMapper;
        #endregion

        #region [ Constructor ]
        public MessageContactsController(IMessageContactService service, IDataUnitOfWork unitOfWork, ILogger logger,
            IWorkflowAuthorizationService workflowAuthorizationService, ISecurity security, IDomainUserModelMessageContactModelMapper domainUserModelMessageContactModelMapper)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _workflowAuthorizationService = workflowAuthorizationService;
            _security = security;
            _domainUserModelMessageContactModelMapper = domainUserModelMessageContactModelMapper;
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetMessageRecipients(string mappingName, Guid workflowInstanceId, string internalActivityId)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetMessageRecipients -> mappingName = ", mappingName, ", workflowInstanceId = ", workflowInstanceId, ", internalActivityId = ", internalActivityId)), LogCategories);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<MessageContactModel> recipients = new List<MessageContactModel>();
                if (string.IsNullOrEmpty(mappingName))
                {
                    _logger.WriteError(new LogMessage("GetMessageRecipients -> Errore validazione parametri in ingresso, mappingName non valorizzato"), LogCategories);
                    return BadRequest("GetMessageRecipients -> Errore validazione parametri in ingresso, mappingName non valorizzato");
                }

                WorkflowRepository repository = _unitOfWork.Repository<WorkflowRepository>().GetByInstanceId(workflowInstanceId);
                if (repository == null)
                {
                    _logger.WriteError(new LogMessage("Nessun repository trovato"), LogCategories);
                    return BadRequest("Nessun repository trovato");
                }
                IEnumerable<WorkflowRoleMapping> workflowRoleMappings;
                if (string.IsNullOrEmpty(internalActivityId))
                {
                    workflowRoleMappings = _unitOfWork.Repository<WorkflowRoleMapping>().GetByMappingTag(mappingName, repository.UniqueId);
                }
                else
                {
                    workflowRoleMappings = _unitOfWork.Repository<WorkflowRoleMapping>().GetByMappingTag(mappingName, repository.UniqueId, internalActivityId);
                }

                ICollection<WorkflowAuthorization> workflowAuthorizations = _workflowAuthorizationService.GetAuthorizationsByMappings(workflowRoleMappings);
                foreach (WorkflowAuthorization workflowAuthorization in workflowAuthorizations)
                {
                    DomainUserModel user = _security.GetUser(workflowAuthorization.Account);
                    MessageContactModel messageContact = _domainUserModelMessageContactModelMapper.Map(user, new MessageContactModel());
                    messageContact.MessageContactEmail.First().User = workflowAuthorization.Account;
                    recipients.Add(messageContact);
                }
                return Ok(recipients);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
