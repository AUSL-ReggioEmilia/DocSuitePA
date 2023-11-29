using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class RoleService : BaseService<Role>, IRoleService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public RoleService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IRoleRuleset roleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, roleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override Role BeforeCreate(Role entity)
        {
            if (entity.TenantAOO != null)
            {
                entity.TenantAOO = _unitOfWork.Repository<TenantAOO>().Find(entity.TenantAOO.UniqueId);
            }

            if (entity.Father != null)
            {
                entity.Father = _unitOfWork.Repository<Role>().Find(entity.Father.EntityShortId);
            }

            return base.BeforeCreate(entity);
        }

        protected override bool ExecuteDelete()
        {
            return false;
        }

        protected override IQueryFluent<Role> SetEntityIncludeOnDelete(IQueryFluent<Role> query)
        {
            query = query.Include(p => p.TenantAOO);
            return query;
        }

        protected override Role BeforeDelete(Role entity, Role entityTransformed)
        {
            entityTransformed.IsActive = false;
            _unitOfWork.Repository<Role>().Update(entityTransformed);

            return entityTransformed;
        }
        #endregion

    }
}