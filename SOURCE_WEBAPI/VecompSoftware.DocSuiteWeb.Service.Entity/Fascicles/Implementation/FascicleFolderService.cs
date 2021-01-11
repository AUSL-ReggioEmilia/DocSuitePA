using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles
{
    public class FascicleFolderService : BaseService<FascicleFolder>, IFascicleFolderService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public FascicleFolderService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IFascicleRuleset fascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, fascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override FascicleFolder BeforeCreate(FascicleFolder entity)
        {
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(entity.Fascicle, FascicleLogType.FolderInsert, $"Creata nuovo cartella {entity.Name}({entity.UniqueId})", CurrentDomainUser.Account));
            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<FascicleFolder> SetEntityIncludeOnUpdate(IQueryFluent<FascicleFolder> query)
        {
            return query.Include(x => x.Fascicle);
        }

        protected override FascicleFolder BeforeUpdate(FascicleFolder entity, FascicleFolder entityTransformed)
        {
            if (entity.Fascicle != null)
            {
                entityTransformed.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (CurrentUpdateActionType == Common.Infrastructures.UpdateActionType.FascicleMoveToFolder)
            {
                _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(entityTransformed.Fascicle, FascicleLogType.Modify, $"Spostata cartella {entityTransformed.Name}({entityTransformed.UniqueId})", CurrentDomainUser.Account));
            }
            else
            {
                _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(entityTransformed.Fascicle, FascicleLogType.Modify, $"Modificata la cartella {entityTransformed.Name}({entityTransformed.UniqueId}", CurrentDomainUser.Account));
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override FascicleFolder BeforeDelete(FascicleFolder entity, FascicleFolder entityTransformed)
        {
            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}