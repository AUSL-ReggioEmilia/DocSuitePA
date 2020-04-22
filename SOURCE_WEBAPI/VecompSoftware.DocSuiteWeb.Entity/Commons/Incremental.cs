using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class Incremental : DSWBaseEntity
    {
        #region [ Constructor ]

        public Incremental() : this(Guid.NewGuid()) { }

        public Incremental(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }

        public int? IncrementalValue { get; set; }
        #endregion

        #region [ Navigation Properties ]
        #endregion
    }
}
