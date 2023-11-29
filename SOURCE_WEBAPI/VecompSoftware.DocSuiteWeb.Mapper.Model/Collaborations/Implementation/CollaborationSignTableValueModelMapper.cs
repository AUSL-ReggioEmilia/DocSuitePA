using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationSignTableValueModelMapper : BaseModelMapper<CollaborationTableValuedModel, CollaborationSignModel>, ICollaborationSignTableValueModelMapper
    {
        public override CollaborationSignModel Map(CollaborationTableValuedModel entity, CollaborationSignModel entityTransformed)
        {
            if (entity.CollaborationSign_IdCollaborationSign.HasValue)
            {
                entityTransformed.IdCollaborationSign = entity.CollaborationSign_IdCollaborationSign;
                entityTransformed.Incremental = entity.CollaborationSign_Incremental.Value;
                entityTransformed.IsActive = entity.CollaborationSign_IsActive.Value;
                entityTransformed.IsRequired = entity.CollaborationSign_IsRequired;
                entityTransformed.SignName = entity.CollaborationSign_SignName;
                entityTransformed.SignDate = entity.CollaborationSign_SignDate;
                entityTransformed.SignUser = entity.CollaborationSign_SignUser;
                entityTransformed.IsAbsent = entity.CollaborationSign_IsAbsent;
                entityTransformed.IdCollaboration = entity.IdCollaboration;
            }

            return entityTransformed;
        }
    }
}
