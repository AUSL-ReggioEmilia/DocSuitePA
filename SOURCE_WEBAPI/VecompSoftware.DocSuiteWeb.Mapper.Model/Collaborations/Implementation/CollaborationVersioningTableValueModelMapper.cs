using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations
{
    public class CollaborationVersioningTableValueModelMapper : BaseModelMapper<CollaborationTableValuedModel, CollaborationVersioningModel>, ICollaborationVersioningTableValueModelMapper
    {
        public override CollaborationVersioningModel Map(CollaborationTableValuedModel entity, CollaborationVersioningModel entityTransformed)
        {
            entityTransformed.CollaborationIncremental = entity.CollaborationVersioning_CollaborationIncremental;
            entityTransformed.DocumentName = entity.CollaborationVersioning_DocumentName;
            entityTransformed.IdCollaborationVersioning = entity.CollaborationVersioning_IdCollaborationVersioning;
            entityTransformed.Incremental = entity.CollaborationVersioning_Incremental;
            entityTransformed.RegistrationUser = entity.CollaborationVersioning_RegistrationUser;

            return entityTransformed;
        }
    }
}
