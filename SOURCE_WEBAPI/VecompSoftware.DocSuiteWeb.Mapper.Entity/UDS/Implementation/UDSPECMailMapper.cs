using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSPECMailMapper : BaseEntityMapper<UDSPECMail, UDSPECMail>, IUDSPECMailMapper
    {
        public override UDSPECMail Map(UDSPECMail entity, UDSPECMail entityTransformed)
        {
            #region [ Base ]

            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.RelationType = entity.RelationType;
            #endregion

            return entityTransformed;
        }
    }
}
