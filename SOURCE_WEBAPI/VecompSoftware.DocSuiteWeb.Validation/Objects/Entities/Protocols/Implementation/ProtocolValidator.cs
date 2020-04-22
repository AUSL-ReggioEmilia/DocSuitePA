using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolValidator : ObjectValidator<Protocol, ProtocolValidator>, IProtocolValidator
    {
        #region [ Constructor ]
        public ProtocolValidator(ILogger logger, IProtocolValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {
            ProtocolLogs = new Collection<ProtocolLog>();
            ProtocolContacts = new Collection<ProtocolContact>();
            ProtocolRoles = new Collection<ProtocolRole>();
            Messages = new Collection<Message>();
            ProtocolParers = new Collection<ProtocolParer>();
            PECMails = new Collection<PECMail>();
            ProtocolLinks = new Collection<ProtocolLink>();
            LinkedProtocols = new Collection<ProtocolLink>();
            ProtocolRoleUsers = new Collection<ProtocolRoleUser>();
            ProtocolUsers = new Collection<ProtocolUser>();
            ProtocolContactManuals = new Collection<ProtocolContactManual>();
        }

        #endregion

        #region[ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public Guid UniqueId { get; set; }
        public string Object { get; set; }
        public string ObjectChangeReason { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string DocumentProtocol { get; set; }
        public int? IdDocument { get; set; }
        public int? IdAttachments { get; set; }
        public string DocumentCode { get; set; }
        public short IdStatus { get; set; }
        public string LastChangedReason { get; set; }
        public string AlternativeRecipient { get; set; }
        public string CheckPublication { get; set; }
        public DateTime? JournalDate { get; set; }
        public string ConservationStatus { get; set; }
        public DateTime? LastConservationDate { get; set; }
        public bool? HasConservatedDocs { get; set; }
        public Guid? IdAnnexed { get; set; }
        public DateTime? HandlerDate { get; set; }
        public bool? Modified { get; set; }
        public long? IdHummingBird { get; set; }
        public DateTime? ProtocolCheckDate { get; set; }
        public int? TdIdDocument { get; set; }
        public string TDError { get; set; }
        public int? DocAreaStatus { get; set; }
        public string DocAreaStatusDesc { get; set; }
        public short? IdAttachLocation { get; set; }
        public short IdProtocolKind { get; set; }
        public int? IdProtocolJournalLog { get; set; }
        public byte[] Timestamp { get; set; }
        public Guid? DematerialisationChainId { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public ProtocolDocumentType DocType { get; set; }
        public Location Location { get; set; }
        public Location AttachLocation { get; set; }
        public Category Category { get; set; }
        public Container Container { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public ICollection<ProtocolLog> ProtocolLogs { get; set; }
        public ICollection<ProtocolContact> ProtocolContacts { get; set; }
        public ICollection<ProtocolRole> ProtocolRoles { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ProtocolParer> ProtocolParers { get; set; }
        public ICollection<PECMail> PECMails { get; set; }
        public ICollection<ProtocolLink> ProtocolLinks { get; set; }
        public ICollection<ProtocolLink> LinkedProtocols { get; set; }
        public ICollection<ProtocolRoleUser> ProtocolRoleUsers { get; set; }
        public ICollection<ProtocolUser> ProtocolUsers { get; set; }
        public ICollection<ProtocolContactManual> ProtocolContactManuals { get; set; }

        #endregion

    }
}
