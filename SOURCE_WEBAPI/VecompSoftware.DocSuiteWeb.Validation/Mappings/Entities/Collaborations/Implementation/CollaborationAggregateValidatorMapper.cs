using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations
{
    public class CollaborationAggregateValidatorMapper : BaseMapper<CollaborationAggregate, CollaborationAggregateValidator>, ICollaborationAggregateValidatorMapper
    {
        public CollaborationAggregateValidatorMapper() { }

        public override CollaborationAggregateValidator Map(CollaborationAggregate entity, CollaborationAggregateValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.CollaborationDocumentType = entity.CollaborationDocumentType;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.CollaborationFather = entity.CollaborationFather;

            #endregion

            return entityTransformed;
        }

    }
}
