using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSPECMailValidatorMapper : BaseMapper<UDSPECMail, UDSPECMailValidator>, IUDSPECMailValidatorMapper
    {
        public UDSPECMailValidatorMapper() { }

        public override UDSPECMailValidator Map(UDSPECMail entity, UDSPECMailValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.RelationType = entity.RelationType;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Repository = entity.Repository;
            entityTransformed.PECMail = entity.Relation;
            #endregion

            return entityTransformed;
        }
    }
}
