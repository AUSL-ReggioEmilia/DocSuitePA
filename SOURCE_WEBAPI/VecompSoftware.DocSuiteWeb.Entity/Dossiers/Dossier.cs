using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class Dossier : DSWBaseEntity
    {
        #region [ Constructor ]
        public Dossier() : this(Guid.NewGuid()) { }

        public Dossier(Guid uniqueId)
            : base(uniqueId)
        {
            DossierLogs = new HashSet<DossierLog>();
            DossierRoles = new HashSet<DossierRole>();
            DossierDocuments = new HashSet<DossierDocument>();
            DossierComments = new HashSet<DossierComment>();
            DossierFolders = new HashSet<DossierFolder>();
            DossierLinks = new HashSet<DossierLink>();
            Contacts = new HashSet<Contact>();
            Messages = new HashSet<Message>();
            WorkflowInstances = new HashSet<WorkflowInstance>();
            LinkedDossiers = new HashSet<DossierLink>();
            Processes = new HashSet<Process>();
        }
        #endregion

        #region [ Properties ]

        public short Year { get; set; }
        public int Number { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string JsonMetadata { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Container Container { get; set; }
        public virtual MetadataRepository MetadataRepository { get; set; }
        public virtual ICollection<Process> Processes { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<DossierLog> DossierLogs { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<DossierRole> DossierRoles { get; set; }
        public virtual ICollection<DossierDocument> DossierDocuments { get; set; }
        public virtual ICollection<DossierComment> DossierComments { get; set; }
        public virtual ICollection<DossierFolder> DossierFolders { get; set; }
        public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; }
        public virtual ICollection<DossierLink> LinkedDossiers { get; set; }
        public virtual ICollection<DossierLink> DossierLinks { get; set; }
        #endregion
    }
}
