using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants
{
    public class TenantConfigurationModelMapper : BaseModelMapper<TenantConfiguration, TenantConfigurationModel>, ITenantConfigurationModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public TenantConfigurationModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]
        public override TenantConfigurationModel Map(TenantConfiguration entity, TenantConfigurationModel modelTransformed)
        {
            modelTransformed.JsonValue = entity.JsonValue;
            modelTransformed.StartDate = entity.StartDate;
            modelTransformed.EndDate = entity.EndDate;
            modelTransformed.Note = entity.Note;

            return modelTransformed;
        }
        #endregion
    }
}
