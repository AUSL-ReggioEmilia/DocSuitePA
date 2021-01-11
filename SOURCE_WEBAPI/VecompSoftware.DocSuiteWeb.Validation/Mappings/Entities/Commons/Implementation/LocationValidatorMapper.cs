using System;
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
            entityTransformed.ConservationArchive = entity.ConservationArchive;
            entityTransformed.DossierArchive = entity.DossierArchive;
            entityTransformed.ProtocolArchive = entity.ProtocolArchive;
            entityTransformed.ResolutionArchive = entity.ResolutionArchive;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.AttachProtocols = entity.AttachProtocols;
            entityTransformed.Collaborations = entity.Collaborations;
            entityTransformed.DeskContainers = entity.DeskContainers;
            entityTransformed.DocmContainers = entity.DocmContainers;
            entityTransformed.DocumentSeriesAnnexedContainers = entity.DocumentSeriesAnnexedContainers;
            entityTransformed.DocumentSeriesContainers = entity.DocumentSeriesContainers;
            entityTransformed.DocumentSeriesItemAnnexes = entity.DocumentSeriesItemAnnexes;
            entityTransformed.DocumentSeriesItems = entity.DocumentSeriesItems;
            entityTransformed.DocumentSeriesItemUnpublishedAnnexes = entity.DocumentSeriesItemUnpublishedAnnexes;
            entityTransformed.DocumentSeriesUnpublishedAnnexedContainers = entity.DocumentSeriesUnpublishedAnnexedContainers;
            entityTransformed.Messages = entity.Messages;
            entityTransformed.PECMailBoxes = entity.PECMailBoxes;
            entityTransformed.PECMails = entity.PECMails;
            entityTransformed.ProtAttachContainers = entity.ProtAttachContainers;
            entityTransformed.ProtContainers = entity.ProtContainers;
            entityTransformed.Protocols = entity.Protocols;
            entityTransformed.ReslContainers = entity.ReslContainers;
            entityTransformed.UDSContainers = entity.UDSContainers;
            #endregion

            return entityTransformed;
        }

    }
}
