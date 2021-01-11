using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public class WorkflowInstanceService : BaseService<WorkflowInstance>, IWorkflowInstanceService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public WorkflowInstanceService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        #endregion

        #region [ Methods ]
        protected override WorkflowInstance BeforeCreate(WorkflowInstance entity)
        {
            if (entity.WorkflowRepository != null)
            {
                entity.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }
            if (entity.WorkflowActivities != null && entity.WorkflowActivities.Count > 0)
            {
                foreach (WorkflowActivity item in entity.WorkflowActivities)
                {
                    item.WorkflowInstance = entity;
                }
                _unitOfWork.Repository<WorkflowActivity>().InsertRange(entity.WorkflowActivities);
            }
            if (entity.WorkflowProperties != null && entity.WorkflowProperties.Count > 0)
            {
                foreach (WorkflowProperty item in entity.WorkflowProperties)
                {
                    item.WorkflowInstance = entity;
                }
                _unitOfWork.Repository<WorkflowProperty>().InsertRange(entity.WorkflowProperties);
            }

            if (entity.WorkflowInstanceRoles != null && entity.WorkflowInstanceRoles.Count > 0)
            {
                foreach (WorkflowInstanceRole item in entity.WorkflowInstanceRoles)
                {
                    item.WorkflowInstance = entity;
                }
                _unitOfWork.Repository<WorkflowInstanceRole>().InsertRange(entity.WorkflowInstanceRoles);
            }

            return base.BeforeCreate(entity);
        }

        protected override WorkflowInstance BeforeUpdate(WorkflowInstance entity, WorkflowInstance entityTransformed)
        {
            if (entity.WorkflowProperties != null)
            {
                foreach (WorkflowProperty item in entityTransformed.WorkflowProperties.Where(f => !entity.WorkflowProperties.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<WorkflowProperty>().Delete(item);
                }
                foreach (WorkflowProperty item in entity.WorkflowProperties.Where(f => !entityTransformed.WorkflowProperties.Any(c => c.UniqueId == f.UniqueId)))
                {
                    item.WorkflowInstance = entityTransformed;
                    _unitOfWork.Repository<WorkflowProperty>().Insert(item);
                }
            }

            if (entity.WorkflowInstanceRoles != null)
            {
                foreach (WorkflowInstanceRole item in entityTransformed.WorkflowInstanceRoles.Where(f => !entity.WorkflowInstanceRoles.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<WorkflowInstanceRole>().Delete(item);
                }
                foreach (WorkflowInstanceRole item in entity.WorkflowInstanceRoles.Where(f => !entityTransformed.WorkflowInstanceRoles.Any(c => c.UniqueId == f.UniqueId)))
                {
                    item.WorkflowInstance = entityTransformed;
                    _unitOfWork.Repository<WorkflowInstanceRole>().Insert(item);
                }
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<WorkflowInstance> SetEntityIncludeOnUpdate(IQueryFluent<WorkflowInstance> query)
        {
            return query
                .Include(f => f.WorkflowProperties)
                .Include(r => r.WorkflowInstanceRoles);
        }

        #endregion
    }
}
