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
    public class WorkflowInstanceRoleService : BaseService<WorkflowInstanceRole>, IWorkflowInstanceRoleService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public WorkflowInstanceRoleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        #endregion

        #region [ Methods ]
        protected override WorkflowInstanceRole BeforeCreate(WorkflowInstanceRole entity)
        {
            if (entity.WorkflowInstance != null)
            {
                entity.WorkflowInstance = _unitOfWork.Repository<WorkflowInstance>().Find(entity.WorkflowInstance.UniqueId);
            }

            if (entity.Role != null)
            {
                entity.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }

            return base.BeforeCreate(entity);
        }

        #endregion
    }
}
