using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class CategoryFascicleValidatorMapper : BaseMapper<CategoryFascicle, CategoryFascicleValidator>, ICategoryFascicleValidatorMapper
    {
        public CategoryFascicleValidatorMapper() { }

        public override CategoryFascicleValidator Map(CategoryFascicle entity, CategoryFascicleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            entityTransformed.FascicleType = entity.FascicleType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Manager = entity.Manager;
            entityTransformed.Category = entity.Category;
            entityTransformed.FasciclePeriod = entity.FasciclePeriod;
            #endregion

            return entityTransformed;
        }

    }
}
