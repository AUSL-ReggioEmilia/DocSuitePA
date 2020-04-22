using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class CategoryFascicleMapper : BaseEntityMapper<CategoryFascicle, CategoryFascicle>, ICategoryFascicleMapper
    {
        public CategoryFascicleMapper()
        {

        }

        public override CategoryFascicle Map(CategoryFascicle entity, CategoryFascicle entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            entityTransformed.FascicleType = entity.FascicleType;
            #endregion

            return entityTransformed;
        }

    }
}
