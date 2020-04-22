using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class CategoryTableValuedModelMapper : BaseModelMapper<ICategoryTableValuedModel, CategoryModel>, ICategoryTableValuedModelMapper
    {
        public override CategoryModel Map(ICategoryTableValuedModel entity, CategoryModel entityTransformed)
        {
            entityTransformed = null;
            if (entity.Category_IdCategory.HasValue)
            {
                entityTransformed = new CategoryModel
                {
                    IdCategory = entity.Category_IdCategory,
                    Name = entity.Category_Name
                };
            }

            return entityTransformed;
        }

    }
}
