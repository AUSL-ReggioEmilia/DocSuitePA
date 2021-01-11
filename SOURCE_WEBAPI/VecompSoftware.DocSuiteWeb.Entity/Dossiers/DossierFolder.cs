using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class DossierFolder : DSWBaseEntity
    {
        #region [ Constructor ]
        public DossierFolder() : this(Guid.NewGuid()) { }

        public DossierFolder(Guid uniqueId)
            : base(uniqueId)
        {
            DossierComments = new HashSet<DossierComment>();
            DossierFolderRoles = new HashSet<DossierFolderRole>();
            DossierLogs = new HashSet<DossierLog>();
            FascicleTemplates = new HashSet<ProcessFascicleTemplate>();
            FascicleWorkflowRepositories = new HashSet<ProcessFascicleWorkflowRepository>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }

        public DossierFolderStatus Status { get; set; }

        public string JsonMetadata { get; set; }

        public string DossierFolderPath { get; set; }

        public short DossierFolderLevel { get; set; }

        /// <summary>
        /// Proprietà fake per passare il parent id in fase di inserimento
        /// </summary>
        public Guid? ParentInsertId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Dossier Dossier { get; set; }

        public virtual Category Category { get; set; }

        public virtual Fascicle Fascicle { get; set; }

        public virtual ICollection<DossierComment> DossierComments { get; set; }

        public virtual ICollection<DossierFolderRole> DossierFolderRoles { get; set; }

        public virtual ICollection<DossierLog> DossierLogs { get; set; }

        public virtual ICollection<ProcessFascicleTemplate> FascicleTemplates { get; set; }

        public virtual ICollection<ProcessFascicleWorkflowRepository> FascicleWorkflowRepositories { get; set; }
        #endregion
    }
}
