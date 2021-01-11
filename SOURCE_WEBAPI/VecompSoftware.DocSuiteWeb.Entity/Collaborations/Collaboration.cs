using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class Collaboration : DSWBaseEntity, IUnauditableEntity, IWorkflowContentBase, IContentBase
    {
        #region [ Constructor ]

        public Collaboration() : this(Guid.NewGuid()) { }

        public Collaboration(Guid uniqueId)
            : base(uniqueId)
        {
            CollaborationLogs = new HashSet<CollaborationLog>();
            CollaborationSigns = new HashSet<CollaborationSign>();
            CollaborationUsers = new HashSet<CollaborationUser>();
            CollaborationVersionings = new HashSet<CollaborationVersioning>();
            CollaborationAggregates = new HashSet<CollaborationAggregate>();
            CollaborationAggregateFathers = new HashSet<CollaborationAggregate>();
            UDSCollaborations = new HashSet<UDSCollaboration>();
            WorkflowActions = new List<IWorkflowAction>();
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Get or set DocumentType
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Get or set IdPriority
        /// </summary>
        public string IdPriority { get; set; }

        /// <summary>
        /// Get or set IdStatus
        /// </summary>
        public string IdStatus { get; set; }

        /// <summary>
        /// Get or set SignCount
        /// </summary>
        public short? SignCount { get; set; }

        /// <summary>
        /// Get or set MemorandumDate
        /// </summary>
        public DateTime? MemorandumDate { get; set; }

        /// <summary>
        /// Get or set Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Get or set Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Get or set Year
        /// </summary>
        public short? Year { get; set; }

        /// <summary>
        /// Get or set Number
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// Get or set TemplateName
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Get or set PublicationUser
        /// </summary>
        public string PublicationUser { get; set; }

        /// <summary>
        /// Get or set PublicationDate
        /// </summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        /// Get or set RegistrationName
        /// </summary>
        public string RegistrationName { get; set; }

        /// <summary>
        /// Get or set RegistrationEmail
        /// </summary>
        public string RegistrationEmail { get; set; }

        /// <summary>
        /// Get or set SourceProtocolYear
        /// </summary>
        public short? SourceProtocolYear { get; set; }

        /// <summary>
        /// Get or set SourceProtocolNumber
        /// </summary>
        public int? SourceProtocolNumber { get; set; }

        /// <summary>
        /// Get or set AlertDate
        /// </summary>
        public DateTime? AlertDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Get or set DeskCollaboration reference.
        /// 1 to 1 relation to create relation between Desk and Collaboration tables.
        /// <remarks>
        /// Questa proprietà è stata implementata per introdurre un artefatto sulle dll di DocSuite.
        /// L'introduzione di questa proprietà ci consente di non avere un errore di referenze circolari.
        /// </remarks>
        /// </summary>
        public virtual DeskCollaboration DeskCollaboration { get; set; }

        /// <summary>
        /// Get or set Desk reference. 
        /// </summary>
        public virtual Desk Desk { get; set; }

        /// <summary>
        /// Get or set Location reference
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>
        /// Get or set DocumentSeriesItem
        /// </summary>
        public virtual DocumentSeriesItem DocumentSeriesItem { get; set; }

        /// <summary>
        /// Get or set Resolution
        /// </summary>
        public virtual Resolution Resolution { get; set; }

        /// <summary>
        /// Get or sert WorkflowInstance
        /// </summary>
        public virtual WorkflowInstance WorkflowInstance { get; set; }

        public virtual DocumentUnit DocumentUnit { get; set; }

        /// <summary>
        /// Get or set CollaborationLog reference
        /// </summary>
        public virtual ICollection<CollaborationLog> CollaborationLogs { get; set; }

        /// <summary>
        /// Get or set CollaborationSign reference
        /// </summary>
        public virtual ICollection<CollaborationSign> CollaborationSigns { get; set; }

        /// <summary>
        /// Get or set CollaborationUser reference
        /// </summary>
        public virtual ICollection<CollaborationUser> CollaborationUsers { get; set; }

        /// <summary>
        /// Get or set CollaborationVersioning reference
        /// </summary>
        public virtual ICollection<CollaborationVersioning> CollaborationVersionings { get; set; }

        /// <summary>
        /// Get or set CollaborationAggregate reference
        /// </summary>
        public virtual ICollection<CollaborationAggregate> CollaborationAggregateFathers { get; set; }

        /// <summary>
        /// Get or set CollaborationAggregate reference
        /// </summary>
        public virtual ICollection<CollaborationAggregate> CollaborationAggregates { get; set; }

        /// <summary>
        /// Get or set UDSCollaboration reference
        /// </summary>
        public virtual ICollection<UDSCollaboration> UDSCollaborations { get; set; }

        #endregion

        #region [ Not Mapping Properties ]
        public bool WorkflowAutoComplete { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }

        #endregion
    }
}
