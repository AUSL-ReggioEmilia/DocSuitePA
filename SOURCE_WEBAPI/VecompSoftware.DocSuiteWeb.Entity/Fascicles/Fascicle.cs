using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class Fascicle : DSWBaseEntity, IWorkflowContentBase
    {
        #region [ Constructor ]

        public Fascicle() : this(Guid.NewGuid()) { }

        public Fascicle(Guid uniqueId)
            : base(uniqueId)
        {
            Contacts = new HashSet<Contact>();
            FascicleLogs = new HashSet<FascicleLog>();
            FascicleLinks = new HashSet<FascicleLink>();
            LinkedFascicles = new HashSet<FascicleLink>();
            DocumentUnits = new HashSet<DocumentUnit>();
            FascicleDocuments = new HashSet<FascicleDocument>();
            FascicleRoles = new HashSet<FascicleRole>();
            DossierFolders = new HashSet<DossierFolder>();
            WorkflowInstances = new HashSet<WorkflowInstance>();
            FascicleFolders = new HashSet<FascicleFolder>();
            WorkflowActions = new List<IWorkflowAction>();
            FascicleDocumentUnits = new HashSet<FascicleDocumentUnit>();
            DocumentUnitFascicleCategories = new HashSet<DocumentUnitFascicleCategory>();
        }
        #endregion

        #region [ Properties ]

        public short Year { get; set; }

        public int Number { get; set; }

        public short? Conservation { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string FascicleObject { get; set; }

        public string Manager { get; set; }

        public string Rack { get; set; }

        public string Note { get; set; }

        public FascicleType FascicleType { get; set; }

        public VisibilityType VisibilityType { get; set; }

        public string MetadataValues { get; set; }

        public int? DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Category Category { get; set; }
        public virtual Container Container { get; set; }
        public virtual MetadataRepository MetadataRepository { get; set; }
        public virtual ProcessFascicleTemplate ProcessFascicleTemplate { get; set; }

        public virtual ICollection<FascicleDocumentUnit> FascicleDocumentUnits { get; set; }


        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<FascicleLog> FascicleLogs { get; set; }

        public virtual ICollection<FascicleLink> LinkedFascicles { get; set; }

        public virtual ICollection<FascicleLink> FascicleLinks { get; set; }

        public virtual ICollection<DocumentUnit> DocumentUnits { get; set; }

        public virtual ICollection<FascicleDocument> FascicleDocuments { get; set; }

        public virtual ICollection<FascicleRole> FascicleRoles { get; set; }

        public virtual ICollection<DossierFolder> DossierFolders { get; set; }

        public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; }

        public virtual ICollection<FascicleFolder> FascicleFolders { get; set; }
        public virtual ICollection<DocumentUnitFascicleCategory> DocumentUnitFascicleCategories { get; set; }

        public string WorkflowName { get; set; }

        public Guid? IdWorkflowActivity { get; set; }

        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion
    }
}
