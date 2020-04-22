using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class CategorySchemaValidatorMapper : BaseMapper<CategorySchema, CategorySchemaValidator>, ICategorySchemaValidatorMapper
    {
        public CategorySchemaValidatorMapper() { }

        public override CategorySchemaValidator Map(CategorySchema entity, CategorySchemaValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Version = entity.Version;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Categories = entity.Categories;

            #endregion

            return entityTransformed;
        }

    }
}
