using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FasciclePeriod : DSWBaseEntity
    {
        #region [ Constructor ]

        public FasciclePeriod() : this(Guid.NewGuid()) { }

        public FasciclePeriod(Guid uniqueId)
            : base(uniqueId)
        {
            CategoryFascicles = new HashSet<CategoryFascicle>();
        }

        #endregion

        #region [ Properties ]

        public bool IsActive { get; set; }

        public string PeriodName { get; set; }

        public double PeriodDays { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<CategoryFascicle> CategoryFascicles { get; set; }

        #endregion
    }
}
