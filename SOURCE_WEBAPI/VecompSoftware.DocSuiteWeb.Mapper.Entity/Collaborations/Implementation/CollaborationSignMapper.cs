using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations
{
    public class CollaborationSignMapper : BaseEntityMapper<CollaborationSign, CollaborationSign>, ICollaborationSignMapper
    {
        public override CollaborationSign Map(CollaborationSign entity, CollaborationSign entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdStatus = entity.IdStatus;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.SignUser = entity.SignUser;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.SignName = entity.SignName;
            entityTransformed.SignEmail = entity.SignEmail;
            entityTransformed.SignDate = entity.SignDate;
            entityTransformed.IsRequired = entity.IsRequired;
            entityTransformed.IsAbsent = entity.IsAbsent;
            #endregion

            return entityTransformed;
        }

    }
}
