using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperCategoryModel : BaseEntityMapper<Category, CategoryModel>
    {
        protected override IQueryOver<Category, Category> MappingProjection(IQueryOver<Category, Category> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override CategoryModel TransformDTO(Category entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Category se l'entità non è inizializzata");

            CategoryModel model = new CategoryModel(entity.Id)
            {
                FullCode = entity.FullCode,
                Name = entity.Name,
                UniqueId = entity.UniqueId
            };

            return model;
        }
    }
}
