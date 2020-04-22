using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Processes;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Processes
{
    public class ProcessFascicleTemplateService : BaseService<ProcessFascicleTemplate>, IProcessFascicleTemplateService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]

        public ProcessFascicleTemplateService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService, 
            IProcessRuleset processRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security) 
            : base(unitOfWork, logger, validationService, processRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override ProcessFascicleTemplate BeforeCreate(ProcessFascicleTemplate entity)
        {
            entity.StartDate = DateTimeOffset.UtcNow;
            entity.EndDate = null;
            if (entity.DossierFolder != null)
            {
                entity.DossierFolder = _unitOfWork.Repository<DossierFolder>().Find(entity.DossierFolder.UniqueId);
            }

            if (entity.DossierFolder != null)
            {
                entity.DossierFolder.Status = DossierFolderStatus.Folder;
                _unitOfWork.Repository<DossierFolder>().Update(entity.DossierFolder);
            }

            if (entity.Process != null)
            {
                entity.Process = _unitOfWork.Repository<Process>().Find(entity.Process.UniqueId);
            }

            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, $"Inserimento tipologia atto {entity.Name}", typeof(ProcessFascicleTemplate).Name, CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override ProcessFascicleTemplate BeforeUpdate(ProcessFascicleTemplate entity, ProcessFascicleTemplate entityTransformed)
        {
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.UPDATE, $"Modificata tipologia atto {entity.Name} con ID {entity.UniqueId}", typeof(ProcessFascicleTemplate).Name, CurrentDomainUser.Account));

            entityTransformed.JsonModel = entity.JsonModel;
            if (entity.Process != null)
            {
                entityTransformed.Process = _unitOfWork.Repository<Process>().Find(entity.Process.UniqueId);
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion
    }
}
