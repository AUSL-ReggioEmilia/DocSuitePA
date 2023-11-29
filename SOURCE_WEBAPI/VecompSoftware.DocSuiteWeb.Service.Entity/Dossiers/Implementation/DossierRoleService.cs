using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public class DossierRoleService : BaseDossierService<DossierRole>, IDossierRoleService
    {
        #region [ Fileds ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public DossierRoleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, dossierRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override DossierRole BeforeCreate(DossierRole entity)
        {
            if (entity.Dossier != null)
            {
                entity.Dossier = _unitOfWork.Repository<Dossier>().Find(entity.Dossier.UniqueId);
            }

            if (entity.Role != null)
            {
                entity.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity.Dossier, null, DossierLogType.Authorize,
                string.Concat("Aggiunta autorizzazione '", entity.AuthorizationRoleType.ToString(), "' al settore ", entity.Role.Name, " (", entity.Role.EntityShortId, ")"), CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<DossierRole> SetEntityIncludeOnDelete(IQueryFluent<DossierRole> query)
        {
            return query.Include(x => x.Role.TenantAOO);
        }

        protected override DossierRole BeforeUpdate(DossierRole entity, DossierRole entityTransformed)
        {
            if (entity.Role != null)
            {
                entityTransformed.Role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);
            }

            if (entity.Dossier != null)
            {
                entityTransformed.Dossier = _unitOfWork.Repository<Dossier>().Find(entity.Dossier.UniqueId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override DossierRole BeforeDelete(DossierRole entity, DossierRole entityTransformed)
        {
            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity.Dossier, null, DossierLogType.Authorize,
               string.Concat("Rimossa autorizzazione '", entity.AuthorizationRoleType.ToString(), "' al settore ", entity.Role.Name, " (", entity.Role.EntityShortId, ")"), CurrentDomainUser.Account));

            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}
