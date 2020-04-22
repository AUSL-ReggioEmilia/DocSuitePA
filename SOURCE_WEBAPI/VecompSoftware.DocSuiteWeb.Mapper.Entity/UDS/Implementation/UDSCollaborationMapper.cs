using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSCollaborationMapper : BaseEntityMapper<UDSCollaboration, UDSCollaboration>, IUDSCollaborationMapper
    {
        public override UDSCollaboration Map(UDSCollaboration entity, UDSCollaboration entityTransformed)
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
