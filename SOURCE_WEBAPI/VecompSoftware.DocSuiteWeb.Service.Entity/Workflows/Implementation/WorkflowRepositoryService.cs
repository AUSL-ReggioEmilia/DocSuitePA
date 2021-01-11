using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public class WorkflowRepositoryService : BaseService<WorkflowRepository>, IWorkflowRepositoryService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public WorkflowRepositoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override WorkflowRepository BeforeCreate(WorkflowRepository entity)
        {
            if (entity.WorkflowInstances != null && entity.WorkflowInstances.Count > 0)
            {
                foreach (WorkflowInstance item in entity.WorkflowInstances)
                {
                    item.WorkflowRepository = entity;
                }
                _unitOfWork.Repository<WorkflowInstance>().InsertRange(entity.WorkflowInstances);
            }

            if (entity.WorkflowEvaluationProperties != null && entity.WorkflowEvaluationProperties.Count > 0)
            {
                foreach (WorkflowEvaluationProperty item in entity.WorkflowEvaluationProperties)
                {
                    item.WorkflowRepository = entity;
                }
                _unitOfWork.Repository<WorkflowEvaluationProperty>().InsertRange(entity.WorkflowEvaluationProperties);
            }

            if (entity.Roles != null && entity.Roles.Count > 0)
            {
                foreach (Role item in entity.Roles)
                {
                    item.WorkflowRepositories.Add(entity);
                }
                _unitOfWork.Repository<Role>().InsertRange(entity.Roles);
            }

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<WorkflowRepository> SetEntityIncludeOnUpdate(IQueryFluent<WorkflowRepository> query)
        {
            query.Include(x => x.Roles);
            return query;
        }

        protected override WorkflowRepository BeforeUpdate(WorkflowRepository entity, WorkflowRepository entityTransformed)
        {
            if (entity.WorkflowInstances != null && entity.WorkflowInstances.Count > 0)
            {
                foreach (WorkflowInstance item in entityTransformed.WorkflowInstances.Where(f => !entity.WorkflowInstances.Any(t => f.UniqueId == t.UniqueId)))
                {
                    item.WorkflowRepository = entityTransformed;
                    _unitOfWork.Repository<WorkflowInstance>().Insert(item);
                }

            }
            if (entity.WorkflowRoleMappings != null && entity.WorkflowRoleMappings.Count > 0)
            {
                foreach (WorkflowRoleMapping item in entityTransformed.WorkflowRoleMappings.Where(f => !entity.WorkflowRoleMappings.Any(t => f.UniqueId == t.UniqueId)))
                {
                    item.WorkflowRepository = entityTransformed;
                    _unitOfWork.Repository<WorkflowRoleMapping>().Insert(item);
                }
            }

            if (entity.WorkflowEvaluationProperties != null && entity.WorkflowEvaluationProperties.Count > 0)
            {
                foreach (WorkflowEvaluationProperty item in entityTransformed.WorkflowEvaluationProperties.Where(f => !entity.WorkflowEvaluationProperties.Any(t => f.UniqueId == t.UniqueId)))
                {
                    item.WorkflowRepository = entityTransformed;
                    _unitOfWork.Repository<WorkflowEvaluationProperty>().Insert(item);
                }
            }

            if (entity.Roles != null && entity.Roles.Count > 0)
            {
                foreach (Role item in entity.Roles.Where(f => !entityTransformed.Roles.Any(t => f.UniqueId == t.UniqueId)))
                {
                    entityTransformed.Roles.Add(_unitOfWork.Repository<Role>().Find(item.EntityShortId));
                }
            }

            List<Role> rolesToDelete = entityTransformed.Roles.Where(f => !entity.Roles.Any(c => c.UniqueId == f.UniqueId)).ToList();
            if(rolesToDelete.Count > 0)
            {
                foreach (Role item in rolesToDelete)
                {
                    entityTransformed.Roles.Remove(item);
                }
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion

    }
}
