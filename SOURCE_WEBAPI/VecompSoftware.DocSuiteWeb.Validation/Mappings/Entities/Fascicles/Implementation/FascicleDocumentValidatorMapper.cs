using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles
{
    public class FascicleDocumentValidatorMapper : BaseMapper<FascicleDocument, FascicleDocumentValidator>, IFascicleDocumentValidatorMapper
    {
        public FascicleDocumentValidatorMapper() { }

        public override FascicleDocumentValidator Map(FascicleDocument entity, FascicleDocumentValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.ChainType = entity.ChainType;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.FascicleFolder = entity.FascicleFolder;
            #endregion

            return entityTransformed;
        }

    }
}
