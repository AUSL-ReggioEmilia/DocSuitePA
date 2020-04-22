using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Securities;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Workflows
{
    public class WorkflowRoleMappingsController : BaseODataController<WorkflowRoleMapping, IWorkflowRoleMappingService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ISecurity _security;
        private readonly IWorkflowRoleMappingCollaborationUserModelMapper _mapperCollaborationUser;
        private readonly IDomainUserModelCollaborationSignModelMapper _domainSignMapper;
        private readonly IDomainUserModelCollaborationUserModelMapper _domainUserMapper;
        private readonly IWorkflowAuthorizationService _workflowAuthorizationService;
        #endregion

        #region [ Constructor ]

        public WorkflowRoleMappingsController(IWorkflowRoleMappingService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security,
            IWorkflowRoleMappingCollaborationUserModelMapper mapperCollaborationUser, IDomainUserModelCollaborationSignModelMapper domainSignMapper,
            IDomainUserModelCollaborationUserModelMapper domainUserMapper, IWorkflowAuthorizationService workflowAuthorizationService)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _security = security;
            _mapperCollaborationUser = mapperCollaborationUser;
            _domainSignMapper = domainSignMapper;
            _domainUserMapper = domainUserMapper;
            _workflowAuthorizationService = workflowAuthorizationService;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetCollaborationUsers(string mappingName, Guid workflowInstanceId, string internalActivityId)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetCollaborationUsers -> mappingName = ", mappingName, ", workflowInstanceId = ", workflowInstanceId, ", internalActivityId = ", internalActivityId)), LogCategories);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<CollaborationUserModel> users = new List<CollaborationUserModel>();
                if (string.IsNullOrEmpty(mappingName))
                {
                    _logger.WriteError(new LogMessage("GetCollaborationUsers -> Errore validazione parametri in ingresso, mappingName non valorizzato"), LogCategories);
                    return BadRequest("GetCollaborationUsers -> Errore validazione parametri in ingresso, mappingName non valorizzato");
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

                short incremental = 1;
                foreach (WorkflowRoleMapping workflowRoleMapping in workflowRoleMappings)
                {
                    CollaborationUserModel collaborationUser = new CollaborationUserModel();
                    if (workflowRoleMapping.AuthorizationType == WorkflowAuthorizationType.UserName)
                    {
                        DomainUserModel user = _security.GetUser(workflowRoleMapping.AccountName);
                        collaborationUser = _domainUserMapper.Map(user, new CollaborationUserModel());
                        collaborationUser.Account = workflowRoleMapping.AccountName;
                    }
                    else
                    {
                        collaborationUser = _mapperCollaborationUser.Map(workflowRoleMapping, collaborationUser);
                    }
                    collaborationUser.Incremental = incremental++;
                    users.Add(collaborationUser);
                }

                return Ok(users);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCollaborationUsers(int? roleId)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetCollaborationUsers -> roleId = ", roleId)), LogCategories);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<CollaborationUserModel> users = new List<CollaborationUserModel>();
                if (!roleId.HasValue)
                {
                    _logger.WriteError(new LogMessage("GetCollaborationUsers -> Errore validazione parametri in ingresso, roleId non valorizzato"), LogCategories);
                    return BadRequest("GetCollaborationUsers -> Errore validazione parametri in ingresso, roleId non valorizzato");
                }

                Role role = _unitOfWork.Repository<Role>().Find(roleId.Value);
                WorkflowRoleMapping workflowRoleMapping = new WorkflowRoleMapping() { Role = role };
                CollaborationUserModel collaborationUser = _mapperCollaborationUser.Map(workflowRoleMapping, new CollaborationUserModel());
                collaborationUser.Incremental = 1;
                users.Add(collaborationUser);
                return Ok(users);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetCollaborationUsers(string domain, string account)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetCollaborationUsers -> domain = ", domain, ", account = ", account)), LogCategories);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<CollaborationUserModel> users = new List<CollaborationUserModel>();
                if (string.IsNullOrEmpty(account))
                {
                    _logger.WriteError(new LogMessage("GetCollaborationUsers -> Errore validazione parametri in ingresso, account non valorizzato"), LogCategories);
                    return BadRequest("GetCollaborationUsers -> Errore validazione parametri in ingresso, account non valorizzato");
                }

                DomainUserModel user = _security.GetUser(account);
                CollaborationUserModel collaborationUser = _domainUserMapper.Map(user, new CollaborationUserModel());
                collaborationUser.Account = account;
                collaborationUser.Incremental = 1;
                users.Add(collaborationUser);
                return Ok(users);
            }, _logger, LogCategories);
        }

        public IHttpActionResult GetCollaborationSigns(string mappingName, Guid workflowInstanceId, string internalActivityId)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetCollaborationSigns -> mappingName = ", mappingName, ", workflowInstanceId = ", workflowInstanceId, ", internalActivityId = ", internalActivityId)), LogCategories);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<CollaborationSignModel> signers = new List<CollaborationSignModel>();
                if (string.IsNullOrEmpty(mappingName))
                {
                    _logger.WriteError(new LogMessage("GetCollaborationSigns -> Errore validazione parametri in ingresso, mappingName non valorizzato"), LogCategories);
                    return BadRequest("GetCollaborationSigns -> Errore validazione parametri in ingresso, mappingName non valorizzato");
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
                short incremental = 1;
                foreach (WorkflowAuthorization workflowAuthorization in workflowAuthorizations)
                {
                    DomainUserModel user = _security.GetUser(workflowAuthorization.Account);
                    CollaborationSignModel collaborationSign = _domainSignMapper.Map(user, new CollaborationSignModel());
                    collaborationSign.SignUser = workflowAuthorization.Account;
                    collaborationSign.Incremental = incremental++;
                    signers.Add(collaborationSign);
                }
                return Ok(signers);
            }, _logger, LogCategories);
        }

        public IHttpActionResult GetCollaborationSigns(int? roleId)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetCollaborationSigns -> roleId = ", roleId)), LogCategories);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<CollaborationSignModel> signers = new List<CollaborationSignModel>();
                if (!roleId.HasValue)
                {
                    _logger.WriteError(new LogMessage("GetCollaborationSigns -> Errore validazione parametri in ingresso, roleId non valorizzato"), LogCategories);
                    return BadRequest("GetCollaborationSigns -> Errore validazione parametri in ingresso, roleId non valorizzato");
                }

                Role role = _unitOfWork.Repository<Role>().Find(roleId.Value);
                WorkflowRoleMapping workflowRoleMapping = new WorkflowRoleMapping() { AuthorizationType = WorkflowAuthorizationType.AllSigner, Role = role };
                ICollection<WorkflowAuthorization> workflowAuthorizations = _workflowAuthorizationService.GetAuthorizationsByMappings(new List<WorkflowRoleMapping>() { workflowRoleMapping });
                short incremental = 1;
                foreach (WorkflowAuthorization workflowAuthorization in workflowAuthorizations)
                {
                    DomainUserModel user = _security.GetUser(workflowAuthorization.Account);
                    CollaborationSignModel collaborationSign = _domainSignMapper.Map(user, new CollaborationSignModel());
                    collaborationSign.SignUser = workflowAuthorization.Account;
                    collaborationSign.Incremental = incremental++;
                    signers.Add(collaborationSign);
                }
                return Ok(signers);
            }, _logger, LogCategories);
        }

        public IHttpActionResult GetCollaborationSigns(string domain, string account)
        {
            _logger.WriteDebug(new LogMessage(string.Concat("GetCollaborationSigns -> domain = ", domain, ", account = ", account)), LogCategories);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<CollaborationSignModel> signers = new List<CollaborationSignModel>();
                if (string.IsNullOrEmpty(account))
                {
                    _logger.WriteError(new LogMessage("GetCollaborationSigns -> Errore validazione parametri in ingresso, account non valorizzato"), LogCategories);
                    return BadRequest("GetCollaborationSigns -> Errore validazione parametri in ingresso, account non valorizzato");
                }

                DomainUserModel user = _security.GetUser(account);
                CollaborationSignModel collaborationSign = _domainSignMapper.Map(user, new CollaborationSignModel());
                collaborationSign.SignUser = string.Concat(domain, "\\", account);
                collaborationSign.Incremental = 1;
                signers.Add(collaborationSign);
                return Ok(signers);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
