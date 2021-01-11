using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
{
    public class DocumentUnit : DSWBaseEntity, IUnauditableEntity, IWorkflowContentBase
    {
        #region [ Constructor ]

        public DocumentUnit() : this(Guid.NewGuid()) { }
        public DocumentUnit(Guid uniqueId)
            : base(uniqueId)
        {
            DocumentUnitRoles = new HashSet<DocumentUnitRole>();
            DocumentUnitChains = new HashSet<DocumentUnitChain>();
            DocumentUnitUsers = new HashSet<DocumentUnitUser>();
            DocumentUnitFascicleHistoricizedCategories = new HashSet<DocumentUnitFascicleHistoricizedCategory>();
            DocumentUnitFascicleCategories = new HashSet<DocumentUnitFascicleCategory>();
            TransparentAdministrationMonitorLogs = new HashSet<TransparentAdministrationMonitorLog>();
            UDSDocumentUnits = new HashSet<UDSDocumentUnit>();
            WorkflowActivities = new HashSet<WorkflowActivity>();
            FascicleDocumentUnits = new HashSet<FascicleDocumentUnit>();
            Collaborations = new HashSet<Collaboration>();
            POLRequests = new HashSet<PosteOnLineRequest>();
            WorkflowActions = new HashSet<IWorkflowAction>();
        }
        #endregion

        #region [ Properties ]

        public short Year { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public int Environment { get; set; }

        public string DocumentUnitName { get; set; }

        public string Subject { get; set; }

        public DocumentUnitStatus Status { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Category Category { get; set; }

        public virtual Container Container { get; set; }

        public virtual ICollection<FascicleDocumentUnit> FascicleDocumentUnits { get; set; }

        public virtual Fascicle Fascicle { get; set; }

        public virtual UDSRepository UDSRepository { get; set; }

        public virtual TenantAOO TenantAOO { get; set; }

        public virtual ICollection<DocumentUnitRole> DocumentUnitRoles { get; set; }

        public virtual ICollection<DocumentUnitChain> DocumentUnitChains { get; set; }

        public virtual ICollection<DocumentUnitUser> DocumentUnitUsers { get; set; }

        public virtual ICollection<DocumentUnitFascicleHistoricizedCategory> DocumentUnitFascicleHistoricizedCategories { get; set; }

        public virtual ICollection<DocumentUnitFascicleCategory> DocumentUnitFascicleCategories { get; set; }

        public virtual ICollection<TransparentAdministrationMonitorLog> TransparentAdministrationMonitorLogs { get; set; }

        public virtual ICollection<UDSDocumentUnit> UDSDocumentUnits { get; set; }

        public virtual ICollection<WorkflowActivity> WorkflowActivities { get; set; }

        public virtual ICollection<PECMail> PecMails { get; set; }

        public virtual ICollection<Collaboration> Collaborations { get; set; }

        public virtual ICollection<PosteOnLineRequest> POLRequests { get; set; }

        public virtual ICollection<UDSDocumentUnit> UDSSourceDocumentUnits { get; set; }

        public virtual ICollection<UDSCollaboration> UDSCollaborations { get; set; }

        public virtual ICollection<UDSContact> UDSContacts { get; set; }

        public virtual ICollection<UDSMessage> UDSMessages { get; set; }

        public virtual ICollection<UDSRole> UDSRoles { get; set; }

        public virtual ICollection<UDSPECMail> UDSPECMails { get; set; }

        public virtual ICollection<UDSUser> UDSUsers { get; set; }
        #endregion

        #region [ Not Mapping Properties ]
        public bool WorkflowAutoComplete { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }

        #endregion
    }
}
