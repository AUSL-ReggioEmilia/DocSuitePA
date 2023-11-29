using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public class WorkflowAuthorizationService : BaseService<WorkflowAuthorization>, IWorkflowAuthorizationService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IRoleUserWorkflowAuthorizationMapper _mapperRoleUserWorkflowAuthorization;
        private readonly ISecurityUserWorkflowAuthorizationMapper _securityUserWorkflowAuthorizationMapper;
        #endregion

        #region [ Constructor ]
        public WorkflowAuthorizationService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, IRoleUserWorkflowAuthorizationMapper mapperRoleUserWorkflowAuthorization, ISecurityUserWorkflowAuthorizationMapper securityUserWorkflowAuthorizationMapper, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperRoleUserWorkflowAuthorization = mapperRoleUserWorkflowAuthorization;
            _securityUserWorkflowAuthorizationMapper = securityUserWorkflowAuthorizationMapper;
        }
        #endregion

        #region [ Methods ]
        protected override WorkflowAuthorization BeforeCreate(WorkflowAuthorization entity)
        {
            if (entity.WorkflowActivity != null)
            {
                entity.WorkflowActivity = _unitOfWork.Repository<WorkflowActivity>().Find(entity.WorkflowActivity.UniqueId);
            }

            return base.BeforeCreate(entity);
        }

        public ICollection<WorkflowAuthorization> GetAuthorizationsByMappings(IEnumerable<WorkflowRoleMapping> workflowMappings)
        {
            List<WorkflowAuthorization> authorizations = new List<WorkflowAuthorization>();
            foreach (WorkflowRoleMapping workflowMapping in workflowMappings)
            {
                switch (workflowMapping.AuthorizationType)
                {
                    case WorkflowAuthorizationType.AllProtocolSecurityUsers:
                        {
                            _logger.WriteDebug(new LogMessage("WorkflowAuthorizationType.AllProtocolSecurityUsers"), LogCategories);
                            IEnumerable<SecurityUser> securityUsers = _unitOfWork.Repository<RoleGroup>().GetProtocolAuthorizedRoleSecurityUsers(workflowMapping.Role.UniqueId);
                            authorizations.AddRange(_securityUserWorkflowAuthorizationMapper.MapCollection(securityUsers));
                            break;
                        }
                    case WorkflowAuthorizationType.AllRoleUser:
                        {
                            _logger.WriteDebug(new LogMessage("WorkflowAuthorizationType.AllRoleUser"), LogCategories);
                            IEnumerable<SecurityUser> securityUsers = _unitOfWork.Repository<RoleGroup>().GetRoleGroupsAllAuthorizationType(workflowMapping.Role.UniqueId);
                            authorizations.AddRange(_securityUserWorkflowAuthorizationMapper.MapCollection(securityUsers));
                            break;
                        }
                    case WorkflowAuthorizationType.AllSecretary:
                        {
                            _logger.WriteDebug(new LogMessage("WorkflowAuthorizationType.AllSecretary"), LogCategories);
                            IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Secretary, workflowMapping.Role.UniqueId, DSWEnvironmentType.Protocol);
                            authorizations.AddRange(_mapperRoleUserWorkflowAuthorization.MapCollection(roleUsers));
                            break;
                        }
                    case WorkflowAuthorizationType.AllSigner:
                        {
                            _logger.WriteDebug(new LogMessage("WorkflowAuthorizationType.AllSigner"), LogCategories);
                            IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(new List<string>() { RoleUserType.Vice, RoleUserType.Manager },
                                workflowMapping.Role.UniqueId, DSWEnvironmentType.Protocol);
                            authorizations.AddRange(_mapperRoleUserWorkflowAuthorization.MapCollection(roleUsers));
                            break;
                        }
                    case WorkflowAuthorizationType.AllManager:
                        {
                            _logger.WriteDebug(new LogMessage("WorkflowAuthorizationType.AllManager"), LogCategories);
                            IEnumerable<RoleUser> roleUsers = _unitOfWork.Repository<RoleUser>().GetByAuthorizationType(RoleUserType.Manager, workflowMapping.Role.UniqueId, DSWEnvironmentType.Protocol);
                            authorizations.AddRange(_mapperRoleUserWorkflowAuthorization.MapCollection(roleUsers));
                            break;
                        }
                    case WorkflowAuthorizationType.UserName:
                        {
                            _logger.WriteDebug(new LogMessage("WorkflowAuthorizationType.UserName"), LogCategories);
                            authorizations.Add(new WorkflowAuthorization() { Account = workflowMapping.AccountName });
                            break;
                        }
                    case WorkflowAuthorizationType.AllOChartRoleUser:
                        throw new DSWException("Workflow Authorization AllOChartRoleUser: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    case WorkflowAuthorizationType.AllOChartManager:
                        throw new DSWException("Workflow Authorization AllOChartManager: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    case WorkflowAuthorizationType.AllOChartHierarchyManager:
                        throw new DSWException("Workflow Authorization AllOChartHierarchyManager: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    case WorkflowAuthorizationType.ADGroup:
                        throw new DSWException("Workflow Authorization ADGroup: Not Supported", null, DSWExceptionCode.SS_RulesetValidation);
                    default:
                        throw new DSWException("Workflow Authorization AuthorizationType argument not defined", null, DSWExceptionCode.SS_RulesetValidation);
                }
            }
            return authorizations;
        }
        #endregion
    }
}