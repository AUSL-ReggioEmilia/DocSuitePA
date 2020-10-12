using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class LocationMapper : BaseEntityMapper<Location, Location>, ILocationMapper
    {
        public LocationMapper()
        {

        }

        public override Location Map(Location entity, Location entityTransformed)
        {
            #region [ Base ]
            entityTransformed.ConservationArchive = entity.ConservationArchive;
            entityTransformed.DossierArchive = entity.DossierArchive;
            entityTransformed.Name = entity.Name;
            entityTransformed.ProtocolArchive = entity.ProtocolArchive;
            entityTransformed.ResolutionArchive = entity.ResolutionArchive;
            #endregion

            return entityTransformed;
        }

    }
}
