using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperLocationModel : BaseEntityMapper<Location, LocationModel>
    {
        protected override IQueryOver<Location, Location> MappingProjection(IQueryOver<Location, Location> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override LocationModel TransformDTO(Location entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Location se l'entità non è inizializzata");

            LocationModel model = new LocationModel(Convert.ToInt16(entity.Id))
            {
                Name = entity.Name,
                UniqueId = entity.UniqueId,
                ProtocolArchive = entity.ProtBiblosDSDB,
                ResolutionArchive = entity.ReslBiblosDSDB,
                DossierArchive = entity.DocmBiblosDSDB,
                ConservationArchive = entity.ConsBiblosDSDB
            };
            

            return model;
        }
    }
}
