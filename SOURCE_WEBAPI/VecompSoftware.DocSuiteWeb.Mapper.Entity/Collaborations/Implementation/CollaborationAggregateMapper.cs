using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations
{
    public class CollaborationAggregateMapper : BaseEntityMapper<CollaborationAggregate, CollaborationAggregate>, ICollaborationAggregateMapper
    {
        public override CollaborationAggregate Map(CollaborationAggregate entity, CollaborationAggregate entityTransformed)
        {
            #region [ Base ]
            entityTransformed.CollaborationDocumentType = entity.CollaborationDocumentType;
            #endregion

            return entityTransformed;
        }

    }
}
