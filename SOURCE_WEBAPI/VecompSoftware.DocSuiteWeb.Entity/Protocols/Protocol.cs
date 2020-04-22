using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class Protocol : DSWBaseEntity, IUnauditableEntity, IWorkflowContentBase
    {
        #region [ Constructor ]

        public Protocol() : this(Guid.NewGuid()) { }

        public Protocol(Guid uniqueId)
            : base(uniqueId)
        {
            ProtocolLogs = new HashSet<ProtocolLog>();
            DocumentSeriesItems = new HashSet<DocumentSeriesItem>();
            ProtocolContacts = new HashSet<ProtocolContact>();
            ProtocolRoles = new HashSet<ProtocolRole>();
            Messages = new HashSet<Message>();
            PECMails = new HashSet<PECMail>();
            ProtocolLinks = new HashSet<ProtocolLink>();
            LinkedProtocols = new HashSet<ProtocolLink>();
            ProtocolRoleUsers = new HashSet<ProtocolRoleUser>();
            ProtocolUsers = new HashSet<ProtocolUser>();
            ProtocolParers = new HashSet<ProtocolParer>();
            ProtocolContactManuals = new HashSet<ProtocolContactManual>();
            WorkflowActions = new List<IWorkflowAction>();
        }
        #endregion

        #region[ Properties ]

        public short Year { get; set; }
        public int Number { get; set; }
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
        public short IdProtocolKind { get; set; }
        public int? IdProtocolJournalLog { get; set; }
        public Guid? DematerialisationChainId { get; set; }
        #endregion

        #region[ Navigation Properties ]

        public virtual AdvancedProtocol AdvancedProtocol { get; set; }

        public virtual ProtocolDocumentType DocType { get; set; }

        public virtual Location Location { get; set; }

        public virtual Location AttachLocation { get; set; }

        public Category Category { get; set; }

        public virtual Container Container { get; set; }

        public virtual ProtocolType ProtocolType { get; set; }

        public virtual ICollection<ProtocolLog> ProtocolLogs { get; set; }

        public virtual ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }

        public virtual ICollection<ProtocolLink> ProtocolLinks { get; set; }

        public virtual ICollection<ProtocolLink> LinkedProtocols { get; set; }

        public virtual ICollection<ProtocolContact> ProtocolContacts { get; set; }

        public virtual ICollection<ProtocolRole> ProtocolRoles { get; set; }

        public virtual ICollection<ProtocolRoleUser> ProtocolRoleUsers { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<PECMail> PECMails { get; set; }

        public virtual ICollection<ProtocolUser> ProtocolUsers { get; set; }

        public virtual ICollection<ProtocolParer> ProtocolParers { get; set; }

        public virtual ICollection<ProtocolContactManual> ProtocolContactManuals { get; set; }

        #endregion

        #region [ Not Mapping Properties ]
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }

        #endregion

        #region[ Methods ]

        public override string GetTitle()
        {
            return string.Format("{0}/{1:0000000}", Year, Number);
        }
        #endregion
    }
}
