using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class CategorySchemaMapper : BaseEntityMapper<CategorySchema, CategorySchema>, ICategorySchemaMapper
    {
        public CategorySchemaMapper()
        {

        }

        public override CategorySchema Map(CategorySchema entity, CategorySchema entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Version = entity.Version;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            #endregion

            return entityTransformed;
        }

    }
}
