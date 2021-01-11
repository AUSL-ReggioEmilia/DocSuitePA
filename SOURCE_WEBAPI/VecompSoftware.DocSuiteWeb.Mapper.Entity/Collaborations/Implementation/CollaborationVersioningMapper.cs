using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations
{
    public class CollaborationVersioningMapper : BaseEntityMapper<CollaborationVersioning, CollaborationVersioning>, ICollaborationVersioningMapper
    {
        public override CollaborationVersioning Map(CollaborationVersioning entity, CollaborationVersioning entityTransformed)
        {
            #region [ Base ]
            entityTransformed.CollaborationIncremental = entity.CollaborationIncremental;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.IdDocument = entity.IdDocument;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.DocumentName = entity.DocumentName;
            entityTransformed.CheckedOut = entity.CheckedOut;
            entityTransformed.CheckOutDate = entity.CheckOutDate;
            entityTransformed.CheckOutUser = entity.CheckOutUser;
            entityTransformed.CheckOutSessionId = entity.CheckOutSessionId;
            entityTransformed.DocumentChecksum = entity.DocumentChecksum;
            entityTransformed.DocumentGroup = entity.DocumentGroup;
            #endregion

            return entityTransformed;
        }

    }
}
