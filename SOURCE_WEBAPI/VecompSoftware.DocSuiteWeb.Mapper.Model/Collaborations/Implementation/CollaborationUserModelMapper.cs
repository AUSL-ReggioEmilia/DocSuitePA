using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationUserModelMapper : BaseModelMapper<CollaborationUser, CollaborationUserModel>, ICollaborationUserModelMapper
    {
        public override CollaborationUserModel Map(CollaborationUser entity, CollaborationUserModel entityTransformed)
        {
            entityTransformed.Account = entity.Account;
            entityTransformed.IdCollaborationUser = entity.UniqueId;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.DestinationFirst = entity.DestinationFirst;
            entityTransformed.DestinationName = entity.DestinationName;

            return entityTransformed;
        }
    }
}
