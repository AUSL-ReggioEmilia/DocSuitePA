using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;


namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSContactValidatorMapper : BaseMapper<UDSContact, UDSContactValidator>, IUDSContactValidatorMapper
    {
        #region [ Constructor ]
        public UDSContactValidatorMapper() { }
        #endregion

        #region [ Methods ]
        public override UDSContactValidator Map(UDSContact entity, UDSContactValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.ContactManual = entity.ContactManual;
            entityTransformed.ContactType = entity.ContactType;
            entityTransformed.ContactLabel = entity.ContactLabel;
            entityTransformed.RelationType = entity.RelationType;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion      

            #region [ Navigation Properties ]
            entityTransformed.Contact = entity.Relation;
            entityTransformed.Repository = entity.Repository;
            #endregion

            return entityTransformed;
        }
        #endregion

    }
}
