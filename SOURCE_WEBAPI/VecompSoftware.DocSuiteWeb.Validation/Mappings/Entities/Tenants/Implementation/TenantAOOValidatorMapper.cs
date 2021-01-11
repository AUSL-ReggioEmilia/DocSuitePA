using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants
{
    public class TenantAOOValidatorMapper : BaseMapper<TenantAOO, TenantAOOValidator>, ITenantAOOValidatorMapper
    {
        public TenantAOOValidatorMapper(){}

        public override TenantAOOValidator Map(TenantAOO entity, TenantAOOValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.Note= entity.Note;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.CategorySuffix = entity.CategorySuffix;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Tenants = entity.Tenants;
            entityTransformed.Categories = entity.Categories;
            entityTransformed.TenantTypologyType = entity.TenantTypology;
            #endregion

            return entityTransformed;
        }
    }
}
