using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolValidatorMapper : BaseMapper<Protocol, ProtocolValidator>, IProtocolValidatorMapper
    {
        public ProtocolValidatorMapper() { }

        public override ProtocolValidator Map(Protocol entity, ProtocolValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Object = entity.Object;
            entityTransformed.ObjectChangeReason = entity.ObjectChangeReason;
            entityTransformed.DocumentDate = entity.DocumentDate;
            entityTransformed.DocumentProtocol = entity.DocumentProtocol;
            entityTransformed.IdDocument = entity.IdDocument;
            entityTransformed.IdAttachments = entity.IdAttachments;
            entityTransformed.DocumentCode = entity.DocumentCode;
            entityTransformed.IdStatus = entity.IdStatus;
            entityTransformed.LastChangedReason = entity.LastChangedReason;
            entityTransformed.AlternativeRecipient = entity.AlternativeRecipient;
            entityTransformed.CheckPublication = entity.CheckPublication;
            entityTransformed.JournalDate = entity.JournalDate;
            entityTransformed.IdProtocolJournalLog = entity.IdProtocolJournalLog;
            entityTransformed.ConservationStatus = entity.ConservationStatus;
            entityTransformed.LastConservationDate = entity.LastConservationDate;
            entityTransformed.HasConservatedDocs = entity.HasConservatedDocs;
            entityTransformed.IdAnnexed = entity.IdAnnexed;
            entityTransformed.HandlerDate = entity.HandlerDate;
            entityTransformed.Modified = entity.Modified;
            entityTransformed.IdHummingBird = entity.IdHummingBird;
            entityTransformed.ProtocolCheckDate = entity.ProtocolCheckDate;
            entityTransformed.TdIdDocument = entity.TdIdDocument;
            entityTransformed.TDError = entity.TDError;
            entityTransformed.DocAreaStatus = entity.DocAreaStatus;
            entityTransformed.DocAreaStatusDesc = entity.DocAreaStatusDesc;
            entityTransformed.IdProtocolKind = entity.IdProtocolKind;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.DematerialisationChainId = entity.DematerialisationChainId;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.DocType = entity.DocType;
            entityTransformed.Location = entity.Location;
            entityTransformed.AttachLocation = entity.AttachLocation;
            entityTransformed.Container = entity.Container;
            entityTransformed.Category = entity.Category;
            entityTransformed.ProtocolType = entity.ProtocolType;
            entityTransformed.ProtocolLogs = entity.ProtocolLogs;
            entityTransformed.ProtocolContacts = entity.ProtocolContacts;
            entityTransformed.ProtocolRoles = entity.ProtocolRoles;
            entityTransformed.Messages = entity.Messages;
            entityTransformed.ProtocolParers = entity.ProtocolParers;
            entityTransformed.PECMails = entity.PECMails;
            entityTransformed.ProtocolLinks = entity.ProtocolLinks;
            entityTransformed.LinkedProtocols = entity.LinkedProtocols;
            entityTransformed.ProtocolRoleUsers = entity.ProtocolRoleUsers;
            entityTransformed.ProtocolContactManuals = entity.ProtocolContactManuals;
            entityTransformed.ProtocolUsers = entity.ProtocolUsers;


            #endregion

            return entityTransformed;
        }

    }
}
