using System;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolMapper : BaseEntityMapper<Protocol, Protocol>, IProtocolMapper
    {
        public override Protocol Map(Protocol entity, Protocol entityTransformed)
        {
            #region [ Base ]

            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            //entityTransformed.Object = entity.Object;
            entityTransformed.ObjectChangeReason = entity.ObjectChangeReason;
            entityTransformed.DocumentProtocol = entity.DocumentProtocol;
            entityTransformed.DocumentCode = entity.DocumentCode;
            entityTransformed.IdStatus = entity.IdStatus;
            entityTransformed.LastChangedReason = entity.LastChangedReason;
            entityTransformed.AlternativeRecipient = entity.AlternativeRecipient;
            entityTransformed.CheckPublication = entity.CheckPublication;
            entityTransformed.ConservationStatus = entity.ConservationStatus;
            entityTransformed.TDError = entity.TDError;
            entityTransformed.DocAreaStatusDesc = entity.DocAreaStatusDesc;
            entityTransformed.IdProtocolKind = entity.IdProtocolKind;

            if (entity.DocumentDate.HasValue)
            {
                entityTransformed.DocumentDate = entity.DocumentDate;
            }

            if (entity.IdDocument.HasValue)
            {
                entityTransformed.IdDocument = entity.IdDocument;
            }

            if (entity.IdAttachments.HasValue)
            {
                entityTransformed.IdAttachments = entity.IdAttachments;
            }

            if (entity.JournalDate.HasValue)
            {
                entityTransformed.JournalDate = entity.JournalDate;
            }

            if (entity.LastConservationDate.HasValue)
            {
                entityTransformed.LastConservationDate = entity.LastConservationDate;
            }

            if (entity.HasConservatedDocs.HasValue)
            {
                entityTransformed.HasConservatedDocs = entity.HasConservatedDocs;
            }

            if (entity.IdAnnexed.HasValue && entity.IdAnnexed != Guid.Empty)
            {
                entityTransformed.IdAnnexed = entity.IdAnnexed;
            }

            if (entity.HandlerDate.HasValue)
            {
                entityTransformed.HandlerDate = entity.HandlerDate;
            }

            if (entity.Modified.HasValue)
            {
                entityTransformed.Modified = entity.Modified;
            }

            if (entity.IdHummingBird.HasValue)
            {
                entityTransformed.IdHummingBird = entity.IdHummingBird;
            }

            if (entity.ProtocolCheckDate.HasValue)
            {
                entityTransformed.ProtocolCheckDate = entity.ProtocolCheckDate;
            }

            if (entity.TdIdDocument.HasValue)
            {
                entityTransformed.TdIdDocument = entity.TdIdDocument;
            }

            if (entity.DocAreaStatus.HasValue)
            {
                entityTransformed.DocAreaStatus = entity.DocAreaStatus;
            }

            if (entity.IdProtocolJournalLog.HasValue)
            {
                entityTransformed.IdProtocolJournalLog = entity.IdProtocolJournalLog;
            }

            if (entity.DematerialisationChainId.HasValue && entity.DematerialisationChainId != Guid.Empty)
            {
                entityTransformed.DematerialisationChainId = entity.DematerialisationChainId;
            }

            #endregion

            return entityTransformed;
        }

    }
}
