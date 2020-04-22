using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSContactEntityMapper : BaseModelMapper<UDSContactModel, UDSContact>, IUDSContactEntityMapper
    {
        #region [ Methods ]
        public override UDSContact Map(UDSContactModel entity, UDSContact entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Environment = entity.Environment;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.ContactLabel = entity.ContactLabel;
            entityTransformed.ContactManual = entity.ContactManual;
            entityTransformed.RelationType = (Entity.UDS.UDSRelationType)entity.RelationType;
            entityTransformed.ContactType = entity.IdContact == 0 ? (short)UDSContactType.Manual : (short)UDSContactType.Normal;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
