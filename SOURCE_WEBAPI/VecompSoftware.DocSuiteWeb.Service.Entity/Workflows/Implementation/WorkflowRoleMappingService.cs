using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public class WorkflowRoleMappingService : BaseService<WorkflowRoleMapping>, IWorkflowRoleMappingService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public WorkflowRoleMappingService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override WorkflowRoleMapping BeforeCreate(WorkflowRoleMapping entity)
        {
            if (entity.WorkflowRepository != null)
            {
                entity.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }

            if (entity.Role != null)
            {
                entity.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }
            return base.BeforeCreate(entity);
        }

        protected override WorkflowRoleMapping BeforeUpdate(WorkflowRoleMapping entity, WorkflowRoleMapping entityTransformed)
        {
            if (entity.Role != null)
            {
                entityTransformed.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
