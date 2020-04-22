using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class CategoryModelMapper : BaseModelMapper<Category, CategoryModel>, ICategoryModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public CategoryModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override CategoryModel Map(Category entity, CategoryModel entityTransformed)
        {
            entityTransformed.FullCode = entity.FullCode;
            entityTransformed.IdCategory = entity.EntityShortId;
            entityTransformed.Name = entity.Name;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.IdParent = entity.Parent?.EntityShortId;
            entityTransformed.CategoryType = CategoryModelType.Category;
            return entityTransformed;
        }

    }
}
