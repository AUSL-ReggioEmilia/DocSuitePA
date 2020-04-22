using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class CategoryFullTableValuedModelMapper : BaseModelMapper<CategoryFullTableValuedModel, CategoryModel>, ICategoryFullTableValuedModelMapper
    {
        public override CategoryModel Map(CategoryFullTableValuedModel entity, CategoryModel entityTransformed)
        {
            entityTransformed.IdCategory = entity.IdCategory;
            entityTransformed.Name = entity.Name;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.FullCode = entity.FullCode;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.IdParent = entity.CategoryParent_IdCategory;
            entityTransformed.HasChildren = entity.HasChildren;
            entityTransformed.HasFascicleDefinition = entity.HasFascicleDefinition;
            entityTransformed.Code = entity.Code;
            entityTransformed.CategoryType = CategoryModelType.Category;

            return entityTransformed;
        }

    }
}
