using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Processes
{
    public class ProcessFascicleWorkflowRepositoryService : BaseService<ProcessFascicleWorkflowRepository>, IProcessFascicleWorkflowRepositoryService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]

        public ProcessFascicleWorkflowRepositoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IProcessRuleset processRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, processRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override ProcessFascicleWorkflowRepository BeforeCreate(ProcessFascicleWorkflowRepository entity)
        {
            if (entity.DossierFolder != null)
            {
                entity.DossierFolder = _unitOfWork.Repository<DossierFolder>().Find(entity.DossierFolder.UniqueId);
            }

            if (entity.Process != null)
            {
                entity.Process = _unitOfWork.Repository<Process>().Find(entity.Process.UniqueId);
            }

            if (entity.WorkflowRepository != null)
            {
                entity.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, $"Inserimento di entità '{typeof(ProcessFascicleWorkflowRepository).Name}' con ID {entity.UniqueId}", typeof(ProcessFascicleWorkflowRepository).Name, CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }
        
        protected override ProcessFascicleWorkflowRepository BeforeUpdate(ProcessFascicleWorkflowRepository entity, ProcessFascicleWorkflowRepository entityTransformed)
        {
            if (entity.WorkflowRepository != null)
            {
                entityTransformed.WorkflowRepository = _unitOfWork.Repository<WorkflowRepository>().Find(entity.WorkflowRepository.UniqueId);
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.UPDATE, $"Modificata di entità '{typeof(ProcessFascicleWorkflowRepository).Name}' con ID {entity.UniqueId}", typeof(ProcessFascicleWorkflowRepository).Name, CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion

    }
}
