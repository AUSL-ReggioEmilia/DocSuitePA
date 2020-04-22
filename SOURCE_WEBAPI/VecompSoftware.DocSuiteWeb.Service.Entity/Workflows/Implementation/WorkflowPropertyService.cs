using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public class WorkflowPropertyService : BaseService<WorkflowProperty>, IWorkflowPropertyService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public WorkflowPropertyService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override WorkflowProperty BeforeCreate(WorkflowProperty entity)
        {
            if (entity.WorkflowActivity != null)
            {
                entity.WorkflowActivity = _unitOfWork.Repository<WorkflowActivity>().Find(entity.WorkflowActivity.UniqueId);
            }

            if (entity.WorkflowInstance != null)
            {
                entity.WorkflowInstance = _unitOfWork.Repository<WorkflowInstance>().Find(entity.WorkflowInstance.UniqueId);
            }
            return base.BeforeCreate(entity);
        }

        #endregion

    }
}
