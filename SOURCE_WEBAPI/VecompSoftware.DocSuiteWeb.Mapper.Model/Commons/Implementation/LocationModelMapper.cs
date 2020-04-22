using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class LocationModelMapper : BaseModelMapper<Location, LocationModel>, ILocationModelMapper
    {
        public override LocationModel Map(Location entity, LocationModel entityTransformed)
        {
            entityTransformed.IdLocation = entity.EntityShortId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.DocumentServer = entity.DocumentServer;
            entityTransformed.ProtocolArchive = entity.ProtocolArchive;
            entityTransformed.ResolutionArchive = entity.ResolutionArchive;
            entityTransformed.DossierArchive = entity.DossierArchive;
            entityTransformed.ConservationArchive = entity.ConservationArchive;
            entityTransformed.ConservationServer = entity.ConservationServer;
            return entityTransformed;
        }

    }
}
