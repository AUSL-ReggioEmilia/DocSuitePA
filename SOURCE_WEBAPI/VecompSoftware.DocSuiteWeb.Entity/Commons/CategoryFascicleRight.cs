using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class CategoryFascicleRight : DSWBaseEntity
    {
        #region [ Constructor ]

        public CategoryFascicleRight() : this(Guid.NewGuid()) { }

        public CategoryFascicleRight(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]

        #endregion

        #region [ Navigation Properties ]

        public virtual CategoryFascicle CategoryFascicle { get; set; }

        public virtual Role Role { get; set; }

        public virtual Container Container { get; set; }
        #endregion
    }
}
