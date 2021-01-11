using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationSignModelMapper : BaseModelMapper<CollaborationSign, CollaborationSignModel>, ICollaborationSignModelMapper
    {
        public override CollaborationSignModel Map(CollaborationSign entity, CollaborationSignModel entityTransformed)
        {
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.IdCollaborationSign = entity.UniqueId;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.IsRequired = entity.IsRequired;
            entityTransformed.SignDate = entity.SignDate;
            entityTransformed.SignUser = entity.SignUser;
            entityTransformed.SignName = entity.SignName;
            entityTransformed.IsAbsent = entity.IsAbsent;

            return entityTransformed;
        }
    }
}
