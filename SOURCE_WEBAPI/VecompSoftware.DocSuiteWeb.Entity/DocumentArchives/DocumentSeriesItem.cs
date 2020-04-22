using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
{
    public class DocumentSeriesItem : DSWBaseEntity, IWorkflowContentBase
    {
        #region [ Constructor ]

        public DocumentSeriesItem() : this(Guid.NewGuid()) { }
        public DocumentSeriesItem(Guid uniqueId)
            : base(uniqueId)
        {
            DocumentSeriesItemRoles = new HashSet<DocumentSeriesItemRole>();
            DocumentSeriesItemLogs = new HashSet<DocumentSeriesItemLog>();
            DocumentSeriesItemLinks = new HashSet<DocumentSeriesItemLink>();
            Messages = new HashSet<Message>();
            Protocols = new HashSet<Protocol>();
            WorkflowActions = new List<IWorkflowAction>();
        }
        #endregion

        #region[ Properties ]

        public int? Year { get; set; }
        public int? Number { get; set; }
        public Guid? IdMain { get; set; }
        public Guid? IdAnnexed { get; set; }
        public Guid? IdUnpublishedAnnexed { get; set; }
        public DateTime? PublishingDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public DocumentSeriesItemStatus? Status { get; set; }
        public string Subject { get; set; }
        public bool? Priority { get; set; }
        public int? IdDocumentSeriesSubsection { get; set; }
        public Guid? DematerialisationChainId { get; set; }
        public bool? HasMainDocument { get; set; }
        public string ConstraintValue { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public virtual DocumentSeries DocumentSeries { get; set; }

        
        public virtual Category Category { get; set; }
        public virtual Location Location { get; set; }
        public virtual Location LocationAnnexed { get; set; }
        public virtual Location LocationUnpublishedAnnexed { get; set; }
        public virtual ICollection<DocumentSeriesItemRole> DocumentSeriesItemRoles { get; set; }
        public virtual ICollection<DocumentSeriesItemLog> DocumentSeriesItemLogs { get; set; }
        public virtual ICollection<DocumentSeriesItemLink> DocumentSeriesItemLinks { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }
        public virtual ICollection<ResolutionDocumentSeriesItem> ResolutionDocumentSeriesItems { get; set; }

        #endregion

        #region [ Not Mapping Properties ]
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }

        public ICollection<IWorkflowAction> WorkflowActions { get; set; }

        #endregion

        #region[ Methods ]
        public override string GetTitle()
        {
            return string.Format("{0}/{1}", Year, Number);
        }
        #endregion
    }
}
