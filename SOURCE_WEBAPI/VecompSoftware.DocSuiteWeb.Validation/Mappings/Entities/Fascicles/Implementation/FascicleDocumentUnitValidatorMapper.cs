using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles
{
    public class FascicleDocumentUnitValidatorMapper : BaseMapper<FascicleDocumentUnit, FascicleDocumentUnitValidator>, IFascicleDocumentUnitValidatorMapper
    {
        public override FascicleDocumentUnitValidator Map(FascicleDocumentUnit entity, FascicleDocumentUnitValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.ReferenceType = entity.ReferenceType;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.FascicleFolder = entity.FascicleFolder;
            entityTransformed.DocumentUnit = entity.DocumentUnit;
            #endregion
            return entityTransformed;
        }
    }
}
