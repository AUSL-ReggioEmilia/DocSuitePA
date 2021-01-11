using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSDocumentUnitValidatorMapper : BaseMapper<UDSDocumentUnit, UDSDocumentUnitValidator>, IUDSDocumentUnitValidatorMapper
    {

        #region [ Constructor ]
        public UDSDocumentUnitValidatorMapper() { }
        #endregion

        #region [ Methods ]
        public override UDSDocumentUnitValidator Map(UDSDocumentUnit entity, UDSDocumentUnitValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Repository = entity.Repository;
            entityTransformed.DocumentUnit = entity.Relation;
            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
