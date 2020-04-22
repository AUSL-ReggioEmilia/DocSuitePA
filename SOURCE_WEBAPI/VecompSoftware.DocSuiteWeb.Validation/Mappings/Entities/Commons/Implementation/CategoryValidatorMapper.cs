using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class CategoryValidatorMapper : BaseMapper<Category, CategoryValidator>, ICategoryValidatorMapper
    {
        public CategoryValidatorMapper() { }

        public override CategoryValidator Map(Category entity, CategoryValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Code = entity.Code;
            entityTransformed.FullCode = entity.FullCode;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.FullSearchComputed = entity.FullSearchComputed;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Parent = entity.Parent;
            entityTransformed.Categories = entity.Categories;
            entityTransformed.Protocols = entity.Protocols;
            entityTransformed.DocumentSeriesItems = entity.DocumentSeriesItems;
            entityTransformed.Fascicles = entity.Fascicles;
            entityTransformed.CategoryFascicles = entity.CategoryFascicles;
            entityTransformed.DocumentUnits = entity.DocumentUnits;
            entityTransformed.DocumentUnitFascicleHistoricizedCategories = entity.DocumentUnitFascicleHistoricizedCategories;
            entityTransformed.DocumentUnitFascicleCategories = entity.DocumentUnitFascicleCategories;
            entityTransformed.DossierFolders = entity.DossierFolders;
            entityTransformed.CategorySchema = entity.CategorySchema;
            entityTransformed.MetadataRepository = entity.MetadataRepository;
            entityTransformed.FascicleFolders = entity.FascicleFolders;
            entityTransformed.Processes = entity.Processes;
            #endregion

            return entityTransformed;
        }

    }
}
