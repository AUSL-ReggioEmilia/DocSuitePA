using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Tenants
{
    public class TenantWorkflowRepositoryService : BaseService<TenantWorkflowRepository>, ITenantWorkflowRepositoryService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public TenantWorkflowRepositoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITenantWorkflowRepositoryRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override TenantWorkflowRepository BeforeCreate(TenantWorkflowRepository entity)
        {
            if (entity.WorkflowRepository != null)
            {
                entity.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }

            if (entity.Tenant != null)
            {
                entity.Tenant = _unitOfWork.Repository<Tenant>().Find(entity.Tenant.UniqueId);
            }
            return base.BeforeCreate(entity);
        }

        protected override TenantWorkflowRepository BeforeUpdate(TenantWorkflowRepository entity, TenantWorkflowRepository entityTransformed)
        {
            if (entity.WorkflowRepository != null)
            {
                entityTransformed.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }
            if (entity.Tenant != null)
            {
                entityTransformed.Tenant = _unitOfWork.Repository<Tenant>().Find(entity.Tenant.UniqueId);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
