using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class CategoryFascicle : DSWBaseEntity
    {
        #region [ Constructor ]

        public CategoryFascicle() : this(Guid.NewGuid()) { }

        public CategoryFascicle(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]

        public FascicleType FascicleType { get; set; }

        public int DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Category Category { get; set; }

        public virtual FasciclePeriod FasciclePeriod { get; set; }

        public virtual Contact Manager { get; set; }

        public virtual ICollection<CategoryFascicleRight> CategoryFascicleRights { get; set; }
        #endregion
    }
}
