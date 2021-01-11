using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class CategoryMapper : BaseEntityMapper<Category, Category>, ICategoryMapper
    {
        public CategoryMapper()
        {

        }

        public override Category Map(Category entity, Category entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Code = entity.Code;
            entityTransformed.FullCode = entity.FullCode;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            #endregion

            return entityTransformed;
        }

    }
}
