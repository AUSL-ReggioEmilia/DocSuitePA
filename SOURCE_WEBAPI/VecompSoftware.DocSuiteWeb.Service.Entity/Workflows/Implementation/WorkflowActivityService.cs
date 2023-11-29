using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Workflows
{
    public class WorkflowActivityService : BaseService<WorkflowActivity>, IWorkflowActivityService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]

        public WorkflowActivityService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IWorkflowRuleset workflowRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, workflowRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        #endregion

        #region [ Methods ]

        protected override WorkflowActivity BeforeCreate(WorkflowActivity entity)
        {
            if (entity.WorkflowInstance != null)
            {
                entity.WorkflowInstance = _unitOfWork.Repository<WorkflowInstance>().Find(entity.WorkflowInstance.UniqueId);
            }

            if (entity.Tenant != null)
            {
                entity.Tenant = _unitOfWork.Repository<Tenant>().Find(entity.Tenant.UniqueId);
            }

            if (entity.WorkflowActivityLogs != null && entity.WorkflowActivityLogs.Count > 0)
            {
                foreach (WorkflowActivityLog item in entity.WorkflowActivityLogs)
                {
                    item.Entity = entity;
                }
                _unitOfWork.Repository<WorkflowActivityLog>().InsertRange(entity.WorkflowActivityLogs);
            }

            if (entity.WorkflowProperties != null && entity.WorkflowProperties.Count > 0)
            {
                foreach (WorkflowProperty item in entity.WorkflowProperties)
                {
                    item.WorkflowActivity = entity;
                }
                _unitOfWork.Repository<WorkflowProperty>().InsertRange(entity.WorkflowProperties);
            }

            if (entity.WorkflowAuthorizations != null && entity.WorkflowAuthorizations.Count > 0)
            {
                foreach (WorkflowAuthorization item in entity.WorkflowAuthorizations)
                {
                    item.WorkflowActivity = entity;
                }
                _unitOfWork.Repository<WorkflowAuthorization>().InsertRange(entity.WorkflowAuthorizations);
            }

            return base.BeforeCreate(entity);
        }
        protected override IQueryFluent<WorkflowActivity> SetEntityIncludeOnUpdate(IQueryFluent<WorkflowActivity> query)
        {
            query.Include(wa => wa.WorkflowInstance);
            return query;
        }


        protected override WorkflowActivity BeforeUpdate(WorkflowActivity entity, WorkflowActivity entityTransformed)
        {
            if (entity.WorkflowAuthorizations != null && entity.WorkflowAuthorizations.Count > 0)
            {
                foreach (WorkflowAuthorization item in entityTransformed.WorkflowAuthorizations.Where(f => !entity.WorkflowAuthorizations.Any(t => f.UniqueId == t.UniqueId)))
                {
                    item.WorkflowActivity = entityTransformed;
                    _unitOfWork.Repository<WorkflowAuthorization>().Insert(item);
                }
            }

            if (CurrentUpdateActionType.HasValue)
            {
                switch (CurrentUpdateActionType)
                {
                    case Common.Infrastructures.UpdateActionType.HandlingWorkflow:
                        {
                            entityTransformed.Status = WorkflowStatus.Progress;
                            foreach (WorkflowAuthorization item in entity.WorkflowAuthorizations)
                            {
                                item.IsHandler = false;
                                if (item.Account.Equals(CurrentDomainUser.Account, System.StringComparison.InvariantCultureIgnoreCase))
                                {
                                    item.IsHandler = true;
                                }

                                _unitOfWork.Repository<WorkflowAuthorization>().Update(item);

                                if (item.IsHandler)
                                {
                                    WorkflowInstanceLog workflowInstanceLogs = new WorkflowInstanceLog()
                                    {
                                        LogType = WorkflowInstanceLogType.WFTakeCharge,
                                        LogDescription = string.Concat("Assegnata a '", item.Account, "'", "l'attività di presa in carico '", entityTransformed.Name, "'"),
                                        SystemComputer = System.Environment.MachineName,
                                        Entity = entityTransformed.WorkflowInstance
                                    };
                                    _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLogs);
                                }
                            }

                            break;
                        };
                    case Common.Infrastructures.UpdateActionType.RelaseHandlingWorkflow:
                        {
                            entityTransformed.Status = WorkflowStatus.Todo;
                            foreach (WorkflowAuthorization item in entity.WorkflowAuthorizations)
                            {
                                if (item.IsHandler == true)
                                {
                                    WorkflowInstanceLog workflowInstanceLogs = new WorkflowInstanceLog()
                                    {
                                        LogType = WorkflowInstanceLogType.WFRelease,
                                        LogDescription = string.Concat("Rilasciata l'attività '", entityTransformed.Name, "' in carico a '", item.Account, "'"),
                                        SystemComputer = System.Environment.MachineName,
                                        Entity = entityTransformed.WorkflowInstance
                                    };
                                    _unitOfWork.Repository<WorkflowInstanceLog>().Insert(workflowInstanceLogs);
                                }
                                item.IsHandler = false;
                                _unitOfWork.Repository<WorkflowAuthorization>().Update(item);

                            }

                            break;
                        }
                }
            }

            if (entity.WorkflowProperties != null && entity.WorkflowProperties.Count > 0)
            {
                foreach (WorkflowProperty item in entityTransformed.WorkflowProperties.Where(f => !entity.WorkflowProperties.Any(t => f.UniqueId == t.UniqueId)))
                {
                    item.WorkflowActivity = entityTransformed;
                    _unitOfWork.Repository<WorkflowProperty>().Insert(item);
                }
            }

            if (entity.WorkflowActivityLogs != null && entity.WorkflowActivityLogs.Count > 0)
            {
                foreach (WorkflowActivityLog item in entityTransformed.WorkflowActivityLogs.Where(f => !entity.WorkflowActivityLogs.Any(t => f.UniqueId == t.UniqueId)))
                {
                    item.Entity = entityTransformed;
                    _unitOfWork.Repository<WorkflowActivityLog>().Insert(item);
                }
            }

            if (entity.DocumentUnitReferenced != null)
            {
                entityTransformed.DocumentUnitReferenced = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnitReferenced.UniqueId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion

    }
}
