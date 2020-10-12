using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants
{
    public class TenantAOOTableValuedModelMapper : BaseModelMapper<ITenantAOOTableValuedModel, TenantAOOModel>, ITenantAOOTableValuedModelMapper
    {
        public TenantAOOTableValuedModelMapper()
        {

        }

        #region [ Methods ]
        public override TenantAOOModel Map(ITenantAOOTableValuedModel entity, TenantAOOModel modelTransformed)
        {
            modelTransformed = null;
            if (entity.TenantAOO_IdTenantAOO.HasValue)
            {
                modelTransformed = new TenantAOOModel()
                {
                    IdTenantAOO = entity.TenantAOO_IdTenantAOO.Value,
                    Name = entity.TenantAOO_Name
                };
            }

            return modelTransformed;
        }
        #endregion
    }
}
