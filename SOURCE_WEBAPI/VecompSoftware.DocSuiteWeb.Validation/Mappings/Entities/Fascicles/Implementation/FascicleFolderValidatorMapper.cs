using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles
{
    public class FascicleFolderValidatorMapper : BaseMapper<FascicleFolder, FascicleFolderValidator>, IFascicleFolderValidatorMapper
    {
        public FascicleFolderValidatorMapper() { }

        public override FascicleFolderValidator Map(FascicleFolder entity, FascicleFolderValidator entityTransformed)
        {
            #region [Base]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.Typology = entity.Typology;
            entityTransformed.FascicleFolderLevel = entity.FascicleFolderLevel;
            entityTransformed.FascicleFolderPath = entity.FascicleFolderPath;
            entityTransformed.ParentInsertId = entity.ParentInsertId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Category = entity.Category;
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.FascicleDocumentUnits = entity.FascicleDocumentUnits;
            entityTransformed.FascicleDocuments = entity.FascicleDocuments;
            #endregion

            return entityTransformed;
        }
    }
}
