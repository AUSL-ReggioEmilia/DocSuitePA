using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.Processes
{
    public class Process : DSWBaseEntity
    {
        #region [ Constructor ]

        public Process(Guid uniqueId) : base(uniqueId)
        {
            Roles = new HashSet<Role>();
            FascicleTemplates = new HashSet<ProcessFascicleTemplate>();
            FascicleWorkflowRepositories = new HashSet<ProcessFascicleWorkflowRepository>();
        }

        public Process() : this(Guid.NewGuid())
        {

        }

        #endregion

        #region [ Properties ]

        public string Name { get; set; }
        public FascicleType FascicleType { get; set; }
        public ProcessType ProcessType { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Dossier Dossier { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<ProcessFascicleTemplate> FascicleTemplates { get; set; }
        public virtual ICollection<ProcessFascicleWorkflowRepository> FascicleWorkflowRepositories { get; set; }

        #endregion
    }
}
