using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Tenants
{
    public class TenantConfigurationService : BaseService<TenantConfiguration>, ITenantConfigurationService
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public TenantConfigurationService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITenantConfigurationRuleset tenantConfigurationRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, tenantConfigurationRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override TenantConfiguration BeforeCreate(TenantConfiguration entity)
        {
            if (entity.Tenant != null)
            {
                entity.Tenant = _unitOfWork.Repository<Tenant>().Find(entity.Tenant.UniqueId);
            }
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entity.UniqueId, null, TableLogEvent.INSERT, string.Concat("Inserimento configurazione azienda ", entity.Tenant != null ? entity.Tenant.TenantName : ""), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        protected override TenantConfiguration BeforeUpdate(TenantConfiguration entity, TenantConfiguration entityTransformed)
        {
            if (entity.Tenant != null)
            {
                entityTransformed.Tenant = _unitOfWork.Repository<Tenant>().Find(entity.Tenant.UniqueId);
            }
            _unitOfWork.Repository<TableLog>().Insert(TableLogService.CreateLog(entityTransformed.UniqueId, null, TableLogEvent.UPDATE, string.Concat("Aggiornamento configurazione azienda ", entity.Tenant != null ? entity.Tenant.TenantName : ""), typeof(TenantConfiguration).Name, CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion

    }
}
