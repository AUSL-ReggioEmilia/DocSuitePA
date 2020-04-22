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
    public class FascicleRoleService : BaseService<FascicleRole>, IFascicleRoleService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public FascicleRoleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IFascicleRuleset fascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, fascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override FascicleRole BeforeCreate(FascicleRole entity)
        {
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.Role != null)
            {
                entity.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }
            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(entity.Fascicle, FascicleLogType.Authorize, $"Aggiunta autorizzazione '{entity.AuthorizationRoleType}' al settore {entity.Role.Name} ({entity.Role.EntityShortId})", CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<FascicleRole> SetEntityIncludeOnUpdate(IQueryFluent<FascicleRole> query)
        {
            return query.Include(x => x.Role)
                .Include(f => f.Fascicle);
        }

        protected override FascicleRole BeforeUpdate(FascicleRole entity, FascicleRole entityTransformed)
        {
            if (entity.Role != null)
            {
                entityTransformed.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }

            if (entity.Fascicle != null)
            {
                entityTransformed.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<FascicleRole> SetEntityIncludeOnDelete(IQueryFluent<FascicleRole> query)
        {
            return query.Include(x => x.Role)
                .Include(f => f.Fascicle);
        }

        protected override FascicleRole BeforeDelete(FascicleRole entity, FascicleRole entityTransformed)
        {
            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(entityTransformed.Fascicle, FascicleLogType.Authorize, $"Rimossa autorizzazione '{entity.AuthorizationRoleType}' al settore {entity.Role.Name} ({entity.Role.EntityShortId})", CurrentDomainUser.Account));
            return base.BeforeDelete(entity, entityTransformed);
        }
    }
    #endregion
}

