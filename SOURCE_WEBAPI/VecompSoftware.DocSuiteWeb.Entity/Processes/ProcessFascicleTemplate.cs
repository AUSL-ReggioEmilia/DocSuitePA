using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.Processes
{
    public class ProcessFascicleTemplate : DSWBaseEntity
    {
        #region[ Constructor ]

        public ProcessFascicleTemplate(Guid uniqueId) : base(uniqueId)
        {
            Fascicles = new HashSet<Fascicle>();
        }

        public ProcessFascicleTemplate() : this(Guid.NewGuid())
        {
            
        }

        #endregion

        #region[ Properties ]

        public string Name { get; set; }
        public string JsonModel { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        #endregion

        #region[ Navigation Properties ]

        public virtual Process Process { get; set; }
        public virtual DossierFolder DossierFolder { get; set; }
        public virtual ICollection<Fascicle> Fascicles { get; set; }

        #endregion

    }
}
