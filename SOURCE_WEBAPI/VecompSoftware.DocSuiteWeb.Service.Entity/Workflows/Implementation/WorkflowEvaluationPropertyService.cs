using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public class WorkflowEvaluationPropertyService : BaseService<WorkflowEvaluationProperty>, IWorkflowEvaluationPropertyService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public WorkflowEvaluationPropertyService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override WorkflowEvaluationProperty BeforeCreate(WorkflowEvaluationProperty entity)
        {
            if (entity.WorkflowRepository != null)
            {
                entity.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }

            return base.BeforeCreate(entity);
        }

        protected override WorkflowEvaluationProperty BeforeUpdate(WorkflowEvaluationProperty entity, WorkflowEvaluationProperty entityTransformed)
        {
            if (entity.WorkflowRepository != null)
            {
                entityTransformed.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
