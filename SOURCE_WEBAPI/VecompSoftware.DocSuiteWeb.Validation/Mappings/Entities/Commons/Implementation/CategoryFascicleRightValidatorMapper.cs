using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class CategoryFascicleRightValidatorMapper : BaseMapper<CategoryFascicleRight, CategoryFascicleRightValidator>, ICategoryFascicleRightValidatorMapper
    {
        public CategoryFascicleRightValidatorMapper() { }

        public override CategoryFascicleRightValidator Map(CategoryFascicleRight entity, CategoryFascicleRightValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.CategoryFascicle = entity.CategoryFascicle;
            entityTransformed.Role = entity.Role;
            entityTransformed.Container = entity.Container;
            #endregion

            return entityTransformed;
        }

    }
}