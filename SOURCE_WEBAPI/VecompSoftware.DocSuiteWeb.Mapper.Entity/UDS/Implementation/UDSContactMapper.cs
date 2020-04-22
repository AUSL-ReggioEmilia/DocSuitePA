using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSContactMapper : BaseEntityMapper<UDSContact, UDSContact>, IUDSContactMapper
    {
        public override UDSContact Map(UDSContact entity, UDSContact entityTransformed)
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

            return entityTransformed;
        }
    }
}
