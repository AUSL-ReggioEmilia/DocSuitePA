using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class LocationValidatorMapper : BaseMapper<Location, LocationValidator>, ILocationValidatorMapper
    {
        public LocationValidatorMapper() { }

        public override LocationValidator Map(Location entity, LocationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.Name = entity.Name;
            entityTransformed.DocumentServer = entity.DocumentServer;
            entityTransformed.ConservationServer = entity.ConservationServer;
            entityTransformed.ConsBiblosDsDb = entity.ConservationArchive;
            entityTransformed.DocmBiblosDsDb = entity.DossierArchive;
            entityTransformed.ProtBiblosDsDb = entity.ProtocolArchive;
            entityTransformed.ReslBiblosDsDb = entity.ResolutionArchive;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Messages = entity.Messages;
            entityTransformed.DocmContainers = entity.DocmContainers;
            entityTransformed.ProtContainers = entity.ProtContainers;
            entityTransformed.ReslContainers = entity.ReslContainers;
            entityTransformed.DeskContainers = entity.DeskContainers;
            entityTransformed.UDSContainers = entity.UDSContainers;
            entityTransformed.Protocols = entity.Protocols;
            entityTransformed.PECMails = entity.PECMails;
            entityTransformed.PECMailBoxes = entity.PECMailBoxes;
            entityTransformed.Collaborations = entity.Collaborations;
            entityTransformed.DocumentSeriesItems = entity.DocumentSeriesItems;
            entityTransformed.DocumentSeriesItemAnnexes = entity.DocumentSeriesItemAnnexes;
            entityTransformed.DocumentSeriesItemUnpublishedAnnexes = entity.DocumentSeriesItemUnpublishedAnnexes;
            #endregion

            return entityTransformed;
        }

    }
}
