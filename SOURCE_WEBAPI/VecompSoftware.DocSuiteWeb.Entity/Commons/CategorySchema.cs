using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class CategorySchema : DSWBaseEntity
    {
        #region [ Constructor ]

        public CategorySchema() : this(Guid.NewGuid()) { }
        public CategorySchema(Guid uniqueId)
            : base(uniqueId)
        {
            Categories = new HashSet<Category>();
        }
        #endregion

        #region[ Properties ]

        public short Version { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<Category> Categories { get; set; }
        #endregion
    }
}
