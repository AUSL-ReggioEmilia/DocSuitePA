using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations
{
    public class CollaborationUserMapper : BaseEntityMapper<CollaborationUser, CollaborationUser>, ICollaborationUserMapper
    {
        public override CollaborationUser Map(CollaborationUser entity, CollaborationUser entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DestinationFirst = entity.DestinationFirst;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.DestinationType = entity.DestinationType;
            entityTransformed.DestinationName = entity.DestinationName;
            entityTransformed.DestinationEmail = entity.DestinationEmail;
            entityTransformed.Account = entity.Account;
            #endregion

            return entityTransformed;
        }

    }
}
