using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperContainerModel : BaseEntityMapper<Container, ContainerModel>
    {
        #region [ Fields ]
        private readonly MapperLocationModel _mapperLocationModel;
        #endregion

        #region [ Constructor ]

        public MapperContainerModel() : base()
        {
            _mapperLocationModel = new MapperLocationModel();
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<Container, Container> MappingProjection(IQueryOver<Container, Container> queryOver)
        {
            throw new NotImplementedException();
        }

        private LocationModel TryMappingLocation(Location location)
        {
            LocationModel model = null;
            try
            {
                model = _mapperLocationModel.MappingDTO(location);
            }
            catch (Exception) {  }
            return model;
        }

        protected override ContainerModel TransformDTO(Container entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Container se l'entità non è inizializzata");

            ContainerModel model = new ContainerModel(entity.Id)
            {
                Name = entity.Name,
                UniqueId = entity.UniqueId
            };

            model.ProtLocation = TryMappingLocation(entity.ProtLocation);
            model.ReslLocation = TryMappingLocation(entity.ReslLocation);
            model.DocmLocation = TryMappingLocation(entity.DocmLocation);
            model.DocumentSeriesLocation = TryMappingLocation(entity.DocumentSeriesLocation);
            model.ProtocolAttachmentLocation = TryMappingLocation(entity.ProtAttachLocation);
            model.DocumentSeriesAnnexedLocation = TryMappingLocation(entity.DocumentSeriesAnnexedLocation);
            model.DocumentSeriesUnpublishedAnnexedLocation = TryMappingLocation(entity.DocumentSeriesUnpublishedAnnexedLocation);

            return model;
        }

        #endregion
    }
}
