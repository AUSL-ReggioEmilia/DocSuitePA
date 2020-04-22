using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class ContainerModelMapper : BaseModelMapper<Container, ContainerModel>, IContainerModelMapper
    {

        #region [ Fields ] 
        private readonly ILocationModelMapper _locationModelMapper;
        #endregion

        #region [ Constructor ] 
        public ContainerModelMapper(ILocationModelMapper locationModelMapper)
        {
            _locationModelMapper = locationModelMapper;
        }
        #endregion
        public override ContainerModel Map(Container entity, ContainerModel entityTransformed)
        {
            entityTransformed.IdContainer = entity.EntityShortId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            if (entity.ProtAttachLocation != null)
            {
                entityTransformed.ProtocolAttachmentLocation = _locationModelMapper.Map(entity.ProtAttachLocation, new LocationModel());
            }
            if (entity.DocumentSeriesAnnexedLocation != null)
            {
                entityTransformed.DocumentSeriesAnnexedLocation = _locationModelMapper.Map(entity.DocumentSeriesAnnexedLocation, new LocationModel());
            }
            if (entity.DocumentSeriesLocation != null)
            {
                entityTransformed.DocumentSeriesLocation = _locationModelMapper.Map(entity.DocumentSeriesLocation, new LocationModel());
            }
            if (entity.DocumentSeriesUnpublishedAnnexedLocation != null)
            {
                entityTransformed.DocumentSeriesUnpublishedAnnexedLocation = _locationModelMapper.Map(entity.DocumentSeriesUnpublishedAnnexedLocation, new LocationModel());
            }
            if (entity.ProtLocation != null)
            {
                entityTransformed.ProtLocation = _locationModelMapper.Map(entity.ProtLocation, new LocationModel());
            }
            if (entity.ReslLocation != null)
            {
                entityTransformed.ReslLocation = _locationModelMapper.Map(entity.ReslLocation, new LocationModel());
            }
            if (entity.DocmLocation != null)
            {
                entityTransformed.DocmLocation = _locationModelMapper.Map(entity.DocmLocation, new LocationModel());
            }
            return entityTransformed;
        }

    }
}
