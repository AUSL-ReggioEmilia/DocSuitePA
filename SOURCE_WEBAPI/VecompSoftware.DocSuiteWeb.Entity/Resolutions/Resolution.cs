using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public class Resolution : DSWBaseEntity, IWorkflowContentBase
    {
        #region [ Constructor ]
        public Resolution() : this(Guid.NewGuid()) { }
        public Resolution(Guid uniqueId)
            : base(uniqueId)
        {
            ResolutionContacts = new HashSet<ResolutionContact>();
            ResolutionLogs = new HashSet<ResolutionLog>();
            //ResolutionRoles = new HashSet<ResolutionRole>();
            WorkflowActions = new List<IWorkflowAction>();
            DocumentSeriesItemLinks = new HashSet<DocumentSeriesItemLink>();
            ResolutionDocumentSeriesItems = new HashSet<ResolutionDocumentSeriesItem>();
            Messages = new HashSet<Message>();
          
        }
        #endregion

        #region [ Properties ]

        public DateTime? AdoptionDate { get; set; }

        public string AlternativeAssignee { get; set; }

        public string AlternativeManager { get; set; }

        public string AlternativeProposer { get; set; }

        public string AlternativeRecipient { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public DateTime? EffectivenessDate { get; set; }

        public DateTime? LeaveDate { get; set; }

        public int? Number { get; set; }

        public DateTime? ProposeDate { get; set; }

        public DateTime? PublishingDate { get; set; }

        public DateTime? ResponseDate { get; set; }

        public string ServiceNumber { get; set; }

        public string Object { get; set; }

        public DateTime? WaitDate { get; set; }

        public DateTime? WarningDate { get; set; }

        public string WorkflowType { get; set; }

        public short? Year { get; set; }

        public string ProposeUser { get; set; }

        public string LeaveUser { get; set; }

        public string EffectivenessUser { get; set; }

        public string ResponseUser { get; set; }

        public string WaitUser { get; set; }

        public string ConfirmUser { get; set; }

        public string WarningUser { get; set; }

        public string PublishingUser { get; set; }

        public string AdoptionUser { get; set; }

        public byte IdType { get; set; }

        public ResolutionStatus Status { get; set; }

        public string InclusiveNumber { get; set; }

        public DateTime? WebPublicationDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Category Category { get; set; }

        public virtual Container Container { get; set; }

        public virtual FileResolution FileResolution { get; set; }

        public virtual ICollection<ResolutionContact> ResolutionContacts { get; set; }

        public virtual ICollection<ResolutionLog> ResolutionLogs { get; set; }

        public virtual ICollection<ResolutionDocumentSeriesItem> ResolutionDocumentSeriesItems { get; set; }

        public virtual ICollection<DocumentSeriesItemLink> DocumentSeriesItemLinks { get; set; }

        public virtual ResolutionKind ResolutionKind { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        //public virtual ICollection<ResolutionRole> ResolutionRoles { get; set; }


        #endregion

        #region [ Not Mapping Properties ]
        public bool WorkflowAutoComplete { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }

        #endregion

        #region[ Methods ]
        public override string GetTitle()
        {
            return string.Format("{0}/{1}", Year, (Number.HasValue) ? Number.ToString() : ServiceNumber);
        }
        #endregion

    }
}
