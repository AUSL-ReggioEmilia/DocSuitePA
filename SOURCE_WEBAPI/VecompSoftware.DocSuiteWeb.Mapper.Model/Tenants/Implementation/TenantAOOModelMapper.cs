using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants
{
    public class TenantAOOModelMapper : BaseModelMapper<TenantAOO, TenantAOOModel>, ITenantAOOModelMapper
    {
        public TenantAOOModelMapper()
        {
            
        }

        #region [ Methods ]
        public override TenantAOOModel Map(TenantAOO entity, TenantAOOModel modelTransformed)
        {
            modelTransformed.IdTenantAOO = entity.UniqueId;
            modelTransformed.Name = entity.Name;
            modelTransformed.Note = entity.Note;

            return modelTransformed;
        }
        #endregion
    }
}
