using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationVersioningModelMapper : BaseModelMapper<CollaborationVersioning, CollaborationVersioningModel>, ICollaborationVersioningModelMapper
    {
        public override CollaborationVersioningModel Map(CollaborationVersioning entity, CollaborationVersioningModel entityTransformed)
        {
            entityTransformed.CheckedOut = entity.CheckedOut;
            entityTransformed.CheckOutDate = entity.CheckOutDate;
            entityTransformed.CheckOutUser = entity.CheckOutUser;
            entityTransformed.CollaborationIncremental = entity.CollaborationIncremental;
            entityTransformed.DocumentName = entity.DocumentName;
            entityTransformed.IdCollaborationVersioning = entity.UniqueId;
            entityTransformed.IdDocument = entity.IdDocument;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.RegistrationUser = entity.RegistrationUser;

            return entityTransformed;
        }
    }
}
