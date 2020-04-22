using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions
{
    public class ResolutionCollaborationTableValueModelMapper : BaseModelMapper<CollaborationTableValuedModel, ResolutionModel>, IResolutionCollaborationTableValueModelMapper
    {
        public override ResolutionModel Map(CollaborationTableValuedModel entity, ResolutionModel entityTransformed)
        {
            entityTransformed = null;
            if (entity.Resolution_IdResolution.HasValue)
            {
                entityTransformed = new ResolutionModel
                {
                    AdoptionDate = entity.Resolution_AdoptionDate,
                    IdResolution = entity.Resolution_IdResolution,
                    Number = entity.Resolution_Number,
                    PublishingDate = entity.Resolution_PublishingDate,
                    ServiceNumber = entity.Resolution_ServiceNumber,
                    Year = entity.Resolution_Year
                };
            }

            return entityTransformed;
        }
    }
}
