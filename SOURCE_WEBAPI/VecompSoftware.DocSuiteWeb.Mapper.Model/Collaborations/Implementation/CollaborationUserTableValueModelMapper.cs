using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationUserTableValueModelMapper : BaseModelMapper<CollaborationTableValuedModel, CollaborationUserModel>, ICollaborationUserTableValueModelMapper
    {
        public override CollaborationUserModel Map(CollaborationTableValuedModel entity, CollaborationUserModel entityTransformed)
        {
            entityTransformed.IdCollaborationUser = entity.CollaborationUser_IdCollaborationUser;
            entityTransformed.DestinationFirst = entity.CollaborationUser_DestinationFirst;
            entityTransformed.DestinationName = entity.CollaborationUser_DestinationName;

            return entityTransformed;
        }
    }
}
